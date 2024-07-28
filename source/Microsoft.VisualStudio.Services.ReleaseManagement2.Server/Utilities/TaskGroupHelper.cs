// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.TaskGroupHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  internal static class TaskGroupHelper
  {
    public static ReleaseDefinition ReplaceTaskGroupWithTasks(
      IVssRequestContext requestContext,
      Guid projectId,
      ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        return (ReleaseDefinition) null;
      if (releaseDefinition.Environments == null)
        return releaseDefinition;
      foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) releaseDefinition.Environments)
        TaskGroupHelper.ReplaceTaskGroupWithTasksInEnvironment(requestContext, projectId, environment);
      return releaseDefinition;
    }

    public static void ReplaceTaskGroupWithTasksInEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      DefinitionEnvironment releaseDefinitionEnvironment)
    {
      if (releaseDefinitionEnvironment == null)
        throw new ArgumentNullException(nameof (releaseDefinitionEnvironment));
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase deployPhase in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase>) releaseDefinitionEnvironment.DeployPhases)
      {
        if (!string.IsNullOrEmpty(deployPhase.Workflow))
          deployPhase.Workflow = TaskGroupHelper.ValidateAndReplaceTaskGroupWithTasksInWorkflow(requestContext, projectId, deployPhase, releaseDefinitionEnvironment.Name);
      }
    }

    public static string ValidateAndReplaceTaskGroupWithTasksInWorkflow(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase deployPhase,
      string environmentName)
    {
      if (deployPhase == null)
        throw new ArgumentNullException(nameof (deployPhase));
      if (deployPhase.Workflow.IsNullOrEmpty<char>())
        return (string) null;
      IList<TaskDefinition> taskDefinitions = requestContext.GetService<IDistributedTaskPoolService>().GetTaskDefinitions(requestContext);
      List<WorkflowTask> workflowTaskList1 = JsonConvert.DeserializeObject<List<WorkflowTask>>(deployPhase.Workflow);
      IList<WorkflowTask> workflowTaskList2 = (IList<WorkflowTask>) new List<WorkflowTask>();
      foreach (WorkflowTask taskGroupTask in workflowTaskList1)
      {
        if (taskGroupTask.DefinitionType == "metaTask")
        {
          TaskGroup taskGroup = TaskGroupHelper.GetTaskGroup(requestContext, projectId, taskGroupTask.TaskId, taskGroupTask.Version, taskGroupTask.Name);
          IList<WorkflowTask> tasksFromTaskGroup = TaskGroupHelper.GetTasksFromTaskGroup(taskGroup);
          TaskGroupHelper.UpdateTaskGroupInputReferences(taskGroupTask, taskGroup);
          TaskGroupHelper.UpdateTaskWithTaskGroupValues(tasksFromTaskGroup, taskGroupTask);
          workflowTaskList2.AddRange<WorkflowTask, IList<WorkflowTask>>((IEnumerable<WorkflowTask>) tasksFromTaskGroup);
        }
        else
          workflowTaskList2.Add(taskGroupTask);
      }
      string runsOnValue = deployPhase.PhaseType.ToRunsOnValue();
      workflowTaskList2.ValidateTasks(taskDefinitions, environmentName, deployPhase.Name, runsOnValue);
      return JsonConvert.SerializeObject((object) workflowTaskList2);
    }

    public static IList<WorkflowTask> ReplaceMetaTaskWithTasksInWorkflow(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<WorkflowTask> workflowTasks)
    {
      if (workflowTasks == null)
        throw new ArgumentNullException(nameof (workflowTasks));
      IList<WorkflowTask> collection = (IList<WorkflowTask>) new List<WorkflowTask>();
      foreach (WorkflowTask workflowTask in (IEnumerable<WorkflowTask>) workflowTasks)
      {
        if (workflowTask.DefinitionType == "metaTask")
        {
          TaskGroup taskGroup = TaskGroupHelper.GetTaskGroup(requestContext, projectId, workflowTask.TaskId, workflowTask.Version, workflowTask.Name);
          IList<WorkflowTask> tasksFromTaskGroup = TaskGroupHelper.GetTasksFromTaskGroup(taskGroup);
          TaskGroupHelper.UpdateTaskGroupInputReferences(workflowTask, taskGroup);
          TaskGroupHelper.UpdateTaskWithTaskGroupValues(tasksFromTaskGroup, workflowTask);
          collection.AddRange<WorkflowTask, IList<WorkflowTask>>((IEnumerable<WorkflowTask>) tasksFromTaskGroup);
        }
        else
          collection.Add(workflowTask);
      }
      return collection;
    }

    private static IList<WorkflowTask> GetTasksFromTaskGroup(TaskGroup taskGroup)
    {
      List<WorkflowTask> source = new List<WorkflowTask>();
      foreach (TaskGroupStep task in (IEnumerable<TaskGroupStep>) taskGroup.Tasks)
      {
        WorkflowTask workflowTask = new WorkflowTask()
        {
          TaskId = task.Task.Id,
          Version = task.Task.VersionSpec,
          AlwaysRun = task.AlwaysRun,
          ContinueOnError = task.ContinueOnError,
          Condition = task.Condition,
          Enabled = task.Enabled,
          TimeoutInMinutes = task.TimeoutInMinutes,
          Name = task.DisplayName,
          DefinitionType = task.Task.DefinitionType,
          RetryCountOnTaskFailure = task.RetryCountOnTaskFailure,
          Inputs = (Dictionary<string, string>) task.Inputs
        };
        if (task.Environment != null)
        {
          foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) task.Environment)
            workflowTask.Environment[keyValuePair.Key] = keyValuePair.Value;
        }
        source.Add(workflowTask);
      }
      return (IList<WorkflowTask>) source.ToList<WorkflowTask>();
    }

    private static TaskGroup GetTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      string version,
      string taskName)
    {
      TaskGroup taskGroup = (TaskGroup) null;
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      if (requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.FastTaskGroupExpansion"))
      {
        string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", (object) projectId.ToString("N"), (object) taskGroupId.ToString("N"), (object) version);
        if (!requestContext.Items.TryGetValue<TaskGroup>(key, out taskGroup))
        {
          taskGroup = service.GetTaskGroup(requestContext, projectId, taskGroupId, version, new bool?(true));
          requestContext.Items.Add(key, (object) taskGroup);
        }
      }
      else
        taskGroup = service.GetTaskGroup(requestContext, projectId, taskGroupId, version, new bool?(true));
      if (taskGroup == null)
        throw new TaskGroupMissingException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MetaTaskDefinitionNotFound, (object) taskName));
      return !taskGroup.Disabled ? taskGroup : throw new TaskGroupDisabledException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TaskGroupDisabled, (object) taskName, (object) taskGroup.Version));
    }

    private static void UpdateTaskWithTaskGroupValues(
      IList<WorkflowTask> tasks,
      WorkflowTask taskGroupTask)
    {
      foreach (WorkflowTask task in (IEnumerable<WorkflowTask>) tasks)
      {
        task.Enabled &= taskGroupTask.Enabled;
        foreach (KeyValuePair<string, string> input1 in taskGroupTask.Inputs)
        {
          KeyValuePair<string, string> input = input1;
          string decoratedName = "$(" + input.Key + ")";
          Dictionary<string, string> inputs = task.Inputs;
          Dictionary<string, string> dictionary = inputs != null ? inputs.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (targetInput => targetInput.Key), (Func<KeyValuePair<string, string>, string>) (targetInput => targetInput.Value?.Replace(decoratedName, input.Value))) : (Dictionary<string, string>) null;
          task.Inputs = dictionary;
          task.Name = task.Name.Replace(decoratedName, input.Value);
          if (!string.IsNullOrWhiteSpace(task.Condition))
          {
            string oldValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}['{1}']", (object) ExpressionConstants.Variables, (object) input.Key);
            string newValue = VariableUtility.PrepareReplacementStringForConditions(input.Value);
            task.Condition = task.Condition.Replace(oldValue, newValue);
          }
        }
      }
    }

    private static void UpdateTaskGroupInputReferences(
      WorkflowTask taskGroupTask,
      TaskGroup taskGroup)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      Dictionary<string, string> taskGroupInputs = TaskGroupHelper.GetTaskGroupInputs(taskGroup);
      foreach (KeyValuePair<string, string> input in taskGroupTask.Inputs)
      {
        string enumerable = input.Value;
        if (enumerable.IsNullOrEmpty<char>() && taskGroupInputs.ContainsKey(input.Key))
          enumerable = taskGroupInputs[input.Key];
        dictionary.Add(input.Key, enumerable);
      }
      foreach (KeyValuePair<string, string> keyValuePair in taskGroupInputs)
      {
        if (!taskGroupTask.Inputs.ContainsKey(keyValuePair.Key) && !keyValuePair.Value.IsNullOrEmpty<char>())
          dictionary.Add(keyValuePair.Key, keyValuePair.Value);
      }
      taskGroupTask.Inputs = dictionary;
    }

    private static Dictionary<string, string> GetTaskGroupInputs(TaskGroup taskGroup) => taskGroup.Inputs.ToDictionary<TaskInputDefinition, string, string>((Func<TaskInputDefinition, string>) (inputDefinition => inputDefinition.Name), (Func<TaskInputDefinition, string>) (inputDefinition => inputDefinition.DefaultValue));
  }
}
