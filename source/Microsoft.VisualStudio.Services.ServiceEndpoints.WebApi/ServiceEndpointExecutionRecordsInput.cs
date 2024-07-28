// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecordsInput
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public sealed class ServiceEndpointExecutionRecordsInput
  {
    private IList<Guid> m_EndpointIds;

    public ServiceEndpointExecutionRecordsInput()
    {
    }

    public ServiceEndpointExecutionRecordsInput(
      IList<Guid> endpointIds,
      ServiceEndpointExecutionData data)
    {
      this.EndpointIds = endpointIds;
      this.Data = data;
    }

    private ServiceEndpointExecutionRecordsInput(
      ServiceEndpointExecutionRecordsInput executionRecordsInputToBeCloned)
    {
      if (executionRecordsInputToBeCloned.EndpointIds != null)
        this.EndpointIds = (IList<Guid>) new List<Guid>((IEnumerable<Guid>) executionRecordsInputToBeCloned.EndpointIds);
      if (executionRecordsInputToBeCloned.Data == null)
        return;
      this.Data = executionRecordsInputToBeCloned.Data.Clone();
    }

    [DataMember(EmitDefaultValue = false)]
    public IList<Guid> EndpointIds
    {
      get => this.m_EndpointIds ?? (this.m_EndpointIds = (IList<Guid>) new List<Guid>());
      internal set => this.m_EndpointIds = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public ServiceEndpointExecutionData Data { get; internal set; }

    public ServiceEndpointExecutionRecordsInput Clone() => new ServiceEndpointExecutionRecordsInput(this);
  }
}
