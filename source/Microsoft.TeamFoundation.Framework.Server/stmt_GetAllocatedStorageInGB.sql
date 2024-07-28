-- Get allocated storage in GB (round up) - this is not the size of the 
-- database but the least amount of size database can be resize to
-- without shrinking the file
SELECT TOP 1 CAST(CEILING(allocated_storage_in_megabytes/1024) AS INT) AS AllocatedStorageInGB
FROM   sys.resource_stats
WHERE  database_name = @databaseName
ORDER BY end_time DESC
