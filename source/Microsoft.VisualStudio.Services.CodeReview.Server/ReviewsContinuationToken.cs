// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ReviewsContinuationToken
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class ReviewsContinuationToken
  {
    private ReviewsContinuationToken(DateTime updatedDate) => this.UpdatedDate = new DateTime?(updatedDate);

    public ReviewsContinuationToken(Review nextReview)
    {
      if (nextReview == null)
        return;
      this.UpdatedDate = nextReview.UpdatedDate;
    }

    public DateTime? UpdatedDate { get; }

    public override string ToString() => this.UpdatedDate.HasValue ? this.UpdatedDate.Value.ToString("o") : string.Empty;

    public static bool TryParse(string value, out ReviewsContinuationToken token)
    {
      token = (ReviewsContinuationToken) null;
      DateTime result;
      if (!DateTime.TryParse(value, out result))
        return false;
      token = new ReviewsContinuationToken(result);
      return true;
    }
  }
}
