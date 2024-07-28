// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadBlobPackageFileAsUriHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DownloadBlobPackageFileAsUriHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri>>
  {
    private readonly IVssRequestContext requestContext;

    public DownloadBlobPackageFileAsUriHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri> Bootstrap()
    {
      IFeatureFlagService featureFlagFacade = this.requestContext.GetFeatureFlagFacade();
      IRegistryWriterService registryFacade = this.requestContext.GetRegistryFacade();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      DownloadUriValidatingHandler validatingHandler = new DownloadUriValidatingHandler(tracerFacade, (ITimeProvider) new DefaultTimeProvider());
      return (IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri>) new DownloadUriEdgeCachingDecoratingHandler((IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri>) new DownloadUrlCachingDecoratingHandler((IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri>) new RetryDecoratingHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri>((IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri>) new DirectDownloadUriCalculator(new ContentBlobStoreFacadeBootstrapper(this.requestContext).Bootstrap(), (IFactory<IPackageFileRequest, DownloadUriSettings>) new DownloadUriSettingsFactory(featureFlagFacade, (IRegistryService) registryFacade), (IFactory<DateTimeOffset>) new SASTokenExpiryFacade(this.requestContext)), new RetryHelper(tracerFacade, (IReadOnlyList<TimeSpan>) new List<TimeSpan>()
      {
        TimeSpan.FromMilliseconds(100.0),
        TimeSpan.FromMilliseconds(200.0),
        TimeSpan.FromMilliseconds(400.0)
      }, (Func<Exception, bool>) (exception => false))).WithRetryOnResultValidatingHandler((IHandler<Uri, bool>) validatingHandler), (Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.IDownloadUrlCacheService) new DownloadUrlCacheServiceFacade(this.requestContext), (IHandler<Uri, bool>) validatingHandler), (IBlobEdgeCaching) new BlobEdgeCachingFacade(this.requestContext), FeatureAvailabilityConstants.EnableEdgeCaching.Bootstrap(this.requestContext));
    }
  }
}
