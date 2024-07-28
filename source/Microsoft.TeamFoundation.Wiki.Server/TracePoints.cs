// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.TracePoints
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class TracePoints
  {
    public const int WikiControllerCreateStart = 15250000;
    public const int WikiControllerPageIdPresent = 15250001;
    public const int WikiControllerCreateEnd = 15250099;
    public const int WikiPagesControllerStart = 15250200;
    public const int WikiPagesControllerEnd = 15250299;
    public const int WikiControllerGetStart = 15250300;
    public const int WikiControllerGetEnd = 15250399;
    public const int WikiAttachmentsControllerStart = 15250300;
    public const int WikiCreateAttachmentFailed = 15250301;
    public const int WikiAttachmentsControllerEnd = 15250399;
    public const int WikiPageMovesControllerStart = 15250400;
    public const int WikiPageMovesControllerPageIdProcess = 15250401;
    public const int WikiPageMovesControllerEnd = 15250499;
    public const int WikiPageIdNotFound = 15250600;
    public const int WikiPageIdNotAvaialble = 15250601;
    public const int WikiPageInvalidCharacters = 15250602;
    public const int WikiPageIdStart = 15250700;
    public const int WikiPageIdProcessingJobGenericError = 15250700;
    public const int WikiPageIdJobQueued = 15250701;
    public const int WikiOneTimeJobFeature = 15250702;
    public const int WikiPageIdProcessingJob = 15250703;
    public const int WikiPluginLoaderServiceGeneric = 15250704;
    public const int WikiPushJobUtilGeneric = 15250705;
    public const int WikiMentionsProcessorJobGenericError = 15250706;
    public const int WikiMentionsProcessorJobPageNotFoundError = 15250707;
    public const int WikiMentionsProcessorJobUserPermissionError = 15250708;
    public const int QueueWikiPushJobsGenericError = 15250709;
    public const int WikiMentionsProcessorJobInterestedTextError = 15250710;
    public const int WikiMentionsProcessorJobItemNotFoundError = 15250711;
    public const int WikiNotificationsStart = 15250800;
    public const int WikiFollowNotificationProcessingJobError = 15250801;
    public const int WikiFollowNotificationAggregationJobJobError = 15250802;
    public const int WikiFollowNotificationEmailFilterInvalidIndentity = 15250803;
    public const int WikiFollowNotificationEmailFilterEnter = 15250804;
    public const int WikiFollowNotificationEmailFilterLeave = 15250805;
    public const int WikiFollowNotificationEmailFilterVerified = 15250806;
    public const int WikiFollowNotificationEmailFilterDenied = 15250807;
    public const int WikiFollowNotificationEmailFilterException = 15250808;
    public const int WikiFollowNotificationEmailFilterInvalidDeliveryContents = 15250809;
    public const int WikiFollowNotificationProcessingMaxCountExceeded = 15250810;
    public const int WikiFollowNotificationFailedToFollow = 15250811;
    public const int WikiPageCommentsControllerStart = 15250900;
    public const int WikiPageCommentsControllerEnd = 15250999;
    public const int WikiPageCommentAttachmentsControllerStart = 15251000;
    public const int WikiPageCommentAttachmentsControllerEnd = 15251099;
    public const int WikiPageCommentReactionsControllerStart = 15251100;
    public const int WikiPageCommentReactionsControllerEnd = 15251199;
    public const int WikiPageCommentReactionsEngagedUsersControllerStart = 15251200;
    public const int WikiPageCommentReactionsEngagedUsersControllerEnd = 15251299;
    public const int WikiPagesBatchControllerStart = 15251300;
    public const int WikiPagesBatchControllerEnd = 15251399;
    public const int WikiPageStatsControllerStart = 15251400;
    public const int WikiPageStatsControllerEnd = 15251499;
    public const int WikiPagesOperationFailed = 15252800;
    public const int MigrateWikiPropertiesToV2Failed = 15252801;
    public const int WikiPageStatsOperationFailed = 15252802;
    public const int WikiUpdateFailed = 15252803;
    public const int WikiPageAggregatedViewsDeserializationFailed = 15252804;
    public const int WikiPageAggregatedViewsArtifactParsingFailed = 15252805;
    public const int LandingPageDataFetchFailed = 15252900;
    public const int AddSharedPermissionsFailed = 15252901;
    public const int FetchPartialTreeHierarchyPageNotFound = 15252902;
    public const int FetchPartialTreeHierarchyFailed = 15252903;
    public const int DataProviderGenericError = 15252904;
    public const int DataProviderPageView = 15252905;
    public const int ArtifactsVisitEventPublishedFailure = 15253000;
    public const int WikiPageViewsStart = 15254000;
    public const int WikiPageViewAggregationJobError = 15254001;
    public const int WikiMetaDataDeletionStart = 15255000;
    public const int WikiMetaDataDeletionJobError = 15255001;
  }
}
