// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DomainBlobHttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class DomainBlobHttpClientWrapper : IBlobStoreHttpClient, IArtifactHttpClient, IDisposable
  {
    private readonly IDomainBlobStoreHttpClient multiDomainBlobStoreHttpClient;
    private readonly IDomainId domainId;

    public DomainBlobHttpClientWrapper(IDomainId domainId, IDomainBlobStoreHttpClient client)
    {
      this.domainId = domainId;
      this.multiDomainBlobStoreHttpClient = client;
    }

    public IDomainId DomainId => this.domainId;

    public Uri BaseAddress => this.multiDomainBlobStoreHttpClient.BaseAddress;

    public void Dispose() => this.multiDomainBlobStoreHttpClient.Dispose();

    public async Task<Stream> GetBlobAsync(
      BlobIdentifier blobId,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainBlobStoreHttpClient.GetBlobAsync(this.domainId, blobId, cancellationToken);
    }

    public async Task<PreauthenticatedUri> GetDownloadUriAsync(
      BlobIdWithHeaders blobId,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainBlobStoreHttpClient.GetDownloadUriAsync(this.domainId, blobId, cancellationToken);
    }

    public async Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      IEnumerable<BlobIdentifier> blobIds,
      EdgeCache edgeCache,
      CancellationToken cancellationToken,
      DateTimeOffset? expiryTime = null)
    {
      return await this.multiDomainBlobStoreHttpClient.GetDownloadUrisAsync(this.domainId, blobIds, edgeCache, cancellationToken);
    }

    public async Task GetOptionsAsync(CancellationToken cancellationToken) => await this.multiDomainBlobStoreHttpClient.GetOptionsAsync(cancellationToken);

    public async Task PutBlobBlockAsync(
      BlobIdentifier blobId,
      byte[] blockBuffer,
      int blockLength,
      CancellationToken cancellationToken)
    {
      await this.multiDomainBlobStoreHttpClient.PutBlobBlockAsync(this.domainId, blobId, blockBuffer, blockLength, cancellationToken);
    }

    public async Task PutBlobBlockAsync(
      BlobIdentifier blobId,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash,
      byte[] blockBuffer,
      int blockLength,
      CancellationToken cancellationToken)
    {
      await this.multiDomainBlobStoreHttpClient.PutBlobBlockAsync(this.domainId, blobId, blockHash, blockBuffer, blockLength, cancellationToken);
    }

    public async Task PutSingleBlockBlobAndReferenceAsync(
      BlobIdentifier blobId,
      byte[] blockBuffer,
      int blockLength,
      BlobReference reference,
      CancellationToken cancellationToken)
    {
      await this.multiDomainBlobStoreHttpClient.PutSingleBlockBlobAndReferenceAsync(this.domainId, blobId, blockBuffer, blockLength, reference, cancellationToken);
    }

    public async Task RemoveReferencesAsync(
      IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>> referencesGroupedByBlobIds)
    {
      await this.multiDomainBlobStoreHttpClient.RemoveReferencesAsync(this.domainId, referencesGroupedByBlobIds);
    }

    public void SetTracer(IAppTraceSource tracer) => this.multiDomainBlobStoreHttpClient.SetTracer(tracer);

    public async Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceAsync(
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> referencesGroupedByBlobIds,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainBlobStoreHttpClient.TryReferenceAsync(this.domainId, referencesGroupedByBlobIds, cancellationToken);
    }

    public async Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceWithBlocksAsync(
      IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> referencesGroupedByBlobIds,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainBlobStoreHttpClient.TryReferenceWithBlocksAsync(this.domainId, referencesGroupedByBlobIds, cancellationToken);
    }

    public async Task<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> UploadBlocksForBlobAsync(
      BlobIdentifier blobId,
      Stream blobStream,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainBlobStoreHttpClient.UploadBlocksForBlobAsync(this.domainId, blobId, blobStream, cancellationToken);
    }

    public async Task<IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>> UploadBlocksForBlobsAsync(
      IEnumerable<BlobToUriMapping> pathToUriMappings,
      CancellationToken cancellationToken)
    {
      return await this.multiDomainBlobStoreHttpClient.UploadBlocksForBlobsAsync(this.domainId, pathToUriMappings, cancellationToken);
    }
  }
}
