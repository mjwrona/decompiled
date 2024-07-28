// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.UpstreamMavenClientFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Aggregations;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PackageVersionsExposedToDownstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class UpstreamMavenClientFactoryBootstrapper : 
    IBootstrapper<IFactory<UpstreamSource, Task<IUpstreamMavenClient>>>
  {
    private readonly IVssRequestContext requestContext;

    public UpstreamMavenClientFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<UpstreamSource, Task<IUpstreamMavenClient>> Bootstrap()
    {
      IVssRequestContext vssRequestContext = this.requestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext requestContext = this.requestContext.Elevate();
      UrlHostResolutionServiceFacade urlHostResolutionService = new UrlHostResolutionServiceFacade(vssRequestContext, vssRequestContext.GetService<Microsoft.TeamFoundation.Framework.Server.IUrlHostResolutionService>());
      IExecutionEnvironment environmentFacade = requestContext.GetExecutionEnvironmentFacade();
      IAsyncHandler<IPackageNameRequest<MavenPackageName>, IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>> versionsToProvideDownstreamsHandler = MavenAggregationResolver.Bootstrap(requestContext).HandlerFor<IPackageNameRequest<MavenPackageName>, IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>>((IRequireAggBootstrapper<IAsyncHandler<IPackageNameRequest<MavenPackageName>, IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>>>) new GetPackageVersionsExposedToDownstreamHandlerBootstrapper<MavenPackageIdentity, MavenPackageName, MavenPackageVersion, IMavenMetadataEntry>());
      return (IFactory<UpstreamSource, Task<IUpstreamMavenClient>>) new UpstreamMavenClientFactory(requestContext, (IHttpClient) new HttpClientFacade(requestContext, UpstreamHttpClient.ForProtocol((IProtocol) Protocol.Maven)), (IFeedService) new FeedServiceFacade(requestContext), new CrossCollectionClientCreatorBootstrapper(requestContext).Bootstrap(), (IMavenPackageVersionServiceFacade) new MavenVersionsServiceFacade(this.requestContext), (IAsyncHandler<UpstreamSource>) new SameAadTenantValidatingHandler(environmentFacade, (Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.IUrlHostResolutionService) urlHostResolutionService, (ICrossCollectionTenantIdService) new CrossCollectionTenantIdService(vssRequestContext)), environmentFacade, versionsToProvideDownstreamsHandler, new GetAuthenticationTokenFromServiceEndpointHandler(this.requestContext));
    }
  }
}
