// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionData
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class ServiceEndpointExecutionData
  {
    public ServiceEndpointExecutionData()
    {
    }

    public ServiceEndpointExecutionData(
      string planType,
      TaskOrchestrationOwner definition,
      TaskOrchestrationOwner owner,
      DateTime startTime,
      DateTime finishTime,
      TaskResult result)
    {
      this.PlanType = planType;
      this.Definition = definition;
      this.Owner = owner;
      this.StartTime = new DateTime?(startTime);
      this.FinishTime = new DateTime?(finishTime);
      this.Result = new TaskResult?(result);
    }

    private ServiceEndpointExecutionData(
      ServiceEndpointExecutionData executionDataToBeCloned)
    {
      this.Id = executionDataToBeCloned.Id;
      this.PlanType = executionDataToBeCloned.PlanType;
      this.Definition = executionDataToBeCloned.Definition.Clone();
      this.Owner = executionDataToBeCloned.Owner.Clone();
      this.StartTime = executionDataToBeCloned.StartTime;
      this.FinishTime = executionDataToBeCloned.FinishTime;
      this.Result = executionDataToBeCloned.Result;
    }

    [DataMember(EmitDefaultValue = false)]
    public long Id { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public string PlanType { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskOrchestrationOwner Definition { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskOrchestrationOwner Owner { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskResult? Result { get; internal set; }

    public ServiceEndpointExecutionData Clone() => new ServiceEndpointExecutionData(this);
  }
}
