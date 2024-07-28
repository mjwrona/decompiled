-- sql azure does not support partitions
IF (CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT <> N'SQL Azure')
BEGIN
    SELECT  t.name table_name, s.name schema_name,
            i.name index_name, kc.is_system_named, c.name column_name,
            i.type index_type, i.is_unique, i.ignore_dup_key, i.is_primary_key, i.is_unique_constraint, i.fill_factor, i.is_padded,
            i.is_disabled, i.is_hypothetical, i.allow_row_locks, i.allow_page_locks, i.has_filter, i.filter_definition,
            ic.is_descending_key, ic.is_included_column,
            st.no_recompute, sp.PartitionCount, sp.DataCompression
    FROM    sys.indexes i
    JOIN    sys.tables t
    ON      i.object_id = t.object_id
    JOIN    sys.schemas s
    ON      t.schema_id = s.schema_id
    JOIN    sys.index_columns ic 
    ON      t.object_id = ic.object_id
            AND i.index_id = ic.index_id
    JOIN    sys.columns c
    ON      c.object_id = t.object_id
            AND c.column_id = ic.column_id
    LEFT OUTER JOIN sys.key_constraints kc
    ON      kc.name = i.name
            AND kc.parent_object_id = i.object_id
    JOIN    sys.stats st
    ON      st.name = i.name
            AND st.object_id = t.object_id
    CROSS APPLY (
            SELECT  COUNT(*) AS PartitionCount,
                    COALESCE(MAX(data_compression), CONVERT(TINYINT, 0)) AS DataCompression
            FROM    sys.partitions sp
            WHERE   sp.object_id = i.object_id 
                    AND sp.index_id = i.index_id
            ) AS sp
    WHERE   i.type > 0
            AND i.object_id = OBJECT_ID(@name)
    ORDER BY schema_name, table_name, is_primary_key DESC, index_name, key_ordinal
END
ELSE
BEGIN
    SELECT  t.name table_name, s.name schema_name,
            i.name index_name, kc.is_system_named, c.name column_name,
            i.type index_type, i.is_unique, i.ignore_dup_key, i.is_primary_key, i.is_unique_constraint, i.fill_factor, i.is_padded, 
            i.is_disabled, i.is_hypothetical, i.allow_row_locks, i.allow_page_locks, i.has_filter, i.filter_definition,
            ic.is_descending_key, ic.is_included_column,
            st.no_recompute, 1 AS PartitionCount, CONVERT(TINYINT, 0) AS DataCompression
    FROM    sys.indexes i
    JOIN    sys.tables t 
    ON      i.object_id = t.object_id
    JOIN    sys.schemas s
    ON      t.schema_id = s.schema_id
    JOIN    sys.index_columns ic 
    ON      t.object_id = ic.object_id 
            AND i.index_id = ic.index_id
    JOIN    sys.columns c
    ON      c.object_id = t.object_id 
            AND c.column_id = ic.column_id
    LEFT OUTER JOIN sys.key_constraints kc 
    ON      kc.name = i.name
            AND kc.parent_object_id = i.object_id
    JOIN    sys.stats st
    ON      st.name = i.name
            AND st.object_id = t.object_id
    WHERE   i.type > 0
            AND i.object_id = OBJECT_ID(@name)
    ORDER BY schema_name, table_name, is_primary_key DESC, index_name, key_ordinal
END
