// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskDefinitionResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public class TaskDefinitionResolver
  {
    private readonly Dictionary<Guid, Dictionary<TaskVersion, TaskDefinition>> m_taskMap = new Dictionary<Guid, Dictionary<TaskVersion, TaskDefinition>>();

    public TaskDefinitionResolver(IList<TaskDefinition> allTasks)
    {
      if (allTasks == null)
        return;
      foreach (TaskDefinition allTask in (IEnumerable<TaskDefinition>) allTasks)
      {
        Dictionary<TaskVersion, TaskDefinition> dictionary = (Dictionary<TaskVersion, TaskDefinition>) null;
        if (!this.m_taskMap.TryGetValue(allTask.Id, out dictionary))
        {
          dictionary = new Dictionary<TaskVersion, TaskDefinition>();
          this.m_taskMap.Add(allTask.Id, dictionary);
        }
        dictionary[allTask.Version] = allTask;
      }
    }

    public bool TryResolveTaskReference(
      Guid taskId,
      string versionSpec,
      out TaskDefinition taskDefinition)
    {
      taskDefinition = (TaskDefinition) null;
      if (string.IsNullOrEmpty(versionSpec))
        versionSpec = "*";
      TaskVersionSpec versionSpec1 = (TaskVersionSpec) null;
      return TaskVersionSpec.TryParse(versionSpec, out versionSpec1) && this.TryResolveTaskReference(taskId, versionSpec1, out taskDefinition);
    }

    public bool TryResolveTaskReference(
      Guid taskId,
      TaskVersionSpec versionSpec,
      out TaskDefinition taskDefinition)
    {
      taskDefinition = (TaskDefinition) null;
      Dictionary<TaskVersion, TaskDefinition> dictionary = (Dictionary<TaskVersion, TaskDefinition>) null;
      if (this.m_taskMap.TryGetValue(taskId, out dictionary))
      {
        TaskVersion key = versionSpec.Match((IEnumerable<TaskVersion>) dictionary.Keys.ToList<TaskVersion>());
        if (key != (TaskVersion) null)
          return dictionary.TryGetValue(key, out taskDefinition);
      }
      return false;
    }
  }
}
