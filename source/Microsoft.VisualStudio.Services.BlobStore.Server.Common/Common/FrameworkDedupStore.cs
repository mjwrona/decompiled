// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.FrameworkDedupStore
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class FrameworkDedupStore : ArtifactsServiceBase, IDedupStore, IVssFrameworkService
  {
    public bool CanRedirect = true;

    protected override string ProductTraceArea => "DedupStore";

    public Task DeleteRootAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef)
    {
      return this.GetHttpClient(requestContext, domainId).DeleteRootAsync(domainId, dedupId, rootRef, requestContext.CancellationToken);
    }

    public async Task<DedupCompressedBuffer> GetChunkAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ChunkDedupIdentifier chunkId)
    {
      return (await this.GetHttpClient(requestContext, domainId).GetChunkAsync(domainId, chunkId, this.CanRedirect, requestContext.CancellationToken).ConfigureAwait(false)).Value;
    }

    public async Task<DedupCompressedBuffer> GetNodeAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      NodeDedupIdentifier nodeId)
    {
      return (await this.GetHttpClient(requestContext, domainId).GetNodeAsync(domainId, nodeId, this.CanRedirect, requestContext.CancellationToken).ConfigureAwait(false)).Value;
    }

    public async Task<DedupDownloadInfo> GetDownloadInfoAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      bool includeChunks)
    {
      return await this.GetHttpClient(requestContext, domainId).GetDownloadInfoAsync(domainId, dedupId, includeChunks, requestContext.CancellationToken).ConfigureAwait(false);
    }

    public Task<IList<DedupDownloadInfo>> GetDownloadInfoBatchAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      bool includeChunks)
    {
      return this.GetHttpClient(requestContext, domainId).GetBatchDownloadInfoAsync(domainId, dedupIds, includeChunks, requestContext.CancellationToken);
    }

    public async Task<IDictionary<DedupIdentifier, PreauthenticatedUri>> GetUris(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      EdgeCache edgeCache)
    {
      return (IDictionary<DedupIdentifier, PreauthenticatedUri>) await this.GetHttpClient(requestContext, domainId).GetDedupUrlsAsync(domainId, dedupIds, edgeCache, requestContext.CancellationToken);
    }

    public Task<KeepUntilReceipt> PutChunkAndKeepUntilReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ChunkDedupIdentifier chunkId,
      DedupCompressedBuffer chunk,
      KeepUntilBlobReference keepUntil)
    {
      return this.GetHttpClient(requestContext, domainId).PutChunkAndKeepUntilReferenceAsync(domainId, chunkId, chunk, keepUntil, requestContext.CancellationToken);
    }

    public Task<PutNodeResponse> PutNodeAndKeepUntilReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      NodeDedupIdentifier nodeId,
      DedupCompressedBuffer node,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipts)
    {
      return this.GetHttpClient(requestContext, domainId).PutNodeAndKeepUntilReferenceAsync(domainId, nodeId, node, keepUntil, receipts, requestContext.CancellationToken);
    }

    public Task PutRootAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef)
    {
      return this.GetHttpClient(requestContext, domainId).PutRootAsync(domainId, dedupId, rootRef, requestContext.CancellationToken);
    }

    public Task<KeepUntilReceipt> TryKeepUntilReferenceChunkAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ChunkDedupIdentifier chunkId,
      KeepUntilBlobReference keepUntil)
    {
      return this.GetHttpClient(requestContext, domainId).TryKeepUntilReferenceChunkAsync(domainId, chunkId, keepUntil, requestContext.CancellationToken);
    }

    public Task<TryReferenceNodeResponse> TryKeepUntilReferenceNodeAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      NodeDedupIdentifier nodeId,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipts)
    {
      return this.GetHttpClient(requestContext, domainId).TryKeepUntilReferenceNodeAsync(domainId, nodeId, keepUntil, receipts, requestContext.CancellationToken);
    }

    public Task<DedupValidationResult> VerifyDedupAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier root,
      DedupTraversalConfig config)
    {
      throw new NotImplementedException("Cannot initiate verification from another service.");
    }

    public Task<RootsValidationResult> VerifyRootsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      RootsValidationConfig config)
    {
      throw new NotImplementedException("Cannot initiate verification from another service.");
    }

    public Task<DateTime> GetKeepUntilAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId)
    {
      throw new NotImplementedException("Cannot read KeepUntils from another service.");
    }

    internal virtual IDomainDedupStoreHttpClient GetHttpClient(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      if (domainId == (IDomainId) null)
        throw new ArgumentNullException(nameof (domainId));
      return (IDomainDedupStoreHttpClient) requestContext.Elevate().GetClient<DomainDedupStoreHttpClient>(ServiceInstanceTypes.BlobStore);
    }
  }
}
