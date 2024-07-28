// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters.ExtraAssetsBlobStorageDeletionHandler
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
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters
{
  public class ExtraAssetsBlobStorageDeletionHandler : 
    IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult>,
    IHaveInputType<IEnumerable<IStorageDeletionRequest>>,
    IHaveOutputType<SimpleResult>
  {
    private readonly ITracerService tracerService;
    private readonly IContentBlobStore contentBlobStore;

    public ExtraAssetsBlobStorageDeletionHandler(
      ITracerService tracerService,
      IContentBlobStore contentBlobStore)
    {
      this.tracerService = tracerService;
      this.contentBlobStore = contentBlobStore;
    }

    public async Task<SimpleResult> Handle(IEnumerable<IStorageDeletionRequest> requests)
    {
      ExtraAssetsBlobStorageDeletionHandler sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        Dictionary<BlobIdentifier, HashSet<IdBlobReference>> referenceDictionary = new Dictionary<BlobIdentifier, HashSet<IdBlobReference>>();
        requests.Where<IStorageDeletionRequest>((Func<IStorageDeletionRequest, bool>) (r => r.ExtraAssetBlobReferences != null && !r.ExtraAssetBlobReferences.IsNullOrEmpty<BlobReferenceIdentifier>())).SelectMany<IStorageDeletionRequest, BlobReferenceIdentifier>((Func<IStorageDeletionRequest, IEnumerable<BlobReferenceIdentifier>>) (r => r.ExtraAssetBlobReferences)).ToList<BlobReferenceIdentifier>().ForEach((Action<BlobReferenceIdentifier>) (r => referenceDictionary.AddToDictionaryValueHashSet<BlobIdentifier, IdBlobReference>(r.BlobIdentifier, new IdBlobReference(r.Name, r.Scope))));
        if (referenceDictionary.Any<KeyValuePair<BlobIdentifier, HashSet<IdBlobReference>>>())
          await sendInTheThisObject.contentBlobStore.RemoveReferencesAsync((IDictionary<BlobIdentifier, ISet<IdBlobReference>>) referenceDictionary.ToDictionary<KeyValuePair<BlobIdentifier, HashSet<IdBlobReference>>, BlobIdentifier, ISet<IdBlobReference>>((Func<KeyValuePair<BlobIdentifier, HashSet<IdBlobReference>>, BlobIdentifier>) (kv => kv.Key), (Func<KeyValuePair<BlobIdentifier, HashSet<IdBlobReference>>, ISet<IdBlobReference>>) (kv => (ISet<IdBlobReference>) kv.Value)));
      }
      return SimpleResult.Processed;
    }
  }
}
