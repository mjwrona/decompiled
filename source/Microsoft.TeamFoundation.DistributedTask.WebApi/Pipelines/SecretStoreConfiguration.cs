// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SecretStoreConfiguration
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SecretStoreConfiguration
  {
    [DataMember(Name = "EndpointId", EmitDefaultValue = false)]
    private Guid m_endpointId;
    [DataMember(Name = "Keys", EmitDefaultValue = false)]
    private List<string> m_keys;

    public SecretStoreConfiguration()
    {
    }

    private SecretStoreConfiguration(SecretStoreConfiguration configurationToCopy)
    {
      this.Endpoint = configurationToCopy.Endpoint?.Clone();
      this.StoreName = configurationToCopy.StoreName;
      this.m_endpointId = configurationToCopy.m_endpointId;
      List<string> keys = configurationToCopy.m_keys;
      // ISSUE: explicit non-virtual call
      if ((keys != null ? (__nonvirtual (keys.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_keys = new List<string>((IEnumerable<string>) configurationToCopy.m_keys);
    }

    [DataMember(EmitDefaultValue = false)]
    public ServiceEndpointReference Endpoint { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StoreName { get; set; }

    public IList<string> Keys
    {
      get
      {
        if (this.m_keys == null)
          this.m_keys = new List<string>();
        return (IList<string>) this.m_keys;
      }
    }

    public SecretStoreConfiguration Clone() => new SecretStoreConfiguration(this);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (this.Endpoint != null || !(this.m_endpointId != Guid.Empty))
        return;
      this.Endpoint = new ServiceEndpointReference()
      {
        Id = this.m_endpointId
      };
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<string> keys = this.m_keys;
      // ISSUE: explicit non-virtual call
      if ((keys != null ? (__nonvirtual (keys.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_keys = (List<string>) null;
      if (this.Endpoint == null || !(this.Endpoint.Id != Guid.Empty))
        return;
      this.m_endpointId = this.Endpoint.Id;
    }
  }
}
