// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Proprietary.CargoUpstreamingController
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Proprietary
{
  [ControllerApiVersion(7.1)]
  [ClientGroupByResource("Cargo")]
  [VersionedApiControllerCustomName(Area = "cargo", ResourceName = "upstreaming")]
  [ClientEnforceBodyParameterToComeBeforeQueryParameters(false)]
  public class CargoUpstreamingController : CargoApiController
  {
    [HttpGet]
    public Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior GetUpstreamingBehavior(
      string feed,
      string packageName)
    {
      IPackageNameRequest<CargoPackageName> packageNameRequest = this.GetPackageNameRequest(feed, packageName);
      return this.TfsRequestContext.GetService<IPackageUpstreamBehaviorService>().GetBehavior(this.TfsRequestContext, packageNameRequest.Feed, (IPackageName) packageNameRequest.PackageName).ToWebApi();
    }

    [HttpPatch]
    public void SetUpstreamingBehavior(
      string feed,
      string packageName,
      [FromBody] Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior behavior)
    {
      IPackageNameRequest<CargoPackageName> packageNameRequest = this.GetPackageNameRequest(feed, packageName, FeedValidator.GetFeedIsNotReadOnlyValidator());
      this.TfsRequestContext.GetService<IPackageUpstreamBehaviorService>().SetBehavior(this.TfsRequestContext, packageNameRequest.Feed, (IPackageName) packageNameRequest.PackageName, new Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamingBehavior(behavior.VersionsFromExternalUpstreams));
    }
  }
}
