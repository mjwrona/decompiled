// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PlatformEnvironmentDeploymentExecutionHistoryService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.Data.Model;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class PlatformEnvironmentDeploymentExecutionHistoryService : 
    IEnvironmentDeploymentExecutionHistoryService,
    IVssFrameworkService,
    IDistributedTaskEnvironmentDeploymentHistoryService
  {
    private readonly EnvironmentSecurityProvider m_securityProvider;
    private readonly RegistryQuery s_readAXDataFromReplica = new RegistryQuery("/Service/DistributedTaskServer/Settings/ReadAXDataFromReplica");

    internal PlatformEnvironmentDeploymentExecutionHistoryService()
      : this(new EnvironmentSecurityProvider())
    {
    }

    protected PlatformEnvironmentDeploymentExecutionHistoryService(
      EnvironmentSecurityProvider securityProvider)
    {
      this.m_securityProvider = securityProvider;
    }

    public EnvironmentDeploymentExecutionRecord QueueEnvironmentDeploymentRequest(
      IVssRequestContext requestContext,
      EnvironmentDeploymentExecutionRecord executionRecord)
    {
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (QueueEnvironmentDeploymentRequest)))
      {
        ArgumentValidation.CheckEnvironmentDeploymentExecutionRecord(executionRecord, nameof (executionRecord));
        this.m_securityProvider.CheckManageHistoryPermissions(requestContext);
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>())
          return component.QueueEnvironmentDeploymentRequest(executionRecord);
      }
    }

    public IPagedList<EnvironmentDeploymentExecutionRecord> GetEnvironmentDeploymentExecutionRecords(
      IVssRequestContext requestContext,
      int environmentId,
      Guid scopeId,
      string continuationToken,
      int maxRecords)
    {
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (GetEnvironmentDeploymentExecutionRecords)))
      {
        ArgumentUtility.CheckForNonPositiveInt(environmentId, nameof (environmentId), "DistributedTask");
        this.m_securityProvider.CheckViewHistoryPermissions(requestContext, new Guid?(scopeId), environmentId);
        long result;
        long.TryParse(continuationToken, out result);
        IList<EnvironmentDeploymentExecutionRecord> deploymentExecutionRecordList;
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>())
          deploymentExecutionRecordList = component.GetEnvironmentDeploymentRequests(environmentId, new long?(result), maxRecords + 1);
        string continuationToken1 = (string) null;
        if (deploymentExecutionRecordList.Count > maxRecords)
        {
          continuationToken1 = deploymentExecutionRecordList[maxRecords].Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          deploymentExecutionRecordList = (IList<EnvironmentDeploymentExecutionRecord>) deploymentExecutionRecordList.Take<EnvironmentDeploymentExecutionRecord>(maxRecords).ToList<EnvironmentDeploymentExecutionRecord>();
        }
        return (IPagedList<EnvironmentDeploymentExecutionRecord>) new PagedList<EnvironmentDeploymentExecutionRecord>((IEnumerable<EnvironmentDeploymentExecutionRecord>) deploymentExecutionRecordList, continuationToken1);
      }
    }

    public EnvironmentDeploymentExecutionRecord UpdateEnvironmentDeploymentRequest(
      IVssRequestContext requestContext,
      int environmentId,
      long requestId,
      DateTime? startTime,
      DateTime? finishTime,
      TaskResult? result)
    {
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (UpdateEnvironmentDeploymentRequest)))
      {
        this.m_securityProvider.CheckManageHistoryPermissions(requestContext);
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>())
          return component.UpdateEnvironmentDeploymentRequest(environmentId, requestId, startTime, finishTime, result);
      }
    }

    public IList<EnvironmentDeploymentExecutionRecord> QueryEnvironmentDeploymentExecutionRecordsWithFilters(
      IVssRequestContext requestContext,
      int environmentId,
      Guid scopeId,
      int? resourceId,
      string continuationToken,
      int maxRecords = 25)
    {
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (QueryEnvironmentDeploymentExecutionRecordsWithFilters)))
      {
        ArgumentUtility.CheckForNonPositiveInt(environmentId, nameof (environmentId), "DistributedTask");
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId), "DistributedTask");
        this.m_securityProvider.CheckViewHistoryPermissions(requestContext, new Guid?(scopeId), environmentId);
        long result;
        if (!long.TryParse(continuationToken, out result) || result == 0L)
          result = long.MaxValue;
        IList<EnvironmentDeploymentExecutionRecord> deploymentExecutionRecordList;
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>())
          deploymentExecutionRecordList = (IList<EnvironmentDeploymentExecutionRecord>) component.QueryEnvironmentDeploymentRequestWithFilters(environmentId, scopeId, resourceId, new long?(result), maxRecords);
        return deploymentExecutionRecordList;
      }
    }

    public IList<TaskOrchestrationOwner> GetDeployedPipelineDefinitions(
      IVssRequestContext requestContext,
      int environmentId,
      string planType,
      Guid scopeId)
    {
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (GetDeployedPipelineDefinitions)))
      {
        ArgumentUtility.CheckForNonPositiveInt(environmentId, nameof (environmentId), "DistributedTask");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(planType, nameof (planType), "DistributedTask");
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId), "DistributedTask");
        this.m_securityProvider.CheckViewHistoryPermissions(requestContext, new Guid?(scopeId), environmentId);
        IList<TaskOrchestrationOwner> pipelineDefinitions;
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>())
          pipelineDefinitions = (IList<TaskOrchestrationOwner>) component.GetDeployedPipelineDefinitions(environmentId, planType, scopeId);
        return pipelineDefinitions;
      }
    }

    public IList<TaskOrchestrationOwner> QueryEnvironmentPreviousDeployments(
      IVssRequestContext requestContext,
      int environmentId,
      string planType,
      Guid scopeId,
      int definitionId,
      int ownerId,
      int maxRecords = 500,
      int daysToLookBack = 365)
    {
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (QueryEnvironmentPreviousDeployments)))
      {
        ArgumentUtility.CheckForNonPositiveInt(environmentId, nameof (environmentId), "DistributedTask");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(planType, nameof (planType), "DistributedTask");
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId), "DistributedTask");
        ArgumentUtility.CheckForNonPositiveInt(definitionId, nameof (definitionId), "DistributedTask");
        ArgumentUtility.CheckForNonPositiveInt(ownerId, nameof (ownerId), "DistributedTask");
        if (maxRecords <= 0 || maxRecords > 500)
          maxRecords = 500;
        if (daysToLookBack <= 0 || daysToLookBack > 365)
          daysToLookBack = 365;
        this.m_securityProvider.CheckViewHistoryPermissions(requestContext, new Guid?(scopeId), environmentId);
        IList<TaskOrchestrationOwner> orchestrationOwnerList;
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>())
          orchestrationOwnerList = (IList<TaskOrchestrationOwner>) component.QueryEnvironmentPreviousDeployments(environmentId, planType, scopeId, definitionId, ownerId, maxRecords, daysToLookBack);
        return orchestrationOwnerList;
      }
    }

    public virtual IList<DeploymentExecutionRecordObject> GetEnvironmentDeploymentRequestsByOwnerId(
      IVssRequestContext requestContext,
      Guid scopeId,
      int ownerId,
      string planType)
    {
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (GetEnvironmentDeploymentRequestsByOwnerId)))
      {
        ArgumentUtility.CheckForNonPositiveInt(ownerId, nameof (ownerId), "DistributedTask");
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId), "DistributedTask");
        IList<DeploymentExecutionRecordObject> requestsByOwnerId;
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>())
          requestsByOwnerId = component.GetEnvironmentDeploymentRequestsByOwnerId(scopeId, ownerId, planType);
        return this.GetFilteredDeploymentExecutionRecordObject(requestContext, scopeId, requestsByOwnerId);
      }
    }

    public EnvironmentResourceDeploymentExecutionRecord QueueEnvironmentResourceDeploymentRequest(
      IVssRequestContext requestContext,
      EnvironmentResourceDeploymentExecutionRecord executionRecord)
    {
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (QueueEnvironmentResourceDeploymentRequest)))
      {
        ArgumentValidation.CheckEnvironmentResourceDeploymentExecutionRecord(executionRecord, nameof (executionRecord));
        this.m_securityProvider.CheckManageHistoryPermissions(requestContext);
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>())
          return component.QueueEnvironmentResourceDeploymentRequest(executionRecord);
      }
    }

    public EnvironmentResourceDeploymentExecutionRecord UpdateEnvironmentResourceDeploymentRequest(
      IVssRequestContext requestContext,
      int environmentId,
      long requestId,
      int resourceId,
      DateTime? finishTime,
      TaskResult? result)
    {
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (UpdateEnvironmentResourceDeploymentRequest)))
      {
        this.m_securityProvider.CheckManageHistoryPermissions(requestContext);
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>())
          return component.UpdateEnvironmentResourceDeploymentRequest(environmentId, requestId, resourceId, finishTime, result);
      }
    }

    public IDictionary<string, EnvironmentDeploymentExecutionRecord> GetLastSuccessfulDeploymentByRunIdOrJobs(
      IVssRequestContext requestContext,
      Guid scopeId,
      string planType,
      int environmentId,
      int definitionId,
      int ownerId,
      string stageName,
      IList<string> jobs = null)
    {
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (GetLastSuccessfulDeploymentByRunIdOrJobs)))
      {
        ArgumentUtility.CheckForNonPositiveInt(definitionId, nameof (definitionId), "DistributedTask");
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId), "DistributedTask");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(planType, nameof (planType), "DistributedTask");
        ArgumentUtility.CheckForNonPositiveInt(environmentId, nameof (environmentId), "DistributedTask");
        ArgumentUtility.CheckForNonPositiveInt(ownerId, nameof (ownerId), "DistributedTask");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(stageName, nameof (stageName), "DistributedTask");
        IList<EnvironmentDeploymentExecutionRecord> deploymentByRunIdOrJobs1;
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>())
          deploymentByRunIdOrJobs1 = component.GetLastSuccessfulDeploymentByRunIdOrJobs(scopeId, planType, environmentId, definitionId, ownerId, stageName, (IEnumerable<string>) jobs);
        Dictionary<string, EnvironmentDeploymentExecutionRecord> dictionary = new Dictionary<string, EnvironmentDeploymentExecutionRecord>();
        foreach (EnvironmentDeploymentExecutionRecord deploymentExecutionRecord in (IEnumerable<EnvironmentDeploymentExecutionRecord>) deploymentByRunIdOrJobs1)
        {
          if (!dictionary.ContainsKey(deploymentExecutionRecord.JobName))
            dictionary[deploymentExecutionRecord.JobName] = deploymentExecutionRecord;
        }
        Dictionary<string, EnvironmentDeploymentExecutionRecord> deploymentByRunIdOrJobs2 = new Dictionary<string, EnvironmentDeploymentExecutionRecord>();
        if (jobs != null)
        {
          foreach (string job in (IEnumerable<string>) jobs)
            deploymentByRunIdOrJobs2[job] = !dictionary.ContainsKey(job) ? (EnvironmentDeploymentExecutionRecord) null : dictionary[job];
        }
        else
          deploymentByRunIdOrJobs2 = dictionary;
        return (IDictionary<string, EnvironmentDeploymentExecutionRecord>) deploymentByRunIdOrJobs2;
      }
    }

    public EnvironmentDeploymentExecutionRecord GetLastSuccessfulDeploymentByRunIdAndJobAttempt(
      IVssRequestContext requestContext,
      Guid scopeId,
      string planType,
      int environmentId,
      int definitionId,
      int ownerId,
      string stageName,
      string jobName,
      int jobAttempt)
    {
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (GetLastSuccessfulDeploymentByRunIdAndJobAttempt)))
      {
        ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId), "DistributedTask");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(planType, nameof (planType), "DistributedTask");
        ArgumentUtility.CheckForNonPositiveInt(environmentId, nameof (environmentId), "DistributedTask");
        ArgumentUtility.CheckForNonPositiveInt(definitionId, nameof (definitionId), "DistributedTask");
        ArgumentUtility.CheckForNonPositiveInt(ownerId, nameof (ownerId), "DistributedTask");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(stageName, nameof (stageName), "DistributedTask");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(jobName, nameof (jobName), "DistributedTask");
        ArgumentUtility.CheckForNonPositiveInt(jobAttempt, nameof (jobAttempt), "DistributedTask");
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          return component.GetLastSuccessfulDeploymentByRunIdAndJobAttempt(scopeId, planType, environmentId, definitionId, ownerId, stageName, jobName, jobAttempt);
      }
    }

    public virtual IList<EnvironmentDeploymentExecutionRecord> GetEnvironmentDeploymentRequestsByDate(
      IVssRequestContext requestContext,
      Guid scopeId,
      DateTime fromDate,
      int maxRecords)
    {
      ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId), "DistributedTask");
      ArgumentUtility.CheckForNonPositiveInt(maxRecords, nameof (maxRecords), "DistributedTask");
      using (new MethodScope(requestContext, "EnvironmentDeploymentHistoryService", nameof (GetEnvironmentDeploymentRequestsByDate)))
      {
        using (EnvironmentDeploymentExecutionHistoryComponent component = requestContext.CreateComponent<EnvironmentDeploymentExecutionHistoryComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          return component.GetEnvironmentDeploymentRequestsByDate(scopeId, fromDate, maxRecords);
      }
    }

    private IList<DeploymentExecutionRecordObject> GetFilteredDeploymentExecutionRecordObject(
      IVssRequestContext requestContext,
      Guid scopeId,
      IList<DeploymentExecutionRecordObject> executionRecords)
    {
      IList<DeploymentExecutionRecordObject> executionRecordObject = (IList<DeploymentExecutionRecordObject>) new List<DeploymentExecutionRecordObject>();
      if (executionRecords == null || executionRecords.Count == 0)
        return executionRecordObject;
      ISet<int> environmentIds = (ISet<int>) new HashSet<int>();
      executionRecords.ForEach<DeploymentExecutionRecordObject>((Action<DeploymentExecutionRecordObject>) (executionRecord =>
      {
        if (environmentIds.Contains(executionRecord.EnvironmentReference.Id))
          return;
        environmentIds.Add(executionRecord.EnvironmentReference.Id);
      }));
      HashSet<int> filteredEnvironmentIdsHashSet = environmentIds.Where<int>((Func<int, bool>) (id => this.m_securityProvider.HasViewPermissions(requestContext, scopeId, id))).ToHashSet<int>();
      return (IList<DeploymentExecutionRecordObject>) executionRecords.Where<DeploymentExecutionRecordObject>((Func<DeploymentExecutionRecordObject, bool>) (record => filteredEnvironmentIdsHashSet.Contains(record.EnvironmentReference.Id))).ToList<DeploymentExecutionRecordObject>();
    }

    private DatabaseConnectionType GetDatabaseConnectionType(IVssRequestContext requestContext) => !requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in this.s_readAXDataFromReplica, true) ? DatabaseConnectionType.Default : DatabaseConnectionType.IntentReadOnly;

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
