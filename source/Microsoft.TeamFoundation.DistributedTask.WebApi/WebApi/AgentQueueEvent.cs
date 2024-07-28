// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.AgentQueueEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [ServiceEventObject]
  public class AgentQueueEvent
  {
    public AgentQueueEvent(string eventType, TaskAgentQueue queue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(eventType, nameof (eventType));
      ArgumentUtility.CheckForNull<TaskAgentQueue>(queue, nameof (queue));
      this.EventType = eventType;
      this.Queue = queue;
    }

    [DataMember]
    public string EventType { get; set; }

    [DataMember]
    public TaskAgentQueue Queue { get; set; }
  }
}
