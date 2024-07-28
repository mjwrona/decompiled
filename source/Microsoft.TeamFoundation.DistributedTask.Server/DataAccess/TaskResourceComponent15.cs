// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent15
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent15 : TaskResourceComponent14
  {
    public override AgentQueueOrMachineGroup UpdateAgentQueue(
      Guid projectId,
      int queueId,
      string name,
      Guid? groupScopeId = null,
      bool? provisioned = null,
      int? poolId = null,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      string description = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgentQueue)))
      {
        if (queueType == TaskAgentQueueType.Deployment)
          throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 26);
        this.PrepareStoredProcedure("Task.prc_UpdateQueue");
        this.BindInt("@queueId", queueId);
        this.BindString("@name", name, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindNullableGuid("@groupScopeId", groupScopeId);
        this.BindNullableBoolean("@provisioned", provisioned);
        this.BindGuid("@writerId", this.Author);
        this.BindNullableInt("@poolId", poolId);
        AgentQueueOrMachineGroup queueOrMachineGroup = new AgentQueueOrMachineGroup();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
          queueOrMachineGroup.Queue = resultCollection.GetCurrent<TaskAgentQueue>().Items.FirstOrDefault<TaskAgentQueue>();
          return queueOrMachineGroup;
        }
      }
    }
  }
}
