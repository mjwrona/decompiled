// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStepExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class BuildDefinitionStepExtensions
  {
    private const string c_ServiceEndPointTaskInputTypePrefix = "connectedService:";

    public static bool IsTaskGroup(this BuildDefinitionStep buildDefinitionStep) => buildDefinitionStep.TaskDefinition.DefinitionType != null && string.Compare(buildDefinitionStep.TaskDefinition.DefinitionType, "metaTask", StringComparison.OrdinalIgnoreCase) == 0;

    public static Step ToStep(this BuildDefinitionStep step) => StringComparer.OrdinalIgnoreCase.Equals(step.TaskDefinition.DefinitionType, "metaTask") ? (Step) step.ToTemplateStep() : (Step) step.ToTaskStep();

    private static TaskStep ToTaskStep(this BuildDefinitionStep step)
    {
      TaskStep taskStep1 = new TaskStep();
      taskStep1.ContinueOnError = step.ContinueOnError;
      taskStep1.Enabled = step.Enabled;
      taskStep1.DisplayName = step.DisplayName;
      taskStep1.Name = step.RefName;
      taskStep1.Reference = new TaskStepDefinitionReference()
      {
        Id = step.TaskDefinition.Id,
        Version = step.TaskDefinition.VersionSpec
      };
      taskStep1.TimeoutInMinutes = step.TimeoutInMinutes;
      taskStep1.RetryCountOnTaskFailure = step.RetryCountOnTaskFailure;
      TaskStep taskStep2 = taskStep1;
      if (!string.IsNullOrEmpty(step.Condition))
        taskStep2.Condition = step.Condition;
      else if (step.AlwaysRun)
        taskStep2.Condition = "succeededOrFailed()";
      else
        taskStep2.Condition = "succeeded()";
      taskStep2.Inputs.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) step.Inputs);
      taskStep2.Environment.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) step.Environment);
      return taskStep2;
    }

    public static TaskInstance ToTaskInstance(this BuildDefinitionStep step)
    {
      TaskInstance taskInstance = new TaskInstance();
      taskInstance.Id = step.TaskDefinition.Id;
      taskInstance.Version = step.TaskDefinition.VersionSpec;
      taskInstance.DisplayName = step.DisplayName;
      taskInstance.Condition = step.Condition;
      taskInstance.AlwaysRun = step.AlwaysRun;
      taskInstance.ContinueOnError = step.ContinueOnError;
      taskInstance.Enabled = step.Enabled;
      taskInstance.TimeoutInMinutes = step.TimeoutInMinutes;
      taskInstance.RetryCountOnTaskFailure = step.RetryCountOnTaskFailure;
      taskInstance.RefName = step.RefName;
      if (TaskConditions.IsLegacyAlwaysRun(step.Condition))
        step.AlwaysRun = true;
      taskInstance.Inputs.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) step.Inputs);
      taskInstance.Environment.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) step.Environment);
      return taskInstance;
    }

    public static TaskTemplateStep ToTemplateStep(this BuildDefinitionStep step)
    {
      TaskTemplateStep templateStep = new TaskTemplateStep();
      templateStep.Name = step.RefName;
      templateStep.DisplayName = step.DisplayName;
      templateStep.Enabled = step.Enabled;
      templateStep.Reference = new TaskTemplateReference()
      {
        Id = step.TaskDefinition.Id,
        Version = step.TaskDefinition.VersionSpec
      };
      templateStep.Parameters.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) step.Inputs);
      return templateStep;
    }

    public static TaskDefinition ToTaskDefinition(
      this BuildDefinitionStep step,
      IVssRequestContext requestContext,
      IDistributedTaskPoolService poolService)
    {
      TaskDefinition taskDefinition = poolService.GetTaskDefinition(requestContext, step.TaskDefinition.Id, step.TaskDefinition.VersionSpec);
      if (taskDefinition == null)
        requestContext.TraceInfo("Service", "Task definition with id: {0} and version : {1} doesn't exist.", (object) step.TaskDefinition.Id, (object) step.TaskDefinition.VersionSpec);
      return taskDefinition;
    }

    public static void FixLatestMajorVersions(
      this IEnumerable<BuildDefinitionStep> steps,
      IVssRequestContext requestContext)
    {
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      foreach (BuildDefinitionStep step in steps)
      {
        if (step.TaskDefinition.VersionSpec == "*")
        {
          TaskDefinition latestMajorVersion = service.GetLatestMajorVersion(requestContext, step.TaskDefinition.Id);
          if (latestMajorVersion != null)
            step.TaskDefinition.VersionSpec = string.Format("{0}.*", (object) latestMajorVersion.Version.Major);
        }
      }
    }

    internal static IEnumerable<Guid> GetServiceEndPointsInUse(
      this IEnumerable<BuildDefinitionStep> steps,
      IVssRequestContext requestContext,
      ProcessParameters processParameters)
    {
      return (IEnumerable<Guid>) steps.GetServiceEndPointsInUseWithType(requestContext, processParameters).Keys;
    }

    internal static IEnumerable<string> GetUniqueEndpointTypesInUse(
      this IEnumerable<BuildDefinitionStep> steps,
      IVssRequestContext requestContext,
      ProcessParameters processParameters)
    {
      return steps.GetServiceEndPointsInUseWithType(requestContext, processParameters).Values.Distinct<string>();
    }

    internal static void FetchTaskDefinitionsWithVersionsInUse(
      this IEnumerable<BuildDefinitionStep> steps,
      IList<TaskDefinition> availableTaskDefinitions,
      out Dictionary<Guid, IList<TaskVersion>> taskVersions,
      out Dictionary<Guid, IDictionary<string, TaskDefinition>> taskDefinitions)
    {
      taskVersions = new Dictionary<Guid, IList<TaskVersion>>();
      taskDefinitions = new Dictionary<Guid, IDictionary<string, TaskDefinition>>();
      if (availableTaskDefinitions == null)
        return;
      foreach (Guid guid in steps.Where<BuildDefinitionStep>((Func<BuildDefinitionStep, bool>) (step => step.Enabled)).Select<BuildDefinitionStep, TaskInstance>((Func<BuildDefinitionStep, TaskInstance>) (x => x.ToTaskInstance())).ToList<TaskInstance>().Select<TaskInstance, Guid>((Func<TaskInstance, Guid>) (task => task.Id)).Distinct<Guid>())
      {
        Guid taskId = guid;
        List<TaskVersion> taskVersionList = new List<TaskVersion>();
        Dictionary<string, TaskDefinition> dictionary = new Dictionary<string, TaskDefinition>();
        foreach (TaskDefinition taskDefinition in availableTaskDefinitions.Where<TaskDefinition>((Func<TaskDefinition, bool>) (x => x.Id.Equals(taskId))))
        {
          taskVersionList.Add(taskDefinition.Version);
          dictionary.Add((string) taskDefinition.Version, taskDefinition);
        }
        taskVersions.Add(taskId, (IList<TaskVersion>) taskVersionList);
        taskDefinitions.Add(taskId, (IDictionary<string, TaskDefinition>) dictionary);
      }
    }

    private static IDictionary<Guid, string> GetServiceEndPointsInUseWithType(
      this IEnumerable<BuildDefinitionStep> steps,
      IVssRequestContext requestContext,
      ProcessParameters processParameters)
    {
      Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      foreach (BuildDefinitionStep step in steps.Where<BuildDefinitionStep>((Func<BuildDefinitionStep, bool>) (x => x.Enabled)))
      {
        TaskDefinition taskDefinition = step.ToTaskDefinition(requestContext, service);
        if (taskDefinition != null)
        {
          IEnumerable<TaskInputDefinition> taskInputDefinitions = taskDefinition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (x => x.InputType.Contains("connectedService:")));
          IDictionary<string, string> parametersInputs = processParameters.GetProcessParametersInputs();
          foreach (TaskInputDefinition taskInputDefinition in taskInputDefinitions)
          {
            string str1 = string.Empty;
            string[] strArray = taskInputDefinition.InputType.Split(':');
            if (strArray.Length > 1 && !string.IsNullOrWhiteSpace(strArray[1]))
              str1 = strArray[1].Trim();
            string str2;
            if (step.Inputs.TryGetValue(taskInputDefinition.Name, out str2) && !string.IsNullOrEmpty(str2))
            {
              string str3 = VariableUtility.ExpandVariables(str2.Trim(), parametersInputs);
              char[] chArray = new char[1]{ ',' };
              foreach (string input in str3.Split(chArray))
              {
                Guid result;
                if (Guid.TryParse(input, out result))
                  dictionary.TryAdd<Guid, string>(result, str1);
              }
            }
          }
        }
      }
      return (IDictionary<Guid, string>) dictionary;
    }
  }
}
