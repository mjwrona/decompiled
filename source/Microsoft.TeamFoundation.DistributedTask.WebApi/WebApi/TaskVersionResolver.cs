// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskVersionResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public sealed class TaskVersionResolver
  {
    private IDictionary<Guid, IList<TaskVersion>> m_taskVersions;
    private IDictionary<Guid, IDictionary<string, TaskDefinition>> m_taskDefinitions;

    public TaskVersionResolver(IDictionary<Guid, IList<TaskVersion>> taskVersions)
      : this((IDictionary<Guid, IDictionary<string, TaskDefinition>>) null, taskVersions)
    {
    }

    public TaskVersionResolver(
      IDictionary<Guid, IDictionary<string, TaskDefinition>> taskDefinitions,
      IDictionary<Guid, IList<TaskVersion>> taskVersions)
    {
      this.m_taskDefinitions = taskDefinitions ?? (IDictionary<Guid, IDictionary<string, TaskDefinition>>) new Dictionary<Guid, IDictionary<string, TaskDefinition>>();
      this.m_taskVersions = taskVersions ?? (IDictionary<Guid, IList<TaskVersion>>) new Dictionary<Guid, IList<TaskVersion>>();
    }

    public TaskVersion ResolveVersion(Guid taskId, string version)
    {
      IList<TaskVersion> versions;
      if (!this.m_taskVersions.TryGetValue(taskId, out versions))
        throw new NotSupportedException(taskId.ToString("D"));
      return TaskVersionSpec.Parse(version).Match((IEnumerable<TaskVersion>) versions);
    }

    public bool TryResolveVersion(Guid taskId, string version, out TaskVersion taskVersion)
    {
      taskVersion = (TaskVersion) null;
      if (string.IsNullOrEmpty(version))
        version = "*";
      IList<TaskVersion> versions;
      if (this.m_taskVersions.TryGetValue(taskId, out versions))
      {
        TaskVersionSpec taskVersionSpec = TaskVersionSpec.Parse(version);
        taskVersion = taskVersionSpec.Match((IEnumerable<TaskVersion>) versions);
      }
      return taskVersion != (TaskVersion) null;
    }
  }
}
