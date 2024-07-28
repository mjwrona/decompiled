// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ArtifactVisitsController
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
  [VersionedApiControllerCustomName(Area = "visits", ResourceName = "artifactVisits")]
  public class ArtifactVisitsController : CodeReviewApiControllerBase
  {
    [HttpPut]
    [ClientResponseType(typeof (ArtifactVisit), null, null)]
    [ClientLocationId("C4BC78AB-8D09-4B62-98F2-EFB1AFFE50F8")]
    public HttpResponseMessage UpdateLastVisit([FromBody] ArtifactVisit visit)
    {
      ICodeReviewVisitService service = this.TfsRequestContext.GetService<ICodeReviewVisitService>();
      return !service.VerifyArtifactId(this.TfsRequestContext, visit?.ArtifactId) ? this.Request.CreateResponse<string>(HttpStatusCode.NotFound, CodeReviewResources.InvalidArtifactIdForVisitUpdate((object) visit.ArtifactId)) : this.Request.CreateResponse<ArtifactVisit>(HttpStatusCode.OK, service.UpdateLastVisit(this.TfsRequestContext, visit));
    }
  }
}
