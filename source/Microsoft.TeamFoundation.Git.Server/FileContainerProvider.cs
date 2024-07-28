// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.FileContainerProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal abstract class FileContainerProvider
  {
    protected const long ContainerIdNotFound = 0;

    public virtual void DeleteBlob(IVssRequestContext rc, OdbId odbId, string resourceId) => this.DeleteBlobs(rc, odbId, (IList<string>) new string[1]
    {
      resourceId
    });

    public virtual void DeleteBlobs(IVssRequestContext rc, OdbId odbId, IList<string> resourceIds)
    {
      rc = rc.Elevate();
      ITeamFoundationFileContainerService service = rc.GetService<ITeamFoundationFileContainerService>();
      long containerId = this.GetContainerId(rc, service, odbId);
      service.DeleteItems(rc, containerId, resourceIds, odbId.Value);
    }

    public virtual void DeleteContainer(
      IVssRequestContext rc,
      OdbId odbId,
      bool throwIfContainerNotFound = true)
    {
      rc = rc.Elevate();
      ITeamFoundationFileContainerService service = rc.GetService<ITeamFoundationFileContainerService>();
      long containerId = this.GetContainerId(rc, service, odbId, throwIfContainerNotFound);
      if (containerId == 0L)
        return;
      service.DeleteContainer(rc, containerId, odbId.Value);
    }

    public virtual bool DownloadToStream(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      Stream destination,
      bool throwIfNotFound = true)
    {
      rc = rc.Elevate();
      FileContainerItem fileContainerItem = this.GetItem(rc, odbId, resourceId, throwIfNotFound);
      if (fileContainerItem == null)
        return false;
      using (Stream stream = this.RetrieveFile(rc, fileContainerItem.FileId, false, out byte[] _, out long _, out CompressionType _))
      {
        stream.CopyTo(destination);
        return true;
      }
    }

    public IEnumerable<string> EnumerateBlobs(
      IVssRequestContext rc,
      OdbId odbId,
      TimeSpan? enumerateBlobsClientTimeout = null)
    {
      return this.QueryAllItems(rc, odbId).Select<FileContainerItem, string>((Func<FileContainerItem, string>) (item => item.Path));
    }

    private IEnumerable<FileContainerItem> QueryAllItems(IVssRequestContext rc, OdbId odbId)
    {
      rc = rc.Elevate();
      ITeamFoundationFileContainerService service = rc.GetService<ITeamFoundationFileContainerService>();
      long containerId = this.GetContainerId(rc, service, odbId);
      return (IEnumerable<FileContainerItem>) service.QueryItems(rc, containerId, (string) null, odbId.Value);
    }

    public virtual FileContainerItem GetItem(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      bool throwIfNotFound = true)
    {
      rc = rc.Elevate();
      ITeamFoundationFileContainerService service = rc.GetService<ITeamFoundationFileContainerService>();
      long containerId = this.GetContainerId(rc, service, odbId, throwIfNotFound);
      if (containerId == 0L)
        return (FileContainerItem) null;
      FileContainerItem fileContainerItem = service.QueryItems(rc, containerId, resourceId, odbId.Value).FirstOrDefault<FileContainerItem>();
      return !(fileContainerItem == null & throwIfNotFound) ? fileContainerItem : throw new FileNotFoundException(Resources.Format("FileNotFoundInContainer", (object) odbId.Value, (object) resourceId));
    }

    public virtual IEnumerable<FileContainerItem> GetItems(
      IVssRequestContext rc,
      OdbId odbId,
      IEnumerable<string> resourceIds,
      bool throwIfNotFound = true)
    {
      rc = rc.Elevate();
      ITeamFoundationFileContainerService service = rc.GetService<ITeamFoundationFileContainerService>();
      long containerId = this.GetContainerId(rc, service, odbId, throwIfNotFound);
      return containerId == 0L ? (IEnumerable<FileContainerItem>) null : (IEnumerable<FileContainerItem>) service.QuerySpecificItems(rc, containerId, resourceIds, odbId.Value);
    }

    public virtual Stream GetStream(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      bool throwIfNotFound = true)
    {
      rc = rc.Elevate();
      FileContainerItem fileContainerItem = this.GetItem(rc, odbId, resourceId, throwIfNotFound);
      return fileContainerItem == null ? (Stream) null : this.RetrieveFile(rc, fileContainerItem.FileId, false, out byte[] _, out long _, out CompressionType _);
    }

    public virtual BlobProperties GetProperties(
      IVssRequestContext rc,
      OdbId odbId,
      string blobName,
      bool throwIfNotFound = true)
    {
      rc = rc.Elevate();
      FileContainerItem fromItem = this.GetItem(rc, odbId, blobName, throwIfNotFound);
      return fromItem == null ? (BlobProperties) null : new BlobProperties(fromItem);
    }

    public virtual void PutChunk(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      byte[] contentBlock,
      int contentBlockLength,
      long totalFileLength,
      long offset,
      bool isLastChunk)
    {
      rc = rc.Elevate();
      ITeamFoundationFileContainerService service = rc.GetService<ITeamFoundationFileContainerService>();
      long containerId = this.GetContainerId(rc, service, odbId);
      totalFileLength = isLastChunk ? (long) contentBlockLength + offset : totalFileLength;
      FileContainerItem fileContainerItem;
      if (offset == 0L)
      {
        fileContainerItem = service.CreateItems(rc, containerId, (IList<FileContainerItem>) new FileContainerItem[1]
        {
          new FileContainerItem()
          {
            Path = resourceId,
            ItemType = ContainerItemType.File,
            FileEncoding = 1,
            FileLength = totalFileLength,
            FileHash = Array.Empty<byte>()
          }
        }, odbId.Value).FirstOrDefault<FileContainerItem>();
      }
      else
      {
        fileContainerItem = service.QueryItems(rc, containerId, resourceId, odbId.Value).FirstOrDefault<FileContainerItem>();
        fileContainerItem.FileLength = totalFileLength;
      }
      if (isLastChunk && totalFileLength == 0L)
        return;
      service.UploadFile(rc, fileContainerItem.ContainerId, fileContainerItem, (Stream) new MemoryStream(contentBlock, 0, contentBlockLength), offset, totalFileLength, CompressionType.None, odbId.Value);
    }

    public virtual void PutStream(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      Stream stream,
      long streamLength)
    {
      this.PutStream(rc, odbId, resourceId, stream, streamLength, out int _);
    }

    public void PutStream(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      Stream stream,
      long streamLength,
      out int fileId)
    {
      rc = rc.Elevate();
      ITeamFoundationFileContainerService service = rc.GetService<ITeamFoundationFileContainerService>();
      long containerId = this.GetContainerId(rc, service, odbId, false);
      if (containerId == 0L)
        containerId = service.CreateContainer(rc, this.BuildContainerUri(odbId), "#", "GitStorage", string.Empty, odbId.Value);
      FileContainerItem fileItem = FileContainerProvider.CreateFileItem(resourceId, streamLength);
      FileContainerItem fileContainerItem1 = service.CreateItems(rc, containerId, (IList<FileContainerItem>) new FileContainerItem[1]
      {
        fileItem
      }, odbId.Value).FirstOrDefault<FileContainerItem>();
      FileContainerItem fileContainerItem2 = service.UploadFile(rc, fileContainerItem1.ContainerId, fileContainerItem1, stream, 0L, streamLength, CompressionType.None, odbId.Value);
      fileId = fileContainerItem2.FileId;
    }

    private static FileContainerItem CreateFileItem(string resourceId, long streamLength) => new FileContainerItem()
    {
      Path = resourceId,
      ItemType = ContainerItemType.File,
      FileEncoding = 1,
      FileLength = streamLength,
      FileHash = Array.Empty<byte>()
    };

    public virtual void RenameBlob(
      IVssRequestContext rc,
      OdbId odbId,
      string sourceResourceId,
      string targetResourceId)
    {
      rc = rc.Elevate();
      ITeamFoundationFileContainerService service = rc.GetService<ITeamFoundationFileContainerService>();
      long containerId = this.GetContainerId(rc, service, odbId);
      service.RenameFiles(rc, containerId, (IList<Tuple<string, string>>) new Tuple<string, string>[1]
      {
        new Tuple<string, string>(sourceResourceId, targetResourceId)
      }, odbId.Value);
    }

    public virtual void CopyBlobs(
      IVssRequestContext rc,
      OdbId odbId,
      IList<Tuple<string, string>> sourcesAndTargets,
      bool overwriteExisting)
    {
      rc = rc.Elevate();
      ITeamFoundationFileContainerService service = rc.GetService<ITeamFoundationFileContainerService>();
      long containerId = this.GetContainerId(rc, service, odbId);
      service.CopyFiles(rc, containerId, sourcesAndTargets, odbId.Value, overwriteTargets: overwriteExisting);
    }

    protected long GetContainerId(
      IVssRequestContext rc,
      ITeamFoundationFileContainerService fileContainerService,
      OdbId odbId,
      bool throwOnContainerNotFound = true)
    {
      FileContainerCacheService service = rc.GetService<FileContainerCacheService>();
      long cacheId;
      if (service.TryGetValue(rc, odbId, out cacheId))
        return cacheId;
      Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer = fileContainerService.QueryContainers(rc, (IList<Uri>) new Uri[1]
      {
        this.BuildContainerUri(odbId)
      }, odbId.Value).FirstOrDefault<Microsoft.VisualStudio.Services.FileContainer.FileContainer>();
      if (fileContainer == null)
      {
        if (throwOnContainerNotFound)
          throw new GitStorageContainerNotFoundException(odbId.Value);
        return 0;
      }
      service.Set(rc, odbId, fileContainer.Id);
      return fileContainer.Id;
    }

    protected virtual Stream RetrieveFile(
      IVssRequestContext rc,
      int fileId,
      bool compressOutput,
      out byte[] hashValue,
      out long contentLength,
      out CompressionType compressionType)
    {
      return rc.GetService<TeamFoundationFileService>().RetrieveFile(rc, (long) fileId, false, out hashValue, out contentLength, out compressionType);
    }

    protected Uri BuildContainerUri(OdbId odbId) => new Uri(string.Format("{0}{1}", (object) GitServerUtils.FileContainerArtifactUriPrefix, (object) odbId.Value.ToString("n")));
  }
}
