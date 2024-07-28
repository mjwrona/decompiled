// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileSystemProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FileSystemProvider : IBlobProvider
  {
    private string m_root;
    private Dictionary<string, Dictionary<string, string>> m_metadataStore;
    private Dictionary<string, Dictionary<string, string>> m_tagsStore;
    internal const string Root = "Root";
    private const string c_blobStore = "BlobStore";

    public void ServiceStart(IVssRequestContext requestContext) => this.ServiceStart(requestContext, (IDictionary<string, string>) null);

    public void ServiceStart(
      IVssRequestContext requestContext,
      IDictionary<string, string> settings)
    {
      string tempPath;
      if (!settings.TryGetValue("Root", out tempPath))
        tempPath = Path.GetTempPath();
      this.m_root = Path.Combine(tempPath, "BlobStore");
      if (!Directory.Exists(this.m_root))
        Directory.CreateDirectory(this.m_root);
      this.m_metadataStore = new Dictionary<string, Dictionary<string, string>>();
      this.m_tagsStore = new Dictionary<string, Dictionary<string, string>>();
    }

    public void ServiceEnd(IVssRequestContext requestContext) => this.m_metadataStore = (Dictionary<string, Dictionary<string, string>>) null;

    public Stream GetStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return (Stream) File.Open(this.GetPathFromResourceId(containerId, resourceId, false), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }

    public void PutChunk(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      byte[] contentBlock,
      int contentBlockLength,
      long compressedLength,
      long offset,
      bool isLastChunk,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      string pathFromResourceId1 = this.GetPathFromResourceId(containerId, resourceId, true);
      using (FileStream fileStream = File.OpenWrite(this.GetPathFromResourceId(containerId, resourceId, true)))
      {
        if (offset > 0L)
          fileStream.Seek(offset, SeekOrigin.Begin);
        else
          fileStream.SetLength(compressedLength);
        fileStream.Write(contentBlock, 0, contentBlockLength);
      }
      if (!isLastChunk)
        return;
      string pathFromResourceId2 = this.GetPathFromResourceId(containerId, resourceId, false);
      File.Move(pathFromResourceId1, pathFromResourceId2);
      this.m_metadataStore[this.GetMetadataOrTagsKey(containerId, resourceId)] = new Dictionary<string, string>(metadata);
    }

    public void PutStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream stream,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      this.PutStream(requestContext, containerId.ToString("n"), resourceId, stream, metadata, clientTimeout);
    }

    public void PutStream(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      Stream stream,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      using (FileStream destination = File.OpenWrite(this.GetPathFromResourceId(containerId, resourceId, false)))
        stream.CopyTo((Stream) destination, (int) Math.Min(stream.Length, 65536L));
      this.m_metadataStore[this.GetMetadataOrTagsKey(containerId, resourceId)] = new Dictionary<string, string>(metadata);
    }

    public void RenameBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string sourceResourceId,
      string targetResourceId,
      TimeSpan? clientTimeout = null)
    {
      this.RenameBlob(requestContext, containerId.ToString("n"), sourceResourceId, targetResourceId, clientTimeout);
    }

    public void RenameBlob(
      IVssRequestContext requestContext,
      string containerId,
      string sourceResourceId,
      string targetResourceId,
      TimeSpan? clientTimeout = null)
    {
      File.Move(this.GetPathFromResourceId(containerId, sourceResourceId, false), this.GetPathFromResourceId(containerId, targetResourceId, false));
    }

    public void WriteBlobMetadata(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>(metadata);
      this.m_metadataStore[this.GetMetadataOrTagsKey(containerId, resourceId)] = dictionary;
    }

    public IDictionary<string, string> ReadBlobMetadata(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.ReadBlobMetadata(requestContext, containerId.ToString("n"), resourceId, clientTimeout);
    }

    public void WriteBlobTags(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      IDictionary<string, string> tags,
      TimeSpan? clientTimeout = null)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>(tags);
      this.m_tagsStore[this.GetMetadataOrTagsKey(containerId, resourceId)] = dictionary;
    }

    public IDictionary<string, string> ReadBlobTags(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId)
    {
      return this.ReadBlobTags(requestContext, containerId.ToString("n"), resourceId);
    }

    public IDictionary<string, string> ReadBlobMetadata(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return (IDictionary<string, string>) new Dictionary<string, string>((IDictionary<string, string>) this.m_metadataStore[this.GetMetadataOrTagsKey(containerId, resourceId)]);
    }

    public IDictionary<string, string> ReadBlobTags(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId)
    {
      return (IDictionary<string, string>) new Dictionary<string, string>((IDictionary<string, string>) this.m_tagsStore[this.GetMetadataOrTagsKey(containerId, resourceId)]);
    }

    public BlobProperties ReadBlobProperties(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      FileInfo fileInfo = new FileInfo(this.GetPathFromResourceId(containerId, resourceId, false));
      return new BlobProperties(resourceId, new DateTimeOffset?((DateTimeOffset) fileInfo.LastWriteTimeUtc), fileInfo.Length);
    }

    public bool DeleteBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.DeleteBlob(requestContext, containerId.ToString("n"), resourceId, clientTimeout);
    }

    public bool DeleteBlob(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      this.m_metadataStore.Remove(this.GetMetadataOrTagsKey(containerId, resourceId));
      string pathFromResourceId = this.GetPathFromResourceId(containerId, resourceId, false);
      if (!File.Exists(pathFromResourceId))
        return false;
      File.Delete(pathFromResourceId);
      return true;
    }

    public List<Guid> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<Guid> resourceIds,
      TimeSpan? clientTimeout = null)
    {
      List<Guid> guidList = new List<Guid>(resourceIds.Count);
      foreach (Guid resourceId in resourceIds)
      {
        if (this.DeleteBlob(requestContext, containerId, resourceId.ToString("n"), clientTimeout))
          guidList.Add(resourceId);
      }
      return guidList;
    }

    public List<string> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<string> resourceIds,
      TimeSpan? clientTimeout = null)
    {
      List<string> stringList = new List<string>(resourceIds.Count);
      foreach (string resourceId in resourceIds)
      {
        if (this.DeleteBlob(requestContext, containerId, resourceId, clientTimeout))
          stringList.Add(resourceId);
      }
      return stringList;
    }

    public void DeleteContainer(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      string containerPath = this.GetContainerPath(containerId, false);
      if (Directory.Exists(containerPath))
        Directory.Delete(containerPath, true);
      foreach (string key in this.m_metadataStore.Keys.ToList<string>())
      {
        if (key.StartsWith(containerId.ToString("D")))
          this.m_metadataStore.Remove(key);
      }
    }

    public void SetBlobHeaders(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      string cacheControl,
      string contentType,
      string contentDisposition,
      string contentEncoding,
      string contentLanguage,
      TimeSpan? clientTimeout = null)
    {
      this.SetBlobHeaders(requestContext, containerId.ToString("n"), resourceId, cacheControl, contentType, contentDisposition, contentEncoding, contentLanguage, clientTimeout);
    }

    public void SetBlobHeaders(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      string cacheControl,
      string contentType,
      string contentDisposition,
      string contentEncoding,
      string contentLanguage,
      TimeSpan? clientTimeout = null)
    {
    }

    internal string GetContainerPath(Guid containerId, bool createIfNotExists) => this.GetContainerPath(containerId.ToString("n"), createIfNotExists);

    private string GetContainerPath(string containerId, bool createIfNotExists)
    {
      string path = Path.Combine(this.m_root, containerId);
      if (createIfNotExists && !Directory.Exists(path))
        Directory.CreateDirectory(path);
      return path;
    }

    public IEnumerable<string> EnumerateBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      string containerPath = this.GetContainerPath(containerId, false);
      return Directory.Exists(containerPath) ? Directory.EnumerateFiles(containerPath, "*.*").Select<string, string>((Func<string, string>) (blobPath => Path.GetFileName(blobPath))) : (IEnumerable<string>) new List<string>(0);
    }

    public RemoteStoreId RemoteStoreId => RemoteStoreId.FileSystem;

    private string GetPathFromResourceId(Guid containerId, string resourceId, bool isTemporary) => this.GetPathFromResourceId(containerId.ToString("n"), resourceId, isTemporary);

    private string GetPathFromResourceId(string containerId, string resourceId, bool isTemporary)
    {
      string str = isTemporary ? ".tmp" : string.Empty;
      string containerPath = this.GetContainerPath(containerId, false);
      if (!Directory.Exists(containerPath))
        Directory.CreateDirectory(containerPath);
      return Path.Combine(containerPath, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) resourceId, (object) str));
    }

    public void DownloadToStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      TimeSpan? clientTimeout = null)
    {
      using (Stream stream = this.GetStream(requestContext, containerId, resourceId, clientTimeout))
        stream.CopyTo(targetStream);
    }

    public void DownloadToStreamLargeBlocks(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      TimeSpan? clientTimeout = null)
    {
      this.DownloadToStream(requestContext, containerId, resourceId, targetStream, clientTimeout);
    }

    public void DownloadToStreamPages(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream)
    {
      this.DownloadToStream(requestContext, containerId, resourceId, targetStream, new TimeSpan?());
    }

    public bool BlobExists(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.BlobExists(requestContext, containerId.ToString("n"), resourceId, clientTimeout);
    }

    public bool BlobExists(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return File.Exists(this.GetPathFromResourceId(containerName, resourceId, false));
    }

    public bool ContainerExists(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      return Directory.Exists(this.GetContainerPath(containerId.ToString("n"), false));
    }

    private string GetMetadataOrTagsKey(Guid containerId, string resourceId) => this.GetMetadataOrTagsKey(containerId.ToString("D"), resourceId);

    private string GetMetadataOrTagsKey(string containerId, string resourceId) => containerId + "_" + resourceId;
  }
}
