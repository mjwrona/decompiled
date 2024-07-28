// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IEnvironmentDeploymentExecutionHistoryService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.Data.Model;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (PlatformEnvironmentDeploymentExecutionHistoryService))]
  public interface IEnvironmentDeploymentExecutionHistoryService : 
    IVssFrameworkService,
    IDistributedTaskEnvironmentDeploymentHistoryService
  {
    EnvironmentDeploymentExecutionRecord QueueEnvironmentDeploymentRequest(
      IVssRequestContext requestContext,
      EnvironmentDeploymentExecutionRecord request);

    EnvironmentDeploymentExecutionRecord UpdateEnvironmentDeploymentRequest(
      IVssRequestContext requestContext,
      int environmentId,
      long requestId,
      DateTime? startTime,
      DateTime? finishTime,
      TaskResult? result);

    IPagedList<EnvironmentDeploymentExecutionRecord> GetEnvironmentDeploymentExecutionRecords(
      IVssRequestContext requestContext,
      int environmentId,
      Guid scopeId,
      string continuationToken,
      int maxRecords);

    IList<TaskOrchestrationOwner> GetDeployedPipelineDefinitions(
      IVssRequestContext requestContext,
      int environmentId,
      string planType,
      Guid scopeId);

    IList<DeploymentExecutionRecordObject> GetEnvironmentDeploymentRequestsByOwnerId(
      IVssRequestContext requestContext,
      Guid scopeId,
      int ownerId,
      string planType);

    EnvironmentResourceDeploymentExecutionRecord QueueEnvironmentResourceDeploymentRequest(
      IVssRequestContext requestContext,
      EnvironmentResourceDeploymentExecutionRecord request);

    EnvironmentResourceDeploymentExecutionRecord UpdateEnvironmentResourceDeploymentRequest(
      IVssRequestContext requestContext,
      int environmentId,
      long requestId,
      int resourceId,
      DateTime? finishTime,
      TaskResult? result);

    IDictionary<string, EnvironmentDeploymentExecutionRecord> GetLastSuccessfulDeploymentByRunIdOrJobs(
      IVssRequestContext requestContext,
      Guid scopeId,
      string planType,
      int environmentId,
      int definitionId,
      int ownerId,
      string stageName,
      IList<string> jobs = null);

    EnvironmentDeploymentExecutionRecord GetLastSuccessfulDeploymentByRunIdAndJobAttempt(
      IVssRequestContext requestContext,
      Guid scopeId,
      string planType,
      int environmentId,
      int definitionId,
      int ownerId,
      string stageName,
      string jobName,
      int jobAttempt);

    IList<EnvironmentDeploymentExecutionRecord> GetEnvironmentDeploymentRequestsByDate(
      IVssRequestContext requestContext,
      Guid scopeId,
      DateTime fromDate,
      int maxRecords);
  }
}
