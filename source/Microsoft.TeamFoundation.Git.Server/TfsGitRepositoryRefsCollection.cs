// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitRepositoryRefsCollection
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class TfsGitRepositoryRefsCollection : ITfsGitRepositoryRefsCollection
  {
    private readonly ITfsGitRepository m_repository;
    private readonly IVssRequestContext m_requestContext;

    internal TfsGitRepositoryRefsCollection(
      IVssRequestContext requestContext,
      ITfsGitRepository repository)
    {
      this.m_requestContext = requestContext;
      this.m_repository = repository;
    }

    public List<TfsGitRef> MatchingNames(
      IEnumerable<string> refNames,
      GitRefSearchType searchType = GitRefSearchType.Exact,
      string refArea = "heads",
      string firstRefName = null,
      int? pageSize = null)
    {
      return this.ReadFilteredSqlRefs(refNames != null ? refNames.Where<string>((Func<string, bool>) (requested => !string.IsNullOrWhiteSpace(requested))) : (IEnumerable<string>) null, searchType, refArea, firstRefName, pageSize);
    }

    public List<TfsGitRefWithResolvedCommit> QueryGitRefsBySearchCriteria(
      IEnumerable<string> refNames,
      GitRefSearchType type,
      IEnumerable<Sha1Id> commitIds)
    {
      this.PruneLongRefName(refNames);
      if ((commitIds == null || !commitIds.Any<Sha1Id>()) && (refNames == null || !refNames.Any<string>()))
        return new List<TfsGitRefWithResolvedCommit>(0);
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return gitCoreComponent.QueryGitRefsBySearchCriteria(this.m_repository.Key, (IEnumerable<string>) ((object) refNames ?? (object) Array.Empty<string>()), commitIds, type);
    }

    public TfsGitRef MatchingName(string refName)
    {
      if (!RefUtil.IsValidRefName(refName, false))
        return (TfsGitRef) null;
      return this.MatchingNames((IEnumerable<string>) new string[1]
      {
        refName
      }, GitRefSearchType.Exact, "heads", (string) null, new int?()).FirstOrDefault<TfsGitRef>();
    }

    public List<TfsGitRef> All() => this.ReadSqlRefs(false);

    public List<TfsGitRef> Limited(int breadcrumbDays = 0) => this.ReadSqlRefs(true, breadcrumbDays);

    public IReadOnlyList<TfsGitRef> AllRefHeads() => (IReadOnlyList<TfsGitRef>) this.ReadFilteredSqlRefs((IEnumerable<string>) new string[1]
    {
      "refs/heads/"
    }, GitRefSearchType.StartsWith);

    public IEnumerable<TfsGitRef> OptimizedRefHeads() => this.m_repository.Settings.OptimizedByDefault ? (IEnumerable<TfsGitRef>) this.Limited(0) : (IEnumerable<TfsGitRef>) this.AllRefHeads();

    public IEnumerable<TfsGitRef> AllRefTags() => (IEnumerable<TfsGitRef>) this.ReadFilteredSqlRefs((IEnumerable<string>) new string[1]
    {
      "refs/tags/"
    }, GitRefSearchType.StartsWith);

    public IEnumerable<TfsGitRefWithResolvedCommit> RefsByCommitIds(
      IEnumerable<Sha1Id> commitIds,
      string refNamePrefix)
    {
      List<string> refNames;
      if (!string.IsNullOrEmpty(refNamePrefix))
        refNames = new List<string>() { refNamePrefix };
      else
        refNames = new List<string>(0);
      IEnumerable<Sha1Id> commitIds1 = commitIds;
      return (IEnumerable<TfsGitRefWithResolvedCommit>) this.QueryGitRefsBySearchCriteria((IEnumerable<string>) refNames, GitRefSearchType.StartsWith, commitIds1);
    }

    public IEnumerable<TfsGitRef> AllRefNotes() => (IEnumerable<TfsGitRef>) this.ReadFilteredSqlRefs((IEnumerable<string>) new string[1]
    {
      "refs/notes/"
    }, GitRefSearchType.StartsWith);

    public IEnumerable<TfsGitRef> RefNotesInNamespace(string noteNamespace)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(noteNamespace, nameof (noteNamespace));
      return (IEnumerable<TfsGitRef>) this.ReadFilteredSqlRefs((IEnumerable<string>) new string[1]
      {
        "refs/notes/" + noteNamespace
      }, GitRefSearchType.Exact);
    }

    public TfsGitRef GetDefault()
    {
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return gitCoreComponent.ReadDefaultBranch(this.m_repository.Key);
    }

    public TfsGitRef GetDefaultOrAny() => this.GetDefault() ?? this.MatchingName("refs/heads/master") ?? this.AllRefHeads().FirstOrDefault<TfsGitRef>();

    public IEnumerable<TfsGitRef> GetMyBranches()
    {
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return (IEnumerable<TfsGitRef>) gitCoreComponent.QueryGitRefs(this.m_repository.Key, this.m_requestContext.GetUserId(), true, false, true, true, 0, (string) null);
    }

    public IEnumerable<TfsGitRef> GetUserCreated()
    {
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return (IEnumerable<TfsGitRef>) gitCoreComponent.QueryGitRefs(this.m_repository.Key, this.m_requestContext.GetUserId(), false, false, true, false, 0, (string) null);
    }

    private List<TfsGitRef> ReadSqlRefs(bool limitRefs, int breadcrumbDays = 0)
    {
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return limitRefs ? gitCoreComponent.QueryGitRefs(this.m_repository.Key, this.m_requestContext.GetUserId(), true, true, true, true, breadcrumbDays, breadcrumbDays > 0 ? "refs/internal/bc/" : (string) null) : gitCoreComponent.ReadRefs(this.m_repository.Key, (IEnumerable<string>) Array.Empty<string>(), GitRefSearchType.StartsWith);
    }

    private List<TfsGitRef> ReadFilteredSqlRefs(
      IEnumerable<string> refNames,
      GitRefSearchType searchType,
      string refArea = "heads",
      string firstRefName = null,
      int? pageSize = null)
    {
      if (refNames != null && refNames.Any<string>())
      {
        refNames = this.PruneLongRefName(refNames);
        if (!refNames.Any<string>())
          return new List<TfsGitRef>(0);
      }
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return gitCoreComponent.ReadRefs(this.m_repository.Key, (IEnumerable<string>) ((object) refNames ?? (object) Array.Empty<string>()), searchType, refArea, firstRefName, pageSize);
    }

    private IEnumerable<string> PruneLongRefName(IEnumerable<string> refNames)
    {
      if (refNames != null && refNames.Any<string>())
        refNames = refNames.Where<string>((Func<string, bool>) (x => x.Length <= GitConstants.MaxGitRefNameLength));
      return refNames;
    }

    public void SetDefault(string refName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(refName, nameof (refName));
      if (this.GetDefault() != null)
        SecurityHelper.Instance.CheckEditPoliciesPermission(this.m_requestContext, (RepoScope) this.m_repository.Key);
      string name = this.GetDefault()?.Name;
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        gitCoreComponent.SetDefaultBranch(this.m_repository.Key, refName);
      try
      {
        GitAuditLogHelper.RepositoryModified(this.m_requestContext, this.m_repository.Key, this.m_repository.Name, (string) null, name, refName);
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(1013881, GitServerUtils.TraceArea, nameof (TfsGitRepositoryRefsCollection), ex);
      }
    }

    public void Lock(string refName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(refName, nameof (refName));
      SecurityHelper.Instance.CheckWritePermission(this.m_requestContext, (RepoScope) this.m_repository.Key, refName);
      Guid teamFoundationId = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(this.m_requestContext).TeamFoundationId;
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        gitCoreComponent.LockRef(this.m_repository.Key, refName, teamFoundationId);
    }

    public void Unlock(string refName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(refName, nameof (refName));
      bool skipIdCheck = SecurityHelper.Instance.HasRemoveOthersLocksPermission(this.m_requestContext, (RepoScope) this.m_repository.Key, refName);
      Guid teamFoundationId = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(this.m_requestContext).TeamFoundationId;
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        gitCoreComponent.UnlockRef(this.m_repository.Key, refName, teamFoundationId, skipIdCheck);
    }

    public static bool IsTagRef(string gitRefName) => !string.IsNullOrEmpty(gitRefName) && gitRefName.StartsWith("refs/tags/", StringComparison.Ordinal);

    public static bool IsNoteRef(string gitRefName) => !string.IsNullOrEmpty(gitRefName) && gitRefName.StartsWith("refs/notes/", StringComparison.Ordinal);
  }
}
