// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobStore
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  [DefaultServiceImplementation(typeof (FrameworkBlobStore))]
  public interface IBlobStore : IVssFrameworkService
  {
    Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> referencesGroupedByBlobIds);

    Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceWithBlocksAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> referencesGroupedByBlobIds);

    Task RemoveReferencesAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>> referencesGroupedByBlobIds);

    Task<HttpResponseMessage> CheckBlobExistsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      Uri blobUri);

    Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IEnumerable<BlobIdentifier> blobIds,
      EdgeCache edgeCache,
      DateTimeOffset? expiryTime = null);

    Task<PreauthenticatedUri> GetDownloadUriAsync(
      IVssRequestContext tfsRequestContext,
      IDomainId domainId,
      BlobIdWithHeaders blobId);

    Task<Stream> GetBlobAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId);

    Task PutSingleBlockBlobAndReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      byte[] blobBuffer,
      int blockLength,
      BlobReference reference);

    Task PutBlobBlockAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      byte[] blockBuffer,
      int blockLength,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash);

    Task PutBlobAndReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      Stream blob,
      BlobReference reference);

    Task<IDictionary<ulong, PreauthenticatedUri>> GetSasUrisAsync(
      IVssRequestContext requestContext,
      IDomainId domainId);

    Task<IDictionary<BlobIdentifier, KeepUntilResult?>> ValidateKeepUntilReferencesAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ISet<BlobIdentifier> blobIds,
      DateTime keepUntil);
  }
}
