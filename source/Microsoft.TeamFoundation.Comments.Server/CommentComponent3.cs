// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentComponent3
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class CommentComponent3 : CommentComponent2
  {
    public override CommentsList GetComments(
      Guid artifactKind,
      string artifactId,
      int top,
      DateTime? startFrom,
      bool fetchText = true,
      bool fetchRenderedText = false,
      bool fetchDeleted = false,
      SortOrder order = SortOrder.Desc)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      this.TraceEnter(140251, nameof (GetComments));
      try
      {
        this.PrepareStoredProcedure("prc_GetComments");
        this.BindString("@artifactId", artifactId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@artifactKind", artifactKind);
        this.BindInt("@top", top);
        this.BindNullableDateTime("@startFrom", startFrom);
        this.BindBoolean("@fetchText", fetchText);
        this.BindBoolean("@fetchRenderedText", fetchRenderedText);
        this.BindBoolean("@fetchDeleted", fetchDeleted);
        this.BindBoolean("@ascending", order == SortOrder.Asc);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Comment>(this.GetCommentBinder());
          List<Comment> commentList = resultCollection.GetCurrent<Comment>().Items ?? new List<Comment>();
          resultCollection.AddBinder<int?>((ObjectBinder<int?>) new CommentComponent.CommentCountBinder());
          resultCollection.NextResult();
          int valueOrDefault = resultCollection.GetCurrent<int?>().Items.SingleOrDefault<int?>().GetValueOrDefault();
          return new CommentsList()
          {
            Comments = (IReadOnlyCollection<Comment>) commentList,
            TotalCount = valueOrDefault
          };
        }
      }
      finally
      {
        this.TraceLeave(140259, nameof (GetComments));
      }
    }
  }
}
