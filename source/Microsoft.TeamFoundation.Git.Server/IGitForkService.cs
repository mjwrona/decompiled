// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitForkService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Forks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (GitForkService))]
  public interface IGitForkService : IVssFrameworkService
  {
    IEnumerable<GitRepositoryRef> QueryChildren(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      Guid collectionId);

    GitRepositoryRef GetParent(IVssRequestContext requestContext, RepoKey repoKey);

    bool TryCalculateMergeBase(
      ITfsGitRepository repo,
      Sha1Id commit,
      ITfsGitRepository otherRepo,
      Sha1Id otherCommit,
      out Sha1Id maybeMergeBase);

    bool TryCalculateMergeBases(
      ITfsGitRepository repo,
      Sha1Id commit,
      ITfsGitRepository otherRepo,
      Sha1Id otherCommit,
      out IEnumerable<Sha1Id> maybeMergeBases);

    ITfsGitRepository CreateFork(
      IVssRequestContext requestContext,
      GlobalGitRepositoryKey source,
      Guid targetProjectId,
      string newRepositoryName);

    ForkFetchAsyncOp SyncFork(
      IVssRequestContext requestContext,
      GitForkSyncRequestParameters sourceParams,
      RepoKey targetRepoKey,
      bool copySourceRepoDefaults = false);

    ForkFetchAsyncOp GetFetchRequestById(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int createForkOperationId);

    IEnumerable<ForkFetchAsyncOp> QueryFetchRequests(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      bool includeAbandoned);

    ForkFetchAsyncOp UpdateFetchProgress(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int createForkOperationId,
      GitAsyncOperationStatus status,
      ForkUpdateStep step,
      string errorMessage = null);
  }
}
