// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadUrlCachingDecoratingHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DownloadUrlCachingDecoratingHandler : 
    IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri>,
    IHaveInputType<IPackageFileRequest<IPackageIdentity, BlobStorageId>>,
    IHaveOutputType<Uri>
  {
    private readonly Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.IDownloadUrlCacheService cacheService;
    private readonly IHandler<Uri, bool> uriValidatingHandler;
    private readonly IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri> innerHandler;

    public DownloadUrlCachingDecoratingHandler(
      IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri> innerHandler,
      Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.IDownloadUrlCacheService cacheService,
      IHandler<Uri, bool> uriValidatingHandler)
    {
      this.innerHandler = innerHandler;
      this.cacheService = cacheService;
      this.uriValidatingHandler = uriValidatingHandler;
    }

    public async Task<Uri> Handle(
      IPackageFileRequest<IPackageIdentity, BlobStorageId> request)
    {
      Uri url;
      if (this.cacheService.TryGetDownloadUrl(request.AdditionalData, out url) && this.uriValidatingHandler.Handle(url))
        return url;
      url = await this.innerHandler.Handle(request);
      this.cacheService.SetDownloadUrl(request.AdditionalData, url);
      return url;
    }
  }
}
