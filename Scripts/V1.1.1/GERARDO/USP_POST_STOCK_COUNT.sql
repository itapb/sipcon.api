
CREATE PROCEDURE [dbo].[USP_POST_STOCK_COUNT]
    @IDUSER     INT,
    @IDSUPPLIER INT,
    @DATA       NVARCHAR(MAX)
/* '===============================================================
  '   NOMBRE                : USP_POST_STOCK_COUNT
  '   FECHA CREACIÓN        :
  '   CREADO POR            : GERARDO JIMENEZ
  '   CREADO PARA           :
  '   FUNCIÓN               : INSERTAR O ACTUALIZAR EL CONTEO DE INVENTARIO
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

    DECLARE @IDDETAIL       INT;
    DECLARE @IDINVENTORY    INT;
    DECLARE @LOCATIONCODE   VARCHAR(50);
    DECLARE @PARTCODE       VARCHAR(50);
    DECLARE @IQUANTITY_NEW  NUMERIC(18,4);

    DECLARE @IDESTATUS      INT;
    DECLARE @IDCOUNTTYPE    INT;
    DECLARE @LOCATIONID     INT;
    DECLARE @PARTID         INT;
    DECLARE @ZONEID         INT;
    DECLARE @VTYPE          VARCHAR(50);
    DECLARE @IFORM          VARCHAR(50);
    DECLARE @IQUANTITY_OLD  NUMERIC(18,4);
    DECLARE @DETAILID       INT;
    DECLARE @DETAILSTATUS   INT;
    DECLARE @DETAILUSER     INT;
    DECLARE @DETAILQTYNEW   NUMERIC(18,4);
    DECLARE @IINSERTED      INT = 0;
    DECLARE @IUPDATED       INT = 0;
    DECLARE @IID            INT = 0;
    DECLARE @ErrorMessage   VARCHAR(500);

    BEGIN TRY

        -- PARSEAR JSON
        SELECT TOP 1
            @IDDETAIL      = Id,
            @IDINVENTORY   = IdInventory,
            @LOCATIONCODE  = LocationCode,
            @PARTCODE      = PartCode,
            @IQUANTITY_NEW = Quantity
        FROM OPENJSON(@DATA)
        WITH (
            Id           INT           '$.Id',
            IdInventory  INT           '$.IdInventory',
            LocationCode VARCHAR(50)   '$.LocationCode',
            PartCode     VARCHAR(50)   '$.PartCode',
            Quantity     NUMERIC(18,4) '$.Quantity'
        );

        -- VALIDAR CANTIDAD
        IF @IQUANTITY_NEW IS NULL OR @IQUANTITY_NEW < 0
        BEGIN
            SET @ErrorMessage = 'LA CANTIDAD NO PUEDE SER VACIA O NEGATIVA';
            RAISERROR(@ErrorMessage, 16, 1);
            RETURN;
        END

        -- OBTENER ETAPA DEL INVENTARIO
        SELECT
            @IDESTATUS   = IC.IDESTATUS,
            @IDCOUNTTYPE = IC.IDCOUNTTYPE
        FROM INVENTORYCOUNT IC WITH (NOLOCK)
        WHERE IC.ID         = @IDINVENTORY
          AND IC.IDSUPPLIER = @IDSUPPLIER
          AND IC.IDESTATUS  IN (20, 6);

        IF @IDESTATUS IS NULL
        BEGIN
            SET @ErrorMessage = 'INVENTARIO NO ENCONTRADO O NO ESTÁ ACTIVO';
            RAISERROR(@ErrorMessage, 16, 1);
            RETURN;
        END

        -- OBTENER UBICACIÓN
        SELECT
            @LOCATIONID = L.ID,
            @ZONEID     = L.IDZONE,
            @VTYPE      = L.VTYPE
        FROM LOCATION L WITH (NOLOCK)
        WHERE L.VNAME      = @LOCATIONCODE
          AND L.IDSUPPLIER = @IDSUPPLIER;

        IF @LOCATIONID IS NULL
        BEGIN
            SET @ErrorMessage = 'UBICACIÓN NO ENCONTRADA';
            RAISERROR(@ErrorMessage, 16, 1);
            RETURN;
        END

        IF @ZONEID IS NULL
        BEGIN
            SET @ErrorMessage = 'LA UBICACIÓN NO TIENE ZONA ASIGNADA';
            RAISERROR(@ErrorMessage, 16, 1);
            RETURN;
        END

        IF @VTYPE <> 'A'
        BEGIN 
            SET @ErrorMessage = 'UBICACION NO ES DE TIPO ALMACENAJE';
            RAISERROR (@ErrorMessage, 16, 1);
            RETURN;
        END
        -- OBTENER REPUESTO
        SELECT @PARTID = P.ID
        FROM PART P WITH (NOLOCK)
        WHERE (P.VINNERCODE = @PARTCODE OR P.VBARCODE = @PARTCODE)
          AND P.IDSUPPLIER  = @IDSUPPLIER;

        IF @PARTID IS NULL
        BEGIN
            SET @ErrorMessage = 'REPUESTO NO ENCONTRADO';
            RAISERROR(@ErrorMessage, 16, 1);
            RETURN;
        END

        -- VALIDAR ASIGNACIÓN SEGÚN ETAPA
        IF @IDESTATUS = 20 AND @IDCOUNTTYPE = 2
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                FROM COUNTBYUSER CBU WITH (NOLOCK)
                WHERE CBU.IDUSER           = @IDUSER
                  AND CBU.IDINVENTORYCOUNT = @IDINVENTORY
                  AND CBU.IDSUPPLIER       = @IDSUPPLIER
                  AND CBU.IDCOUNT          = 0
                  AND CBU.VASSIGNTYPE      = 'Z'
                  AND CBU.IDASSIGN         = @ZONEID
            )
            BEGIN
                SET @ErrorMessage = 'USUARIO NO TIENE ACCESO A ESTA ZONA';
                RAISERROR(@ErrorMessage, 16, 1);
                RETURN;
            END
        END
        -- OBTENER STOCK ACTUAL
        SELECT @IQUANTITY_OLD = ISNULL(I.ISTOCK, 0)
        FROM INVENTORY I WITH (NOLOCK)
        WHERE I.IDLOCATION = @LOCATIONID
          AND I.IDPART     = @PARTID;

        SET @IQUANTITY_OLD = ISNULL(@IQUANTITY_OLD, 0);

        -- PREINVENTARIO 
        IF @IDESTATUS = 20 AND @IDCOUNTTYPE = 2
        BEGIN
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

            IF @DETAILID IS NOT NULL
            BEGIN
                BEGIN TRANSACTION;

                -- RESETEADO: UPDATE COMPLETO
                IF @DETAILSTATUS = 2 AND @DETAILUSER IS NULL AND @DETAILQTYNEW IS NULL
                BEGIN
                    UPDATE INVENTORYCOUNTDETAIL
                    SET IQUANTITY_OLD = @IQUANTITY_OLD,
                        IQUANTITY_NEW = @IQUANTITY_NEW,
                        IDUSER        = @IDUSER,
                        IDSTATUS      = 18,
                        DCOUNTDATE    = GETDATE(),
                        BBLOCK        = 1
                    WHERE ID = @DETAILID;
                END
                -- YA CONTADO:(solo cantidad y fecha)
                ELSE
                BEGIN
                    UPDATE INVENTORYCOUNTDETAIL
                    SET IQUANTITY_NEW = @IQUANTITY_NEW,
                        DCOUNTDATE    = GETDATE()
                    WHERE ID = @DETAILID;
                END

                SET @IUPDATED = @@ROWCOUNT;
                SET @IID      = @DETAILID;

                COMMIT TRANSACTION;
            END
            ELSE
            BEGIN
                BEGIN TRANSACTION;

                INSERT INTO INVENTORYCOUNTDETAIL
                (
                    IDINVENTORY, IDCOUNT, IDLOCATION, IDPART, IDZONE, IFORM,
                    IQUANTITY_OLD, IQUANTITY_NEW, IDUSER, IDSTATUS,
                    DCREATED, DCOUNTDATE, BBLOCK
                )
                VALUES
                (
                    @IDINVENTORY, 0, @LOCATIONID, @PARTID, @ZONEID, NULL,
                    @IQUANTITY_OLD, @IQUANTITY_NEW, @IDUSER, 18,
                    GETDATE(), GETDATE(), 1
                );

                SET @IINSERTED = @@ROWCOUNT;
                SET @IID       = SCOPE_IDENTITY();

                COMMIT TRANSACTION;
            END
        END

        -- FÍSICO FINAL(INVENTARIO)
        IF @IDESTATUS = 6 AND @IDCOUNTTYPE = 2
        BEGIN
            -- CONTEO NORMAL (con Id)
            IF @IDDETAIL IS NOT NULL AND @IDDETAIL <> 0
            BEGIN
                SELECT TOP 1
                    @DETAILID     = ICD.ID,
                    @DETAILSTATUS = ICD.IDSTATUS
                FROM INVENTORYCOUNTDETAIL ICD WITH (NOLOCK)
                WHERE ICD.ID        = @IDDETAIL
                  AND ICD.IDINVENTORY = @IDINVENTORY
                  AND ICD.IDCOUNT     >= 1;

                IF @DETAILID IS NULL
                BEGIN
                    SET @ErrorMessage = 'DETALLE NO ENCONTRADO';
                    RAISERROR(@ErrorMessage, 16, 1);
                    RETURN;
                END

                BEGIN TRANSACTION;

                IF @DETAILSTATUS = 2
                BEGIN
                    UPDATE INVENTORYCOUNTDETAIL
                    SET IQUANTITY_OLD = @IQUANTITY_OLD,
                        IQUANTITY_NEW = @IQUANTITY_NEW,
                        IDUSER        = @IDUSER,
                        IDSTATUS      = 18,
                        DCOUNTDATE    = GETDATE(),
                        BBLOCK        = 1
                    WHERE ID = @DETAILID;
                END
                ELSE
                BEGIN
                    UPDATE INVENTORYCOUNTDETAIL
                    SET IQUANTITY_NEW = @IQUANTITY_NEW,
                        DCOUNTDATE    = GETDATE()
                    WHERE ID = @DETAILID;
                END

                SET @IUPDATED = @@ROWCOUNT;
                SET @IID      = @DETAILID;

                COMMIT TRANSACTION;
            END
            -- BOTÓN (+) (Id 0)
            ELSE
            BEGIN
                -- OBTENER IFORM DEL REGISTRO CABECERA DE ZONA
                SELECT TOP 1 @IFORM = ICD.IFORM
                FROM INVENTORYCOUNTDETAIL ICD WITH (NOLOCK)
                WHERE ICD.IDINVENTORY = @IDINVENTORY
                  AND ICD.IDCOUNT     >= 1
                  AND ICD.IDZONE      = @ZONEID
                  AND ICD.IDLOCATION  IS NULL
                  AND ICD.IDPART      IS NULL;

                IF @IFORM IS NULL
                BEGIN
                    SET @ErrorMessage = 'NO HAY DOCUMENTO DE INVENTARIO PARA ESTA ZONA';
                    RAISERROR(@ErrorMessage, 16, 1);
                    RETURN;
                END

                -- BUSCAR SOBRANTE EXISTENTE
                SELECT TOP 1
                    @DETAILID     = ICD.ID,
                    @DETAILSTATUS = ICD.IDSTATUS
                FROM INVENTORYCOUNTDETAIL ICD WITH (NOLOCK)
                WHERE ICD.IDINVENTORY = @IDINVENTORY
                  AND ICD.IDCOUNT     >= 1
                  AND ICD.IDZONE      = @ZONEID
                  AND ICD.IDLOCATION  = @LOCATIONID
                  AND ICD.IDPART      = @PARTID;

                IF @DETAILID IS NOT NULL
                BEGIN
                    BEGIN TRANSACTION;

                    IF @DETAILSTATUS = 2
                    BEGIN
                        UPDATE INVENTORYCOUNTDETAIL
                        SET IQUANTITY_OLD = @IQUANTITY_OLD,
                            IQUANTITY_NEW = @IQUANTITY_NEW,
                            IDUSER        = @IDUSER,
                            IDSTATUS      = 18,
                            DCOUNTDATE    = GETDATE(),
                            BBLOCK        = 1
                        WHERE ID = @DETAILID;
                    END
                    ELSE
                    BEGIN
                        UPDATE INVENTORYCOUNTDETAIL
                        SET IQUANTITY_NEW = @IQUANTITY_NEW,
                            DCOUNTDATE    = GETDATE()
                        WHERE ID = @DETAILID;
                    END

                    SET @IUPDATED = @@ROWCOUNT;
                    SET @IID      = @DETAILID;

                    COMMIT TRANSACTION;
                END
                ELSE
                BEGIN
                    BEGIN TRANSACTION;

                    INSERT INTO INVENTORYCOUNTDETAIL
                    (
                        IDINVENTORY, IDCOUNT, IDLOCATION, IDPART, IDZONE, IFORM,
                        IQUANTITY_OLD, IQUANTITY_NEW, IDUSER, IDSTATUS,
                        DCREATED, DCOUNTDATE, BBLOCK
                    )
                    VALUES
                    (
                        @IDINVENTORY, 1, @LOCATIONID, @PARTID, @ZONEID, @IFORM,
                        @IQUANTITY_OLD, @IQUANTITY_NEW, @IDUSER, 18,
                        GETDATE(), GETDATE(), 1
                    );

                    SET @IINSERTED = @@ROWCOUNT;
                    SET @IID       = SCOPE_IDENTITY();

                    COMMIT TRANSACTION;
                END
            END
        END

        -- CÍCLICO
        IF @IDESTATUS = 6 AND @IDCOUNTTYPE = 1
        BEGIN
            SELECT TOP 1
                @DETAILID     = ICD.ID,
                @DETAILSTATUS = ICD.IDSTATUS
            FROM INVENTORYCOUNTDETAIL ICD WITH (NOLOCK)
            WHERE ICD.ID        = @IDDETAIL
              AND ICD.IDINVENTORY = @IDINVENTORY
              AND ICD.IDCOUNT     >= 1;

            IF @DETAILID IS NULL
            BEGIN
                SET @ErrorMessage = 'DETALLE NO ENCONTRADO';
                RAISERROR(@ErrorMessage, 16, 1);
                RETURN;
            END

            BEGIN TRANSACTION;

            IF @DETAILSTATUS = 2
            BEGIN
                UPDATE INVENTORYCOUNTDETAIL
                SET IQUANTITY_OLD = @IQUANTITY_OLD,
                    IQUANTITY_NEW = @IQUANTITY_NEW,
                    IDUSER        = @IDUSER,
                    IDSTATUS      = 18,
                    DCOUNTDATE    = GETDATE(),
                    BBLOCK        = 1
                WHERE ID = @DETAILID;
            END
            ELSE
            BEGIN
                UPDATE INVENTORYCOUNTDETAIL
                SET IQUANTITY_NEW = @IQUANTITY_NEW,
                    DCOUNTDATE    = GETDATE()
                WHERE ID = @DETAILID;
            END

            SET @IUPDATED = @@ROWCOUNT;
            SET @IID      = @DETAILID;

            COMMIT TRANSACTION;
        END

        
        SELECT
            ISNULL(@IID, 0)       AS IID,
            ISNULL(@IINSERTED, 0) AS IINSERTED,
            ISNULL(@IUPDATED, 0)  AS IUPDATED;

    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        SET @ErrorMessage = ERROR_PROCEDURE() + ' : ' + ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END