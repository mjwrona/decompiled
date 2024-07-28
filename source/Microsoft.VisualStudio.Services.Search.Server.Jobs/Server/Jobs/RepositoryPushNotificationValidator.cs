// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.RepositoryPushNotificationValidator
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class RepositoryPushNotificationValidator
  {
    private ExecutionContext m_executionContext;
    private const string s_traceArea = "Indexing Pipeline";
    private const string s_traceLayer = "RepositoryPushNotificationValidator";

    public RepositoryPushNotificationValidator(ExecutionContext executionContext) => this.m_executionContext = executionContext;

    public virtual GitCommit FetchBranchCommitInfo(
      string repositoryId,
      string projectId,
      string branchName)
    {
      return (this.GitHttpClient ?? new GitHttpClientWrapper(this.m_executionContext, projectId, repositoryId, new TraceMetaData(1080292, "Indexing Pipeline", nameof (RepositoryPushNotificationValidator)))).GetLatestCommitFromTFS(branchName, new Guid(repositoryId));
    }

    public virtual GitPush FetchBranchNextGitPushInfo(
      string repositoryId,
      string projectId,
      string branchName,
      int pushId)
    {
      return (this.GitHttpClient ?? new GitHttpClientWrapper(this.m_executionContext, projectId, repositoryId, new TraceMetaData(1080292, "Indexing Pipeline", nameof (RepositoryPushNotificationValidator)))).GetNextGitPush(branchName, new Guid(repositoryId), pushId);
    }

    public virtual GitCommit FetchBranchCommitInfo(
      string repositoryId,
      string projectId,
      string branchName,
      string commitId)
    {
      return (this.GitHttpClient ?? new GitHttpClientWrapper(this.m_executionContext, projectId, repositoryId, new TraceMetaData(1080292, "Indexing Pipeline", nameof (RepositoryPushNotificationValidator)))).GetCommit(new Guid(repositoryId), commitId);
    }

    internal GitHttpClientWrapper GitHttpClient { private get; set; }

    public class BranchStat
    {
      public string LastCommitId { get; set; }

      public DateTime LastCommitDate { get; set; }
    }
  }
}
