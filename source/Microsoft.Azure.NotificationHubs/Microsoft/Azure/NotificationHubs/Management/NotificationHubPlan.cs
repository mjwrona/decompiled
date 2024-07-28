// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.NotificationHubPlan
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Management
{
  [DataContract(Name = "NotificationHubPlan", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class NotificationHubPlan
  {
    private const string RevisionName = "Revision";
    [DataMember(Name = "SKU", Order = 1001, IsRequired = true)]
    public NotificationHubSKUType SKU;

    [DataMember(Name = "SelectedSKUMaxUnits", Order = 1002, IsRequired = false, EmitDefaultValue = false)]
    public int SelectedSKUMaxUnits { get; set; }

    [DataMember(Name = "SelectedSKUMinUnits", Order = 1003, IsRequired = false, EmitDefaultValue = false)]
    public int SelectedSKUMinUnits { get; set; }

    [DataMember(Name = "UpdatedAt", IsRequired = false, Order = 1004, EmitDefaultValue = false)]
    public DateTime UpdatedAt { get; internal set; }

    [DataMember(Name = "Revision", IsRequired = false, Order = 1005, EmitDefaultValue = false)]
    public long Revision { get; set; }
  }
}
