// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DedupDataPortExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.DataDeduplication.Interop;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public static class DedupDataPortExtensions
  {
    public static Task<DataPortResult> GetResultAsync(this IDedupDataPort dataPort, Guid requestId) => Task.Factory.StartNew<DataPortResult>((Func<DataPortResult>) (() =>
    {
label_0:
      int batchResult;
      uint batchCount;
      DedupDataPortRequestStatus status;
      int[] itemResults;
      while (true)
      {
        try
        {
          dataPort.GetRequestResults(requestId, 1000U, out batchResult, out batchCount, out status, out itemResults);
          break;
        }
        catch (COMException ex) when (ex.HResult == -2147483638)
        {
        }
      }
      switch (status)
      {
        case DedupDataPortRequestStatus.Queued:
        case DedupDataPortRequestStatus.Processing:
          goto label_0;
        case DedupDataPortRequestStatus.Complete:
        case DedupDataPortRequestStatus.Failed:
          return new DataPortResult(batchResult, batchCount, itemResults);
        default:
          throw new InvalidOperationException(status.ToString());
      }
    }), TaskCreationOptions.LongRunning);

    public static async Task<Dictionary<DedupHash, bool>> ContainsChunksAsync(
      this IDedupDataPort dataPort,
      IEnumerable<DedupHash> chunkHashes)
    {
      Dictionary<DedupHash, bool> results = new Dictionary<DedupHash, bool>((IEqualityComparer<DedupHash>) DedupHashComparer.Instance);
      foreach (List<DedupHash> page in chunkHashes.GetPages<DedupHash>(1024))
      {
        Guid requestId;
        dataPort.LookupChunks((uint) page.Count, page.ToArray(), out requestId);
        DataPortResult resultAsync = await dataPort.GetResultAsync(requestId);
        if (resultAsync.BatchResult != 0)
          throw new InvalidOperationException("LookupChunks failed: " + resultAsync.BatchResult.ToString());
        for (int index = 0; index < page.Count; ++index)
          results.Add(page[index], resultAsync.ItemResults[index] == 0);
      }
      Dictionary<DedupHash, bool> dictionary = results;
      results = (Dictionary<DedupHash, bool>) null;
      return dictionary;
    }

    public static async Task<bool> ContainsChunkAsync(
      this IDedupDataPort dataPort,
      DedupHash chunkHash)
    {
      return (await dataPort.ContainsChunksAsync((IEnumerable<DedupHash>) new DedupHash[1]
      {
        chunkHash
      })).Single<KeyValuePair<DedupHash, bool>>().Value;
    }

    public static async Task WriteStreamAsync(
      this IDedupDataPort dataPort,
      DedupNode node,
      string volumeRelativePath)
    {
      ulong chunkOffset = 0;
      DedupStreamEntry[] array = node.EnumerateChunkLeafsInOrder().Select<DedupNode, DedupStreamEntry>((Func<DedupNode, DedupStreamEntry>) (chunk =>
      {
        DedupStreamEntry dedupStreamEntry = new DedupStreamEntry()
        {
          Hash = new DedupHash() { Hash = chunk.Hash },
          LogicalSize = (uint) chunk.TransitiveContentBytes,
          Offset = chunkOffset
        };
        chunkOffset += chunk.TransitiveContentBytes;
        return dedupStreamEntry;
      })).ToArray<DedupStreamEntry>();
      DedupStream[] streams = new DedupStream[1]
      {
        new DedupStream()
        {
          ChunkCount = (uint) array.Length,
          Length = node.TransitiveContentBytes,
          Offset = 0UL,
          Path = volumeRelativePath
        }
      };
      Guid requestId;
      dataPort.CommitStreams((uint) streams.Length, streams, (uint) array.Length, array, out requestId);
      DataPortResult resultAsync = await dataPort.GetResultAsync(requestId);
      if (resultAsync.BatchResult != 0 || resultAsync.ItemResults[0] != 0)
        throw new InvalidOperationException(string.Format("CommitStream failed 0x{0:x} 0x{1:x}", (object) resultAsync.BatchResult, (object) resultAsync.ItemResults[0]));
    }

    public static Task InsertChunkAsync(
      this IDedupDataPort dataPort,
      ChunkDedupIdentifier chunkId,
      DedupCompressedBuffer buffer)
    {
      return dataPort.InsertChunksAsync(new Dictionary<ChunkDedupIdentifier, DedupCompressedBuffer>()
      {
        {
          chunkId,
          buffer
        }
      });
    }

    public static async Task InsertChunksAsync(
      this IDedupDataPort dataPort,
      Dictionary<ChunkDedupIdentifier, DedupCompressedBuffer> buffers)
    {
      long length = 0;
      int num = 0;
      foreach (KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer> buffer1 in buffers)
      {
        ArraySegment<byte> buffer2;
        buffer1.Value.GetBytes(out bool _, out buffer2);
        if (buffer2.Count == 0)
          throw new ArgumentException("Cannot insert a zero-length chunk.");
        length += (long) buffer2.Count;
        ++num;
      }
      DedupChunk[] chunkMetadata = new DedupChunk[buffers.Count];
      byte[] numArray = new byte[length];
      int index = 0;
      int dstOffset = 0;
      foreach (KeyValuePair<ChunkDedupIdentifier, DedupCompressedBuffer> buffer3 in buffers)
      {
        bool isCompressed;
        ArraySegment<byte> buffer4;
        buffer3.Value.GetBytes(out isCompressed, out buffer4);
        chunkMetadata[index] = new DedupChunk()
        {
          DataSize = (uint) buffer4.Count,
          Flags = isCompressed ? DedupChunkFlags.Compressed : DedupChunkFlags.None,
          LogicalSize = (uint) buffer3.Value.Uncompressed.Count,
          Hash = new DedupHash()
          {
            Hash = buffer3.Key.AlgorithmResult
          }
        };
        System.Buffer.BlockCopy((Array) buffer4.Array, buffer4.Offset, (Array) numArray, dstOffset, buffer4.Count);
        ++index;
        dstOffset += buffer4.Count;
      }
      Guid requestId;
      dataPort.InsertChunks((uint) chunkMetadata.Length, chunkMetadata, (uint) numArray.Length, numArray, out requestId);
      DataPortResult resultAsync = await dataPort.GetResultAsync(requestId);
      if (resultAsync.BatchResult != 0)
        throw new InvalidOperationException("InsertChunks batch failed: " + resultAsync.BatchResult.ToString());
      if (((IEnumerable<int>) resultAsync.ItemResults).Any<int>((Func<int, bool>) (r => r != 0 && r != 1)))
        throw new InvalidOperationException("InsertChunks failed: " + string.Join<int>(",", (IEnumerable<int>) resultAsync.ItemResults));
    }
  }
}
