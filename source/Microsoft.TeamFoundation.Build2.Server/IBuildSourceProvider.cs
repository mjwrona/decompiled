// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildSourceProvider
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [InheritedExport]
  public interface IBuildSourceProvider
  {
    bool RequiresServiceConnection(IVssRequestContext requestContext);

    SourceProviderAttributes GetAttributes(IVssRequestContext requestContext);

    void InitializeExceptionMap(ApiExceptionMapping exceptionMap);

    void BeforeSerialize(IVssRequestContext requestContext, BuildRepository repository);

    void AfterDeserialize(IVssRequestContext requestContext, BuildRepository repository);

    void BuildCompleted(
      IVssRequestContext requestContext,
      ITimelineRecordContext jobContext,
      BuildDefinition definition,
      BuildData build);

    void BuildQueued(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildData build);

    void BuildStarted(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildData build);

    void ValidateTrigger(
      IVssRequestContext requestContext,
      BuildTrigger trigger,
      BuildRepository repository);

    BuildRepository GetBuildRepository(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      string repoId = null,
      string branch = null);

    void SetRepositoryDefaultInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository);

    void SetRepositoryNameAndUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository);

    ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      EndpointAuthorization authorization);

    void SetProperties(
      IVssRequestContext requestContext,
      RepositoryResource repository,
      BuildRepository buildRepository,
      string sourceBranch = null,
      string sourceVersion = null);

    List<Change> GetChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository,
      IEnumerable<string> changeDescriptors);

    string NormalizeRepositoryId(
      IVssRequestContext requestContext,
      string repoId,
      string repoName = null,
      bool isRepoNameRequired = false);

    string NormalizeSourceBranch(
      string sourceBranch,
      BuildDefinition definition,
      IDictionary<string, string> variables = null);

    BuildReason NormalizeBuildReason(BuildReason buildReason, BuildData build);

    void NormalizeBuildChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository,
      IEnumerable<Change> changes);

    IEnumerable<Change> GetChangesBetweenBuilds(
      IVssRequestContext requestContext,
      BuildData fromBuild,
      BuildData toBuild,
      int maxChanges);

    IEnumerable<Change> GetChangeHistory(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      string inHistoryOfObject,
      int maxChanges);

    IEnumerable<Change> GetChangeHistory(
      IVssRequestContext requestContext,
      string repositoryId,
      string inHistoryOfObject,
      int maxChanges);

    bool IsSourceVersionValid(
      IVssRequestContext requestContext,
      string sourceVersion,
      out string errorMessage);

    bool IsSourceBranchProtected(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string sourceBranch);

    bool IsSourceBranchValid(
      IVssRequestContext requestContext,
      string sourceBranch,
      out string errorMessage);

    bool IsSourceBranchTemporary(IVssRequestContext requestContext, string sourceBranch);

    bool IsSourceVersionValidInSourceBranch(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository,
      string sourceBranch,
      string sourceVersion,
      out string errorMessage);

    string GetLatestSourceVersion(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      string sourceBranch,
      IDictionary<string, string> variables = null);

    string ResolveToCommit(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository,
      string objectId);

    string ResolveVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository,
      string version);

    void ResolveUrl(IVssRequestContext requestContext, Guid projectId, BuildRepository repository);

    bool TryGetSourceVersionDisplayUrl(
      IVssRequestContext requestContext,
      BuildData build,
      string sourceVersion,
      out string displayUrl);

    string GetVersionSpec(
      IVssRequestContext requestContext,
      string buildSourceBranch,
      string buildSourceVersion);

    string GetChangeText(IVssRequestContext requestContext, Change change);

    string GetChangeText(IVssRequestContext requestContext, Microsoft.TeamFoundation.Build.WebApi.Change change);

    IEnumerable<string> GetRepositorySecretPropertyNames();

    bool TryGetFilesChanged(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      string repositoryId,
      string oldObjectId,
      string newObjectId,
      out List<string> filesChanged);

    bool TryGetChangedPrFiles(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      string repositoryId,
      string pullRequestId,
      string mergeSha,
      out List<string> filesChanged);

    SourceRepository GetUserRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId,
      string repository,
      bool includeRichInformation = false);

    SourceRepositories GetUserRepositories(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId,
      bool pageResults = false,
      string continuationToken = null);

    SourceRepositories GetUserRepositories(
      IVssRequestContext requestContext,
      Guid projectId,
      ServiceEndpoint serviceEndpoint,
      bool pageResults = false,
      string continuationToken = null);

    SourceRepositories GetTopUserRepositories(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId);

    SourceRepositories GetTopUserRepositories(
      IVssRequestContext requestContext,
      Guid projectId,
      ServiceEndpoint serviceEndpoint);

    IEnumerable<string> GetRepositoryBranches(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId,
      string repository);

    bool CheckForBranch(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId,
      string repository,
      string branch);

    IDictionary<string, bool?> CheckForBranchesProtection(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<ArtifactSourceVersion> sourceVersions);

    IEnumerable<RepositoryWebhook> GetRepositoryWebhooks(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId,
      string repository);

    IList<InputValues> GetInputValues(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> inputIds,
      IDictionary<string, string> currentValues);

    IDictionary<string, string> GetJobEnvironmentVariables(
      IVssRequestContext requestContext,
      BuildData build);

    bool IsRepositoryBranchIncluded(IEnumerable<string> branchSpecs, string branch);

    IList<string> GetMatchingBranches(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IList<string> branchFilters);

    List<Change> GetNoCICommits(
      IVssRequestContext requestContext,
      string repositoryId,
      IEnumerable<Change> includedChanges,
      string branchHead,
      List<Change> commitsCache,
      List<BuildDefinition> definitions,
      out bool hasNoCICommits);

    bool HasLimitedRights(IVssRequestContext requestContext, BuildDefinition definition);

    void PostStatusForSkippedChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository,
      int definitionId,
      string statusName,
      string reasonSkipped,
      string conclusion,
      string sourceVersion,
      string detailsUrl,
      string detailsTitle = null);

    void PostMultipleStatusesForSkippedChanges(
      IVssRequestContext requestContext,
      List<BuildDefinition> definitions,
      string reasonSkipped,
      string conclusion,
      string sourceVersion,
      string detailsUrl = null,
      string detailsTitle = null);

    void ValidateRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository);

    IEnumerable<Uri> GetCommitUris(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      IEnumerable<string> commitIds);

    BuildArtifactResource CreateLabel(
      IVssRequestContext requestContext,
      ITimelineRecordContext taskContext,
      BuildDefinition definition,
      BuildData build,
      string label);

    bool TryGetPreviousVersion(
      IVssRequestContext requestContext,
      BuildData build,
      out string changes);

    bool TryCalculateChanges(
      IVssRequestContext requestContext,
      BuildData build,
      BuildDefinition definition,
      int maxChanges,
      out List<Change> changes);

    bool IsExemptFromMinimumRetentionPolicy(BuildForRetention build);

    PullRequest GetPullRequest(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      string pullRequestId);

    PullRequest GetPullRequest(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string pullRequestId,
      Guid? serviceEndpointId);

    PullRequest CreatePullRequest(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId,
      string repository,
      string targetBranch,
      string sourceBranch,
      string title,
      string description = null);

    PullRequest CreatePullRequest(
      IVssRequestContext requestContext,
      Guid projectId,
      ServiceEndpoint serviceEndpoint,
      string repository,
      string targetBranch,
      string sourceBranch,
      string title,
      string description = null);

    bool TryGetWorkItemIds(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      int maxItems,
      out IEnumerable<int> workItemIds);

    IEnumerable<SourceRelatedWorkItem> GetRelatedWorkitems(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      IReadOnlyList<Change> changes,
      Guid? serviceEndpointId,
      int maxItems);

    string GetArtifactUriForPullRequest(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      BuildRepository repository);

    IEnumerable<string> GetArtifactUrisForChanges(
      IVssRequestContext requestContext,
      BuildRepository repository,
      IEnumerable<Change> changes);

    void FixOutgoingDefinition(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      ApiResourceVersion apiVersion);

    void FixIncomingDefinition(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      ApiResourceVersion apiVersion,
      bool isUpdate);

    string GetFileName(string path);

    string GetDirectoryName(string path);

    string GetFileContent(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId,
      string repository,
      string commitOrBranch,
      string path);

    FileContentData GetFileContentData(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId,
      string repository,
      string branch,
      string filePath);

    FileContentData GetFileContentData(
      IVssRequestContext requestContext,
      Guid projectId,
      ServiceEndpoint serviceEndpoint,
      string repository,
      string branch,
      string filePath);

    IReadOnlyList<FileContentData> CommitFiles(
      IVssRequestContext requestContext,
      Guid projectId,
      ServiceEndpoint serviceEndpoint,
      string repository,
      IReadOnlyList<MinimalFileContentData> files,
      string newBranch,
      string baseBranch,
      string message);

    IEnumerable<SourceRepositoryItem> GetPathContents(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId,
      string repository,
      string commitOrBranch,
      string path);

    string ResolvePath(string defaultRoot, string path);

    IList<Demand> GetDemands(IVssRequestContext requestContext);

    void RecreateSubscription(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId,
      string repository,
      ICollection<DefinitionTriggerType> triggerTypes);

    Guid GetRepositoryProjectId(IVssRequestContext requestContext, BuildRepository repository);

    RepositoryVisibility GetRepositoryVisibility(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository);

    string GetSourceVersionMessage(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository,
      string sourceVersion);

    List<string> GetFavoriteBranches(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository,
      Guid identityId);

    int ExtractPullRequestIdFromSourceBranch(string buildSourceBranch);

    bool IsAbusiveUser(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository,
      string username);

    bool IsRepositoryOwnedByAbusiveUser(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository);

    TreeTraversalResult GetTreeTraversalResult(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? serviceEndpointId,
      string repoName,
      string branch,
      long timeoutMs,
      int depth = 3);

    bool TryGetCurrentUserCredentialsServiceEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      string repository,
      out ServiceEndpoint userConnection);

    void CheckEndpointAuthorization(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildRepository repository,
      BuildRepository previouslyAuthorizedRepository = null);

    IEnumerable<InformationNodeData> GenerateInformationNodes(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      IList<string> informationTypes);

    IEnumerable<InformationNodeData> GenerateInformationNodesForNotification(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build);

    string GetShelvesetName(IReadOnlyBuildData build);

    bool TryConvertSourceVersionToChangeId(
      IVssRequestContext requestContext,
      string sourceVersion,
      out string changeId);
  }
}
