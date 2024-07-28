// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api.PyPiVersionsController
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Python")]
  [VersionedApiControllerCustomName(Area = "pypi", ResourceName = "versions", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowJson = true, AllowJsonPatch = true)]
  public class PyPiVersionsController : PyPiApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package), null, null)]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package> GetPackageVersion(
      string feedId,
      string packageName,
      string packageVersion,
      bool showDeleted = false)
    {
      PyPiVersionsController versionsController = this;
      Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package packageVersion1;
      using (versionsController.EnterTracer(nameof (GetPackageVersion)))
      {
        FeedCore feed = versionsController.GetFeedRequest(feedId).Feed;
        ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feed);
        Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package packageVersion2 = await versionsController.PyPiVersionsService.GetPackageVersion(versionsController.TfsRequestContext, feed, packageName, packageVersion, showDeleted);
        packageVersion2.SetSecuredObject(securedObject);
        packageVersion1 = packageVersion2;
      }
      return packageVersion1;
    }

    [HttpDelete]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package), null, null)]
    public async Task<HttpResponseMessage> DeletePackageVersion(
      string feedId,
      string packageName,
      string packageVersion)
    {
      PyPiVersionsController versionsController = this;
      HttpResponseMessage response;
      using (versionsController.EnterTracer(nameof (DeletePackageVersion)))
      {
        FeedCore feed = versionsController.GetFeedRequest(feedId).Feed;
        Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package package = await versionsController.PyPiVersionsService.DeletePackageVersion(versionsController.TfsRequestContext, feed, packageName, packageVersion);
        response = versionsController.Request.CreateResponse<Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package>(HttpStatusCode.Accepted, package);
      }
      return response;
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientSwaggerOperationId("Update Package Version")]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public async Task<HttpResponseMessage> UpdatePackageVersion(
      string feedId,
      string packageName,
      string packageVersion,
      PackageVersionDetails packageVersionDetails)
    {
      PyPiVersionsController versionsController = this;
      HttpResponseMessage response;
      using (versionsController.EnterTracer(nameof (UpdatePackageVersion)))
      {
        FeedCore feed = versionsController.GetFeedRequest(feedId).Feed;
        await versionsController.PyPiVersionsService.UpdatePackageVersion(versionsController.TfsRequestContext, feed, packageName, packageVersion, packageVersionDetails);
        response = versionsController.Request.CreateResponse(HttpStatusCode.Accepted);
      }
      return response;
    }
  }
}
