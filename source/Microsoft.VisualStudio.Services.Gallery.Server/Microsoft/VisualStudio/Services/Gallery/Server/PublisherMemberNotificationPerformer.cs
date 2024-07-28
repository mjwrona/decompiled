// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherMemberNotificationPerformer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class PublisherMemberNotificationPerformer : MailNotificationPerformerBase
  {
    private const string s_layer = "PublisherMemberNotificationPerformer";

    public override void SendNotifications(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      IDictionary<string, object> notificationsData)
    {
      MailNotificationEventData notificationEventData = this.PrepareNotificationsData(requestContext, notificationsData, recipients);
      this.GetMailNotification().SendMailNotificationToRecipients(requestContext, recipients, notificationEventData, false);
      this.PublishCIEvent(requestContext, recipients, notificationsData);
    }

    public override IList<Guid> FetchAndValidateRecipients(
      IVssRequestContext requestContext,
      NotificationsData notificationsData)
    {
      List<Guid> guidList = new List<Guid>();
      ArgumentUtility.CheckForNull<NotificationsData>(notificationsData, nameof (notificationsData));
      string str = notificationsData.Data["PublisherName"] as string;
      ArgumentUtility.CheckForNull<string>(str, "publisherName");
      Publisher publisher = requestContext.GetService<IPublisherService>().QueryPublisher(requestContext, str, PublisherQueryFlags.None);
      if (publisher != null)
      {
        GallerySecurity.CheckPublisherPermission(requestContext, publisher, PublisherPermissions.Read | PublisherPermissions.UpdateExtension | PublisherPermissions.PublishExtension | PublisherPermissions.PrivateRead | PublisherPermissions.DeleteExtension | PublisherPermissions.EditSettings | PublisherPermissions.ViewPermissions | PublisherPermissions.ManagePermissions | PublisherPermissions.DeletePublisher);
        string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Member contact publisher found: {0}", (object) publisher.PublisherName);
        requestContext.Trace(12062041, TraceLevel.Info, "gallery", nameof (PublisherMemberNotificationPerformer), message);
        object identity = notificationsData.Identities["UserId"];
        Guid result;
        if (identity != null && Guid.TryParse(Convert.ToString(identity, (IFormatProvider) CultureInfo.InvariantCulture), out result))
          guidList.Add(result);
      }
      return guidList.Count != 0 ? (IList<Guid>) guidList : throw new IdentityNotFoundException();
    }

    protected override MailNotificationEventData PrepareNotificationsData(
      IVssRequestContext requestContext,
      IDictionary<string, object> notificationData,
      IList<Guid> recipients)
    {
      string str1 = notificationData["PublisherDisplayName"] as string;
      string str2 = notificationData["PublisherName"] as string;
      string a = notificationData["PublisherAction"] as string;
      GalleryResources.CustomerContactMailSubject((object) str1, (object) null);
      GalleryResources.CustomerContactMailHeader((object) str1);
      string str3 = "";
      if (recipients != null && recipients.Count > 0)
      {
        Guid recipient = recipients[0];
        str3 = IdentityHelper.GetPreferredEmailAddress(requestContext, recipient);
      }
      MailNotificationEventData notificationEventData = (MailNotificationEventData) new PublisherMemberMailNotificationEvent();
      if (string.Equals(a, "add"))
      {
        notificationEventData.Subject = GalleryResources.MemberAdditionEmailSubject((object) str1);
        string anchorText = notificationEventData.GetAnchorText(str1 + " (" + str2 + ")", GalleryServerUtil.GetGalleryUrl(requestContext, "/manage/publishers/" + str2));
        notificationEventData.NotificationContent = GalleryResources.MemberAdditionEmailContent((object) str3, (object) anchorText);
      }
      else
      {
        notificationEventData.Subject = GalleryResources.MemberDeletionEmailSubject((object) str1);
        notificationEventData.NotificationContent = GalleryResources.MemberDeletionEmailContent((object) str3, (object) str1, (object) str2);
      }
      return notificationEventData;
    }

    protected override MailNotificationEventData PrepareNotificationsData(
      IVssRequestContext requestContext,
      IDictionary<string, object> notificationData)
    {
      throw new NotImplementedException("Need recipient to prepare notification content");
    }
  }
}
