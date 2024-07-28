// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.PackagesIngestedUpstreamController
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9BF15B3F-0578-4452-9C4B-B5237E218DF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.PackagesIngestedUpstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.WebApi;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers
{
  [ControllerApiVersion(6.0)]
  [ClientGroupByResource("Feed Management")]
  [VersionedApiControllerCustomName(Area = "AnyProtocolPackagingInternal", ResourceName = "PackagesIngestedUpstream", ResourceVersion = 1)]
  public class PackagesIngestedUpstreamController : AnyProtocolPackagingApiController
  {
    [HttpPost]
    [ClientInternalUseOnly(true)]
    [ValidateModel]
    public void NotifyOfPackagesIngestedInUpstreamsAsync(
      string protocol,
      [FromBody, Required] NotifyOfPackagesIngestedInUpstreamsParameters triggerParameters)
    {
      FeedSecurityHelper.CheckModifyIndexPermissions(this.TfsRequestContext);
      IProtocol protocol1 = AnyProtocolPackagingApiController.GetProtocol(protocol);
      this.TfsRequestContext.SetProtocolForPackagingTraces(protocol1);
      new PackagesIngestedUpstreamControllerHandlerBootstrapper(this.TfsRequestContext).Bootstrap().Handle(protocol1, triggerParameters);
    }
  }
}
