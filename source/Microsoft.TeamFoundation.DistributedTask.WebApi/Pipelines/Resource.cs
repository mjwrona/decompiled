// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Resource
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class Resource
  {
    [DataMember(Name = "Properties", EmitDefaultValue = false)]
    private ResourceProperties m_properties;

    protected Resource()
    {
    }

    protected Resource(Resource resourceToCopy)
    {
      this.Alias = resourceToCopy.Alias;
      this.Endpoint = resourceToCopy.Endpoint?.Clone();
      this.m_properties = resourceToCopy.m_properties?.Clone();
    }

    [DataMember(EmitDefaultValue = false)]
    public string Alias { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ServiceEndpointReference Endpoint { get; set; }

    public ResourceProperties Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new ResourceProperties();
        return this.m_properties;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      ResourceProperties properties = this.m_properties;
      if ((properties != null ? (properties.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_properties = (ResourceProperties) null;
    }
  }
}
