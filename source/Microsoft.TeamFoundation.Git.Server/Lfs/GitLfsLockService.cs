// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Lfs.GitLfsLockService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Lfs
{
  internal sealed class GitLfsLockService : IGitLfsLockService, IVssFrameworkService
  {
    public GitLfsCreateLockResult TryCreateLock(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      string path)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repo, nameof (repo));
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      if (!SecurityHelper.Instance.HasWritePermission(rc, (RepoScope) repo.Key, (string) null, true))
        return new GitLfsCreateLockResult(GitLfsCreateLockResultStatus.NotAllowedToCreate);
      if (path.Length > 1024)
        return new GitLfsCreateLockResult(GitLfsCreateLockResultStatus.PathTooLong);
      Guid userId = rc.GetUserId();
      LfsLock lfsLock;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        lfsLock = gitCoreComponent.CreateLfsLock(repo.Key, userId, path);
      if (lfsLock == null)
        return new GitLfsCreateLockResult(GitLfsCreateLockResultStatus.CreateFailed);
      this.FillIdentityName(rc, lfsLock);
      GitLfsCreateLockResultStatus status = lfsLock.LockActionSucceeded ? GitLfsCreateLockResultStatus.Created : GitLfsCreateLockResultStatus.Conflict;
      return new GitLfsCreateLockResult(lfsLock, status);
    }

    public GitLfsDeleteLockResult DeleteLock(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      int id,
      bool force)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repo, nameof (repo));
      if (!SecurityHelper.Instance.HasWritePermission(rc, (RepoScope) repo.Key, (string) null, true))
      {
        LfsLock lfsLock = this.GetLock(rc, repo, id);
        this.FillIdentityName(rc, lfsLock);
        return new GitLfsDeleteLockResult(lfsLock, GitLfsDeleteLockResultStatus.NotAllowedToDelete);
      }
      if (force && !SecurityHelper.Instance.HasRemoveOthersLocksPermission(rc, (RepoScope) repo.Key))
      {
        LfsLock lfsLock = this.GetLock(rc, repo, id);
        this.FillIdentityName(rc, lfsLock);
        return new GitLfsDeleteLockResult(lfsLock, GitLfsDeleteLockResultStatus.NotAllowedToForceDelete);
      }
      LfsLock lfsLock1;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        lfsLock1 = gitCoreComponent.DeleteLfsLock(repo.Key, id, rc.GetUserId(), force);
      this.FillIdentityName(rc, lfsLock1);
      if (lfsLock1 == null)
        return new GitLfsDeleteLockResult(GitLfsDeleteLockResultStatus.LockDoesNotExist);
      GitLfsDeleteLockResultStatus status = lfsLock1.LockActionSucceeded ? GitLfsDeleteLockResultStatus.Deleted : GitLfsDeleteLockResultStatus.MustForceDelete;
      return new GitLfsDeleteLockResult(lfsLock1, status);
    }

    public LfsLock GetLock(IVssRequestContext rc, ITfsGitRepository repo, int id)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repo, nameof (repo));
      LfsLock lfsLockById;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        lfsLockById = gitCoreComponent.GetLfsLockById(repo.Key, id);
      this.FillIdentityName(rc, lfsLockById);
      return lfsLockById;
    }

    public GitLfsGetLocksResult GetLocks(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      int cursor,
      int limit,
      string path)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repo, nameof (repo));
      IReadOnlyList<LfsLock> lfsLocks;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        lfsLocks = gitCoreComponent.GetLfsLocks(repo.Key, cursor, limit + 1, path);
      this.FillIdentityNames(rc, lfsLocks);
      if (lfsLocks.Count <= limit)
        return new GitLfsGetLocksResult(lfsLocks, new int?());
      List<LfsLock> locks = new List<LfsLock>();
      for (int index = 0; index < limit; ++index)
        locks.Add(lfsLocks[index]);
      return new GitLfsGetLocksResult((IReadOnlyList<LfsLock>) locks, new int?(lfsLocks[limit].Id));
    }

    public GitLfsVerifyLocksResult VerifyLocks(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      int cursor,
      int limit)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repo, nameof (repo));
      GitLfsGetLocksResult locks = this.GetLocks(rc, repo, cursor, limit, (string) null);
      List<LfsLock> lfsLockList1 = new List<LfsLock>();
      List<LfsLock> lfsLockList2 = new List<LfsLock>();
      Guid userId = rc.GetUserId();
      for (int index = 0; index < locks.Locks.Count; ++index)
      {
        if (locks.Locks[index].OwnerId.Equals(userId))
          lfsLockList1.Add(locks.Locks[index]);
        else
          lfsLockList2.Add(locks.Locks[index]);
      }
      return new GitLfsVerifyLocksResult((IReadOnlyList<LfsLock>) lfsLockList1.AsReadOnly(), (IReadOnlyList<LfsLock>) lfsLockList2.AsReadOnly(), locks.NextCursor);
    }

    private void FillIdentityName(IVssRequestContext rc, LfsLock lfsLock)
    {
      if (lfsLock == null)
        return;
      this.FillIdentityNames(rc, (IReadOnlyList<LfsLock>) new LfsLock[1]
      {
        lfsLock
      });
    }

    private void FillIdentityNames(IVssRequestContext rc, IReadOnlyList<LfsLock> locks)
    {
      Guid[] array = locks.Select<LfsLock, Guid>((Func<LfsLock, Guid>) (l => l.OwnerId)).Distinct<Guid>().ToArray<Guid>();
      Dictionary<Guid, string> dictionary = ((IEnumerable<TeamFoundationIdentity>) rc.GetService<ITeamFoundationIdentityService>().ReadIdentities(rc, array)).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (i => i != null)).ToDictionary<TeamFoundationIdentity, Guid, string>((Func<TeamFoundationIdentity, Guid>) (id => id.TeamFoundationId), (Func<TeamFoundationIdentity, string>) (id => id.DisplayName));
      foreach (LfsLock lfsLock in (IEnumerable<LfsLock>) locks)
      {
        string str;
        if (dictionary.TryGetValue(lfsLock.OwnerId, out str))
          lfsLock.OwnerName = str;
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
