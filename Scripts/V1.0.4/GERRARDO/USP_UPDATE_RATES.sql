USE [SODA]
GO
/****** Objeto: StoredProcedure [dbo].[USP_UPDATE_RATES] Fecha de script: 17/4/2026 9:03:46 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].USP_UPDATE_RATES --USP_UPDATE_RATES
@ID INT,
@NRATE DECIMAL(18,2)
AS
/* '===============================================================          
  '   NOMBRE                : 
  '   FECHA CREACIÓN        : 
  '   CREADO POR            : GERARDO JIMENEZ
  '   CREADO PARA           : 
  '   FUNCIÓN               : EDITAR LAS TASAS
  '   VERSIÓN               : 
  '   MODIFICADO EN         : 
  '   MODIFICADO POR        : 
  '   RAZÓN DE MODIFICACIÓN : 
  '===============================================================*/

  
SET XACT_ABORT ON               
SET NOCOUNT ON
SET LOCK_TIMEOUT 180000

BEGIN TRANSACTION

BEGIN TRY
    -- Validar que exista el registro
    IF NOT EXISTS (SELECT 1 FROM RATE WITH (NOLOCK) WHERE ID = @ID)
    BEGIN
        RAISERROR('No se encontró la tasa con ID %d', 16, 1, @ID)
        RETURN
    END

    -- Validar que la tasa no sea negativa
    IF @NRATE < 0
    BEGIN
        RAISERROR('La tasa no puede ser negativa', 16, 1)
        RETURN
    END
    
    -- Validar que la tasa no sea CERO
    IF @NRATE = 0
    BEGIN
        RAISERROR('La tasa no puede ser 0', 16, 1)
        RETURN
    END
    -- Actualizar solo la tasa, conservando la fecha 
    UPDATE RATE
    SET NRATE = @NRATE
    WHERE ID = @ID

    SELECT 
        ID,
        DDATE,
        NRATE
    FROM RATE WITH (NOLOCK)
    WHERE ID = @ID

    COMMIT TRANSACTION
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
    DECLARE @ErrorState INT = ERROR_STATE()
    
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
GO