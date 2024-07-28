// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionFileBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionFileBinder : ObjectBinder<ExtensionFile>
  {
    protected SqlColumnBinder referenceIdColumn = new SqlColumnBinder("ReferenceId");
    protected SqlColumnBinder versionColumn = new SqlColumnBinder("Version");
    protected SqlColumnBinder assetTypeColumn = new SqlColumnBinder("AssetType");
    protected SqlColumnBinder contentTypeColumn = new SqlColumnBinder("ContentType");
    protected SqlColumnBinder fileIdColumn = new SqlColumnBinder("FileId");
    protected SqlColumnBinder shortDescriptionColumn = new SqlColumnBinder("ShortDescription");

    protected override ExtensionFile Bind()
    {
      AssetInfo assetInfo = new AssetInfo(this.assetTypeColumn.GetString((IDataReader) this.Reader, false));
      return new ExtensionFile()
      {
        ReferenceId = this.referenceIdColumn.GetGuid((IDataReader) this.Reader),
        Version = this.versionColumn.GetString((IDataReader) this.Reader, false),
        AssetType = assetInfo.AssetType,
        Language = assetInfo.Language,
        ContentType = this.contentTypeColumn.GetString((IDataReader) this.Reader, false),
        FileId = this.fileIdColumn.GetInt32((IDataReader) this.Reader),
        ShortDescription = this.shortDescriptionColumn.GetString((IDataReader) this.Reader, true)
      };
    }
  }
}
