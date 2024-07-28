-- Returns all the stored procedure definitions matching the name filter
-- Parameters:
--   @nameFilter - LIKE expression filter

SELECT  OBJECT_NAME(p.object_id) AS name,
        OBJECT_SCHEMA_NAME(p.object_id) AS schemaName,
        c.colid AS sectionId,
        CASE
            WHEN @includeContent = 1 THEN
                c.text
            ELSE NULL
        END AS text
FROM    sys.syscomments c
INNER JOIN sys.procedures p
ON      c.id = p.object_id
AND     p.name like @nameFilter