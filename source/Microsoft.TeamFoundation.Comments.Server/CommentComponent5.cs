// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentComponent5
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
  public class CommentComponent5 : CommentComponent4
  {
    protected static readonly SqlMetaData[] TypCommentAttachmentTable = new SqlMetaData[4]
    {
      new SqlMetaData("AttachmentId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("FileId", SqlDbType.Int),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CreatedDate", SqlDbType.DateTime)
    };
    protected static readonly SqlMetaData[] TypCommentToAttachmentTable = new SqlMetaData[4]
    {
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ArtifactKind", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CommentId", SqlDbType.Int),
      new SqlMetaData("AttachmentId", SqlDbType.UniqueIdentifier)
    };

    public override List<Comment> AddComments(
      Guid artifactKind,
      IEnumerable<AddComment> comments,
      CommentFormat commentFormat)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) comments, nameof (comments));
      using (this.RequestContext.TraceBlock(140211, 140219, "CommentService", "Service", nameof (AddComments)))
      {
        this.PrepareStoredProcedure("prc_AddComments");
        List<SqlDataRecord> rows1 = new List<SqlDataRecord>();
        List<SqlDataRecord> rows2 = new List<SqlDataRecord>();
        int num1 = 0;
        foreach (AddComment comment in comments)
        {
          SqlDataRecord sqlDataRecord1 = new SqlDataRecord(CommentComponent4.TypCommentAddTable2);
          int num2 = 0;
          SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
          int ordinal1 = num2;
          int num3 = ordinal1 + 1;
          int num4 = num1;
          sqlDataRecord2.SetInt32(ordinal1, num4);
          SqlDataRecord record1 = sqlDataRecord1;
          int ordinal2 = num3;
          int num5 = ordinal2 + 1;
          string artifactId1 = comment.ArtifactId;
          record1.SetString(ordinal2, artifactId1, BindStringBehavior.Unchanged);
          SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
          int ordinal3 = num5;
          int num6 = ordinal3 + 1;
          Guid guid1 = artifactKind;
          sqlDataRecord3.SetGuid(ordinal3, guid1);
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
          rows1.Add(sqlDataRecord1);
          foreach (Guid attachment in (IEnumerable<Guid>) comment.Attachments)
          {
            SqlDataRecord sqlDataRecord7 = new SqlDataRecord(CommentComponent5.TypCommentToAttachmentTable);
            int num17 = 0;
            SqlDataRecord record8 = sqlDataRecord7;
            int ordinal13 = num17;
            int num18 = ordinal13 + 1;
            string artifactId2 = comment.ArtifactId;
            record8.SetString(ordinal13, artifactId2, BindStringBehavior.Unchanged);
            SqlDataRecord sqlDataRecord8 = sqlDataRecord7;
            int ordinal14 = num18;
            int num19 = ordinal14 + 1;
            Guid guid2 = artifactKind;
            sqlDataRecord8.SetGuid(ordinal14, guid2);
            SqlDataRecord sqlDataRecord9 = sqlDataRecord7;
            int ordinal15 = num19;
            int ordinal16 = ordinal15 + 1;
            int num20 = num1;
            sqlDataRecord9.SetInt32(ordinal15, num20);
            sqlDataRecord7.SetGuid(ordinal16, attachment);
            rows2.Add(sqlDataRecord7);
          }
          ++num1;
        }
        this.BindTable("@comments", "typ_CommentAddTable2", (IEnumerable<SqlDataRecord>) rows1);
        this.BindTable("@attachments", "typ_CommentToAttachmentTable", (IEnumerable<SqlDataRecord>) rows2);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Comment>(this.GetCommentBinder());
          return resultCollection.GetCurrent<Comment>().Items ?? new List<Comment>();
        }
      }
    }

    public override List<Comment> UpdateComments(
      Guid artifactKind,
      IEnumerable<UpdateComment> comments,
      CommentFormat commentFormat)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) comments, nameof (comments));
      using (this.RequestContext.TraceBlock(140221, 140229, "CommentService", "Service", nameof (UpdateComments)))
      {
        this.PrepareStoredProcedure("prc_UpdateComments");
        List<SqlDataRecord> rows1 = new List<SqlDataRecord>();
        List<SqlDataRecord> rows2 = new List<SqlDataRecord>();
        foreach (UpdateComment comment in comments)
        {
          SqlDataRecord sqlDataRecord1 = new SqlDataRecord(CommentComponent4.TypCommentUpdateTable2);
          int num1 = 0;
          SqlDataRecord record1 = sqlDataRecord1;
          int ordinal1 = num1;
          int num2 = ordinal1 + 1;
          string artifactId1 = comment.ArtifactId;
          record1.SetString(ordinal1, artifactId1, BindStringBehavior.Unchanged);
          SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
          int ordinal2 = num2;
          int num3 = ordinal2 + 1;
          Guid guid1 = artifactKind;
          sqlDataRecord2.SetGuid(ordinal2, guid1);
          SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
          int ordinal3 = num3;
          int num4 = ordinal3 + 1;
          int commentId1 = comment.CommentId;
          sqlDataRecord3.SetInt32(ordinal3, commentId1);
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
          rows1.Add(sqlDataRecord1);
          foreach (Guid attachment in (IEnumerable<Guid>) comment.Attachments)
          {
            SqlDataRecord sqlDataRecord6 = new SqlDataRecord(CommentComponent5.TypCommentToAttachmentTable);
            int num14 = 0;
            SqlDataRecord record9 = sqlDataRecord6;
            int ordinal13 = num14;
            int num15 = ordinal13 + 1;
            string artifactId2 = comment.ArtifactId;
            record9.SetString(ordinal13, artifactId2, BindStringBehavior.Unchanged);
            SqlDataRecord sqlDataRecord7 = sqlDataRecord6;
            int ordinal14 = num15;
            int num16 = ordinal14 + 1;
            Guid guid2 = artifactKind;
            sqlDataRecord7.SetGuid(ordinal14, guid2);
            SqlDataRecord sqlDataRecord8 = sqlDataRecord6;
            int ordinal15 = num16;
            int ordinal16 = ordinal15 + 1;
            int commentId2 = comment.CommentId;
            sqlDataRecord8.SetInt32(ordinal15, commentId2);
            sqlDataRecord6.SetGuid(ordinal16, attachment);
            rows2.Add(sqlDataRecord6);
          }
        }
        this.BindTable("@comments", "typ_CommentUpdateTable2", (IEnumerable<SqlDataRecord>) rows1);
        this.BindTable("@attachments", "typ_CommentToAttachmentTable", (IEnumerable<SqlDataRecord>) rows2);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Comment>(this.GetCommentBinder());
          return resultCollection.GetCurrent<Comment>().Items ?? new List<Comment>();
        }
      }
    }

    public override List<CommentAttachment> GetAttacmhents(
      IEnumerable<CommentToAttachment> commentToAttachments)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) commentToAttachments, nameof (commentToAttachments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) commentToAttachments, nameof (commentToAttachments));
      using (this.RequestContext.TraceBlock(140281, 140289, "CommentService", "Service", nameof (GetAttacmhents)))
      {
        this.PrepareStoredProcedure("prc_GetCommentAttachments");
        this.BindTypCommentToAttachmentTable("@attachmentIds", commentToAttachments);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CommentAttachment>((ObjectBinder<CommentAttachment>) new CommentComponent5.CommentAttachmentBinder());
          return resultCollection.GetCurrent<CommentAttachment>().Items ?? new List<CommentAttachment>();
        }
      }
    }

    public override List<CommentAttachment> AddAttacmhents(
      IEnumerable<CommentAttachment> attachments)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) attachments, nameof (attachments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) attachments, nameof (attachments));
      using (this.RequestContext.TraceBlock(140291, 140299, "CommentService", "Service", nameof (AddAttacmhents)))
      {
        this.PrepareStoredProcedure("prc_AddCommentAttachments");
        this.BindTypCommentAttachmentTable("@attachments", attachments);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CommentAttachment>((ObjectBinder<CommentAttachment>) new CommentComponent5.CommentAttachmentBinder());
          return resultCollection.GetCurrent<CommentAttachment>().Items ?? new List<CommentAttachment>();
        }
      }
    }

    protected virtual void BindTypCommentAttachmentTable(
      string paramName,
      IEnumerable<CommentAttachment> attachments)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (CommentAttachment attachment in attachments)
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(CommentComponent5.TypCommentAttachmentTable);
        int num1 = 0;
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        Guid id = attachment.Id;
        sqlDataRecord2.SetGuid(ordinal1, id);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        int fileId = attachment.FileId;
        sqlDataRecord3.SetInt32(ordinal2, fileId);
        SqlDataRecord sqlDataRecord4 = sqlDataRecord1;
        int ordinal3 = num3;
        int ordinal4 = ordinal3 + 1;
        Guid createdBy = attachment.CreatedBy;
        sqlDataRecord4.SetGuid(ordinal3, createdBy);
        sqlDataRecord1.SetDateTime(ordinal4, attachment.CreatedDate);
        rows.Add(sqlDataRecord1);
      }
      this.BindTable(paramName, "typ_CommentAttachmentTable", (IEnumerable<SqlDataRecord>) rows);
    }

    protected virtual void BindTypCommentToAttachmentTable(
      string paramName,
      IEnumerable<CommentToAttachment> attachments)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (CommentToAttachment attachment in attachments)
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(CommentComponent5.TypCommentToAttachmentTable);
        int num1 = 0;
        SqlDataRecord record = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        string artifactId = attachment.ArtifactId;
        record.SetString(ordinal1, artifactId, BindStringBehavior.Unchanged);
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        Guid artifactKind = attachment.ArtifactKind;
        sqlDataRecord2.SetGuid(ordinal2, artifactKind);
        int ordinal3 = num3 + 1;
        sqlDataRecord1.SetGuid(ordinal3, attachment.AttachmentId);
        rows.Add(sqlDataRecord1);
      }
      this.BindTable(paramName, "typ_CommentToAttachmentTable", (IEnumerable<SqlDataRecord>) rows);
    }

    protected class CommentAttachmentBinder : ObjectBinder<CommentAttachment>
    {
      private SqlColumnBinder attachmentId = new SqlColumnBinder("AttachmentId");
      private SqlColumnBinder fileId = new SqlColumnBinder("FileId");
      private SqlColumnBinder createdBy = new SqlColumnBinder("CreatedBy");
      private SqlColumnBinder createdDate = new SqlColumnBinder("CreatedDate");

      protected override CommentAttachment Bind() => new CommentAttachment(this.attachmentId.GetGuid((IDataReader) this.Reader, false), this.fileId.GetInt32((IDataReader) this.Reader))
      {
        CreatedBy = this.createdBy.GetGuid((IDataReader) this.Reader, false),
        CreatedDate = this.createdDate.GetDateTime((IDataReader) this.Reader)
      };
    }
  }
}
