// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IWorkItemTrackingConfigurationInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public interface IWorkItemTrackingConfigurationInfo
  {
    bool FullTextEnabled { get; }

    bool EventsEnabled { get; }

    int MaxIdentityInGroupSize { get; }

    int MaxQueryResultSize { get; }

    int MaxQueryResultSizeForPublicUser { get; }

    int MaxQueryItemResultSizeforPartialHierarchy { get; }

    int MaxQueryItemResultSizeForEntireHierarchy { get; }

    int MaxWiqlTextLength { get; }

    int MaxWiqlTextLengthForDataImport { get; }

    int MaxWiqlTextLengthForMSProject { get; }

    int MaxAllowedWorkItemAttachments { get; }

    int DefaultWebAccessQueryResultSize { get; }

    int MaxTrendChartTimeSliceResultSize { get; }

    long MaxAttachmentSize { get; }

    long MaxAttachmentSizeForPublicUser { get; }

    int MaxWorkItemTagLimit { get; }

    int WorkItemQueryTimeoutInSecond { get; }

    int WorkItemQueryTimeoutReadReplicaInSecond { get; }

    int GetWorkItemQueryTimeoutInSecond(bool useReadReplica);

    int WorkItemQueryTimeoutAnonymousUser { get; }

    int MaxGetQueriesDepth { get; }

    int MaxQueriesBatchSize { get; }

    int MaxQueryItemChildrenUnderParent { get; }

    int MaxBuildListSize { get; }

    int MaxLongTextSize { get; }

    int MaxRevisionLongTextSize { get; }

    int MaxRevisionsSupportedByGetWorkItemHistory { get; }

    int MaxRevisionsSupportedByUpdateWorkItem { get; }

    int MinClientVersionToProvision { get; }

    bool IsInProcBuildCompletionNotificationEnabled { get; }

    bool MetadataFilterEnabled { get; }

    int WorkItemReclassificationTimeout { get; }

    int GetMetadataSoapTimeoutInSeconds { get; }

    StringComparer ServerStringComparer { get; }

    CultureInfo ServerCulture { get; }

    int WorkItemReclassificationStatusCheckInterval { get; }

    int WorkItemSyncApiBatchSize { get; }

    long WorkItemChangeWatermarkOffset { get; }

    IWitProcessTemplateValidatorConfiguration ValidatorConfiguration { get; }

    int WorkItemMaxJsonLength { get; }

    int WorkItemMetadataCacheMaxAgeInDays { get; }

    int? ReclassificationSqlCommandTimeoutOverride { get; }

    int MaxQueryDOPValue { get; }

    int MaxWorkItemPageSize { get; }

    int QueryMaxGrantPercent { get; }

    int QueryInThreshold { get; }

    int TopLevelOrOptimizationMaxClauseNumber { get; }

    int ConfigWebLayoutVersion { get; }

    int CollectionWebLayoutVersion { get; }

    int WorkItemUpdateEventsAggregationTime { get; }

    WorkItemTrackingReadReplicaConfiguration ReadReplicaSettings { get; }

    WorkItemTrackingQueryOptimizationConfiguration QueryOptimizationSettings { get; }

    WorkItemTrackingQueryExecutionLoggingConfiguration QueryExecutionLoggingConfig { get; }

    int ControlContributionInputLimit { get; set; }

    int WorkItemLinksLimit { get; }

    int WorkItemRestoreLinksLimit { get; }

    int WorkItemRemoteLinksLimit { get; }

    int LinksCountCiThreshold { get; }

    int LinkUpdateNotificationThreshold { get; }

    int PromotePerProjectSleepTimeInSeconds { get; }

    int RichTextFieldSerializationLength { get; }

    int MaxSecretsScanContentLength { get; }

    int MaxSecretsScanServiceRequestTimeoutInMilliseconds { get; }

    HashSet<string> EmptyAliases { get; }

    int GetIdentityChangesPageSize { get; }

    int MinimalImsSyncIntervalInSeconds { get; }

    int MaxNumberOfWorkItemsToProcessInExternalMentions { get; }

    int UncommittedLinkChangesLookbackWindowInSeconds { get; }

    int ExternalConnectionMaxReposCountLimit { get; }

    int BadgeTimeOutInMilliseconds { get; }

    int BadgeBrowserCacheDurationInMinutes { get; }
  }
}
