// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.IDedupStoreHttpClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public interface IDedupStoreHttpClient : IArtifactHttpClient
  {
    Task<Dictionary<DedupIdentifier, PreauthenticatedUri>> GetDedupUrlsAsync(
      ISet<DedupIdentifier> dedupIds,
      EdgeCache edgeCache,
      CancellationToken cancellationToken);

    Task<Dictionary<DedupIdentifier, GetDedupAsyncFunc>> GetDedupGettersAsync(
      ISet<DedupIdentifier> dedupIds,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken,
      bool haveProxyRetrieveSasUris = false);

    Task<Dictionary<ChunkDedupIdentifier, KeepUntilReceipt>> PutChunksAsync(
      Dictionary<ChunkDedupIdentifier, DedupCompressedBuffer> chunks,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken);

    Task<KeepUntilReceipt> PutChunkAndKeepUntilReferenceAsync(
      ChunkDedupIdentifier chunkId,
      DedupCompressedBuffer chunk,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken);

    Task<MaybeCached<DedupCompressedBuffer>> GetChunkAsync(
      ChunkDedupIdentifier chunkId,
      bool canRedirect,
      CancellationToken cancellationToken);

    Task<KeepUntilReceipt> TryKeepUntilReferenceChunkAsync(
      ChunkDedupIdentifier chunkId,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken);

    Task<PutNodeResponse> PutNodeAndKeepUntilReferenceAsync(
      NodeDedupIdentifier nodeId,
      DedupCompressedBuffer node,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt,
      CancellationToken cancellationToken);

    Task<MaybeCached<DedupCompressedBuffer>> GetNodeAsync(
      NodeDedupIdentifier nodeId,
      bool canRedirect,
      CancellationToken cancellationToken);

    Task<TryReferenceNodeResponse> TryKeepUntilReferenceNodeAsync(
      NodeDedupIdentifier nodeId,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt,
      CancellationToken cancellationToken);

    Task PostEchoAsync(
      byte[] echoBytes,
      bool hash,
      bool base64,
      bool echo,
      bool vsoHash,
      bool storeInBlobStore,
      CancellationToken cancellationToken);

    Task PutRootAsync(
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      CancellationToken cancellationToken);

    Task DeleteRootAsync(
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      CancellationToken cancellationToken);

    Task<DedupDownloadInfo> GetDownloadInfoAsync(
      DedupIdentifier dedupId,
      bool includeChunks,
      CancellationToken cancellationToken);

    Task<IList<DedupDownloadInfo>> GetBatchDownloadInfoAsync(
      ISet<DedupIdentifier> dedupIds,
      bool includeChunks,
      CancellationToken cancellationToken);

    void SetRedirectTimeout(int? redirectTimeoutSeconds);

    long Calls { get; }

    long ThrottledCalls { get; }

    long XCacheHits { get; }

    long XCacheMisses { get; }

    int RecommendedChunkCountPerCall { get; set; }
  }
}
