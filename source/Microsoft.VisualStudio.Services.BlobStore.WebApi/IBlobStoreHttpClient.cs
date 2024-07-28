// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.IBlobStoreHttpClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public interface IBlobStoreHttpClient : IArtifactHttpClient, IDisposable
  {
    Task<Stream> GetBlobAsync(BlobIdentifier blobId, CancellationToken cancellationToken);

    Task<PreauthenticatedUri> GetDownloadUriAsync(
      BlobIdWithHeaders blobId,
      CancellationToken cancellationToken);

    Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      IEnumerable<BlobIdentifier> blobIds,
      EdgeCache edgeCache,
      CancellationToken cancellationToken,
      DateTimeOffset? expiryTime = null);

    Task PutBlobBlockAsync(
      BlobIdentifier blobId,
      byte[] blockBuffer,
      int blockLength,
      CancellationToken cancellationToken);

    Task PutBlobBlockAsync(
      BlobIdentifier blobId,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash,
      byte[] blockBuffer,
      int blockLength,
      CancellationToken cancellationToken);

    Task PutSingleBlockBlobAndReferenceAsync(
      BlobIdentifier blobId,
      byte[] blockBuffer,
      int blockLength,
      BlobReference reference,
      CancellationToken cancellationToken);

    Task RemoveReferencesAsync(
      IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>> referencesGroupedByBlobIds);

    Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceAsync(
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> referencesGroupedByBlobIds,
      CancellationToken cancellationToken);

    Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceWithBlocksAsync(
      IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> referencesGroupedByBlobIds,
      CancellationToken cancellationToken);

    Task<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> UploadBlocksForBlobAsync(
      BlobIdentifier blobId,
      Stream blobStream,
      CancellationToken cancellationToken);

    Task<IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>> UploadBlocksForBlobsAsync(
      IEnumerable<BlobToUriMapping> pathToUriMappings,
      CancellationToken cancellationToken);
  }
}
