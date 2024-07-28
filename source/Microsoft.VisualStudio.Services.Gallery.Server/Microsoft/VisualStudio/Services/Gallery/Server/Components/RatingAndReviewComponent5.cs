// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent5
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
  internal class RatingAndReviewComponent5 : RatingAndReviewComponent4
  {
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
          resultCollection.AddBinder<ReviewReply>((ObjectBinder<ReviewReply>) new RatingAndReviewComponent4.ReviewReplyBinder());
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
          resultCollection.AddBinder<ReviewReply>((ObjectBinder<ReviewReply>) new RatingAndReviewComponent4.ReviewReplyBinder());
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

    protected virtual List<Review> CombineReviewsAndReplies(
      List<Review> reviews,
      List<ReviewReply> replies)
    {
      if (replies == null || replies.Count <= 0)
        return reviews;
      Dictionary<long, ReviewReply> dictionary = new Dictionary<long, ReviewReply>();
      int count1 = replies.Count;
      for (int index = 0; index < count1; ++index)
        dictionary.Add(replies[index].ReviewId, replies[index]);
      int count2 = reviews.Count;
      for (int index = 0; index < count2; ++index)
      {
        if (dictionary.ContainsKey(reviews[index].Id))
          reviews[index].Reply = dictionary[reviews[index].Id];
      }
      return reviews;
    }
  }
}
