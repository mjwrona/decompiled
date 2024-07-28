// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadUrlCacheServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DownloadUrlCacheServiceFacade : Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.IDownloadUrlCacheService
  {
    private readonly IVssRequestContext requestContext;
    private readonly IDownloadUrlCacheService cacheService;

    public DownloadUrlCacheServiceFacade(IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
      this.cacheService = requestContext.GetService<IDownloadUrlCacheService>();
    }

    public bool TryGetDownloadUrl(BlobStorageId storageId, out Uri url) => this.cacheService.TryGetDownloadUrl(this.requestContext, storageId, out url);

    public void SetDownloadUrl(BlobStorageId storageId, Uri url) => this.cacheService.SetDownloadUrl(this.requestContext, storageId, url);

    public void InvalidateDownloadUrl(BlobStorageId storageId) => this.cacheService.InvalidateDownloadUri(this.requestContext, storageId);
  }
}
