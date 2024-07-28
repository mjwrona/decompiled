// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ElasticPoolComponent
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
  internal class ElasticPoolComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ElasticPoolComponent>(4),
      (IComponentCreator) new ComponentCreator<ElasticPoolComponent>(5),
      (IComponentCreator) new ComponentCreator<ElasticPoolComponent6>(6)
    }, "DistributedTaskElasticPool", "DistributedTask");

    public ElasticPoolComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public virtual async Task<ElasticPool> AddElasticPoolAsync(ElasticPool ep)
    {
      ElasticPoolComponent elasticPoolComponent = this;
      ElasticPool elasticPool;
      using (elasticPoolComponent.TraceScope(nameof (AddElasticPoolAsync)))
      {
        elasticPoolComponent.PrepareStoredProcedure("Task.prc_AddElasticPool");
        elasticPoolComponent.BindInt("@poolId", ep.PoolId);
        elasticPoolComponent.BindGuid("@serviceEndpointId", ep.ServiceEndpointId);
        elasticPoolComponent.BindGuid("@serviceEndpointScope", ep.ServiceEndpointScope);
        elasticPoolComponent.BindString("@azureId", ep.AzureId, 320, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        elasticPoolComponent.BindInt("@maxCapacity", ep.MaxCapacity);
        elasticPoolComponent.BindInt("@desiredIdle", ep.DesiredIdle);
        elasticPoolComponent.BindBoolean("@recycleAfterEachUse", ep.RecycleAfterEachUse);
        elasticPoolComponent.BindByte("@osType", (byte) ep.OsType);
        elasticPoolComponent.BindByte("@state", (byte) ep.State);
        elasticPoolComponent.BindNullableDateTime2("@offlineSince", ep.OfflineSince);
        elasticPoolComponent.BindInt("@desiredSize", ep.DesiredSize);
        elasticPoolComponent.BindInt("@sizingAttempts", ep.SizingAttempts);
        elasticPoolComponent.BindBoolean("@agentInteractiveUI", ep.AgentInteractiveUI);
        elasticPoolComponent.BindInt("@timeToLiveMinutes", ep.TimeToLiveMinutes);
        if (elasticPoolComponent.Version >= 5)
          elasticPoolComponent.BindInt("@maxSavedNodeCount", ep.MaxSavedNodeCount);
        else
          elasticPoolComponent.BindBoolean("@preserveFailures", false);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticPoolComponent.ExecuteReaderAsync(), elasticPoolComponent.ProcedureName, elasticPoolComponent.RequestContext))
        {
          resultCollection.AddBinder<ElasticPool>((ObjectBinder<ElasticPool>) elasticPoolComponent.GetElasticPoolBinder());
          elasticPool = resultCollection.GetCurrent<ElasticPool>().Items.SingleOrDefault<ElasticPool>();
        }
      }
      return elasticPool;
    }

    public virtual async Task<ElasticPool> UpdateElasticPoolAsync(NullableElasticPool pool)
    {
      ElasticPoolComponent elasticPoolComponent1 = this;
      ElasticPool elasticPool;
      using (elasticPoolComponent1.TraceScope(nameof (UpdateElasticPoolAsync)))
      {
        elasticPoolComponent1.PrepareStoredProcedure("Task.prc_UpdateElasticPool");
        elasticPoolComponent1.BindInt("@poolId", pool.PoolId);
        elasticPoolComponent1.BindNullableInt("@maxCapacity", pool.MaxCapacity);
        elasticPoolComponent1.BindNullableInt("@desiredIdle", pool.DesiredIdle);
        elasticPoolComponent1.BindNullableBoolean("@recycleAfterEachUse", pool.RecycleAfterEachUse);
        elasticPoolComponent1.BindNullableGuid("@serviceEndpointId", pool.ServiceEndpointId);
        elasticPoolComponent1.BindNullableGuid("@serviceEndpointScope", pool.ServiceEndpointScope);
        elasticPoolComponent1.BindString("@azureId", pool.AzureId, 320, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        ElasticPoolComponent elasticPoolComponent2 = elasticPoolComponent1;
        OperatingSystemType? osType = pool.OsType;
        byte? parameterValue1 = osType.HasValue ? new byte?((byte) osType.GetValueOrDefault()) : new byte?();
        elasticPoolComponent2.BindNullableByte("@osType", parameterValue1);
        ElasticPoolComponent elasticPoolComponent3 = elasticPoolComponent1;
        ElasticPoolState? state = pool.State;
        byte? parameterValue2 = state.HasValue ? new byte?((byte) state.GetValueOrDefault()) : new byte?();
        elasticPoolComponent3.BindNullableByte("@state", parameterValue2);
        elasticPoolComponent1.BindNullableDateTime2("@offlineSince", pool.OfflineSince);
        elasticPoolComponent1.BindNullableInt("@desiredSize", pool.DesiredSize);
        elasticPoolComponent1.BindNullableInt("@sizingAttempts", pool.SizingAttempts);
        elasticPoolComponent1.BindNullableBoolean("@agentInteractiveUI", pool.AgentInteractiveUI);
        elasticPoolComponent1.BindNullableInt("@timeToLiveMinutes", pool.TimeToLiveMinutes);
        if (elasticPoolComponent1.Version >= 5)
          elasticPoolComponent1.BindNullableInt("@maxSavedNodeCount", pool.MaxSavedNodeCount);
        else
          elasticPoolComponent1.BindNullableBoolean("@preserveFailures", new bool?());
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticPoolComponent1.ExecuteReaderAsync(), elasticPoolComponent1.ProcedureName, elasticPoolComponent1.RequestContext))
        {
          resultCollection.AddBinder<ElasticPool>((ObjectBinder<ElasticPool>) elasticPoolComponent1.GetElasticPoolBinder());
          elasticPool = resultCollection.GetCurrent<ElasticPool>().Items.SingleOrDefault<ElasticPool>();
        }
      }
      return elasticPool;
    }

    public virtual async Task DeleteElasticPoolAsync(int poolId)
    {
      ElasticPoolComponent elasticPoolComponent = this;
      using (elasticPoolComponent.TraceScope(nameof (DeleteElasticPoolAsync)))
      {
        elasticPoolComponent.PrepareStoredProcedure("Task.prc_DeleteElasticPool");
        elasticPoolComponent.BindInt("@poolId", poolId);
        int num = await elasticPoolComponent.ExecuteNonQueryAsync();
      }
    }

    public virtual async Task<ElasticPool> GetElasticPoolAsync(int poolId)
    {
      ElasticPoolComponent elasticPoolComponent = this;
      ElasticPool elasticPoolAsync;
      using (elasticPoolComponent.TraceScope(nameof (GetElasticPoolAsync)))
      {
        elasticPoolComponent.PrepareStoredProcedure("Task.prc_GetElasticPool");
        elasticPoolComponent.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticPoolComponent.ExecuteReaderAsync(), elasticPoolComponent.ProcedureName, elasticPoolComponent.RequestContext))
        {
          resultCollection.AddBinder<ElasticPool>((ObjectBinder<ElasticPool>) elasticPoolComponent.GetElasticPoolBinder());
          elasticPoolAsync = resultCollection.GetCurrent<ElasticPool>().Items.SingleOrDefault<ElasticPool>();
        }
      }
      return elasticPoolAsync;
    }

    public virtual async Task<IReadOnlyList<ElasticPool>> GetElasticPoolsAsync()
    {
      ElasticPoolComponent elasticPoolComponent = this;
      IReadOnlyList<ElasticPool> items;
      using (elasticPoolComponent.TraceScope(nameof (GetElasticPoolsAsync)))
      {
        elasticPoolComponent.PrepareStoredProcedure("Task.prc_GetElasticPools");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticPoolComponent.ExecuteReaderAsync(), elasticPoolComponent.ProcedureName, elasticPoolComponent.RequestContext))
        {
          resultCollection.AddBinder<ElasticPool>((ObjectBinder<ElasticPool>) elasticPoolComponent.GetElasticPoolBinder());
          items = (IReadOnlyList<ElasticPool>) resultCollection.GetCurrent<ElasticPool>().Items;
        }
      }
      return items;
    }

    public virtual async Task<IReadOnlyList<ElasticPool>> GetElasticPoolsByTypeAsync(
      OrchestrationType type)
    {
      IReadOnlyList<ElasticPool> elasticPoolsAsync = await this.GetElasticPoolsAsync();
      return elasticPoolsAsync == null || !elasticPoolsAsync.Any<ElasticPool>() ? (IReadOnlyList<ElasticPool>) new List<ElasticPool>() : (IReadOnlyList<ElasticPool>) elasticPoolsAsync.Where<ElasticPool>((System.Func<ElasticPool, bool>) (pool => pool.OrchestrationType == type)).ToList<ElasticPool>();
    }

    public virtual async Task<ElasticPoolLog> AddElasticPoolLogAsync(ElasticPoolLog epl)
    {
      ElasticPoolComponent elasticPoolComponent = this;
      using (elasticPoolComponent.TraceScope(nameof (AddElasticPoolLogAsync)))
      {
        if (elasticPoolComponent.Version < 4)
          return epl;
        elasticPoolComponent.PrepareStoredProcedure("Task.prc_AddElasticPoolLog");
        elasticPoolComponent.BindInt("@poolId", epl.PoolId);
        elasticPoolComponent.BindByte("@level", (byte) epl.Level);
        elasticPoolComponent.BindByte("@operation", (byte) epl.Operation);
        elasticPoolComponent.BindString("@message", epl.Message, int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        elasticPoolComponent.BindDateTime2("@timestamp", epl.Timestamp);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticPoolComponent.ExecuteReaderAsync(), elasticPoolComponent.ProcedureName, elasticPoolComponent.RequestContext))
        {
          resultCollection.AddBinder<ElasticPoolLog>((ObjectBinder<ElasticPoolLog>) elasticPoolComponent.GetElasticPoolLogBinder());
          return resultCollection.GetCurrent<ElasticPoolLog>().Items.SingleOrDefault<ElasticPoolLog>();
        }
      }
    }

    public virtual async Task<IReadOnlyList<ElasticPoolLog>> GetElasticPoolLogsAsync(
      int poolId,
      int maxLogCount)
    {
      ElasticPoolComponent elasticPoolComponent = this;
      using (elasticPoolComponent.TraceScope(nameof (GetElasticPoolLogsAsync)))
      {
        if (elasticPoolComponent.Version < 4)
          return (IReadOnlyList<ElasticPoolLog>) new List<ElasticPoolLog>();
        elasticPoolComponent.PrepareStoredProcedure("Task.prc_GetElasticPoolLogs");
        elasticPoolComponent.BindInt("@poolId", poolId);
        elasticPoolComponent.BindInt("@maxLogCount", maxLogCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await elasticPoolComponent.ExecuteReaderAsync(), elasticPoolComponent.ProcedureName, elasticPoolComponent.RequestContext))
        {
          resultCollection.AddBinder<ElasticPoolLog>((ObjectBinder<ElasticPoolLog>) elasticPoolComponent.GetElasticPoolLogBinder());
          return (IReadOnlyList<ElasticPoolLog>) resultCollection.GetCurrent<ElasticPoolLog>().Items;
        }
      }
    }

    public virtual async Task DeleteElasticPoolLogsByTimeAsync(
      int poolId,
      DateTime lastLogTimestampToKeep)
    {
      ElasticPoolComponent elasticPoolComponent = this;
      using (elasticPoolComponent.TraceScope(nameof (DeleteElasticPoolLogsByTimeAsync)))
      {
        if (elasticPoolComponent.Version >= 4)
        {
          elasticPoolComponent.PrepareStoredProcedure("Task.prc_DeleteElasticPoolLogsByTime");
          elasticPoolComponent.BindInt("@poolId", poolId);
          elasticPoolComponent.BindDateTime2("@lastLogTimestampToKeep", lastLogTimestampToKeep);
          int num = await elasticPoolComponent.ExecuteNonQueryAsync();
        }
      }
    }

    protected virtual ElasticPoolBinder GetElasticPoolBinder() => this.Version <= 4 ? new ElasticPoolBinder() : (ElasticPoolBinder) new ElasticPoolBinder5();

    protected virtual ElasticPoolLogBinder GetElasticPoolLogBinder() => new ElasticPoolLogBinder();

    private IDisposable TraceScope([CallerMemberName] string method = null) => this.RequestContext.TraceScope(this.TraceLayer, method);

    private string TraceLayer => nameof (ElasticPoolComponent);
  }
}
