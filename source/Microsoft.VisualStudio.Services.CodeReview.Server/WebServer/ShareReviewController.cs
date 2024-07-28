// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.WebServer.ShareReviewController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.WebServer
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "share")]
  public class ShareReviewController : CodeReviewApiControllerBase
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage ShareReview(int reviewId, NotificationContext userMessage)
    {
      this.TfsRequestContext.GetService<ICodeReviewShareService>().ShareReview(this.TfsRequestContext, this.GetProjectId(), reviewId, userMessage);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
