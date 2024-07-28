// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionBinder3
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionBinder3 : PublishedExtensionBinder2
  {
    protected SqlColumnBinder domainColumn = new SqlColumnBinder("Domain");
    protected SqlColumnBinder isDomainVerifiedColumn = new SqlColumnBinder("IsDomainVerified");

    protected override PublishedExtension Bind()
    {
      bool? nullable1 = this.isDomainVerifiedColumn.ColumnExists((IDataReader) this.Reader) ? this.isDomainVerifiedColumn.GetNullableBoolean((IDataReader) this.Reader) : new bool?(false);
      PublishedExtension publishedExtension = new PublishedExtension();
      PublisherFacts publisherFacts = new PublisherFacts();
      publisherFacts.PublisherId = this.publisherIdColumn.GetGuid((IDataReader) this.Reader);
      publisherFacts.PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, false);
      publisherFacts.DisplayName = this.publisherDisplayNameColumn.GetString((IDataReader) this.Reader, false);
      publisherFacts.Flags = (PublisherFlags) this.publisherFlagsColumn.GetInt32((IDataReader) this.Reader);
      publisherFacts.Domain = this.domainColumn.ColumnExists((IDataReader) this.Reader) ? this.domainColumn.GetString((IDataReader) this.Reader, true) : (string) null;
      bool? nullable2 = nullable1;
      bool flag = true;
      publisherFacts.IsDomainVerified = nullable2.GetValueOrDefault() == flag & nullable2.HasValue;
      publishedExtension.Publisher = publisherFacts;
      publishedExtension.ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader);
      publishedExtension.ExtensionName = this.extensionNameColumn.GetString((IDataReader) this.Reader, false);
      publishedExtension.DisplayName = this.displayNameColumn.GetString((IDataReader) this.Reader, false);
      publishedExtension.Flags = (PublishedExtensionFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader);
      publishedExtension.LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader);
      publishedExtension.ShortDescription = this.shortDescriptionColumn.GetString((IDataReader) this.Reader, true);
      publishedExtension.LongDescription = this.longDescriptionColumn.GetString((IDataReader) this.Reader, true);
      publishedExtension.PublishedDate = this.publishedDateColumn.GetDateTime((IDataReader) this.Reader);
      publishedExtension.ReleaseDate = this.releaseDateColumn.GetDateTime((IDataReader) this.Reader);
      return publishedExtension;
    }
  }
}
