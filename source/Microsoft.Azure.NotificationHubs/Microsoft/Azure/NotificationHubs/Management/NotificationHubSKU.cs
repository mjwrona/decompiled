// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.NotificationHubSKU
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Management
{
  [DataContract(Name = "NotificationHubSKU", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class NotificationHubSKU
  {
    [Key]
    [DataMember(Name = "SKU", Order = 1001)]
    public NotificationHubSKUType SKU;

    [DataMember(Name = "MaxAllowedUnits", Order = 1002)]
    public int MaxAllowedUnits { get; set; }

    [DataMember(Name = "MinAllowedUnits", Order = 1003)]
    public int MinAllowedUnits { get; set; }

    [DataMember(Name = "MaxAllowedOperationsPerDayPerUnit", EmitDefaultValue = false, Order = 1004)]
    public long MaxAllowedOperationsPerDayPerUnit { get; set; }

    [DataMember(Name = "MaxAllowedRegistrationsPerUnit", Order = 1005)]
    public long MaxAllowedRegistrationsPerUnit { get; set; }

    [DataMember(Name = "MaxAllowedDevicesPerUnit", Order = 1006)]
    public long MaxAllowedDevicesPerUnit { get; set; }

    [DataMember(Name = "MaxAllowedPushesPerDayPerUnit", EmitDefaultValue = false, Order = 1007)]
    public long MaxAllowedPushesPerDayPerUnit { get; set; }

    [DataMember(Name = "MaxAllowedApiCallsPerDayPerUnit", EmitDefaultValue = false, Order = 1008)]
    public long MaxAllowedApiCallsPerDayPerUnit { get; set; }
  }
}
