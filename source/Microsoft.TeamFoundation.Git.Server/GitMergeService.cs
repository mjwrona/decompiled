// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitMergeService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Native;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitMergeService : 
    TeamFoundationGitAsyncOperationService<GitMergeAsyncOp, GitMergeParameters, GitMergeOperationStatusDetail>,
    IGitMergeService,
    IVssFrameworkService
  {
    private static readonly GitMergeOperationStatusDetail s_defaultStatusDetail = new GitMergeOperationStatusDetail()
    {
      MergeCommitId = (string) null,
      FailureMessage = (string) null
    };
    private const string c_initialJobIntervalInSecondsRegistryPath = "/Service/Git/Settings/Merges/InitialJobIntervalInSeconds";
    private const int c_defaultInitialJobIntervalInSeconds = 15;
    private const string c_Layer = "GitMergeService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public GitMergeAsyncOp CreateMergeRequest(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitMergeParameters mergeParameters)
    {
      SecurityHelper.Instance.CheckWritePermission(requestContext, (RepoScope) repoKey, (string) null);
      GitMergeAsyncOp asyncOperation = this.CreateAsyncOperation(requestContext, repoKey, GitAsyncOperationType.Merge, mergeParameters);
      this.QueueMergeJob(requestContext, repoKey, asyncOperation.OperationId);
      return this.FillDefaultDetailedStatus(asyncOperation);
    }

    public GitMergeAsyncOp GetMergeRequestById(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int mergeOperationId)
    {
      SecurityHelper.Instance.CheckReadPermission(requestContext, (RepoScope) repoKey, repoKey.RepoId.ToString());
      return this.FillDefaultDetailedStatus(this.GetAsyncOperationById(requestContext, repoKey, mergeOperationId, true));
    }

    public void PerformMerge(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitMergeAsyncOp mergeRequest,
      ClientTraceData ctData)
    {
      GitMergeParameters parameters = mergeRequest.Parameters;
      Sha1Id mergeSourceCommitId = new Sha1Id(parameters.Parents[0]);
      Sha1Id mergeTargetCommitId = new Sha1Id(parameters.Parents[1]);
      Sha1Id[] parentIdsForMergeCommit = new Sha1Id[2]
      {
        mergeSourceCommitId,
        mergeTargetCommitId
      };
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      int num1 = service1.GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/MergeConflictRenameThreshold", true, 100);
      int num2 = service1.GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/MergeConflictTargetLimit", true, 500);
      ITeamFoundationIdentityService service2 = requestContext.GetService<ITeamFoundationIdentityService>();
      LibGit2Sharp.Signature signature = ITfsGitRepositoryExtensions.CreateSignature(requestContext, service2, mergeRequest.CreatorId);
      CommitDetails commitDetails = new CommitDetails(mergeRequest.Parameters.Comment, signature, signature);
      MergeWithConflictsOptions options = new MergeWithConflictsOptions()
      {
        CommitDetails = commitDetails,
        RenameThreshold = num1,
        TargetLimit = num2
      };
      this.UpdateMergeProgress(requestContext, repoKey, mergeRequest.OperationId, GitAsyncOperationStatus.InProgress, new Sha1Id?(), (string) null);
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, mergeRequest.RepositoryId))
      {
        using (LibGit2NativeLibrary git2NativeLibrary = new LibGit2NativeLibrary(requestContext, repositoryById))
        {
          requestContext.TraceAlways(1013864, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitMergeService), "Beginning merge. Parents: {0}@{1}", (object) mergeSourceCommitId, (object) mergeTargetCommitId);
          MergeWithConflictsResult conflictTracking;
          using (requestContext.TimeRegion("LibGit2NativeLibrary", "CreateMergeWithConflictTracking"))
            conflictTracking = git2NativeLibrary.CreateMergeWithConflictTracking(mergeSourceCommitId, mergeTargetCommitId, options, (IEnumerable<Sha1Id>) parentIdsForMergeCommit, ctData);
          requestContext.TraceAlways(1013865, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitMergeService), "Completed merge. Parents: {0}@{1}", (object) mergeSourceCommitId, (object) mergeTargetCommitId);
          switch (conflictTracking.Status)
          {
            case PullRequestAsyncStatus.Conflicts:
              IVssRequestContext requestContext1 = requestContext;
              RepoKey repoKey1 = repoKey;
              int operationId1 = mergeRequest.OperationId;
              string str1 = Resources.Get("GitConflict");
              Sha1Id? mergeCommitId1 = new Sha1Id?();
              string errorMessage1 = str1;
              this.UpdateMergeProgress(requestContext1, repoKey1, operationId1, GitAsyncOperationStatus.Failed, mergeCommitId1, errorMessage1);
              break;
            case PullRequestAsyncStatus.Succeeded:
              this.UpdateRefs(requestContext, mergeRequest, conflictTracking);
              this.UpdateMergeProgress(requestContext, repoKey, mergeRequest.OperationId, GitAsyncOperationStatus.Completed, new Sha1Id?(conflictTracking.MergeCommitId), (string) null);
              break;
            case PullRequestAsyncStatus.RejectedByPolicy:
              IVssRequestContext requestContext2 = requestContext;
              RepoKey repoKey2 = repoKey;
              int operationId2 = mergeRequest.OperationId;
              string str2 = Resources.Get("GitPolicyRejection");
              Sha1Id? mergeCommitId2 = new Sha1Id?();
              string errorMessage2 = str2;
              this.UpdateMergeProgress(requestContext2, repoKey2, operationId2, GitAsyncOperationStatus.Failed, mergeCommitId2, errorMessage2);
              break;
            default:
              IVssRequestContext requestContext3 = requestContext;
              RepoKey repoKey3 = repoKey;
              int operationId3 = mergeRequest.OperationId;
              string str3 = Resources.Get("GitOperationFailure");
              Sha1Id? mergeCommitId3 = new Sha1Id?();
              string errorMessage3 = str3;
              this.UpdateMergeProgress(requestContext3, repoKey3, operationId3, GitAsyncOperationStatus.Failed, mergeCommitId3, errorMessage3);
              break;
          }
        }
      }
    }

    private void QueueMergeJob(IVssRequestContext requestContext, RepoKey repoKey, int operationId)
    {
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/Merges/InitialJobIntervalInSeconds", 15);
      TeamFoundationJobSchedule foundationJobSchedule = new TeamFoundationJobSchedule()
      {
        Interval = num,
        ScheduledTime = DateTime.UtcNow
      };
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new AsyncGitOperationJobData()
      {
        RepositoryId = repoKey.RepoId,
        OperationId = operationId
      });
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(Guid.NewGuid(), "GitMergeJob", "Microsoft.TeamFoundation.Git.Server.Plugins.Jobs.GitMergeJob", xml, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.AboveNormal);
      foundationJobDefinition.Schedule.Add(foundationJobSchedule);
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      service.UpdateJobDefinitions(requestContext, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
      service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        foundationJobDefinition.JobId
      });
    }

    public GitMergeAsyncOp UpdateMergeProgress(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int mergeOperationId,
      GitAsyncOperationStatus status,
      Sha1Id? mergeCommitId = null,
      string errorMessage = null)
    {
      GitMergeOperationStatusDetail detailedStatus = new GitMergeOperationStatusDetail()
      {
        MergeCommitId = mergeCommitId?.ToString(),
        FailureMessage = errorMessage
      };
      this.UpdateAsyncOperationStatus(requestContext, repoKey, mergeOperationId, status, detailedStatus, new GitAsyncOperationStatus?());
      return this.GetAsyncOperationById(requestContext, repoKey, mergeOperationId, true);
    }

    private void UpdateRefs(
      IVssRequestContext requestContext,
      GitMergeAsyncOp mergeRequest,
      MergeWithConflictsResult mergeResult)
    {
      ITeamFoundationGitRefService service = requestContext.GetService<ITeamFoundationGitRefService>();
      string name = "refs/azure-repos/merges/" + mergeRequest.OperationId.ToString();
      if (service.UpdateRefs(requestContext.Elevate(), mergeRequest.RepositoryId, new List<TfsGitRefUpdateRequest>()
      {
        new TfsGitRefUpdateRequest(name, Sha1Id.Empty, mergeResult.MergeCommitId)
      }).CountFailed > 0)
      {
        service.UpdateRefs(requestContext.Elevate(), mergeRequest.RepositoryId, new List<TfsGitRefUpdateRequest>()
        {
          new TfsGitRefUpdateRequest(name, Sha1Id.Empty, Sha1Id.Empty)
        });
        service.UpdateRefs(requestContext.Elevate(), mergeRequest.RepositoryId, new List<TfsGitRefUpdateRequest>()
        {
          new TfsGitRefUpdateRequest(name, Sha1Id.Empty, mergeResult.MergeCommitId)
        });
      }
      service.UpdateRefs(requestContext.Elevate(), mergeRequest.RepositoryId, new List<TfsGitRefUpdateRequest>()
      {
        new TfsGitRefUpdateRequest(name, mergeResult.MergeCommitId, Sha1Id.Empty)
      });
    }

    private GitMergeAsyncOp FillDefaultDetailedStatus(GitMergeAsyncOp mergeAsyncOp)
    {
      if (mergeAsyncOp.DetailedStatus == null)
        mergeAsyncOp.DetailedStatus = GitMergeService.s_defaultStatusDetail;
      return mergeAsyncOp;
    }
  }
}
