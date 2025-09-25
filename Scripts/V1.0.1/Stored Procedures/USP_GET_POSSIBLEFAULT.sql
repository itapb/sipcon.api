GO
/****** Object:  StoredProcedure [dbo].[USP_GET_POSSIBLEFAULT]    Script Date: 18/08/2025 8:46:26 ******/
SET ANSI_NULLS ON

GO
SET QUOTED_IDENTIFIER ON
GO
IF  NOT EXISTS (SELECT * FROM SYS.OBJECTS

                WHERE OBJECT_ID = OBJECT_ID(N'[DBO].[USP_GET_POSSIBLEFAULT]')

                AND TYPE IN (N'P', N'PC', N'TF', N'FN'))
EXEC('CREATE PROCEDURE [DBO].[USP_GET_POSSIBLEFAULT] AS BEGIN SET NOCOUNT ON  END')
GO
ALTER PROCEDURE [dbo].[USP_GET_POSSIBLEFAULT]

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


SELECT C.* FROM POSSIBLEFAULT C WITH (NOLOCK)
WHERE C.VNAME!='NO APLICA'
