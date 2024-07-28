// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.CodeReviewCommentIdTable
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  internal static class CodeReviewCommentIdTable
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
    {
      new SqlMetaData("DiscussionId", SqlDbType.Int),
      new SqlMetaData("CommentId", SqlDbType.SmallInt)
    };

    internal static SqlParameter Bind(
      this IEnumerable<CommentId> entries,
      DiscussionSqlResourceComponent component,
      string parameterName)
    {
      entries = entries ?? Enumerable.Empty<CommentId>();
      return component.BindTable(parameterName, "CodeReview.typ_CommentIdTable", CodeReviewCommentIdTable.BindCommentIdRows(entries));
    }

    private static IEnumerable<SqlDataRecord> BindCommentIdRows(IEnumerable<CommentId> entries)
    {
      foreach (CommentId entry in entries)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CodeReviewCommentIdTable.s_metadata);
        sqlDataRecord.SetInt32(0, entry.DiscussionId);
        sqlDataRecord.SetInt16(1, entry.Id);
        yield return sqlDataRecord;
      }
    }
  }
}
