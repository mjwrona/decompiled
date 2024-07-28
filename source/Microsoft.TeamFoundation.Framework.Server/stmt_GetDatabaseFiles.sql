-- Returns the database files

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
        sf.is_percent_growth,
        fg.name AS file_group_name,
        fg.is_default AS in_default_file_group
FROM    sys.database_files sf
LEFT JOIN sys.filegroups fg    -- Left join to get back the log files as well as the ROWS files.
ON      fg.data_space_id = sf.data_space_id
ORDER BY sf.file_id