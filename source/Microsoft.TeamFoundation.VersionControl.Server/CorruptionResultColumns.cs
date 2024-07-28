// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CorruptionResultColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CorruptionResultColumns : VersionControlObjectBinder<CorruptionResult>
  {
    protected SqlColumnBinder itemId = new SqlColumnBinder("ItemId");
    protected SqlColumnBinder itemDataspaceId = new SqlColumnBinder("ItemDataspaceId");
    protected SqlColumnBinder minVersionFrom = new SqlColumnBinder("MinVersionFrom");
    protected SqlColumnBinder serverPath = new SqlColumnBinder("ServerPath");

    public CorruptionResultColumns()
    {
    }

    public CorruptionResultColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override CorruptionResult Bind() => new CorruptionResult()
    {
      ItemId = this.itemId.GetInt32((IDataReader) this.Reader),
      ItemDataspaceId = this.itemDataspaceId.GetInt32((IDataReader) this.Reader),
      ServerPath = this.serverPath.GetServerItem(this.Reader, false),
      MinVersionFrom = this.minVersionFrom.GetInt32((IDataReader) this.Reader, 0)
    };
  }
}
