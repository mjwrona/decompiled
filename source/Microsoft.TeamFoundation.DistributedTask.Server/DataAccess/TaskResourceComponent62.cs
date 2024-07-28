// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent62
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
  internal class TaskResourceComponent62 : TaskResourceComponent61
  {
    public override async Task<TaskAgentCloudRequest> GetAgentCloudRequestAsync(
      int agentCloudId,
      Guid requestId)
    {
      TaskResourceComponent62 component = this;
      TaskAgentCloudRequest cloudRequestAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentCloudRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentCloudRequest");
        component.BindInt("@agentCloudId", agentCloudId);
        component.BindGuid("@requestId", requestId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloudRequest>(component.CreateTaskAgentCloudRequestBinder());
          cloudRequestAsync = resultCollection.GetCurrent<TaskAgentCloudRequest>().FirstOrDefault<TaskAgentCloudRequest>();
        }
      }
      return cloudRequestAsync;
    }

    public override IEnumerable<DeploymentPoolSummary> GetDeploymentPoolsSummaryById(
      IList<int> poolIds)
    {
      List<DeploymentPoolSummary> poolsSummaryById = new List<DeploymentPoolSummary>();
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetDeploymentPoolsSummaryById)))
      {
        this.PrepareStoredProcedure("Task.prc_GetDeploymentPoolsMetricsById");
        this.BindUniqueInt32Table("@poolIds", (IEnumerable<int>) poolIds.Distinct<int>().ToList<int>());
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<DeploymentPoolSummary>((ObjectBinder<DeploymentPoolSummary>) new DeploymentPoolSummaryBinder(this.RequestContext));
          poolsSummaryById.AddRange((IEnumerable<DeploymentPoolSummary>) resultCollection.GetCurrent<DeploymentPoolSummary>().Items);
        }
      }
      return (IEnumerable<DeploymentPoolSummary>) poolsSummaryById;
    }
  }
}
