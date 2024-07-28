// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryMergesColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryMergesColumns : VersionControlObjectBinder<ItemMerge>
  {
    protected SqlColumnBinder sourceItemId = new SqlColumnBinder("SourceItemId");
    protected SqlColumnBinder sourceVersionFrom = new SqlColumnBinder("SourceVersionFrom");
    protected SqlColumnBinder sourceServerItem = new SqlColumnBinder("SourceServerItem");
    protected SqlColumnBinder targetItemId = new SqlColumnBinder("TargetItemId");
    protected SqlColumnBinder targetVersionFrom = new SqlColumnBinder("TargetVersionFrom");
    protected SqlColumnBinder targetServerItem = new SqlColumnBinder("TargetServerItem");

    public QueryMergesColumns()
    {
    }

    public QueryMergesColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override ItemMerge Bind() => new ItemMerge()
    {
      SourceItemId = this.sourceItemId.GetInt32((IDataReader) this.Reader),
      SourceVersionFrom = this.sourceVersionFrom.GetInt32((IDataReader) this.Reader),
      SourceItemPathPair = this.GetPreDataspaceItemPathPair(this.sourceServerItem.GetServerItem(this.Reader, false)),
      TargetItemId = this.targetItemId.GetInt32((IDataReader) this.Reader, 0),
      TargetVersionFrom = this.targetVersionFrom.GetInt32((IDataReader) this.Reader, 0),
      TargetItemPathPair = this.GetPreDataspaceItemPathPair(this.targetServerItem.GetServerItem(this.Reader, true))
    };
  }
}
