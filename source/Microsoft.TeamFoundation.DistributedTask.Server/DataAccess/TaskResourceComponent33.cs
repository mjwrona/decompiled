// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent33
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent33 : TaskResourceComponent32
  {
    protected static SqlMetaData[] typ_AgentPoolMaintenanceJobTargetAgentTable = new SqlMetaData[3]
    {
      new SqlMetaData("AgentId", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Result", SqlDbType.TinyInt)
    };

    public override AgentQueueOrMachineGroup AddAgentQueue(
      Guid projectId,
      string name,
      int poolId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      string description = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddAgentQueue)))
      {
        this.PrepareStoredProcedure("Task.prc_AddQueue");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindString("@queueName", name, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindByte("@queueType", (byte) queueType);
        this.BindInt("@poolId", poolId);
        this.BindString("@description", description, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@writerId", this.Author);
        AgentQueueOrMachineGroup queueOrMachineGroup = new AgentQueueOrMachineGroup();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          if (queueType == TaskAgentQueueType.Automation)
          {
            resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
            queueOrMachineGroup.Queue = resultCollection.GetCurrent<TaskAgentQueue>().FirstOrDefault<TaskAgentQueue>();
          }
          else
          {
            resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
            queueOrMachineGroup.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>();
          }
          return queueOrMachineGroup;
        }
      }
    }

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
        this.PrepareStoredProcedure("Task.prc_UpdateQueue");
        this.BindInt("@queueId", queueId);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindByte("@queueType", (byte) queueType);
        this.BindString("@name", name, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindNullableGuid("@groupScopeId", groupScopeId);
        this.BindNullableBoolean("@provisioned", provisioned);
        this.BindGuid("@writerId", this.Author);
        this.BindNullableInt("@poolId", poolId);
        this.BindString("@description", description, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        AgentQueueOrMachineGroup queueOrMachineGroup = new AgentQueueOrMachineGroup();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          if (queueType == TaskAgentQueueType.Automation)
          {
            resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
            queueOrMachineGroup.Queue = resultCollection.GetCurrent<TaskAgentQueue>().Items.FirstOrDefault<TaskAgentQueue>();
          }
          else
          {
            resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
            queueOrMachineGroup.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>();
          }
          return queueOrMachineGroup;
        }
      }
    }

    public override TaskAgentPoolMaintenanceJob QueueAgentPoolMaintenanceJob(
      int poolId,
      int definitionId,
      Guid requestedBy)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (QueueAgentPoolMaintenanceJob)))
      {
        this.PrepareStoredProcedure("Task.prc_QueueAgentPoolMaintenanceJob");
        this.BindInt("@poolId", poolId);
        this.BindInt("@definitionId", definitionId);
        this.BindGuid("@requestedBy", requestedBy);
        TaskAgentPoolMaintenanceJob poolMaintenanceJob = new TaskAgentPoolMaintenanceJob();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJob>((ObjectBinder<TaskAgentPoolMaintenanceJob>) new TaskAgentPoolMaintenanceJobBinder());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJobTargetAgent>((ObjectBinder<TaskAgentPoolMaintenanceJobTargetAgent>) new TaskAgentPoolMaintenanceJobTargetAgentBinder2());
          poolMaintenanceJob = resultCollection.GetCurrent<TaskAgentPoolMaintenanceJob>().First<TaskAgentPoolMaintenanceJob>();
          resultCollection.NextResult();
          poolMaintenanceJob.TargetAgents = resultCollection.GetCurrent<TaskAgentPoolMaintenanceJobTargetAgent>().Items;
        }
        return poolMaintenanceJob;
      }
    }

    public override TaskAgentPoolMaintenanceJob GetAgentPoolMaintenanceJob(int poolId, int jobId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPoolMaintenanceJob)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPoolMaintenanceJob");
        this.BindInt("@poolId", poolId);
        this.BindInt("@jobId", jobId);
        TaskAgentPoolMaintenanceJob poolMaintenanceJob = new TaskAgentPoolMaintenanceJob();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJob>((ObjectBinder<TaskAgentPoolMaintenanceJob>) new TaskAgentPoolMaintenanceJobBinder());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJobTargetAgent>((ObjectBinder<TaskAgentPoolMaintenanceJobTargetAgent>) new TaskAgentPoolMaintenanceJobTargetAgentBinder2());
          poolMaintenanceJob = resultCollection.GetCurrent<TaskAgentPoolMaintenanceJob>().FirstOrDefault<TaskAgentPoolMaintenanceJob>();
          resultCollection.NextResult();
          poolMaintenanceJob.TargetAgents = resultCollection.GetCurrent<TaskAgentPoolMaintenanceJobTargetAgent>().Items;
        }
        return poolMaintenanceJob;
      }
    }

    public override IList<TaskAgentPoolMaintenanceJob> GetAgentPoolMaintenanceJobs(
      int poolId,
      int? defintionId = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPoolMaintenanceJobs)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPoolMaintenanceJobs");
        this.BindInt("@poolId", poolId);
        this.BindNullableInt("@definitionId", defintionId);
        Dictionary<int, TaskAgentPoolMaintenanceJob> dictionary1 = new Dictionary<int, TaskAgentPoolMaintenanceJob>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJob>((ObjectBinder<TaskAgentPoolMaintenanceJob>) new TaskAgentPoolMaintenanceJobBinder());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJobTargetAgent>((ObjectBinder<TaskAgentPoolMaintenanceJobTargetAgent>) new TaskAgentPoolMaintenanceJobTargetAgentBinder2());
          Dictionary<int, TaskAgentPoolMaintenanceJob> dictionary2 = resultCollection.GetCurrent<TaskAgentPoolMaintenanceJob>().Items.ToDictionary<TaskAgentPoolMaintenanceJob, int>((System.Func<TaskAgentPoolMaintenanceJob, int>) (x => x.JobId));
          resultCollection.NextResult();
          foreach (TaskAgentPoolMaintenanceJobTargetAgent maintenanceJobTargetAgent in resultCollection.GetCurrent<TaskAgentPoolMaintenanceJobTargetAgent>().Items)
            dictionary2[maintenanceJobTargetAgent.JobId].TargetAgents.Add(maintenanceJobTargetAgent);
          return (IList<TaskAgentPoolMaintenanceJob>) dictionary2.Values.ToList<TaskAgentPoolMaintenanceJob>();
        }
      }
    }

    public override UpdateTaskAgentPoolMaintenanceJobResult UpdateAgentPoolMaintenanceJob(
      int poolId,
      int jobId,
      TaskAgentPoolMaintenanceJobStatus? status = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskAgentPoolMaintenanceJobResult? result = null,
      IList<TaskAgentPoolMaintenanceJobTargetAgent> targetAgents = null,
      bool updateTargetAgents = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgentPoolMaintenanceJob)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateAgentPoolMaintenanceJob");
        this.BindInt("@poolId", poolId);
        this.BindInt("@jobId", jobId);
        if (status.HasValue)
          this.BindByte("@status", (byte) status.Value);
        if (startTime.HasValue)
          this.BindDateTime2("@startTime", startTime.Value);
        if (finishTime.HasValue)
          this.BindDateTime2("@finishTime", finishTime.Value);
        if (result.HasValue)
          this.BindByte("@result", (byte) result.Value);
        if (targetAgents != null)
          this.BindAgentPoolMaintenanceJobTargetAgentTable("@targetAgents", (IEnumerable<TaskAgentPoolMaintenanceJobTargetAgent>) targetAgents);
        this.BindBoolean("@updateTargetAgents", updateTargetAgents);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJob>((ObjectBinder<TaskAgentPoolMaintenanceJob>) new TaskAgentPoolMaintenanceJobBinder());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJobTargetAgent>((ObjectBinder<TaskAgentPoolMaintenanceJobTargetAgent>) new TaskAgentPoolMaintenanceJobTargetAgentBinder2());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJob>((ObjectBinder<TaskAgentPoolMaintenanceJob>) new TaskAgentPoolMaintenanceJobBinder());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJobTargetAgent>((ObjectBinder<TaskAgentPoolMaintenanceJobTargetAgent>) new TaskAgentPoolMaintenanceJobTargetAgentBinder2());
          TaskAgentPoolMaintenanceJob poolMaintenanceJob1 = resultCollection.GetCurrent<TaskAgentPoolMaintenanceJob>().First<TaskAgentPoolMaintenanceJob>();
          resultCollection.NextResult();
          poolMaintenanceJob1.TargetAgents = resultCollection.GetCurrent<TaskAgentPoolMaintenanceJobTargetAgent>().Items;
          resultCollection.NextResult();
          TaskAgentPoolMaintenanceJob poolMaintenanceJob2 = resultCollection.GetCurrent<TaskAgentPoolMaintenanceJob>().First<TaskAgentPoolMaintenanceJob>();
          resultCollection.NextResult();
          poolMaintenanceJob2.TargetAgents = resultCollection.GetCurrent<TaskAgentPoolMaintenanceJobTargetAgent>().Items;
          return new UpdateTaskAgentPoolMaintenanceJobResult()
          {
            OldJob = poolMaintenanceJob1,
            UpdatedJob = poolMaintenanceJob2
          };
        }
      }
    }

    protected SqlParameter BindAgentPoolMaintenanceJobTargetAgentTable(
      string parameterName,
      IEnumerable<TaskAgentPoolMaintenanceJobTargetAgent> rows)
    {
      return this.BindTable(parameterName, "Task.typ_AgentPoolMaintenanceJobTargetAgentTable", (rows ?? Enumerable.Empty<TaskAgentPoolMaintenanceJobTargetAgent>()).Select<TaskAgentPoolMaintenanceJobTargetAgent, SqlDataRecord>(new System.Func<TaskAgentPoolMaintenanceJobTargetAgent, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected SqlDataRecord ConvertToSqlDataRecord(TaskAgentPoolMaintenanceJobTargetAgent row)
    {
      SqlDataRecord record = new SqlDataRecord(TaskResourceComponent33.typ_AgentPoolMaintenanceJobTargetAgentTable);
      record.SetNullableInt32(0, new int?(row.Agent.Id));
      TaskAgentPoolMaintenanceJobStatus? status = row.Status;
      record.SetNullableByte(1, status.HasValue ? new byte?((byte) status.GetValueOrDefault()) : new byte?());
      TaskAgentPoolMaintenanceJobResult? result = row.Result;
      record.SetNullableByte(2, result.HasValue ? new byte?((byte) result.GetValueOrDefault()) : new byte?());
      return record;
    }
  }
}
