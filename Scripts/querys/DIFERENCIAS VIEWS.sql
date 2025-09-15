SELECT 
    ISNULL(S.TABLE_SCHEMA + '.' + S.TABLE_NAME, T.TABLE_SCHEMA + '.' + T.TABLE_NAME) AS ViewName,
    CASE 
        WHEN S.TABLE_NAME IS NULL THEN 'Solo en la base de datos de destino'
        WHEN T.TABLE_NAME IS NULL THEN 'Solo en la base de datos de origen'
        WHEN CHECKSUM(S.definition_cleaned) <> CHECKSUM(T.definition_cleaned) THEN 'Diferente'
        ELSE 'Igual'
    END AS Comparacion,
    S.VIEW_DEFINITION AS Definition_Origen,
    T.VIEW_DEFINITION AS Definition_Destino
FROM 
    (
        SELECT 
            TABLE_SCHEMA, 
            TABLE_NAME,
            VIEW_DEFINITION,
            SODA.dbo.NormalizeCode(VIEW_DEFINITION) AS definition_cleaned
        FROM SODA.INFORMATION_SCHEMA.VIEWS
    ) AS S
FULL OUTER JOIN 
    (
        SELECT 
            TABLE_SCHEMA, 
            TABLE_NAME,
            VIEW_DEFINITION,
            OASIS.dbo.NormalizeCode(VIEW_DEFINITION) AS definition_cleaned
        FROM OASIS.INFORMATION_SCHEMA.VIEWS
    ) AS T
    ON S.TABLE_NAME = T.TABLE_NAME AND S.TABLE_SCHEMA = T.TABLE_SCHEMA
WHERE 
    S.TABLE_NAME IS NULL 
    OR T.TABLE_NAME IS NULL 
    OR CHECKSUM(S.definition_cleaned) <> CHECKSUM(T.definition_cleaned)
ORDER BY 
    ISNULL(S.TABLE_SCHEMA + '.' + S.TABLE_NAME, T.TABLE_SCHEMA + '.' + T.TABLE_NAME);