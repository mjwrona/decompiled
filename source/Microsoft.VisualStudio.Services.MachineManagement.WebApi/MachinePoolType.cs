// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolType
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachinePoolType
  {
    private PropertiesCollection m_properties;

    internal MachinePoolType()
    {
    }

    public MachinePoolType(string name, string description)
    {
      this.Name = name;
      this.Description = description;
    }

    [IgnoreDataMember]
    internal int InternalId { get; set; }

    [DataMember(IsRequired = true)]
    public string Name { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; internal set; }

    [IgnoreDataMember]
    internal string ScopePath { get; set; }

    [IgnoreDataMember]
    public MachinePoolSecurity SecurityConfiguration { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PropertiesCollection Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new PropertiesCollection();
        return this.m_properties;
      }
      internal set => this.m_properties = value;
    }
  }
}
