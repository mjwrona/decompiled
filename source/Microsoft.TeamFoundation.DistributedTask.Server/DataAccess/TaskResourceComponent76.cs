// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent76
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent76 : TaskResourceComponent75
  {
    public override async Task<TaskAgentJobRequest> BumpAgentRequestPriorityAsync(
      int poolId,
      long requestId)
    {
      TaskResourceComponent76 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (BumpAgentRequestPriorityAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_BumpAgentRequestPriority");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return taskAgentJobRequest;
    }

    protected override ObjectBinder<TaskAgentJobRequest> CreateTaskAgentRequestBinder() => (ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder16();

    protected override ObjectBinder<TaskAgentJobRequest> CreateTaskAgentRequestBinder(int poolId) => (ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder16();
  }
}
