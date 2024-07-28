// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.QnAComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class QnAComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private const string s_area = "QnAComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<QnAComponent>(0, true),
      (IComponentCreator) new ComponentCreator<QnAComponent1>(1),
      (IComponentCreator) new ComponentCreator<QnAComponent2>(2),
      (IComponentCreator) new ComponentCreator<QnAComponent3>(3),
      (IComponentCreator) new ComponentCreator<QnAComponent4>(4),
      (IComponentCreator) new ComponentCreator<QnAComponent5>(5)
    }, "QnA");

    static QnAComponent()
    {
      QnAComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      QnAComponent.s_sqlExceptionFactories.Add(270021, new SqlExceptionFactory(typeof (QnAItemDoesNotExistException)));
      QnAComponent.s_sqlExceptionFactories.Add(270022, new SqlExceptionFactory(typeof (QnAItemAlreadyReportedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorNumber, ex, sqlError) => (Exception) new QnAItemAlreadyReportedException(GalleryWebApiResources.QnAItemAlreadyReportedException()))));
      QnAComponent.s_sqlExceptionFactories.Add(270023, new SqlExceptionFactory(typeof (QnAQuestionIdNotSpecifiedException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) QnAComponent.s_sqlExceptionFactories;

    protected override string TraceArea => nameof (QnAComponent);

    internal class ExtensionQnAItemBinder : ObjectBinder<ExtensionQnAItem>
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

      protected override ExtensionQnAItem Bind() => new ExtensionQnAItem()
      {
        Id = this.idColumn.GetInt64((IDataReader) this.Reader),
        ParentId = this.parentIdColumn.GetInt64((IDataReader) this.Reader),
        Text = this.qnATextColumn.GetString((IDataReader) this.Reader, true),
        ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader),
        CreatedDate = this.createdDateColumn.GetDateTime((IDataReader) this.Reader),
        UpdatedDate = this.updatedDateColumn.GetDateTime((IDataReader) this.Reader),
        IsPublisherCreated = this.isPublisherCreatedColumn.GetBoolean((IDataReader) this.Reader),
        IsQuestion = this.isQuestionColumn.GetBoolean((IDataReader) this.Reader),
        UserId = this.userIdColumn.GetGuid((IDataReader) this.Reader),
        ExtensionVersion = this.extensionVersionColumn.GetString((IDataReader) this.Reader, false),
        IsDeleted = this.isDeletedColumn.GetBoolean((IDataReader) this.Reader)
      };
    }

    internal class QnAMetaDataBinder : ObjectBinder<bool>
    {
      protected SqlColumnBinder hasMoreQuestionsColumn = new SqlColumnBinder("HasMoreQuestions");

      protected override bool Bind() => this.hasMoreQuestionsColumn.GetBoolean((IDataReader) this.Reader);
    }
  }
}
