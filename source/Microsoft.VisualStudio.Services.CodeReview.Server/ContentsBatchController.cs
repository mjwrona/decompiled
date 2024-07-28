// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ContentsBatchController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "contentsBatch")]
  public class ContentsBatchController : CodeReviewApiControllerBase
  {
    [HttpPost]
    [ClientResponseType(typeof (ReviewFilesZipContent), "DownloadContentsBatchZip", "application/zip")]
    public HttpResponseMessage DownloadContentsBatch(
      int reviewId,
      [FromBody] DownloadContentsCriteria downloadContentsCriteria,
      [FromUri(Name = "top")] int? top = null,
      [FromUri(Name = "skip")] int? skip = null,
      [FromUri(Name = "downloadFileName")] string downloadFileName = null)
    {
      ValidationHelper.ValidateDownloadContentsCriteria(downloadContentsCriteria);
      if (MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Zip,
        RequestMediaType.None
      }).FirstOrDefault<RequestMediaType>() != RequestMediaType.Zip)
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, CodeReviewResources.MustAcceptZip());
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(30.0);
      HttpResponseMessage httpResponseMessage = this.Request.CreateResponse(HttpStatusCode.OK);
      bool needsNextPage = false;
      if (downloadContentsCriteria.ContentHashes != null && downloadContentsCriteria.ContentHashes.Count > 0)
      {
        ReviewFileType fileType = !downloadContentsCriteria.FileType.HasValue ? ReviewFileType.ChangeEntry : downloadContentsCriteria.FileType.Value;
        ICodeReviewService service = this.TfsRequestContext.GetService<ICodeReviewService>();
        httpResponseMessage.Content = (HttpContent) service.DownloadFilesZipped(this.TfsRequestContext, this.GetProjectId(), reviewId, downloadContentsCriteria.ContentHashes, out needsNextPage, top, skip, fileType);
      }
      else if (downloadContentsCriteria.IterationIds != null && downloadContentsCriteria.IterationIds.Count > 0)
      {
        ICodeReviewIterationService service = this.TfsRequestContext.GetService<ICodeReviewIterationService>();
        httpResponseMessage.Content = (HttpContent) service.GetIterationFilesContent(this.TfsRequestContext, this.GetProjectId(), reviewId, downloadContentsCriteria.IterationIds, downloadContentsCriteria.FilterBy, out needsNextPage, top, skip);
      }
      if (needsNextPage)
        httpResponseMessage = this.AddNextPageHeaders(httpResponseMessage, top, skip);
      downloadFileName = CodeReviewApiControllerBase.TrimAndValidateFilename(downloadFileName);
      downloadFileName = downloadFileName ?? string.Format("logs_{0}.{1}", (object) reviewId, (object) ".zip");
      CodeReviewApiControllerBase.AddContentDisposition(httpResponseMessage, downloadFileName);
      return httpResponseMessage;
    }
  }
}
