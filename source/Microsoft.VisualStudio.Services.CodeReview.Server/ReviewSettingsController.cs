// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ReviewSettingsController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "settings")]
  public class ReviewSettingsController : CodeReviewApiControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (ReviewSettings), null, null)]
    public HttpResponseMessage GetReviewSettings() => this.Request.CreateResponse<ReviewSettings>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ICodeReviewSettingService>().GetReviewSettings(this.TfsRequestContext, this.GetProjectId()));

    [HttpPost]
    [ClientResponseType(typeof (ReviewSettings), null, null)]
    public HttpResponseMessage CreateReviewSettings(ReviewSettings reviewSettings) => reviewSettings == null ? this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, CodeReviewResources.ReviewSettingsMalformed()) : this.Request.CreateResponse<ReviewSettings>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ICodeReviewSettingService>().SaveReviewSettings(this.TfsRequestContext, this.GetProjectId(), reviewSettings));

    [HttpPut]
    [ClientResponseType(typeof (ReviewSettings), null, null)]
    public HttpResponseMessage UpdateReviewSettings(ReviewSettings reviewSettings) => reviewSettings == null ? this.Request.CreateResponse<string>(HttpStatusCode.BadRequest, CodeReviewResources.ReviewSettingsMalformed()) : this.Request.CreateResponse<ReviewSettings>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ICodeReviewSettingService>().SaveReviewSettings(this.TfsRequestContext, this.GetProjectId(), reviewSettings));
  }
}
