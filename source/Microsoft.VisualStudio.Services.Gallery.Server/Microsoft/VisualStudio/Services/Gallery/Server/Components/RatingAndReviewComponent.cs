// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class RatingAndReviewComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private const string s_area = "ReviewsComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[17]
    {
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent>(1),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent2>(2),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent3>(3),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent4>(4),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent5>(5),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent6>(6),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent7>(7),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent8>(8),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent9>(9),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent10>(10),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent11>(11),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent12>(12),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent13>(13),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent14>(14),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent15>(15),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent16>(16),
      (IComponentCreator) new ComponentCreator<RatingAndReviewComponent17>(17)
    }, "Reviews");

    static RatingAndReviewComponent()
    {
      RatingAndReviewComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      RatingAndReviewComponent.s_sqlExceptionFactories.Add(270007, new SqlExceptionFactory(typeof (ReviewExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorNumber, ex, sqlError) => (Exception) new ReviewExistsException(GalleryWebApiResources.ReviewAlreadyExistsException()))));
      RatingAndReviewComponent.s_sqlExceptionFactories.Add(270008, new SqlExceptionFactory(typeof (ReviewAlreadyReportedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorNumber, ex, sqlError) => (Exception) new ReviewAlreadyReportedException(GalleryWebApiResources.ReviewAlreadyReportedException()))));
      RatingAndReviewComponent.s_sqlExceptionFactories.Add(270009, new SqlExceptionFactory(typeof (ReviewDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorNumber, ex, sqlError) => (Exception) new ReviewDoesNotExistException(string.Format(GalleryWebApiResources.ReviewDoesNotExistException(ExceptionHelper.ExtractStrings(sqlError, (object) "ReviewId")[0]))))));
      RatingAndReviewComponent.s_sqlExceptionFactories.Add(270014, new SqlExceptionFactory(typeof (ReviewDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorNumber, ex, sqlError) => (Exception) new ReviewDoesNotExistException(string.Format(GalleryWebApiResources.ReviewDoesNotExistException(ExceptionHelper.ExtractStrings(sqlError, (object) "ReviewId")[0]))))));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) RatingAndReviewComponent.s_sqlExceptionFactories;

    protected override string TraceArea => "ReviewsComponent";

    public virtual void IgnoreReview(string productId, long reviewId, bool setIgnoreState) => throw new InvalidOperationException(GalleryResources.OperationNotSupported((object) nameof (IgnoreReview)));

    public virtual ReviewsResult GetReviews(
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
        List<Review> reviewList = (List<Review>) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetReviews", this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent.ReviewsBinder());
          reviewList = resultCollection.GetCurrent<Review>().Items;
        }
        long count1 = reviewList == null ? 0L : (long) reviewList.Count;
        return new ReviewsResult()
        {
          TotalReviewCount = count1,
          Reviews = reviewList,
          HasMoreReviews = false
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

    public virtual ReviewSummary GetReviewSummary(
      string productId,
      DateTime? beforeDate,
      DateTime? afterDate,
      string productVersion = null)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetReviewSummary");
      this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      SqlParameter sqlParameter1 = this.BindDouble("@averageRating", -1.0);
      sqlParameter1.Direction = ParameterDirection.Output;
      SqlParameter sqlParameter2 = this.BindLong("@ratingCount", -1L);
      sqlParameter2.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return new ReviewSummary()
      {
        AverageRating = Convert.ToSingle((double) sqlParameter1.Value),
        RatingCount = (long) sqlParameter2.Value
      };
    }

    public virtual Review CreateReview(
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
        this.PrepareStoredProcedure("Gallery.prc_CreateReview");
        this.BindByte(nameof (rating), rating);
        this.BindString(nameof (title), title, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("reviewText", text, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString(nameof (productVersion), productVersion, 25, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindGuid(nameof (userId), userId);
        Review review = (Review) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_CreateReview", this.RequestContext))
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

    public virtual Review UpdateReview(
      string productId,
      long reviewId,
      byte rating,
      string title,
      string text,
      string productVersion,
      Guid userId,
      DateTime modifiedTime)
    {
      throw new InvalidOperationException(GalleryResources.OperationNotSupported((object) nameof (UpdateReview)));
    }

    public virtual int DeleteReviewReply(
      string productId,
      long? reviewId,
      bool deleteAll,
      bool isAdminReply)
    {
      throw new InvalidOperationException(GalleryResources.OperationNotSupported((object) nameof (DeleteReviewReply)));
    }

    public virtual List<Review> GetReviewsByUserId(Guid userId, string productId) => throw new InvalidOperationException(GalleryResources.OperationNotSupported((object) nameof (GetReviewsByUserId)));

    public virtual int DeleteReviews(string productId, long? reviewId, bool isHardDelete = false) => throw new InvalidOperationException(GalleryResources.OperationNotSupported((object) nameof (DeleteReviews)));

    public virtual List<FlaggedReview> GetFlaggedReviews(string productId = null) => throw new InvalidOperationException(GalleryResources.OperationNotSupported((object) nameof (GetFlaggedReviews)));

    public virtual Review UpdatePublisherReply(
      long reviewId,
      Guid userId,
      string title,
      string replyText,
      string productVersion)
    {
      throw new InvalidOperationException(GalleryResources.OperationNotSupported((object) "CreateReplyForReview"));
    }

    public virtual Review UpdatePublisherReply(
      long reviewId,
      Guid userId,
      string title,
      string replyText,
      DateTime creationTime,
      bool isAdminReply = false)
    {
      throw new InvalidOperationException(GalleryResources.OperationNotSupported((object) "CreateReplyForReview"));
    }

    public virtual long FlagReview(
      string productId,
      long reviewId,
      Guid userId,
      ConcernCategory concernCategory,
      string concernText)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreateUserReportedConcern");
      this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindLong(nameof (reviewId), reviewId);
      this.BindGuid(nameof (userId), userId);
      this.BindInt(nameof (concernCategory), (int) concernCategory);
      this.BindString(nameof (concernText), concernText, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.ExecuteScalar();
      return reviewId;
    }

    internal class ReviewsBinder : ObjectBinder<Review>
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
        IsDeleted = this.isDeletedColumn.GetBoolean((IDataReader) this.Reader)
      };
    }
  }
}
