// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent13
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501: Avoid excessive inheritance", Justification = "Need inhertance here")]
  internal class RatingAndReviewComponent13 : RatingAndReviewComponent12
  {
    public override ReviewSummary GetReviewSummary(
      string productId,
      DateTime? beforeDate,
      DateTime? afterDate,
      string productVersion = null)
    {
      ReviewSummary reviewSummary = (ReviewSummary) null;
      this.PrepareStoredProcedure("Gallery.prc_GetReviewSummary");
      this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      if (beforeDate.HasValue)
        this.BindDateTime(nameof (beforeDate), beforeDate.Value);
      if (afterDate.HasValue)
        this.BindDateTime(nameof (afterDate), afterDate.Value);
      if (!string.IsNullOrWhiteSpace(productVersion))
        this.BindString(nameof (productVersion), productVersion, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetReviewSummary", this.RequestContext))
      {
        resultCollection.AddBinder<ReviewSummary>((ObjectBinder<ReviewSummary>) new RatingAndReviewComponent13.ReviewSummaryBinder());
        reviewSummary = resultCollection.GetCurrent<ReviewSummary>().Items[0];
        resultCollection.AddBinder<RatingCountPerRating>((ObjectBinder<RatingCountPerRating>) new RatingAndReviewComponent13.RatingCountPerRatingBinder());
        if (resultCollection.TryNextResult())
          reviewSummary.RatingSplit = resultCollection.GetCurrent<RatingCountPerRating>().Items;
      }
      return reviewSummary;
    }

    internal class RatingCountPerRatingBinder : ObjectBinder<RatingCountPerRating>
    {
      protected SqlColumnBinder ratingCountColumn = new SqlColumnBinder("RatingCount");
      protected SqlColumnBinder ratingColumn = new SqlColumnBinder("Rating");

      protected override RatingCountPerRating Bind() => new RatingCountPerRating()
      {
        Rating = this.ratingColumn.GetByte((IDataReader) this.Reader),
        RatingCount = this.ratingCountColumn.GetInt64((IDataReader) this.Reader)
      };
    }

    internal class ReviewSummaryBinder : ObjectBinder<ReviewSummary>
    {
      protected SqlColumnBinder averageRatingColumn = new SqlColumnBinder("AverageRating");
      protected SqlColumnBinder ratingCountColumn = new SqlColumnBinder("RatingCount");

      protected override ReviewSummary Bind() => new ReviewSummary()
      {
        AverageRating = Convert.ToSingle(this.averageRatingColumn.GetDouble((IDataReader) this.Reader)),
        RatingCount = this.ratingCountColumn.GetInt64((IDataReader) this.Reader)
      };
    }
  }
}
