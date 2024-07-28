// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionData
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public sealed class ServiceEndpointExecutionData
  {
    public ServiceEndpointExecutionData()
    {
    }

    public ServiceEndpointExecutionData(
      string planType,
      ServiceEndpointExecutionOwner definition,
      ServiceEndpointExecutionOwner owner,
      DateTime startTime,
      DateTime finishTime,
      ServiceEndpointExecutionResult result,
      string ownerDetails = null)
    {
      this.PlanType = planType;
      this.Definition = definition;
      this.Owner = owner;
      this.OwnerDetails = ownerDetails;
      this.StartTime = new DateTime?(startTime);
      this.FinishTime = new DateTime?(finishTime);
      this.Result = new ServiceEndpointExecutionResult?(result);
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
      this.OwnerDetails = executionDataToBeCloned.OwnerDetails;
    }

    [DataMember(EmitDefaultValue = false)]
    public long Id { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public string PlanType { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public ServiceEndpointExecutionOwner Definition { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public ServiceEndpointExecutionOwner Owner { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public ServiceEndpointExecutionResult? Result { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public string OwnerDetails { get; internal set; }

    public ServiceEndpointExecutionData Clone() => new ServiceEndpointExecutionData(this);
  }
}
