// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.StreamChunker
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public class StreamChunker
  {
    public const int IdentifiersPerNode = 512;
    private IDedupProcessingQueue processingQueue;
    private readonly CancellationToken cancellationToken;
    private static readonly ByteArrayPool bufferPool = new ByteArrayPool(ChunkerHelper.GetMaxChunkContentSize(), 100);
    private readonly LinkedList<Pool<byte[]>.PoolHandle> processedBuffers;
    private DedupTreeBuilder treeBuilder;
    private ulong bufferStartPositionInStream;

    public StreamChunker(IDedupProcessingQueue processingQueue, CancellationToken cancellationToken)
    {
      this.processingQueue = processingQueue;
      this.cancellationToken = cancellationToken;
      this.processedBuffers = new LinkedList<Pool<byte[]>.PoolHandle>();
      this.treeBuilder = new DedupTreeBuilder(processingQueue);
    }

    public async Task<DedupIdentifier> CreateFromStreamAsync(Stream stream)
    {
      StreamChunker streamChunker = this;
      using (IChunker chunker = Chunker.Create(ChunkerConfiguration.SupportedComChunkerConfiguration))
      {
        // ISSUE: reference to a compiler-generated method
        using (IChunkerSession session = chunker.BeginChunking(new Action<ChunkInfo>(streamChunker.\u003CCreateFromStreamAsync\u003Eb__8_0)))
        {
          Pool<byte[]>.PoolHandle buffer = StreamChunker.bufferPool.Get();
          while (true)
          {
            int count;
            if ((count = await streamChunker.FillBufferAsync(buffer.Value, stream)) > 0)
            {
              streamChunker.processedBuffers.AddLast(buffer);
              session.PushBuffer(buffer.Value, 0, count);
              buffer = StreamChunker.bufferPool.Get();
              streamChunker.cancellationToken.ThrowIfCancellationRequested();
            }
            else
              break;
          }
          buffer = new Pool<byte[]>.PoolHandle();
        }
      }
      DedupIdentifier rootNodeId = streamChunker.treeBuilder.CreateRootNode();
      await streamChunker.processingQueue.FlushAsync();
      DedupIdentifier fromStreamAsync = rootNodeId;
      rootNodeId = (DedupIdentifier) null;
      return fromStreamAsync;
    }

    private async Task<int> FillBufferAsync(byte[] buffer, Stream stream)
    {
      int totalRead = 0;
      int num;
      do
      {
        num = await stream.ReadAsync(buffer, totalRead, buffer.Length - totalRead);
        totalRead += num;
      }
      while (num > 0 && totalRead < buffer.Length);
      return totalRead;
    }

    private void ProcessChunkInfo(ChunkInfo info)
    {
      DedupNode dedupNode = new DedupNode(info);
      byte[] chunk = this.ExtractChunk(info);
      this.treeBuilder.AddNode(dedupNode);
      this.processingQueue.Add((DedupIdentifier) dedupNode.GetChunkIdentifier(), chunk);
    }

    private byte[] ExtractChunk(ChunkInfo info)
    {
      uint size = info.Size;
      byte[] destinationArray = new byte[(int) info.Size];
      uint destinationIndex = 0;
      while (size > 0U)
      {
        byte[] bufferAt = this.MoveToBufferAt(info.Offset + (ulong) destinationIndex);
        long sourceIndex = (long) info.Offset + (long) destinationIndex - (long) this.bufferStartPositionInStream;
        uint length = Math.Min((uint) ((ulong) bufferAt.Length - (ulong) sourceIndex), size);
        Array.Copy((Array) bufferAt, sourceIndex, (Array) destinationArray, (long) destinationIndex, (long) length);
        size -= length;
        destinationIndex += length;
      }
      return destinationArray;
    }

    private byte[] MoveToBufferAt(ulong offset)
    {
      offset -= this.bufferStartPositionInStream;
      Pool<byte[]>.PoolHandle poolHandle;
      for (poolHandle = this.processedBuffers.First.Value; offset >= (ulong) (uint) poolHandle.Value.Length; poolHandle = this.processedBuffers.First.Value)
      {
        uint length = (uint) poolHandle.Value.Length;
        offset -= (ulong) length;
        this.bufferStartPositionInStream += (ulong) length;
        this.processedBuffers.RemoveFirst();
        poolHandle.Dispose();
      }
      return poolHandle.Value;
    }
  }
}
