// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.GcmRegistrationDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 1)]
  [DataContract(Name = "GcmRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class GcmRegistrationDescription : RegistrationDescription
  {
    public GcmRegistrationDescription(GcmRegistrationDescription sourceRegistration)
      : base((RegistrationDescription) sourceRegistration)
    {
      this.GcmRegistrationId = sourceRegistration.GcmRegistrationId;
    }

    public GcmRegistrationDescription(string gcmRegistrationId)
      : this(string.Empty, gcmRegistrationId, (IEnumerable<string>) null)
    {
    }

    public GcmRegistrationDescription(string gcmRegistrationId, IEnumerable<string> tags)
      : this(string.Empty, gcmRegistrationId, tags)
    {
    }

    internal GcmRegistrationDescription(
      string notificationHubPath,
      string gcmRegistrationId,
      IEnumerable<string> tags)
      : base(notificationHubPath)
    {
      this.GcmRegistrationId = !string.IsNullOrWhiteSpace(gcmRegistrationId) ? gcmRegistrationId : throw new ArgumentNullException(nameof (gcmRegistrationId));
      if (tags == null)
        return;
      this.Tags = (ISet<string>) new HashSet<string>(tags);
    }

    [AmqpMember(Order = 3, Mandatory = false)]
    [DataMember(Name = "GcmRegistrationId", Order = 2001, IsRequired = true)]
    public string GcmRegistrationId { get; set; }

    internal override string AppPlatForm => "gcm";

    internal override string RegistrationType => "gcm";

    internal override string PlatformType => "gcm";

    internal override string GetPnsHandle() => this.GcmRegistrationId;

    internal override void SetPnsHandle(string pnsHandle) => this.GcmRegistrationId = pnsHandle;

    internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
    {
      if (string.IsNullOrWhiteSpace(this.GcmRegistrationId))
        throw new InvalidDataContractException(SRClient.GCMRegistrationInvalidId);
    }

    internal override RegistrationDescription Clone() => (RegistrationDescription) new GcmRegistrationDescription(this);
  }
}
