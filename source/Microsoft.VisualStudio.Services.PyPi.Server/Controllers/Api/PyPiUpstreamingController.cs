// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api.PyPiUpstreamingController
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api
{
  [ControllerApiVersion(6.1)]
  [ClientGroupByResource("Python")]
  [VersionedApiControllerCustomName(Area = "pypi", ResourceName = "upstreaming")]
  [ClientEnforceBodyParameterToComeBeforeQueryParameters(false)]
  public class PyPiUpstreamingController : PyPiApiController
  {
    [HttpGet]
    public Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior GetUpstreamingBehavior(
      string feedId,
      string packageName)
    {
      PyPiPackageIngestionValidationUtils.ValidatePackageName(packageName);
      FeedCore feed = this.GetFeedRequest(feedId, FeedValidator.GetFeedIsNotReadOnlyValidator()).Feed;
      return this.TfsRequestContext.GetService<IPackageUpstreamBehaviorService>().GetBehavior(this.TfsRequestContext, feed, (IPackageName) new PyPiPackageName(packageName)).ToWebApi();
    }

    [HttpPatch]
    public void SetUpstreamingBehavior(
      string feedId,
      string packageName,
      [FromBody] Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior behavior)
    {
      FeedCore feed = this.GetFeedRequest(feedId, FeedValidator.GetFeedIsNotReadOnlyValidator()).Feed;
      this.TfsRequestContext.GetService<IPackageUpstreamBehaviorService>().SetBehavior(this.TfsRequestContext, feed, (IPackageName) new PyPiPackageName(packageName), new Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamingBehavior(behavior.VersionsFromExternalUpstreams));
    }
  }
}
