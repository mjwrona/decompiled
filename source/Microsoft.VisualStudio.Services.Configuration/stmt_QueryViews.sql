-- Returns all the views that match the optional name filter and optional modified after (which can be set to the creating time of the database)
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

SELECT  v.name,
        SCHEMA_NAME(v.schema_id) AS schema_name,
        v.modify_date,

        -- definition_length: items that have a view definition will have length > 0
        LEN(ISNULL(i.VIEW_DEFINITION, '')) AS definition_length,

        -- white_listed: items that include a specific comment are in the white list all others are not
        CASE
            WHEN
                (((SELECT  c.text
                   FROM    sys.syscomments c
                   WHERE   (c.id = v.object_id)
                   ORDER BY c.colid
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)')
                 ) LIKE N'%--EEPR WHITELISTED:%'
                ) THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS whitelisted
FROM    sys.views v
JOIN    INFORMATION_SCHEMA.VIEWS i
ON      (v.name = i.TABLE_NAME)
        AND (SCHEMA_NAME(v.schema_id) = i.TABLE_SCHEMA)
WHERE   ((@name_filter IS NULL) OR (v.name LIKE @name_filter))
        AND ((@modified_after IS NULL) OR (v.modify_date > @modified_after))
ORDER BY schema_name, name;