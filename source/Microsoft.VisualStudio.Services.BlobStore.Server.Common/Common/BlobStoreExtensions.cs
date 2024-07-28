// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlobStoreExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public static class BlobStoreExtensions
  {
    public static async Task<bool> TryReferenceAsync(
      this IBlobStore blobStore,
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      BlobReference reference)
    {
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> source = await blobStore.TryReferenceAsync(requestContext, domainId, (IDictionary<BlobIdentifier, IEnumerable<BlobReference>>) new Dictionary<BlobIdentifier, IEnumerable<BlobReference>>()
      {
        {
          blobId,
          (IEnumerable<BlobReference>) new BlobReference[1]
          {
            reference
          }
        }
      }).ConfigureAwait(false);
      requestContext = (IVssRequestContext) null;
      return !source.Any<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>();
    }

    public static async Task<bool> TryReferenceWithBlocksAsync(
      this IBlobStore blobStore,
      IVssRequestContext requestContext,
      IDomainId domainId,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobIdAndBlocks,
      BlobReference reference)
    {
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> source = await blobStore.TryReferenceWithBlocksAsync(requestContext, domainId, (IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>) new Dictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>()
      {
        {
          blobIdAndBlocks,
          (IEnumerable<BlobReference>) new BlobReference[1]
          {
            reference
          }
        }
      }).ConfigureAwait(false);
      requestContext = (IVssRequestContext) null;
      return !source.Any<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>();
    }

    public static Task RemoveReferenceAsync(
      this IBlobStore blobStore,
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId,
      IdBlobReference reference)
    {
      return blobStore.RemoveReferencesAsync(requestContext, domainId, (IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>>) new Dictionary<BlobIdentifier, IEnumerable<IdBlobReference>>()
      {
        {
          blobId,
          (IEnumerable<IdBlobReference>) new IdBlobReference[1]
          {
            reference
          }
        }
      });
    }
  }
}
