-- Returns all the functions that match the optional name filter and optional modified after (which can be set to the creating time of the database)
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

SELECT  o.name,
        SCHEMA_NAME(o.schema_id) AS schema_name,
        o.modify_date,

        -- definition_length: items that have a routine definition will have length > 0
        CONVERT(INT, LEN(ISNULL(r.ROUTINE_DEFINITION, ''))) AS definition_length,

        -- white_listed: items that include a specific comment are in the white list all others are not
        CASE
            WHEN
                (((SELECT  c.text
                   FROM    sys.syscomments c
                   WHERE   (c.id = o.object_id)
                   ORDER BY c.colid
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)')
                 ) LIKE N'%--EEPR WHITELISTED:%'
                ) THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS whitelisted,

        -- execution_statement: Returning either 
        ---   for Table Functions WITH parameters DECLARE @param1 TYPE1, @param2 TYPE2,...; SELECT * FROME name(@param1, @param2)
        ---   for Table Functions withOUT parameters  SELECT * FROM name()
        ---   for Non-Table Function EXEC name
        CASE
          WHEN (o.type IN(N'IF', N'TF')) THEN
            -- If there are no parameter then the COALESCE will eat the version with Declare and parameters if we have no parameters
            COALESCE(
              N'DECLARE '
              -- Using stuff to pull off the leading comma
              --- Building list used for the declare statement
              + STUFF( (SELECT  ',' + p.name + ' ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + '.' + QUOTENAME(TYPE_NAME(p.user_type_id))
                        FROM    sys.parameters p
                        JOIN    sys.types t
                        ON      p.user_type_id = t.user_type_id
                        WHERE   (p.object_id = o.object_id)
                                AND (p.is_output = 0)
                        FOR XML PATH(''), TYPE
                       ).value('.', 'NVARCHAR(MAX)')
                       ,1,1,'')
              + N'; SELECT * FROM ' + QUOTENAME(SCHEMA_NAME(o.schema_id)) + '.' + QUOTENAME(o.name) + '('
              --- Building list used to pass the parameters
              + STUFF( (SELECT  ',' + p.name
                        FROM    sys.parameters p
                        WHERE   (p.object_id = o.object_id)
                                AND (p.is_output = 0)
                        ORDER BY p.parameter_id
                        FOR XML PATH(''), TYPE
                       ).value('.', 'NVARCHAR(MAX)')
                      ,1,1,'')
             + ')'
             ,  -- If there are no parameters, then we don't need the Declare statement
             N'SELECT * FROM ' + QUOTENAME(SCHEMA_NAME(o.schema_id)) + '.' + QUOTENAME(o.name) + '()')
          -- For all the other function that are not type='IF' or type ='TF'
          ELSE QUOTENAME(SCHEMA_NAME(o.schema_id)) + '.' + QUOTENAME(o.name)
        END  As execution_statement
FROM    sys.objects o
JOIN    INFORMATION_SCHEMA.ROUTINES r
ON      (o.name = r.ROUTINE_NAME)
        AND (SCHEMA_NAME(o.schema_id) = r.ROUTINE_SCHEMA)
WHERE   (o.type IN(N'FN', N'IF', N'TF'))
        AND ((@name_filter IS NULL) OR (o.name LIKE @name_filter))
        AND ((@modified_after IS NULL) OR (o.modify_date > @modified_after))
ORDER BY schema_name, name;