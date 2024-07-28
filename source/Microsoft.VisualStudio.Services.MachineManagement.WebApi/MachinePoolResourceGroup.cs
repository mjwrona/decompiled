// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolResourceGroup
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public class MachinePoolResourceGroup : ICloneable
  {
    private PropertiesCollection m_properties;

    internal MachinePoolResourceGroup()
    {
    }

    private MachinePoolResourceGroup(MachinePoolResourceGroup cloneFrom)
    {
      this.Data = cloneFrom.Data;
      this.Identifier = cloneFrom.Identifier;
      this.InternalId = cloneFrom.InternalId;
      this.PerformanceTier = cloneFrom.PerformanceTier;
      this.Region = cloneFrom.Region;
      this.ResourceProviderType = cloneFrom.ResourceProviderType;
      if (cloneFrom.m_properties == null)
        return;
      this.m_properties = new PropertiesCollection((IDictionary<string, object>) cloneFrom.m_properties);
    }

    [IgnoreDataMember]
    internal int InternalId { get; set; }

    [DataMember]
    public string Identifier { get; set; }

    [DataMember]
    public string ResourceProviderType { get; set; }

    [DataMember]
    public string Region { get; set; }

    [DataMember]
    public string PerformanceTier { get; set; }

    [DataMember]
    public string Data { get; set; }

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

    public MachinePoolResourceGroup Clone() => new MachinePoolResourceGroup(this);

    object ICloneable.Clone() => (object) this.Clone();
  }
}
