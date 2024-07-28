// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IDedupStore
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  [DefaultServiceImplementation(typeof (FrameworkDedupStore))]
  public interface IDedupStore : IVssFrameworkService
  {
    Task<IDictionary<DedupIdentifier, PreauthenticatedUri>> GetUris(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      EdgeCache edgeCache);

    Task<DedupDownloadInfo> GetDownloadInfoAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      bool includeChunks);

    Task<IList<DedupDownloadInfo>> GetDownloadInfoBatchAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ISet<DedupIdentifier> dedupIds,
      bool includeChunks);

    Task<KeepUntilReceipt> PutChunkAndKeepUntilReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ChunkDedupIdentifier chunkId,
      DedupCompressedBuffer chunk,
      KeepUntilBlobReference keepUntil);

    Task<DedupCompressedBuffer> GetChunkAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ChunkDedupIdentifier chunkId);

    Task<KeepUntilReceipt> TryKeepUntilReferenceChunkAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ChunkDedupIdentifier chunkId,
      KeepUntilBlobReference keepUntil);

    Task<PutNodeResponse> PutNodeAndKeepUntilReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      NodeDedupIdentifier nodeId,
      DedupCompressedBuffer node,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipts);

    Task<DedupCompressedBuffer> GetNodeAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      NodeDedupIdentifier nodeId);

    Task<TryReferenceNodeResponse> TryKeepUntilReferenceNodeAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      NodeDedupIdentifier nodeId,
      KeepUntilBlobReference keepUntil,
      SummaryKeepUntilReceipt receipts);

    Task PutRootAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef);

    Task DeleteRootAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      IdBlobReference rootRef);

    Task<DedupValidationResult> VerifyDedupAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier root,
      DedupTraversalConfig config);

    Task<RootsValidationResult> VerifyRootsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      RootsValidationConfig config);

    Task<DateTime> GetKeepUntilAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId);
  }
}
