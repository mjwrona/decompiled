// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.MessagingSKU
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Management
{
  [DataContract(Name = "NamespaceSKU", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class MessagingSKU
  {
    [DataMember(Name = "SKU", Order = 1001, IsRequired = true, EmitDefaultValue = true)]
    public int SKU { get; set; }

    [DataMember(Name = "SKUDescription", Order = 1002, IsRequired = false)]
    public string SKUDescription { get; internal set; }

    [DataMember(Name = "MinAllowedEventHubUnit", Order = 1003, IsRequired = false)]
    public int MinAllowedEventHubUnit { get; internal set; }

    [DataMember(Name = "MaxAllowedEventHubUnit", Order = 1004, IsRequired = false)]
    public int MaxAllowedEventHubUnit { get; internal set; }
  }
}
