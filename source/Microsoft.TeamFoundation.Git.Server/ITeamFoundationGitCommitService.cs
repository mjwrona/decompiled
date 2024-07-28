// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITeamFoundationGitCommitService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationGitCommitService))]
  public interface ITeamFoundationGitCommitService : IVssFrameworkService
  {
    IEnumerable<TfsGitCommitLineageDiff> DiffCommitLineages(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id baseCommitId,
      IEnumerable<TfsGitRef> refs);

    IEnumerable<TfsGitCommitLineageDiff> DiffCommitLineagesAgainstCommits(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id baseCommitId,
      IEnumerable<Sha1Id> commitIdsToDiff);

    bool HasMultipleMergeBases(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId1,
      Sha1Id commitId2);

    TfsGitCommit GetMergeBase(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId1,
      Sha1Id commitId2);

    IEnumerable<TfsGitCommit> GetMergeBases(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId1,
      Sha1Id commitId2);

    Artifact[] GetArtifacts(IVssRequestContext requestContext, string[] artifactUris);

    CommitMetadataAndChanges GetCommitManifest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId,
      bool isEntriesCountLimited = false);

    IReadOnlyList<TfsGitCommitMetadata> GetCommitsById(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<Sha1Id> commitIds);

    IReadOnlyList<TfsGitCommitMetadata> GetOrderedCommitsById(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<Sha1Id> commitIds);

    IReadOnlyList<TfsGitCommitMetadata> GetPushCommitsByPushId(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      int pushId,
      int? skip = null,
      int? take = null);

    IReadOnlyList<TfsGitPushMetadata> GetPushDataForPushIds(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int[] pushIds,
      bool includeRefUpdates = false);

    IReadOnlyDictionary<Sha1Id, int> GetPushIdsByCommitIds(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<Sha1Id> commitIds);

    IReadOnlyList<string> QueryCommitAuthorNames(
      IVssRequestContext requestContext,
      ITfsGitRepository repository);

    IEnumerable<TfsGitCommitHistoryEntry> QueryCommitHistory(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id? commitId,
      string path,
      bool recursive,
      bool excludeDeletes = false,
      string author = null,
      string committer = null,
      DateTime? fromDate = null,
      DateTime? toDate = null,
      Sha1Id? fromCommitId = null,
      Sha1Id? toCommitId = null,
      Sha1Id? compareCommitId = null,
      int? skip = null,
      int? maxItemCount = null,
      bool useTopoOrder = false,
      GitLogHistoryMode historyMode = GitLogHistoryMode.Simplified,
      bool showOldestCommitsFirst = false);

    IEnumerable<TfsGitCommitHistoryEntry> QueryCommitItems(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      Sha1Id commitId,
      string parentPath,
      QueryCommitItemsRecursionLevel recursionLevel);

    GitUserDate CreateUserDate(
      IVssRequestContext rc,
      string name,
      string email,
      DateTime date,
      bool includeImageUrl);
  }
}
