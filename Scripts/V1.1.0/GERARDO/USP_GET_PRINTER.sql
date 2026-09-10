USE [SODA]
GO
/****** Objeto: StoredProcedure [dbo].[USP_GET_PRINTER] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[USP_GET_PRINTER] -- [USP_GET_PRINTER] 4076
 @IDSUPPLIER INT = NULL
AS
/* '===============================================================          
  '   NOMBRE                : USP_GET_PRINTER
  '   FECHA CREACIÓN        : 
  '   CREADO POR            : GERARDO JIMENEZ 
  '   CREADO PARA           : 
  '   FUNCIÓN               : Obtener listado de impresoras por proveedor
  '   VERSIÓN               : 
  '   MODIFICADO EN         : 
  '   MODIFICADO POR        :  
  '   RAZÓN DE MODIFICACIÓN : 
  '===============================================================*/

SET XACT_ABORT ON               
SET NOCOUNT ON
SET LOCK_TIMEOUT 180000

BEGIN

    SELECT 
        ID,
        PRINTER,
        IDSUPPLIER,
        BDEFAULT
    FROM PRINTER WITH (NOLOCK)
    WHERE (@IDSUPPLIER IS NULL OR IDSUPPLIER = @IDSUPPLIER)
    ORDER BY BDEFAULT DESC, PRINTER

END