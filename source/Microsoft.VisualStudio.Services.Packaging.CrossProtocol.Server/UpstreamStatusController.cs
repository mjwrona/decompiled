// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.UpstreamStatusController
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9BF15B3F-0578-4452-9C4B-B5237E218DF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers
{
  [ControllerApiVersion(5.0)]
  [ClientGroupByResource("Feed Management")]
  [VersionedApiControllerCustomName(Area = "AllProtocolsInternal", ResourceName = "UpstreamStatus", ResourceVersion = 1)]
  public class UpstreamStatusController : ProtocolAgnosticPackagingApiController
  {
    [HttpGet]
    [ClientInternalUseOnly(true)]
    public async Task<IEnumerable<UpstreamHealthStatus>> GetUpstreamStatus(string feedId)
    {
      UpstreamStatusController statusController = this;
      return await new UpstreamStatusHandlerBootstrapper(statusController.TfsRequestContext).Bootstrap().Handle((IProtocolAgnosticFeedRequest) statusController.GetProtocolAgnosticFeedRequest(feedId));
    }
  }
}
