# --- CONFIGURACIÓN ---
$folderOrigen  = "C:\Proyectos\bd\OASIS"
$folderDestino = "C:\Proyectos\bd\ZEPPELIN"
$folderMigra   = "C:\Proyectos\bd\DIFF"

if (!(Test-Path $folderMigra)) { New-Item -ItemType Directory -Path $folderMigra -Force }

$filesOrigen = Get-ChildItem -Path $folderOrigen -Filter "*.sql"

foreach ($file in $filesOrigen) {
    $pathDestino = Join-Path $folderDestino $file.Name
    $pathMigra   = Join-Path $folderMigra $file.Name
    
    $partes = $file.BaseName.Split('.')
    $objetoNombre = $partes[1]
    $objetoTipo   = $partes[2]

    if (!(Test-Path $pathDestino)) {
        Write-Host "[NUEVO] $($file.Name)" -ForegroundColor Green
        Copy-Item -Path $file.FullName -Destination $pathMigra
    } 
    else {
        $contOrigen  = Get-Content $file.FullName
        $contDestino = Get-Content $pathDestino

        if ($objetoTipo -eq "Table") {
            # Extraer líneas de columnas
            $extract = {
                param($lines)
                $start = -1; $end = -1
                for ($i=0; $i -lt $lines.Count; $i++) {
                    if ($lines[$i] -like "*CREATE TABLE*") { $start = $i }
                    if ($lines[$i] -like "*PRIMARY*") { $end = $i; break }
                }
                if ($start -ne -1 -and $end -gt $start) { return $lines[($start + 1)..($end - 1)] | Where-Object { $_.Trim() -ne "" } }
                return @()
            }

            $colsOrigen  = &$extract $contOrigen
            $colsDestino = &$extract $contDestino

            $scriptsTabla = New-Object System.Collections.Generic.List[string]

            foreach ($lineOrig in $colsOrigen) {
                # Extraer nombre de columna entre corchetes: [COLUMNA]
                if ($lineOrig -match "\[(?<col>.*?)\]\s+(?<rest>.*)") {
                    $nombreCol = $Matches['col']
                    $definicionCol = $lineOrig.Trim().TrimEnd(',')
                    
                    # Buscar si la columna existe en el destino
                    $lineaDestino = $colsDestino | Where-Object { $_ -match "\[$nombreCol\]" }

                    if ($null -eq $lineaDestino) {
                        # CASO: COLUMNA NUEVA (ADD) - Versión Compacta
            		Write-Host "[ADD COMPACTO] Columna $nombreCol en $objetoNombre" -ForegroundColor Green
            
            		if ($definicionCol -match "\[int\]|\[bit\]") {
                		# Generar una sola línea con ADD + CONSTRAINT + DEFAULT
                		$lineaCompacta = "ALTER TABLE [dbo].[$objetoNombre] ADD $definicionCol CONSTRAINT [DF_$( $objetoNombre )_$( $nombreCol )] DEFAULT ((0));"
                		$scriptsTabla.Add($lineaCompacta)
            		} 
            		else {
                		# Si no es int/bit, se añade normal sin el default de (0)
                		$scriptsTabla.Add("ALTER TABLE [dbo].[$objetoNombre] ADD $definicionCol;")
            			}
                    } 
                    else {
                        # CASO: COLUMNA EXISTE PERO ES DIFERENTE (ALTER)
                        if ($lineOrig.Trim() -ne $lineaDestino.Trim()) {
                            Write-Host "[ALTER] Columna $nombreCol en $objetoNombre" -ForegroundColor Yellow
                            $scriptsTabla.Add("ALTER TABLE [dbo].[$objetoNombre] ALTER COLUMN $definicionCol;")
                        }
                    }
                }
            }

            if ($scriptsTabla.Count -gt 0) {
                $scriptsTabla | Out-File $pathMigra -Encoding UTF8
            }
        } 
        else {
            # VISTAS, PROCS, FUNCIONES
            if (Compare-Object $contOrigen $contDestino) {
                Write-Host "[MODIFICADO] ${objetoTipo}: ${objetoNombre}" -ForegroundColor Yellow
                $coletilla = switch ($objetoTipo) {
                    "StoredProcedure"     { "DROP PROCEDURE IF EXISTS [dbo].[$objetoNombre]" }
                    "View"                { "DROP VIEW IF EXISTS [dbo].[$objetoNombre]" }
                    "UserDefinedFunction" { "DROP FUNCTION IF EXISTS [dbo].[$objetoNombre]" }
                }
                $nuevoContenido = "$coletilla`r`nGO`r`n`r`n" + ($contOrigen -join "`r`n")
                $nuevoContenido | Out-File $pathMigra -Encoding UTF8
            }
        }
    }
}

Write-Host "--- Migración generada exitosamente ---" -ForegroundColor Cyan
