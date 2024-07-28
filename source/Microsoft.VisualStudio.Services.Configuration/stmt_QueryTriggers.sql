SELECT  tr.name, 
        s.name
FROM    sys.triggers tr
JOIN    sys.tables t 
ON      t.object_id = tr.parent_id
JOIN    sys.schemas s 
ON      s.schema_id = t.schema_id
