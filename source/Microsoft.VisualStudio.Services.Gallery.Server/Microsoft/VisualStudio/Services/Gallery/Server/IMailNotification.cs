// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IMailNotification
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public interface IMailNotification
  {
    void SendMailNotificationToPublisher(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      MailNotificationEventData notificationEventData);

    void SendExtensionPublishMailNotification(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      VersionValidationMailNotificationEvent notificationEventData,
      Guid id = default (Guid));

    void SendMailNotificationToUser(
      IVssRequestContext requestContext,
      Guid userId,
      MailNotificationEventData notificationEventData);

    void SendMailNotificationToUser(
      IVssRequestContext requestContext,
      Guid userId,
      MailNotificationEventData notificationEventData,
      bool shouldEncodeHtmlNotificationContent);

    void SendMailNotificationToRecipients(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      MailNotificationEventData notificationEventData);

    void SendMailNotificationToRecipients(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      MailNotificationEventData notificationEventData,
      bool shouldEncodeHtmlNotificationContent);

    void SendMailNotificationUsingTeamFoundationMailService(
      IVssRequestContext requestContext,
      IList<Guid> ccIdentities,
      MailNotificationEventData notificationEventData,
      string defaultSubject);

    string GetUserMailAddress(IVssRequestContext requestContext, Guid userId);

    void SendMailNotificationToIdentitiesWithCC(
      IVssRequestContext requestContext,
      MailNotificationEventData notificationEventData,
      ICollection<Microsoft.VisualStudio.Services.Identity.Identity> ccIdentities,
      Guid toUserId);
  }
}
