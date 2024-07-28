// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.EmailRegistrationDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 4)]
  [DataContract(Name = "EmailRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  internal class EmailRegistrationDescription : RegistrationDescription
  {
    internal static Regex EmailAddressRegex = new Regex("^[\\w!#$%&'*+/=?`{|}~^-]+(?:\\.[\\w!#$%&'*+/=?`{|}~^-]+)*@(?:[A-Za-z0-9-]+\\.)+[A-Za-z]{2,6}$");

    public EmailRegistrationDescription(EmailRegistrationDescription description)
      : this(description.EmailAddress)
    {
    }

    public EmailRegistrationDescription(string emailAddress)
      : this(string.Empty, emailAddress)
    {
      this.EmailAddress = emailAddress;
    }

    internal EmailRegistrationDescription(string notificationHubPath, string emailAddress)
      : base(notificationHubPath)
    {
      this.EmailAddress = emailAddress;
    }

    internal override string AppPlatForm => "smtp";

    internal override string RegistrationType => "smtp";

    internal override string GetPnsHandle() => this.EmailAddress;

    internal override string PlatformType => "smtp";

    internal override void SetPnsHandle(string pnsHandle) => this.EmailAddress = pnsHandle;

    [AmqpMember(Mandatory = false, Order = 3)]
    [DataMember(Name = "EmailAddress", IsRequired = true)]
    public string EmailAddress { get; private set; }

    internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
    {
      if (string.IsNullOrWhiteSpace(this.EmailAddress) || !EmailRegistrationDescription.EmailAddressRegex.IsMatch(this.EmailAddress))
        throw new InvalidDataContractException("Email Address is invalid");
    }

    internal override RegistrationDescription Clone() => (RegistrationDescription) new EmailRegistrationDescription(this);
  }
}
