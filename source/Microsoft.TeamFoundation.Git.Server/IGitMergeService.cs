// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitMergeService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (GitMergeService))]
  public interface IGitMergeService : IVssFrameworkService
  {
    GitMergeAsyncOp CreateMergeRequest(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitMergeParameters mergeParameters);

    GitMergeAsyncOp GetMergeRequestById(
      IVssRequestContext requestContext,
      RepoKey key,
      int mergeOperationId);

    void PerformMerge(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitMergeAsyncOp mergeRequest,
      ClientTraceData ctdata);

    GitMergeAsyncOp UpdateMergeProgress(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int mergeOperationId,
      GitAsyncOperationStatus status,
      Sha1Id? mergeCommitId = null,
      string errorMessage = null);
  }
}
