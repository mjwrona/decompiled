// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api.NuGetPackagesBatchController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("NuGet")]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "packagesBatch", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowJson = true)]
  public class NuGetPackagesBatchController : NuGetApiController
  {
    [HttpPost]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public async Task<HttpResponseMessage> UpdatePackageVersions(
      string feedId,
      [FromBody] NuGetPackagesBatchRequest batchRequest)
    {
      NuGetPackagesBatchController packagesBatchController = this;
      HttpResponseMessage response;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(packagesBatchController.TfsRequestContext, NuGetTracePoints.NuGetPackagesBatchController.TraceData, 5720500, nameof (UpdatePackageVersions)))
      {
        FeedCore feed = packagesBatchController.GetFeedRequest(feedId).Feed;
        await packagesBatchController.NuGetVersionsService.UpdatePackageVersions(packagesBatchController.TfsRequestContext, feed, batchRequest);
        response = packagesBatchController.Request.CreateResponse(HttpStatusCode.Accepted);
      }
      return response;
    }
  }
}
