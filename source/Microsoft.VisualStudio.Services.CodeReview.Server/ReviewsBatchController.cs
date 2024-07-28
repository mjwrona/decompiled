// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ReviewsBatchController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "reviewsBatch")]
  public class ReviewsBatchController : CodeReviewApiControllerBase
  {
    [HttpPost]
    [ClientResponseType(typeof (List<Review>), null, null)]
    public HttpResponseMessage GetReviewsBatch([FromBody] string[] sourceArtifactIds, [FromUri(Name = "includeDeleted")] bool includeDeleted = false) => this.GenerateResponse<Review>(this.TfsRequestContext.GetService<ICodeReviewService>().QueryReviewsBySourceArtifactIds(this.TfsRequestContext, this.GetProjectId(), (IEnumerable<string>) sourceArtifactIds, includeDeleted));
  }
}
