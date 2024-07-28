// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Descriptions.RegistrationSecretPayload
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Descriptions
{
  [DataContract(Name = "RegistrationSecretPayload")]
  internal class RegistrationSecretPayload
  {
    [AmqpMember(Order = 1, Mandatory = true)]
    [DataMember(Name = "WANHRegistrationSecret", Order = 1001, IsRequired = true)]
    public string WANHRegistrationSecret { get; internal set; }

    [AmqpMember(Order = 2, Mandatory = true)]
    [DataMember(Name = "WANHExpirationTime", Order = 1002, IsRequired = true)]
    public string WANHExpirationTime { get; internal set; }

    [AmqpMember(Order = 3, Mandatory = true)]
    [DataMember(Name = "WANHDeviceChallenge", Order = 1003, IsRequired = true)]
    public string WANHDeviceChallenge { get; set; }
  }
}
