-- Returns the database files
-- Parameters:
--   @databaseName sysname

SELECT  sf.file_id,
        sf.type,
        sf.data_space_id,
        sf.name,
        sf.physical_name,
        sf.state,
        size,
        sf.max_size,
        sf.growth,
        sf.is_media_read_only,
        sf.is_read_only,
        sf.is_sparse,
        sf.is_percent_growth
FROM    sys.master_files sf
WHERE   sf.database_id = DB_ID(@databaseName)
ORDER BY sf.file_id