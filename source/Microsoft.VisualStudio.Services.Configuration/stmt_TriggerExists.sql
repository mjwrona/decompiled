SELECT  CONVERT(BIT, COUNT(*)) TriggerExists
FROM    sys.objects o
JOIN    sys.schemas s
ON      s.schema_id = o.schema_id
WHERE   o.type = 'TR' -- sql trigger
        AND o.name = @triggerName
        AND s.name = @schema