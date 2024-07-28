// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent35
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent35 : TaskResourceComponent34
  {
    protected override ObjectBinder<TaskAgent> CreateTaskAgentBinder() => (ObjectBinder<TaskAgent>) new TaskAgentBinder7();

    public override async Task<RequestAgentsUpdateResult> RequestAgentsUpdateAsync(
      int poolId,
      IEnumerable<int> agentIds,
      string targetVersion,
      Guid requestedBy,
      string currentState,
      TaskAgentUpdateReasonData reasonData)
    {
      TaskResourceComponent35 component = this;
      RequestAgentsUpdateResult agentsUpdateResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (RequestAgentsUpdateAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_RequestAgentsUpdate");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@agentIds", agentIds);
        component.BindString("@targetVersion", targetVersion, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindGuid("@requestedBy", requestedBy);
        component.BindString("@currentState", currentState, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindByte("@reason", (byte) reasonData.Reason);
        component.BindGuid("@serviceOwner", reasonData.ServiceOwner);
        component.BindGuid("@hostId", reasonData.HostId);
        component.BindGuid("@scopeId", reasonData.ScopeId);
        component.BindString("@planType", reasonData.PlanType, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindNullableInt("@definitionId", reasonData.DefinitionReference != null ? reasonData.DefinitionReference.Id : 0, 0);
        component.BindBinary("@definitionReference", JsonUtility.Serialize((object) reasonData.DefinitionReference, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindNullableInt("@ownerId", reasonData.OwnerReference != null ? reasonData.OwnerReference.Id : 0, 0);
        component.BindBinary("@ownerReference", JsonUtility.Serialize((object) reasonData.OwnerReference, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindString("@data", JsonUtility.ToString((object) reasonData.MinAgentVersion), int.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        RequestAgentsUpdateResult result = new RequestAgentsUpdateResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>(component.CreateTaskAgentBinder());
          resultCollection.AddBinder<TaskAgent>(component.CreateTaskAgentBinder());
          result.NewUpdates.AddRange((IEnumerable<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items);
          resultCollection.NextResult();
          result.ExistingUpdates.AddRange((IEnumerable<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items);
          agentsUpdateResult = result;
        }
      }
      return agentsUpdateResult;
    }
  }
}
