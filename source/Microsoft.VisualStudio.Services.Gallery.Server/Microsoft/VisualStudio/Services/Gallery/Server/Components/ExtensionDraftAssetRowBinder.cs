// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDraftAssetRowBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDraftAssetRowBinder : ObjectBinder<ExtensionDraftAssetRow>
  {
    protected SqlColumnBinder draftIdColumn = new SqlColumnBinder("draftId");
    protected SqlColumnBinder assetTypeColumn = new SqlColumnBinder("AssetType");
    protected SqlColumnBinder isOldAssetColumn = new SqlColumnBinder("IsOldAsset");
    protected SqlColumnBinder isPayloadAssetColumn = new SqlColumnBinder("IsPayloadAsset");
    protected SqlColumnBinder contentTypeColumn = new SqlColumnBinder("ContentType");
    protected SqlColumnBinder fileIdColumn = new SqlColumnBinder("FileId");

    protected override ExtensionDraftAssetRow Bind()
    {
      AssetInfo assetInfo = new AssetInfo(this.assetTypeColumn.GetString((IDataReader) this.Reader, false));
      return new ExtensionDraftAssetRow()
      {
        DraftId = this.draftIdColumn.GetGuid((IDataReader) this.Reader),
        AssetType = assetInfo.AssetType,
        Language = assetInfo.Language,
        IsOldAsset = this.isOldAssetColumn.GetBoolean((IDataReader) this.Reader),
        IsPayloadAsset = this.isPayloadAssetColumn.GetBoolean((IDataReader) this.Reader),
        ContentType = this.contentTypeColumn.GetString((IDataReader) this.Reader, true),
        FileId = this.fileIdColumn.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
