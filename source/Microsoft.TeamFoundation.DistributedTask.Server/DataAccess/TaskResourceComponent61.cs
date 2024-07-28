// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent61
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent61 : TaskResourceComponent60
  {
    protected override ObjectBinder<DeprovisioningAgentResult> CreateDeprovisionedAgentResultBinder() => (ObjectBinder<DeprovisioningAgentResult>) new DeprovisionedAgentResultBinder2();

    protected override ObjectBinder<TaskAgentCloud> CreateTaskAgentCloudBinder() => (ObjectBinder<TaskAgentCloud>) new TaskAgentCloudBinder5();

    protected override ObjectBinder<AssignedAgentRequestResult> CreateAssignedAgentRequestResultBinder() => (ObjectBinder<AssignedAgentRequestResult>) new AssignedAgentRequestResultBinder4();

    public override async Task<TaskAgentCloud> AddAgentCloudAsync(
      Guid id,
      string name,
      string type,
      string getAgentDefinitionEndpoint,
      string acquireAgentEndpoint,
      string getAgentRequestStatusEndpoint,
      string releaseAgentEndpoint,
      string getAccountParallelismEndpoint,
      int? maxParallelism,
      int? acquisitionTimeout,
      bool internalAgentCloud = false)
    {
      TaskResourceComponent61 component = this;
      TaskAgentCloud taskAgentCloud;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddAgentCloudAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_AddAgentCloud");
        component.BindGuid("@id", id);
        component.BindString("@name", name, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@type", type, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@getAgentDefinitionEndpoint", getAgentDefinitionEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@acquireAgentEndpoint", acquireAgentEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@getAgentRequestStatusEndpoint", getAgentRequestStatusEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@releaseAgentEndpoint", releaseAgentEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindNullableInt("@acquisitionTimeout", acquisitionTimeout);
        component.BindString("@getAccountParallelismEndpoint", getAccountParallelismEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindNullableInt("@maxParallelism", maxParallelism);
        component.BindBoolean("@internal", internalAgentCloud);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          taskAgentCloud = resultCollection.GetCurrent<TaskAgentCloud>().First<TaskAgentCloud>();
        }
      }
      return taskAgentCloud;
    }

    public override async Task<TaskAgentCloud> UpdateAgentCloudAsync(
      int agentCloudId,
      string name,
      string type,
      string getAgentDefinitionEndpoint,
      string acquireAgentEndpoint,
      string getAgentRequestStatusEndpoint,
      string releaseAgentEndpoint,
      string getAccountParallelismEndpoint,
      int? maxParallelism,
      int? acquisitionTimeout,
      bool internalAgentCloud = false)
    {
      TaskResourceComponent61 component = this;
      TaskAgentCloud taskAgentCloud;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentCloudAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentCloud");
        component.BindInt("@agentCloudId", agentCloudId);
        component.BindString("@name", name, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@type", type, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@getAgentDefinitionEndpoint", getAgentDefinitionEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@acquireAgentEndpoint", acquireAgentEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@getAgentRequestStatusEndpoint", getAgentRequestStatusEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@releaseAgentEndpoint", releaseAgentEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindNullableInt("@acquisitionTimeout", acquisitionTimeout);
        component.BindString("@getAccountParallelismEndpoint", getAccountParallelismEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindNullableInt("@maxParallelism", maxParallelism);
        component.BindBoolean("@internal", internalAgentCloud);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          taskAgentCloud = resultCollection.GetCurrent<TaskAgentCloud>().First<TaskAgentCloud>();
        }
      }
      return taskAgentCloud;
    }
  }
}
