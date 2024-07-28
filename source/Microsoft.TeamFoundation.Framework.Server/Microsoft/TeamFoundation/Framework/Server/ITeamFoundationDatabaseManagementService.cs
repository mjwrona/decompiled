// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationDatabaseManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationDatabaseManagementService))]
  public interface ITeamFoundationDatabaseManagementService : IVssFrameworkService
  {
    ITeamFoundationDatabaseProperties AcquireDatabasePartition(
      IVssRequestContext deploymentRequestContext,
      int databaseId,
      AcquirePartitionOptions acquireOptions,
      ITFLogger logger);

    ITeamFoundationDatabaseProperties AcquireDatabasePartition(
      IVssRequestContext deploymentRequestContext,
      string poolName,
      AcquirePartitionOptions acquireOptions,
      ITFLogger logger);

    ITeamFoundationDatabaseProperties GetDatabase(
      IVssRequestContext requestContext,
      int databaseId,
      bool useCache = true);

    TeamFoundationDatabasePool GetDatabasePool(IVssRequestContext requestContext, string poolName);

    bool TryGetDatabasePool(
      IVssRequestContext requestContext,
      string poolName,
      out TeamFoundationDatabasePool pool);

    AzureDatabaseProperties GetDatabaseProperties(ISqlConnectionInfo connectionInfo);

    void IncrementTenantsPendingDelete(
      IVssRequestContext requestContext,
      int databaseId,
      int tenantCount);

    List<ITeamFoundationDatabaseProperties> QueryDatabases(
      IVssRequestContext requestContext,
      bool useCache = true);

    List<ITeamFoundationDatabaseProperties> QueryDatabases(
      IVssRequestContext requestContext,
      string poolName,
      bool useCache = true);

    ITeamFoundationDatabaseProperties RegisterDatabase(
      IVssRequestContext requestContext,
      string connectionString,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      TeamFoundationDatabaseStatus status,
      DateTime? statusChangedDate,
      string statusReason,
      DateTime? lastTenantAdded,
      bool registerCredential,
      string userId,
      byte[] passwordEncrypted,
      Guid signingKeyId,
      TeamFoundationDatabaseFlags flags,
      string serviceObjective = null);

    void ReleaseDatabasePartition(
      IVssRequestContext requestContext,
      int databaseId,
      bool partitionDeleted);

    bool TryGetDatabaseProperties(
      IVssRequestContext requestContext,
      string datasource,
      string initialCatalog,
      out ITeamFoundationDatabaseProperties databaseProperties);

    void UpdateDatabaseProperties(
      IVssRequestContext requestContext,
      int databaseId,
      Action<TeamFoundationDatabaseProperties> action);

    bool ThrottleDatabaseAccess(
      IVssRequestContext deploymentContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      out string reason);

    ISqlConnectionInfo GetSqlConnectionInfo(IVssRequestContext requestContext, int databaseId);

    bool IsDowngradeOrDownsizeDisabled(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database,
      out DateTime downgradeDisabledUntilTime);

    void EnsureDatabaseMaintenanceJobCreated(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database);
  }
}
