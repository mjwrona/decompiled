// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.Extensions.WorkItemLooseFileStorageEndpoint
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Compression;
using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.Extensions
{
  [Export(typeof (IStorageEndpoint))]
  public class WorkItemLooseFileStorageEndpoint : 
    IStorageEndpoint,
    IEnumerable<IObjectStoreItem>,
    IEnumerable
  {
    private readonly WorkItemLooseFileStorageEndpointIo m_io;
    private readonly string m_storePath;
    private readonly IFileSystem m_fileSystem;
    private readonly bool m_useCompression;
    [StaticSafe("Grandfathered")]
    private static bool s_isCompressionSupported = Environment.OSVersion.Version >= new Version(6, 2);

    public bool CanAdd => true;

    public bool CanGet => true;

    public WorkItemLooseFileStorageEndpoint()
    {
    }

    public WorkItemLooseFileStorageEndpoint(string storePath, bool useCompression = true)
    {
      this.m_storePath = storePath;
      this.m_fileSystem = (IFileSystem) Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Local.FileSystem.Instance;
      this.m_useCompression = WorkItemLooseFileStorageEndpoint.s_isCompressionSupported & useCompression;
      this.m_io = new WorkItemLooseFileStorageEndpointIo(storePath, this.m_fileSystem);
    }

    public WorkItemLooseFileStorageEndpoint(
      string storePath,
      IFileSystem fileSystem,
      bool useCompression = true)
    {
      this.m_storePath = storePath;
      this.m_fileSystem = fileSystem;
      this.m_useCompression = WorkItemLooseFileStorageEndpoint.s_isCompressionSupported & useCompression;
      this.m_io = new WorkItemLooseFileStorageEndpointIo(storePath, this.m_fileSystem);
    }

    public void Add(ContentId contentId, byte[] blob)
    {
      ArraySegment<byte> blob1 = this.m_useCompression ? Compressor.Compress(CompressorAlgorithm.CompressAlgorithmXpressHuff, blob) : new ArraySegment<byte>(blob);
      this.m_io.WriteBlob(contentId, blob1, this.m_useCompression);
    }

    public bool Contains(ContentId contentId) => this.m_fileSystem.FileExists(this.m_io.GetBlobPath(contentId));

    public IEnumerator<IObjectStoreItem> GetEnumerator()
    {
      foreach (string orderFolder in WorkItemLooseFileStorageEndpointHelpers.OrderFolders(this.m_fileSystem.EnumerateDirectories(this.m_storePath)))
      {
        foreach (ContentId orderFile in WorkItemLooseFileStorageEndpointHelpers.OrderFiles(this.m_fileSystem.GetDirectory(Path.Combine(this.m_storePath, orderFolder), false).Files))
          yield return (IObjectStoreItem) new WorkItemLooseFileStorageEndpoint.ObjectStoreItem(this.m_io, orderFile.ObjectId, orderFile.ContentKey, orderFile.ItemId);
      }
    }

    public string GetItemPath(IObjectStoreItem item)
    {
      if (!(item is WorkItemLooseFileStorageEndpoint.ObjectStoreItem objectStoreItem) || this.m_io != objectStoreItem.Io)
        throw new ArgumentException("Item is not from PartitionedFileObjectStore.");
      return this.m_io.GetBlobPath(new ContentId(item.ObjectId, item.ContentKey));
    }

    public bool TryGet(ContentId contentId, out IObjectStoreItem item)
    {
      if (!this.m_fileSystem.FileExists(this.m_io.GetBlobPath(contentId)))
      {
        item = (IObjectStoreItem) null;
        return false;
      }
      item = (IObjectStoreItem) new WorkItemLooseFileStorageEndpoint.ObjectStoreItem(this.m_io, contentId.ObjectId, contentId.ContentKey, contentId.ItemId);
      return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    private class ObjectStoreItem : IObjectStoreItem
    {
      private byte[] m_blob;
      private bool m_blobCompressed;
      private readonly WorkItemLooseFileStorageEndpointIo m_io;

      public byte[] Blob
      {
        get
        {
          if (this.m_blob == null)
            this.m_blob = this.ReadBlob(true, out bool _);
          else if (this.m_blobCompressed)
          {
            this.m_blob = this.DecompressBlob(this.m_blob);
            this.m_blobCompressed = false;
          }
          return this.m_blob;
        }
      }

      public WorkItemLooseFileStorageEndpointIo Io => this.m_io;

      public Hash ObjectId { get; private set; }

      public string ContentKey { get; private set; }

      public string ItemId { get; private set; }

      public long Size => this.m_blob == null ? this.m_io.GetRawSize(new ContentId(this.ObjectId, this.ContentKey)) : (long) this.m_blob.Length;

      public ObjectStoreItem(WorkItemLooseFileStorageEndpointIo io, Hash objectId)
      {
        this.m_io = io;
        this.ObjectId = objectId;
      }

      public ObjectStoreItem(
        WorkItemLooseFileStorageEndpointIo io,
        Hash objectId,
        string contentKey)
        : this(io, objectId)
      {
        this.ContentKey = contentKey;
      }

      public ObjectStoreItem(
        WorkItemLooseFileStorageEndpointIo io,
        Hash objectId,
        string contentKey,
        string itemId)
        : this(io, objectId, contentKey)
      {
        this.ItemId = itemId;
      }

      public void CopyTo(IFileWriter sink, CompressorAlgorithm outputCompressorAlgorithm)
      {
        bool isCompressed;
        byte[] blob = this.ReadBlob(false, out isCompressed);
        CompressorAlgorithm compressorAlgorithm = isCompressed ? CompressorAlgorithm.CompressAlgorithmXpressHuff : CompressorAlgorithm.None;
        IFileWriter sink1 = sink;
        int inputCompressorAlgorithm = (int) compressorAlgorithm;
        int outputCompressorAlgorithm1 = (int) outputCompressorAlgorithm;
        ObjectStoreHelpers.CopyBlob(blob, sink1, (CompressorAlgorithm) inputCompressorAlgorithm, (CompressorAlgorithm) outputCompressorAlgorithm1);
      }

      public void Preload()
      {
        if (this.m_blob != null)
          return;
        this.m_blob = this.ReadBlob(false, out this.m_blobCompressed);
      }

      private byte[] DecompressBlob(byte[] blob) => Compressor.Decompress(CompressorAlgorithm.CompressAlgorithmXpressHuff, blob);

      private byte[] ReadBlob(bool decompress, out bool isCompressed)
      {
        try
        {
          byte[] blob;
          this.m_io.ReadBlob(new ContentId(this.ObjectId, this.ContentKey, this.ItemId), out blob, out isCompressed);
          if (decompress & isCompressed)
            blob = this.DecompressBlob(blob);
          return blob;
        }
        catch (Exception ex)
        {
          ObjectStoreException objectStoreException = new ObjectStoreException("Unable to read blob.", ex);
          objectStoreException.Data[(object) "Path"] = (object) this.m_io.GetBlobPath(new ContentId(this.ObjectId, this.ContentKey));
          throw objectStoreException;
        }
      }
    }
  }
}
