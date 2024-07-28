// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewNotActiveException
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [Serializable]
  public class CodeReviewNotActiveException : TeamFoundationServiceException
  {
    public CodeReviewNotActiveException(int codeReviewId)
      : base(CodeReviewResources.CodeReviewCannotBeUpdatedSinceItsNotActive((object) codeReviewId))
    {
    }

    public CodeReviewNotActiveException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractInt(sqlError, "reviewId"))
    {
    }
  }
}
