// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.IDomainDedupStoreHttpClient
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
  public interface IDomainDedupStoreHttpClient : IArtifactHttpClient
  {
    Task<Dictionary<DedupIdentifier, PreauthenticatedUri>> GetDedupUrlsAsync(
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      EdgeCache edgeCache,
      CancellationToken cancellationToken);

    Task<Dictionary<DedupIdentifier, GetDedupAsyncFunc>> GetDedupGettersAsync(
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken,
      bool haveProxyRetrieveSasUris = false);

    Task<Dictionary<ChunkDedupIdentifier, KeepUntilReceipt>> PutChunksAsync(
      IDomainId domainId,
      Dictionary<ChunkDedupIdentifier, DedupCompressedBuffer> chunks,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken);

    Task<KeepUntilReceipt> PutChunkAndKeepUntilReferenceAsync(
      IDomainId domainId,
      ChunkDedupIdentifier chunkId,
      DedupCompressedBuffer chunk,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken);

    Task<MaybeCached<DedupCompressedBuffer>> GetChunkAsync(
      IDomainId domainId,
      ChunkDedupIdentifier chunkId,
      bool canRedirect,
      CancellationToken cancellationToken);

    Task<KeepUntilReceipt> TryKeepUntilReferenceChunkAsync(
      IDomainId domainId,
      ChunkDedupIdentifier chunkId,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken);

    Task<PutNodeResponse> PutNodeAndKeepUntilReferenceAsync(
      IDomainId domainId,
      NodeDedupIdentifier nodeId,
      DedupCompressedBuffer node,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt,
      CancellationToken cancellationToken);

    Task<MaybeCached<DedupCompressedBuffer>> GetNodeAsync(
      IDomainId domainId,
      NodeDedupIdentifier nodeId,
      bool canRedirect,
      CancellationToken cancellationToken);

    Task<TryReferenceNodeResponse> TryKeepUntilReferenceNodeAsync(
      IDomainId domainId,
      NodeDedupIdentifier nodeId,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt,
      CancellationToken cancellationToken);

    Task PutRootAsync(
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      CancellationToken cancellationToken);

    Task DeleteRootAsync(
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      CancellationToken cancellationToken);

    Task<DedupDownloadInfo> GetDownloadInfoAsync(
      IDomainId domainId,
      DedupIdentifier dedupId,
      bool includeChunks,
      CancellationToken cancellationToken);

    Task<IList<DedupDownloadInfo>> GetBatchDownloadInfoAsync(
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      bool includeChunks,
      CancellationToken cancellationToken);

    void SetRedirectTimeout(int? redirectTimeOutSeconds);

    long Calls { get; }

    long ThrottledCalls { get; }

    long XCacheHits { get; }

    long XCacheMisses { get; }

    int RecommendedChunkCountPerCall { get; set; }
  }
}
