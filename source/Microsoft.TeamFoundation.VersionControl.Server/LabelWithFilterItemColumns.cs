// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelWithFilterItemColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class LabelWithFilterItemColumns : LabelColumns
  {
    protected SqlColumnBinder VersionFrom = new SqlColumnBinder(nameof (VersionFrom));
    protected SqlColumnBinder DeletionId = new SqlColumnBinder(nameof (DeletionId));
    protected SqlColumnBinder ItemId = new SqlColumnBinder(nameof (ItemId));
    protected SqlColumnBinder Encoding = new SqlColumnBinder(nameof (Encoding));
    protected SqlColumnBinder FileLength = new SqlColumnBinder(nameof (FileLength));
    protected SqlColumnBinder HashValue = new SqlColumnBinder(nameof (HashValue));
    protected SqlColumnBinder CheckinDate = new SqlColumnBinder(nameof (CheckinDate));
    protected SqlColumnBinder FileId = new SqlColumnBinder(nameof (FileId));

    public LabelWithFilterItemColumns()
    {
    }

    public LabelWithFilterItemColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override VersionControlLabel Bind()
    {
      VersionControlLabel versionControlLabel = base.Bind();
      Item obj = new Item()
      {
        ChangesetId = this.VersionFrom.GetInt32((IDataReader) this.Reader),
        DeletionId = this.DeletionId.GetInt32((IDataReader) this.Reader),
        ItemId = this.ItemId.GetInt32((IDataReader) this.Reader),
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
