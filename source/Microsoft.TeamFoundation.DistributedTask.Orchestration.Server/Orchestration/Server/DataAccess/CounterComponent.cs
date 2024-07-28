// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class CounterComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<CounterComponent>(1, true),
      (IComponentCreator) new ComponentCreator<CounterComponent2>(2),
      (IComponentCreator) new ComponentCreator<CounterComponent3>(3)
    }, "PipelineCounterService", "DistributedTaskOrchestration", 3);
    protected const int MaxPlanTypeStringLength = 260;
    protected const int MaxPrefixStringLength = 256;

    public CounterComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual async Task<List<CounterVariable>> GetAllCountersAsync(
      Guid scopeId,
      string planType,
      int definitionId)
    {
      CounterComponent component = this;
      List<CounterVariable> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAllCountersAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAllCounters");
        component.BindDefaultDataspaceId(scopeId);
        component.BindString("@planType", planType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindInt("@definitionId", definitionId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<CounterVariable>(component.GetCounterVariableBinder());
          items = resultCollection.GetCurrent<CounterVariable>().Items;
        }
      }
      return items;
    }

    public virtual async Task<CounterVariable> GetCounterAsync(
      Guid scopeId,
      string planType,
      int definitionId,
      string prefix)
    {
      CounterComponent component = this;
      CounterVariable counterAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetCounterAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetCounter");
        component.BindDefaultDataspaceId(scopeId);
        component.BindString("@planType", planType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindInt("@definitionId", definitionId);
        component.BindString("@prefix", prefix, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<CounterVariable>(component.GetCounterVariableBinder());
          counterAsync = resultCollection.GetCurrent<CounterVariable>().Items.SingleOrDefault<CounterVariable>();
        }
      }
      return counterAsync;
    }

    public virtual async Task<int> IncrementCounterAsync(
      Guid scopeId,
      string planType,
      int definitionId,
      Guid planId,
      string prefix,
      int seed)
    {
      CounterComponent component = this;
      int num;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (IncrementCounterAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_IncrementCounter");
        component.BindDefaultDataspaceId(scopeId);
        component.BindInt("@definitionId", definitionId);
        component.BindString("@planType", planType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindString("@prefix", prefix, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindInt("@seed", seed);
        num = (int) await component.ExecuteScalarAsync();
      }
      return num;
    }

    public virtual async Task RemoveAllCountersAsync(
      Guid scopeId,
      string planType,
      int definitionId)
    {
      CounterComponent component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (RemoveAllCountersAsync));
      try
      {
        component.PrepareStoredProcedure("Task.prc_RemoveAllCounters");
        component.BindDefaultDataspaceId(scopeId);
        component.BindString("@planType", planType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindInt("@definitionId", definitionId);
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
    }

    public virtual async Task RemoveCounterAsync(
      Guid scopeId,
      string planType,
      int definitionId,
      string prefix)
    {
      CounterComponent component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (RemoveCounterAsync));
      try
      {
        component.PrepareStoredProcedure("Task.prc_RemoveCounter");
        component.BindDefaultDataspaceId(scopeId);
        component.BindString("@planType", planType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindInt("@definitionId", definitionId);
        component.BindString("@prefix", prefix, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
    }

    protected virtual ObjectBinder<CounterVariable> GetCounterVariableBinder() => (ObjectBinder<CounterVariable>) new CounterVariableBinder();

    protected SqlParameter BindDefaultDataspaceId(Guid scopeIdentifier) => this.BindInt("@dataspaceId", this.GetDataspaceId(scopeIdentifier, "Default", true));
  }
}
