// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.Property
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Management
{
  [DataContract(Name = "Property", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class Property : ICloneable
  {
    private const string PropertyName = "Property";
    private const string NameName = "Name";
    private const string ValueName = "Value";
    private const string ModifiedName = "Modified";
    private const string RevisionName = "Revision";
    private const string CreatedName = "Created";
    public static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof (Property));

    [DataMember(Name = "Name", IsRequired = true, Order = 100)]
    public string Name { get; set; }

    [DataMember(Name = "Value", IsRequired = true, Order = 101)]
    public string Value { get; set; }

    [DataMember(Name = "Created", IsRequired = false, Order = 102, EmitDefaultValue = false)]
    public DateTime Created { get; internal set; }

    [DataMember(Name = "Modified", IsRequired = false, Order = 103, EmitDefaultValue = false)]
    public DateTime Modified { get; internal set; }

    [DataMember(Name = "Revision", IsRequired = false, Order = 104, EmitDefaultValue = false)]
    public long Revision { get; set; }

    public object Clone() => this.MemberwiseClone();
  }
}
