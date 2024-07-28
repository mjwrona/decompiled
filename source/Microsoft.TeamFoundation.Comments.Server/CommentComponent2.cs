// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentComponent2
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class CommentComponent2 : CommentComponent
  {
    private static readonly SqlMetaData[] TypCommentIdTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ArtifactKind", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CommentId", SqlDbType.Int),
      new SqlMetaData("Version", SqlDbType.Int)
    };

    public override List<CommentVersion> GetCommentsVersions(
      Guid artifactKind,
      IEnumerable<GetCommentVersion> commentVersions)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) commentVersions, nameof (commentVersions));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) commentVersions, nameof (commentVersions));
      this.TraceEnter(140244, nameof (GetCommentsVersions));
      try
      {
        this.PrepareStoredProcedure("prc_GetCommentsVersions");
        this.BindTypCommentIdTable("@comments", artifactKind, commentVersions);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CommentVersion>(this.GetCommentVersionBinder());
          return resultCollection.GetCurrent<CommentVersion>().Items ?? new List<CommentVersion>();
        }
      }
      finally
      {
        this.TraceLeave(140245, nameof (GetCommentsVersions));
      }
    }

    protected virtual void BindTypCommentIdTable(
      string paramName,
      Guid artifactKind,
      IEnumerable<GetCommentVersion> commentVersions)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (GetCommentVersion commentVersion in commentVersions)
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(CommentComponent2.TypCommentIdTable2);
        int num1 = 0;
        SqlDataRecord record = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string artifactId = commentVersion.ArtifactId;
        record.SetString(ordinal1, artifactId, BindStringBehavior.Unchanged);
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        Guid guid = artifactKind;
        sqlDataRecord2.SetGuid(ordinal2, guid);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        int ordinal3 = num3;
        int ordinal4 = ordinal3 + 1;
        int commentId = commentVersion.CommentId;
        sqlDataRecord3.SetInt32(ordinal3, commentId);
        sqlDataRecord1.SetInt32(ordinal4, commentVersion.Version);
        rows.Add(sqlDataRecord1);
      }
      this.BindTable(paramName, "typ_CommentIdTable2", (IEnumerable<SqlDataRecord>) rows);
    }

    protected override void BindTypCommentIdTable(
      string paramName,
      Guid artifactKind,
      string artifactId,
      IEnumerable<int> commentIds)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (int commentId in commentIds)
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(CommentComponent2.TypCommentIdTable2);
        int num1 = 0;
        SqlDataRecord record = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string str = artifactId;
        record.SetString(ordinal1, str, BindStringBehavior.Unchanged);
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal2 = num2;
        int ordinal3 = ordinal2 + 1;
        Guid guid = artifactKind;
        sqlDataRecord2.SetGuid(ordinal2, guid);
        sqlDataRecord1.SetInt32(ordinal3, commentId);
        rows.Add(sqlDataRecord1);
      }
      this.BindTable(paramName, "typ_CommentIdTable2", (IEnumerable<SqlDataRecord>) rows);
    }
  }
}
