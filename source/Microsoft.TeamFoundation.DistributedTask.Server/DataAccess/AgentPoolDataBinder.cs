// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.AgentPoolDataBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class AgentPoolDataBinder : ObjectBinder<AgentPoolData>
  {
    private SqlColumnBinder poolId = new SqlColumnBinder("PoolId");
    private SqlColumnBinder poolName = new SqlColumnBinder("PoolName");
    private SqlColumnBinder poolType = new SqlColumnBinder("PoolType");
    private SqlColumnBinder isHosted = new SqlColumnBinder("IsHosted");
    private SqlColumnBinder poolOptions = new SqlColumnBinder("Options");
    private SqlColumnBinder lastModified = new SqlColumnBinder("LastModified");

    protected override AgentPoolData Bind() => new AgentPoolData()
    {
      PoolId = this.poolId.GetInt32((IDataReader) this.Reader),
      PoolName = this.poolName.GetString((IDataReader) this.Reader, false),
      PoolType = (TaskAgentPoolType) this.poolType.GetByte((IDataReader) this.Reader),
      IsHosted = this.isHosted.GetBoolean((IDataReader) this.Reader, false),
      PoolOptions = (TaskAgentPoolOptions) this.poolOptions.GetInt32((IDataReader) this.Reader),
      LastModified = this.lastModified.GetDateTime((IDataReader) this.Reader)
    };
  }
}
