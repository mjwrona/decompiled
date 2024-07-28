// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IInternalGitCommitService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (InternalGitCommitService))]
  internal interface IInternalGitCommitService : IVssFrameworkService
  {
    TimeSpan ProcessingJobTimeout { get; }

    bool ComputeAndPersistCommitMetadata(IVssRequestContext rc, OdbId odbId);

    bool ComputeAndPersistCommitMetadata(
      IVssRequestContext requestContext,
      OdbId odbId,
      IEnumerable<CommitMetadataKey> commits,
      Func<CommitMetadataKey, CommitMetadataUpdate> metadataUpdateBuilder = null);

    void QueueCommitMetadataCatchupJob(
      IVssRequestContext requestContext,
      OdbId odbId,
      bool queueNow,
      bool idlePriority = false);

    List<UnprocessedCommit> QueryUnprocessedCommits(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      Guid jobId,
      int take);

    void MarkProcessedCommits(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      Guid jobId,
      IEnumerable<UnprocessedCommit> commits);
  }
}
