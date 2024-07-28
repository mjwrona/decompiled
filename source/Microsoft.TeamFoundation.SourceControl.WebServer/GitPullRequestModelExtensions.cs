// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestModelExtensions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitPullRequestModelExtensions
  {
    private static string TraceAreaAndLayer = nameof (GitPullRequestModelExtensions);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<GitPullRequest> ToWebApiItems(
      this IEnumerable<TfsGitPullRequest> pullRequests,
      IVssRequestContext requestContext,
      IDictionary<string, TeamProjectReference> projectLookup = null,
      IDictionary<Guid, IdentityRef> identityLookup = null,
      bool includeLinks = false,
      bool minimalData = false,
      bool includeLabels = false)
    {
      if (identityLookup == null)
        identityLookup = GitPullRequestModelExtensions.GetIdentitiesLookupForPullRequests(requestContext, pullRequests, IdentitiesLookupType.ReviewersAndCreator, minimalData);
      if (projectLookup == null)
        projectLookup = GitPullRequestModelExtensions.GenerateProjectUriLookup(requestContext);
      Dictionary<int, WebApiTagDefinition[]> dictionary = (Dictionary<int, WebApiTagDefinition[]>) null;
      if (includeLabels)
        dictionary = pullRequests.GetWebApiPullRequestsLabels(requestContext, projectLookup);
      List<GitPullRequest> webApiItems = new List<GitPullRequest>();
      foreach (TfsGitPullRequest pullRequest in pullRequests)
      {
        TeamProjectReference projectReference = (TeamProjectReference) null;
        if (pullRequest.ProjectUri == null || !projectLookup.TryGetValue(pullRequest.ProjectUri, out projectReference))
        {
          requestContext.Trace(1013799, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitPullRequestModelExtensions), "Project cannot be found for the pull request or ProjectUri is empty. ProjectUri: {0}, RepoId: {1}, PullRequestId: {2}", (object) pullRequest.ProjectUri, (object) pullRequest.RepositoryId, (object) pullRequest.PullRequestId);
        }
        else
        {
          ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(projectReference.Id, pullRequest.RepositoryId);
          RepoKey repoKey = new RepoKey(projectReference.Id, pullRequest.RepositoryId);
          WebApiTagDefinition[] apiTagDefinitionArray = (WebApiTagDefinition[]) null;
          dictionary?.TryGetValue(pullRequest.PullRequestId, out apiTagDefinitionArray);
          if (!minimalData)
            pullRequest.CompletionOptions?.Normalize();
          GitPullRequest gitPullRequest = new GitPullRequest()
          {
            Repository = new GitRepository()
            {
              Id = pullRequest.RepositoryId,
              Url = minimalData ? (string) null : GitReferenceLinksUtility.GetRepositoryUrl(requestContext, repoKey),
              Name = pullRequest.RepositoryName,
              ProjectReference = projectReference,
              IsFork = pullRequest.RepositoryCreatedByForking
            },
            ForkSource = pullRequest.ForkSource.ToWebApiItem(requestContext),
            PullRequestId = pullRequest.PullRequestId,
            CodeReviewId = pullRequest.CodeReviewId,
            Status = pullRequest.Status,
            CreatedBy = identityLookup.ContainsKey(pullRequest.Creator) ? identityLookup[pullRequest.Creator] : (IdentityRef) null,
            CreationDate = pullRequest.CreationDate,
            ClosedDate = pullRequest.ClosedDate,
            Title = string.IsNullOrEmpty(pullRequest.Title) ? (string) null : pullRequest.Title,
            Description = string.IsNullOrEmpty(pullRequest.Description) ? (string) null : pullRequest.Description,
            SourceRefName = pullRequest.SourceBranchName,
            TargetRefName = pullRequest.TargetBranchName,
            MergeStatus = pullRequest.Status == PullRequestStatus.Completed ? PullRequestAsyncStatus.Succeeded : pullRequest.MergeStatus,
            MergeId = pullRequest.MergeId,
            Labels = apiTagDefinitionArray,
            LastMergeSourceCommit = minimalData ? (GitCommitRef) null : GitPullRequestModelExtensions.GetOrDefaultGitCommitRef(requestContext, pullRequest.LastMergeSourceCommit, repoKey),
            LastMergeTargetCommit = minimalData ? (GitCommitRef) null : GitPullRequestModelExtensions.GetOrDefaultGitCommitRef(requestContext, pullRequest.LastMergeTargetCommit, repoKey),
            LastMergeCommit = minimalData ? (GitCommitRef) null : GitPullRequestModelExtensions.GetOrDefaultGitCommitRef(requestContext, pullRequest.LastMergeCommit, repoKey),
            AutoCompleteSetBy = !(pullRequest.AutoCompleteAuthority != new Guid()) || !identityLookup.ContainsKey(pullRequest.AutoCompleteAuthority) ? (IdentityRef) null : identityLookup[pullRequest.AutoCompleteAuthority],
            Reviewers = pullRequest.Reviewers.ToIdentityRefWithVotes(requestContext, repoKey, pullRequest.PullRequestId, identityLookup, !minimalData),
            Url = minimalData ? (string) null : pullRequest.GetPullRequestRESTUrl(requestContext, repoKey),
            CompletionOptions = minimalData ? (GitPullRequestCompletionOptions) null : pullRequest.CompletionOptions,
            SupportsIterations = pullRequest.SupportsIterations,
            CompletionQueueTime = pullRequest.CompletionQueueTime,
            IsDraft = new bool?(pullRequest.IsDraft)
          };
          gitPullRequest.Links = includeLinks ? gitPullRequest.GetPullRequestReferenceLinks(requestContext, repositoryReadOnly) : (ReferenceLinks) null;
          gitPullRequest.SetSecuredObject(repositoryReadOnly);
          webApiItems.Add(gitPullRequest);
        }
      }
      return (IEnumerable<GitPullRequest>) webApiItems;
    }

    public static IDictionary<string, TeamProjectReference> GenerateProjectUriLookup(
      IVssRequestContext requestContext,
      ProjectInfo project = null)
    {
      if (project == null)
        return (IDictionary<string, TeamProjectReference>) requestContext.GetService<IProjectService>().GetProjects(requestContext).ToDictionary<ProjectInfo, string, TeamProjectReference>((Func<ProjectInfo, string>) (projectInfo => projectInfo.Uri), (Func<ProjectInfo, TeamProjectReference>) (projectInfo => new TeamProjectReference()
        {
          Name = projectInfo.Name,
          Id = projectInfo.Id
        }), (IEqualityComparer<string>) TFStringComparer.ProjectUri);
      return (IDictionary<string, TeamProjectReference>) new Dictionary<string, TeamProjectReference>()
      {
        {
          project.Uri,
          new TeamProjectReference()
          {
            Name = project.Name,
            Id = project.Id
          }
        }
      };
    }

    private static IDictionary<Guid, IdentityRef> GenerateIdentitiesLookup(
      IVssRequestContext requestContext,
      Guid[] uniqueTeamFoundationIds,
      bool minimalData = false)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      TeamFoundationIdentity[] foundationIdentityArray = vssRequestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(vssRequestContext, uniqueTeamFoundationIds);
      Dictionary<Guid, TeamFoundationIdentity> source1 = new Dictionary<Guid, TeamFoundationIdentity>();
      Dictionary<Guid, TeamFoundationIdentity> source2 = new Dictionary<Guid, TeamFoundationIdentity>();
      foreach (TeamFoundationIdentity foundationIdentity in foundationIdentityArray)
      {
        if (foundationIdentity != null)
          source1[foundationIdentity.TeamFoundationId] = foundationIdentity;
      }
      if (uniqueTeamFoundationIds.Length == foundationIdentityArray.Length)
      {
        for (int index = 0; index < uniqueTeamFoundationIds.Length; ++index)
        {
          Guid teamFoundationId = uniqueTeamFoundationIds[index];
          if (!source1.ContainsKey(teamFoundationId))
          {
            if (foundationIdentityArray[index] != null)
              source1[teamFoundationId] = foundationIdentityArray[index];
            source2[teamFoundationId] = foundationIdentityArray[index];
          }
        }
      }
      else
        requestContext.Trace(1013535, TraceLevel.Error, GitPullRequestModelExtensions.TraceAreaAndLayer, GitPullRequestModelExtensions.TraceAreaAndLayer, "ReadIdentities did not return expected number of results.");
      if (source2.Any<KeyValuePair<Guid, TeamFoundationIdentity>>())
      {
        StringBuilder stringBuilder1 = new StringBuilder();
        stringBuilder1.AppendLine("ReadIdentities returned different identities than requested (requested / returned):");
        foreach (KeyValuePair<Guid, TeamFoundationIdentity> keyValuePair in source2)
        {
          StringBuilder stringBuilder2 = stringBuilder1;
          Guid guid = keyValuePair.Key;
          string str1 = guid.ToString();
          string str2;
          if (keyValuePair.Value != null)
          {
            guid = keyValuePair.Value.TeamFoundationId;
            str2 = guid.ToString();
          }
          else
            str2 = "null";
          stringBuilder2.AppendFormat("{0} {1}", (object) str1, (object) str2);
          stringBuilder1.AppendLine();
        }
        requestContext.Trace(1013535, TraceLevel.Error, GitPullRequestModelExtensions.TraceAreaAndLayer, GitPullRequestModelExtensions.TraceAreaAndLayer, stringBuilder1.ToString());
      }
      return (IDictionary<Guid, IdentityRef>) source1.Where<KeyValuePair<Guid, TeamFoundationIdentity>>((Func<KeyValuePair<Guid, TeamFoundationIdentity>, bool>) (kvp => kvp.Value != null)).ToDictionary<KeyValuePair<Guid, TeamFoundationIdentity>, Guid, IdentityRef>((Func<KeyValuePair<Guid, TeamFoundationIdentity>, Guid>) (kvp => kvp.Key), (Func<KeyValuePair<Guid, TeamFoundationIdentity>, IdentityRef>) (kvp =>
      {
        IdentityRef identityRef = kvp.Value.ToIdentityRef(requestContext);
        if (minimalData)
        {
          identityRef.Url = (string) null;
          identityRef.ProfileUrl = (string) null;
          identityRef.ImageUrl = (string) null;
          identityRef.UniqueName = (string) null;
        }
        return identityRef;
      }));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static GitPullRequest ToWebApiItem(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository tfsGitRepository,
      GitRepositoryRef sourceForkRepositoryRef = null,
      string sourceCommitId = null,
      string targetCommitId = null,
      bool includeLinks = false,
      bool includeCommits = false,
      bool includeWorkItemRefs = false,
      bool includeLabels = false,
      bool includeUserImageUrl = false)
    {
      if (pullRequest == null)
        return (GitPullRequest) null;
      ArgumentUtility.CheckForNull<ITfsGitRepository>(tfsGitRepository, nameof (tfsGitRepository));
      ISecuredObject securedObject = GitPullRequestModelExtensions.GetSecuredObject(tfsGitRepository);
      RepoKey key = tfsGitRepository.Key;
      IdentityRef autoCompleteAuthority;
      IdentityRef closedBy;
      IdentityRef creator;
      IdentityRefWithVote[] voters = pullRequest.GenerateVoters(requestContext, tfsGitRepository.Key, out autoCompleteAuthority, out closedBy, out creator);
      GitRepository webApiItem = tfsGitRepository.ToWebApiItem(requestContext);
      GitCommitRef[] gitCommitRefArray = (GitCommitRef[]) null;
      if (tfsGitRepository != null & includeCommits)
        gitCommitRefArray = pullRequest.GenerateCommits(requestContext, tfsGitRepository, sourceCommitId: sourceCommitId, targetCommitId: targetCommitId, includeUserImageUrl: includeUserImageUrl);
      ResourceRef[] resourceRefArray = (ResourceRef[]) null;
      if (tfsGitRepository != null & includeWorkItemRefs)
      {
        IEnumerable<int> associatedWorkItems = pullRequest.GetAssociatedWorkItems(requestContext, tfsGitRepository);
        if (includeWorkItemRefs && associatedWorkItems != null && associatedWorkItems.Count<int>() > 0)
          resourceRefArray = GitPullRequestModelExtensions.ToWorkItemResourceRefs(requestContext, tfsGitRepository, associatedWorkItems, securedObject).ToArray<ResourceRef>();
      }
      WebApiTagDefinition[] apiTagDefinitionArray = (WebApiTagDefinition[]) null;
      if (includeLabels && tfsGitRepository != null)
        apiTagDefinitionArray = pullRequest.GetWebApiPullRequestLabels(requestContext, tfsGitRepository);
      GitCommitRef gitCommitRef = (GitCommitRef) null;
      if (tfsGitRepository != null)
      {
        Sha1Id? lastMergeCommit = pullRequest.LastMergeCommit;
        if (lastMergeCommit.GetValueOrDefault() != Sha1Id.Empty)
        {
          ITfsGitRepository repo = tfsGitRepository;
          lastMergeCommit = pullRequest.LastMergeCommit;
          Sha1Id objectId = lastMergeCommit.Value;
          TfsGitCommit commit = repo.TryLookupObject<TfsGitCommit>(objectId);
          if (commit != null)
          {
            TfsGitCommitMetadata metadata = new TfsGitCommitMetadata(commit);
            gitCommitRef = new GitCommitTranslator(requestContext, tfsGitRepository.Key).ToGitCommitShallow(metadata, includeUserImageUrl);
          }
        }
      }
      ArtifactId artifactId = tfsGitRepository != null ? pullRequest.BuildArtifactIdForPullRequests(tfsGitRepository.Key.GetProjectUri()) : (ArtifactId) null;
      string str = artifactId != null ? LinkingUtilities.EncodeUri(artifactId) : (string) null;
      pullRequest.CompletionOptions?.Normalize();
      GitPullRequest gitPullRequest = new GitPullRequest()
      {
        Repository = webApiItem,
        ForkSource = pullRequest.ForkSource.ToWebApiItem(requestContext, sourceForkRepositoryRef, securedObject),
        PullRequestId = pullRequest.PullRequestId,
        CodeReviewId = pullRequest.CodeReviewId,
        Status = pullRequest.Status,
        CreatedBy = creator,
        CreationDate = pullRequest.CreationDate,
        ClosedDate = pullRequest.ClosedDate,
        Title = string.IsNullOrEmpty(pullRequest.Title) ? (string) null : pullRequest.Title,
        Description = string.IsNullOrEmpty(pullRequest.Description) ? (string) null : pullRequest.Description,
        SourceRefName = pullRequest.SourceBranchName,
        TargetRefName = pullRequest.TargetBranchName,
        MergeStatus = pullRequest.Status == PullRequestStatus.Completed ? PullRequestAsyncStatus.Succeeded : pullRequest.MergeStatus,
        MergeId = pullRequest.MergeId,
        LastMergeSourceCommit = GitPullRequestModelExtensions.GetOrDefaultGitCommitRef(requestContext, pullRequest.LastMergeSourceCommit, key),
        LastMergeTargetCommit = GitPullRequestModelExtensions.GetOrDefaultGitCommitRef(requestContext, pullRequest.LastMergeTargetCommit, key),
        LastMergeCommit = gitCommitRef,
        Reviewers = voters.Length != 0 ? voters : new IdentityRefWithVote[0],
        Url = pullRequest.GetPullRequestRESTUrl(requestContext, key),
        CompletionOptions = pullRequest.CompletionOptions,
        MergeOptions = pullRequest.MergeOptions,
        SupportsIterations = pullRequest.SupportsIterations,
        CompletionQueueTime = pullRequest.CompletionQueueTime,
        ClosedBy = closedBy,
        AutoCompleteSetBy = autoCompleteAuthority,
        ArtifactId = str,
        MergeFailureType = pullRequest.MergeFailureType,
        MergeFailureMessage = pullRequest.MergeFailureMessage,
        IsDraft = new bool?(pullRequest.IsDraft)
      };
      gitPullRequest.Labels = apiTagDefinitionArray;
      gitPullRequest.Commits = gitCommitRefArray;
      gitPullRequest.WorkItemRefs = resourceRefArray;
      gitPullRequest.Links = includeLinks ? gitPullRequest.GetPullRequestReferenceLinks(requestContext, securedObject) : (ReferenceLinks) null;
      gitPullRequest.SetSecuredObject(securedObject);
      return gitPullRequest;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static GitPullRequest ToShallowWebApiItem(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      IDictionary<Guid, TeamFoundationIdentity> guidToIdentity,
      ISecuredObject securedObject)
    {
      if (pullRequest == null)
        return (GitPullRequest) null;
      TeamFoundationIdentity identity;
      if (!guidToIdentity.TryGetValue(pullRequest.Creator, out identity))
      {
        requestContext.Trace(1013880, TraceLevel.Error, GitPullRequestModelExtensions.TraceAreaAndLayer, GitPullRequestModelExtensions.TraceAreaAndLayer, string.Format("Pull request creator identity is missining. Repo={0}, PullRequestId={1}, Creator={2}", (object) pullRequest.RepositoryId, (object) pullRequest.PullRequestId, (object) pullRequest.Creator));
        return (GitPullRequest) null;
      }
      GitRepository gitRepository = new GitRepository()
      {
        Id = pullRequest.RepositoryId,
        Url = GitReferenceLinksUtility.GetRepositoryUrl(requestContext, repoKey)
      };
      pullRequest.CompletionOptions?.Normalize();
      GitPullRequest shallowWebApiItem = new GitPullRequest();
      shallowWebApiItem.Repository = gitRepository;
      shallowWebApiItem.ForkSource = pullRequest.ForkSource.ToWebApiItem(requestContext);
      shallowWebApiItem.PullRequestId = pullRequest.PullRequestId;
      shallowWebApiItem.CodeReviewId = pullRequest.CodeReviewId;
      shallowWebApiItem.Status = pullRequest.Status;
      shallowWebApiItem.CreatedBy = identity.ToIdentityRef(requestContext);
      shallowWebApiItem.CreationDate = pullRequest.CreationDate;
      shallowWebApiItem.ClosedDate = pullRequest.ClosedDate;
      shallowWebApiItem.Title = string.IsNullOrEmpty(pullRequest.Title) ? (string) null : pullRequest.Title;
      shallowWebApiItem.Description = string.IsNullOrEmpty(pullRequest.Description) ? (string) null : pullRequest.Description;
      shallowWebApiItem.SourceRefName = pullRequest.SourceBranchName;
      shallowWebApiItem.TargetRefName = pullRequest.TargetBranchName;
      shallowWebApiItem.MergeStatus = pullRequest.Status == PullRequestStatus.Completed ? PullRequestAsyncStatus.Succeeded : pullRequest.MergeStatus;
      shallowWebApiItem.MergeId = pullRequest.MergeId;
      shallowWebApiItem.LastMergeSourceCommit = GitPullRequestModelExtensions.GetOrDefaultGitCommitRef(requestContext, pullRequest.LastMergeSourceCommit, repoKey);
      shallowWebApiItem.LastMergeTargetCommit = GitPullRequestModelExtensions.GetOrDefaultGitCommitRef(requestContext, pullRequest.LastMergeTargetCommit, repoKey);
      shallowWebApiItem.LastMergeCommit = GitPullRequestModelExtensions.GetOrDefaultGitCommitRef(requestContext, pullRequest.LastMergeCommit, repoKey);
      shallowWebApiItem.Url = pullRequest.GetPullRequestRESTUrl(requestContext, repoKey);
      shallowWebApiItem.CompletionOptions = pullRequest.CompletionOptions;
      shallowWebApiItem.SupportsIterations = pullRequest.SupportsIterations;
      shallowWebApiItem.CompletionQueueTime = pullRequest.CompletionQueueTime;
      shallowWebApiItem.IsDraft = new bool?(pullRequest.IsDraft);
      shallowWebApiItem.SetSecuredObject(securedObject);
      return shallowWebApiItem;
    }

    public static GitRepository ToRepositoryWebApiItem(
      this GitRepositoryRef forkRepositoryRef,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      if (forkRepositoryRef?.ProjectReference == null || forkRepositoryRef == null || forkRepositoryRef.Name == null)
        return (GitRepository) null;
      string repositoryCloneUrl = GitServerUtils.GetRepositoryCloneUrl(requestContext, GitServerUtils.GetPublicBaseUrl(requestContext, true), forkRepositoryRef.ProjectReference.Name, forkRepositoryRef.Name);
      GitRepository repositoryWebApiItem = new GitRepository();
      repositoryWebApiItem.Id = forkRepositoryRef.Id;
      repositoryWebApiItem.Name = forkRepositoryRef.Name;
      repositoryWebApiItem.Url = GitReferenceLinksUtility.GetRepositoryUrl(requestContext, new RepoKey(forkRepositoryRef.ProjectReference.Id, forkRepositoryRef.Id));
      repositoryWebApiItem.IsFork = forkRepositoryRef.IsFork;
      GitForkTeamProjectReference projectReference = new GitForkTeamProjectReference(securedObject);
      projectReference.Id = forkRepositoryRef.ProjectReference.Id;
      projectReference.Name = forkRepositoryRef.ProjectReference.Name;
      projectReference.Url = forkRepositoryRef.ProjectReference.Url;
      repositoryWebApiItem.ProjectReference = (TeamProjectReference) projectReference;
      repositoryWebApiItem.RemoteUrl = repositoryCloneUrl;
      repositoryWebApiItem.SshUrl = GitServerUtils.GetSshUrl(requestContext, repositoryCloneUrl, out bool _);
      return repositoryWebApiItem;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static GitForkRef ToWebApiItem(
      this TfsGitForkRef forkSource,
      IVssRequestContext requestContext,
      GitRepositoryRef sourceForkRepositoryRef = null,
      ISecuredObject securedObject = null)
    {
      if (forkSource == null)
        return (GitForkRef) null;
      GitRepository gitRepository = (GitRepository) null;
      if (sourceForkRepositoryRef != null)
        gitRepository = sourceForkRepositoryRef.ToRepositoryWebApiItem(requestContext, securedObject);
      if (gitRepository == null)
      {
        string str1 = (string) null;
        string str2 = (string) null;
        ITfsGitRepository repo;
        if (requestContext.GetService<ITeamFoundationGitRepositoryService>().TryFindRepositoryById(requestContext, forkSource.RepositoryId, false, out repo))
        {
          using (repo)
          {
            str1 = repo.Name;
            str2 = GitReferenceLinksUtility.GetRepositoryUrl(requestContext, repo.Key);
          }
        }
        gitRepository = new GitRepository()
        {
          Id = forkSource.RepositoryId,
          Url = str2,
          Name = str1
        };
      }
      GitForkRef webApiItem = new GitForkRef();
      webApiItem.Repository = gitRepository;
      webApiItem.Name = forkSource.RefName;
      return webApiItem;
    }

    public static IEnumerable<ResourceRef> ToWorkItemResourceRefs(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      IEnumerable<int> workItemIds,
      ISecuredObject securedObject = null)
    {
      ILocationService locationService = (ILocationService) null;
      if (requestContext != null)
        locationService = requestContext.GetService<ILocationService>();
      securedObject = securedObject ?? GitPullRequestModelExtensions.GetSecuredObject(repo);
      return workItemIds != null ? workItemIds.Select<int, ResourceRef>((Func<int, ResourceRef>) (x => new ResourceRef(securedObject)
      {
        Id = x.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        Url = GitPullRequestModelExtensions.GetWorkItemUrlById(requestContext, x, locationService)
      })) : (IEnumerable<ResourceRef>) new List<ResourceRef>();
    }

    public static GitCommitRef[] GenerateCommits(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      int top = 100,
      int skip = 0,
      string sourceCommitId = null,
      string targetCommitId = null,
      bool includeUserImageUrl = false)
    {
      IEnumerable<TfsGitCommitMetadata> gitCommitMetaData = pullRequest.GetCommits(requestContext, repo, new int?(top), new int?(skip), sourceCommitId, targetCommitId).ToGitCommitMetaData(repo);
      ISecuredObject securedObject = GitPullRequestModelExtensions.GetSecuredObject(repo);
      GitCommitTranslator translator = new GitCommitTranslator(requestContext, repo.Key);
      Func<TfsGitCommitMetadata, GitCommitRef> selector = (Func<TfsGitCommitMetadata, GitCommitRef>) (commitMetaData => translator.ToGitCommitShallow(commitMetaData, includeUserImageUrl, securedObject));
      IEnumerable<GitCommitRef> source = gitCommitMetaData.Select<TfsGitCommitMetadata, GitCommitRef>(selector);
      return source == null ? (GitCommitRef[]) null : source.ToArray<GitCommitRef>();
    }

    public static GitCommitRef[] GenerateCommitsWithContinuationToken(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      int top,
      PullRequestCommitsContinuationToken previousToken,
      out PullRequestCommitsContinuationToken currentToken)
    {
      Sha1Id rootCommitId;
      Sha1Id nextCommitId;
      IReadOnlyList<Sha1Id> usingContinuation = pullRequest.GetCommitsUsingContinuation(requestContext, repo, top, out rootCommitId, out nextCommitId, previousToken?.RootCommitId, previousToken?.NextCommitId);
      currentToken = nextCommitId != Sha1Id.Empty ? new PullRequestCommitsContinuationToken(rootCommitId, nextCommitId) : (PullRequestCommitsContinuationToken) null;
      ITfsGitRepository repo1 = repo;
      IEnumerable<TfsGitCommitMetadata> gitCommitMetaData = usingContinuation.ToGitCommitMetaData(repo1);
      ISecuredObject securedObject = GitPullRequestModelExtensions.GetSecuredObject(repo);
      GitCommitTranslator translator = new GitCommitTranslator(requestContext, repo.Key);
      Func<TfsGitCommitMetadata, GitCommitRef> selector = (Func<TfsGitCommitMetadata, GitCommitRef>) (commitMetaData => translator.ToGitCommitShallow(commitMetaData, false, securedObject));
      IEnumerable<GitCommitRef> source = gitCommitMetaData.Select<TfsGitCommitMetadata, GitCommitRef>(selector);
      return source == null ? (GitCommitRef[]) null : source.ToArray<GitCommitRef>();
    }

    public static WebApiTagDefinition[] GetWebApiPullRequestLabels(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository tfsGitRepository)
    {
      return requestContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestLabels(requestContext, tfsGitRepository, pullRequest).ToApiTagDefinitions().ToArray<WebApiTagDefinition>();
    }

    public static Dictionary<int, WebApiTagDefinition[]> GetWebApiPullRequestsLabels(
      this IEnumerable<TfsGitPullRequest> pullRequests,
      IVssRequestContext requestContext,
      IDictionary<string, TeamProjectReference> projectLookup)
    {
      Dictionary<int, Guid> dictionary = new Dictionary<int, Guid>();
      List<TagArtifact<int>> artifacts = new List<TagArtifact<int>>();
      foreach (TfsGitPullRequest pullRequest in pullRequests)
      {
        TeamProjectReference projectReference;
        if (pullRequest.ProjectUri != null && projectLookup.TryGetValue(pullRequest.ProjectUri, out projectReference))
          artifacts.Add(new TagArtifact<int>(projectReference.Id, pullRequest.PullRequestId));
        else
          requestContext.Trace(1013764, TraceLevel.Error, GitPullRequestModelExtensions.TraceAreaAndLayer, GitPullRequestModelExtensions.TraceAreaAndLayer, "Could not find project for pull request {0} with projectUri: {1}", (object) pullRequest.PullRequestId, (object) pullRequest.ProjectUri);
      }
      requestContext.Trace(1013739, TraceLevel.Verbose, GitPullRequestModelExtensions.TraceAreaAndLayer, GitPullRequestModelExtensions.TraceAreaAndLayer, "Trying to get all pull request labels for multiple pull requests {0}", (object) string.Join<int>(",", pullRequests.Select<TfsGitPullRequest, int>((Func<TfsGitPullRequest, int>) (pr => pr.PullRequestId))));
      IEnumerable<ArtifactTags<int>> tagsForArtifacts = requestContext.GetService<ITeamFoundationTaggingService>().GetTagsForArtifacts<int>(requestContext.Elevate(), GitWebApiConstants.PullRequestLabelsId, (IEnumerable<TagArtifact<int>>) artifacts);
      requestContext.Trace(1013740, TraceLevel.Verbose, GitPullRequestModelExtensions.TraceAreaAndLayer, GitPullRequestModelExtensions.TraceAreaAndLayer, "Successfully gathered labels for pull requests {0}", (object) string.Join<int>(",", pullRequests.Select<TfsGitPullRequest, int>((Func<TfsGitPullRequest, int>) (pr => pr.PullRequestId))));
      return tagsForArtifacts.ToDictionary<ArtifactTags<int>, int, WebApiTagDefinition[]>((Func<ArtifactTags<int>, int>) (artifactTags => artifactTags.Artifact.Id), (Func<ArtifactTags<int>, WebApiTagDefinition[]>) (artifactTags => artifactTags.Tags.ToApiTagDefinitions().ToArray<WebApiTagDefinition>()));
    }

    public static List<IterationCommitsData> GenerateCommitsBatch(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository tfsGitRepository,
      IList<GitPullRequestIteration> iterations,
      bool includeUserImageUrl)
    {
      IList<ReachableSetAndBoundary<Sha1Id>> commitsBatch1 = pullRequest.GetCommitsBatch(requestContext, tfsGitRepository, (IEnumerable<GitPullRequestIteration>) iterations);
      List<IterationCommitsData> commitsBatch2 = new List<IterationCommitsData>();
      IEnumerable<Sha1Id> second = (IEnumerable<Sha1Id>) new HashSet<Sha1Id>();
      GitCommitTranslator translator = new GitCommitTranslator(requestContext, tfsGitRepository.Key);
      for (int index = 0; index < commitsBatch1.Count; ++index)
      {
        bool hasMore = false;
        ReachableSetAndBoundary<Sha1Id> reachableSetAndBoundary = commitsBatch1[index];
        List<Sha1Id> list = reachableSetAndBoundary.ReachableSet.Take<Sha1Id>(101).ToList<Sha1Id>();
        if (list.Count > 100)
        {
          hasMore = true;
          list.RemoveRange(100, list.Count - 100);
        }
        IterationReason reason = IterationReason.Create;
        if (index > 0)
        {
          GitPullRequestIteration iteration1 = iterations[index];
          GitPullRequestIteration iteration2 = iterations[index - 1];
          if (iteration1.Reason == IterationReason.Retarget)
            reason = IterationReason.Retarget;
          else if (iteration1.Reason == IterationReason.ResolveConflicts)
            reason = IterationReason.ResolveConflicts;
          else if (iteration1.CommonRefCommit == null || iteration2.CommonRefCommit == null)
          {
            reason = IterationReason.Unknown;
          }
          else
          {
            reachableSetAndBoundary = commitsBatch1[index];
            if (reachableSetAndBoundary.Boundary.Contains<Sha1Id>(new Sha1Id(iteration2.SourceRefCommit.CommitId)))
            {
              reason = IterationReason.Push;
            }
            else
            {
              if (!string.Equals(iteration1.CommonRefCommit.CommitId, iteration2.CommonRefCommit.CommitId, StringComparison.OrdinalIgnoreCase))
              {
                reachableSetAndBoundary = commitsBatch1[index];
                if (!reachableSetAndBoundary.Boundary.Intersect<Sha1Id>(second).Any<Sha1Id>())
                {
                  reason = IterationReason.ForcePush | IterationReason.Rebase;
                  goto label_16;
                }
              }
              reason = IterationReason.ForcePush;
            }
          }
        }
label_16:
        if (reason == IterationReason.Push)
        {
          IEnumerable<Sha1Id> first = second;
          reachableSetAndBoundary = commitsBatch1[index];
          IEnumerable<Sha1Id> reachableSet = reachableSetAndBoundary.ReachableSet;
          first.Union<Sha1Id>(reachableSet);
        }
        else
        {
          reachableSetAndBoundary = commitsBatch1[index];
          second = reachableSetAndBoundary.ReachableSet;
        }
        ISecuredObject securedObject = GitPullRequestModelExtensions.GetSecuredObject(tfsGitRepository);
        IEnumerable<GitCommitRef> source = list.ToGitCommitMetaData(tfsGitRepository).Select<TfsGitCommitMetadata, GitCommitRef>((Func<TfsGitCommitMetadata, GitCommitRef>) (commitMetaData => translator.ToGitCommitShallow(commitMetaData, includeUserImageUrl, securedObject)));
        commitsBatch2.Add(new IterationCommitsData((IList<GitCommitRef>) source.ToList<GitCommitRef>(), hasMore, reason));
      }
      return commitsBatch2;
    }

    internal static string GetWorkItemUrlById(
      IVssRequestContext requestContext,
      int id,
      ILocationService locationService)
    {
      string workItemUrlById = string.Empty;
      if (locationService != null)
        workItemUrlById = locationService.GetResourceUri(requestContext, "wit", WitConstants.WorkItemTrackingLocationIds.WorkItems, (object) new
        {
          id = id
        }).AbsoluteUri;
      return workItemUrlById;
    }

    public static IDictionary<Guid, IdentityRef> GetIdentitiesLookupForPullRequests(
      IVssRequestContext requestContext,
      IEnumerable<TfsGitPullRequest> pullRequests,
      IdentitiesLookupType lookupType,
      bool minimalData = false)
    {
      HashSet<Guid> guidSet1 = new HashSet<Guid>();
      HashSet<Guid> guidSet2 = new HashSet<Guid>();
      foreach (TfsGitPullRequest pullRequest in pullRequests)
      {
        if (pullRequest != null)
        {
          if (pullRequest.Reviewers != null)
            guidSet1.AddRange<Guid, HashSet<Guid>>(pullRequest.Reviewers.Select<TfsGitPullRequest.ReviewerWithVote, Guid>((Func<TfsGitPullRequest.ReviewerWithVote, Guid>) (reviewer => reviewer.Reviewer)));
          if (lookupType == IdentitiesLookupType.All || lookupType == IdentitiesLookupType.ReviewersAndCreator)
            guidSet2.Add(pullRequest.Creator);
          if (lookupType == IdentitiesLookupType.All)
          {
            if (pullRequest.CompleteWhenMergedAuthority != new Guid())
              guidSet2.Add(pullRequest.CompleteWhenMergedAuthority);
            if (pullRequest.AutoCompleteAuthority != new Guid())
              guidSet2.Add(pullRequest.AutoCompleteAuthority);
          }
        }
      }
      guidSet1.AddRange<Guid, HashSet<Guid>>((IEnumerable<Guid>) guidSet2);
      return GitPullRequestModelExtensions.GenerateAndVerifyIdentitiesLookup(requestContext, (IEnumerable<Guid>) guidSet1, (IEnumerable<Guid>) guidSet2, minimalData);
    }

    private static IDictionary<Guid, IdentityRef> GenerateAndVerifyIdentitiesLookup(
      IVssRequestContext requestContext,
      IEnumerable<Guid> allTfids,
      IEnumerable<Guid> requiredTfids = null,
      bool minimalData = false)
    {
      IDictionary<Guid, IdentityRef> lookup = GitPullRequestModelExtensions.GenerateIdentitiesLookup(requestContext, allTfids.Distinct<Guid>().ToArray<Guid>(), minimalData);
      if (requiredTfids != null && requiredTfids.Any<Guid>((Func<Guid, bool>) (id => !lookup.ContainsKey(id))))
      {
        IEnumerable<string> strings = requiredTfids.Where<Guid>((Func<Guid, bool>) (id => !lookup.ContainsKey(id))).Select<Guid, string>((Func<Guid, string>) (id => id.ToString()));
        requestContext.Trace(1013712, TraceLevel.Error, GitPullRequestModelExtensions.TraceAreaAndLayer, GitPullRequestModelExtensions.TraceAreaAndLayer, "GenerateIdentitiesLookup did not return all required identities. Missing identities: " + string.Join(",", strings));
        throw new GitIdentityNotFoundException(strings.First<string>());
      }
      if (allTfids.Any<Guid>((Func<Guid, bool>) (id => !lookup.ContainsKey(id))))
      {
        IEnumerable<string> values = allTfids.Where<Guid>((Func<Guid, bool>) (id => !lookup.ContainsKey(id))).Select<Guid, string>((Func<Guid, string>) (id => id.ToString()));
        requestContext.Trace(1013713, TraceLevel.Error, GitPullRequestModelExtensions.TraceAreaAndLayer, GitPullRequestModelExtensions.TraceAreaAndLayer, "GenerateIdentitiesLookup did not return all identities. Missing identities: " + string.Join(",", values));
      }
      return lookup;
    }

    private static IdentityRefWithVote[] ToIdentityRefWithVotes(
      this IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewers,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      IDictionary<Guid, IdentityRef> identityLookup,
      bool includeLinks = true)
    {
      if (reviewers == null)
        return Array.Empty<IdentityRefWithVote>();
      includeLinks = includeLinks && (RepoScope) repoKey != (RepoScope) null;
      List<IdentityRefWithVote> identityRefWithVoteList = new List<IdentityRefWithVote>();
      foreach (TfsGitPullRequest.ReviewerWithVote reviewer in reviewers)
      {
        List<IdentityRefWithVote> votedFor = GitPullRequestModelExtensions.CreateVotedFor(requestContext, repoKey, pullRequestId, identityLookup, reviewer.VotedFor, (RepoScope) repoKey != (RepoScope) null);
        IdentityRef identity = identityLookup.ContainsKey(reviewer.Reviewer) ? identityLookup[reviewer.Reviewer] : (IdentityRef) null;
        if (identity != null)
          identityRefWithVoteList.Add(new IdentityRefWithVote(identity)
          {
            Vote = reviewer.Vote,
            IsRequired = reviewer.IsRequired,
            VotedFor = votedFor.Count > 0 ? votedFor.ToArray() : (IdentityRefWithVote[]) null,
            ReviewerUrl = !includeLinks ? (string) null : GitPullRequestModelExtensions.GetPullRequestReviewerUrl(requestContext, repoKey, pullRequestId, reviewer.Reviewer.ToString()),
            IsFlagged = new bool?(reviewer.IsFlagged),
            HasDeclined = new bool?(reviewer.HasDeclined)
          });
      }
      return identityRefWithVoteList.ToArray();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IdentityRefWithVote[] ToIdentityRefWithVotes(
      this IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewers,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId)
    {
      reviewers = reviewers ?? Enumerable.Empty<TfsGitPullRequest.ReviewerWithVote>();
      IDictionary<Guid, IdentityRef> identitiesLookup = GitPullRequestModelExtensions.GenerateAndVerifyIdentitiesLookup(requestContext, reviewers.Select<TfsGitPullRequest.ReviewerWithVote, Guid>((Func<TfsGitPullRequest.ReviewerWithVote, Guid>) (x => x.Reviewer)));
      return reviewers.ToIdentityRefWithVotes(requestContext, repoKey, pullRequestId, identitiesLookup);
    }

    private static List<IdentityRefWithVote> CreateVotedFor(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      IDictionary<Guid, IdentityRef> identityLookup,
      IReadOnlyList<Guid> votedForGuids,
      bool includeReviewerUrl)
    {
      List<IdentityRefWithVote> votedFor = new List<IdentityRefWithVote>(votedForGuids.Count);
      foreach (Guid votedForGuid in (IEnumerable<Guid>) votedForGuids)
      {
        IdentityRef identity = identityLookup.ContainsKey(votedForGuid) ? identityLookup[votedForGuid] : (IdentityRef) null;
        if (identity != null)
          votedFor.Add(new IdentityRefWithVote(identity)
          {
            ReviewerUrl = includeReviewerUrl ? GitPullRequestModelExtensions.GetPullRequestReviewerUrl(requestContext, repoKey, pullRequestId, identity.Id) : (string) null
          });
      }
      return votedFor;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IdentityRefWithVote[] GenerateVoters(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      RepoKey repoKey,
      out IdentityRef autoCompleteAuthority,
      out IdentityRef closedBy,
      out IdentityRef creator)
    {
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewers = pullRequest.Reviewers;
      IDictionary<Guid, IdentityRef> lookupForPullRequests = GitPullRequestModelExtensions.GetIdentitiesLookupForPullRequests(requestContext, (IEnumerable<TfsGitPullRequest>) new TfsGitPullRequest[1]
      {
        pullRequest
      }, IdentitiesLookupType.All);
      creator = lookupForPullRequests[pullRequest.Creator];
      Guid whenMergedAuthority = pullRequest.CompleteWhenMergedAuthority;
      Guid guid1 = new Guid();
      closedBy = !(whenMergedAuthority != guid1) ? (IdentityRef) null : lookupForPullRequests[pullRequest.CompleteWhenMergedAuthority];
      Guid completeAuthority = pullRequest.AutoCompleteAuthority;
      Guid guid2 = new Guid();
      autoCompleteAuthority = !(completeAuthority != guid2) ? (IdentityRef) null : lookupForPullRequests[pullRequest.AutoCompleteAuthority];
      return reviewers.ToIdentityRefWithVotes(requestContext, repoKey, pullRequest.PullRequestId, lookupForPullRequests);
    }

    private static GitCommitRef GetOrDefaultGitCommitRef(
      IVssRequestContext requestContext,
      Sha1Id? b,
      RepoKey repoKey)
    {
      if (!b.HasValue || b.Value.IsEmpty)
        return (GitCommitRef) null;
      string commitId = b.Value.ToString();
      return new GitCommitRef()
      {
        CommitId = commitId,
        Url = GitReferenceLinksUtility.GetCommitRefUrl(requestContext, repoKey, commitId, (UrlHelper) null)
      };
    }

    private static string GetPullRequestRESTUrl(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      RepoKey repoKey)
    {
      string pullRequestRestUrl = (string) null;
      if (pullRequest != null && requestContext != null)
        pullRequestRestUrl = requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestsLocationId, RouteValuesFactory.PullRequest(repoKey, pullRequest.PullRequestId)).AbsoluteUri;
      return pullRequestRestUrl;
    }

    private static string GetPullRequestReviewerUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      string reviewerId)
    {
      return requestContext == null ? (string) null : requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestReviewersLocationId, RouteValuesFactory.PRReviewer(repoKey, pullRequestId, reviewerId)).AbsoluteUri;
    }

    private static ISecuredObject GetSecuredObject(ITfsGitRepository repository) => GitSecuredObjectFactory.CreateRepositoryReadOnly(repository.Key);
  }
}
