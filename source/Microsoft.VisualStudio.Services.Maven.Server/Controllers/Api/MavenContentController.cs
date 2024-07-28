// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Controllers.Api.MavenContentController
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Maven.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Maven")]
  [VersionedApiControllerCustomName(Area = "maven", ResourceName = "content", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Maven.Service")]
  public class MavenContentController : MavenBaseController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientSwaggerOperationId("DownloadPackage")]
    [ControllerMethodTraceFilter(12091600)]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<HttpResponseMessage> DownloadPackage(
      string feedId,
      string groupId,
      string artifactId,
      string version,
      string fileName)
    {
      MavenContentController contentController = this;
      HttpResponseMessage httpResponseMessage;
      using (contentController.EnterTracer(nameof (DownloadPackage)))
      {
        FeedCore feed = contentController.GetFeedRequest(feedId).Feed;
        contentController.TfsRequestContext.UpdateTimeToFirstPage();
        string path = groupId + "/" + artifactId + "/" + version + "/" + fileName;
        httpResponseMessage = await new MavenContentResponseMessageGenerator().Handle(await contentController.MavenPackageVersionService.GetPackageFile(contentController.TfsRequestContext, feed, path, true, contentController.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment));
      }
      return httpResponseMessage;
    }
  }
}
