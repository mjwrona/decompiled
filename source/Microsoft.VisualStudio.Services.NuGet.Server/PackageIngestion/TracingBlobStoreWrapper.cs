// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.TracingBlobStoreWrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  internal class TracingBlobStoreWrapper : IBlobStore, IVssFrameworkService
  {
    private static readonly TraceData TraceData = NuGetTracePoints.TracingBlobStoreWrapper.TraceData;
    private readonly IBlobStore realStore;

    public TracingBlobStoreWrapper(IBlobStore realStore) => this.realStore = realStore;

    public async Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> referenceIdsGroupedByBlobIds)
    {
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> dictionary;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingBlobStoreWrapper.TraceData, 5722900, nameof (TryReferenceAsync)))
      {
        tracer.TraceInfo(string.Format("TryReferenceAsync {0}", (object) TracingBlobStoreWrapper.FormatBlobReferences<BlobIdentifier>(referenceIdsGroupedByBlobIds)), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        dictionary = await this.realStore.TryReferenceAsync(requestContext, domainId, referenceIdsGroupedByBlobIds).ConfigureAwait(true);
      }
      return dictionary;
    }

    public async Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceWithBlocksAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> referenceIdsGroupedByBlobIds)
    {
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> dictionary;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingBlobStoreWrapper.TraceData, 5722910, nameof (TryReferenceWithBlocksAsync)))
      {
        tracer.TraceInfo(string.Format("TryReferenceWithBlocksAsync {0}", (object) TracingBlobStoreWrapper.FormatBlobReferences<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>(referenceIdsGroupedByBlobIds)), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        dictionary = await this.realStore.TryReferenceWithBlocksAsync(requestContext, domainId, referenceIdsGroupedByBlobIds).ConfigureAwait(true);
      }
      return dictionary;
    }

    public async Task RemoveReferencesAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>> referenceIdsGroupedByBlobIds)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingBlobStoreWrapper.TraceData, 5722920, nameof (RemoveReferencesAsync)))
      {
        tracer.TraceInfo(string.Format("RemoveReferencesAsync {0}", (object) TracingBlobStoreWrapper.FormatBlobReferences<BlobIdentifier>(referenceIdsGroupedByBlobIds)), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        await this.realStore.RemoveReferencesAsync(requestContext, domainId, referenceIdsGroupedByBlobIds).ConfigureAwait(true);
      }
    }

    public async Task<PreauthenticatedUri> GetDownloadUriAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdWithHeaders blobIdWithHeaders)
    {
      PreauthenticatedUri downloadUriAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingBlobStoreWrapper.TraceData, 5722980, nameof (GetDownloadUriAsync)))
      {
        tracer.TraceInfo(string.Format(nameof (GetDownloadUriAsync)), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        downloadUriAsync = await this.realStore.GetDownloadUriAsync(requestContext, domainId, blobIdWithHeaders).ConfigureAwait(true);
      }
      return downloadUriAsync;
    }

    public async Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IEnumerable<BlobIdentifier> blobIds,
      EdgeCache edgeCache,
      DateTimeOffset? expiryTime)
    {
      IDictionary<BlobIdentifier, PreauthenticatedUri> downloadUrisAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingBlobStoreWrapper.TraceData, 5722930, nameof (GetDownloadUrisAsync)))
      {
        tracer.TraceInfo(string.Format(nameof (GetDownloadUrisAsync)), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        downloadUrisAsync = await this.realStore.GetDownloadUrisAsync(requestContext, domainId, blobIds, edgeCache, expiryTime).ConfigureAwait(true);
      }
      return downloadUrisAsync;
    }

    public async Task<Stream> GetBlobAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId)
    {
      Stream blobAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingBlobStoreWrapper.TraceData, 5722940, nameof (GetBlobAsync)))
      {
        tracer.TraceInfo(string.Format("GetBlobAsync {0}", (object) blobId), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        blobAsync = await this.realStore.GetBlobAsync(requestContext, domainId, blobId).ConfigureAwait(true);
      }
      return blobAsync;
    }

    public async Task PutSingleBlockBlobAndReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      byte[] blobBuffer,
      int blockLength,
      BlobReference referenceId)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingBlobStoreWrapper.TraceData, 5722950, nameof (PutSingleBlockBlobAndReferenceAsync)))
      {
        tracer.TraceInfo(string.Format("PutSingleBlockBlobAndReferenceAsync {0} => {1}", (object) blobId, (object) referenceId), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        await this.realStore.PutSingleBlockBlobAndReferenceAsync(requestContext, domainId, blobId, blobBuffer, blockLength, referenceId).ConfigureAwait(true);
      }
    }

    public async Task PutBlobBlockAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      byte[] blockBuffer,
      int blockLength,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingBlobStoreWrapper.TraceData, 5722960, nameof (PutBlobBlockAsync)))
      {
        tracer.TraceInfo(string.Format("PutBlobBlockAsync {0} [{1}]", (object) blobId, (object) blockHash), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        await this.realStore.PutBlobBlockAsync(requestContext, domainId, blobId, blockBuffer, blockLength, blockHash).ConfigureAwait(true);
      }
    }

    public async Task PutBlobAndReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      Stream blob,
      BlobReference referenceId)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingBlobStoreWrapper.TraceData, 5722970, nameof (PutBlobAndReferenceAsync)))
      {
        tracer.TraceInfo(string.Format("PutBlobAndReferenceAsync {0} => {1}", (object) blobId, (object) referenceId), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        await this.realStore.PutBlobAndReferenceAsync(requestContext, domainId, blobId, blob, referenceId).ConfigureAwait(true);
      }
    }

    public async Task<IDictionary<ulong, PreauthenticatedUri>> GetSasUrisAsync(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      IDictionary<ulong, PreauthenticatedUri> sasUrisAsync;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingBlobStoreWrapper.TraceData, 5722990, nameof (GetSasUrisAsync)))
      {
        tracer.TraceInfo(nameof (GetSasUrisAsync), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        sasUrisAsync = await this.realStore.GetSasUrisAsync(requestContext, domainId).ConfigureAwait(true);
      }
      return sasUrisAsync;
    }

    public async Task<IDictionary<BlobIdentifier, KeepUntilResult?>> ValidateKeepUntilReferencesAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ISet<BlobIdentifier> blobIds,
      DateTime keepUntil)
    {
      IDictionary<BlobIdentifier, KeepUntilResult?> dictionary;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, TracingBlobStoreWrapper.TraceData, 5723500, nameof (ValidateKeepUntilReferencesAsync)))
      {
        tracer.TraceInfo(nameof (ValidateKeepUntilReferencesAsync), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        dictionary = await this.realStore.ValidateKeepUntilReferencesAsync(requestContext, domainId, blobIds, keepUntil).ConfigureAwait(true);
      }
      return dictionary;
    }

    public Task<HttpResponseMessage> CheckBlobExistsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      Uri blobUri)
    {
      throw new NotImplementedException();
    }

    private static string FormatBlobReferences<T>(
      IDictionary<T, IEnumerable<BlobReference>> referenceIdsGroupedByBlobIds)
    {
      return string.Join("; ", referenceIdsGroupedByBlobIds.Select<KeyValuePair<T, IEnumerable<BlobReference>>, string>((Func<KeyValuePair<T, IEnumerable<BlobReference>>, string>) (x => string.Format("{0} => {1}", (object) x.Key, (object) string.Join<BlobReference>(", ", x.Value)))));
    }

    private static string FormatBlobReferences<T>(
      IDictionary<T, IEnumerable<IdBlobReference>> referenceIdsGroupedByBlobIds)
    {
      return string.Join("; ", referenceIdsGroupedByBlobIds.Select<KeyValuePair<T, IEnumerable<IdBlobReference>>, string>((Func<KeyValuePair<T, IEnumerable<IdBlobReference>>, string>) (x => string.Format("{0} => {1}", (object) x.Key, (object) string.Join<IdBlobReference>(", ", x.Value)))));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => throw new InvalidTracingWrapperUsageException();

    public void ServiceEnd(IVssRequestContext systemRequestContext) => throw new InvalidTracingWrapperUsageException();
  }
}
