// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.UpstreamCargoClientFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.PublicUpstreamClient;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams
{
  public class UpstreamCargoClientFactoryBootstrapper : 
    IBootstrapper<IFactory<UpstreamSource, Task<IUpstreamCargoClient>>>
  {
    private readonly IVssRequestContext requestContext;

    public UpstreamCargoClientFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<UpstreamSource, Task<IUpstreamCargoClient>> Bootstrap()
    {
      IVssRequestContext vssRequestContext = this.requestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext requestContext = this.requestContext.Elevate();
      UrlHostResolutionServiceFacade urlHostResolutionService = new UrlHostResolutionServiceFacade(vssRequestContext, vssRequestContext.GetService<Microsoft.TeamFoundation.Framework.Server.IUrlHostResolutionService>());
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      return (IFactory<UpstreamSource, Task<IUpstreamCargoClient>>) new UpstreamCargoClientFactory((IHttpClient) new HttpClientFacade(requestContext, UpstreamHttpClient.ForProtocol((IProtocol) Protocol.Cargo)), requestContext, new FeedServiceFacade(requestContext), new CrossCollectionClientCreatorBootstrapper(requestContext).Bootstrap(), (IAsyncHandler<UpstreamSource>) new SameAadTenantValidatingHandler(environmentFacade, (Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.IUrlHostResolutionService) urlHostResolutionService, (ICrossCollectionTenantIdService) new CrossCollectionTenantIdService(vssRequestContext)), environmentFacade, new GetAuthenticationTokenFromServiceEndpointHandler(requestContext), (ICargoDashFixer) new CargoDashFixer(CargoSettings.AdditionalDashMappings.Bootstrap(this.requestContext)));
    }
  }
}
