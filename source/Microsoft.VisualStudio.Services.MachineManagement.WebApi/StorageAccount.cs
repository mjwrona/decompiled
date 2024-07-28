// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.StorageAccount
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
  public class StorageAccount : ICloneable
  {
    private PropertiesCollection m_properties;

    public StorageAccount(string name) => this.Name = name;

    internal StorageAccount()
    {
    }

    private StorageAccount(StorageAccount cloneFrom)
    {
      this.Enabled = cloneFrom.Enabled;
      this.Id = cloneFrom.Id;
      this.Location = cloneFrom.Location;
      this.Name = cloneFrom.Name;
      this.ResourceGroupIdentifier = cloneFrom.ResourceGroupIdentifier;
      this.State = cloneFrom.State;
      this.Type = cloneFrom.Type;
      if (cloneFrom.m_properties == null)
        return;
      this.m_properties = new PropertiesCollection((IDictionary<string, object>) cloneFrom.m_properties);
    }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Location { get; set; }

    [DataMember]
    public string Type { get; set; }

    [DataMember]
    public bool Enabled { get; set; }

    [DataMember]
    public string State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ResourceGroupIdentifier { get; set; }

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

    public StorageAccount Clone() => new StorageAccount(this);

    object ICloneable.Clone() => (object) this.Clone();
  }
}
