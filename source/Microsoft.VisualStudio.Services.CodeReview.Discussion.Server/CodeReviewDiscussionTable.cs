// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.CodeReviewDiscussionTable
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
  internal static class CodeReviewDiscussionTable
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[7]
    {
      new SqlMetaData("Index", SqlDbType.Int),
      new SqlMetaData("DiscussionId", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Severity", SqlDbType.TinyInt),
      new SqlMetaData("WorkItemId", SqlDbType.Int),
      new SqlMetaData("VersionId", SqlDbType.NVarChar, 2083L),
      new SqlMetaData("PropertyId", SqlDbType.Int)
    };

    internal static SqlParameter Bind(
      this DiscussionThread[] discussions,
      DiscussionSqlResourceComponent component,
      string parameterName)
    {
      discussions = discussions ?? Array.Empty<DiscussionThread>();
      return component.BindTable(parameterName, "CodeReview.typ_DiscussionTableV5", CodeReviewDiscussionTable.BindDiscussionRows(discussions));
    }

    private static IEnumerable<SqlDataRecord> BindDiscussionRows(DiscussionThread[] discussions)
    {
      int index1 = 0;
      DiscussionThread[] discussionThreadArray = discussions;
      for (int index2 = 0; index2 < discussionThreadArray.Length; ++index2)
      {
        DiscussionThread discussionThread = discussionThreadArray[index2];
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(CodeReviewDiscussionTable.s_metadata);
        int num1 = 0;
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        int num3 = index1++;
        sqlDataRecord2.SetInt32(ordinal1, num3);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        int ordinal2 = num2;
        int num4 = ordinal2 + 1;
        int discussionId = discussionThread.DiscussionId;
        sqlDataRecord3.SetInt32(ordinal2, discussionId);
        int num5;
        if (discussionThread.DiscussionId < 0)
        {
          SqlDataRecord sqlDataRecord4 = sqlDataRecord1;
          int ordinal3 = num4;
          int num6 = ordinal3 + 1;
          int status = (int) (byte) discussionThread.Status;
          sqlDataRecord4.SetByte(ordinal3, (byte) status);
          SqlDataRecord sqlDataRecord5 = sqlDataRecord1;
          int ordinal4 = num6;
          int num7 = ordinal4 + 1;
          int severity = (int) (byte) discussionThread.Severity;
          sqlDataRecord5.SetByte(ordinal4, (byte) severity);
          SqlDataRecord sqlDataRecord6 = sqlDataRecord1;
          int ordinal5 = num7;
          int num8 = ordinal5 + 1;
          int workItemId = discussionThread.WorkItemId;
          sqlDataRecord6.SetInt32(ordinal5, workItemId);
          SqlDataRecord sqlDataRecord7 = sqlDataRecord1;
          int ordinal6 = num8;
          int num9 = ordinal6 + 1;
          string versionId = discussionThread.VersionId;
          sqlDataRecord7.SetString(ordinal6, versionId);
          SqlDataRecord sqlDataRecord8 = sqlDataRecord1;
          int ordinal7 = num9;
          num5 = ordinal7 + 1;
          int propertyId = discussionThread.PropertyId;
          sqlDataRecord8.SetInt32(ordinal7, propertyId);
        }
        else
        {
          int num10;
          if (discussionThread.IsDirty)
          {
            SqlDataRecord sqlDataRecord9 = sqlDataRecord1;
            int ordinal8 = num4;
            num10 = ordinal8 + 1;
            int status = (int) (byte) discussionThread.Status;
            sqlDataRecord9.SetByte(ordinal8, (byte) status);
          }
          else
          {
            SqlDataRecord sqlDataRecord10 = sqlDataRecord1;
            int ordinal9 = num4;
            num10 = ordinal9 + 1;
            sqlDataRecord10.SetDBNull(ordinal9);
          }
          SqlDataRecord sqlDataRecord11 = sqlDataRecord1;
          int ordinal10 = num10;
          int num11 = ordinal10 + 1;
          sqlDataRecord11.SetDBNull(ordinal10);
          SqlDataRecord sqlDataRecord12 = sqlDataRecord1;
          int ordinal11 = num11;
          int num12 = ordinal11 + 1;
          sqlDataRecord12.SetDBNull(ordinal11);
          SqlDataRecord sqlDataRecord13 = sqlDataRecord1;
          int ordinal12 = num12;
          int num13 = ordinal12 + 1;
          sqlDataRecord13.SetDBNull(ordinal12);
          SqlDataRecord sqlDataRecord14 = sqlDataRecord1;
          int ordinal13 = num13;
          num5 = ordinal13 + 1;
          sqlDataRecord14.SetDBNull(ordinal13);
        }
        yield return sqlDataRecord1;
      }
      discussionThreadArray = (DiscussionThread[]) null;
    }
  }
}
