GO
/****** Object:  StoredProcedure [dbo].[USP_GET_ITEMS]    Script Date: 18/08/2025 8:46:26 ******/
SET ANSI_NULLS ON

GO
SET QUOTED_IDENTIFIER ON
GO
IF  NOT EXISTS (SELECT * FROM SYS.OBJECTS

                WHERE OBJECT_ID = OBJECT_ID(N'[DBO].[USP_GET_ITEMS]')

                AND TYPE IN (N'P', N'PC', N'TF', N'FN'))
EXEC('CREATE PROCEDURE [DBO].[USP_GET_ITEMS] AS BEGIN SET NOCOUNT ON  END')
GO
ALTER PROCEDURE [dbo].[USP_GET_ITEMS] -- USP_GET_ITEMS 1,4069,L,NULL,NULL
@IDUSER INT=NULL,
@IDSUPPLIER INT ,
@VTYPE VARCHAR(1),
@VFILTER VARCHAR(100) = NULL,
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
	SELECT 
		L.*
		FROM ITEM L WITH (NOLOCK)
		WHERE  L.IDSUPPLIER = @IDSUPPLIER AND 
		VTYPE =@VTYPE
		AND (L.VDESCRIPTION LIKE '%' + ISNULL(@VFILTER, '') + '%' ) ), 
    T2 AS (SELECT COUNT(*) AS TOTAL FROM T1),
    T3 AS (
        SELECT * FROM T1
        ORDER BY T1.ID
        OFFSET @IROWFROM ROWS
        FETCH NEXT @TOPE ROWS ONLY
    )
    SELECT T3.*, T2.TOTAL
    FROM T3, T2

END
