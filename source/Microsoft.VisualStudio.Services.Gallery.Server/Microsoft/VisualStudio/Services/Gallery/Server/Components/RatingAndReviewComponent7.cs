// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent7
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class RatingAndReviewComponent7 : RatingAndReviewComponent6
  {
    public override int DeleteReviews(string productId, long? reviewId, bool isHardDelete = false)
    {
      try
      {
        this.TraceEnter(12061025, nameof (DeleteReviews));
        this.PrepareStoredProcedure("Gallery.prc_DeleteReviews");
        this.BindString(nameof (productId), productId, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        if (reviewId.HasValue)
          this.BindLong(nameof (reviewId), reviewId.Value);
        this.BindBoolean(nameof (isHardDelete), isHardDelete);
        return (int) this.ExecuteNonQuery(true);
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
  }
}
