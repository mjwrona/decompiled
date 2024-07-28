// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRegistryPaths
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GitRegistryPaths
  {
    public const string GitServiceRoot = "/Service/Git";
    public const string ForceCloneHack = "/Service/Git/Settings/ForceCloneHack";
    public const string MaxMergeableSizeSetting = "/Service/Git/Settings/MergeableSize";
    public const string MaxGitmoduleFileSizeSetting = "/Service/Git/Settings/GitmoduleFileSizeCap";
    public const string NumDaysBeforeFilesAreDeletable = "/Service/Git/Settings/NumDaysBeforeFilesAreDeletable";
    public const string NumIntervalsBeforeFilesAreDeletable = "/Service/Git/Settings/NumberOfIntervalsBeforeFilesAreDeletable";
    public const string NumDaysBeforeDeletingFiles = "/Service/Git/Settings/NumDaysBeforeDeletingFiles";
    public const string NumIntervalsBeforeDeletingFiles = "/Service/Git/Settings/NumberOfIntervalsBeforeDeletingFiles";
    public const string NumParallelPushThreads = "/Service/Git/Settings/NumberOfParallelPushThreads";
    public const string NumParallelOdbFsckThreads = "/Service/Git/Settings/NumParallelOdbFsckThreads";
    public const string NumParallelGitPackerThreads = "/Service/Git/Settings/NumParallelGitPackerThreads";
    public const string GvfsAuthorizationLifetime = "/Service/Git/Settings/GVFS/AuthorizationLifetime";
    public const string CreatePullRequestWithIterationTimeoutSeconds = "/Service/Git/Settings/CreatePullRequestWithIterationTimeoutSeconds";
    public const string MinimumRecommendedGitVersion = "/Service/Git/Settings/MinimumRecommendedGitVersion";
    public const string UserAgentExemptions = "/Service/Git/Settings/UserAgentExemptions";
    public const string UpdateGitMessage = "/Service/Git/Settings/UpdateGitMessage";
    public const string MaxMigrationJobConcurrency = "/Service/Git/Settings/MaxMigrationJobConcurrency";
    public const string MigrationRequeueDelayMinutes = "/Service/Git/Settings/MigrationRequeueDelayMinutes";
    public const string EnumerateBlobsClientTimeout = "/Service/Git/Settings/EnumerateBlobsClientTimeout";
    public const string GitImportContainerIdFlag = "/Temporary/Git/Import/RegeneratedContainerIds/{0}";
    public const string CodeReviewServiceDependency = "/Service/Git/CodeReview/{0}/{1}";
    public const string MaxCompletionMergeAttempts = "/Service/Git/Settings/MaxCompletionMergeAttempts";
    public const string MaxAsyncRefOperationRetryAttempts = "/Service/Git/Settings/MaxAsyncRefOperationRetryAttempts";
    public const string MergeConflictFindRenames = "/Service/Git/Settings/MergeConflictFindRenames";
    public const string MergeConflictRenameThreshold = "/Service/Git/Settings/MergeConflictRenameThreshold";
    public const string MergeConflictTargetLimit = "/Service/Git/Settings/MergeConflictTargetLimit";
    public const string PullRequest_BuildPolicyMinimumRequeueDelaySeconds = "/Service/Git/Settings/PullRequest/BuildPolicyMinimumRequeueDelaySeconds";
    public const string PullRequest_BuildPolicyQueueLeaseTimeSeconds = "/Service/Git/Settings/PullRequest/BuildPolicyQueueLeaseTimeSeconds";
    public const string PullRequest_MaxFilesSyncPrCreatePolicyNotification = "/Service/Git/Settings/PullRequest/MaxFilesSyncPrCreatePolicyNotification";
    public const string PullRequestNumberOfDaysToAutoMergePRonPush = "/Service/Git/Settings/PullRequest/NumberOfDaysToAutoMergeOnPush";
    public const string DaysToRetainMetrics = "/Service/Git/Settings/DaysToRetainMetrics";
    public const string RepoSizeJobDelaySeconds = "/Service/Git/Settings/RepoSizeJobDelaySeconds";
    public const string RepoInMaintenanceMessage = "/Service/Git/Setting/RepoInMaintenanceMessage";
    public const string CacheWarmupMaxConcurrentRepos = "/Configuration/CacheWarmup/Git/MaxConcurrentRepos";
    public const string CacheWarmupDelayVipSwapTimeoutInMinutes = "/Configuration/CacheWarmup/Git/DelayVipSwapTimeoutInMinutes";
    public const string GitCacheWarmupRoot = "/Configuration/CacheWarmup/Git/HostAndRepo";
    public const string GitCacheWarmupFilter = "/Configuration/CacheWarmup/Git/HostAndRepo/*";
    public const string GitCacheWarmupDelayVipSwapRoot = "/Configuration/CacheWarmup/Git/DelayVipSwapHostAndRepo";
    public const string GitCacheWarmupDelayVipSwapFilter = "/Configuration/CacheWarmup/Git/DelayVipSwapHostAndRepo/*";
    public const string PackIndexMaxSize = "/Service/Git/Configuration/PackIndex/MaxSize";
    public const string PackIndexMaxPackCount = "/Service/Git/Configuration/PackIndex/MaxPackCount";
    public const string FullRepackNoReuseIndexesReposFolder = "/Service/Git/FullRepackNoReuseIndexes";
    public const string FullRepackNoReusePacksReposFolder = "/Service/Git/FullRepackNoReusePacks";
    public const string FullRepackBatchSizeReposFolder = "/Service/Git/FullRepackBatchSize";
    public const string FullRepackLastKnownObjectId = "/Service/Git/FullRepackLastKnownObjectId";
    public const string RedeltifyReposFolder = "/Service/Git/RedeltifyGitRepository";
    public const string RepairReposFolder = "/Service/Git/RepairGitRepository";
    public const string ForceRecomputeRepoGraphFolder = "/Service/Git/ForceRecomputeGitRepositoryGraph";
    public const string MaxEventsForPush = "/Service/Git/Settings/MaxEventsForPush";
    public const string MaxCommitsForPushMetric = "/Service/Git/Settings/MaxCommitsForPushMetric";
    public const string PushEventAffectedFoldersProcessLimit = "/Service/Git/Settings/PushEventAffectedFoldersProcessLimit";
    public const string PushEventAffectedFoldersReportedLimit = "/Service/Git/Settings/PushEventAffectedFoldersReportedLimit";
    public const string PushEventMofidedFilesReportedLimit = "/Service/Git/Settings/PushEventModifiedFilesReportedLimit";
    public const string PullRequestShareMaxReceivers = "/Service/Git/Settings/PullRequestShareMaxReceivers";
    public const string PullRequestMaxReviewersCount = "/Service/Git/Settings/PullRequestMaxReviewersCount";
    public const string PullRequestShareMaxMessaage = "/Service/Git/Settings/PullRequestShareMaxMessage";
    public const string PullRequestCommentEventMaxComments = "/Service/Git/Settings/PullRequestCommentEventMaxComments";
    public const string PullRequestVoteEventMaxThreads = "/Service/Git/Settings/PullRequestVoteEventMaxThreads";
    public const string PullRequestVoteEventMaxComments = "/Service/Git/Settings/PullRequestVoteEventMaxComments";
    public const string PullRequestLabelsFixedVersion = "/Service/Git/Settings/PullRequestLabelsFixedVersion";
    public const string ConcurrentCloneTarpit = "/Service/Git/Settings/ConcurrentCloneSelfThrottling/Tarpit";
    public const string ConcurrentCloneWorkUnitSize = "/Service/Git/Settings/ConcurrentCloneSelfThrottling/WorkUnitSize";
    public const string ConcurrentCloneSize = "/Service/Git/Settings/ConcurrentCloneSelfThrottling/Size";
    public const string MaxPushSize = "/Service/Git/Settings/MaxPushSize";
    public const string MaxPathLength = "/Service/Git/Settings/MaxPathLength";
    public const string MaxPathComponentLength = "/Service/Git/Settings/MaxPathComponentLength";
    public const string NumUnpackedFilesBeforeRepack = "/Service/Git/Settings/NumUnpackedFilesBeforeRepack";
    public const string TaggedPRFromUserIgnoresApprovers = "/Service/Git/Settings/UserIgnoresApprovers";
    public const string MaxCommitEntriesCount = "/Service/Git/Settings/MaxCommitEntriesCount";
    public const string HeartbeatIntervalMillis = "/Service/Git/Settings/HeartbeatIntervalMillis";
    public const string HeartbeatMinimumWriteThresholdMillis = "/Service/Git/Settings/HeartbeatMinimumWriteThresholdMillis";
    public const string GitNativeMergeJobPercentageOfCoresConcurrencyLimit = "/Service/Git/Settings/GitNativeMergeJob/PercentageOfCoresConcurrencyLimit";
    public const string PushProtectionExceptionList = "/Service/Git/Settings/PushProtectionExceptionList";
    public const string GitHfsFileReadSize = "/Service/Git/Settings/GitHfsFileReadSize";
    public const string AdvSecCommitScanningBatchScanTargetCompressionLevel = "/Service/Git/Settings/AdvSec/CommitScanning/BatchScanTargetCompressionLevel";
    public const string AdvSecCommitScanningCommitTimeout = "/Service/Git/Settings/AdvSec/CommitScanning/CommitTimeout";
    public const string AdvSecCommitScanningDisabledHostIds = "/Service/Git/Settings/AdvSec/CommitScanning/DisabledHostIds";
    public const string AdvSecCommitScanningDisabledProjectIds = "/Service/Git/Settings/AdvSec/CommitScanning/DisabledProjectIds";
    public const string AdvSecCommitScanningDisabledRepositoryIds = "/Service/Git/Settings/AdvSec/CommitScanning/DisabledRepositoryIds";
    public const string AdvSecCommitScanningIterationTimeProportion = "/Service/Git/Settings/AdvSec/CommitScanning/IterationTimeProportion";
    public const string AdvSecCommitScanningJobTimeout = "/Service/Git/Settings/AdvSec/CommitScanning/JobTimeout";
    public const string AdvSecCommitScanningMaxAttemptCount = "/Service/Git/Settings/AdvSec/CommitScanning/MaxAttemptCount";
    public const string AdvSecCommitScanningMaxBatchSizeInMb = "/Service/Git/Settings/AdvSec/CommitScanning/MaxBatchSizeInMb";
    public const string AdvSecCommitScanningMaxCommitsToProcess = "/Service/Git/Settings/AdvSec/CommitScanning/MaxCommitsToProcess";
    public const string AdvSecCommitScanningMaxFileSizeInMb = "/Service/Git/Settings/AdvSec/CommitScanning/MaxFileSizeInMb";
    public const string AdvSecCommitScanningMaxIterationCount = "/Service/Git/Settings/AdvSec/CommitScanning/MaxIterationCount";
    public const string AdvSecCommitScanningMaxRequestsConcurrency = "/Service/Git/Settings/AdvSec/CommitScanning/MaxRequestsConcurrency";
    public const string AdvSecCommitScanningVersionIdInternal = "/Service/Git/Settings/AdvSec/CommitScanning/VersionIdInternal";
    public const string AdvSecCommitScanningVersionIdExternal = "/Service/Git/Settings/AdvSec/CommitScanning/VersionIdExternal";
    public const string AdvSecEnableOnCreateHost = "/Service/Git/Settings/AdvSec/EnableOnCreateHost";
    public const string AdvSecEnableOnCreateProject = "/Service/Git/Settings/AdvSec/EnableOnCreateProject";
    public const string AdvSecPermissionCacheInvalidationDelayMS = "/Service/Git/Settings/AdvSec/PermissionCacheDelayInMS";
    public const string AdvSecRequeueDelayInSeconds = "/Service/Git/Settings/AdvSec/RequeueDelayInSeconds";
  }
}
