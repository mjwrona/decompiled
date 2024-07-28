// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent31
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

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent31 : TaskResourceComponent30
  {
    public override TaskAgentPoolMaintenanceDefinition CreateMaintenanceDefinition(
      int poolId,
      TaskAgentPoolMaintenanceDefinition definition)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreateMaintenanceDefinition)))
      {
        this.PrepareStoredProcedure("Task.prc_AddAgentPoolMaintenanceDefinition");
        this.BindInt("@poolId", poolId);
        this.BindBoolean("@enabled", definition.Enabled);
        this.BindInt("@jobTimeout", definition.JobTimeoutInMinutes);
        this.BindInt("@agentsConcurrent", definition.MaxConcurrentAgentsPercentage);
        this.BindString("@options", JsonUtility.ToString((object) definition.Options), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@scheduleSetting", JsonUtility.ToString((object) definition.ScheduleSetting), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@retentionPolicy", JsonUtility.ToString((object) definition.RetentionPolicy), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceDefinition>((ObjectBinder<TaskAgentPoolMaintenanceDefinition>) new TaskAgentPoolMaintenanceDefinitionBinder());
          return resultCollection.GetCurrent<TaskAgentPoolMaintenanceDefinition>().First<TaskAgentPoolMaintenanceDefinition>();
        }
      }
    }

    public override GetTaskAgentPoolMaintenanceDefinitionResult GetAgentPoolMaintenanceDefinition(
      int poolId,
      int definitionId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPoolMaintenanceDefinition)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPoolMaintenanceDefinition");
        this.BindInt("@poolId", poolId);
        this.BindInt("@definitionId", definitionId);
        GetTaskAgentPoolMaintenanceDefinitionResult maintenanceDefinition = new GetTaskAgentPoolMaintenanceDefinitionResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceDefinition>((ObjectBinder<TaskAgentPoolMaintenanceDefinition>) new TaskAgentPoolMaintenanceDefinitionBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          maintenanceDefinition.Definition = resultCollection.GetCurrent<TaskAgentPoolMaintenanceDefinition>().FirstOrDefault<TaskAgentPoolMaintenanceDefinition>();
          resultCollection.NextResult();
          maintenanceDefinition.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          return maintenanceDefinition;
        }
      }
    }

    public override IList<TaskAgentPoolMaintenanceDefinition> GetAgentPoolMaintenanceDefinitions(
      int poolId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPoolMaintenanceDefinitions)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPoolMaintenanceDefinitions");
        this.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceDefinition>((ObjectBinder<TaskAgentPoolMaintenanceDefinition>) new TaskAgentPoolMaintenanceDefinitionBinder());
          return (IList<TaskAgentPoolMaintenanceDefinition>) resultCollection.GetCurrent<TaskAgentPoolMaintenanceDefinition>().Items;
        }
      }
    }

    public override TaskAgentPoolMaintenanceDefinition UpdateAgentPoolMaintenanceDefinition(
      int poolId,
      int definitionId,
      TaskAgentPoolMaintenanceDefinition definition)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgentPoolMaintenanceDefinition)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateAgentPoolMaintenanceDefinition");
        this.BindInt("@poolId", poolId);
        this.BindInt("@definitionId", definitionId);
        this.BindBoolean("@enabled", definition.Enabled);
        this.BindInt("@jobTimeout", definition.JobTimeoutInMinutes);
        this.BindInt("@agentsConcurrent", definition.MaxConcurrentAgentsPercentage);
        this.BindString("@options", JsonUtility.ToString((object) definition.Options), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@scheduleSetting", JsonUtility.ToString((object) definition.ScheduleSetting), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@retentionPolicy", JsonUtility.ToString((object) definition.RetentionPolicy), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceDefinition>((ObjectBinder<TaskAgentPoolMaintenanceDefinition>) new TaskAgentPoolMaintenanceDefinitionBinder());
          return resultCollection.GetCurrent<TaskAgentPoolMaintenanceDefinition>().First<TaskAgentPoolMaintenanceDefinition>();
        }
      }
    }

    public override DeletePoolMaintenanceDefinitionResult DeleteAgentPoolMaintenanceDefinitions(
      int poolId,
      IEnumerable<int> definitionIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgentPoolMaintenanceDefinitions)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteAgentPoolMaintenanceDefinitions");
        this.BindInt("@poolId", poolId);
        this.BindInt32Table("@definitionIds", definitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJob>((ObjectBinder<TaskAgentPoolMaintenanceJob>) new TaskAgentPoolMaintenanceJobBinder());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceDefinition>((ObjectBinder<TaskAgentPoolMaintenanceDefinition>) new TaskAgentPoolMaintenanceDefinitionBinder());
          List<TaskAgentPoolMaintenanceJob> items1 = resultCollection.GetCurrent<TaskAgentPoolMaintenanceJob>().Items;
          resultCollection.NextResult();
          List<TaskAgentPoolMaintenanceDefinition> items2 = resultCollection.GetCurrent<TaskAgentPoolMaintenanceDefinition>().Items;
          return new DeletePoolMaintenanceDefinitionResult()
          {
            DeletedPoolMaintenanceJobs = (IList<TaskAgentPoolMaintenanceJob>) items1,
            DeletedPoolMaintenanceDefinitions = (IList<TaskAgentPoolMaintenanceDefinition>) items2
          };
        }
      }
    }

    public override DeleteAgentPoolResult DeleteAgentPool(int poolId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgentPool)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteAgentPool");
        this.BindInt("@poolId", poolId);
        this.BindGuid("@writerId", this.Author);
        DeleteAgentPoolResult deleteAgentPoolResult = new DeleteAgentPoolResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJob>((ObjectBinder<TaskAgentPoolMaintenanceJob>) new TaskAgentPoolMaintenanceJobBinder());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceDefinition>((ObjectBinder<TaskAgentPoolMaintenanceDefinition>) new TaskAgentPoolMaintenanceDefinitionBinder());
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder4());
          resultCollection.AddBinder<TaskAgentSessionData>((ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder2());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          deleteAgentPoolResult.DeletedPoolMaintenanceJobs = (IList<TaskAgentPoolMaintenanceJob>) resultCollection.GetCurrent<TaskAgentPoolMaintenanceJob>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.DeletedPoolMaintenanceDefinitions = (IList<TaskAgentPoolMaintenanceDefinition>) resultCollection.GetCurrent<TaskAgentPoolMaintenanceDefinition>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.DeletedAgents = (IList<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.DeletedSessions = (IList<TaskAgentSessionData>) resultCollection.GetCurrent<TaskAgentSessionData>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          return deleteAgentPoolResult;
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
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJobTargetAgent>((ObjectBinder<TaskAgentPoolMaintenanceJobTargetAgent>) new TaskAgentPoolMaintenanceJobTargetAgentBinder());
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
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJobTargetAgent>((ObjectBinder<TaskAgentPoolMaintenanceJobTargetAgent>) new TaskAgentPoolMaintenanceJobTargetAgentBinder());
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
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJobTargetAgent>((ObjectBinder<TaskAgentPoolMaintenanceJobTargetAgent>) new TaskAgentPoolMaintenanceJobTargetAgentBinder());
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
          this.BindInt32Table("@targetAgents", targetAgents.Select<TaskAgentPoolMaintenanceJobTargetAgent, int>((System.Func<TaskAgentPoolMaintenanceJobTargetAgent, int>) (a => a.Agent.Id)));
        this.BindBoolean("@updateTargetAgents", updateTargetAgents);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJob>((ObjectBinder<TaskAgentPoolMaintenanceJob>) new TaskAgentPoolMaintenanceJobBinder());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJobTargetAgent>((ObjectBinder<TaskAgentPoolMaintenanceJobTargetAgent>) new TaskAgentPoolMaintenanceJobTargetAgentBinder());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJob>((ObjectBinder<TaskAgentPoolMaintenanceJob>) new TaskAgentPoolMaintenanceJobBinder());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJobTargetAgent>((ObjectBinder<TaskAgentPoolMaintenanceJobTargetAgent>) new TaskAgentPoolMaintenanceJobTargetAgentBinder());
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

    public override IList<TaskAgentPoolMaintenanceJob> DeleteAgentPoolMaintenanceJobs(
      int poolId,
      IEnumerable<int> jobIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgentPoolMaintenanceJobs)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteAgentPoolMaintenanceJobs");
        this.BindInt("@poolId", poolId);
        this.BindInt32Table("@jobIds", jobIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJob>((ObjectBinder<TaskAgentPoolMaintenanceJob>) new TaskAgentPoolMaintenanceJobBinder());
          return (IList<TaskAgentPoolMaintenanceJob>) resultCollection.GetCurrent<TaskAgentPoolMaintenanceJob>().Items;
        }
      }
    }

    public override IList<TaskAgentMachine> GetAgentMachines(IEnumerable<string> machineNames)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentMachines)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentMachines");
        this.BindStringTable("@machineNames", machineNames, maxLength: 512);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentMachine>((ObjectBinder<TaskAgentMachine>) new TaskAgentMachineBinder());
          return (IList<TaskAgentMachine>) resultCollection.GetCurrent<TaskAgentMachine>().Items;
        }
      }
    }
  }
}
