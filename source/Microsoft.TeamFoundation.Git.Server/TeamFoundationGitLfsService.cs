// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitLfsService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TeamFoundationGitLfsService : ITeamFoundationGitLfsService, IVssFrameworkService
  {
    private ITfsGitLfsProvider m_gitLfsContainerProvider;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_gitLfsContainerProvider = this.GetTfsGitLfsContainerProvider(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Stream GetLfsObject(IVssRequestContext rc, RepoKey repoKey, Sha256Id lfsObjectId)
    {
      ArgumentUtility.CheckForNull<RepoKey>(repoKey, nameof (repoKey));
      if (!this.IsAuthorizedToRead(rc, repoKey))
        return (Stream) null;
      try
      {
        return this.m_gitLfsContainerProvider.GetLfsObject(rc, repoKey.OdbId, lfsObjectId);
      }
      catch (FileIdNotFoundException ex)
      {
        return (Stream) null;
      }
      catch (FileNotFoundException ex)
      {
        return (Stream) null;
      }
    }

    public GitLfsObjectRef GetLfsObjectReference(
      IVssRequestContext rc,
      RepoKey repoKey,
      Sha256Id lfsObjectId)
    {
      ArgumentUtility.CheckForNull<RepoKey>(repoKey, nameof (repoKey));
      return !this.IsAuthorizedToRead(rc, repoKey) ? (GitLfsObjectRef) null : this.m_gitLfsContainerProvider.GetLfsObjectReference(rc, repoKey.OdbId, lfsObjectId);
    }

    public IEnumerable<GitLfsObjectRef> GetLfsObjectReferences(
      IVssRequestContext rc,
      RepoKey repoKey,
      IEnumerable<Sha256Id> lfsObjectIds)
    {
      ArgumentUtility.CheckForNull<RepoKey>(repoKey, nameof (repoKey));
      ArgumentUtility.CheckForNull<IEnumerable<Sha256Id>>(lfsObjectIds, nameof (lfsObjectIds));
      return !this.IsAuthorizedToRead(rc, repoKey) ? (IEnumerable<GitLfsObjectRef>) null : this.m_gitLfsContainerProvider.GetLfsObjectReferences(rc, repoKey.OdbId, lfsObjectIds);
    }

    public TfsGitLfsObjectCreateResult UploadLfsObject(
      IVssRequestContext rc,
      RepoKey repoKey,
      Sha256Id lfsObjectId,
      Stream data)
    {
      ArgumentUtility.CheckForNull<RepoKey>(repoKey, nameof (repoKey));
      ArgumentUtility.CheckForNull<Stream>(data, nameof (data));
      if (!this.IsAuthorizedToWrite(rc, repoKey))
        return TfsGitLfsObjectCreateResult.MissingWritePermission;
      return this.m_gitLfsContainerProvider.LfsObjectExists(rc, repoKey.OdbId, lfsObjectId) ? TfsGitLfsObjectCreateResult.AlreadyExists : this.m_gitLfsContainerProvider.WriteLfsObject(rc, repoKey.OdbId, lfsObjectId, data);
    }

    protected virtual bool IsAuthorizedToRead(IVssRequestContext rc, RepoKey repoKey) => SecurityHelper.Instance.HasReadPermission(rc, (RepoScope) repoKey);

    protected virtual bool IsAuthorizedToWrite(IVssRequestContext rc, RepoKey repoKey) => SecurityHelper.Instance.HasWritePermission(rc, (RepoScope) repoKey, (string) null, true);

    internal virtual ITfsGitLfsProvider GetTfsGitLfsContainerProvider(IVssRequestContext systemRC) => (ITfsGitLfsProvider) new GitContainerLfsProvider();
  }
}
