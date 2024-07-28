// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent78
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent78 : TaskResourceComponent77
  {
    public override async Task<AssignAgentRequestsResult> AssignAgentRequestsAsyncV2(
      TimeSpan privateLeaseTimeout,
      TimeSpan hostedLeaseTimeout,
      TimeSpan defaultLeaseTimeout,
      IList<ResourceLimit> resourceLimits,
      int maxRequestsCount,
      bool transitionAgentState)
    {
      TaskResourceComponent78 component = this;
      AssignAgentRequestsResult agentRequestsResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AssignAgentRequestsAsyncV2)))
      {
        AssignAgentRequestsResult result = new AssignAgentRequestsResult();
        component.PrepareStoredProcedure("Task.prc_AssignAgentRequestsV2");
        component.BindInt("@privateLeaseTimeout", (int) privateLeaseTimeout.TotalSeconds);
        component.BindInt("@hostedLeaseTimeout", (int) hostedLeaseTimeout.TotalSeconds);
        component.BindInt("@defaultLeaseTimeout", (int) defaultLeaseTimeout.TotalSeconds);
        component.BindResourceLimitsTable("@resourceLimits", resourceLimits);
        component.BindInt("@maxRequestsCount", maxRequestsCount);
        component.BindBoolean("@transitionAgentState", transitionAgentState);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<AssignedAgentRequestResult>(component.CreateAssignedAgentRequestResultBinder());
          resultCollection.AddBinder<ResourceUsageData>((ObjectBinder<ResourceUsageData>) new ResourceUsageDataBinder());
          resultCollection.AddBinder<ProvisionAgentEvent>(component.CreateProvisionAgentEventBinder());
          resultCollection.AddBinder<RequestAssignedEvent>(component.CreateRequestAssignedEventBinder());
          resultCollection.AddBinder<DeprovisionEvent>(component.CreateDeprovisionEventBinder());
          result.AssignedRequestResults.AddRange((IEnumerable<AssignedAgentRequestResult>) resultCollection.GetCurrent<AssignedAgentRequestResult>().Items);
          resultCollection.NextResult();
          result.ResourceUsageDataCollection.AddRange((IEnumerable<ResourceUsageData>) resultCollection.GetCurrent<ResourceUsageData>().Items);
          resultCollection.NextResult();
          result.Events.AddRange((IEnumerable<RunAgentEvent>) resultCollection.GetCurrent<ProvisionAgentEvent>().Items);
          resultCollection.NextResult();
          result.Events.AddRange((IEnumerable<RunAgentEvent>) resultCollection.GetCurrent<RequestAssignedEvent>().Items);
          resultCollection.NextResult();
          result.Events.AddRange((IEnumerable<RunAgentEvent>) resultCollection.GetCurrent<DeprovisionEvent>().Items);
        }
        agentRequestsResult = result;
      }
      return agentRequestsResult;
    }
  }
}
