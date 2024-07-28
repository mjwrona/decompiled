// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.IContentBlobStore
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public interface IContentBlobStore
  {
    Task<Uri> GetDownloadUriAsync(BlobIdWithHeaders blobId);

    Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      IEnumerable<BlobIdentifier> blobIds,
      EdgeCache edgeCache,
      DateTimeOffset? expiryTime = null);

    Task<Stream> GetBlobAsync(BlobIdentifier blobIdentifier);

    Task PutBlobAndReferenceAsync(
      BlobIdentifier blobId,
      Stream contentStream,
      BlobReference blobReference,
      bool excludeFromAvailabilityTimings = true);

    Task RemoveReferencesAsync(
      IDictionary<BlobIdentifier, ISet<IdBlobReference>> referencesGroupedByBlobIds);

    Task RemoveReferenceAsync(BlobIdentifier blobId, IdBlobReference blobReference);

    Task<bool> TryReferenceAsync(BlobIdentifier blobId, BlobReference blobReference);

    Task<bool> TryReferenceWithBlocksAsync(
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobId,
      BlobReference blobReference);
  }
}
