// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskStepProcessor
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskStepProcessor
  {
    private const string c_endpointInputTypePrefix = "connectedService:";

    internal static IList<PipelineValidationError> ResolveStepsAndAutoFillEnvironmentInputs(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Step> steps,
      EnvironmentDeploymentTarget environmentTarget,
      string phaseName)
    {
      IList<PipelineValidationError> error = (IList<PipelineValidationError>) new List<PipelineValidationError>();
      if (!steps.Any<Step>())
        return error;
      bool flag = false;
      string empty = string.Empty;
      string expectedEndPointTypeString = string.Empty;
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = TaskStepProcessor.FetchEndpointTypeUsedInResource(requestContext, projectId, environmentTarget.EnvironmentId, environmentTarget.Resource);
      if (serviceEndpoint != null && serviceEndpoint.Id != Guid.Empty)
      {
        flag = true;
        empty = serviceEndpoint.Id.ToString();
        expectedEndPointTypeString = "connectedService:" + serviceEndpoint.Type;
      }
      TaskStore taskStore = new TaskStore((IEnumerable<TaskDefinition>) TaskStepProcessor.GetTaskDefinitions(requestContext), (ITaskResolver) new TaskResolver(requestContext), featureCallback: new Func<string, bool>(((VssRequestContextExtensions) requestContext).IsFeatureEnabled));
      Dictionary<string, int> stepNameCounterMap = new Dictionary<string, int>();
      foreach (Step step in (IEnumerable<Step>) steps)
      {
        TaskDefinition taskDefinition = TaskStepProcessor.ResolveStep((TaskStep) step, taskStore, phaseName, stepNameCounterMap, error);
        if (taskDefinition != null & flag)
          TaskStepProcessor.TryFillEndPointAndInputAlias((TaskStep) step, taskDefinition, empty, expectedEndPointTypeString);
      }
      return error;
    }

    private static IList<TaskDefinition> GetTaskDefinitions(IVssRequestContext requestContext)
    {
      IList<TaskDefinition> taskDefinitions = requestContext.GetService<IDistributedTaskService>().GetTaskDefinitions(requestContext);
      taskDefinitions.Add(new TaskDefinition()
      {
        Name = "downloadBuild",
        Id = Guid.NewGuid(),
        Version = new TaskVersion("1.0.0")
      });
      taskDefinitions.Add(new TaskDefinition()
      {
        Name = "getPackage",
        Id = Guid.NewGuid(),
        Version = new TaskVersion("1.0.0")
      });
      return taskDefinitions;
    }

    private static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint FetchEndpointTypeUsedInResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      EnvironmentResourceReference resource)
    {
      if (resource == null)
        return (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint) null;
      return EnvironmentResourceDataProviderFactory.GetProviderManager(resource.Type)?.GetEndPoint(requestContext, projectId, environmentId, resource.Id);
    }

    private static TaskDefinition ResolveStep(
      TaskStep taskStep,
      TaskStore taskStore,
      string phaseName,
      Dictionary<string, int> stepNameCounterMap,
      IList<PipelineValidationError> error)
    {
      TaskDefinition taskDefinition = (TaskDefinition) null;
      try
      {
        if (taskStep.Reference.Id != Guid.Empty)
          taskDefinition = taskStore.ResolveTask(taskStep.Reference.Id, taskStep.Reference.Version);
        else if (!string.IsNullOrEmpty(taskStep.Reference.Name))
          taskDefinition = taskStore.ResolveTask(taskStep.Reference.Name, taskStep.Reference.Version);
      }
      catch (AmbiguousTaskSpecificationException ex)
      {
        error.Add(new PipelineValidationError(PipelineStrings.TaskStepReferenceInvalid((object) phaseName, (object) taskStep.Name, (object) ex.Message)));
        return taskDefinition;
      }
      if (taskDefinition == null || taskDefinition.Disabled)
      {
        string str = taskStep.Reference.Id != Guid.Empty ? taskStep.Reference.Id.ToString() : taskStep.Reference.Name;
        error.Add(new PipelineValidationError(PipelineStrings.TaskMissing((object) phaseName, (object) taskStep.Name, (object) str, (object) taskStep.Reference.Version)));
        return taskDefinition;
      }
      taskStep.Reference.Id = taskDefinition.Id;
      taskStep.Reference.Name = taskDefinition.Name;
      taskStep.Reference.Version = (string) taskDefinition.Version;
      if (string.IsNullOrWhiteSpace(taskStep.Name))
        taskStep.Name = NameValidation.Sanitize(taskStep.Reference.Name);
      TaskStepProcessor.ValidateAndUpdateStepNameIfRequired(stepNameCounterMap, taskStep);
      if (string.IsNullOrWhiteSpace(taskStep.DisplayName))
        taskStep.DisplayName = taskStep.Name;
      return taskDefinition;
    }

    private static void TryFillEndPointAndInputAlias(
      TaskStep taskStep,
      TaskDefinition taskDefinition,
      string resourceEndpointId,
      string expectedEndPointTypeString)
    {
      foreach (TaskInputDefinition input in (IEnumerable<TaskInputDefinition>) taskDefinition.Inputs)
      {
        TaskStepProcessor.ResolveAlias(taskStep, input);
        if (!taskStep.Inputs.TryGetValue(input.Name, out string _) && input.InputType.Equals(expectedEndPointTypeString, StringComparison.OrdinalIgnoreCase))
          taskStep.Inputs.Add(input.Name, resourceEndpointId);
      }
    }

    private static void ResolveAlias(TaskStep step, TaskInputDefinition input)
    {
      foreach (string alias in (IEnumerable<string>) input.Aliases)
      {
        string str;
        if (step.Inputs.TryGetValue(alias, out str))
        {
          step.Inputs.Remove(alias);
          step.Inputs.Add(input.Name, str);
          break;
        }
      }
    }

    private static void ValidateAndUpdateStepNameIfRequired(
      Dictionary<string, int> stepNameCounterMap,
      TaskStep step)
    {
      while (stepNameCounterMap.TryGetValue(step.Name, out int _))
        step.Name = string.Format("{0}{1}", (object) step.Name, (object) ++stepNameCounterMap[step.Name]);
      stepNameCounterMap.Add(step.Name, 1);
    }
  }
}
