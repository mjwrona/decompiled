// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentUpdate
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentUpdate
  {
    internal TaskAgentUpdate()
    {
    }

    private TaskAgentUpdate(TaskAgentUpdate agentUpdateToBeCloned)
    {
      this.CurrentState = agentUpdateToBeCloned.CurrentState;
      if (agentUpdateToBeCloned.SourceVersion != null)
        this.SourceVersion = agentUpdateToBeCloned.SourceVersion.Clone();
      if (agentUpdateToBeCloned.TargetVersion != null)
        this.TargetVersion = agentUpdateToBeCloned.TargetVersion.Clone();
      if (agentUpdateToBeCloned.RequestTime.HasValue)
        this.RequestTime = agentUpdateToBeCloned.RequestTime;
      if (agentUpdateToBeCloned.RequestedBy != null)
        this.RequestedBy = agentUpdateToBeCloned.RequestedBy.Clone();
      if (agentUpdateToBeCloned.Reason == null)
        return;
      switch (agentUpdateToBeCloned.Reason.Code)
      {
        case TaskAgentUpdateReasonType.Manual:
          this.Reason = (TaskAgentUpdateReason) (agentUpdateToBeCloned.Reason as TaskAgentManualUpdate).Clone();
          break;
        case TaskAgentUpdateReasonType.MinAgentVersionRequired:
          this.Reason = (TaskAgentUpdateReason) (agentUpdateToBeCloned.Reason as TaskAgentMinAgentVersionRequiredUpdate).Clone();
          break;
        case TaskAgentUpdateReasonType.Downgrade:
          this.Reason = (TaskAgentUpdateReason) (agentUpdateToBeCloned.Reason as TaskAgentDowngrade).Clone();
          break;
      }
    }

    [DataMember]
    public PackageVersion SourceVersion { get; internal set; }

    [DataMember]
    public PackageVersion TargetVersion { get; internal set; }

    [DataMember]
    public DateTime? RequestTime { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef RequestedBy { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public string CurrentState { get; set; }

    [DataMember]
    public TaskAgentUpdateReason Reason { get; set; }

    public TaskAgentUpdate Clone() => new TaskAgentUpdate(this);
  }
}
