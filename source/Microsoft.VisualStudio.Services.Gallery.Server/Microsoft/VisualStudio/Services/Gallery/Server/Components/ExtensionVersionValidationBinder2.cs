// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionVersionValidationBinder2
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionVersionValidationBinder2 : ObjectBinder<ExtensionVersionValidation>
  {
    protected SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");
    protected SqlColumnBinder extensionNameColumn = new SqlColumnBinder("ExtensionName");
    protected SqlColumnBinder extensionIdColumn = new SqlColumnBinder("ExtensionId");
    protected SqlColumnBinder versionColumn = new SqlColumnBinder("Version");
    protected SqlColumnBinder validationIdColumn = new SqlColumnBinder("ValidationId");
    protected SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
    protected SqlColumnBinder versionDescriptionColumn = new SqlColumnBinder("VersionDescription");

    protected override ExtensionVersionValidation Bind() => new ExtensionVersionValidation()
    {
      PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, true),
      ExtensionName = this.extensionNameColumn.GetString((IDataReader) this.Reader, true),
      ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader, true),
      Version = this.versionColumn.GetString((IDataReader) this.Reader, false),
      ValidationId = this.validationIdColumn.GetGuid((IDataReader) this.Reader),
      LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader),
      VersionDescription = this.versionDescriptionColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
