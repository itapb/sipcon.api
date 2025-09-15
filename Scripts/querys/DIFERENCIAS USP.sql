SELECT 
    ISNULL(S.ROUTINE_SCHEMA + '.' + S.ROUTINE_NAME, T.ROUTINE_SCHEMA + '.' + T.ROUTINE_NAME) AS ProcedureName,
    CASE 
        WHEN S.ROUTINE_NAME IS NULL THEN 'Solo en la base de datos de destino'
        WHEN T.ROUTINE_NAME IS NULL THEN 'Solo en la base de datos de origen'
        WHEN CHECKSUM(S.definition_cleaned) <> CHECKSUM(T.definition_cleaned) THEN 'Diferente'
        ELSE 'Igual'
    END AS Comparacion,
    S.ROUTINE_DEFINITION AS Definition_Origen,
    T.ROUTINE_DEFINITION AS Definition_Destino
FROM 
    (
        SELECT 
            ROUTINE_SCHEMA, 
            ROUTINE_NAME,
            ROUTINE_DEFINITION,
            SODA.dbo.NormalizeCode(ROUTINE_DEFINITION) AS definition_cleaned -- Llamamos a la función de normalización
        FROM SODA.INFORMATION_SCHEMA.ROUTINES
        WHERE ROUTINE_TYPE = 'PROCEDURE'
    ) AS S
FULL OUTER JOIN 
    (
        SELECT 
            ROUTINE_SCHEMA, 
            ROUTINE_NAME,
            ROUTINE_DEFINITION,
            OASIS.dbo.NormalizeCode(ROUTINE_DEFINITION) AS definition_cleaned -- Llamamos a la función de normalización
        FROM OASIS.INFORMATION_SCHEMA.ROUTINES
        WHERE ROUTINE_TYPE = 'PROCEDURE'
    ) AS T
    ON S.ROUTINE_NAME = T.ROUTINE_NAME AND S.ROUTINE_SCHEMA = T.ROUTINE_SCHEMA
WHERE 
    S.ROUTINE_NAME IS NULL 
    OR T.ROUTINE_NAME IS NULL 
    OR CHECKSUM(S.definition_cleaned) <> CHECKSUM(T.definition_cleaned)
ORDER BY 
    ISNULL(S.ROUTINE_SCHEMA + '.' + S.ROUTINE_NAME, T.ROUTINE_SCHEMA + '.' + T.ROUTINE_NAME);