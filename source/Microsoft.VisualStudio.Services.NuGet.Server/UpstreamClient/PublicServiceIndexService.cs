// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.UpstreamClient.PublicServiceIndexService
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.UpstreamClient
{
  internal class PublicServiceIndexService : 
    VssMemoryCacheService<
    #nullable disable
    PublicServiceIndexService.Key, ServiceIndex>,
    IPublicServiceIndexVssService,
    IVssFrameworkService
  {
    private static readonly MemoryCacheConfiguration<PublicServiceIndexService.Key, ServiceIndex> DefaultMemoryCacheConfiguration = new MemoryCacheConfiguration<PublicServiceIndexService.Key, ServiceIndex>().WithCleanupInterval(TimeSpan.FromMinutes(30.0)).WithExpiryInterval(TimeSpan.FromMinutes(30.0));

    public PublicServiceIndexService()
      : base((IEqualityComparer<PublicServiceIndexService.Key>) EqualityComparer<PublicServiceIndexService.Key>.Default, PublicServiceIndexService.DefaultMemoryCacheConfiguration)
    {
    }

    public async Task<ServiceIndex> GetServiceIndex(
      IVssRequestContext requestContext,
      Uri packageSourceUri,
      Func<Uri, Task<ServiceIndex>> fetchOnCacheMissFunc)
    {
      PublicServiceIndexService serviceIndexService = this;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Uri>(packageSourceUri, nameof (packageSourceUri));
      PublicServiceIndexService.Key key = PublicServiceIndexService.BuildKey(requestContext, packageSourceUri);
      ServiceIndex serviceIndex1;
      if (serviceIndexService.TryGetValue(requestContext, key, out serviceIndex1))
        return serviceIndex1;
      ServiceIndex serviceIndex2 = await fetchOnCacheMissFunc(packageSourceUri);
      serviceIndexService.Set(requestContext, key, serviceIndex2);
      return serviceIndex2;
    }

    private static PublicServiceIndexService.Key BuildKey(
      IVssRequestContext requestContext,
      Uri packageSourceUri)
    {
      return new PublicServiceIndexService.Key(requestContext.GetService<INuGetPublicRepoCacheUniverseService>().GetCacheUniverseName(requestContext), packageSourceUri);
    }

    public record Key(string CacheUniverse, Uri PackageSourceUri);
  }
}
