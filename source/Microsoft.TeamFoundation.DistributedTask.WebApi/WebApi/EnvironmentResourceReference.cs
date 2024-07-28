// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.EnvironmentResourceReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class EnvironmentResourceReference
  {
    private IList<EnvironmentLinkedResourceReference> m_linkedResources;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Tags")]
    private IList<string> m_tags;

    [DataMember]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public EnvironmentResourceType Type { get; set; }

    public IList<string> Tags
    {
      get
      {
        if (this.m_tags == null)
          this.m_tags = (IList<string>) new List<string>();
        return this.m_tags;
      }
      set => this.m_tags = value;
    }

    public IList<EnvironmentLinkedResourceReference> LinkedResources
    {
      get
      {
        if (this.m_linkedResources == null)
          this.m_linkedResources = (IList<EnvironmentLinkedResourceReference>) new List<EnvironmentLinkedResourceReference>();
        return this.m_linkedResources;
      }
    }
  }
}
