// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentDeploymentExecutionHistoryComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Data.Model;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class EnvironmentDeploymentExecutionHistoryComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<EnvironmentDeploymentExecutionHistoryComponent>(1),
      (IComponentCreator) new ComponentCreator<EnvironmentDeploymentExecutionHistoryComponent2>(2),
      (IComponentCreator) new ComponentCreator<EnvironmentDeploymentExecutionHistoryComponent3>(3),
      (IComponentCreator) new ComponentCreator<EnvironmentDeploymentExecutionHistoryComponent4>(4)
    }, "DistributedTaskEnvironmentDeploymentExecutionHistory", "DistributedTask");

    public EnvironmentDeploymentExecutionHistoryComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual EnvironmentDeploymentExecutionRecord QueueEnvironmentDeploymentRequest(
      EnvironmentDeploymentExecutionRecord record)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (QueueEnvironmentDeploymentRequest)))
      {
        this.PrepareStoredProcedure("Task.prc_QueueEnvironmentDeploymentRequest");
        this.BindInt("@environmentId", record.EnvironmentId);
        this.BindNullableInt("@resourceId", record.ResourceId);
        this.BindGuid("@serviceOwner", record.ServiceOwner);
        this.BindGuid("@scopeId", record.ScopeId);
        this.BindString("@planType", record.PlanType, 260, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindGuid("@planId", record.PlanId);
        this.BindString("@requestIdentifier", record.RequestIdentifier, 260, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindString("@stageName", record.StageName, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindString("@jobName", record.JobName, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindInt("@stageAttempt", record.StageAttempt);
        this.BindInt("@jobAttempt", record.JobAttempt);
        this.BindInt("@definitionId", record.Definition.Id);
        this.BindString("@definitionReference", record.Definition.Serialize<TaskOrchestrationOwner>(true), 3000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@ownerId", record.Owner.Id);
        this.BindString("@ownerReference", record.Owner.Serialize<TaskOrchestrationOwner>(true), 3000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentDeploymentExecutionRecord>((ObjectBinder<EnvironmentDeploymentExecutionRecord>) new EnvironmentDeploymentExecutionRecordBinder(this));
          return resultCollection.GetCurrent<EnvironmentDeploymentExecutionRecord>().Items.FirstOrDefault<EnvironmentDeploymentExecutionRecord>();
        }
      }
    }

    public virtual IList<EnvironmentDeploymentExecutionRecord> GetEnvironmentDeploymentRequests(
      int environmentId,
      long? continuationToken,
      int maxRecords = 25)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentDeploymentRequests)))
      {
        this.PrepareStoredProcedure("Task.prc_GetEnvironmentDeploymentRequest");
        continuationToken = new long?(!continuationToken.HasValue || continuationToken.Value <= 0L ? long.MaxValue : continuationToken.Value);
        this.BindInt("@environmentId", environmentId);
        this.BindLong("@continuationToken", continuationToken.Value);
        this.BindNullableInt("@maxRecords", new int?(maxRecords));
        List<EnvironmentDeploymentExecutionRecord> deploymentRequests = new List<EnvironmentDeploymentExecutionRecord>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentDeploymentExecutionRecord>((ObjectBinder<EnvironmentDeploymentExecutionRecord>) new EnvironmentDeploymentExecutionRecordBinder(this));
          deploymentRequests.AddRange((IEnumerable<EnvironmentDeploymentExecutionRecord>) resultCollection.GetCurrent<EnvironmentDeploymentExecutionRecord>().Items);
          return (IList<EnvironmentDeploymentExecutionRecord>) deploymentRequests;
        }
      }
    }

    public virtual IList<DeploymentExecutionRecordObject> GetEnvironmentDeploymentRequestsByOwnerId(
      Guid scopeId,
      int ownerId,
      string planType)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentDeploymentRequestsByOwnerId)))
      {
        this.PrepareStoredProcedure("Task.prc_GetEnvironmentDeploymentRequestsByOwnerId");
        this.BindGuid("@scopeId", scopeId);
        this.BindInt("@ownerId", ownerId);
        this.BindString("@planType", planType, 260, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        List<DeploymentExecutionRecordObject> requestsByOwnerId = new List<DeploymentExecutionRecordObject>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<DeploymentExecutionRecordObject>((ObjectBinder<DeploymentExecutionRecordObject>) new DeploymentExecutionRecordObjectBinder2(this));
          requestsByOwnerId.AddRange((IEnumerable<DeploymentExecutionRecordObject>) resultCollection.GetCurrent<DeploymentExecutionRecordObject>().Items);
          return (IList<DeploymentExecutionRecordObject>) requestsByOwnerId;
        }
      }
    }

    public virtual EnvironmentDeploymentExecutionRecord UpdateEnvironmentDeploymentRequest(
      int environmentId,
      long requestId,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateEnvironmentDeploymentRequest)))
      {
        this.PrepareForAuditingAction("Pipelines.DeploymentJobCompleted");
        this.PrepareStoredProcedure("Task.prc_UpdateEnvironmentDeploymentRequest");
        this.BindInt("@environmentId", environmentId);
        this.BindLong("@requestId", requestId);
        this.BindNullableDateTime2("@startTime", startTime);
        this.BindNullableDateTime2("@finishTime", finishTime);
        if (result.HasValue)
          this.BindByte("@result", (byte) result.Value);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentDeploymentExecutionRecord>((ObjectBinder<EnvironmentDeploymentExecutionRecord>) new EnvironmentDeploymentExecutionRecordBinder(this));
          return resultCollection.GetCurrent<EnvironmentDeploymentExecutionRecord>().Items.FirstOrDefault<EnvironmentDeploymentExecutionRecord>();
        }
      }
    }

    public virtual List<EnvironmentDeploymentExecutionRecord> QueryEnvironmentDeploymentRequestWithFilters(
      int environmentId,
      Guid scopeId,
      int? resourceIdFilter,
      long? continuationToken,
      int maxRecords = 25)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (QueryEnvironmentDeploymentRequestWithFilters)))
      {
        this.PrepareStoredProcedure("Task.prc_QueryEnvironmentDeploymentRequestWithFilters");
        this.BindInt("@environmentId", environmentId);
        this.BindGuid("@scopeId", scopeId);
        this.BindInt("@resourceIdFilter", resourceIdFilter.HasValue ? resourceIdFilter.Value : 0);
        this.BindLong("@continuationToken", continuationToken.HasValue ? continuationToken.Value : long.MaxValue);
        this.BindInt("@maxRecords", maxRecords);
        List<EnvironmentDeploymentExecutionRecord> deploymentExecutionRecordList = new List<EnvironmentDeploymentExecutionRecord>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentDeploymentExecutionRecord>((ObjectBinder<EnvironmentDeploymentExecutionRecord>) new EnvironmentDeploymentExecutionRecordBinder(this));
          deploymentExecutionRecordList.AddRange((IEnumerable<EnvironmentDeploymentExecutionRecord>) resultCollection.GetCurrent<EnvironmentDeploymentExecutionRecord>().Items);
          return deploymentExecutionRecordList;
        }
      }
    }

    public virtual List<TaskOrchestrationOwner> GetDeployedPipelineDefinitions(
      int environmentId,
      string planType,
      Guid scopeId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetDeployedPipelineDefinitions)))
      {
        this.PrepareStoredProcedure("Task.prc_GetDeployedPipelineDefinitions");
        this.BindInt("@environmentId", environmentId);
        this.BindString("@planType", planType, 260, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindGuid("@scopeId", scopeId);
        List<TaskOrchestrationOwner> pipelineDefinitions = new List<TaskOrchestrationOwner>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationOwner>((ObjectBinder<TaskOrchestrationOwner>) new TaskOrchestrationOwnerBinder((TaskSqlComponentBase) this, "Definition"));
          pipelineDefinitions.AddRange((IEnumerable<TaskOrchestrationOwner>) resultCollection.GetCurrent<TaskOrchestrationOwner>().Items);
          return pipelineDefinitions;
        }
      }
    }

    public virtual List<TaskOrchestrationOwner> QueryEnvironmentPreviousDeployments(
      int environmentId,
      string planType,
      Guid scopeId,
      int definitionId,
      int ownerId,
      int maxRecords,
      int noOfDaysToLookBack)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (QueryEnvironmentPreviousDeployments)))
      {
        this.PrepareStoredProcedure("Task.prc_QueryEnvironmentPreviousDeployments");
        this.BindInt("@environmentId", environmentId);
        this.BindString("@planType", planType, 260, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindGuid("@scopeId", scopeId);
        this.BindInt("@definitionId", definitionId);
        this.BindInt("@ownerId", ownerId);
        this.BindInt("@maxRecords", maxRecords);
        this.BindInt("@noOfDaysToLookBack", noOfDaysToLookBack);
        List<TaskOrchestrationOwner> orchestrationOwnerList = new List<TaskOrchestrationOwner>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationOwner>((ObjectBinder<TaskOrchestrationOwner>) new TaskOrchestrationOwnerBinder((TaskSqlComponentBase) this, "Owner"));
          orchestrationOwnerList.AddRange((IEnumerable<TaskOrchestrationOwner>) resultCollection.GetCurrent<TaskOrchestrationOwner>().Items);
          return orchestrationOwnerList;
        }
      }
    }

    public virtual EnvironmentResourceDeploymentExecutionRecord QueueEnvironmentResourceDeploymentRequest(
      EnvironmentResourceDeploymentExecutionRecord record)
    {
      return new EnvironmentResourceDeploymentExecutionRecord();
    }

    public virtual EnvironmentResourceDeploymentExecutionRecord UpdateEnvironmentResourceDeploymentRequest(
      int environmentId,
      long requestId,
      int resourceId,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      return new EnvironmentResourceDeploymentExecutionRecord();
    }

    public virtual IList<EnvironmentDeploymentExecutionRecord> GetLastSuccessfulDeploymentByRunIdOrJobs(
      Guid scopeId,
      string planType,
      int environmentId,
      int definitionId,
      int ownerId,
      string stageName,
      IEnumerable<string> jobs)
    {
      return (IList<EnvironmentDeploymentExecutionRecord>) new List<EnvironmentDeploymentExecutionRecord>()
      {
        new EnvironmentDeploymentExecutionRecord()
      };
    }

    public virtual EnvironmentDeploymentExecutionRecord GetLastSuccessfulDeploymentByRunIdAndJobAttempt(
      Guid scopeId,
      string planType,
      int environmentId,
      int definitionId,
      int ownerId,
      string stageName,
      string jobName,
      int jobAttempt)
    {
      return new EnvironmentDeploymentExecutionRecord();
    }

    public virtual IList<EnvironmentDeploymentExecutionRecord> GetEnvironmentDeploymentRequestsByDate(
      Guid scopeId,
      DateTime fromDate,
      int maxRecords)
    {
      return (IList<EnvironmentDeploymentExecutionRecord>) new List<EnvironmentDeploymentExecutionRecord>();
    }
  }
}
