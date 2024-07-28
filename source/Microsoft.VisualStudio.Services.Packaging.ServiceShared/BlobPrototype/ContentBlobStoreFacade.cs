// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ContentBlobStoreFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ContentBlobStoreFacade : IContentBlobStore
  {
    private readonly IVssRequestContext requestContext;
    private readonly ITracerService tracerService;

    public ContentBlobStoreFacade(IVssRequestContext requestContext, ITracerService tracerService)
    {
      this.requestContext = requestContext;
      this.tracerService = tracerService;
    }

    public async Task<Uri> GetDownloadUriAsync(BlobIdWithHeaders blobId)
    {
      ContentBlobStoreFacade sendInTheThisObject = this;
      Uri notNullUri;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetDownloadUriAsync)))
        notNullUri = (await sendInTheThisObject.requestContext.GetService<IElevatedBlobStore>().GetDownloadUriAsync(sendInTheThisObject.requestContext, WellKnownDomainIds.OriginalDomainId, blobId)).NotNullUri;
      return notNullUri;
    }

    public async Task<IDictionary<BlobIdentifier, PreauthenticatedUri>> GetDownloadUrisAsync(
      IEnumerable<BlobIdentifier> blobIds,
      EdgeCache edgeCache,
      DateTimeOffset? expiryTime = null)
    {
      ContentBlobStoreFacade sendInTheThisObject = this;
      IDictionary<BlobIdentifier, PreauthenticatedUri> downloadUrisAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetDownloadUrisAsync)))
        downloadUrisAsync = await sendInTheThisObject.requestContext.GetService<IElevatedBlobStore>().GetDownloadUrisAsync(sendInTheThisObject.requestContext, WellKnownDomainIds.OriginalDomainId, blobIds, edgeCache, expiryTime);
      return downloadUrisAsync;
    }

    public async Task<Stream> GetBlobAsync(BlobIdentifier blobIdentifier)
    {
      ContentBlobStoreFacade sendInTheThisObject = this;
      Stream blobAsync;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetBlobAsync)))
        blobAsync = await sendInTheThisObject.requestContext.GetService<IElevatedBlobStore>().GetBlobAsync(sendInTheThisObject.requestContext, WellKnownDomainIds.OriginalDomainId, blobIdentifier);
      return blobAsync;
    }

    public async Task PutBlobAndReferenceAsync(
      BlobIdentifier blobId,
      Stream contentStream,
      BlobReference blobReference,
      bool excludeFromAvailabilityTimings = true)
    {
      if (excludeFromAvailabilityTimings)
      {
        using (this.requestContext.CreateTimeToFirstPageExclusionBlock())
          await this.PutBlobAndReferenceInternalAsync(blobId, contentStream, blobReference);
      }
      else
        await this.PutBlobAndReferenceInternalAsync(blobId, contentStream, blobReference);
    }

    private async Task PutBlobAndReferenceInternalAsync(
      BlobIdentifier blobId,
      Stream contentStream,
      BlobReference blobReference)
    {
      ContentBlobStoreFacade sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (PutBlobAndReferenceInternalAsync)))
        await sendInTheThisObject.requestContext.GetService<IElevatedBlobStore>().PutBlobAndReferenceAsync(sendInTheThisObject.requestContext, WellKnownDomainIds.OriginalDomainId, blobId, contentStream, blobReference);
    }

    public async Task RemoveReferencesAsync(
      IDictionary<BlobIdentifier, ISet<IdBlobReference>> referencesGroupedByBlobIds)
    {
      ContentBlobStoreFacade sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RemoveReferencesAsync)))
      {
        IElevatedBlobStore blobStore = sendInTheThisObject.requestContext.GetService<IElevatedBlobStore>();
        if (sendInTheThisObject.requestContext.GetService<IVssRegistryService>().GetValue<bool>(sendInTheThisObject.requestContext, new RegistryQuery("/Configuration/Packaging/ContentBlobStore/BatchRemoveReferences/Enabled"), true, true))
        {
          Dictionary<BlobIdentifier, IEnumerable<IdBlobReference>> dictionary = referencesGroupedByBlobIds.ToDictionary<KeyValuePair<BlobIdentifier, ISet<IdBlobReference>>, BlobIdentifier, IEnumerable<IdBlobReference>>((Func<KeyValuePair<BlobIdentifier, ISet<IdBlobReference>>, BlobIdentifier>) (kv => kv.Key), (Func<KeyValuePair<BlobIdentifier, ISet<IdBlobReference>>, IEnumerable<IdBlobReference>>) (kv => (IEnumerable<IdBlobReference>) kv.Value));
          await blobStore.RemoveReferencesAsync(sendInTheThisObject.requestContext, WellKnownDomainIds.OriginalDomainId, (IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>>) dictionary);
        }
        else
        {
          foreach (KeyValuePair<BlobIdentifier, ISet<IdBlobReference>> referencesGroupedByBlobId in (IEnumerable<KeyValuePair<BlobIdentifier, ISet<IdBlobReference>>>) referencesGroupedByBlobIds)
          {
            KeyValuePair<BlobIdentifier, ISet<IdBlobReference>> entry = referencesGroupedByBlobId;
            foreach (IdBlobReference reference in (IEnumerable<IdBlobReference>) entry.Value)
              await blobStore.RemoveReferenceAsync(sendInTheThisObject.requestContext, WellKnownDomainIds.OriginalDomainId, entry.Key, reference);
            entry = new KeyValuePair<BlobIdentifier, ISet<IdBlobReference>>();
          }
        }
        blobStore = (IElevatedBlobStore) null;
      }
    }

    public async Task RemoveReferenceAsync(BlobIdentifier blobId, IdBlobReference blobReference)
    {
      ContentBlobStoreFacade sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RemoveReferenceAsync)))
        await sendInTheThisObject.requestContext.GetService<IElevatedBlobStore>().RemoveReferenceAsync(sendInTheThisObject.requestContext, WellKnownDomainIds.OriginalDomainId, blobId, blobReference);
    }

    public async Task<bool> TryReferenceAsync(BlobIdentifier blobId, BlobReference blobReference)
    {
      ContentBlobStoreFacade sendInTheThisObject = this;
      bool flag;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (TryReferenceAsync)))
        flag = await sendInTheThisObject.requestContext.GetService<IElevatedBlobStore>().TryReferenceAsync(sendInTheThisObject.requestContext, WellKnownDomainIds.OriginalDomainId, blobId, blobReference);
      return flag;
    }

    public async Task<bool> TryReferenceWithBlocksAsync(
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobId,
      BlobReference blobReference)
    {
      ContentBlobStoreFacade sendInTheThisObject = this;
      bool flag;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (TryReferenceWithBlocksAsync)))
        flag = await sendInTheThisObject.requestContext.GetService<IElevatedBlobStore>().TryReferenceWithBlocksAsync(sendInTheThisObject.requestContext, WellKnownDomainIds.OriginalDomainId, blobId, blobReference);
      return flag;
    }
  }
}
