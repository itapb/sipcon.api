GO
/** Object:  StoredProcedure [dbo].[USP_POST_ADJUSTMENT]    Script Date: 18/08/2025 8:46:26 **/
SET ANSI_NULLS ON

GO
SET QUOTED_IDENTIFIER ON
GO
IF  NOT EXISTS (SELECT * FROM SYS.OBJECTS

                WHERE OBJECT_ID = OBJECT_ID(N'[DBO].[USP_POST_ADJUSTMENT]')

                AND TYPE IN (N'P', N'PC', N'TF', N'FN'))
EXEC('CREATE PROCEDURE [DBO].[USP_POST_ADJUSTMENT] AS BEGIN SET NOCOUNT ON  END')
GO


ALTER PROCEDURE [dbo].[USP_POST_ADJUSTMENT]
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
		BEGIN TRAN;

        -- tabla temporal para datos de cabecera
        DECLARE @TDATA AS TABLE
        (
            ID INT,
            IDUSER INT,
            IDSTATUS INT,
            IDSUPPLIER INT,
			VCOMMENT varchar(100)
        ); 

        -- parsear datos de cabecera
        INSERT INTO @TDATA
		SELECT 
            L.Id, 
            L.UserId,
            L.StatusId,  
            L.SupplierId,
			L.Comment
        FROM OPENJSON(@DATA)
		WITH (
            Id int, 
            UserId int,
            StatusId int,
            SupplierId int,
			Comment varchar(100)
        ) AS L;
 
 
		EXEC USP_CHECK_SUPPLIERS @DATA, @IDUSER;
        -- validar credenciales primero
        /*DECLARE @IDMODULE INT = [dbo].[UFN_GET_IDMODULE]('INVENTARIO-PROCESOS-AJUSTE');
        DECLARE @VACTION VARCHAR(20) = 'CREATE';
        EXEC USP_CHECK_CREDENTIALS @IDUSER, @VACTION, @IDMODULE; 
        */

        -- variables de retorno
        DECLARE @IID INT;
        DECLARE @IINSERTED INT;
        DECLARE @IUPDATED INT; 
	
        -- insertar nuevos ajustes
       IF EXISTS (SELECT * FROM @TDATA WHERE ID=0)
		BEGIN

		INSERT INTO ADJUSTMENT(
            IDUSER,
            IDSTATUS,
            IDSUPPLIER,
            DCREATED,
			VCOMMENT,
			DUPDATED
        )
        SELECT 
            IDUSER,
            IDSTATUS, 
            IDSUPPLIER,
            GETDATE(),
			VCOMMENT,
			GETDATE()
        FROM @TDATA D

		SELECT @IINSERTED=@@ROWCOUNT
		SELECT @IID=SCOPE_IDENTITY() 

        -- actualizar ajustes existentes
        END

		UPDATE A
		SET
		A.IDUSER = D.IDUSER,
		A.IDSTATUS= D.IDSTATUS,
		A.IDSUPPLIER= D.IDSUPPLIER,
		A.VCOMMENT= D.VCOMMENT,
		A.DUPDATED= GETDATE()
		FROM ADJUSTMENT A 
		INNER JOIN @TDATA D ON D.ID=A.ID;

        SELECT
            ISNULL(@IID, 0) AS IID, 
            ISNULL(@IINSERTED, 0) AS IINSERTED,
            ISNULL(@IUPDATED, 0) AS IUPDATED;

 		COMMIT TRAN;
	END TRY
	BEGIN CATCH
		IF XACT_STATE() <> 0
			ROLLBACK TRAN;
		
		DECLARE @ErrorMessage NVARCHAR(4000);
		SELECT @ErrorMessage = ERROR_PROCEDURE() + ' : ' + ERROR_MESSAGE();
		RAISERROR (@ErrorMessage, 16, 1);
	END CATCH
END
GO


