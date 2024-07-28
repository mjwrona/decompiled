--
-- Queries the grantors of a permission
--
-- Parameters:
--   @grantee
--   @permission
--

SELECT  gtor.name AS grantor
FROM    sys.server_permissions p
JOIN    sys.server_principals gtor
ON      p.grantor_principal_id = gtor.principal_id
JOIN    sys.server_principals gtee
ON      p.grantee_principal_id = gtee.principal_id
WHERE   p.permission_name = @permission
        AND gtee.name = @grantee
