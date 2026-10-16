CREATE PROCEDURE [dbo].[USP_GET_REPORTS_REP017]
    @IDUSER     INT,
    @IDSUPPLIER INT          = NULL,
    @IROWFROM   INT          = NULL,
    @VFILTER    VARCHAR(100) = NULL,
    @IBACKUP    INT          = NULL   -- 1 = Preinventario, 2 = Inventario, 3= Postinventario
AS
/* '===============================================================
  '   NOMBRE                : USP_GET_REPORTS_REP017
  '   FECHA CREACIÓN        : 
  '   CREADO POR            : GERARDO JIMENEZ 
  '   CREADO PARA           : REPORTE DE TOMA FÍSICA DE INVENTARIO
  '   FUNCIÓN               : Mostrar stock de partes por ubicación según etapa (Preinventario / Inventario)
  '   VERSIÓN               : 
  '   MODIFICADO EN         : 
  '   MODIFICADO POR        : 
  '   RAZÓN DE MODIFICACIÓN : 
  '===============================================================*/

SET XACT_ABORT ON;
SET NOCOUNT ON;
SET LOCK_TIMEOUT 180000;

  -- PREPARAR PAGINACION
    DECLARE @TOPE INT
    IF @IROWFROM IS NULL
    BEGIN
        SET @TOPE     = 1000000
        SET @IROWFROM = 0
    END
    ELSE
    BEGIN
        SET @TOPE = 100
    END

    -- PREPARAR FILTRO
    DECLARE @VFILTER2   VARCHAR(100) = ISNULL(@VFILTER, '')
    DECLARE @LIKEFILTER VARCHAR(100) = '%' + @VFILTER2 + '%'

BEGIN

    WITH T1 AS
    (
        -- 1. CONSULTA BASE: UNA FILA POR (PARTE, UBICACION) CON AMBAS ETAPAS
        SELECT
            P.VINNERCODE                                                    AS CODIGO_PARTE,
            P.VDESCRIPTION                                                  AS DESCRIPCION_PARTE,
            L.VNAME                                                         AS UBICACION,
            MAX(CASE WHEN IB.IBACKUP = 1 THEN IB.ISTOCK ELSE 0 END)         AS CANTIDAD_PREINVENTARIO,
            MAX(CASE WHEN IB.IBACKUP = 2 THEN IB.ISTOCK ELSE 0 END)         AS CANTIDAD_INVENTARIO,
            MAX(CASE WHEN IB.IBACKUP = 3 THEN IB.ISTOCK ELSE 0 END)         AS CANTIDAD_POSTINVENTARIO,
            MAX(IB.DCREATED)                                                AS FECHA
        FROM dbo.INVENTORYBACKUP IB WITH (NOLOCK)
            INNER JOIN PART     P WITH (NOLOCK) ON P.ID = IB.IDPART
            INNER JOIN LOCATION L WITH (NOLOCK) ON L.ID = IB.IDLOCATION
        WHERE (@IDSUPPLIER IS NULL OR P.IDSUPPLIER = @IDSUPPLIER)
          AND (@VFILTER2 = '' OR P.VINNERCODE LIKE @LIKEFILTER OR P.VDESCRIPTION LIKE @LIKEFILTER OR L.VNAME LIKE @LIKEFILTER)
        GROUP BY P.VINNERCODE, P.VDESCRIPTION, L.VNAME
    ),
    T2 AS
    (
        -- 2. TOTAL DE REGISTROS
        SELECT COUNT(*) AS TOTAL FROM T1
    ),
    T3 AS
    (
        -- 3. SELECCION FINAL PAGINADA
        SELECT
            T1.CODIGO_PARTE,
            T1.DESCRIPCION_PARTE,
            T1.UBICACION,
            T1.CANTIDAD_PREINVENTARIO,
            T1.CANTIDAD_INVENTARIO,
            CANTIDAD_POSTINVENTARIO,	
            T1.FECHA
        FROM T1
        ORDER BY T1.UBICACION, T1.CODIGO_PARTE
        OFFSET @IROWFROM ROWS
        FETCH NEXT @TOPE ROWS ONLY
    )
    SELECT T3.*, T2.TOTAL
    FROM T3, T2

END