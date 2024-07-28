// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TaskGroupResolver
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class TaskGroupResolver
  {
    public static BuildDefinition ResolveSteps(
      this BuildDefinition buildDefinition,
      IVssRequestContext requestContext,
      bool throwException = false)
    {
      List<BuildDefinitionStep> missingSteps;
      BuildDefinition buildDefinition1 = buildDefinition.ResolveSteps(requestContext, out missingSteps);
      // ISSUE: explicit non-virtual call
      if (!throwException || missingSteps == null || __nonvirtual (missingSteps.Count) <= 0)
        return buildDefinition1;
      throw new MetaTaskDefinitionNotFoundException(BuildServerResources.MetaTaskDefinitionsNotFound((object) string.Join(",", missingSteps.Select<BuildDefinitionStep, string>((Func<BuildDefinitionStep, string>) (e => e.DisplayName)))));
    }

    public static BuildDefinition ResolveSteps(
      this BuildDefinition buildDefinition,
      IVssRequestContext requestContext,
      out List<BuildDefinitionStep> missingSteps)
    {
      missingSteps = new List<BuildDefinitionStep>();
      if (buildDefinition?.Process is DesignerProcess process)
      {
        foreach (Phase phase in process.Phases)
        {
          List<BuildDefinitionStep> missingSteps1 = new List<BuildDefinitionStep>();
          List<BuildDefinitionStep> collection = phase.Steps.ResolveSteps(requestContext, buildDefinition.ProjectId, out missingSteps1);
          phase.Steps.Clear();
          phase.Steps.AddRange((IEnumerable<BuildDefinitionStep>) collection);
          missingSteps1.AddRange((IEnumerable<BuildDefinitionStep>) missingSteps1);
        }
      }
      return buildDefinition;
    }

    public static List<BuildDefinitionStep> ResolveSteps(
      this List<BuildDefinitionStep> steps,
      IVssRequestContext requestContext,
      Guid projectId,
      bool throwException = true)
    {
      return steps.ResolveSteps(requestContext, projectId, out List<BuildDefinitionStep> _, throwException);
    }

    public static List<BuildDefinitionStep> ResolveSteps(
      this List<BuildDefinitionStep> steps,
      IVssRequestContext requestContext,
      Guid projectId,
      out List<BuildDefinitionStep> missingSteps,
      bool throwException = true)
    {
      List<BuildDefinitionStep> buildDefinitionStepList = new List<BuildDefinitionStep>();
      missingSteps = new List<BuildDefinitionStep>();
      foreach (BuildDefinitionStep step in steps)
      {
        if (step.IsTaskGroup())
        {
          TaskGroup taskGroup = (TaskGroup) null;
          string versionSpec = (string) null;
          Guid taskGroupId = Guid.Empty;
          try
          {
            taskGroupId = step.TaskDefinition.Id;
            versionSpec = step.TaskDefinition.VersionSpec;
            taskGroup = TaskGroupResolver.GetTaskGroup(requestContext, projectId, taskGroupId, versionSpec);
          }
          catch (Exception ex)
          {
            if (throwException)
              throw;
          }
          if (taskGroup == null || taskGroup.Disabled)
          {
            string format = taskGroup == null ? "TaskGroup {0} with name {1} and version {2} is missing" : "TaskGroup {0} with name {1} and version {2} is disabled";
            requestContext.TraceWarning(12030027, "Service", format, (object) taskGroupId, (object) step.DisplayName, (object) versionSpec);
            missingSteps.Add(step);
          }
          else
          {
            IList<BuildDefinitionStep> tasksFromTaskGroup = TaskGroupResolver.GetTasksFromTaskGroup(taskGroup);
            TaskGroupResolver.UpdateTaskGroupInputReferences(step, taskGroup);
            TaskGroupResolver.UpdateTaskWithTaskGroupValues(tasksFromTaskGroup, step);
            buildDefinitionStepList.AddRange((IEnumerable<BuildDefinitionStep>) tasksFromTaskGroup);
          }
        }
        else
          buildDefinitionStepList.Add(step);
      }
      return buildDefinitionStepList;
    }

    private static IList<BuildDefinitionStep> GetTasksFromTaskGroup(TaskGroup taskGroup) => (IList<BuildDefinitionStep>) taskGroup.Tasks.Select<TaskGroupStep, BuildDefinitionStep>((Func<TaskGroupStep, BuildDefinitionStep>) (taskGroupStep =>
    {
      BuildDefinitionStep tasksFromTaskGroup = new BuildDefinitionStep();
      tasksFromTaskGroup.AlwaysRun = taskGroupStep.AlwaysRun;
      tasksFromTaskGroup.ContinueOnError = taskGroupStep.ContinueOnError;
      tasksFromTaskGroup.Condition = taskGroupStep.Condition;
      tasksFromTaskGroup.Enabled = taskGroupStep.Enabled;
      tasksFromTaskGroup.DisplayName = taskGroupStep.DisplayName;
      tasksFromTaskGroup.TimeoutInMinutes = taskGroupStep.TimeoutInMinutes;
      tasksFromTaskGroup.TaskDefinition = new TaskDefinitionReference()
      {
        Id = taskGroupStep.Task.Id,
        DefinitionType = taskGroupStep.Task.DefinitionType,
        VersionSpec = taskGroupStep.Task.VersionSpec
      };
      tasksFromTaskGroup.Inputs.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) taskGroupStep.Inputs);
      return tasksFromTaskGroup;
    })).ToList<BuildDefinitionStep>();

    private static TaskGroup GetTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      string versionSpec)
    {
      return requestContext.GetService<IDistributedTaskPoolService>().GetTaskGroup(requestContext, projectId, taskGroupId, versionSpec, new bool?(true));
    }

    private static void UpdateTaskWithTaskGroupValues(
      IList<BuildDefinitionStep> tasks,
      BuildDefinitionStep definitionTaskGroupStep)
    {
      foreach (BuildDefinitionStep task in (IEnumerable<BuildDefinitionStep>) tasks)
      {
        task.Enabled &= definitionTaskGroupStep.Enabled;
        foreach (KeyValuePair<string, string> input1 in (IEnumerable<KeyValuePair<string, string>>) definitionTaskGroupStep.Inputs)
        {
          KeyValuePair<string, string> input = input1;
          string decoratedName = "$(" + input.Key + ")";
          Dictionary<string, string> dictionary = task.Inputs.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (targetInput => targetInput.Key), (Func<KeyValuePair<string, string>, string>) (targetInput => targetInput.Value.Replace(decoratedName, input.Value)));
          task.Inputs.Clear();
          task.Inputs.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) dictionary);
          task.DisplayName = task.DisplayName.Replace(decoratedName, input.Value);
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
      BuildDefinitionStep definitionTaskGroupStep,
      TaskGroup taskGroup)
    {
      Dictionary<string, string> values = new Dictionary<string, string>();
      Dictionary<string, string> taskGroupInputs = TaskGroupResolver.GetTaskGroupInputs(taskGroup);
      foreach (KeyValuePair<string, string> input in (IEnumerable<KeyValuePair<string, string>>) definitionTaskGroupStep.Inputs)
      {
        string enumerable = input.Value;
        if (enumerable.IsNullOrEmpty<char>() && taskGroupInputs.ContainsKey(input.Key))
          enumerable = taskGroupInputs[input.Key];
        values.Add(input.Key, enumerable);
      }
      foreach (KeyValuePair<string, string> keyValuePair in taskGroupInputs)
      {
        if (!definitionTaskGroupStep.Inputs.ContainsKey(keyValuePair.Key) && !keyValuePair.Value.IsNullOrEmpty<char>())
          values.Add(keyValuePair.Key, keyValuePair.Value);
      }
      definitionTaskGroupStep.Inputs.Clear();
      definitionTaskGroupStep.Inputs.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) values);
    }

    private static Dictionary<string, string> GetTaskGroupInputs(TaskGroup taskGroup) => taskGroup.Inputs.ToDictionary<TaskInputDefinition, string, string>((Func<TaskInputDefinition, string>) (inputDefinition => inputDefinition.Name), (Func<TaskInputDefinition, string>) (inputDefinition => inputDefinition.DefaultValue));
  }
}
