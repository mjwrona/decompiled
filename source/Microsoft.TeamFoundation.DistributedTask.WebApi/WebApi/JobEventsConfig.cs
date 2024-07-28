// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.JobEventsConfig
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class JobEventsConfig : EventsConfig
  {
    private Dictionary<string, JobEventConfig> m_jobEvents = new Dictionary<string, JobEventConfig>();

    [DataMember(EmitDefaultValue = false)]
    public JobEventConfig JobAssigned
    {
      get => this.GetEvent(nameof (JobAssigned));
      set => this.SetEvent(nameof (JobAssigned), value);
    }

    [DataMember(EmitDefaultValue = false)]
    public JobEventConfig JobStarted
    {
      get => this.GetEvent(nameof (JobStarted));
      set => this.SetEvent(nameof (JobStarted), value);
    }

    [DataMember(EmitDefaultValue = false)]
    public JobEventConfig JobCompleted
    {
      get => this.GetEvent(nameof (JobCompleted));
      set => this.SetEvent(nameof (JobCompleted), value);
    }

    public Dictionary<string, JobEventConfig> All => this.m_jobEvents;

    private void SetEvent(string name, JobEventConfig jobEventConfig) => this.m_jobEvents[name] = jobEventConfig;

    private JobEventConfig GetEvent(string name)
    {
      JobEventConfig jobEventConfig;
      return this.m_jobEvents.TryGetValue(name, out jobEventConfig) ? jobEventConfig : (JobEventConfig) null;
    }
  }
}
