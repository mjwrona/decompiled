// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Management.NamespaceDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Management
{
  [DataContract(Name = "NamespaceDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class NamespaceDescription
  {
    public static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof (NamespaceDescription));

    [DataMember(Name = "Name", IsRequired = false, Order = 100, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "Region", IsRequired = false, Order = 101, EmitDefaultValue = false)]
    public string Region { get; set; }

    [DataMember(Name = "DefaultKey", IsRequired = false, Order = 102, EmitDefaultValue = false)]
    public string DefaultKey { get; set; }

    [DataMember(Name = "Status", IsRequired = false, Order = 103, EmitDefaultValue = false)]
    public NamespaceState Status { get; set; }

    [DataMember(Name = "CreatedAt", IsRequired = false, Order = 104, EmitDefaultValue = false)]
    public DateTime CreatedAt { get; set; }

    [DataMember(Name = "AcsManagementEndpoint", IsRequired = false, Order = 105, EmitDefaultValue = false)]
    public Uri AcsManagementEndpoint { get; set; }

    [DataMember(Name = "ServiceBusEndpoint", IsRequired = false, Order = 106, EmitDefaultValue = false)]
    public Uri ServiceBusEndpoint { get; set; }

    [DataMember(Name = "ConnectionString", IsRequired = false, Order = 107, EmitDefaultValue = false)]
    public string ConnectionString { get; set; }

    [DataMember(Name = "SubscriptionId", IsRequired = false, Order = 108, EmitDefaultValue = false)]
    public string SubscriptionId { get; set; }

    [DataMember(Name = "Enabled", IsRequired = false, Order = 109, EmitDefaultValue = true)]
    public bool Enabled { get; set; }

    [DataMember(Name = "Critical", IsRequired = false, Order = 110, EmitDefaultValue = false)]
    public bool? Critical { get; set; }

    [DataMember(Name = "ScaleUnit", IsRequired = false, Order = 201, EmitDefaultValue = false)]
    internal string ScaleUnit { get; set; }

    [DataMember(Name = "DataCenter", IsRequired = false, Order = 202, EmitDefaultValue = false)]
    internal string DataCenter { get; set; }

    [DataMember(Name = "UpdatedAt", IsRequired = false, Order = 203, EmitDefaultValue = false)]
    internal DateTime? UpdatedAt { get; set; }

    [DataMember(Name = "CreateACSNamespace", IsRequired = false, Order = 204, EmitDefaultValue = false)]
    public bool CreateACSNamespace { get; set; }

    [DataMember(Name = "EventHubEnabled", IsRequired = false, Order = 205, EmitDefaultValue = false)]
    public bool EventHubEnabled { get; set; }

    [DataMember(Name = "NamespaceType", IsRequired = false, Order = 206, EmitDefaultValue = false)]
    public Microsoft.Azure.NotificationHubs.Management.NamespaceType? NamespaceType { get; set; }

    internal string ProjectKey { get; set; }

    internal string ScopeKey { get; set; }

    internal NamespaceState State { get; set; }

    internal string CurrentState { get; set; }

    internal string TargetState { get; set; }

    internal string ScaleUnitKey { get; set; }

    internal bool InDeletedSubscription { get; set; }
  }
}
