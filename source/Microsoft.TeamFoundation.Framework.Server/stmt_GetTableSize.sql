SELECT  t.name  TableName,
        s.name  SchemaName,
        SUM(CASE 
                WHEN p.index_id <= 1 THEN row_count
                ELSE 0
            END) Row_Count,
        SUM(reserved_page_count) * 8192 SizeInBytes
        FROM    sys.tables t
        JOIN    sys.schemas s
        ON      t.schema_id = s.schema_id
        JOIN    sys.dm_db_partition_stats p
        ON      p.object_id = t.object_id 
WHERE   s.name = @schemaName
        AND t.name = @tableName
GROUP BY t.name, s.name
