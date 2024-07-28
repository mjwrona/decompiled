// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ArtifactStatsBatchController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "visits", ResourceName = "artifactStatsBatch")]
  public class ArtifactStatsBatchController : CodeReviewApiControllerBase
  {
    [HttpPost]
    [ClientResponseType(typeof (IEnumerable<ArtifactStats>), null, null)]
    [ClientLocationId("2D358C96-88CC-42BA-9B5D-A2CB26C64972")]
    public IEnumerable<ArtifactStats> GetStats(
      [FromBody] IEnumerable<ArtifactStats> stats,
      [FromUri(Name = "includeUpdatesSinceLastVisit")] bool includeUpdatesSinceLastVisit = false)
    {
      return this.TfsRequestContext.GetService<ICodeReviewVisitService>().QueryStats(this.TfsRequestContext, stats, includeUpdatesSinceLastVisit);
    }
  }
}
