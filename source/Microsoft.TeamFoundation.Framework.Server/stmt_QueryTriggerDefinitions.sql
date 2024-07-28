-- Returns all the trigger definitions matching the name filter
-- Parameters:
--   @nameFilter - LIKE expression filter

SELECT  OBJECT_NAME(tr.object_id) AS name,
        OBJECT_SCHEMA_NAME(tr.object_id) AS schemaName,
        c.colid AS sectionId,
        CASE
            WHEN @includeContent = 1 THEN
                c.text
            ELSE NULL
        END AS text
FROM    sys.syscomments c
INNER JOIN sys.triggers tr
ON      c.id = tr.object_id
AND     tr.name like @nameFilter