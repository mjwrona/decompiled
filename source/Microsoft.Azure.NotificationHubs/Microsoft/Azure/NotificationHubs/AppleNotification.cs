// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.AppleNotification
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.NotificationHubs
{
  public sealed class AppleNotification : Notification, INativeNotification
  {
    public AppleNotification(string jsonPayload)
      : this(jsonPayload, new DateTime?())
    {
    }

    [Obsolete("This method is obsolete.")]
    public AppleNotification(string jsonPayload, string tag)
      : this(jsonPayload, new DateTime?(), tag)
    {
    }

    public AppleNotification(string jsonPayload, DateTime? expiry)
      : base((IDictionary<string, string>) null, (string) null)
    {
      if (string.IsNullOrWhiteSpace(jsonPayload))
        throw new ArgumentNullException(nameof (jsonPayload));
      this.Expiry = expiry;
      this.Body = jsonPayload;
    }

    [Obsolete("This method is obsolete.")]
    public AppleNotification(string jsonPayload, DateTime? expiry, string tag)
      : base((IDictionary<string, string>) null, tag)
    {
      if (string.IsNullOrWhiteSpace(jsonPayload))
        throw new ArgumentNullException(nameof (jsonPayload));
      this.Expiry = expiry;
      this.Body = jsonPayload;
    }

    public DateTime? Expiry { get; set; }

    public int? Priority { get; set; }

    protected override string PlatformType => "apple";

    protected override void OnValidateAndPopulateHeaders()
    {
      if (this.Expiry.HasValue)
        this.AddOrUpdateHeader("ServiceBusNotification-Apns-Expiry", this.Expiry.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!this.Priority.HasValue)
        return;
      this.AddOrUpdateHeader("X-Apns-Priority", this.Priority.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
