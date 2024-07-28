// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ICodeReviewAttachmentService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [DefaultServiceImplementation(typeof (CodeReviewAttachmentService))]
  public interface ICodeReviewAttachmentService : IVssFrameworkService
  {
    Attachment GetAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int attachmentId);

    IEnumerable<Attachment> GetAttachments(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      DateTime? modifiedSince = null);

    Attachment SaveAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Attachment attachment);

    void DeleteAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int attachmentId,
      bool deleteFile = false);
  }
}
