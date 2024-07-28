// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitStatusStateMapper
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitStatusStateMapper
  {
    private static bool IsFeatureEnabled(IVssRequestContext context) => context.IsFeatureEnabled("SourceControl.EnableReturningPartiallySucceededGitStatusState");

    private static void UpdateGitStatus(GitStatus status)
    {
      if (status == null)
        return;
      status.State = GitStatusStateMapper.UpdateStatusState(status.State);
    }

    private static void UpdateGitStatuses(IEnumerable<GitStatus> statuses)
    {
      if (statuses == null)
        return;
      foreach (GitStatus statuse in statuses)
        GitStatusStateMapper.UpdateGitStatus(statuse);
    }

    private static GitStatusState UpdateStatusState(GitStatusState state) => state != GitStatusState.PartiallySucceeded ? state : GitStatusState.Failed;

    public static void MapGitEntity<T>(T entity, IVssRequestContext context) where T : class
    {
      if ((object) entity == null || !GitStatusStateMapper.IsFeatureEnabled(context))
        return;
      switch (entity)
      {
        case IEnumerable<GitPullRequestStatus> statuses3:
          GitStatusStateMapper.UpdateGitStatuses((IEnumerable<GitStatus>) statuses3);
          break;
        case GitPullRequestStatus status:
          GitStatusStateMapper.UpdateGitStatus((GitStatus) status);
          break;
        case GitBranchStats gitBranchStats:
          GitStatusStateMapper.UpdateGitStatuses((IEnumerable<GitStatus>) gitBranchStats.Commit?.Statuses);
          break;
        case IEnumerable<GitCommitRef> source1:
          GitStatusStateMapper.UpdateGitStatuses(source1.SelectMany<GitCommitRef, GitStatus>((Func<GitCommitRef, IEnumerable<GitStatus>>) (x => (IEnumerable<GitStatus>) x?.Statuses ?? Enumerable.Empty<GitStatus>())));
          break;
        case IEnumerable<GitRef> source2:
          GitStatusStateMapper.UpdateGitStatuses(source2.SelectMany<GitRef, GitStatus>((Func<GitRef, IEnumerable<GitStatus>>) (x => x?.Statuses ?? Enumerable.Empty<GitStatus>())));
          break;
        case GitRef gitRef:
          GitStatusStateMapper.UpdateGitStatuses(gitRef.Statuses);
          break;
        case GitCommit gitCommit:
          GitStatusStateMapper.UpdateGitStatuses((IEnumerable<GitStatus>) gitCommit.Statuses);
          break;
        case GitPush gitPush:
          IEnumerable<GitCommitRef> commits1 = gitPush.Commits;
          GitStatusStateMapper.UpdateGitStatuses(commits1 != null ? commits1.SelectMany<GitCommitRef, GitStatus>((Func<GitCommitRef, IEnumerable<GitStatus>>) (x => (IEnumerable<GitStatus>) x?.Statuses ?? Enumerable.Empty<GitStatus>())) : (IEnumerable<GitStatus>) null);
          break;
        case IEnumerable<GitStatus> statuses4:
          GitStatusStateMapper.UpdateGitStatuses(statuses4);
          break;
        case IEnumerable<GitPush> source3:
          GitStatusStateMapper.UpdateGitStatuses(source3.SelectMany<GitPush, GitCommitRef>((Func<GitPush, IEnumerable<GitCommitRef>>) (x => x?.Commits ?? (IEnumerable<GitCommitRef>) Enumerable.Empty<GitCommit>())).SelectMany<GitCommitRef, GitStatus>((Func<GitCommitRef, IEnumerable<GitStatus>>) (c => (IEnumerable<GitStatus>) c?.Statuses ?? Enumerable.Empty<GitStatus>())));
          break;
        case GitCherryPick gitCherryPick:
          GitAsyncRefOperationParameters parameters1 = gitCherryPick.Parameters;
          IEnumerable<GitStatus> statuses1;
          if (parameters1 == null)
          {
            statuses1 = (IEnumerable<GitStatus>) null;
          }
          else
          {
            GitAsyncRefOperationSource source = parameters1.Source;
            if (source == null)
            {
              statuses1 = (IEnumerable<GitStatus>) null;
            }
            else
            {
              GitCommitRef[] commitList = source.CommitList;
              statuses1 = commitList != null ? ((IEnumerable<GitCommitRef>) commitList).SelectMany<GitCommitRef, GitStatus>((Func<GitCommitRef, IEnumerable<GitStatus>>) (c => (IEnumerable<GitStatus>) c?.Statuses ?? Enumerable.Empty<GitStatus>())) : (IEnumerable<GitStatus>) null;
            }
          }
          GitStatusStateMapper.UpdateGitStatuses(statuses1);
          break;
        case GitRevert gitRevert:
          GitAsyncRefOperationParameters parameters2 = gitRevert.Parameters;
          IEnumerable<GitStatus> statuses2;
          if (parameters2 == null)
          {
            statuses2 = (IEnumerable<GitStatus>) null;
          }
          else
          {
            GitAsyncRefOperationSource source = parameters2.Source;
            if (source == null)
            {
              statuses2 = (IEnumerable<GitStatus>) null;
            }
            else
            {
              GitCommitRef[] commitList = source.CommitList;
              statuses2 = commitList != null ? ((IEnumerable<GitCommitRef>) commitList).SelectMany<GitCommitRef, GitStatus>((Func<GitCommitRef, IEnumerable<GitStatus>>) (c => (IEnumerable<GitStatus>) c?.Statuses ?? Enumerable.Empty<GitStatus>())) : (IEnumerable<GitStatus>) null;
            }
          }
          GitStatusStateMapper.UpdateGitStatuses(statuses2);
          break;
        case IEnumerable<GitBranchStats> source4:
          GitStatusStateMapper.UpdateGitStatuses(source4.SelectMany<GitBranchStats, GitStatus>((Func<GitBranchStats, IEnumerable<GitStatus>>) (x => (IEnumerable<GitStatus>) x.Commit?.Statuses ?? Enumerable.Empty<GitStatus>())));
          break;
        case IEnumerable<GitPullRequest> source5:
          GitStatusStateMapper.UpdateGitStatuses(source5.SelectMany<GitPullRequest, GitCommitRef>((Func<GitPullRequest, IEnumerable<GitCommitRef>>) (x => (IEnumerable<GitCommitRef>) x?.Commits ?? Enumerable.Empty<GitCommitRef>())).SelectMany<GitCommitRef, GitStatus>((Func<GitCommitRef, IEnumerable<GitStatus>>) (c => (IEnumerable<GitStatus>) c?.Statuses ?? Enumerable.Empty<GitStatus>())));
          GitStatusStateMapper.UpdateGitStatuses(source5.SelectMany<GitPullRequest, GitStatus>((Func<GitPullRequest, IEnumerable<GitStatus>>) (x => (IEnumerable<GitStatus>) x?.LastMergeCommit?.Statuses ?? Enumerable.Empty<GitStatus>())));
          GitStatusStateMapper.UpdateGitStatuses(source5.SelectMany<GitPullRequest, GitStatus>((Func<GitPullRequest, IEnumerable<GitStatus>>) (x => (IEnumerable<GitStatus>) x?.LastMergeSourceCommit?.Statuses ?? Enumerable.Empty<GitStatus>())));
          GitStatusStateMapper.UpdateGitStatuses(source5.SelectMany<GitPullRequest, GitStatus>((Func<GitPullRequest, IEnumerable<GitStatus>>) (x => (IEnumerable<GitStatus>) x?.LastMergeTargetCommit?.Statuses ?? Enumerable.Empty<GitStatus>())));
          break;
        case GitPullRequest gitPullRequest:
          GitCommitRef[] commits2 = gitPullRequest.Commits;
          GitStatusStateMapper.UpdateGitStatuses(commits2 != null ? ((IEnumerable<GitCommitRef>) commits2).SelectMany<GitCommitRef, GitStatus>((Func<GitCommitRef, IEnumerable<GitStatus>>) (c => (IEnumerable<GitStatus>) c?.Statuses ?? Enumerable.Empty<GitStatus>())) : (IEnumerable<GitStatus>) null);
          GitStatusStateMapper.UpdateGitStatuses((IEnumerable<GitStatus>) gitPullRequest.LastMergeCommit?.Statuses);
          GitStatusStateMapper.UpdateGitStatuses((IEnumerable<GitStatus>) gitPullRequest.LastMergeSourceCommit?.Statuses);
          GitStatusStateMapper.UpdateGitStatuses((IEnumerable<GitStatus>) gitPullRequest.LastMergeTargetCommit?.Statuses);
          break;
        default:
          throw new NotSupportedException(string.Format("The type {0} is not supported in the MapGitEntity method.", (object) entity.GetType()));
      }
    }
  }
}
