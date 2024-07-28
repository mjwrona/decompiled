// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointPartial
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  [DataContract]
  public class ServiceEndpointPartial
  {
    public ServiceEndpointPartial()
    {
    }

    private ServiceEndpointPartial(ServiceEndpointPartial endpointDetailsToClone)
    {
      this.Id = endpointDetailsToClone.Id;
      this.Name = endpointDetailsToClone.Name;
      this.CreatedBy = endpointDetailsToClone.CreatedBy;
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    public ServiceEndpointPartial Clone() => new ServiceEndpointPartial(this);
  }
}
