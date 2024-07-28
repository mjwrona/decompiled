// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.AppleRegistrationDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 2)]
  [DataContract(Name = "AppleRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class AppleRegistrationDescription : RegistrationDescription
  {
    internal static Regex DeviceTokenRegex = new Regex("^[a-fA-F0-9]+$");
    internal const string ExpiryHeader = "ServiceBusNotification-Apns-Expiry";
    internal const string PriorityHeader = "X-Apns-Priority";

    public AppleRegistrationDescription(AppleRegistrationDescription sourceRegistration)
      : base((RegistrationDescription) sourceRegistration)
    {
      this.DeviceToken = sourceRegistration.DeviceToken;
    }

    public AppleRegistrationDescription(string deviceToken)
      : this(string.Empty, deviceToken, (IEnumerable<string>) null)
    {
    }

    public AppleRegistrationDescription(string deviceToken, IEnumerable<string> tags)
      : this(string.Empty, deviceToken, tags)
    {
    }

    internal AppleRegistrationDescription(
      string notificationHubPath,
      string deviceToken,
      IEnumerable<string> tags)
      : base(notificationHubPath)
    {
      this.DeviceToken = !string.IsNullOrWhiteSpace(deviceToken) ? deviceToken : throw new ArgumentNullException(nameof (deviceToken));
      if (tags == null)
        return;
      this.Tags = (ISet<string>) new HashSet<string>(tags);
    }

    internal override string AppPlatForm => "apple";

    internal override string RegistrationType => "apple";

    internal override string PlatformType => "apple";

    [AmqpMember(Order = 3, Mandatory = false)]
    [DataMember(Name = "DeviceToken", Order = 2001, IsRequired = true)]
    public string DeviceToken { get; set; }

    internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
    {
      if (string.IsNullOrWhiteSpace(this.DeviceToken))
        throw new InvalidDataContractException(SRClient.DeviceTokenIsEmpty);
      if (!AppleRegistrationDescription.DeviceTokenRegex.IsMatch(this.DeviceToken) || this.DeviceToken.Length % 2 != 0)
        throw new InvalidDataContractException(SRClient.DeviceTokenHexaDecimalDigitError);
    }

    internal byte[] GetDeviceTokenBytes() => AppleRegistrationDescription.GetDeviceTokenBytes(this.DeviceToken);

    internal static byte[] GetDeviceTokenBytes(string deviceToken)
    {
      if (string.IsNullOrWhiteSpace(deviceToken))
        throw new InvalidDataContractException(SRClient.DeviceTokenIsEmpty);
      if (!AppleRegistrationDescription.DeviceTokenRegex.IsMatch(deviceToken) || deviceToken.Length % 2 != 0)
        throw new InvalidDataContractException(SRClient.DeviceTokenHexaDecimalDigitError);
      byte[] deviceTokenBytes = new byte[deviceToken.Length / 2];
      for (int index = 0; index < deviceTokenBytes.Length; ++index)
        deviceTokenBytes[index] = byte.Parse(deviceToken.Substring(index * 2, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
      return deviceTokenBytes;
    }

    internal override string GetPnsHandle() => this.DeviceToken.ToUpperInvariant();

    internal override void SetPnsHandle(string pnsHandle) => this.DeviceToken = pnsHandle;

    internal override RegistrationDescription Clone() => (RegistrationDescription) new AppleRegistrationDescription(this);
  }
}
