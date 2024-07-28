// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationSubscriberService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationSubscriberService : INotificationSubscriberService, IVssFrameworkService
  {
    private SubscriberCacheService m_subscribersCache;
    private Guid m_subscriberGeneration;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.m_subscribersCache = requestContext.GetService<SubscriberCacheService>();
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), NotificationFrameworkConstants.SubscriberServiceRoot + "/...");
      (requestContext.GetService<INotificationAdminSettingsService>() as INotificationAdminSettingsServiceInternal).SettingsChanged += new SettingsChangedHandler(this.OnSettingsChanged);
      this.ReadSettings(requestContext);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      (requestContext.GetService<INotificationAdminSettingsService>() as INotificationAdminSettingsServiceInternal).SettingsChanged -= new SettingsChangedHandler(this.OnSettingsChanged);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.ReadSettings(requestContext);
    }

    private void OnSettingsChanged(IVssRequestContext requestContext)
    {
      this.m_subscriberGeneration = Guid.NewGuid();
      this.ReadSettings(requestContext);
    }

    private void ReadSettings(IVssRequestContext requestContext)
    {
      Guid g = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) (NotificationFrameworkConstants.SubscriberServiceRoot + "/**"))["SubscriberGeneration"].GetValue<Guid>(Guid.Empty);
      if (this.m_subscriberGeneration.Equals(g))
        return;
      this.m_subscriberGeneration = g;
      this.ResetCache(requestContext);
    }

    internal void ResetCache(IVssRequestContext requestContext) => this.m_subscribersCache.Clear(requestContext);

    internal SubscriberType GetSubscriberType(Microsoft.VisualStudio.Services.Identity.Identity identity) => !identity.IsContainer ? SubscriberType.Individual : (AadIdentityHelper.IsAadGroup(identity.Descriptor) ? SubscriberType.AadGroup : SubscriberType.VsGroup);

    internal NotificationSubscriber GetSubscriber(
      IVssRequestContext requestContext,
      Guid subscriberId,
      SubscriberType subscriberType,
      SubscriberApiFlags apiFlags)
    {
      return this.GetSubscriberInternal(requestContext, subscriberId, subscriberType, apiFlags);
    }

    public NotificationSubscriber GetSubscriber(
      IVssRequestContext requestContext,
      Guid subscriberId)
    {
      return this.GetSubscriberInternal(requestContext, subscriberId, SubscriberType.Unknown, NotificationSubscriberService.DefaultApiFlags);
    }

    private NotificationSubscriber GetSubscriberInternal(
      IVssRequestContext requestContext,
      Guid subscriberId,
      SubscriberType subscriberType,
      SubscriberApiFlags apiFlags)
    {
      NotificationSubscriber subscriberInternal = (NotificationSubscriber) null;
      if (!this.m_subscribersCache.TryGetValue(requestContext, subscriberId, out subscriberInternal))
      {
        Microsoft.VisualStudio.Services.Identity.Identity subscriberIdentity = this.GetSubscriberIdentity(requestContext, subscriberId);
        if (subscriberIdentity == null)
          throw new IdentityNotFoundException();
        if (subscriberType == SubscriberType.Unknown)
          subscriberType = this.GetSubscriberType(subscriberIdentity);
        subscriberInternal = new NotificationSubscriber()
        {
          Id = subscriberIdentity.Id,
          Flags = this.GetSubscriberFlags(requestContext, subscriberIdentity, subscriberType, apiFlags)
        };
        switch (subscriberType - 1)
        {
          case SubscriberType.Unknown:
            subscriberInternal.DeliveryPreference = subscriberIdentity.GetProperty<NotificationSubscriberDeliveryPreference>("NotificationSubscriberDeliveryPreference", NotificationSubscriberDeliveryPreference.UseDefault);
            subscriberInternal.PreferredEmailAddress = subscriberIdentity.GetProperty<string>("NotificationSubscriberPreferrredEmailAddress", (string) null);
            if (subscriberInternal.DeliveryPreference == NotificationSubscriberDeliveryPreference.UseDefault)
            {
              subscriberInternal.DeliveryPreference = this.GetDefaultGroupDeliveryPreference(requestContext);
              break;
            }
            break;
          case SubscriberType.VsGroup:
            string property = subscriberIdentity.GetProperty<string>("Mail", string.Empty);
            if (!string.IsNullOrEmpty(property))
            {
              subscriberInternal.DeliveryPreference = NotificationSubscriberDeliveryPreference.PreferredEmailAddress;
              subscriberInternal.PreferredEmailAddress = property;
              break;
            }
            subscriberInternal.DeliveryPreference = NotificationSubscriberDeliveryPreference.NoDelivery;
            break;
          case SubscriberType.AadGroup:
            subscriberInternal.DeliveryPreference = NotificationSubscriberDeliveryPreference.PreferredEmailAddress;
            subscriberInternal.PreferredEmailAddress = !apiFlags.HasFlag((Enum) SubscriberApiFlags.DontFetchPreferredEmail) ? IdentityHelper.GetPreferredEmailAddress(requestContext, subscriberIdentity.Id) : subscriberIdentity.GetProperty<string>("Mail", string.Empty);
            if (string.IsNullOrEmpty(subscriberInternal.PreferredEmailAddress))
            {
              subscriberInternal.DeliveryPreference = NotificationSubscriberDeliveryPreference.NoDelivery;
              break;
            }
            break;
        }
        this.m_subscribersCache.Set(requestContext, subscriberId, subscriberInternal);
      }
      return subscriberInternal;
    }

    private NotificationSubscriberDeliveryPreference GetDefaultGroupDeliveryPreference(
      IVssRequestContext requestContext)
    {
      return NotificationAdminSettings.TranslateDefaultGroupDeliveryPreference(requestContext.GetService<INotificationAdminSettingsService>().GetSettings(requestContext).DefaultGroupDeliveryPreference);
    }

    public void InvalidateSubscriber(IVssRequestContext requestContext, Guid subscriberId)
    {
      this.m_subscriberGeneration = NotificationSubscriberService.NotifyChange(requestContext);
      this.m_subscribersCache.Remove(requestContext, subscriberId);
    }

    public NotificationSubscriber UpdateSubscriber(
      IVssRequestContext requestContext,
      Guid subscriberId,
      NotificationSubscriberUpdateParameters updateParameters)
    {
      Microsoft.VisualStudio.Services.Identity.Identity subscriberIdentity = this.GetSubscriberIdentity(requestContext, subscriberId, 2);
      SubscriberType subscriberType = this.GetSubscriberType(subscriberIdentity);
      this.ValidateUpdateParameters(requestContext, subscriberIdentity, subscriberType, updateParameters);
      subscriberIdentity.SetProperty("NotificationSubscriberDeliveryPreference", (object) updateParameters.DeliveryPreference);
      subscriberIdentity.SetProperty("NotificationSubscriberPreferrredEmailAddress", (object) updateParameters.PreferredEmailAddress);
      requestContext.GetService<IdentityService>().UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        subscriberIdentity
      });
      NotificationAuditing.PublishUpdateSubscriberEvent(requestContext, subscriberId, updateParameters);
      this.m_subscriberGeneration = NotificationSubscriberService.NotifyChange(requestContext);
      this.m_subscribersCache.Remove(requestContext, subscriberId);
      return this.GetSubscriber(requestContext, subscriberId);
    }

    private Microsoft.VisualStudio.Services.Identity.Identity GetSubscriberIdentity(
      IVssRequestContext requestContext,
      Guid subscriberId,
      int requiredPermissions = 0)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriberId, nameof (subscriberId));
      Microsoft.VisualStudio.Services.Identity.Identity subscriptionIdentity;
      if (requestContext.GetUserId() != subscriberId)
      {
        subscriptionIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
        {
          subscriberId
        }, QueryMembership.None, (IEnumerable<string>) new List<string>()
        {
          "NotificationSubscriberDeliveryPreference",
          "NotificationSubscriberPreferrredEmailAddress"
        })[0];
      }
      else
      {
        subscriptionIdentity = requestContext.GetUserIdentity();
        requiredPermissions = 0;
      }
      if (subscriptionIdentity == null)
        throw new IdentityNotFoundException(subscriberId);
      if (requiredPermissions != 0)
      {
        bool flag = NotificationSubscriptionSecurityUtils.CallerHasAdminPermissions(requestContext, 2);
        if (!flag)
          flag = !subscriptionIdentity.IsContainer ? NotificationSubscriptionSecurityUtils.HasPermissionsNoGroupAdminCheck(requestContext, subscriptionIdentity, requiredPermissions) : NotificationSubscriptionSecurityUtils.HasPermissions(requestContext, subscriptionIdentity, requiredPermissions);
        if (!flag)
          throw new UnauthorizedAccessException(CoreRes.UnauthorizedSubscriberDeliveryPreferenceUpdate((object) subscriptionIdentity.DisplayName));
      }
      return subscriptionIdentity;
    }

    private void ValidateUpdateParameters(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity subscriberIdentity,
      SubscriberType subscriberType,
      NotificationSubscriberUpdateParameters updateParameters)
    {
      ArgumentUtility.CheckForNull<NotificationSubscriberUpdateParameters>(updateParameters, nameof (updateParameters));
      SubscriberFlags subscriberFlags = this.GetSubscriberFlags(requestContext, subscriberIdentity, subscriberType, NotificationSubscriberService.DefaultApiFlags);
      if (!subscriberFlags.HasFlag((Enum) SubscriberFlags.DeliveryPreferencesEditable))
        throw new UnsupportedDeliveryPreference(CoreRes.UnsupportedUpdateDeliveryPreference());
      NotificationSubscriberDeliveryPreference? deliveryPreference1 = updateParameters.DeliveryPreference;
      if (deliveryPreference1.HasValue)
      {
        switch (deliveryPreference1.GetValueOrDefault())
        {
          case NotificationSubscriberDeliveryPreference.NoDelivery:
            if (!subscriberFlags.HasFlag((Enum) SubscriberFlags.SupportsNoDelivery))
              throw new UnsupportedDeliveryPreference(CoreRes.UnsupportedDeliveryPreference((object) updateParameters.DeliveryPreference));
            break;
          case NotificationSubscriberDeliveryPreference.PreferredEmailAddress:
            if (!subscriberFlags.HasFlag((Enum) SubscriberFlags.SupportsPreferredEmailAddressDelivery))
              throw new UnsupportedDeliveryPreference(CoreRes.UnsupportedDeliveryPreference((object) updateParameters.DeliveryPreference));
            break;
          case NotificationSubscriberDeliveryPreference.EachMember:
            if (!subscriberFlags.HasFlag((Enum) SubscriberFlags.SupportsEachMemberDelivery))
              throw new UnsupportedDeliveryPreference(CoreRes.UnsupportedDeliveryPreference((object) updateParameters.DeliveryPreference));
            break;
        }
      }
      deliveryPreference1 = updateParameters.DeliveryPreference;
      NotificationSubscriberDeliveryPreference deliveryPreference2 = NotificationSubscriberDeliveryPreference.PreferredEmailAddress;
      if (deliveryPreference1.GetValueOrDefault() == deliveryPreference2 & deliveryPreference1.HasValue && string.IsNullOrEmpty(updateParameters.PreferredEmailAddress))
        throw new ArgumentException(CoreRes.RequiredEmailAddressErrorMessage());
      deliveryPreference1 = updateParameters.DeliveryPreference;
      NotificationSubscriberDeliveryPreference deliveryPreference3 = NotificationSubscriberDeliveryPreference.PreferredEmailAddress;
      if (!(deliveryPreference1.GetValueOrDefault() == deliveryPreference3 & deliveryPreference1.HasValue))
        return;
      bool flag = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) NotificationFrameworkConstants.AsciiOnlyEmailAddressesFullPath, requestContext.ExecutionEnvironment.IsHostedDeployment);
      List<string> stringList = new List<string>();
      stringList.AddRange((IEnumerable<string>) updateParameters.PreferredEmailAddress.Split(new char[2]
      {
        ',',
        ';'
      }, StringSplitOptions.RemoveEmptyEntries));
      foreach (string str in stringList)
      {
        MailAddress mailAddress;
        try
        {
          mailAddress = new MailAddress(str.Trim());
        }
        catch (Exception ex)
        {
          throw new ArgumentException(CoreRes.EventSubscriptionInvalidEmail((object) str));
        }
        if (flag && !NotifHelpers.IsAscii(mailAddress.Address))
          throw new ArgumentException(CoreRes.EventSubscriptionInvalidEmail((object) str));
      }
    }

    private SubscriberFlags GetSubscriberFlags(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity subscriberIdentity,
      SubscriberType subscriberType,
      SubscriberApiFlags apiFlags)
    {
      SubscriberFlags subscriberFlags;
      switch (subscriberType)
      {
        case SubscriberType.VsGroup:
          subscriberFlags = SubscriberFlags.DeliveryPreferencesEditable | SubscriberFlags.SupportsPreferredEmailAddressDelivery | SubscriberFlags.SupportsEachMemberDelivery | SubscriberFlags.SupportsNoDelivery | SubscriberFlags.IsGroup;
          if (!apiFlags.HasFlag((Enum) SubscriberApiFlags.DontFetchTeamInfo) && subscriberIdentity.IsTeam(requestContext))
          {
            subscriberFlags |= SubscriberFlags.IsTeam;
            break;
          }
          break;
        case SubscriberType.AadGroup:
          subscriberFlags = SubscriberFlags.SupportsPreferredEmailAddressDelivery | SubscriberFlags.IsGroup;
          break;
        case SubscriberType.Individual:
          subscriberFlags = SubscriberFlags.SupportsPreferredEmailAddressDelivery | SubscriberFlags.IsUser;
          break;
        default:
          subscriberFlags = SubscriberFlags.None;
          break;
      }
      return subscriberFlags;
    }

    private static Guid NotifyChange(IVssRequestContext requestContext)
    {
      Guid guid = Guid.NewGuid();
      requestContext.GetService<IVssRegistryService>().SetValue<Guid>(requestContext, NotificationFrameworkConstants.SubscriberServiceRoot + "/SubscriberGeneration", guid);
      return guid;
    }

    internal static SubscriberApiFlags DefaultApiFlags => SubscriberApiFlags.None;

    internal static SubscriberApiFlags ExpansionApiFlags => SubscriberApiFlags.DontFetchPreferredEmail | SubscriberApiFlags.DontFetchTeamInfo;
  }
}
