// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DomainHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class DomainHttpClientWrapper : IDedupStoreHttpClient, IArtifactHttpClient
  {
    private readonly IDomainDedupStoreHttpClient multiDomainDedupStoreHttpClient;
    private readonly IDomainId domainId;

    public DomainHttpClientWrapper(IDomainId domainId, IDomainDedupStoreHttpClient client)
    {
      this.multiDomainDedupStoreHttpClient = client;
      this.domainId = domainId;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public IDomainId DomainId => this.domainId;

    public long Calls => this.multiDomainDedupStoreHttpClient.Calls;

    public long ThrottledCalls => this.multiDomainDedupStoreHttpClient.ThrottledCalls;

    public long XCacheHits => this.multiDomainDedupStoreHttpClient.XCacheHits;

    public long XCacheMisses => this.multiDomainDedupStoreHttpClient.XCacheMisses;

    public int RecommendedChunkCountPerCall
    {
      get => this.multiDomainDedupStoreHttpClient.RecommendedChunkCountPerCall;
      set => this.multiDomainDedupStoreHttpClient.RecommendedChunkCountPerCall = value;
    }

    public Uri BaseAddress => this.multiDomainDedupStoreHttpClient.BaseAddress;

    public async Task DeleteRootAsync(
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      CancellationToken cancellationToken)
    {
      await this.multiDomainDedupStoreHttpClient.DeleteRootAsync(this.domainId, dedupId, rootRef, cancellationToken);
    }

    public async Task<IList<DedupDownloadInfo>> GetBatchDownloadInfoAsync(
      ISet<DedupIdentifier> dedupIds,
      bool includeChunks,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainDedupStoreHttpClient.GetBatchDownloadInfoAsync(this.domainId, dedupIds, includeChunks, cancellationToken);
    }

    public async Task<MaybeCached<DedupCompressedBuffer>> GetChunkAsync(
      ChunkDedupIdentifier chunkId,
      bool canRedirect,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainDedupStoreHttpClient.GetChunkAsync(this.domainId, chunkId, canRedirect, cancellationToken);
    }

    public async Task<Dictionary<DedupIdentifier, GetDedupAsyncFunc>> GetDedupGettersAsync(
      ISet<DedupIdentifier> dedupIds,
      Uri proxyUri,
      EdgeCache edgeCache,
      CancellationToken cancellationToken,
      bool haveProxyRetrieveSasUris = false)
    {
      return await this.multiDomainDedupStoreHttpClient.GetDedupGettersAsync(this.domainId, dedupIds, proxyUri, edgeCache, cancellationToken, haveProxyRetrieveSasUris);
    }

    public async Task<Dictionary<DedupIdentifier, PreauthenticatedUri>> GetDedupUrlsAsync(
      ISet<DedupIdentifier> dedupIds,
      EdgeCache edgeCache,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainDedupStoreHttpClient.GetDedupUrlsAsync(this.domainId, dedupIds, edgeCache, cancellationToken);
    }

    public async Task<DedupDownloadInfo> GetDownloadInfoAsync(
      DedupIdentifier dedupId,
      bool includeChunks,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainDedupStoreHttpClient.GetDownloadInfoAsync(this.domainId, dedupId, includeChunks, cancellationToken);
    }

    public async Task<MaybeCached<DedupCompressedBuffer>> GetNodeAsync(
      NodeDedupIdentifier nodeId,
      bool canRedirect,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainDedupStoreHttpClient.GetNodeAsync(this.domainId, nodeId, canRedirect, cancellationToken);
    }

    public async Task GetOptionsAsync(CancellationToken cancellationToken) => await this.multiDomainDedupStoreHttpClient.GetOptionsAsync(cancellationToken);

    public Task PostEchoAsync(
      byte[] echoBytes,
      bool hash,
      bool base64,
      bool echo,
      bool vsoHash,
      bool storeInBlobStore,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public async Task<KeepUntilReceipt> PutChunkAndKeepUntilReferenceAsync(
      ChunkDedupIdentifier chunkId,
      DedupCompressedBuffer chunk,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainDedupStoreHttpClient.PutChunkAndKeepUntilReferenceAsync(this.domainId, chunkId, chunk, keepUntil, cancellationToken);
    }

    public async Task<Dictionary<ChunkDedupIdentifier, KeepUntilReceipt>> PutChunksAsync(
      Dictionary<ChunkDedupIdentifier, DedupCompressedBuffer> chunks,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainDedupStoreHttpClient.PutChunksAsync(this.domainId, chunks, keepUntil, cancellationToken);
    }

    public async Task<PutNodeResponse> PutNodeAndKeepUntilReferenceAsync(
      NodeDedupIdentifier nodeId,
      DedupCompressedBuffer node,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainDedupStoreHttpClient.PutNodeAndKeepUntilReferenceAsync(this.domainId, nodeId, node, keepUntil, receipt, cancellationToken);
    }

    public async Task PutRootAsync(
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      CancellationToken cancellationToken)
    {
      await this.multiDomainDedupStoreHttpClient.PutRootAsync(this.domainId, dedupId, rootRef, cancellationToken);
    }

    public void SetTracer(IAppTraceSource tracer) => this.multiDomainDedupStoreHttpClient.SetTracer(tracer);

    public void SetRedirectTimeout(int? redirectTimeOutSeconds) => this.multiDomainDedupStoreHttpClient.SetRedirectTimeout(redirectTimeOutSeconds);

    public async Task<KeepUntilReceipt> TryKeepUntilReferenceChunkAsync(
      ChunkDedupIdentifier chunkId,
      KeepUntilBlobReference keepUntil,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainDedupStoreHttpClient.TryKeepUntilReferenceChunkAsync(this.domainId, chunkId, keepUntil, cancellationToken);
    }

    public async Task<TryReferenceNodeResponse> TryKeepUntilReferenceNodeAsync(
      NodeDedupIdentifier nodeId,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipt,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainDedupStoreHttpClient.TryKeepUntilReferenceNodeAsync(this.domainId, nodeId, keepUntil, receipt, cancellationToken);
    }
  }
}
