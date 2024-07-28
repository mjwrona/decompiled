// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.RatingAndReviewComponent17
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501: Avoid excessive inheritance", Justification = "Need inhertance here")]
  internal class RatingAndReviewComponent17 : RatingAndReviewComponent16
  {
    public virtual int AnonymizeReviews(Guid userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_AnonymizeReviews");
      this.BindGuid(nameof (userId), userId);
      SqlParameter sqlParameter = this.BindInt("@rowCount", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }

    public virtual IEnumerable<ReviewReply> GetPublisherReplyByUserId(Guid userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetPublisherReplyByUserId");
      this.BindGuid(nameof (userId), userId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetPublisherReplyByUserId", this.RequestContext))
      {
        resultCollection.AddBinder<ReviewReply>((ObjectBinder<ReviewReply>) new RatingAndReviewComponent4.ReviewReplyBinder());
        return (IEnumerable<ReviewReply>) resultCollection.GetCurrent<ReviewReply>().Items;
      }
    }

    public virtual int AnonymizePublisherReply(Guid userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_AnonymizePublisherReply");
      this.BindGuid(nameof (userId), userId);
      SqlParameter sqlParameter = this.BindInt("@rowCount", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }
  }
}
