// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlConnectionInfoFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SqlConnectionInfoFactory
  {
    private static ConcurrentDictionary<string, SqlConnectionInfoFactory.CredentialInfo> s_credentialCache = new ConcurrentDictionary<string, SqlConnectionInfoFactory.CredentialInfo>((IEqualityComparer<string>) StringComparer.Ordinal);
    private static readonly string s_area = "ConnectionInfo";
    private static readonly string s_layer = nameof (SqlConnectionInfoFactory);
    private static readonly Dictionary<string, string> s_sqlAzureTestDomains = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "mscds.com",
        AadSupportedResources.AzureSql
      },
      {
        "sqlazurelabs.com",
        AadSupportedResources.AzureSql
      },
      {
        "database.secure.windows.net",
        AadSupportedResources.AzureSql
      }
    };
    private static readonly Dictionary<string, string> s_azureSqlStandardDomainMap = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "database.windows.net",
        AadSupportedResources.AzureSql
      },
      {
        "database.usgovcloudapi.net",
        AadSupportedResources.AzureGovSql
      },
      {
        "database.cloudapi.de",
        AadSupportedResources.AzureDeSql
      },
      {
        "database.chinacloudapi.cn",
        AadSupportedResources.AzureCnSql
      },
      {
        "database.cloudapi.microsoft.scloud",
        AadSupportedResources.AzureUSSecSql
      },
      {
        "database.cloudapi.eaglex.ic.gov",
        AadSupportedResources.AzureUSNatSql
      }
    };
    private static Dictionary<string, string> s_azureSqlDomainMap;
    private const string c_additionalAzureDomainsRegistryKey = "Software\\Microsoft\\TeamFoundationServer";
    private const string c_additionalAzureDomainsRegistryValue = "SqlAzureDomains";

    public static bool TryCreate(
      string connectionString,
      string userName,
      out ISqlConnectionInfo connectionInfo)
    {
      return SqlConnectionInfoFactory.TryCreate(connectionString, userName, (byte[]) null, out connectionInfo);
    }

    public static bool TryCreate(
      string connectionString,
      string userName,
      byte[] passwordEncrypted,
      out ISqlConnectionInfo connectionInfo)
    {
      string cacheKey = SqlConnectionInfoFactory.GetCacheKey(new SqlConnectionStringBuilder(connectionString).DataSource, userName);
      connectionInfo = (ISqlConnectionInfo) null;
      SqlConnectionInfoFactory.CredentialInfo credentialInfo;
      if (!SqlConnectionInfoFactory.s_credentialCache.TryGetValue(cacheKey, out credentialInfo) || passwordEncrypted != null && passwordEncrypted.Length != 0 && (credentialInfo.PasswordEncrypted == null || !ArrayUtil.Equals(passwordEncrypted, credentialInfo.PasswordEncrypted)))
        return false;
      connectionInfo = (ISqlConnectionInfo) new SqlConnectionInfoFactory.DatabaseConnectionInfoHosted(connectionString, credentialInfo.SqlCredential);
      return true;
    }

    public static ISqlConnectionInfo Create(
      string connectionString,
      string userName = null,
      SecureString password = null,
      byte[] passwordEncrypted = null)
    {
      if (string.IsNullOrEmpty(connectionString))
        return (ISqlConnectionInfo) null;
      if (!string.IsNullOrEmpty(userName) && password == null)
        throw new ArgumentException(FrameworkResources.UserNameButNoPassword());
      if (password != null && string.IsNullOrEmpty(userName))
        throw new ArgumentException(FrameworkResources.PasswordButNoUserName());
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      if (string.IsNullOrEmpty(userName))
      {
        if (connectionStringBuilder.IntegratedSecurity)
          return (ISqlConnectionInfo) new SqlConnectionInfoFactory.DatabaseConnectionInfoIntegratedSecurity(connectionString);
        if (connectionStringBuilder.Authentication == SqlAuthenticationMethod.ActiveDirectoryIntegrated)
          return (ISqlConnectionInfo) new SqlConnectionInfoFactory.DatabaseConnectionInfoAADIntegrated(connectionString);
        if (connectionStringBuilder.Authentication == SqlAuthenticationMethod.NotSpecified && string.IsNullOrWhiteSpace(connectionStringBuilder.UserID) && string.IsNullOrWhiteSpace(connectionStringBuilder.Password) && string.IsNullOrWhiteSpace(userName) && password == null && passwordEncrypted == null)
          return (ISqlConnectionInfo) new SqlConnectionInfoFactory.DatabaseConnectionInfoMsiAad(connectionString, (IMsiAccessTokenProvider) new MsiAccessTokenProvider(MsiTokenCache.SharedCache, (IAzureInstanceMetadataProvider) new AzureInstanceMetadataProvider(new HttpClient())));
        string userId = connectionStringBuilder.UserID;
        SecureString secureString = Encoding.UTF8.GetBytes(connectionStringBuilder.Password).ToSecureString();
        connectionStringBuilder.Remove("User ID");
        connectionStringBuilder.Remove("Password");
        if (string.IsNullOrEmpty(userId) || secureString == null)
          throw new ArgumentException(FrameworkResources.InvalidConnectionString());
        SqlCredential sqlCredential = SqlConnectionInfoFactory.GetOrCreateSqlCredential(connectionStringBuilder.DataSource, userId, secureString, passwordEncrypted);
        return (ISqlConnectionInfo) new SqlConnectionInfoFactory.DatabaseConnectionInfoHosted(connectionStringBuilder.ConnectionString, sqlCredential);
      }
      SqlCredential sqlCredential1 = SqlConnectionInfoFactory.GetOrCreateSqlCredential(connectionStringBuilder.DataSource, userName, password, passwordEncrypted);
      return (ISqlConnectionInfo) new SqlConnectionInfoFactory.DatabaseConnectionInfoHosted(connectionString, sqlCredential1);
    }

    public static ISqlConnectionInfo Create(
      string connectionString,
      Func<string> accessTokenDelegate)
    {
      if (string.IsNullOrEmpty(connectionString))
        return (ISqlConnectionInfo) null;
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      return (ISqlConnectionInfo) new SqlConnectionInfoFactory.DatabaseConnectionInfoAADAccessToken(connectionString, accessTokenDelegate);
    }

    public static bool IsSqlAzure(string dataSource)
    {
      if (string.IsNullOrEmpty(dataSource))
        return false;
      int length = dataSource.IndexOf(',');
      if (length > 0)
        dataSource = dataSource.Substring(0, length);
      foreach (KeyValuePair<string, string> azureDomain in (IEnumerable<KeyValuePair<string, string>>) SqlConnectionInfoFactory.GetAzureDomainMap())
      {
        if (dataSource.EndsWith(azureDomain.Key, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    public static bool IsAzureSqlManagedInstance(string dataSource)
    {
      if (string.IsNullOrEmpty(dataSource))
        return false;
      int length = dataSource.IndexOf(',');
      if (length > 0)
        dataSource = dataSource.Substring(0, length);
      foreach (KeyValuePair<string, string> azureDomain in (IEnumerable<KeyValuePair<string, string>>) SqlConnectionInfoFactory.GetAzureDomainMap())
      {
        if (dataSource.EndsWith(azureDomain.Key, StringComparison.OrdinalIgnoreCase))
          return dataSource.IndexOf('.', 0, dataSource.Length - azureDomain.Key.Length - 1) > 0;
      }
      return false;
    }

    public static IReadOnlyDictionary<string, string> GetAzureDomainMap()
    {
      if (SqlConnectionInfoFactory.s_azureSqlDomainMap != null)
        return (IReadOnlyDictionary<string, string>) SqlConnectionInfoFactory.s_azureSqlDomainMap;
      Dictionary<string, string> collection = new Dictionary<string, string>((IDictionary<string, string>) SqlConnectionInfoFactory.s_azureSqlStandardDomainMap, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      try
      {
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\TeamFoundationServer"))
        {
          string str1 = registryKey != null ? registryKey.GetValue("SqlAzureDomains", (object) "") as string : "";
          char[] separator1 = new char[1]{ ';' };
          foreach (string str2 in str1.Split(separator1, StringSplitOptions.RemoveEmptyEntries))
          {
            string[] separator2 = new string[1]{ ":=" };
            string[] strArray = str2.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length == 2)
              collection[strArray[0]] = strArray[1];
          }
        }
      }
      catch (Exception ex)
      {
        Trace.TraceError(ex.ToStringDemystified());
      }
      collection.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) SqlConnectionInfoFactory.s_sqlAzureTestDomains);
      SqlConnectionInfoFactory.s_azureSqlDomainMap = collection;
      return (IReadOnlyDictionary<string, string>) collection;
    }

    internal static bool RemoveSqlCredentialFromCache(string sqlInstance, string userId)
    {
      string cacheKey = SqlConnectionInfoFactory.GetCacheKey(sqlInstance, userId);
      return SqlConnectionInfoFactory.s_credentialCache.TryRemove(cacheKey, out SqlConnectionInfoFactory.CredentialInfo _);
    }

    internal static void ClearSqlCredentialCache(bool clearAllSqlConnectionPools)
    {
      SqlConnectionInfoFactory.s_credentialCache.Clear();
      if (!clearAllSqlConnectionPools)
        return;
      SqlConnection.ClearAllPools();
    }

    private static SqlCredential GetOrCreateSqlCredential(
      string sqlInstance,
      string userId,
      SecureString password,
      byte[] passwordEncrypted)
    {
      string cacheKey = SqlConnectionInfoFactory.GetCacheKey(sqlInstance, userId);
      for (int index = 0; index < 10; ++index)
      {
        SqlConnectionInfoFactory.CredentialInfo comparisonValue;
        if (SqlConnectionInfoFactory.s_credentialCache.TryGetValue(cacheKey, out comparisonValue))
        {
          if (passwordEncrypted == null || passwordEncrypted.Length == 0)
          {
            TeamFoundationTracingService.TraceRaw(107005, TraceLevel.Info, SqlConnectionInfoFactory.s_area, SqlConnectionInfoFactory.s_layer, "Returning a new, un-cached SqlCredential because the password was not encrypted. User: {0} SqlInstance: {1}", (object) userId, (object) sqlInstance);
            return new SqlCredential(userId, password);
          }
          if (comparisonValue.PasswordEncrypted != null && ArrayUtil.Equals(passwordEncrypted, comparisonValue.PasswordEncrypted))
            return comparisonValue.SqlCredential;
          if (comparisonValue.PasswordEncrypted != null)
            TeamFoundationTracingService.TraceRaw(107000, TraceLevel.Info, SqlConnectionInfoFactory.s_area, SqlConnectionInfoFactory.s_layer, "Detected password change for {0} ({1}). Replacing stale credential.", (object) userId, (object) sqlInstance);
          else
            TeamFoundationTracingService.TraceRaw(107001, TraceLevel.Info, SqlConnectionInfoFactory.s_area, SqlConnectionInfoFactory.s_layer, "Upgrading credential for {0} ({1}) to include an encrypted password for comparison.", (object) userId, (object) sqlInstance);
          SqlConnectionInfoFactory.CredentialInfo newValue = new SqlConnectionInfoFactory.CredentialInfo()
          {
            SqlCredential = new SqlCredential(userId, password),
            PasswordEncrypted = passwordEncrypted
          };
          if (SqlConnectionInfoFactory.s_credentialCache.TryUpdate(cacheKey, newValue, comparisonValue))
            return newValue.SqlCredential;
          TeamFoundationTracingService.TraceRaw(107002, TraceLevel.Warning, SqlConnectionInfoFactory.s_area, SqlConnectionInfoFactory.s_layer, "Failed to replace credential for {0} ({1}). Beat by another thread?", (object) userId, (object) sqlInstance);
        }
        else
        {
          SqlConnectionInfoFactory.CredentialInfo credentialInfo = new SqlConnectionInfoFactory.CredentialInfo()
          {
            SqlCredential = new SqlCredential(userId, password),
            PasswordEncrypted = passwordEncrypted
          };
          if (SqlConnectionInfoFactory.s_credentialCache.TryAdd(cacheKey, credentialInfo))
            return credentialInfo.SqlCredential;
          TeamFoundationTracingService.TraceRaw(107003, TraceLevel.Warning, SqlConnectionInfoFactory.s_area, SqlConnectionInfoFactory.s_layer, "Failed to add credential for {0} ({1}). Beat by another thread?", (object) userId, (object) sqlInstance);
        }
      }
      TeamFoundationTracingService.TraceRaw(107004, TraceLevel.Error, SqlConnectionInfoFactory.s_area, SqlConnectionInfoFactory.s_layer, "Failed to retrieve sql credential for {0} ({1}) after 10 attempts.", (object) userId, (object) sqlInstance);
      throw new ApplicationException("Failed to retrieve a sql credential after 10 attempts.");
    }

    private static void UpdateSqlCredentialInCache(
      string sqlInstance,
      SqlCredential credential,
      byte[] passwordEncrypted)
    {
      string cacheKey = SqlConnectionInfoFactory.GetCacheKey(sqlInstance, credential.UserId);
      SqlConnectionInfoFactory.s_credentialCache[cacheKey] = new SqlConnectionInfoFactory.CredentialInfo()
      {
        SqlCredential = credential,
        PasswordEncrypted = passwordEncrypted
      };
    }

    private static string GetCacheKey(string sqlInstance, string userId)
    {
      string str = sqlInstance;
      if (sqlInstance.StartsWith(FrameworkServerConstants.TcpProtocolPrefix, StringComparison.OrdinalIgnoreCase))
        str = sqlInstance.Substring(FrameworkServerConstants.TcpProtocolPrefix.Length);
      return str.ToUpperInvariant() + ":" + userId;
    }

    private class CredentialInfo
    {
      public SqlCredential SqlCredential { get; set; }

      public byte[] PasswordEncrypted { get; set; }
    }

    internal abstract class DatabaseConnectionInfo : ISqlConnectionInfo
    {
      protected SqlConnectionStringBuilder m_builder;

      protected DatabaseConnectionInfo(string connectionString) => this.ConnectionString = connectionString;

      public virtual string ConnectionString
      {
        get => this.m_builder?.ConnectionString;
        protected set
        {
          SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(value);
          if (!string.IsNullOrEmpty(builder.Password))
            throw new ArgumentException("Connection string must not contain a password.", nameof (value));
          if (!string.IsNullOrEmpty(builder.UserID))
            throw new ArgumentException("Connection string must not contain a User ID.", nameof (value));
          SqlConnectionHelper.SetConnectionTimeout(builder);
          this.m_builder = builder;
        }
      }

      public string InitialCatalog => this.m_builder.InitialCatalog;

      public string DataSource => this.m_builder.DataSource;

      public int MaxPoolSize => this.m_builder.MaxPoolSize;

      public ApplicationIntent ApplicationIntent => this.m_builder.ApplicationIntent;

      public override string ToString() => this.ConnectionString;

      SqlConnection ISqlConnectionInfo.CreateSqlConnection() => this.CreateSqlConnection();

      public bool IsSqlAzure => SqlConnectionInfoFactory.IsSqlAzure(this.DataSource);

      public bool IsAzureSqlManagedInstance => SqlConnectionInfoFactory.IsAzureSqlManagedInstance(this.DataSource);

      public abstract ISqlConnectionInfo Create(string connectionStringOverride);

      protected abstract SqlConnection CreateSqlConnection();
    }

    internal class DatabaseConnectionInfoIntegratedSecurity : 
      SqlConnectionInfoFactory.DatabaseConnectionInfo
    {
      private static readonly ConcurrentDictionary<string, bool> s_dataSources = new ConcurrentDictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      private bool? m_useMultiSubnetFailover;
      private const string c_explicitMultiSubnetFailover = "MultiSubnetFailover=";

      internal DatabaseConnectionInfoIntegratedSecurity(string connectionString)
        : base(connectionString)
      {
      }

      public override string ConnectionString
      {
        get => this.m_builder?.ConnectionString;
        protected set
        {
          SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(value);
          if (!string.IsNullOrEmpty(connectionStringBuilder.Password))
            throw new ArgumentException("Connection string must not contain a password.", nameof (value));
          if (!string.IsNullOrEmpty(connectionStringBuilder.UserID))
            throw new ArgumentException("Connection string must not contain a User ID.", nameof (value));
          this.AdjustForAlwaysOnUsingCache(connectionStringBuilder);
          this.m_builder = connectionStringBuilder;
        }
      }

      private void AdjustForAlwaysOnUsingCache(SqlConnectionStringBuilder connectionStringBuilder)
      {
        if (!string.IsNullOrEmpty(connectionStringBuilder.ConnectionString) && connectionStringBuilder.ConnectionString.Contains("MultiSubnetFailover="))
        {
          this.m_useMultiSubnetFailover = new bool?(connectionStringBuilder.GetMultiSubnetFailover());
        }
        else
        {
          if (connectionStringBuilder.DataSource == null)
            return;
          bool? nullable = SqlConnectionInfoFactory.DatabaseConnectionInfoIntegratedSecurity.UseMultiSubnetFailover(connectionStringBuilder.DataSource);
          if (!nullable.HasValue)
            return;
          connectionStringBuilder.SetMultiSubnetFailover(nullable.Value);
          this.m_useMultiSubnetFailover = new bool?(nullable.Value);
        }
      }

      public override ISqlConnectionInfo Create(string connectionStringOverride) => (ISqlConnectionInfo) new SqlConnectionInfoFactory.DatabaseConnectionInfoIntegratedSecurity(connectionStringOverride);

      protected override SqlConnection CreateSqlConnection()
      {
        if (!this.m_useMultiSubnetFailover.HasValue)
        {
          try
          {
            this.m_useMultiSubnetFailover = new bool?(this.QueryUseMultiSubnetFailover((ISqlConnectionInfo) this));
            this.ConnectionString = this.ConnectionString;
          }
          catch
          {
          }
        }
        return new SqlConnection(this.ConnectionString);
      }

      private bool QueryUseMultiSubnetFailover(ISqlConnectionInfo connectionInfo)
      {
        bool flag;
        if (SqlConnectionInfoFactory.DatabaseConnectionInfoIntegratedSecurity.s_dataSources.TryGetValue(connectionInfo.DataSource, out flag))
          return flag;
        using (SqlConnection connection = new SqlConnection(new SqlConnectionStringBuilder()
        {
          DataSource = connectionInfo.DataSource,
          InitialCatalog = TeamFoundationSqlResourceComponent.Master,
          IntegratedSecurity = true
        }.ConnectionString))
        {
          SqlCommand sqlCommand = new SqlCommand(EmbeddedResourceUtil.GetResourceAsString("stmt_UseMultiSubnetFailover.sql"), connection);
          connection.Open();
          flag = (bool) sqlCommand.ExecuteScalar();
        }
        SqlConnectionInfoFactory.DatabaseConnectionInfoIntegratedSecurity.s_dataSources[connectionInfo.DataSource] = flag;
        return flag;
      }

      private static bool? UseMultiSubnetFailover(string dataSource)
      {
        bool flag;
        return SqlConnectionInfoFactory.DatabaseConnectionInfoIntegratedSecurity.s_dataSources.TryGetValue(dataSource, out flag) ? new bool?(flag) : new bool?();
      }
    }

    internal class DatabaseConnectionInfoAADIntegrated : 
      SqlConnectionInfoFactory.DatabaseConnectionInfo
    {
      public DatabaseConnectionInfoAADIntegrated(string connectionString)
        : base(connectionString)
      {
      }

      public override string ConnectionString
      {
        get => this.m_builder?.ConnectionString;
        protected set
        {
          SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(value);
          if (!string.IsNullOrEmpty(builder.Password))
            throw new ArgumentException("Connection string must not contain a password.", nameof (value));
          if (!string.IsNullOrEmpty(builder.UserID))
            throw new ArgumentException("Connection string must not contain a User ID.", nameof (value));
          if (builder.IntegratedSecurity)
            throw new ArgumentException("Integrated Security is not supported when using Active Directory Authentication", nameof (value));
          if (builder.Authentication != SqlAuthenticationMethod.ActiveDirectoryIntegrated)
            throw new ArgumentException("Connection string does not have a valid Authentication setting", nameof (value));
          SqlConnectionHelper.SetConnectionTimeout(builder);
          this.m_builder = builder;
        }
      }

      public override ISqlConnectionInfo Create(string connectionStringOverride) => (ISqlConnectionInfo) new SqlConnectionInfoFactory.DatabaseConnectionInfoAADIntegrated(connectionStringOverride);

      protected override SqlConnection CreateSqlConnection() => new SqlConnection(this.ConnectionString);
    }

    internal class DatabaseConnectionInfoAADAccessToken : 
      SqlConnectionInfoFactory.DatabaseConnectionInfo
    {
      private Func<string> m_accessTokenDelegate;

      public DatabaseConnectionInfoAADAccessToken(
        string connectionString,
        Func<string> accessTokenDelegate)
        : base(connectionString)
      {
        ArgumentUtility.CheckForNull<Func<string>>(accessTokenDelegate, nameof (accessTokenDelegate));
        this.m_accessTokenDelegate = accessTokenDelegate;
      }

      public override string ConnectionString
      {
        get => this.m_builder?.ConnectionString;
        protected set
        {
          SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(value);
          if (!string.IsNullOrEmpty(builder.Password))
            throw new ArgumentException("Connection string must not contain a password.", nameof (value));
          if (!string.IsNullOrEmpty(builder.UserID))
            throw new ArgumentException("Connection string must not contain a User ID.", nameof (value));
          if (builder.IntegratedSecurity)
            throw new ArgumentException("Integrated Security is not supported when using AAD token authentication.", nameof (value));
          if (!SqlConnectionInfoFactory.IsSqlAzure(builder.DataSource))
            throw new ArgumentException("Data source is required to be a SQL Azure server when using AAD token authentication.", nameof (value));
          if (!builder.Encrypt)
            throw new ArgumentException("Encryption is required when using AAD token authentication.", nameof (value));
          SqlConnectionHelper.SetConnectionTimeout(builder);
          this.m_builder = builder;
        }
      }

      public override ISqlConnectionInfo Create(string connectionStringOverride)
      {
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionStringOverride);
        connectionStringBuilder.Remove("User ID");
        connectionStringBuilder.Remove("Password");
        return (ISqlConnectionInfo) new SqlConnectionInfoFactory.DatabaseConnectionInfoAADAccessToken(connectionStringBuilder.ConnectionString, this.m_accessTokenDelegate);
      }

      protected override SqlConnection CreateSqlConnection() => new SqlConnection(this.ConnectionString)
      {
        AccessToken = this.m_accessTokenDelegate()
      };
    }

    internal class DatabaseConnectionInfoMsiAad : 
      SqlConnectionInfoFactory.DatabaseConnectionInfoAADAccessToken
    {
      public DatabaseConnectionInfoMsiAad(string connectionString, IMsiAccessTokenProvider provider)
        : base(connectionString, (Func<string>) (() => provider.GetAccessToken(SqlConnectionInfoFactory.DatabaseConnectionInfoMsiAad.AzureResourceFromConnectionString(connectionString))))
      {
      }

      internal static string AzureResourceFromConnectionString(string connectionString)
      {
        string dataSource = new SqlConnectionStringBuilder(connectionString).DataSource;
        if (dataSource.Contains<char>(','))
          dataSource = dataSource.Split(',')[0];
        foreach (KeyValuePair<string, string> azureDomain in (IEnumerable<KeyValuePair<string, string>>) SqlConnectionInfoFactory.GetAzureDomainMap())
        {
          if (dataSource.EndsWith(azureDomain.Key))
            return azureDomain.Value;
        }
        throw new Exception("Could not resolve SQL Azure Resource Id for datasource: " + dataSource);
      }
    }

    private class DatabaseConnectionInfoHosted : 
      SqlConnectionInfoFactory.DatabaseConnectionInfo,
      ISupportPasswordReset,
      ISupportSqlCredential,
      ISupportInsecureConnectionString
    {
      private SqlCredential m_credential;
      private SecureString m_pendingPassword;

      internal DatabaseConnectionInfoHosted(string connectionString, SqlCredential credential)
        : base(connectionString)
      {
        this.m_credential = credential;
      }

      public override ISqlConnectionInfo Create(string connectionStringOverride) => (ISqlConnectionInfo) new SqlConnectionInfoFactory.DatabaseConnectionInfoHosted(connectionStringOverride, this.m_credential);

      protected override SqlConnection CreateSqlConnection() => new SqlConnection(this.ConnectionString, this.m_credential);

      [Obsolete("Do not call this method unless absolutely necessary for back compat. It is insecure and can potentially leak secrets.")]
      public string GetInsecureConnectionString()
      {
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(this.ConnectionString);
        IntPtr num = IntPtr.Zero;
        try
        {
          num = Marshal.SecureStringToBSTR(this.m_credential.Password);
          string stringBstr = Marshal.PtrToStringBSTR(num);
          connectionStringBuilder.UserID = this.m_credential.UserId;
          connectionStringBuilder.Password = stringBstr;
          return connectionStringBuilder.ConnectionString;
        }
        finally
        {
          Marshal.ZeroFreeBSTR(num);
        }
      }

      public string UserId => this.m_credential.UserId;

      public SecureString Password => this.m_credential.Password;

      public string PasswordChanging(IVssRequestContext requestContext, string newUnsecuredPassword)
      {
        if (this.m_pendingPassword != null)
          throw new InvalidOperationException(FrameworkResources.PasswordChangeAlreadyPending());
        requestContext.CheckServicingRequestContext();
        this.m_pendingPassword = Encoding.UTF8.GetBytes(newUnsecuredPassword).ToSecureString();
        return newUnsecuredPassword;
      }

      public void PasswordChanged(IVssRequestContext requestContext)
      {
        if (this.m_pendingPassword == null)
          throw new InvalidOperationException(FrameworkResources.PasswordChangeNotPending());
        requestContext.CheckServicingRequestContext();
        this.m_credential = new SqlCredential(this.m_credential.UserId, this.m_pendingPassword);
        SqlConnectionInfoFactory.UpdateSqlCredentialInCache(this.DataSource, this.m_credential, (byte[]) null);
        this.m_pendingPassword = (SecureString) null;
      }

      public void AbortPasswordChange(IVssRequestContext requestContext)
      {
        if (this.m_pendingPassword == null)
          throw new InvalidOperationException(FrameworkResources.PasswordChangeNotPending());
        requestContext.CheckServicingRequestContext();
        this.m_pendingPassword.Dispose();
        this.m_pendingPassword = (SecureString) null;
      }
    }
  }
}
