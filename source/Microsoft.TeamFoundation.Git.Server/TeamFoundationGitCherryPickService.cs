// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitCherryPickService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TeamFoundationGitCherryPickService : 
    TeamFoundationGitAsyncRefOperationService,
    ITeamFoundationGitCherryPickService,
    ITeamFoundationGitAsyncRefOperationService,
    IVssFrameworkService
  {
    private const string s_Layer = "GitCherryPick";
    private const string c_WorkItemLinkType = "Fixed in Commit";

    protected override GitAsyncOperationType OperationType => GitAsyncOperationType.CherryPick;

    protected override string CustomerIntelligenceInfoCreateActionName => "CreateCherryPick";

    protected override string CustomerIntelligenceInfoFeatureName => "CherryPick";

    protected override void ValidateSourcePullRequest(TfsGitPullRequest pullRequest)
    {
    }

    protected override IReadOnlyList<Sha1Id> GetPullRequestCommitsForOperation(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest)
    {
      return (IReadOnlyList<Sha1Id>) pullRequest.GetCommits(requestContext, repository, new int?(2147483646)).Reverse<Sha1Id>().ToList<Sha1Id>();
    }

    protected override void QueueJobToRun(
      IVssRequestContext requestContext,
      Guid repositoryId,
      int operationId)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      requestContext.TraceAlways(1013826, TraceLevel.Verbose, GitServerUtils.TraceArea, "GitCherryPick", string.Format("CherryPick: {0}/{1}", (object) repositoryId, (object) operationId));
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new AsyncGitOperationJobData()
      {
        RepositoryId = repositoryId,
        OperationId = operationId
      });
      IVssRequestContext requestContext1 = requestContext;
      string jobName = string.Format("Git Native Cherry-pick Job (repo: {0}, op: {1})", (object) repositoryId, (object) operationId);
      XmlNode jobData = xml;
      TimeSpan zero = TimeSpan.Zero;
      service.QueueOneTimeJob(requestContext1, jobName, "Microsoft.TeamFoundation.Git.Server.Plugins.GitNativeCherryPickJob", jobData, JobPriorityLevel.Normal, JobPriorityClass.High, zero);
    }

    protected override AsyncRefOperationResult RunOperationOnRepository(
      IVssRequestContext context,
      ITfsGitRepository repository,
      GitAsyncRefOperation operationData,
      IReadOnlyList<Sha1Id> commits,
      Action<Sha1Id, double> progressCallback,
      Action<Sha1Id> conflictCallback,
      ClientTraceData ctData,
      out List<Sha1Id> resultCommits,
      out Sha1Id resultBaseCommitId,
      out GitAsyncRefOperationDetail operationDetail)
    {
      operationDetail = (GitAsyncRefOperationDetail) null;
      AsyncRefOperationResult refOperationResult;
      try
      {
        refOperationResult = repository.CreateNativeCherryPick(context, operationData, commits, progressCallback, conflictCallback, ctData, out resultCommits, out resultBaseCommitId);
      }
      catch (GitRefNotFoundException ex)
      {
        resultBaseCommitId = Sha1Id.Empty;
        resultCommits = (List<Sha1Id>) null;
        refOperationResult = AsyncRefOperationResult.Failure;
        operationDetail = new GitAsyncRefOperationDetail()
        {
          Status = new GitAsyncRefOperationFailureStatus?(GitAsyncRefOperationFailureStatus.TargetBranchDeleted),
          FailureMessage = ex.Message
        };
      }
      catch (EmptySignatureException ex)
      {
        resultBaseCommitId = Sha1Id.Empty;
        resultCommits = (List<Sha1Id>) null;
        refOperationResult = AsyncRefOperationResult.Failure;
        operationDetail = new GitAsyncRefOperationDetail()
        {
          Status = new GitAsyncRefOperationFailureStatus?(GitAsyncRefOperationFailureStatus.EmptyCommitterSignature),
          FailureMessage = ex.Message
        };
      }
      if (refOperationResult == AsyncRefOperationResult.Success)
        this.LinkWorkItemsToNewCommits(context, repository.Key, operationData, commits, resultCommits);
      return refOperationResult;
    }

    private void LinkWorkItemsToNewCommits(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitAsyncRefOperation operationData,
      IReadOnlyList<Sha1Id> initialCommits,
      List<Sha1Id> resultCommits)
    {
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      IEnumerable<string> artifactUris = initialCommits.Select<Sha1Id, string>((Func<Sha1Id, string>) (id => LinkingUtilities.EncodeUri(GitCommitArtifactId.GetArtifactIdForCommit(repoKey, id))));
      IEnumerable<ArtifactUriQueryResult> idsForArtifactUris = service.GetWorkItemIdsForArtifactUris(requestContext, artifactUris);
      foreach (Tuple<Sha1Id, ArtifactUriQueryResult> tuple in resultCommits.Zip<Sha1Id, ArtifactUriQueryResult, Tuple<Sha1Id, ArtifactUriQueryResult>>(idsForArtifactUris, (Func<Sha1Id, ArtifactUriQueryResult, Tuple<Sha1Id, ArtifactUriQueryResult>>) ((commit, items) => Tuple.Create<Sha1Id, ArtifactUriQueryResult>(commit, items))))
      {
        if (tuple.Item2.WorkItemIds.Any<int>())
        {
          string artifactUri = LinkingUtilities.EncodeUri(GitCommitArtifactId.GetArtifactIdForCommit(repoKey, tuple.Item1));
          IEnumerable<WorkItemUpdate> workItemUpdates = tuple.Item2.WorkItemIds.Select<int, WorkItemUpdate>((Func<int, WorkItemUpdate>) (workItemId =>
          {
            WorkItemUpdate newCommits = new WorkItemUpdate();
            newCommits.Id = workItemId;
            WorkItemUpdate workItemUpdate = newCommits;
            WorkItemResourceLinkUpdate[] resourceLinkUpdateArray = new WorkItemResourceLinkUpdate[1]
            {
              new WorkItemResourceLinkUpdate()
              {
                UpdateType = LinkUpdateType.Add,
                Type = new ResourceLinkType?(ResourceLinkType.ArtifactLink),
                Location = artifactUri,
                Name = ArtifactLinkIds.Commit
              }
            };
            workItemUpdate.ResourceLinkUpdates = (IEnumerable<WorkItemResourceLinkUpdate>) resourceLinkUpdateArray;
            return newCommits;
          }));
          try
          {
            service.UpdateWorkItems(requestContext, workItemUpdates, includeInRecentActivity: true);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1013588, TraceLevel.Error, GitServerUtils.TraceArea, "GitCherryPick", ex);
          }
        }
      }
    }

    protected override void DoPostRefUpdateActions(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitAsyncRefOperation operationData)
    {
      if (!operationData.Parameters.Source.PullRequestId.HasValue)
        return;
      string[] artifactUris = new string[1]
      {
        LinkingUtilities.EncodeUri(PullRequestArtifactId.GetArtifactIdForPullRequest(repository.Key.GetProjectUri(), repository.Key.RepoId, operationData.Parameters.Source.PullRequestId.Value))
      };
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      IEnumerable<ArtifactUriQueryResult> idsForArtifactUris = service.GetWorkItemIdsForArtifactUris(requestContext, (IEnumerable<string>) artifactUris);
      if (!idsForArtifactUris.Any<ArtifactUriQueryResult>())
        return;
      string artifactUriForRef = GitRefArtifactId.GetArtifactUriForRef(repository.Key, "GB" + GitUtils.GetFriendlyBranchName(operationData.Parameters.GeneratedRefName));
      WorkItemResourceLinkUpdate resourceLinkUpdate = new WorkItemResourceLinkUpdate();
      resourceLinkUpdate.UpdateType = LinkUpdateType.Add;
      resourceLinkUpdate.Type = new ResourceLinkType?(ResourceLinkType.ArtifactLink);
      resourceLinkUpdate.Location = artifactUriForRef;
      resourceLinkUpdate.Name = ArtifactLinkIds.Branch;
      WorkItemResourceLinkUpdate[] witLinkUpdateArray = new WorkItemResourceLinkUpdate[1]
      {
        resourceLinkUpdate
      };
      IEnumerable<WorkItemUpdate> workItemUpdates = idsForArtifactUris.SelectMany<ArtifactUriQueryResult, int>((Func<ArtifactUriQueryResult, IEnumerable<int>>) (workItemLinks => workItemLinks.WorkItemIds)).Select<int, WorkItemUpdate>((Func<int, WorkItemUpdate>) (workItemId => new WorkItemUpdate()
      {
        Id = workItemId,
        ResourceLinkUpdates = (IEnumerable<WorkItemResourceLinkUpdate>) witLinkUpdateArray
      }));
      try
      {
        service.UpdateWorkItems(requestContext, workItemUpdates, includeInRecentActivity: true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1013588, TraceLevel.Error, GitServerUtils.TraceArea, "GitCherryPick", ex);
      }
    }
  }
}
