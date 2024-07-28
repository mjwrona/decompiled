SELECT  o.name AS Objective,
        do.state_desc AS DatabaseState
FROM    slo_database_objectives do
INNER JOIN    slo_service_objectives  o
ON      o.objective_id = do.current_objective_id
INNER JOIN  sys.databases d
ON      d.database_id = do.database_id
WHERE   d.name = @databaseName
