// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.PackagesIngestedUpstream.PackagesIngestedUpstreamControllerHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9BF15B3F-0578-4452-9C4B-B5237E218DF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.PackagesIngestedUpstream
{
  public class PackagesIngestedUpstreamControllerHandlerBootstrapper
  {
    private readonly IVssRequestContext requestContext;

    public PackagesIngestedUpstreamControllerHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public PackagesIngestedUpstreamControllerHandler Bootstrap()
    {
      IFactory<IProtocol, ICollectionPackageUpstreamRefreshJobQueuer> jobQueuerFactory = new ByFuncInputFactory<IProtocol, ICollectionPackageUpstreamRefreshJobQueuer>((Func<IProtocol, ICollectionPackageUpstreamRefreshJobQueuer>) (protocolObject => ProtocolRegistrar.Instance.GetBootstrappers(protocolObject).GetCollectionPackageUpstreamRefreshJobQueuerBootstrapper(this.requestContext).Bootstrap())).SingleElementCache<IProtocol, ICollectionPackageUpstreamRefreshJobQueuer>();
      IFeedService feedService = (IFeedService) new FeedServiceFacade(this.requestContext);
      ByAsyncFuncAsyncHandler<(Guid, IProtocol), IFeedRequest> funcAsyncHandler = new ByAsyncFuncAsyncHandler<(Guid, IProtocol), IFeedRequest>((Func<(Guid, IProtocol), Task<IFeedRequest>>) (async tuple => (IFeedRequest) new FeedRequest(await feedService.GetFeedByIdForAnyScopeAsync(tuple.FeedId), tuple.Protocol)));
      return new PackagesIngestedUpstreamControllerHandler(jobQueuerFactory, this.requestContext.GetTracerFacade(), (IPackagingTraces) new PackagingTracesFacade(this.requestContext), (ITimeProvider) new DefaultTimeProvider(), this.requestContext.GetExecutionEnvironmentFacade(), (IRegistryService) this.requestContext.GetRegistryFacade());
    }
  }
}
