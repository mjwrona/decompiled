// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointDetails
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class ServiceEndpointDetails
  {
    [DataMember(EmitDefaultValue = false, Name = "Data")]
    private Dictionary<string, string> data;

    public ServiceEndpointDetails()
    {
    }

    private ServiceEndpointDetails(ServiceEndpointDetails endpointDetailsToClone)
    {
      this.Type = endpointDetailsToClone.Type;
      this.Url = endpointDetailsToClone.Url;
      if (endpointDetailsToClone.Authorization != null)
        this.Authorization = endpointDetailsToClone.Authorization.Clone();
      if (endpointDetailsToClone.data == null)
        return;
      this.data = new Dictionary<string, string>((IDictionary<string, string>) endpointDetailsToClone.data, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Uri Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public EndpointAuthorization Authorization { get; set; }

    public IDictionary<string, string> Data
    {
      get => (IDictionary<string, string>) this.data;
      set
      {
        if (value == null)
          return;
        this.data = new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
    }

    public ServiceEndpointDetails Clone() => new ServiceEndpointDetails(this);
  }
}
