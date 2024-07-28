// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.FileBlobProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class FileBlobProvider : IBlobProvider, IDisposable
  {
    private const string BlockMetadataExtension = "blocks";
    private static readonly Task CompletedTask = (Task) Task.FromResult<int>(0);
    private static readonly object syncObject = new object();
    private readonly string location;

    public FileBlobProvider(string location)
    {
      Directory.CreateDirectory(location);
      this.location = location;
    }

    public void Dispose() => this.Dispose(true);

    public Task<string> GetBlobEtagAsync(VssRequestPump.Processor processor, BlobIdentifier blobId)
    {
      lock (FileBlobProvider.syncObject)
        return Task.FromResult<string>(this.GetEtag(blobId));
    }

    public Task<long?> GetBlobLengthAsync(VssRequestPump.Processor processor, BlobIdentifier blobId)
    {
      lock (FileBlobProvider.syncObject)
      {
        FileInfo fileInfo = new FileInfo(this.GetPath(blobId));
        return Task.FromResult<long?>(fileInfo.Exists ? new long?(fileInfo.Length) : new long?());
      }
    }

    public Task<DisposableEtagValue<Stream>> GetBlobAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      lock (FileBlobProvider.syncObject)
      {
        string path = this.GetPath(blobId);
        string etag = this.GetEtag(path);
        Stream stream = (Stream) null;
        if (etag != null)
          stream = (Stream) File.OpenRead(path);
        return Task.FromResult<DisposableEtagValue<Stream>>(new DisposableEtagValue<Stream>(stream, etag));
      }
    }

    public Task PutBlobBlockByteArrayAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      ArraySegment<byte> blobBlock,
      string blockName,
      bool useHttpClient)
    {
      lock (FileBlobProvider.syncObject)
      {
        Dictionary<string, FileBlobProvider.FileBlockBlobMetadata> dictionary = this.ReadBlockMetadata(blobId);
        if (!dictionary.ContainsKey(blockName))
          dictionary.Add(blockName, new FileBlobProvider.FileBlockBlobMetadata(blockName, false, dictionary.Count<KeyValuePair<string, FileBlobProvider.FileBlockBlobMetadata>>().ToString()));
        this.WriteFile(this.GetBlockPath(blobId, dictionary[blockName].Extension), blobBlock);
        this.WriteBlockMetadata(blobId, dictionary);
        return FileBlobProvider.CompletedTask;
      }
    }

    public Task<PreauthenticatedUri> GetDownloadUriAsync(
      VssRequestPump.Processor processor,
      BlobIdWithHeaders blobId,
      SASUriExpiry expiry,
      string policyId,
      (string, Guid)[] sasTracing)
    {
      return Task.FromResult<PreauthenticatedUri>(new PreauthenticatedUri(new Uri("file://" + this.GetPath(blobId.BlobId)), EdgeType.NotEdge));
    }

    public Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      VssRequestPump.Processor processor,
      IEnumerable<BlobIdentifier> identifiers,
      SASUriExpiry expiry,
      string policyId,
      (string, Guid)[] sasTracing)
    {
      return Task.FromResult<IDictionary<BlobIdentifier, PreauthenticatedUri>>((IDictionary<BlobIdentifier, PreauthenticatedUri>) identifiers.ToDictionary<BlobIdentifier, BlobIdentifier, PreauthenticatedUri>((Func<BlobIdentifier, BlobIdentifier>) (blobId => blobId), (Func<BlobIdentifier, PreauthenticatedUri>) (blobId => new PreauthenticatedUri(new Uri("file://" + this.GetPath(blobId)), EdgeType.NotEdge))));
    }

    public Task<EtagValue<IList<BlockInfo>>> GetBlockListAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      lock (FileBlobProvider.syncObject)
        return !File.Exists(this.GetBlocksMetadataPath(blobId)) ? Task.FromResult<EtagValue<IList<BlockInfo>>>(new EtagValue<IList<BlockInfo>>((IList<BlockInfo>) null, (string) null)) : Task.FromResult<EtagValue<IList<BlockInfo>>>(new EtagValue<IList<BlockInfo>>((IList<BlockInfo>) this.ReadBlockMetadata(blobId).Select<KeyValuePair<string, FileBlobProvider.FileBlockBlobMetadata>, BlockInfo>((Func<KeyValuePair<string, FileBlobProvider.FileBlockBlobMetadata>, BlockInfo>) (kvp =>
        {
          FileBlobProvider.FileBlockBlobMetadata blockBlobMetadata = kvp.Value;
          FileInfo fileInfo = new FileInfo(this.GetBlockPath(blobId, blockBlobMetadata.Extension));
          return new BlockInfo(blockBlobMetadata.Name)
          {
            Committed = blockBlobMetadata.Committed,
            Length = fileInfo.Length
          };
        })).ToList<BlockInfo>(), this.GetEtag(blobId)));
    }

    public Task<EtagValue<bool>> PutBlockListAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToMatch,
      IEnumerable<string> blockIds)
    {
      lock (FileBlobProvider.syncObject)
      {
        string etag = this.GetEtag(this.GetPath(blobId));
        if (etag != etagToMatch)
          return Task.FromResult<EtagValue<bool>>(new EtagValue<bool>(false, etag));
        Dictionary<string, FileBlobProvider.FileBlockBlobMetadata> metadata = this.ReadBlockMetadata(blobId);
        foreach (KeyValuePair<string, FileBlobProvider.FileBlockBlobMetadata> keyValuePair in metadata)
          keyValuePair.Value.Committed = false;
        List<byte> source = new List<byte>();
        foreach (string blockId in blockIds)
        {
          if (!metadata.ContainsKey(blockId))
            throw new ArgumentException("BlockId " + blockId + " is not in blob.");
          metadata[blockId].Committed = true;
          source.AddRange((IEnumerable<byte>) File.ReadAllBytes(this.GetBlockPath(blobId, metadata[blockId].Extension)));
        }
        using (Stream stream = (Stream) new FileStream(this.GetPath(blobId), FileMode.Create))
          stream.Write(source.ToArray(), 0, source.Count<byte>());
        this.WriteBlockMetadata(blobId, metadata);
        return Task.FromResult<EtagValue<bool>>(new EtagValue<bool>(true, this.GetEtag(this.GetPath(blobId))));
      }
    }

    public Task<EtagValue<bool>> PutBlobByteArrayAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToMatch,
      ArraySegment<byte> data,
      bool useHttpClient = false)
    {
      if (blobId == (BlobIdentifier) null)
        throw new ArgumentNullException(nameof (blobId));
      if (data.Array == null)
        throw new ArgumentNullException(nameof (data));
      lock (FileBlobProvider.syncObject)
      {
        string etag = this.GetEtag(this.GetPath(blobId));
        if (etag != etagToMatch)
          return Task.FromResult<EtagValue<bool>>(new EtagValue<bool>(false, etag));
        foreach (KeyValuePair<string, FileBlobProvider.FileBlockBlobMetadata> keyValuePair in this.ReadBlockMetadata(blobId))
        {
          FileBlobProvider.FileBlockBlobMetadata blockBlobMetadata = keyValuePair.Value;
          File.Delete(this.GetBlockPath(blobId, blockBlobMetadata.Extension));
        }
        File.Delete(this.GetBlocksMetadataPath(blobId));
        this.WriteFile(this.GetPath(blobId), data);
        return Task.FromResult<EtagValue<bool>>(new EtagValue<bool>(true, this.GetEtag(blobId)));
      }
    }

    public IConcurrentIterator<BlobIdentifier> GetBlobIdentifiersConcurrentIterator(
      VssRequestPump.Processor processor)
    {
      lock (FileBlobProvider.syncObject)
        return (IConcurrentIterator<BlobIdentifier>) new ConcurrentIterator<BlobIdentifier>(((IEnumerable<string>) Directory.GetFiles(this.location)).Where<string>((Func<string, bool>) (file => !Path.HasExtension(file))).Select<string, BlobIdentifier>((Func<string, BlobIdentifier>) (path => BlobIdentifier.Deserialize(Path.GetFileName(path)))));
    }

    public Task<EtagValue<bool>> DeleteBlobAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToDelete)
    {
      if (etagToDelete == null)
        throw new ArgumentNullException(nameof (etagToDelete));
      string path = this.GetPath(blobId);
      string blocksMetadataPath = this.GetBlocksMetadataPath(blobId);
      lock (FileBlobProvider.syncObject)
      {
        string etag = this.GetEtag(path);
        if (etagToDelete != etag)
          return Task.FromResult<EtagValue<bool>>(new EtagValue<bool>(false, etag));
        File.Delete(path);
        File.Delete(blocksMetadataPath);
        return Task.FromResult<EtagValue<bool>>(new EtagValue<bool>(true, (string) null));
      }
    }

    public Task<PreauthenticatedUri> GetContainerUri(
      VssRequestPump.Processor processor,
      string policyId)
    {
      throw new NotImplementedException();
    }

    public IConcurrentIterator<IEnumerable<BasicBlobMetadata>> GetBasicBlobMetadataConcurrentIterator(
      VssRequestPump.Processor processor,
      string prefix)
    {
      throw new NotImplementedException();
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    private string GetEtag(BlobIdentifier blobId) => this.GetEtag(this.GetPath(blobId));

    private string GetEtag(string filepath) => !File.Exists(filepath) ? (string) null : File.GetLastWriteTimeUtc(filepath).ToString();

    private string GetPath(BlobIdentifier blobId) => Path.Combine(this.location, blobId.ValueString);

    private string GetBlocksMetadataPath(BlobIdentifier blobId) => Path.Combine(this.location, string.Format("{0}.{1}", (object) blobId.ValueString, (object) "blocks"));

    private string GetBlockPath(BlobIdentifier blobId, string blockExtension) => Path.Combine(this.location, string.Format("{0}.{1}", (object) blobId.ValueString, (object) blockExtension));

    private void WriteFile(string path, ArraySegment<byte> data)
    {
      using (Stream stream = (Stream) new FileStream(path, FileMode.Create))
        stream.Write(data.Array, data.Offset, data.Count);
    }

    private Dictionary<string, FileBlobProvider.FileBlockBlobMetadata> ReadBlockMetadata(
      BlobIdentifier blobId)
    {
      Dictionary<string, FileBlobProvider.FileBlockBlobMetadata> dictionary = new Dictionary<string, FileBlobProvider.FileBlockBlobMetadata>();
      string blocksMetadataPath = this.GetBlocksMetadataPath(blobId);
      if (!File.Exists(blocksMetadataPath))
        return dictionary;
      lock (FileBlobProvider.syncObject)
      {
        Stream stream = (Stream) new FileStream(blocksMetadataPath, FileMode.Open);
        try
        {
          using (StreamReader streamReader = new StreamReader(stream))
          {
            stream = (Stream) null;
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
              FileBlobProvider.FileBlockBlobMetadata blockBlobMetadata = FileBlobProvider.FileBlockBlobMetadata.Parse(line);
              dictionary.Add(blockBlobMetadata.Name, blockBlobMetadata);
            }
          }
        }
        catch
        {
          stream?.Dispose();
          throw;
        }
      }
      return dictionary;
    }

    private void WriteBlockMetadata(
      BlobIdentifier blobId,
      Dictionary<string, FileBlobProvider.FileBlockBlobMetadata> metadata)
    {
      lock (FileBlobProvider.syncObject)
      {
        Stream stream = (Stream) new FileStream(this.GetBlocksMetadataPath(blobId), FileMode.Create);
        try
        {
          using (StreamWriter streamWriter = new StreamWriter(stream))
          {
            stream = (Stream) null;
            foreach (KeyValuePair<string, FileBlobProvider.FileBlockBlobMetadata> keyValuePair in metadata)
              streamWriter.WriteLine(keyValuePair.Value.ToString());
          }
        }
        catch
        {
          stream?.Dispose();
          throw;
        }
      }
    }

    private class FileBlockBlobMetadata
    {
      public static readonly char MetadataSeparator = ' ';
      public readonly string Extension;
      public readonly string Name;
      private static readonly string CommittedSerialization = "1";
      private static readonly string UncommittedSerialization = "0";

      public FileBlockBlobMetadata(string name, bool committed, string extension)
      {
        this.Name = name;
        this.Committed = committed;
        this.Extension = extension;
      }

      public bool Committed { get; set; }

      public static FileBlobProvider.FileBlockBlobMetadata Parse(string line)
      {
        string[] strArray = line.Split(FileBlobProvider.FileBlockBlobMetadata.MetadataSeparator);
        return new FileBlobProvider.FileBlockBlobMetadata(strArray[0], strArray[1].Equals(FileBlobProvider.FileBlockBlobMetadata.CommittedSerialization), strArray[2]);
      }

      public override string ToString() => string.Format("{0}{3}{1}{3}{2}", (object) this.Name, this.Committed ? (object) FileBlobProvider.FileBlockBlobMetadata.CommittedSerialization : (object) FileBlobProvider.FileBlockBlobMetadata.UncommittedSerialization, (object) this.Extension, (object) FileBlobProvider.FileBlockBlobMetadata.MetadataSeparator);
    }
  }
}
