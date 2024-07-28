-- They are 8 Kb Pages, so 8 * 1024 or 8192 bytes
SELECT  OBJECT_SCHEMA_NAME(id) AS [Schema],
        OBJECT_NAME(id) AS [Table],
        [name] AS [Index],
        indid AS [IndexId],
        CONVERT(BIGINT, reserved * 8192.0) AS Reserved,
        CONVERT(BIGINT, used * 8192.0) AS Used
FROM    sys.sysindexes WITH(NOLOCK)
ORDER   BY OBJECT_SCHEMA_NAME(id) ASC,
        OBJECT_NAME(id) ASC,
        [name] ASC
