// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Upstreams.NuGetUpstreamMetadataServiceWithNameCacheFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PublicClient;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamCache.V2;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Upstreams
{
  public class NuGetUpstreamMetadataServiceWithNameCacheFactoryBootstrapper : 
    IBootstrapper<IFactory<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetUpstreamMetadataServiceWithNameCacheFactoryBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    public IFactory<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>> Bootstrap()
    {
      IFactory<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>> defaultUpstreamServiceFactory = new NuGetUpstreamMetadataServiceFactoryBootstrapper(this.requestContext).Bootstrap();
      IFactory<UpstreamSource, Task<IUpstreamNuGetClient>> upstreamClientFactory = new UpstreamNuGetClientFactoryBootstrapper(this.requestContext).Bootstrap();
      RequestContextItemsAsCacheFacade requestContextItemsAsCache1 = new RequestContextItemsAsCacheFacade(this.requestContext);
      return ByFuncInputFactory.For<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>>((Func<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>>) (async upstreamSource =>
      {
        IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry> defaultUpstreamService = await defaultUpstreamServiceFactory.Get(upstreamSource);
        if (upstreamSource.UpstreamSourceType != UpstreamSourceType.Internal)
          return defaultUpstreamService;
        ICache<string, object> requestContextItemsAsCache2 = (ICache<string, object>) requestContextItemsAsCache1;
        return (IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>) new PackageNameCachingUpstreamService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>((IPackageNameCacheService<VssNuGetPackageName>) new PackageNameCacheService<VssNuGetPackageName>(requestContextItemsAsCache2, (IUpstreamPackageNamesClient) await upstreamClientFactory.Get(upstreamSource), upstreamSource), defaultUpstreamService);
      }));
    }
  }
}
