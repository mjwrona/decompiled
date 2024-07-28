// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CustomerSupportRequestAcknowledgementMailNotification
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Notifications;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [NotificationEventBindings(EventSerializerType.Json, "ms.GalleryNotifications.customer-support-request-submit-event")]
  [DataContract]
  public class CustomerSupportRequestAcknowledgementMailNotification : MailNotificationEventData
  {
    public CustomerSupportRequestAcknowledgementMailNotification() => this.EventType = "ms.GalleryNotifications.customer-support-request-submit-event";
  }
}
