// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITfsGitRepositoryRefsCollection
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public interface ITfsGitRepositoryRefsCollection
  {
    List<TfsGitRef> MatchingNames(
      IEnumerable<string> refNames,
      GitRefSearchType searchType = GitRefSearchType.Exact,
      string refArea = "heads",
      string firstRefName = null,
      int? pageSize = null);

    TfsGitRef MatchingName(string refName);

    List<TfsGitRef> All();

    List<TfsGitRef> Limited(int breadcrumbDays = 0);

    IReadOnlyList<TfsGitRef> AllRefHeads();

    IEnumerable<TfsGitRef> OptimizedRefHeads();

    IEnumerable<TfsGitRef> AllRefTags();

    IEnumerable<TfsGitRefWithResolvedCommit> RefsByCommitIds(
      IEnumerable<Sha1Id> commitIds,
      string refNamePrefix);

    List<TfsGitRefWithResolvedCommit> QueryGitRefsBySearchCriteria(
      IEnumerable<string> refNames,
      GitRefSearchType type,
      IEnumerable<Sha1Id> commitIds);

    IEnumerable<TfsGitRef> AllRefNotes();

    IEnumerable<TfsGitRef> RefNotesInNamespace(string noteNamespace);

    TfsGitRef GetDefault();

    TfsGitRef GetDefaultOrAny();

    IEnumerable<TfsGitRef> GetMyBranches();

    IEnumerable<TfsGitRef> GetUserCreated();

    void SetDefault(string refName);

    void Lock(string refName);

    void Unlock(string refName);
  }
}
