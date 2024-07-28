// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api.NuGetUpstreamingController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api
{
  [ControllerApiVersion(6.1)]
  [ClientGroupByResource("NuGet")]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "upstreaming")]
  [ClientEnforceBodyParameterToComeBeforeQueryParameters(false)]
  public class NuGetUpstreamingController : NuGetApiController
  {
    [HttpGet]
    public Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior GetUpstreamingBehavior(
      string feedId,
      string packageName)
    {
      FeedCore feed1 = this.GetFeedRequest(feedId, FeedValidator.GetFeedIsNotReadOnlyValidator()).Feed;
      IPackageUpstreamBehaviorService service = this.TfsRequestContext.GetService<IPackageUpstreamBehaviorService>();
      IConverter<string, VssNuGetPackageName> converter = new NuGetPackageNameParsingConverterBootstrapper(this.TfsRequestContext).Bootstrap();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      FeedCore feed2 = feed1;
      VssNuGetPackageName nuGetPackageName = converter.Convert(packageName);
      return service.GetBehavior(tfsRequestContext, feed2, (IPackageName) nuGetPackageName).ToWebApi();
    }

    [HttpPatch]
    public void SetUpstreamingBehavior(
      string feedId,
      string packageName,
      [FromBody] Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior behavior)
    {
      FeedCore feed1 = this.GetFeedRequest(feedId, FeedValidator.GetFeedIsNotReadOnlyValidator()).Feed;
      IPackageUpstreamBehaviorService service = this.TfsRequestContext.GetService<IPackageUpstreamBehaviorService>();
      IConverter<string, VssNuGetPackageName> converter = new NuGetPackageNameParsingConverterBootstrapper(this.TfsRequestContext).Bootstrap();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      FeedCore feed2 = feed1;
      VssNuGetPackageName nuGetPackageName = converter.Convert(packageName);
      Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamingBehavior behavior1 = new Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamingBehavior(behavior.VersionsFromExternalUpstreams);
      service.SetBehavior(tfsRequestContext, feed2, (IPackageName) nuGetPackageName, behavior1);
    }
  }
}
