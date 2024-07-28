// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitAsyncRefOperationService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Framework.Server.Tracing;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public abstract class TeamFoundationGitAsyncRefOperationService : 
    TeamFoundationGitAsyncOperationService<GitAsyncRefOperation, GitAsyncRefOperationParameters, GitAsyncRefOperationDetail>,
    ITeamFoundationGitAsyncRefOperationService,
    IVssFrameworkService
  {
    private const string s_Layer = "GitAsyncRefOperation";
    private const int c_maxOperationRetryAttemptsDefaultValue = 2;

    protected abstract string CustomerIntelligenceInfoCreateActionName { get; }

    protected abstract string CustomerIntelligenceInfoFeatureName { get; }

    protected abstract GitAsyncOperationType OperationType { get; }

    public GitAsyncRefOperation CreateAsyncRefOperation(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitAsyncRefOperationParameters parameters)
    {
      requestContext.Trace(1013568, TraceLevel.Verbose, GitServerUtils.TraceArea, "GitAsyncRefOperation", "Creating {0} from source {1} onto {2} in ref {3}", (object) this.OperationType.ToString(), (object) parameters.Source.GetSourceString(), (object) parameters.OntoRefName, (object) parameters.GeneratedRefName);
      ClientTraceData properties = new ClientTraceData();
      properties.Add("Action", (object) this.CustomerIntelligenceInfoCreateActionName);
      properties.Add("RepositoryId", (object) repository.Key.RepoId.ToString());
      properties.Add("RepositoryName", (object) repository.Name);
      repository.Permissions.CanCreateBranch(parameters.GeneratedRefName);
      if (repository.Refs.MatchingName(parameters.OntoRefName) == null)
        throw new GitAsyncRefOperationInvalidOntoOrGeneratedRefs(Resources.Get("AsyncRefOperationInvalidOntoRef"));
      if (repository.Refs.MatchingName(parameters.GeneratedRefName) != null)
        throw new GitAsyncRefOperationInvalidOntoOrGeneratedRefs(Resources.Format("AsyncRefOperationInvalidGeneratedRef", (object) GitUtils.GetFriendlyBranchName(parameters.GeneratedRefName)));
      this.ValidateSource(requestContext, repository.Key.RepoId, parameters.Source);
      GitAsyncRefOperation asyncOperation = this.CreateAsyncOperation(requestContext, repository.Key, this.OperationType, parameters);
      Guid orchestrationId = TeamFoundationGitAsyncRefOperationService.OrchestrationIdForAsyncRefOperation(repository.Key.RepoId, asyncOperation.OperationId);
      string feature;
      switch (this.OperationType)
      {
        case GitAsyncOperationType.CherryPick:
          feature = "AsyncCherryPick";
          break;
        case GitAsyncOperationType.Revert:
          feature = "AsyncRevert";
          break;
        default:
          feature = this.OperationType.ToString();
          break;
      }
      requestContext.GetService<IOrchestrationLogTracingService>().TraceOrchestrationLogNewOrchestration(requestContext, orchestrationId, -1L, GitServerUtils.TraceArea, feature);
      requestContext.Trace(1013573, TraceLevel.Verbose, GitServerUtils.TraceArea, "GitAsyncRefOperation", "Created {0} with operation id {1} for source {2}", (object) this.OperationType.ToString(), (object) asyncOperation.OperationId, (object) asyncOperation.Parameters.Source.GetSourceString());
      this.QueueJobToRun(requestContext, asyncOperation.RepositoryId, asyncOperation.OperationId);
      properties.Add("AsyncOperationId", (object) asyncOperation.OperationId);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", this.CustomerIntelligenceInfoFeatureName, properties);
      return asyncOperation;
    }

    public GitAsyncRefOperation GetAsyncRefOperationById(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int operationId)
    {
      return this.GetAsyncOperationById(requestContext, repository.Key, operationId, true);
    }

    public GitAsyncRefOperation GetAsyncRefOperationByRef(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string refName)
    {
      return this.QueryAsyncOperationsByType(requestContext, repository.Key, this.OperationType).FirstOrDefault<GitAsyncRefOperation>((Func<GitAsyncRefOperation, bool>) (x => x.Parameters.GeneratedRefName.Equals(refName))) ?? throw new GitAsyncRefOperationNotFoundException(this.OperationType.ToString(), refName);
    }

    public AsyncRefOperationResult PerformAsyncRefOperation(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      int operationId,
      ClientTraceData ctData,
      out GitAsyncRefOperationDetail operationDetail)
    {
      GitAsyncRefOperation asyncOperation = this.GetAsyncOperationById(requestContext, repo.Key, operationId, false);
      if (asyncOperation == null)
      {
        operationDetail = new GitAsyncRefOperationDetail()
        {
          FailureMessage = string.Format("No active {0} found with operation id {1}", (object) this.OperationType.ToString(), (object) operationId),
          Status = new GitAsyncRefOperationFailureStatus?(GitAsyncRefOperationFailureStatus.AsyncOperationNotFound)
        };
        requestContext.Trace(1013570, TraceLevel.Verbose, GitServerUtils.TraceArea, "GitAsyncRefOperation", operationDetail.FailureMessage);
        return AsyncRefOperationResult.Failure;
      }
      ctData.Add("AsyncOperationId", (object) asyncOperation.OperationId);
      ctData.Add("RepositoryId", (object) asyncOperation.RepositoryId);
      IdentityDescriptor identityDescriptor = IdentityHelper.Instance.GetIdentityDescriptor(requestContext, asyncOperation.CreatorId);
      if (identityDescriptor == (IdentityDescriptor) null)
      {
        operationDetail = new GitAsyncRefOperationDetail()
        {
          FailureMessage = "Can't find the user who authorized the operation",
          Status = new GitAsyncRefOperationFailureStatus?(GitAsyncRefOperationFailureStatus.OperationIndentityNotFound)
        };
        this.UpdateAsyncOperationStatus(requestContext, repo.Key, operationId, GitAsyncOperationStatus.Failed, operationDetail, new GitAsyncOperationStatus?());
        requestContext.Trace(1013577, TraceLevel.Verbose, GitServerUtils.TraceArea, "GitAsyncRefOperation", operationDetail.FailureMessage);
        return AsyncRefOperationResult.Failure;
      }
      using (IVssRequestContext userContext = requestContext.CreateUserContext(identityDescriptor))
      {
        userContext.RequestTimeout = requestContext.RequestTimeout;
        userContext.ProgressTimerJoin(requestContext);
        ITeamFoundationGitRepositoryService service = userContext.GetService<ITeamFoundationGitRepositoryService>();
        IAsyncGitOperationDispatcher dispatcher = requestContext.GetService<IAsyncGitOperationDispatcher>();
        using (ITfsGitRepository repository = service.FindRepositoryById(userContext, asyncOperation.RepositoryId))
        {
          try
          {
            this.UpdateAsyncOperationStatus(userContext, repository.Key, operationId, GitAsyncOperationStatus.InProgress, (GitAsyncRefOperationDetail) null, new GitAsyncOperationStatus?());
            IReadOnlyList<Sha1Id> sha1IdList = (IReadOnlyList<Sha1Id>) null;
            int? pullRequestId1 = asyncOperation.Parameters.Source.PullRequestId;
            if (pullRequestId1.HasValue)
            {
              pullRequestId1 = asyncOperation.Parameters.Source.PullRequestId;
              int pullRequestId2 = pullRequestId1.Value;
              ctData.Add("PullRequestId", (object) pullRequestId2);
              TfsGitPullRequest pullRequestDetails = userContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestDetails(userContext, repository, pullRequestId2);
              sha1IdList = this.GetPullRequestCommitsForOperation(requestContext, repository, pullRequestDetails);
            }
            else if (asyncOperation.Parameters.Source.CommitList != null)
            {
              sha1IdList = (IReadOnlyList<Sha1Id>) asyncOperation.Parameters.Source.CommitList;
              ctData.Add("SrcCommitId", (object) sha1IdList.FirstOrDefault<Sha1Id>().ToString());
            }
            if (sha1IdList == null || sha1IdList.Any<Sha1Id>((Func<Sha1Id, bool>) (commitId => false)))
            {
              requestContext.Trace(1013576, TraceLevel.Error, GitServerUtils.TraceArea, "GitAsyncRefOperation", "commit list for {0} ({1}) operation {2}", (object) this.OperationType.ToString(), (object) operationId, sha1IdList == null ? (object) "is null" : (object) "contains nulls");
              throw new GitAsyncOperationSourceException();
            }
            List<Sha1Id> resultCommits = (List<Sha1Id>) null;
            bool generateNewRef = asyncOperation.Parameters.GeneratedRefName != null;
            int num = 0;
            int maxRetryCount = this.GetMaxRetryCount(requestContext, generateNewRef);
            double maxProgressMade = 0.0;
            ctData.Add("GenerateNewRef", (object) generateNewRef);
            TfsGitRefUpdateResultSet refUpdateResultSet;
            do
            {
              bool hadConflict = false;
              if (num > 0)
              {
                repository.Dispose();
                repository = service.FindRepositoryById(userContext, asyncOperation.RepositoryId);
              }
              Sha1Id resultBaseCommitId;
              AsyncRefOperationResult refOperationResult = this.RunOperationOnRepository(userContext, repository, asyncOperation, sha1IdList, closure_0 ?? (closure_0 = (Action<Sha1Id, double>) ((sha, percent) => this.UpdateProgress(requestContext, repository, dispatcher, operationId, sha, maxProgressMade = this.GetProgressPercent(generateNewRef, percent, maxProgressMade)))), (Action<Sha1Id>) (sha =>
              {
                hadConflict = true;
                this.UpdateConflict(requestContext, repository, dispatcher, operationId, sha);
              }), ctData, out resultCommits, out resultBaseCommitId, out operationDetail);
              if (hadConflict)
                return AsyncRefOperationResult.Conflict;
              if (refOperationResult == AsyncRefOperationResult.Timeout)
                this.UpdateTimeout(requestContext, repository, dispatcher, operationId);
              if (refOperationResult == AsyncRefOperationResult.Failure && operationDetail != null)
              {
                this.UpdateAsyncOperationStatus(userContext, repository.Key, operationId, GitAsyncOperationStatus.Failed, operationDetail, new GitAsyncOperationStatus?());
                return AsyncRefOperationResult.Failure;
              }
              if (refOperationResult != AsyncRefOperationResult.Success)
              {
                operationDetail = new GitAsyncRefOperationDetail()
                {
                  FailureMessage = "Error performing operation"
                };
                return refOperationResult;
              }
              refUpdateResultSet = !generateNewRef ? repository.UpdateTargetBranchRef(userContext, asyncOperation, resultCommits, resultBaseCommitId) : repository.UpdateGeneratedBranchRef(userContext, asyncOperation, resultCommits, resultBaseCommitId);
              ++num;
            }
            while (refUpdateResultSet.Results[0].Status == GitRefUpdateStatus.StaleOldObjectId && num <= maxRetryCount);
            ctData.Add("RetryCount", (object) (num - 1));
            if (refUpdateResultSet.CountFailed != 0)
            {
              GitAsyncRefOperationFailureStatus operationFailureStatus = refUpdateResultSet.CountFailed == 1 ? this.ToGitAsyncRefOperationFailureStatus(new GitRefUpdateStatus?(refUpdateResultSet.Results.First<TfsGitRefUpdateResult>((Func<TfsGitRefUpdateResult, bool>) (res => res != null && !res.Succeeded)).Status)) : GitAsyncRefOperationFailureStatus.Other;
              IEnumerable<string> values = refUpdateResultSet.Results.Where<TfsGitRefUpdateResult>((Func<TfsGitRefUpdateResult, bool>) (res => res != null && !res.Succeeded)).Select<TfsGitRefUpdateResult, string>((Func<TfsGitRefUpdateResult, string>) (res => string.IsNullOrEmpty(res.CustomMessage) ? res.Status.ToString() : res.CustomMessage));
              operationDetail = new GitAsyncRefOperationDetail()
              {
                Status = new GitAsyncRefOperationFailureStatus?(operationFailureStatus),
                FailureMessage = string.Join(" ", values)
              };
              this.UpdateAsyncOperationStatus(userContext, repository.Key, operationId, GitAsyncOperationStatus.Failed, operationDetail, new GitAsyncOperationStatus?());
              dispatcher.SendFailureNotification(requestContext, (AsyncGitOperationNotification) new AsyncRefOperationGeneralFailureNotification(operationId));
              TfsGitRefUpdateResult[] failures = refUpdateResultSet.Results.Where<TfsGitRefUpdateResult>((Func<TfsGitRefUpdateResult, bool>) (res => res != null && !res.Succeeded)).ToArray<TfsGitRefUpdateResult>();
              TracepointUtils.Tracepoint(requestContext, 1013725, GitServerUtils.TraceArea, "GitAsyncRefOperation", (Func<object>) (() => (object) new
              {
                RepoId = repository.Key.RepoId,
                OperationId = asyncOperation.OperationId,
                PullRequestId = asyncOperation.Parameters.Source.PullRequestId,
                failures = failures
              }), TraceLevel.Error, caller: nameof (PerformAsyncRefOperation));
              return AsyncRefOperationResult.Failure;
            }
            if (!generateNewRef)
              this.UpdateProgress(requestContext, repository, dispatcher, operationId, resultCommits.Last<Sha1Id>(), 1.0);
            this.DoPostRefUpdateActions(userContext, repository, asyncOperation);
            this.UpdateAsyncOperationStatus(userContext, repository.Key, operationId, GitAsyncOperationStatus.Completed, (GitAsyncRefOperationDetail) null, new GitAsyncOperationStatus?());
            dispatcher.SendCompletionNotification(requestContext, (AsyncGitOperationNotification) new AsyncRefOperationCompletedNotification(operationId, GitUtils.GetFriendlyBranchName(asyncOperation.Parameters.GeneratedRefName)));
            return AsyncRefOperationResult.Success;
          }
          catch (LibGit2SharpException ex)
          {
            TracepointUtils.TraceException(requestContext, 1013702, GitServerUtils.TraceArea, "GitAsyncRefOperation", (Exception) ex, (object) new
            {
              RepoId = repository.Key.RepoId,
              OperationId = asyncOperation.OperationId,
              PullRequestId = asyncOperation.Parameters.Source.PullRequestId
            }, caller: nameof (PerformAsyncRefOperation));
            bool flag = ex.Message.StartsWith("TF401022");
            operationDetail = new GitAsyncRefOperationDetail()
            {
              Status = new GitAsyncRefOperationFailureStatus?(flag ? GitAsyncRefOperationFailureStatus.GitObjectTooLarge : GitAsyncRefOperationFailureStatus.Other),
              FailureMessage = ex.Message
            };
            this.UpdateAsyncOperationStatus(userContext, repository.Key, operationId, GitAsyncOperationStatus.Failed, operationDetail, new GitAsyncOperationStatus?());
            dispatcher.SendFailureNotification(requestContext, (AsyncGitOperationNotification) new AsyncRefOperationGeneralFailureNotification(operationId));
            if (flag)
              return AsyncRefOperationResult.Failure;
            throw;
          }
          catch (Exception ex)
          {
            TracepointUtils.TraceException(requestContext, 1013571, GitServerUtils.TraceArea, "GitAsyncRefOperation", ex, (object) new
            {
              RepoId = repository.Key.RepoId,
              OperationId = asyncOperation.OperationId,
              PullRequestId = asyncOperation.Parameters.Source.PullRequestId
            }, caller: nameof (PerformAsyncRefOperation));
            operationDetail = new GitAsyncRefOperationDetail()
            {
              Status = new GitAsyncRefOperationFailureStatus?(GitAsyncRefOperationFailureStatus.Other),
              FailureMessage = ex.Message
            };
            this.UpdateAsyncOperationStatus(userContext, repository.Key, operationId, GitAsyncOperationStatus.Failed, operationDetail, new GitAsyncOperationStatus?());
            dispatcher.SendFailureNotification(userContext, (AsyncGitOperationNotification) new AsyncRefOperationGeneralFailureNotification(operationId));
            throw;
          }
        }
      }
    }

    private double GetProgressPercent(
      bool generateNewRef,
      double latestProgress,
      double maxProgressMade)
    {
      if (!generateNewRef)
        latestProgress *= 0.8;
      return latestProgress <= maxProgressMade ? maxProgressMade : latestProgress;
    }

    private void UpdateProgress(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IAsyncGitOperationDispatcher operationDispatcher,
      int operationId,
      Sha1Id commitId,
      double percent)
    {
      IVssRequestContext requestContext1 = requestContext;
      RepoKey key = repository.Key;
      int operationId1 = operationId;
      GitAsyncRefOperationDetail detailedStatus = new GitAsyncRefOperationDetail();
      detailedStatus.CurrentCommitId = commitId.ToAbbreviatedString();
      detailedStatus.Progress = percent;
      GitAsyncOperationStatus? previousStatus = new GitAsyncOperationStatus?();
      this.UpdateAsyncOperationStatus(requestContext1, key, operationId1, GitAsyncOperationStatus.InProgress, detailedStatus, previousStatus);
      operationDispatcher.SendProgressNotification(requestContext, (AsyncGitOperationNotification) new AsyncRefOperationProgressNotification(operationId, commitId.ToAbbreviatedString(), percent));
    }

    private void UpdateConflict(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IAsyncGitOperationDispatcher operationDispatcher,
      int operationId,
      Sha1Id commitId)
    {
      IVssRequestContext requestContext1 = requestContext;
      RepoKey key = repository.Key;
      int operationId1 = operationId;
      GitAsyncRefOperationDetail detailedStatus = new GitAsyncRefOperationDetail();
      detailedStatus.CurrentCommitId = commitId.ToAbbreviatedString();
      detailedStatus.Conflict = true;
      GitAsyncOperationStatus? previousStatus = new GitAsyncOperationStatus?();
      this.UpdateAsyncOperationStatus(requestContext1, key, operationId1, GitAsyncOperationStatus.Failed, detailedStatus, previousStatus);
      operationDispatcher.SendFailureNotification(requestContext, (AsyncGitOperationNotification) new AsyncRefOperationConflictNotification(operationId, commitId.ToAbbreviatedString()));
    }

    private void UpdateTimeout(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IAsyncGitOperationDispatcher operationDispatcher,
      int operationId)
    {
      IVssRequestContext requestContext1 = requestContext;
      RepoKey key = repository.Key;
      int operationId1 = operationId;
      GitAsyncRefOperationDetail detailedStatus = new GitAsyncRefOperationDetail();
      detailedStatus.Timedout = true;
      GitAsyncOperationStatus? previousStatus = new GitAsyncOperationStatus?();
      this.UpdateAsyncOperationStatus(requestContext1, key, operationId1, GitAsyncOperationStatus.Abandoned, detailedStatus, previousStatus);
      operationDispatcher.SendTimeoutNotification(requestContext, (AsyncGitOperationNotification) new AsyncRefOperationTimeoutNotification(operationId));
    }

    internal static Guid OrchestrationIdForAsyncRefOperation(Guid repositoryId, int operationId) => DeterministicGuid.Compute(string.Format("AsyncRefOperationJob:{0}:{1}", (object) repositoryId.ToString("N"), (object) operationId));

    protected abstract IReadOnlyList<Sha1Id> GetPullRequestCommitsForOperation(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest);

    protected abstract void QueueJobToRun(
      IVssRequestContext requestContext,
      Guid repositoryId,
      int operationId);

    protected abstract AsyncRefOperationResult RunOperationOnRepository(
      IVssRequestContext context,
      ITfsGitRepository repository,
      GitAsyncRefOperation operationData,
      IReadOnlyList<Sha1Id> commits,
      Action<Sha1Id, double> progressCallback,
      Action<Sha1Id> conflictCallback,
      ClientTraceData ctData,
      out List<Sha1Id> resultCommits,
      out Sha1Id resultBaseCommitId,
      out GitAsyncRefOperationDetail operationDetail);

    protected virtual void DoPostRefUpdateActions(
      IVssRequestContext context,
      ITfsGitRepository repository,
      GitAsyncRefOperation operationData)
    {
    }

    protected abstract void ValidateSourcePullRequest(TfsGitPullRequest pullRequest);

    private void ValidateSource(
      IVssRequestContext requestContext,
      Guid repoId,
      GitAsyncRefOperationSource source)
    {
      if (source.PullRequestId.HasValue)
      {
        using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
          this.ValidateSourcePullRequest(gitCoreComponent.GetPullRequestDetails(repoId, source.PullRequestId.Value) ?? throw new GitAsyncRefOperationInvalidSourceException(Resources.Get("PullRequestNotFound")));
      }
      else if (source.CommitList == null || source.CommitList.Length == 0)
        throw new GitAsyncRefOperationInvalidSourceException(string.Format(Resources.Get("AsyncOperationRequiresSource"), (object) this.OperationType.ToString()));
    }

    private GitAsyncRefOperationFailureStatus ToGitAsyncRefOperationFailureStatus(
      GitRefUpdateStatus? refUpdateStatus)
    {
      if (!refUpdateStatus.HasValue)
        return GitAsyncRefOperationFailureStatus.None;
      if (refUpdateStatus.HasValue)
      {
        switch (refUpdateStatus.GetValueOrDefault())
        {
          case GitRefUpdateStatus.InvalidRefName:
            return GitAsyncRefOperationFailureStatus.InvalidRefName;
          case GitRefUpdateStatus.WritePermissionRequired:
            return GitAsyncRefOperationFailureStatus.WritePermissionRequired;
          case GitRefUpdateStatus.CreateBranchPermissionRequired:
            return GitAsyncRefOperationFailureStatus.CreateBranchPermissionRequired;
          case GitRefUpdateStatus.RefNameConflict:
            return GitAsyncRefOperationFailureStatus.RefNameConflict;
        }
      }
      return GitAsyncRefOperationFailureStatus.Other;
    }

    private int GetMaxRetryCount(IVssRequestContext requestContext, bool generateNewRef) => generateNewRef ? 0 : requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/MaxAsyncRefOperationRetryAttempts", true, 2);
  }
}
