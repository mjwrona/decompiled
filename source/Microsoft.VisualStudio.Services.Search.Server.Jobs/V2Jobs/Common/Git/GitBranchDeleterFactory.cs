// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git.GitBranchDeleterFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Indexer;

namespace Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git
{
  internal static class GitBranchDeleterFactory
  {
    public static GitBranchDeleter GetGitBranchDeleter(
      IndexingExecutionContext indexingExecutionContext)
    {
      return indexingExecutionContext.RepositoryIndexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext) ? (GitBranchDeleter) new LargeRepoAccountGitBranchDeleter(indexingExecutionContext) : (GitBranchDeleter) new NormalRepoGitBranchDeleter();
    }
  }
}
