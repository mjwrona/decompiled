// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.MailNotification
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Security.AntiXss;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class MailNotification : IMailNotification
  {
    private const string s_area = "gallery";
    private const string s_layer = "MailNotification";
    private const string s_vsmarketplace_mailaddress = "VSMarketplace@microsoft.com";
    private const string s_testmarketplace_mailaddress = "billtest550563@live.com";

    public void SendMailNotificationToPublisher(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      MailNotificationEventData notificationEventData)
    {
      requestContext.TraceEnter(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationToPublisher));
      if (extension != null)
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        ICollection<Microsoft.VisualStudio.Services.Identity.Identity> publisherOrExtension = vssRequestContext.GetService<IPublisherService>().GetContributorIdentitiesOfPublisherOrExtension(vssRequestContext, extension.Publisher.PublisherName, extension.ExtensionName);
        this.SendMailNotificationToIdentities(requestContext, extension, notificationEventData, publisherOrExtension, GalleryServiceConstants.UnsubscribeFromRnRSetting);
      }
      requestContext.TraceLeave(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationToPublisher));
    }

    public void SendExtensionPublishMailNotification(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      VersionValidationMailNotificationEvent notificationEventData,
      Guid id = default (Guid))
    {
      requestContext.TraceEnter(12061058, "gallery", nameof (MailNotification), nameof (SendExtensionPublishMailNotification));
      if (extension != null && notificationEventData != null)
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        ICollection<Microsoft.VisualStudio.Services.Identity.Identity> publisherOrExtension = vssRequestContext.GetService<IPublisherService>().GetCreatorIdentitiesOfPublisherOrExtension(vssRequestContext, extension.Publisher.PublisherName, extension.ExtensionName);
        string mailSubscriptionSettingKey = (string) null;
        if (notificationEventData.IsCvs != "true")
          mailSubscriptionSettingKey = notificationEventData.ValidationStatus == ValidationStatus.Failure ? GalleryServiceConstants.UnsubscribeFromExtensionPublishFailedSetting : GalleryServiceConstants.UnsubscribeFromExtensionPublishSuccessSetting;
        if (id == Guid.Empty)
          this.SendMailNotificationToIdentities(requestContext, extension, (MailNotificationEventData) notificationEventData, publisherOrExtension, mailSubscriptionSettingKey);
        else
          this.SendMailNotificationToIdentitiesWithCC(requestContext, extension, (MailNotificationEventData) notificationEventData, publisherOrExtension, mailSubscriptionSettingKey, id);
      }
      requestContext.TraceLeave(12061058, "gallery", nameof (MailNotification), nameof (SendExtensionPublishMailNotification));
    }

    public void SendMailNotificationToUser(
      IVssRequestContext requestContext,
      Guid userId,
      MailNotificationEventData notificationEventData)
    {
      this.SendMailNotificationToUser(requestContext, userId, notificationEventData, true);
    }

    public void SendMailNotificationToUser(
      IVssRequestContext requestContext,
      Guid userId,
      MailNotificationEventData notificationEventData,
      bool shouldEncodeHtmlNotificationContent)
    {
      requestContext.TraceEnter(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationToUser));
      if (this.GetMailSubscription().HasUserUnsubscribed(requestContext, userId, GalleryServiceConstants.UnsubscribeFromRnRSetting))
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User {0} has unsubscribed from receiving mail notification", (object) userId);
        requestContext.Trace(12061058, TraceLevel.Info, "gallery", nameof (MailNotification), message);
      }
      else if (string.IsNullOrWhiteSpace(this.GetUserMailAddress(requestContext, userId)))
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Mail Address for user Id {0} was not found.", (object) userId);
        requestContext.Trace(12061058, TraceLevel.Info, "gallery", nameof (MailNotification), message);
      }
      else
      {
        VssNotificationEvent theEvent = new VssNotificationEvent();
        theEvent.EventType = notificationEventData.EventType;
        theEvent.AddActor("recipient", userId);
        if (shouldEncodeHtmlNotificationContent)
          notificationEventData.NotificationContent = AntiXssEncoder.HtmlEncode(notificationEventData.NotificationContent, false).Replace("&#10;", "<br/>");
        theEvent.Data = (object) notificationEventData;
        requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, theEvent);
        requestContext.TraceLeave(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationToUser));
      }
    }

    public void SendMailNotificationToRecipients(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      MailNotificationEventData notificationEventData)
    {
      this.SendMailNotificationToRecipients(requestContext, recipients, notificationEventData, true);
    }

    public void SendMailNotificationToRecipients(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      MailNotificationEventData notificationEventData,
      bool shouldEncodeHtmlNotificationContent)
    {
      requestContext.TraceEnter(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationToRecipients));
      VssNotificationEvent theEvent = new VssNotificationEvent();
      theEvent.EventType = notificationEventData.EventType;
      foreach (Guid recipient in (IEnumerable<Guid>) recipients)
        theEvent.AddActor("recipient", recipient);
      if (shouldEncodeHtmlNotificationContent && notificationEventData.NotificationContent != null)
        notificationEventData.NotificationContent = AntiXssEncoder.HtmlEncode(notificationEventData.NotificationContent, false).Replace("&#10;", "<br/>");
      theEvent.Data = (object) notificationEventData;
      requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, theEvent);
      requestContext.TraceLeave(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationToRecipients));
    }

    public void SendMailNotificationUsingTeamFoundationMailService(
      IVssRequestContext requestContext,
      IList<Guid> ccIdentities,
      MailNotificationEventData notificationEventData,
      string defaultSubject)
    {
      requestContext.TraceEnter(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationUsingTeamFoundationMailService));
      VssNotificationEvent notificationEvent = new VssNotificationEvent();
      notificationEvent.EventType = notificationEventData.EventType;
      notificationEvent.Data = (object) JsonConvert.SerializeObject((object) notificationEventData, new VssJsonMediaTypeFormatter().SerializerSettings);
      NotificationDeliveryDetails notificationDeliveryDetails = new NotificationDeliveryDetails()
      {
        NotificationSource = notificationEventData.EventType,
        Matcher = "ActorMatcher",
        SourceIdentity = requestContext.GetAuthenticatedIdentity(),
        AuthId = requestContext.GetAuthenticatedIdentity().Id
      };
      TeamFoundationNotification notification = new TeamFoundationNotification()
      {
        Event = TeamFoundationEventFactory.GetEvent(notificationEvent),
        Channel = "EmailHtml",
        DeliveryDetails = notificationDeliveryDetails
      };
      NotificationTransformResult transformResult = requestContext.GetService<INotificationTransformService>().ApplyTransform(requestContext, notification);
      MailMessage message = new MailMessage()
      {
        Subject = MailNotification.ExtractSubject(requestContext, transformResult, notificationEvent, defaultSubject),
        SubjectEncoding = Encoding.UTF8,
        Body = transformResult.Content,
        BodyEncoding = Encoding.UTF8,
        IsBodyHtml = true
      };
      if (requestContext.RequestUri().Scheme == Uri.UriSchemeHttps && requestContext.RequestUri().Host.Equals("marketplace.visualstudio.com", StringComparison.OrdinalIgnoreCase))
        message.To.Add("VSMarketplace@microsoft.com");
      else
        message.To.Add("billtest550563@live.com");
      foreach (Guid ccIdentity in (IEnumerable<Guid>) ccIdentities)
        message.CC.Add(this.GetUserMailAddress(requestContext, ccIdentity));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<ITeamFoundationMailService>().QueueMailJob(vssRequestContext, message);
      requestContext.TraceLeave(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationUsingTeamFoundationMailService));
    }

    public string GetUserMailAddress(IVssRequestContext requestContext, Guid userId)
    {
      try
      {
        requestContext.TraceEnter(12061058, "gallery", nameof (MailNotification), nameof (GetUserMailAddress));
        return IdentityHelper.GetPreferredEmailAddress(requestContext, userId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061059, "gallery", nameof (MailNotification), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12061058, "gallery", nameof (MailNotification), nameof (GetUserMailAddress));
      }
    }

    public virtual IMailSubscription GetMailSubscription() => (IMailSubscription) new MailSubscription();

    private void SendMailNotificationToIdentities(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      MailNotificationEventData notificationEventData,
      ICollection<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      string mailSubscriptionSettingKey)
    {
      requestContext.TraceEnter(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationToIdentities));
      VssNotificationEvent theEvent = new VssNotificationEvent();
      if (identities == null)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Did not find any identities to notify for the publisher {0} found.", (object) extension.Publisher.PublisherName);
        requestContext.Trace(12061058, TraceLevel.Info, "gallery", nameof (MailNotification), message);
      }
      else
      {
        bool flag = false;
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
        {
          if (this.GetMailSubscription().HasUserUnsubscribed(requestContext, identity.Id, mailSubscriptionSettingKey))
          {
            string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User {0} has unsubscribed from receiving mail notification", (object) identity.Id);
            requestContext.Trace(12061058, TraceLevel.Info, "gallery", nameof (MailNotification), message);
          }
          else
          {
            flag = true;
            theEvent.AddActor("recipient", identity.Id);
          }
        }
        if (!flag)
          return;
        theEvent.EventType = notificationEventData.EventType;
        notificationEventData.NotificationContent = AntiXssEncoder.HtmlEncode(notificationEventData.NotificationContent, false)?.Replace("&#10;", "<br/>");
        theEvent.Data = (object) notificationEventData;
        requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, theEvent);
        requestContext.TraceLeave(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationToIdentities));
      }
    }

    public void SendMailNotificationToIdentitiesWithCC(
      IVssRequestContext requestContext,
      MailNotificationEventData notificationEventData,
      ICollection<Microsoft.VisualStudio.Services.Identity.Identity> ccIdentities,
      Guid toUserId)
    {
      this.SendMailNotificationToIdentitiesWithCC(requestContext, (PublishedExtension) null, notificationEventData, ccIdentities, (string) null, toUserId);
    }

    private void SendMailNotificationToIdentitiesWithCC(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      MailNotificationEventData notificationEventData,
      ICollection<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      string mailSubscriptionSettingKey,
      Guid id)
    {
      requestContext.TraceEnter(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationToIdentitiesWithCC));
      List<Guid> guidList1 = new List<Guid>();
      List<Guid> guidList2 = new List<Guid>();
      VssNotificationEvent notificationEvent = new VssNotificationEvent();
      if (identities == null)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Did not find any identities to notify for the publisher {0} found.", (object) extension.Publisher.PublisherName);
        requestContext.Trace(12061058, TraceLevel.Info, "gallery", nameof (MailNotification), message);
      }
      else
      {
        bool flag = false;
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
        {
          if (mailSubscriptionSettingKey != null && this.GetMailSubscription().HasUserUnsubscribed(requestContext, identity.Id, mailSubscriptionSettingKey))
          {
            string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User {0} has unsubscribed from receiving mail notification", (object) identity.Id);
            requestContext.Trace(12061058, TraceLevel.Info, "gallery", nameof (MailNotification), message);
          }
          else
          {
            if (identity.Id != id)
              guidList2.Add(identity.Id);
            flag = true;
          }
        }
        if (!flag)
          return;
        notificationEvent.EventType = notificationEventData.EventType;
        notificationEvent.Data = (object) JsonConvert.SerializeObject((object) notificationEventData, new VssJsonMediaTypeFormatter().SerializerSettings);
        NotificationDeliveryDetails notificationDeliveryDetails = new NotificationDeliveryDetails()
        {
          NotificationSource = notificationEventData.EventType,
          Matcher = "ActorMatcher",
          SourceIdentity = requestContext.GetAuthenticatedIdentity(),
          AuthId = requestContext.GetAuthenticatedIdentity().Id
        };
        TeamFoundationNotification notification = new TeamFoundationNotification()
        {
          Event = TeamFoundationEventFactory.GetEvent(notificationEvent),
          Channel = "EmailHtml",
          DeliveryDetails = notificationDeliveryDetails
        };
        NotificationTransformResult transformResult = requestContext.GetService<INotificationTransformService>().ApplyTransform(requestContext, notification);
        MailMessage message1 = new MailMessage()
        {
          Subject = MailNotification.ExtractSubject(requestContext, transformResult, notificationEvent, ""),
          SubjectEncoding = Encoding.UTF8,
          Body = transformResult.Content,
          BodyEncoding = Encoding.UTF8,
          IsBodyHtml = true
        };
        string userMailAddress1 = this.GetUserMailAddress(requestContext, id);
        if (!string.IsNullOrEmpty(userMailAddress1))
          message1.To.Add(userMailAddress1);
        foreach (Guid userId in guidList2)
        {
          string userMailAddress2 = this.GetUserMailAddress(requestContext, userId);
          if (!string.IsNullOrEmpty(userMailAddress2))
            message1.CC.Add(userMailAddress2);
        }
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<ITeamFoundationMailService>().QueueMailJob(vssRequestContext, message1);
        requestContext.TraceLeave(12061058, "gallery", nameof (MailNotification), nameof (SendMailNotificationToIdentitiesWithCC));
      }
    }

    private static string ExtractSubject(
      IVssRequestContext requestContext,
      NotificationTransformResult transformResult,
      VssNotificationEvent notificationEvent,
      string defaultSubject)
    {
      string input = string.Empty;
      if (transformResult.Properties != null)
        input = transformResult.Properties.Value<string>((object) "emailSubject");
      if (string.IsNullOrEmpty(input) && transformResult.Properties != null)
        input = transformResult.Properties.Value<string>((object) "email-subject");
      if (string.IsNullOrEmpty(input))
        input = defaultSubject;
      try
      {
        string subject = Regex.Replace(input, "[\r\n\t ]+", " ", RegexOptions.None, TimeSpan.FromSeconds(1.0)).Trim();
        if (subject.Length > 150)
          subject = subject.Substring(0, 147) + "...";
        return subject;
      }
      catch (RegexMatchTimeoutException ex)
      {
        requestContext.TraceLeave(12061058, "gallery", ex.ToString(), nameof (ExtractSubject));
        return input;
      }
    }
  }
}
