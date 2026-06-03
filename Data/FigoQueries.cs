namespace Data
{
    public static class FigoQueries
    {
        public static readonly string ReportCxC = """
            WITH DetalleProductos AS (
                SELECT 
                    s_tf.ID_REGISTRO AS FAC_ID_REGISTRO,
                    s_tp.NB_PRODUCTO,
                    s_tlp.NU_SERIAL_PRODUCTO,
                    s_tgp.CO_GRUPO_PRODUCTO
                FROM T04_FACTURA s_tf
                    INNER JOIN T04_COTIZACION s_tc ON s_tc.ID_REGISTRO = s_tf.COT_ID_COTIZACION
                    INNER JOIN T04_COTIZACION_PRODUCTO s_tcp ON s_tcp.COT_ID_COTIZACION = s_tc.ID_REGISTRO
                    INNER JOIN T03_PRODUCTO s_tp ON s_tp.ID_REGISTRO = s_tcp.PRD_ID_PRODUCTO
                    INNER JOIN T03_GRUPO_PRODUCTO s_tgp ON s_tgp.ID_REGISTRO = s_tp.GRP_ID_GRUPO_PRODUCTO
                    INNER JOIN T03_LOTE_PRODUCTO s_tlp ON s_tlp.ID_REGISTRO = s_tcp.LOP_ID_LOTE_PRODUCTO
                WHERE SUBSTR(s_tgp.CO_GRUPO_PRODUCTO, 1, 2) IN ('01', '02')
            )
            SELECT
                tf.ID_REGISTRO,
                te.NB_EMPRESA                               EMPRESA,
                torg.CO_RIF                                 RIF,
                torg.NB_ORGANIZACION                        ORGANIZACION,
                torg.NU_TELEFONO_PRINCIPAL                  TELEFONO,
                tug.NB_UBICACION                            ZONA,
                tvc.CO_OCURRENCIA                           OCURRENCIA,
                tf.CO_SERIE_FACTURA || tf.NU_FACTURA        DOCUMENTO,
                TO_CHAR(tf.FE_EMISION, 'DD/MM/YYYY')        EMISION,
                TO_CHAR(tf.FE_VENCIMIENTO, 'DD/MM/YYYY')    VENCIMIENTO,
                CASE 
                    WHEN TO_DATE(:FECHA_CORTE, 'MM/DD/YYYY') > tf.FE_VENCIMIENTO
                    THEN TRUNC(TO_DATE(:FECHA_CORTE, 'MM/DD/YYYY')) - TRUNC(tf.FE_VENCIMIENTO)
                    ELSE 0 
                END AS                                      DIAS_VENCIDOS,
                :MONEDA_REPORTE                             MONEDA,
                tum.CO_UNIDAD_MEDIDA                        MONEDA_DOCUMENTO,
                CAST(CASE
                    WHEN tum.CO_UNIDAD_MEDIDA = 'BS' AND :MONEDA_REPORTE = 'USD' THEN tf.MN_DEUDA_PENDIENTE / NULLIF(tf.FC_TASA_CAMBIO, 0)
                    WHEN tum.CO_UNIDAD_MEDIDA = 'USD' AND :MONEDA_REPORTE = 'BS' THEN tf.MN_DEUDA_PENDIENTE * tf.FC_TASA_CAMBIO
                    ELSE tf.MN_DEUDA_PENDIENTE
                END AS NUMBER(19,4)) AS                     VENCIDO,
                CAST(CASE
                    WHEN tum.CO_UNIDAD_MEDIDA = 'BS' AND :MONEDA_REPORTE = 'USD' THEN (CASE WHEN tf.MN_DEUDA_PENDIENTE = 0 THEN tf.MN_PAGO_INICIAL ELSE 0 END) / NULLIF(tf.FC_TASA_CAMBIO, 0)
                    WHEN tum.CO_UNIDAD_MEDIDA = 'USD' AND :MONEDA_REPORTE = 'BS' THEN (CASE WHEN tf.MN_DEUDA_PENDIENTE = 0 THEN tf.MN_PAGO_INICIAL ELSE 0 END) * tf.FC_TASA_CAMBIO
                    ELSE (CASE WHEN tf.MN_DEUDA_PENDIENTE = 0 THEN tf.MN_PAGO_INICIAL ELSE 0 END)
                END AS NUMBER(19,4)) AS                     POR_VENCER,
                CAST(CASE
                    WHEN tum.CO_UNIDAD_MEDIDA = 'BS' AND :MONEDA_REPORTE = 'USD' THEN (CASE WHEN tf.MN_DEUDA_PENDIENTE = 0 THEN tf.MN_PAGO_INICIAL ELSE tf.MN_DEUDA_PENDIENTE END) / NULLIF(tf.FC_TASA_CAMBIO, 0)
                    WHEN tum.CO_UNIDAD_MEDIDA = 'USD' AND :MONEDA_REPORTE = 'BS' THEN (CASE WHEN tf.MN_DEUDA_PENDIENTE = 0 THEN tf.MN_PAGO_INICIAL ELSE tf.MN_DEUDA_PENDIENTE END) * tf.FC_TASA_CAMBIO
                    ELSE (CASE WHEN tf.MN_DEUDA_PENDIENTE = 0 THEN tf.MN_PAGO_INICIAL ELSE tf.MN_DEUDA_PENDIENTE END)
                END AS NUMBER(19,4)) AS                     TOTAL_DEUDA,
                tf.FC_TASA_CAMBIO                           TASA,
                ud.NB_PRODUCTO                              PRODUCTO,
                ud.NU_SERIAL_PRODUCTO                       SERIAL,
                ud.CO_GRUPO_PRODUCTO                        GRUPO_PRODUCTO,
                tf.FE_EMISION                               AS FECHA_ORDEN_DATE
            FROM T04_FACTURA tf 
                INNER JOIN T00_UNIDAD_MEDIDA tum ON tum.ID_REGISTRO = tf.UNM_ID_UNIDAD_MONETARIA 
                INNER JOIN T00_EMPRESA te ON te.ID_REGISTRO = tf.EMP_ID_EMPRESA
                INNER JOIN T00_ORGANIZACION torg ON torg.ID_REGISTRO = tf.ORG_ID_ORGANIZACION 
                INNER JOIN T00_UBICACION_GEOGRAFICA tug ON tug.ID_REGISTRO = torg.UBG_ID_UBICACION
                INNER JOIN T00_CONTENIDO_TABLA_VIRTUAL tvc ON tvc.ID_REGISTRO = tf.CTV_ID_TIPO_DOCUMENTO 
                LEFT JOIN DetalleProductos ud ON ud.FAC_ID_REGISTRO = tf.ID_REGISTRO
            WHERE 
                CASE WHEN tf.MN_DEUDA_PENDIENTE = 0 THEN tf.MN_PAGO_INICIAL ELSE tf.MN_DEUDA_PENDIENTE END <> 0
                AND tvc.CO_OCURRENCIA = ('FAC')
                AND (:BUSQUEDA IS NULL OR 
                     UPPER(torg.CO_RIF) LIKE '%' || UPPER(:BUSQUEDA) || '%' OR 
                     UPPER(torg.NB_ORGANIZACION) LIKE '%' || UPPER(:BUSQUEDA) || '%' OR 
                     UPPER(tf.CO_SERIE_FACTURA || tf.NU_FACTURA) LIKE '%' || UPPER(:BUSQUEDA) || '%' OR 
                     TO_CHAR(tf.FE_EMISION, 'DD/MM/YYYY') LIKE '%' || :BUSQUEDA || '%')
            UNION ALL
            SELECT
                toddc.ID_REGISTRO,
                te.NB_EMPRESA                                                                               EMPRESA,
                torg.CO_RIF                                                                                 RIF,
                torg.NB_ORGANIZACION                                                                        ORGANIZACION,
                torg.NU_TELEFONO_PRINCIPAL                                                                  TELEFONO,
                tug.NB_UBICACION                                                                            ZONA,
                tvc.CO_OCURRENCIA                                                                           OCURRENCIA,
                toddc.CO_SERIE_DOCUMENTO || toddc.NU_DOCUMENTO                                              DOCUMENTO,
                TO_CHAR(toddc.FE_EMISION, 'DD/MM/YYYY')                                                     EMISION,
                TO_CHAR(toddc.FE_VENCIMIENTO, 'DD/MM/YYYY')                                                 VENCIMIENTO,
                TRUNC(TO_DATE(:FECHA_CORTE, 'MM/DD/YYYY')) - TRUNC(toddc.FE_VENCIMIENTO)                    DIAS_VENCIDOS,
                :MONEDA_REPORTE                                                                             MONEDA,
                tum.CO_UNIDAD_MEDIDA                                                                        MONEDA_DOCUMENTO,
                CAST(CASE 
                    WHEN TO_DATE(:FECHA_CORTE, 'MM/DD/YYYY') > toddc.FE_VENCIMIENTO THEN 
                        CASE
                            WHEN tum.CO_UNIDAD_MEDIDA = 'BS' AND :MONEDA_REPORTE = 'USD' THEN toddc.MN_SALDO_PENDIENTE / NULLIF(toddc.FC_TASA_CAMBIO, 0)
                            WHEN tum.CO_UNIDAD_MEDIDA = 'USD' AND :MONEDA_REPORTE = 'BS' THEN toddc.MN_SALDO_PENDIENTE * toddc.FC_TASA_CAMBIO
                            ELSE toddc.MN_SALDO_PENDIENTE
                        END
                    ELSE 0 
                END AS NUMBER(19,4)) AS                                                                     VENCIDO,
                CAST(CASE 
                    WHEN TO_DATE(:FECHA_CORTE, 'MM/DD/YYYY') <= toddc.FE_VENCIMIENTO THEN 
                        CASE
                            WHEN tum.CO_UNIDAD_MEDIDA = 'BS' AND :MONEDA_REPORTE = 'USD' THEN toddc.MN_SALDO_PENDIENTE / NULLIF(toddc.FC_TASA_CAMBIO, 0)
                            WHEN tum.CO_UNIDAD_MEDIDA = 'USD' AND :MONEDA_REPORTE = 'BS' THEN toddc.MN_SALDO_PENDIENTE * toddc.FC_TASA_CAMBIO
                            ELSE toddc.MN_SALDO_PENDIENTE
                        END
                    ELSE 0 
                END AS NUMBER(19,4)) AS                                                                     POR_VENCER,
                CAST(CASE
                    WHEN tum.CO_UNIDAD_MEDIDA = 'BS' AND :MONEDA_REPORTE = 'USD' THEN toddc.MN_SALDO_PENDIENTE / NULLIF(toddc.FC_TASA_CAMBIO, 0)
                    WHEN tum.CO_UNIDAD_MEDIDA = 'USD' AND :MONEDA_REPORTE = 'BS' THEN toddc.MN_SALDO_PENDIENTE * toddc.FC_TASA_CAMBIO
                    ELSE toddc.MN_SALDO_PENDIENTE
                END AS NUMBER(19,4)) AS                                                                     TOTAL_DEUDA,
                toddc.FC_TASA_CAMBIO                                                                        TASA,
                ud.NB_PRODUCTO                                                                              PRODUCTO,
                ud.NU_SERIAL_PRODUCTO                                                                       SERIAL,
                ud.CO_GRUPO_PRODUCTO                                                                        GRUPO_PRODUCTO,
                toddc.FE_EMISION                                                                            AS FECHA_ORDEN_DATE
            FROM T06_OTRO_DOCUMENTO_DEB_CRED toddc 
                INNER JOIN T00_UNIDAD_MEDIDA tum ON tum.ID_REGISTRO = toddc.UNM_ID_UNIDAD_MONETARIA 
                INNER JOIN T00_EMPRESA te ON te.ID_REGISTRO = toddc.EMP_ID_EMPRESA
                INNER JOIN T00_ORGANIZACION torg ON torg.ID_REGISTRO = toddc.ORG_ID_ORGANIZACION 
                INNER JOIN T00_UBICACION_GEOGRAFICA tug ON tug.ID_REGISTRO = torg.UBG_ID_UBICACION
                INNER JOIN T00_CONTENIDO_TABLA_VIRTUAL tvc ON tvc.ID_REGISTRO = toddc.CTV_ID_TIPO_DOCUMENTO 
                LEFT JOIN DetalleProductos ud ON ud.FAC_ID_REGISTRO = toddc.ID_REGISTRO
            WHERE 
                toddc.MN_SALDO_PENDIENTE <> 0
                AND tvc.CO_OCURRENCIA = 'NDN'
                AND (:BUSQUEDA IS NULL OR 
                     UPPER(torg.CO_RIF) LIKE '%' || UPPER(:BUSQUEDA) || '%' OR 
                     UPPER(torg.NB_ORGANIZACION) LIKE '%' || UPPER(:BUSQUEDA) || '%' OR 
                     UPPER(toddc.CO_SERIE_DOCUMENTO || toddc.NU_DOCUMENTO) LIKE '%' || UPPER(:BUSQUEDA) || '%' OR 
                     TO_CHAR(toddc.FE_EMISION, 'DD/MM/YYYY') LIKE '%' || :BUSQUEDA || '%')
            ORDER BY EMPRESA ASC, RIF ASC, FECHA_ORDEN_DATE ASC
            """;
    }   
}