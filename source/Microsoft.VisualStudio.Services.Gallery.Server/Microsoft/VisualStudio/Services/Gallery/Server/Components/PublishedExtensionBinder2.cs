// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionBinder2
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionBinder2 : PublishedExtensionBinder
  {
    protected SqlColumnBinder publisherFlagsColumn = new SqlColumnBinder("PublisherFlags");
    protected SqlColumnBinder publishedDateColumn = new SqlColumnBinder("PublishedDate");
    protected SqlColumnBinder releaseDateColumn = new SqlColumnBinder("ReleaseDate");

    protected override PublishedExtension Bind() => new PublishedExtension()
    {
      Publisher = new PublisherFacts()
      {
        PublisherId = this.publisherIdColumn.GetGuid((IDataReader) this.Reader),
        PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, false),
        DisplayName = this.publisherDisplayNameColumn.GetString((IDataReader) this.Reader, false),
        Flags = (PublisherFlags) this.publisherFlagsColumn.GetInt32((IDataReader) this.Reader)
      },
      ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader),
      ExtensionName = this.extensionNameColumn.GetString((IDataReader) this.Reader, false),
      DisplayName = this.displayNameColumn.GetString((IDataReader) this.Reader, false),
      Flags = (PublishedExtensionFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader),
      LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader),
      ShortDescription = this.shortDescriptionColumn.GetString((IDataReader) this.Reader, true),
      LongDescription = this.longDescriptionColumn.GetString((IDataReader) this.Reader, true),
      PublishedDate = this.publishedDateColumn.GetDateTime((IDataReader) this.Reader),
      ReleaseDate = this.releaseDateColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
