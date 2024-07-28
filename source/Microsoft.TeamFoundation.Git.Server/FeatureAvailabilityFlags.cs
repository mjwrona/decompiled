// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.FeatureAvailabilityFlags
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Git.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class FeatureAvailabilityFlags
  {
    public const string GVFSDisableReachablePrefetch = "Git.GVFS.DisableReachablePrefetch";
    public const string GVFSUseSqlPrefetchHavesWants = "Git.GVFS.UseSqlPrefetchHavesWants";
    public const string GitAdvSecCommitScanningTfsBinaryEvaluation = "Git.AdvSecCommitScanning.TfsBinaryEvaluation";
    public const string GitBitmapComputeReach = "Git.Bitmap.ComputeReach";
    public const string GitBitmapUseOnClone = "Git.Bitmap.UseOnClone";
    public const string GitBitmapUseOnFetch = "Git.Bitmap.UseOnFetch";
    public const string GitBitmapDoNotUseOnPrefetch = "Git.Bitmap.DoNotUseOnPrefetch";
    public const string GitBitmapCompareOnFetch = "Git.Bitmap.CompareOnFetch";
    public const string GitBitmapsUseM161Format = "Git.Bitmap.UseM161Format";
    public const string GitRefNameConventionNotMetErrorMessage = "Git.RefNameConventionNotMetErrorMessage";
    public const string GitDefaultBranchIsMain = "Git.DefaultBranchIsMain";
    public const string GitDisableRefUpdateBlockUnsafeSubmodule = "Git.DisableRefUpdateBlockUnsafeSubmodule";
    public const string GitDisableRefUpdateBlockDotGitAlternateDataStreamCheck = "Git.DisableRefUpdateBlockDotGitAlternateDataStreamCheck";
    public const string GitEnableRefUpdateWarnItemPathBackslash = "Git.EnableRefUpdateWarnItemPathBackslash";
    public const string GitGetLabelsThatCanReachNew = "Git.GetLabelsThatCanReachNew";
    public const string GitMerges = "Git.Merges";
    public const string GitIsolationBitmapRead = "Git.IsolationBitmap.Read";
    public const string GitIsolationBitmapWrite = "Git.IsolationBitmap.Write";
    public const string GitCherryPickRelationship = "Git.CherryPickRelationship";
    public const string GitProtocolV2 = "Git.ProtocolVersionTwo";
    public const string GitPartialClone = "Git.EnablePartialClone";
    public const string DisallowShallowAndPartialClone = "Git.DisallowShallowAndPartialClone";
    public const string ResumableRepacker = "Git.EnableResumableRepacker";
    public const string ParallelFsckOdbContent = "Git.EnableParallelFsckOdbContent";
    public const string ParallelFsckGitRepacker = "Git.EnableParallelFsckGitRepacker";
    public const string ParallelPackBuilder = "Git.EnableParallelPackBuilder";
    public const string GvfsOnlySettingAlsoAppliesToPartialClones = "Git.GvfsOnlySettingAlsoAppliesToPartialClones";
    public const string GitPushRecentActivities = "Git.Push.RecentActivities";
    public const string GitPushNewPushPolicies = "Git.Push.NewPushPolicies";
    public const string GitPushCommitAuthorPushPolicy = "Git.Push.CommitAuthorPushPolicy";
    public const string GitUtilRequestContextCaching = "Git.Util.RequestContextCaching";
    public const string GitHttpClonePrependOrgName = "Git.HttpCloneUrl.PrependCodexWithOrgName";
    public const string LibGit2DisableStrictObjectCreation = "Git.LibGit2.DisableStrictObjectCreation";
    public const string GitFsckLfs = "Git.FsckLfs";
    public const string GitGetRepositoriesAsyncResponse = "Git.GetRepositories.AsyncResponse";
    public const string GitItemsSanitizeSVG = "Git.Items.SanitizeSVG";
    public const string GitRequireGenericContributeForRefUpdatesIncludingCommits = "Git.RequireGenericContributeForRefUpdatesIncludingCommits";
    public const string GitItemsZipForUnix = "Git.Items.ZipForUnix";
    public const string GitUse32KbStreamBufferSize = "Git.Use32KbStreamBufferSize";
    public const string GitTelemetryPublishNumberOfExistedBytes = "Git.Telemetry.NumberOfExistedBytes";
    public const string PolicyTruncateMessageByTextElements = "Policy.TruncateMessageByTextElements";
    public const string GitMergeBaseReturnsAllBases = "Git.MergeBase.ReturnAllBases";
    public const string SourceControlGitPullRequestsLighterThreads = "SourceControl.GitPullRequests.LighterThreads";
    public const string SourceControlGitPullRequestsConflicts = "SourceControl.GitPullRequests.Conflicts";
    public const string SourceControlGitCherryPickConflicts = "SourceControl.GitCherryPick.Conflicts";
    public const string SourceControlGitRevertConflicts = "SourceControl.GitRevert.Conflicts";
    public const string SourceControlGitPullRequestsConflictResolutionAuthorCommits = "SourceControl.GitPullRequests.ConflictResolutionAuthorCommits";
    public const string SourceControlGitPullRequestsEnforceAdHocRequiredReviewers = "SourceControl.GitPullRequests.EnforceAdHocRequiredReviewers";
    public const string SourceControlGitPullRequestsSelectiveAutoComplete = "SourceControl.GitPullRequests.SelectiveAutoComplete";
    public const string SourceControlGitPullRequestsAutoCompleteLegacyNonBlockingBehavior = "SourceControl.GitPullRequests.AutoCompleteLegacyNonBlockingBehavior";
    public const string SourceControlGitPullRequestsAlwaysSendNewMergeNotification = "SourceControl.GitPullRequests.AlwaysSendNewMergeNotification";
    public const string SourceControlGitPullRequestsCreatedServiceBusEvent = "SourceControl.GitPullRequests.CreatedServiceBusEvent";
    public const string SourceControlGitPullRequestsMergedServiceBusEvent = "SourceControl.GitPullRequests.MergedServiceBusEvent";
    public const string SourceControlServiceBusEventsPublishAsync = "SourceControl.ServiceBusEventsPublishAsync";
    public const string SourceControlShowToggleToSwitchOldCheckinPoliciesSaving = "SourceControl.ShowToggleToSwitchOldCheckinPoliciesSaving";
    public const string SourceControlNewToggleToSwitchOldCheckinPoliciesSaving = "SourceControl.NewToggleToSwitchOldCheckinPoliciesSaving";
    public const string PolicyEventBasedCacheEnabled = "Policy.EventBasedCacheEnabled";
    public const string PolicyEventBasedCacheEnabledOnTargetUpdate = "Policy.EventBasedCacheEnabled.OnTargetUpdate";
    public const string SourceControlGitDisableRepositoryDownloadEvent = "SourceControl.Git.DisableRepositoryDownloadEvent";
    public const string SourceControlGitDisableRepositoryDownloadEventFromUI = "SourceControl.Git.DisableRepositoryDownloadEventFromUI";
    public const string SourceControlGitDisableRepositoryCreatedEvent = "SourceControl.Git.DisableRepositoryCreatedEvent";
    public const string SourceControlGitDisableRepositoryCreatedEventFromUI = "SourceControl.Git.DisableRepositoryCreatedEventFromUI";
    public const string SourceControlGitDisableRepositoryRenamedEvent = "SourceControl.Git.DisableRepositoryRenamedEvent";
    public const string SourceControlGitDisableRepositoryRenamedEventFromUI = "SourceControl.Git.DisableRepositoryRenamedEventFromUI";
    public const string SourceControlGitDisableRepositoryDeletedEvent = "SourceControl.Git.DisableRepositoryDeletedEvent";
    public const string SourceControlGitDisableRepositoryDeletedEventFromUI = "SourceControl.Git.DisableRepositoryDeletedEventFromUI";
    public const string SourceControlGitDisableRepositoryUndeletedEvent = "SourceControl.Git.DisableRepositoryUndeletedEvent";
    public const string SourceControlGitDisableRepositoryUndeletedEventFromUI = "SourceControl.Git.DisableRepositoryUndeletedEventFromUI";
    public const string SourceControlGitDisableRepositoryForkedEvent = "SourceControl.Git.DisableRepositoryForkedEvent";
    public const string SourceControlGitDisableRepositoryForkedEventFromUI = "SourceControl.Git.DisableRepositoryForkedEventFromUI";
    public const string SourceControlGitDisableRepositoryFetchEvent = "SourceControl.Git.DisableRepositoryFetchEvent";
    public const string SourceControlGitDisableRepositoryFetchEventFromUI = "SourceControl.Git.DisableRepositoryFetchEventFromUI";
    public const string SourceControlGitDisableRepositoryStatusChangedEvent = "SourceControl.Git.DisableRepositoryStatusChangedEvent";
    public const string SourceControlGitDisableRepositoryStatusChangedEventFromUI = "SourceControl.Git.DisableRepositoryStatusChangedEventFromUI";
    public const string WebAccessVersionControlPullRequestsAutoComplete = "WebAccess.VersionControl.PullRequests.AutoComplete";
    public const string WebAccessVersionControlPullRequestsFollows = "WebAccess.VersionControl.PullRequests.Follows";
    public const string WebAccessVersionControlPullRequestsLabels = "WebAccess.VersionControl.PullRequests.Labels";
    public const string WebAccessVersionControlPullRequestsUpdateTargetBranches2 = "WebAccess.VersionControl.PullRequests.UpdateTargetBranches2";
    public const string DisableTfvcRepository = "WebAccess.VersionControl.DisableTfvcRepository";
    public const string SourceControlGitPullRequestsAttachments = "SourceControl.GitPullRequests.Attachments";
    public const string GitPullRequestUsePolicyPRTarget = "Git.PullRequests.UsePolicyPRTarget";
    public const string GitPullRequestUseNewQueryCodePath = "Git.PullRequests.UseNewQueryCodePath";
    public const string GitPullRequestsCommentsInVoteNotification = "Git.PullRequests.CommentsInVoteNotification";
    public const string GitPullRequestsAtomicIterationSave = "Git.PullRequests.AtomicIterationSave";
    public const string GitPullRequestsSourceBranchChangedMessage = "Git.PullRequests.SourceBranchChangedMessage";
    public const string WebAccessVersionControlGitSecretScanning = "WebAccess.VersionControl.GitSecretsScanning";
    public const string GitServerDisablePushProtectionEnablementJob = "Git.Server.DisablePushProtectionEnablementJob";
    public const string GitServerDisablePrescriptiveSecretsBlocking = "Git.Server.DisablePrescriptiveSecretsBlocking";
    public const string GitServerEnableSecretScanningExternalOverride = "Git.Server.EnableSecretScanningExternalOverride";
    public const string GitServerEnableAdvancedSecurityPrescriptiveBlocking = "Git.Server.EnableAdvancedSecurityPrescriptiveBlocking";
    public const string GitServerDisableAdvancedSecurityRepositoryVerification = "Git.Server.DisableAdvancedSecurityRepositoryVerification";
    public const string GitServerEnableAdvancedSecurityExtendedSecretsAnalysis = "Git.Server.EnableAdvancedSecurityExtendedSecretsAnalysis";
    public const string GitPullRequestsTagedPrFromUserIgnoresApprovers = "Git.PullRequests.TagCanOverrideApproverPolicy";
    public const string GitPullRequestsPolicyReportStatistics = "Git.PullRequests.PolicyReportStatistics";
    public const string GitNewOnIndexUpdateAndOnRefsUpdateJobs = "Git.NewOnIndexUpdateAndOnRefsUpdateJobs";
    public const string SourceControlPolicyRunOnCreatedAsync = "SourceControl.Policy.RunOnCreatedAsync";
    public const string SourceControlCherryPick = "SourceControl.CherryPick";
    public const string SourceControlRevert = "SourceControl.Revert";
    public const string GitUseIntervalBasedFileCleanup = "Git.IntervalBasedFileCleanup";
    public const string SourceControlPullRequestRetarget = "SourceControl.GitPullRequests.Retarget";
    public const string SourceControlGitPullRequestsMergeStrategyRebase = "SourceControl.GitPullRequests.MergeStrategy.Rebase";
    public const string SourceControlPolicyDefaultBranchScope = "SourceControl.Policy.DefaultBranchScope";
    public const string SourceControlGitPullRequestsReviewerFlags = "SourceControl.GitPullRequests.ReviewerFlags";
    public const string SourceControlGitCommitEntriesCountPolicy = "SourceControl.GitCommitEntriesCountPolicy";
    public const string SourceControlSuggestBranchName = "SourceControl.SuggestBranchName";
    public const string GitServerUseReadOnlyComponent = "Git.Server.UseReadOnlyComponent";
    public const string GitServerLoggingGitThrottlingSettingsDatabaseWideJob = "Git.Server.Logging.GitThrottlingSettingsDatabaseWideJob";
    public const string EnableReturningPartiallySucceededGitStatusState = "SourceControl.EnableReturningPartiallySucceededGitStatusState";
    public const string CheckQueueBuildPermissionOnPipeline = "Build2.CheckQueueBuildPermissionOnPipeline";
    public const string EnableFindRepositoryByIdCaching = "Git.Server.EnableFindRepositoryByIdCaching";
    public const string EnableRepoImportDNSPinning = "SourceControl.EnableRepoImportDNSPinning";
    public const string GitEnableIOWaitMetric = "Git.EnableIOWaitMetric";
    public const string WikiDetectTextFileCharset = "Wiki.DetectTextFileCharset";
  }
}
