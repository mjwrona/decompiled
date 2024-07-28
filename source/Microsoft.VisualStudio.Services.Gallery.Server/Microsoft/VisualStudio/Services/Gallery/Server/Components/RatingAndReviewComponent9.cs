// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent9
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class RatingAndReviewComponent9 : RatingAndReviewComponent8
  {
    public override Review UpdatePublisherReply(
      long reviewId,
      Guid userId,
      string title,
      string replyText,
      string productVersion)
    {
      throw new InvalidOperationException(GalleryResources.OperationNotSupported((object) "CreateReplyForReview"));
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

    public override ReviewsResult GetReviews(
      string productId,
      int count,
      ReviewFilterOptions filterOptions,
      DateTime? beforeDate,
      DateTime? afterDate)
    {
      try
      {
        this.TraceEnter(12061012, nameof (GetReviews));
        this.PrepareStoredProcedure("Gallery.prc_GetReviews");
        this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindInt(nameof (count), count);
        this.BindInt(nameof (filterOptions), (int) filterOptions);
        if (beforeDate.HasValue)
          this.BindDateTime(nameof (beforeDate), beforeDate.Value.ToUniversalTime());
        if (afterDate.HasValue)
          this.BindDateTime(nameof (afterDate), afterDate.Value.ToUniversalTime());
        List<Review> reviews = (List<Review>) null;
        RatingAndReviewComponent2.ReviewsMetaData reviewsMetaData = new RatingAndReviewComponent2.ReviewsMetaData()
        {
          HasMoreItems = false,
          TotalRecords = -1
        };
        List<ReviewReply> replies = (List<ReviewReply>) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetReviews", this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent.ReviewsBinder());
          reviews = resultCollection.GetCurrent<Review>().Items;
          resultCollection.AddBinder<RatingAndReviewComponent2.ReviewsMetaData>((ObjectBinder<RatingAndReviewComponent2.ReviewsMetaData>) new RatingAndReviewComponent2.ReviewsMetaDataBinder());
          resultCollection.NextResult();
          reviewsMetaData = resultCollection.GetCurrent<RatingAndReviewComponent2.ReviewsMetaData>().Items[0];
          resultCollection.AddBinder<ReviewReply>((ObjectBinder<ReviewReply>) new RatingAndReviewComponent9.ReviewReplyBinder2());
          resultCollection.NextResult();
          replies = resultCollection.GetCurrent<ReviewReply>().Items;
        }
        List<Review> reviewList = this.CombineReviewsAndReplies(reviews, replies);
        return new ReviewsResult()
        {
          TotalReviewCount = (long) reviewsMetaData.TotalRecords,
          Reviews = reviewList,
          HasMoreReviews = reviewsMetaData.HasMoreItems
        };
      }
      catch (Exception ex)
      {
        this.TraceException(12061016, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061013, nameof (GetReviews));
      }
    }

    public override Review UpdateReview(
      string productId,
      long reviewId,
      byte rating,
      string title,
      string text,
      string productVersion,
      Guid userId,
      DateTime modifiedTime)
    {
      try
      {
        this.TraceEnter(12061019, nameof (UpdateReview));
        this.PrepareStoredProcedure("Gallery.prc_UpdateReview");
        this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindLong(nameof (reviewId), reviewId);
        this.BindByte(nameof (rating), rating);
        this.BindString(nameof (title), title, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("reviewText", text, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString(nameof (productVersion), productVersion, 25, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindGuid(nameof (userId), userId);
        Review review = (Review) null;
        ReviewReply reviewReply = (ReviewReply) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateReview", this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent.ReviewsBinder());
          review = resultCollection.GetCurrent<Review>().Items[0];
          resultCollection.AddBinder<ReviewReply>((ObjectBinder<ReviewReply>) new RatingAndReviewComponent9.ReviewReplyBinder2());
          resultCollection.NextResult();
          List<ReviewReply> items = resultCollection.GetCurrent<ReviewReply>().Items;
          if (items != null)
          {
            if (items.Count > 0)
              reviewReply = items[0];
          }
        }
        review.Reply = reviewReply;
        return review;
      }
      catch (Exception ex)
      {
        this.TraceException(12061016, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061020, nameof (UpdateReview));
      }
    }

    internal class ReviewReplyBinder2 : ObjectBinder<ReviewReply>
    {
      private SqlColumnBinder m_idColumn = new SqlColumnBinder("Id");
      private SqlColumnBinder m_reviewIdColumn = new SqlColumnBinder("ReviewId");
      private SqlColumnBinder m_userIdColumn = new SqlColumnBinder("UserId");
      private SqlColumnBinder m_titleColumn = new SqlColumnBinder("Title");
      private SqlColumnBinder m_replyTextColumn = new SqlColumnBinder("ReplyText");
      private SqlColumnBinder m_updatedDateColumn = new SqlColumnBinder("UpdatedDate");
      private SqlColumnBinder m_isDeletedColumn = new SqlColumnBinder("IsDeleted");

      protected override ReviewReply Bind() => new ReviewReply()
      {
        Id = this.m_idColumn.GetInt64((IDataReader) this.Reader),
        ReviewId = this.m_reviewIdColumn.GetInt64((IDataReader) this.Reader),
        UserId = this.m_userIdColumn.GetGuid((IDataReader) this.Reader),
        Title = this.m_titleColumn.GetString((IDataReader) this.Reader, true),
        ReplyText = this.m_replyTextColumn.GetString((IDataReader) this.Reader, false),
        UpdatedDate = this.m_updatedDateColumn.GetDateTime((IDataReader) this.Reader),
        IsDeleted = this.m_isDeletedColumn.GetBoolean((IDataReader) this.Reader)
      };
    }
  }
}
