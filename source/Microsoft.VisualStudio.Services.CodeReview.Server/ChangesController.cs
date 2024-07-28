// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ChangesController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "changes")]
  public class ChangesController : CodeReviewApiControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (IterationChanges), null, null)]
    public HttpResponseMessage GetChanges(
      int reviewId,
      int iterationId,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$compareTo")] int? compareTo = null)
    {
      ICodeReviewIterationService service = this.TfsRequestContext.GetService<ICodeReviewIterationService>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.GetProjectId();
      int reviewId1 = reviewId;
      List<int> iterationIds = new List<int>();
      iterationIds.Add(iterationId);
      int? top1 = top;
      int? skip1 = skip;
      int? baseIteration = compareTo;
      IEnumerable<ChangeEntry> changeList = service.GetChangeList(tfsRequestContext, projectId, reviewId1, iterationIds, top1, skip1, baseIteration);
      HttpResponseMessage responseMessage = this.Request.CreateResponse<IEnumerable<ChangeEntry>>(HttpStatusCode.OK, changeList);
      int? nullable = changeList != null ? new int?(changeList.Count<ChangeEntry>()) : new int?();
      int num = 0;
      if (nullable.GetValueOrDefault() > num & nullable.HasValue)
      {
        int totalChangesCount = changeList.First<ChangeEntry>().TotalChangesCount;
        responseMessage = this.AddNextPageHeaders(responseMessage, top, skip, new int?(totalChangesCount), new int?(2000));
      }
      return responseMessage;
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetContent", "application/octet-stream")]
    public HttpResponseMessage GetContent(
      int reviewId,
      int iterationId,
      int changeId,
      string fileTarget)
    {
      ChangeEntryFileType changeEntryFileType = CodeReviewApiControllerBase.ParseChangeEntryFileType(fileTarget);
      return this.CreateDownloadResponse(this.TfsRequestContext.GetService<ICodeReviewIterationService>().ReadChangeEntryFile(this.TfsRequestContext, this.GetProjectId(), reviewId, iterationId, changeId, changeEntryFileType));
    }
  }
}
