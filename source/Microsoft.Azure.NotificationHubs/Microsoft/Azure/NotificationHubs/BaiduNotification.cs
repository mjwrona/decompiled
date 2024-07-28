// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.BaiduNotification
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.NotificationHubs
{
  public sealed class BaiduNotification : Notification, INativeNotification
  {
    public BaiduNotification(string message)
      : this(message, new int?())
    {
    }

    public BaiduNotification(string message, int? messageType)
      : base((IDictionary<string, string>) null, (string) null)
    {
      this.Body = !string.IsNullOrWhiteSpace(message) ? message : throw new ArgumentNullException("baidu notification message.description");
      this.MessageType = messageType;
      this.ContentType = "application/x-www-form-urlencoded";
    }

    public int? MessageType { get; set; }

    protected override string PlatformType => "baidu";

    protected override void OnValidateAndPopulateHeaders()
    {
      if (!this.MessageType.HasValue)
        return;
      this.AddOrUpdateHeader("X-Baidu-Message-Type", this.MessageType.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
