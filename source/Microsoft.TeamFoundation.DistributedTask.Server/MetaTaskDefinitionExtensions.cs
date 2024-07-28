// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.MetaTaskDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class MetaTaskDefinitionExtensions
  {
    public static bool HasMetaTask(this TaskGroup taskGroup) => taskGroup != null && taskGroup.Tasks.Any<TaskGroupStep>((Func<TaskGroupStep, bool>) (ts => ts.Task != null && ts.Task.DefinitionType != null && ts.Task.DefinitionType.Equals("metaTask", StringComparison.OrdinalIgnoreCase)));

    public static void Validate(
      this TaskGroup taskGroup,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (MetaTaskDefinitionExtensions.IsTestTaskGroup(taskGroup))
      {
        if (!taskGroup.ParentDefinitionId.HasValue)
          throw new TeamFoundationServerInvalidRequestException(TaskResources.InvalidParentDefinitionIdForDraftTaskGroup());
        IList<TaskGroup> taskGroups = requestContext.GetService<IDistributedTaskPoolService>().GetTaskGroups(requestContext, projectId, taskGroup.ParentDefinitionId);
        if (!taskGroups.Any<TaskGroup>() || taskGroups.All<TaskGroup>((Func<TaskGroup, bool>) (t => t.Disabled)))
          throw new MetaTaskDefinitionNotFoundException(TaskResources.ParentTaskGroupNotFound((object) taskGroup.ParentDefinitionId));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        if (taskGroups.Any<TaskGroup>(MetaTaskDefinitionExtensions.\u003C\u003EO.\u003C0\u003E__IsTestTaskGroup ?? (MetaTaskDefinitionExtensions.\u003C\u003EO.\u003C0\u003E__IsTestTaskGroup = new Func<TaskGroup, bool>(MetaTaskDefinitionExtensions.IsTestTaskGroup))))
          throw new TeamFoundationServerInvalidRequestException(TaskResources.DraftForDraftTaskGroupIsNotAllowed((object) taskGroup.ParentDefinitionId));
      }
      if (taskGroup.Disabled)
        throw new TeamFoundationServerInvalidRequestException("Cannot add a disabled TaskGroup.");
      if (!taskGroup.DefinitionType.Equals("metaTask", StringComparison.OrdinalIgnoreCase))
        throw new InvalidTaskDefinitionTypeException(TaskResources.InvalidTaskDefinitionTypeForTaskGroup((object) taskGroup.DefinitionType, (object) taskGroup.Name, (object) "metaTask"));
      MetaTaskDefinitionExtensions.TaskInputsShouldBeUnique(taskGroup);
      IEnumerable<TaskDefinition> taskDefinitions = MetaTaskDefinitionExtensions.GetTaskDefinitions(requestContext, projectId, taskGroup.HasMetaTask());
      string str = string.Empty;
      if (taskGroup.HasMetaTask())
        str = !(taskGroup.Version != (TaskVersion) null) || !taskGroup.Version.IsTest ? MetaTaskDefinitionExtensions.GetCyclicDependencyPathIfAny(taskGroup.Id, new Guid?(), (IEnumerable<TaskGroupStep>) taskGroup.Tasks, taskDefinitions) : MetaTaskDefinitionExtensions.GetCyclicDependencyPathIfAny(taskGroup.Id, taskGroup.ParentDefinitionId, (IEnumerable<TaskGroupStep>) taskGroup.Tasks, taskDefinitions);
      if (!string.IsNullOrWhiteSpace(str))
        throw new TaskGroupCyclicDependencyException(TaskResources.TaskGroupCyclicDependencyErrorFormat((object) TaskResources.TaskGroupCyclicDependencyPathItemFormat((object) taskGroup.Name, (object) str)));
      IOrderedEnumerable<TaskDefinition> orderedTaskDefinitions = taskDefinitions.OrderByDescending<TaskDefinition, TaskVersion>((Func<TaskDefinition, TaskVersion>) (td => td.Version));
      IEnumerable<TaskDefinition> childTaskDefinitions = MetaTaskDefinitionExtensions.ValidateChildTasks(taskGroup, orderedTaskDefinitions);
      MetaTaskDefinitionExtensions.ValidateRunsOn(taskGroup, childTaskDefinitions);
    }

    private static bool IsTestTaskGroup(TaskGroup taskGroup) => taskGroup.Version != (TaskVersion) null && taskGroup.Version.IsTest;

    public static void FixLatestMajorTaskVersions(
      this TaskGroup definition,
      IVssRequestContext requestContext)
    {
      if (definition == null)
        return;
      Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
      foreach (TaskGroupStep taskGroupStep in definition.Tasks.Where<TaskGroupStep>((Func<TaskGroupStep, bool>) (t => t.Task.IsTask() && t.Task.VersionSpec == "*")))
      {
        string str = (string) null;
        if (!dictionary.TryGetValue(taskGroupStep.Task.Id, out str))
        {
          TaskDefinition latestMajorVersion = requestContext.GetService<IDistributedTaskPoolService>().GetLatestMajorVersion(requestContext, taskGroupStep.Task.Id);
          if (latestMajorVersion != null)
            str = string.Format("{0}.*", (object) latestMajorVersion.Version.Major);
          dictionary[taskGroupStep.Task.Id] = str;
        }
        if (!string.IsNullOrEmpty(str))
          taskGroupStep.Task.VersionSpec = str;
      }
    }

    public static TaskGroup ResolveIdentityRefs(
      this TaskGroup taskGroup,
      IVssRequestContext requestContext)
    {
      IDictionary<string, IdentityRef> dictionary = new HashSet<string>()
      {
        taskGroup.CreatedBy.Id,
        taskGroup.ModifiedBy.Id
      }.QueryIdentities(requestContext);
      taskGroup.CreatedBy = dictionary[taskGroup.CreatedBy.Id];
      taskGroup.ModifiedBy = dictionary[taskGroup.ModifiedBy.Id];
      return taskGroup;
    }

    public static IEnumerable<TaskGroup> ResolveIdentityRefs(
      this IEnumerable<TaskGroup> taskGroups,
      IVssRequestContext requestContext)
    {
      if (!(taskGroups is IList<TaskGroup> taskGroupList1))
        taskGroupList1 = (IList<TaskGroup>) taskGroups.ToList<TaskGroup>();
      IList<TaskGroup> taskGroupList2 = taskGroupList1;
      HashSet<string> identityIds = new HashSet<string>();
      foreach (TaskGroup taskGroup in (IEnumerable<TaskGroup>) taskGroupList2)
      {
        identityIds.Add(taskGroup.CreatedBy.Id);
        identityIds.Add(taskGroup.ModifiedBy.Id);
      }
      IDictionary<string, IdentityRef> dictionary = identityIds.QueryIdentities(requestContext);
      foreach (TaskGroup taskGroup in (IEnumerable<TaskGroup>) taskGroupList2)
      {
        taskGroup.CreatedBy = dictionary[taskGroup.CreatedBy.Id];
        taskGroup.ModifiedBy = dictionary[taskGroup.ModifiedBy.Id];
      }
      return (IEnumerable<TaskGroup>) taskGroupList2;
    }

    public static Dictionary<Guid, IList<string>> GetRunsOnForTaskGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<TaskGroup> expandedTaskGroupDefinition,
      IList<TaskDefinition> taskDefinitions)
    {
      Dictionary<Guid, IList<string>> runsOnForTaskGroups = new Dictionary<Guid, IList<string>>();
      int num = 0;
      foreach (TaskGroup taskGroup in (IEnumerable<TaskGroup>) expandedTaskGroupDefinition)
      {
        try
        {
          IList<string> runsOn = taskGroup.RunsOn;
          IList<TaskDefinition> definitionsForTaskGroup = MetaTaskDefinitionExtensions.GetValidTasksDefinitionsForTaskGroup(requestContext, taskDefinitions, taskGroup.Tasks);
          IList<string> second = (IList<string>) null;
          foreach (TaskDefinition taskDefinition in (IEnumerable<TaskDefinition>) definitionsForTaskGroup)
            second = second == null ? taskDefinition.RunsOn : (IList<string>) taskDefinition.RunsOn.Intersect<string>((IEnumerable<string>) second, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();
          if (second != null)
          {
            runsOnForTaskGroups.Add(taskGroup.Id, second);
            ++num;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceWarning("TaskGroup", string.Format("Error occurred while processing task group {0} - Id {1}  Project - {2}. Error - {3}", (object) taskGroup.Name, (object) taskGroup.Id, (object) projectId, (object) ex.ToString()));
        }
      }
      return runsOnForTaskGroups;
    }

    private static IList<TaskDefinition> GetValidTasksDefinitionsForTaskGroup(
      IVssRequestContext requestContext,
      IList<TaskDefinition> allTaskDefinitions,
      IList<TaskGroupStep> taskGroupSteps)
    {
      List<TaskDefinition> definitionsForTaskGroup = new List<TaskDefinition>();
      Dictionary<Guid, IList<TaskVersion>> taskVersions;
      Dictionary<Guid, IDictionary<string, TaskDefinition>> taskDefinitions;
      MetaTaskDefinitionExtensions.FillTaskDetails(taskGroupSteps, allTaskDefinitions, out taskVersions, out taskDefinitions);
      TaskVersionResolver taskVersionResolver = new TaskVersionResolver((IDictionary<Guid, IList<TaskVersion>>) taskVersions);
      foreach (TaskGroupStep taskGroupStep in (IEnumerable<TaskGroupStep>) taskGroupSteps)
      {
        TaskVersion taskVersion;
        if (taskVersionResolver.TryResolveVersion(taskGroupStep.Task.Id, taskGroupStep.Task.VersionSpec, out taskVersion))
        {
          TaskDefinition taskDefinition = taskDefinitions[taskGroupStep.Task.Id][taskVersion.ToString()];
          if (taskDefinition != null)
            definitionsForTaskGroup.Add(taskDefinition);
          else
            requestContext.TraceWarning("TaskGroup", string.Format("Unable to get task definition for task id {0} with version {1}", (object) taskGroupStep.Task.Id, (object) taskGroupStep.Task.VersionSpec));
        }
      }
      return (IList<TaskDefinition>) definitionsForTaskGroup;
    }

    private static void FillTaskDetails(
      IList<TaskGroupStep> tasks,
      IList<TaskDefinition> allTaskDefinitions,
      out Dictionary<Guid, IList<TaskVersion>> taskVersions,
      out Dictionary<Guid, IDictionary<string, TaskDefinition>> taskDefinitions)
    {
      taskVersions = new Dictionary<Guid, IList<TaskVersion>>();
      taskDefinitions = new Dictionary<Guid, IDictionary<string, TaskDefinition>>();
      foreach (Guid guid in tasks.Select<TaskGroupStep, Guid>((Func<TaskGroupStep, Guid>) (task => task.Task.Id)).Distinct<Guid>())
      {
        Guid taskId = guid;
        List<TaskVersion> taskVersionList = new List<TaskVersion>();
        Dictionary<string, TaskDefinition> dictionary = new Dictionary<string, TaskDefinition>();
        foreach (TaskDefinition taskDefinition in allTaskDefinitions.Where<TaskDefinition>((Func<TaskDefinition, bool>) (x => x.Id.Equals(taskId))))
        {
          taskVersionList.Add(taskDefinition.Version);
          dictionary.Add((string) taskDefinition.Version, taskDefinition);
        }
        taskVersions.Add(taskId, (IList<TaskVersion>) taskVersionList);
        taskDefinitions.Add(taskId, (IDictionary<string, TaskDefinition>) dictionary);
      }
    }

    private static void ValidateRunsOn(
      TaskGroup taskGroup,
      IEnumerable<TaskDefinition> childTaskDefinitions)
    {
      foreach (TaskDefinition childTaskDefinition in childTaskDefinitions)
      {
        if (!childTaskDefinition.RunsOn.Intersect<string>((IEnumerable<string>) taskGroup.RunsOn, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Count<string>().Equals(taskGroup.RunsOn.Count))
        {
          string str1 = string.Join(", ", (IEnumerable<string>) childTaskDefinition.RunsOn);
          string str2 = string.Join(", ", (IEnumerable<string>) taskGroup.RunsOn);
          throw new MetaTaskDefinitionRunsOnMismatchException(TaskResources.MetaTaskDefinitionRunsOnMismatch((object) taskGroup.Name, (object) str2, (object) childTaskDefinition.Name, (object) str1));
        }
      }
    }

    private static IEnumerable<TaskDefinition> ValidateChildTasks(
      TaskGroup taskGroup,
      IOrderedEnumerable<TaskDefinition> orderedTaskDefinitions)
    {
      List<TaskDefinition> taskDefinitionList = new List<TaskDefinition>();
      foreach (TaskGroupStep task in (IEnumerable<TaskGroupStep>) taskGroup.Tasks)
      {
        TaskGroupStep taskGroupStep = task;
        TaskDefinition taskDefinitionFromServer = orderedTaskDefinitions.FirstOrDefault<TaskDefinition>((Func<TaskDefinition, bool>) (td =>
        {
          if (td == null)
            return false;
          Guid id = td.Id;
          return td.Id.Equals((object) taskGroupStep?.Task?.Id);
        }));
        if (taskDefinitionFromServer == null)
        {
          if (taskGroupStep?.Task?.DefinitionType != null && taskGroupStep.Task.DefinitionType.Equals("metaTask"))
            throw new MetaTaskDefinitionNotFoundException(TaskResources.MetaTaskDefinitionNotFound((object) taskGroupStep.Task.Id));
          throw new TaskDefinitionNotFoundException(TaskResources.TaskDefinitionNotFound((object) taskGroupStep?.Task?.Id, (object) taskGroupStep?.Task?.VersionSpec));
        }
        MetaTaskDefinitionExtensions.ValidTaskDefinitionType(taskDefinitionFromServer, taskGroupStep);
        taskDefinitionList.Add(taskDefinitionFromServer);
      }
      return (IEnumerable<TaskDefinition>) taskDefinitionList;
    }

    private static void ValidTaskDefinitionType(
      TaskDefinition taskDefinitionFromServer,
      TaskGroupStep taskGroupStep)
    {
      if (taskDefinitionFromServer.DefinitionType.Equals(taskGroupStep.Task.DefinitionType, StringComparison.OrdinalIgnoreCase))
        return;
      if (taskDefinitionFromServer.DefinitionType.Equals("metaTask", StringComparison.OrdinalIgnoreCase))
        throw new InvalidTaskDefinitionTypeException(TaskResources.InvalidTaskDefinitionTypeForTaskGroup((object) taskGroupStep.Task.DefinitionType, (object) taskGroupStep.DisplayName, (object) "metaTask"));
      if (!taskGroupStep.Task.DefinitionType.IsNullOrEmpty<char>())
        throw new InvalidTaskDefinitionTypeException(TaskResources.InvalidTaskDefinitionTypeForTask((object) taskGroupStep.Task.DefinitionType, (object) taskGroupStep.DisplayName, (object) "task"));
    }

    private static IEnumerable<TaskDefinition> GetTaskDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      bool includeTaskGroups)
    {
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      IList<TaskDefinition> taskDefinitions = service.GetTaskDefinitions(requestContext);
      if (includeTaskGroups)
      {
        foreach (TaskGroup taskGroup in (IEnumerable<TaskGroup>) service.GetTaskGroups(requestContext, projectId))
          taskDefinitions.Add((TaskDefinition) taskGroup);
      }
      return (IEnumerable<TaskDefinition>) taskDefinitions;
    }

    private static void TaskInputsShouldBeUnique(TaskGroup taskGroup)
    {
      for (int index1 = 0; index1 < taskGroup.Inputs.Count; ++index1)
      {
        for (int index2 = index1 + 1; index2 < taskGroup.Inputs.Count; ++index2)
        {
          if (taskGroup.Inputs[index1].Name.Equals(taskGroup.Inputs[index2].Name, StringComparison.OrdinalIgnoreCase))
            throw new InvalidTaskDefinitionInputsException(TaskResources.DuplicateTaskDefinitionInputKey((object) string.Join(", ", new string[2]
            {
              taskGroup.Inputs[index1].Name,
              taskGroup.Inputs[index2].Name
            })));
        }
      }
    }

    private static string GetCyclicDependencyPathIfAny(
      Guid taskGroupId,
      Guid? parentTaskGroupId,
      IEnumerable<TaskGroupStep> childSteps,
      IEnumerable<TaskDefinition> allTasks)
    {
      TaskGroupStep matchingStep = childSteps.FirstOrDefault<TaskGroupStep>((Func<TaskGroupStep, bool>) (step => step.Task.Id.Equals(taskGroupId)));
      if (matchingStep == null && parentTaskGroupId.HasValue)
        matchingStep = childSteps.FirstOrDefault<TaskGroupStep>((Func<TaskGroupStep, bool>) (step => step.Task.Id.Equals((object) parentTaskGroupId)));
      if (matchingStep != null)
      {
        TaskDefinition taskDefinition = allTasks.FirstOrDefault<TaskDefinition>((Func<TaskDefinition, bool>) (task => task.Id == matchingStep.Task.Id));
        return taskDefinition != null ? taskDefinition.Name : matchingStep.DisplayName;
      }
      foreach (TaskGroupStep childStep in childSteps)
      {
        TaskGroupStep step = childStep;
        TaskDefinition taskDefinition = allTasks.FirstOrDefault<TaskDefinition>((Func<TaskDefinition, bool>) (task => task.Id == step.Task.Id));
        if (taskDefinition != null && taskDefinition.DefinitionType.Equals("metaTask", StringComparison.OrdinalIgnoreCase))
        {
          TaskGroup taskGroup = taskDefinition as TaskGroup;
          string dependencyPathIfAny = MetaTaskDefinitionExtensions.GetCyclicDependencyPathIfAny(taskGroupId, parentTaskGroupId, (IEnumerable<TaskGroupStep>) taskGroup.Tasks, allTasks);
          if (!string.IsNullOrWhiteSpace(dependencyPathIfAny))
            return TaskResources.TaskGroupCyclicDependencyPathItemFormat((object) taskGroup.Name, (object) dependencyPathIfAny);
        }
      }
      return (string) null;
    }
  }
}
