// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.MetaTaskHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class MetaTaskHelper
  {
    private const string c_layer = "MetaTaskHelper";

    public static void ExpandTasks(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskGroup taskGroup,
      IList<TaskGroupStep> expandedTasks)
    {
      using (new MethodScope(requestContext, nameof (MetaTaskHelper), nameof (ExpandTasks)))
      {
        foreach (TaskGroupStep task in (IEnumerable<TaskGroupStep>) taskGroup.Tasks)
        {
          TaskGroupStep taskGroupStep = task;
          MetaTaskHelper.UpdateMetaTaskInputReferences(taskGroupStep, taskGroup);
          if (taskGroupStep?.Task?.DefinitionType == "metaTask")
          {
            TaskGroup taskGroup1;
            using (MetaTaskDefinitionComponent component = requestContext.CreateComponent<MetaTaskDefinitionComponent>())
            {
              IList<TaskGroup> list = (IList<TaskGroup>) component.GetMetaTaskDefinitions(projectId, new Guid?(taskGroupStep.Task.Id)).Select<MetaTaskDefinitionData, TaskGroup>((Func<MetaTaskDefinitionData, TaskGroup>) (x => x.GetDefinition())).ToList<TaskGroup>();
              if (list.IsNullOrEmpty<TaskGroup>())
                list = (IList<TaskGroup>) TaskGroupExtensionsRetriever.GetAllTaskGroupTemplates(requestContext).Where<TaskGroup>((Func<TaskGroup, bool>) (tg => tg.Id == taskGroupStep.Task.Id)).ToList<TaskGroup>();
              if (list.IsNullOrEmpty<TaskGroup>())
                throw new MetaTaskDefinitionNotFoundException(TaskResources.MetaTaskMissingChildTask((object) taskGroupStep.Task.Id.ToString(), (object) taskGroupStep.DisplayName));
              taskGroup1 = MetaTaskHelper.ResolveTaskGroupVersion(list, taskGroupStep.Task.Id, taskGroupStep.Task.VersionSpec, true);
            }
            MetaTaskHelper.UpdateTaskWithMetaTaskValues(taskGroup1, taskGroupStep);
            MetaTaskHelper.ExpandTasks(requestContext, projectId, taskGroup1, expandedTasks);
          }
          else
            expandedTasks.Add(taskGroupStep);
        }
      }
    }

    public static TaskGroup ResolveTaskGroupVersion(
      IList<TaskGroup> taskGroups,
      Guid taskGroupId,
      string versionSpec,
      bool throwIfDisabled = false)
    {
      TaskVersionResolver taskVersionResolver = new TaskVersionResolver((IDictionary<Guid, IList<TaskVersion>>) new Dictionary<Guid, IList<TaskVersion>>()
      {
        {
          taskGroupId,
          (IList<TaskVersion>) taskGroups.Select<TaskGroup, TaskVersion>((Func<TaskGroup, TaskVersion>) (t => t.Version)).ToList<TaskVersion>()
        }
      });
      TaskGroup taskGroup = (TaskGroup) null;
      Guid taskId = taskGroupId;
      string version = versionSpec;
      TaskVersion taskVersion;
      ref TaskVersion local = ref taskVersion;
      if (taskVersionResolver.TryResolveVersion(taskId, version, out local))
        taskGroup = taskGroups.FirstOrDefault<TaskGroup>((Func<TaskGroup, bool>) (t => t.Version.Equals(taskVersion)));
      if (taskGroup == null)
        throw new MetaTaskDefinitionNotFoundException(TaskResources.MetaTaskDefinitionNotFound((object) taskGroupId));
      if (throwIfDisabled && taskGroup.Disabled)
        throw new TaskGroupDisabledException(TaskResources.TaskGroupDisabled((object) taskGroup.Name, (object) taskVersion));
      return taskGroup;
    }

    public static void ExpandTasks(
      IVssRequestContext requestContext,
      TaskGroup taskGroup,
      List<TaskGroup> allTaskGroups,
      List<TaskGroupStep> expandedTasks)
    {
      using (new MethodScope(requestContext, nameof (MetaTaskHelper), nameof (ExpandTasks)))
      {
        foreach (TaskGroupStep task in (IEnumerable<TaskGroupStep>) taskGroup.Tasks)
        {
          TaskGroupStep taskGroupStep = task;
          MetaTaskHelper.UpdateMetaTaskInputReferences(taskGroupStep, taskGroup);
          if (taskGroupStep.Task.DefinitionType == "metaTask")
          {
            TaskGroup taskGroup1 = MetaTaskHelper.ResolveTaskGroupVersion((IList<TaskGroup>) allTaskGroups.Where<TaskGroup>((Func<TaskGroup, bool>) (x => x.Id == taskGroupStep.Task.Id)).ToList<TaskGroup>(), taskGroupStep.Task.Id, taskGroupStep.Task.VersionSpec, true);
            MetaTaskHelper.UpdateTaskWithMetaTaskValues(taskGroup1, taskGroupStep);
            MetaTaskHelper.ExpandTasks(requestContext, taskGroup1, allTaskGroups, expandedTasks);
          }
          else
            expandedTasks.Add(taskGroupStep);
        }
      }
    }

    public static IEnumerable<TaskDefinition> GetTaskDefinitionsContainedInTaskGroups(
      IEnumerable<TaskGroup> taskGroups,
      IEnumerable<TaskDefinition> allTaskDefinitions)
    {
      IEnumerable<Guid> distinctIds = MetaTaskHelper.GetDistinctTaskIdsFromTaskGroups(taskGroups);
      return allTaskDefinitions.Where<TaskDefinition>((Func<TaskDefinition, bool>) (task => distinctIds.Contains<Guid>(task.Id)));
    }

    public static IDictionary<Guid, IList<TaskGroup>> GroupTaskGroupVersionsById(
      IList<TaskGroup> allTaskGroupsList)
    {
      if (allTaskGroupsList == null)
        return (IDictionary<Guid, IList<TaskGroup>>) null;
      IDictionary<Guid, IList<TaskGroup>> dictionary = (IDictionary<Guid, IList<TaskGroup>>) new Dictionary<Guid, IList<TaskGroup>>();
      foreach (Guid distinctTaskGroupId in MetaTaskHelper.GetDistinctTaskGroupIds(allTaskGroupsList))
      {
        Guid id = distinctTaskGroupId;
        IList<TaskGroup> list = (IList<TaskGroup>) allTaskGroupsList.Where<TaskGroup>((Func<TaskGroup, bool>) (tg => tg.Id.Equals(id))).ToList<TaskGroup>();
        dictionary.Add(id, list);
      }
      return dictionary;
    }

    public static TaskGroup GetLatestVersionTaskGroup(IList<TaskGroup> taskGroupVersions)
    {
      if (taskGroupVersions == null || taskGroupVersions.Count == 0)
        return (TaskGroup) null;
      TaskGroup versionTaskGroup = taskGroupVersions[0];
      foreach (TaskGroup taskGroupVersion in (IEnumerable<TaskGroup>) taskGroupVersions)
      {
        if (versionTaskGroup.Version.Major < taskGroupVersion.Version.Major)
          versionTaskGroup = taskGroupVersion;
        else if (versionTaskGroup.Version.Major == taskGroupVersion.Version.Major)
        {
          if (versionTaskGroup.Version.Minor < taskGroupVersion.Version.Minor)
            versionTaskGroup = taskGroupVersion;
          else if (versionTaskGroup.Version.Minor == taskGroupVersion.Version.Minor && versionTaskGroup.Version.Patch < taskGroupVersion.Version.Patch)
            versionTaskGroup = taskGroupVersion;
        }
      }
      return versionTaskGroup;
    }

    public static IList<TaskGroup> GetLatestVersionsFromTaskGroups(IList<TaskGroup> allTaskGroups)
    {
      if (allTaskGroups == null)
        return (IList<TaskGroup>) null;
      IDictionary<Guid, IList<TaskGroup>> dictionary = MetaTaskHelper.GroupTaskGroupVersionsById(allTaskGroups);
      IEnumerable<Guid> distinctTaskGroupIds = MetaTaskHelper.GetDistinctTaskGroupIds(allTaskGroups);
      IList<TaskGroup> versionsFromTaskGroups = (IList<TaskGroup>) new List<TaskGroup>();
      foreach (Guid key in distinctTaskGroupIds)
      {
        TaskGroup versionTaskGroup = MetaTaskHelper.GetLatestVersionTaskGroup(dictionary[key]);
        versionsFromTaskGroups.Add(versionTaskGroup);
      }
      return versionsFromTaskGroups;
    }

    public static IEnumerable<Guid> GetDistinctTaskGroupIds(IList<TaskGroup> taskGroups) => taskGroups == null ? (IEnumerable<Guid>) null : taskGroups.Select<TaskGroup, Guid>((Func<TaskGroup, Guid>) (tg => tg.Id)).Distinct<Guid>();

    private static void UpdateTaskWithMetaTaskValues(
      TaskGroup childTaskGroup,
      TaskGroupStep taskGroupStep)
    {
      foreach (TaskGroupStep task in (IEnumerable<TaskGroupStep>) childTaskGroup.Tasks)
      {
        Dictionary<string, bool> alreadyReplaced = new Dictionary<string, bool>();
        foreach (KeyValuePair<string, string> input1 in (IEnumerable<KeyValuePair<string, string>>) taskGroupStep.Inputs)
        {
          KeyValuePair<string, string> input = input1;
          string oldValue1 = "$(" + input.Key + ")";
          Dictionary<string, string> dictionary = new Dictionary<string, string>();
          foreach (KeyValuePair<string, string> input2 in (IEnumerable<KeyValuePair<string, string>>) task.Inputs)
          {
            string str = string.IsNullOrWhiteSpace(input.Value) ? input2.Value : input2.Value?.Replace(oldValue1, input.Value);
            if ((string.IsNullOrWhiteSpace(input.Value) ? 0 : (input2.Value.IndexOf(oldValue1, StringComparison.Ordinal) >= 0 ? 1 : 0)) != 0)
              alreadyReplaced[MetaTaskHelper.CreateKey(input2.Key, input.Key)] = true;
            dictionary.Add(input2.Key, str);
          }
          task.Inputs = (IDictionary<string, string>) dictionary;
          string defaultValue = input.Value;
          if (string.IsNullOrWhiteSpace(defaultValue))
            defaultValue = childTaskGroup.Inputs.FirstOrDefault<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (childInput => childInput.Name == input.Key))?.DefaultValue;
          task.DisplayName = task.DisplayName.Replace(oldValue1, defaultValue);
          if (!string.IsNullOrWhiteSpace(task.Condition))
          {
            string oldValue2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}['{1}']", (object) ExpressionConstants.Variables, (object) input.Key);
            string newValue = VariableUtility.PrepareReplacementStringForConditions(input.Value);
            task.Condition = task.Condition.Replace(oldValue2, newValue);
          }
        }
        MetaTaskHelper.UpdateInputsNotReplacedWithDefaults(task, childTaskGroup.Inputs, alreadyReplaced);
        task.Enabled &= taskGroupStep.Enabled;
      }
    }

    private static void UpdateInputsNotReplacedWithDefaults(
      TaskGroupStep task,
      IList<TaskInputDefinition> inputs,
      Dictionary<string, bool> alreadyReplaced)
    {
      foreach (TaskInputDefinition input1 in (IEnumerable<TaskInputDefinition>) inputs)
      {
        string oldValue = "$(" + input1.Name + ")";
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> input2 in (IEnumerable<KeyValuePair<string, string>>) task.Inputs)
        {
          string str = !alreadyReplaced.ContainsKey(MetaTaskHelper.CreateKey(input2.Key, input1.Name)) ? input2.Value.Replace(oldValue, input1.DefaultValue) : input2.Value;
          dictionary.Add(input2.Key, str);
        }
        task.Inputs = (IDictionary<string, string>) dictionary;
      }
    }

    private static string CreateKey(string key1, string key2) => key1 + "$" + key2;

    private static void UpdateMetaTaskInputReferences(
      TaskGroupStep taskGroupStep,
      TaskGroup taskGroup)
    {
      if (taskGroupStep == null)
        throw new ArgumentNullException(nameof (taskGroupStep));
      if (taskGroup == null)
        throw new ArgumentNullException(nameof (taskGroup));
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (taskGroupStep.Inputs == null)
      {
        taskGroupStep.Inputs = (IDictionary<string, string>) dictionary;
      }
      else
      {
        Dictionary<string, string> definitionInputs = MetaTaskHelper.GetMetaTaskDefinitionInputs(taskGroup);
        foreach (KeyValuePair<string, string> input in (IEnumerable<KeyValuePair<string, string>>) taskGroupStep.Inputs)
        {
          string enumerable = input.Value;
          if (enumerable.IsNullOrEmpty<char>() && definitionInputs.ContainsKey(input.Key) && !string.IsNullOrWhiteSpace(definitionInputs[input.Key]))
            enumerable = definitionInputs[input.Key];
          dictionary.Add(input.Key, enumerable);
        }
        taskGroupStep.Inputs = (IDictionary<string, string>) dictionary;
      }
    }

    private static Dictionary<string, string> GetMetaTaskDefinitionInputs(TaskGroup taskGroup) => taskGroup.Inputs == null ? new Dictionary<string, string>() : taskGroup.Inputs.ToDictionary<TaskInputDefinition, string, string>((Func<TaskInputDefinition, string>) (inputDefinition => inputDefinition.Name), (Func<TaskInputDefinition, string>) (inputDefinition => inputDefinition.DefaultValue));

    private static IEnumerable<Guid> GetDistinctTaskIdsFromTaskGroups(
      IEnumerable<TaskGroup> taskGroups)
    {
      List<Guid> source = new List<Guid>();
      foreach (TaskGroup taskGroup in taskGroups)
        source.AddRange(taskGroup.Tasks.Select<TaskGroupStep, Guid>((Func<TaskGroupStep, Guid>) (taskGroupStep => taskGroupStep.Task.Id)));
      return source.Distinct<Guid>();
    }
  }
}
