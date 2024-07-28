// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitRevertService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TeamFoundationGitRevertService : 
    TeamFoundationGitAsyncRefOperationService,
    ITeamFoundationGitRevertService,
    ITeamFoundationGitAsyncRefOperationService,
    IVssFrameworkService
  {
    private const string s_Layer = "GitRevert";

    protected override GitAsyncOperationType OperationType => GitAsyncOperationType.Revert;

    protected override string CustomerIntelligenceInfoCreateActionName => "CreateRevert";

    protected override string CustomerIntelligenceInfoFeatureName => "Revert";

    protected override void ValidateSourcePullRequest(TfsGitPullRequest pullRequest)
    {
      Sha1Id? nullable = pullRequest.Status == PullRequestStatus.Completed ? pullRequest.LastMergeCommit : throw new GitAsyncRefOperationInvalidSourceException(Resources.Get("RevertSourceMustBeCompleted"));
      if (nullable.HasValue)
      {
        nullable = pullRequest.LastMergeCommit;
        if (!(nullable.Value == Sha1Id.Empty))
        {
          GitPullRequestCompletionOptions completionOptions = pullRequest.CompletionOptions;
          int num;
          if (completionOptions == null)
          {
            num = 0;
          }
          else
          {
            GitPullRequestMergeStrategy? mergeStrategy = completionOptions.MergeStrategy;
            GitPullRequestMergeStrategy requestMergeStrategy = GitPullRequestMergeStrategy.Rebase;
            num = mergeStrategy.GetValueOrDefault() == requestMergeStrategy & mergeStrategy.HasValue ? 1 : 0;
          }
          if (num == 0)
            return;
          nullable = pullRequest.LastMergeTargetCommit;
          nullable = nullable.HasValue ? pullRequest.LastMergeTargetCommit : throw new GitAsyncRefOperationInvalidSourceException(Resources.Get("AsyncOperationSourceError"));
          if (!(nullable.Value == Sha1Id.Empty))
            ;
        }
      }
    }

    protected override IReadOnlyList<Sha1Id> GetPullRequestCommitsForOperation(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest)
    {
      ITfsGitRepository repo = repository;
      IVssRequestContext rc = requestContext;
      Sha1Id? nullable = pullRequest.LastMergeCommit;
      Sha1Id inHistoryOfCommit = nullable.Value;
      nullable = pullRequest.LastMergeTargetCommit;
      Sha1Id? notInHistoryOfCommit = new Sha1Id?(nullable.Value);
      return (IReadOnlyList<Sha1Id>) repo.GetCommitHistory(rc, inHistoryOfCommit, notInHistoryOfCommit).Where<Sha1Id>((Func<Sha1Id, bool>) (commitId => repository.IsAncestor(requestContext, commitId, pullRequest.LastMergeTargetCommit.Value))).ToList<Sha1Id>().AsReadOnly();
    }

    protected override void QueueJobToRun(
      IVssRequestContext requestContext,
      Guid repositoryId,
      int operationId)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      requestContext.TraceAlways(1013827, TraceLevel.Verbose, GitServerUtils.TraceArea, "GitRevert", string.Format("Revert: {0}/{1}", (object) repositoryId, (object) operationId));
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new AsyncGitOperationJobData()
      {
        RepositoryId = repositoryId,
        OperationId = operationId
      });
      IVssRequestContext requestContext1 = requestContext;
      string jobName = string.Format("Git Native Revert Job (repo: {0}, op: {1})", (object) repositoryId, (object) operationId);
      XmlNode jobData = xml;
      TimeSpan zero = TimeSpan.Zero;
      service.QueueOneTimeJob(requestContext1, jobName, "Microsoft.TeamFoundation.Git.Server.Plugins.GitNativeRevertJob", jobData, JobPriorityLevel.Normal, JobPriorityClass.High, zero);
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
      try
      {
        return repository.CreateNativeRevert(context, operationData, commits, progressCallback, conflictCallback, ctData, out resultCommits, out resultBaseCommitId);
      }
      catch (GitRefNotFoundException ex)
      {
        resultBaseCommitId = Sha1Id.Empty;
        resultCommits = (List<Sha1Id>) null;
        operationDetail = new GitAsyncRefOperationDetail()
        {
          Status = new GitAsyncRefOperationFailureStatus?(GitAsyncRefOperationFailureStatus.TargetBranchDeleted),
          FailureMessage = ex.Message
        };
        return AsyncRefOperationResult.Failure;
      }
      catch (EmptySignatureException ex)
      {
        resultBaseCommitId = Sha1Id.Empty;
        resultCommits = (List<Sha1Id>) null;
        operationDetail = new GitAsyncRefOperationDetail()
        {
          Status = new GitAsyncRefOperationFailureStatus?(GitAsyncRefOperationFailureStatus.EmptyCommitterSignature),
          FailureMessage = ex.Message
        };
        return AsyncRefOperationResult.Failure;
      }
    }
  }
}
