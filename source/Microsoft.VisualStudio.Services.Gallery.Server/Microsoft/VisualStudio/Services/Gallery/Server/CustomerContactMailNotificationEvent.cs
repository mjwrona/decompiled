// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CustomerContactMailNotificationEvent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Notifications;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [NotificationEventBindings(EventSerializerType.Json, "ms.GalleryNotifications.customer-contact-event")]
  [DataContract]
  public class CustomerContactMailNotificationEvent : CustomerContactMailNotificationData
  {
    public CustomerContactMailNotificationEvent() => this.EventType = "ms.GalleryNotifications.customer-contact-event";

    [DataMember]
    public string ReasonCodeText { get; set; }

    [DataMember]
    public string ReasonCode { get; set; }

    [DataMember]
    public string OtherReasonText { get; set; }

    [DataMember]
    public string OtherReason { get; set; }
  }
}
