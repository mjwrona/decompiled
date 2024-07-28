// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.GcmNotification
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs
{
  public sealed class GcmNotification : Notification, INativeNotification
  {
    public GcmNotification(string jsonPayload)
      : base((IDictionary<string, string>) null, (string) null)
    {
      this.Body = !string.IsNullOrWhiteSpace(jsonPayload) ? jsonPayload : throw new ArgumentNullException(nameof (jsonPayload));
    }

    [Obsolete("This method is obsolete.")]
    public GcmNotification(string jsonPayload, string tag)
      : base((IDictionary<string, string>) null, tag)
    {
      this.Body = !string.IsNullOrWhiteSpace(jsonPayload) ? jsonPayload : throw new ArgumentNullException(nameof (jsonPayload));
    }

    protected override string PlatformType => "gcm";

    protected override void OnValidateAndPopulateHeaders()
    {
    }
  }
}
