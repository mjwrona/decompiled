// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api.NuGetVersionsController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("NuGet")]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "Versions", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowJson = true, AllowJsonPatch = true)]
  public class NuGetVersionsController : NuGetApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package), null, null)]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package> GetPackageVersion(
      string feedId,
      string packageName,
      string packageVersion,
      bool showDeleted = false)
    {
      NuGetVersionsController versionsController = this;
      Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package packageVersion1;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(versionsController.TfsRequestContext, NuGetTracePoints.NuGetPackagingApiController.TraceData, 5720600, nameof (GetPackageVersion)))
      {
        FeedCore feed = versionsController.GetFeedRequest(feedId).Feed;
        ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feed);
        Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package packageVersion2 = await versionsController.NuGetVersionsService.GetPackageVersion(versionsController.TfsRequestContext, feed, packageName, packageVersion, showDeleted);
        packageVersion2.SetSecuredObject(securedObject);
        packageVersion1 = packageVersion2;
      }
      return packageVersion1;
    }

    [HttpDelete]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package), null, null)]
    public async Task<HttpResponseMessage> DeletePackageVersion(
      string feedId,
      string packageName,
      string packageVersion)
    {
      NuGetVersionsController versionsController = this;
      HttpResponseMessage response;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(versionsController.TfsRequestContext, NuGetTracePoints.NuGetPackagingApiController.TraceData, 5720610, nameof (DeletePackageVersion)))
      {
        FeedCore feed = versionsController.GetFeedRequest(feedId).Feed;
        Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package package = await versionsController.NuGetVersionsService.DeletePackageVersion(versionsController.TfsRequestContext, feed, packageName, packageVersion);
        response = versionsController.Request.CreateResponse<Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package>(HttpStatusCode.Accepted, package);
      }
      return response;
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public async Task<HttpResponseMessage> UpdatePackageVersion(
      string feedId,
      string packageName,
      string packageVersion,
      PackageVersionDetails packageVersionDetails)
    {
      NuGetVersionsController versionsController = this;
      HttpResponseMessage response;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(versionsController.TfsRequestContext, NuGetTracePoints.NuGetPackagingApiController.TraceData, 5720620, nameof (UpdatePackageVersion)))
      {
        FeedCore feed = versionsController.GetFeedRequest(feedId).Feed;
        await versionsController.NuGetVersionsService.UpdatePackageVersion(versionsController.TfsRequestContext, feed, packageName, packageVersion, packageVersionDetails);
        response = versionsController.Request.CreateResponse(HttpStatusCode.Accepted);
      }
      return response;
    }
  }
}
