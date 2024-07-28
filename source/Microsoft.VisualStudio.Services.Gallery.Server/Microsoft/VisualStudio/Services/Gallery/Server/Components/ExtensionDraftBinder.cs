// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDraftBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDraftBinder : ObjectBinder<ExtensionDraft>
  {
    protected SqlColumnBinder draftIdColumn = new SqlColumnBinder("draftId");
    protected SqlColumnBinder extensionDeploymentTypeColumn = new SqlColumnBinder("ExtensionDeploymentType");
    protected SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");
    protected SqlColumnBinder extensionNameColumn = new SqlColumnBinder("ExtensionName");
    protected SqlColumnBinder extensionIdColumn = new SqlColumnBinder("ExtensionId");
    protected SqlColumnBinder userIdColumn = new SqlColumnBinder("UserId");
    protected SqlColumnBinder payloadFileNameColumn = new SqlColumnBinder("PayloadFileName");
    protected SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
    protected SqlColumnBinder createdDateColumn = new SqlColumnBinder("CreatedDate");
    protected SqlColumnBinder draftStateColumn = new SqlColumnBinder("DraftState");
    protected SqlColumnBinder productColumn = new SqlColumnBinder("Product");
    protected SqlColumnBinder editReferenceDateColumn = new SqlColumnBinder("EditReferenceDate");

    protected override ExtensionDraft Bind() => new ExtensionDraft()
    {
      Id = this.draftIdColumn.GetGuid((IDataReader) this.Reader),
      PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, false),
      ExtensionName = this.extensionNameColumn.GetString((IDataReader) this.Reader, true),
      ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader, true),
      UserId = this.userIdColumn.GetGuid((IDataReader) this.Reader),
      LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader),
      CreatedDate = this.createdDateColumn.GetDateTime((IDataReader) this.Reader),
      DraftState = (DraftStateType) this.draftStateColumn.GetInt32((IDataReader) this.Reader),
      EditReferenceDate = this.editReferenceDateColumn.GetDateTime((IDataReader) this.Reader),
      Product = this.productColumn.GetString((IDataReader) this.Reader, false),
      Payload = new ExtensionPayload()
      {
        Type = (ExtensionDeploymentTechnology) this.extensionDeploymentTypeColumn.GetInt32((IDataReader) this.Reader),
        FileName = this.payloadFileNameColumn.GetString((IDataReader) this.Reader, true)
      }
    };
  }
}
