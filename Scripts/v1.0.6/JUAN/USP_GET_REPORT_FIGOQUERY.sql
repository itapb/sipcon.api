USE [SODA]
GO
/****** Object:  StoredProcedure [dbo].[USP_GET_REPORTING_TYPE]    Script Date: 17/06/2026 8:35:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_GET_REPORT_FIGOQUERY] -- USP_GET_REPORT_FIGOQUERY 1,1,0
@IDUSER INT=NULL,
@IDREPORT INT,
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
     WITH T1 AS (
	SELECT  ID,VCONTENT FROM REPORTFIGO  WITH (NOLOCK)
	WHERE BACTIVE=1 AND ID=@IDREPORT
	),
    T2 AS (SELECT COUNT(*) AS TOTAL FROM T1)
    SELECT T1.*, T2.TOTAL
    FROM T1, T2

END
