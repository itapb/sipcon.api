-- Consulta para comparar objetos entre dos bases de datos

-- Stored Procedures que difieren entre las bases de datos
SELECT 
    COALESCE(p1.name, p2.name) AS ProcedureName,
    CASE 
        WHEN p1.object_id IS NULL THEN 'Solo existe en ' + 'OASIS'
        WHEN p2.object_id IS NULL THEN 'Solo existe en ' + 'SODA'
        ELSE 'Definición diferente'
    END AS Diferencia,
    CASE 
        WHEN p1.object_id IS NULL THEN 'OASIS'
        ELSE 'SODA'
    END AS Origen
FROM 
    (SELECT name, object_id, OBJECT_DEFINITION(object_id) AS definition 
     FROM SODA.sys.procedures) p1
FULL OUTER JOIN 
    (SELECT name, object_id, OBJECT_DEFINITION(object_id) AS definition 
     FROM OASIS.sys.procedures) p2
    ON p1.name = p2.name
WHERE 
    p1.object_id IS NULL OR 
    p2.object_id IS NULL OR 
    REPLACE(REPLACE(p1.definition, ' ', ''), CHAR(13) + CHAR(10), '') <> 
    REPLACE(REPLACE(p2.definition, ' ', ''), CHAR(13) + CHAR(10), '')

UNION ALL

-- Vistas que difieren entre las bases de datos
SELECT 
    COALESCE(v1.name, v2.name) AS ViewName,
    CASE 
        WHEN v1.object_id IS NULL THEN 'Solo existe en ' + 'OASIS'
        WHEN v2.object_id IS NULL THEN 'Solo existe en ' + 'SODA'
        ELSE 'Definición diferente'
    END AS Diferencia,
    CASE 
        WHEN v1.object_id IS NULL THEN 'OASIS'
        ELSE 'SODA'
    END AS Origen
FROM 
    (SELECT name, object_id, OBJECT_DEFINITION(object_id) AS definition 
     FROm SODA.sys.views) v1
FULL OUTER JOIN 
    (SELECT name, object_id, OBJECT_DEFINITION(object_id) AS definition 
     FROM OASIS.sys.views) v2
    ON v1.name = v2.name
WHERE 
    v1.object_id IS NULL OR 
    v2.object_id IS NULL OR 
    REPLACE(REPLACE(v1.definition, ' ', ''), CHAR(13) + CHAR(10), '') <> 
    REPLACE(REPLACE(v2.definition, ' ', ''), CHAR(13) + CHAR(10), '');


