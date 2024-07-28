// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemRegistryKey
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WorkItemRegistryKey
  {
    public const string Root = "/Service/WorkItemTracking/Settings/";
    public const string All = "/Service/WorkItemTracking/Settings/**";
    public const string TurnOffOptimizePartitionIdUnknown = "/Service/WorkItemTracking/Settings/TurnOffOptimizePartitionIdUnknown";
    public const string MaxRevisionsSupportedByGetWorkItemHistoryPath = "/Service/WorkItemTracking/Settings/MaxRevisionsSupportedByGetWorkItemHistory";
    public const string MaxRevisionsSupportedByUpdateWorkItemPath = "/Service/WorkItemTracking/Settings/MaxRevisionsSupportedByUpdateWorkItem";
    public const string MaxQueryResultSizePath = "/Service/WorkItemTracking/Settings/MaxResultsSize";
    public const string MaxQueryResultSizePathForPublicUser = "/Service/WorkItemTracking/Settings/MaxResultsSizeForPublicUser";
    public const string MaxQueryItemResultSizeForPartialHierarchyPath = "/Service/WorkItemTracking/Settings/MaxQueryItemResultsSizeForPartialHierarchyPath";
    public const string MaxQueryItemResultSizeForEntireHierarchyPath = "/Service/WorkItemTracking/Settings/MaxQueryItemResultsSizeForEntireHierarchy";
    public const string DefaultWebAccessQueryResultSizePath = "/Service/WorkItemTracking/Settings/DefaultWebAccessResultsSize";
    public const string MaxTrendChartTimeSliceResultSizePath = "/Service/WorkItemTracking/Settings/MaxTrendChartTimeSliceResultSize";
    public const string IsBisNotificationEnabledPath = "/Service/WorkItemTracking/Settings/BisNotification";
    public const string MaxAttachmentSizePath = "/Service/WorkItemTracking/Settings/MaxAttachmentSize";
    public const string MaxAttachmentSizePathForPublicUser = "/Service/WorkItemTracking/Settings/MaxAttachmentSizeForPublicUser";
    public const string MaxCreateWorkItemTagLimitPath = "/Service/WorkItemTracking/Settings/CreateWorkItemMaxTagLimit";
    public const string MaxLongTextSizePath = "/Service/WorkItemTracking/Settings/MaxLongTextSize";
    public const string MaxRevisionLongTextSizePath = "/Service/WorkItemTracking/Settings/MaxRevisionLongTextSize";
    public const string IsMetadataFilterEnabledPath = "/Service/WorkItemTracking/Settings/FilterMetadata";
    public const string ExcludedAgentsPath = "/Service/WorkItemTracking/Settings/ExcludedAgents";
    public const string MaxBuildListSizePath = "/Service/WorkItemTracking/Settings/MaxBuildListSize";
    public const string IsInProcBuildNotificationEnabledPath = "/Service/WorkItemTracking/Settings/IsProcBuildCompletionNotificationEnabled";
    public const string WorkItemQueryTimeoutPath = "/Service/WorkItemTracking/Settings/WorkItemQueryTimeout";
    public const string WorkItemQueryTimeoutReadReplicaPath = "/Service/WorkItemTracking/Settings/WorkItemQueryTimeoutReadReplica";
    public const string WorkItemQueryServicingTimeoutPath = "/Service/WorkItemTracking/Settings/WorkItemQueryServicingTimeout/db_{0}";
    public const string WorkItemQueryTimeoutAnonymousUserPath = "/Service/WorkItemTracking/Settings/WorkItemQueryTimeoutNoCrossProjectPermission";
    public const string MinClientToProvisionPath = "/Service/WorkItemTracking/Settings/MinClientVersionToProvision";
    public const string MaxGetQueriesDepth = "/Service/WorkItemTracking/Settings/MaxGetQueriesDepth";
    public const string MaxQueriesBatchSize = "/Service/WorkItemTracking/Settings/MaxQueriesBatchSize";
    public const string MaxQueryItemChildren = "/Service/WorkItemTracking/Settings/MaxQueryItemChildren";
    public const string WorkItemReclassificationTimeout = "/Service/WorkItemTracking/Settings/WorkItemReclassificationTimeout";
    public const string GetMetadataSoapTimeoutInSeconds = "/Service/WorkItemTracking/Settings/GetMetadataSoapTimeoutInSeconds";
    public const string MaxIdentityInGroupSize = "/Service/WorkItemTracking/Settings/MaxIdentityInGroupSize";
    public const string WorkItemReclassificationStatusCheckInterval = "/Service/WorkItemTracking/Settings/WorkItemReclassificationStatusCheckInterval";
    public const string MaxChunkSizeToUpdateWiqls = "/Service/WorkItemTracking/Settings/MaxChunkSizeToUpdateWiqls";
    public const string WorkItemSyncApiBatchSize = "/Service/WorkItemTracking/Settings/WorkItemSyncApiBatchSize";
    public const string WorkItemChangeWatermarkOffset = "/Service/WorkItemTracking/Settings/WorkItemChangeWatermarkOffset";
    public const string MaxDaysWorkItemInRecyclebin = "/Service/WorkItemTracking/Settings/MaxDaysWorkItemInRecyclebin";
    public const string WorkItemMaxJsonLength = "/Service/WorkItemTracking/Settings/WorkItemMaxJsonLength";
    public const string WorkItemMetadataCacheMaxAgeInDays = "/Service/WorkItemTracking/Settings/WorkItemMetadataCacheMaxAgeInDays";
    public const string ReclassificationSqlCommandTimeoutOverride = "/Service/WorkItemTracking/Settings/ReclassificationSqlCommandTimeoutOverride";
    public const string WebLayoutState = "/Service/WorkItemTracking/Settings/WebLayoutState";
    public const string WorkItemsChangedWithExtensionsBatchEventMaxSize = "/Service/WorkItemTracking/Settings/WorkItemsChangedWithExtensionsBatchEventMaxSize";
    public const string MaxQueryDOPValue = "/Service/WorkItemTracking/Settings/MaxQueryDOPValue";
    public const string QueryMaxGrantPercent = "/Service/WorkItemTracking/Settings/QueryMaxGrantPercent";
    public const string QueryInThreshold = "/Service/WorkItemTracking/Settings/QueryInThreshold";
    public const string TopLevelOrOptimizationMaxClauseNumber = "/Service/WorkItemTracking/Settings/TopLevelOrOptimizationMaxClauseNumber";
    public const string DataImportMode = "/Service/WorkItemTracking/Settings/DataImportMode";
    public const string ForceMatchOOBAccounts = "/Service/WorkItemTracking/Settings/ForceMatchOOBAccounts";
    public const string ConfigWebLayoutVersion = "/Service/WorkItemTracking/Settings/ConfigWebLayoutVersion";
    public const string CollectionWebLayoutVersion = "/Service/WorkItemTracking/Settings/CollectionWebLayoutVersion";
    public const string MaxNumberOfProjectsAllowedForImport = "/Service/WorkItemTracking/Settings/MaxNumberOfProjectsAllowedForImport";
    public const string WorkItemUpdateEventsAggregationTime = "/Service/WorkItemTracking/Settings/WorkItemUpdateEventsAggregationTime";
    public const string ReadReplicaUsers = "/Service/WorkItemTracking/Settings/ReadReplicaUsers";
    public const string ReadReplicaUserAgents = "/Service/WorkItemTracking/Settings/ReadReplicaUserAgents";
    public const string ReadReplicaEnabledCommands = "/Service/WorkItemTracking/Settings/ReadReplicaEnabledCommands";
    public const string ForcedReadReplicaCommands = "/Service/WorkItemTracking/Settings/ForcedReadReplicaCommands";
    public const string TemplateFieldValueMaxLength = "/Service/WorkItemTracking/Settings/TemplatesFieldValueMaxLength";
    public const string MaxNumberOfTemplatesPerWorkItemType = "/Service/WorkItemTracking/Settings/MaxNumberOfTemplatesPerWorkItemType";
    public const string ControlContributionInputLimit = "/Service/WorkItemTracking/Settings/ControlContributionInputLimit";
    public const string ForNotGroupMembershipCacheExpirationTimeInMinutes = "/Service/WorkItemTracking/Settings/ForNotGroupMembershipCacheExpirationTimeInMinutes";
    public const string WorkItemLinksLimit = "/Service/WorkItemTracking/Settings/WorkItemLinksLimit";
    public const string WorkItemRestoreLinksLimit = "/Service/WorkItemTracking/Settings/WorkItemRestoreLinksLimit";
    public const string WorkItemRemoteLinksLimit = "/Service/WorkItemTracking/Settings/WorkItemRemoteLinksLimit";
    public const string LinksCountCiThreshold = "/Service/WorkItemTracking/Settings/LinksCountCiThreshold";
    public const string MaxWiqlTextLengthPath = "/Service/WorkItemTracking/Settings/MaxWiqlTextLength";
    public const string MaxWiqlTextForMSProjectLengthPath = "/Service/WorkItemTracking/Settings/MaxWiqlTextForMSProjectLengthPath";
    public const string LinkUpdateNotificationThreshold = "/Service/WorkItemTracking/Settings/LinkUpdateNotificationThreshold";
    public const string MaxAllowedWorkItemAttachments = "/Service/WorkItemTracking/Settings/MaxAllowedWorkItemAttachments";
    public const string PromotePerProjectSleepTimeInSeconds = "/Service/WorkItemTracking/Settings/PromotePerProjectSleepTimeInSeconds";
    public const string RichTextFieldSerializationLength = "/Service/WorkItemTracking/Settings/RichTextFieldSerializationLength";
    public const string MaxSecretsScanContentLength = "/Service/WorkItemTracking/Settings/MaxSecretsScanContentLength";
    public const string MaxSecretsScanServiceRequestTimeoutInMilliseconds = "/Service/WorkItemTracking/Settings/MaxSecretsScanServiceRequestTimeoutInMilliseconds";
    public const string QueryOptimizations = "/Service/WorkItemTracking/Settings/QueryOptimizations";
    public const string IMSSyncTimeOutInSeconds = "/Service/WorkItemTracking/Settings/IMSSyncTimeOutInSeconds";
    public const string IMSSyncBatchSize = "/Service/WorkItemTracking/Settings/IMSSyncBatchSize";
    public const string QueryExecutionLogging = "/Service/WorkItemTracking/Settings/QueryExecutionLogging";
    public const string QueryOptimizationCache = "/Service/WorkItemTracking/Settings/QueryOptimizationCache";
    public const string QueryExperiment = "/Service/WorkItemTracking/Settings/QueryExperiment";
    public const string MaxWorkItemPageSize = "/Service/WorkItemTracking/Settings/WorkItemPageSize";
    public const string GetIdentityChangesPageSize = "/Service/WorkItemTracking/Settings/GetIdentityChangesPageSize";
    public const string MinimalImsSyncIntervalInSeconds = "/Service/WorkItemTracking/Settings/MinimalImsSyncIntervalInSeconds";
    public const string UncommittedLinkChangesLookbackWindowInSeconds = "/Service/WorkItemTracking/Settings/UncommittedLinkChangesLookbackWindowInSeconds";
    public const string WorkItemDestroyBatchSize = "/Service/WorkItemTracking/Settings/WorkItemDestroyBatchSize";
    public const string BackfillAndProvisionCommentCountMaxConcurrentJobCount = "/Service/WorkItemTracking/Settings/BackfillAndProvisionCommentCountMaxConcurrentJobCount";
    public const string BackfillCommentCountBatchSize = "/Service/WorkItemTracking/Settings/BackfillCommentCountBatchSize";
    public const string MaxNumberOfWorkItemsToProcessInExternalMentions = "/Service/WorkItemTracking/Settings/MaxNumberOfWorkItemsToProcessInExternalMentions";
    public const string ExternalConnectionMaxReposCountLimit = "/Service/WorkItemTracking/Settings/ExternalConnectionMaxReposCountLimit";
    public const string BadgeTimeOutInMilliseconds = "/Service/WorkItemTracking/Settings/BadgeTimeOutInMilliseconds";
    public const string BadgeBrowserCacheDurationInMinutes = "/Service/WorkItemTracking/Settings/BadgeBrowserCacheDurationInMinutes";
    public const string GetMetadataRequestedProjects = "/Service/WorkItemTracking/Settings/GetMetadataRequestedProjects";
    public const string WorkItemsDeleteBatchSize = "/Service/WorkItemTracking/Settings/WorkItemsDeleteBatchSize";
  }
}
