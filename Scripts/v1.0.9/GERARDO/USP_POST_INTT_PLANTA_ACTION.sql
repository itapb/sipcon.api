USE [OASIS]
GO
/****** Objeto: StoredProcedure [dbo].[USP_POST_INTT_PLANTA_ACTION] Fecha de script: 17/8/2026 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[USP_POST_INTT_PLANTA_ACTION]
    @DATA VARCHAR(MAX),
    @IDUSER INT,
    @IDSUPPLIER INT

/* '===============================================================          
  '   NOMBRE                : USP_POST_INTT_PLANTA_ACTION
  '   FECHA CREACIÓN        : 
  '   CREADO POR            : GERARDO JIMENEZ
  '   CREADO PARA           : 
  '   FUNCIÓN               : 
  '   VERSIÓN               : 
  '   MODIFICADO EN         : 
  '   MODIFICADO POR        : 
  '   RAZÓN DE MODIFICACIÓN : 
  '===============================================================*/
AS
SET XACT_ABORT ON               
SET NOCOUNT ON
SET LOCK_TIMEOUT 180000
BEGIN
    BEGIN TRY
        BEGIN TRAN 

        DECLARE @TDATA AS TABLE
        (
            IDRECORD INT,
            IDMODULE INT,
            VACTION VARCHAR(20),
            VCOMMENT VARCHAR(200),
            IDRELATED INT
        );

        INSERT INTO @TDATA
        SELECT RecordId, ModuleId, ActionName, ActionComment, RelatedId
        FROM OPENJSON(@DATA)
        WITH (
            RecordId int, 
            ModuleId int, 
            ActionName varchar(20),
            ActionComment varchar(200),
            RelatedId int
        ) AS C

        DECLARE @VACTION VARCHAR(20)
        DECLARE @IDMODULE INT
        DECLARE @IID INT
        DECLARE @IUPDATED INT
        DECLARE @CORRELATIVE INT
        DECLARE @VCONTROLNUMBER VARCHAR(20)
        DECLARE @RECORDS_COUNT INT

        SELECT TOP 1 @VACTION = VACTION, @IDMODULE = IDMODULE FROM @TDATA

        EXEC USP_CHECK_CREDENTIALS @IDUSER, @VACTION, @IDMODULE 

        IF @VACTION = 'GENERATE'
        BEGIN 
            -- Contar registros seleccionados
            SELECT @RECORDS_COUNT = COUNT(*) FROM @TDATA

            -- Generar correlativo para PLANTA
            EXEC USP_ACT_SECUENCE 'INTT_PLANTA', @CORRELATIVE OUT 
            SET @VCONTROLNUMBER = RIGHT('0000000000' + CAST(@CORRELATIVE AS VARCHAR(10)), 10)

            -- Actualizar VEHICLE con el número de TXT de PLANTA
            UPDATE VEH
            SET VEH.VNUMBERPLANTATXT = @VCONTROLNUMBER
            FROM VEHICLE VEH
            INNER JOIN @TDATA T ON VEH.ID = T.IDRECORD
            WHERE VEH.IDSUPPLIER = @IDSUPPLIER 
              AND (VEH.VNUMBERPLANTATXT IS NULL OR VEH.VNUMBERPLANTATXT = '' OR VEH.VNUMBERPLANTATXT = '0')

            SELECT @IUPDATED = @@ROWCOUNT
            SET @IID = @CORRELATIVE
        END 
         
        SELECT
            ISNULL(@IID, 0) AS IID, 
            ISNULL(@IUPDATED, 0) AS IUPDATED

        IF @IUPDATED > 0
        BEGIN
            INSERT INTO AUDIT (IDUSER, IDRECORD, IDMODULE, VACTION, VCOMMENT, IDRELATED, DCREATED)
            SELECT @IDUSER, IDRECORD, IDMODULE, @VACTION, VCOMMENT, IDRELATED, GETDATE()
            FROM @TDATA
        END

        COMMIT TRAN
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRAN
        DECLARE @ErrorMessage NVARCHAR(4000) 
        SELECT @ErrorMessage = ERROR_PROCEDURE() + ' : ' + ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1) 
    END CATCH
END
GO