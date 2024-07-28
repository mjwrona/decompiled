SELECT  s.name  SchemaName,
        t.name  TableName,
        SUM(reserved_page_count) SizePages,
        COUNT(DISTINCT c.name) AS DataspaceColumnCount
FROM    sys.tables t
JOIN    sys.schemas s
ON      t.schema_id = s.schema_id
JOIN    sys.dm_db_partition_stats p
ON      p.object_id = t.object_id
LEFT JOIN sys.columns c
ON      c.object_id = t.object_id
        AND c.name LIKE '%Dataspace%'  -- Tells us the table is either already upgraded, or is set up for migration. If its set up for migration then we've previously pre-allocated file space for this table.
GROUP BY s.name, t.name
ORDER BY s.name, t.name
