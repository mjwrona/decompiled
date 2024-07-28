// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent6
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class RatingAndReviewComponent6 : RatingAndReviewComponent5
  {
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
  }
}
