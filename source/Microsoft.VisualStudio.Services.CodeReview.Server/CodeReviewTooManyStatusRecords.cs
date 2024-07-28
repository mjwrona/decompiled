// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewTooManyStatusRecords
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [Serializable]
  public class CodeReviewTooManyStatusRecords : TeamFoundationServiceException
  {
    public string StatusName { get; }

    public int ReviewId { get; }

    public int MaxStatusCount { get; }

    public CodeReviewTooManyStatusRecords(string statusName, int reviewId, int maxStatusCount)
      : base(CodeReviewResources.TooManyStatusRecords((object) statusName, (object) reviewId, (object) maxStatusCount))
    {
      this.StatusName = statusName;
      this.ReviewId = reviewId;
      this.MaxStatusCount = maxStatusCount;
    }

    public CodeReviewTooManyStatusRecords(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "statusName"), TeamFoundationServiceException.ExtractInt(sqlError, "reviewId"), TeamFoundationServiceException.ExtractInt(sqlError, "maxStatusCount"))
    {
    }
  }
}
