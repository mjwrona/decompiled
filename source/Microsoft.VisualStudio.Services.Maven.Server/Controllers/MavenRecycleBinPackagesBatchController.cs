// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Controllers.MavenRecycleBinPackagesBatchController
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Maven.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [ClientGroupByResource("Maven")]
  [VersionedApiControllerCustomName(Area = "maven", ResourceName = "recyclebinpackagesbatch", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowJson = true)]
  public class MavenRecycleBinPackagesBatchController : MavenBaseController
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    [ControllerMethodTraceFilter(12091400)]
    public async Task<HttpResponseMessage> UpdateRecycleBinPackagesAsync(
      string feed,
      [FromBody] MavenPackagesBatchRequest batchRequest)
    {
      MavenRecycleBinPackagesBatchController packagesBatchController = this;
      HttpResponseMessage response;
      using (packagesBatchController.EnterTracer(nameof (UpdateRecycleBinPackagesAsync)))
      {
        packagesBatchController.ValidateBatchOperationType(batchRequest, BatchOperationType.RestoreToFeed, BatchOperationType.PermanentDelete);
        FeedCore feed1 = packagesBatchController.GetFeedRequest(feed).Feed;
        await packagesBatchController.MavenPackageVersionService.UpdatePackageVersions(packagesBatchController.TfsRequestContext, feed1, batchRequest);
        response = packagesBatchController.Request.CreateResponse(HttpStatusCode.Accepted);
      }
      return response;
    }
  }
}
