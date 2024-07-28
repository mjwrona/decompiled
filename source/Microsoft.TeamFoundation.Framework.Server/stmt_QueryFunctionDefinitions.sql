-- Returns all the function definitions matching the name filter
-- Parameters:
--   @nameFilter - LIKE expression filter

SELECT  OBJECT_NAME(o.object_id) AS name,
        OBJECT_SCHEMA_NAME(o.object_id) AS schemaName,
        c.colid AS sectionId,
        CASE
            WHEN @includeContent = 1 THEN
                c.text
            ELSE NULL
        END AS text
FROM    sys.syscomments c
INNER JOIN sys.objects o
ON      c.id = o.object_id
WHERE   o.type IN ('FN', 'IF', 'TF')
AND     o.name like @nameFilter