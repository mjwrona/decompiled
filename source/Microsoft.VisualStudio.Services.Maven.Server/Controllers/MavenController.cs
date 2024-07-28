// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Controllers.MavenController
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Attributes;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Maven.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Maven")]
  [VersionedApiControllerCustomName(Area = "maven", ResourceName = "v1", ResourceVersion = 1)]
  [ErrorInReasonPhraseExceptionFilter]
  public class MavenController : MavenBaseController
  {
    protected override bool ExemptFromGlobalExceptionFormatting { get; } = true;

    [HttpGet]
    [HttpHead]
    [ClientIgnore]
    [ControllerMethodTraceFilter(12090030)]
    [MavenNoRedirectPublicProjectRequestRestrictions]
    [PackagingPublicProjectRequestRestrictions]
    public IHttpActionResult Index(string feed)
    {
      IFeedRequest feedRequest = this.GetFeedRequest(feed);
      string content = JsonConvert.SerializeObject((object) new
      {
        Info = Microsoft.VisualStudio.Services.Maven.Server.Resources.Info_RepositoryLandingPage((object) feed, (object) this.TraceArea)
      });
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feedRequest.Feed);
      HttpRequestMessage request = this.Request;
      ISecuredObject securedObject = securedObjectReadOnly;
      return (IHttpActionResult) new JsonResult(content, request, securedObject);
    }

    [HttpGet]
    [ClientIgnore]
    [ControllerMethodTraceFilter(12090010)]
    [MavenNoRedirectPublicProjectRequestRestrictions]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<HttpResponseMessage> GetPackageFileAsync(
      string feed,
      string path,
      Guid? aadTenantId = null)
    {
      MavenController mavenController = this;
      HttpResponseMessage responseMessage;
      using (mavenController.EnterTracer(nameof (GetPackageFileAsync)))
      {
        FeedCore feedCore = mavenController.GetFeedRequest(feed).Feed;
        if (aadTenantId.HasValue)
          new UpstreamVerificationHelperBootstrapper(mavenController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(mavenController.TfsRequestContext, feedCore, aadTenantId.Value);
        MavenPackageFileResponse packageFile = await mavenController.MavenPackageVersionService.GetPackageFile(mavenController.TfsRequestContext, feedCore, path, true);
        mavenController.TfsRequestContext.UpdateTimeToFirstPage();
        ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feedCore);
        responseMessage = MavenHttpResponseUtility.CreateResponseMessage(packageFile.FileName, packageFile.Content, securedObjectReadOnly);
      }
      return responseMessage;
    }

    [HttpHead]
    [ClientIgnore]
    [ControllerMethodTraceFilter(12090020)]
    [MavenNoRedirectPublicProjectRequestRestrictions]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<IHttpActionResult> HeadPackageFileAsync(string feed, string path)
    {
      MavenController mavenController = this;
      IHttpActionResult httpActionResult;
      using (mavenController.EnterTracer(nameof (HeadPackageFileAsync)))
      {
        FeedCore feed1 = mavenController.GetFeedRequest(feed).Feed;
        MavenPackageFileResponse packageFile = await mavenController.MavenPackageVersionService.GetPackageFile(mavenController.TfsRequestContext, feed1, path, false);
        httpActionResult = (IHttpActionResult) mavenController.Ok();
      }
      return httpActionResult;
    }
  }
}
