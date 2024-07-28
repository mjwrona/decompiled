// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiTraceInfo
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class WikiTraceInfo
  {
    public const string Area = "Microsoft.TeamFoundation.Wiki.Server";
    public const string FeatureWikiUniquePageIds = "WikiUniquePageIds";
    public const string FeatureGetWikiUniquePageIds = "GetWikiUniquePageIds";
    public const string FollowNotificationAggregation = "NotificationAggregation";
    public const string FollowNotificationAggregationCount = "NotificationAggregationCount";
    public const string FollowNotificationAutoFollowPage = "AutoFollowPage";
    public const string FollowNotificationAutoFollowPageFailed = "AutoFollowPageFailed";
    public const string Action = "Action";
    public const string ActionWikiWikiPageIdProcessingJob = "WikiPageIdProcessingJob";
    public const string KeyIsWikiMappedTimeMs = "IsWikiMappedTimeMs";
    public const string KeyPageIdProcessingJobIsWikiAssociated = "PageIdProcessingJobIsWikiAssociated";
    public const string KeyNumWikiVersionsInRepo = "NumWikiVersionsInRepo";
    public const string KeyProcessAllWikiVersions = "ProcessAllWikiVersions";
    public const string KeyPushDiffTime = "PushDiffTimeTicks";
    public const string KeyPushDiffCount = "PushDiffTimeCount";
    public const string KeyAddedDiffCount = "AddedPageCount";
    public const string KeyDeletedDiffCount = "DeletedPageCount";
    public const string KeyRenamedDiffCount = "RenamedPageCount";
    public const string KeyEditedDiffCount = "EditedPageCount";
    public const string KeyEditRenamedDiffCount = "EditRenamedPageCount";
    public const string KeyPushPerWikiVersion = "PushPerWikiVersion";
    public const string KeyInterestedPushPerWikiVersionCount = "InterestedPushPerWikiVersionCount";
    public const string KeyOneTimeJobElapsedTimeMs = "OneTimeJobElapsedTimeMs";
    public const string KeyOneTimeJobRequeueCount = "OneTimeJobRequeueCount";
    public const string KeyWikiPublishedPagesCount = "WikiPublishedPagesCount";
    public const string KeyWikiUnpublishedVersionsCount = "WikiUnpublishedVersionsCount";
    public const string KeyWikiOneTimeJobMaxLimit = "WikiOneTimeJobMaxLimit";
    public const string KeyWikiBranchDeletedCount = "WikiBranchDeletedCount";
    public const string KeyWikiBranchRecreatedCount = "WikiBranchRecreatedCount";
    public const string KeyGetWikiPushWaterMark = "GetWikiPushWaterMark";
    public const string KeyGetNextRefLogEntries = "GetNextRefLogEntries";
    public const string KeyGetEditedPages = "GetEditedPages";
    public const string KeyConstructWikiPushJobData = "ConstructWikiPushJobData";
    public const string KeyUpdatePageId = "UpdatePageId";
    public const string KeyDeleteAllWikiPageIds = "DeleteAllWikiPageIds";
    public const string KeyPublishWikiPages = "PublishWikiPages";
    public const string KeyRepoHasAssociatedWiki = "RepoHasAssociatedWiki";
    public const string KeyGetFlattenedPagePathList = "GetFlattenedPagePathList";
    public const string KeyGetWikiById = "GetWikiById";
    public const string KeyWikiPageIdGenerationServicingStep = "PageIdGenerationServicingStep";
    public const string KeyPushAlreadyProcessed = "PushAlreadyProcessed";
    public const string KeyTooManyPushesPending = "TooManyPushesPending";
    public const string KeyGetPageIdByPath = "GetPageIdByPath";
    public const string KeyPageIdFound = "PageIdFound";
    public const string KeyUnPublishWikiVersion = "UnPublishWikiVersion";
    public const string KeyWarningPushNotLatest = "Warning_PushNotLatest";
    public const string KeyGetPageIdDetails = "GetPageIdDetails";
    public const string KeyWikiPushJobExtensionsCount = "WikiPushJobExtensionsCount";
    public const string KeyHasPageId = "HasPageId";
    public const string KeyCallerName = "CallerName";
    public const string KeyWikiId = "WikiId";
    public const string KeyVersion = "Version";
    public const string KeyWikiType = "WikiType";
    public const string KeyPushTimeUtc = "PushTimeUtc";
    public const string KeyPageIdJobTimeUtc = "PageIdJobTimeUtc";
    public const string KeyPageIdDelayInms = "PageIdDelayInms";
    public const string KeyWikiMentionsProcessorJob = "WikiMentionsProcessorJob";
    public const string KeyGetWikiPageMentionCandidates = "GetWikiPageMentionCandidates";
    public const string KeyPageMentionGetInterestedText = "PageMentionGetInterestedText";
    public const string KeyWikiMentionsProcessorJobResult = "WikiMentionsProcessorJobResult";
    public const string KeyRepoId = "RepoId";
    public const string KeyPushId = "PushId";
    public const string KeyErrorUnknownVersionType = "Error_UnknownVersionType";
    public const string KeyErrorInvalidRefLog = "Error_InvalidRefLog";
    public const string KeyWikiNotFound = "Error_WikiNotFound";
    public const string KeyWikiVersionNotFound = "Error_WikiVersionNotFound";
    public const string KeyOneTimeJobRequeueLimitReached = "OneTimeJobRequeueLimitReached";
    public const string KeyErrorRefLogNotFound = "Error_RefLogNotFound";
    public const string KeyErrorInvalidTfsGitDiff = "Error_InvalidTfsGitDiff";
    public const string KeyErrorWikiVersionExists = "Error_WikiVersionExists";
    public const string KeyErrorWaterMarkAbsent = "Error_WaterMarkAbsent";
    public const string KeyErrorUnableToUnpublishVersion = "Error_UnableToUnpublishVersion";
    public const string KeyErrorInvalidFilePath = "Error_InvalidFilePath";
    public const string KeyErrorPageIdSyncError = "Error_PageIdSyncError";
    public const string KeyErrorWikiPushJobUtil = "Error_WikiPushJobUtil";
  }
}
