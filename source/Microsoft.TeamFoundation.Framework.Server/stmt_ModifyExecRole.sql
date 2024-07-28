print 'ModifyExecRole'

--
-- Add to exec role - expects the db version of the account
-- 
DECLARE @statement          NVARCHAR(4000)
DECLARE @return             INT

-- see AccountsActivity.cs for matching cs enums

-- Account @operation --
-- 1 = add
-- 2 = remove 


/*
** If @account is an empty string then use current login.
*/
IF (@account = '')
BEGIN
    SET @account = SUSER_SNAME(SUSER_SID())
END

print 'account=' + @account
print 'operation=' + cast(@operation as varchar)

-- Accounts results -- 
-- all accounts operations are executescalar with an int return
-- these results are stable across all accounts operations
-- 0 = success
-- 1 = fail
-- 2 = noop (for example, add but already there, delete but not there)
-- 3 = skipped (for example, skip delete if dbo)

DECLARE @accountsResult         INT
DECLARE @userName SYSNAME
SET @accountsResult = 1

-- Warning: if @account is null, QUOTENAME will return null, @statement will be null and sp_executesql will return success! 
IF (@account IS NULL)
BEGIN
    RAISERROR (800032, 16, -1, 'ModifyExecRole', '@account')
    RETURN
END

-- create the role if it doesn't exist
-- useful in certain DR and clone scenarios

IF NOT EXISTS ( SELECT  * 
                FROM    sys.database_principals 
                WHERE   NAME = @roleName )
BEGIN
    SELECT @statement = 'CREATE ROLE ' + QUOTENAME(@roleName)
    EXEC sp_executesql @statement
END

