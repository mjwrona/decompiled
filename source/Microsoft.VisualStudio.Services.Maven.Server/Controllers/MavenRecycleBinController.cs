// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Controllers.MavenRecycleBinController
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Maven.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Maven")]
  [VersionedApiControllerCustomName(Area = "maven", ResourceName = "RecycleBinVersions", ResourceVersion = 1)]
  public class MavenRecycleBinController : MavenBaseController
  {
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientSwaggerOperationId("DeletePackageVersionFromRecycleBin")]
    [ControllerMethodTraceFilter(12091300)]
    public async Task<HttpResponseMessage> DeletePackageVersionFromRecycleBin(
      string feed,
      string groupId,
      string artifactId,
      string version)
    {
      MavenRecycleBinController recycleBinController = this;
      HttpResponseMessage httpResponseMessage;
      using (recycleBinController.EnterTracer(nameof (DeletePackageVersionFromRecycleBin)))
      {
        FeedCore feed1 = recycleBinController.GetFeedRequest(feed).Feed;
        await recycleBinController.MavenPackageVersionService.PermanentDeletePackageVersion(recycleBinController.TfsRequestContext, feed1, groupId, artifactId, version);
        httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted);
      }
      return httpResponseMessage;
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ControllerMethodTraceFilter(12091310)]
    [ValidateModel]
    public async Task<HttpResponseMessage> RestorePackageVersionFromRecycleBin(
      string feed,
      string groupId,
      string artifactId,
      string version,
      MavenRecycleBinPackageVersionDetails packageVersionDetails)
    {
      MavenRecycleBinController recycleBinController = this;
      HttpResponseMessage response;
      using (recycleBinController.EnterTracer(nameof (RestorePackageVersionFromRecycleBin)))
      {
        FeedCore feed1 = recycleBinController.GetFeedRequest(feed).Feed;
        await recycleBinController.MavenPackageVersionService.RestorePackageVersionToFeed(recycleBinController.TfsRequestContext, feed1, groupId, artifactId, version, (IRecycleBinPackageVersionDetails) packageVersionDetails);
        response = recycleBinController.Request.CreateResponse(HttpStatusCode.Accepted);
      }
      return response;
    }

    [HttpGet]
    [ClientResponseType(typeof (MavenPackageVersionDeletionState), null, null)]
    [ControllerMethodTraceFilter(12091320)]
    [ClientSwaggerOperationId("GetPackageVersionFromRecycleBin")]
    public async Task<MavenPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBin(
      string feed,
      string groupId,
      string artifactId,
      string version)
    {
      MavenRecycleBinController recycleBinController = this;
      MavenPackageVersionDeletionState versionDeletionState;
      using (recycleBinController.EnterTracer(nameof (GetPackageVersionMetadataFromRecycleBin)))
      {
        FeedCore feed1 = recycleBinController.GetFeedRequest(feed).Feed;
        versionDeletionState = (await recycleBinController.MavenPackageVersionService.GetPackageVersionMetadataFromRecycleBin(recycleBinController.TfsRequestContext, feed1, groupId, artifactId, version)).ToPackageVersionDeletionState();
      }
      return versionDeletionState;
    }
  }
}
