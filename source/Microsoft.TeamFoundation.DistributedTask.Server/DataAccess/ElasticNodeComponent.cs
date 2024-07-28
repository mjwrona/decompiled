// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ElasticNodeComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class ElasticNodeComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<ElasticNodeComponent>(1),
      (IComponentCreator) new ComponentCreator<ElasticNodeComponent>(2)
    }, "DistributedTaskElasticNode", "DistributedTask");
    protected static readonly SqlMetaData[] typ_ElasticNodeTable = new SqlMetaData[10]
    {
      new SqlMetaData("PoolId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 100L),
      new SqlMetaData("State", SqlDbType.Int),
      new SqlMetaData("DesiredState", SqlDbType.Int),
      new SqlMetaData("AgentId", SqlDbType.Int),
      new SqlMetaData("AgentState", SqlDbType.Int),
      new SqlMetaData("ComputeId", SqlDbType.NVarChar, 100L),
      new SqlMetaData("ComputeState", SqlDbType.Int),
      new SqlMetaData("RequestId", SqlDbType.BigInt)
    };

    public ElasticNodeComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public virtual async Task<IReadOnlyList<ElasticNode>> AddElasticNodesAsync(
      int poolId,
      IEnumerable<ElasticNode> nodes)
    {
      ElasticNodeComponent elasticNodeComponent = this;
      IReadOnlyList<ElasticNode> items;
      using (elasticNodeComponent.TraceScope(nameof (AddElasticNodesAsync)))
      {
        elasticNodeComponent.PrepareStoredProcedure("Task.prc_AddElasticNodes");
        elasticNodeComponent.BindInt("@poolId", poolId);
        elasticNodeComponent.BindElasticNodeTable("@nodes", nodes);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticNodeComponent.ExecuteReaderAsync(), elasticNodeComponent.ProcedureName, elasticNodeComponent.RequestContext))
        {
          resultCollection.AddBinder<ElasticNode>((ObjectBinder<ElasticNode>) elasticNodeComponent.GetElasticNodeBinder());
          items = (IReadOnlyList<ElasticNode>) resultCollection.GetCurrent<ElasticNode>().Items;
        }
      }
      return items;
    }

    public virtual async Task DeleteElasticNodesAsync(int poolId, IEnumerable<ElasticNode> nodes)
    {
      ElasticNodeComponent component = this;
      using (component.TraceScope(nameof (DeleteElasticNodesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_DeleteElasticNodes");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@ids", nodes.Select<ElasticNode, int>((System.Func<ElasticNode, int>) (n => n.Id)));
        int num = await component.ExecuteNonQueryAsync();
      }
    }

    public virtual async Task<IReadOnlyList<ElasticNode>> GetElasticNodesAsync(
      int poolId,
      ElasticNodeState? state = null)
    {
      ElasticNodeComponent elasticNodeComponent1 = this;
      IReadOnlyList<ElasticNode> items;
      using (elasticNodeComponent1.TraceScope(nameof (GetElasticNodesAsync)))
      {
        elasticNodeComponent1.PrepareStoredProcedure("Task.prc_GetElasticNodes");
        elasticNodeComponent1.BindInt("@poolId", poolId);
        if (elasticNodeComponent1.Version >= 2)
        {
          ElasticNodeComponent elasticNodeComponent2 = elasticNodeComponent1;
          ElasticNodeState? nullable = state;
          int? parameterValue = nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?();
          elasticNodeComponent2.BindNullableInt("@state", parameterValue);
        }
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticNodeComponent1.ExecuteReaderAsync(), elasticNodeComponent1.ProcedureName, elasticNodeComponent1.RequestContext))
        {
          resultCollection.AddBinder<ElasticNode>((ObjectBinder<ElasticNode>) elasticNodeComponent1.GetElasticNodeBinder());
          items = (IReadOnlyList<ElasticNode>) resultCollection.GetCurrent<ElasticNode>().Items;
        }
      }
      return items;
    }

    public virtual async Task<IReadOnlyList<ElasticNode>> UpdateElasticNodesAsync(
      int poolId,
      IEnumerable<ElasticNode> nodes)
    {
      ElasticNodeComponent elasticNodeComponent = this;
      IReadOnlyList<ElasticNode> items;
      using (elasticNodeComponent.TraceScope(nameof (UpdateElasticNodesAsync)))
      {
        elasticNodeComponent.PrepareStoredProcedure("Task.prc_UpdateElasticNodes");
        elasticNodeComponent.BindInt("@poolId", poolId);
        elasticNodeComponent.BindElasticNodeTable("@nodes", nodes);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticNodeComponent.ExecuteReaderAsync(), elasticNodeComponent.ProcedureName, elasticNodeComponent.RequestContext))
        {
          resultCollection.AddBinder<ElasticNode>((ObjectBinder<ElasticNode>) elasticNodeComponent.GetElasticNodeBinder());
          items = (IReadOnlyList<ElasticNode>) resultCollection.GetCurrent<ElasticNode>().Items;
        }
      }
      return items;
    }

    public virtual async Task<IReadOnlyList<ElasticNode>> UpdateElasticNodesAsync(
      int poolId,
      IEnumerable<NullableElasticNode> nodes)
    {
      ElasticNodeComponent elasticNodeComponent = this;
      IReadOnlyList<ElasticNode> items;
      using (elasticNodeComponent.TraceScope(nameof (UpdateElasticNodesAsync)))
      {
        elasticNodeComponent.PrepareStoredProcedure("Task.prc_UpdateElasticNodes");
        elasticNodeComponent.BindInt("@poolId", poolId);
        elasticNodeComponent.BindNullableElasticNodeTable("@nodes", nodes);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticNodeComponent.ExecuteReaderAsync(), elasticNodeComponent.ProcedureName, elasticNodeComponent.RequestContext))
        {
          resultCollection.AddBinder<ElasticNode>((ObjectBinder<ElasticNode>) elasticNodeComponent.GetElasticNodeBinder());
          items = (IReadOnlyList<ElasticNode>) resultCollection.GetCurrent<ElasticNode>().Items;
        }
      }
      return items;
    }

    protected virtual ElasticNodeBinder GetElasticNodeBinder() => new ElasticNodeBinder();

    protected SqlParameter BindElasticNodeTable(string parameterName, IEnumerable<ElasticNode> rows)
    {
      rows = rows ?? Enumerable.Empty<ElasticNode>();
      System.Func<ElasticNode, SqlDataRecord> selector = (System.Func<ElasticNode, SqlDataRecord>) (node =>
      {
        SqlDataRecord record = new SqlDataRecord(ElasticNodeComponent.typ_ElasticNodeTable);
        record.SetInt32(0, node.PoolId);
        record.SetNullableInt32(1, new int?(node.Id));
        record.SetNullableString(2, node.Name);
        record.SetNullableInt32(3, new int?((int) node.State));
        record.SetNullableInt32(4, new int?((int) node.DesiredState));
        record.SetNullableInt32(5, node.AgentId);
        record.SetNullableInt32(6, new int?((int) node.AgentState));
        record.SetNullableString(7, node.ComputeId);
        record.SetNullableInt32(8, new int?((int) node.ComputeState));
        record.SetNullableInt64(9, node.RequestId);
        return record;
      });
      return this.BindTable(parameterName, "Task.typ_ElasticNodeTable", rows.Select<ElasticNode, SqlDataRecord>(selector));
    }

    protected SqlParameter BindNullableElasticNodeTable(
      string parameterName,
      IEnumerable<NullableElasticNode> rows)
    {
      rows = rows ?? Enumerable.Empty<NullableElasticNode>();
      System.Func<NullableElasticNode, SqlDataRecord> selector = (System.Func<NullableElasticNode, SqlDataRecord>) (node =>
      {
        SqlDataRecord record = new SqlDataRecord(ElasticNodeComponent.typ_ElasticNodeTable);
        record.SetInt32(0, node.PoolId);
        record.SetNullableInt32(1, new int?(node.Id));
        record.SetNullableString(2, node.Name);
        ElasticNodeState? nullable = node.State;
        record.SetNullableInt32(3, nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?());
        nullable = node.DesiredState;
        record.SetNullableInt32(4, nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?());
        record.SetNullableInt32(5, node.AgentId);
        ElasticAgentState? agentState = node.AgentState;
        record.SetNullableInt32(6, agentState.HasValue ? new int?((int) agentState.GetValueOrDefault()) : new int?());
        record.SetNullableString(7, node.ComputeId);
        ElasticComputeState? computeState = node.ComputeState;
        record.SetNullableInt32(8, computeState.HasValue ? new int?((int) computeState.GetValueOrDefault()) : new int?());
        record.SetNullableInt64(9, node.RequestId);
        return record;
      });
      return this.BindTable(parameterName, "Task.typ_ElasticNodeTable", rows.Select<NullableElasticNode, SqlDataRecord>(selector));
    }

    private IDisposable TraceScope([CallerMemberName] string method = null) => this.RequestContext.TraceScope(this.TraceLayer, method);

    private string TraceLayer => nameof (ElasticNodeComponent);
  }
}
