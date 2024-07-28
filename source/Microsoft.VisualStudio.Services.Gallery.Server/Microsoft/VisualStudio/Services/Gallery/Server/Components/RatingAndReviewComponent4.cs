// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent4
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class RatingAndReviewComponent4 : RatingAndReviewComponent3
  {
    public override Review UpdatePublisherReply(
      long reviewId,
      Guid userId,
      string title,
      string replyText,
      string productVersion)
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
        this.BindString(nameof (productVersion), productVersion, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        Review review = (Review) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
        {
          resultCollection.AddBinder<Review>((ObjectBinder<Review>) new RatingAndReviewComponent.ReviewsBinder());
          review = resultCollection.GetCurrent<Review>().Items[0];
          resultCollection.AddBinder<ReviewReply>((ObjectBinder<ReviewReply>) new RatingAndReviewComponent4.ReviewReplyBinder());
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

    internal class ReviewReplyBinder : ObjectBinder<ReviewReply>
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
