// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitContainerLfsProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitContainerLfsProvider : ITfsGitLfsProvider
  {
    private readonly Func<IVssRequestContext, ITfsGitBlobProvider> m_getBlobPrv;

    internal GitContainerLfsProvider()
      : this(new Func<IVssRequestContext, ITfsGitBlobProvider>(DefaultGitDependencyRoot.Instance.GetBlobProvider))
    {
    }

    internal GitContainerLfsProvider(
      Func<IVssRequestContext, ITfsGitBlobProvider> getblobPrv)
    {
      this.m_getBlobPrv = getblobPrv;
    }

    public Stream GetLfsObject(IVssRequestContext rc, OdbId odbId, Sha256Id lfsObjectId)
    {
      ITfsGitBlobProvider tfsGitBlobProvider = this.m_getBlobPrv(rc);
      string objectResourceId = this.GetLfsObjectResourceId(lfsObjectId);
      IVssRequestContext rc1 = rc;
      OdbId odbId1 = odbId;
      string resourceId = objectResourceId;
      return tfsGitBlobProvider.GetStream(rc1, odbId1, resourceId, false);
    }

    public GitLfsObjectRef GetLfsObjectReference(
      IVssRequestContext rc,
      OdbId odbId,
      Sha256Id lfsObjectId)
    {
      GitLfsObject gitLfsObject = this.ReadLfsObjects(rc, odbId, (IEnumerable<Sha256Id>) new Sha256Id[1]
      {
        lfsObjectId
      }).FirstOrDefault<GitLfsObject>();
      return gitLfsObject == null ? (GitLfsObjectRef) null : new GitLfsObjectRef(gitLfsObject.ObjectId, gitLfsObject.Size);
    }

    public IEnumerable<GitLfsObjectRef> GetLfsObjectReferences(
      IVssRequestContext rc,
      OdbId odbId,
      IEnumerable<Sha256Id> lfsObjectIds)
    {
      return this.ReadLfsObjects(rc, odbId, lfsObjectIds).Select<GitLfsObject, GitLfsObjectRef>((Func<GitLfsObject, GitLfsObjectRef>) (x => new GitLfsObjectRef(x.ObjectId, x.Size)));
    }

    public bool LfsObjectExists(IVssRequestContext rc, OdbId odbId, Sha256Id lfsObjectId) => this.GetLfsObjectReference(rc, odbId, lfsObjectId) != (GitLfsObjectRef) null;

    public TfsGitLfsObjectCreateResult WriteLfsObject(
      IVssRequestContext rc,
      OdbId odbId,
      Sha256Id expectedLfsObjectId,
      Stream data)
    {
      ITfsGitBlobProvider blobProvider = this.m_getBlobPrv(rc);
      string str = "gitTemp/" + Guid.NewGuid().ToString("D");
      string objectResourceId = this.GetLfsObjectResourceId(expectedLfsObjectId);
      bool flag;
      using (HashingStream<SHA256Managed> source = new HashingStream<SHA256Managed>(data, FileAccess.Read, leaveOpen: true))
      {
        using (rc.CreateTimeToFirstPageExclusionBlock())
        {
          using (BlobProviderChunkingWriterStream chunkingWriterStream = new BlobProviderChunkingWriterStream(rc, blobProvider, odbId, str))
          {
            using (WriteBufferStream destination = new WriteBufferStream((Stream) chunkingWriterStream, 4194304))
              GitStreamUtil.SmartCopyTo((Stream) source, (Stream) destination);
          }
        }
        flag = expectedLfsObjectId == new Sha256Id(source.Hash);
      }
      TfsGitLfsObjectCreateResult objectCreateResult = TfsGitLfsObjectCreateResult.CreatedSuccessfully;
      if (!flag)
        objectCreateResult = TfsGitLfsObjectCreateResult.ContentsDoNotMatchLfsObjectId;
      else if (data.Length == 0L)
        objectCreateResult = TfsGitLfsObjectCreateResult.ContentsCannotBeEmpty;
      if (objectCreateResult == TfsGitLfsObjectCreateResult.CreatedSuccessfully)
      {
        try
        {
          blobProvider.RenameBlob(rc, odbId, str, objectResourceId);
        }
        catch (ContainerItemExistsException ex)
        {
        }
        using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(odbId))
          gitOdbComponent.WriteLfsObjects((IEnumerable<GitLfsObject>) new GitLfsObject[1]
          {
            new GitLfsObject(expectedLfsObjectId, data.Length)
          });
      }
      else
        blobProvider.DeleteBlob(rc, odbId, str);
      return objectCreateResult;
    }

    internal IReadOnlyList<GitLfsObject> ReadAllLfsObjects(IVssRequestContext rc, OdbId odbId)
    {
      using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(odbId))
        return gitOdbComponent.ReadAllLfsObjects();
    }

    private IReadOnlyList<GitLfsObject> ReadLfsObjects(
      IVssRequestContext rc,
      OdbId odbId,
      IEnumerable<Sha256Id> ids)
    {
      using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(odbId))
        return gitOdbComponent.ReadLfsObjects(ids);
    }

    internal string GetLfsObjectResourceId(Sha256Id lfsObjectId) => string.Format("lfs/{0}.lfs", (object) lfsObjectId);
  }
}
