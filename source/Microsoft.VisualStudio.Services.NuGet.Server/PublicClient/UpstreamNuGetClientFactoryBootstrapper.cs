// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicClient.UpstreamNuGetClientFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicClient
{
  public class UpstreamNuGetClientFactoryBootstrapper : 
    IBootstrapper<IFactory<UpstreamSource, Task<IUpstreamNuGetClient>>>
  {
    private readonly IVssRequestContext requestContext;

    public UpstreamNuGetClientFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<UpstreamSource, Task<IUpstreamNuGetClient>> Bootstrap()
    {
      IVssRequestContext vssRequestContext1 = this.requestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext vssRequestContext2 = this.requestContext.Elevate();
      UrlHostResolutionServiceFacade urlHostResolutionService = new UrlHostResolutionServiceFacade(vssRequestContext1, vssRequestContext1.GetService<Microsoft.TeamFoundation.Framework.Server.IUrlHostResolutionService>());
      IExecutionEnvironment environmentFacade = vssRequestContext2.GetExecutionEnvironmentFacade();
      bool flag = environmentFacade.IsHosted();
      IOrgLevelPackagingSetting<bool> packagingSetting = FeatureEnabledConstants.UsePublicRepository.Bootstrap(this.requestContext);
      return (IFactory<UpstreamSource, Task<IUpstreamNuGetClient>>) new UpstreamNuGetClientFactory((IHttpClient) new HttpClientFacade(vssRequestContext2, (IRequestContextAwareHttpClient) vssRequestContext2.GetService<IPublicUpstreamHttpClient>()), (IHttpClient) new HttpClientFacade(vssRequestContext2, (IRequestContextAwareHttpClient) vssRequestContext2.GetService<INonForwardingPublicUpstreamHttpClient>()), vssRequestContext2, (IFeedService) new FeedServiceFacade(vssRequestContext2), new CrossCollectionClientCreatorBootstrapper(vssRequestContext2).Bootstrap(), (IAsyncHandler<UpstreamSource>) new SameAadTenantValidatingHandler(environmentFacade, (Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.IUrlHostResolutionService) urlHostResolutionService, (ICrossCollectionTenantIdService) new CrossCollectionTenantIdService(vssRequestContext1)), environmentFacade, new GetAuthenticationTokenFromServiceEndpointHandler(vssRequestContext2), (IVersionCountsFromFileProvider) new VersionCountsFromFileProvider((INuGetPackageMetadataSearchVersionFilteringStrategy) new NuGetPackageMetadataSearchVersionFilteringStrategy(), (INuGetLatestVersionsFinder) new NuGetLatestVersionsFinder()), !flag || !packagingSetting.Get() ? (IPublicRepositoryProvider<IUpstreamNuGetClient>) new PublicRepositoryProvider<IUpstreamNuGetClient>((IEnumerable<IPublicRepository<IUpstreamNuGetClient>>) ImmutableArray<INuGetPublicRepository<IUpstreamNuGetClient>>.Empty) : NuGetPublicRepositoryProvider.Bootstrap(this.requestContext), flag ? packagingSetting : (IOrgLevelPackagingSetting<bool>) ConstantPackagingSetting.From<bool>(false));
    }
  }
}
