// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.NamespaceEntityStats
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Management
{
  [DataContract(Name = "NamespaceEntityStats", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class NamespaceEntityStats
  {
    public static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof (NamespaceEntityStats));

    [DataMember(Name = "TopicCount", IsRequired = false, Order = 100, EmitDefaultValue = true)]
    public long TopicCount { get; set; }

    [DataMember(Name = "QueueCount", IsRequired = false, Order = 101, EmitDefaultValue = true)]
    public long QueueCount { get; set; }

    [DataMember(Name = "RelayCount", IsRequired = false, Order = 102, EmitDefaultValue = true)]
    public long RelayCount { get; set; }

    [DataMember(Name = "NotificationHubCount", IsRequired = false, Order = 103, EmitDefaultValue = true)]
    public long NotificationHubCount { get; set; }

    [DataMember(Name = "EventHubCount", IsRequired = false, Order = 104, EmitDefaultValue = true)]
    public long EventHubCount { get; set; }
  }
}
