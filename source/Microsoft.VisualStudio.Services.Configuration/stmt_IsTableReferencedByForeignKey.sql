-- Checks if the table specified by @tableName parameter is referenced by a foreign key
-- Parameters:
--  @tableName
SELECT  CONVERT(BIT, COUNT(*)) hasForeignKeys
FROM    sys.foreign_keys
WHERE   referenced_object_id = OBJECT_ID(@tableName, 'U')