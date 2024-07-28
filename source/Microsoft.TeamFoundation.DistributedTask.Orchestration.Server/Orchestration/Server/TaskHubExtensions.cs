// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskHubExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration.History;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class TaskHubExtensions
  {
    private const string c_ServiceEndPointTaskInputTypePrefix = "connectedService:";
    private const string c_agentVersion = "Agent.Version";
    private const string c_minimumAgentTaskEnvironment = "2.120.0";
    private const string c_minimumAgentConditionalsVersion = "2.115.0";
    private const string c_minimumAgentOutputVariablesVersion = "2.119.0";
    private const string c_secureFileTaskInputType = "secureFile";

    public static TaskLog AppendLog(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      int logId,
      Stream content,
      bool useBlob = true)
    {
      return requestContext.RunSynchronously<TaskLog>((Func<Task<TaskLog>>) (() => hub.AppendLogAsync(requestContext, scopeIdentifier, planId, logId, content, useBlob)));
    }

    public static void CancelPlan(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      TimeSpan timeout,
      string reason)
    {
      requestContext.RunSynchronously((Func<Task>) (() => hub.CancelPlanAsync(requestContext, scopeIdentifier, planId, timeout, reason)));
    }

    public static void CancelStage(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      string StageId,
      TimeSpan timeout,
      string reason)
    {
      requestContext.RunSynchronously((Func<Task>) (() => hub.CancelNodeAsync(requestContext, projectId, planId, StageId, reason, timeout)));
    }

    public static TaskOrchestrationPlan CreatePlan(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string planGroup,
      Uri artifactUri,
      IOrchestrationEnvironment environment,
      IOrchestrationProcess process,
      BuildOptions validationOptions,
      Guid requestedForId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference,
      in string pipelineInitializationLog = null,
      in string pipelineExpandedYaml = null)
    {
      return hub.CreatePlan(requestContext, scopeIdentifier, planId, planGroup, PlanTemplateType.Designer, artifactUri, environment, process, validationOptions, requestedForId, definitionReference, ownerReference, pipelineInitializationLog, pipelineExpandedYaml);
    }

    public static TaskLog CreateLog(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string logPath)
    {
      return requestContext.RunSynchronously<TaskLog>((Func<Task<TaskLog>>) (() => hub.CreateLogAsync(requestContext, scopeIdentifier, planId, logPath)));
    }

    public static Stream GetAttachment(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      TaskAttachment attachment)
    {
      return hub.GetAttachment(requestContext, scopeIdentifier, planId, attachment.TimelineId, attachment.RecordId, attachment.Type, attachment.Name);
    }

    public static Stream GetAttachment(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      return requestContext.RunSynchronously<Stream>((Func<Task<Stream>>) (() => hub.GetAttachmentAsync(requestContext, scopeIdentifier, planId, timelineId, recordId, type, name)));
    }

    public static IList<TaskAttachment> GetAttachments(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      string type,
      Guid? timelineId = null,
      Guid? recordId = null)
    {
      return requestContext.RunSynchronously<IList<TaskAttachment>>((Func<Task<IList<TaskAttachment>>>) (() => hub.GetAttachmentsAsync(requestContext, scopeIdentifier, planId, type, timelineId, recordId)));
    }

    public static TaskOrchestrationJob GetJob(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      out TaskOrchestrationPlan plan)
    {
      GetTaskOrchestrationJobResult orchestrationJobResult = requestContext.RunSynchronously<GetTaskOrchestrationJobResult>((Func<Task<GetTaskOrchestrationJobResult>>) (() => hub.GetJobAsync(requestContext, scopeIdentifier, planId, jobId)));
      plan = orchestrationJobResult.Plan;
      return orchestrationJobResult.Job;
    }

    public static TaskOrchestrationPlan GetPlan(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      bool includeSecretVariables = false)
    {
      return requestContext.RunSynchronously<TaskOrchestrationPlan>((Func<Task<TaskOrchestrationPlan>>) (() => hub.GetPlanAsync(requestContext, scopeIdentifier, planId, includeSecretVariables)));
    }

    public static Job GetAgentRequestJob(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string orchestrationId)
    {
      return requestContext.RunSynchronously<Job>((Func<Task<Job>>) (() => hub.GetAgentRequestJobAsync(requestContext, scopeIdentifier, orchestrationId)));
    }

    public static OutputVariableScope GetPlanOutputVariables(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      return requestContext.RunSynchronously<OutputVariableScope>((Func<Task<OutputVariableScope>>) (() => hub.GetPlanOutputVariablesAsync(requestContext, scopeIdentifier, planId)));
    }

    public static Timeline GetTimeline(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      int changeId = 0,
      bool includeRecords = false,
      bool includeSecretVariables = false)
    {
      return requestContext.RunSynchronously<Timeline>((Func<Task<Timeline>>) (() => hub.GetTimelineAsync(requestContext, scopeIdentifier, planId, timelineId, changeId, includeRecords, includeSecretVariables)));
    }

    internal static void RaiseJobEvent(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      string eventName,
      JobEvent eventData,
      DateTime? fireAt = null)
    {
      requestContext.RunSynchronously((Func<Task>) (() => hub.RaiseJobEventAsync(requestContext, scopeIdentifier, planId, jobId, eventName, eventData, fireAt)));
    }

    internal static void RaiseTaskEvent(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId,
      Guid taskId,
      string eventName,
      TaskEvent eventData,
      DateTime? fireAt = null)
    {
      requestContext.RunSynchronously((Func<Task>) (() => hub.RaiseTaskEventAsync(requestContext, scopeIdentifier, planId, jobId, taskId, eventName, eventData, fireAt)));
    }

    internal static void RaiseOrchestrationEvent(
      this TaskHub hub,
      IVssRequestContext requestContext,
      string instanceId,
      string eventName,
      object eventData,
      bool ensureOrchestrationExist = false)
    {
      requestContext.RunSynchronously((Func<Task>) (() => hub.RaiseOrchestrationEventAsync(requestContext, instanceId, eventName, eventData, ensureOrchestrationExist)));
    }

    public static TaskOrchestrationPlan RunPlan(
      this TaskHub hub,
      IVssRequestContext requestContext,
      TaskAgentPoolReference pool,
      Guid scopeIdentifier,
      Guid planId,
      string planGroup,
      Uri artifactUri,
      IOrchestrationEnvironment environment,
      IOrchestrationProcess process,
      BuildOptions validationOptions,
      Guid requestedForId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference,
      in string pipelineInitializationLog = null,
      in string pipelineExpandedYaml = null)
    {
      return hub.RunPlan(requestContext, pool, scopeIdentifier, planId, planGroup, PlanTemplateType.Designer, artifactUri, environment, process, validationOptions, requestedForId, definitionReference, ownerReference, pipelineInitializationLog, pipelineExpandedYaml);
    }

    public static bool TryCreateJob(
      this TaskHub hub,
      IVssRequestContext requestContext,
      string jobName,
      string jobRefName,
      List<TaskInstance> tasks,
      out List<TaskInstance> missingTasks,
      out TaskOrchestrationJob job,
      List<Demand> jobDemands = null,
      int jobTimeoutInMinutes = 2147483647,
      string minAgentVersion = null,
      string executionMode = "Agent",
      bool includeTaskDemands = true)
    {
      using (new MethodScope(requestContext, "TaskHub", nameof (TryCreateJob)))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckStringForNullOrEmpty(jobName, nameof (jobName));
        bool job1 = true;
        missingTasks = new List<TaskInstance>();
        DemandSource source = (DemandSource) null;
        IList<Demand> demandList = (IList<Demand>) new List<Demand>();
        job = new TaskOrchestrationJob(Guid.NewGuid(), jobName, jobRefName, executionMode);
        TimeSpan timeSpan = TimeSpan.FromMinutes((double) jobTimeoutInMinutes);
        if (timeSpan > TimeSpan.Zero && timeSpan < TimeSpan.FromMilliseconds((double) int.MaxValue))
          job.ExecutionTimeout = new TimeSpan?(timeSpan);
        HashSet<string> tasksSatisfy = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        ITaskStore taskStore = requestContext.GetService<IPipelineBuilderService>().GetTaskStore(requestContext);
        foreach (TaskInstance taskInstance1 in tasks.Where<TaskInstance>((Func<TaskInstance, bool>) (x => x.Enabled)))
        {
          TaskDefinition taskDefinition = taskStore.ResolveTask(taskInstance1.Id, taskInstance1.Version);
          if (taskDefinition == null || taskDefinition.Disabled)
          {
            missingTasks.Add(taskInstance1);
            job1 = false;
            if (taskDefinition == null)
              requestContext.TraceWarning(10015518, "TaskHub", "Couldn't find task definition with id {0} and version {1}", (object) taskInstance1.Id, (object) taskInstance1.Version);
            else
              requestContext.TraceWarning(10015518, "TaskHub", "taskdefinition {0} with name {1} is disabled", (object) taskDefinition.Id, (object) taskDefinition.Name);
          }
          else
          {
            foreach (string satisfy in (IEnumerable<string>) taskDefinition.Satisfies)
              tasksSatisfy.Add(satisfy);
            TaskInstance taskInstance2 = new TaskInstance();
            taskInstance2.Enabled = taskInstance1.Enabled;
            taskInstance2.AlwaysRun = taskInstance1.AlwaysRun;
            taskInstance2.ContinueOnError = taskInstance1.ContinueOnError;
            taskInstance2.TimeoutInMinutes = taskInstance1.TimeoutInMinutes;
            taskInstance2.Id = taskInstance1.Id;
            taskInstance2.RefName = taskInstance1.RefName;
            taskInstance2.Version = (string) taskDefinition.Version;
            taskInstance2.InstanceId = Guid.NewGuid();
            taskInstance2.Name = taskDefinition.Name;
            TaskInstance taskInstance3 = taskInstance2;
            taskInstance3.Condition = taskInstance1.Condition;
            if (TaskConditions.RequiresAgentSupport(taskInstance3.Condition) && DemandMinimumVersion.CompareVersion("2.115.0", minAgentVersion) > 0)
            {
              minAgentVersion = "2.115.0";
              source = new DemandSource()
              {
                SourceName = "Custom conditions",
                SourceType = DemandSourceType.Feature
              };
            }
            if (taskDefinition.OutputVariables.Count > 0)
            {
              minAgentVersion = "2.119.0";
              source = new DemandSource()
              {
                SourceName = "Output variables",
                SourceType = DemandSourceType.Feature
              };
            }
            foreach (KeyValuePair<string, string> input in (IEnumerable<KeyValuePair<string, string>>) taskInstance1.Inputs)
              taskInstance3.Inputs[input.Key] = input.Value;
            foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) taskInstance1.Environment)
              taskInstance3.Environment[keyValuePair.Key] = keyValuePair.Value;
            if (taskInstance1.Environment.Count > 0 && DemandMinimumVersion.CompareVersion("2.120.0", minAgentVersion) > 0)
            {
              minAgentVersion = "2.120.0";
              source = new DemandSource()
              {
                SourceName = "Environments",
                SourceType = DemandSourceType.Feature
              };
            }
            taskInstance3.DisplayName = string.IsNullOrEmpty(taskInstance1.DisplayName) ? taskDefinition.ComputeDisplayName(taskInstance1.Inputs) : taskInstance1.DisplayName;
            if (!requestContext.IsFeatureEnabled("DistributedTask.DisableTaskDefinitionInputsMerging"))
            {
              foreach (TaskInputDefinition taskInputDefinition in taskDefinition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (x => x != null)))
              {
                string key = taskInputDefinition.Name?.Trim() ?? string.Empty;
                if (!string.IsNullOrEmpty(key) && !taskInstance3.Inputs.ContainsKey(key))
                  taskInstance3.Inputs[key] = taskInputDefinition?.DefaultValue?.Trim() ?? string.Empty;
              }
            }
            bool newMinimum;
            minAgentVersion = taskDefinition.GetMinimumAgentVersion(minAgentVersion, out newMinimum);
            if (newMinimum)
              source = new DemandSource()
              {
                SourceName = taskDefinition.Name,
                SourceVersion = (string) taskDefinition.Version,
                SourceType = DemandSourceType.Task
              };
            IEnumerable<Demand> demands = taskDefinition.Demands.Where<Demand>((Func<Demand, bool>) (d => !tasksSatisfy.Contains(d.Name)));
            if (demands.Any<Demand>() & includeTaskDemands)
              demandList.AddRange<Demand, IList<Demand>>(demands);
            job.Tasks.Add(taskInstance3);
          }
        }
        TaskHubExtensions.PurgeMinAgentVersionDemands(demandList, ref minAgentVersion);
        List<Demand> list = ((IEnumerable<Demand>) jobDemands ?? Enumerable.Empty<Demand>()).Where<Demand>((Func<Demand, bool>) (d => !tasksSatisfy.Contains(d.Name))).ToList<Demand>();
        TaskHubExtensions.PurgeMinAgentVersionDemands((IList<Demand>) list, ref minAgentVersion);
        if (minAgentVersion != null)
          demandList.Add((Demand) new DemandMinimumVersion("Agent.Version", minAgentVersion, source));
        job.Demands.AddRange((IEnumerable<Demand>) list);
        job.Demands.AddRange((IEnumerable<Demand>) demandList);
        return job1;
      }
    }

    public static Timeline UpdateTimeline(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      IList<TimelineRecord> records)
    {
      return requestContext.RunSynchronously<Timeline>((Func<Task<Timeline>>) (() => hub.UpdateTimelineAsync(requestContext, scopeIdentifier, planId, timelineId, records)));
    }

    internal static void PlanCompleted(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      DateTime finishTime,
      TaskResult result,
      string resultCode)
    {
      requestContext.RunSynchronously((Func<Task>) (() => hub.PlanCompletedAsync(requestContext, scopeIdentifier, planId, finishTime, result, resultCode)));
    }

    public static void TerminatePlanUnsafe(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId)
    {
      requestContext.RunSynchronously((Func<Task>) (() => hub.PlanCompletedAsync(requestContext, scopeIdentifier, planId, DateTime.UtcNow, TaskResult.Failed, typeof (ExecutionTerminatedEvent).Name)));
    }

    public static void CancelAgentJobUnsafe(
      this TaskHub hub,
      IVssRequestContext requestContext,
      TaskAgentJobRequest request,
      TimeSpan timeout)
    {
      requestContext.RunSynchronously((Func<Task>) (() => hub.CancelAgentJobAsync(requestContext, request.PoolId, request.RequestId, (TaskAgentReference) null, Guid.Empty, Guid.Empty, request.JobId, timeout)));
    }

    public static void UpdateTimelineRecord(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      TimelineRecordState? state = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      string currentOperation = null,
      int? percentComplete = null,
      TaskResult? result = null,
      string resultCode = null,
      TaskLogReference log = null,
      IEnumerable<Issue> issues = null)
    {
      requestContext.RunSynchronously((Func<Task>) (() => hub.UpdateTimelineRecordAsync(requestContext, scopeIdentifier, planId, timelineId, recordId, state, startTime, finishTime, currentOperation, percentComplete, result, resultCode, log, issues)));
    }

    public static IEnumerable<TaskStep> GetTaskSteps(
      this PhaseNode phase,
      IVssRequestContext requestContext,
      IResourceStore resourceStore)
    {
      switch (phase)
      {
        case Phase phase1:
          return phase1.Steps.OfType<TaskStep>();
        case ProviderPhase providerPhase:
          ValidationResult validationResult = new ValidationResult();
          return DeploymentStrategyBuilder.Build(resourceStore, providerPhase.Strategy, validationResult, requestContext.IsFeatureEnabled("DistributedTask.DeploymentJobAllowTaskMinorVersion"), requestContext.IsFeatureEnabled("DistributedTask.FixForDownloadBuildTaskNotResolved")).Hooks.Where<DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, bool>) (h => h?.Steps != null && h.Steps.Any<Step>())).SelectMany<DeploymentLifeCycleHookBase, Step>((Func<DeploymentLifeCycleHookBase, IEnumerable<Step>>) (h => (IEnumerable<Step>) h.Steps)).OfType<TaskStep>();
        default:
          return Enumerable.Empty<TaskStep>();
      }
    }

    internal static async Task<IEnumerable<SecureFile>> GetReferencedSecureFilesAsync(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid projectId,
      IList<TaskInstance> tasks)
    {
      IEnumerable<SecureFile> secureFilesAsync;
      using (new MethodScope(requestContext, "TaskHub", nameof (GetReferencedSecureFilesAsync)))
      {
        requestContext.GetService<IDistributedTaskPoolService>();
        ISecureFileService service = requestContext.GetService<ISecureFileService>();
        HashSet<SecureFile> referencedSecureFiles = new HashSet<SecureFile>((IEqualityComparer<SecureFile>) new SecureFileComparer());
        IEnumerable<Guid> secureFilesInUse = TaskHubExtensions.GetSecureFilesInUse(requestContext, tasks);
        if (secureFilesInUse.Any<Guid>())
        {
          foreach (SecureFile secureFile in (IEnumerable<SecureFile>) await service.GetSecureFilesAsync(requestContext, projectId, (IEnumerable<Guid>) secureFilesInUse.ToList<Guid>(), true))
            referencedSecureFiles.Add(secureFile.CloneShallow());
        }
        secureFilesAsync = (IEnumerable<SecureFile>) referencedSecureFiles;
      }
      return secureFilesAsync;
    }

    internal static async Task<IEnumerable<ServiceEndpoint>> GetReferencedServiceEndpointsAsync(
      this TaskHub hub,
      IVssRequestContext requestContext,
      Guid projectId,
      JobEnvironment environment,
      IList<TaskInstance> tasks)
    {
      IEnumerable<ServiceEndpoint> serviceEndpointsAsync;
      using (new MethodScope(requestContext, "TaskHub", nameof (GetReferencedServiceEndpointsAsync)))
      {
        IEnumerable<ServiceEndpoint> source = (IEnumerable<ServiceEndpoint>) new List<ServiceEndpoint>();
        requestContext.GetService<IDistributedTaskPoolService>();
        Dictionary<Guid, List<string>> endpoint2TaskMap = TaskHubExtensions.GetServiceEndpointsInUse(requestContext, environment, tasks);
        if (endpoint2TaskMap.Any<KeyValuePair<Guid, List<string>>>())
        {
          IEnumerable<Guid> endpointIdsInUse = endpoint2TaskMap.Select<KeyValuePair<Guid, List<string>>, Guid>((Func<KeyValuePair<Guid, List<string>>, Guid>) (kvp => kvp.Key));
          IEnumerable<ServiceEndpoint> serviceEndpoints = await DistributedTaskEndpointServiceHelper.QueryServiceEndpointsAsync(requestContext, projectId, (string) null, (IEnumerable<string>) null, endpointIdsInUse, true, true);
          source = (IEnumerable<ServiceEndpoint>) serviceEndpoints.ToList<ServiceEndpoint>();
          foreach (ServiceEndpoint serviceEndpoint in source)
            serviceEndpoint.Name = serviceEndpoint.Id.ToString();
          List<Guid> list1 = endpointIdsInUse.Where<Guid>((Func<Guid, bool>) (endpointId => serviceEndpoints.All<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (endpoint => endpoint.Id != endpointId)))).ToList<Guid>();
          if (list1.Any<Guid>())
          {
            List<string> endpointErrorStrings = new List<string>();
            list1.ForEach((Action<Guid>) (endpointId => endpointErrorStrings.Add(TaskHubExtensions.GetEndpointErrorString(endpointId.ToString("D"), endpoint2TaskMap[endpointId]))));
            throw new DistributedTaskException(TaskResources.EndpointUsedInTaskNotFound((object) string.Join(", ", (IEnumerable<string>) endpointErrorStrings)));
          }
          if (requestContext.IsFeatureEnabled("ServiceEndpoints.AllowDisablingServiceEndpoints"))
          {
            List<ServiceEndpoint> list2 = source.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (endpoint => endpoint.IsDisabled)).ToList<ServiceEndpoint>();
            if (list2.Count > 0)
            {
              List<string> endpointErrorStrings = new List<string>(list2.Count);
              list2.ForEach((Action<ServiceEndpoint>) (endpoint => endpointErrorStrings.Add(TaskHubExtensions.GetEndpointErrorString(endpoint.Name, endpoint2TaskMap[endpoint.Id]))));
              throw new DistributedTaskException(TaskResources.TaskEndpointInDisabledState((object) string.Join(", ", (IEnumerable<string>) endpointErrorStrings)));
            }
          }
          List<ServiceEndpoint> list3 = source.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (endpoint => !endpoint.IsReady)).ToList<ServiceEndpoint>();
          if (list3.Any<ServiceEndpoint>())
          {
            List<string> endpointErrorStrings = new List<string>();
            list3.ForEach((Action<ServiceEndpoint>) (endpoint => endpointErrorStrings.Add(TaskHubExtensions.GetEndpointErrorString(endpoint.Name, endpoint2TaskMap[endpoint.Id]))));
            throw new DistributedTaskException(TaskResources.TaskEndpointInDirtyState((object) string.Join(", ", (IEnumerable<string>) endpointErrorStrings)));
          }
          endpointIdsInUse = (IEnumerable<Guid>) null;
        }
        serviceEndpointsAsync = source;
      }
      return serviceEndpointsAsync;
    }

    private static string GetEndpointErrorString(
      string endpointString,
      List<string> taskDisplayNames)
    {
      return TaskResources.ServiceEndpointLinkedToTask((object) endpointString, (object) string.Join(", ", (IEnumerable<string>) taskDisplayNames));
    }

    private static IEnumerable<Guid> GetSecureFilesInUse(
      IVssRequestContext requestContext,
      IList<TaskInstance> tasks)
    {
      HashSet<Guid> secureFilesInUse = new HashSet<Guid>();
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      foreach (TaskInstance task in (IEnumerable<TaskInstance>) tasks)
      {
        TaskDefinition taskDefinition = service.GetTaskDefinition(requestContext, task.Id, task.Version);
        if (taskDefinition == null)
        {
          requestContext.TraceError(10016004, "Orchestration", "Could not find a task definition matching task, task.Id: {0}, task.Version: {1}", (object) task.Id, (object) task.Version);
        }
        else
        {
          foreach (TaskInputDefinition taskInputDefinition in taskDefinition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (x => x.InputType.Equals("secureFile"))))
          {
            string str;
            Guid result;
            if (task.Inputs.TryGetValue(taskInputDefinition.Name, out str) && !string.IsNullOrWhiteSpace(str) && Guid.TryParse(str.Trim(), out result))
              secureFilesInUse.Add(result);
          }
        }
      }
      return (IEnumerable<Guid>) secureFilesInUse;
    }

    private static Dictionary<Guid, List<string>> GetServiceEndpointsInUse(
      IVssRequestContext requestContext,
      JobEnvironment environment,
      IList<TaskInstance> tasks)
    {
      Dictionary<Guid, List<string>> serviceEndpointsInUse = new Dictionary<Guid, List<string>>();
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      foreach (TaskInstance task in (IEnumerable<TaskInstance>) tasks)
      {
        TaskDefinition taskDefinition = service.GetTaskDefinition(requestContext, task.Id, task.Version);
        if (taskDefinition == null)
        {
          requestContext.TraceError(10016004, "Orchestration", "Could not find a task definition matching task, task.Id: {0}, task.Version: {1}", (object) task.Id, (object) task.Version);
          throw new DistributedTaskException(TaskResources.TaskDefinitionNotFound((object) task.Id, (object) task.Version));
        }
        Dictionary<string, string> processParameters = environment.GetProcessParameters();
        Dictionary<string, string> source = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> input in (IEnumerable<KeyValuePair<string, string>>) task.Inputs)
        {
          if (!input.Value.IsNullOrEmpty<char>())
          {
            string str = VariableUtility.ExpandVariables(input.Value?.Trim(), (IDictionary<string, string>) processParameters, false);
            source[input.Key] = str;
          }
          else
          {
            source[input.Key] = string.Empty;
            requestContext.TraceWarning(10015545, "Orchestration", "Task input variable value is Empty, taskInputKey: {0}, processParameters: {1}", (object) input.Key, (object) processParameters.Serialize<Dictionary<string, string>>());
          }
        }
        foreach (TaskInputDefinition serviceEndpointTypeInput in taskDefinition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (x => x.IsEndpointTypeInput())))
        {
          string str1;
          if (task.Inputs.TryGetValue(serviceEndpointTypeInput.Name, out str1) && !string.IsNullOrEmpty(str1) && serviceEndpointTypeInput.IsEndpointInputVisible((IDictionary<string, string>) source.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key), (Func<KeyValuePair<string, string>, string>) (x => x.Value))))
          {
            string str2 = VariableUtility.ExpandVariables(str1?.Trim(), (IDictionary<string, string>) processParameters, false);
            char[] chArray = new char[1]{ ',' };
            foreach (string input in str2.Split(chArray))
            {
              Guid result;
              if (Guid.TryParse(input, out result))
              {
                if (serviceEndpointsInUse.ContainsKey(result))
                  serviceEndpointsInUse[result].Add(task.DisplayName ?? task.Name);
                else
                  serviceEndpointsInUse[result] = new List<string>()
                  {
                    task.DisplayName ?? task.Name
                  };
              }
            }
          }
        }
      }
      return serviceEndpointsInUse;
    }

    private static void PurgeMinAgentVersionDemands(
      IList<Demand> demands,
      ref string minAgentVersion)
    {
      foreach (Demand demand in demands.Where<Demand>((Func<Demand, bool>) (x => x.Name == "Agent.Version")).ToList<Demand>())
      {
        if (DemandMinimumVersion.CompareVersion(demand.Value, minAgentVersion) > 0)
          minAgentVersion = demand.Value;
        demands.Remove(demand);
      }
    }
  }
}
