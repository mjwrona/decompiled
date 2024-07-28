// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.FileContainerGitLfsProvider
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
  internal class FileContainerGitLfsProvider : FileContainerProvider, ITfsGitLfsProvider
  {
    public bool LfsObjectExists(IVssRequestContext rc, OdbId odbId, Sha256Id lfsObjectId) => this.GetItem(rc, odbId, lfsObjectId.ToString(), false) != null;

    public Stream GetLfsObject(IVssRequestContext rc, OdbId odbId, Sha256Id lfsObjectId) => this.GetStream(rc, odbId, lfsObjectId.ToString(), false);

    public GitLfsObjectRef GetLfsObjectReference(
      IVssRequestContext rc,
      OdbId odbId,
      Sha256Id lfsObjectId)
    {
      FileContainerItem fileContainerItem = this.GetItem(rc, odbId, lfsObjectId.ToString(), false);
      return fileContainerItem != null ? new GitLfsObjectRef(lfsObjectId, fileContainerItem.FileLength) : (GitLfsObjectRef) null;
    }

    public IEnumerable<GitLfsObjectRef> GetLfsObjectReferences(
      IVssRequestContext rc,
      OdbId odbId,
      IEnumerable<Sha256Id> lfsObjectIds)
    {
      IEnumerable<FileContainerItem> items = this.GetItems(rc, odbId, lfsObjectIds.Select<Sha256Id, string>((Func<Sha256Id, string>) (x => x.ToString())), false);
      return items != null ? items.Select<FileContainerItem, GitLfsObjectRef>((Func<FileContainerItem, GitLfsObjectRef>) (item => new GitLfsObjectRef(item.Path, item.FileLength))) : (IEnumerable<GitLfsObjectRef>) null;
    }

    public TfsGitLfsObjectCreateResult WriteLfsObject(
      IVssRequestContext rc,
      OdbId odbId,
      Sha256Id expectedLfsObjectId,
      Stream data)
    {
      string str = "gitTemp\\" + Guid.NewGuid().ToString("D");
      bool flag;
      using (HashingStream<SHA256Managed> hashingStream = new HashingStream<SHA256Managed>(data, FileAccess.Read, leaveOpen: true))
      {
        using (rc.CreateTimeToFirstPageExclusionBlock())
          this.PutStream(rc, odbId, str, (Stream) hashingStream, hashingStream.Length);
        flag = expectedLfsObjectId == new Sha256Id(hashingStream.Hash);
      }
      if (flag)
      {
        try
        {
          this.RenameBlob(rc, odbId, str, expectedLfsObjectId.ToString());
          return TfsGitLfsObjectCreateResult.CreatedSuccessfully;
        }
        catch (ContainerItemExistsException ex)
        {
          return TfsGitLfsObjectCreateResult.AlreadyExists;
        }
      }
      else
      {
        this.DeleteBlob(rc, odbId, str);
        return TfsGitLfsObjectCreateResult.ContentsDoNotMatchLfsObjectId;
      }
    }
  }
}
