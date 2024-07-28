// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AzureReposProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class AzureReposProvider : IVersionControlProvider
  {
    private const string PullRequestNotFoundErrorCode = "TF401180";

    public int GetFileCountInRepo(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext)
    {
      GitHttpClient gitClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
      try
      {
        return TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy, GitFilePathsCollection>((Func<GitFilePathsCollection>) (() => Task.Run<GitFilePathsCollection>((Func<Task<GitFilePathsCollection>>) (async () => await gitClient.GetFilePathsAsync(pipelineContext.ProjectId, pipelineContext.RepositoryId))).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy).Paths.Count;
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Error(1015656, string.Format("GetFileCountInRepo: Failed to find file count in repo: {0}", (object) ex));
        throw;
      }
    }

    public bool IsFilePresentInRepo(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      string filePath)
    {
      GitHttpClient gitClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
      try
      {
        GitFilePathsCollection filePathsCollection = TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy, GitFilePathsCollection>((Func<GitFilePathsCollection>) (() => Task.Run<GitFilePathsCollection>((Func<Task<GitFilePathsCollection>>) (async () => await gitClient.GetFilePathsAsync(pipelineContext.ProjectId, pipelineContext.RepositoryId, filePath))).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
        if (filePathsCollection.Paths != null)
        {
          if (filePathsCollection.Paths.Count > 0)
            return true;
        }
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("TF401174"))
          tcmRequestContext.Logger.Info(1015654, "IsFilePresentInRepo: Failed to find the file in repo: " + filePath);
        else
          tcmRequestContext.Logger.Error(1015655, string.Format("IsFilePresentInRepo: Failed to find if file is present in repo: {0}", (object) ex));
      }
      return false;
    }

    public PullRequestChanges GetChangesInCurrentIteration(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      bool includeFilesOnly = false,
      bool includeDeletes = true,
      IEnumerable<string> extensionFilter = null)
    {
      using (new SimpleTimer(tcmRequestContext.RequestContext, "AzureReposProvider: GetChangesInCurrentIteration"))
      {
        GitHttpClient gitClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        IEnumerable<GitChange> gitChanges = TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy, GitCommitChanges>((Func<GitCommitChanges>) (() => Task.Run<GitCommitChanges>((Func<Task<GitCommitChanges>>) (async () => await gitClient.GetChangesAsync(pipelineContext.ProjectId, pipelineContext.SourceVersion, pipelineContext.RepositoryId))).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy).Changes.Where<GitChange>((Func<GitChange, bool>) (change => this.ApplyFilters(change, includeFilesOnly, includeDeletes, extensionFilter)));
        List<FileParams> fileParamsList = new List<FileParams>();
        VersionControlChangeType controlChangeType = VersionControlChangeType.Delete | VersionControlChangeType.SourceRename;
        foreach (GitChange gitChange in gitChanges)
        {
          if (gitChange.ChangeType != controlChangeType)
          {
            string path = gitChange.Item.Path;
            string str = (string) null;
            if (!string.IsNullOrEmpty(gitChange.SourceServerItem))
              str = gitChange.SourceServerItem;
            else if (gitChange.ChangeType != VersionControlChangeType.Add)
              str = gitChange.Item.Path;
            fileParamsList.Add(new FileParams()
            {
              Path = path,
              OriginalPath = str
            });
          }
        }
        string parentCommitId = this.GetParentCommitId(tcmRequestContext, gitClient, pipelineContext.SourceVersion, pipelineContext.RepositoryId);
        if (parentCommitId == null)
        {
          tcmRequestContext.Logger.Error(1015692, "GetChangesInCurrentIteration: Failed to find parent commit id of build source version: " + pipelineContext.SourceVersion);
          return (PullRequestChanges) null;
        }
        return new PullRequestChanges()
        {
          BaseVersionCommit = parentCommitId,
          FileParams = fileParamsList
        };
      }
    }

    public List<GitPullRequestIteration> GetPullRequestIterations(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      CancellationToken cancellationToken)
    {
      using (new SimpleTimer(tcmRequestContext.RequestContext, "AzureReposProvider: GetPullRequestIterations"))
      {
        GitHttpClient gitHttpClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        return TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy, List<GitPullRequestIteration>>((Func<List<GitPullRequestIteration>>) (() => Task.Run<List<GitPullRequestIteration>>((Func<Task<List<GitPullRequestIteration>>>) (async () =>
        {
          GitHttpClient gitHttpClient1 = gitHttpClient;
          Guid projectId = pipelineContext.ProjectId;
          Guid repositoryId = pipelineContext.RepositoryId;
          int pullRequestId = pipelineContext.PullRequestId;
          CancellationToken cancellationToken1 = cancellationToken;
          bool? includeCommits = new bool?();
          CancellationToken cancellationToken2 = cancellationToken1;
          return await gitHttpClient1.GetPullRequestIterationsAsync(projectId, repositoryId, pullRequestId, includeCommits, cancellationToken: cancellationToken2);
        })).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
      }
    }

    public GitPullRequestIteration GetPullRequestIteration(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext)
    {
      try
      {
        GitHttpClient gitClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        List<GitPullRequestIteration> requestIterationList = TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy, List<GitPullRequestIteration>>((Func<List<GitPullRequestIteration>>) (() => Task.Run<List<GitPullRequestIteration>>((Func<Task<List<GitPullRequestIteration>>>) (async () => await gitClient.GetPullRequestIterationsAsync(pipelineContext.RepositoryId, pipelineContext.PullRequestId))).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
        if (requestIterationList == null)
          return (GitPullRequestIteration) null;
        foreach (GitPullRequestIteration requestIteration in requestIterationList)
        {
          if (string.Equals(requestIteration.SourceRefCommit.CommitId, pipelineContext.SourceCommitId, StringComparison.OrdinalIgnoreCase))
            return requestIteration;
        }
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException != null && ex.InnerException.Message.Contains("TF401180"))
          tcmRequestContext.Logger.Error(1015652, string.Format("GetPullRequestIteration: PullRequest:{0} not found. PipelineId:{1}", (object) pipelineContext.PullRequestId, (object) pipelineContext.Id));
        else
          throw;
      }
      catch (VssServiceException ex) when (ex.Message.Contains("TF401180"))
      {
        if (ex.Message.Contains("TF401180"))
          tcmRequestContext.Logger.Error(1015653, string.Format("GetPullRequestIteration: PullRequest:{0} not found. PipelineId:{1}", (object) pipelineContext.PullRequestId, (object) pipelineContext.Id));
        else
          throw;
      }
      return (GitPullRequestIteration) null;
    }

    public GitPullRequestIteration GetPullRequestIterationByIterationId(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext)
    {
      try
      {
        GitHttpClient gitClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        return TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy, GitPullRequestIteration>((Func<GitPullRequestIteration>) (() => tcmRequestContext.RequestContext.RunSynchronously<GitPullRequestIteration>((Func<Task<GitPullRequestIteration>>) (() => gitClient.GetPullRequestIterationAsync(pipelineContext.RepositoryId, pipelineContext.PullRequestId, pipelineContext.PullRequestIterationId)))), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException != null && ex.InnerException.Message.Contains("TF401180"))
          tcmRequestContext.Logger.Error(1015652, string.Format("GetPullRequestIteration: PullRequest:{0} not found. PipelineId:{1}", (object) pipelineContext.PullRequestId, (object) pipelineContext.Id));
        else
          throw;
      }
      catch (VssServiceException ex) when (ex.Message.Contains("TF401180"))
      {
        if (ex.Message.Contains("TF401180"))
          tcmRequestContext.Logger.Error(1015653, string.Format("GetPullRequestIteration: PullRequest:{0} not found. PipelineId:{1}", (object) pipelineContext.PullRequestId, (object) pipelineContext.Id));
        else
          throw;
      }
      return (GitPullRequestIteration) null;
    }

    public GitPullRequest GetPullRequestById(
      TestManagementRequestContext tcmRequestContext,
      int pullRequestId,
      CancellationToken cancellationToken)
    {
      using (new SimpleTimer(tcmRequestContext.RequestContext, "AzureReposProvider: GetPullRequestById"))
      {
        GitHttpClient gitHttpClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        return TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy, GitPullRequest>((Func<GitPullRequest>) (() => Task.Run<GitPullRequest>((Func<Task<GitPullRequest>>) (async () => await gitHttpClient.GetPullRequestByIdAsync(pullRequestId, cancellationToken: cancellationToken))).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
      }
    }

    public GitPullRequestCommentThread CreateThread(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      GitPullRequestCommentThread commentThread,
      CancellationToken cancellationToken)
    {
      using (new SimpleTimer(tcmRequestContext.RequestContext, "AzureReposProvider: CreateThread"))
      {
        GitHttpClient gitHttpClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        return TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy, GitPullRequestCommentThread>((Func<GitPullRequestCommentThread>) (() => Task.Run<GitPullRequestCommentThread>((Func<Task<GitPullRequestCommentThread>>) (async () => await gitHttpClient.CreateThreadAsync(commentThread, pipelineContext.ProjectId, pipelineContext.RepositoryId, pipelineContext.PullRequestId, cancellationToken: cancellationToken))).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
      }
    }

    public List<GitPullRequestCommentThread> GetPullRequestCommentThreads(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      int? baseIteration,
      CancellationToken cancellationToken)
    {
      using (new SimpleTimer(tcmRequestContext.RequestContext, "AzureReposProvider: Get Pull Request Comment threads"))
      {
        GitHttpClient gitHttpClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        return TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy, List<GitPullRequestCommentThread>>((Func<List<GitPullRequestCommentThread>>) (() => Task.Run<List<GitPullRequestCommentThread>>((Func<Task<List<GitPullRequestCommentThread>>>) (async () => await gitHttpClient.GetThreadsAsync(pipelineContext.ProjectId, pipelineContext.RepositoryId, pipelineContext.PullRequestId, new int?(pipelineContext.PullRequestIterationId), baseIteration, cancellationToken: cancellationToken))).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
      }
    }

    public void UpdatePullRequestThread(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      GitPullRequestCommentThread commentThread,
      int threadId,
      CancellationToken cancellationToken)
    {
      using (new SimpleTimer(tcmRequestContext.RequestContext, "AzureReposProvider: Update Pull Request Comment threads"))
      {
        GitHttpClient gitHttpClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy>((Action) (() => Task.Run<GitPullRequestCommentThread>((Func<Task<GitPullRequestCommentThread>>) (async () => await gitHttpClient.UpdateThreadAsync(commentThread, pipelineContext.ProjectId, pipelineContext.RepositoryId, pipelineContext.PullRequestId, threadId, cancellationToken: cancellationToken))).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
      }
    }

    public void UpdatePullRequestThreadComment(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      GitPullRequestCommentThread commentThread,
      string commentText,
      int threadId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (new SimpleTimer(tcmRequestContext.RequestContext, "AzureReposProvider:UpdatePullRequestThreadComment"))
      {
        GitHttpClient gitHttpClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        Comment comment = new Comment()
        {
          Id = commentThread.Comments[0].Id,
          Content = commentText
        };
        TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy>((Action) (() => Task.Run<Comment>((Func<Task<Comment>>) (async () => await gitHttpClient.UpdateCommentAsync(comment, pipelineContext.RepositoryId, pipelineContext.PullRequestId, threadId, (int) comment.Id, cancellationToken: cancellationToken))).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
      }
    }

    public List<FileDiffInfo> GetFileDiffs(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      PullRequestChanges pullRequestChanges)
    {
      List<FileDiff> fileDiffList = new List<FileDiff>();
      int diffsCallBatchSize = new CoverageConfiguration().GetFileDiffsCallBatchSize(tcmRequestContext.RequestContext);
      List<List<FileParams>> fileParamsListList = pullRequestChanges.FileParams.ChunkBy<FileParams>(diffsCallBatchSize);
      GitHttpClient client = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
      foreach (List<FileParams> fileParamBatch in fileParamsListList)
      {
        List<FileDiff> fileDiffs = this.GetFileDiffs(client, pipelineContext, fileParamBatch, pullRequestChanges.BaseVersionCommit, pipelineContext.SourceVersion);
        fileDiffList.AddRange((IEnumerable<FileDiff>) fileDiffs);
      }
      List<FileDiffInfo> fileDiffs1 = new List<FileDiffInfo>();
      foreach (FileDiff fileDiff in fileDiffList)
      {
        FileDiffInfo fileDiffInfo = new FileDiffInfo(fileDiff.Path);
        fileDiffInfo.DiffBlocks = new List<KeyValuePair<LineRange, LineRangeStatus>>();
        foreach (LineDiffBlock lineDiffBlock in fileDiff.LineDiffBlocks)
        {
          LineRangeStatus? nullable = new LineRangeStatus?();
          switch (lineDiffBlock.ChangeType)
          {
            case LineDiffBlockChangeType.None:
              nullable = new LineRangeStatus?(LineRangeStatus.None);
              break;
            case LineDiffBlockChangeType.Add:
              nullable = new LineRangeStatus?(LineRangeStatus.Added);
              break;
            case LineDiffBlockChangeType.Edit:
              nullable = new LineRangeStatus?(LineRangeStatus.Modified);
              break;
          }
          if (nullable.HasValue)
          {
            KeyValuePair<LineRange, LineRangeStatus> keyValuePair = new KeyValuePair<LineRange, LineRangeStatus>(new LineRange(lineDiffBlock.ModifiedLineNumberStart, lineDiffBlock.ModifiedLinesCount), nullable.Value);
            fileDiffInfo.DiffBlocks.Add(keyValuePair);
          }
        }
        fileDiffs1.Add(fileDiffInfo);
      }
      return fileDiffs1;
    }

    public Dictionary<string, FileDiffMapping> GetFileDiffMappingsInCurrentIteration(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      PullRequestChanges pullRequestChanges)
    {
      using (new SimpleTimer(tcmRequestContext.RequestContext, "AzureReposProvider: ComputeLinesChangedForFilesCost"))
      {
        GitHttpClient client = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        string parentCommitId = this.GetParentCommitId(tcmRequestContext, client, pipelineContext.SourceVersion, pipelineContext.RepositoryId);
        if (parentCommitId == null)
        {
          tcmRequestContext.Logger.Error(1015693, "GetFileDiffMappingsInCurrentIteration: Failed to find parent commit id of build source version: " + pipelineContext.SourceVersion);
          return (Dictionary<string, FileDiffMapping>) null;
        }
        int diffsCallBatchSize = new CoverageConfiguration().GetFileDiffsCallBatchSize(tcmRequestContext.RequestContext);
        List<List<FileParams>> fileParamsListList = pullRequestChanges.FileParams.ChunkBy<FileParams>(diffsCallBatchSize);
        Dictionary<string, FileDiffMapping> currentIteration = new Dictionary<string, FileDiffMapping>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (List<FileParams> fileParamBatch in fileParamsListList)
        {
          try
          {
            List<FileDiff> fileDiffs1 = this.GetFileDiffs(client, pipelineContext, fileParamBatch, pipelineContext.CommonRefCommitId, pipelineContext.SourceRefCommitId);
            List<FileDiff> fileDiffs2 = this.GetFileDiffs(client, pipelineContext, fileParamBatch, parentCommitId, pipelineContext.SourceVersion);
            foreach (FileDiffMapping fileDiffMapping in this.MapChangesInPRViewToChangesInMergeView(tcmRequestContext, fileDiffs1, fileDiffs2))
            {
              if (currentIteration.ContainsKey(fileDiffMapping.Path))
                tcmRequestContext.Logger.Warning(1015775, "Duplicate fileDiffMapping found: " + fileDiffMapping.Path);
              else
                currentIteration.Add(fileDiffMapping.Path, fileDiffMapping);
            }
          }
          catch (VssServiceException ex)
          {
            if (ex.Message.Contains("VS403420"))
              throw new FilePathNotFoundException(ex.Message, (Exception) ex);
          }
        }
        return currentIteration;
      }
    }

    public string GetPullRequestIterationUrl(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext)
    {
      string requestIterationUrl = string.Empty;
      if (string.IsNullOrEmpty(pipelineContext.SourceRepositoryUri))
        return requestIterationUrl;
      try
      {
        if (pipelineContext.SourceRepositoryUri.Contains("_git/_full"))
          pipelineContext.SourceRepositoryUri = pipelineContext.SourceRepositoryUri.Replace("_git/_full", "_git");
        else if (pipelineContext.SourceRepositoryUri.Contains("_git/_optimized"))
          pipelineContext.SourceRepositoryUri = pipelineContext.SourceRepositoryUri.Replace("_git/_optimized", "_git");
        Uri uri = new Uri(pipelineContext.SourceRepositoryUri);
        UriBuilder uriBuilder = new UriBuilder();
        uriBuilder.Scheme = uri.Scheme;
        uriBuilder.Host = uri.Host;
        uriBuilder.Path = uri.PathAndQuery;
        string str = string.Format("&iteration={0}", (object) pipelineContext.PullRequestIterationId);
        requestIterationUrl = string.Format("{0}/pullrequest/{1}?_a=files{2}", (object) uriBuilder.Uri.AbsoluteUri, (object) pipelineContext.PullRequestId, (object) str);
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Warning(1015788, string.Format("GetPullRequestIterationUrl: Ignoring: {0}", (object) ex));
      }
      return requestIterationUrl;
    }

    public void CreatePullRequestStatus(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      CoverageStatusCheckResult coverageStatusCheckResult,
      CancellationToken cancellationToken)
    {
      using (new SimpleTimer(tcmRequestContext.RequestContext, "AzureReposProvider: CreatePullRequestStatus"))
      {
        GitHttpClient gitHttpClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        GitPullRequestStatus gitPullRequestStatus = this.ConvertToGitPullRequestStatus(coverageStatusCheckResult, new int?(pipelineContext.PullRequestIterationId));
        TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy>((Action) (() => Task.Run<GitPullRequestStatus>((Func<Task<GitPullRequestStatus>>) (async () => await gitHttpClient.CreatePullRequestStatusAsync(gitPullRequestStatus, pipelineContext.ProjectId, pipelineContext.RepositoryId, pipelineContext.PullRequestId, cancellationToken: cancellationToken))).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
      }
    }

    public List<GitPullRequestStatus> GetPullRequestStatuses(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      CancellationToken cancellationToken)
    {
      using (new SimpleTimer(tcmRequestContext.RequestContext, "AzureReposProvider: GetPullRequestStatuses"))
      {
        GitHttpClient gitHttpClient = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
        return TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy, List<GitPullRequestStatus>>((Func<List<GitPullRequestStatus>>) (() => Task.Run<List<GitPullRequestStatus>>((Func<Task<List<GitPullRequestStatus>>>) (async () =>
        {
          GitHttpClient gitHttpClient1 = gitHttpClient;
          Guid repositoryId1 = pipelineContext.RepositoryId;
          Guid projectId = pipelineContext.ProjectId;
          Guid repositoryId2 = repositoryId1;
          int pullRequestId = pipelineContext.PullRequestId;
          CancellationToken cancellationToken1 = cancellationToken;
          return await gitHttpClient1.GetPullRequestStatusesAsync(projectId, repositoryId2, pullRequestId, cancellationToken: cancellationToken1);
        })).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
      }
    }

    public Stream GetFile(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      string buildSourceVersionCommit,
      string filePath)
    {
      GitHttpClient client = tcmRequestContext.RequestContext.GetClient<GitHttpClient>();
      GitVersionDescriptor versionDescriptor1 = new GitVersionDescriptor()
      {
        Version = buildSourceVersionCommit,
        VersionType = GitVersionType.Commit
      };
      Guid projectId = pipelineContext.ProjectId;
      Guid repositoryId = pipelineContext.RepositoryId;
      string path = filePath;
      GitVersionDescriptor versionDescriptor2 = versionDescriptor1;
      VersionControlRecursionType? recursionLevel = new VersionControlRecursionType?();
      bool? includeContentMetadata = new bool?();
      bool? latestProcessedChange = new bool?();
      bool? download = new bool?();
      GitVersionDescriptor versionDescriptor3 = versionDescriptor2;
      bool? includeContent = new bool?();
      bool? resolveLfs = new bool?();
      bool? sanitize = new bool?();
      CancellationToken cancellationToken = new CancellationToken();
      return client.GetItemTextAsync(projectId, repositoryId, path, recursionLevel: recursionLevel, includeContentMetadata: includeContentMetadata, latestProcessedChange: latestProcessedChange, download: download, versionDescriptor: versionDescriptor3, includeContent: includeContent, resolveLfs: resolveLfs, sanitize: sanitize, cancellationToken: cancellationToken).Result;
    }

    private GitPullRequestStatus ConvertToGitPullRequestStatus(
      CoverageStatusCheckResult coverageStatusCheckResult,
      int? iterationId)
    {
      GitStatusState gitStatusState = GitStatusState.NotSet;
      switch (coverageStatusCheckResult.State)
      {
        case CoverageStatusCheckState.Error:
          gitStatusState = GitStatusState.Error;
          break;
        case CoverageStatusCheckState.Failed:
          gitStatusState = GitStatusState.Failed;
          break;
        case CoverageStatusCheckState.InProgress:
          gitStatusState = GitStatusState.Pending;
          break;
        case CoverageStatusCheckState.Queued:
          gitStatusState = GitStatusState.NotSet;
          break;
        case CoverageStatusCheckState.Succeeded:
          gitStatusState = GitStatusState.Succeeded;
          break;
        case CoverageStatusCheckState.NotApplicable:
          gitStatusState = GitStatusState.NotApplicable;
          break;
      }
      GitPullRequestStatus pullRequestStatus = new GitPullRequestStatus();
      pullRequestStatus.Context = new GitStatusContext()
      {
        Genre = coverageStatusCheckResult.Source.ToLower(),
        Name = coverageStatusCheckResult.Name.ToLower()
      };
      pullRequestStatus.Description = coverageStatusCheckResult.Description;
      pullRequestStatus.IterationId = iterationId;
      pullRequestStatus.State = gitStatusState;
      return pullRequestStatus;
    }

    private string GetParentCommitId(
      TestManagementRequestContext tcmRequestContext,
      GitHttpClient gitClient,
      string commitId,
      Guid repositoryId)
    {
      GitCommit result = gitClient.GetCommitAsync(commitId, repositoryId).Result;
      string parentCommitId = (string) null;
      if (result.Parents != null && result.Parents.Any<string>())
        parentCommitId = result.Parents.First<string>();
      else
        tcmRequestContext.Logger.Error(1015691, "GetParentCommitId: Failed to find parent commit id of: " + commitId);
      return parentCommitId;
    }

    private List<FileDiff> GetFileDiffs(
      GitHttpClient gitClient,
      PipelineContext pipelineContext,
      List<FileParams> fileParamBatch,
      string baseVersionCommit,
      string targetVersionCommit)
    {
      List<FileDiffParams> fileDiffParamsList = new List<FileDiffParams>();
      foreach (FileParams fileParams in fileParamBatch)
        fileDiffParamsList.Add(new FileDiffParams()
        {
          Path = fileParams.Path,
          OriginalPath = fileParams.OriginalPath
        });
      FileDiffsCriteria fileDiffCriteriaForPR = new FileDiffsCriteria()
      {
        BaseVersionCommit = baseVersionCommit,
        TargetVersionCommit = targetVersionCommit,
        FileDiffParams = (IEnumerable<FileDiffParams>) fileDiffParamsList
      };
      return TransientErrorActionRetryer.TryAction<CatchNetworkErrorStrategy, List<FileDiff>>((Func<List<FileDiff>>) (() => Task.Run<List<FileDiff>>((Func<Task<List<FileDiff>>>) (async () => await gitClient.GetFileDiffsAsync(fileDiffCriteriaForPR, pipelineContext.ProjectId, pipelineContext.RepositoryId))).GetAwaiter().GetResult()), TransientErrorActionRetryer.ShorterExponentialBackoffRetryStrategy);
    }

    private List<FileDiffMapping> MapChangesInPRViewToChangesInMergeView(
      TestManagementRequestContext tcmRequestContext,
      List<FileDiff> prFilesDiff,
      List<FileDiff> mergeFilesDiff)
    {
      List<FileDiffMapping> changesInMergeView = new List<FileDiffMapping>();
      if (prFilesDiff.Count != mergeFilesDiff.Count)
      {
        tcmRequestContext.Logger.Info(1015688, string.Format("AzureReposProvider: Cannot map difference in FilesCount. PRFileCount: {0}, MergeFileCount: {1} ", (object) prFilesDiff.Count, (object) mergeFilesDiff.Count));
        return changesInMergeView;
      }
      prFilesDiff = prFilesDiff.OrderBy<FileDiff, string>((Func<FileDiff, string>) (prFileDiff => prFileDiff.Path)).ToList<FileDiff>();
      mergeFilesDiff = mergeFilesDiff.OrderBy<FileDiff, string>((Func<FileDiff, string>) (mergeFileDiff => mergeFileDiff.Path)).ToList<FileDiff>();
      foreach (var data1 in prFilesDiff.Zip((IEnumerable<FileDiff>) mergeFilesDiff, (first, second) => new
      {
        first = first,
        second = second
      }))
      {
        if (data1.first.LineDiffBlocks.Count<LineDiffBlock>() != data1.second.LineDiffBlocks.Count<LineDiffBlock>())
        {
          tcmRequestContext.Logger.Warning(1015689, "AzureReposProvider: Cannot map line differences for the file: " + data1.first.Path);
        }
        else
        {
          Dictionary<LineRange, LineRange> dictionary = new Dictionary<LineRange, LineRange>();
          foreach (var data2 in data1.first.LineDiffBlocks.Zip(data1.second.LineDiffBlocks, (first, second) => new
          {
            first = first,
            second = second
          }))
          {
            if (data2.first.ChangeType == LineDiffBlockChangeType.Add || data2.first.ChangeType == LineDiffBlockChangeType.Edit)
              dictionary.Add(new LineRange()
              {
                Start = (uint) data2.first.ModifiedLineNumberStart,
                Count = (uint) data2.first.ModifiedLinesCount
              }, new LineRange()
              {
                Start = (uint) data2.second.ModifiedLineNumberStart,
                Count = (uint) data2.second.ModifiedLinesCount
              });
          }
          changesInMergeView.Add(new FileDiffMapping()
          {
            Path = data1.first.Path,
            OriginalPath = data1.first.OriginalPath,
            DiffBlocksMap = dictionary
          });
        }
      }
      return changesInMergeView;
    }

    private bool ApplyFilters(
      GitChange gitChange,
      bool includeFilesOnly,
      bool includeDeletes,
      IEnumerable<string> extensionFilter)
    {
      if (includeFilesOnly && gitChange.Item.GitObjectType != GitObjectType.Blob || !includeDeletes && gitChange.ChangeType == VersionControlChangeType.Delete)
        return false;
      return extensionFilter == null || extensionFilter.Any<string>((Func<string, bool>) (extension => gitChange.Item.Path.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase)));
    }
  }
}
