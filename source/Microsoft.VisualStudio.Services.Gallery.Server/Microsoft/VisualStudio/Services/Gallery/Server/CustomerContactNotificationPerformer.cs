// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CustomerContactNotificationPerformer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.MarketingPreferences;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class CustomerContactNotificationPerformer : MailNotificationPerformerBase
  {
    private const string s_layer = "CustomerMailNotificationPerformer";

    public override IList<Guid> FetchAndValidateRecipients(
      IVssRequestContext requestContext,
      NotificationsData notificationsData)
    {
      List<Guid> guidList = new List<Guid>();
      string str = notificationsData.Data["ExtensionName"] as string;
      string publisherName = notificationsData.Data["PublisherName"] as string;
      ArgumentUtility.CheckForNull<string>(str, "extensionName");
      ArgumentUtility.CheckForNull<string>(str, "extensionName");
      PublishedExtension extension = this.GetExtension(requestContext, publisherName, str);
      if (extension != null)
      {
        string message1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "CustomerContact Extension found: {0}", (object) extension.ExtensionName);
        requestContext.Trace(12062024, TraceLevel.Info, "gallery", "CustomerMailNotificationPerformer", message1);
        object identity = notificationsData.Identities["EventIds"];
        if (identity != null)
        {
          long int64 = Convert.ToInt64(identity, (IFormatProvider) CultureInfo.InvariantCulture);
          IExtensionDailyStatsService service = requestContext.GetService<IExtensionDailyStatsService>();
          ExtensionEvent extensionEvent;
          if ((notificationsData.Data["EventType"] as string).Equals("sales", StringComparison.OrdinalIgnoreCase))
          {
            DateTime? eventDateTime = notificationsData.Data["EventDate"] as DateTime?;
            string eventId = notificationsData.Data["EventId"] as string;
            string filterString = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "(EventId eq '{0}')", (object) eventId);
            extensionEvent = new CommerceDataHelper().GetExtensionCommerceEventById(requestContext, extension, eventId, eventDateTime, filterString);
          }
          else
            extensionEvent = service.GetExtensionEventByEventId(requestContext, int64, extension.ExtensionId, ExtensionLifecycleEventType.Uninstall);
          if (extensionEvent != null)
          {
            string message2 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Extension uninstall event found for extension: {0} and eventId: {1}", (object) extension.ExtensionName, (object) int64);
            requestContext.Trace(12062024, TraceLevel.Info, "gallery", "CustomerMailNotificationPerformer", message2);
            this.ValidateContibutorAccess(requestContext, extension);
            this.ValidateForPaidExtension(extension);
            this.ValidateForContactDisableOption(requestContext, extension.Publisher.PublisherName);
            Guid result;
            if (Guid.TryParse((string) extensionEvent.Properties.SelectToken("vsid"), out result))
            {
              string message3 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "reciepients {0} found for extension: {1} and eventId: {2}", (object) result, (object) extension.ExtensionName, (object) int64);
              requestContext.Trace(12062024, TraceLevel.Info, "gallery", "CustomerMailNotificationPerformer", message3);
              if (this.CanContactRecipient(requestContext, result))
              {
                string message4 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "reciepients {0} can be contacted extension: {1} and eventId: {2}", (object) result.ToString(), (object) extension.ExtensionName, (object) int64);
                requestContext.Trace(12062024, TraceLevel.Info, "gallery", "CustomerMailNotificationPerformer", message4);
                guidList.Add(result);
              }
            }
          }
        }
      }
      return (IList<Guid>) guidList;
    }

    public override void PostNotificationsStep(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      IDictionary<string, object> notificationData)
    {
      if (notificationData == null)
        return;
      string str1 = notificationData["ExtensionName"] as string;
      string publisherName = notificationData["PublisherName"] as string;
      ArgumentUtility.CheckForNull<string>(str1, "extensionName");
      ArgumentUtility.CheckForNull<string>(str1, "extensionName");
      string str2 = notificationData["AccountName"] as string;
      string propertyName = notificationData["LastContactKey"] as string;
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      ArtifactSpec artifactSpec = new ArtifactSpec(GalleryServiceConstants.CreateExtensionLastContactArtifactKind, GalleryUtil.CreateFullyQualifiedName(publisherName, str1), 0);
      List<PropertyValue> propertyValueList = new List<PropertyValue>();
      DateTime utcNow = DateTime.UtcNow;
      propertyValueList.Add(new PropertyValue(propertyName, (object) utcNow));
      List<ArtifactPropertyValue> artifactPropertyValueList1 = new List<ArtifactPropertyValue>();
      artifactPropertyValueList1.Add(new ArtifactPropertyValue(artifactSpec, (IEnumerable<PropertyValue>) propertyValueList));
      IVssRequestContext requestContext1 = requestContext;
      List<ArtifactPropertyValue> artifactPropertyValueList2 = artifactPropertyValueList1;
      service.SetProperties(requestContext1, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValueList2);
      string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Property last contacted date {0} is stored for : {1}.{2} and account: {3}", (object) utcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) publisherName, (object) str1, (object) str2);
      requestContext.Trace(12062024, TraceLevel.Info, "gallery", "CustomerMailNotificationPerformer", message);
    }

    protected override MailNotificationEventData PrepareNotificationsData(
      IVssRequestContext requestContext,
      IDictionary<string, object> notificationData)
    {
      string str1 = notificationData["PublisherDisplayName"] as string;
      string str2 = notificationData["ExtensionDisplayName"] as string;
      string str3 = notificationData["ContactText"] as string;
      string str4 = notificationData["EventType"] as string;
      string str5 = GalleryResources.CustomerContactMailSubject((object) str1, (object) str2);
      string str6 = GalleryResources.CustomerContactMailHeader((object) str1);
      string preferredEmailAddress = IdentityHelper.GetPreferredEmailAddress(requestContext, this.GetUserId(requestContext));
      CustomerContactMailNotificationData notificationData1;
      if (str4.Equals("uninstall", StringComparison.OrdinalIgnoreCase))
      {
        CustomerContactMailNotificationEvent notificationEvent = new CustomerContactMailNotificationEvent();
        string str7 = notificationData["ReasonCode"] as string;
        string str8 = notificationData["ReasonText"] as string;
        string str9 = notificationData["UninstallDate"] as string;
        string str10 = notificationData["AccountName"] as string;
        notificationEvent.IntroductionNote = GalleryResources.CustomerContactMailIntroNote((object) str2, (object) str10, (object) str9);
        notificationEvent.ReasonCodeText = GalleryResources.CustomerContactMailReasonCodeText();
        notificationEvent.ReasonCode = str7;
        notificationEvent.OtherReasonText = GalleryResources.CustomerContactMailReasonText();
        notificationEvent.OtherReason = str8;
        notificationData1 = (CustomerContactMailNotificationData) notificationEvent;
      }
      else
        notificationData1 = (CustomerContactMailNotificationData) new CustomerContactMailNotificationSalesEvent();
      notificationData1.UnsubscribeUrl = GalleryServerUtil.GetGalleryUrl(requestContext, "/unsubscribe");
      notificationData1.NotificationContent = str3;
      notificationData1.ActionButtonText = GalleryResources.CustomerContactMailActionButtonText();
      notificationData1.ActionButtonUrl = "mailto:" + preferredEmailAddress + "?subject=Re:" + str5;
      notificationData1.FromAddress = preferredEmailAddress;
      notificationData1.Subject = str5;
      notificationData1.HeaderNote = str6;
      notificationData1.ConsentNote = GalleryResources.CustomerContactMailConsentText();
      return (MailNotificationEventData) notificationData1;
    }

    protected override void PublishCIEvent(
      IVssRequestContext requestContext,
      IList<Guid> recipients,
      IDictionary<string, object> notificationData)
    {
      CustomerIntelligenceData data = new CustomerIntelligenceData();
      string var = notificationData["ExtensionName"] as string;
      string str1 = notificationData["PublisherName"] as string;
      ArgumentUtility.CheckForNull<string>(var, "extensionName");
      ArgumentUtility.CheckForNull<string>(var, "publisherName");
      string str2 = notificationData["AccountName"] as string;
      data.Add(CustomerIntelligenceProperty.Action, "ContactCustomerMail");
      data.Add("ExtensionName", var);
      data.Add("PublisherName", str1);
      data.Add("AccountName", str2);
      List<string> values = new List<string>();
      foreach (Guid recipient in (IEnumerable<Guid>) recipients)
        values.Add(recipient.ToString());
      data.Add("Recipient", values);
      this.PublishCustomerIntelligenceEvent(requestContext, data);
    }

    private void PublishCustomerIntelligenceEvent(
      IVssRequestContext requestContext,
      CustomerIntelligenceData data)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "CustomerContactNotification", data);
    }

    private bool CanContactRecipient(IVssRequestContext requestContext, Guid customerId) => this.canBeContactedPII(requestContext, customerId) && this.canBeContactedGallerySettings(requestContext, customerId);

    private bool canBeContactedPII(IVssRequestContext requestContext, Guid customerId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      bool? nullable = new bool?(vssRequestContext.GetService<IMarketingPreferencesService>().GetContactUserWithOffersSetting(vssRequestContext, customerId));
      bool flag = false;
      if (nullable.HasValue)
        flag = nullable.Value;
      return flag;
    }

    private bool canBeContactedGallerySettings(IVssRequestContext requestContext, Guid customerId) => !this.GetMailSubscription().HasUserUnsubscribed(requestContext, customerId, GalleryServiceConstants.UnsubscribeFromPublisherContactSetting);

    private void ValidateContibutorAccess(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (!GallerySecurity.HasExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.UpdateExtension, false))
        throw new HttpException(405, "User is not a contributor of the extension");
    }

    private void ValidateForPaidExtension(PublishedExtension extension)
    {
      if ((!extension.IsPaid() ? 0 : (!extension.IsPreview() ? 1 : 0)) == 0)
        throw new CustomerContactNotAllowedException("Method only allowed for Paid Extension");
    }

    private void ValidateForContactDisableOption(
      IVssRequestContext requestContext,
      string publisherName)
    {
      if (requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) ("/Configuration/Service/Gallery/DisableContactOption/" + publisherName), false, false))
        throw new CustomerContactNotAllowedException("Method not allowed");
    }

    private PublishedExtension GetExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<IPublishedExtensionService>().QueryExtension(vssRequestContext, publisherName, extensionName, (string) null, ExtensionQueryFlags.None, (string) null);
    }

    public virtual IMailSubscription GetMailSubscription() => (IMailSubscription) new MailSubscription();

    internal virtual Guid GetUserId(IVssRequestContext requestContext) => requestContext.GetUserId();
  }
}
