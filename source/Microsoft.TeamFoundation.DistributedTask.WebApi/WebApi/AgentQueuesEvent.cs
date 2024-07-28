// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.AgentQueuesEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [ServiceEventObject]
  public class AgentQueuesEvent
  {
    public AgentQueuesEvent(string eventType, IEnumerable<TaskAgentQueue> queues)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(eventType, nameof (eventType));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) queues, nameof (queues));
      this.EventType = eventType;
      this.Queues = (IEnumerable<TaskAgentQueue>) new List<TaskAgentQueue>(queues);
    }

    [DataMember]
    public string EventType { get; set; }

    [DataMember]
    public IEnumerable<TaskAgentQueue> Queues { get; set; }
  }
}
