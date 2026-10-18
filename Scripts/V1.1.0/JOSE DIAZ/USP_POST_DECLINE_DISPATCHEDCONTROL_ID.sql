CREATE PROCEDURE [dbo].[USP_POST_DECLINE_DISPATCHEDCONTROL_ID] 
@IDDISPATCHED INT
AS
/* '===============================================================          
  '   NOMBRE                : 
  '   FECHA CREACIÓN        : 
  '   CREADO POR            : WILMAN CAMACHO 
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
    BEGIN TRAN

            DECLARE @BACKORDER BIT = 1
            DECLARE @IDBACKORDER_NEW INT,@IREQUIREDNEW INT
            DECLARE @IDLOCATION INT ,@IREAL INT,@VMODE VARCHAR(1)
            DECLARE @ID INT,@IDINVOICE INT,@IDPART INT,@IDMOVEMENTDETAIL INT,@IDBACKORDER INT
            
            DECLARE @IUPDATED INT = 0
            
            DECLARE DETAIL_CURSOR CURSOR FOR 
                SELECT DISTINCT DC.IDPART,DC.IDINVOICE,DC.IDBACKORDER
                FROM DISPATCHEDCONTROL DC WITH (NOLOCK)
                INNER JOIN PART P WITH (NOLOCK) ON P.ID=DC.IDPART
                WHERE DC.ID = @IDDISPATCHED                
                --AND P.VINNERCODE= CASE WHEN @VINNERCODE='' THEN P.VINNERCODE ELSE @VINNERCODE END
            OPEN DETAIL_CURSOR;
            FETCH NEXT FROM DETAIL_CURSOR INTO @IDPART,@IDINVOICE,@IDBACKORDER ;

            WHILE @@FETCH_STATUS = 0
            BEGIN
    
                UPDATE DISPATCHEDCONTROL 
                SET IDCONTROL = 0,
                IESTATUS = CASE WHEN @BACKORDER = 1 THEN 19 ELSE 2 END, 
                BSINC = CASE WHEN @BACKORDER = 1 THEN 0 ELSE 1 END, 
                BMARK = 0,
                IDINVOICE=0,
                IINVOICED=0
                WHERE ID=@IDDISPATCHED

                IF @IDINVOICE>0
                BEGIN
                    UPDATE INVOICECONTROL SET IDCONTROL=0,IDISPATCHED=0,IDESTATUS=2
                    WHERE IDINVOICE=@IDINVOICE AND IDPART=@IDPART
                END

                IF @BACKORDER=1
                BEGIN
                    DECLARE DETAIL_LOCATIONS CURSOR FOR 
                        SELECT IDLOCATION,IREAL,CASE WHEN VTYPE='P' THEN '+' ELSE '-' END AS VMODE 
                        FROM MOVEMENTDETAILS WITH (NOLOCK)
                        WHERE IDBACKORDER=@IDBACKORDER 
                        AND BPROCESSED =1
                    OPEN DETAIL_LOCATIONS;
                    FETCH NEXT FROM DETAIL_LOCATIONS INTO @IDLOCATION, @IREAL, @VMODE;

                    WHILE @@FETCH_STATUS = 0
                    BEGIN   
                            EXEC USP_POST_INVENTORY @IDLOCATION,@IDPART,@IREAL,1,@VMODE

                        FETCH NEXT FROM DETAIL_LOCATIONS INTO @IDLOCATION, @IREAL, @VMODE;
                    END;

                    CLOSE DETAIL_LOCATIONS;
                    DEALLOCATE DETAIL_LOCATIONS;

                    SELECT @IREQUIREDNEW= IBACKORDER FROM BACKORDER WITH (NOLOCK) WHERE ID=@IDBACKORDER

                    EXEC USP_NEW_BACKORDER @IDBACKORDER_NEW OUTPUT,@IDBACKORDER,@IREQUIREDNEW,1

                END


                FETCH NEXT FROM DETAIL_CURSOR INTO @IDPART,@IDINVOICE,@IDBACKORDER ;
            END;

            CLOSE DETAIL_CURSOR;
            DEALLOCATE DETAIL_CURSOR;
            
         SELECT
                ISNULL(NULLIF(@IDDISPATCHED, 0), 0) AS IID, 
                1 AS IUPDATED,
                0 AS IDELETED,
                0 IINSERTED
        
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRAN
        DECLARE @ErrorMessage NVARCHAR(4000) 
        SELECT  @ErrorMessage = ERROR_PROCEDURE() + ' : ' + ERROR_MESSAGE()
        RAISERROR ( @ErrorMessage , 16,1) 
    END CATCH

END
