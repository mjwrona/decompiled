// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ResourceLockRequest
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class ResourceLockRequest
  {
    public ResourceLockRequest()
    {
    }

    private ResourceLockRequest(ResourceLockRequest requestToBeCloned)
    {
      this.RequestId = requestToBeCloned.RequestId;
      this.QueueTime = requestToBeCloned.QueueTime;
      this.AssignTime = requestToBeCloned.AssignTime;
      this.FinishTime = requestToBeCloned.FinishTime;
      this.Status = requestToBeCloned.Status;
      this.PlanId = requestToBeCloned.PlanId;
      this.NodeName = requestToBeCloned.NodeName;
      this.NodeAttempt = requestToBeCloned.NodeAttempt;
      this.ResourceId = requestToBeCloned.ResourceId;
      this.ResourceType = requestToBeCloned.ResourceType;
      this.ProjectId = requestToBeCloned.ProjectId;
      this.CheckRunId = requestToBeCloned.CheckRunId;
      this.DefinitionId = requestToBeCloned.DefinitionId;
    }

    [DataMember]
    public long RequestId { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime QueueTime { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? AssignTime { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceLockStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid PlanId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string NodeName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int NodeAttempt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResourceId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResourceType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ProjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid CheckRunId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int DefinitionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ExclusiveLockType LockType { get; set; }

    public ResourceLockRequest Clone() => new ResourceLockRequest(this);
  }
}
