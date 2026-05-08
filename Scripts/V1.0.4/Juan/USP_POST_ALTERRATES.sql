USE [SODA]
GO
/****** Object:  StoredProcedure [dbo].[USP_INSERT_RATES]    Script Date: 08/05/2026 7:55:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[USP_POST_ALTERRATES] --[USP_POST_ALTERRATES] 
    @DATA NVARCHAR(MAX)
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

BEGIN
    BEGIN TRY
     
        DECLARE @ErrorMessage NVARCHAR(4000)
        DECLARE @NAME         VARCHAR(100) = NULL
        DECLARE @VACTION      VARCHAR(20)
        DECLARE @IID          INT
        DECLARE @IINSERTED    INT
        DECLARE @IUPDATED     INT
        DECLARE @IDMODULE     INT = [dbo].[UFN_GET_IDMODULE]('PAGOS-TASAS')
		DECLARE  @IDUSER INT=1
         
        DECLARE @TDATA AS TABLE (
            NALTERRATE      NUMERIC(18,6),
			DRATEDATE  DATETIME
        )

       INSERT INTO @TDATA ( NALTERRATE, DRATEDATE)
		SELECT
			NAlterRate,
			CONVERT(DATETIME2, DDate, 127) -- El estilo 127 maneja el formato ISO (T) que recibes
		FROM OPENJSON(@DATA)
		WITH (
			NAlterRate   NUMERIC(18,6), 
			DDate   VARCHAR(30) -- Aumenta a 30 por seguridad, ya que viene con hora y zona horaria
		)

        -- TASA NEGATIVA
        IF @ErrorMessage IS NULL
            IF EXISTS (SELECT 1 FROM @TDATA WHERE NALTERRATE < 0)
                SET @ErrorMessage = 'LA TASA NO PUEDE SER NEGATIVA' 

        IF @ErrorMessage IS NOT NULL
        BEGIN
            RAISERROR(@ErrorMessage, 16, 1)
            RETURN
        END
         
        BEGIN TRAN 
            IF NOT EXISTS (SELECT 1 FROM @TDATA t 
			            INNER JOIN RATE R WITH (NOLOCK) ON T.DRATEDATE=R.DDATE AND ISNULL(R.NALTERRATE,0)=0 )
            BEGIN
               INSERT INTO RATE(NRATE,DDATE,NALTERRATE)
			   SELECT 0,T.DRATEDATE,ISNULL(T.NALTERRATE,0) FROM @TDATA T
			   WHERE NOT EXISTS (SELECT * FROM RATE R WITH (NOLOCK) WHERE T.NALTERRATE=R.NALTERRATE AND T.DRATEDATE=R.DDATE) 
                SELECT @IINSERTED = @@ROWCOUNT
            END
			ELSE
			BEGIN
			 UPDATE R
			 SET R.NALTERRATE =T.NALTERRATE
			 FROM RATE R 
			 INNER JOIN @TDATA T ON R.DDATE=T.DRATEDATE AND ISNULL(R.NALTERRATE,0)=0
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
