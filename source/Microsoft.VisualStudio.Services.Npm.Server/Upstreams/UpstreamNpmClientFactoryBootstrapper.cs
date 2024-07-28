// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.UpstreamNpmClientFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Npm.Server.Registry;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class UpstreamNpmClientFactoryBootstrapper : 
    IBootstrapper<IFactory<UpstreamSource, Task<IUpstreamNpmClient>>>
  {
    private readonly IVssRequestContext requestContext;

    public UpstreamNpmClientFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<UpstreamSource, Task<IUpstreamNpmClient>> Bootstrap()
    {
      IVssRequestContext vssRequestContext = this.requestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext requestContext = this.requestContext.Elevate();
      ITracerService tracerFacade = this.requestContext.GetTracerFacade();
      PackageMetadataSerializer packageMetadataSerializer = new PackageMetadataSerializer(tracerFacade);
      UnknownUpstreamResponseSerializer unknownResponseSerializer = new UnknownUpstreamResponseSerializer(tracerFacade);
      DefaultTimeProvider defaultTimeProvider = new DefaultTimeProvider();
      UrlHostResolutionServiceFacade urlHostResolutionService = new UrlHostResolutionServiceFacade(vssRequestContext, vssRequestContext.GetService<Microsoft.TeamFoundation.Framework.Server.IUrlHostResolutionService>());
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      SameAadTenantValidatingHandler upstreamSourceValidatingHandler = new SameAadTenantValidatingHandler(environmentFacade, (Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.IUrlHostResolutionService) urlHostResolutionService, (ICrossCollectionTenantIdService) new CrossCollectionTenantIdService(vssRequestContext));
      return (IFactory<UpstreamSource, Task<IUpstreamNpmClient>>) new UpstreamNpmClientFactory(requestContext, (IHttpClient) new HttpClientFacade(requestContext, UpstreamHttpClient.ForProtocol((IProtocol) Protocol.npm)), new CrossCollectionClientCreatorBootstrapper(requestContext).Bootstrap(), (IAsyncHandler<UpstreamSource>) upstreamSourceValidatingHandler, environmentFacade, requestContext.GetTracerFacade(), (INpmUpstreamMetadataDocumentParser) new NpmUpstreamMetadataDocumentParser((ITimeProvider) defaultTimeProvider, (IPackageMetadataSerializer) packageMetadataSerializer, (IUnknownUpstreamResponseSerializer) unknownResponseSerializer, tracerFacade, FeatureFlagConstants.PropagateDeprecateFromUpstream.Bootstrap(this.requestContext)), new GetAuthenticationTokenFromServiceEndpointHandler(requestContext));
    }
  }
}
