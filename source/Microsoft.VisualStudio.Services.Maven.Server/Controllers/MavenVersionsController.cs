// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Controllers.MavenVersionsController
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Maven.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Maven")]
  [VersionedApiControllerCustomName(Area = "maven", ResourceName = "versions", ResourceVersion = 1)]
  public class MavenVersionsController : MavenBaseController
  {
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientSwaggerOperationId("DeletePackageVersion")]
    [ClientResponseCode(HttpStatusCode.Accepted, "The package has been successfully marked for deletion.", true)]
    [ControllerMethodTraceFilter(12090910)]
    public async Task<HttpResponseMessage> PackageDelete(
      string feed,
      string groupId,
      string artifactId,
      string version)
    {
      MavenVersionsController versionsController = this;
      FeedCore feed1 = versionsController.GetFeedRequest(feed).Feed;
      await versionsController.MavenPackageVersionService.DeletePackageVersion(versionsController.TfsRequestContext, feed1, groupId, artifactId, version);
      return versionsController.Request.CreateResponse(HttpStatusCode.Accepted);
    }

    [HttpGet]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package), null, null)]
    [ControllerMethodTraceFilter(12090920)]
    [PackagingPublicProjectRequestRestrictions]
    public Task<Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package> GetPackageVersion(
      string feed,
      string groupId,
      string artifactId,
      string version,
      bool showDeleted = false)
    {
      return this.GetPackageVersionInternal(feed, groupId, artifactId, version, new Guid?(), showDeleted);
    }

    [HttpGet]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package), null, null)]
    [ControllerMethodTraceFilter(12090920)]
    [PackagingPublicProjectRequestRestrictions]
    [ClientIgnore]
    public Task<Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package> GetPackageVersion(
      string feed,
      string groupId,
      string artifactId,
      string version,
      Guid aadTenantId,
      bool showDeleted = false)
    {
      return this.GetPackageVersionInternal(feed, groupId, artifactId, version, new Guid?(aadTenantId), showDeleted);
    }

    private async Task<Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package> GetPackageVersionInternal(
      string feed,
      string groupId,
      string artifactId,
      string version,
      Guid? aadTenantId,
      bool showDeleted = false)
    {
      MavenVersionsController versionsController = this;
      FeedCore feed1 = versionsController.GetFeedRequest(feed).Feed;
      if (aadTenantId.HasValue)
        new UpstreamVerificationHelperBootstrapper(versionsController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(versionsController.TfsRequestContext, feed1, aadTenantId.Value);
      ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feed1);
      Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package packageVersion = await versionsController.MavenPackageVersionService.GetPackageVersion(versionsController.TfsRequestContext, feed1, groupId, artifactId, version, showDeleted);
      packageVersion.SetSecuredObject(securedObject);
      Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package packageVersionInternal = packageVersion;
      securedObject = (ISecuredObject) null;
      return packageVersionInternal;
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientSwaggerOperationId("Update Package Version")]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public async Task<HttpResponseMessage> UpdatePackageVersion(
      string feed,
      string groupId,
      string artifactId,
      string version,
      PackageVersionDetails packageVersionDetails)
    {
      MavenVersionsController versionsController = this;
      HttpResponseMessage response;
      using (versionsController.EnterTracer(nameof (UpdatePackageVersion)))
      {
        FeedCore feed1 = versionsController.GetFeedRequest(feed).Feed;
        await versionsController.MavenPackageVersionService.UpdatePackageVersion(versionsController.TfsRequestContext, feed1, groupId, artifactId, version, packageVersionDetails);
        response = versionsController.Request.CreateResponse(HttpStatusCode.Accepted);
      }
      return response;
    }
  }
}
