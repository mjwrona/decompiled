// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationQueuedPlanGroup
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskOrchestrationQueuedPlanGroup
  {
    private List<TaskOrchestrationQueuedPlan> _plans;

    [DataMember]
    public ProjectReference Project { get; internal set; }

    [DataMember]
    public string PlanGroup { get; internal set; }

    [DataMember]
    public int QueuePosition { get; internal set; }

    [DataMember]
    public List<TaskOrchestrationQueuedPlan> Plans
    {
      get
      {
        if (this._plans == null)
          this._plans = new List<TaskOrchestrationQueuedPlan>();
        return this._plans;
      }
    }

    [DataMember]
    public TaskOrchestrationOwner Definition { get; set; }

    [DataMember]
    public TaskOrchestrationOwner Owner { get; set; }
  }
}
