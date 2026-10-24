CREATE PROCEDURE [dbo].[USP_POST_STOCK_COUNT_IMPORT]
    @IDUSER         INT,
    @IDSUPPLIER     INT,
    @IDINVENTORY    INT,
    @DATA           NVARCHAR(MAX)
/* '===============================================================
  '   NOMBRE                : USP_POST_STOCK_COUNT_IMPORT
  '   FECHA CREACIÓN        :
  '   CREADO POR            : GERARDO JIMENEZ
  '   CREADO PARA           :
  '   FUNCIÓN               : IMPORTAR CONTEO DE PREINVENTARIO DESDE EXCEL
  '   VERSIÓN               : 
  '   MODIFICADO EN         : 
  '   MODIFICADO POR        : 
  '   RAZÓN DE MODIFICACIÓN :
  '===============================================================*/
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    SET LOCK_TIMEOUT 180000;

    DECLARE @IDESTATUS    INT;
    DECLARE @IDCOUNTTYPE  INT;
    DECLARE @ErrorMessage VARCHAR(500);

    BEGIN TRY

        -- VALIDAR INVENTARIO (DEBE SER PREINVENTARIO ACTIVO)
        SELECT
            @IDESTATUS   = IC.IDESTATUS,
            @IDCOUNTTYPE = IC.IDCOUNTTYPE
        FROM INVENTORYCOUNT IC WITH (NOLOCK)
        WHERE IC.ID         = @IDINVENTORY
          AND IC.IDSUPPLIER = @IDSUPPLIER;

        IF @IDESTATUS IS NULL
        BEGIN
            SET @ErrorMessage = 'INVENTARIO NO ENCONTRADO';
            RAISERROR(@ErrorMessage, 16, 1);
            RETURN;
        END

        IF NOT (@IDESTATUS = 20 AND @IDCOUNTTYPE = 2)
        BEGIN
            SET @ErrorMessage = 'INVENTARIO NO ESTA ACTIVO';
            RAISERROR(@ErrorMessage, 16, 1);
            RETURN;
        END

        -- CREAR TABLA TEMPORAL DE RESULTADOS
        CREATE TABLE #RESULTADOS (
            Fila     INT NULL,
            Estado   VARCHAR(20),
            Mensaje  VARCHAR(500)
        );

        -- ITERAR JSON CON CURSOR
        DECLARE @Fila          INT = 0;
        DECLARE @LocationCode  VARCHAR(50);
        DECLARE @PartCode      VARCHAR(50);
        DECLARE @Quantity      NUMERIC(18,4);

        DECLARE @LOCATIONID    INT;
        DECLARE @ZONEID        INT;
        DECLARE @PARTID        INT;
        DECLARE @IQUANTITY_OLD NUMERIC(18,4);
        DECLARE @DETAILID      INT;
        DECLARE @DETAILSTATUS  INT;
        DECLARE @DETAILUSER    INT;
        DECLARE @DETAILQTYNEW  NUMERIC(18,4);
        
        -- Variables para BVALID
        DECLARE @BVALID        BIT = 0;
        DECLARE @CONTEOS_COINCIDENTES INT = 0;

        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT LocationCode, PartCode, Quantity
            FROM OPENJSON(@DATA)
            WITH (
                LocationCode VARCHAR(50)   '$.LocationCode',
                PartCode     VARCHAR(50)   '$.PartCode',
                Quantity     NUMERIC(18,4) '$.Quantity'
            );

        OPEN cur;
        FETCH NEXT FROM cur INTO @LocationCode, @PartCode, @Quantity;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @Fila = @Fila + 1;

            -- RESET VARIABLES
            SET @LOCATIONID    = NULL;
            SET @ZONEID        = NULL;
            SET @PARTID        = NULL;
            SET @IQUANTITY_OLD = 0;
            SET @DETAILID      = NULL;
            SET @DETAILSTATUS  = NULL;
            SET @DETAILUSER    = NULL;
            SET @DETAILQTYNEW  = NULL;
            SET @BVALID        = 0;
            SET @CONTEOS_COINCIDENTES = 0;

            -- 1. VALIDAR CANTIDAD (Permitimos NULL, pero no negativos)
            IF @Quantity < 0
            BEGIN
                INSERT INTO #RESULTADOS VALUES (@Fila, 'RECHAZADO', 'CANTIDAD INVÁLIDA (NEGATIVA)');
                FETCH NEXT FROM cur INTO @LocationCode, @PartCode, @Quantity;
                CONTINUE;
            END

            -- 2. VALIDAR UBICACIÓN
            SELECT
                @LOCATIONID = L.ID,
                @ZONEID     = L.IDZONE
            FROM LOCATION L WITH (NOLOCK)
            WHERE L.VNAME      = @LocationCode
              AND L.IDSUPPLIER = @IDSUPPLIER
              AND L.VTYPE      = 'A';

            IF @LOCATIONID IS NULL
            BEGIN
                INSERT INTO #RESULTADOS VALUES (@Fila, 'RECHAZADO', 'UBICACIÓN NO ENCONTRADA O NO ES DE ALMACENAJE');
                FETCH NEXT FROM cur INTO @LocationCode, @PartCode, @Quantity;
                CONTINUE;
            END

            -- 3. VALIDAR REPUESTO
            SELECT @PARTID = P.ID
            FROM PART P WITH (NOLOCK)
            WHERE (P.VINNERCODE = @PartCode OR P.VBARCODE = @PartCode)
              AND P.IDSUPPLIER  = @IDSUPPLIER;

            IF @PARTID IS NULL
            BEGIN
                INSERT INTO #RESULTADOS VALUES (@Fila, 'RECHAZADO', 'REPUESTO NO ENCONTRADO');
                FETCH NEXT FROM cur INTO @LocationCode, @PartCode, @Quantity;
                CONTINUE;
            END

            -- 4. OBTENER STOCK ACTUAL
            SELECT @IQUANTITY_OLD = ISNULL(I.ISTOCK, 0)
            FROM INVENTORY I WITH (NOLOCK)
            WHERE I.IDLOCATION = @LOCATIONID
              AND I.IDPART     = @PARTID;

            SET @IQUANTITY_OLD = ISNULL(@IQUANTITY_OLD, 0);

            -- 5. CALCULAR BVALID (Solo si el Excel trae cantidad)
            IF @Quantity IS NOT NULL
            BEGIN
                SELECT @CONTEOS_COINCIDENTES = COUNT(1)
                FROM INVENTORYCOUNTDETAIL ICD WITH (NOLOCK)
                WHERE ICD.IDINVENTORY   = @IDINVENTORY
                  AND ICD.IDLOCATION    = @LOCATIONID
                  AND ICD.IDPART        = @PARTID
                  AND ICD.IQUANTITY_NEW = @Quantity
                  AND ICD.IDCOUNT       = 0;

                IF @CONTEOS_COINCIDENTES > 0
                    SET @BVALID = 1;
                ELSE IF @Quantity = @IQUANTITY_OLD
                    SET @BVALID = 1;
                ELSE
                    SET @BVALID = 0;
            END

            -- 6. BUSCAR EN INVENTORYCOUNTDETAIL
            SELECT TOP 1
                @DETAILID     = ICD.ID,
                @DETAILSTATUS = ICD.IDSTATUS,
                @DETAILUSER   = ICD.IDUSER,
                @DETAILQTYNEW = ICD.IQUANTITY_NEW
            FROM INVENTORYCOUNTDETAIL ICD WITH (NOLOCK)
            WHERE ICD.IDINVENTORY = @IDINVENTORY
              AND ICD.IDCOUNT     = 0
              AND ICD.IDLOCATION  = @LOCATIONID
              AND ICD.IDPART      = @PARTID;

            -- 7. INSERT O UPDATE
            IF @DETAILID IS NOT NULL
            BEGIN
                -- ACTUALIZAR
                UPDATE INVENTORYCOUNTDETAIL
                SET IQUANTITY_OLD = @IQUANTITY_OLD,
                    IQUANTITY_NEW = @Quantity,
                    IDUSER        = CASE WHEN @Quantity IS NOT NULL THEN @IDUSER ELSE NULL END,
                    IDSTATUS      = CASE WHEN @Quantity IS NOT NULL THEN 18 ELSE 2 END,
                    DCOUNTDATE    = CASE WHEN @Quantity IS NOT NULL THEN GETDATE() ELSE NULL END,
                    BBLOCK        = CASE WHEN @Quantity IS NOT NULL THEN 1 ELSE 0 END,
                    VOBSERVATION  = NULL,
                    BVALID        = @BVALID
                WHERE ID = @DETAILID;

                INSERT INTO #RESULTADOS VALUES (@Fila, 'IMPORTADO', 'ACTUALIZADO');
            END
            ELSE
            BEGIN
                -- INSERTAR
                INSERT INTO INVENTORYCOUNTDETAIL
                (
                    IDINVENTORY, IDCOUNT, IDLOCATION, IDPART, IDZONE, IFORM,
                    IQUANTITY_OLD, IQUANTITY_NEW, IDUSER, IDSTATUS,
                    DCREATED, DCOUNTDATE, BBLOCK, VOBSERVATION, BVALID
                )
                VALUES
                (
                    @IDINVENTORY, 0, @LOCATIONID, @PARTID, @ZONEID, NULL,
                    @IQUANTITY_OLD, @Quantity, 
                    CASE WHEN @Quantity IS NOT NULL THEN @IDUSER ELSE NULL END,
                    CASE WHEN @Quantity IS NOT NULL THEN 18 ELSE 2 END,
                    GETDATE(), 
                    CASE WHEN @Quantity IS NOT NULL THEN GETDATE() ELSE NULL END,
                    CASE WHEN @Quantity IS NOT NULL THEN 1 ELSE 0 END,
                    NULL, @BVALID
                );

                INSERT INTO #RESULTADOS VALUES (@Fila, 'IMPORTADO', CASE WHEN @Quantity IS NOT NULL THEN 'INSERTADO' ELSE 'INSERTADO (PENDIENTE)' END);
            END

            FETCH NEXT FROM cur INTO @LocationCode, @PartCode, @Quantity;
        END

        CLOSE cur;
        DEALLOCATE cur;

        -- INSERTAR RESUMEN
        INSERT INTO #RESULTADOS
        SELECT
            NULL,
            'RESUMEN',
            'Total: ' + CAST(COUNT(*) AS VARCHAR(10)) +
            ', Importados: ' + CAST(SUM(CASE WHEN Estado = 'IMPORTADO' THEN 1 ELSE 0 END) AS VARCHAR(10)) +
            ', Rechazados: ' + CAST(SUM(CASE WHEN Estado = 'RECHAZADO' THEN 1 ELSE 0 END) AS VARCHAR(10))
        FROM #RESULTADOS
        WHERE Estado IN ('IMPORTADO', 'RECHAZADO');

        -- DEVOLVER RESULTADOS
        SELECT
            Fila,
            Estado,
            Mensaje
        FROM #RESULTADOS
        ORDER BY CASE WHEN Fila IS NULL THEN 999999 ELSE Fila END;

        DROP TABLE #RESULTADOS;

    END TRY
    BEGIN CATCH
        IF OBJECT_ID('tempdb..#RESULTADOS') IS NOT NULL
            DROP TABLE #RESULTADOS;

        SET @ErrorMessage = ERROR_PROCEDURE() + ' : ' + ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END