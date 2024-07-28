// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.PersistedStageGroupBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class PersistedStageGroupBinder : ObjectBinder<PersistedStageGroup>
  {
    private SqlColumnBinder m_stageId = new SqlColumnBinder("StageId");
    private SqlColumnBinder m_groupId = new SqlColumnBinder("GroupId");
    private SqlColumnBinder m_groupPath = new SqlColumnBinder("GroupPath");
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");

    protected override PersistedStageGroup Bind()
    {
      long int64_1 = this.m_stageId.GetInt64((IDataReader) this.Reader);
      long int64_2 = this.m_groupId.GetInt64((IDataReader) this.Reader);
      string str = this.m_groupPath.GetString((IDataReader) this.Reader, false);
      int int32 = this.m_buildId.GetInt32((IDataReader) this.Reader);
      return new PersistedStageGroup()
      {
        Id = int64_2,
        Path = str,
        StageId = int64_1,
        BuildId = int32
      };
    }
  }
}
