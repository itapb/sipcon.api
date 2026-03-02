# --- CONFIGURACION ---
$folderMigra = "C:\Proyectos\bd\DIFF"
$archivoMaestro = "C:\Proyectos\bd\DIFF\script.sql"

# Orden de ejecucion
$ordenSufijos = @(".Table.sql", ".UserDefinedFunction.sql", ".View.sql", ".StoredProcedure.sql")

# Inicializar array de lineas
$scriptSQL = New-Object System.Collections.Generic.List[string]

$scriptSQL.Add("-- ======================================================")
$scriptSQL.Add("-- SCRIPT MAESTRO DE MIGRACION")
$scriptSQL.Add("-- RECUERDA ACTIVAR EL SQLCMD MODE EN SSMS (Query -> SQLCMD Mode)")
$scriptSQL.Add("-- ======================================================")
$scriptSQL.Add("USE [ZEPPELIN];")
$scriptSQL.Add("GO")

foreach ($sufijo in $ordenSufijos) {
    # Buscar archivos
    $archivos = Get-ChildItem -Path $folderMigra -Filter "*$sufijo" | Sort-Object Name
    
    if ($archivos.Count -gt 0) {
        $scriptSQL.Add("`r`n-- >>> CATEGORIA: $sufijo")
        foreach ($file in $archivos) {
            $scriptSQL.Add("PRINT 'Ejecutando: $($file.Name)...';")
            # El comando :r requiere la ruta completa entre comillas
            $scriptSQL.Add(":r `"$($file.FullName)`"")
            $scriptSQL.Add("GO")
        }
    }
}

# Guardar el archivo usando codificacion UTF8 simple (sin BOM) para evitar problemas en SQL
[System.IO.File]::WriteAllLines($archivoMaestro, $scriptSQL)

Write-Host "Script maestro generado exitosamente en:" -ForegroundColor Cyan
Write-Host $archivoMaestro -ForegroundColor White
