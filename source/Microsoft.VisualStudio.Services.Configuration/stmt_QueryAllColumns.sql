SELECT  t.name table_name,
        s.name table_schema,
        t.is_ms_shipped is_system_table,
        OBJECTPROPERTY(t.object_id, N'TableTextInRowLimit') tableTextInRowLimit,
        c.name column_name,
        c.is_nullable,
        d.definition column_default,
        CONVERT(BIT, ISNULL(ic.is_not_for_replication, 0)) is_not_for_replication,
        c.is_replicated, c.is_ansi_padded, c.is_rowguidcol,
        st.name data_type, c.max_length, c.precision, c.scale, c.collation_name, 
        c.is_identity, CAST(ic.seed_value AS BIGINT) seed_value, CAST(ic.increment_value AS BIGINT) increment_value,
        c.is_computed, c.is_sparse, cc.definition computed_definition, cc.is_persisted, st.is_user_defined
FROM    sys.tables t
JOIN    sys.columns c
ON      t.object_id = c.object_id
JOIN    sys.schemas s
ON      t.schema_id = s.schema_id
LEFT JOIN sys.default_constraints d
ON      c.default_object_id = d.object_id
JOIN    sys.types st
ON      c.system_type_id = st.system_type_id
        AND c.user_type_id = st.user_type_id
LEFT JOIN sys.identity_columns ic
ON      c.column_id = ic.column_id
        AND c.object_id = ic.object_id
LEFT JOIN sys.computed_columns cc
ON      c.column_id = cc.column_id
        AND c.object_id = cc.object_id
WHERE   t.type = 'U'
ORDER BY table_schema, table_name, c.column_id