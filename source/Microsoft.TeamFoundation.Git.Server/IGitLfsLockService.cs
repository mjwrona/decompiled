// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitLfsLockService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Lfs;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (GitLfsLockService))]
  public interface IGitLfsLockService : IVssFrameworkService
  {
    LfsLock GetLock(IVssRequestContext rc, ITfsGitRepository repo, int id);

    GitLfsCreateLockResult TryCreateLock(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      string path);

    GitLfsDeleteLockResult DeleteLock(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      int id,
      bool force);

    GitLfsGetLocksResult GetLocks(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      int cursor,
      int limit,
      string path);

    GitLfsVerifyLocksResult VerifyLocks(
      IVssRequestContext rc,
      ITfsGitRepository repo,
      int cursor,
      int limit);
  }
}
