// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentComponent4
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
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class CommentComponent4 : CommentComponent3
  {
    private const int MaxAllowedDestroyBatchSize = 4000;
    protected static readonly SqlMetaData[] TypCommentAddTable2 = new SqlMetaData[12]
    {
      new SqlMetaData("TempId", SqlDbType.Int),
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ArtifactKind", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Text", SqlDbType.NVarChar, -1L),
      new SqlMetaData("RenderedText", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Format", SqlDbType.TinyInt),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CreatedDate", SqlDbType.DateTime),
      new SqlMetaData("CreatedOnBehalfOf", SqlDbType.NVarChar, 256L),
      new SqlMetaData("CreatedOnBehalfDate", SqlDbType.DateTime),
      new SqlMetaData("ParentId", SqlDbType.Int),
      new SqlMetaData("Properties", SqlDbType.NVarChar, -1L)
    };
    protected static readonly SqlMetaData[] TypCommentUpdateTable2 = new SqlMetaData[12]
    {
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ArtifactKind", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CommentId", SqlDbType.Int),
      new SqlMetaData("Text", SqlDbType.NVarChar, -1L),
      new SqlMetaData("RenderedText", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Format", SqlDbType.TinyInt),
      new SqlMetaData("ModifiedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ModifiedDate", SqlDbType.DateTime),
      new SqlMetaData("IsDeleted", SqlDbType.Bit),
      new SqlMetaData("ParentId", SqlDbType.Int),
      new SqlMetaData("State", SqlDbType.TinyInt),
      new SqlMetaData("Properties", SqlDbType.NVarChar, -1L)
    };

    public override CommentsList GetCommentsWithChildren(
      Guid artifactKind,
      string artifactId,
      int top,
      DateTime? startFrom,
      bool fetchText = true,
      bool fetchRenderedText = false,
      bool fetchDeleted = false,
      SortOrder order = SortOrder.Desc,
      int expandChildCount = 5)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNonPositiveInt(expandChildCount, nameof (expandChildCount));
      this.TraceEnter(140261, nameof (GetCommentsWithChildren));
      try
      {
        this.PrepareStoredProcedure("prc_GetCommentsWithChildren");
        this.BindString("@artifactId", artifactId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@artifactKind", artifactKind);
        this.BindInt("@top", top);
        this.BindNullableDateTime("@startFrom", startFrom);
        this.BindBoolean("@fetchText", fetchText);
        this.BindBoolean("@fetchRenderedText", fetchRenderedText);
        this.BindBoolean("@fetchDeleted", fetchDeleted);
        this.BindBoolean("@ascending", order == SortOrder.Asc);
        this.BindInt("@childExpandCount", expandChildCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Comment>(this.GetCommentBinder());
          List<Comment> resultComments = resultCollection.GetCurrent<Comment>().Items ?? new List<Comment>();
          resultCollection.AddBinder<int?>((ObjectBinder<int?>) new CommentComponent.CommentCountBinder());
          resultCollection.NextResult();
          int valueOrDefault = resultCollection.GetCurrent<int?>().Items.SingleOrDefault<int?>().GetValueOrDefault();
          this.ReadChildComments(resultCollection, resultComments);
          return new CommentsList()
          {
            Comments = (IReadOnlyCollection<Comment>) resultComments,
            TotalCount = valueOrDefault
          };
        }
      }
      finally
      {
        this.TraceLeave(140269, nameof (GetCommentsWithChildren));
      }
    }

    public override CommentsList GetCommentsFromParent(
      Guid artifactKind,
      string artifactId,
      int top,
      DateTime? startFrom,
      bool fetchText = true,
      bool fetchRenderedText = false,
      bool fetchDeleted = false,
      SortOrder order = SortOrder.Desc,
      int? parentId = null)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      if (parentId.HasValue)
        ArgumentUtility.CheckForNonPositiveInt(parentId.Value, nameof (parentId));
      this.TraceEnter(140271, nameof (GetCommentsFromParent));
      try
      {
        this.PrepareStoredProcedure("prc_GetCommentsFromParent");
        this.BindString("@artifactId", artifactId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@artifactKind", artifactKind);
        this.BindInt("@top", top);
        this.BindNullableDateTime("@startFrom", startFrom);
        this.BindBoolean("@fetchText", fetchText);
        this.BindBoolean("@fetchRenderedText", fetchRenderedText);
        this.BindBoolean("@fetchDeleted", fetchDeleted);
        this.BindBoolean("@ascending", order == SortOrder.Asc);
        this.BindNullableInt("@parentId", parentId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Comment>((ObjectBinder<Comment>) new CommentComponent4.CommentBinder2());
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
        this.TraceLeave(140279, nameof (GetCommentsFromParent));
      }
    }

    public override List<Comment> GetCommentsByIds(
      Guid artifactKind,
      string artifactId,
      ISet<int> commentIds,
      bool fetchText = true,
      bool fetchRenderedText = false,
      bool fetchDeleted = false,
      bool includeChildren = false,
      int expandChildCount = 5)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) commentIds, nameof (commentIds));
      this.TraceEnter(140201, nameof (GetCommentsByIds));
      try
      {
        this.PrepareStoredProcedure("prc_GetCommentsByIds");
        this.BindBoolean("@fetchText", fetchText);
        this.BindBoolean("@fetchRenderedText", fetchRenderedText);
        this.BindBoolean("@fetchDeleted", fetchDeleted);
        this.BindBoolean("@includeChildren", includeChildren);
        this.BindInt("@childExpandCount", expandChildCount);
        this.BindTypCommentIdTable("@comments", artifactKind, artifactId, (IEnumerable<int>) commentIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Comment>(this.GetCommentBinder());
          List<Comment> resultComments = resultCollection.GetCurrent<Comment>().Items ?? new List<Comment>();
          if (includeChildren)
            this.ReadChildComments(resultCollection, resultComments);
          return resultComments;
        }
      }
      finally
      {
        this.TraceLeave(140209, nameof (GetCommentsByIds));
      }
    }

    public override List<Comment> AddComments(
      Guid artifactKind,
      IEnumerable<AddComment> comments,
      CommentFormat commentFormat)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) comments, nameof (comments));
      this.TraceEnter(140211, nameof (AddComments));
      try
      {
        this.PrepareStoredProcedure("prc_AddComments");
        List<SqlDataRecord> rows = new List<SqlDataRecord>();
        int num1 = 0;
        foreach (AddComment comment in comments)
        {
          SqlDataRecord sqlDataRecord1 = new SqlDataRecord(CommentComponent4.TypCommentAddTable2);
          int num2 = 0;
          SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
          int ordinal1 = num2;
          int num3 = ordinal1 + 1;
          int num4 = num1++;
          sqlDataRecord2.SetInt32(ordinal1, num4);
          SqlDataRecord record1 = sqlDataRecord1;
          int ordinal2 = num3;
          int num5 = ordinal2 + 1;
          string artifactId = comment.ArtifactId;
          record1.SetString(ordinal2, artifactId, BindStringBehavior.Unchanged);
          SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
          int ordinal3 = num5;
          int num6 = ordinal3 + 1;
          Guid guid = artifactKind;
          sqlDataRecord3.SetGuid(ordinal3, guid);
          SqlDataRecord record2 = sqlDataRecord1;
          int ordinal4 = num6;
          int num7 = ordinal4 + 1;
          string text = comment.Text;
          record2.SetString(ordinal4, text, BindStringBehavior.Unchanged);
          SqlDataRecord record3 = sqlDataRecord1;
          int ordinal5 = num7;
          int num8 = ordinal5 + 1;
          string renderedText = comment.RenderedText;
          record3.SetString(ordinal5, renderedText, BindStringBehavior.EmptyStringToNull);
          SqlDataRecord sqlDataRecord4 = sqlDataRecord1;
          int ordinal6 = num8;
          int num9 = ordinal6 + 1;
          int num10 = (int) commentFormat;
          sqlDataRecord4.SetByte(ordinal6, (byte) num10);
          SqlDataRecord sqlDataRecord5 = sqlDataRecord1;
          int ordinal7 = num9;
          int num11 = ordinal7 + 1;
          Guid changedBy = comment.ChangedBy;
          sqlDataRecord5.SetGuid(ordinal7, changedBy);
          SqlDataRecord sqlDataRecord6 = sqlDataRecord1;
          int ordinal8 = num11;
          int num12 = ordinal8 + 1;
          DateTime changedDate = comment.ChangedDate;
          sqlDataRecord6.SetDateTime(ordinal8, changedDate);
          SqlDataRecord record4 = sqlDataRecord1;
          int ordinal9 = num12;
          int num13 = ordinal9 + 1;
          string createdOnBehalfOf = comment.CreatedOnBehalfOf;
          record4.SetString(ordinal9, createdOnBehalfOf, BindStringBehavior.EmptyStringToNull);
          SqlDataRecord record5 = sqlDataRecord1;
          int ordinal10 = num13;
          int num14 = ordinal10 + 1;
          DateTime? createdOnBehalfDate = comment.CreatedOnBehalfDate;
          record5.SetNullableDateTime(ordinal10, createdOnBehalfDate);
          SqlDataRecord record6 = sqlDataRecord1;
          int ordinal11 = num14;
          int num15 = ordinal11 + 1;
          int? parentId = comment.ParentId;
          record6.SetNullableInt32(ordinal11, parentId);
          SqlDataRecord record7 = sqlDataRecord1;
          int ordinal12 = num15;
          int num16 = ordinal12 + 1;
          string properties = comment.Properties;
          record7.SetNullableString(ordinal12, properties);
          rows.Add(sqlDataRecord1);
        }
        this.BindTable("@comments", "typ_CommentAddTable2", (IEnumerable<SqlDataRecord>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Comment>(this.GetCommentBinder());
          return resultCollection.GetCurrent<Comment>().Items ?? new List<Comment>();
        }
      }
      finally
      {
        this.TraceLeave(140219, nameof (AddComments));
      }
    }

    public override List<Comment> UpdateComments(
      Guid artifactKind,
      IEnumerable<UpdateComment> comments,
      CommentFormat commentFormat)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) comments, nameof (comments));
      this.TraceEnter(140221, nameof (UpdateComments));
      try
      {
        this.PrepareStoredProcedure("prc_UpdateComments");
        List<SqlDataRecord> rows = new List<SqlDataRecord>();
        foreach (UpdateComment comment in comments)
        {
          SqlDataRecord sqlDataRecord1 = new SqlDataRecord(CommentComponent4.TypCommentUpdateTable2);
          int num1 = 0;
          SqlDataRecord record1 = sqlDataRecord1;
          int ordinal1 = num1;
          int num2 = ordinal1 + 1;
          string artifactId = comment.ArtifactId;
          record1.SetString(ordinal1, artifactId, BindStringBehavior.Unchanged);
          SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
          int ordinal2 = num2;
          int num3 = ordinal2 + 1;
          Guid guid = artifactKind;
          sqlDataRecord2.SetGuid(ordinal2, guid);
          SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
          int ordinal3 = num3;
          int num4 = ordinal3 + 1;
          int commentId = comment.CommentId;
          sqlDataRecord3.SetInt32(ordinal3, commentId);
          SqlDataRecord record2 = sqlDataRecord1;
          int ordinal4 = num4;
          int num5 = ordinal4 + 1;
          string text = comment.Text;
          record2.SetString(ordinal4, text, BindStringBehavior.Unchanged);
          SqlDataRecord record3 = sqlDataRecord1;
          int ordinal5 = num5;
          int num6 = ordinal5 + 1;
          string renderedText = comment.RenderedText;
          record3.SetString(ordinal5, renderedText, BindStringBehavior.EmptyStringToNull);
          SqlDataRecord record4 = sqlDataRecord1;
          int ordinal6 = num6;
          int num7 = ordinal6 + 1;
          byte? nullable1 = new byte?((byte) commentFormat);
          record4.SetNullableByte(ordinal6, nullable1);
          SqlDataRecord sqlDataRecord4 = sqlDataRecord1;
          int ordinal7 = num7;
          int num8 = ordinal7 + 1;
          Guid changedBy = comment.ChangedBy;
          sqlDataRecord4.SetGuid(ordinal7, changedBy);
          SqlDataRecord sqlDataRecord5 = sqlDataRecord1;
          int ordinal8 = num8;
          int num9 = ordinal8 + 1;
          DateTime changedDate = comment.ChangedDate;
          sqlDataRecord5.SetDateTime(ordinal8, changedDate);
          SqlDataRecord record5 = sqlDataRecord1;
          int ordinal9 = num9;
          int num10 = ordinal9 + 1;
          int? nullable2 = new int?();
          record5.SetNullableInt32(ordinal9, nullable2);
          SqlDataRecord record6 = sqlDataRecord1;
          int ordinal10 = num10;
          int num11 = ordinal10 + 1;
          int? nullable3 = new int?();
          record6.SetNullableInt32(ordinal10, nullable3);
          SqlDataRecord record7 = sqlDataRecord1;
          int ordinal11 = num11;
          int num12 = ordinal11 + 1;
          CommentState? state = comment.State;
          byte? nullable4 = state.HasValue ? new byte?((byte) state.GetValueOrDefault()) : new byte?();
          record7.SetNullableByte(ordinal11, nullable4);
          SqlDataRecord record8 = sqlDataRecord1;
          int ordinal12 = num12;
          int num13 = ordinal12 + 1;
          string properties = comment.Properties;
          record8.SetNullableString(ordinal12, properties);
          rows.Add(sqlDataRecord1);
        }
        this.BindTable("@comments", "typ_CommentUpdateTable2", (IEnumerable<SqlDataRecord>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Comment>(this.GetCommentBinder());
          return resultCollection.GetCurrent<Comment>().Items ?? new List<Comment>();
        }
      }
      finally
      {
        this.TraceLeave(140229, nameof (UpdateComments));
      }
    }

    public override List<Comment> DeleteComments(
      Guid artifactKind,
      IEnumerable<DeleteComment> comments)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) comments, nameof (comments));
      this.TraceEnter(140231, nameof (DeleteComments));
      try
      {
        this.PrepareStoredProcedure("prc_UpdateComments");
        this.BindTable("@comments", "typ_CommentUpdateTable2", comments.Select<DeleteComment, SqlDataRecord>((System.Func<DeleteComment, SqlDataRecord>) (comment =>
        {
          SqlDataRecord record = new SqlDataRecord(CommentComponent4.TypCommentUpdateTable2);
          int ordinal1 = 0;
          int num1 = ordinal1 + 1;
          record.SetString(ordinal1, comment.ArtifactId, BindStringBehavior.Unchanged);
          int ordinal2 = num1;
          int num2 = ordinal2 + 1;
          record.SetGuid(ordinal2, artifactKind);
          int ordinal3 = num2;
          int num3 = ordinal3 + 1;
          record.SetInt32(ordinal3, comment.CommentId);
          int ordinal4 = num3;
          int num4 = ordinal4 + 1;
          record.SetNullableString(ordinal4, (string) null);
          int ordinal5 = num4;
          int num5 = ordinal5 + 1;
          record.SetNullableString(ordinal5, (string) null);
          int ordinal6 = num5;
          int num6 = ordinal6 + 1;
          record.SetNullableByte(ordinal6, new byte?());
          int ordinal7 = num6;
          int num7 = ordinal7 + 1;
          record.SetGuid(ordinal7, comment.ChangedBy);
          int ordinal8 = num7;
          int num8 = ordinal8 + 1;
          record.SetDateTime(ordinal8, comment.ChangedDate);
          int ordinal9 = num8;
          int num9 = ordinal9 + 1;
          record.SetBoolean(ordinal9, true);
          int ordinal10 = num9;
          int num10 = ordinal10 + 1;
          record.SetNullableInt32(ordinal10, new int?());
          int ordinal11 = num10;
          int num11 = ordinal11 + 1;
          record.SetNullableInt32(ordinal11, new int?());
          int ordinal12 = num11;
          int num12 = ordinal12 + 1;
          record.SetNullableString(ordinal12, (string) null);
          return record;
        })));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Comment>(this.GetCommentBinder());
          return resultCollection.GetCurrent<Comment>().Items ?? new List<Comment>();
        }
      }
      finally
      {
        this.TraceLeave(140239, nameof (DeleteComments));
      }
    }

    public override void DestroyComments(
      Guid artifactKind,
      IEnumerable<string> artifactIds,
      int batchSize = 2000,
      DateTime? destroyedDate = null)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) artifactIds, nameof (artifactIds));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) artifactIds, nameof (artifactIds));
      ArgumentUtility.CheckBoundsInclusive(batchSize, 1, 4000, nameof (batchSize));
      try
      {
        this.PrepareStoredProcedure("prc_DestroyComments");
        this.BindGuid("@artifactKind", artifactKind);
        this.BindStringTable("@artifactIds", artifactIds, maxLength: 256);
        this.BindInt("@batchSize", batchSize);
        this.BindDateTime("@changeDate", destroyedDate ?? DateTime.UtcNow, true);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(140239, nameof (DestroyComments));
      }
    }

    private void ReadChildComments(ResultCollection resultCollection, List<Comment> resultComments)
    {
      resultCollection.AddBinder<Comment>(this.GetCommentBinder());
      resultCollection.NextResult();
      List<Comment> source1 = resultCollection.GetCurrent<Comment>().Items ?? new List<Comment>();
      resultCollection.AddBinder<CommentComponent4.ChildCommentCounts>((ObjectBinder<CommentComponent4.ChildCommentCounts>) new CommentComponent4.CommentChildCountBinder());
      resultCollection.NextResult();
      List<CommentComponent4.ChildCommentCounts> source2 = resultCollection.GetCurrent<CommentComponent4.ChildCommentCounts>().Items ?? new List<CommentComponent4.ChildCommentCounts>();
      foreach (Comment resultComment in resultComments)
      {
        Comment comment = resultComment;
        IEnumerable<Comment> source3 = source1.Where<Comment>((System.Func<Comment, bool>) (c => c.ParentId.Value == comment.CommentId));
        if (source3.Any<Comment>())
        {
          CommentComponent4.ChildCommentCounts childCommentCounts = source2.FirstOrDefault<CommentComponent4.ChildCommentCounts>((System.Func<CommentComponent4.ChildCommentCounts, bool>) (c => c.ParentId == comment.CommentId));
          comment.Children = new CommentsList((ICollection<Comment>) source3.ToList<Comment>())
          {
            TotalCount = childCommentCounts != null ? childCommentCounts.CommentCount : 0
          };
        }
      }
    }

    protected override ObjectBinder<Comment> GetCommentBinder() => (ObjectBinder<Comment>) new CommentComponent4.CommentBinder2();

    protected override ObjectBinder<CommentVersion> GetCommentVersionBinder() => (ObjectBinder<CommentVersion>) new CommentComponent4.CommentVersionBinder2();

    internal class CommentBinder2 : CommentComponent.CommentBinder
    {
      protected SqlColumnBinder parentId = new SqlColumnBinder("ParentId");
      protected SqlColumnBinder state = new SqlColumnBinder("State");
      protected SqlColumnBinder properties = new SqlColumnBinder("Properties");

      protected override Comment Bind()
      {
        Comment comment = base.Bind();
        int int32 = this.parentId.GetInt32((IDataReader) this.Reader, 0, 0);
        comment.ParentId = int32 > 0 ? new int?(int32) : new int?();
        comment.State = (CommentState) this.state.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0);
        comment.Properties = this.properties.GetString((IDataReader) this.Reader, (string) null);
        return comment;
      }
    }

    internal class ChildCommentCounts
    {
      public int ParentId { get; set; }

      public int CommentCount { get; set; }
    }

    internal class CommentChildCountBinder : ObjectBinder<CommentComponent4.ChildCommentCounts>
    {
      private SqlColumnBinder parentId = new SqlColumnBinder("ParentId");
      private SqlColumnBinder commentCount = new SqlColumnBinder("CommentCount");

      protected override CommentComponent4.ChildCommentCounts Bind() => new CommentComponent4.ChildCommentCounts()
      {
        ParentId = this.parentId.GetInt32((IDataReader) this.Reader),
        CommentCount = this.commentCount.GetInt32((IDataReader) this.Reader)
      };
    }

    internal class CommentVersionBinder2 : CommentComponent.CommentVersionBinder
    {
      protected SqlColumnBinder parentId = new SqlColumnBinder("ParentId");
      protected SqlColumnBinder state = new SqlColumnBinder("State");
      protected SqlColumnBinder properties = new SqlColumnBinder("Properties");

      protected override CommentVersion Bind()
      {
        CommentVersion commentVersion = base.Bind();
        int int32 = this.parentId.GetInt32((IDataReader) this.Reader, 0, 0);
        commentVersion.ParentId = int32 > 0 ? new int?(int32) : new int?();
        commentVersion.State = (CommentState) this.state.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0);
        commentVersion.Properties = this.properties.GetString((IDataReader) this.Reader, (string) null);
        return commentVersion;
      }
    }
  }
}