--
-- do the operation now
--
IF @operation = 1
BEGIN
    ------------
    --  ADD   --
    ------------

    --
    -- ensure the login exists
    --
    print 'Adding ...';
    print 'checking if login exists';
    -- Add login if it doesn't exist. 
    IF (SUSER_ID(@account) IS NULL) 
    BEGIN 
        print @account + ' is not a login. creating login.';
        SELECT @statement = 'CREATE LOGIN '+ QUOTENAME(@account) +' FROM WINDOWS'
        
        EXEC @return = sp_executesql @statement
        IF (@return <> 0)
        BEGIN 
            SELECT 1;
            RETURN; --error
        END
        print 'created login.'; 
    END
    --
    -- if dbo return skipped
    --
    print 'checking if dbo';
    IF EXISTS 
    (
        SELECT  *
        FROM    sys.database_principals
        WHERE   principal_id = 1 -- dbo
                AND sid = SUSER_SID(@account)
    ) 
    BEGIN
        print @account + ' is dbo. skipping add';
        SELECT 3; -- skipped (dbo)
        RETURN;
    END

    --
    -- if already in role, no op
    --
    print 'checking if already in role';
    SELECT  @userName = memberp.name
    FROM    sys.database_principals rolep
    JOIN    sys.database_role_members drm
    ON      drm.role_principal_id = rolep.principal_id
    JOIN    sys.database_principals memberp
    ON      memberp.principal_id = drm.member_principal_id
    WHERE   rolep.type = 'R' AND rolep.name = @roleName
            AND memberp.sid = SUSER_SID(@account)
    IF (@userName IS NOT NULL)
    BEGIN    
        print @account + ' is in the role. noop';
        print 'Setting default schema to dbo'
        -- Make sure that default_schema for this user is dbo
        SELECT @statement = 'ALTER USER ' + QUOTENAME(@userName) + ' WITH DEFAULT_SCHEMA=dbo'
        EXEC @return = sp_executesql @statement
        IF (@return <> 0)
        BEGIN 
            print 'Failed to set default schema'
            SELECT 1;
            RETURN; --error
        END

        SELECT 2; -- noop (in role already)
        RETURN;
    END

    -- query user for the specified login    
    SELECT  @userName = dp.name
    FROM    sys.database_principals dp
    JOIN    sys.syslogins l
    ON      dp.sid = l.sid
    WHERE   dp.type = 'U'
            AND l.name =  @account

    -- Create a user for this login if it does not exist
    IF (@userName IS NULL)
    BEGIN
        SET @userName = @account
        -- Before creating a user, let us check that user with the same name does not exist.
        -- if it exists, we need to drop it provided that it is not mapped to other logins.
        -- We already verified that login is not mapped to any users in this database.
        -- If [domain-name\user-name] user exists than we are in scenario when someone created a local user, created SQL login for that windows user
        -- and mapped a login to the database user.
        -- After that customer deleted an NT local user, dropped login. 
        -- Later customer recreated NT local user and login. SID of login changed but SID of the database use did not.
        
        IF ( EXISTS(SELECT  *
                    FROM    sys.database_principals p
                    LEFT JOIN sys.syslogins l
                    ON      p.sid = l.sid
                    WHERE   p.name = @userName
                            AND l.sid IS NULL) )
        BEGIN
            -- We cannot drop user if it owns any roles.
            -- Transfer role ownership to the dbo user.
            DECLARE @owningRole SYSNAME
            DECLARE owningRolesCursor CURSOR FOR
            SELECT  name
            FROM    sys.database_principals dp
            WHERE   dp.owning_principal_id = USER_ID(@userName)

            OPEN owningRolesCursor

            FETCH NEXT FROM owningRolesCursor
            INTO @owningRole

            WHILE @@FETCH_STATUS = 0
            BEGIN
                DECLARE @stmt NVARCHAR(1000)
                PRINT 'Transfering ownership of ' + @owningRole + ' role to dbo.' 
                SET @stmt = 'ALTER AUTHORIZATION ON ROLE::' + QUOTENAME(@owningRole) + ' TO dbo'
                EXEC(@stmt)
                SET @return = @@ERROR
                IF (@return <> 0)
                BEGIN 
                    CLOSE owningRolesCursor
                    DEALLOCATE owningRolesCursor

                    SELECT 1;
                    RETURN; --error 
                END

                FETCH NEXT FROM owningRolesCursor
                INTO    @owningRole
            END
            CLOSE owningRolesCursor
            DEALLOCATE owningRolesCursor
        
            PRINT 'Dropping user: ' + @userName
            SET @statement = N'DROP USER ' + QUOTENAME(@userName)
            EXEC(@statement)
            SET @return = @@ERROR
            IF (@return <> 0)
            BEGIN 
                SELECT 1;
                RETURN; --error 
            END            
        END     
        
        PRINT 'Creating user: ' + @account 
        SET @statement = 'CREATE USER ' + QUOTENAME(@userName) + ' FOR LOGIN ' + QUOTENAME(@account)
        EXEC(@statement)
        SET @return = @@ERROR
        IF (@return <> 0)
        BEGIN 
            SELECT 1;
            RETURN; --error 
        END            
    END        
    -- Add the account to the role.
    print 'adding to the role...';
    EXEC @return = sp_addrolemember @roleName, @userName
    IF (@return <> 0)
    BEGIN 
        SELECT 1;
        RETURN; --error 
    END    
    print @account + ' added to ' + @roleName;
    
    print 'Setting default schema to dbo'
    -- Make sure that default_schema for this user is dbo
    SELECT @statement = 'ALTER USER ' + QUOTENAME(@userName) + ' WITH DEFAULT_SCHEMA=dbo'
    EXEC @return = sp_executesql @statement
    IF (@return <> 0)
    BEGIN 
        print 'Failed to set default schema'
        SELECT 1;
        RETURN; --error
    END
    
    SELECT 0; --success
    RETURN;
END -- add
ELSE IF @operation = 2
BEGIN
    ------------
    -- REMOVE --
    ------------
    
    --
    -- if not in role, no op
    --
    print 'checking if already in role';
    IF NOT EXISTS 
    (
        SELECT  *
        FROM    sys.database_principals rolep
        JOIN    sys.database_role_members drm
        ON      drm.role_principal_id = rolep.principal_id
        JOIN    sys.database_principals memberp
        ON      memberp.principal_id = drm.member_principal_id
        WHERE   rolep.type = 'R' AND rolep.name = @roleName
                AND memberp.name = @account
    ) 
    BEGIN    
        print @account + ' is not in the role. noop';
        SELECT 2; -- noop (in role already)
        RETURN;
    END
    
    -- remove the account from the role
    print 'removing from the role';
    EXEC @return = sp_droprolemember @roleName, @account
    IF (@return <> 0)
    BEGIN 
        SELECT 3;
        RETURN; --error 
    END    
    print @account + ' dropped from ' + @roleName;
    
    SELECT 0; --success
    RETURN;    
END

-- error if reached here
SELECT 1;
RETURN; --error 