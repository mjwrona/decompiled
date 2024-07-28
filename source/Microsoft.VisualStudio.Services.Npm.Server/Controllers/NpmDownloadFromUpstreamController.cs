// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmDownloadFromUpstreamController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Npm.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "upstream", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Npm.Service")]
  [ClientIgnore]
  public class NpmDownloadFromUpstreamController : NpmApiController
  {
    [HttpGet]
    [ControllerMethodTraceFilter(12000020)]
    public IHttpActionResult GetPackageAsync(
      string feedId,
      string packageName,
      string packageFileName)
    {
      var routeValues = new
      {
        area = "npm",
        resource = "-",
        feedId = feedId,
        packageName = packageName,
        packageFileName = packageFileName
      };
      return (IHttpActionResult) this.Redirect(this.TfsRequestContext.GetService<ILocationService>().GetResourceUri(this.TfsRequestContext, "npm", ResourceIds.DownloadPackageResourceId, this.ProjectId, (object) routeValues));
    }

    [HttpGet]
    [ControllerMethodTraceFilter(12000030)]
    public IHttpActionResult GetPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageFileName)
    {
      var routeValues = new
      {
        area = "npm",
        resource = "-",
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageFileName = packageFileName
      };
      return (IHttpActionResult) this.Redirect(this.TfsRequestContext.GetService<ILocationService>().GetResourceUri(this.TfsRequestContext, "npm", ResourceIds.DownloadScopedPackageResourceId, this.ProjectId, (object) routeValues));
    }
  }
}
