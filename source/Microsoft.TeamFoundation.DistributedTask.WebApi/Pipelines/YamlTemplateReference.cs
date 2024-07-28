// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.YamlTemplateReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  public sealed class YamlTemplateReference
  {
    [DataMember(Name = "ExtendedBy", EmitDefaultValue = false)]
    private List<int> m_extendedBy;
    [DataMember(Name = "IncludedBy", EmitDefaultValue = false)]
    private List<int> m_includedBy;

    public YamlTemplateReference()
    {
    }

    public YamlTemplateReference(int id, YamlTemplateLocation location)
    {
      this.Id = id;
      this.Location = location;
    }

    [DataMember(Name = "Id")]
    public int Id { get; set; }

    [DataMember(Name = "Location")]
    public YamlTemplateLocation Location { get; set; }

    public List<int> ExtendedBy
    {
      get
      {
        if (this.m_extendedBy == null)
          this.m_extendedBy = new List<int>();
        return this.m_extendedBy;
      }
    }

    public List<int> IncludedBy
    {
      get
      {
        if (this.m_includedBy == null)
          this.m_includedBy = new List<int>();
        return this.m_includedBy;
      }
    }
  }
}
