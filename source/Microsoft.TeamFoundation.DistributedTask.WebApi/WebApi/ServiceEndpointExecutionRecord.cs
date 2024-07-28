// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class ServiceEndpointExecutionRecord
  {
    public ServiceEndpointExecutionRecord()
    {
    }

    public ServiceEndpointExecutionRecord(Guid endpointId, ServiceEndpointExecutionData data)
    {
      this.EndpointId = endpointId;
      this.Data = data;
    }

    private ServiceEndpointExecutionRecord(
      ServiceEndpointExecutionRecord executionRecordToBeCloned)
    {
      this.EndpointId = executionRecordToBeCloned.EndpointId;
      if (executionRecordToBeCloned.Data == null)
        return;
      this.Data = executionRecordToBeCloned.Data.Clone();
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid EndpointId { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public ServiceEndpointExecutionData Data { get; internal set; }

    public ServiceEndpointExecutionRecord Clone() => new ServiceEndpointExecutionRecord(this);
  }
}
