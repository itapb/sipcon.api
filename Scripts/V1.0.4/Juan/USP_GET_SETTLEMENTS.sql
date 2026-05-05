USE [SODA]
GO
/****** Object:  StoredProcedure [dbo].[USP_GET_ACCOUNTRECEIVABLE_CONSOLIDATED]    Script Date: 05/05/2026 8:24:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_GET_SETTLEMENTS] -- USP_GET_SETTLEMENTS 'J500681054'
@VSUPPLIERVAT VARCHAR(12)
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

	DECLARE  @IDSUPPLIER INT

	SET @IDSUPPLIER= (SELECT IDSUPPLIER FROM V_SUPPLIERS WHERE VVAT=@VSUPPLIERVAT) 



BEGIN
	WITH 
		T1 AS (
			-- 3. FILTRAR MUESTRA DE PART
			SELECT 
			S.IDACCOUNTRECEIVABLE,
			S.IDPAYMENT,
			COUNT(*) OVER() AS TOTAL
			FROM dbo.ACCOUNTRECEIVABLE A WITH (NOLOCK)
			INNER JOIN SETTLEMENTS S WITH (NOLOCK) ON S.IDACCOUNTRECEIVABLE = A.ID
			INNER JOIN PAYMENT P WITH (NOLOCK) ON S.IDPAYMENT = P.ID
			WHERE A.IDSUPPLIER = @IDSUPPLIER
			-- LÓGICA: No debe existir ningún detalle que tenga un estatus diferente a 'APPROVE'
			AND NOT EXISTS (
				SELECT 1 
				FROM PAYMENTDETAILS PD WITH (NOLOCK) 
				WHERE PD.IDPAYMENT = P.ID 
				AND PD.IDSTATUS <> dbo.UFN_GET_ISTATUS('APPROVE')
			)
			-- OPCIONAL: Asegurar que al menos tenga un detalle (que no sea un pago vacío)
			AND EXISTS (
				SELECT 1 
				FROM PAYMENTDETAILS PD WITH (NOLOCK) 
				WHERE PD.IDPAYMENT = P.ID
			)
		)
		 -- 5. PAGINAR 
		SELECT T1.* 
		FROM T1
        ORDER BY T1.IDPAYMENT
END
