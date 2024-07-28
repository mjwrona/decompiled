// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointRequest
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
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
