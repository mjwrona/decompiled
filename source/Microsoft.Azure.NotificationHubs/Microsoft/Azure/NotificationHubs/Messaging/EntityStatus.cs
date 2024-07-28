// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.EntityStatus
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [DataContract(Name = "EntityStatus", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public enum EntityStatus
  {
    [EnumMember] Active,
    [EnumMember] Disabled,
    [EnumMember] Restoring,
    [EnumMember] SendDisabled,
    [EnumMember] ReceiveDisabled,
    [EnumMember] Creating,
    [EnumMember] Deleting,
  }
}
