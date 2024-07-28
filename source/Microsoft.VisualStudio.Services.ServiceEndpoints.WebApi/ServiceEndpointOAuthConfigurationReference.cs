// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointOAuthConfigurationReference
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class ServiceEndpointOAuthConfigurationReference
  {
    public ServiceEndpointOAuthConfigurationReference()
    {
    }

    public ServiceEndpointOAuthConfigurationReference(
      Guid endpointProjectId,
      Guid endpointId,
      Guid configurationId)
    {
      this.ServiceEndpointProjectId = endpointProjectId;
      this.ServiceEndpointId = endpointId;
      this.ConfigurationId = configurationId;
    }

    [DataMember]
    public Guid ServiceEndpointId { get; set; }

    [DataMember]
    public Guid ConfigurationId { get; set; }

    [DataMember]
    public Guid ServiceEndpointProjectId { get; set; }
  }
}
