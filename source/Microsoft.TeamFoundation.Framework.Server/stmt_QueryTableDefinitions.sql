-- Returns all the table definitions matching the name filter, this always return empty content
-- Parameters:
--   @nameFilter - LIKE expression filter

SELECT  t.name AS name,
        s.name AS schemaName,
        CAST(1 AS SMALLINT) AS sectionId,   -- empty content
        NULL AS text                        -- empty content
FROM    sys.tables t
JOIN    sys.schemas s
on	    t.schema_id = s.schema_id
WHERE   t.type = 'U'
        AND (t.name LIKE @nameFilter)
