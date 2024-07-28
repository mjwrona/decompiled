// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters.DedupStoreStorageDeleterHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
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
  public class DedupStoreStorageDeleterHandler : 
    IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult>,
    IHaveInputType<IEnumerable<IStorageDeletionRequest>>,
    IHaveOutputType<SimpleResult>
  {
    private readonly IPackagingDedupStore dedupStore;
    private readonly ITracerService tracerService;
    private readonly Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference> getRefIdFunc;

    public DedupStoreStorageDeleterHandler(
      IPackagingDedupStore dedupStore,
      ITracerService tracerService,
      Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference> getRefIdFunc)
    {
      this.dedupStore = dedupStore;
      this.tracerService = tracerService;
      this.getRefIdFunc = getRefIdFunc;
    }

    public async Task<SimpleResult> Handle(IEnumerable<IStorageDeletionRequest> requests)
    {
      DedupStoreStorageDeleterHandler sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        List<IStorageDeletionRequest<DedupStoreStorageId>> list = requests.Select<IStorageDeletionRequest, IStorageDeletionRequest<DedupStoreStorageId>>((Func<IStorageDeletionRequest, IStorageDeletionRequest<DedupStoreStorageId>>) (r => r as IStorageDeletionRequest<DedupStoreStorageId>)).ToList<IStorageDeletionRequest<DedupStoreStorageId>>();
        if (list.Any<IStorageDeletionRequest<DedupStoreStorageId>>((Func<IStorageDeletionRequest<DedupStoreStorageId>, bool>) (r => r == null)))
          return SimpleResult.CouldNotProcess;
        foreach (IStorageDeletionRequest<DedupStoreStorageId> storageDeletionRequest in list)
        {
          if (storageDeletionRequest.StorageId != null)
          {
            DedupIdentifier superRootId = storageDeletionRequest.StorageId.SuperRootId;
            IdBlobReference rootRef = sendInTheThisObject.getRefIdFunc(storageDeletionRequest);
            await sendInTheThisObject.dedupStore.DeleteRootAsync(superRootId, rootRef);
          }
        }
        return SimpleResult.Processed;
      }
    }
  }
}
