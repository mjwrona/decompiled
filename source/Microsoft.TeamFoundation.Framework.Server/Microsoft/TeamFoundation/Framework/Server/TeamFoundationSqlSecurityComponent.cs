// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationSqlSecurityComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationSqlSecurityComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<TeamFoundationSqlSecurityComponent>(0, true)
    }, "SqlSecurity");
    private const int c_serverPrincipalAlreadyExistRetries = 10;
    private const string c_stmtAlterAuthorizationFormat = "ALTER AUTHORIZATION ON {0}::{1} TO {2}";
    private const string c_stmtAlterSqlLoginFormat = "ALTER LOGIN {0} WITH password = {1}";
    private const string c_stmtCreateSqlLoginFormat = "CREATE LOGIN {0} WITH password = {1}";
    private const string c_stmtCreateSqlLoginForSidFormat = "CREATE LOGIN {0} WITH password = {1}, SID = {2}";
    private const string c_stmtDisableSqlLoginFormat = "ALTER LOGIN {0} DISABLE";
    private const string c_stmtDropDatabaseUserFormat = "DROP USER {0}";
    private const string c_stmtCreateUserFromAADUser = "CREATE USER [{0}] WITH SID={1}, TYPE=E";
    private const string c_stmtCreateDatabaseRoleFormat = "CREATE ROLE {0}";
    private const string c_stmtAlterUserFormat = "ALTER USER {0} WITH LOGIN = {1}";
    private const string c_stmtCreateUserFormat = "CREATE USER {0} FOR LOGIN {1} WITH DEFAULT_SCHEMA=dbo";
    private const string c_stmtHasPermissionByName = "SELECT CONVERT(BIT, HAS_PERMS_BY_NAME(@securable, @securableClass, @permission))";
    private const string c_stmtIsServerRoleMember = "SELECT CONVERT(BIT,  ISNULL(IS_SRVROLEMEMBER(@roleName), 0)) isMember";
    private const string c_stmtGetSid = "SELECT sid FROM sys.sysusers WHERE name = @userName";
    private const string c_stmtRevokePermission = "REVOKE {0} TO {1} {2} {3}";
    private const string c_stmtGrantExecuteOnObject = "IF(OBJECT_ID('{0}', 'P') IS NOT NULL)\r\nBEGIN\r\nGRANT EXECUTE ON {0} TO {1}\r\nEND";
    private const string c_testGrantOptionForPermission = "BEGIN TRAN\r\nGRANT {0} TO public\r\nROLLBACK";
    private const string c_checkPolicyOff = ", CHECK_POLICY = OFF";

    protected override ITFLogger GetDefaultLogger() => (ITFLogger) new ServerTraceLogger();

    public void AddRoleMember(string roleName, string memberName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(roleName, nameof (roleName));
      ArgumentUtility.CheckStringForNullOrEmpty(memberName, nameof (memberName));
      this.Logger.Info("Adding {0} to the following role: {1}", (object) memberName, (object) roleName);
      this.PrepareStoredProcedure("sys.sp_addrolemember");
      this.BindSysname("@rolename", roleName, false);
      this.BindSysname("@membername", memberName, false);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public void DropRoleMember(string roleName, string memberName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(roleName, nameof (roleName));
      ArgumentUtility.CheckStringForNullOrEmpty(memberName, nameof (memberName));
      this.Logger.Info("Droping {0} for following role: {1}", (object) memberName, (object) roleName);
      this.PrepareStoredProcedure("sys.sp_droprolemember");
      this.BindSysname("@rolename", roleName, false);
      this.BindSysname("@membername", memberName, false);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public void AlterRoleAuthorization(string roleName, string principalName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(roleName, nameof (roleName));
      ArgumentUtility.CheckStringForNullOrEmpty(principalName, nameof (principalName));
      this.AlterAuthorization("ROLE", roleName, principalName);
    }

    public void AlterSqlLoginPassword(string loginName, string password)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(loginName, nameof (loginName));
      ArgumentUtility.CheckStringForNullOrEmpty(password, nameof (password));
      this.VerifyInMasterDbOnAzure();
      this.Logger.Info("Changing password for the following login: {0}", (object) loginName);
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER LOGIN {0} WITH password = {1}", (object) StringUtil.QuoteName(loginName), (object) StringUtil.QuoteName(password, '\''));
      if (!this.IsSqlAzure)
        sqlStatement += ", CHECK_POLICY = OFF";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
      SqlConnectionInfoFactory.RemoveSqlCredentialFromCache(this.DataSource, loginName);
      this.Logger.Info("Success");
    }

    public void DisableSqlLogin(string loginName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(loginName, nameof (loginName));
      this.VerifyInMasterDbOnAzure();
      this.Logger.Info("Disabling the following sql login: {0}", (object) loginName);
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER LOGIN {0} DISABLE", (object) StringUtil.QuoteName(loginName));
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public void DropDatabaseUser(string userName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(userName, nameof (userName));
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DROP USER {0}", (object) StringUtil.QuoteName(userName));
      this.Logger.Info("Dropping database user: {0}. Database name: {1}. Data source: {2}.", (object) userName, (object) this.Database, (object) this.DataSource);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public void DropLogin(string loginName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(loginName, nameof (loginName));
      this.VerifyInMasterDbOnAzure();
      this.Logger.Info("Dropping sql login: {0}. Data source: {1}", (object) loginName, (object) this.DataSource);
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_DropLogin.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@loginName", loginName, false);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public SqlLoginInfo CreateNetworkServiceLogin()
    {
      this.VerifyNotSqlAzure();
      this.Logger.Info("Creating a login for the network service account.");
      SqlLoginInfo networkServiceLogin = this.ExecuteEmbeddedSqlLoginStatement("stmt_CreateNetworkServiceLogin.sql").FirstOrDefault<SqlLoginInfo>();
      if (networkServiceLogin != null)
        this.Logger.Info("Login created. Login name: {0}.", (object) networkServiceLogin.LoginName);
      else
        this.Logger.Info("login is null.");
      return networkServiceLogin;
    }

    public void ProvisionTfsRoles()
    {
      this.Logger.Info("Provisioning roles.");
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_ProvisionTfsRoles.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public void ProvisionTfsRolesOnprem()
    {
      this.Logger.Info("Provisioning roles.");
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_ProvisionTfsRolesOnprem.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public void ProvisionVsoDiagRole()
    {
      this.Logger.Info("Provisioning VsoDiagRole.");
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_ProvisionVsoDiagRole.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public void ChangeDatabaseOwner(string databaseName, string loginName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      ArgumentUtility.CheckStringForNullOrEmpty(loginName, nameof (loginName));
      this.AlterAuthorization("DATABASE", databaseName, loginName);
    }

    public void AlterAuthorizationToDbo()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_AlterAuthorizationToDbo.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.ExecuteNonQuery();
    }

    public void CreateDatabaseRole(string roleName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(roleName, nameof (roleName));
      this.Logger.Info("Creating database role: {0}. Data Source: {1}. Database Name: {2}", (object) roleName, (object) this.DataSource, (object) this.Database);
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CREATE ROLE {0}", (object) roleName);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public SqlLoginInfo CreateSqlAuthLogin(string loginName, string password)
    {
      try
      {
        this.TraceEnter(67001, nameof (CreateSqlAuthLogin));
        ArgumentUtility.CheckStringForNullOrEmpty(loginName, nameof (loginName));
        ArgumentUtility.CheckStringForNullOrEmpty(password, nameof (password));
        this.VerifyInMasterDbOnAzure();
        this.Logger.Info("Creating sql authentication login: {0}. Data Source: {1}", (object) loginName, (object) this.DataSource);
        string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CREATE LOGIN {0} WITH password = {1}", (object) StringUtil.QuoteName(loginName), (object) StringUtil.QuoteName(password, '\''));
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CREATE LOGIN {0} WITH password = {1}", (object) StringUtil.QuoteName(loginName), (object) StringUtil.QuoteName("passwordRemovedForTracing", '\''));
        if (!this.IsSqlAzure)
        {
          sqlStatement += ", CHECK_POLICY = OFF";
          str += ", CHECK_POLICY = OFF";
        }
        int num = 0;
        while (true)
        {
          this.PrepareSqlBatch(sqlStatement.Length);
          this.AddStatement(sqlStatement);
          this.Trace(67005, TraceLevel.Info, "CreateSqlAuthLogin: Executing sql statement {0}", (object) str);
          try
          {
            this.ExecuteNonQuery();
            break;
          }
          catch (SqlException ex)
          {
            this.TraceException(67012, (Exception) ex);
            if (num < 10 && ex.Errors.Count == 1 && ex.Errors[0].Number == 15025 && this.IsSqlAzure && this.GetLogin(loginName) == null)
            {
              this.Logger.Warning("CREATE LOGIN returned 15025, but login does not exist. retrying");
              this.Trace(67013, TraceLevel.Info, "CREATE LOGIN returned 15025, but login does not exist. retrying");
            }
            else
              throw;
          }
          ++num;
        }
        this.Trace(67006, TraceLevel.Info, "CreateSqlAuthLogin: Successfully executed sql statement {0}", (object) str);
        SqlLoginInfo login = this.GetLogin(loginName);
        if (login == null)
        {
          this.Trace(67007, TraceLevel.Error, "CreateSqlAuthLogin: No SQL login found for {0}", (object) loginName);
          this.Logger.Error("CreateSqlAuthLogin: No SQL login found for {0}", (object) loginName);
        }
        else
        {
          this.Trace(67008, TraceLevel.Info, "CreateSqlAuthLogin: returning SQL Login: {0}", (object) login);
          this.Logger.Info("Success");
        }
        return login;
      }
      catch (Exception ex)
      {
        this.TraceException(67009, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(67010, nameof (CreateSqlAuthLogin));
      }
    }

    public SqlLoginInfo CreateSqlAuthLoginForSid(string loginName, string password, byte[] sid)
    {
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(loginName, nameof (loginName));
        ArgumentUtility.CheckStringForNullOrEmpty(password, nameof (password));
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) sid, nameof (sid));
        this.VerifyInMasterDbOnAzure();
        string hex = StringUtil.ConvertToHex(sid);
        this.Logger.Info("Creating sql authentication login '" + loginName + "' with SID " + hex + ". Data Source: " + this.DataSource);
        string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CREATE LOGIN {0} WITH password = {1}, SID = {2}", (object) StringUtil.QuoteName(loginName), (object) StringUtil.QuoteName(password, '\''), (object) hex);
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CREATE LOGIN {0} WITH password = {1}, SID = {2}", (object) StringUtil.QuoteName(loginName), (object) StringUtil.QuoteName("passwordRemovedForTracing", '\''), (object) hex);
        if (!this.IsSqlAzure)
        {
          sqlStatement += ", CHECK_POLICY = OFF";
          str += ", CHECK_POLICY = OFF";
        }
        for (int index = 0; index < 10; ++index)
        {
          this.PrepareSqlBatch(sqlStatement.Length);
          this.AddStatement(sqlStatement);
          try
          {
            this.ExecuteNonQuery();
          }
          catch (SqlException ex)
          {
            if (index < 10 && ex.Errors.Count == 1 && ex.Errors[0].Number == 15025 && this.IsSqlAzure && this.GetLogin(loginName) == null)
            {
              this.Logger.Warning("CREATE LOGIN returned 15025, but login does not exist. retrying");
              continue;
            }
            throw;
          }
          this.Logger.Info("CreateSqlAuthLogin: Successfully executed sql statement {0}", (object) str);
          SqlLoginInfo login = this.GetLogin(loginName);
          if (login == null)
            this.Logger.Error("CreateSqlAuthLoginForSID: No SQL login found for {0}", (object) loginName);
          else
            this.Logger.Info("Success");
          return login;
        }
        throw new ApplicationException(string.Format("CreateSqlAuthLoginForSID for user {0} retries exhausted", (object) loginName));
      }
      catch (Exception ex)
      {
        this.Logger.Error(ex);
        throw;
      }
    }

    public string CreateUser(string loginName, bool isAADUser = false, string aadGuid = "")
    {
      ArgumentUtility.CheckStringForNullOrEmpty(loginName, nameof (loginName));
      this.Logger.Info("Creating user for the following login: {0}. Data Source: {1}. Database Name: {2}.", (object) loginName, (object) this.DataSource, (object) this.Database);
      int num = 0;
      string text;
      while (true)
      {
        try
        {
          text = loginName;
          if (num > 0)
            text += num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          this.Logger.Info("User name: {0}.", (object) text);
          string sqlStatement = !isAADUser || string.IsNullOrEmpty(aadGuid) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CREATE USER {0} FOR LOGIN {1} WITH DEFAULT_SCHEMA=dbo", (object) StringUtil.QuoteName(text), (object) StringUtil.QuoteName(loginName)) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CREATE USER [{0}] WITH SID={1}, TYPE=E", (object) loginName, (object) aadGuid);
          this.PrepareSqlBatch(sqlStatement.Length);
          this.AddStatement(sqlStatement);
          this.ExecuteNonQuery();
          break;
        }
        catch (SqlException ex)
        {
          if (num >= 100)
            throw;
          else if (ex.Errors.Cast<SqlError>().Any<SqlError>((System.Func<SqlError, bool>) (err => err.Number == 15023)))
          {
            this.Logger.Info("Specified user already exists.");
          }
          else
          {
            if (ex.Errors.Cast<SqlError>().Any<SqlError>((System.Func<SqlError, bool>) (err => err.Number == 15063)))
            {
              this.Logger.Info("The login already has an account under a different user name.");
              SqlLoginInfo login;
              using (TeamFoundationSqlSecurityComponent componentRaw = this.ConnectionInfo.CloneReplaceInitialCatalog(TeamFoundationSqlResourceComponent.Master).CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: this.Logger))
                login = componentRaw.GetLogin(loginName);
              text = this.GetDatabaseUserBySid(login.Sid).Name;
              break;
            }
            throw;
          }
        }
        ++num;
      }
      this.Logger.Info("Success");
      return text;
    }

    public SqlLoginInfo CreateWindowsAuthLogin(string loginName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(loginName, nameof (loginName));
      this.VerifyNotSqlAzure();
      this.Logger.Info("Creating the following windows authentication login: {0}. Data Source: {1}", (object) loginName, (object) this.DataSource);
      SqlLoginInfo windowsAuthLogin = this.ExecuteEmbeddedSqlLoginStatement("stmt_CreateWindowsAuthLogin.sql", "@loginName", loginName).FirstOrDefault<SqlLoginInfo>();
      this.Logger.Info(windowsAuthLogin != null ? "Success" : "Failed");
      return windowsAuthLogin;
    }

    public void EnsureRoleExists(string roleName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(roleName, nameof (roleName));
      this.Logger.Info("Ensuring that the following database role exists: {0}", (object) roleName);
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_EnsureRoleExists.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@roleName", roleName, false);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public byte[] GetDboLoginSid(string databaseName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      this.VerifyInMasterDbOnAzure();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDatabaseDboLoginSid.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@databaseName", databaseName, false);
      return (byte[]) this.ExecuteScalar();
    }

    public SqlDatabaseRole GetDatabaseRole(string databaseRole)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseRole, nameof (databaseRole));
      return this.ExecuteEmbeddedDatabaseRoleStatement("stmt_GetDatabaseRoleByName.sql", "@databaseRole", databaseRole).FirstOrDefault<SqlDatabaseRole>();
    }

    public List<SqlDatabaseRole> GetDatabaseRoles() => this.ExecuteEmbeddedDatabaseRoleStatement("stmt_GetDatabaseRoles.sql");

    public SqlDatabaseUser GetDatabaseUserBySid(byte[] sid)
    {
      this.Logger.Info("Getting database user by SID");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) sid, nameof (sid));
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDatabaseUserBySid.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindBinary("@sid", sid, SqlDbType.VarBinary);
      return this.ExecuteDatabaseUserQuery("stmt_GetDatabaseUserBySid.sql").FirstOrDefault<SqlDatabaseUser>();
    }

    public string GetDatabaseUserNameBySid(byte[] sid) => this.GetDatabaseUserBySid(sid)?.Name;

    public List<SqlDatabaseUser> GetDatabaseUsers()
    {
      this.Logger.Info("Getting all database users");
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetDatabaseUsers.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      return this.ExecuteDatabaseUserQuery("stmt_GetDatabaseUsers.sql");
    }

    internal SqlLoginInfo GetDboLogin()
    {
      this.VerifyInMasterDbOnAzure();
      return this.GetUserLogin("dbo");
    }

    internal SqlLoginInfo GetDboLogin(string databaseName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      this.VerifyInMasterDbOnAzure();
      byte[] dboLoginSid = this.GetDboLoginSid(databaseName);
      return dboLoginSid == null || dboLoginSid.Length == 0 ? (SqlLoginInfo) null : this.GetLogin(dboLoginSid);
    }

    public SqlLoginInfo GetLogin(SecurityIdentifier loginSid)
    {
      ArgumentUtility.CheckForNull<SecurityIdentifier>(loginSid, nameof (loginSid));
      this.VerifyInMasterDbOnAzure();
      byte[] numArray = new byte[loginSid.BinaryLength];
      loginSid.GetBinaryForm(numArray, 0);
      return this.GetLogin(numArray);
    }

    public SqlLoginInfo GetLogin(byte[] loginSid)
    {
      ArgumentUtility.CheckForNull<byte[]>(loginSid, nameof (loginSid));
      string str = HexConverter.ToString(loginSid);
      this.Logger.Info("Getting database login for SID 0x{0}", (object) str);
      this.VerifyInMasterDbOnAzure();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetLoginBySid.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindBinary("@loginSid", loginSid, SqlDbType.VarBinary);
      SqlLoginInfo login = this.ExecuteSqlLoginQuery("stmt_GetLoginBySid.sql").FirstOrDefault<SqlLoginInfo>();
      if (login == null)
        this.Logger.Info("Login for SID 0x{0} was not found.", (object) str);
      else
        this.Logger.Info("Found login for SID 0x{0}: {1}", (object) str, (object) login.LoginName);
      return login;
    }

    public SqlLoginInfo GetLogin(string loginName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(loginName, nameof (loginName));
      this.VerifyInMasterDbOnAzure();
      return this.ExecuteEmbeddedSqlLoginStatement("stmt_GetLogin.sql", "@loginName", loginName).FirstOrDefault<SqlLoginInfo>();
    }

    public byte[] GetUserSid(string userName)
    {
      this.PrepareSqlBatch("SELECT sid FROM sys.sysusers WHERE name = @userName".Length);
      this.AddStatement("SELECT sid FROM sys.sysusers WHERE name = @userName");
      this.BindString("@userName", userName, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      byte[] data = (byte[]) this.ExecuteScalar();
      this.Logger.Info("GetUserSid: Database={0}, User={1}, Sid={2}", (object) this.InitialCatalog, (object) userName, data != null ? (object) HexConverter.ToString(data) : (object) "not found");
      return data;
    }

    public List<SqlLoginInfo> GetRoleLogins(string roleName, bool windowsAuthLoginsOnly)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(roleName, nameof (roleName));
      this.VerifyNotSqlAzure();
      List<SqlLoginInfo> source = this.ExecuteEmbeddedSqlLoginStatement("stmt_GetRoleLogins.sql", "@roleName", roleName);
      if (windowsAuthLoginsOnly)
        source = source.Where<SqlLoginInfo>((System.Func<SqlLoginInfo, bool>) (l => l.IsNTName)).ToList<SqlLoginInfo>();
      return source;
    }

    public List<SqlLoginInfo> GetServiceAccountLogins()
    {
      this.VerifyNotSqlAzure();
      IEnumerable<SqlLoginInfo> source = this.GetRoleLogins("TFSEXECROLE", true).Where<SqlLoginInfo>((System.Func<SqlLoginInfo, bool>) (l => l.IsNTUser)).Union<SqlLoginInfo>(this.GetRoleLogins("db_owner", true).Where<SqlLoginInfo>((System.Func<SqlLoginInfo, bool>) (l => l.IsNTUser)));
      if (source.Any<SqlLoginInfo>())
        return source.ToList<SqlLoginInfo>();
      SqlLoginInfo dboLogin = this.GetDboLogin();
      if (dboLogin == null || !dboLogin.IsNTUser)
        return new List<SqlLoginInfo>();
      return new List<SqlLoginInfo>() { dboLogin };
    }

    public SqlLoginInfo GetUserLogin(string userName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(userName, nameof (userName));
      this.VerifyNotSqlAzure();
      SqlLoginInfo userLogin = this.ExecuteEmbeddedSqlLoginStatement("stmt_GetUserLogin.sql", "@userName", userName).FirstOrDefault<SqlLoginInfo>();
      this.Logger.Info("GetUserLogin: Database={0}, User={1}, LoginName={2}", (object) this.InitialCatalog, (object) userName, userLogin != null ? (object) userLogin.LoginName : (object) "not found");
      return userLogin;
    }

    public void GrantAlterAnyDatabasePermission(string login, bool withGrantOption) => this.GrantServerScopePermission(login, "ALTER ANY DATABASE", withGrantOption);

    public void GrantAlterAnyLoginPermission(string login, bool withGrantOption) => this.GrantServerScopePermission(login, "ALTER ANY LOGIN", withGrantOption);

    public void GrantAlterAnyConnectionPermission(
      string login,
      bool withGrantOption,
      string grantor = null)
    {
      this.GrantServerScopePermission(login, "ALTER ANY CONNECTION", withGrantOption, grantor);
    }

    public void GrantAlterEventSessionPermission(
      string login,
      bool withGrantOption,
      string grantor = null)
    {
      this.GrantServerScopePermission(login, "ALTER ANY EVENT SESSION", withGrantOption, grantor);
    }

    public void GrantCreateAnyDatabasePermission(string login, bool withGrantOption) => this.GrantServerScopePermission(login, "CREATE ANY DATABASE", withGrantOption);

    public void GrantViewAnyDefinitionPermission(string login, bool withGrantOption) => this.GrantServerScopePermission(login, "VIEW ANY DEFINITION", withGrantOption);

    public void GrantViewServerStatePermission(string login, bool withGrantOption = false, string grantor = null) => this.GrantServerScopePermission(login, "VIEW SERVER STATE", withGrantOption, grantor);

    public void RevokeViewServerStatePermission(string login, bool cascade = false, string grantor = null) => this.RevokeServerScopePermission(login, "VIEW SERVER STATE", grantor, cascade);

    public void RevokeAlterAnyConnectionPermission(string login, bool cascade = false, string grantor = null) => this.RevokeServerScopePermission(login, "ALTER ANY CONNECTION", grantor, cascade);

    public void GrantAlterAnyUserPermission(string user, bool withGrantOption = false, string grantor = null) => this.GrantDatabaseScopePermission(user, "ALTER ANY USER", withGrantOption, grantor);

    public void GrantExecutePermission(string login, string item = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(login, nameof (login));
      this.Logger.Info("Granting execute permission on {0}. Login: {1}.", (object) item, (object) login);
      string sqlStatement;
      if (string.IsNullOrEmpty(item))
      {
        sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GRANT EXECUTE TO {0}", (object) StringUtil.QuoteName(login));
      }
      else
      {
        string[] strArray = item.Split('.');
        sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IF(OBJECT_ID('{0}', 'P') IS NOT NULL)\r\nBEGIN\r\nGRANT EXECUTE ON {0} TO {1}\r\nEND", strArray.Length == 2 ? (object) (StringUtil.QuoteName(strArray[0]) + "." + StringUtil.QuoteName(strArray[1])) : (object) StringUtil.QuoteName(strArray[0]), (object) StringUtil.QuoteName(login));
      }
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public bool HasDatabasePermission(string permissionName, string databaseName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(permissionName, nameof (permissionName));
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      return this.HasPermission(databaseName, "DATABASE", permissionName);
    }

    public bool HasPermission(string securable, string securableClass, string permissionName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(permissionName, nameof (permissionName));
      this.Logger.Info("HasPermission: '{0}', Securable: '{1}', Class: '{2}' on {3}.", (object) permissionName, (object) securable, (object) securableClass, (object) this.DataSource);
      this.PrepareSqlBatch("SELECT CONVERT(BIT, HAS_PERMS_BY_NAME(@securable, @securableClass, @permission))".Length);
      this.AddStatement("SELECT CONVERT(BIT, HAS_PERMS_BY_NAME(@securable, @securableClass, @permission))");
      this.BindSysname("@securable", securable, true);
      this.BindSysname("@securableClass", securableClass, true);
      this.BindSysname("@permission", permissionName, false);
      bool flag = (bool) this.ExecuteScalar();
      this.Logger.Info("hasPermission '{0}'={1}", (object) permissionName, (object) flag);
      return flag;
    }

    public bool HasRolePermission(string permissionName, string roleName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(permissionName, nameof (permissionName));
      ArgumentUtility.CheckStringForNullOrEmpty(roleName, nameof (roleName));
      return this.HasPermission(roleName, "ROLE", permissionName);
    }

    public bool HasServerPermission(string permissionName)
    {
      this.VerifyNotSqlAzure();
      return this.HasPermission((string) null, (string) null, permissionName);
    }

    public bool HasGrantOptionForPermission(string permissionName)
    {
      this.VerifyNotSqlAzure();
      this.VerifyInMaster();
      ArgumentUtility.CheckStringForNullOrEmpty(permissionName, nameof (permissionName));
      this.Logger.Info("HasGrantOptionForPermission: '{0}' on {1}.", (object) permissionName, (object) this.DataSource);
      string sqlStatement = string.Format("BEGIN TRAN\r\nGRANT {0} TO public\r\nROLLBACK", (object) permissionName);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      bool flag;
      try
      {
        this.ExecuteNonQuery();
        flag = true;
      }
      catch (SqlException ex)
      {
        if (ex.Number == 4613)
          flag = false;
        else
          throw;
      }
      this.Logger.Info("HasGrantOptionForPermission '{0}'= {1}", (object) permissionName, (object) flag);
      return flag;
    }

    public int GetUserId(string userName = null)
    {
      string sqlStatement = !string.IsNullOrEmpty(userName) ? "SELECT USER_ID(@userName)" : "SELECT USER_ID()";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindSysname("@userName", userName, true);
      int userId = -1;
      object obj = this.ExecuteScalar();
      if (!(obj is DBNull))
        userId = Convert.ToInt32(obj);
      if (string.IsNullOrEmpty(userName))
        this.Logger.Info(string.Format("Current UserId = {0}", (object) userId));
      else
        this.Logger.Info(string.Format("{0} UserId = {1}", (object) userName, (object) userId));
      return userId;
    }

    public bool IsRoleMember(string roleName, string userName = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(roleName, nameof (roleName));
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_IsRoleMember.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@roleName", roleName, false);
      this.BindSysname("@userName", userName, true);
      bool flag = false;
      if (!(this.ExecuteScalar() is DBNull))
        flag = Convert.ToBoolean(this.ExecuteScalar());
      if (string.IsNullOrEmpty(userName))
        this.Logger.Info(string.Format("Current user isRoleMember '{0}' = {1}", (object) roleName, (object) flag));
      else
        this.Logger.Info(string.Format("{0} isRoleMember '{1}' = {2}", (object) userName, (object) roleName, (object) flag));
      return flag;
    }

    public bool IsInServerRole(string roleName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(roleName, nameof (roleName));
      this.PrepareSqlBatch("SELECT CONVERT(BIT,  ISNULL(IS_SRVROLEMEMBER(@roleName), 0)) isMember".Length);
      this.AddStatement("SELECT CONVERT(BIT,  ISNULL(IS_SRVROLEMEMBER(@roleName), 0)) isMember");
      this.BindSysname("@roleName", roleName, false);
      bool flag = (bool) this.ExecuteScalar();
      this.Logger.Info("IsInServerRole '{0}'={1}", (object) roleName, (object) flag);
      return flag;
    }

    public bool IsSA()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_IsSuperUser.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      return (bool) this.ExecuteScalar();
    }

    public List<SqlDatabaseRole> QueryDatabaseRoles() => this.ExecuteEmbeddedDatabaseRoleStatement("stmt_QueryDataRoles.sql");

    public AccountsResult ModifyExecRole(
      string accountName,
      string role,
      AccountsOperation operation)
    {
      this.Logger.Info("ModifyExecRole: {0}:{1}", (object) operation.ToString(), (object) accountName);
      this.VerifyNotSqlAzure();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_ModifyExecRole.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@account", accountName, false);
      this.BindSysname("@roleName", role, false);
      this.BindInt("@operation", (int) operation);
      AccountsResult accountsResult = (AccountsResult) this.ExecuteScalar();
      this.Logger.Info("result: {0}", (object) accountsResult);
      return accountsResult;
    }

    protected override string TraceArea => "SqlSecurity";

    private void AlterAuthorization(string classType, string entityName, string principalName)
    {
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER AUTHORIZATION ON {0}::{1} TO {2}", (object) classType, (object) StringUtil.QuoteName(entityName), (object) StringUtil.QuoteName(principalName));
      this.Logger.Info("Altering authorization. Entity name: {0}. Class: {1}. To: {2}", (object) entityName, (object) classType, (object) principalName);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public void AlterUser(string userName, string loginName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(loginName, nameof (loginName));
      this.Logger.Info("Altering user {0}. Login: {1}. Data Source: {2}. Database Name: {3}.", (object) userName, (object) loginName, (object) this.DataSource, (object) this.Database);
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ALTER USER {0} WITH LOGIN = {1}", (object) StringUtil.QuoteName(userName), (object) StringUtil.QuoteName(loginName));
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public void RevokeServerScopePermission(
      string login,
      string permission,
      string grantor,
      bool cascade)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(login, nameof (login));
      ArgumentUtility.CheckStringForNullOrEmpty(permission, nameof (permission));
      this.VerifyNotSqlAzure();
      string str1 = cascade ? "CASCADE" : string.Empty;
      string str2 = string.IsNullOrEmpty(grantor) ? string.Empty : "AS " + StringUtil.QuoteName(grantor);
      this.Logger.Info("Revoke server scope permission. Login: {0}. Permission: {1}. Data Source: {2}", (object) login, (object) permission, (object) this.DataSource);
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "REVOKE {0} TO {1} {2} {3}", (object) permission, (object) StringUtil.QuoteName(login), (object) str1, (object) str2);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public List<string> GetGrantorsOfPermission(string login, string permission)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(login, nameof (login));
      ArgumentUtility.CheckStringForNullOrEmpty(permission, nameof (permission));
      this.VerifyNotSqlAzure();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryGrantorsOfPermission.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@grantee", login, false);
      this.BindString("@permission", permission, 128, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "stmt_QueryGrantorsOfPermission.sql", (IVssRequestContext) null);
      resultCollection.AddBinder<string>((ObjectBinder<string>) new GrantorColumn());
      return resultCollection.GetCurrent<string>().Items;
    }

    public List<AvailabilityReplica> GetAvailabilityReplicas(string domainExtension, string port)
    {
      this.VerifyNotSqlAzure();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryAvailabilityReplicas.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "stmt_QueryAvailabilityReplicas.sql", (IVssRequestContext) null);
      resultCollection.AddBinder<AvailabilityReplica>((ObjectBinder<AvailabilityReplica>) new AvailabilityReplicaColumns());
      List<AvailabilityReplica> items = resultCollection.GetCurrent<AvailabilityReplica>().Items;
      foreach (AvailabilityReplica availabilityReplica in items)
        availabilityReplica.Node = TeamFoundationSqlSecurityComponent.ConstructReplicaNodeName(availabilityReplica.Node, domainExtension, port);
      return items;
    }

    internal static string ConstructReplicaNodeName(
      string node,
      string domainExtension,
      string port = null)
    {
      bool flag = node.IndexOf(',') < 0 && !string.IsNullOrEmpty(port);
      if (!string.IsNullOrEmpty(domainExtension))
      {
        string str = flag ? domainExtension + "," + port : domainExtension;
        node = node.IndexOf(',') <= 0 ? (node.IndexOf('\\') <= 0 ? node + str : node.Replace("\\", str + "\\")) : node.Replace(",", domainExtension + ",");
      }
      else if (flag)
        node = node.IndexOf('\\') <= 0 ? node + "," + port : node.Replace("\\", "," + port + "\\");
      return node;
    }

    protected override void AddStatement(string sqlStatement) => this.AddStatement(sqlStatement, 0, true, false);

    private List<SqlDatabaseRole> ExecuteEmbeddedDatabaseRoleStatement(string stmtName)
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString(stmtName);
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      return this.ExecuteDatabaseRoleQuery(stmtName);
    }

    private List<SqlDatabaseRole> ExecuteEmbeddedDatabaseRoleStatement(
      string stmtName,
      string parameterName,
      string parameterValue)
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString(stmtName);
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString(parameterName, parameterValue, parameterValue.Length, false, SqlDbType.NVarChar);
      return this.ExecuteDatabaseRoleQuery(stmtName);
    }

    private List<SqlDatabaseRole> ExecuteDatabaseRoleQuery(string statementName)
    {
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), statementName, (IVssRequestContext) null);
      resultCollection.AddBinder<SqlDatabaseRole>((ObjectBinder<SqlDatabaseRole>) new SqlDatabaseRoleColumns());
      return resultCollection.GetCurrent<SqlDatabaseRole>().Items;
    }

    private List<SqlLoginInfo> ExecuteEmbeddedSqlLoginStatement(string stmtName)
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString(stmtName);
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      return this.ExecuteSqlLoginQuery(stmtName);
    }

    private List<SqlLoginInfo> ExecuteEmbeddedSqlLoginStatement(
      string stmtName,
      string parameterName,
      string parameterValue)
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString(stmtName);
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString(parameterName, parameterValue, parameterValue.Length, false, SqlDbType.NVarChar);
      return this.ExecuteSqlLoginQuery(stmtName);
    }

    private List<SqlLoginInfo> ExecuteSqlLoginQuery(string statementName)
    {
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), statementName, (IVssRequestContext) null);
      resultCollection.AddBinder<SqlLoginInfo>((ObjectBinder<SqlLoginInfo>) new SqlLoginColumns());
      return resultCollection.GetCurrent<SqlLoginInfo>().Items;
    }

    private List<SqlDatabaseUser> ExecuteDatabaseUserQuery(string statementName)
    {
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), statementName, (IVssRequestContext) null);
      resultCollection.AddBinder<SqlDatabaseUser>((ObjectBinder<SqlDatabaseUser>) new SqlDatabaseUserColumns());
      return resultCollection.GetCurrent<SqlDatabaseUser>().Items;
    }

    public void GrantServerScopePermission(
      string login,
      string permission,
      bool withGrantOption,
      string grantor = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(login, nameof (login));
      ArgumentUtility.CheckStringForNullOrEmpty(permission, nameof (permission));
      this.VerifyNotSqlAzure();
      this.GrantPermission(login, permission, withGrantOption, grantor);
    }

    public void GrantDatabaseScopePermission(
      string user,
      string permission,
      bool withGrantOption,
      string grantor = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(user, "login");
      ArgumentUtility.CheckStringForNullOrEmpty(permission, nameof (permission));
      this.GrantPermission(user, permission, withGrantOption, grantor);
    }

    private void GrantPermission(
      string loginOrUser,
      string permission,
      bool withGrantOption,
      string grantor)
    {
      this.Logger.Info("Granting permission. Login/User: {0}. Permission: {1}. Grant option: {2}. Data Source: {3}", (object) loginOrUser, (object) permission, (object) withGrantOption, (object) this.DataSource);
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GRANT {0} TO {1}", (object) permission, (object) StringUtil.QuoteName(loginOrUser));
      if (withGrantOption)
        sqlStatement += " WITH GRANT OPTION";
      if (!string.IsNullOrEmpty(grantor))
        sqlStatement = sqlStatement + " AS " + StringUtil.QuoteName(grantor);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteNonQuery();
      this.Logger.Info("Success");
    }

    public string CreateAADUserForServiceAccount(
      string serviceAcountName,
      Guid serviceAccountId,
      bool isSecurityGroup)
    {
      this.VerifySqlAzure();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_CreateAadUserForServiceAccount.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindSysname("@serviceAccountName", serviceAcountName, false);
      this.BindGuid("@serviceAccountId", serviceAccountId);
      this.BindBoolean("@isSecurityGroup", isSecurityGroup);
      string forServiceAccount = (string) this.ExecuteScalar();
      this.Logger.Info("Success");
      return forServiceAccount;
    }
  }
}
