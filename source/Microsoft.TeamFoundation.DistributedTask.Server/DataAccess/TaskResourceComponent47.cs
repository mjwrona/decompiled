// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent47
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent47 : TaskResourceComponent46
  {
    public override async Task<AssignAgentRequestsResult> AssignAgentRequestsAsync(
      TimeSpan privateLeaseTimeout,
      TimeSpan hostedLeaseTimeout,
      TimeSpan defaultLeaseTimeout,
      IList<ResourceLimit> resourceLimits,
      int maxRequestsCount,
      bool transitionAgentState)
    {
      TaskResourceComponent47 component = this;
      AssignAgentRequestsResult agentRequestsResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AssignAgentRequestsAsync)))
      {
        AssignAgentRequestsResult result = new AssignAgentRequestsResult();
        component.PrepareStoredProcedure("Task.prc_AssignAgentRequests");
        component.BindBoolean("@poolIsHosted", false);
        component.BindInt("@leaseTimeoutInSeconds", (int) privateLeaseTimeout.TotalSeconds);
        component.BindResourceLimitsTable("@resourceLimits", resourceLimits);
        component.BindInt("@maxRequestsCount", maxRequestsCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<ResourceUsageData>((ObjectBinder<ResourceUsageData>) new ResourceUsageDataBinder());
          result.AssignedRequestResults.AddRange(resultCollection.GetCurrent<TaskAgentJobRequest>().Items.Select<TaskAgentJobRequest, AssignedAgentRequestResult>((System.Func<TaskAgentJobRequest, AssignedAgentRequestResult>) (x => new AssignedAgentRequestResult(x, (TaskAgentCloud) null, (TaskAgentCloud) null))));
          resultCollection.NextResult();
          result.ResourceUsageDataCollection.AddRange((IEnumerable<ResourceUsageData>) resultCollection.GetCurrent<ResourceUsageData>().Items);
        }
        agentRequestsResult = result;
      }
      return agentRequestsResult;
    }
  }
}
