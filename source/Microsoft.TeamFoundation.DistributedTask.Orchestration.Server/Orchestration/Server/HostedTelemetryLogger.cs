// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.HostedTelemetryLogger
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class HostedTelemetryLogger : ITelemetryLogger
  {
    private static readonly HostedTelemetryLogger s_instance = new HostedTelemetryLogger();

    public static HostedTelemetryLogger Instance => HostedTelemetryLogger.s_instance;

    public void PublishTaskHubPlanStartedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan)
    {
      CustomerIntelligenceHelper.PublishTaskHubPlanStarted(requestContext, taskHub, plan);
    }

    public void PublishTaskHubPlanCompletedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan)
    {
      CustomerIntelligenceHelper.PublishTaskHubPlanCompleted(requestContext, taskHub, plan);
    }

    public void PublishTaskHubPhaseStartedTelemetry(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      Phase phase,
      ExpandPhaseResult expandPhaseResult)
    {
      CustomerIntelligenceHelper.PublishTaskHubPhaseStarted(requestContext, plan, phase, expandPhaseResult);
    }

    public void PublishTaskHubSendJobTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      int poolId,
      Guid authorizationId,
      Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentJobRequestMessage jobRequest,
      TaskAgentReference agent,
      string planTemplateType)
    {
      HostedTelemetryLogger.LogPlanContext(requestContext, authorizationId, jobRequest.Plan, jobRequest.JobId, jobRequest.JobName);
      IVssRequestContext requestContext1 = requestContext;
      TaskHub taskHub1 = taskHub;
      int num = poolId;
      Guid jobId = jobRequest.JobId;
      string jobDisplayName = jobRequest.JobDisplayName;
      string jobContainer = jobRequest.JobContainer;
      TaskOrchestrationPlanReference plan = jobRequest.Plan;
      IList<JobStep> steps = jobRequest.Steps;
      JobResources resources1 = jobRequest.Resources;
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> list = resources1 != null ? resources1.Endpoints.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) (endpoint => endpoint.ToServiceEndpoint())).ToList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>() : (List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) null;
      Dictionary<string, string> dictionary = jobRequest.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x => !x.Value.IsSecret)).ToDictionary<KeyValuePair<string, VariableValue>, string, string>((Func<KeyValuePair<string, VariableValue>, string>) (k => k.Key), (Func<KeyValuePair<string, VariableValue>, string>) (v => v.Value.Value));
      JobResources resources2 = jobRequest.Resources;
      string version = agent?.Version;
      string str = planTemplateType;
      string osDescription = agent?.OSDescription;
      string planTemplateType1 = str;
      int poolId1 = num;
      CustomerIntelligenceHelper.PublishTaskHubSendJobData(requestContext1, taskHub1, jobId, jobDisplayName, jobContainer, plan, steps, list, (IDictionary<string, string>) dictionary, resources2, version, osDescription, planTemplateType1, poolId1);
    }

    public void PublishTaskHubExecuteTaskTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      Guid authorizationId,
      ServerTaskRequestMessage taskRequest,
      string planTemplateType)
    {
      HostedTelemetryLogger.LogPlanContext(requestContext, authorizationId, taskRequest.Plan, taskRequest.JobId, taskRequest.JobRefName, taskRequest.TaskInstance);
      IVssRequestContext requestContext1 = requestContext;
      TaskHub taskHub1 = taskHub;
      Guid jobId = taskRequest.JobId;
      string jobName = taskRequest.JobName;
      TaskOrchestrationPlanReference plan = taskRequest.Plan;
      List<JobStep> steps = new List<JobStep>();
      steps.Add((JobStep) new TaskStep(taskRequest.TaskInstance));
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> list = taskRequest.Environment.Endpoints.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) (endpoint => endpoint.ToServiceEndpoint())).ToList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>();
      IDictionary<string, string> variables = taskRequest.Environment.Variables;
      string str = planTemplateType;
      string empty = string.Empty;
      string planTemplateType1 = str;
      CustomerIntelligenceHelper.PublishTaskHubSendJobData(requestContext1, taskHub1, jobId, jobName, (string) null, plan, (IList<JobStep>) steps, list, variables, (JobResources) null, "server", empty, planTemplateType1);
    }

    public void PublishTaskHubJobCompletedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan,
      Timeline timeline,
      Guid jobId)
    {
      CustomerIntelligenceHelper.PublishTaskHubJobCompleted(requestContext, taskHub, plan, jobId, timeline);
      ClientTraceHelper.PublishTaskHubTimelineRecordIssues(requestContext, plan.PlanId, jobId, timeline);
    }

    public void PublishPlanQueueEvaluationJobPlanStartedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationQueuedPlan queuedPlan)
    {
      CustomerIntelligenceHelper.PublishPlanQueueEvaluationJobPlanStarted(requestContext, taskHub, queuedPlan);
    }

    public void PublishPlanQueueEvaluationJobTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      int licensingLimit,
      int runnablePlansFetchedCount,
      int startedPlansCount,
      int startedPlanGroupsCount)
    {
      CustomerIntelligenceHelper.PublishPlanQueueEvaluationJobTelemetry(requestContext, taskHub, licensingLimit, runnablePlansFetchedCount, startedPlansCount, startedPlanGroupsCount);
    }

    private static void LogPlanContext(
      IVssRequestContext requestContext,
      Guid authorizationId,
      TaskOrchestrationPlanReference plan,
      Guid jobId,
      string jobRefName,
      TaskInstance task = null)
    {
      string projectName = string.Empty;
      if (plan.ScopeIdentifier != Guid.Empty)
        projectName = requestContext.GetService<IProjectService>().GetProject(requestContext, plan.ScopeIdentifier).Name;
      DistributedTaskEventSource.Log.PublishOrchestrationPlanContext(requestContext.OrchestrationId, authorizationId, requestContext.ServiceHost.InstanceId, plan.ScopeIdentifier, projectName, plan.PlanType, plan.PlanId, jobId, jobRefName, plan.Definition, plan.Owner, task);
    }
  }
}
