// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryMergesColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryMergesColumns15 : QueryMergesColumns
  {
    public QueryMergesColumns15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override ItemMerge Bind() => new ItemMerge()
    {
      SourceItemId = this.sourceItemId.GetInt32((IDataReader) this.Reader),
      SourceVersionFrom = this.sourceVersionFrom.GetInt32((IDataReader) this.Reader),
      SourceItemPathPair = this.GetItemPathPair(this.sourceServerItem.GetServerItem(this.Reader, false)),
      TargetItemId = this.targetItemId.GetInt32((IDataReader) this.Reader, 0),
      TargetVersionFrom = this.targetVersionFrom.GetInt32((IDataReader) this.Reader, 0),
      TargetItemPathPair = this.GetItemPathPair(this.targetServerItem.GetServerItem(this.Reader, true))
    };
  }
}
