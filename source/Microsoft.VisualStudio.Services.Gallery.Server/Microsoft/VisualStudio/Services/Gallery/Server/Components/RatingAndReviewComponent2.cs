// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent2
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
  internal class RatingAndReviewComponent2 : RatingAndReviewComponent
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
        List<Review> reviewList = (List<Review>) null;
        RatingAndReviewComponent2.ReviewsMetaData reviewsMetaData = new RatingAndReviewComponent2.ReviewsMetaData()
        {
          HasMoreItems = false,
          TotalRecords = -1
        };
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetReviews", this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent.ReviewsBinder());
          reviewList = resultCollection.GetCurrent<Review>().Items;
          resultCollection.AddBinder<RatingAndReviewComponent2.ReviewsMetaData>((ObjectBinder<RatingAndReviewComponent2.ReviewsMetaData>) new RatingAndReviewComponent2.ReviewsMetaDataBinder());
          resultCollection.NextResult();
          reviewsMetaData = resultCollection.GetCurrent<RatingAndReviewComponent2.ReviewsMetaData>().Items[0];
        }
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
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateReview", this.RequestContext))
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
        this.TraceLeave(12061020, nameof (UpdateReview));
      }
    }

    public override List<Review> GetReviewsByUserId(Guid userId, string productId)
    {
      try
      {
        this.TraceEnter(12061023, nameof (GetReviewsByUserId));
        this.PrepareStoredProcedure("Gallery.prc_GetReviewsByUserId");
        this.BindGuid(nameof (userId), userId);
        if (productId != null)
          this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        List<Review> items;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetReviewsByUserId", this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent.ReviewsBinder());
          items = resultCollection.GetCurrent<Review>().Items;
        }
        return items;
      }
      catch (Exception ex)
      {
        this.TraceException(12061016, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061024, nameof (GetReviewsByUserId));
      }
    }

    public override int DeleteReviews(string productId, long? reviewId, bool isHardDelete = false)
    {
      try
      {
        this.TraceEnter(12061025, nameof (DeleteReviews));
        this.PrepareStoredProcedure("Gallery.prc_DeleteReviews");
        this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        if (reviewId.HasValue)
          this.BindLong(nameof (reviewId), reviewId.Value);
        List<Review> items;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_DeleteReviews", this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent.ReviewsBinder());
          items = resultCollection.GetCurrent<Review>().Items;
        }
        return items.Count;
      }
      catch (Exception ex)
      {
        this.TraceException(12061016, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061026, nameof (DeleteReviews));
      }
    }

    public override List<FlaggedReview> GetFlaggedReviews(string productId = null)
    {
      try
      {
        this.TraceEnter(12061027, nameof (GetFlaggedReviews));
        this.PrepareStoredProcedure("Gallery.prc_GetReportedReviews");
        if (!string.IsNullOrEmpty(productId))
          this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        List<FlaggedReview> items;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetReportedReviews", this.RequestContext))
        {
          resultCollection.AddBinder<FlaggedReview>((ObjectBinder<FlaggedReview>) new RatingAndReviewComponent2.FlaggedReviewsBinder());
          items = resultCollection.GetCurrent<FlaggedReview>().Items;
        }
        return items;
      }
      catch (Exception ex)
      {
        this.TraceException(12061016, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061028, nameof (GetFlaggedReviews));
      }
    }

    internal class FlaggedReviewsBinder : ObjectBinder<FlaggedReview>
    {
      protected SqlColumnBinder idColumn = new SqlColumnBinder("Id");
      protected SqlColumnBinder userIdColumn = new SqlColumnBinder("UserId");
      protected SqlColumnBinder productIdColumn = new SqlColumnBinder("ProductId");
      protected SqlColumnBinder createdDateColumn = new SqlColumnBinder("CreatedDate");
      protected SqlColumnBinder updatedDateColumn = new SqlColumnBinder("UpdatedDate");
      protected SqlColumnBinder ratingColumn = new SqlColumnBinder("Rating");
      protected SqlColumnBinder titleColumn = new SqlColumnBinder("Title");
      protected SqlColumnBinder reviewTextColumn = new SqlColumnBinder("ReviewText");
      protected SqlColumnBinder productVersionColumn = new SqlColumnBinder("ProductVersion");
      protected SqlColumnBinder isDeletedColumn = new SqlColumnBinder("IsDeleted");
      protected SqlColumnBinder reportedByColumn = new SqlColumnBinder("ReportedBy");
      protected SqlColumnBinder concernCategoryColumn = new SqlColumnBinder("ConcernCategory");
      protected SqlColumnBinder concernTextColumn = new SqlColumnBinder("ConcernText");
      protected SqlColumnBinder submittedDateColumn = new SqlColumnBinder("SubmittedDate");

      protected override FlaggedReview Bind()
      {
        FlaggedReview flaggedReview = new FlaggedReview();
        flaggedReview.Id = this.idColumn.GetInt64((IDataReader) this.Reader);
        flaggedReview.UserId = this.userIdColumn.GetGuid((IDataReader) this.Reader);
        flaggedReview.ProductId = this.productIdColumn.GetString((IDataReader) this.Reader, false);
        flaggedReview.CreatedDate = this.createdDateColumn.GetDateTime((IDataReader) this.Reader);
        flaggedReview.UpdatedDate = this.updatedDateColumn.GetDateTime((IDataReader) this.Reader);
        flaggedReview.Rating = this.ratingColumn.GetByte((IDataReader) this.Reader);
        flaggedReview.Title = this.titleColumn.GetString((IDataReader) this.Reader, true);
        flaggedReview.Text = this.reviewTextColumn.GetString((IDataReader) this.Reader, true);
        flaggedReview.ProductVersion = this.productVersionColumn.GetString((IDataReader) this.Reader, false);
        flaggedReview.IsDeleted = this.isDeletedColumn.GetBoolean((IDataReader) this.Reader);
        flaggedReview.ReportedBy = this.reportedByColumn.GetGuid((IDataReader) this.Reader);
        flaggedReview.ConcernCategory = this.concernCategoryColumn.GetInt32((IDataReader) this.Reader);
        flaggedReview.ConcernText = this.concernTextColumn.GetString((IDataReader) this.Reader, false);
        flaggedReview.ConcernSubmittedDate = this.submittedDateColumn.GetDateTime((IDataReader) this.Reader);
        return flaggedReview;
      }
    }

    internal class ReviewsMetaDataBinder : ObjectBinder<RatingAndReviewComponent2.ReviewsMetaData>
    {
      protected SqlColumnBinder hasMoreItemsColumn = new SqlColumnBinder("HasMoreItems");
      protected SqlColumnBinder totalRecordsColumn = new SqlColumnBinder("TotalRecords");

      protected override RatingAndReviewComponent2.ReviewsMetaData Bind() => new RatingAndReviewComponent2.ReviewsMetaData()
      {
        HasMoreItems = this.hasMoreItemsColumn.GetBoolean((IDataReader) this.Reader),
        TotalRecords = this.totalRecordsColumn.GetInt32((IDataReader) this.Reader)
      };
    }

    internal class ReviewsMetaData
    {
      public bool HasMoreItems { get; set; }

      public int TotalRecords { get; set; }
    }
  }
}
