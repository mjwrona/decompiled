// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.UpstreamPyPiClientFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.PyPi.Server.Constants;
using Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class UpstreamPyPiClientFactoryBootstrapper : 
    IBootstrapper<IFactory<UpstreamSource, Task<IUpstreamPyPiClient>>>
  {
    private readonly IVssRequestContext requestContext;

    public UpstreamPyPiClientFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<UpstreamSource, Task<IUpstreamPyPiClient>> Bootstrap()
    {
      IVssRequestContext vssRequestContext = this.requestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext requestContext = this.requestContext.Elevate();
      UrlHostResolutionServiceFacade urlHostResolutionService = new UrlHostResolutionServiceFacade(vssRequestContext, vssRequestContext.GetService<Microsoft.TeamFoundation.Framework.Server.IUrlHostResolutionService>());
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      bool flag = environmentFacade.IsHosted();
      return (IFactory<UpstreamSource, Task<IUpstreamPyPiClient>>) new UpstreamPyPiClientFactory((IHttpClient) new HttpClientFacade(requestContext, UpstreamHttpClient.ForProtocol((IProtocol) Protocol.PyPi)), requestContext, new FeedServiceFacade(requestContext), new CrossCollectionClientCreatorBootstrapper(requestContext).Bootstrap(), (IAsyncHandler<UpstreamSource>) new SameAadTenantValidatingHandler(environmentFacade, (Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.IUrlHostResolutionService) urlHostResolutionService, (ICrossCollectionTenantIdService) new CrossCollectionTenantIdService(vssRequestContext)), environmentFacade, new GetAuthenticationTokenFromServiceEndpointHandler(requestContext), flag ? PyPiPublicRepositoryProvider.Bootstrap(this.requestContext) : (IPublicRepositoryProvider<IUpstreamPyPiClient>) new PublicRepositoryProvider<IPublicUpstreamPyPiClient>((IEnumerable<IPublicRepository<IPublicUpstreamPyPiClient>>) ImmutableArray<IPublicRepository<IPublicUpstreamPyPiClient>>.Empty), flag ? PyPiFeatureFlags.UsePublicRepository.Bootstrap(this.requestContext) : (IOrgLevelPackagingSetting<bool>) ConstantPackagingSetting.From<bool>(false));
    }
  }
}
