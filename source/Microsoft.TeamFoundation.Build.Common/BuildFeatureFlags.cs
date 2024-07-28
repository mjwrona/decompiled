// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildFeatureFlags
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class BuildFeatureFlags
  {
    public const string BuildPollingTrigger = "Build2.PollingTrigger";
    public const string AllowOutOfScopeRepository = "Build2.AllowOutOfScopeRepository";
    public const string BuildPollingUseLibGit2Sharp = "Build2.PollingJob.UseLibGit2Sharp";
    public const string passPercentageForTIARuns = "Build2.TestImpact.PassPercentageforTIARuns";
    public const string BuildAndReleaseResourceLimits = "WebAccess.BuildAndRelease.ResourceLimits";
    public const string WorkItemTrackingBuildAutoLinkWorkItems = "WorkItemTracking.Build.AutoLinkWorkItems";
    public const string WorkItemTrackingBuildAutoLinkWorkItemsNoCache = "WorkItemTracking.Build.AutoLinkWorkItemsNoCache";
    public const string XamlHubEnabled = "Build.XamlHub";
    public const string XamlEnabled = "Build.XamlEnabled";
    public const string FireBuildEventAsync = "Build.FireBuildEventAsync";
    public const string ScanPipelineArtifacts = "Build2.ScanPipelineArtifacts";
    public const string DisableGithubUseGraphQLForPollingJob = "Build2.DisableGithubUseGraphQLForPollingJob";
    public const string DoNotSyncScheduleIfDesigner = "Build2.DoNotSyncScheduleIfDesigner";
    public const string SelectTriggeringRepositoriesBasedOnSHA = "Build2.SelectTriggeringRepositoriesBasedOnSHA";
    public const string ResolveRetentionLeasesInBatch = "Build2.ResolveRetentionLeasesInBatch";
    public const string UsePreviousSundayToScheduleJobs = "Build2.ScheduledTriggers.UsePreviousSundayToScheduleJobs";
    public const string DoNotIncreaseCountersWhenDownloadingYAML = "Build2.DoNotIncreaseCountersWhenDownloadingYAML";
    public const string FixBuildAuthorizationScopeOverride = "Build2.FixBuildAuthorizationScopeOverride";
    public const string GetRunRestAPIConsumedResources = "Build2.GetRunRestAPIConsumedResources";
    public const string ExitIfNoControllerMatchesURI = "Build.ExitIfNoControllerMatchesURI";
    public const string HandleBuildJobScheduleDSTTransitions = "Build2.HandleBuildJobScheduleDSTTransitions";
    public const string FixIsSourceBranchProtectedChange = "Build2.FixIsSourceBranchProtected";
    public const string DoNotAllowSpoofedRequestedForField = "WebAccess.Build2.DoNotAllowUsersToSpoofRequestedFor";
    public const string StoreServiceConnectionListInsteadOfId = "Build2.StoreServiceConnectionListInsteadOfId";
    public const string AllowCentralizedPipelineControls = "Build2.AllowCentralizedPipelineControls";
    public const string LogSuspiciousAccessToBuildApi = "Build2.LogSuspiciousAccessToBuildApi";
    public const string EnablePartiallySucceededGitStatusStateCreation = "Build2.SourceProviders.EnablePartiallySucceededGitStatusStateCreation";
  }
}
