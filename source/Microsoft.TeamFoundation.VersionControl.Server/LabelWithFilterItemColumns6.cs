// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelWithFilterItemColumns6
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class LabelWithFilterItemColumns6 : LabelWithFilterItemColumns
  {
    protected SqlColumnBinder propertyDataspaceId = new SqlColumnBinder("PropertyDataspaceId");

    public LabelWithFilterItemColumns6(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override VersionControlLabel Bind()
    {
      VersionControlLabel versionControlLabel = new VersionControlLabel();
      versionControlLabel.Name = this.LabelName.GetString((IDataReader) this.Reader, false);
      versionControlLabel.ScopePair = this.GetItemPathPair(this.ServerItem.GetServerItem(this.Reader, false));
      versionControlLabel.Comment = this.Comment.GetString((IDataReader) this.Reader, false);
      versionControlLabel.ownerId = this.OwnerId.GetGuid((IDataReader) this.Reader);
      versionControlLabel.LastModifiedDate = this.LastModified.GetDateTime((IDataReader) this.Reader);
      versionControlLabel.LabelId = this.LabelId.GetInt32((IDataReader) this.Reader);
      Item obj = new Item()
      {
        ChangesetId = this.VersionFrom.GetInt32((IDataReader) this.Reader),
        DeletionId = this.DeletionId.GetInt32((IDataReader) this.Reader),
        ItemId = this.ItemId.GetInt32((IDataReader) this.Reader),
        ItemDataspaceId = this.GetDataspaceIdentifier(this.propertyDataspaceId.GetInt32((IDataReader) this.Reader)),
        Encoding = this.Encoding.GetInt32((IDataReader) this.Reader)
      };
      obj.ItemType = VersionedItemComponent.EncodingToItemType(obj.Encoding);
      obj.FileLength = this.FileLength.GetInt64((IDataReader) this.Reader, 0L);
      obj.HashValue = VersionControlObjectBinder<VersionControlLabel>.NormalizeHashValue(this.HashValue.GetBytes((IDataReader) this.Reader, true));
      obj.CheckinDate = this.CheckinDate.GetDateTime((IDataReader) this.Reader);
      obj.fileId = this.FileId.GetInt32((IDataReader) this.Reader, 0);
      versionControlLabel.filterItem = obj;
      return versionControlLabel;
    }
  }
}
