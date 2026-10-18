
CREATE PROCEDURE [dbo].[USP_GET_STOCK_COUNT] -- [USP_GET_STOCK_COUNT] 1,4069
    @IDUSER     INT,
    @IDSUPPLIER INT

/* '==============================================================
  '   NOMBRE                : USP_GET_STOCK_COUNT
  '   FECHA CREACIÓN        :
  '   CREADO POR            : GERARDO JIMENEZ
  '   CREADO PARA           :
  '   FUNCIÓN               : Devuelve los tipos de inventario activos (FÍSICO / CÍCLICO)
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

    ;WITH INVENTARIOS_ACTIVOS AS (
        SELECT
            IC.ID,
            IC.IDCOUNTTYPE,
            IC.IDESTATUS,
            IC.VDESCRIPTION
        FROM INVENTORYCOUNT IC WITH (NOLOCK)
        WHERE IC.IDSUPPLIER = @IDSUPPLIER
          AND (
                (IC.IDCOUNTTYPE = 1 AND IC.IDESTATUS = 6)           -- CÍCLICO
             OR (IC.IDCOUNTTYPE = 2 AND IC.IDESTATUS IN (20, 6))    -- FÍSICO
              )
    ),
    INVENTARIO_SELECCIONADO AS (
        -- CÍCLICO: el más antiguo
        SELECT IA.ID, IA.IDCOUNTTYPE, IA.IDESTATUS, IA.VDESCRIPTION
        FROM INVENTARIOS_ACTIVOS IA
        WHERE IA.ID = (
            SELECT MIN(IA2.ID)
            FROM INVENTARIOS_ACTIVOS IA2
            WHERE IA2.IDCOUNTTYPE = 1
        )
        UNION ALL
        -- FÍSICO: el único activo (preinventario o inventario final)
        SELECT IA.ID, IA.IDCOUNTTYPE, IA.IDESTATUS, IA.VDESCRIPTION
        FROM INVENTARIOS_ACTIVOS IA
        WHERE IA.ID = (
            SELECT MIN(IA2.ID)
            FROM INVENTARIOS_ACTIVOS IA2
            WHERE IA2.IDCOUNTTYPE = 2
        )
    )
    SELECT
        CASE
            WHEN ISEL.IDCOUNTTYPE = 1 THEN 'CICLICO'
            WHEN ISEL.IDCOUNTTYPE = 2 THEN 'FISICO'
        END                                                 AS TIPO,
        ISEL.ID                                             AS IDINVENTORYCOUNT,
        ISEL.IDCOUNTTYPE,
        ISEL.IDESTATUS,
        ISEL.VDESCRIPTION,
        CASE WHEN EXISTS (
            SELECT 1
            FROM COUNTBYUSER CBU WITH (NOLOCK)
            WHERE CBU.IDINVENTORYCOUNT = ISEL.ID
              AND CBU.IDSUPPLIER       = @IDSUPPLIER
              AND CBU.IDUSER           = @IDUSER
        ) THEN 1 ELSE 0 END                                 AS TIENE_ASIGNACION
    FROM INVENTARIO_SELECCIONADO ISEL
    ORDER BY
        CASE WHEN ISEL.IDCOUNTTYPE = 2 THEN 1 ELSE 2 END,  
        ISEL.ID;
END
