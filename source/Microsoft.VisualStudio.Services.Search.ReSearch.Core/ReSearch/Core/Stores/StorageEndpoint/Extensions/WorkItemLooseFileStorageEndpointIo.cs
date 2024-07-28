// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.Extensions.WorkItemLooseFileStorageEndpointIo
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Crawler.Entities;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Compression;
using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.Extensions
{
  public class WorkItemLooseFileStorageEndpointIo
  {
    [StaticSafe]
    private static readonly HashedBlobReader s_blobReader = new HashedBlobReader();
    [StaticSafe]
    private static readonly HashedBlobWriter s_blobWriter = new HashedBlobWriter();
    private readonly string m_storePath;
    private readonly IFileSystem m_fileStore;

    public WorkItemLooseFileStorageEndpointIo(string storePath, IFileSystem fileSystem)
    {
      this.m_storePath = storePath;
      this.m_fileStore = fileSystem;
    }

    public virtual string GetBlobPath(ContentId contentId) => WorkItemLooseFileStorageEndpointHelpers.GetObjectPath(this.m_storePath, contentId);

    public virtual long GetRawSize(ContentId contentId) => new FileInfo(this.GetBlobPath(contentId)).Length;

    public void ReadBlob(ContentId contentId, out byte[] blob, out bool isCompressed)
    {
      using (IFileReader reader = this.OpenStream(contentId))
        WorkItemLooseFileStorageEndpointIo.s_blobReader.Read(reader, out blob, out isCompressed);
    }

    public void WriteBlob(ContentId contentId, ArraySegment<byte> blob, bool isCompressed)
    {
      ArraySegment<byte> blob1 = blob;
      string blobPath = this.GetBlobPath(contentId);
      int num = int.Parse(contentId.ItemId, (IFormatProvider) CultureInfo.InvariantCulture);
      if (blob.Array == null || blob.Array.Length == 0)
      {
        this.TraceStoreServiceExceptionalConditions(FormattableString.Invariant(FormattableStringFactory.Create("Null or empty blob passed as an argument to write to store for workItem Id : {0}", (object) num)));
      }
      else
      {
        if (File.Exists(blobPath))
        {
          if (contentId.ContentKey.Equals("snapshot"))
          {
            File.Delete(blobPath);
          }
          else
          {
            if (!contentId.ContentKey.Equals("discussion"))
              throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Workitems are stored as \"snapshot\" OR \"discussion\" content keys. The contentKey received is {0}", (object) contentId.ContentKey));
            byte[] blob2;
            this.ReadBlob(contentId, out blob2, out isCompressed);
            if (blob2 == null || blob2.Length == 0)
            {
              this.TraceStoreServiceExceptionalConditions(FormattableString.Invariant(FormattableStringFactory.Create("Failed to read saved discussions as bytes for workItem Id : {0} from the content sink. ", (object) num)));
              return;
            }
            if (isCompressed)
              blob2 = this.DecompressBlob(blob2);
            if (blob2 == null || blob2.Length == 0)
            {
              this.TraceStoreServiceExceptionalConditions(FormattableString.Invariant(FormattableStringFactory.Create("Failed to decompress saved discussion bytes for workItem Id : {0} from the content sink. ", (object) num)));
              return;
            }
            string str1 = Encoding.UTF8.GetString(blob2);
            if (string.IsNullOrEmpty(str1))
            {
              this.TraceStoreServiceExceptionalConditions(FormattableString.Invariant(FormattableStringFactory.Create("Failed to encode already saved discussions to UTF8 for workItem Id : {0}", (object) num)));
              return;
            }
            List<DiscussionEntity> discussionEntityList = JsonConvert.DeserializeObject<List<DiscussionEntity>>(str1);
            if (discussionEntityList == null || discussionEntityList.Count <= 0)
            {
              this.TraceStoreServiceExceptionalConditions(FormattableString.Invariant(FormattableStringFactory.Create("Failed to deserialize discussions already added to the store for workItem ID : {0}", (object) num)));
              return;
            }
            byte[] bytes1 = this.DecompressBlob(blob.Array);
            if (bytes1 == null || bytes1.Length == 0)
            {
              this.TraceStoreServiceExceptionalConditions(FormattableString.Invariant(FormattableStringFactory.Create("Failed to decompress blob for workItem ID : {0}", (object) num)));
              return;
            }
            string str2 = Encoding.UTF8.GetString(bytes1);
            if (string.IsNullOrEmpty(str2))
            {
              this.TraceStoreServiceExceptionalConditions(FormattableString.Invariant(FormattableStringFactory.Create("Failed to encode discussions(to add) to UTF8 for workItem Id : {0}", (object) num)));
              return;
            }
            List<DiscussionEntity> collection = JsonConvert.DeserializeObject<List<DiscussionEntity>>(str2);
            if (collection == null || collection.Count <= 0)
            {
              this.TraceStoreServiceExceptionalConditions(FormattableString.Invariant(FormattableStringFactory.Create("Failed to deserialize discussions(to be added) to the store for workItem ID : {0}", (object) num)));
              return;
            }
            discussionEntityList.AddRange((IEnumerable<DiscussionEntity>) collection);
            byte[] bytes2 = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) discussionEntityList));
            blob1 = isCompressed ? Compressor.Compress(CompressorAlgorithm.CompressAlgorithmXpressHuff, bytes2) : new ArraySegment<byte>(bytes2);
          }
        }
        using (IFileWriter stream = this.CreateStream(contentId))
          WorkItemLooseFileStorageEndpointIo.s_blobWriter.Write(stream, blob1, isCompressed);
      }
    }

    private void TraceStoreServiceExceptionalConditions(string message) => Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080560, "Indexing Pipeline", "Store", message);

    private byte[] DecompressBlob(byte[] blob) => Compressor.Decompress(CompressorAlgorithm.CompressAlgorithmXpressHuff, blob);

    protected virtual IFileWriter CreateStream(ContentId contentId)
    {
      string blobPath = this.GetBlobPath(contentId);
      return this.m_fileStore.GetDirectory(Path.GetDirectoryName(blobPath) ?? "\\", true).GetFileWriter(Path.GetFileName(blobPath));
    }

    protected virtual IFileReader OpenStream(ContentId contentId)
    {
      string blobPath = this.GetBlobPath(contentId);
      return this.m_fileStore.GetDirectory(Path.GetDirectoryName(blobPath) ?? "\\", false).GetFileReader(Path.GetFileName(blobPath));
    }
  }
}
