// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent27
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent27 : TaskResourceComponent26
  {
    public override UpdateDeploymentMachinesResult UpdateDeploymentMachines(
      Guid projectId,
      int machineGroupId,
      IEnumerable<DeploymentMachine> machines)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateDeploymentMachines)))
      {
        IList<int> agentIdsForTagDeletion = (IList<int>) null;
        UpdateDeploymentMachinesResult deploymentMachinesResult = new UpdateDeploymentMachinesResult();
        IList<KeyValuePair<int, string>> taskAgentTagsTable = this.GetTaskAgentTagsTable(machines, out agentIdsForTagDeletion);
        this.PrepareStoredProcedure("Task.prc_UpdateDeploymentMachines");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@machineGroupId", machineGroupId);
        this.BindKeyValuePairInt32StringTable("@tagsTable", (IEnumerable<KeyValuePair<int, string>>) taskAgentTagsTable);
        this.BindUniqueInt32Table("@agentIdsForTagDeletion", (IEnumerable<int>) agentIdsForTagDeletion);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentTag>((ObjectBinder<TaskAgentTag>) new TaskAgentTagBinder());
          resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
          List<TaskAgentTag> items = resultCollection.GetCurrent<TaskAgentTag>().Items;
          deploymentMachinesResult.Machines = TaskResourceComponent25.CreateDeploymentMachinesChangedDataWithTags((IEnumerable<TaskAgentTag>) items);
          resultCollection.NextResult();
          deploymentMachinesResult.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().First<DeploymentGroup>();
        }
        return deploymentMachinesResult;
      }
    }
  }
}
