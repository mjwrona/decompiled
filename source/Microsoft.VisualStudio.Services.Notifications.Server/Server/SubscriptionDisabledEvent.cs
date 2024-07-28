// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionDisabledEvent
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [DataContract]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-notifications.subscription-disabled-event")]
  public class SubscriptionDisabledEvent
  {
    public const string EventType = "ms.vss-notifications.subscription-disabled-event";

    [DataMember]
    public string SubscriptionId { get; set; }

    [DataMember]
    public string SubscriptionUri { get; set; }

    [DataMember]
    public string SubscriptionTitle { get; set; }

    [DataMember]
    public string Message { get; set; }

    public static class Roles
    {
      public static string AuthorizedAs = "authorizedAs";
      public static string Subscriber = "subscriber";
      public static string Admins = "admins";
      public static string Recipient = "recipient";
    }
  }
}
