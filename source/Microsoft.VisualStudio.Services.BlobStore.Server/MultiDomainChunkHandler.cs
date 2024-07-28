// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainChunkHandler
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class MultiDomainChunkHandler
  {
    private readonly IVssRequestContext requestContext;
    private readonly HttpRequestMessage requestMessage;
    private readonly IDomainId domainId;

    public MultiDomainChunkHandler(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage,
      IDomainId domainId)
    {
      this.requestContext = requestContext;
      this.requestMessage = requestMessage;
      this.domainId = domainId;
    }

    public async Task<HttpResponseMessage> GetChunkAsync(
      string dedupId,
      bool allowEdge,
      bool redirect)
    {
      IDedupStore dedupStore1 = this.requestContext.AllowMultiDomainOperations(this.domainId) ? this.requestContext.GetService<IDedupStore>() : throw new FeatureDisabledException("Multi-Domain");
      ChunkDedupIdentifier chunkId = ChunkDedupIdentifier.Parse(dedupId);
      if (redirect)
      {
        EdgeCache edgeCache = allowEdge ? EdgeCache.Allowed : EdgeCache.NotAllowed;
        IDedupStore dedupStore2 = dedupStore1;
        IVssRequestContext requestContext = this.requestContext;
        IDomainId domainId = this.domainId;
        HashSet<DedupIdentifier> dedupIds = new HashSet<DedupIdentifier>();
        dedupIds.Add((DedupIdentifier) chunkId);
        int num = (int) edgeCache;
        IDictionary<DedupIdentifier, PreauthenticatedUri> source = await dedupStore2.GetUris(requestContext, domainId, (ISet<DedupIdentifier>) dedupIds, (EdgeCache) num).ConfigureAwait(true);
        return new HttpResponseMessage()
        {
          StatusCode = HttpStatusCode.SeeOther,
          Headers = {
            Location = source.Single<KeyValuePair<DedupIdentifier, PreauthenticatedUri>>().Value.NotNullUri
          }
        };
      }
      using (DedupCompressedBuffer buffer = await dedupStore1.GetChunkAsync(this.requestContext, this.domainId, chunkId).ConfigureAwait(true))
        return buffer != null ? new HttpResponseMessage()
        {
          Content = (HttpContent) CompressionHelpers.CreatePossiblyCompressedResponseContent(this.requestMessage, buffer),
          StatusCode = HttpStatusCode.OK
        } : throw DedupNotFoundException.Create(dedupId);
    }

    public async Task<HttpResponseMessage> PutChunkAsync(string dedupId, string keepUntil)
    {
      IDedupStore service = this.requestContext.AllowMultiDomainOperations(this.domainId) ? this.requestContext.GetService<IDedupStore>() : throw new FeatureDisabledException("Multi-Domain");
      ChunkDedupIdentifier parsedDedupId = ChunkDedupIdentifier.Parse(dedupId);
      KeepUntilReceipt dataContractObject;
      using (DedupCompressedBuffer buffer = await CompressionHelpers.GetPossiblyCompressedBufferAsync(this.requestMessage.Content))
        dataContractObject = await service.PutChunkAndKeepUntilReferenceAsync(this.requestContext, this.domainId, parsedDedupId, buffer, new KeepUntilBlobReference(keepUntil)).ConfigureAwait(true);
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<KeepUntilReceipt>(dataContractObject),
        StatusCode = HttpStatusCode.OK
      };
      service = (IDedupStore) null;
      parsedDedupId = (ChunkDedupIdentifier) null;
      return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> PutChunksAsync(string keepUntil)
    {
      IDedupStore service = this.requestContext.AllowMultiDomainOperations(this.domainId) ? this.requestContext.GetService<IDedupStore>() : throw new FeatureDisabledException("Multi-Domain");
      List<Tuple<ChunkDedupIdentifier, int, bool>> chunks = this.requestMessage.Headers.Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (kvp => kvp.Key.StartsWith("X-ms-chunk-", StringComparison.OrdinalIgnoreCase))).Select<KeyValuePair<string, IEnumerable<string>>, Tuple<ChunkDedupIdentifier, int, bool>>((Func<KeyValuePair<string, IEnumerable<string>>, Tuple<ChunkDedupIdentifier, int, bool>>) (header =>
      {
        ChunkDedupIdentifier chunkDedupIdentifier = ChunkDedupIdentifier.Parse(header.Key.Substring("X-ms-chunk-".Length));
        string[] strArray = header.Value.Single<string>().Split('/');
        return Tuple.Create<ChunkDedupIdentifier, int, bool>(chunkDedupIdentifier, int.Parse(strArray[0]), bool.Parse(strArray[1]));
      })).ToList<Tuple<ChunkDedupIdentifier, int, bool>>();
      long num = (long) chunks.Sum<Tuple<ChunkDedupIdentifier, int, bool>>((Func<Tuple<ChunkDedupIdentifier, int, bool>, int>) (c => c.Item2));
      long? contentLength = this.requestMessage.Content.Headers.ContentLength;
      long valueOrDefault = contentLength.GetValueOrDefault();
      if (!(num == valueOrDefault & contentLength.HasValue))
        throw new ArgumentException("Sum of X-ms-chunk headers does not match Content-Length.");
      KeepUntilBlobReference reference = new KeepUntilBlobReference(keepUntil);
      Dictionary<ChunkDedupIdentifier, KeepUntilReceipt> receipts = new Dictionary<ChunkDedupIdentifier, KeepUntilReceipt>();
      using (Stream contentStream = await this.requestMessage.Content.ReadAsStreamAsync())
      {
        using (IPoolHandle<byte[]> chunkBuffer = ChunkerHelper.BorrowChunkBuffer((int) this.requestMessage.Content.Headers.ContentLength.Value))
        {
          foreach (Tuple<ChunkDedupIdentifier, int, bool> chunk in chunks)
          {
            bool isCompressed = chunk.Item3;
            int chunkWireSize = chunk.Item2;
            using (this.requestContext.RequestTimer.CreateTimeToFirstPageExclusionBlock())
              await contentStream.ReadToEntireBufferAsync(new ArraySegment<byte>(chunkBuffer.Value, 0, chunkWireSize), this.requestContext.CancellationToken);
            using (DedupCompressedBuffer buffer = isCompressed ? DedupCompressedBuffer.FromCompressed(new ArraySegment<byte>(chunkBuffer.Value, 0, chunkWireSize)) : DedupCompressedBuffer.FromUncompressed(new ArraySegment<byte>(chunkBuffer.Value, 0, chunkWireSize)))
              receipts.Add(chunk.Item1, await service.PutChunkAndKeepUntilReferenceAsync(this.requestContext, this.domainId, chunk.Item1, buffer, reference).ConfigureAwait(true));
          }
        }
      }
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<Dictionary<ChunkDedupIdentifier, KeepUntilReceipt>>(receipts),
        StatusCode = HttpStatusCode.OK
      };
      service = (IDedupStore) null;
      chunks = (List<Tuple<ChunkDedupIdentifier, int, bool>>) null;
      receipts = (Dictionary<ChunkDedupIdentifier, KeepUntilReceipt>) null;
      return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> TryReferenceAsync(string dedupId, string keepUntil)
    {
      IDedupStore dedupStore = this.requestContext.AllowMultiDomainOperations(this.domainId) ? this.requestContext.GetService<IDedupStore>() : throw new FeatureDisabledException("Multi-Domain");
      ChunkDedupIdentifier chunkDedupIdentifier = ChunkDedupIdentifier.Parse(dedupId);
      IVssRequestContext requestContext = this.requestContext;
      IDomainId domainId = this.domainId;
      ChunkDedupIdentifier chunkId = chunkDedupIdentifier;
      KeepUntilBlobReference keepUntil1 = new KeepUntilBlobReference(keepUntil);
      KeepUntilReceipt dataContractObject = await dedupStore.TryKeepUntilReferenceChunkAsync(requestContext, domainId, chunkId, keepUntil1).ConfigureAwait(true);
      return !(dataContractObject == (KeepUntilReceipt) null) ? new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<KeepUntilReceipt>(dataContractObject),
        StatusCode = HttpStatusCode.OK
      } : throw DedupNotFoundException.Create(dedupId);
    }
  }
}
