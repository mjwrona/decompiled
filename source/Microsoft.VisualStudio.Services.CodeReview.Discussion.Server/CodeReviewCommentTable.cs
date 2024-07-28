// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.CodeReviewCommentTable
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  internal static class CodeReviewCommentTable
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[6]
    {
      new SqlMetaData("Index", SqlDbType.Int),
      new SqlMetaData("DiscussionId", SqlDbType.Int),
      new SqlMetaData("ParentCommentId", SqlDbType.SmallInt),
      new SqlMetaData("Author", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CommentType", SqlDbType.TinyInt),
      new SqlMetaData("Content", SqlDbType.NVarChar, -1L)
    };

    internal static SqlParameter Bind(
      this DiscussionComment[] entries,
      DiscussionSqlResourceComponent component,
      string parameterName)
    {
      entries = entries ?? Array.Empty<DiscussionComment>();
      return component.BindTable(parameterName, "CodeReview.typ_CommentTable", CodeReviewCommentTable.BindDiscussionCommentRows(entries));
    }

    private static IEnumerable<SqlDataRecord> BindDiscussionCommentRows(DiscussionComment[] entries)
    {
      int index1 = 0;
      DiscussionComment[] discussionCommentArray = entries;
      for (int index2 = 0; index2 < discussionCommentArray.Length; ++index2)
      {
        DiscussionComment discussionComment = discussionCommentArray[index2];
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CodeReviewCommentTable.s_metadata);
        sqlDataRecord.SetInt32(0, index1++);
        sqlDataRecord.SetInt32(1, discussionComment.DiscussionId);
        sqlDataRecord.SetInt16(2, discussionComment.ParentCommentId);
        sqlDataRecord.SetGuid(3, discussionComment.GetAuthorId());
        sqlDataRecord.SetByte(4, (byte) discussionComment.CommentType);
        sqlDataRecord.SetString(5, discussionComment.Content);
        yield return sqlDataRecord;
      }
      discussionCommentArray = (DiscussionComment[]) null;
    }
  }
}
