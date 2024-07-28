// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryNotificationsService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class GalleryNotificationsService : IGalleryNotificationsService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void SendNotifications(
      IVssRequestContext requestContext,
      NotificationsData notificationsData)
    {
      IMailNotificationPerformer notificationPerformer = this.GetMailNotificationPerformer(notificationsData.Type);
      if (notificationPerformer == null)
        return;
      IList<Guid> recipients = notificationPerformer.FetchAndValidateRecipients(requestContext, notificationsData);
      if (recipients != null && recipients.Count > 0)
        notificationPerformer.SendNotifications(requestContext, recipients, notificationsData.Data);
      notificationPerformer.PostNotificationsStep(requestContext, recipients, notificationsData.Data);
    }

    public virtual IMailNotification GetMailNotification() => (IMailNotification) new MailNotification();

    public virtual IMailNotificationPerformer GetMailNotificationPerformer(
      NotificationTemplateType notificationType)
    {
      IMailNotificationPerformer notificationPerformer = (IMailNotificationPerformer) null;
      switch (notificationType)
      {
        case NotificationTemplateType.CustomerContactNotification:
          notificationPerformer = (IMailNotificationPerformer) new CustomerContactNotificationPerformer();
          break;
        case NotificationTemplateType.PublisherMemberUpdateNotification:
          notificationPerformer = (IMailNotificationPerformer) new PublisherMemberNotificationPerformer();
          break;
      }
      return notificationPerformer;
    }
  }
}
