// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionOwner
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class ServiceEndpointExecutionOwner : ICloneable
  {
    [DataMember(Name = "_links")]
    private ReferenceLinks m_links;

    public ServiceEndpointExecutionOwner()
    {
    }

    private ServiceEndpointExecutionOwner(ServiceEndpointExecutionOwner ownerToBeCloned)
    {
      this.Id = ownerToBeCloned.Id;
      this.Name = ownerToBeCloned.Name;
      this.m_links = ownerToBeCloned.Links.Clone();
    }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }

    public ServiceEndpointExecutionOwner Clone() => new ServiceEndpointExecutionOwner(this);

    object ICloneable.Clone() => (object) this.Clone();
  }
}
