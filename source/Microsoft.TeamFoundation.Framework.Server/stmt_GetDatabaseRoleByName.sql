--
-- Queries the database role by name
--
-- Parameter:
--   @databaseRole sysname - The database role name to query    

SELECT  dp.name,
        dp.principal_id,
        DATEADD(second, DATEDIFF(second, GETDATE(), GETUTCDATE()), dp.create_date) AS utcCreateDate,
        DATEADD(second, DATEDIFF(second, GETDATE(), GETUTCDATE()), dp.modify_date) AS utcModifyDate,
        dp.sid,
        dp.is_fixed_role,
        op.name owner
FROM    sys.database_principals dp
JOIN    sys.database_principals op
ON      dp.owning_principal_id = op.principal_id
WHERE   dp.type = 'R'
        AND dp.name = @databaseRole