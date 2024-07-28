// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Internal.AllPackageVersionsController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Internal
{
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "allPackageVersionsInternal")]
  public class AllPackageVersionsController : NuGetApiController
  {
    [HttpGet]
    [ClientLocationId("4A1DC31F-D500-41E4-9FCA-2500FEE6B44C")]
    [ControllerMethodTraceFilter(5722540)]
    [ClientResponseType(typeof (Stream), null, null, MediaType = "application/octet-stream")]
    public async 
    #nullable disable
    Task<HttpResponseMessage> GetAllPackageVersionsInternalAsync(string feedId, Guid aadTenantId)
    {
      AllPackageVersionsController versionsController = this;
      IFeedRequest feedRequest = versionsController.GetFeedRequest(feedId);
      FeedSecurityHelper.CheckModifyIndexPermissions(versionsController.TfsRequestContext, feedRequest.Feed);
      new UpstreamVerificationHelperBootstrapper(versionsController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(versionsController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      byte[] listDocumentBytes = await (await NuGetAggregationResolver.Bootstrap(versionsController.TfsRequestContext).FactoryFor<INuGetPackageVersionCountsDocumentBytesService>().Get(feedRequest)).GetVersionListDocumentBytes(feedRequest);
      HttpResponseMessage response = versionsController.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new ByteArrayContent(listDocumentBytes);
      HttpResponseMessage versionsInternalAsync = response;
      feedRequest = (IFeedRequest) null;
      return versionsInternalAsync;
    }
  }
}
