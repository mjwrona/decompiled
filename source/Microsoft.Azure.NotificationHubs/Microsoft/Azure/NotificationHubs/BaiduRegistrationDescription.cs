// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.BaiduRegistrationDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [AmqpContract(Code = 15)]
  [DataContract(Name = "BaiduRegistrationDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class BaiduRegistrationDescription : RegistrationDescription
  {
    internal const string MessageTypeHeader = "X-Baidu-Message-Type";

    public BaiduRegistrationDescription(string pnsHandle)
      : base(string.Empty)
    {
      int length = pnsHandle.IndexOf('-');
      this.BaiduUserId = pnsHandle.Substring(0, length);
      this.BaiduChannelId = pnsHandle.Substring(length + 1, pnsHandle.Length - length - 1);
      if (string.IsNullOrWhiteSpace(this.BaiduUserId))
        throw new ArgumentNullException("baiduRegistrationId");
    }

    public BaiduRegistrationDescription(BaiduRegistrationDescription sourceRegistration)
      : base((RegistrationDescription) sourceRegistration)
    {
      this.BaiduUserId = sourceRegistration.BaiduUserId;
      this.BaiduChannelId = sourceRegistration.BaiduChannelId;
    }

    public BaiduRegistrationDescription(
      string baiduUserId,
      string baiduChannelId,
      IEnumerable<string> tags)
      : this(string.Empty, baiduUserId, baiduChannelId, tags)
    {
    }

    internal BaiduRegistrationDescription(
      string notificationHubPath,
      string baiduUserId,
      string baiduChannelId,
      IEnumerable<string> tags)
      : base(notificationHubPath)
    {
      this.BaiduUserId = !string.IsNullOrWhiteSpace(baiduUserId) ? baiduUserId : throw new ArgumentNullException("baiduRegistrationId");
      this.BaiduChannelId = baiduChannelId;
      if (tags == null)
        return;
      this.Tags = (ISet<string>) new HashSet<string>(tags);
    }

    [AmqpMember(Order = 3, Mandatory = true)]
    [DataMember(Name = "BaiduUserId", Order = 2001, IsRequired = true)]
    public string BaiduUserId { get; set; }

    [AmqpMember(Order = 4, Mandatory = false)]
    [DataMember(Name = "BaiduChannelId", Order = 2002, IsRequired = true)]
    public string BaiduChannelId { get; set; }

    internal override string AppPlatForm => "baidu";

    internal override string RegistrationType => "baidu";

    internal override string PlatformType => "baidu";

    internal override string GetPnsHandle() => this.BaiduUserId + "-" + this.BaiduChannelId;

    internal override void SetPnsHandle(string pnsHandle)
    {
      if (string.IsNullOrEmpty(pnsHandle))
        return;
      int length = pnsHandle.IndexOf('-');
      this.BaiduUserId = pnsHandle.Substring(0, length);
      this.BaiduChannelId = pnsHandle.Substring(length + 1, pnsHandle.Length - length - 1);
    }

    internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
    {
      if (string.IsNullOrWhiteSpace(this.BaiduChannelId))
        throw new InvalidDataContractException(SRClient.BaiduRegistrationInvalidId);
    }

    internal override RegistrationDescription Clone() => (RegistrationDescription) new BaiduRegistrationDescription(this);
  }
}
