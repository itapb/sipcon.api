
CREATE PROCEDURE [dbo].[USP_GET_DETAIL_STOCK_COUNT] -- [USP_GET_DETAIL_STOCK_COUNT] 1,4076,2
    @IDUSER             INT,
    @IDSUPPLIER         INT,
    @IDINVENTORYCOUNT   INT
/* '==============================================================
  '   NOMBRE                : USP_GET_DETAIL_STOCK_COUNT
  '   FECHA CREACIÓN        :
  '   CREADO POR            : GERARDO JIMENEZ
  '   CREADO PARA           :
  '   FUNCIÓN               : Mostrar detalles del conteo en el PDA
  '   VERSIÓN               : 
  '   MODIFICADO EN         :
  '   MODIFICADO POR        : 
  '   RAZÓN DE MODIFICACIÓN : 
  '==============================================================*/
AS
BEGIN
    SET XACT_ABORT ON;
    SET NOCOUNT ON;
    SET LOCK_TIMEOUT 180000;

    DECLARE @IDESTATUS    INT;
    DECLARE @IDCOUNTTYPE  INT;
    DECLARE @ErrorMessage NVARCHAR(4000);

    -- OBTENER ETAPA DEL INVENTARIO
    SELECT
        @IDESTATUS   = IC.IDESTATUS,
        @IDCOUNTTYPE = IC.IDCOUNTTYPE
    FROM INVENTORYCOUNT IC WITH (NOLOCK)
    WHERE IC.ID         = @IDINVENTORYCOUNT
      AND IC.IDSUPPLIER = @IDSUPPLIER;

     -- VALIDAR ASIGNACIÓN SOLO EN PREINVENTARIO
    IF @IDESTATUS = 20 AND @IDCOUNTTYPE = 2
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM COUNTBYUSER CBU WITH (NOLOCK)
            WHERE CBU.IDINVENTORYCOUNT = @IDINVENTORYCOUNT
              AND CBU.IDSUPPLIER       = @IDSUPPLIER
              AND CBU.IDUSER           = @IDUSER
              AND CBU.IDCOUNT          = 0
        )
        BEGIN
            SET @ErrorMessage = 'NO TIENE ASIGNACIONES PARA ESTE INVENTARIO';
            RAISERROR(@ErrorMessage, 16, 1);
            RETURN;
        END
    

    -- PREINVENTARIO (FÍSICO: IDCOUNTTYPE = 2, IDESTATUS = 20)
    BEGIN
        SELECT TOP 1
            ICD.ID                      AS IDINVENTORYDETAIL,
            ICD.IDLOCATION,
            ICD.IDPART,
            ICD.IQUANTITY_OLD,
            CAST(NULL AS NUMERIC(18,4)) AS IQUANTITY_NEW,
            CAST(NULL AS VARCHAR(50))   AS IFORM,
            L.VNAME                     AS LOCATIONNAME,
            P.VINNERCODE                AS PARTCODE,
            P.VDESCRIPTION              AS PARTDESCRIPTION,
            CBU.IDASSIGN,
            CBU.VASSIGNTYPE,
            'PREINVENTARIO'             AS MODO,
            'Reconteo pendiente'        AS MENSAJE
        FROM INVENTORYCOUNTDETAIL ICD WITH (NOLOCK)
        INNER JOIN PART     P WITH (NOLOCK)
              ON  P.ID         = ICD.IDPART
              AND P.IDSUPPLIER = @IDSUPPLIER
        INNER JOIN LOCATION L WITH (NOLOCK)
              ON  L.ID         = ICD.IDLOCATION
              AND L.IDSUPPLIER = @IDSUPPLIER
        INNER JOIN ZONE     Z WITH (NOLOCK)
              ON  Z.ID         = L.IDZONE
        INNER JOIN COUNTBYUSER CBU WITH (NOLOCK)
              ON  CBU.IDINVENTORYCOUNT = ICD.IDINVENTORY
              AND CBU.IDSUPPLIER       = @IDSUPPLIER
              AND CBU.IDUSER           = @IDUSER
              AND CBU.IDCOUNT          = 0
              AND CBU.VASSIGNTYPE      = 'Z'
              AND CBU.IDASSIGN         = Z.ID
        WHERE ICD.IDINVENTORY   = @IDINVENTORYCOUNT
          AND ICD.IDCOUNT       = 0
          AND ICD.IDSTATUS      = 2
          AND ICD.IDUSER        IS NULL
          AND ICD.IQUANTITY_NEW IS NULL
          ORDER BY
            CASE 
                WHEN Z.VSIZE = 'P' THEN 1
                WHEN Z.VSIZE = 'M' THEN 2 
                ELSE 3 
            END ASC,
            L.VNAME ASC,
            P.VINNERCODE ASC,
            ICD.ID ASC;
          END
            
END

    -- INVENTARIO FINAL FÍSICO Y CÍCLICO 
    BEGIN
        SELECT
            ICD.ID                  AS IDINVENTORYDETAIL,
            ICD.IDLOCATION,
            ICD.IDPART,
            ICD.IQUANTITY_OLD,
            ICD.IQUANTITY_NEW,
            ICD.IFORM,
            L.VNAME                 AS LOCATIONNAME,
            P.VINNERCODE            AS PARTCODE,
            P.VDESCRIPTION          AS PARTDESCRIPTION,
            CAST(NULL AS INT)           AS IDASSIGN,       
            CAST(NULL AS VARCHAR(10))   AS VASSIGNTYPE,   
            CASE 
            WHEN @IDCOUNTTYPE = 2 THEN 'INVENTARIO FINAL'
            WHEN @IDCOUNTTYPE = 1 THEN 'CICLICO'
            END AS MODO,
            'Conteo pendiente'      AS MENSAJE
        FROM INVENTORYCOUNTDETAIL ICD WITH (NOLOCK)
        INNER JOIN PART     P WITH (NOLOCK)
              ON  P.ID         = ICD.IDPART
              AND P.IDSUPPLIER = @IDSUPPLIER
        INNER JOIN LOCATION L WITH (NOLOCK)
              ON  L.ID         = ICD.IDLOCATION
              AND L.IDSUPPLIER = @IDSUPPLIER
        INNER JOIN ZONE     Z WITH (NOLOCK)
              ON  Z.ID         = L.IDZONE
        WHERE ICD.IDINVENTORY = @IDINVENTORYCOUNT
          AND ICD.IDUSER = @IDUSER
          AND ICD.IDCOUNT     >= 1
          AND ICD.IDSTATUS    = 2
            ORDER BY
            CASE 
                WHEN Z.VSIZE = 'P' THEN 1
                WHEN Z.VSIZE = 'M' THEN 2 
                ELSE 3 
            END ASC,
            L.VNAME ASC,
            P.VINNERCODE ASC,
            ICD.ID ASC;
     END
   END
