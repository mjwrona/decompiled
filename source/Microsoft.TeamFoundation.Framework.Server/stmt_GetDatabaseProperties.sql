SELECT  DATABASEPROPERTYEX(db_name(), 'ServiceObjective') AS Objective,
        DATABASEPROPERTYEX(db_name(), 'Edition') AS Edition,
        CONVERT(BIGINT, DATABASEPROPERTYEX(db_name(), 'MaxSizeInBytes')) / 1024 / 1024 AS MaxSizeInMB,
        CAST(SUM(FILEPROPERTY(name, 'SpaceUsed')  * 8.0 / 1024) AS BIGINT) AS SizeInMB
FROM    sys.database_files
WHERE   type_desc = 'ROWS'