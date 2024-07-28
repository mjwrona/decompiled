// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore.ElevatedBlobStore
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore
{
  public class ElevatedBlobStore : IElevatedBlobStore, IBlobStore, IVssFrameworkService
  {
    public virtual bool IsPermissionChecked
    {
      get => false;
      set
      {
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> referencesGroupedByBlobIds)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IBlobStore>().TryReferenceAsync(vssRequestContext, domainId, referencesGroupedByBlobIds);
    }

    public Task<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> TryReferenceWithBlocksAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> referencesGroupedByBlobIds)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IBlobStore>().TryReferenceWithBlocksAsync(vssRequestContext, domainId, referencesGroupedByBlobIds);
    }

    public Task RemoveReferencesAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>> referencesGroupedByBlobIds)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IBlobStore>().RemoveReferencesAsync(vssRequestContext, domainId, referencesGroupedByBlobIds);
    }

    public Task<PreauthenticatedUri> GetDownloadUriAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdWithHeaders blobIdWithHeaders)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IBlobStore>().GetDownloadUriAsync(vssRequestContext, domainId, blobIdWithHeaders);
    }

    public Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IEnumerable<BlobIdentifier> blobIds,
      EdgeCache edgeCache,
      DateTimeOffset? expiryTime)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IBlobStore>().GetDownloadUrisAsync(vssRequestContext, domainId, blobIds, edgeCache, expiryTime);
    }

    public Task<Stream> GetBlobAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IBlobStore>().GetBlobAsync(vssRequestContext, domainId, blobId);
    }

    public Task PutSingleBlockBlobAndReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      byte[] blobBuffer,
      int blockLength,
      BlobReference reference)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IBlobStore>().PutSingleBlockBlobAndReferenceAsync(vssRequestContext, domainId, blobId, blobBuffer, blockLength, reference);
    }

    public Task PutBlobBlockAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      byte[] blockBuffer,
      int blockLength,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IBlobStore>().PutBlobBlockAsync(vssRequestContext, domainId, blobId, blockBuffer, blockLength, blockHash);
    }

    public Task PutBlobAndReferenceAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      Stream blob,
      BlobReference reference)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IBlobStore>().PutBlobAndReferenceAsync(vssRequestContext, domainId, blobId, blob, reference);
    }

    public Task<IDictionary<ulong, PreauthenticatedUri>> GetSasUrisAsync(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IBlobStore>().GetSasUrisAsync(vssRequestContext, domainId);
    }

    public Task<IDictionary<BlobIdentifier, KeepUntilResult?>> ValidateKeepUntilReferencesAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      ISet<BlobIdentifier> blobIds,
      DateTime keepUntil)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IBlobStore>().ValidateKeepUntilReferencesAsync(vssRequestContext, domainId, blobIds, keepUntil);
    }

    public Task<HttpResponseMessage> CheckBlobExistsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      Uri blobUri)
    {
      throw new NotImplementedException();
    }
  }
}
