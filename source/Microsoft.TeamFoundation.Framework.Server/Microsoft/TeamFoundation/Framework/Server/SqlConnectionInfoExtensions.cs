// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlConnectionInfoExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SqlConnectionInfoExtensions
  {
    public const int DefaultCommandTimeout = 3660;

    public static ISqlConnectionInfo CloneReplaceInitialCatalog(
      this ISqlConnectionInfo connectionInfo,
      string initialCatalog)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionInfo.ConnectionString)
      {
        InitialCatalog = initialCatalog
      };
      return connectionInfo.Create(connectionStringBuilder.ConnectionString);
    }

    public static ISqlConnectionInfo CloneReplaceDataSource(
      this ISqlConnectionInfo connectionInfo,
      string dataSource)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionInfo.ConnectionString)
      {
        DataSource = dataSource
      };
      return connectionInfo.Create(connectionStringBuilder.ConnectionString);
    }

    public static ISqlConnectionInfo CloneReplaceApplicationIntent(
      this ISqlConnectionInfo connectionInfo,
      ApplicationIntent applicationIntent)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionInfo.ConnectionString)
      {
        ApplicationIntent = applicationIntent
      };
      return connectionInfo.Create(connectionStringBuilder.ConnectionString);
    }

    public static ISqlConnectionInfo CloneReplaceDataSourceAndApplicationIntent(
      this ISqlConnectionInfo connectionInfo,
      string dataSource,
      ApplicationIntent applicationIntent)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionInfo.ConnectionString)
      {
        DataSource = dataSource,
        ApplicationIntent = applicationIntent
      };
      return connectionInfo.Create(connectionStringBuilder.ConnectionString);
    }

    public static TComponent CreateComponentRaw<TComponent>(
      this ISqlConnectionInfo connectionInfo,
      int commandTimeout = 3660,
      int deadlockPause = 200,
      int maxDeadlockRetries = 25,
      bool handleNoResourceManagementSchema = false,
      ITFLogger logger = null)
      where TComponent : class, ISqlResourceComponent, new()
    {
      return TeamFoundationResourceManagementService.CreateComponentRaw<TComponent>(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, handleNoResourceManagementSchema, logger);
    }

    [Obsolete("This function recreates the full connection string with decrypted secrets if present. Is should only be used in servicing situations where there is no alternative.")]
    public static string GetFullConnectionStringInsecure(this ISqlConnectionInfo connectionInfo)
    {
      if (new SqlConnectionStringBuilder(connectionInfo.ConnectionString).IntegratedSecurity)
        return connectionInfo.ConnectionString;
      switch (connectionInfo)
      {
        case SqlConnectionInfoFactory.DatabaseConnectionInfoAADAccessToken _:
          return connectionInfo.ConnectionString;
        case ISupportInsecureConnectionString _:
          return ((ISupportInsecureConnectionString) connectionInfo).GetInsecureConnectionString();
        default:
          throw new InvalidOperationException(FrameworkResources.DatabaseManagementComponentVersionMismatch());
      }
    }
  }
}
