-- Returns all the views definitions matching the name filter
-- Parameters:
--   @nameFilter - LIKE expression filter

SELECT  OBJECT_NAME(v.object_id) AS name,
        OBJECT_SCHEMA_NAME(v.object_id) AS schemaName,
        c.colid AS sectionId,
        CASE
            WHEN @includeContent = 1 THEN
                c.text
            ELSE NULL
        END AS text
FROM    sys.syscomments c
INNER JOIN sys.views v
ON      c.id = v.object_id
AND     v.name like @nameFilter