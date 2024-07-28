// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NokiaXRegistrationDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 13)]
  [DataContract(Name = "NokiaXRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  internal class NokiaXRegistrationDescription : RegistrationDescription
  {
    public NokiaXRegistrationDescription(NokiaXRegistrationDescription sourceRegistration)
      : base((RegistrationDescription) sourceRegistration)
    {
      this.NokiaXRegistrationId = sourceRegistration.NokiaXRegistrationId;
    }

    public NokiaXRegistrationDescription(string nokiaXRegistrationId, IEnumerable<string> tags)
      : this(string.Empty, nokiaXRegistrationId, tags)
    {
    }

    public NokiaXRegistrationDescription(string nokiaXRegistrationId)
      : this(string.Empty, nokiaXRegistrationId, (IEnumerable<string>) null)
    {
    }

    internal NokiaXRegistrationDescription(
      string notificationHubPath,
      string nokiaXRegistrationId,
      IEnumerable<string> tags)
      : base(notificationHubPath)
    {
      this.NokiaXRegistrationId = !string.IsNullOrWhiteSpace(nokiaXRegistrationId) ? nokiaXRegistrationId : throw new ArgumentNullException(nameof (nokiaXRegistrationId));
      if (tags == null)
        return;
      this.Tags = (ISet<string>) new HashSet<string>(tags);
    }

    [AmqpMember(Order = 3, Mandatory = false)]
    [DataMember(Name = "NokiaXRegistrationId", Order = 2001, IsRequired = true)]
    public string NokiaXRegistrationId { get; set; }

    internal override string AppPlatForm => "nokiax";

    internal override string RegistrationType => "nokiax";

    internal override string PlatformType => "nokiax";

    internal override string GetPnsHandle() => this.NokiaXRegistrationId;

    internal override void SetPnsHandle(string pnsHandle) => this.NokiaXRegistrationId = pnsHandle;

    internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
    {
      if (string.IsNullOrWhiteSpace(this.NokiaXRegistrationId))
        throw new InvalidDataContractException(SRClient.NokiaXRegistrationInvalidId);
    }

    internal override RegistrationDescription Clone() => (RegistrationDescription) new NokiaXRegistrationDescription(this);
  }
}
