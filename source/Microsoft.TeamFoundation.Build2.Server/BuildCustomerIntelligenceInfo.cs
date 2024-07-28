// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildCustomerIntelligenceInfo
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [GenerateAllConstants(null)]
  internal static class BuildCustomerIntelligenceInfo
  {
    public const string Area = "Build";

    internal static class Features
    {
      public const string AssociatedWorkItems = "AssociatedWorkItems";
      public const string AutoLinkWorkItems = "AutoLinkWorkItems";
      public const string BuildArtifactDownload = "BuildArtifactDownload";
      public const string BuildCommittersDetails = "BuildCommittersDetails";
      public const string BuildCompletion = "BuildCompletion";
      public const string BuildDefinition = "BuildDefinition";
      public const string BuildGitEvent = "BuildGitEvent";
      public const string BuildPollingSummary = "BuildPollingSummary";
      public const string Properties = "Properties";
      public const string SignalR = "Build.SignalR";
    }

    internal static class Keys
    {
      public const string ArtifactCount = "ArtifactCount";
      public const string ArtifactCountFormat = "ArtifactCount_{0}";
      public const string ArtifactResourceType = "ArtifactResourceType";
      public const string BadgeEnabled = "BadgeEnabled";
      public const string BatchedCIEnabled = "BatchedCIEnabled";
      public const string BuildChangeType = "BuildChangeType";
      public const string BuildCommitterHash = "BuildCommitterHash";
      public const string BuildId = "BuildId";
      public const string BuildNumber = "BuildNumber";
      public const string BuildNumberFormat = "BuildNumberFormat";
      public const string BuildReason = "BuildReason";
      public const string BuildType = "BuildType";
      public const string BuildUri = "BuildUri";
      public const string ChangeType = "ChangeType";
      public const string CIBranchFilterCount = "CIBranchFilterCount";
      public const string CIEnabled = "CIEnabled";
      public const string CleanOption = "CleanOption";
      public const string CommitId = "CommitId";
      public const string ConsoleLogLinesCount = "ConsoleLogLinesCount";
      public const string DefinitionId = "DefinitionId";
      public const string DefinitionName = "DefinitionName";
      public const string DefinitionProperties = "DefinitionProperties";
      public const string DefinitionQuality = "DefinitionQuality";
      public const string DefinitionType = "DefinitionType";
      public const string DefinitionUri = "DefinitionUri";
      public const string Depth = "Depth";
      public const string DropLocation = "DropLocation";
      public const string Duration = "Duration";
      public const string EnsureSourceVersionInfoPopulatedElapsedMilliseconds = "EnsureSourceVersionInfoPopulatedElapsedMilliseconds";
      public const string EvaluatedBranches = "EvaluatedBranches";
      public const string ExceededMaxItemsForWorkItemLimit = "ExceededMaxItemsForWorkItemLimit";
      public const string FetchTags = "FetchTags";
      public const string FinishTime = "FinishTime";
      public const string GetChangesElapsedMilliseconds = "GetChangesElapsedMilliseconds";
      public const string GetDirectWorkItemsElapsedMilliseconds = "GetDirectWorkItemsElapsedMilliseconds";
      public const string GetIndirectWorkItemsElapsedMilliseconds = "GetIndirectWorkItemsElapsedMilliseconds";
      public const string GetPropertiesElapsedMilliseconds = "GetPropertiesElapsedMilliseconds";
      public const string IndirectWorkItemCount = "IndirectWorkItemCount";
      public const string IsBuildAzure = "IsAzure";
      public const string ItemsBetweenBuildsForWorkItemCount = "ItemsBetweenBuildsForWorkItemCount";
      public const string LastVersionBuilt = "LastVersionBuilt";
      public const string LastVersionInResults = "LastVersionInResults";
      public const string LimitExceededType = "LimitExceededType";
      public const string Message = "Message";
      public const string NumberOfBuildsQueued = "NumberOfBuildsQueued";
      public const string NumberOfCommitsToBuild = "NumberOfCommitsToBuild";
      public const string NumberOfLogItems = "NumberOfLogItems";
      public const string NumberOfRevisionsInResults = "NumberOfRevisionsInResults";
      public const string PhaseCount = "PhaseCount";
      public const string PollingElapsedMilliseconds = "PollingElapsedMilliseconds";
      public const string PollingEnabled = "PollingEnabled";
      public const string PollingInterval = "PollingInterval";
      public const string PollingPathFilters = "PollingPathFilters";
      public const string PollingRemainingRevisions = "PollingRemainingRevisions";
      public const string PollingUnfilteredRevisions = "PollingUnfilteredRevisions";
      public const string PotentialRevisions = "PotentialRevisions";
      public const string PreviousVersionEvaluated = "PreviousVersionEvaluated";
      public const string ProcessType = "ProcessType";
      public const string ProjectId = "ProjectId";
      public const string ProjectAndPipelineId = "ProjectAndPipelineId";
      public const string ProjectUri = "ProjectUri";
      public const string PropertyCount = "PropertyCount";
      public const string PusherId = "PusherId";
      public const string PushId = "PushId";
      public const string PushTime = "PushTime";
      public const string QueueId = "QueueId";
      public const string Reason = "Reason";
      public const string RemainingRevisions = "RemainingRevisions";
      public const string RepositoryType = "RepositoryType";
      public const string RepositoryId = "RepositoryId";
      public const string RepositoryUrl = "RepositoryUrl";
      public const string RepositoryUrlHash = "RepositoryUrlHash";
      public const string Result = "Result";
      public const string RetryAttempts = "RetryAttempts";
      public const string ScheduledBranchFilterCount = "ScheduledBranchFilterCount";
      public const string ScheduledEnabled = "ScheduledEnabled";
      public const string ScheduledTimeCount = "ScheduledTimeCount";
      public const string SignalRReconnected = "SignalRReconnected";
      public const string SkipSyncSource = "SkipSyncSource";
      public const string Source = "Source";
      public const string SourceBranch = "SourceBranch";
      public const string SourceProvider = "SourceProvider";
      public const string SourceVersion = "SourceVersion";
      public const string StartTime = "StartTime";
      public const string Status = "Status";
      public const string StepCount = "StepCount";
      public const string TaskGroupCount = "TaskGroupCount";
      public const string TimelineRecordsCount = "TimelineRecordsCount";
      public const string TriggerInfo = "TriggerInfo";
      public const string UpdatePropertiesElapsedMilliseconds = "UpdatePropertiesElapsedMilliseconds";
      public const string UserAgent = "UserAgent";
      public const string VariableCount = "VariableCount";
      public const string WorkItemCount = "WorkItemCount";
      public const string YamlErrors = "YamlErrors";
    }
  }
}
