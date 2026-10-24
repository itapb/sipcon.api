
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

    -- PREINVENTARIO (FÍSICO: IDCOUNTTYPE = 2, IDESTATUS = 20)
    IF @IDESTATUS = 20 AND @IDCOUNTTYPE = 2
    BEGIN
        SELECT 
            ICD.ID                      AS IDINVENTORYDETAIL,
            ICD.IDLOCATION,
            ICD.IDPART,
            ICD.IQUANTITY_OLD,
     		ICD.VOBSERVATION,
     		ICD.BVALID,
            CAST(NULL AS NUMERIC(18,4)) AS IQUANTITY_NEW,
            CAST(NULL AS VARCHAR(50))   AS IFORM,
            L.VNAME                     AS LOCATIONNAME,
            P.VINNERCODE                AS PARTCODE,
            P.VDESCRIPTION              AS PARTDESCRIPTION,
            --CAST(NULL AS INT)           AS IDASSIGN,
            -- CAST(NULL AS VARCHAR(10))   AS VASSIGNTYPE,
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
        WHERE ICD.IDINVENTORY   = @IDINVENTORYCOUNT
          AND ICD.IDUSER        = @IDUSER
          AND ICD.IDCOUNT       = 0 
          AND ICD.IDSTATUS      = 2
          AND ICD.IQUANTITY_NEW   IS NULL
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

    -- INVENTARIO FINAL FÍSICO Y CÍCLICO 
    IF @IDESTATUS = 6 AND (@IDCOUNTTYPE = 2 OR @IDCOUNTTYPE = 1)
    BEGIN
        SELECT
            ICD.ID                      AS IDINVENTORYDETAIL,
            ICD.IDLOCATION,
            ICD.IDPART,
            ICD.IQUANTITY_OLD,
            ICD.IQUANTITY_NEW,
            ICD.IFORM,
            ICD.VOBSERVATION,
            ICD.BVALID,
            L.VNAME                     AS LOCATIONNAME,
            P.VINNERCODE                AS PARTCODE,
            P.VDESCRIPTION              AS PARTDESCRIPTION,
           -- CAST(NULL AS INT)           AS IDASSIGN,       
           --CAST(NULL AS VARCHAR(10))   AS VASSIGNTYPE,   
            CASE 
                WHEN @IDCOUNTTYPE = 2 THEN 'INVENTARIO FINAL'
                WHEN @IDCOUNTTYPE = 1 THEN 'CICLICO'
            END                         AS MODO,
            'Conteo pendiente'          AS MENSAJE
        FROM INVENTORYCOUNTDETAIL ICD WITH (NOLOCK)
        INNER JOIN PART     P WITH (NOLOCK)
              ON  P.ID         = ICD.IDPART
              AND P.IDSUPPLIER = @IDSUPPLIER
        INNER JOIN LOCATION L WITH (NOLOCK)
              ON  L.ID         = ICD.IDLOCATION
              AND L.IDSUPPLIER = @IDSUPPLIER
        INNER JOIN ZONE     Z WITH (NOLOCK)
              ON  Z.ID         = L.IDZONE
        WHERE ICD.IDINVENTORY   = @IDINVENTORYCOUNT
          AND ICD.IDUSER        = @IDUSER
          AND ICD.IDCOUNT       >= 1
          AND ICD.IDSTATUS      = 2
        ORDER BY
            CASE 
                WHEN Z.VSIZE = 'P' THEN 1
                WHEN Z.VSIZE = 'M' THEN 2 
                ELSE 3 
            END ASC,
            L.VNAME ASC,
            P.VINNERCODE ASC;
    END
END
