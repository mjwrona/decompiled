// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent41
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent41 : TaskTrackingComponent40
  {
    private static readonly SqlMetaData[] typ_PlanConcurrencyLimit = new SqlMetaData[3]
    {
      new SqlMetaData("@DataspaceId", SqlDbType.Int),
      new SqlMetaData("@DefinitionId", SqlDbType.Int),
      new SqlMetaData("@MaxConcurrency", SqlDbType.Int)
    };

    public override IList<TaskOrchestrationPlan> GetRunnablePlans(
      ICollection<PlanConcurrency> concurrencyLimits,
      int maxCount)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetRunnablePlans)))
      {
        this.PrepareStoredProcedure("Task.prc_GetRunnablePlans");
        this.BindPlanConcurrencyTable("@planConcurrency", concurrencyLimits);
        this.BindInt("@maxCount", maxCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>(this.GetPlanBinder());
          return (IList<TaskOrchestrationPlan>) resultCollection.GetCurrent<TaskOrchestrationPlan>().Items;
        }
      }
    }

    private SqlParameter BindPlanConcurrencyTable(
      string parameterName,
      ICollection<PlanConcurrency> rows)
    {
      return this.BindTable(parameterName, "Task.typ_PlanConcurrencyTable", (IEnumerable<SqlDataRecord>) ((rows != null ? (object) rows.Select<PlanConcurrency, SqlDataRecord>(new System.Func<PlanConcurrency, SqlDataRecord>(this.ConvertToSqlDataRecord)) : (object) null) ?? (object) Array.Empty<SqlDataRecord>()));
    }

    private SqlDataRecord ConvertToSqlDataRecord(PlanConcurrency planConcurrency)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(TaskTrackingComponent41.typ_PlanConcurrencyLimit);
      sqlDataRecord.SetInt32(0, this.GetDataspaceId(planConcurrency.ScopeIdentifier, planConcurrency.DataspaceCategory));
      sqlDataRecord.SetInt32(1, planConcurrency.DefinitionId);
      sqlDataRecord.SetInt32(2, planConcurrency.MaxConcurrency);
      return sqlDataRecord;
    }
  }
}
