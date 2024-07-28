// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.MailNotificationPerformerBase
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal abstract class MailNotificationPerformerBase : IMailNotificationPerformer
  {
    protected const string s_area = "gallery";

    public abstract IList<Guid> FetchAndValidateRecipients(
      IVssRequestContext requestContext,
      NotificationsData notificationsData);

    public virtual void PostNotificationsStep(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      IDictionary<string, object> notificationsData)
    {
    }

    public virtual void SendNotifications(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      IDictionary<string, object> notificationsData)
    {
      MailNotificationEventData notificationEventData = this.PrepareNotificationsData(requestContext, notificationsData);
      this.GetMailNotification().SendMailNotificationToRecipients(requestContext, recipients, notificationEventData);
      this.PublishCIEvent(requestContext, recipients, notificationsData);
    }

    public virtual IMailNotification GetMailNotification() => (IMailNotification) new MailNotification();

    protected abstract MailNotificationEventData PrepareNotificationsData(
      IVssRequestContext requestContext,
      IDictionary<string, object> notificationData);

    protected virtual MailNotificationEventData PrepareNotificationsData(
      IVssRequestContext requestContext,
      IDictionary<string, object> notificationData,
      IList<Guid> recipients)
    {
      return this.PrepareNotificationsData(requestContext, notificationData);
    }

    protected virtual void PublishCIEvent(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      IDictionary<string, object> notificationData)
    {
    }
  }
}
