USE [SODA]
GO
/****** Objeto: StoredProcedure [dbo].[USP_POST_RETURNS_FIGO] Fecha de script: 30/8/2026 7:36:56 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[USP_POST_RETURNS_FIGO]
    @DATA NVARCHAR(MAX),
    @IDUSER INT 
AS

/* '===============================================================          
  '   NOMBRE                : 
  '   FECHA CREACIÓN        : 
  '   CREADO POR            : GERARDO JIMENEZ
  '   CREADO PARA           : 
  '   FUNCIÓN               : INSERTAR LAS DEVOLUCIONES DE VENTAS DE FIGO
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
        BEGIN TRANSACTION;
        
        INSERT INTO RETURNSFIGO (
            VNOTENUMBER,
            DNOTEDATE,
            VDESCRIPTION,
            VINNERCODE,
            IQUANTITY,
            VREASON,
            VINVOICENUMBER,
            DINVOICEDATE,
            IDSUPPLIER,
            DCREATED
        )
        SELECT 
            VNOTENUMBER,
            DNOTEDATE,
            VDESCRIPTION,
            VINNERCODE,
            IQUANTITY,
            VREASON,
            VINVOICENUMBER,
            DINVOICEDATE,
            IDSUPPLIER,
            GETDATE() AS DCREATED
        FROM OPENJSON(@DATA)
        WITH (
            VNOTENUMBER VARCHAR(100) '$.vnoteNumber',
            DNOTEDATE DATETIME '$.dnoteDate',
            VDESCRIPTION VARCHAR(200) '$.vDescription',
            VINNERCODE VARCHAR(50) '$.vInnerCode',
            IQUANTITY INT '$.iQuantity',
            VREASON VARCHAR(100) '$.vreason',
            VINVOICENUMBER VARCHAR(100) '$.vinvoiceNumber',
            DINVOICEDATE DATETIME '$.dinvoiceDate',
            IDSUPPLIER INT '$.idSupplier'
        ) JSONDATA
        WHERE NOT EXISTS (
            SELECT 1 FROM RETURNSFIGO 
            WHERE VNOTENUMBER = JSONDATA.VNOTENUMBER 
            AND VINNERCODE = JSONDATA.VINNERCODE
            AND VINVOICENUMBER = JSONDATA.VINVOICENUMBER
        );
        
        SELECT 
            1 AS Processed,
            'Success' AS Message,
            @@ROWCOUNT AS IINSERTED,
            0 AS IUPDATED,      
            SCOPE_IDENTITY() AS IID  
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        SELECT 
            0 AS Processed,
            ERROR_MESSAGE() AS Message,
            0 AS IINSERTED,
            0 AS IUPDATED,
            0 AS IID
    END CATCH
END;
GO