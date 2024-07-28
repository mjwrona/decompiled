SELECT  name, 
        object_name(parent_object_id) table_name, 
        schema_name(schema_id) schema_name 
FROM    sys.foreign_keys 
WHERE   is_disabled = 0