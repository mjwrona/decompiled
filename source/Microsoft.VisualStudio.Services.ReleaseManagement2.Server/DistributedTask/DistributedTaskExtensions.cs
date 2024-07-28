// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.DistributedTaskExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Extension class")]
  public static class DistributedTaskExtensions
  {
    public static TaskInstance ToTaskInstance(this WorkflowTask releaseTask) => releaseTask != null ? releaseTask.ToTaskInstance((IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) null, (IVssRequestContext) null) : throw new ArgumentNullException(nameof (releaseTask));

    public static TaskInstance ToTaskInstance(
      this WorkflowTask releaseTask,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables,
      IVssRequestContext context)
    {
      if (releaseTask == null)
        throw new ArgumentNullException(nameof (releaseTask));
      TaskInstance taskInstance1 = new TaskInstance();
      taskInstance1.Id = releaseTask.TaskId;
      taskInstance1.Version = releaseTask.Version;
      taskInstance1.DisplayName = releaseTask.Name;
      taskInstance1.AlwaysRun = releaseTask.AlwaysRun;
      taskInstance1.ContinueOnError = releaseTask.ContinueOnError;
      taskInstance1.Enabled = releaseTask.Enabled;
      taskInstance1.TimeoutInMinutes = releaseTask.TimeoutInMinutes;
      taskInstance1.RefName = releaseTask.RefName;
      TaskInstance taskInstance2 = taskInstance1;
      if (context != null)
      {
        taskInstance2.Condition = releaseTask.Condition;
        if (TaskConditions.IsLegacyAlwaysRun(taskInstance2.Condition))
          taskInstance2.AlwaysRun = true;
      }
      IDictionary<string, string> overrideInputs = releaseTask.OverrideInputs;
      if (variables != null && overrideInputs != null)
        taskInstance2.TimeoutInMinutes = DistributedTaskExtensions.GetOverriddenValue("TimeoutInMinutes", overrideInputs, taskInstance2.TimeoutInMinutes, variables, context);
      taskInstance2.Inputs.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) releaseTask.Inputs);
      taskInstance2.Environment.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) releaseTask.Environment);
      return taskInstance2;
    }

    public static TaskStep ToTaskStep(
      this WorkflowTask releaseTask,
      IDictionary<string, string> variables,
      IVssRequestContext context)
    {
      if (releaseTask == null)
        throw new ArgumentNullException(nameof (releaseTask));
      if (variables == null)
        throw new ArgumentNullException(nameof (variables));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      TaskStep taskStep1 = new TaskStep();
      taskStep1.Reference = new TaskStepDefinitionReference()
      {
        Id = releaseTask.TaskId,
        Version = releaseTask.Version
      };
      taskStep1.DisplayName = releaseTask.Name;
      taskStep1.Name = releaseTask.RefName;
      taskStep1.ContinueOnError = releaseTask.ContinueOnError;
      taskStep1.Enabled = releaseTask.Enabled;
      taskStep1.TimeoutInMinutes = releaseTask.TimeoutInMinutes;
      taskStep1.RetryCountOnTaskFailure = releaseTask.RetryCountOnTaskFailure;
      TaskStep taskStep2 = taskStep1;
      if (!string.IsNullOrEmpty(releaseTask.Condition))
        taskStep2.Condition = releaseTask.Condition;
      else if (releaseTask.AlwaysRun)
        taskStep2.Condition = "succeededOrFailed()";
      else
        taskStep2.Condition = "succeeded()";
      IDictionary<string, string> overrideInputs = releaseTask.OverrideInputs;
      if (overrideInputs != null)
        taskStep2.TimeoutInMinutes = DistributedTaskExtensions.GetOverriddenValue("TimeoutInMinutes", overrideInputs, taskStep2.TimeoutInMinutes, context, variables);
      if (overrideInputs != null)
        taskStep2.RetryCountOnTaskFailure = DistributedTaskExtensions.GetOverriddenValue("RetryCountOnTaskFailure", overrideInputs, taskStep2.RetryCountOnTaskFailure, context, variables);
      taskStep2.Inputs.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) releaseTask.Inputs);
      taskStep2.Environment.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) releaseTask.Environment);
      return taskStep2;
    }

    public static TaskStep ToTaskStep(
      this TaskInstance taskInstance,
      IDictionary<string, string> variables,
      IVssRequestContext context)
    {
      if (taskInstance == null)
        throw new ArgumentNullException(nameof (taskInstance));
      if (variables == null)
        throw new ArgumentNullException(nameof (variables));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      TaskStep taskStep1 = new TaskStep();
      taskStep1.Reference = new TaskStepDefinitionReference()
      {
        Id = taskInstance.Id,
        Version = taskInstance.Version
      };
      taskStep1.Name = taskInstance.Name;
      taskStep1.DisplayName = taskInstance.DisplayName;
      taskStep1.ContinueOnError = taskInstance.ContinueOnError;
      taskStep1.Enabled = taskInstance.Enabled;
      taskStep1.TimeoutInMinutes = taskInstance.TimeoutInMinutes;
      taskStep1.RetryCountOnTaskFailure = taskInstance.RetryCountOnTaskFailure;
      TaskStep taskStep2 = taskStep1;
      if (!string.IsNullOrEmpty(taskInstance.Condition))
        taskStep2.Condition = taskInstance.Condition;
      else if (taskInstance.AlwaysRun)
        taskStep2.Condition = "succeededOrFailed()";
      else
        taskStep2.Condition = "succeeded()";
      taskStep2.Inputs.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) taskInstance.Inputs);
      taskStep2.Environment.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) taskInstance.Environment);
      return taskStep2;
    }

    private static int GetOverriddenValue(
      string key,
      IDictionary<string, string> taskOverrideInputs,
      int defaultValue,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables,
      IVssRequestContext contex)
    {
      Dictionary<string, string> dictionary = variables.Where<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, bool>) (v => v.Value != null && !v.Value.IsSecret)).ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (v => v.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (v => v.Value.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return DistributedTaskExtensions.GetOverriddenValue(key, taskOverrideInputs, defaultValue, contex, (IDictionary<string, string>) dictionary);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static int GetOverriddenValue(
      string key,
      IDictionary<string, string> taskOverrideInputs,
      int defaultValue,
      IVssRequestContext context,
      IDictionary<string, string> environmentVariables)
    {
      taskOverrideInputs = (IDictionary<string, string>) taskOverrideInputs.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (v => v.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string inputValue;
      if (!taskOverrideInputs.TryGetValue(key, out inputValue))
        return defaultValue;
      string s = BuildCommonUtil.ExpandEnvironmentVariables(inputValue, environmentVariables, (Func<string, string, string>) ((m, e) => e));
      int overriddenValue = 0;
      ref int local = ref overriddenValue;
      if (int.TryParse(s, out local) && overriddenValue >= 0)
        return overriddenValue;
      if (context != null)
        context.Trace(1950021, TraceLevel.Info, "ReleaseManagementService", "DistributedTask", "ToTaskInstance: Unable to resolve override input '{0}' with value '{1}'.", (object) key, (object) inputValue);
      return defaultValue;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "To be refactored.")]
    public static IOrchestrationProcess GetPipelineProcess(
      this DeployPhaseSnapshot phaseSnapshot,
      IVssRequestContext context,
      Guid projectId,
      AutomationEngineInput input)
    {
      Phase pipelinePhase = DistributedTaskExtensions.ToPipelinePhase(phaseSnapshot, context, projectId, input);
      pipelinePhase.Condition = (string) null;
      return (IOrchestrationProcess) DistributedTaskExtensions.GetPipelineProcess(projectId, input, (IList<PhaseNode>) new List<PhaseNode>()
      {
        (PhaseNode) pipelinePhase
      });
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "To be refactored.")]
    public static IOrchestrationProcess GetPipelineProcess(
      this IList<DeployPhaseSnapshot> phaseSnapshots,
      IVssRequestContext context,
      Guid projectId,
      AutomationEngineInput input)
    {
      phaseSnapshots = (IList<DeployPhaseSnapshot>) phaseSnapshots.OrderBy<DeployPhaseSnapshot, int>((Func<DeployPhaseSnapshot, int>) (x => x.Rank)).ToList<DeployPhaseSnapshot>();
      List<PhaseNode> phases = new List<PhaseNode>();
      string str = (string) null;
      for (int index = 0; index < phaseSnapshots.Count; ++index)
      {
        Phase pipelinePhase = DistributedTaskExtensions.ToPipelinePhase(phaseSnapshots[index], context, projectId, input);
        if (str != null)
          pipelinePhase.DependsOn.Add(str);
        phases.Add((PhaseNode) pipelinePhase);
        str = pipelinePhase.Name;
      }
      return (IOrchestrationProcess) DistributedTaskExtensions.GetPipelineProcess(projectId, input, (IList<PhaseNode>) phases);
    }

    public static ParallelExecutionOptions ToParallelExecutionOptions(
      this ExecutionInput execution,
      IDictionary<string, string> variables)
    {
      if (execution == null)
        return (ParallelExecutionOptions) null;
      if (execution is NoneExecutionInput)
        return (ParallelExecutionOptions) null;
      ParallelExecutionOptions executionOptions = new ParallelExecutionOptions();
      if (execution.ParallelExecutionType == ParallelExecutionTypes.MultiMachine)
      {
        MultiMachineInput multiMachineInput = (MultiMachineInput) execution;
        executionOptions.MaxConcurrency = (ExpressionValue<int>) multiMachineInput.MaxNumberOfAgents;
      }
      if (execution.ParallelExecutionType == ParallelExecutionTypes.MultiConfiguration)
      {
        MultiConfigInput multiplierOptions = (MultiConfigInput) execution;
        executionOptions.MaxConcurrency = (ExpressionValue<int>) multiplierOptions.MaxNumberOfAgents;
        executionOptions.Matrix = (ExpressionValue<IDictionary<string, IDictionary<string, string>>>) (IDictionary<string, IDictionary<string, string>>) multiplierOptions.GetMatrix(variables, true);
      }
      return executionOptions;
    }

    private static Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineProcess GetPipelineProcess(
      Guid projectId,
      AutomationEngineInput input,
      IList<PhaseNode> phases)
    {
      Stage stage = new Stage(DistributedTaskExtensions.GetStageName(projectId, input), phases);
      string str;
      if (input.Data != null && input.Data.TryGetValue("ReleaseEnvironmentName", out str))
        stage.DisplayName = str;
      return new Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineProcess((IList<Stage>) new List<Stage>()
      {
        stage
      });
    }

    private static string GetStageName(Guid projectId, AutomationEngineInput input)
    {
      string str;
      input.Data.TryGetValue("ReleaseDefinitionEnvironmentId", out str);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Stage_{0}_{1}_{2}", (object) projectId.ToString("N"), (object) str, (object) input.AttemptNumber);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Reviewed.")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safe handle exception.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "To be refactored.")]
    private static Phase ToPipelinePhase(
      DeployPhaseSnapshot phaseSnapshot,
      IVssRequestContext context,
      Guid projectId,
      AutomationEngineInput input)
    {
      if (phaseSnapshot == null)
        throw new ArgumentNullException(nameof (phaseSnapshot));
      Phase phase = new Phase();
      phase.DisplayName = phaseSnapshot.Name;
      Phase pipelinePhase = phase;
      pipelinePhase.Name = !string.IsNullOrEmpty(phaseSnapshot.RefName) ? phaseSnapshot.RefName : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Phase_{0}", (object) phaseSnapshot.Rank);
      if (context.IsFeatureEnabled("VisualStudio.ReleaseManagement.PipelineOrchestrator.PurgeInvisibleEndpoints"))
      {
        try
        {
          DistributedTaskExtensions.PurgeInvisibleEndpoints(context, phaseSnapshot.Workflow, input.Variables.ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (x => x.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (x => x.Value.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
          context.TraceException(1976501, "ReleaseManagementService", "DistributedTask", ex);
        }
      }
      foreach (WorkflowTask releaseTask in (IEnumerable<WorkflowTask>) phaseSnapshot.Workflow)
        pipelinePhase.Steps.Add((Step) releaseTask.ToTaskStep((IDictionary<string, string>) new Dictionary<string, string>(), context));
      Dictionary<string, string> dictionary = input.Variables.Where<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, bool>) (x => !x.Value.IsSecret)).ToDictionary<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string, string>((Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (x => x.Key), (Func<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>, string>) (x => x.Value.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (phaseSnapshot.PhaseType == DeployPhaseTypes.AgentBasedDeployment)
      {
        AgentDeploymentInput deploymentInput = phaseSnapshot.GetDeploymentInput<AgentDeploymentInput>();
        AgentQueueTarget agentQueueTarget1 = new AgentQueueTarget();
        agentQueueTarget1.Queue = new AgentQueueReference()
        {
          Id = deploymentInput.QueueId
        };
        agentQueueTarget1.TimeoutInMinutes = (ExpressionValue<int>) deploymentInput.TimeoutInMinutes;
        agentQueueTarget1.CancelTimeoutInMinutes = (ExpressionValue<int>) deploymentInput.JobCancelTimeoutInMinutes;
        agentQueueTarget1.Execution = deploymentInput.ParallelExecution.ToParallelExecutionOptions((IDictionary<string, string>) dictionary);
        AgentSpecification agentSpecification = deploymentInput.AgentSpecification;
        agentQueueTarget1.AgentSpecification = agentSpecification != null ? agentSpecification.ToJObject() : (JObject) null;
        AgentQueueTarget agentQueueTarget2 = agentQueueTarget1;
        List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> demandList = JsonUtility.FromString<List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>>(JsonUtility.ToString<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Demand>(deploymentInput.Demands));
        if (demandList != null)
        {
          foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.Demand demand in demandList)
            agentQueueTarget2.Demands.Add(demand);
        }
        pipelinePhase.Target = (PhaseTarget) agentQueueTarget2;
        pipelinePhase.Condition = deploymentInput.Condition;
        if (input.ExternalVariableTasks.Any<TaskInstance>())
        {
          int num = 0;
          foreach (TaskInstance externalVariableTask in (IEnumerable<TaskInstance>) input.ExternalVariableTasks)
            pipelinePhase.Steps.Insert(num++, (Step) externalVariableTask.ToTaskStep((IDictionary<string, string>) dictionary, context));
        }
        if (!deploymentInput.SkipArtifactsDownload)
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          List<WorkflowTask> list = ArtifactTaskMapper.GetArtifactTasks(context, projectId, DistributedTaskExtensions.\u003C\u003EO.\u003C0\u003E__GetExtensionArtifacts ?? (DistributedTaskExtensions.\u003C\u003EO.\u003C0\u003E__GetExtensionArtifacts = new Func<IVssRequestContext, IEnumerable<ArtifactTypeBase>>(ExtensionArtifactsRetriever.GetExtensionArtifacts)), input.Artifacts, deploymentInput.ArtifactsDownloadInput, input).ToList<WorkflowTask>();
          int num = 0;
          foreach (WorkflowTask releaseTask in list)
            pipelinePhase.Steps.Insert(num++, (Step) releaseTask.ToTaskStep((IDictionary<string, string>) dictionary, context));
        }
        string str = ArtifactTaskMapper.GetSkipArtifactsDownloadValueForAgent((DeploymentInput) deploymentInput).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        pipelinePhase.Variables.Add((IVariable) new Variable()
        {
          Name = "release.skipartifactsDownload",
          Value = str,
          Secret = false
        });
        pipelinePhase.Variables.Add((IVariable) new Variable()
        {
          Name = WellKnownDistributedTaskVariables.EnableAccessToken,
          Value = deploymentInput.EnableAccessToken.ToString((IFormatProvider) CultureInfo.InvariantCulture),
          Secret = false
        });
      }
      else
      {
        ServerDeploymentInput serverDeploymentInput = phaseSnapshot.PhaseType == DeployPhaseTypes.RunOnServer ? phaseSnapshot.GetDeploymentInput<ServerDeploymentInput>() : throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PhaseType {0} is not valid.", (object) phaseSnapshot.PhaseType));
        ServerTarget serverTarget1 = new ServerTarget();
        serverTarget1.TimeoutInMinutes = (ExpressionValue<int>) serverDeploymentInput.TimeoutInMinutes;
        serverTarget1.CancelTimeoutInMinutes = (ExpressionValue<int>) serverDeploymentInput.JobCancelTimeoutInMinutes;
        serverTarget1.Execution = serverDeploymentInput.ParallelExecution.ToParallelExecutionOptions((IDictionary<string, string>) dictionary);
        ServerTarget serverTarget2 = serverTarget1;
        pipelinePhase.Target = (PhaseTarget) serverTarget2;
        pipelinePhase.Condition = serverDeploymentInput.Condition;
      }
      return pipelinePhase;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void PurgeInvisibleEndpoints(
      IVssRequestContext requestContext,
      IList<WorkflowTask> phaseSnapshotWorkflow,
      Dictionary<string, string> variables)
    {
      ITaskStore taskStore = requestContext.GetService<IPipelineBuilderService>().GetTaskStore(requestContext);
      foreach (WorkflowTask workflowTask in (IEnumerable<WorkflowTask>) phaseSnapshotWorkflow)
      {
        if (workflowTask.Enabled)
        {
          TaskDefinition taskDefinition = taskStore.ResolveTask(workflowTask.TaskId, workflowTask.Version);
          if (taskDefinition != null)
          {
            Dictionary<string, string> taskInputs = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            List<string> stringList = new List<string>();
            foreach (string key in workflowTask.Inputs.Keys)
            {
              string input = workflowTask.Inputs[key];
              if (!string.IsNullOrWhiteSpace(input))
              {
                string str = VariableUtility.ExpandVariables(input.Trim(), (IDictionary<string, string>) variables, false);
                taskInputs[key] = str;
              }
            }
            foreach (TaskInputDefinition serviceEndpointTypeInput in taskDefinition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (x => x.IsEndpointTypeInput())))
            {
              if (!serviceEndpointTypeInput.IsEndpointInputVisible((IDictionary<string, string>) taskInputs))
                stringList.Add(serviceEndpointTypeInput.Name);
            }
            foreach (string key in stringList)
            {
              if (workflowTask.Inputs.ContainsKey(key))
              {
                workflowTask.Inputs.Remove(key);
                requestContext.Trace(1976501, TraceLevel.Info, "ReleaseManagementService", "DistributedTask", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Removed input {0} from task {1}", (object) key, (object) taskDefinition.Name));
              }
            }
          }
        }
      }
    }
  }
}
