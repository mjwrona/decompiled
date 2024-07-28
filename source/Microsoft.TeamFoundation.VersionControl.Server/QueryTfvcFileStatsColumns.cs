// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryTfvcFileStatsColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryTfvcFileStatsColumns : VersionControlObjectBinder<TfvcFileStats>
  {
    private SqlColumnBinder changesetId = new SqlColumnBinder("ChangesetId");
    private SqlColumnBinder fileCountTotal = new SqlColumnBinder("FileCountTotal");

    public QueryTfvcFileStatsColumns()
    {
    }

    public QueryTfvcFileStatsColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override TfvcFileStats Bind() => new TfvcFileStats()
    {
      ChangesetId = this.changesetId.GetInt32((IDataReader) this.Reader),
      FileCountTotal = this.fileCountTotal.GetInt64((IDataReader) this.Reader)
    };
  }
}
