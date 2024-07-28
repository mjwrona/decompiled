// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionUpdateBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionUpdateBinder : ObjectBinder<PublishedExtensionUpdate>
  {
    protected SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");
    protected SqlColumnBinder extensionIdColumn = new SqlColumnBinder("ExtensionId");
    protected SqlColumnBinder extensionNameColumn = new SqlColumnBinder("ExtensionName");

    protected override PublishedExtensionUpdate Bind() => new PublishedExtensionUpdate()
    {
      PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, false),
      ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader, false),
      ExtensionName = this.extensionNameColumn.GetString((IDataReader) this.Reader, false)
    };
  }
}
