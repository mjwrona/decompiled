// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskEventsConfig
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [ClientIgnore]
  [DataContract]
  public sealed class TaskEventsConfig : EventsConfig
  {
    private Dictionary<string, TaskEventConfig> m_taskEvents = new Dictionary<string, TaskEventConfig>();

    [DataMember(EmitDefaultValue = false)]
    public TaskEventConfig TaskAssigned
    {
      get => this.GetEvent(nameof (TaskAssigned));
      set => this.SetEvent(nameof (TaskAssigned), value);
    }

    [DataMember(EmitDefaultValue = false)]
    public TaskEventConfig TaskStarted
    {
      get => this.GetEvent(nameof (TaskStarted));
      set => this.SetEvent(nameof (TaskStarted), value);
    }

    [DataMember(EmitDefaultValue = false)]
    public TaskEventConfig TaskCompleted
    {
      get => this.GetEvent(nameof (TaskCompleted));
      set => this.SetEvent(nameof (TaskCompleted), value);
    }

    public Dictionary<string, TaskEventConfig> All => this.m_taskEvents;

    private void SetEvent(string name, TaskEventConfig taskEventConfig) => this.m_taskEvents[name] = taskEventConfig;

    private TaskEventConfig GetEvent(string name)
    {
      TaskEventConfig taskEventConfig;
      return this.m_taskEvents.TryGetValue(name, out taskEventConfig) ? taskEventConfig : (TaskEventConfig) null;
    }
  }
}
