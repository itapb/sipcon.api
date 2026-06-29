INSERT INTO REPORTFIGO (
    VNAME,
    VCONTENT,
    IDACCESSGROUP,
    DCREATED,
    DUPDATED,
    BACTIVE,
    BPDFREPORT,
    ISREPORT
)
VALUES (
    'ExtractTransitRepuestos',
    'WITH
            PEDIDO AS (
                SELECT
                    p.ID_REGISTRO AS PEC_ID_ORDEN_COMPRA,
                    pp.PRD_ID_PRODUCTO,
                    SUM(NVL(pp.CA_PRODUCTO_ORIGINAL, 0)) AS CANTIDAD_PEDIDA
                FROM T05_PEDIDO p
                INNER JOIN T05_PEDIDO_PRODUCTO pp
                    ON pp.PEC_ID_PEDIDO = p.ID_REGISTRO
                INNER JOIN T03_PRODUCTO tp
                    ON tp.ID_REGISTRO = pp.PRD_ID_PRODUCTO
                INNER JOIN T03_GRUPO_PRODUCTO tgp
                    ON tgp.ID_REGISTRO = tp.GRP_ID_GRUPO_PRODUCTO
                WHERE p.EMP_ID_EMPRESA IN (6354886, 6569364)
                  AND SUBSTR(tgp.CO_GRUPO_PRODUCTO, 1, 5) IN (''06-01'', ''06-02'')
                  AND tgp.CO_GRUPO_PRODUCTO NOT IN (''06-01-91'', ''06-02-91'')
                GROUP BY
                    p.ID_REGISTRO,
                    pp.PRD_ID_PRODUCTO
            ),
            RECIBIDO AS (
                SELECT
                    ne.PEC_ID_ORDEN_COMPRA,
                    nep.PRD_ID_PRODUCTO,
                    SUM(
                        NVL(
                            NVL(nep.CA_PRODUCTO_SEGUN_PROV,
                                nep.CA_PRODUCTO_ORG),
                            nep.CA_PRODUCTO
                        )
                    ) AS CANTIDAD_RECIBIDA
                FROM T03_NOTA_ENTRADA ne
                INNER JOIN T00_CONTENIDO_TABLA_VIRTUAL ctv
                    ON ctv.ID_REGISTRO = ne.CTV_ID_ESTADO_DOCUMENTO
                INNER JOIN T03_NOTA_ENTRADA_PRODUCTO nep
                    ON nep.NOE_ID_NOTA_ENTRADA = ne.ID_REGISTRO
                WHERE ctv.TVR_NB_TABLA = ''STATUS_TIPO_DOCUMENTO''
                  AND ctv.CO_OCURRENCIA = ''PR''
                GROUP BY
                    ne.PEC_ID_ORDEN_COMPRA,
                    nep.PRD_ID_PRODUCTO
            ),
            TRANSITO AS (
                SELECT
                    ped.PEC_ID_ORDEN_COMPRA,
                    ped.PRD_ID_PRODUCTO,
                    ped.CANTIDAD_PEDIDA,
                    NVL(rec.CANTIDAD_RECIBIDA, 0) AS CANTIDAD_RECIBIDA,
                    ped.CANTIDAD_PEDIDA - NVL(rec.CANTIDAD_RECIBIDA, 0) AS CANTIDAD_TRANSITO
                FROM PEDIDO ped
                LEFT JOIN RECIBIDO rec
                    ON rec.PEC_ID_ORDEN_COMPRA = ped.PEC_ID_ORDEN_COMPRA
                   AND rec.PRD_ID_PRODUCTO = ped.PRD_ID_PRODUCTO
            )
            SELECT
                t.PEC_ID_ORDEN_COMPRA,
                tp.CO_PRODUCTO AS CODIGO_REPUESTO,
                t.CANTIDAD_PEDIDA,
                t.CANTIDAD_RECIBIDA,
                t.CANTIDAD_TRANSITO,
                CASE
                    WHEN t.CANTIDAD_RECIBIDA = 0 THEN ''SIN RECIBIR''
                    WHEN t.CANTIDAD_TRANSITO = 0 THEN ''COMPLETO''
                    ELSE ''PARCIAL''
                END AS ESTADO
            FROM TRANSITO t
            INNER JOIN T03_PRODUCTO tp
                ON tp.ID_REGISTRO = t.PRD_ID_PRODUCTO
            ORDER BY
                t.PEC_ID_ORDEN_COMPRA,
                tp.CO_PRODUCTO',
    1,
    GETDATE(),
    GETDATE(),
    1,
    0,
    0
);


INSERT INTO REPORTFIGO (
    VNAME,
    VCONTENT,
    IDACCESSGROUP,
    DCREATED,
    DUPDATED,
    BACTIVE,
    BPDFREPORT,
    ISREPORT
)
VALUES (
    'ExtractSalesRepuestos',
    'SELECT
                tf.CO_SERIE_FACTURA || tf.NU_FACTURA AS INVOICENUMBER,
                tf.FE_EMISION AS DINVOICEDATE,
                torg.CO_RIF AS VVAT,
                tp.CO_PRODUCTO AS VINNERCODE,
                tfp.CA_PRODUCTO AS IQUANTITY,
                CASE 
                    WHEN tf.EMP_ID_EMPRESA = 6354886 THEN 4069
                    WHEN tf.EMP_ID_EMPRESA = 6569364 THEN 4076
                    ELSE tf.EMP_ID_EMPRESA
                END AS IDSUPPLIER
            FROM T04_FACTURA tf
                INNER JOIN T04_FACTURA_PRODUCTO tfp ON tfp.FAC_ID_FACTURA = tf.ID_REGISTRO
                INNER JOIN T03_PRODUCTO tp ON tp.ID_REGISTRO = tfp.PRD_ID_PRODUCTO
                INNER JOIN T03_GRUPO_PRODUCTO tgp ON tgp.ID_REGISTRO = tp.GRP_ID_GRUPO_PRODUCTO
                INNER JOIN T00_ORGANIZACION torg ON torg.ID_REGISTRO = tf.ORG_ID_ORGANIZACION
            WHERE SUBSTR(tgp.CO_GRUPO_PRODUCTO, 1, 5) IN (''06-01'', ''06-02'') 
                AND tgp.CO_GRUPO_PRODUCTO NOT IN (''06-01-91'', ''06-02-91'')
                AND TRUNC(tf.FE_EMISION) = TRUNC(:FECHA_EMISION)
                AND tf.CO_SERIE_FACTURA = ''A''  
            ORDER BY tf.FE_EMISION ASC',
    1,
    GETDATE(),
    GETDATE(),
    1,
    0,
    0
);