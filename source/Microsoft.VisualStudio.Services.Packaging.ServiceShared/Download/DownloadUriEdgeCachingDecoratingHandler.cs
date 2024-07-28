// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadUriEdgeCachingDecoratingHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DownloadUriEdgeCachingDecoratingHandler : 
    IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri>,
    IHaveInputType<IPackageFileRequest<IPackageIdentity, BlobStorageId>>,
    IHaveOutputType<Uri>
  {
    private readonly IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri> innerHandler;
    private readonly IBlobEdgeCaching edgeCachingService;
    private readonly IOrgLevelPackagingSetting<bool> enableEdgeCachingSetting;

    public DownloadUriEdgeCachingDecoratingHandler(
      IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri> innerHandler,
      IBlobEdgeCaching edgeCachingService,
      IOrgLevelPackagingSetting<bool> enableEdgeCachingSetting)
    {
      this.innerHandler = innerHandler;
      this.edgeCachingService = edgeCachingService;
      this.enableEdgeCachingSetting = enableEdgeCachingSetting;
    }

    public async Task<Uri> Handle(
      IPackageFileRequest<IPackageIdentity, BlobStorageId> request)
    {
      Uri uri = await this.innerHandler.Handle(request);
      if (this.EdgeCachingIsEnabled() && !this.edgeCachingService.UserIsExcluded())
        uri = this.edgeCachingService.GetEdgeUri(uri);
      return uri;
    }

    private bool EdgeCachingIsEnabled() => this.enableEdgeCachingSetting.Get();
  }
}
