// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IVersionControlProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface IVersionControlProvider
  {
    int GetFileCountInRepo(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext);

    bool IsFilePresentInRepo(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      string filePath);

    PullRequestChanges GetChangesInCurrentIteration(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      bool includeFilesOnly = false,
      bool includeDeletes = true,
      IEnumerable<string> extensionFilter = null);

    List<GitPullRequestIteration> GetPullRequestIterations(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      CancellationToken cancellationToken);

    GitPullRequestIteration GetPullRequestIteration(
      TestManagementRequestContext tcmRequestContent,
      PipelineContext pipelineContext);

    GitPullRequest GetPullRequestById(
      TestManagementRequestContext tcmRequestContext,
      int pullRequestId,
      CancellationToken cancellationToken);

    GitPullRequestCommentThread CreateThread(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      GitPullRequestCommentThread commentThread,
      CancellationToken cancellationToken);

    List<GitPullRequestCommentThread> GetPullRequestCommentThreads(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      int? baseIteration,
      CancellationToken cancellationToken);

    void UpdatePullRequestThread(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      GitPullRequestCommentThread commentThread,
      int threadId,
      CancellationToken cancellationToken = default (CancellationToken));

    void UpdatePullRequestThreadComment(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      GitPullRequestCommentThread commentThread,
      string comment,
      int threadId,
      CancellationToken cancellationToken = default (CancellationToken));

    List<FileDiffInfo> GetFileDiffs(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      PullRequestChanges pullRequestChanges);

    Dictionary<string, FileDiffMapping> GetFileDiffMappingsInCurrentIteration(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      PullRequestChanges pullRequestChanges);

    string GetPullRequestIterationUrl(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext);

    void CreatePullRequestStatus(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      CoverageStatusCheckResult coverageStatusCheckResult,
      CancellationToken cancellationToken);

    List<GitPullRequestStatus> GetPullRequestStatuses(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      CancellationToken cancellationToken);

    Stream GetFile(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      string buildSourceVersionCommit,
      string filePath);

    GitPullRequestIteration GetPullRequestIterationByIterationId(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext);
  }
}
