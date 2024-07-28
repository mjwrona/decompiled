// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ElasticPoolComponent6
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class ElasticPoolComponent6 : ElasticPoolComponent
  {
    public override async Task<ElasticPool> AddElasticPoolAsync(ElasticPool ep)
    {
      ElasticPoolComponent6 elasticPoolComponent6 = this;
      ElasticPool elasticPool;
      using (elasticPoolComponent6.TraceScope(nameof (AddElasticPoolAsync)))
      {
        elasticPoolComponent6.PrepareStoredProcedure("Task.prc_AddElasticPool");
        elasticPoolComponent6.BindInt("@poolId", ep.PoolId);
        elasticPoolComponent6.BindGuid("@serviceEndpointId", ep.ServiceEndpointId);
        elasticPoolComponent6.BindGuid("@serviceEndpointScope", ep.ServiceEndpointScope);
        elasticPoolComponent6.BindString("@azureId", ep.AzureId, 320, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        elasticPoolComponent6.BindInt("@maxCapacity", ep.MaxCapacity);
        elasticPoolComponent6.BindInt("@desiredIdle", ep.DesiredIdle);
        elasticPoolComponent6.BindBoolean("@recycleAfterEachUse", ep.RecycleAfterEachUse);
        elasticPoolComponent6.BindByte("@osType", (byte) ep.OsType);
        elasticPoolComponent6.BindByte("@state", (byte) ep.State);
        elasticPoolComponent6.BindNullableDateTime2("@offlineSince", ep.OfflineSince);
        elasticPoolComponent6.BindInt("@desiredSize", ep.DesiredSize);
        elasticPoolComponent6.BindInt("@sizingAttempts", ep.SizingAttempts);
        elasticPoolComponent6.BindBoolean("@agentInteractiveUI", ep.AgentInteractiveUI);
        elasticPoolComponent6.BindInt("@timeToLiveMinutes", ep.TimeToLiveMinutes);
        elasticPoolComponent6.BindNullableByte("@orchestrationType", new byte?((byte) ep.OrchestrationType));
        elasticPoolComponent6.BindInt("@maxSavedNodeCount", ep.MaxSavedNodeCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticPoolComponent6.ExecuteReaderAsync(), elasticPoolComponent6.ProcedureName, elasticPoolComponent6.RequestContext))
        {
          resultCollection.AddBinder<ElasticPool>((ObjectBinder<ElasticPool>) new ElasticPoolBinder6());
          elasticPool = resultCollection.GetCurrent<ElasticPool>().Items.SingleOrDefault<ElasticPool>();
        }
      }
      return elasticPool;
    }

    public override async Task<ElasticPool> UpdateElasticPoolAsync(NullableElasticPool pool)
    {
      ElasticPoolComponent6 elasticPoolComponent6_1 = this;
      ElasticPool elasticPool;
      using (elasticPoolComponent6_1.TraceScope(nameof (UpdateElasticPoolAsync)))
      {
        elasticPoolComponent6_1.PrepareStoredProcedure("Task.prc_UpdateElasticPool");
        elasticPoolComponent6_1.BindInt("@poolId", pool.PoolId);
        elasticPoolComponent6_1.BindNullableInt("@maxCapacity", pool.MaxCapacity);
        elasticPoolComponent6_1.BindNullableInt("@desiredIdle", pool.DesiredIdle);
        elasticPoolComponent6_1.BindNullableBoolean("@recycleAfterEachUse", pool.RecycleAfterEachUse);
        elasticPoolComponent6_1.BindNullableGuid("@serviceEndpointId", pool.ServiceEndpointId);
        elasticPoolComponent6_1.BindNullableGuid("@serviceEndpointScope", pool.ServiceEndpointScope);
        elasticPoolComponent6_1.BindString("@azureId", pool.AzureId, 320, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        ElasticPoolComponent6 elasticPoolComponent6_2 = elasticPoolComponent6_1;
        OperatingSystemType? osType = pool.OsType;
        byte? parameterValue1 = osType.HasValue ? new byte?((byte) osType.GetValueOrDefault()) : new byte?();
        elasticPoolComponent6_2.BindNullableByte("@osType", parameterValue1);
        ElasticPoolComponent6 elasticPoolComponent6_3 = elasticPoolComponent6_1;
        ElasticPoolState? state = pool.State;
        byte? parameterValue2 = state.HasValue ? new byte?((byte) state.GetValueOrDefault()) : new byte?();
        elasticPoolComponent6_3.BindNullableByte("@state", parameterValue2);
        elasticPoolComponent6_1.BindNullableDateTime2("@offlineSince", pool.OfflineSince);
        elasticPoolComponent6_1.BindNullableInt("@desiredSize", pool.DesiredSize);
        elasticPoolComponent6_1.BindNullableInt("@sizingAttempts", pool.SizingAttempts);
        elasticPoolComponent6_1.BindNullableBoolean("@agentInteractiveUI", pool.AgentInteractiveUI);
        elasticPoolComponent6_1.BindNullableInt("@timeToLiveMinutes", pool.TimeToLiveMinutes);
        elasticPoolComponent6_1.BindNullableInt("@maxSavedNodeCount", pool.MaxSavedNodeCount);
        ElasticPoolComponent6 elasticPoolComponent6_4 = elasticPoolComponent6_1;
        OrchestrationType? orchestrationType = pool.OrchestrationType;
        byte? parameterValue3 = orchestrationType.HasValue ? new byte?((byte) orchestrationType.GetValueOrDefault()) : new byte?();
        elasticPoolComponent6_4.BindNullableByte("@orchestrationType", parameterValue3);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticPoolComponent6_1.ExecuteReaderAsync(), elasticPoolComponent6_1.ProcedureName, elasticPoolComponent6_1.RequestContext))
        {
          resultCollection.AddBinder<ElasticPool>((ObjectBinder<ElasticPool>) new ElasticPoolBinder6());
          elasticPool = resultCollection.GetCurrent<ElasticPool>().Items.SingleOrDefault<ElasticPool>();
        }
      }
      return elasticPool;
    }

    public override async Task<IReadOnlyList<ElasticPool>> GetElasticPoolsByTypeAsync(
      OrchestrationType type)
    {
      ElasticPoolComponent6 elasticPoolComponent6 = this;
      IReadOnlyList<ElasticPool> items;
      using (elasticPoolComponent6.TraceScope(nameof (GetElasticPoolsByTypeAsync)))
      {
        elasticPoolComponent6.PrepareStoredProcedure("Task.prc_GetElasticPoolsByType");
        elasticPoolComponent6.BindNullableByte("@type", new byte?((byte) type));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticPoolComponent6.ExecuteReaderAsync(), elasticPoolComponent6.ProcedureName, elasticPoolComponent6.RequestContext))
        {
          resultCollection.AddBinder<ElasticPool>((ObjectBinder<ElasticPool>) new ElasticPoolBinder6());
          items = (IReadOnlyList<ElasticPool>) resultCollection.GetCurrent<ElasticPool>().Items;
        }
      }
      return items;
    }

    public override async Task<ElasticPool> GetElasticPoolAsync(int poolId)
    {
      ElasticPoolComponent6 elasticPoolComponent6 = this;
      ElasticPool elasticPoolAsync;
      using (elasticPoolComponent6.TraceScope(nameof (GetElasticPoolAsync)))
      {
        elasticPoolComponent6.PrepareStoredProcedure("Task.prc_GetElasticPool");
        elasticPoolComponent6.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticPoolComponent6.ExecuteReaderAsync(), elasticPoolComponent6.ProcedureName, elasticPoolComponent6.RequestContext))
        {
          resultCollection.AddBinder<ElasticPool>((ObjectBinder<ElasticPool>) new ElasticPoolBinder6());
          elasticPoolAsync = resultCollection.GetCurrent<ElasticPool>().Items.SingleOrDefault<ElasticPool>();
        }
      }
      return elasticPoolAsync;
    }

    public override async Task<IReadOnlyList<ElasticPool>> GetElasticPoolsAsync()
    {
      ElasticPoolComponent6 elasticPoolComponent6 = this;
      IReadOnlyList<ElasticPool> items;
      using (elasticPoolComponent6.TraceScope(nameof (GetElasticPoolsAsync)))
      {
        elasticPoolComponent6.PrepareStoredProcedure("Task.prc_GetElasticPools");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticPoolComponent6.ExecuteReaderAsync(), elasticPoolComponent6.ProcedureName, elasticPoolComponent6.RequestContext))
        {
          resultCollection.AddBinder<ElasticPool>((ObjectBinder<ElasticPool>) new ElasticPoolBinder6());
          items = (IReadOnlyList<ElasticPool>) resultCollection.GetCurrent<ElasticPool>().Items;
        }
      }
      return items;
    }

    private IDisposable TraceScope([CallerMemberName] string method = null) => this.RequestContext.TraceScope(this.TraceLayer, method);

    private string TraceLayer => nameof (ElasticPoolComponent6);
  }
}
