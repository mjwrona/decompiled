// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmUpstreamingController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [ControllerApiVersion(6.1)]
  [ClientGroupByResource("npm")]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "upstreaming")]
  [ClientEnforceBodyParameterToComeBeforeQueryParameters(false)]
  public class NpmUpstreamingController : NpmApiController
  {
    [HttpGet]
    [ClientSwaggerOperationId("GetPackageUpstreamingBehavior")]
    public Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior GetScopedUpstreamingBehavior(
      string feedId,
      string packageScope,
      string unscopedPackageName)
    {
      FeedCore feed1 = this.GetFeedRequest(feedId, FeedValidator.GetFeedIsNotReadOnlyValidator()).Feed;
      IPackageUpstreamBehaviorService service = this.TfsRequestContext.GetService<IPackageUpstreamBehaviorService>();
      NpmPackageName npmPackageName1 = new NpmPackageName(packageScope, unscopedPackageName);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      FeedCore feed2 = feed1;
      NpmPackageName npmPackageName2 = npmPackageName1;
      return service.GetBehavior(tfsRequestContext, feed2, (IPackageName) npmPackageName2).ToWebApi();
    }

    [HttpGet]
    [ClientSwaggerOperationId("GetScopedPackageUpstreamingBehavior")]
    public Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior GetUpstreamingBehavior(
      string feedId,
      string packageName)
    {
      return this.GetScopedUpstreamingBehavior(feedId, (string) null, packageName);
    }

    [HttpPatch]
    public void SetScopedUpstreamingBehavior(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      [FromBody] Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior behavior)
    {
      FeedCore feed1 = this.GetFeedRequest(feedId, FeedValidator.GetFeedIsNotReadOnlyValidator()).Feed;
      IPackageUpstreamBehaviorService service = this.TfsRequestContext.GetService<IPackageUpstreamBehaviorService>();
      NpmPackageName npmPackageName1 = new NpmPackageName(packageScope, unscopedPackageName);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      FeedCore feed2 = feed1;
      NpmPackageName npmPackageName2 = npmPackageName1;
      Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamingBehavior behavior1 = new Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamingBehavior(behavior.VersionsFromExternalUpstreams);
      service.SetBehavior(tfsRequestContext, feed2, (IPackageName) npmPackageName2, behavior1);
    }

    [HttpPatch]
    public void SetUpstreamingBehavior(
      string feedId,
      string packageName,
      [FromBody] Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior behavior)
    {
      this.SetScopedUpstreamingBehavior(feedId, (string) null, packageName, behavior);
    }
  }
}
