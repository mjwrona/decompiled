// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent45
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent45 : TaskResourceComponent44
  {
    public override int GetResourceUsage(
      Guid hostId,
      string resourceType,
      bool poolIsHosted,
      bool includeRunningRequests,
      int maxRequestsCount,
      out List<TaskAgentJobRequest> runningRequests)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetResourceUsage)))
      {
        this.PrepareStoredProcedure("Task.prc_GetResourceUsage");
        this.BindGuid("@hostId", hostId);
        this.BindString("@resourceType", resourceType, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBoolean("@poolIsHosted", poolIsHosted);
        this.BindBoolean("@includeRunningRequests", includeRunningRequests);
        this.BindInt("@maxRequestsCount", maxRequestsCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          SqlColumnBinder runningRequestsCountBinder = new SqlColumnBinder("RunningRequestsCount");
          resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => runningRequestsCountBinder.GetInt32(reader))));
          if (includeRunningRequests)
            resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          int resourceUsage = resultCollection.GetCurrent<int>().First<int>();
          runningRequests = new List<TaskAgentJobRequest>();
          if (includeRunningRequests)
          {
            resultCollection.NextResult();
            runningRequests.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          }
          return resourceUsage;
        }
      }
    }

    public override async Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      int poolId,
      int? agentCloudId,
      TaskAgentJobRequest request,
      string requestResourceType)
    {
      TaskResourceComponent45 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (QueueAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_QueueAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) request.MatchedAgents.Select<TaskAgentReference, int>((System.Func<TaskAgentReference, int>) (x => x.Id)).Distinct<int>().ToList<int>());
        component.BindGuid("@serviceOwner", request.ServiceOwner);
        component.BindGuid("@hostId", request.HostId);
        component.BindGuid("@scopeId", request.ScopeId);
        component.BindString("@planType", request.PlanType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindGuid("@planId", request.PlanId);
        component.BindGuid("@jobId", request.JobId);
        component.BindString("@jobName", request.JobName, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindBinary("@demands", JsonUtility.Serialize((object) request.Demands, false), int.MaxValue, SqlDbType.VarBinary);
        TaskResourceComponent45 resourceComponent45_1 = component;
        TaskOrchestrationOwner definition = request.Definition;
        int? parameterValue1 = new int?(definition != null ? definition.Id : 0);
        resourceComponent45_1.BindNullableInt("@definitionId", parameterValue1);
        component.BindBinary("@definitionReference", JsonUtility.Serialize((object) request.Definition, false), int.MaxValue, SqlDbType.VarBinary);
        TaskResourceComponent45 resourceComponent45_2 = component;
        TaskOrchestrationOwner owner = request.Owner;
        int? parameterValue2 = new int?(owner != null ? owner.Id : 0);
        resourceComponent45_2.BindNullableInt("@ownerId", parameterValue2);
        component.BindBinary("@ownerReference", JsonUtility.Serialize((object) request.Owner, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindBinary("@data", JsonUtility.Serialize((object) request.Data, false), 1024, SqlDbType.VarBinary);
        component.BindString("@resourceType", requestResourceType, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindBoolean("@assignRequests", false);
        component.BindNullableInt("@queueId", request.QueueId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<MatchedAgent>((ObjectBinder<MatchedAgent>) new MatchedAgentBinder2());
          request = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          if (request != null)
          {
            resultCollection.NextResult();
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
              request.MatchedAgents.Add(matchedAgent.Agent);
          }
          taskAgentJobRequest = request;
        }
      }
      return taskAgentJobRequest;
    }

    public override UpdateDeploymentMachinesResult UpdateDeploymentTargets(
      Guid projectId,
      int machineGroupId,
      IEnumerable<DeploymentMachine> machines)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateDeploymentTargets)))
      {
        IList<int> machineIdsForTagDeletion = (IList<int>) null;
        UpdateDeploymentMachinesResult deploymentMachinesResult = new UpdateDeploymentMachinesResult();
        IList<KeyValuePair<int, string>> machinesTagsTable = this.GetDeploymentMachinesTagsTable(machines, out machineIdsForTagDeletion);
        this.PrepareStoredProcedure("Task.prc_UpdateDeploymentTargets");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@deploymentGroupId", machineGroupId);
        this.BindKeyValuePairInt32StringTable("@tagsTable", (IEnumerable<KeyValuePair<int, string>>) machinesTagsTable);
        this.BindUniqueInt32Table("@agentIdsForTagDeletion", (IEnumerable<int>) machineIdsForTagDeletion);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
          resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
          resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
          resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
          List<QueueAgent> items = resultCollection.GetCurrent<QueueAgent>().Items;
          resultCollection.NextResult();
          ILookup<int, string> lookup1 = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
          resultCollection.NextResult();
          deploymentMachinesResult.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().First<DeploymentGroup>((System.Func<DeploymentGroup, bool>) (d => d.Id == machineGroupId));
          resultCollection.NextResult();
          ILookup<int, string> lookup2 = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
          deploymentMachinesResult.Machines = this.CreateDeploymentTargetsChangedDataWithTags((IEnumerable<QueueAgent>) items, lookup1, lookup2, machineGroupId);
        }
        return deploymentMachinesResult;
      }
    }
  }
}
