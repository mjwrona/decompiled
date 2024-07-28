// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent12
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
  internal class RatingAndReviewComponent12 : RatingAndReviewComponent11
  {
    public override void IgnoreReview(string productId, long reviewId, bool setIgnoreState)
    {
      this.PrepareStoredProcedure("Gallery.prc_IgnoreReview");
      this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindLong(nameof (reviewId), reviewId);
      this.ExecuteNonQuery();
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
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent12.ReviewsBinder2());
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

    public override List<FlaggedReview> GetFlaggedReviews(string productId = null)
    {
      try
      {
        string str = "Gallery.prc_GetFlaggedReviews";
        this.TraceEnter(12061027, nameof (GetFlaggedReviews));
        this.PrepareStoredProcedure(str);
        if (!string.IsNullOrEmpty(productId))
          this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        List<FlaggedReview> items;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
        {
          resultCollection.AddBinder<FlaggedReview>((ObjectBinder<FlaggedReview>) new RatingAndReviewComponent12.FlaggedReviewsBinder2());
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
        RatingAndReviewComponent12.ReviewsMetaData2 reviewsMetaData2 = new RatingAndReviewComponent12.ReviewsMetaData2()
        {
          HasMoreItems = false,
          TotalRecords = -1,
          TotalIgnoreRecords = -1
        };
        List<ReviewReply> replies = (List<ReviewReply>) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetReviews", this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent12.ReviewsBinder2());
          reviews = resultCollection.GetCurrent<Review>().Items;
          resultCollection.AddBinder<RatingAndReviewComponent12.ReviewsMetaData2>((ObjectBinder<RatingAndReviewComponent12.ReviewsMetaData2>) new RatingAndReviewComponent12.ReviewsMetaDataBinder2());
          resultCollection.NextResult();
          reviewsMetaData2 = resultCollection.GetCurrent<RatingAndReviewComponent12.ReviewsMetaData2>().Items[0];
          resultCollection.AddBinder<ReviewReply>((ObjectBinder<ReviewReply>) new RatingAndReviewComponent12.ReviewReplyBinder3());
          resultCollection.NextResult();
          replies = resultCollection.GetCurrent<ReviewReply>().Items;
        }
        List<Review> reviewList = this.CombineReviewsAndReplies(reviews, replies);
        return new ReviewsResult()
        {
          TotalReviewCount = (long) reviewsMetaData2.TotalRecords,
          TotalIgnoredReviewCount = (long) reviewsMetaData2.TotalIgnoreRecords,
          Reviews = reviewList,
          HasMoreReviews = reviewsMetaData2.HasMoreItems
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
        this.BindDateTime(nameof (modifiedTime), modifiedTime);
        Review review = (Review) null;
        List<ReviewReply> publisherReplies = (List<ReviewReply>) null;
        List<ReviewReply> adminReplies = (List<ReviewReply>) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateReview", this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent12.ReviewsBinder2());
          review = resultCollection.GetCurrent<Review>().Items[0];
          resultCollection.AddBinder<ReviewReply>((ObjectBinder<ReviewReply>) new RatingAndReviewComponent12.ReviewReplyBinder3());
          resultCollection.NextResult();
          this.ArrangeAdminAndPublisherReplies(resultCollection.GetCurrent<ReviewReply>().Items, out publisherReplies, out adminReplies);
        }
        if (publisherReplies != null && publisherReplies.Count > 0)
          review.Reply = publisherReplies[0];
        if (adminReplies != null && adminReplies.Count > 0)
          review.AdminReply = adminReplies[0];
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
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent12.ReviewsBinder2());
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
        this.BindBoolean(nameof (isAdminReply), isAdminReply);
        Review review = (Review) null;
        List<ReviewReply> publisherReplies = (List<ReviewReply>) null;
        List<ReviewReply> adminReplies = (List<ReviewReply>) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent12.ReviewsBinder2());
          review = resultCollection.GetCurrent<Review>().Items[0];
          resultCollection.AddBinder<ReviewReply>((ObjectBinder<ReviewReply>) new RatingAndReviewComponent12.ReviewReplyBinder3());
          resultCollection.NextResult();
          this.ArrangeAdminAndPublisherReplies(resultCollection.GetCurrent<ReviewReply>().Items, out publisherReplies, out adminReplies);
        }
        if (publisherReplies != null && publisherReplies.Count > 0)
          review.Reply = publisherReplies[0];
        if (adminReplies != null && adminReplies.Count > 0)
          review.AdminReply = adminReplies[0];
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

    protected void ArrangeAdminAndPublisherReplies(
      List<ReviewReply> replies,
      out List<ReviewReply> publisherReplies,
      out List<ReviewReply> adminReplies)
    {
      adminReplies = new List<ReviewReply>();
      publisherReplies = new List<ReviewReply>();
      if (replies == null || replies.Count <= 0)
        return;
      foreach (ReviewReply reply in replies)
      {
        if (reply.IsAdminReply)
          adminReplies.Add(reply);
        else
          publisherReplies.Add(reply);
      }
    }

    protected override List<Review> CombineReviewsAndReplies(
      List<Review> reviews,
      List<ReviewReply> replies)
    {
      if (replies == null || replies.Count <= 0)
        return reviews;
      List<ReviewReply> adminReplies = (List<ReviewReply>) null;
      List<ReviewReply> publisherReplies = (List<ReviewReply>) null;
      this.ArrangeAdminAndPublisherReplies(replies, out publisherReplies, out adminReplies);
      int count1 = reviews.Count;
      Dictionary<long, ReviewReply> dictionary1 = new Dictionary<long, ReviewReply>();
      if (publisherReplies != null && publisherReplies.Count > 0)
      {
        int count2 = publisherReplies.Count;
        for (int index = 0; index < count2; ++index)
          dictionary1.Add(publisherReplies[index].ReviewId, publisherReplies[index]);
      }
      Dictionary<long, ReviewReply> dictionary2 = new Dictionary<long, ReviewReply>();
      if (adminReplies != null && adminReplies.Count > 0)
      {
        int count3 = adminReplies.Count;
        for (int index = 0; index < count3; ++index)
          dictionary2.Add(adminReplies[index].ReviewId, adminReplies[index]);
      }
      for (int index = 0; index < count1; ++index)
      {
        if (dictionary1.ContainsKey(reviews[index].Id))
          reviews[index].Reply = dictionary1[reviews[index].Id];
        if (dictionary2.ContainsKey(reviews[index].Id))
          reviews[index].AdminReply = dictionary2[reviews[index].Id];
      }
      return reviews;
    }

    internal class ReviewsMetaDataBinder2 : ObjectBinder<RatingAndReviewComponent12.ReviewsMetaData2>
    {
      protected SqlColumnBinder hasMoreItemsColumn = new SqlColumnBinder("HasMoreItems");
      protected SqlColumnBinder totalRecordsColumn = new SqlColumnBinder("TotalRecords");
      protected SqlColumnBinder TotalIgnoreRecordsColumn = new SqlColumnBinder("TotalIgnoreRecords");

      protected override RatingAndReviewComponent12.ReviewsMetaData2 Bind() => new RatingAndReviewComponent12.ReviewsMetaData2()
      {
        HasMoreItems = this.hasMoreItemsColumn.GetBoolean((IDataReader) this.Reader),
        TotalRecords = this.totalRecordsColumn.GetInt32((IDataReader) this.Reader),
        TotalIgnoreRecords = this.TotalIgnoreRecordsColumn.GetInt32((IDataReader) this.Reader)
      };
    }

    internal class ReviewsMetaData2
    {
      public bool HasMoreItems { get; set; }

      public int TotalRecords { get; set; }

      public int TotalIgnoreRecords { get; set; }
    }

    internal class ReviewReplyBinder3 : ObjectBinder<ReviewReply>
    {
      private SqlColumnBinder m_idColumn = new SqlColumnBinder("Id");
      private SqlColumnBinder m_reviewIdColumn = new SqlColumnBinder("ReviewId");
      private SqlColumnBinder m_userIdColumn = new SqlColumnBinder("UserId");
      private SqlColumnBinder m_titleColumn = new SqlColumnBinder("Title");
      private SqlColumnBinder m_replyTextColumn = new SqlColumnBinder("ReplyText");
      private SqlColumnBinder m_updatedDateColumn = new SqlColumnBinder("UpdatedDate");
      private SqlColumnBinder m_isDeletedColumn = new SqlColumnBinder("IsDeleted");
      private SqlColumnBinder m_IsAdminReplyColumn = new SqlColumnBinder("IsAdminReply");

      protected override ReviewReply Bind() => new ReviewReply()
      {
        Id = this.m_idColumn.GetInt64((IDataReader) this.Reader),
        ReviewId = this.m_reviewIdColumn.GetInt64((IDataReader) this.Reader),
        UserId = this.m_userIdColumn.GetGuid((IDataReader) this.Reader),
        Title = this.m_titleColumn.GetString((IDataReader) this.Reader, true),
        ReplyText = this.m_replyTextColumn.GetString((IDataReader) this.Reader, false),
        UpdatedDate = this.m_updatedDateColumn.GetDateTime((IDataReader) this.Reader),
        IsDeleted = this.m_isDeletedColumn.GetBoolean((IDataReader) this.Reader),
        IsAdminReply = this.m_IsAdminReplyColumn.GetBoolean((IDataReader) this.Reader)
      };
    }

    internal class ReviewsBinder2 : ObjectBinder<Review>
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
      protected SqlColumnBinder isIgnoredColumn = new SqlColumnBinder("IsIgnored");

      protected override Review Bind() => new Review()
      {
        Id = this.idColumn.GetInt64((IDataReader) this.Reader),
        UserId = this.userIdColumn.GetGuid((IDataReader) this.Reader),
        ProductId = this.productIdColumn.GetString((IDataReader) this.Reader, false),
        CreatedDate = this.createdDateColumn.GetDateTime((IDataReader) this.Reader),
        UpdatedDate = this.updatedDateColumn.GetDateTime((IDataReader) this.Reader),
        Rating = this.ratingColumn.GetByte((IDataReader) this.Reader),
        Title = this.titleColumn.GetString((IDataReader) this.Reader, true),
        Text = this.reviewTextColumn.GetString((IDataReader) this.Reader, true),
        ProductVersion = this.productVersionColumn.GetString((IDataReader) this.Reader, false),
        IsDeleted = this.isDeletedColumn.GetBoolean((IDataReader) this.Reader),
        IsIgnored = this.isIgnoredColumn.GetBoolean((IDataReader) this.Reader)
      };
    }

    internal class FlaggedReviewsBinder2 : ObjectBinder<FlaggedReview>
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
      protected SqlColumnBinder isIgnoredColumn = new SqlColumnBinder("IsIgnored");
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
        flaggedReview.IsIgnored = this.isIgnoredColumn.GetBoolean((IDataReader) this.Reader);
        flaggedReview.ReportedBy = this.reportedByColumn.GetGuid((IDataReader) this.Reader);
        flaggedReview.ConcernCategory = this.concernCategoryColumn.GetInt32((IDataReader) this.Reader);
        flaggedReview.ConcernText = this.concernTextColumn.GetString((IDataReader) this.Reader, false);
        flaggedReview.ConcernSubmittedDate = this.submittedDateColumn.GetDateTime((IDataReader) this.Reader);
        return flaggedReview;
      }
    }
  }
}
