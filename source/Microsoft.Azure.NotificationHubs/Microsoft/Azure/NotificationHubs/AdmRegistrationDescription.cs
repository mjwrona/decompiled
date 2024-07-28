// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.AdmRegistrationDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 11)]
  [DataContract(Name = "AdmRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class AdmRegistrationDescription : RegistrationDescription
  {
    public AdmRegistrationDescription(string admRegistrationId)
      : this(string.Empty, admRegistrationId, (IEnumerable<string>) null)
    {
    }

    public AdmRegistrationDescription(string admRegistrationId, IEnumerable<string> tags)
      : this(string.Empty, admRegistrationId, tags)
    {
    }

    public AdmRegistrationDescription(AdmRegistrationDescription sourceRegistration)
      : base((RegistrationDescription) sourceRegistration)
    {
      this.AdmRegistrationId = sourceRegistration.AdmRegistrationId;
    }

    internal AdmRegistrationDescription(
      string notificationHubPath,
      string admRegistrationId,
      IEnumerable<string> tags)
      : base(notificationHubPath)
    {
      this.AdmRegistrationId = admRegistrationId != null ? admRegistrationId : throw new ArgumentNullException(nameof (admRegistrationId));
      if (tags == null)
        return;
      this.Tags = (ISet<string>) new HashSet<string>(tags);
    }

    [AmqpMember(Mandatory = false, Order = 3)]
    [DataMember(Name = "AdmRegistrationId", Order = 2001, IsRequired = true)]
    public string AdmRegistrationId { get; set; }

    internal override string AppPlatForm => "adm";

    internal override string RegistrationType => "adm";

    internal override string PlatformType => "adm";

    internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
    {
      if (string.IsNullOrWhiteSpace(this.AdmRegistrationId))
        throw new InvalidDataContractException(SRClient.AdmRegistrationIdInvalid);
    }

    internal override string GetPnsHandle() => this.AdmRegistrationId;

    internal override void SetPnsHandle(string pnsHandle) => this.AdmRegistrationId = pnsHandle;

    internal override RegistrationDescription Clone() => (RegistrationDescription) new AdmRegistrationDescription(this);
  }
}
