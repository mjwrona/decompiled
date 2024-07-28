-- Check if collation is supported by TFS
-- TFS requires accent sensitive, not case sensitive, and not binary
SELECT CASE CONVERT(INT, COLLATIONPROPERTY(@collation, 'ComparisonStyle')) & 3 WHEN 1 THEN 1 END
