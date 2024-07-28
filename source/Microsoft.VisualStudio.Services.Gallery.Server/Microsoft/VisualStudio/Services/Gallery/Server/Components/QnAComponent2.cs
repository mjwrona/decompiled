// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.QnAComponent2
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class QnAComponent2 : QnAComponent1
  {
    public virtual void ReportQnAItem(
      Guid extensionId,
      long parentId,
      long itemId,
      Guid userId,
      ConcernSource concernSource,
      ConcernCategory concernCategory,
      string concernText,
      DateTime submittedDate)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreateUserReportedConcernForQnA");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindLong(nameof (parentId), parentId);
      this.BindLong(nameof (itemId), itemId);
      this.BindGuid(nameof (userId), userId);
      this.BindInt(nameof (concernSource), (int) concernSource);
      this.BindInt(nameof (concernCategory), (int) concernCategory);
      this.BindString(nameof (concernText), concernText, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindDateTime(nameof (submittedDate), submittedDate);
      this.ExecuteScalar();
    }

    public virtual List<ReportedExtensionQnAItem> GetReportedQnAItem(
      Guid? extensionId,
      ConcernSource concernSource)
    {
      string str = "Gallery.prc_GetFlaggedQnAItems";
      this.PrepareStoredProcedure(str);
      if (extensionId.HasValue)
        this.BindGuid(nameof (extensionId), extensionId.Value);
      this.BindInt(nameof (concernSource), (int) concernSource);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ReportedExtensionQnAItem>((ObjectBinder<ReportedExtensionQnAItem>) new QnAComponent2.ReportedExtensionQnAItemBinder());
        return resultCollection.GetCurrent<ReportedExtensionQnAItem>().Items;
      }
    }

    public virtual int DeleteQnAItems(
      Guid extensionId,
      long? parentId = null,
      long? itemId = null,
      bool isHardDelete = false)
    {
      this.PrepareStoredProcedure("Gallery.prc_DeleteQnAItems");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindBoolean(nameof (isHardDelete), isHardDelete);
      if (parentId.HasValue)
        this.BindLong(nameof (parentId), parentId.Value);
      if (itemId.HasValue)
        this.BindLong(nameof (itemId), itemId.Value);
      return (int) this.ExecuteNonQuery(true);
    }

    internal class ReportedExtensionQnAItemBinder : ObjectBinder<ReportedExtensionQnAItem>
    {
      protected SqlColumnBinder idColumn = new SqlColumnBinder("Id");
      protected SqlColumnBinder parentIdColumn = new SqlColumnBinder("ParentId");
      protected SqlColumnBinder qnATextColumn = new SqlColumnBinder("QnAText");
      protected SqlColumnBinder extensionIdColumn = new SqlColumnBinder("ExtensionId");
      protected SqlColumnBinder createdDateColumn = new SqlColumnBinder("CreatedDate");
      protected SqlColumnBinder updatedDateColumn = new SqlColumnBinder("UpdatedDate");
      protected SqlColumnBinder isQuestionColumn = new SqlColumnBinder("IsQuestion");
      protected SqlColumnBinder userIdColumn = new SqlColumnBinder("UserId");
      protected SqlColumnBinder isPublisherCreatedColumn = new SqlColumnBinder("IsPublisherCreated");
      protected SqlColumnBinder extensionVersionColumn = new SqlColumnBinder("ExtensionVersion");
      protected SqlColumnBinder isDeletedColumn = new SqlColumnBinder("IsDeleted");
      protected SqlColumnBinder reportedByColumn = new SqlColumnBinder("ReportedBy");
      protected SqlColumnBinder concernCategoryColumn = new SqlColumnBinder("ConcernCategory");
      protected SqlColumnBinder concernTextColumn = new SqlColumnBinder("ConcernText");
      protected SqlColumnBinder submittedDateColumn = new SqlColumnBinder("SubmittedDate");

      protected override ReportedExtensionQnAItem Bind()
      {
        ReportedExtensionQnAItem extensionQnAitem = new ReportedExtensionQnAItem();
        extensionQnAitem.Id = this.idColumn.GetInt64((IDataReader) this.Reader);
        extensionQnAitem.ParentId = this.parentIdColumn.GetInt64((IDataReader) this.Reader);
        extensionQnAitem.Text = this.qnATextColumn.GetString((IDataReader) this.Reader, true);
        extensionQnAitem.ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader);
        extensionQnAitem.CreatedDate = this.createdDateColumn.GetDateTime((IDataReader) this.Reader);
        extensionQnAitem.UpdatedDate = this.updatedDateColumn.GetDateTime((IDataReader) this.Reader);
        extensionQnAitem.IsPublisherCreated = this.isPublisherCreatedColumn.GetBoolean((IDataReader) this.Reader);
        extensionQnAitem.IsQuestion = this.isQuestionColumn.GetBoolean((IDataReader) this.Reader);
        extensionQnAitem.UserId = this.userIdColumn.GetGuid((IDataReader) this.Reader);
        extensionQnAitem.ExtensionVersion = this.extensionVersionColumn.GetString((IDataReader) this.Reader, false);
        extensionQnAitem.IsDeleted = this.isDeletedColumn.GetBoolean((IDataReader) this.Reader);
        extensionQnAitem.ReportedBy = this.reportedByColumn.GetGuid((IDataReader) this.Reader);
        extensionQnAitem.ConcernCategory = (ConcernCategory) this.concernCategoryColumn.GetInt32((IDataReader) this.Reader);
        extensionQnAitem.ConcernText = this.concernTextColumn.GetString((IDataReader) this.Reader, false);
        extensionQnAitem.ConcernSubmittedDate = this.submittedDateColumn.GetDateTime((IDataReader) this.Reader);
        return extensionQnAitem;
      }
    }
  }
}
