// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupStoreClientWithDataport
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.DataDeduplication.Interop;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [CLSCompliant(false)]
  public class DedupStoreClientWithDataport : 
    DedupStoreClient,
    IDedupStoreClientWithDataport,
    IDedupStoreClient
  {
    private readonly bool canRedirect;

    public DedupStoreClientWithDataport(
      IDedupStoreHttpClient client,
      int maxParallelism,
      bool canRedirect = true)
      : this(client, new DedupStoreClientContext(new int?(maxParallelism)), canRedirect)
    {
    }

    public DedupStoreClientWithDataport(
      IDedupStoreHttpClient client,
      DedupStoreClientContext context,
      bool canRedirect = true)
      : this(client, context, ChunkerHelper.DefaultChunkHashType, canRedirect)
    {
    }

    public DedupStoreClientWithDataport(
      IDedupStoreHttpClient client,
      DedupStoreClientContext context,
      HashType hashType,
      bool canRedirect = true)
      : base(client, context, hashType)
    {
      this.canRedirect = canRedirect;
    }

    protected override bool GetNodeCanRedirect => this.canRedirect;

    public async Task<ChunkedFileDownloadResult> DownloadToFileAsync(
      IDedupDataPort dataport,
      DedupIdentifier dedupId,
      string fullPath,
      ulong fileSize,
      GetDedupAsyncFunc dedupFetcher,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      if (fileSize == 0UL)
      {
        // ISSUE: reference to a compiler-generated method
        return await this.\u003C\u003En__1(dedupId, fullPath, dedupFetcher, proxyUri, edgeCache, cancellationToken).ConfigureAwait(false);
      }
      // ISSUE: reference to a compiler-generated method
      dedupFetcher = dedupFetcher ?? (GetDedupAsyncFunc) (ct => this.\u003C\u003En__0(dedupId, ct));
      DedupCompressedBuffer compressedBuffer;
      if (ChunkerHelper.IsChunk(dedupId.AlgorithmId))
      {
        if (dataport == null)
        {
          // ISSUE: reference to a compiler-generated method
          return await this.\u003C\u003En__1(dedupId, fullPath, dedupFetcher, proxyUri, edgeCache, cancellationToken).ConfigureAwait(false);
        }
        DedupHash hash = new DedupHash()
        {
          Hash = dedupId.AlgorithmResult
        };
        if (!await dataport.ContainsChunkAsync(hash))
        {
          MaybeCached<DedupCompressedBuffer> maybeCached = await dedupFetcher(cancellationToken).ConfigureAwait(false);
          compressedBuffer = maybeCached.Value;
          try
          {
            await dataport.InsertChunkAsync(dedupId.CastToChunkDedupIdentifier(), maybeCached.Value).ConfigureAwait(false);
          }
          finally
          {
            compressedBuffer?.Dispose();
          }
          compressedBuffer = (DedupCompressedBuffer) null;
        }
        DedupNode node = new DedupNode(new ChunkInfo(0UL, (uint) fileSize, hash.Hash));
        await dataport.WriteStreamAsync(node, fullPath.Substring(2)).ConfigureAwait(false);
        return new ChunkedFileDownloadResult(node);
      }
      MaybeCached<DedupCompressedBuffer> maybeCached1 = await dedupFetcher(cancellationToken);
      compressedBuffer = maybeCached1.Value;
      try
      {
        DedupNode node = DedupNode.Deserialize(maybeCached1.Value.Uncompressed);
        return new ChunkedFileDownloadResult(await this.DownloadToFileAsync(dataport, node, fullPath, proxyUri, edgeCache, cancellationToken).ConfigureAwait(false));
      }
      finally
      {
        compressedBuffer?.Dispose();
      }
    }

    public async Task<DedupNode> DownloadToFileAsync(
      IDedupDataPort dataport,
      DedupNode node,
      string fullPath,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      DedupStoreClientWithDataport clientWithDataport = this;
      if (dataport == null)
      {
        // ISSUE: explicit non-virtual call
        return await __nonvirtual (clientWithDataport.DownloadToFileAsync(node, fullPath, proxyUri, edgeCache, cancellationToken));
      }
      // ISSUE: explicit non-virtual call
      node = await __nonvirtual (clientWithDataport.GetFilledNodesAsync(node, proxyUri, edgeCache, cancellationToken));
      // ISSUE: explicit non-virtual call
      await __nonvirtual (clientWithDataport.EnsureChunksAreLocalAsync(dataport, node.EnumerateChunkLeafsInOrder().Distinct<DedupNode>(), proxyUri, edgeCache, cancellationToken));
      await dataport.WriteStreamAsync(node, fullPath.Substring(2));
      return node;
    }

    public async Task EnsureChunksAreLocalAsync(
      IDedupDataPort dataPort,
      IEnumerable<DedupNode> chunks,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      await Task.WhenAll(chunks.GetPages<DedupNode>(1000).Select<List<DedupNode>, Task>((Func<List<DedupNode>, Task>) (chunkPage => Task.Run((Func<Task>) (() => this.EnsureChunksAreLocalPageAsync(dataPort, (IReadOnlyList<DedupNode>) chunkPage, proxyUri, edgeCache, cancellationToken))))));
    }

    private async Task EnsureChunksAreLocalPageAsync(
      IDedupDataPort dataPort,
      IReadOnlyList<DedupNode> chunkPage,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      DedupStoreClientWithDataport clientWithDataport = this;
      cancellationToken.ThrowIfCancellationRequested();
      ConcurrentBag<IDisposable> disposables = new ConcurrentBag<IDisposable>();
      try
      {
        foreach (DedupIdentifier key in (IEnumerable<DedupIdentifier>) chunkPage.Select<DedupNode, DedupIdentifier>((Func<DedupNode, DedupIdentifier>) (n => n.GetDedupIdentifier())).OrderBy<DedupIdentifier, DedupIdentifier>((Func<DedupIdentifier, DedupIdentifier>) (n => n)))
        {
          ConcurrentBag<IDisposable> concurrentBag = disposables;
          concurrentBag.Add((IDisposable) await clientWithDataport.currentOperations.Acquire(key));
          concurrentBag = (ConcurrentBag<IDisposable>) null;
        }
        Dictionary<DedupHash, bool> dictionary = await dataPort.ContainsChunksAsync(chunkPage.Select<DedupNode, DedupHash>((Func<DedupNode, DedupHash>) (c => new DedupHash()
        {
          Hash = c.Hash
        })));
        HashSet<DedupIdentifier> misses = new HashSet<DedupIdentifier>();
        foreach (DedupNode dedupNode in (IEnumerable<DedupNode>) chunkPage)
        {
          if (dictionary[new DedupHash()
          {
            Hash = dedupNode.Hash
          }])
            Interlocked.Add(ref clientWithDataport.dedupDownloadBytesSaved, (long) dedupNode.TransitiveContentBytes);
          else
            misses.Add((DedupIdentifier) new ChunkDedupIdentifier(dedupNode.Hash));
        }
        if (misses.Any<DedupIdentifier>())
        {
          Dictionary<DedupIdentifier, GetDedupAsyncFunc> dedupGettersAsync;
          // ISSUE: explicit non-virtual call
          using (await __nonvirtual (clientWithDataport.AcquireParallelismTokenAsync()))
          {
            // ISSUE: explicit non-virtual call
            dedupGettersAsync = await __nonvirtual (clientWithDataport.Client).GetDedupGettersAsync((ISet<DedupIdentifier>) misses, proxyUri, edgeCache, cancellationToken);
          }
          Dictionary<DedupIdentifier, Task<DedupCompressedBuffer>> chunkBufferTasks = dedupGettersAsync.ToDictionary<KeyValuePair<DedupIdentifier, GetDedupAsyncFunc>, DedupIdentifier, Task<DedupCompressedBuffer>>((Func<KeyValuePair<DedupIdentifier, GetDedupAsyncFunc>, DedupIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<DedupIdentifier, GetDedupAsyncFunc>, Task<DedupCompressedBuffer>>) (kvp => Task.Run<DedupCompressedBuffer>((Func<Task<DedupCompressedBuffer>>) (() =>
          {
            DedupCompressedBuffer compressedBuffer;
            using (await this.AcquireParallelismTokenAsync())
            {
              MaybeCached<DedupCompressedBuffer> bytes = await kvp.Value(cancellationToken);
              this.UpdateCountersOfDownload(bytes, DedupNode.NodeType.ChunkLeaf);
              disposables.Add((IDisposable) bytes.Value);
              compressedBuffer = bytes.Value;
            }
            return compressedBuffer;
          }))));
          DedupCompressedBuffer[] compressedBufferArray = await Task.WhenAll<DedupCompressedBuffer>((IEnumerable<Task<DedupCompressedBuffer>>) chunkBufferTasks.Values);
          await dataPort.InsertChunksAsync(chunkBufferTasks.ToDictionary<KeyValuePair<DedupIdentifier, Task<DedupCompressedBuffer>>, ChunkDedupIdentifier, DedupCompressedBuffer>((Func<KeyValuePair<DedupIdentifier, Task<DedupCompressedBuffer>>, ChunkDedupIdentifier>) (kvp => new ChunkDedupIdentifier(new HashAndAlgorithm(kvp.Key.Value))), (Func<KeyValuePair<DedupIdentifier, Task<DedupCompressedBuffer>>, DedupCompressedBuffer>) (kvp => kvp.Value.Result)));
          chunkBufferTasks = (Dictionary<DedupIdentifier, Task<DedupCompressedBuffer>>) null;
        }
        misses = (HashSet<DedupIdentifier>) null;
      }
      finally
      {
        foreach (IDisposable disposable in disposables)
          disposable.Dispose();
      }
    }
  }
}
