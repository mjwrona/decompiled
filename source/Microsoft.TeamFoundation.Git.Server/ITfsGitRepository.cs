// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITfsGitRepository
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  public interface ITfsGitRepository : IDisposable
  {
    string Name { get; }

    bool IsFork { get; }

    long Size { get; }

    RepoKey Key { get; }

    bool IsDisabled { get; }

    bool IsInMaintenance { get; }

    IGitObjectSet Objects { get; }

    GitRepoSettings Settings { get; }

    GitOdbSettings OdbSettings { get; }

    ITfsGitRepositoryRefsCollection Refs { get; }

    ICherryPickRelationships CherryPickRelationships { get; }

    IRepoPermissionsManager Permissions { get; }

    PackAndRefIngester CreatePackAndRefIngester(out Stream tempPackStream);

    PackAndRefIngester CreatePackAndRefIngester(Stream packStream, ClientTraceData ctData = null);

    IGitCommitGraph GetCommitGraph(IEnumerable<Sha1Id> requiredCommits);

    string GetRepositoryWebUri();

    string GetRepositoryCloneUri();

    string GetRepositoryFullUri();

    TfsGitRefUpdateResultSet ModifyPaths(
      string refName,
      Sha1Id baseCommitId,
      string comment,
      IEnumerable<GitChange> changes,
      GitUserDate author,
      GitUserDate committer,
      TimeSpan? authorOffset = null,
      TimeSpan? committerOffset = null);

    List<TfsGitPushMetadata> QueryPushHistory(
      bool includeRefUpdates,
      DateTime? fromDate,
      DateTime? toDate,
      IEnumerable<Guid> pusherIds,
      bool excludePushers,
      int? skip,
      int? take,
      string refName);

    IObjectMetadata ObjectMetadata { get; }
  }
}
