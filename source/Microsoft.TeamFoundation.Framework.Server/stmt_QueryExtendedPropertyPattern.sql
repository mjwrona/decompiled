-- Read all database extended attributes that meet the pattern defined
SELECT  ep.name, 
        ep.value 
FROM    sys.extended_properties ep
WHERE   ep.name LIKE @attributePattern
        AND ep.class = 0
