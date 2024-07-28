// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.InMemoryBlobProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  internal class InMemoryBlobProvider : IBlobProvider, IDisposable
  {
    private static readonly ConcurrentDictionary<string, Dictionary<BlobIdentifier, BlockBlob>> BlobsByHost = new ConcurrentDictionary<string, Dictionary<BlobIdentifier, BlockBlob>>();
    private readonly string blobNamespace;

    public InMemoryBlobProvider(string blobNamespace) => this.blobNamespace = blobNamespace;

    public static void ResetAllWithPrefix(string blobNamespace)
    {
      foreach (string key in InMemoryBlobProvider.BlobsByHost.Keys.Where<string>((Func<string, bool>) (n => n.StartsWith(blobNamespace))))
        InMemoryBlobProvider.BlobsByHost.TryRemove(key, out Dictionary<BlobIdentifier, BlockBlob> _);
    }

    private Dictionary<BlobIdentifier, BlockBlob> GetBlobs() => InMemoryBlobProvider.BlobsByHost.GetOrAdd(this.blobNamespace, (Func<string, Dictionary<BlobIdentifier, BlockBlob>>) (_ => new Dictionary<BlobIdentifier, BlockBlob>()));

    private PreauthenticatedUri GetInternalDownloadUri(
      Dictionary<BlobIdentifier, BlockBlob> blobsDict,
      BlobIdentifier identifier)
    {
      string str = DateTime.UtcNow.ToString("o");
      BlockBlob blockBlob;
      Uri preauthenticatedNotNullUri;
      if (blobsDict.TryGetValue(identifier, out blockBlob) && blockBlob.ContentBytes != null && blockBlob.ContentBytes.Length < 500)
        preauthenticatedNotNullUri = new Uri("blob://" + identifier.ValueString + "?content=" + blockBlob.ContentBytes.ToHexString() + "&se=" + str);
      else
        preauthenticatedNotNullUri = new Uri("blob://" + identifier.ValueString + "?se=" + str);
      return new PreauthenticatedUri(preauthenticatedNotNullUri, EdgeType.NotEdge);
    }

    public Task<string> GetBlobEtagAsync(VssRequestPump.Processor processor, BlobIdentifier blobId) => Task.Run<string>((Func<string>) (() =>
    {
      Dictionary<BlobIdentifier, BlockBlob> blobs = this.GetBlobs();
      lock (blobs)
      {
        string blobEtagAsync = (string) null;
        BlockBlob blockBlob;
        if (blobs.TryGetValue(blobId, out blockBlob))
          blobEtagAsync = blockBlob.Etag;
        return blobEtagAsync;
      }
    }));

    public Task<long?> GetBlobLengthAsync(VssRequestPump.Processor processor, BlobIdentifier blobId) => Task.Run<long?>((Func<long?>) (() =>
    {
      Dictionary<BlobIdentifier, BlockBlob> blobs = this.GetBlobs();
      lock (blobs)
      {
        long num = 0;
        BlockBlob blockBlob;
        if (blobs.TryGetValue(blobId, out blockBlob))
          num = (long) blockBlob.ContentBytes.Length;
        return new long?(num);
      }
    }));

    public Task<DisposableEtagValue<Stream>> GetBlobAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      return Task.Run<DisposableEtagValue<Stream>>((Func<DisposableEtagValue<Stream>>) (() =>
      {
        Dictionary<BlobIdentifier, BlockBlob> blobs = this.GetBlobs();
        lock (blobs)
        {
          Stream stream = (Stream) null;
          string etag = (string) null;
          BlockBlob blockBlob;
          if (blobs.TryGetValue(blobId, out blockBlob) && blockBlob.ContentBytes != null)
          {
            stream = (Stream) new MemoryStream(blockBlob.ContentBytes);
            etag = blockBlob.Etag;
          }
          return new DisposableEtagValue<Stream>(stream, etag);
        }
      }));
    }

    public Task PutBlobBlockByteArrayAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      ArraySegment<byte> blobBlock,
      string blockName,
      bool useHttpClient)
    {
      return Task.Run((Action) (() =>
      {
        if (blobBlock.Count == 0)
          throw new ArgumentException("Azure does not support blocks of zero length.");
        Dictionary<BlobIdentifier, BlockBlob> blobs = this.GetBlobs();
        lock (blobs)
        {
          BlockBlob blockBlob;
          if (!blobs.TryGetValue(blobId, out blockBlob))
            blockBlob = blobs[blobId] = new BlockBlob();
          blockBlob.Blocks[blockName] = new BlockData()
          {
            Bytes = blobBlock.ToArray<byte>(),
            Info = new BlockInfo(blockName)
            {
              Length = (long) blobBlock.Count,
              Committed = false
            }
          };
        }
      }));
    }

    public Task<PreauthenticatedUri> GetDownloadUriAsync(
      VssRequestPump.Processor processor,
      BlobIdWithHeaders blobId,
      SASUriExpiry expiry,
      string policyId,
      (string, Guid)[] sasTracing)
    {
      return Task.Run<PreauthenticatedUri>((Func<PreauthenticatedUri>) (() => this.GetInternalDownloadUri(this.GetBlobs(), blobId.BlobId)));
    }

    public Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      VssRequestPump.Processor processor,
      IEnumerable<BlobIdentifier> identifiers,
      SASUriExpiry expiry,
      string policyId,
      (string, Guid)[] sasTracing)
    {
      return Task.Run<IDictionary<BlobIdentifier, PreauthenticatedUri>>((Func<IDictionary<BlobIdentifier, PreauthenticatedUri>>) (() =>
      {
        Dictionary<BlobIdentifier, BlockBlob> blobs = this.GetBlobs();
        lock (blobs)
          return (IDictionary<BlobIdentifier, PreauthenticatedUri>) identifiers.ToDictionary<BlobIdentifier, BlobIdentifier, PreauthenticatedUri>((Func<BlobIdentifier, BlobIdentifier>) (blobId => blobId), (Func<BlobIdentifier, PreauthenticatedUri>) (blobId => this.GetInternalDownloadUri(blobs, blobId)));
      }));
    }

    public Task<EtagValue<IList<BlockInfo>>> GetBlockListAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      return Task.Run<EtagValue<IList<BlockInfo>>>((Func<EtagValue<IList<BlockInfo>>>) (() =>
      {
        Dictionary<BlobIdentifier, BlockBlob> blobs = this.GetBlobs();
        lock (blobs)
        {
          BlockBlob blockBlob;
          return !blobs.TryGetValue(blobId, out blockBlob) ? new EtagValue<IList<BlockInfo>>((IList<BlockInfo>) null, (string) null) : new EtagValue<IList<BlockInfo>>((IList<BlockInfo>) blockBlob.Blocks.Values.Select<BlockData, BlockInfo>((Func<BlockData, BlockInfo>) (blockData => blockData.Info)).ToList<BlockInfo>(), blockBlob.Etag);
        }
      }));
    }

    public Task<EtagValue<bool>> PutBlockListAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToMatch,
      IEnumerable<string> blockIds)
    {
      return Task.Run<EtagValue<bool>>((Func<EtagValue<bool>>) (() =>
      {
        Dictionary<BlobIdentifier, BlockBlob> blobs = this.GetBlobs();
        lock (blobs)
        {
          BlockBlob blockBlob;
          if (!blobs.TryGetValue(blobId, out blockBlob))
            throw new InvalidOperationException("Cannot write block list to non-existent blob.");
          if (blockBlob.Etag != etagToMatch)
            return new EtagValue<bool>(false, blockBlob.Etag);
          foreach (string blockId in blockIds)
          {
            if (!blockBlob.Blocks.ContainsKey(blockId))
              return new EtagValue<bool>(false, blockBlob.Etag);
          }
          List<BlockInfo> list = blockBlob.BlockList.ToList<BlockInfo>();
          blockBlob.BlockList.Clear();
          try
          {
            long length = 0;
            foreach (string blockId in blockIds)
            {
              BlockData block = blockBlob.Blocks[blockId];
              length += (long) block.Bytes.Length;
            }
            byte[] numArray = new byte[length];
            long index = 0;
            foreach (string blockId in blockIds)
            {
              BlockData block = blockBlob.Blocks[blockId];
              byte[] bytes = block.Bytes;
              bytes.CopyTo((Array) numArray, index);
              index += (long) bytes.Length;
              block.Info.Committed = true;
              blockBlob.BlockList.Add(block.Info);
            }
            blockBlob.ContentBytes = numArray;
            return new EtagValue<bool>(true, blockBlob.BumpEtag());
          }
          catch
          {
            blockBlob.BlockList.Clear();
            blockBlob.BlockList.AddRange((IEnumerable<BlockInfo>) list);
            throw;
          }
        }
      }));
    }

    public Task<EtagValue<bool>> PutBlobByteArrayAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToMatch,
      ArraySegment<byte> data,
      bool useHttpClient = false)
    {
      return Task.Run<EtagValue<bool>>((Func<EtagValue<bool>>) (() =>
      {
        if (blobId == (BlobIdentifier) null)
          throw new ArgumentNullException(nameof (blobId));
        if (data.Array == null)
          throw new ArgumentNullException(nameof (data));
        Dictionary<BlobIdentifier, BlockBlob> blobs = this.GetBlobs();
        lock (blobs)
        {
          BlockBlob blockBlob;
          if (blobs.TryGetValue(blobId, out blockBlob) && blockBlob.Etag != etagToMatch)
            return new EtagValue<bool>(false, blockBlob.Etag);
          if (blockBlob == null)
          {
            blobs[blobId] = blockBlob = new BlockBlob();
            blockBlob.CreatedDate = new DateTimeOffset?(DateTimeOffset.UtcNow);
          }
          blockBlob.BumpEtag();
          blockBlob.ContentBytes = data.ToArray<byte>();
          return new EtagValue<bool>(true, blockBlob.Etag);
        }
      }));
    }

    public IConcurrentIterator<BlobIdentifier> GetBlobIdentifiersConcurrentIterator(
      VssRequestPump.Processor processor)
    {
      Dictionary<BlobIdentifier, BlockBlob> blobs = this.GetBlobs();
      lock (blobs)
        return (IConcurrentIterator<BlobIdentifier>) new ConcurrentIterator<BlobIdentifier>((IEnumerable<BlobIdentifier>) blobs.Keys.ToList<BlobIdentifier>());
    }

    public Task<EtagValue<bool>> DeleteBlobAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      string etagToDelete)
    {
      return Task.Run<EtagValue<bool>>((Func<EtagValue<bool>>) (() =>
      {
        Dictionary<BlobIdentifier, BlockBlob> blobs = this.GetBlobs();
        lock (blobs)
        {
          BlockBlob blockBlob;
          if (!blobs.TryGetValue(blobId, out blockBlob))
            return new EtagValue<bool>(true, (string) null);
          if (etagToDelete != "*" && etagToDelete != blockBlob.Etag)
            return new EtagValue<bool>(false, blockBlob.Etag);
          blobs.Remove(blobId);
          return new EtagValue<bool>(true, (string) null);
        }
      }));
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
      IEnumerable<BlockBlob> list = (IEnumerable<BlockBlob>) this.GetBlobs().Where<KeyValuePair<BlobIdentifier, BlockBlob>>((Func<KeyValuePair<BlobIdentifier, BlockBlob>, bool>) (kvp => kvp.Key.ValueString.StartsWith(prefix))).Select<KeyValuePair<BlobIdentifier, BlockBlob>, BlockBlob>((Func<KeyValuePair<BlobIdentifier, BlockBlob>, BlockBlob>) (kvp => kvp.Value)).ToList<BlockBlob>();
      lock (list)
        return (IConcurrentIterator<IEnumerable<BasicBlobMetadata>>) new ConcurrentIterator<IEnumerable<BasicBlobMetadata>>((IEnumerable<IEnumerable<BasicBlobMetadata>>) list.Select<BlockBlob, BasicBlobMetadata>((Func<BlockBlob, BasicBlobMetadata>) (b => new BasicBlobMetadata(b.Etag, (long) b.ContentBytes.Length, b.CreatedDate))).GetPages<BasicBlobMetadata>(2));
    }

    public void Dispose()
    {
    }
  }
}
