USE [SODA]
GO

/****** Object:  StoredProcedure [dbo].[USP_POST_AREAS]    Script Date: 15/04/2026 10:13:13 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[USP_POST_AREAS]
    @DATA NVARCHAR(MAX),
    @IDUSER INT
AS
/* '===============================================================          
  '   NOMBRE                : 
  '   CREADO POR            : SARAHY CHIRINOS
  '   CREADO PARA           : 
  '   FUNCIÓN               : 
  '   VERSIÓN               :
  '   MODIFICADO EN         : 
  '   MODIFICADO POR        : 
  '   RAZÓN DE MODIFICACIÓN : 
  '===============================================================*/

SET XACT_ABORT ON
SET NOCOUNT ON
SET LOCK_TIMEOUT 180000
BEGIN
    BEGIN TRY
  
        DECLARE @ErrorMessage NVARCHAR(4000)
        DECLARE @NAME         VARCHAR(100) = NULL
        DECLARE @VACTION      VARCHAR(20)
        DECLARE @IID          INT
        DECLARE @IINSERTED    INT
        DECLARE @IUPDATED     INT
        DECLARE @IDMODULE     INT = [dbo].[UFN_GET_IDMODULE]('INSPECCION-AREA')
         
        DECLARE @TDATA AS TABLE (
            ID         INT,
            VNAME      VARCHAR(100),
            IDSUPPLIER INT,
            IDDEALER   INT,
            BACTIVE    BIT
        )

        INSERT INTO @TDATA (ID, VNAME, IDSUPPLIER, IDDEALER, BACTIVE)
        SELECT
            Id,
            LTRIM(RTRIM(Name)),
            SupplierId,
            DealerId,
            IsActive
        FROM OPENJSON(@DATA)
        WITH (
            Id         INT,
            Name       VARCHAR(100),
            SupplierId INT,
            DealerId   INT,
            IsActive   BIT
        ) 
        -- NOMBRE VACÍO  
        IF @ErrorMessage IS NULL
            IF EXISTS (SELECT 1 FROM @TDATA WHERE ISNULL(VNAME,'') = '')
                SET @ErrorMessage = 'EL NOMBRE DEL ÁREA ES OBLIGATORIO'

        -- LONGITUD 
        IF @ErrorMessage IS NULL
            IF EXISTS (SELECT 1 FROM @TDATA WHERE LEN(VNAME) > 100)
                SET @ErrorMessage = 'EL NOMBRE DEL ÁREA NO PUEDE SUPERAR LOS 100 CARACTERES'  

        -- DUPLICADO CONTRA LA BD
        IF @ErrorMessage IS NULL
        BEGIN
            SELECT TOP 1 @NAME = D.VNAME
            FROM @TDATA D
            INNER JOIN AREA A WITH (NOLOCK)
                ON  A.VNAME      = D.VNAME
                AND A.IDSUPPLIER = D.IDSUPPLIER
                AND A.IDDEALER   = D.IDDEALER
                AND A.ID        != D.ID
            IF @NAME IS NOT NULL
                SET @ErrorMessage = 'EL ÁREA QUE INTENTAS CREAR YA EXISTE: ' + @NAME
        END
         
        IF @ErrorMessage IS NOT NULL
        BEGIN
            RAISERROR(@ErrorMessage, 16, 1)
            RETURN
        END
         
        BEGIN TRAN
            IF EXISTS (SELECT 1 FROM @TDATA WHERE ID = 0)
            BEGIN
                SET @VACTION = 'CREATE'
                --EXEC USP_CHECK_CREDENTIALS @IDUSER, @VACTION, @IDMODULE

                INSERT INTO AREA (VNAME, IDSUPPLIER, IDDEALER, BACTIVE, DCREATED, DUPDATED, VUPDATEDBY)
                SELECT
                    D.VNAME,
                    D.IDSUPPLIER,
                    D.IDDEALER,
                    D.BACTIVE,
                    GETDATE(),
                    GETDATE(),
                    DBO.UFN_GET_LOGIN(@IDUSER)
                FROM @TDATA D
                WHERE NOT EXISTS (SELECT 1 FROM AREA A WITH (NOLOCK) WHERE A.ID = D.ID)

                SELECT @IINSERTED = @@ROWCOUNT
                SELECT @IID       = SCOPE_IDENTITY()
            END
            ELSE
            BEGIN
                SET @VACTION = 'EDIT'
                --EXEC USP_CHECK_CREDENTIALS @IDUSER, @VACTION, @IDMODULE

                UPDATE A
                SET
                    A.VNAME      = D.VNAME,
                    A.BACTIVE    = D.BACTIVE,
                    A.DUPDATED   = GETDATE(),
                    A.VUPDATEDBY = DBO.UFN_GET_LOGIN(@IDUSER)
                FROM AREA A
                INNER JOIN @TDATA D ON D.ID = A.ID

                SELECT @IUPDATED = @@ROWCOUNT
            END
             
        SELECT
            ISNULL(@IID,       0) AS IID,
            ISNULL(@IINSERTED, 0) AS IINSERTED,
            ISNULL(@IUPDATED,  0) AS IUPDATED

        COMMIT TRAN

    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRAN
        SELECT @ErrorMessage = ERROR_PROCEDURE() + ' : ' + ERROR_MESSAGE()
        RAISERROR(@ErrorMessage, 16, 1)
    END CATCH
END
GO


