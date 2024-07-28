// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.AgentChangeEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [ServiceEventObject]
  public class AgentChangeEvent
  {
    [DataMember(Name = "Pool")]
    private TaskAgentPoolReference m_pool;

    [JsonConstructor]
    internal AgentChangeEvent()
    {
    }

    [Obsolete]
    public AgentChangeEvent(string eventType, int poolId, TaskAgent agent, DateTime timeStamp)
    {
      this.Agent = agent;
      this.EventType = eventType;
      this.PoolId = poolId;
      this.TimeStamp = timeStamp;
    }

    public AgentChangeEvent(string eventType, TaskAgent agent, TaskAgentPoolReference pool)
    {
      this.Agent = agent;
      this.EventType = eventType;
      this.m_pool = pool;
      this.TimeStamp = DateTime.Now;
      if (pool == null)
        return;
      this.PoolId = pool.Id;
    }

    [DataMember]
    public TaskAgent Agent { get; set; }

    [DataMember]
    public string EventType { get; set; }

    [Obsolete]
    [DataMember]
    public int PoolId { get; set; }

    [Obsolete]
    [DataMember]
    public DateTime TimeStamp { get; set; }

    public TaskAgentPoolReference Pool
    {
      get
      {
        if (this.m_pool == null)
          this.m_pool = new TaskAgentPoolReference(Guid.Empty, this.PoolId);
        return this.m_pool;
      }
    }
  }
}
