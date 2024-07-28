// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryPendingChangesForCheckinBinder15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryPendingChangesForCheckinBinder15 : QueryPendingChangesForCheckinBinder
  {
    public QueryPendingChangesForCheckinBinder15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override PendingChangeLight Bind() => new PendingChangeLight()
    {
      ItemPathPair = this.GetItemPathPair(this.serverItem.GetServerItem(this.Reader, false)),
      SourceItemPathPair = this.GetItemPathPair(this.existingServerItem.GetServerItem(this.Reader, true)),
      ChangeType = VersionControlSqlResourceComponent.GetChangeType(this.command.GetInt32((IDataReader) this.Reader, 0)),
      ItemType = (ItemType) this.itemType.GetByte((IDataReader) this.Reader),
      HasContentChange = this.hasContentChange.GetBoolean((IDataReader) this.Reader)
    };
  }
}
