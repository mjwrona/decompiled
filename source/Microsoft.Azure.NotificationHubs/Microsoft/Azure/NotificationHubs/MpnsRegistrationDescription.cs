// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.MpnsRegistrationDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 7)]
  [DataContract(Name = "MpnsRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class MpnsRegistrationDescription : RegistrationDescription
  {
    internal const string MpnsHeaderPrefix = "X-";
    internal const string NotificationClass = "X-NotificationClass";
    internal const string Type = "X-WindowsPhone-Target";
    internal const string Tile = "token";
    internal const string Toast = "toast";
    internal const string TileClass = "1";
    internal const string ToastClass = "2";
    internal const string RawClass = "3";
    internal const string NamespaceName = "WPNotification";
    internal const string NotificationElementName = "Notification";
    internal const string ProdChannelUriPart = ".notify.live.net";
    internal const string MockChannelUriPart = "localhost:8450/MPNS/Mock";
    internal const string MockSSLChannelUriPart = "localhost:8451/MPNS/Mock";
    internal const string MockRunnerChannelUriPart = "pushtestservice.cloudapp.net";
    internal const string MockIntChannelUriPart = "pushtestservice2.cloudapp.net";
    internal const string MockPerformanceChannelUriPart = "pushperfnotificationserver.cloudapp.net";
    internal const string MockEnduranceChannelUriPart = "pushstressnotificationserver.cloudapp.net";
    internal const string MockEnduranceChannelUriPart1 = "pushnotificationserver.cloudapp.net";

    public MpnsRegistrationDescription(MpnsRegistrationDescription sourceRegistration)
      : base((RegistrationDescription) sourceRegistration)
    {
      this.ChannelUri = sourceRegistration.ChannelUri;
      this.SecondaryTileName = sourceRegistration.SecondaryTileName;
    }

    public MpnsRegistrationDescription(string channelUri)
      : this(string.Empty, new Uri(channelUri), (IEnumerable<string>) null)
    {
    }

    public MpnsRegistrationDescription(Uri channelUri)
      : this(string.Empty, channelUri, (IEnumerable<string>) null)
    {
    }

    public MpnsRegistrationDescription(string channelUri, IEnumerable<string> tags)
      : this(string.Empty, new Uri(channelUri), tags)
    {
    }

    public MpnsRegistrationDescription(Uri channelUri, IEnumerable<string> tags)
      : this(string.Empty, channelUri, tags)
    {
    }

    internal MpnsRegistrationDescription(
      string notificationHubPath,
      string channelUri,
      IEnumerable<string> tags)
      : this(notificationHubPath, new Uri(channelUri), tags)
    {
    }

    internal MpnsRegistrationDescription(
      string notificationHubPath,
      Uri channelUri,
      IEnumerable<string> tags)
      : base(notificationHubPath)
    {
      this.ChannelUri = channelUri;
      if (tags == null)
        return;
      this.Tags = (ISet<string>) new HashSet<string>(tags);
    }

    internal override string AppPlatForm => "windowsphone";

    internal override string RegistrationType => "windowsphone";

    internal override string PlatformType => "windowsphone";

    internal bool IsMockMpns() => this.ChannelUri.Host.ToUpperInvariant().Contains("CLOUDAPP.NET");

    [AmqpMember(Mandatory = false, Order = 3)]
    [DataMember(Name = "ChannelUri", Order = 2001, IsRequired = true)]
    public Uri ChannelUri { get; set; }

    [AmqpMember(Mandatory = false, Order = 10)]
    [DataMember(Name = "SecondaryTileName", Order = 2002, IsRequired = false, EmitDefaultValue = false)]
    public string SecondaryTileName { get; set; }

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

    internal override RegistrationDescription Clone() => (RegistrationDescription) new MpnsRegistrationDescription(this);
  }
}
