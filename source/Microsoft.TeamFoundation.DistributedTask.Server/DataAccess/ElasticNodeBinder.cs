// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ElasticNodeBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class ElasticNodeBinder : ObjectBinder<ElasticNode>
  {
    protected override ElasticNode Bind() => new ElasticNode()
    {
      PoolId = ElasticNodeBinder.PoolId.GetInt32((IDataReader) this.Reader),
      Id = ElasticNodeBinder.Id.GetInt32((IDataReader) this.Reader),
      Name = ElasticNodeBinder.Name.GetString((IDataReader) this.Reader, true),
      State = (ElasticNodeState) ElasticNodeBinder.State.GetInt32((IDataReader) this.Reader),
      StateChangedOn = ElasticNodeBinder.StateChangedOn.GetDateTime((IDataReader) this.Reader),
      DesiredState = (ElasticNodeState) ElasticNodeBinder.DesiredState.GetInt32((IDataReader) this.Reader),
      AgentId = ElasticNodeBinder.AgentId.GetNullableInt32((IDataReader) this.Reader),
      AgentState = (ElasticAgentState) ElasticNodeBinder.AgentState.GetInt32((IDataReader) this.Reader),
      ComputeId = ElasticNodeBinder.ComputeId.GetString((IDataReader) this.Reader, true),
      ComputeState = (ElasticComputeState) ElasticNodeBinder.ComputeState.GetInt32((IDataReader) this.Reader),
      RequestId = ElasticNodeBinder.RequestId.GetNullableInt64((IDataReader) this.Reader)
    };

    protected static SqlColumnBinder PoolId { get; } = new SqlColumnBinder(nameof (PoolId));

    protected static SqlColumnBinder Id { get; } = new SqlColumnBinder(nameof (Id));

    protected static SqlColumnBinder Name { get; } = new SqlColumnBinder(nameof (Name));

    protected static SqlColumnBinder State { get; } = new SqlColumnBinder(nameof (State));

    protected static SqlColumnBinder StateChangedOn { get; } = new SqlColumnBinder(nameof (StateChangedOn));

    protected static SqlColumnBinder DesiredState { get; } = new SqlColumnBinder(nameof (DesiredState));

    protected static SqlColumnBinder AgentId { get; } = new SqlColumnBinder(nameof (AgentId));

    protected static SqlColumnBinder AgentState { get; } = new SqlColumnBinder(nameof (AgentState));

    protected static SqlColumnBinder ComputeId { get; } = new SqlColumnBinder(nameof (ComputeId));

    protected static SqlColumnBinder ComputeState { get; } = new SqlColumnBinder(nameof (ComputeState));

    protected static SqlColumnBinder RequestId { get; } = new SqlColumnBinder(nameof (RequestId));
  }
}
