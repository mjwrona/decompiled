-- Returns all the stored procedures that match the optional name filter and optional modified after (which can be set to the creating time of the database)
-- Parameters:
--  @name_filter
--  @modified_after
--  @use_DB_creation

IF (@use_DB_creation = 1)
BEGIN
    SELECT  @modified_after = create_date
    FROM    sys.databases
    WHERE   database_id = DB_ID()
END


SELECT  p.name,
        SCHEMA_NAME(p.schema_id) AS schema_name,
        p.modify_date,

        -- definition_length: items that have a routine definition will have length > 0
        CONVERT(INT, LEN(ISNULL(r.ROUTINE_DEFINITION, ''))) AS definition_length,

        -- white_listed: items that include a specific comment are in the white list all others are not
        CASE
            WHEN
                (((SELECT  c.text
                   FROM    sys.syscomments c
                   WHERE   (c.id = p.object_id)
                   ORDER BY c.colid
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)')
                 ) LIKE N'%--EEPR WHITELISTED:%'
                ) THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS whitelisted
FROM    sys.procedures p 
JOIN    INFORMATION_SCHEMA.ROUTINES r
ON      (p.name = r.ROUTINE_NAME)
        AND (SCHEMA_NAME(p.schema_id) = r.ROUTINE_SCHEMA)
WHERE   ((@name_filter IS NULL) OR (p.name LIKE @name_filter))
        AND ((@modified_after IS NULL) OR (p.modify_date > @modified_after))
ORDER BY schema_name, name;