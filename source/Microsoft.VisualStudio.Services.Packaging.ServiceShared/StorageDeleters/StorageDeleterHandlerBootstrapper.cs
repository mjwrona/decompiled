// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters.StorageDeleterHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters
{
  public class StorageDeleterHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<IEnumerable<IStorageDeletionRequest>, NullResult>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference> getBlobReferenceFunc;
    private readonly Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference> getDedupStoreReferenceFunc;

    public StorageDeleterHandlerBootstrapper(
      IVssRequestContext requestContext,
      Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference> getBlobReferenceFunc,
      Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference> getDedupStoreReferenceFunc)
    {
      this.requestContext = requestContext;
      this.getBlobReferenceFunc = getBlobReferenceFunc;
      this.getDedupStoreReferenceFunc = getDedupStoreReferenceFunc;
    }

    public IAsyncHandler<IEnumerable<IStorageDeletionRequest>, NullResult> Bootstrap() => (IAsyncHandler<IEnumerable<IStorageDeletionRequest>, NullResult>) new GroupByStorageIdStorageDeletionHandler(UntilNonNullHandler.Create<IEnumerable<IStorageDeletionRequest>, SimpleResult>(this.GetHandlers()));

    private IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult>[] GetHandlers()
    {
      List<IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult>> asyncHandlerList = new List<IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult>>();
      IContentBlobStore contentBlobStore = new ContentBlobStoreFacadeBootstrapper(this.requestContext).Bootstrap();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      DedupStoreFacade dedupStore = new DedupStoreFacade(this.requestContext);
      asyncHandlerList.Add((IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult>) new BlobStorageDeleterHandler(tracerFacade, this.getBlobReferenceFunc, contentBlobStore));
      asyncHandlerList.Add((IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult>) new DropStorageDeleterHandler((IDropHttpClient) new DropClientFacade(this.requestContext), tracerFacade));
      asyncHandlerList.Add((IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult>) new DedupStoreStorageDeleterHandler((IPackagingDedupStore) dedupStore, tracerFacade, this.getDedupStoreReferenceFunc));
      asyncHandlerList.Add((IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult>) new ExtraAssetsBlobStorageDeletionHandler(this.requestContext.GetTracerFacade(), contentBlobStore));
      return asyncHandlerList.ToArray();
    }
  }
}
