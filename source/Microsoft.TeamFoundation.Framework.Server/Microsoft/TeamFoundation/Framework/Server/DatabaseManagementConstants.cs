// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class DatabaseManagementConstants
  {
    public static readonly string CollectionImportPool = nameof (CollectionImportPool);
    public static readonly string CollectionImportSourcePool = nameof (CollectionImportSourcePool);
    public static readonly string CollectionImportDacPacPool = nameof (CollectionImportDacPacPool);
    public static readonly string CollectionExportPool = nameof (CollectionExportPool);
    public static readonly string CollectionStagingPool = nameof (CollectionStagingPool);
    public static readonly string DefaultPartitionPoolName = "DefaultPartitionPool";
    public static readonly string BuildPartitionPoolName = "BuildPartitionPool";
    public static readonly string WorkItemTrackingPartitionPoolName = "WorkItemTrackingPartitionPool";
    public static readonly string ConfigurationPoolName = "ConfigurationPool";
    public static readonly string MigrationStagingPool = nameof (MigrationStagingPool);
    public static readonly string RestrictedAcquisitionPartitionPool = nameof (RestrictedAcquisitionPartitionPool);
    public static readonly string NoUpgradePartitionPool = nameof (NoUpgradePartitionPool);
    public static readonly string PartitionDeletionLockPrefix = "PartitionDeletionLock_";
    public static readonly string DefaultServiceObjective = "S0";
    public static readonly int InvalidDatabaseId = 0;
    public static readonly int InvalidDatabaseVersion = 0;
    public static readonly int UnassignedPoolId = -1;
    public static readonly short InvalidThreshold = -1;
    public static readonly short DefaultDTDataStaleSeconds = 300;
    public static readonly short DefaultDTAvgCpuPercentThreshold = 80;
    public static readonly short DefaultDTAvgDataIOPercentThreshold = 80;
    public static readonly short DefaultDTAvgLogWritePercentThreshold = 80;
    public static readonly short DefaultDTMaxWorkerPercentThreshold = 80;
    public static readonly short DefaultDTAvgMemoryUsagePercentThreshold = DatabaseManagementConstants.InvalidThreshold;
    public static readonly int DefaultPageLatchAverageWaitTimeMSThreshold = 3;
    internal static readonly double PerfCacheRefreshInterval = 30.0;
    public static readonly Guid DatabasePrecreationJobId = new Guid("DE9181C6-5EEE-4BA3-AE81-43D7C254DD2A");
    public static readonly Guid DatabaseMaintenanceBaseJobId = new Guid("E9159747-0D1E-4F40-A690-FE656262D418");
    public static readonly TeamFoundationDatabaseLoggingOptions DefaultDatabaseLoggingOptions = TeamFoundationDatabaseLoggingOptions.Statistics;
  }
}
