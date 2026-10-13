
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
            SET @ErrorMessage = 'INVENTARIO NO ES PREINVENTARIO ACTIVO';
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

            -- 1. VALIDAR CANTIDAD
            IF @Quantity IS NULL OR @Quantity < 0
            BEGIN
                INSERT INTO #RESULTADOS VALUES (@Fila, 'RECHAZADO', 'CANTIDAD INVÁLIDA');
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

            -- 5. BUSCAR EN INVENTORYCOUNTDETAIL
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

            -- 6. INSERT O UPDATE
            IF @DETAILID IS NOT NULL
            BEGIN
                -- ACTUALIZAR
                IF @DETAILSTATUS = 2 AND @DETAILUSER IS NULL AND @DETAILQTYNEW IS NULL
                BEGIN
                    UPDATE INVENTORYCOUNTDETAIL
                    SET IQUANTITY_OLD = @IQUANTITY_OLD,
                        IQUANTITY_NEW = @Quantity,
                        IDUSER        = @IDUSER,
                        IDSTATUS      = 18,
                        DCOUNTDATE    = GETDATE(),
                        BBLOCK        = 1
                    WHERE ID = @DETAILID;

                    INSERT INTO #RESULTADOS VALUES (@Fila, 'IMPORTADO', 'ACTUALIZADO');
                END
                ELSE
                BEGIN
                    UPDATE INVENTORYCOUNTDETAIL
                    SET IQUANTITY_NEW = @Quantity,
                        DCOUNTDATE    = GETDATE()
                    WHERE ID = @DETAILID;

                    INSERT INTO #RESULTADOS VALUES (@Fila, 'IMPORTADO', 'ACTUALIZADO (CORRECCIÓN)');
                END
            END
            ELSE
            BEGIN
                -- INSERTAR
                INSERT INTO INVENTORYCOUNTDETAIL
                (
                    IDINVENTORY, IDCOUNT, IDLOCATION, IDPART, IDZONE, IFORM,
                    IQUANTITY_OLD, IQUANTITY_NEW, IDUSER, IDSTATUS,
                    DCREATED, DCOUNTDATE, BBLOCK
                )
                VALUES
                (
                    @IDINVENTORY, 0, @LOCATIONID, @PARTID, @ZONEID, NULL,
                    @IQUANTITY_OLD, @Quantity, @IDUSER, 18,
                    GETDATE(), GETDATE(), 1
                );

                INSERT INTO #RESULTADOS VALUES (@Fila, 'IMPORTADO', 'INSERTADO');
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
