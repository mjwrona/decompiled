// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ServerExecutionDefinition
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class ServerExecutionDefinition
  {
    [DataMember(EmitDefaultValue = false, Name = "Events")]
    private EventsConfig m_EventsConfig;

    [JsonConstructor]
    internal ServerExecutionDefinition()
    {
    }

    protected ServerExecutionDefinition(string handlerName) => this.HandlerName = handlerName;

    [DataMember]
    public string HandlerName { get; }

    public EventsConfig EventsConfig
    {
      get
      {
        if (this.m_EventsConfig == null)
          this.m_EventsConfig = (EventsConfig) new JobEventsConfig();
        return this.m_EventsConfig;
      }
      set => this.m_EventsConfig = value;
    }
  }
}
