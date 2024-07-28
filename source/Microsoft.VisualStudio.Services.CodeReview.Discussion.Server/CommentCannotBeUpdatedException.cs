// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.CommentCannotBeUpdatedException
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  [Serializable]
  public class CommentCannotBeUpdatedException : TeamFoundationServiceException
  {
    public CommentCannotBeUpdatedException(int commentId)
      : base(string.Format(Resources.CommentCanNotBeUpdated, (object) commentId))
    {
    }

    public CommentCannotBeUpdatedException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractInt(sqlError, "commentId"))
    {
    }
  }
}
