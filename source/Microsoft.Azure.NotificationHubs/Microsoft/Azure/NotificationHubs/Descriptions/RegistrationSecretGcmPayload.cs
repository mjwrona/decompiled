// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Descriptions.RegistrationSecretGcmPayload
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Descriptions
{
  [DataContract(Name = "RegistrationSecretGcmPayload")]
  internal class RegistrationSecretGcmPayload
  {
    [AmqpMember(Order = 1, Mandatory = true)]
    [DataMember(Name = "data", Order = 1001, IsRequired = true)]
    public RegistrationSecretPayload Data { get; internal set; }
  }
}
