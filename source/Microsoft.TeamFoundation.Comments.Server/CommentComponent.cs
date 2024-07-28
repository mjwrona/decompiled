// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentComponent
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
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class CommentComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[5]
    {
      (IComponentCreator) new ComponentCreator<CommentComponent>(1),
      (IComponentCreator) new ComponentCreator<CommentComponent2>(2),
      (IComponentCreator) new ComponentCreator<CommentComponent3>(3),
      (IComponentCreator) new ComponentCreator<CommentComponent4>(4),
      (IComponentCreator) new ComponentCreator<CommentComponent5>(5)
    }, "Comments");
    private static readonly IDictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static readonly SqlMetaData[] TypCommentAddTable = new SqlMetaData[10]
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
      new SqlMetaData("CreatedOnBehalfDate", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] TypCommentIdTable = new SqlMetaData[3]
    {
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ArtifactKind", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CommentId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] TypCommentUpdateTable = new SqlMetaData[9]
    {
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ArtifactKind", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CommentId", SqlDbType.Int),
      new SqlMetaData("Text", SqlDbType.NVarChar, -1L),
      new SqlMetaData("RenderedText", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Format", SqlDbType.TinyInt),
      new SqlMetaData("ModifiedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ModifiedDate", SqlDbType.DateTime),
      new SqlMetaData("IsDeleted", SqlDbType.Bit)
    };

    static CommentComponent()
    {
      CommentComponent.s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();
      CommentComponent.s_sqlExceptionFactories[2100001] = new SqlExceptionFactory(typeof (InvalidOperationException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((_1, _2, _3, _4) => (Exception) new InvalidOperationException(Resources.InvalidParentSpecified)));
    }

    public CommentComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => CommentComponent.s_sqlExceptionFactories;

    public virtual CommentsList GetComments(
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

    public virtual CommentsList GetCommentsWithChildren(
      Guid artifactKind,
      string artifactId,
      int top,
      DateTime? startFrom,
      bool fetchText = true,
      bool fetchRenderedText = false,
      bool fetchDeleted = false,
      SortOrder order = SortOrder.Desc,
      int expandChildCount = 0)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      this.TraceEnter(140261, nameof (GetCommentsWithChildren));
      try
      {
        return this.GetComments(artifactKind, artifactId, top, startFrom, fetchText, fetchRenderedText, fetchDeleted, order);
      }
      finally
      {
        this.TraceLeave(140269, nameof (GetCommentsWithChildren));
      }
    }

    public virtual CommentsList GetCommentsFromParent(
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
      this.TraceEnter(140261, nameof (GetCommentsFromParent));
      try
      {
        return this.GetComments(artifactKind, artifactId, top, startFrom, fetchText, fetchRenderedText, fetchDeleted, order);
      }
      finally
      {
        this.TraceLeave(140269, nameof (GetCommentsFromParent));
      }
    }

    public virtual List<Comment> GetCommentsByIds(
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
        this.BindTypCommentIdTable("@comments", artifactKind, artifactId, (IEnumerable<int>) commentIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Comment>(this.GetCommentBinder());
          return resultCollection.GetCurrent<Comment>().Items ?? new List<Comment>();
        }
      }
      finally
      {
        this.TraceLeave(140209, nameof (GetCommentsByIds));
      }
    }

    public virtual List<CommentVersion> GetCommentVersions(
      Guid artifactKind,
      string artifactId,
      int commentId)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      this.TraceEnter(140241, nameof (GetCommentVersions));
      try
      {
        this.PrepareStoredProcedure("prc_GetCommentVersions");
        this.BindTypCommentIdTable("@comments", artifactKind, artifactId, (IEnumerable<int>) new int[1]
        {
          commentId
        });
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CommentVersion>(this.GetCommentVersionBinder());
          return resultCollection.GetCurrent<CommentVersion>().Items ?? new List<CommentVersion>();
        }
      }
      finally
      {
        this.TraceLeave(140242, nameof (GetCommentVersions));
      }
    }

    public virtual List<CommentVersion> GetCommentsVersions(
      Guid artifactKind,
      IEnumerable<GetCommentVersion> commentVersions)
    {
      return new List<CommentVersion>();
    }

    public virtual List<Comment> AddComments(
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
          SqlDataRecord record1 = new SqlDataRecord(CommentComponent.TypCommentAddTable);
          int num2 = 0;
          SqlDataRecord sqlDataRecord1 = record1;
          int ordinal1 = num2;
          int num3 = ordinal1 + 1;
          int num4 = num1++;
          sqlDataRecord1.SetInt32(ordinal1, num4);
          SqlDataRecord record2 = record1;
          int ordinal2 = num3;
          int num5 = ordinal2 + 1;
          string artifactId = comment.ArtifactId;
          record2.SetString(ordinal2, artifactId, BindStringBehavior.Unchanged);
          SqlDataRecord sqlDataRecord2 = record1;
          int ordinal3 = num5;
          int num6 = ordinal3 + 1;
          Guid guid = artifactKind;
          sqlDataRecord2.SetGuid(ordinal3, guid);
          SqlDataRecord record3 = record1;
          int ordinal4 = num6;
          int num7 = ordinal4 + 1;
          string text = comment.Text;
          record3.SetString(ordinal4, text, BindStringBehavior.Unchanged);
          SqlDataRecord record4 = record1;
          int ordinal5 = num7;
          int num8 = ordinal5 + 1;
          string renderedText = comment.RenderedText;
          record4.SetString(ordinal5, renderedText, BindStringBehavior.EmptyStringToNull);
          SqlDataRecord sqlDataRecord3 = record1;
          int ordinal6 = num8;
          int num9 = ordinal6 + 1;
          int num10 = (int) commentFormat;
          sqlDataRecord3.SetByte(ordinal6, (byte) num10);
          SqlDataRecord sqlDataRecord4 = record1;
          int ordinal7 = num9;
          int num11 = ordinal7 + 1;
          Guid changedBy = comment.ChangedBy;
          sqlDataRecord4.SetGuid(ordinal7, changedBy);
          SqlDataRecord sqlDataRecord5 = record1;
          int ordinal8 = num11;
          int num12 = ordinal8 + 1;
          DateTime changedDate = comment.ChangedDate;
          sqlDataRecord5.SetDateTime(ordinal8, changedDate);
          SqlDataRecord record5 = record1;
          int ordinal9 = num12;
          int ordinal10 = ordinal9 + 1;
          string createdOnBehalfOf = comment.CreatedOnBehalfOf;
          record5.SetString(ordinal9, createdOnBehalfOf, BindStringBehavior.EmptyStringToNull);
          record1.SetNullableDateTime(ordinal10, comment.CreatedOnBehalfDate);
          rows.Add(record1);
        }
        this.BindTable("@comments", "typ_CommentAddTable", (IEnumerable<SqlDataRecord>) rows);
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

    public virtual List<Comment> UpdateComments(
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
          SqlDataRecord sqlDataRecord1 = new SqlDataRecord(CommentComponent.TypCommentUpdateTable);
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
          SqlDataRecord sqlDataRecord4 = sqlDataRecord1;
          int ordinal6 = num6;
          int num7 = ordinal6 + 1;
          int num8 = (int) commentFormat;
          sqlDataRecord4.SetByte(ordinal6, (byte) num8);
          SqlDataRecord sqlDataRecord5 = sqlDataRecord1;
          int ordinal7 = num7;
          int ordinal8 = ordinal7 + 1;
          Guid changedBy = comment.ChangedBy;
          sqlDataRecord5.SetGuid(ordinal7, changedBy);
          sqlDataRecord1.SetDateTime(ordinal8, comment.ChangedDate);
          rows.Add(sqlDataRecord1);
        }
        this.BindTable("@comments", "typ_CommentUpdateTable", (IEnumerable<SqlDataRecord>) rows);
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

    public virtual List<Comment> DeleteComments(
      Guid artifactKind,
      IEnumerable<DeleteComment> comments)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) comments, nameof (comments));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) comments, nameof (comments));
      this.TraceEnter(140231, nameof (DeleteComments));
      try
      {
        this.PrepareStoredProcedure("prc_UpdateComments");
        this.BindTable("@comments", "typ_CommentUpdateTable", comments.Select<DeleteComment, SqlDataRecord>((System.Func<DeleteComment, SqlDataRecord>) (comment =>
        {
          SqlDataRecord record = new SqlDataRecord(CommentComponent.TypCommentUpdateTable);
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
          int ordinal9 = ordinal8 + 1;
          record.SetDateTime(ordinal8, comment.ChangedDate);
          record.SetBoolean(ordinal9, true);
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

    public virtual List<CommentAttachment> GetAttacmhents(
      IEnumerable<CommentToAttachment> commentToAttachments)
    {
      return new List<CommentAttachment>();
    }

    public virtual List<CommentAttachment> AddAttacmhents(IEnumerable<CommentAttachment> attachments) => new List<CommentAttachment>();

    public virtual void DestroyComments(
      Guid artifactKind,
      IEnumerable<string> artifactIds,
      int batchSize = 2000,
      DateTime? destroyedDate = null)
    {
    }

    protected virtual ObjectBinder<Comment> GetCommentBinder() => (ObjectBinder<Comment>) new CommentComponent.CommentBinder();

    protected virtual ObjectBinder<CommentVersion> GetCommentVersionBinder() => (ObjectBinder<CommentVersion>) new CommentComponent.CommentVersionBinder();

    protected virtual void BindTypCommentIdTable(
      string paramName,
      Guid artifactKind,
      string artifactId,
      IEnumerable<int> commentIds)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (int commentId in commentIds)
      {
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(CommentComponent.TypCommentIdTable);
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
      this.BindTable(paramName, "typ_CommentIdTable", (IEnumerable<SqlDataRecord>) rows);
    }

    internal class CommentBinder : ObjectBinder<Comment>
    {
      private SqlColumnBinder artifactId = new SqlColumnBinder("ArtifactId");
      private SqlColumnBinder artifactKind = new SqlColumnBinder("ArtifactKind");
      private SqlColumnBinder commentId = new SqlColumnBinder("CommentId");
      private SqlColumnBinder version = new SqlColumnBinder("Version");
      private SqlColumnBinder text = new SqlColumnBinder("Text");
      private SqlColumnBinder renderedText = new SqlColumnBinder("RenderedText");
      private SqlColumnBinder format = new SqlColumnBinder("Format");
      private SqlColumnBinder createdBy = new SqlColumnBinder("CreatedBy");
      private SqlColumnBinder createdDate = new SqlColumnBinder("CreatedDate");
      private SqlColumnBinder createdOnBehalfOf = new SqlColumnBinder("CreatedOnBehalfOf");
      private SqlColumnBinder createdOnBehalfDate = new SqlColumnBinder("CreatedOnBehalfDate");
      private SqlColumnBinder modifiedBy = new SqlColumnBinder("ModifiedBy");
      private SqlColumnBinder modifiedDate = new SqlColumnBinder("ModifiedDate");
      private SqlColumnBinder isDeleted = new SqlColumnBinder("IsDeleted");

      protected override Comment Bind() => new Comment(this.artifactKind.GetGuid((IDataReader) this.Reader, false), this.artifactId.GetString((IDataReader) this.Reader, false), this.commentId.GetInt32((IDataReader) this.Reader), this.text.GetString((IDataReader) this.Reader, false), this.renderedText.GetString((IDataReader) this.Reader, true))
      {
        Version = this.version.GetInt32((IDataReader) this.Reader),
        Format = (CommentFormat) this.format.GetByte((IDataReader) this.Reader),
        CreatedBy = this.createdBy.GetGuid((IDataReader) this.Reader, false),
        CreatedDate = this.createdDate.GetDateTime((IDataReader) this.Reader),
        CreatedOnBehalfOf = this.createdOnBehalfOf.GetString((IDataReader) this.Reader, false),
        CreatedOnBehalfDate = this.createdOnBehalfDate.GetDateTime((IDataReader) this.Reader),
        ModifiedBy = this.modifiedBy.GetGuid((IDataReader) this.Reader, false),
        ModifiedDate = this.modifiedDate.GetDateTime((IDataReader) this.Reader),
        IsDeleted = this.isDeleted.GetBoolean((IDataReader) this.Reader, false)
      };
    }

    internal class CommentVersionBinder : ObjectBinder<CommentVersion>
    {
      private SqlColumnBinder artifactId = new SqlColumnBinder("ArtifactId");
      private SqlColumnBinder artifactKind = new SqlColumnBinder("ArtifactKind");
      private SqlColumnBinder commentId = new SqlColumnBinder("CommentId");
      private SqlColumnBinder version = new SqlColumnBinder("Version");
      private SqlColumnBinder text = new SqlColumnBinder("Text");
      private SqlColumnBinder renderedText = new SqlColumnBinder("RenderedText");
      private SqlColumnBinder format = new SqlColumnBinder("Format");
      private SqlColumnBinder createdBy = new SqlColumnBinder("CreatedBy");
      private SqlColumnBinder createdDate = new SqlColumnBinder("CreatedDate");
      private SqlColumnBinder createdOnBehalfOf = new SqlColumnBinder("CreatedOnBehalfOf");
      private SqlColumnBinder createdOnBehalfDate = new SqlColumnBinder("CreatedOnBehalfDate");
      private SqlColumnBinder modifiedBy = new SqlColumnBinder("ModifiedBy");
      private SqlColumnBinder modifiedDate = new SqlColumnBinder("ModifiedDate");
      private SqlColumnBinder isDeleted = new SqlColumnBinder("IsDeleted");

      protected override CommentVersion Bind() => new CommentVersion(this.artifactKind.GetGuid((IDataReader) this.Reader, false), this.artifactId.GetString((IDataReader) this.Reader, false), this.commentId.GetInt32((IDataReader) this.Reader), this.version.GetInt32((IDataReader) this.Reader), this.text.GetString((IDataReader) this.Reader, false))
      {
        RenderedText = this.renderedText.GetString((IDataReader) this.Reader, true),
        Format = (CommentFormat) this.format.GetByte((IDataReader) this.Reader),
        CreatedBy = this.createdBy.GetGuid((IDataReader) this.Reader, false),
        CreatedDate = this.createdDate.GetDateTime((IDataReader) this.Reader),
        CreatedOnBehalfOf = this.createdOnBehalfOf.GetString((IDataReader) this.Reader, false),
        CreatedOnBehalfDate = this.createdOnBehalfDate.GetDateTime((IDataReader) this.Reader),
        ModifiedBy = this.modifiedBy.GetGuid((IDataReader) this.Reader, false),
        ModifiedDate = this.modifiedDate.GetDateTime((IDataReader) this.Reader),
        IsDeleted = this.isDeleted.GetBoolean((IDataReader) this.Reader, false)
      };
    }

    internal class CommentCountBinder : ObjectBinder<int?>
    {
      private SqlColumnBinder commentCount = new SqlColumnBinder("CommentCount");

      protected override int? Bind() => new int?(this.commentCount.GetInt32((IDataReader) this.Reader));
    }
  }
}
