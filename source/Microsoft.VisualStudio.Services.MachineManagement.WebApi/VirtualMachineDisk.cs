// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.VirtualMachineDisk
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public class VirtualMachineDisk
  {
    private PropertiesCollection m_properties;

    internal VirtualMachineDisk() => this.CreatedOn = DateTime.MinValue;

    public VirtualMachineDisk(string name)
    {
      this.CreatedOn = DateTime.MinValue;
      this.Name = name;
    }

    [DataMember]
    public long Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string ImageName { get; set; }

    [DataMember]
    public string ImageVersion { get; set; }

    [DataMember]
    public long ParentId { get; set; }

    [DataMember]
    public string Location { get; set; }

    [DataMember]
    public int StorageAccountId { get; set; }

    [DataMember]
    public string ResourceGroupIdentifier { get; set; }

    [DataMember]
    public string DiskType { get; set; }

    [DataMember]
    public string State { get; set; }

    [DataMember]
    public DateTime CreatedOn { get; set; }

    [DataMember]
    public DateTime DeletedOn { get; set; }

    [DataMember]
    public DateTime DeleteVerifiedOn { get; set; }

    [DataMember]
    public DateTime RetainUntil { get; set; }

    [DataMember]
    public int FailedDeletes { get; set; }

    [DataMember]
    public DateTime StateChangedOn { get; set; }

    [Obsolete("This is no longer supported, but the property remains to avoid breaking API compat.")]
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
