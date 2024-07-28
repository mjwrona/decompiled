// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ArtifactVisitsBatchController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "visits", ResourceName = "artifactVisitsBatch")]
  public class ArtifactVisitsBatchController : CodeReviewApiControllerBase
  {
    [HttpPost]
    [ClientResponseType(typeof (IEnumerable<ArtifactVisit>), null, null)]
    [ClientLocationId("D1786677-7A19-445B-9A7A-25728F48D149")]
    public IEnumerable<ArtifactVisit> GetVisits([FromBody] IEnumerable<ArtifactVisit> visits) => this.TfsRequestContext.GetService<ICodeReviewVisitService>().QueryVisits(this.TfsRequestContext, visits);
  }
}
