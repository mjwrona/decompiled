// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupStoreClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using BuildXL.Cache.ContentStore.Interfaces.Utils;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common.Telemetry;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [CLSCompliant(false)]
  public class DedupStoreClient : IDedupStoreClient
  {
    protected readonly LockSet<DedupIdentifier> currentOperations = new LockSet<DedupIdentifier>();
    protected readonly SemaphoreSlim maxParallelism;
    protected readonly SemaphoreSlim maxWriterParallelism;
    protected long physicalContentBytesDownloaded;
    protected long compressionDownloadBytesSaved;
    protected long dedupDownloadBytesSaved;
    protected long chunksDownloaded;
    protected long nodesDownloaded;

    public IDedupStoreHttpClient Client { get; }

    public int MaxParallelismCount => this.ClientContext.MaxParallelismCount;

    public bool DisableHardLinks => this.ClientContext.DisableHardLinks;

    public HashType HashType { get; } = ChunkerHelper.DefaultChunkHashType;

    public DedupStoreClientContext ClientContext { get; }

    public DedupDownloadStatistics DownloadStatistics => new DedupDownloadStatistics(this.chunksDownloaded, this.compressionDownloadBytesSaved, this.dedupDownloadBytesSaved, this.nodesDownloaded, this.physicalContentBytesDownloaded);

    public DedupStoreClient(IDedupStoreHttpClient client, int maxParallelism)
      : this(client, new DedupStoreClientContext(new int?(maxParallelism)))
    {
    }

    public DedupStoreClient(IDedupStoreHttpClient client, int maxParallelism, HashType hashType)
      : this(client, new DedupStoreClientContext(new int?(maxParallelism)), hashType)
    {
    }

    public DedupStoreClient(IDedupStoreHttpClient client, DedupStoreClientContext clientContext)
      : this(client, clientContext, ChunkerHelper.DefaultChunkHashType)
    {
    }

    public DedupStoreClient(
      IDedupStoreHttpClient client,
      DedupStoreClientContext clientContext,
      HashType hashType)
    {
      this.Client = client;
      this.HashType = hashType;
      this.ClientContext = clientContext ?? throw new ArgumentNullException("The client context must not be null");
      this.maxParallelism = new SemaphoreSlim(this.ClientContext.MaxParallelismCount, this.ClientContext.MaxParallelismCount);
      this.maxWriterParallelism = new SemaphoreSlim(this.ClientContext.MaxParallelismCount, this.ClientContext.MaxParallelismCount);
    }

    public void ResetDownloadStatistics()
    {
      this.physicalContentBytesDownloaded = 0L;
      this.compressionDownloadBytesSaved = 0L;
      this.dedupDownloadBytesSaved = 0L;
      this.chunksDownloaded = 0L;
      this.nodesDownloaded = 0L;
    }

    public async Task<IDisposable> AcquireParallelismTokenAsync() => (IDisposable) await SemaphoreSlimToken.Wait(this.maxParallelism);

    public IDedupUploadSession CreateUploadSession(
      KeepUntilBlobReference keepUntilReference,
      IAppTraceSource tracer,
      IFileSystem fileSystem)
    {
      return (IDedupUploadSession) new DedupStoreClient.UploadSession((IDedupStoreClient) this, keepUntilReference, tracer, fileSystem);
    }

    protected virtual bool GetNodeCanRedirect => true;

    public async Task<MaybeCached<DedupCompressedBuffer>> GetNodeAsync(
      NodeDedupIdentifier nodeId,
      CancellationToken cancellationToken)
    {
      MaybeCached<DedupCompressedBuffer> nodeAsync1;
      using (await this.currentOperations.Acquire((DedupIdentifier) nodeId))
      {
        using (await this.AcquireParallelismTokenAsync())
        {
          MaybeCached<DedupCompressedBuffer> nodeAsync2 = await this.Client.GetNodeAsync(nodeId, this.GetNodeCanRedirect, cancellationToken);
          this.UpdateCountersOfDownload(nodeAsync2, DedupNode.NodeType.InnerNode);
          nodeAsync1 = nodeAsync2;
        }
      }
      return nodeAsync1;
    }

    public async Task<MaybeCached<DedupCompressedBuffer>> GetChunkAsync(
      ChunkDedupIdentifier chunkId,
      CancellationToken cancellationToken)
    {
      MaybeCached<DedupCompressedBuffer> chunkAsync1;
      using (await this.currentOperations.Acquire((DedupIdentifier) chunkId))
      {
        using (await this.AcquireParallelismTokenAsync())
        {
          MaybeCached<DedupCompressedBuffer> chunkAsync2 = await this.Client.GetChunkAsync(chunkId, this.GetNodeCanRedirect, cancellationToken);
          this.UpdateCountersOfDownload(chunkAsync2, DedupNode.NodeType.ChunkLeaf);
          chunkAsync1 = chunkAsync2;
        }
      }
      return chunkAsync1;
    }

    public Task<MaybeCached<DedupCompressedBuffer>> GetDedupAsync(
      DedupIdentifier dedupId,
      CancellationToken cancellationToken)
    {
      return ChunkerHelper.IsChunk(dedupId.AlgorithmId) ? this.GetChunkAsync(dedupId.CastToChunkDedupIdentifier(), cancellationToken) : this.GetNodeAsync(dedupId.CastToNodeDedupIdentifier(), cancellationToken);
    }

    public async Task<ChunkedFileDownloadResult> DownloadToFileAsync(
      DedupIdentifier dedupId,
      string fullPath,
      GetDedupAsyncFunc dedupFetcher,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      if (dedupFetcher == null)
        dedupFetcher = (await this.Client.GetDedupGettersAsync((ISet<DedupIdentifier>) new HashSet<DedupIdentifier>()
        {
          dedupId
        }, proxyUri, edgeCache, cancellationToken))[dedupId];
      MaybeCached<DedupCompressedBuffer> dedupBuffer = await dedupFetcher(cancellationToken);
      if (dedupBuffer.Value == null)
        throw DedupNotFoundException.Create(dedupId);
      Func<MaybeCached<DedupCompressedBuffer>, CancellationToken, Task> chunkWriter = (Func<MaybeCached<DedupCompressedBuffer>, CancellationToken, Task>) ((b, ct) => this.WriteChunkToFileAsync(fullPath, b, ct));
      Func<DedupNode, Uri, EdgeCache, CancellationToken, Task> nodeWriter = (Func<DedupNode, Uri, EdgeCache, CancellationToken, Task>) ((dn, u, e, ct) => (Task) this.DownloadToFileAsync(dn, fullPath, u, e, ct));
      return new ChunkedFileDownloadResult(await this.DownloadToDestinationAsync(dedupBuffer, dedupId, proxyUri, edgeCache, chunkWriter, nodeWriter, (Action<ulong>) (size => { }), cancellationToken));
    }

    public async Task DownloadToStreamAsync(
      DedupIdentifier dedupId,
      Stream stream,
      Uri proxyUri,
      EdgeCache edgeCache,
      Action<ulong> traceDownloadProgressFunc,
      Action<ulong> traceFinalizeDownloadProgressFunc,
      CancellationToken cancellationToken)
    {
      MaybeCached<DedupCompressedBuffer> dedupAsync = await this.GetDedupAsync(dedupId, cancellationToken);
      if (dedupAsync.Value == null)
        throw DedupNotFoundException.Create(dedupId);
      Func<MaybeCached<DedupCompressedBuffer>, CancellationToken, Task> chunkWriter = (Func<MaybeCached<DedupCompressedBuffer>, CancellationToken, Task>) ((b, ct) => this.WriteChunkToStreamAsync(stream, b, ct));
      Func<DedupNode, Uri, EdgeCache, CancellationToken, Task> nodeWriter = (Func<DedupNode, Uri, EdgeCache, CancellationToken, Task>) ((dn, u, e, ct) => (Task) this.DownloadToStreamAsync(dn, stream, u, e, ct));
      traceFinalizeDownloadProgressFunc((await this.DownloadToDestinationAsync(dedupAsync, dedupId, proxyUri, edgeCache, chunkWriter, nodeWriter, traceDownloadProgressFunc, cancellationToken)).TransitiveContentBytes);
    }

    private async Task<DedupNode> DownloadToDestinationAsync(
      MaybeCached<DedupCompressedBuffer> dedupBuffer,
      DedupIdentifier dedupId,
      Uri proxyUri,
      EdgeCache edgeCache,
      Func<MaybeCached<DedupCompressedBuffer>, CancellationToken, Task> chunkWriter,
      Func<DedupNode, Uri, EdgeCache, CancellationToken, Task> nodeWriter,
      Action<ulong> traceDownloadProgressFunc,
      CancellationToken cancellationToken)
    {
      using (dedupBuffer.Value)
      {
        if (ChunkerHelper.IsChunk(dedupId.AlgorithmId))
        {
          this.UpdateCountersOfDownload(dedupBuffer, DedupNode.NodeType.ChunkLeaf);
          ulong chunkSize = (ulong) dedupBuffer.Value.Uncompressed.Count;
          traceDownloadProgressFunc(chunkSize);
          await chunkWriter(dedupBuffer, cancellationToken);
          return new DedupNode(new ChunkInfo(0UL, (uint) chunkSize, dedupId.AlgorithmResult));
        }
        DedupNode node = DedupNode.Deserialize(dedupBuffer.Value.Uncompressed.CreateCopy<byte>());
        traceDownloadProgressFunc(node.TransitiveContentBytes);
        await nodeWriter(node, proxyUri, edgeCache, cancellationToken);
        return node;
      }
    }

    protected async Task WriteChunkToFileAsync(
      string fullPath,
      MaybeCached<DedupCompressedBuffer> dedupBuffer,
      CancellationToken cancellationToken)
    {
      using (FileStream file = this.OpenForWrite(fullPath))
        await this.WriteChunkToStreamAsync((Stream) file, dedupBuffer, cancellationToken);
    }

    protected async Task WriteChunkToStreamAsync(
      Stream stream,
      MaybeCached<DedupCompressedBuffer> dedupBuffer,
      CancellationToken cancellationToken)
    {
      ArraySegment<byte> uncompressed = dedupBuffer.Value.Uncompressed;
      await stream.WriteAsync(uncompressed.Array, uncompressed.Offset, uncompressed.Count, cancellationToken);
    }

    protected FileStream OpenForWrite(string fullPath, FileOptions fileOptions = FileOptions.None)
    {
      Func<FileStream> func = (Func<FileStream>) (() => FileStreamUtils.OpenFileStreamForAsync(fullPath, FileMode.Create, FileAccess.Write, FileShare.Write, fileOptions));
      try
      {
        return func();
      }
      catch (UnauthorizedAccessException ex)
      {
        File.Delete(fullPath);
        return func();
      }
    }

    private Task<DedupNode> DownloadToStreamAsync(
      DedupNode node1,
      Stream stream,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      long startOffsetOfNextChunk = 0;
      int writesInProgress = 0;
      uint expectedNextChunkIndex = 0;
      return this.DownloadToWriterAsync(node1, 1, true, (Func<DedupNode, uint, long, ArraySegment<byte>, Task>) (async (node2, chunkIndex, fileOffset, buffer) =>
      {
        Func<string> func = (Func<string>) (() => string.Format("Chunk #{0} at offset {1} with hash: {2} and size {3}", (object) chunkIndex, (object) fileOffset, (object) node2.GetChunkIdentifier(), (object) buffer.Count));
        if (1 != Interlocked.Increment(ref writesInProgress))
          throw new InvalidOperationException(func() + ": Writes to a stream should not be happening in parallel.");
        if ((int) expectedNextChunkIndex != (int) chunkIndex)
          throw new InvalidOperationException(string.Format("{0}: Expected chunk #{1}.", (object) func(), (object) expectedNextChunkIndex));
        ++expectedNextChunkIndex;
        if (fileOffset != startOffsetOfNextChunk)
          throw new InvalidOperationException(string.Format("{0}: Excepted offset {1}.", (object) func(), (object) startOffsetOfNextChunk));
        startOffsetOfNextChunk += (long) buffer.Count;
        await stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count, cancellationToken);
        Interlocked.Decrement(ref writesInProgress);
      }), proxyUri, edgeCache, cancellationToken);
    }

    private async Task<DedupNode> DownloadToWriterAsync(
      DedupNode fileNode,
      int writerParallelism,
      bool ensureOrdered,
      Func<DedupNode, uint, long, ArraySegment<byte>, Task> writer,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      int writeCapacity = Math.Max(this.ClientContext.WriterCapacity, writerParallelism);
      int downloadCapacity = Math.Max(this.ClientContext.DownloadCapacity, this.ClientContext.MaxParallelismCount);
      bool ensureUriOrder = ((int) this.ClientContext.EnsureUriOrdered ?? (ensureOrdered ? 1 : 0)) != 0;
      fileNode = await this.GetFilledNodesAsync(fileNode, proxyUri, edgeCache, cancellationToken);
      Dictionary<DedupNode, int> source = new Dictionary<DedupNode, int>();
      foreach (DedupNode key in fileNode.EnumerateChunkLeafsInOrder())
      {
        if (source.ContainsKey(key))
          ++source[key];
        else
          source.Add(key, 1);
      }
      ulong cacheBytesUsed = 0;
      Dictionary<DedupNode, int> mostFrequentChunksWithMultipleCopies = source.Where<KeyValuePair<DedupNode, int>>((Func<KeyValuePair<DedupNode, int>, bool>) (k => k.Value > 1)).OrderByDescending<KeyValuePair<DedupNode, int>, int>((Func<KeyValuePair<DedupNode, int>, int>) (k => k.Value)).TakeWhile<KeyValuePair<DedupNode, int>>((Func<KeyValuePair<DedupNode, int>, int, bool>) ((k, i) =>
      {
        cacheBytesUsed += 2UL * (ulong) ChunkerHelper.GetChunkBufferSize((int) k.Key.TransitiveContentBytes);
        return cacheBytesUsed <= (ulong) this.ClientContext.ChunkCacheSizeInMegabytes * 1024UL * 1024UL;
      })).ToDictionary<KeyValuePair<DedupNode, int>, DedupNode, int>((Func<KeyValuePair<DedupNode, int>, DedupNode>) (k => k.Key), (Func<KeyValuePair<DedupNode, int>, int>) (v => v.Value));
      Func<IEnumerable<(DedupNode, uint, long)>, Task<IEnumerable<(DedupNode, uint, long, GetDedupAsyncFunc)>>> transform1 = (Func<IEnumerable<(DedupNode, uint, long)>, Task<IEnumerable<(DedupNode, uint, long, GetDedupAsyncFunc)>>>) (async chunks =>
      {
        HashSet<DedupIdentifier> distinctChunkIds = new HashSet<DedupIdentifier>((IEnumerable<DedupIdentifier>) chunks.Select<(DedupNode, uint, long), ChunkDedupIdentifier>((Func<(DedupNode, uint, long), ChunkDedupIdentifier>) (t => t.node.GetChunkIdentifier())));
        Dictionary<DedupIdentifier, GetDedupAsyncFunc> fetchers;
        using (await this.AcquireParallelismTokenAsync())
          fetchers = await this.Client.GetDedupGettersAsync((ISet<DedupIdentifier>) distinctChunkIds, proxyUri, edgeCache, cancellationToken);
        IEnumerable<(DedupNode, uint, long, GetDedupAsyncFunc)> writerAsync = chunks.Select<(DedupNode, uint, long), (DedupNode, uint, long, GetDedupAsyncFunc)>((Func<(DedupNode, uint, long), (DedupNode, uint, long, GetDedupAsyncFunc)>) (chunk => (chunk.node, chunk.chunkIndex, chunk.fileOffset, fetchers[(DedupIdentifier) chunk.node.GetChunkIdentifier()])));
        distinctChunkIds = (HashSet<DedupIdentifier>) null;
        return writerAsync;
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions1 = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions1.BoundedCapacity = downloadCapacity + 2;
      dataflowBlockOptions1.MaxDegreeOfParallelism = this.ClientContext.MaxParallelismCount;
      dataflowBlockOptions1.CancellationToken = cancellationToken;
      dataflowBlockOptions1.EnsureOrdered = ensureUriOrder;
      TransformManyBlock<IEnumerable<(DedupNode, uint, long)>, (DedupNode, uint, long, GetDedupAsyncFunc)> targetBlock = NonSwallowingTransformManyBlock.Create<IEnumerable<(DedupNode, uint, long)>, (DedupNode, uint, long, GetDedupAsyncFunc)>(transform1, dataflowBlockOptions1);
      DedupNode writerAsync1;
      using (RunOnceDisposable<DedupNode, DedupStoreClient.DownloadedChunk> downloadEachChunkOnce = new RunOnceDisposable<DedupNode, DedupStoreClient.DownloadedChunk>(true))
      {
        Func<(DedupNode, uint, long, GetDedupAsyncFunc), Task<DedupStoreClient.DownloadedChunk>> transform2 = (Func<(DedupNode, uint, long, GetDedupAsyncFunc), Task<DedupStoreClient.DownloadedChunk>>) (async chunk =>
        {
          Func<Task<DedupStoreClient.DownloadedChunk>> getChunkAsync = (Func<Task<DedupStoreClient.DownloadedChunk>>) (async () =>
          {
            MaybeCached<DedupCompressedBuffer> maybeCached = await chunk.fetcher(cancellationToken);
            this.UpdateCountersOfDownload(maybeCached, DedupNode.NodeType.ChunkLeaf);
            if ((long) maybeCached.Value.Uncompressed.Count != (long) chunk.node.TransitiveContentBytes)
              throw new EndOfStreamException(string.Format("Dedup size does not match the downloaded size. DedupId: {0}", (object) chunk.node.GetDedupIdentifier()));
            if ((this.ClientContext.ChunkValidationLevel == ChunkValidationLevel.ChunkOnly || this.ClientContext.ChunkValidationLevel == ChunkValidationLevel.ChunkAndFile) && !this.IsCacheEnabled(this.Client))
            {
              byte[] data = await ChunkerHelper.CreateHashSingleChunkFromStreamAsync((Stream) maybeCached.Value.Uncompressed.AsMemoryStream(), cancellationToken, true).ConfigureAwait(true);
              if (data.ToHexString() != chunk.node.HashString)
                throw new InvalidDataException("The hash of chunks don't match, Calculated hash: " + data.ToHexString() + " Chunk hash: " + chunk.node.Hash.ToHexString() + " ");
            }
            DedupStoreClient.DownloadedChunk writerAsync3 = new DedupStoreClient.DownloadedChunk(chunk.node, chunk.chunkIndex, chunk.fileOffset, maybeCached.Value);
            maybeCached = new MaybeCached<DedupCompressedBuffer>();
            return writerAsync3;
          });
          using (await this.AcquireParallelismTokenAsync())
          {
            if (!mostFrequentChunksWithMultipleCopies.ContainsKey(chunk.node))
              return await getChunkAsync();
            DedupStoreClient.DownloadedChunk downloadedChunk = await downloadEachChunkOnce.RunOnceAsync(chunk.node, getChunkAsync);
            this.UpdateCountersOfDownload(new MaybeCached<DedupCompressedBuffer>(downloadedChunk.Buffer, true), DedupNode.NodeType.ChunkLeaf);
            return new DedupStoreClient.DownloadedChunk(chunk.node, chunk.chunkIndex, chunk.fileOffset, downloadedChunk.Buffer);
          }
        });
        ExecutionDataflowBlockOptions dataflowBlockOptions2 = new ExecutionDataflowBlockOptions();
        dataflowBlockOptions2.BoundedCapacity = downloadCapacity + 2;
        dataflowBlockOptions2.MaxDegreeOfParallelism = this.ClientContext.MaxParallelismCount;
        dataflowBlockOptions2.CancellationToken = cancellationToken;
        dataflowBlockOptions2.EnsureOrdered = ensureOrdered;
        TransformBlock<(DedupNode, uint, long, GetDedupAsyncFunc), DedupStoreClient.DownloadedChunk> target = NonSwallowingTransformBlock.Create<(DedupNode, uint, long, GetDedupAsyncFunc), DedupStoreClient.DownloadedChunk>(transform2, dataflowBlockOptions2);
        targetBlock.LinkTo((ITargetBlock<(DedupNode, uint, long, GetDedupAsyncFunc)>) target, new DataflowLinkOptions()
        {
          PropagateCompletion = true
        });
        Func<DedupStoreClient.DownloadedChunk, Task> action = (Func<DedupStoreClient.DownloadedChunk, Task>) (async chunk =>
        {
          try
          {
            SemaphoreSlimToken semaphoreSlimToken = await SemaphoreSlimToken.Wait(this.maxWriterParallelism);
            try
            {
              chunk.Buffer.AssertValid();
              await writer(chunk.Node, chunk.ChunkIndex, chunk.FileOffset, chunk.Buffer.Uncompressed);
            }
            finally
            {
              semaphoreSlimToken.Dispose();
            }
            semaphoreSlimToken = new SemaphoreSlimToken();
          }
          finally
          {
            if (!mostFrequentChunksWithMultipleCopies.ContainsKey(chunk.Node))
              chunk.Buffer.Dispose();
          }
        });
        ExecutionDataflowBlockOptions dataflowBlockOptions3 = new ExecutionDataflowBlockOptions();
        dataflowBlockOptions3.BoundedCapacity = writeCapacity + 1;
        dataflowBlockOptions3.MaxDegreeOfParallelism = writerParallelism;
        dataflowBlockOptions3.CancellationToken = cancellationToken;
        dataflowBlockOptions3.EnsureOrdered = ensureOrdered;
        ActionBlock<DedupStoreClient.DownloadedChunk> actionBlock = NonSwallowingActionBlock.Create<DedupStoreClient.DownloadedChunk>(action, dataflowBlockOptions3);
        target.LinkTo((ITargetBlock<DedupStoreClient.DownloadedChunk>) actionBlock, new DataflowLinkOptions()
        {
          PropagateCompletion = true
        });
        long fileOffset = 0;
        uint chunkIndex = 0;
        IEnumerable<IEnumerable<(DedupNode, uint, long)>> inputs = fileNode.EnumerateChunkLeafsInOrder().GetPages<DedupNode>(this.ClientContext.DownloadPageSize).Select<List<DedupNode>, IEnumerable<(DedupNode, uint, long)>>((Func<List<DedupNode>, IEnumerable<(DedupNode, uint, long)>>) (page =>
        {
          List<(DedupNode, uint, long)> writerAsync4 = new List<(DedupNode, uint, long)>(page.Count);
          foreach (DedupNode dedupNode in page)
          {
            writerAsync4.Add((dedupNode, chunkIndex, fileOffset));
            fileOffset += (long) dedupNode.TransitiveContentBytes;
            ++chunkIndex;
          }
          return (IEnumerable<(DedupNode, uint, long)>) writerAsync4;
        }));
        await targetBlock.SendAllAndCompleteAsync<IEnumerable<(DedupNode, uint, long)>, DedupStoreClient.DownloadedChunk>(inputs, (ITargetBlock<DedupStoreClient.DownloadedChunk>) actionBlock, cancellationToken);
        writerAsync1 = fileNode;
      }
      return writerAsync1;
    }

    public async Task<DedupNode> DownloadToFileAsync(
      DedupNode node,
      string fullPath,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      node = await this.GetFilledNodesAsync(node, proxyUri, edgeCache, cancellationToken);
      if (this.DisableHardLinks)
        await this.DownloadAndMoveTempFile(node, fullPath, proxyUri, edgeCache, cancellationToken);
      else
        await this.DownloadAndHardlinkTempFile(node, fullPath, proxyUri, edgeCache, cancellationToken);
      return node;
    }

    private async Task DownloadAndHardlinkTempFile(
      DedupNode node1,
      string fullPath,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      string tempFilePath = Path.Combine(Path.GetDirectoryName(fullPath), Guid.NewGuid().ToString() + ".tmp");
      using (FileStream wholeFile = this.OpenForWrite(tempFilePath, FileOptions.DeleteOnClose))
      {
        wholeFile.SetLength((long) node1.TransitiveContentBytes);
        DedupNode writerAsync = await this.DownloadToWriterAsync(node1, Environment.ProcessorCount, false, (Func<DedupNode, uint, long, ArraySegment<byte>, Task>) ((node2, _chunkIndex, fileOffset, buffer) => AsyncFile.WriteAsync(wholeFile, fileOffset, buffer).EnforceCancellation(cancellationToken, (Func<string>) (() => "Timed out waiting for WriteAsync to '" + wholeFile.Name + "'."), "D:\\a\\_work\\1\\s\\BlobStore\\Client\\WebApi\\DedupStoreClient.cs", nameof (DownloadAndHardlinkTempFile), 1144)), proxyUri, edgeCache, cancellationToken);
        FileLink.CreateHardLinkStatus hardLink = FileLink.CreateHardLink(tempFilePath, fullPath);
        switch (hardLink)
        {
          case FileLink.CreateHardLinkStatus.Success:
            break;
          case FileLink.CreateHardLinkStatus.FailedAccessDenied:
            throw new UnauthorizedAccessException("Access denied: " + fullPath);
          case FileLink.CreateHardLinkStatus.FailedInvalidName:
            throw new IOException("Hard linking failed due to invalid name: " + fullPath + ".");
          case FileLink.CreateHardLinkStatus.FailedPathNotFound:
            throw new DirectoryNotFoundException("Path not found! \n Path: " + tempFilePath);
          case FileLink.CreateHardLinkStatus.FailedFileNotFound:
            throw new FileNotFoundException("File not found! \n Path: " + tempFilePath);
          default:
            throw new IOException("Hard linking failed! \n Status: " + hardLink.ToString() + " \n Path: " + fullPath);
        }
      }
      tempFilePath = (string) null;
    }

    private async Task DownloadAndMoveTempFile(
      DedupNode node1,
      string fullPath,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      string tempFilePath = Path.Combine(Path.GetDirectoryName(fullPath), Guid.NewGuid().ToString() + ".tmp");
      try
      {
        using (FileStream wholeFile = this.OpenForWrite(tempFilePath))
        {
          wholeFile.SetLength((long) node1.TransitiveContentBytes);
          DedupNode writerAsync = await this.DownloadToWriterAsync(node1, Environment.ProcessorCount, false, (Func<DedupNode, uint, long, ArraySegment<byte>, Task>) ((node2, _chunkIndex, fileOffset, buffer) => AsyncFile.WriteAsync(wholeFile, fileOffset, buffer).EnforceCancellation(cancellationToken, (Func<string>) (() => "Timed out waiting for WriteAsync to '" + wholeFile.Name + "'."), "D:\\a\\_work\\1\\s\\BlobStore\\Client\\WebApi\\DedupStoreClient.cs", nameof (DownloadAndMoveTempFile), 1189)), proxyUri, edgeCache, cancellationToken);
        }
        if (File.Exists(fullPath))
          File.Delete(fullPath);
        File.Move(tempFilePath, fullPath);
      }
      catch
      {
        if (File.Exists(tempFilePath))
          File.Delete(tempFilePath);
        throw;
      }
      tempFilePath = (string) null;
    }

    public async Task<DedupNode> GetFilledNodesAsync(
      DedupNode node,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      if (node.Type == DedupNode.NodeType.ChunkLeaf)
        return node;
      if (node.ChildNodes == null)
      {
        using (DedupCompressedBuffer nodeAsync = (DedupCompressedBuffer) await this.GetNodeAsync(new NodeDedupIdentifier(node.Hash), cancellationToken))
        {
          if (nodeAsync == null)
            return node;
          node = DedupNode.Deserialize(nodeAsync.Uncompressed.CreateCopy<byte>());
        }
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      HashSet<DedupIdentifier> childNodeDedupIds = new HashSet<DedupIdentifier>(node.ChildNodes.Where<DedupNode>((Func<DedupNode, bool>) (n => n.Type == DedupNode.NodeType.InnerNode)).Select<DedupNode, DedupIdentifier>(DedupStoreClient.\u003C\u003EO.\u003C0\u003E__Create ?? (DedupStoreClient.\u003C\u003EO.\u003C0\u003E__Create = new Func<DedupNode, DedupIdentifier>(DedupIdentifier.Create))));
      if (!childNodeDedupIds.Any<DedupIdentifier>())
        return node;
      Dictionary<DedupIdentifier, GetDedupAsyncFunc> dedupGettersAsync;
      using (await this.AcquireParallelismTokenAsync())
        dedupGettersAsync = await this.Client.GetDedupGettersAsync((ISet<DedupIdentifier>) childNodeDedupIds, proxyUri, edgeCache, cancellationToken);
      Dictionary<DedupIdentifier, Task<DedupNode>> childNodes = dedupGettersAsync.ToDictionary<KeyValuePair<DedupIdentifier, GetDedupAsyncFunc>, DedupIdentifier, Task<DedupNode>>((Func<KeyValuePair<DedupIdentifier, GetDedupAsyncFunc>, DedupIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<DedupIdentifier, GetDedupAsyncFunc>, Task<DedupNode>>) (kvp => Task.Run<DedupNode>((Func<Task<DedupNode>>) (() =>
      {
        LockSet<DedupIdentifier>.LockHandle lockHandle = await this.currentOperations.Acquire(kvp.Key);
        DedupNode node1;
        try
        {
          using (await this.AcquireParallelismTokenAsync())
          {
            MaybeCached<DedupCompressedBuffer> bytes = await kvp.Value(cancellationToken);
            using (bytes.Value)
            {
              this.UpdateCountersOfDownload(bytes, DedupNode.NodeType.InnerNode);
              node1 = DedupNode.Deserialize(bytes.Value.Uncompressed);
            }
          }
        }
        finally
        {
          lockHandle.Dispose();
        }
        lockHandle = new LockSet<DedupIdentifier>.LockHandle();
        return await this.GetFilledNodesAsync(node1, proxyUri, edgeCache, cancellationToken);
      }))));
      List<DedupNode> filledChildren = new List<DedupNode>();
      foreach (DedupNode childNode in (IEnumerable<DedupNode>) node.ChildNodes)
      {
        NodeDedupIdentifier key = new NodeDedupIdentifier(childNode.Hash);
        if (childNodes.ContainsKey((DedupIdentifier) key))
        {
          List<DedupNode> dedupNodeList = filledChildren;
          dedupNodeList.Add(await childNodes[(DedupIdentifier) key]);
          dedupNodeList = (List<DedupNode>) null;
        }
        else
          filledChildren.Add(childNode);
      }
      return new DedupNode((IEnumerable<DedupNode>) filledChildren);
    }

    private bool IsCacheEnabled(IDedupStoreHttpClient client) => client.GetType() != typeof (DedupStoreHttpClient);

    private async Task<DedupNode> WriteToStreamAsync(
      TryAddValueAsyncFunc<DedupCompressedBuffer> writeBufferAsync,
      long nodeFileOffset,
      DedupNode node,
      GetDedupAsyncFunc nodeFetcher,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      if (node.Type == DedupNode.NodeType.ChunkLeaf)
      {
        int num = await writeBufferAsync((DedupCompressedBuffer) await this.GetChunkAsync(new ChunkDedupIdentifier(node.Hash), cancellationToken)) ? 1 : 0;
        return node;
      }
      if (node.ChildNodes == null)
      {
        DedupCompressedBuffer nodeAsync;
        if (nodeFetcher == null)
        {
          nodeAsync = (DedupCompressedBuffer) await this.GetNodeAsync(new NodeDedupIdentifier(node.Hash), cancellationToken);
        }
        else
        {
          MaybeCached<DedupCompressedBuffer> bytes = await nodeFetcher(cancellationToken);
          this.UpdateCountersOfDownload(bytes, DedupNode.NodeType.InnerNode);
          nodeAsync = bytes.Value;
        }
        using (nodeAsync)
          node = DedupNode.Deserialize(nodeAsync.Uncompressed.CreateCopy<byte>());
      }
      long fileOffset = nodeFileOffset;
      List<KeyValuePair<long, DedupNode>> childOffsets = node.ChildNodes.Select<DedupNode, KeyValuePair<long, DedupNode>>((Func<DedupNode, KeyValuePair<long, DedupNode>>) (n =>
      {
        long key = fileOffset;
        fileOffset += (long) n.TransitiveContentBytes;
        DedupNode dedupNode = n;
        return new KeyValuePair<long, DedupNode>(key, dedupNode);
      })).ToList<KeyValuePair<long, DedupNode>>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      HashSet<DedupIdentifier> dedupIds = new HashSet<DedupIdentifier>(node.ChildNodes.Select<DedupNode, DedupIdentifier>(DedupStoreClient.\u003C\u003EO.\u003C0\u003E__Create ?? (DedupStoreClient.\u003C\u003EO.\u003C0\u003E__Create = new Func<DedupNode, DedupIdentifier>(DedupIdentifier.Create))));
      Dictionary<DedupIdentifier, GetDedupAsyncFunc> childDownloaders;
      using (await this.AcquireParallelismTokenAsync())
        childDownloaders = await this.Client.GetDedupGettersAsync((ISet<DedupIdentifier>) dedupIds, proxyUri, edgeCache, cancellationToken);
      Dictionary<DedupNode, Task<MaybeCached<DedupCompressedBuffer>>> chunks = node.ChildNodes.Where<DedupNode>((Func<DedupNode, bool>) (c => c.Type == DedupNode.NodeType.ChunkLeaf)).Distinct<DedupNode>().ToDictionary<DedupNode, DedupNode, Task<MaybeCached<DedupCompressedBuffer>>>((Func<DedupNode, DedupNode>) (c => c), (Func<DedupNode, Task<MaybeCached<DedupCompressedBuffer>>>) (c => Task.Run<MaybeCached<DedupCompressedBuffer>>((Func<Task<MaybeCached<DedupCompressedBuffer>>>) (async () =>
      {
        DedupIdentifier key = DedupIdentifier.Create(c);
        GetDedupAsyncFunc fetcher = childDownloaders[key];
        MaybeCached<DedupCompressedBuffer> streamAsync;
        using (await this.currentOperations.Acquire(key))
        {
          using (await this.AcquireParallelismTokenAsync())
            streamAsync = await fetcher(cancellationToken);
        }
        fetcher = (GetDedupAsyncFunc) null;
        return streamAsync;
      }))));
      Dictionary<DedupNode, int> chunkCounts = childOffsets.GroupBy<KeyValuePair<long, DedupNode>, DedupNode, long>((Func<KeyValuePair<long, DedupNode>, DedupNode>) (offsetAndNode => offsetAndNode.Value), (Func<KeyValuePair<long, DedupNode>, long>) (offsetAndNode => offsetAndNode.Key)).ToDictionary<IGrouping<DedupNode, long>, DedupNode, int>((Func<IGrouping<DedupNode, long>, DedupNode>) (grouping => grouping.Key), (Func<IGrouping<DedupNode, long>, int>) (grouping => grouping.Count<long>()));
      List<DedupNode> children = new List<DedupNode>(childOffsets.Count);
      foreach (KeyValuePair<long, DedupNode> keyValuePair in childOffsets)
      {
        DedupNode childNode = keyValuePair.Value;
        long key = keyValuePair.Key;
        if (childNode.Type == DedupNode.NodeType.ChunkLeaf)
        {
          MaybeCached<DedupCompressedBuffer> bytes = await chunks[childNode];
          if (chunkCounts[childNode] > 1)
          {
            IPoolHandle<byte[]> uncompressed = ChunkerHelper.BorrowChunkBuffer(bytes.Value.Uncompressed.Count);
            System.Buffer.BlockCopy((Array) bytes.Value.Uncompressed.Array, bytes.Value.Uncompressed.Offset, (Array) uncompressed.Value, 0, bytes.Value.Uncompressed.Count);
            bytes = MaybeCached.FromCached<DedupCompressedBuffer>(DedupCompressedBuffer.FromUncompressed(uncompressed, 0, bytes.Value.Uncompressed.Count));
          }
          this.UpdateCountersOfDownload(bytes, DedupNode.NodeType.ChunkLeaf);
          int num = await writeBufferAsync(bytes.Value) ? 1 : 0;
          chunkCounts[childNode]--;
        }
        else
          childNode = await this.WriteToStreamAsync(writeBufferAsync, key, childNode, childDownloaders[childNode.GetDedupIdentifier()], proxyUri, edgeCache, cancellationToken);
        children.Add(childNode);
        childNode = new DedupNode();
      }
      DedupNode streamAsync1 = new DedupNode((IEnumerable<DedupNode>) children);
      if (((IList<byte>) streamAsync1.Hash).ToHex() != ((IList<byte>) node.Hash).ToHex())
        throw new InvalidOperationException();
      return streamAsync1;
    }

    protected void UpdateCountersOfDownload(
      MaybeCached<DedupCompressedBuffer> bytes,
      DedupNode.NodeType type)
    {
      if (type == DedupNode.NodeType.InnerNode && bytes.Value != null)
      {
        Interlocked.Increment(ref this.nodesDownloaded);
      }
      else
      {
        if (bytes.Value == null)
          return;
        if (bytes.Cached)
        {
          Interlocked.Add(ref this.dedupDownloadBytesSaved, (long) bytes.Value.Uncompressed.Count);
        }
        else
        {
          bool flag = true;
          ArraySegment<byte>? compressedBytes;
          if (!bytes.Value.TryGetAlreadyCompressed(out compressedBytes))
          {
            flag = false;
            compressedBytes = new ArraySegment<byte>?(bytes.Value.Uncompressed);
          }
          if (flag)
            Interlocked.Add(ref this.compressionDownloadBytesSaved, (long) (bytes.Value.Uncompressed.Count - compressedBytes.Value.Count));
          Interlocked.Increment(ref this.chunksDownloaded);
          Interlocked.Add(ref this.physicalContentBytesDownloaded, (long) compressedBytes.Value.Count);
        }
      }
    }

    public Task<Dictionary<DedupIdentifier, GetDedupAsyncFunc>> GetDedupGettersAsync(
      ISet<DedupIdentifier> dedupIds,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      return this.Client.GetDedupGettersAsync(dedupIds, proxyUri, edgeCache, cancellationToken);
    }

    public static string MaskDedupId(DedupIdentifier nodeId)
    {
      string valueString = nodeId.ValueString;
      char[] chArray = new char[5];
      int length = valueString.Length;
      chArray[0] = valueString[0];
      chArray[1] = valueString[1];
      chArray[2] = '*';
      chArray[3] = valueString[length - 2];
      chArray[4] = valueString[length - 1];
      return new string(chArray);
    }

    public class UploadSession : IDedupUploadSession
    {
      private long logicalContentBytesUploaded;
      private long physicalContentBytesUploaded;
      private long compressionBytesSaved;
      private long dedupUploadBytesSaved;
      private long chunksUploaded;
      private long totalChunkNumber;
      protected readonly ConcurrentDictionary<DedupIdentifier, string> filePaths = new ConcurrentDictionary<DedupIdentifier, string>();
      private readonly ConcurrentDictionary<DedupIdentifier, KeepUntilReceipt> sessionReceipts = new ConcurrentDictionary<DedupIdentifier, KeepUntilReceipt>();
      private readonly LockSet<DedupIdentifier> putChunkInProgress = new LockSet<DedupIdentifier>();
      private readonly IDedupStoreClient client;
      private readonly KeepUntilBlobReference keepUntilReference;
      private readonly IAppTraceSource Tracer;
      private readonly SemaphoreSlim chunkUploadsToBuffer = new SemaphoreSlim(Environment.ProcessorCount, Environment.ProcessorCount);
      protected readonly ConcurrentDictionary<NodeDedupIdentifier, DedupNode> allNodes = new ConcurrentDictionary<NodeDedupIdentifier, DedupNode>();
      protected readonly ConcurrentDictionary<DedupIdentifier, NodeDedupIdentifier> parentLookup = new ConcurrentDictionary<DedupIdentifier, NodeDedupIdentifier>();
      protected readonly IFileSystem fileSystem;

      public long TotalContentBytes => this.logicalContentBytesUploaded + this.dedupUploadBytesSaved;

      public DedupUploadStatistics UploadStatistics => new DedupUploadStatistics(this.chunksUploaded, this.compressionBytesSaved, this.dedupUploadBytesSaved, this.logicalContentBytesUploaded, this.physicalContentBytesUploaded, this.totalChunkNumber);

      public IReadOnlyDictionary<NodeDedupIdentifier, DedupNode> AllNodes => (IReadOnlyDictionary<NodeDedupIdentifier, DedupNode>) this.allNodes;

      public IReadOnlyDictionary<DedupIdentifier, NodeDedupIdentifier> ParentLookup => (IReadOnlyDictionary<DedupIdentifier, NodeDedupIdentifier>) this.parentLookup;

      public UploadSession(
        IDedupStoreClient client,
        KeepUntilBlobReference keepUntilReference,
        IAppTraceSource tracer,
        IFileSystem fileSystem)
      {
        this.client = client;
        this.keepUntilReference = keepUntilReference;
        this.Tracer = tracer;
        this.fileSystem = fileSystem;
      }

      public virtual Task<KeepUntilReceipt> UploadAsync(
        DedupNode node,
        IReadOnlyDictionary<DedupIdentifier, string> filePaths,
        CancellationToken cancellationToken)
      {
        node.AssertFilled();
        foreach (KeyValuePair<DedupIdentifier, string> filePath in (IEnumerable<KeyValuePair<DedupIdentifier, string>>) filePaths)
          this.filePaths[filePath.Key] = filePath.Value;
        this.totalChunkNumber = 0L;
        foreach (DedupNode node1 in node.EnumerateInnerNodesDepthFirst())
        {
          this.AddNode(node1);
          this.totalChunkNumber += (long) node1.EnumerateChunkLeafsInOrder().Count<DedupNode>();
        }
        return this.UploadNodeAsync(node, (string) null, (Lazy<FileStream>) null, new long?(), "", cancellationToken);
      }

      protected void AddNode(DedupNode node)
      {
        NodeDedupIdentifier nodeDedupIdentifier = node.CalculateNodeDedupIdentifier();
        foreach (DedupNode childNode in (IEnumerable<DedupNode>) node.ChildNodes)
          this.parentLookup[childNode.GetDedupIdentifier()] = nodeDedupIdentifier;
        this.allNodes[nodeDedupIdentifier] = node;
      }

      private async Task<KeepUntilReceipt> UploadNodeAsync(
        DedupNode node,
        string path,
        Lazy<FileStream> file,
        long? offset,
        string indent,
        CancellationToken cancellationToken)
      {
        NodeDedupIdentifier nodeDedupIdentifier = new NodeDedupIdentifier(node.Hash);
        KeepUntilReceipt keepUntilReceipt;
        if (this.sessionReceipts.TryGetValue((DedupIdentifier) nodeDedupIdentifier, out keepUntilReceipt))
          Interlocked.Add(ref this.dedupUploadBytesSaved, (long) node.TransitiveContentBytes);
        else
          keepUntilReceipt = await this.UploadNodeAsyncInternal(nodeDedupIdentifier, node, file, path, offset, indent, cancellationToken);
        return keepUntilReceipt;
      }

      private async Task<KeepUntilReceipt> UploadNodeAsyncInternal(
        NodeDedupIdentifier nodeId,
        DedupNode node,
        Lazy<FileStream> file,
        string path,
        long? offset,
        string indent,
        CancellationToken cancellationToken)
      {
        KeepUntilReceipt keepUntilReceipt1;
        using (DedupCompressedBuffer serializedNode = DedupCompressedBuffer.FromUncompressed(node.Serialize()))
        {
          List<DedupIdentifier> childIds = node.ChildNodes.Select<DedupNode, DedupIdentifier>((Func<DedupNode, DedupIdentifier>) (n => n.GetDedupIdentifier())).ToList<DedupIdentifier>();
          string maskedNodeId = DedupStoreClient.MaskDedupId((DedupIdentifier) nodeId);
          KeepUntilReceipt[] receipts = new KeepUntilReceipt[node.ChildNodes.Count];
          for (int index = 0; index < receipts.Length; ++index)
          {
            KeepUntilReceipt keepUntilReceipt2;
            if (receipts[index] == (KeepUntilReceipt) null && this.sessionReceipts.TryGetValue(childIds[index], out keepUntilReceipt2))
              receipts[index] = keepUntilReceipt2;
          }
          SummaryKeepUntilReceipt summaryReceipt = ((IEnumerable<KeepUntilReceipt>) receipts).Any<KeepUntilReceipt>((Func<KeepUntilReceipt, bool>) (r => r != (KeepUntilReceipt) null)) ? new SummaryKeepUntilReceipt(receipts) : (SummaryKeepUntilReceipt) null;
          PutNodeResponse putNodeResponse;
          using (await this.client.AcquireParallelismTokenAsync())
          {
            List<string> values = new List<string>();
            for (int index = 0; index < node.ChildNodes.Count; ++index)
              values.Add(DedupStoreClient.MaskDedupId(node.ChildNodes[index].GetDedupIdentifier()) + " [" + (receipts[index]?.KeepUntil.KeepUntilString ?? "None") + "]");
            this.Tracer.Verbose("{0}Trying to put node {1} of {2} children, {3} receipts used. (Children: {4})", (object) indent, (object) maskedNodeId, (object) node.ChildNodes.Count, (object) ((IEnumerable<KeepUntilReceipt>) receipts).Count<KeepUntilReceipt>((Func<KeepUntilReceipt, bool>) (r => r != (KeepUntilReceipt) null)), (object) string.Join(", ", (IEnumerable<string>) values));
            putNodeResponse = await this.client.Client.PutNodeAndKeepUntilReferenceAsync(nodeId, serializedNode, this.keepUntilReference, summaryReceipt, cancellationToken);
          }
          Func<DedupNodeUpdated, KeepUntilReceipt> func;
          keepUntilReceipt1 = await putNodeResponse.Match<Task<KeepUntilReceipt>>((Func<DedupNodeChildrenNeedAction, Task<KeepUntilReceipt>>) (async childrenAction =>
          {
            this.Tracer.Verbose("{0}Could not add node {1} of {2} children as {3} children are missing and {4} children have insufficient keepuntil. Got {5} receipts back though. (Missing: {6}; InsufficientKeepUntil: {7}; Receipts: {8})", (object) indent, (object) maskedNodeId, (object) node.ChildNodes.Count, (object) ((IEnumerable<DedupIdentifier>) childrenAction.Missing).Count<DedupIdentifier>(), (object) ((IEnumerable<DedupIdentifier>) childrenAction.InsufficientKeepUntil).Count<DedupIdentifier>(), (object) childrenAction.Receipts.Count<KeyValuePair<DedupIdentifier, KeepUntilReceipt>>(), (object) string.Join(", ", ((IEnumerable<DedupIdentifier>) childrenAction.Missing).Select<DedupIdentifier, string>((Func<DedupIdentifier, string>) (c => DedupStoreClient.MaskDedupId(c)))), (object) string.Join(", ", ((IEnumerable<DedupIdentifier>) childrenAction.InsufficientKeepUntil).Select<DedupIdentifier, string>((Func<DedupIdentifier, string>) (c => DedupStoreClient.MaskDedupId(c)))), (object) string.Join(", ", childrenAction.Receipts.Select<KeyValuePair<DedupIdentifier, KeepUntilReceipt>, string>((Func<KeyValuePair<DedupIdentifier, KeepUntilReceipt>, string>) (kvp => DedupStoreClient.MaskDedupId(kvp.Key) + " [" + kvp.Value.KeepUntil.KeepUntilString + "]"))));
            KeepUntilReceipt[] childReceipts = await this.UploadChildrenAsync(node, receipts, childrenAction, file, path, offset, indent, cancellationToken);
            for (int index = 0; index < childIds.Count; ++index)
              this.sessionReceipts[childIds[index]] = childReceipts[index];
            summaryReceipt = new SummaryKeepUntilReceipt(childReceipts);
            KeepUntilReceipt keepUntilReceipt3;
            using (await this.client.AcquireParallelismTokenAsync())
            {
              List<string> values = new List<string>();
              for (int index = 0; index < node.ChildNodes.Count; ++index)
                values.Add(DedupStoreClient.MaskDedupId(node.ChildNodes[index].GetDedupIdentifier()) + " [" + (childReceipts[index]?.KeepUntil.KeepUntilString ?? "None") + "]");
              this.Tracer.Verbose("{0}After uploading children, trying to put node {1} of {2} children, {3} receipts used. (Children: {4})", (object) indent, (object) maskedNodeId, (object) node.ChildNodes.Count, (object) ((IEnumerable<KeepUntilReceipt>) childReceipts).Count<KeepUntilReceipt>((Func<KeepUntilReceipt, bool>) (r => r != (KeepUntilReceipt) null)), (object) string.Join(", ", (IEnumerable<string>) values));
              keepUntilReceipt3 = (await this.client.Client.PutNodeAndKeepUntilReferenceAsync(nodeId, serializedNode, this.keepUntilReference, summaryReceipt, cancellationToken)).Match<KeepUntilReceipt>((Func<DedupNodeChildrenNeedAction, KeepUntilReceipt>) (stillChildrenAction =>
              {
                throw new InvalidOperationException();
              }), func ?? (func = (Func<DedupNodeUpdated, KeepUntilReceipt>) (added =>
              {
                this.Tracer.Verbose("{0}After uploading children, added node {1} of {2} children and got {3} receipts back. (Receipts: {4})", (object) indent, (object) maskedNodeId, (object) node.ChildNodes.Count, (object) added.Receipts.Count, (object) string.Join(", ", added.Receipts.Select<KeyValuePair<DedupIdentifier, KeepUntilReceipt>, string>((Func<KeyValuePair<DedupIdentifier, KeepUntilReceipt>, string>) (kvp => DedupStoreClient.MaskDedupId(kvp.Key) + " [" + kvp.Value.KeepUntil.KeepUntilString + "]"))));
                foreach (KeyValuePair<DedupIdentifier, KeepUntilReceipt> receipt in added.Receipts)
                  this.sessionReceipts[receipt.Key] = receipt.Value;
                return added.Receipts[(DedupIdentifier) nodeId];
              })));
            }
            childReceipts = (KeepUntilReceipt[]) null;
            return keepUntilReceipt3;
          }), (Func<DedupNodeUpdated, Task<KeepUntilReceipt>>) (added =>
          {
            this.Tracer.Verbose("{0}Added node {1} of {2} children and got {3} receipts back. (Receipts: {4})", (object) indent, (object) maskedNodeId, (object) node.ChildNodes.Count, (object) added.Receipts.Count, (object) string.Join(", ", added.Receipts.Select<KeyValuePair<DedupIdentifier, KeepUntilReceipt>, string>((Func<KeyValuePair<DedupIdentifier, KeepUntilReceipt>, string>) (kvp => DedupStoreClient.MaskDedupId(kvp.Key) + " [" + kvp.Value.KeepUntil.KeepUntilString + "]"))));
            Interlocked.Add(ref this.dedupUploadBytesSaved, (long) node.TransitiveContentBytes);
            foreach (KeyValuePair<DedupIdentifier, KeepUntilReceipt> receipt in added.Receipts)
              this.sessionReceipts[receipt.Key] = receipt.Value;
            return Task.FromResult<KeepUntilReceipt>(added.Receipts[(DedupIdentifier) nodeId]);
          }));
        }
        return keepUntilReceipt1;
      }

      private async Task<KeepUntilReceipt[]> UploadChildrenAsync(
        DedupNode node,
        KeepUntilReceipt[] existingReceipts,
        DedupNodeChildrenNeedAction childrenAction,
        Lazy<FileStream> file,
        string path,
        long? offset,
        string indent,
        CancellationToken cancellationToken)
      {
        NodeDedupIdentifier nodeIdentifier = node.GetNodeIdentifier();
        Dictionary<DedupIdentifier, Task<KeepUntilReceipt>> childUploadTasks = new Dictionary<DedupIdentifier, Task<KeepUntilReceipt>>();
        Dictionary<ChunkDedupIdentifier, DedupCompressedBuffer> childChunksToUpload = new Dictionary<ChunkDedupIdentifier, DedupCompressedBuffer>();
        ConcurrentDictionary<DedupIdentifier, KeepUntilReceipt> newReceipts = new ConcurrentDictionary<DedupIdentifier, KeepUntilReceipt>();
        Lazy<FileStream> fileToClose = (Lazy<FileStream>) null;
        if (path == null && this.filePaths.ContainsKey((DedupIdentifier) nodeIdentifier))
        {
          path = this.filePaths[(DedupIdentifier) nodeIdentifier];
          offset = new long?(0L);
          fileToClose = file = new Lazy<FileStream>((Func<FileStream>) (() => this.fileSystem.OpenFileStreamForAsync(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete)));
        }
        int i;
        int chunkSize;
        int num1;
        try
        {
          IDisposable chunkUploadToken = (IDisposable) await this.chunkUploadsToBuffer.WaitToken();
          try
          {
            for (i = 0; i < node.ChildNodes.Count; ++i)
            {
              DedupNode childNode = node.ChildNodes[i];
              DedupIdentifier dedupId = DedupIdentifier.Create(childNode);
              if (existingReceipts[i] != (KeepUntilReceipt) null && !childUploadTasks.ContainsKey(dedupId))
                childUploadTasks.Add(dedupId, Task.FromResult<KeepUntilReceipt>(existingReceipts[i]));
              if (childUploadTasks.ContainsKey(dedupId))
                Interlocked.Add(ref this.dedupUploadBytesSaved, (long) childNode.TransitiveContentBytes);
              else if (((IEnumerable<DedupIdentifier>) childrenAction.Missing).Contains<DedupIdentifier>(dedupId) || ((IEnumerable<DedupIdentifier>) childrenAction.InsufficientKeepUntil).Contains<DedupIdentifier>(dedupId))
              {
                if (childNode.Type == DedupNode.NodeType.ChunkLeaf)
                {
                  ChunkDedupIdentifier key = (ChunkDedupIdentifier) dedupId;
                  KeepUntilReceipt result;
                  if (this.sessionReceipts.TryGetValue((DedupIdentifier) key, out result))
                  {
                    Interlocked.Add(ref this.dedupUploadBytesSaved, (long) childNode.TransitiveContentBytes);
                    childUploadTasks.Add(dedupId, Task.FromResult<KeepUntilReceipt>(result));
                  }
                  else if (!childChunksToUpload.ContainsKey(key))
                  {
                    chunkSize = (int) childNode.TransitiveContentBytes;
                    string chunkPath;
                    long chunkOffset;
                    FileStream chunkFile;
                    if (offset.HasValue)
                    {
                      chunkPath = path;
                      chunkFile = file.Value;
                      chunkOffset = offset.Value;
                    }
                    else
                    {
                      if (!this.filePaths.TryGetValue((DedupIdentifier) key, out chunkPath))
                        throw new ArgumentException(string.Format("Path not found for chunkId {0} among {1} paths", (object) key, (object) this.filePaths.Count));
                      chunkOffset = 0L;
                      chunkFile = chunkPath.EndsWith(FileBlobDescriptorConstants.EmptyDirectoryEndingPattern) ? (FileStream) null : this.fileSystem.OpenFileStreamForAsync(chunkPath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);
                    }
                    DedupCompressedBuffer compressedBuffer;
                    try
                    {
                      using (await this.client.AcquireParallelismTokenAsync())
                      {
                        IPoolHandle<byte[]> chunkBuffer = ChunkerHelper.BorrowChunkBuffer(chunkSize);
                        if (chunkFile != null || !chunkPath.EndsWith(FileBlobDescriptorConstants.EmptyDirectoryEndingPattern))
                          await AsyncFile.ReadWholeBufferAsync(chunkFile, chunkOffset, chunkBuffer.Value, chunkSize);
                        compressedBuffer = DedupCompressedBuffer.FromUncompressed(chunkBuffer, 0, chunkSize);
                        bool isCompressed;
                        ArraySegment<byte> buffer;
                        compressedBuffer.GetBytesTryCompress(out isCompressed, out buffer);
                        this.Tracer.Verbose("{0}Uploading chunk {1} consisting of {2:N0} bytes ({3}) from {4}@{5}...", (object) indent, (object) DedupStoreClient.MaskDedupId(dedupId), (object) chunkSize, isCompressed ? (object) string.Format("compressed to {0:N0}", (object) buffer.Count) : (object) "not compressed", (object) chunkPath, (object) chunkOffset);
                        chunkBuffer = (IPoolHandle<byte[]>) null;
                      }
                    }
                    finally
                    {
                      if (num1 < 0 && chunkFile != null && (file == null || !file.IsValueCreated || chunkFile != file.Value))
                        chunkFile.Dispose();
                    }
                    childChunksToUpload.Add((ChunkDedupIdentifier) dedupId, compressedBuffer);
                    chunkPath = (string) null;
                    chunkFile = (FileStream) null;
                  }
                }
                else
                  childUploadTasks.Add(dedupId, this.UploadNodeAsync(childNode, path, file, offset, indent + " ", cancellationToken));
              }
              else
              {
                Interlocked.Add(ref this.dedupUploadBytesSaved, (long) childNode.TransitiveContentBytes);
                KeepUntilReceipt existingReceipt;
                if (!childrenAction.Receipts.TryGetValue(dedupId, out existingReceipt))
                  existingReceipt = existingReceipts[i];
                childUploadTasks.Add(dedupId, Task.FromResult<KeepUntilReceipt>(existingReceipt));
              }
              if (offset.HasValue)
              {
                long? nullable = offset;
                long transitiveContentBytes = (long) childNode.TransitiveContentBytes;
                offset = nullable.HasValue ? new long?(nullable.GetValueOrDefault() + transitiveContentBytes) : new long?();
              }
              childNode = new DedupNode();
              dedupId = (DedupIdentifier) null;
            }
            Action<string> action;
            await Task.WhenAll(childChunksToUpload.GetPages<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>>(128).Select<List<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>>, Task>((Func<List<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>>, Task>) (page => Task.Run((Func<Task>) (async () =>
            {
              page.Sort((Comparison<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>>) ((kvp1, kvp2) => ByteArrayComparer.Instance.Compare(kvp1.Key.Value, kvp2.Key.Value)));
              using (DedupStoreClient.Disposer chunkUploadDisposer = new DedupStoreClient.Disposer(action ?? (action = (Action<string>) (msg => this.Tracer.Verbose(msg)))))
              {
                HashSet<DedupIdentifier> chunksToUpload = new HashSet<DedupIdentifier>();
                foreach (DedupIdentifier dedupIdentifier in page.Select<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>, ChunkDedupIdentifier>((Func<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>, ChunkDedupIdentifier>) (kvp => kvp.Key)))
                {
                  DedupIdentifier dedupId = dedupIdentifier;
                  KeepUntilReceipt keepUntilReceipt;
                  if (this.sessionReceipts.TryGetValue(dedupId, out keepUntilReceipt))
                  {
                    newReceipts.TryAdd(dedupId, keepUntilReceipt);
                  }
                  else
                  {
                    DedupStoreClient.Disposer disposer = chunkUploadDisposer;
                    disposer.Add<LockSet<DedupIdentifier>.LockHandle>(await this.putChunkInProgress.Acquire(dedupId));
                    disposer = (DedupStoreClient.Disposer) null;
                    if (this.sessionReceipts.TryGetValue(dedupId, out keepUntilReceipt))
                      newReceipts.TryAdd(dedupId, keepUntilReceipt);
                    else
                      chunksToUpload.Add(dedupId);
                    dedupId = (DedupIdentifier) null;
                  }
                }
                page = page.Where<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>>((Func<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>, bool>) (kvp => chunksToUpload.Contains((DedupIdentifier) kvp.Key))).ToList<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>>();
                using (await this.client.AcquireParallelismTokenAsync())
                {
                  this.Tracer.Verbose("{0}Uploading {1} chunks consisting of {2:N0} bytes...", (object) indent, (object) page.Count, (object) page.Sum<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>>((Func<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>, int>) (c => c.Value.Uncompressed.Count)));
                  Dictionary<ChunkDedupIdentifier, KeepUntilReceipt> dictionary = await this.client.Client.PutChunksAsync(page.ToDictionary<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>, ChunkDedupIdentifier, DedupCompressedBuffer>((Func<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>, ChunkDedupIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>, DedupCompressedBuffer>) (kvp => kvp.Value)), this.keepUntilReference, cancellationToken);
                  foreach (KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer> keyValuePair in page)
                  {
                    ArraySegment<byte>? compressedBytes;
                    bool alreadyCompressed = keyValuePair.Value.TryGetAlreadyCompressed(out compressedBytes);
                    ref long local1 = ref this.logicalContentBytesUploaded;
                    ArraySegment<byte> uncompressed = keyValuePair.Value.Uncompressed;
                    long count1 = (long) uncompressed.Count;
                    Interlocked.Add(ref local1, count1);
                    ref long local2 = ref this.physicalContentBytesUploaded;
                    int count2;
                    if (!alreadyCompressed)
                    {
                      uncompressed = keyValuePair.Value.Uncompressed;
                      count2 = uncompressed.Count;
                    }
                    else
                    {
                      uncompressed = compressedBytes.Value;
                      count2 = uncompressed.Count;
                    }
                    long num2 = (long) count2;
                    Interlocked.Add(ref local2, num2);
                    ref long local3 = ref this.compressionBytesSaved;
                    int num3;
                    if (!alreadyCompressed)
                    {
                      num3 = 0;
                    }
                    else
                    {
                      uncompressed = keyValuePair.Value.Uncompressed;
                      int count3 = uncompressed.Count;
                      uncompressed = compressedBytes.Value;
                      int count4 = uncompressed.Count;
                      num3 = count3 - count4;
                    }
                    long num4 = (long) num3;
                    Interlocked.Add(ref local3, num4);
                    Interlocked.Increment(ref this.chunksUploaded);
                    keyValuePair.Value.Dispose();
                  }
                  foreach (KeyValuePair<ChunkDedupIdentifier, KeepUntilReceipt> keyValuePair in dictionary)
                  {
                    newReceipts.TryAdd((DedupIdentifier) keyValuePair.Key, keyValuePair.Value);
                    this.sessionReceipts.TryAdd((DedupIdentifier) keyValuePair.Key, keyValuePair.Value);
                  }
                  this.Tracer.Verbose("{0}Uploaded {1} chunks consisting of {2:N0} bytes.", (object) indent, (object) page.Count, (object) page.Sum<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>>((Func<KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer>, int>) (c => c.Value.Uncompressed.Count)));
                }
              }
            })))));
          }
          finally
          {
            foreach (DedupCompressedBuffer compressedBuffer in childChunksToUpload.Values)
              compressedBuffer.Dispose();
            chunkUploadToken.Dispose();
          }
          KeepUntilReceipt[] keepUntilReceiptArray = await Task.WhenAll<KeepUntilReceipt>((IEnumerable<Task<KeepUntilReceipt>>) childUploadTasks.Values);
          chunkUploadToken = (IDisposable) null;
        }
        finally
        {
          if (num1 < 0 && fileToClose != null && fileToClose.IsValueCreated)
            fileToClose.Value.Dispose();
        }
        KeepUntilReceipt[] receipts = new KeepUntilReceipt[node.ChildNodes.Count];
        for (i = 0; i < node.ChildNodes.Count; ++i)
        {
          DedupIdentifier key = DedupIdentifier.Create(node.ChildNodes[i]);
          KeepUntilReceipt keepUntilReceipt;
          if (newReceipts.TryGetValue(key, out keepUntilReceipt))
          {
            receipts[i] = keepUntilReceipt;
          }
          else
          {
            KeepUntilReceipt[] keepUntilReceiptArray = receipts;
            chunkSize = i;
            keepUntilReceiptArray[chunkSize] = await childUploadTasks[key];
            keepUntilReceiptArray = (KeepUntilReceipt[]) null;
          }
        }
        KeepUntilReceipt[] keepUntilReceiptArray1 = receipts;
        childUploadTasks = (Dictionary<DedupIdentifier, Task<KeepUntilReceipt>>) null;
        childChunksToUpload = (Dictionary<ChunkDedupIdentifier, DedupCompressedBuffer>) null;
        fileToClose = (Lazy<FileStream>) null;
        receipts = (KeepUntilReceipt[]) null;
        return keepUntilReceiptArray1;
      }

      public async Task<Dictionary<DedupIdentifier, CheckIfUploadNeededResult>> CheckIfUploadIsNeededAsync(
        IReadOnlyDictionary<DedupIdentifier, long> fileSizes,
        CancellationToken cancellationToken)
      {
        Dictionary<DedupIdentifier, CheckIfUploadNeededResult> results = fileSizes.ToDictionary<KeyValuePair<DedupIdentifier, long>, DedupIdentifier, CheckIfUploadNeededResult>((Func<KeyValuePair<DedupIdentifier, long>, DedupIdentifier>) (kvp => kvp.Key), (Func<KeyValuePair<DedupIdentifier, long>, CheckIfUploadNeededResult>) (_ => CheckIfUploadNeededResult.UploadNeeded));
        foreach (List<KeyValuePair<DedupIdentifier, long>> page in fileSizes.Where<KeyValuePair<DedupIdentifier, long>>((Func<KeyValuePair<DedupIdentifier, long>, bool>) (f =>
        {
          if (!this.sessionReceipts.TryGetValue(f.Key, out KeepUntilReceipt _))
            return true;
          results[f.Key] = CheckIfUploadNeededResult.UploadNotNeeded;
          return false;
        })).GetPages<KeyValuePair<DedupIdentifier, long>>(512))
        {
          Dictionary<DedupNode, DedupIdentifier> nodeToDedupId = page.ToDictionary<KeyValuePair<DedupIdentifier, long>, DedupNode, DedupIdentifier>((Func<KeyValuePair<DedupIdentifier, long>, DedupNode>) (f => new DedupNode(ChunkerHelper.IsChunk(f.Key.AlgorithmId) ? DedupNode.NodeType.ChunkLeaf : DedupNode.NodeType.InnerNode, (ulong) f.Value, f.Key.AlgorithmResult, new uint?())), (Func<KeyValuePair<DedupIdentifier, long>, DedupIdentifier>) (f => f.Key));
          DedupNode tempNode = new DedupNode((IEnumerable<DedupNode>) nodeToDedupId.Keys);
          PutNodeResponse putNodeResponse;
          using (DedupCompressedBuffer serializedNode = DedupCompressedBuffer.FromUncompressed(tempNode.Serialize()))
            putNodeResponse = await this.client.Client.PutNodeAndKeepUntilReferenceAsync(tempNode.CalculateNodeDedupIdentifier(), serializedNode, this.keepUntilReference, (SummaryKeepUntilReceipt) null, cancellationToken);
          DedupNode? successfulNode = new DedupNode?();
          await putNodeResponse.Match<Task>((Func<DedupNodeChildrenNeedAction, Task>) (async childrenAction =>
          {
            foreach (KeyValuePair<DedupIdentifier, KeepUntilReceipt> receipt in childrenAction.Receipts)
              this.sessionReceipts[receipt.Key] = receipt.Value;
            DedupNode[] array = tempNode.ChildNodes.Where<DedupNode>((Func<DedupNode, bool>) (child => childrenAction.Receipts.ContainsKey(child.GetDedupIdentifier()))).ToArray<DedupNode>();
            if (!((IEnumerable<DedupNode>) array).Any<DedupNode>())
              return;
            DedupNode onlyExistingChildrenNode = new DedupNode((IEnumerable<DedupNode>) array);
            using (DedupCompressedBuffer serializedNode = DedupCompressedBuffer.FromUncompressed(onlyExistingChildrenNode.Serialize()))
              (await this.client.Client.PutNodeAndKeepUntilReferenceAsync(onlyExistingChildrenNode.CalculateNodeDedupIdentifier(), serializedNode, this.keepUntilReference, new SummaryKeepUntilReceipt(((IEnumerable<DedupNode>) array).Select<DedupNode, KeepUntilReceipt>((Func<DedupNode, KeepUntilReceipt>) (c => childrenAction.Receipts[c.GetDedupIdentifier()])).ToArray<KeepUntilReceipt>()), cancellationToken)).Match((Action<DedupNodeChildrenNeedAction>) (childrenAction2 =>
              {
                throw new InvalidOperationException("Should have been able to put a node with a full set of receipts");
              }), (Action<DedupNodeUpdated>) (complete2 =>
              {
                foreach (KeyValuePair<DedupIdentifier, KeepUntilReceipt> receipt in complete2.Receipts)
                  this.sessionReceipts[receipt.Key] = receipt.Value;
                this.AddNode(onlyExistingChildrenNode);
                successfulNode = new DedupNode?(onlyExistingChildrenNode);
              }));
          }), (Func<DedupNodeUpdated, Task>) (complete =>
          {
            foreach (KeyValuePair<DedupIdentifier, KeepUntilReceipt> receipt in complete.Receipts)
              this.sessionReceipts[receipt.Key] = receipt.Value;
            this.AddNode(tempNode);
            successfulNode = new DedupNode?(tempNode);
            return Task.CompletedTask;
          }));
          if (successfulNode.HasValue)
          {
            foreach (DedupNode childNode in (IEnumerable<DedupNode>) successfulNode.Value.ChildNodes)
              results[nodeToDedupId[childNode]] = CheckIfUploadNeededResult.UploadNotNeeded;
          }
          nodeToDedupId = (Dictionary<DedupNode, DedupIdentifier>) null;
        }
        return results;
      }
    }

    private sealed class DownloadedChunk : IDisposable
    {
      public readonly DedupNode Node;
      public readonly uint ChunkIndex;
      public readonly long FileOffset;
      public readonly DedupCompressedBuffer Buffer;

      public DownloadedChunk(
        DedupNode node,
        uint chunkIndex,
        long fileOffset,
        DedupCompressedBuffer buffer)
      {
        this.Node = node;
        this.ChunkIndex = chunkIndex;
        this.FileOffset = fileOffset;
        this.Buffer = buffer;
      }

      public void Dispose() => this.Buffer.Dispose();
    }

    private sealed class Disposer : IDisposable
    {
      private Dictionary<IDisposable, string> disposables;
      private readonly Action<string> logger;

      public Disposer(Action<string> logger = null)
      {
        this.logger = logger;
        this.disposables = (Dictionary<IDisposable, string>) null;
      }

      public T Add<T>(T disposable) where T : IDisposable
      {
        this.disposables = this.disposables ?? new Dictionary<IDisposable, string>();
        this.disposables.Add((IDisposable) disposable, (string) null);
        return disposable;
      }

      public T Add<T>(T disposable, string label) where T : IDisposable
      {
        this.disposables = this.disposables ?? new Dictionary<IDisposable, string>();
        this.disposables.Add((IDisposable) disposable, label);
        return disposable;
      }

      public void Dispose()
      {
        if (this.disposables == null)
          return;
        foreach (KeyValuePair<IDisposable, string> disposable in this.disposables)
        {
          disposable.Key.Dispose();
          if (this.logger != null & disposable.Value != null)
            this.logger(disposable.Value);
        }
        this.disposables = (Dictionary<IDisposable, string>) null;
      }
    }
  }
}
