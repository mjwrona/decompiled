// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers.ManualUpstreamIngestionController
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9BF15B3F-0578-4452-9C4B-B5237E218DF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Server.Controllers
{
  [ControllerApiVersion(6.0)]
  [ClientGroupByResource("Feed Management")]
  [VersionedApiControllerCustomName(Area = "AnyProtocolPackagingInternal", ResourceName = "ManualUpstreamIngestion", ResourceVersion = 1)]
  public class ManualUpstreamIngestionController : AnyProtocolPackagingApiController
  {
    [HttpPost]
    [ClientInternalUseOnly(true)]
    [ValidateModel]
    public async Task IngestFromUpstreamAsync(
      string protocol,
      string feedId,
      string packageName,
      string packageVersion,
      [FromBody, Required] ManualUpstreamIngestionParameters triggerParameters)
    {
      ManualUpstreamIngestionController ingestionController = this;
      if (!triggerParameters.IngestFromUpstream)
        throw new InvalidOperationException();
      IFeedRequest feedRequest = ingestionController.GetFeedRequest(protocol, feedId);
      FeedSecurityHelper.CheckAddUpstreamPackagePermissions(ingestionController.TfsRequestContext, feedRequest.Feed);
      packageName = AlternativeUriEscaping.UnescapeString(packageName);
      packageVersion = AlternativeUriEscaping.UnescapeString(packageVersion);
      NullResult nullResult = await ProtocolRegistrar.Instance.GetBootstrappers(feedRequest.Protocol).GetManualUpstreamIngestionBootstrapper(ingestionController.TfsRequestContext).Bootstrap().Handle((IRawPackageRequest) new RawPackageRequest(feedRequest, packageName, packageVersion));
    }
  }
}
