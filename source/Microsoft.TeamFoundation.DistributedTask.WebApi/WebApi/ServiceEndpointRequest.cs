// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointRequest
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class ServiceEndpointRequest
  {
    public ServiceEndpointRequest()
    {
    }

    private ServiceEndpointRequest(ServiceEndpointRequest endpointRequestToClone)
    {
      if (endpointRequestToClone.ServiceEndpointDetails != null)
        this.ServiceEndpointDetails = endpointRequestToClone.ServiceEndpointDetails.Clone();
      if (endpointRequestToClone.DataSourceDetails != null)
        this.DataSourceDetails = endpointRequestToClone.DataSourceDetails.Clone();
      if (endpointRequestToClone.ResultTransformationDetails == null)
        return;
      this.ResultTransformationDetails = endpointRequestToClone.ResultTransformationDetails.Clone();
    }

    [DataMember(EmitDefaultValue = false)]
    public ServiceEndpointDetails ServiceEndpointDetails { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DataSourceDetails DataSourceDetails { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResultTransformationDetails ResultTransformationDetails { get; set; }

    public ServiceEndpointRequest Clone() => new ServiceEndpointRequest(this);
  }
}
