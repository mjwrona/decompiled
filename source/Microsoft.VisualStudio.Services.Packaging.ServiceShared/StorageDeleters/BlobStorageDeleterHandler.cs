// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters.BlobStorageDeleterHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters
{
  public class BlobStorageDeleterHandler : 
    IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult>,
    IHaveInputType<IEnumerable<IStorageDeletionRequest>>,
    IHaveOutputType<SimpleResult>
  {
    private readonly ITracerService tracerService;
    private readonly Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference> getBlobReferenceFunc;
    private readonly IContentBlobStore contentBlobStore;

    public BlobStorageDeleterHandler(
      ITracerService tracerService,
      Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference> getBlobReferenceFunc,
      IContentBlobStore contentBlobStore)
    {
      this.tracerService = tracerService;
      this.getBlobReferenceFunc = getBlobReferenceFunc;
      this.contentBlobStore = contentBlobStore;
    }

    public async Task<SimpleResult> Handle(IEnumerable<IStorageDeletionRequest> requests)
    {
      BlobStorageDeleterHandler sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        List<IStorageDeletionRequest<BlobStorageId>> list = requests.Select<IStorageDeletionRequest, IStorageDeletionRequest<BlobStorageId>>((Func<IStorageDeletionRequest, IStorageDeletionRequest<BlobStorageId>>) (r => r as IStorageDeletionRequest<BlobStorageId>)).ToList<IStorageDeletionRequest<BlobStorageId>>();
        if (list.Any<IStorageDeletionRequest<BlobStorageId>>((Func<IStorageDeletionRequest<BlobStorageId>, bool>) (r => r == null)))
          return SimpleResult.CouldNotProcess;
        Dictionary<BlobIdentifier, HashSet<IdBlobReference>> referenceDictionary = new Dictionary<BlobIdentifier, HashSet<IdBlobReference>>();
        list.Where<IStorageDeletionRequest<BlobStorageId>>((Func<IStorageDeletionRequest<BlobStorageId>, bool>) (r => r.ExtraAssetBlobReferences != null && !r.ExtraAssetBlobReferences.IsNullOrEmpty<BlobReferenceIdentifier>())).SelectMany<IStorageDeletionRequest<BlobStorageId>, BlobReferenceIdentifier>((Func<IStorageDeletionRequest<BlobStorageId>, IEnumerable<BlobReferenceIdentifier>>) (r => r.ExtraAssetBlobReferences)).ToList<BlobReferenceIdentifier>().ForEach((Action<BlobReferenceIdentifier>) (r => referenceDictionary.AddToDictionaryValueHashSet<BlobIdentifier, IdBlobReference>(r.BlobIdentifier, new IdBlobReference(r.Name, r.Scope))));
        list.ForEach((Action<IStorageDeletionRequest<BlobStorageId>>) (r =>
        {
          if (r.StorageId == null)
            return;
          referenceDictionary.AddToDictionaryValueHashSet<BlobIdentifier, IdBlobReference>(r.StorageId.BlobId, this.getBlobReferenceFunc(r));
        }));
        if (referenceDictionary.Any<KeyValuePair<BlobIdentifier, HashSet<IdBlobReference>>>())
          await sendInTheThisObject.contentBlobStore.RemoveReferencesAsync((IDictionary<BlobIdentifier, ISet<IdBlobReference>>) referenceDictionary.ToDictionary<KeyValuePair<BlobIdentifier, HashSet<IdBlobReference>>, BlobIdentifier, ISet<IdBlobReference>>((Func<KeyValuePair<BlobIdentifier, HashSet<IdBlobReference>>, BlobIdentifier>) (kv => kv.Key), (Func<KeyValuePair<BlobIdentifier, HashSet<IdBlobReference>>, ISet<IdBlobReference>>) (kv => (ISet<IdBlobReference>) kv.Value)));
        return SimpleResult.Processed;
      }
    }
  }
}
