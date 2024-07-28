// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.UpstreamStatusHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9BF15B3F-0578-4452-9C4B-B5237E218DF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers
{
  public class UpstreamStatusHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<IProtocolAgnosticFeedRequest, IEnumerable<UpstreamHealthStatus>>>
  {
    private readonly IVssRequestContext requestContext;

    public UpstreamStatusHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<IProtocolAgnosticFeedRequest, IEnumerable<UpstreamHealthStatus>> Bootstrap() => (IAsyncHandler<IProtocolAgnosticFeedRequest, IEnumerable<UpstreamHealthStatus>>) new UpstreamStatusHandler(new UpstreamStatusAnalyzerBootstrapper(this.requestContext).Bootstrap(), this.requestContext.GetFeatureFlagFacade());
  }
}
