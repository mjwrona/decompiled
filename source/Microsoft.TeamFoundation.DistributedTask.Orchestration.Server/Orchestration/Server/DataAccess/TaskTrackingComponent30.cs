// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent30
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent30 : TaskTrackingComponent29
  {
    public override bool SupportStoreJobInstance => true;

    public override async Task<T> AddPlanContextAsync<T>(
      Guid scopeIdentifier,
      Guid planId,
      string contextName,
      T contextData)
    {
      TaskTrackingComponent30 component = this;
      T obj;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddPlanContextAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_AddPlanContext");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindString("@contextName", contextName, 512, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        component.BindString("@contextType", nameof (T), 512, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        component.BindBinary("@contextData", JsonUtility.Serialize((object) (T) contextData), SqlDbType.VarBinary);
        PlanContext planContext;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<PlanContext>((ObjectBinder<PlanContext>) new PlanContextBinder());
          planContext = resultCollection.GetCurrent<PlanContext>().FirstOrDefault<PlanContext>();
        }
        obj = JsonUtility.Deserialize<T>(planContext?.ContextData);
      }
      return obj;
    }

    public override T AddPlanContext<T>(
      Guid scopeIdentifier,
      Guid planId,
      string contextName,
      T contextData)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddPlanContext)))
      {
        this.PrepareStoredProcedure("Task.prc_AddPlanContext");
        this.BindDataspaceId(scopeIdentifier);
        this.BindGuid("@planId", planId);
        this.BindString("@contextName", contextName, 512, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindString("@contextType", nameof (T), 512, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindBinary("@contextData", JsonUtility.Serialize((object) contextData), SqlDbType.VarBinary);
        PlanContext planContext;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PlanContext>((ObjectBinder<PlanContext>) new PlanContextBinder());
          planContext = resultCollection.GetCurrent<PlanContext>().FirstOrDefault<PlanContext>();
        }
        return JsonUtility.Deserialize<T>(planContext?.ContextData);
      }
    }

    public override async Task DeletePlanContextAsync(
      Guid scopeIdentifier,
      Guid planId,
      string contextName)
    {
      TaskTrackingComponent30 component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (DeletePlanContextAsync));
      try
      {
        component.PrepareStoredProcedure("Task.prc_DeletePlanContext");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindString("@contextName", contextName, 512, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
    }

    public override async Task<T> GetPlanContextAsync<T>(
      Guid scopeIdentifier,
      Guid planId,
      string contextName)
    {
      TaskTrackingComponent30 component = this;
      T planContextAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetPlanContextAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetPlanContext");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindString("@contextName", contextName, 512, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        PlanContext planContext;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<PlanContext>((ObjectBinder<PlanContext>) new PlanContextBinder());
          planContext = resultCollection.GetCurrent<PlanContext>().FirstOrDefault<PlanContext>();
        }
        planContextAsync = JsonUtility.Deserialize<T>(planContext?.ContextData);
      }
      return planContextAsync;
    }

    public override async Task<T> UpdatePlanContextAsync<T>(
      Guid scopeIdentifier,
      Guid planId,
      string contextName,
      T contextData)
    {
      TaskTrackingComponent30 component = this;
      T obj;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdatePlanContextAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdatePlanContext");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindString("@contextName", contextName, 512, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        component.BindBinary("@contextData", JsonUtility.Serialize((object) (T) contextData), SqlDbType.VarBinary);
        PlanContext planContext;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<PlanContext>((ObjectBinder<PlanContext>) new PlanContextBinder());
          planContext = resultCollection.GetCurrent<PlanContext>().FirstOrDefault<PlanContext>();
        }
        obj = JsonUtility.Deserialize<T>(planContext?.ContextData);
      }
      return obj;
    }
  }
}
