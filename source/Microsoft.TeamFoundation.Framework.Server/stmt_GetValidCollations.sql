-- Select all collations supported by TFS
-- TFS requires accent sensitive, not case sensitive, and not binary
SELECT name
FROM fn_helpcollations()
WHERE name like '%CI_AS%'
