// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Git.GitData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Git
{
  public class GitData : IGitData
  {
    public GitCommit GetCommit(
      IVssRequestContext context,
      Guid projectId,
      Guid repositoryId,
      string commitId)
    {
      GitHttpClient gitHttpClient = context.GetClient<GitHttpClient>();
      Func<Task<GitCommit>> func = (Func<Task<GitCommit>>) (() => gitHttpClient.GetCommitAsync(projectId, commitId, repositoryId));
      return context.ExecuteAsyncAndSyncResult<GitCommit>(func);
    }

    public IList<GitCommitRef> GetCommits(
      IVssRequestContext context,
      GitQueryCommitsCriteria criteria,
      Guid repositoryId,
      string branch,
      int totalCommits)
    {
      GitHttpClient gitHttpClient = context.GetClient<GitHttpClient>();
      Func<Task<List<GitCommitRef>>> func = (Func<Task<List<GitCommitRef>>>) (() =>
      {
        GitHttpClient gitHttpClient1 = gitHttpClient;
        GitQueryCommitsCriteria searchCriteria = criteria;
        Guid repositoryId1 = repositoryId;
        int? nullable = new int?(totalCommits);
        int? skip = new int?();
        int? top = nullable;
        bool? includeStatuses = new bool?();
        CancellationToken cancellationToken = new CancellationToken();
        return gitHttpClient1.GetCommitsBatchAsync(searchCriteria, repositoryId1, skip, top, includeStatuses, cancellationToken: cancellationToken);
      });
      return (IList<GitCommitRef>) context.ExecuteAsyncAndSyncResult<List<GitCommitRef>>(func);
    }

    public IList<GitBranchStats> GetBranches(IVssRequestContext context, Guid repositoryId)
    {
      GitHttpClient gitHttpClient = context.GetClient<GitHttpClient>();
      Func<Task<List<GitBranchStats>>> func = (Func<Task<List<GitBranchStats>>>) (() => gitHttpClient.GetBranchesAsync(repositoryId));
      return (IList<GitBranchStats>) context.ExecuteAsyncAndSyncResult<List<GitBranchStats>>(func);
    }

    public IList<GitRepository> GetRepositories(IVssRequestContext context, Guid projectId)
    {
      GitHttpClient gitHttpClient = context.GetClient<GitHttpClient>();
      Func<Task<List<GitRepository>>> func = (Func<Task<List<GitRepository>>>) (() => gitHttpClient.GetRepositoriesAsync(projectId));
      return (IList<GitRepository>) context.ExecuteAsyncAndSyncResult<List<GitRepository>>(func);
    }

    public GitRepository GetRepository(
      IVssRequestContext context,
      Guid projectId,
      string repositoryId)
    {
      GitHttpClient gitHttpClient = context.GetClient<GitHttpClient>();
      Func<Task<GitRepository>> func = (Func<Task<GitRepository>>) (() => gitHttpClient.GetRepositoryAsync(projectId, repositoryId, (object) null, new CancellationToken()));
      return context.ExecuteAsyncAndSyncResult<GitRepository>(func);
    }

    public IList<List<GitItem>> GetItemsBatch(
      IVssRequestContext context,
      Guid repositoryId,
      string branch,
      string path)
    {
      GitItemDescriptor gitItemDescriptor = new GitItemDescriptor()
      {
        Path = path,
        RecursionLevel = VersionControlRecursionType.OneLevelPlusNestedEmptyFolders,
        Version = branch,
        VersionType = GitVersionType.Branch
      };
      GitItemRequestData gitItemRequestData = new GitItemRequestData()
      {
        ItemDescriptors = new GitItemDescriptor[1]
        {
          gitItemDescriptor
        }
      };
      GitHttpClient gitHttpClient = context.GetClient<GitHttpClient>();
      Func<Task<List<List<GitItem>>>> func = (Func<Task<List<List<GitItem>>>>) (() => gitHttpClient.GetItemsBatchAsync(gitItemRequestData, repositoryId));
      return (IList<List<GitItem>>) context.ExecuteAsyncAndSyncResult<List<List<GitItem>>>(func);
    }

    public string GetItemContent(
      IVssRequestContext context,
      Guid repositoryId,
      string branch,
      string path)
    {
      string itemContent = (string) null;
      if (!string.IsNullOrEmpty(path))
      {
        GitVersionDescriptor gitVersionDescriptor = new GitVersionDescriptor()
        {
          VersionType = GitVersionType.Branch,
          Version = branch
        };
        GitHttpClient gitHttpClient = context.GetClient<GitHttpClient>();
        Func<Task<Stream>> func = (Func<Task<Stream>>) (() =>
        {
          GitHttpClient gitHttpClient1 = gitHttpClient;
          Guid repositoryId1 = repositoryId;
          string path1 = path;
          GitVersionDescriptor versionDescriptor1 = gitVersionDescriptor;
          VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?();
          bool? includeContentMetadata = new bool?();
          bool? latestProcessedChange = new bool?();
          bool? download = new bool?();
          GitVersionDescriptor versionDescriptor2 = versionDescriptor1;
          bool? includeContent = new bool?();
          bool? resolveLfs = new bool?();
          bool? sanitize = new bool?();
          CancellationToken cancellationToken = new CancellationToken();
          return gitHttpClient1.GetItemTextAsync(repositoryId1, path1, recursionLevel: recursionLevel, includeContentMetadata: includeContentMetadata, latestProcessedChange: latestProcessedChange, download: download, versionDescriptor: versionDescriptor2, includeContent: includeContent, resolveLfs: resolveLfs, sanitize: sanitize, cancellationToken: cancellationToken);
        });
        using (StreamReader streamReader = new StreamReader(context.ExecuteAsyncAndSyncResult<Stream>(func)))
          itemContent = streamReader.ReadToEnd();
      }
      return itemContent;
    }

    public IList<GitItem> GetItemsFirstLevel(
      IVssRequestContext context,
      Guid repositoryId,
      string branch,
      string commit)
    {
      GitVersionDescriptor gitVersionDescriptor;
      if (string.IsNullOrWhiteSpace(commit))
        gitVersionDescriptor = new GitVersionDescriptor()
        {
          VersionType = GitVersionType.Branch,
          Version = branch
        };
      else
        gitVersionDescriptor = new GitVersionDescriptor()
        {
          VersionType = GitVersionType.Commit,
          Version = commit
        };
      GitHttpClient gitHttpClient = context.GetClient<GitHttpClient>();
      Func<Task<List<GitItem>>> func = (Func<Task<List<GitItem>>>) (() =>
      {
        GitHttpClient gitHttpClient1 = gitHttpClient;
        Guid repositoryId1 = repositoryId;
        VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?(VersionControlRecursionType.OneLevelPlusNestedEmptyFolders);
        bool? nullable1 = new bool?(false);
        bool? nullable2 = new bool?(false);
        GitVersionDescriptor versionDescriptor1 = gitVersionDescriptor;
        bool? includeContentMetadata = new bool?();
        bool? latestProcessedChange = nullable1;
        bool? download = nullable2;
        bool? includeLinks = new bool?();
        GitVersionDescriptor versionDescriptor2 = versionDescriptor1;
        bool? zipForUnix = new bool?();
        CancellationToken cancellationToken = new CancellationToken();
        return gitHttpClient1.GetItemsAsync(repositoryId1, "/", recursionLevel, includeContentMetadata, latestProcessedChange, download, includeLinks, versionDescriptor2, zipForUnix, (object) null, cancellationToken);
      });
      return (IList<GitItem>) context.ExecuteAsyncAndSyncResult<List<GitItem>>(func);
    }
  }
}
