// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.Extensions.LooseFileStorageEndpointIo
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.Extensions
{
  public class LooseFileStorageEndpointIo
  {
    [StaticSafe]
    private static readonly HashedBlobReader s_blobReader = new HashedBlobReader();
    [StaticSafe]
    private static readonly HashedBlobWriter s_blobWriter = new HashedBlobWriter();
    private readonly int m_prefixLength;
    private readonly string m_storePath;
    private IFileSystem m_fileStore;

    public LooseFileStorageEndpointIo(string storePath, int prefixLength, IFileSystem fileSystem)
    {
      this.m_storePath = storePath;
      this.m_prefixLength = prefixLength;
      this.m_fileStore = fileSystem;
    }

    public virtual string GetBlobPath(ContentId contentId) => LooseFileStorageEndpointHelpers.GetObjectPath(this.m_storePath, contentId, this.m_prefixLength);

    public virtual long GetRawSize(ContentId contentId) => new FileInfo(this.GetBlobPath(contentId)).Length;

    public void ReadBlob(ContentId contentId, out byte[] blob, out bool isCompressed)
    {
      using (IFileReader reader = this.OpenStream(contentId))
        LooseFileStorageEndpointIo.s_blobReader.Read(reader, out blob, out isCompressed);
    }

    public void WriteBlob(ContentId contentId, ArraySegment<byte> blob, bool isCompressed)
    {
      using (IFileWriter stream = this.CreateStream(contentId))
        LooseFileStorageEndpointIo.s_blobWriter.Write(stream, blob, isCompressed);
    }

    protected virtual IFileWriter CreateStream(ContentId contentId)
    {
      string blobPath = this.GetBlobPath(contentId);
      return this.m_fileStore.GetDirectory(Path.GetDirectoryName(blobPath) ?? "\\", true).GetFileWriter(Path.GetFileName(blobPath));
    }

    protected virtual IFileReader OpenStream(ContentId contentId)
    {
      string blobPath = this.GetBlobPath(contentId);
      return this.m_fileStore.GetDirectory(Path.GetDirectoryName(blobPath) ?? "\\", true).GetFileReader(Path.GetFileName(blobPath));
    }
  }
}
