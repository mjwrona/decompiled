// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherCertificationMailNotificationEvent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [NotificationEventBindings(EventSerializerType.Json, "ms.GalleryNotifications.publisher-certification-requested-event")]
  [DataContract]
  public class PublisherCertificationMailNotificationEvent : MailNotificationEventData
  {
    [DataMember]
    public string PublisherName { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string ManagePageLink { get; set; }

    public PublisherCertificationMailNotificationEvent(IVssRequestContext requestContext) => this.EventType = "ms.GalleryNotifications.publisher-certification-requested-event";
  }
}
