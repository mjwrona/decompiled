// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDatabaseExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class TeamFoundationDatabaseExtensions
  {
    public static readonly DatabasePoolExtendedProperties[] DbPoolExtendedProperties = new DatabasePoolExtendedProperties[12]
    {
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.CollectionImportPool, false, true, true, false, true, true, false, false),
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.CollectionImportSourcePool, true, false, true, false, false, false, false, false),
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.CollectionImportDacPacPool, false, false, true, false, false, false, false, false),
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.CollectionExportPool, false, false, false, true, false, false, false, false),
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.CollectionStagingPool, false, false, false, false, true, true, false, false),
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.DefaultPartitionPoolName, false, false, false, false, true, true, true, true),
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.BuildPartitionPoolName, false, false, false, false, true, true, true, true, "Build"),
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.WorkItemTrackingPartitionPoolName, false, false, false, false, true, true, true, true, "WorkItem"),
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.ConfigurationPoolName, false, false, false, false, true, true, true, true),
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.MigrationStagingPool, true, false, false, false, false, false, false, false),
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.RestrictedAcquisitionPartitionPool, false, false, false, false, true, true, true, true),
      new DatabasePoolExtendedProperties(DatabaseManagementConstants.NoUpgradePartitionPool, false, false, false, false, false, true, false, false)
    };
    public static readonly Dictionary<string, DatabasePoolExtendedProperties> DbPoolExtendedPropertiesMap = ((IEnumerable<DatabasePoolExtendedProperties>) TeamFoundationDatabaseExtensions.DbPoolExtendedProperties).ToDictionary<DatabasePoolExtendedProperties, string>((Func<DatabasePoolExtendedProperties, string>) (ep => ep.PoolName));
    public static readonly string s_TestPoolPrefix = "TestPool_";
    public const string EnableDataspaceDbSplitForNewAccountsFeatureFlag = "Microsoft.TeamFoundation.Framework.EnableDataspaceDbSplitForNewAccounts.Build";
    public const string AccountDbIsSplitFeatureFlag = "Microsoft.TeamFoundation.Framework.AccountDbIsSplit.Build";

    public static bool IsExternalDatabase(this ITeamFoundationDatabaseProperties db) => db.GetExtendedProperties("D:\\a\\_work\\1\\s\\Vssf\\Sdk\\Server\\DatabaseManagement\\TeamFoundationDatabaseExtensions.cs").IsExternal;

    public static bool IsStagingDatabase(this ITeamFoundationDatabaseProperties db) => db.GetExtendedProperties("D:\\a\\_work\\1\\s\\Vssf\\Sdk\\Server\\DatabaseManagement\\TeamFoundationDatabaseExtensions.cs").IsStaging;

    public static bool IsImportDatabase(this ITeamFoundationDatabaseProperties db) => db.GetExtendedProperties("D:\\a\\_work\\1\\s\\Vssf\\Sdk\\Server\\DatabaseManagement\\TeamFoundationDatabaseExtensions.cs").IsImport;

    public static bool IsExportDatabase(this ITeamFoundationDatabaseProperties db) => db.GetExtendedProperties("D:\\a\\_work\\1\\s\\Vssf\\Sdk\\Server\\DatabaseManagement\\TeamFoundationDatabaseExtensions.cs").IsExport;

    public static bool ShouldBuildDataspaceSplitOnCreate(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.TeamFoundation.Framework.EnableDataspaceDbSplitForNewAccounts.Build");

    public static bool BuildDataspaceIsSplit(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.TeamFoundation.Framework.AccountDbIsSplit.Build");

    public static string GetDatabaseIdServicingToken(string dataspaceCategory) => dataspaceCategory + "DatabaseId";

    public static bool IsEligibleForUpgrade(
      this ITeamFoundationDatabaseProperties db,
      bool allowUpgradingStagingDatabase = false)
    {
      DatabasePoolExtendedProperties extendedProperties = db.GetExtendedProperties("D:\\a\\_work\\1\\s\\Vssf\\Sdk\\Server\\DatabaseManagement\\TeamFoundationDatabaseExtensions.cs");
      if (string.IsNullOrEmpty(db.ServiceLevel) || db.Status != TeamFoundationDatabaseStatus.Online && db.Status != TeamFoundationDatabaseStatus.Servicing || !extendedProperties.CanUpgrade)
        return false;
      return allowUpgradingStagingDatabase || !extendedProperties.IsStaging;
    }

    public static bool IsEligibleForFailoverGroup(this ITeamFoundationDatabaseProperties db) => !string.IsNullOrEmpty(db.ServiceLevel) && db.GetExtendedProperties("D:\\a\\_work\\1\\s\\Vssf\\Sdk\\Server\\DatabaseManagement\\TeamFoundationDatabaseExtensions.cs").CanFailover;

    public static bool IsEligibleForAvailabilityZone(this ITeamFoundationDatabaseProperties db) => db.GetExtendedProperties("D:\\a\\_work\\1\\s\\Vssf\\Sdk\\Server\\DatabaseManagement\\TeamFoundationDatabaseExtensions.cs").CanUseAvailabilityZone;

    public static bool IsEligibleForRightSizing(this ITeamFoundationDatabaseProperties db) => !string.IsNullOrEmpty(db.ServiceLevel) && (db.Status == TeamFoundationDatabaseStatus.Online || db.Status == TeamFoundationDatabaseStatus.Servicing) && db.GetExtendedProperties("D:\\a\\_work\\1\\s\\Vssf\\Sdk\\Server\\DatabaseManagement\\TeamFoundationDatabaseExtensions.cs").CanRightSize;

    public static bool ShouldHaveDoNotDeleteLock(this ITeamFoundationDatabaseProperties db)
    {
      if (string.IsNullOrEmpty(db.ServiceLevel) || db.Status != TeamFoundationDatabaseStatus.Online && db.Status != TeamFoundationDatabaseStatus.Servicing)
        return false;
      return string.Equals(db.PoolName, DatabaseManagementConstants.ConfigurationPoolName, StringComparison.OrdinalIgnoreCase) || string.Equals(db.PoolName, DatabaseManagementConstants.DefaultPartitionPoolName, StringComparison.OrdinalIgnoreCase) || string.Equals(db.PoolName, DatabaseManagementConstants.BuildPartitionPoolName, StringComparison.OrdinalIgnoreCase) || string.Equals(db.PoolName, DatabaseManagementConstants.WorkItemTrackingPartitionPoolName, StringComparison.OrdinalIgnoreCase);
    }

    private static DatabasePoolExtendedProperties GetExtendedProperties(
      this ITeamFoundationDatabaseProperties db,
      [CallerFilePath] string sourceFilePath = "")
    {
      if (string.IsNullOrEmpty(db.PoolName) || db.PoolName.StartsWith(TeamFoundationDatabaseExtensions.s_TestPoolPrefix, StringComparison.OrdinalIgnoreCase))
        return DatabasePoolExtendedProperties.Default;
      DatabasePoolExtendedProperties extendedProperties;
      if (TeamFoundationDatabaseExtensions.DbPoolExtendedPropertiesMap.TryGetValue(db.PoolName, out extendedProperties))
        return extendedProperties;
      throw new InvalidOperationException("Extended properties have not been defined for database pool " + db.PoolName + ".  An extended property entry for the pool must be added to " + sourceFilePath);
    }

    public static void ExecuteDataTierOperation(
      this ITeamFoundationDatabaseProperties db,
      IVssRequestContext requestContext,
      Action<TeamFoundationDataTierComponent, TeamFoundationDataTierComponent> operation)
    {
      ArgumentUtility.CheckForNull<Action<TeamFoundationDataTierComponent, TeamFoundationDataTierComponent>>(operation, nameof (operation));
      ISqlConnectionInfo dboConnectionInfo = db.GetDboConnectionInfo();
      ISqlConnectionInfo connectionInfo1 = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, dboConnectionInfo.ConnectionString).ConnectionInfo.CloneReplaceInitialCatalog(dboConnectionInfo.InitialCatalog);
      ISqlConnectionInfo connectionInfo2 = connectionInfo1.CloneReplaceApplicationIntent(ApplicationIntent.ReadOnly);
      using (TeamFoundationDataTierComponent componentRaw1 = connectionInfo1.CreateComponentRaw<TeamFoundationDataTierComponent>())
      {
        using (TeamFoundationDataTierComponent componentRaw2 = connectionInfo2.CreateComponentRaw<TeamFoundationDataTierComponent>())
          operation(componentRaw1, componentRaw2);
      }
    }
  }
}
