// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskExecution
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskExecution
  {
    public TaskExecution()
    {
    }

    private TaskExecution(TaskExecution taskExecutionToBeCloned)
    {
      if (taskExecutionToBeCloned.ExecTask != null)
        this.ExecTask = taskExecutionToBeCloned.ExecTask.Clone();
      if (taskExecutionToBeCloned.PlatformInstructions == null)
        return;
      this.PlatformInstructions = new Dictionary<string, Dictionary<string, string>>((IDictionary<string, Dictionary<string, string>>) taskExecutionToBeCloned.PlatformInstructions, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember(Order = 10, EmitDefaultValue = false)]
    public TaskReference ExecTask { get; set; }

    [DataMember(Order = 20, EmitDefaultValue = false)]
    public Dictionary<string, Dictionary<string, string>> PlatformInstructions { get; set; }

    internal TaskExecution Clone() => new TaskExecution(this);
  }
}
