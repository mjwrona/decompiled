// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ChangeColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ChangeColumns : VersionControlObjectBinder<Change>
  {
    protected SqlColumnBinder versionFrom = new SqlColumnBinder("VersionFrom");
    protected SqlColumnBinder command = new SqlColumnBinder("Command");
    protected SqlColumnBinder contentId = new SqlColumnBinder("ContentId");
    protected SqlColumnBinder fullPath = new SqlColumnBinder("FullPath");
    protected SqlColumnBinder itemId = new SqlColumnBinder("ItemId");
    protected SqlColumnBinder deletionId = new SqlColumnBinder("DeletionId");
    protected SqlColumnBinder itemType = new SqlColumnBinder("ItemType");
    protected SqlColumnBinder encoding = new SqlColumnBinder("Encoding");
    protected SqlColumnBinder fileLength = new SqlColumnBinder("FileLength");
    protected SqlColumnBinder hashValue = new SqlColumnBinder("HashValue");
    protected SqlColumnBinder fileId = new SqlColumnBinder("FileId");

    public ChangeColumns()
    {
    }

    public ChangeColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override Change Bind()
    {
      Change change = new Change()
      {
        Item = {
          ChangesetId = this.versionFrom.GetInt32((IDataReader) this.Reader)
        },
        ChangeType = VersionControlSqlResourceComponent.GetChangeType((int) this.command.GetInt16((IDataReader) this.Reader))
      };
      change.Item.DeletionId = this.deletionId.GetInt32((IDataReader) this.Reader);
      change.Item.ItemId = this.itemId.GetInt32((IDataReader) this.Reader);
      change.Item.ItemPathPair = this.GetPreDataspaceItemPathPair(this.fullPath.GetServerItem(this.Reader, false));
      change.Item.ItemType = (ItemType) this.itemType.GetByte((IDataReader) this.Reader);
      change.Item.Encoding = this.encoding.GetInt32((IDataReader) this.Reader);
      change.Item.FileLength = this.fileLength.GetInt64((IDataReader) this.Reader, 0L);
      change.Item.HashValue = VersionControlObjectBinder<Change>.NormalizeHashValue(this.hashValue.GetBytes((IDataReader) this.Reader, true));
      change.Item.fileId = this.fileId.GetInt32((IDataReader) this.Reader, 0);
      return change;
    }
  }
}
