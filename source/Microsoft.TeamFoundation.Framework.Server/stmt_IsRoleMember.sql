-- Checks if the specified used (or current user if none is specified) is member of the provided role
-- Equivalent to
--  SELECT CONVERT(BIT, ISNULL(IS_MEMBER(@roleName), 0)) isMember, when @roleName is role and not a Windows Group
--  SELECT CONVERT(BIT, ISNULL(IS_ROLEMEMBER(@roleName [, @userName]), 0)) isMember
-- Assume recursive depth does not exceed the SQL default.
-- It works around the a problem in SQL when granting roles to a user that is currently logged in
--
-- Parameters:
--  @roleName SYSNAME containing the role to check
--  @userName SYSNAME containing the name of the user to check, optional when not provided the current user/login is checked

;WITH CTE_UsersRoles (RoleName) AS
(
    -- Seed our cte with the Role that this user has
    SELECT  r.name RoleName
    FROM    sys.database_principals r
    JOIN    sys.database_role_members rm
    ON      r.principal_id = rm.role_principal_id
    JOIN    sys.database_principals m
    ON      rm.member_principal_id = m.principal_id
            --
            -- If a user name is provided use that, if not we will attempt to match name against either login name or database user
    WHERE   (@userName IS NOT NULL AND (m.name = @userName))
            OR (@userName IS NULL AND (m.name = SUSER_NAME() OR m.name = USER_NAME()))
    --
    -- Recursively include any Role that their Role is a member of, stopping if we find specified role or run out of parent roles 
    UNION ALL
    SELECT  r.name RoleName
    FROM    sys.database_principals r
    JOIN    sys.database_role_members rm
    ON      r.principal_id = rm.role_principal_id
    JOIN    sys.database_principals m
    ON      rm.member_principal_id = m.principal_id
    JOIN    CTE_UsersRoles cte
    ON      m.name = cte.RoleName
    WHERE   m.name != @roleName  -- This where clause stops us from continuing down the recursive path after we found the role we are looking for
)
SELECT CONVERT(BIT, CASE
                        -- 
                        -- There is a bug in SQL where in come case granting a role to user  that is currently logged in does got get reflected in IS_ROLEMEMBER
                        -- to work around it we will check the sys tables directly
                        WHEN EXISTS(
                            SELECT TOP 1 1
                            FROM   CTE_UsersRoles
                            WHERE  RoleName = @roleName
                        ) THEN 1
                        --
                        -- There are special roles like public which are not represented in the sys tables, so we still need to call check IS_ROLEMEMBER
                        -- additional explicitly passing NULL or the result for USER_NAME() for users like vseqa1@microsoft.com will not work
                        WHEN @userName IS NULL THEN ISNULL(IS_ROLEMEMBER(@roleName), 0)
                        ELSE ISNULL(IS_ROLEMEMBER(@roleName, @userName), 0)
                    END) AS isMember