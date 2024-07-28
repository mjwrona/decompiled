// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ReviewAttachmentsController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "attachments")]
  public class ReviewAttachmentsController : CodeReviewApiControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (List<Attachment>), null, null)]
    public HttpResponseMessage GetAttachments(int reviewId, [FromUri(Name = "modifiedSince")] DateTime? modifiedSince = null) => this.GenerateResponse<Attachment>(this.TfsRequestContext.GetService<ICodeReviewAttachmentService>().GetAttachments(this.TfsRequestContext, this.GetProjectId(), reviewId, modifiedSince));

    [HttpGet]
    [ClientResponseType(typeof (Attachment), null, null)]
    public HttpResponseMessage GetAttachment(int reviewId, int attachmentId) => this.Request.CreateResponse<Attachment>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ICodeReviewAttachmentService>().GetAttachment(this.TfsRequestContext, this.GetProjectId(), reviewId, attachmentId));

    [HttpPost]
    [ClientResponseType(typeof (Attachment), null, null)]
    public HttpResponseMessage AddAttachment(int reviewId, Attachment attachment)
    {
      if (attachment == null)
        throw new ArgumentNullException(nameof (attachment), CodeReviewResources.AttachmentMalformed());
      return this.Request.CreateResponse<Attachment>(HttpStatusCode.Created, this.TfsRequestContext.GetService<ICodeReviewAttachmentService>().SaveAttachment(this.TfsRequestContext, this.GetProjectId(), reviewId, attachment));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteAttachment(int reviewId, int attachmentId)
    {
      this.TfsRequestContext.GetService<ICodeReviewAttachmentService>().DeleteAttachment(this.TfsRequestContext, this.GetProjectId(), reviewId, attachmentId);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
