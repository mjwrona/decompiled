// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent14
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501: Avoid excessive inheritance", Justification = "Need inhertance here")]
  internal class RatingAndReviewComponent14 : RatingAndReviewComponent13
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
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent12.ReviewsBinder2());
          reviews = resultCollection.GetCurrent<Review>().Items;
          resultCollection.AddBinder<RatingAndReviewComponent2.ReviewsMetaData>((ObjectBinder<RatingAndReviewComponent2.ReviewsMetaData>) new RatingAndReviewComponent2.ReviewsMetaDataBinder());
          resultCollection.NextResult();
          reviewsMetaData = resultCollection.GetCurrent<RatingAndReviewComponent2.ReviewsMetaData>().Items[0];
          resultCollection.AddBinder<ReviewReply>((ObjectBinder<ReviewReply>) new RatingAndReviewComponent12.ReviewReplyBinder3());
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
  }
}
