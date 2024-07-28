// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.WindowsRegistrationDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 5)]
  [DataContract(Name = "WindowsRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class WindowsRegistrationDescription : RegistrationDescription
  {
    internal const string WnsHeaderPrefix = "X-WNS-";
    internal const string Type = "X-WNS-Type";
    internal const string Raw = "wns/raw";
    internal const string Badge = "wns/badge";
    internal const string Tile = "wns/tile";
    internal const string Toast = "wns/toast";
    internal const string ProdChannelUriPart = "notify.windows.com";
    internal const string MockChannelUriPart = "localhost:8450/WNS/Mock";
    internal const string MockRunnerChannelUriPart = "pushtestservice.cloudapp.net";
    internal const string MockIntChannelUriPart = "pushtestservice2.cloudapp.net";
    internal const string MockPerformanceChannelUriPart = "pushperfnotificationserver.cloudapp.net";
    internal const string MockEnduranceChannelUriPart = "pushstressnotificationserver.cloudapp.net";
    internal const string MockEnduranceChannelUriPart1 = "pushnotificationserver.cloudapp.net";
    internal static HashSet<string> SupportedWnsTypes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "wns/raw",
      "wns/badge",
      "wns/toast",
      "wns/tile"
    };

    internal override string AppPlatForm => "windows";

    internal override string RegistrationType => "windows";

    internal override string PlatformType => "windows";

    public WindowsRegistrationDescription(string channelUri)
      : this(string.Empty, new Uri(channelUri), (IEnumerable<string>) null)
    {
    }

    public WindowsRegistrationDescription(string channelUri, IEnumerable<string> tags)
      : this(string.Empty, new Uri(channelUri), tags)
    {
    }

    public WindowsRegistrationDescription(Uri channelUri)
      : this(string.Empty, channelUri, (IEnumerable<string>) null)
    {
    }

    public WindowsRegistrationDescription(Uri channelUri, IEnumerable<string> tags)
      : this(string.Empty, channelUri, tags)
    {
    }

    public WindowsRegistrationDescription(WindowsRegistrationDescription sourceRegistration)
      : base((RegistrationDescription) sourceRegistration)
    {
      this.ChannelUri = sourceRegistration.ChannelUri;
      this.SecondaryTileName = sourceRegistration.SecondaryTileName;
    }

    internal WindowsRegistrationDescription(
      string notificationHubPath,
      string channelUri,
      IEnumerable<string> tags)
      : base(notificationHubPath)
    {
      this.ChannelUri = !string.IsNullOrWhiteSpace(channelUri) ? new Uri(channelUri) : throw new ArgumentNullException(nameof (channelUri));
      if (tags == null)
        return;
      this.Tags = (ISet<string>) new HashSet<string>(tags);
    }

    internal WindowsRegistrationDescription(
      string notificationHubPath,
      Uri channelUri,
      IEnumerable<string> tags)
      : base(notificationHubPath)
    {
      this.ChannelUri = !(channelUri == (Uri) null) ? channelUri : throw new ArgumentNullException(nameof (channelUri));
      if (tags == null)
        return;
      this.Tags = (ISet<string>) new HashSet<string>(tags);
    }

    [AmqpMember(Mandatory = false, Order = 3)]
    [DataMember(Name = "ChannelUri", Order = 2001, IsRequired = true)]
    public Uri ChannelUri { get; set; }

    [AmqpMember(Mandatory = false, Order = 10)]
    [DataMember(Name = "SecondaryTileName", Order = 2002, IsRequired = false, EmitDefaultValue = false)]
    public string SecondaryTileName { get; set; }

    internal bool IsMockWns() => this.ChannelUri.Host.ToUpperInvariant().Contains("CLOUDAPP.NET");

    internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
    {
      if (string.IsNullOrWhiteSpace(this.ChannelUri.ToString()) || !this.ChannelUri.IsAbsoluteUri)
        throw new InvalidDataContractException(SRClient.ChannelUriNullOrEmpty);
    }

    internal override string GetPnsHandle() => this.ChannelUri.AbsoluteUri;

    internal override void SetPnsHandle(string pnsHandle)
    {
      if (!string.IsNullOrEmpty(pnsHandle))
        this.ChannelUri = new Uri(pnsHandle);
      else
        this.ChannelUri = (Uri) null;
    }

    internal override RegistrationDescription Clone() => (RegistrationDescription) new WindowsRegistrationDescription(this);
  }
}
