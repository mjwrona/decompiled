// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.OnPremTelemetryLogger
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public class OnPremTelemetryLogger : ITelemetryLogger
  {
    public static readonly string BuildCountPerDayRegistryKey = "/Service/TFS/OnPrem/Build/BuildCountPerDay";
    public static readonly string BuildTasksTelemetryRegistryRoot = "/Service/TFS/OnPrem/Build/TaskCountPerDay/";
    public static readonly string BuildsWithEndpointTelemetryRegistryRoot = "/Service/TFS/OnPrem/Build/BuildsWithEndpointCountPerDay/";
    public static readonly string BuildsWithTIAEnabledTelemetryRegistryRoot = "/Service/TFS/OnPrem/Build/BuildsWithTIAEnabledPerDay/";
    public static readonly string BuildsVsTesTDurationTelemetryRegistryRoot = "/Service/TFS/OnPrem/Build/BuildsVsTestDurationInMinPerDay/";
    private static readonly OnPremTelemetryLogger s_instance = new OnPremTelemetryLogger();

    public static OnPremTelemetryLogger Instance => OnPremTelemetryLogger.s_instance;

    public void PublishTaskHubPlanStartedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan)
    {
    }

    public void PublishTaskHubPlanCompletedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan)
    {
    }

    public void PublishTaskHubPhaseStartedTelemetry(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      Phase phase,
      ExpandPhaseResult expandPhaseResult)
    {
    }

    public void PublishTaskHubExecuteTaskTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      Guid authorizationId,
      ServerTaskRequestMessage taskRequest,
      string planTemplateType)
    {
      try
      {
        IVssRequestContext requestContext1 = requestContext;
        TaskOrchestrationPlanReference plan = taskRequest.Plan;
        List<JobStep> steps = new List<JobStep>(1);
        steps.Add((JobStep) new TaskStep(taskRequest.TaskInstance));
        List<ServiceEndpoint> endpoints = taskRequest.Environment.Endpoints;
        IDictionary<string, string> variables = taskRequest.Environment.Variables;
        this.UpdateRegistryEntries(requestContext1, plan, (IList<JobStep>) steps, endpoints, variables);
      }
      catch (Exception ex)
      {
      }
      OnPremTelemetryLogger.LogPlanContext(requestContext, authorizationId, taskRequest.Plan, taskRequest.JobId, taskRequest.JobRefName, taskRequest.TaskInstance);
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
      try
      {
        this.UpdateRegistryEntries(requestContext, jobRequest.Plan, jobRequest.Steps, jobRequest.Resources?.Endpoints, (IDictionary<string, string>) jobRequest.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x => !x.Value.IsSecret)).ToDictionary<KeyValuePair<string, VariableValue>, string, string>((Func<KeyValuePair<string, VariableValue>, string>) (k => k.Key), (Func<KeyValuePair<string, VariableValue>, string>) (v => v.Value.Value)));
      }
      catch (Exception ex)
      {
      }
      OnPremTelemetryLogger.LogPlanContext(requestContext, authorizationId, jobRequest.Plan, jobRequest.JobId, jobRequest.JobName);
    }

    public void PublishTaskHubJobCompletedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan,
      Timeline timelines)
    {
    }

    public void PublishTaskHubJobCompletedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationPlan plan,
      Timeline timeline,
      Guid jobId)
    {
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        VariableValue variableValue1;
        VariableValue variableValue2;
        if (!service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false) || !string.Equals(plan.PlanType, "Build", StringComparison.OrdinalIgnoreCase) || !plan.ProcessEnvironment.Variables.TryGetValue("system.collectionId", out variableValue1) || !plan.ProcessEnvironment.Variables.TryGetValue("system.definitionId", out variableValue2))
          return;
        RegistryQuery query = (RegistryQuery) string.Format("{0}{1}_{2}", (object) OnPremTelemetryLogger.BuildsVsTesTDurationTelemetryRegistryRoot, (object) variableValue1, (object) variableValue2);
        double averageRunDuration = service.GetValue<double>(vssRequestContext, in query, 0.0);
        double aggregateDuration = OnPremTelemetryHelper.GetVsTestAggregateDuration(timeline, jobId, averageRunDuration);
        if (aggregateDuration == 0.0)
          return;
        RegistryEntry[] registryEntryArray = new RegistryEntry[1]
        {
          new RegistryEntry(string.Format("{0}{1}_{2}", (object) OnPremTelemetryLogger.BuildsVsTesTDurationTelemetryRegistryRoot, (object) variableValue1, (object) variableValue2), aggregateDuration.ToString((IFormatProvider) CultureInfo.InvariantCulture))
        };
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryArray);
      }
      catch (Exception ex)
      {
      }
    }

    public void PublishPlanQueueEvaluationJobPlanStartedTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      TaskOrchestrationQueuedPlan queuedPlan)
    {
    }

    public void PublishPlanQueueEvaluationJobTelemetry(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      int licensingLimit,
      int runnablePlansFetchedCount,
      int startedPlansCount,
      int startedPlanGroupsCount)
    {
    }

    private void UpdateRegistryEntries(
      IVssRequestContext requestContext,
      TaskOrchestrationPlanReference plan,
      IList<JobStep> steps,
      List<ServiceEndpoint> endpointsInUse,
      IDictionary<string, string> environmentVariables)
    {
      try
      {
        List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
        if (!service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.CollectTeamFoundationServerSqmData, false) || !string.Equals(plan.PlanType, "Build", StringComparison.OrdinalIgnoreCase))
          return;
        int num1 = service.GetValue<int>(vssRequestContext, (RegistryQuery) OnPremTelemetryLogger.BuildCountPerDayRegistryKey, 0) + 1;
        registryEntryList.Add(new RegistryEntry(OnPremTelemetryLogger.BuildCountPerDayRegistryKey, num1.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        string environmentVariable1 = environmentVariables["system.collectionId"];
        string environmentVariable2 = environmentVariables["system.definitionId"];
        foreach (JobStep step1 in (IEnumerable<JobStep>) steps)
        {
          if (step1.Type == StepType.Task)
          {
            TaskStep task = step1 as TaskStep;
            registryEntryList.AddRange((IEnumerable<RegistryEntry>) this.UpdateTaskStepRegistryEntries(vssRequestContext, service, task, environmentVariable1, environmentVariable2));
          }
          else if (step1.Type == StepType.Group)
          {
            foreach (TaskStep step2 in (IEnumerable<TaskStep>) (step1 as GroupStep).Steps)
              registryEntryList.AddRange((IEnumerable<RegistryEntry>) this.UpdateTaskStepRegistryEntries(vssRequestContext, service, step2, environmentVariable1, environmentVariable2));
          }
        }
        if (endpointsInUse != null)
        {
          foreach (string str1 in endpointsInUse.Select<ServiceEndpoint, string>((Func<ServiceEndpoint, string>) (e => e.Type)).Where<string>((Func<string, bool>) (e => !string.IsNullOrWhiteSpace(e))).Distinct<string>())
          {
            string str2 = OnPremTelemetryLogger.BuildsWithEndpointTelemetryRegistryRoot + str1;
            int num2 = service.GetValue<int>(vssRequestContext, (RegistryQuery) str2, 0) + 1;
            registryEntryList.Add(new RegistryEntry(str2, num2.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          }
        }
        service.WriteEntries(vssRequestContext, (IEnumerable<RegistryEntry>) registryEntryList);
      }
      catch (Exception ex)
      {
      }
    }

    private List<RegistryEntry> UpdateTaskStepRegistryEntries(
      IVssRequestContext deploymentLevelRequestContext,
      CachedRegistryService registryService,
      TaskStep task,
      string collectionId,
      string definitionId)
    {
      List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
      int num = registryService.GetValue<int>(deploymentLevelRequestContext, (RegistryQuery) (OnPremTelemetryLogger.BuildTasksTelemetryRegistryRoot + task.Reference.Id.ToString()), 0) + 1;
      registryEntryList.Add(new RegistryEntry(OnPremTelemetryLogger.BuildTasksTelemetryRegistryRoot + task.Reference.Id.ToString(), num.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      if (task.Inputs.ContainsKey("runOnlyImpactedTests") && string.Equals(task.Inputs["runOnlyImpactedTests"], "True", StringComparison.OrdinalIgnoreCase))
        registryEntryList.Add(new RegistryEntry(string.Format("{0}{1}_{2}", (object) OnPremTelemetryLogger.BuildsWithTIAEnabledTelemetryRegistryRoot, (object) collectionId, (object) definitionId), "1"));
      return registryEntryList;
    }

    private static void LogPlanContext(
      IVssRequestContext requestContext,
      Guid authorizationId,
      TaskOrchestrationPlanReference plan,
      Guid jobId,
      string jobRefName,
      TaskInstance task = null)
    {
      DistributedTaskEventSource.Log.PublishOrchestrationPlanContext(requestContext.OrchestrationId, authorizationId, requestContext.ServiceHost.InstanceId, plan.ScopeIdentifier, string.Empty, plan.PlanType, plan.PlanId, jobId, jobRefName, plan.Definition, plan.Owner, task);
    }
  }
}
