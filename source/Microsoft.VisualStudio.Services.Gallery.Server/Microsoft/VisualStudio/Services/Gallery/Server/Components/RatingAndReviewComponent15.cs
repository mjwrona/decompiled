// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent15
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501: Avoid excessive inheritance", Justification = "Need inhertance here")]
  internal class RatingAndReviewComponent15 : RatingAndReviewComponent14
  {
    public override int DeleteReviewReply(
      string productId,
      long? reviewId,
      bool deleteAll,
      bool isAdminReply)
    {
      try
      {
        this.TraceEnter(12061049, nameof (DeleteReviewReply));
        this.PrepareStoredProcedure("Gallery.prc_DeletePublisherReply");
        this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        if (reviewId.HasValue)
          this.BindLong(nameof (reviewId), reviewId.Value);
        this.BindBoolean(nameof (deleteAll), deleteAll);
        this.BindBoolean(nameof (isAdminReply), isAdminReply);
        return (int) this.ExecuteNonQuery(true);
      }
      catch (Exception ex)
      {
        this.TraceException(12061016, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061050, nameof (DeleteReviewReply));
      }
    }

    public override void IgnoreReview(string productId, long reviewId, bool setIgnoreState)
    {
      this.PrepareStoredProcedure("Gallery.prc_IgnoreReview");
      this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindLong(nameof (reviewId), reviewId);
      this.BindBoolean(nameof (setIgnoreState), setIgnoreState);
      this.ExecuteNonQuery();
    }
  }
}
