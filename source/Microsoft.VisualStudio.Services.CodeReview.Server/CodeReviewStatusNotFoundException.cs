// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewStatusNotFoundException
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [Serializable]
  public class CodeReviewStatusNotFoundException : TeamFoundationServiceException
  {
    public int StatusId { get; private set; }

    public CodeReviewStatusNotFoundException(int statusId, int reviewId, int? iterationId)
      : base(CodeReviewResources.StatusNotFoundException((object) statusId, (object) reviewId, (object) iterationId))
    {
      this.StatusId = statusId;
    }

    public CodeReviewStatusNotFoundException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractInt(sqlError, "statusId"), TeamFoundationServiceException.ExtractInt(sqlError, "reviewId"), new int?(TeamFoundationServiceException.ExtractInt(sqlError, "iterationId")))
    {
      this.StatusId = TeamFoundationServiceException.ExtractInt(sqlError, "statusId");
    }
  }
}
