// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsPerfCounters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class TfsPerfCounters
  {
    private const string UriBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_";
    internal const string CurrentServerRequests = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentServerRequests";
    internal const string CurrentRequestsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentRequestsPerSec";
    internal const string AverageResponseTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageResponseTime";
    internal const string AverageResponseTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageResponseTimeBase";
    internal const string ConfigDBActiveSqlConnections = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ConfigDBActiveSqlConnections";
    internal const string AverageSqlConnectionTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlConnectionTime";
    internal const string AverageSqlConnectionTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlConnectionTimeBase";
    internal const string CurrentSqlConnectionFailures = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentSqlConnectionFailures";
    internal const string CurrentSqlConnectionRetries = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentSqlConnectionRetries";
    internal const string CurrentSqlExecutionRetries = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentSqlExecutionRetries";
    internal const string ActiveDeploymentHosts = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ActiveDeploymentHosts";
    internal const string ActiveApplicationHosts = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ActiveApplicationHosts";
    internal const string ActiveCollectionHosts = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ActiveCollectionHosts";
    internal const string TaskExecutedPerSecond = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_TaskExecutedPerSecond";
    internal const string CurrentSQLNotificationQueriesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentSQLNotificationQueriesPerSec";
    internal const string ConfigDBCurrentSQLExecutionsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ConfigDBCurrentSQLExecutionsPerSec";
    internal const string TotalSqlBatches = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalSqlBatches";
    internal const string TotalThrottlingEvents = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalThrottlingEvents";
    internal const string TotalFailedRetrySequences = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalFailedRetrySequences";
    internal const string CurrentRunningJobs = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentRunningJobs";
    internal const string TotalRunningJobs = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalRunningJobs";
    internal const string WaitingOnPingResponse = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_WaitingOnPingResponse";
    internal const string RequestsForDormantCollectionsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_RequestsForDormantCollectionsPerSec";
    internal const string TotalCacheHits = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalCacheHits";
    internal const string TotalCacheMisses = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalCacheMisses";
    internal const string AverageCacheLatencyTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageCacheLatencyTime";
    internal const string AverageCacheLatencyTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageCacheLatencyTimeBase";
    internal const string TotalCacheCalls = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalCacheCalls";
    internal const string CurrentMSDNCallsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentMSDNCallsPerSec";
    internal const string CurrentMSDNCallsExecuting = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentMSDNCallsExecuting";
    internal const string AverageMSDNResponseTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageMSDNResponseTime";
    internal const string AverageMSDNResponseTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageMSDNResponseTimeBase";
    internal const string AzureStorageUploadedBytesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageUploadedBytesPerSec";
    internal const string AzureStorageUploadsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageUploadsPerSec";
    internal const string AzureStorageUploadsExecuting = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageUploadsExecuting";
    internal const string AzureStorageDownloadedBytesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDownloadedBytesPerSec";
    internal const string AzureStorageDownloadsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDownloadsPerSec";
    internal const string AzureStorageDownloadsExecuting = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDownloadsExecuting";
    internal const string AzureStorageDeletesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDeletesPerSec";
    internal const string AzureStorageDeletesExecuting = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDeletesExecuting";
    internal const string AzureStorageRenamesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageRenamesPerSec";
    internal const string AzureStorageRenamesExecuting = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageRenamesExecuting";
    internal const string CurrentACSCallsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentACSCallsPerSec";
    internal const string CurrentACSCallsExecuting = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentACSCallsExecuting";
    internal const string AverageACSResponseTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageACSResponseTime";
    internal const string AverageACSResponseTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageACSResponseTimeBase";
    internal const string SigningServiceDecryptsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceDecryptsPerSec";
    internal const string SigningServiceAvgDecryptTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgDecryptTime";
    internal const string SigningServiceAvgDecryptTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgDecryptTimeBase";
    internal const string SigningServiceEncryptsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceEncryptsPerSec";
    internal const string SigningServiceAvgEncryptTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgEncryptTime";
    internal const string SigningServiceAvgEncryptTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgEncryptTimeBase";
    internal const string SigningServiceSignsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceSignsPerSec";
    internal const string SigningServiceAvgSigningTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgSigningTime";
    internal const string SigningServiceAvgSigningTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgSigningTimeBase";
    internal const string SigningServiceVerifiesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceVerifiesPerSec";
    internal const string TimeSinceLastNotification = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_TimeSinceLastNotification";
    internal const string LastNotificationQueueLength = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_LastNotificationQueueLength";
    internal const string AverageSqlNotificationTaskTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlNotificationTaskTime";
    internal const string AverageSqlNotificationTaskTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlNotificationTaskTimeBase";
    internal const string AverageSqlNotificationTaskCallbackTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlNotificationTaskCallbackTime";
    internal const string AverageSqlNotificationTaskCallbackTimeBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlNotificationTaskCallbackTimeBase";
    internal const string AzureStorageUploadsFailed = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageUploadsFailed";
    internal const string AzureStorageUploadsFailedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageUploadsFailedPerSec";
    internal const string AzureStorageDownloadsFailed = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDownloadsFailed";
    internal const string AzureStorageDownloadsFailedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDownloadsFailedPerSec";
    internal const string AzureStorageDeletesFailed = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDeletesFailed";
    internal const string AzureStorageDeletesFailedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDeletesFailedPerSec";
    internal const string AzureStorageRenamesFailed = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageRenamesFailed";
    internal const string AzureStorageRenamesFailedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageRenamesFailedPerSec";
    internal const string CancelledServerRequestsTotal = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CancelledServerRequestsTotal";
    internal const string CancelledRequestsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CancelledRequestsPerSec";
    internal const string DeploymentRegistryCacheTotal = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_DeploymentRegistryCacheTotal";
    internal const string DeploymentRegistryCacheHitsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_DeploymentRegistryCacheHitsPerSec";
    internal const string DeploymentRegistryQueriesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_DeploymentRegistryQueriesPerSec";
    internal const string RegistryCacheHitsPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_RegistryCacheHitsPerSec";
    internal const string RegistryQueriesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_RegistryQueriesPerSec";
    internal const string TotalSecurityRefreshesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalSecurityRefreshesPerSec";
    internal const string LocalSecurityRefreshesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_LocalSecurityRefreshesPerSec";
    internal const string SystemSecurityRefreshesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_SystemSecurityRefreshesPerSec";
    internal const string RemoteSecurityRefreshesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_RemoteSecurityRefreshesPerSec";
    internal const string DeploymentRegistrySubscriptionsTotal = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_DeploymentRegistrySubscriptionsTotal";
    internal const string AzureFrontDoorCacheMisses = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureFrontDoorCacheMisses";
    internal const string LegacyFedAuthCookiesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_LegacyFedAuthCookiesPerSec";
    internal const string NewFedAuthCookiesPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_NewFedAuthCookiesPerSec";
    internal const string ImpactedUsersInLastMinuteMetric = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_ImpactedUsersInLastMinuteMetric";
  }
}
