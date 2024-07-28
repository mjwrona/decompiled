// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineRequestType
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
  public class MachineRequestType : ICloneable
  {
    private PropertiesCollection m_properties;

    internal MachineRequestType()
    {
    }

    public MachineRequestType(string name, string description)
    {
      this.Name = name;
      this.Description = description;
      this.AllowMultipleRequestsOfTypePerHost = true;
    }

    private MachineRequestType(MachineRequestType cloneFrom)
    {
      this.InternalId = cloneFrom.InternalId;
      this.Description = cloneFrom.Description;
      this.AllowMultipleRequestsOfTypePerHost = cloneFrom.AllowMultipleRequestsOfTypePerHost;
      this.Name = cloneFrom.Name;
      if (cloneFrom.m_properties == null)
        return;
      this.m_properties = new PropertiesCollection((IDictionary<string, object>) cloneFrom.m_properties);
    }

    [IgnoreDataMember]
    internal int InternalId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public bool AllowMultipleRequestsOfTypePerHost { get; set; }

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

    public MachineRequestType Clone() => new MachineRequestType(this);

    object ICloneable.Clone() => (object) this.Clone();
  }
}
