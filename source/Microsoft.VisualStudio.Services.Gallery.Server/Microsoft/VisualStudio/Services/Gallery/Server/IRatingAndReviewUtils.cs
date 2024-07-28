// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IRatingAndReviewUtils
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public interface IRatingAndReviewUtils
  {
    Microsoft.VisualStudio.Services.Identity.Identity GetAuthenticatedIdentity(
      IVssRequestContext requestContext);

    UserReportedConcern SanitizeReportedConcern(UserReportedConcern reportedConcern);

    Review SanitizeReview(Review review);

    ReviewReply SanitizeReviewReply(ReviewReply reviewReply);

    ReviewPatch SanitizeReviewPatch(ReviewPatch reviewPatch);

    void ValidateReportedConcern(UserReportedConcern reportedConcern);

    void ValidateReview(Review review);

    void ValidateReviewPatch(ReviewPatch reviewPatch, string productId, long reviewId);

    void PublishReCaptchaTokenCIForReview(
      IVssRequestContext requestContext,
      IDictionary<string, object> ciData);
  }
}
