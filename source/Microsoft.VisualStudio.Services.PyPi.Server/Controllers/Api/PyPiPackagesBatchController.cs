// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api.PyPiPackagesBatchController
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Python")]
  [VersionedApiControllerCustomName(Area = "pypi", ResourceName = "packagesBatch", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowJson = true)]
  public class PyPiPackagesBatchController : PyPiApiController
  {
    [HttpPost]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public async Task<HttpResponseMessage> UpdatePackageVersions(
      string feedId,
      [FromBody] PyPiPackagesBatchRequest batchRequest)
    {
      PyPiPackagesBatchController packagesBatchController = this;
      HttpResponseMessage response;
      using (packagesBatchController.EnterTracer(nameof (UpdatePackageVersions)))
      {
        IFeedRequest feedRequest = packagesBatchController.GetFeedRequest(feedId);
        await packagesBatchController.PyPiVersionsService.UpdatePackageVersions(packagesBatchController.TfsRequestContext, feedRequest.Feed, batchRequest);
        response = packagesBatchController.Request.CreateResponse(HttpStatusCode.Accepted);
      }
      return response;
    }
  }
}
