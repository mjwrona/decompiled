// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ChunkerHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common.Exceptions;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public static class ChunkerHelper
  {
    public static readonly HashType DefaultChunkHashType = HashType.Dedup64K;
    private const int SmallBufferSize = 131072;
    private const int LargeBufferSize = 2097152;
    private static readonly Dictionary<HashType, IContentHasher> infoByType = new Dictionary<HashType, IContentHasher>()
    {
      {
        HashType.Dedup64K,
        DedupNode64KHashInfo.Instance.CreateContentHasher()
      },
      {
        HashType.Dedup1024K,
        Dedup1024KHashInfo.Instance.CreateContentHasher()
      }
    };
    private static readonly ByteArrayPool NodeBufferPool = new ByteArrayPool(131072, 4 * Environment.ProcessorCount);
    private static readonly ByteArrayPool ChunkBufferPool = new ByteArrayPool(2097152, 4 * Environment.ProcessorCount);
    private static Pool<IChunker> chunkerPool = new Pool<IChunker>((Func<IChunker>) (() => Chunker.Create(ChunkerConfiguration.SupportedComChunkerConfiguration)), (Action<IChunker>) (chunker => { }), 4 * Environment.ProcessorCount);

    public static void ResizeChunkBufferPool(int maxToKeep) => ChunkerHelper.ChunkBufferPool.Resize(maxToKeep);

    public static void ResizeNodeBufferPool(int maxToKeep) => ChunkerHelper.NodeBufferPool.Resize(maxToKeep);

    public static IPoolHandle<byte[]> BorrowChunkBuffer(int size) => (IPoolHandle<byte[]>) (size <= 131072 ? ChunkerHelper.NodeBufferPool.Get() : ChunkerHelper.ChunkBufferPool.Get());

    public static IPoolHandle<byte[]> BorrowNodeBuffer() => (IPoolHandle<byte[]>) ChunkerHelper.NodeBufferPool.Get();

    public static int GetChunkBufferSize(int size) => size > 131072 ? 2097152 : 131072;

    public static async Task<DedupNode> CreateFromFileAsync(
      IFileSystem fileSystem,
      string path,
      CancellationToken cancellationToken,
      bool configureAwait)
    {
      return await ChunkerHelper.CreateFromFileAsync(fileSystem, path, configureAwait, ChunkerHelper.DefaultChunkHashType, cancellationToken).ConfigureAwait(configureAwait);
    }

    public static async Task<DedupNode> CreateFromFileAsync(
      IFileSystem fileSystem,
      string path,
      bool configureAwait,
      HashType hashType,
      CancellationToken cancellationToken)
    {
      DedupNode fromFileAsync;
      using (Stream file = fileSystem.OpenStreamForFile(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
        fromFileAsync = await ChunkerHelper.CreateFromStreamAsync(file, configureAwait, hashType, cancellationToken).ConfigureAwait(configureAwait);
      return fromFileAsync;
    }

    public static async Task WalkTreeDepthFirstAsync(
      DedupNode root,
      ChunkerHelper.NodeCallbackAsync nodeCallbackAsync,
      ChunkerHelper.ChunkCallbackAsync chunkCallbackAsync,
      CancellationToken cancellationToken,
      bool configureAwait,
      ulong offset = 0)
    {
      switch (root.Type)
      {
        case DedupNode.NodeType.ChunkLeaf:
          await chunkCallbackAsync(root, offset).ConfigureAwait(configureAwait);
          break;
        case DedupNode.NodeType.InnerNode:
          switch (await nodeCallbackAsync(root, offset).ConfigureAwait(configureAwait))
          {
            case ChunkerHelper.NodeCallbackResult.Done:
              return;
            case ChunkerHelper.NodeCallbackResult.WalkChildren:
              foreach (DedupNode childNode in (IEnumerable<DedupNode>) root.ChildNodes)
              {
                DedupNode child = childNode;
                await ChunkerHelper.WalkTreeDepthFirstAsync(child, nodeCallbackAsync, chunkCallbackAsync, cancellationToken, configureAwait, offset).ConfigureAwait(configureAwait);
                offset += child.TransitiveContentBytes;
                child = new DedupNode();
              }
              return;
            default:
              throw new InvalidOperationException();
          }
        default:
          throw new InvalidOperationException();
      }
    }

    public static async Task<DedupNode> CreateFromStreamAsync(
      Stream content,
      CancellationToken cancellationToken,
      bool configureAwait)
    {
      return await ChunkerHelper.CreateFromStreamAsync(content, configureAwait, ChunkerHelper.DefaultChunkHashType, cancellationToken);
    }

    public static async Task<DedupNode> CreateFromStreamAsync(
      Stream content,
      bool configureAwait,
      HashType hashType,
      CancellationToken cancellationToken)
    {
      HasherToken hashToken = ChunkerHelper.infoByType[hashType].CreateToken();
      DedupNodeOrChunkHashAlgorithm hasher;
      DedupNode node;
      try
      {
        hasher = (DedupNodeOrChunkHashAlgorithm) hashToken.Hasher;
        Pool<byte[]>.PoolHandle bufferHandle = ChunkerHelper.ChunkBufferPool.Get();
        try
        {
          byte[] buffer = bufferHandle.Value;
          while (true)
          {
            int inputCount;
            if ((inputCount = await content.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(configureAwait)) != 0)
              hasher.TransformBlock(buffer, 0, inputCount, (byte[]) null, 0);
            else
              break;
          }
          hasher.TransformFinalBlock(buffer, 0, 0);
          buffer = (byte[]) null;
        }
        finally
        {
          bufferHandle.Dispose();
        }
        bufferHandle = new Pool<byte[]>.PoolHandle();
        node = hasher.GetNode();
      }
      finally
      {
        hashToken.Dispose();
      }
      hashToken = new HasherToken();
      hasher = (DedupNodeOrChunkHashAlgorithm) null;
      return node;
    }

    public static async Task<byte[]> CreateHashSingleChunkFromStreamAsync(
      Stream content,
      CancellationToken cancellationToken,
      bool configureAwait)
    {
      byte[] hash;
      using (HasherToken hashToken = DedupSingleChunkHashInfo.Instance.CreateContentHasher().CreateToken())
      {
        DedupChunkHashAlgorithm hasher = (DedupChunkHashAlgorithm) hashToken.Hasher;
        Pool<byte[]>.PoolHandle bufferHandle = ChunkerHelper.ChunkBufferPool.Get();
        try
        {
          byte[] buffer = bufferHandle.Value;
          while (true)
          {
            int inputCount;
            if ((inputCount = await content.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(configureAwait)) != 0)
              hasher.TransformBlock(buffer, 0, inputCount, (byte[]) null, 0);
            else
              break;
          }
          hasher.TransformFinalBlock(buffer, 0, 0);
          buffer = (byte[]) null;
        }
        finally
        {
          bufferHandle.Dispose();
        }
        bufferHandle = new Pool<byte[]>.PoolHandle();
        hash = hasher.Hash;
      }
      return hash;
    }

    public static DedupNode CreateFromStream(Stream content) => ChunkerHelper.CreateFromStream(content, ChunkerHelper.DefaultChunkHashType);

    public static DedupNode CreateFromStream(Stream content, HashType hashType)
    {
      using (HasherToken token = ChunkerHelper.infoByType[hashType].CreateToken())
      {
        DedupNodeOrChunkHashAlgorithm hasher = (DedupNodeOrChunkHashAlgorithm) token.Hasher;
        using (Pool<byte[]>.PoolHandle poolHandle = ChunkerHelper.ChunkBufferPool.Get())
        {
          byte[] numArray = poolHandle.Value;
          int inputCount;
          while ((inputCount = content.Read(numArray, 0, numArray.Length)) != 0)
            hasher.TransformBlock(numArray, 0, inputCount, (byte[]) null, 0);
          hasher.TransformFinalBlock(numArray, 0, 0);
        }
        return hasher.GetNode();
      }
    }

    public static bool IsHashTypeChunk(HashType hashType) => hashType == HashType.Dedup64K || hashType == HashType.Dedup1024K;

    public static void ValidateHashType(HashType hashType, bool isChunked)
    {
      if (isChunked && !ChunkerHelper.IsHashTypeChunk(hashType) || !isChunked && ChunkerHelper.IsHashTypeChunk(hashType))
        throw new MismatchedHashTypeException(isChunked, hashType);
    }

    public static bool IsNodeOrChunk(BlobIdentifier blobId) => ChunkerHelper.IsNode(blobId.AlgorithmId) || ChunkerHelper.IsChunk(blobId.AlgorithmId);

    public static bool IsNode(byte algorithmId) => (byte) 2 == algorithmId || (byte) 6 == algorithmId;

    public static bool IsChunk(byte algorithmId) => (byte) 1 == algorithmId;

    public static HashType GetHashType(byte algorithmId)
    {
      if (algorithmId == (byte) 2)
        return HashType.Dedup64K;
      if (algorithmId == (byte) 6)
        return HashType.Dedup1024K;
      throw new ArgumentException("Invalid algorithm Id");
    }

    public static bool IsValidHashType(HashType hashType) => hashType == HashType.Dedup64K || hashType == HashType.Dedup1024K || hashType == HashType.Vso0;

    public static int GetMaxChunkContentSize() => 2097152;

    public static int GetMaxNodeContentSize() => 131072;

    public enum NodeCallbackResult
    {
      Done,
      WalkChildren,
    }

    public delegate Task<ChunkerHelper.NodeCallbackResult> NodeCallbackAsync(
      DedupNode node,
      ulong offset);

    public delegate Task ChunkCallbackAsync(DedupNode chunk, ulong offset);
  }
}
