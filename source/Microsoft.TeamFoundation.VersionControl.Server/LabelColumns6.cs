// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelColumns6
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class LabelColumns6 : LabelColumns
  {
    public LabelColumns6(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override VersionControlLabel Bind() => new VersionControlLabel()
    {
      Name = this.LabelName.GetString((IDataReader) this.Reader, false),
      ScopePair = this.GetItemPathPair(this.ServerItem.GetServerItem(this.Reader, false)),
      Comment = this.Comment.GetString((IDataReader) this.Reader, false),
      ownerId = this.OwnerId.GetGuid((IDataReader) this.Reader),
      LastModifiedDate = this.LastModified.GetDateTime((IDataReader) this.Reader),
      LabelId = this.LabelId.GetInt32((IDataReader) this.Reader)
    };
  }
}
