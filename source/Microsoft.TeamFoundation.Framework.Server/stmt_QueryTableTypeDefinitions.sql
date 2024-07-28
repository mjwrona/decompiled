-- Returns all the type table definitions matching the name filter, this always return empty content
-- Parameters:
--   @nameFilter - LIKE expression filter

SELECT  t.name AS name,
        s.name AS schemaName,
        CAST(1 AS SMALLINT) AS sectionId,       -- empty content
        NULL AS text          -- empty content
FROM    sys.objects o
JOIN    sys.table_types t
ON      t.type_table_object_id = o.object_id
JOIN    sys.schemas s
on	    t.schema_id = s.schema_id
WHERE   o.type = 'TT'
        AND (t.name LIKE @nameFilter)
