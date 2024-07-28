// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent10
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class RatingAndReviewComponent10 : RatingAndReviewComponent9
  {
    public override Review CreateReview(
      byte rating,
      string title,
      string text,
      string productId,
      string productVersion,
      Guid userId,
      DateTime creationTime)
    {
      try
      {
        this.TraceEnter(12061014, nameof (CreateReview));
        string str = "Gallery.prc_CreateReview";
        this.PrepareStoredProcedure(str);
        this.BindByte(nameof (rating), rating);
        this.BindString(nameof (title), title, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("reviewText", text, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString(nameof (productVersion), productVersion, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindGuid(nameof (userId), userId);
        this.BindDateTime(nameof (creationTime), creationTime);
        Review review = (Review) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent.ReviewsBinder());
          review = resultCollection.GetCurrent<Review>().Items[0];
        }
        return review;
      }
      catch (Exception ex)
      {
        this.TraceException(12061016, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061015, nameof (CreateReview));
      }
    }

    public override Review UpdatePublisherReply(
      long reviewId,
      Guid userId,
      string title,
      string replyText,
      DateTime modifiedTime,
      bool isAdminReply = false)
    {
      try
      {
        this.TraceEnter(12061044, nameof (UpdatePublisherReply));
        string str = "Gallery.prc_UpdatePublisherReply";
        this.PrepareStoredProcedure(str);
        this.BindLong(nameof (reviewId), reviewId);
        this.BindGuid(nameof (userId), userId);
        this.BindString(nameof (title), title, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString(nameof (replyText), replyText, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindDateTime(nameof (modifiedTime), modifiedTime);
        Review review = (Review) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent.ReviewsBinder());
          review = resultCollection.GetCurrent<Review>().Items[0];
          resultCollection.AddBinder<ReviewReply>((ObjectBinder<ReviewReply>) new RatingAndReviewComponent9.ReviewReplyBinder2());
          resultCollection.NextResult();
          ReviewReply reviewReply = resultCollection.GetCurrent<ReviewReply>().Items[0];
          review.Reply = reviewReply;
        }
        return review;
      }
      catch (Exception ex)
      {
        this.TraceException(12061016, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061045, nameof (UpdatePublisherReply));
      }
    }
  }
}
