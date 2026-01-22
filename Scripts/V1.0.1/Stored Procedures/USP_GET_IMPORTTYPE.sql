GO
/****** Object:  StoredProcedure [dbo].[USP_GET_IMPORTTYPE]    Script Date: 18/08/2025 8:46:26 ******/
SET ANSI_NULLS ON

GO
SET QUOTED_IDENTIFIER ON
GO
IF  NOT EXISTS (SELECT * FROM SYS.OBJECTS

                WHERE OBJECT_ID = OBJECT_ID(N'[DBO].[USP_GET_IMPORTTYPE]')

                AND TYPE IN (N'P', N'PC', N'TF', N'FN'))
EXEC('CREATE PROCEDURE [DBO].[USP_GET_IMPORTTYPE] AS BEGIN SET NOCOUNT ON  END')
GO


ALTER PROCEDURE [dbo].[USP_GET_IMPORTTYPE] -- EXEC USP_GET_IMPORTTYPE 4076,NULL
@IDSUPPLIER INT = 0,
@IROWFROM INT = NULL
AS
/* '===============================================================          
  '   NOMBRE                : 
  '   FECHA CREACIÓN        : 
  '   CREADO POR            : JUAN GUARECUCO
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

	DECLARE @TOPE INT

	IF @IROWFROM IS NULL
	BEGIN
		SET @TOPE=1000000
		SET @IROWFROM=0
	END
	ELSE
	BEGIN
		SET @TOPE=100
	END

BEGIN

		WITH T1 as(
		
		
		 SELECT 
		 I.ID,
		 CASE WHEN @IDSUPPLIER=4076 AND I.ID=2 THEN 'APROBACION-FINAL'
		 ELSE I.VNAME
		 END AS VNAME
		 FROM IMPORTTYPE I
		 WHERE (@IDSUPPLIER=4076 AND I.ID IN (1,2)) OR (@IDSUPPLIER=4069 AND  I.ID IN (1,2,3) ) 

		
		),
		T2 AS (SELECT COUNT(*) AS TOTAL FROM T1),
		T3 AS (
			SELECT * FROM T1
			ORDER BY ID ASC
			OFFSET @IROWFROM ROWS  --Indica la posicion del registro Inicial...(el resultado de ..(1))
			FETCH NEXT @TOPE ROWS ONLY
		)
		SELECT T3.*, T2.* FROM T3, T2;
	
END