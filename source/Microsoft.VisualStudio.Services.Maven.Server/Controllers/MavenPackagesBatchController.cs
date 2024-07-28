// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Controllers.MavenPackagesBatchController
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Maven.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [ClientGroupByResource("Maven")]
  [VersionedApiControllerCustomName(Area = "maven", ResourceName = "packagesBatch", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowJson = true)]
  public class MavenPackagesBatchController : MavenBaseController
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ControllerMethodTraceFilter(12091500)]
    public async Task<HttpResponseMessage> UpdatePackageVersions(
      string feedId,
      [FromBody] MavenPackagesBatchRequest batchRequest)
    {
      MavenPackagesBatchController packagesBatchController = this;
      HttpResponseMessage response;
      using (packagesBatchController.EnterTracer(nameof (UpdatePackageVersions)))
      {
        packagesBatchController.ValidateBatchOperationType(batchRequest, BatchOperationType.Delete, BatchOperationType.Promote);
        IFeedRequest feedRequest = packagesBatchController.GetFeedRequest(feedId);
        await packagesBatchController.MavenPackageVersionService.UpdatePackageVersions(packagesBatchController.TfsRequestContext, feedRequest.Feed, batchRequest);
        response = packagesBatchController.Request.CreateResponse(HttpStatusCode.Accepted);
      }
      return response;
    }
  }
}
