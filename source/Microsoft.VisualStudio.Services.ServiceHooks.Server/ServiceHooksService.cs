// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.ServiceHooksService
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.Notifications.WebApi.Clients;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers;
using Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  public class ServiceHooksService : INotificationTracer, IVssFrameworkService
  {
    private const string c_falseLower = "false";
    private const int c_maxNotifyConsumerRetries = 8;
    private const int c_deleteNotificationsOlderThanDays = 7;
    private const int c_maxRetriesWhenSubscriptionOnProbation = 7;
    private const string c_registryPathServiceHooksRoot = "/Service/ServiceHooks";
    private const string c_registryPathDoNotWaitBetweenProbationRetries = "/Service/ServiceHooks/Settings/DoNotWaitBetweenProbationRetries";
    private const string c_registryPathDoNotSleepBetweenTransientRetries = "/Service/ServiceHooks/Settings/DoNotSleepBetweenTransientRetries";
    private const string c_registryPathOnPremisesIncludeAllConsumers = "/Service/ServiceHooks/Settings/OnPremises/IncludeAllConsumers";
    private const string c_allowFullContentFeatureFlagName = "ServiceHooks.Notification.AllowFullContentLength";
    private const string c_readSingleSubscriptionFlagName = "ServiceHooks.Notification.ReadSingleSubscription";
    private const string c_preventOnProbationOverwriteFeatureFlagName = "ServiceHooks.Notification.PreventOnProbationOverwrite";
    private const string c_useProbationDateFlagName = "ServiceHooks.Notification.UseLastProbationRetryDate";
    private const string c_null = "<null>";
    private const long c_subscriptionTracingThresholdInMs = 10000;
    private static readonly string s_layer = typeof (ServiceHooksService).Name;
    private static readonly string s_area = typeof (ServiceHooksService).Namespace;
    private static readonly TimeSpan s_transientFailureMinBackoff = TimeSpan.FromSeconds(1.0);
    private static readonly TimeSpan s_transientFailureMaxBackoff = TimeSpan.FromSeconds(60.0);
    private static readonly TimeSpan s_transientFailureDeltaBackoff = TimeSpan.FromSeconds(1.0);
    private static readonly TimeSpan s_subscriptionProbationMaxBackoff = TimeSpan.FromHours(15.0);
    private static readonly TimeSpan s_subscriptionProbationMinBackoff = TimeSpan.FromMinutes(10.0);
    private static readonly TimeSpan s_subscriptionProbationDeltaBackoff = ServiceHooksService.s_subscriptionProbationMinBackoff;
    protected static bool s_includeAllConsumers = false;
    protected const bool c_includeAllConsumersDefaultValue = false;
    private static HashSet<string> s_publisherInputKeysToLog = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "tag",
      "projectId"
    };
    private IdentityService m_identityService;
    private CustomerIntelligenceService m_ciService;
    private IVssDeploymentServiceHost m_deploymentServiceHost;
    private IVssRegistryService m_registryService;

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      systemRequestContext.TraceEnter(1051010, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (ServiceStart));
      try
      {
        this.m_identityService = systemRequestContext.GetService<IdentityService>();
        this.m_ciService = systemRequestContext.GetService<CustomerIntelligenceService>();
        this.m_registryService = systemRequestContext.GetService<IVssRegistryService>();
        this.m_deploymentServiceHost = (IVssDeploymentServiceHost) systemRequestContext.To(TeamFoundationHostType.Deployment).ServiceHost;
        this.CacheServiceHooksRegistryValues(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1051020, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1051030, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (ServiceStart));
      }
    }

    protected virtual void CacheServiceHooksRegistryValues(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      systemRequestContext.TraceEnter(1051888, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CacheServiceHooksRegistryValues));
      try
      {
        IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
        ServiceHooksService.s_includeAllConsumers = service.GetValue<bool>(systemRequestContext, (RegistryQuery) "/Service/ServiceHooks/Settings/OnPremises/IncludeAllConsumers", false);
        systemRequestContext.Trace(1051011, TraceLevel.Verbose, ServiceHooksService.s_area, ServiceHooksService.s_layer, "Read 'OnPremisesIncludeAllConsumers' with value '{0}'", (object) ServiceHooksService.s_includeAllConsumers);
        service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), "/Service/ServiceHooks/**");
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1051899, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1051900, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CacheServiceHooksRegistryValues));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      systemRequestContext.TraceEnter(1051730, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (ServiceEnd));
      try
      {
        IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
        systemRequestContext.Trace(1051011, TraceLevel.Verbose, ServiceHooksService.s_area, ServiceHooksService.s_layer, "Read 'OnPremisesIncludeAllConsumers' with value '{0}'", (object) ServiceHooksService.s_includeAllConsumers);
        IVssRequestContext requestContext = systemRequestContext;
        RegistrySettingsChangedCallback callback = new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged);
        service.UnregisterNotification(requestContext, callback);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1051740, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1051750, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (ServiceEnd));
      }
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051760, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (OnRegistrySettingsChanged));
      try
      {
        ServiceHooksService.s_includeAllConsumers = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Service/ServiceHooks/Settings/OnPremises/IncludeAllConsumers", false);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051770, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051780, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (OnRegistrySettingsChanged));
      }
    }

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription GetSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051040, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetSubscription));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
        IServiceHooksComponent serviceHooksComponent;
        Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription;
        using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          subscription = serviceHooksComponent.ReadSubscription(subscriptionId);
        if (subscription == null)
          throw new SubscriptionNotFoundException(subscriptionId);
        SubscriptionInputValueStrongBoxHelper.AddConfidentialInputValuesToSubscription(requestContext, subscription, true);
        this.PopulateSubscriptionIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>()
        {
          subscription
        });
        return subscription;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051050, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051060, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetSubscription));
      }
    }

    public IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> GetSubscriptions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> subscriptionIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051070, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetSubscriptions));
      try
      {
        ArgumentUtility.CheckForNull<IEnumerable<Guid>>(subscriptionIds, nameof (subscriptionIds));
        ArgumentUtility.CheckForOutOfRange(subscriptionIds.Count<Guid>(), nameof (subscriptionIds), 1);
        IServiceHooksComponent serviceHooksComponent;
        IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptionList;
        using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          subscriptionList = serviceHooksComponent.ReadSubscriptions(subscriptionIds);
        IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions = this.RemoveSubscriptionsOfInactiveConsumers(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptionList);
        foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptions)
          SubscriptionInputValueStrongBoxHelper.AddConfidentialInputValuesToSubscription(requestContext, subscription, true);
        this.PopulateSubscriptionIdentities(requestContext, subscriptions);
        return subscriptions;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051080, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051090, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetSubscriptions));
      }
    }

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription CreateSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051100, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CreateSubscription));
      try
      {
        this.ValidateSubscription(requestContext, subscription, true);
        if (subscription.Id == Guid.Empty)
          subscription.Id = Guid.NewGuid();
        Guid requestorIdentityId = this.GetRequestorIdentityId(requestContext);
        if (subscription.CreatedBy == null)
          subscription.CreatedBy = new IdentityRef()
          {
            Id = requestorIdentityId.ToString()
          };
        if (subscription.ModifiedBy == null)
          subscription.ModifiedBy = subscription.CreatedBy;
        if (subscription.EventDescription == null)
          subscription.EventDescription = ServiceHooksResources.NoCustomEventDescription;
        List<KeyValuePair<string, string>> confidentialPublisherInputs;
        List<KeyValuePair<string, string>> confidentialConsumerInputs;
        SubscriptionInputValueStrongBoxHelper.StripConfidentialInputs(requestContext, subscription, out confidentialPublisherInputs, out confidentialConsumerInputs);
        SubscriptionInputValueStrongBoxHelper.StoreConfidentialInputs(requestContext, subscription, confidentialPublisherInputs, confidentialConsumerInputs);
        this.TraceSubscription(requestContext, subscription, nameof (CreateSubscription));
        IServiceHooksComponent serviceHooksComponent;
        Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription1;
        using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          subscription1 = serviceHooksComponent.CreateSubscription(subscription);
        SubscriptionInputValueStrongBoxHelper.ApplyConfidentialInputs(requestContext, subscription1, confidentialPublisherInputs, confidentialConsumerInputs);
        this.PopulateSubscriptionIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>()
        {
          subscription1
        });
        this.PublishCreateUpdateSubscriptionEvent(requestContext, ServiceHooksCustomerIntelligenceInfo.FeatureCreateSubscription, subscription1);
        return subscription1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051110, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051120, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CreateSubscription));
      }
    }

    public void UpdateSubscriptionStatus(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus status,
      bool resetProbationRetries,
      bool incrementProbationRetries,
      bool requestedByUser = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051580, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (UpdateSubscriptionStatus));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
        if (resetProbationRetries && incrementProbationRetries)
          throw new InvalidOperationException(ServiceHooksResources.Error_CannotResetAndIncrementProbationRetries);
        Guid requestorIdentityId = this.GetRequestorIdentityId(requestContext);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        requestedByUser = requestedByUser && IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity);
        using (IServiceHooksComponent serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          serviceHooksComponent.UpdateSubscriptionStatus(subscriptionId, status, requestorIdentityId, resetProbationRetries, incrementProbationRetries, requestedByUser);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051590, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051600, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (UpdateSubscriptionStatus));
      }
    }

    private void DisableSubscriptionBySytem(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      ActionTaskResult actionResult,
      double sendDuration,
      IReadOnlyDictionary<Guid, string> notificationSubscriptionIdMap)
    {
      this.UpdateSubscriptionStatus(requestContext, subscription.Id, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.DisabledBySystem, true, false, false);
      this.PublishCiFailedNotificationEvent(requestContext, notification, subscription, actionResult, ServiceHooksCustomerIntelligenceInfo.FeatureSubscriptionDisableBySystem, sendDuration);
      this.TryUpdateRemoteSubscriptionStatus(requestContext, notification, subscription, notificationSubscriptionIdMap, false);
    }

    private void PlaceSubscriptionOnProbation(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      ActionTaskResult actionResult,
      double sendDuration)
    {
      this.UpdateSubscriptionStatus(requestContext, subscription.Id, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.OnProbation, true, false, false);
      this.PublishCiFailedNotificationEvent(requestContext, notification, subscription, actionResult, ServiceHooksCustomerIntelligenceInfo.FeatureSubscriptionPlaceOnProbation, sendDuration);
    }

    private void TryUpdateRemoteSubscriptionStatus(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      IReadOnlyDictionary<Guid, string> notificationSubscriptionIdMap,
      bool enable)
    {
      if (notificationSubscriptionIdMap == null || !notificationSubscriptionIdMap.ContainsKey(notification.SubscriptionId))
        return;
      NotificationHttpClient notificationHttpClient = subscription.GetNotificationHttpClient(requestContext);
      Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus subscriptionStatus;
      string str;
      if (enable)
      {
        subscriptionStatus = Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.Enabled;
        str = "Enabled from probation";
      }
      else
      {
        subscriptionStatus = Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.DisabledFromProbation;
        str = "Disabled by system";
      }
      NotificationSubscriptionUpdateParameters updateParameters = new NotificationSubscriptionUpdateParameters();
      updateParameters.Status = new Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus?(subscriptionStatus);
      updateParameters.StatusMessage = str;
      string notificationSubscriptionId = notificationSubscriptionIdMap[notification.SubscriptionId];
      CancellationToken cancellationToken = new CancellationToken();
      notificationHttpClient.UpdateSubscriptionAsync(updateParameters, notificationSubscriptionId, cancellationToken: cancellationToken).Wait();
    }

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription UpdateSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      return this.UpdateSubscription(requestContext, subscription, false, true);
    }

    internal Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription UpdateSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      bool updateProbationRetries = false,
      bool requestedByUser = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051130, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (UpdateSubscription));
      try
      {
        this.ValidateSubscription(requestContext, subscription, false);
        Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription1 = this.GetSubscription(requestContext, subscription.Id);
        this.ValidateUpdatedConfidentialInputs(requestContext, subscription.ConsumerId, subscription.ConsumerActionId, subscription.ConsumerInputs, subscription1.ConsumerInputs);
        if (subscription.EventDescription == null)
          subscription.EventDescription = ServiceHooksResources.NoCustomEventDescription;
        if (subscription.ModifiedBy == null)
          subscription.ModifiedBy = new IdentityRef()
          {
            Id = this.GetRequestorIdentityId(requestContext).ToString()
          };
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          Guid.Parse(subscription.ModifiedBy.Id)
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        requestedByUser = requestedByUser && IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) identity);
        List<KeyValuePair<string, string>> confidentialPublisherInputs;
        List<KeyValuePair<string, string>> confidentialConsumerInputs;
        SubscriptionInputValueStrongBoxHelper.StripConfidentialInputs(requestContext, subscription, out confidentialPublisherInputs, out confidentialConsumerInputs);
        this.TraceSubscription(requestContext, subscription, nameof (UpdateSubscription));
        IServiceHooksComponent serviceHooksComponent;
        Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription2;
        using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          subscription2 = serviceHooksComponent.UpdateSubscription(subscription, updateProbationRetries, requestedByUser);
        if (subscription2 != null)
        {
          SubscriptionInputValueStrongBoxHelper.UpdateUnmaskedInputsInStrongBox(requestContext, subscription2, confidentialPublisherInputs, confidentialConsumerInputs);
          SubscriptionInputValueStrongBoxHelper.RemoveOrphanedInputValuesFromStrongBox(requestContext, subscription2, confidentialPublisherInputs, confidentialConsumerInputs);
        }
        this.PopulateSubscriptionIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>()
        {
          subscription2
        });
        this.PublishCreateUpdateSubscriptionEvent(requestContext, ServiceHooksCustomerIntelligenceInfo.FeatureUpdateSubscription, subscription2);
        return subscription2;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051140, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051150, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (UpdateSubscription));
      }
    }

    public void DeleteSubscription(IVssRequestContext requestContext, Guid subscriptionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051160, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (DeleteSubscription));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
        string consumerId = this.GetSubscription(requestContext, subscriptionId).ConsumerId;
        IServiceHooksComponent serviceHooksComponent;
        using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          serviceHooksComponent.DeleteSubscription(subscriptionId);
        SubscriptionInputValueStrongBoxHelper.DeleteDrawer(requestContext, subscriptionId);
        ServiceHooksTokenHelper.DeleteTokensForSubscription(requestContext, subscriptionId);
        requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          JobConstants.NotificationDetailsCleanupJobId
        }, false);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(ServiceHooksCustomerIntelligenceInfo.KeySubscriptionId, subscriptionId.ToString());
        properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerId, consumerId);
        this.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ServiceHooksCustomerIntelligenceInfo.FeatureDeleteSubscription, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051170, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051180, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (DeleteSubscription));
      }
    }

    public IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> QuerySubscriptions(
      IVssRequestContext requestContext,
      SubscriptionsQuery query,
      bool unmaskConfidentialInputs = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051190, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (QuerySubscriptions));
      try
      {
        ArgumentUtility.CheckForNull<SubscriptionsQuery>(query, nameof (query));
        return this.QuerySubscriptions(requestContext, new Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus?(), query.PublisherId, query.EventType, query.PublisherInputFilters, query.ConsumerInputFilters, query.ConsumerId, query.ConsumerActionId, unmaskConfidentialInputs);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051200, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051210, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (QuerySubscriptions));
      }
    }

    public IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> QuerySubscriptions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus? status,
      string publisherId,
      string eventType,
      IEnumerable<InputFilter> publisherInputs,
      IEnumerable<InputFilter> consumerInputs,
      string consumerId,
      string consumerActionId,
      bool unmaskConfidentialInputs = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051220, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (QuerySubscriptions));
      try
      {
        ServiceHooksTimer serviceHooksTimer = ServiceHooksTimer.StartNew();
        if (publisherId != null)
          ArgumentUtility.CheckStringForNullOrWhiteSpace(publisherId, nameof (publisherId));
        if (eventType != null)
          ArgumentUtility.CheckStringForNullOrWhiteSpace(eventType, nameof (eventType));
        if (consumerId != null)
          ArgumentUtility.CheckStringForNullOrWhiteSpace(consumerId, nameof (consumerId));
        if (consumerActionId != null)
          ArgumentUtility.CheckStringForNullOrWhiteSpace(consumerActionId, nameof (consumerActionId));
        if (publisherInputs != null)
        {
          foreach (InputFilter publisherInput in publisherInputs)
            ArgumentUtility.CheckForNull<List<InputFilterCondition>>(publisherInput?.Conditions, "publisherInputFilter.conditions");
        }
        if (consumerInputs != null)
        {
          foreach (InputFilter consumerInput in consumerInputs)
            ArgumentUtility.CheckForNull<List<InputFilterCondition>>(consumerInput?.Conditions, "consumerInputFilter.conditions");
        }
        IServiceHooksComponent serviceHooksComponent;
        IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions;
        using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          subscriptions = serviceHooksComponent.QuerySubscriptions(status, publisherId, eventType, consumerId, consumerActionId);
        serviceHooksTimer.RecordTick();
        if (publisherInputs != null)
          subscriptions = (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptions.Where<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, bool>) (subscription => publisherInputs.Evaluate(subscription.PublisherInputs))).ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
        serviceHooksTimer.RecordTick();
        if (consumerInputs != null)
          subscriptions = (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptions.Where<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, bool>) (subscription => consumerInputs.Evaluate(subscription.ConsumerInputs))).ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
        serviceHooksTimer.RecordTick();
        IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> source = (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) this.RemoveSubscriptionsOfInactiveConsumers(requestContext, subscriptions);
        serviceHooksTimer.RecordTick();
        foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in source)
          SubscriptionInputValueStrongBoxHelper.AddConfidentialInputValuesToSubscription(requestContext, subscription, !unmaskConfidentialInputs);
        serviceHooksTimer.RecordTick();
        this.PopulateSubscriptionIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) source.ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>());
        serviceHooksTimer.Stop();
        requestContext.Trace(1051930, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, string.Format("Total millis: {0}, percent breakdown: {1}", (object) serviceHooksTimer.Millis, (object) serviceHooksTimer.Percents));
        return source;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051230, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051240, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (QuerySubscriptions));
      }
    }

    public void PublishEvent(IVssRequestContext requestContext, PublisherEvent eventToPublish)
    {
      ArgumentUtility.CheckForNull<PublisherEvent>(eventToPublish, nameof (eventToPublish));
      this.PublishEvents(requestContext, (IEnumerable<PublisherEvent>) new PublisherEvent[1]
      {
        eventToPublish
      });
    }

    public void PublishEvents(
      IVssRequestContext requestContext,
      IEnumerable<PublisherEvent> eventsToPublish)
    {
      Dictionary<Guid, string> notificationToSubscriptionIdMap = new Dictionary<Guid, string>();
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051280, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (PublishEvents));
      ServiceHooksTimer publishEventsOverallTimer = ServiceHooksTimer.StartNew();
      Dictionary<Guid, ServiceHooksTimer> publishEventTimers = new Dictionary<Guid, ServiceHooksTimer>();
      Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>> dictionary = new Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>>();
      try
      {
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> notifications = new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>();
        ArgumentUtility.CheckForNull<IEnumerable<PublisherEvent>>(eventsToPublish, nameof (eventsToPublish));
        ArgumentUtility.CheckForOutOfRange(eventsToPublish.Count<PublisherEvent>(), nameof (eventsToPublish), 1);
        IEnumerable<PublisherEvent> source = eventsToPublish.Where<PublisherEvent>((Func<PublisherEvent, bool>) (eventToPublish => !eventToPublish.IsFilteredEvent));
        string publisherId = source != null ? source.FirstOrDefault<PublisherEvent>()?.Event?.PublisherId : (string) null;
        bool flag = source.Count<PublisherEvent>() > 0 && source.All<PublisherEvent>((Func<PublisherEvent, bool>) (eventToPublish => eventToPublish != null && eventToPublish.Event?.PublisherId != null && eventToPublish.Event.PublisherId.Equals(publisherId)));
        foreach (PublisherEvent publisherEvent in eventsToPublish)
        {
          Guid key = Guid.NewGuid();
          publishEventTimers.Add(key, ServiceHooksTimer.StartNew());
          if (publisherEvent.IsFilteredEvent)
          {
            this.CreateNotificationForFilteredEvent(requestContext, publisherEvent);
            publishEventTimers[key].Stop();
          }
          else if (this.ValidateEvent(requestContext, publisherEvent))
          {
            publisherEvent.Event.Id = key;
            publisherEvent.Event.CreatedDate = DateTime.UtcNow;
            string str = (string) null;
            publisherEvent.Diagnostics?.TryGetValue("NotificationSubscriptionId", out str);
            CustomerIntelligenceData eventData = new CustomerIntelligenceData();
            eventData.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventId, (object) publisherEvent.Event.Id);
            eventData.Add(ServiceHooksCustomerIntelligenceInfo.KeyPublisherId, publisherEvent.Event.PublisherId);
            eventData.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventType, publisherEvent.Event.EventType);
            IDictionary<string, string> diagnostics = publisherEvent.Diagnostics;
            if (diagnostics != null)
              diagnostics.ForEach<KeyValuePair<string, string>>((Action<KeyValuePair<string, string>>) (pair => eventData.Add(pair.Key, pair.Value)));
            this.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ServiceHooksCustomerIntelligenceInfo.FeatureEventPublished, eventData);
            publishEventTimers[key].RecordTick();
            IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> forPublishedEvent = this.CreateNotificationsForPublishedEvent(requestContext, publisherEvent, publishEventTimers[key], flag ? dictionary : (Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>>) null);
            if (str != null)
            {
              foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification in (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) forPublishedEvent)
              {
                if (!notificationToSubscriptionIdMap.ContainsKey(notification.SubscriptionId))
                  notificationToSubscriptionIdMap.Add(notification.SubscriptionId, str);
              }
            }
            notifications.AddRange((IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) forPublishedEvent);
            publishEventTimers[key].Stop();
          }
        }
        publishEventsOverallTimer.RecordTick();
        this.DeliverNotifications(requestContext, (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) notifications, (IReadOnlyDictionary<Guid, string>) notificationToSubscriptionIdMap);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051290, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        if (publishEventsOverallTimer != null)
        {
          publishEventsOverallTimer.Stop();
          requestContext.TraceConditionally(1051285, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, (Func<string>) (() =>
          {
            string str1 = publishEventsOverallTimer.Millis.ToString("D") ?? "";
            string str2 = publishEventsOverallTimer.Percents ?? "";
            IEnumerable<PublisherEvent> source1 = eventsToPublish;
            string str3 = string.Format("{0}", (object) (source1 != null ? new int?(source1.Count<PublisherEvent>()) : new int?()));
            IEnumerable<PublisherEvent> source2 = eventsToPublish;
            string str4 = (source2 != null ? source2.First<PublisherEvent>()?.Event?.EventType : (string) null) ?? "";
            IEnumerable<\u003C\u003Ef__AnonymousType4<Guid, ServiceHooksTimer, string>> datas = publishEventTimers.Keys.Select(eventId => new
            {
              EventId = eventId,
              TimeTakenToProcessInMs = publishEventTimers[eventId],
              Percents = publishEventTimers[eventId].Percents
            });
            return JsonConvert.SerializeObject((object) new
            {
              OverallTimeTakenInMs = str1,
              OverallPercents = str2,
              NumberOfEventsToPublish = str3,
              EventType = str4,
              EventTimings = datas
            });
          }));
        }
        requestContext.TraceLeave(1051300, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (PublishEvents));
      }
    }

    public void DeliverNotifications(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> notifications,
      IReadOnlyDictionary<Guid, string> notificationToSubscriptionIdMap = null)
    {
      if (notifications == null || notifications.Count == 0)
        return;
      requestContext.Elevate().Fork((Func<IVssRequestContext, Task>) (async forkedContext =>
      {
        forkedContext.TraceEnter(1051700, ServiceHooksService.s_area, ServiceHooksService.s_layer, "PublishEventsInner");
        try
        {
          await this.SendNotificationsAsync(forkedContext, notifications, notificationToSubscriptionIdMap);
        }
        catch (Exception ex)
        {
          forkedContext.TraceException(1051710, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
          throw;
        }
        finally
        {
          forkedContext.TraceLeave(1051720, ServiceHooksService.s_area, ServiceHooksService.s_layer, "PublishEventsInner");
        }
      }), "ServiceHooksService.SendNotificationsAsync");
    }

    public Consumer GetConsumer(IVssRequestContext requestContext, string consumerId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051310, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetConsumer));
      try
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(consumerId, nameof (consumerId));
        return this.GetConsumerImplementation(requestContext, consumerId).ToConsumer();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051320, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051330, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetConsumer));
      }
    }

    public IEnumerable<Consumer> GetConsumers(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051340, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetConsumers));
      try
      {
        return (IEnumerable<Consumer>) this.GetConsumerImplementations(requestContext).ToConsumers(this.IsOnPremisesDeployment(requestContext), ServiceHooksService.s_includeAllConsumers);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051350, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051360, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetConsumers));
      }
    }

    public IList<InputValues> GetConsumerInputValues(
      IVssRequestContext requestContext,
      string consumerId,
      string consumerActionId,
      IDictionary<string, string> currentConsumerInputValues,
      IEnumerable<string> inputIds,
      Guid? subscriptionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051610, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetConsumerInputValues));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(consumerId, nameof (consumerId));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(consumerActionId, nameof (consumerActionId));
        ArgumentUtility.CheckForNull<IDictionary<string, string>>(currentConsumerInputValues, nameof (currentConsumerInputValues));
        ArgumentUtility.CheckForNull<IEnumerable<string>>(inputIds, nameof (inputIds));
        if (subscriptionId.HasValue)
          ArgumentUtility.CheckForEmptyGuid(subscriptionId.Value, nameof (subscriptionId));
        ConsumerImplementation consumerImplementation = this.GetConsumerImplementation(requestContext, consumerId);
        ConsumerActionImplementation actionImplementation = consumerImplementation.Actions.FirstOrDefault<ConsumerActionImplementation>((Func<ConsumerActionImplementation, bool>) (a => string.Equals(consumerActionId, a.Id)));
        if (actionImplementation == null)
          throw new ConsumerActionNotFoundException(consumerId, consumerActionId);
        if (subscriptionId.HasValue && currentConsumerInputValues.Count != 0)
          SubscriptionInputValueStrongBoxHelper.RestoreConfidentialInputValues(requestContext, consumerId, consumerActionId, subscriptionId.Value, currentConsumerInputValues);
        List<InputValues> consumerInputValues = new List<InputValues>(inputIds.Count<string>());
        foreach (string inputId in inputIds)
        {
          InputValues inputValues = actionImplementation.GetInputValues(requestContext, inputId, currentConsumerInputValues) ?? consumerImplementation.GetInputValues(requestContext, inputId, currentConsumerInputValues) ?? new InputValues();
          inputValues.InputId = inputId;
          consumerInputValues.Add(inputValues);
        }
        return (IList<InputValues>) consumerInputValues;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051620, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051630, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetConsumerInputValues));
      }
    }

    private void TraceSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      string prefix = null)
    {
      if (subscription == null)
        return;
      try
      {
        if (!requestContext.IsTracing(1051908, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer))
          return;
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(prefix ?? "Subscription");
        stringBuilder.AppendLine(string.Format("Id: {0}", (object) subscription.Id));
        stringBuilder.AppendLine(string.Format("Status: {0}", (object) subscription.Status));
        stringBuilder.AppendLine("PublisherId: " + (subscription.PublisherId ?? "<null>"));
        stringBuilder.AppendLine("EventType:  " + (subscription.EventType ?? "<null>"));
        stringBuilder.AppendLine("ResourceVersion:  " + (subscription.ResourceVersion ?? "<null>"));
        stringBuilder.AppendLine("EventDescription:  " + (subscription.EventDescription ?? "<null>"));
        stringBuilder.AppendLine("ConsumerId:  " + (subscription.ConsumerId ?? "<null>"));
        stringBuilder.AppendLine("ConsumerActionId:  " + (subscription.ConsumerActionId ?? "<null>"));
        stringBuilder.AppendLine("ActionDescription:  " + (subscription.ActionDescription ?? "<null>"));
        stringBuilder.AppendLine(string.Format("ProbationRetries:  {0}", (object) subscription.ProbationRetries));
        stringBuilder.AppendLine("CreatedBy: " + (subscription.CreatedBy?.Id ?? "<null>"));
        stringBuilder.AppendLine("ModifiedBy: " + (subscription.ModifiedBy?.Id ?? "<null>"));
        stringBuilder.AppendLine(this.InputsToString("ConsumerInputs: ", subscription.ConsumerInputs));
        stringBuilder.AppendLine(this.InputsToString("PublisherInputs: ", subscription.PublisherInputs));
        string message = stringBuilder.ToString();
        requestContext.Trace(1051908, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, message);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051908, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
      }
    }

    private string InputsToString(string prefix, IDictionary<string, string> inputs)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (inputs == null)
      {
        stringBuilder.Append(prefix + "<null>");
      }
      else
      {
        int num = 0;
        stringBuilder.Append(prefix ?? "");
        foreach (string key in (IEnumerable<string>) inputs.Keys)
        {
          stringBuilder.AppendLine();
          stringBuilder.Append(string.Format("\t[{0}]: {1}, {2}", (object) num++, (object) (key ?? "<null>"), (object) (inputs[key] ?? "<null>")));
        }
      }
      return stringBuilder.ToString();
    }

    private void TraceNotification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      string prefix = "")
    {
      if (notification == null)
        return;
      try
      {
        string format = prefix + ": SubscriptionId: {0}; NotificationId: {1}, EventId: {2}; Result: {3}; Status: {4}; PublisherId: {5}; QueuedDate: {6}; RequestAttempts: {7}; ConsumerId: {8}; ConsumerActionId: {9}; EventType: {10}; ThreadId: {11}";
        object[] objArray = new object[12]
        {
          (object) notification.SubscriptionId,
          (object) notification.Id,
          (object) notification.EventId,
          (object) notification.Result,
          (object) notification.Status,
          (object) (notification.Details?.PublisherId ?? "<null>"),
          (object) ((DateTime?) notification.Details?.QueuedDate ?? DateTime.MinValue),
          null,
          null,
          null,
          null,
          null
        };
        NotificationDetails details = notification.Details;
        objArray[7] = (object) (details != null ? details.RequestAttempts : int.MinValue);
        objArray[8] = (object) (notification.Details?.ConsumerId ?? "<null>");
        objArray[9] = (object) (notification.Details?.ConsumerActionId ?? "<null>");
        objArray[10] = (object) (notification.Details?.Event.EventType ?? "<null>");
        objArray[11] = (object) Environment.CurrentManagedThreadId;
        string message = string.Format(format, objArray);
        requestContext.Trace(1051904, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, message);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051904, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
      }
    }

    internal async Task SendNotificationsAsync(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> notifications,
      IReadOnlyDictionary<Guid, string> notificationSubscriptionIdMap = null)
    {
      bool doNotSleepBetweenTransientRetries = this.m_registryService.GetValue<bool>(requestContext, (RegistryQuery) "/Service/ServiceHooks/Settings/DoNotSleepBetweenTransientRetries", true, false);
      await this.SendNotificationsAsync(requestContext, (IEnumerable<ServiceHooksService.NotificationItem>) notifications.Select<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification, ServiceHooksService.NotificationItem>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification, ServiceHooksService.NotificationItem>) (n => new ServiceHooksService.NotificationItem(n))).ToList<ServiceHooksService.NotificationItem>(), notificationSubscriptionIdMap, doNotSleepBetweenTransientRetries: doNotSleepBetweenTransientRetries);
    }

    private async Task SendNotificationsAsync(
      IVssRequestContext requestContext,
      IEnumerable<ServiceHooksService.NotificationItem> notifications,
      IReadOnlyDictionary<Guid, string> notificationSubscriptionIdMap = null,
      int attempt = 0,
      bool doNotSleepBetweenTransientRetries = false)
    {
      requestContext.TraceEnter(1051640, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (SendNotificationsAsync));
      List<ServiceHooksService.NotificationItem> itemsForRetry = new List<ServiceHooksService.NotificationItem>();
      try
      {
        Queue<ServiceHooksService.NotificationItem> queue = new Queue<ServiceHooksService.NotificationItem>(notifications);
        ActionTaskResult result;
        ServiceHooksService.NotificationItem currentItem;
        while (queue.Count > 0)
        {
          result = (ActionTaskResult) null;
          currentItem = queue.Dequeue();
          DateTime notifyConsumerStartUtc = DateTime.UtcNow;
          this.TraceNotification(requestContext, currentItem.Notification, "Dequeued");
          if (!currentItem.IsTest)
          {
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyPublisherId, currentItem.Notification.Details.PublisherId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventType, currentItem.Notification.Details.EventType);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyNotificationId, (double) currentItem.Notification.Id);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeySubscriptionId, (object) currentItem.Notification.SubscriptionId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventId, (object) currentItem.Notification.EventId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerId, currentItem.Notification.Details.ConsumerId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerActionId, currentItem.Notification.Details.ConsumerActionId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyQueueWaitTime, (DateTime.UtcNow - currentItem.Notification.CreatedDate).TotalSeconds);
            this.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ServiceHooksCustomerIntelligenceInfo.FeatureNotificationDequeued, properties);
          }
          try
          {
            this.UpdateNotification(requestContext, currentItem.Notification.SubscriptionId, currentItem.Notification.Id, new NotificationStatus?(NotificationStatus.Processing));
            if (!currentItem.ConfidentialValuesRestored)
              SubscriptionInputValueStrongBoxHelper.RestoreConfidentialInputValuesToNotification(requestContext, currentItem.Notification);
            notifyConsumerStartUtc = DateTime.UtcNow;
            result = await this.NotifyConsumerAsync(requestContext, currentItem.Notification);
          }
          catch (Exception ex)
          {
            this.UpdateNotification(requestContext, currentItem.Notification.SubscriptionId, currentItem.Notification.Id, new NotificationStatus?(NotificationStatus.Completed), new NotificationResult?(NotificationResult.Failed), errorMessage: ServiceHooksResources.Error_CouldNotSendNotification, errorDetail: this.GetErrorDetailMessageWithActivityId(requestContext));
            requestContext.TraceException(1051650, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
          }
          if (result != null)
          {
            try
            {
              if (result.ResultLevel == ActionTaskResultLevel.Success)
              {
                this.TraceNotification(requestContext, currentItem.Notification, "Action-Success");
                this.HandleNotificationSuccess(requestContext, currentItem.Notification, notifyConsumerStartUtc, notificationSubscriptionIdMap);
              }
              else if (result.ResultLevel == ActionTaskResultLevel.TransientError)
              {
                this.TraceNotification(requestContext, currentItem.Notification, "Action-TransientError");
                if (!currentItem.IsTest && attempt < 8)
                {
                  itemsForRetry.Add(currentItem.CreateNewAttempt());
                }
                else
                {
                  this.TraceNotification(requestContext, currentItem.Notification, "Action-TransientError-RetriesExceeded");
                  this.HandleNotificationFailure(requestContext, currentItem.Notification, notifyConsumerStartUtc, result, notificationSubscriptionIdMap);
                }
              }
              else
                this.HandleNotificationFailure(requestContext, currentItem.Notification, notifyConsumerStartUtc, result, notificationSubscriptionIdMap);
            }
            catch (Exception ex)
            {
              this.UpdateNotification(requestContext, currentItem.Notification.SubscriptionId, currentItem.Notification.Id, new NotificationStatus?(NotificationStatus.Completed), new NotificationResult?(NotificationResult.Failed), errorMessage: ServiceHooksResources.Error_CouldNotSendNotification, errorDetail: this.GetErrorDetailMessageWithActivityId(requestContext));
              requestContext.TraceException(1051650, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
            }
          }
        }
        result = (ActionTaskResult) null;
        currentItem = (ServiceHooksService.NotificationItem) null;
        queue = (Queue<ServiceHooksService.NotificationItem>) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051650, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
      }
      finally
      {
        if (itemsForRetry.Count > 0)
        {
          ++attempt;
          TimeSpan delay = TimeSpan.Zero;
          if (!doNotSleepBetweenTransientRetries)
            delay = BackoffTimerHelper.GetExponentialBackoff(attempt, ServiceHooksService.s_transientFailureMinBackoff, ServiceHooksService.s_transientFailureMaxBackoff, ServiceHooksService.s_transientFailureDeltaBackoff);
          requestContext.Schedule((Func<IVssRequestContext, Task>) (forkedContext => this.SendNotificationsAsync(forkedContext, (IEnumerable<ServiceHooksService.NotificationItem>) itemsForRetry, notificationSubscriptionIdMap, attempt, doNotSleepBetweenTransientRetries)), delay, "ServiceHooksService.SendNotificationsAsync");
        }
        requestContext.TraceLeave(1051660, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (SendNotificationsAsync));
      }
    }

    public void CleanupNotificationDetails(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051520, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CleanupNotificationDetails));
      try
      {
        IServiceHooksComponent serviceHooksComponent;
        using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          serviceHooksComponent.CleanupNotificationDetails(7);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051530, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051540, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CleanupNotificationDetails));
      }
    }

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification GetNotification(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      int notificationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051370, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetNotification));
      ArgumentUtility.CheckForOutOfRange(notificationId, nameof (notificationId), 1);
      try
      {
        Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification = (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification) null;
        using (IServiceHooksComponent serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          notification = serviceHooksComponent.ReadNotification(subscriptionId, notificationId);
        return notification != null ? notification : throw new NotificationNotFoundException(notificationId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051380, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051390, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetNotification));
      }
    }

    public IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> GetNotifications(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      NotificationStatus? status,
      NotificationResult? result,
      int? maxResults)
    {
      return this.QueryNotifications(requestContext, new Guid?(subscriptionId), status, result, new DateTime?(), new DateTime?(), maxResults);
    }

    public IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> QueryNotifications(
      IVssRequestContext requestContext,
      Guid? subscriptionId,
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate,
      int? maxResults)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051400, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (QueryNotifications));
      try
      {
        if (subscriptionId.HasValue)
          ArgumentUtility.CheckForEmptyGuid(subscriptionId.Value, nameof (subscriptionId));
        if (minDate.HasValue && maxDate.HasValue)
        {
          DateTime? nullable1 = minDate;
          DateTime? nullable2 = maxDate;
          if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0)
          {
            nullable2 = minDate;
            DateTime? nullable3 = maxDate;
            if ((nullable2.HasValue & nullable3.HasValue ? (nullable2.GetValueOrDefault() > nullable3.GetValueOrDefault() ? 1 : 0) : 0) == 0)
              goto label_7;
          }
          throw new ArgumentException("minDate, maxDate");
        }
label_7:
        if (maxResults.HasValue)
          ArgumentUtility.CheckForOutOfRange(maxResults.Value, nameof (maxResults), 1);
        using (IServiceHooksComponent serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          return serviceHooksComponent.QueryNotifications(subscriptionId, status, result, minDate, maxDate, maxResults);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051410, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051420, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (QueryNotifications));
      }
    }

    public IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> QueryNotifications(
      IVssRequestContext requestContext,
      IEnumerable<Guid> subscriptionIds,
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate,
      int? maxResults,
      int? maxResultsPerSubscription)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051430, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (QueryNotifications));
      try
      {
        if (maxResults.HasValue)
          ArgumentUtility.CheckForOutOfRange(maxResults.Value, nameof (maxResults), 1);
        if (maxResultsPerSubscription.HasValue)
          ArgumentUtility.CheckForOutOfRange(maxResultsPerSubscription.Value, nameof (maxResultsPerSubscription), 1);
        using (IServiceHooksComponent serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          return serviceHooksComponent.QueryNotifications(subscriptionIds, status, result, minDate, maxDate, maxResults, maxResultsPerSubscription);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051440, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051450, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (QueryNotifications));
      }
    }

    public IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> QueryNotificationsWithDetails(
      IVssRequestContext requestContext,
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate,
      int? maxResults)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051465, ServiceHooksService.s_area, ServiceHooksService.s_layer, "QueryNotificationDetails");
      try
      {
        if (maxResults.HasValue)
          ArgumentUtility.CheckForOutOfRange(maxResults.Value, nameof (maxResults), 1);
        if (minDate.HasValue && maxDate.HasValue)
        {
          DateTime? nullable1 = minDate;
          DateTime? nullable2 = maxDate;
          if ((nullable1.HasValue & nullable2.HasValue ? (nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
            throw new ArgumentException("minDate and maxDate are equal or minDate is greater than maxDate.");
        }
        using (IServiceHooksComponent serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          return serviceHooksComponent.QueryNotificationsWithDetails(status, result, minDate, maxDate, maxResults);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051475, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051485, ServiceHooksService.s_area, ServiceHooksService.s_layer, "QueryNotificationDetails");
      }
    }

    public ConsumerImplementation GetConsumerImplementation(
      IVssRequestContext requestContext,
      string consumerId)
    {
      return this.GetConsumerService(requestContext).GetConsumer(requestContext, consumerId);
    }

    public IEnumerable<ConsumerImplementation> GetConsumerImplementations(
      IVssRequestContext requestContext)
    {
      return this.GetConsumerService(requestContext).GetConsumers(requestContext);
    }

    private void EnsureNotificationQueryObjects(
      IVssRequestContext requestContext,
      NotificationsQuery query)
    {
      requestContext.TraceEnter(1051460, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (EnsureNotificationQueryObjects));
      if (query.Summary == null)
        query.Summary = (IList<NotificationSummary>) new List<NotificationSummary>();
      if (query.Results == null)
        query.Results = (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>();
      requestContext.TraceLeave(1051460, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (EnsureNotificationQueryObjects));
    }

    public NotificationsQuery QueryNotifications(
      IVssRequestContext requestContext,
      NotificationsQuery query)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051460, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (QueryNotifications));
      ArgumentUtility.CheckForNull<NotificationsQuery>(query, nameof (query));
      try
      {
        this.EnsureNotificationQueryObjects(requestContext, query);
        query.AssociatedSubscriptions = this.GetAssociatedSubscriptions(requestContext, query.SubscriptionIds);
        if (query.AssociatedSubscriptions == null || query.AssociatedSubscriptions.Count == 0)
        {
          requestContext.Trace(1051460, TraceLevel.Verbose, ServiceHooksService.s_area, ServiceHooksService.s_layer, "QueryNotification3: returning no results since there are no associated subscriptions.");
          return query;
        }
        query.Summary = this.GetNotificationSummary(requestContext, query.AssociatedSubscriptions, query.Status, query.ResultType, query.MinCreatedDate, query.MaxCreatedDate);
        bool? includeDetails = query.IncludeDetails;
        if (includeDetails.HasValue)
        {
          includeDetails = query.IncludeDetails;
          if (!includeDetails.Value)
            goto label_6;
        }
        query.Results = this.QueryNotifications(requestContext, query.AssociatedSubscriptions.Select<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, Guid>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, Guid>) (s => s.Id)), query.Status, query.ResultType, query.MinCreatedDate, query.MaxCreatedDate, query.MaxResults, query.MaxResultsPerSubscription);
label_6:
        if (query.Results == null || query.Results.Count == 0)
        {
          query.AssociatedSubscriptions.Clear();
        }
        else
        {
          HashSet<Guid> guidSet = new HashSet<Guid>(query.Results.Select<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification, Guid>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification, Guid>) (s => s.SubscriptionId)));
          List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptionList = new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
          foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription associatedSubscription in (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) query.AssociatedSubscriptions)
          {
            if (!guidSet.Contains(associatedSubscription.Id))
              subscriptionList.Add(associatedSubscription);
          }
          subscriptionList.ForEach((Action<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) (deleteThisSubscription => query.AssociatedSubscriptions.Remove(deleteThisSubscription)));
        }
        return query;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051460, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051460, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (QueryNotifications));
      }
    }

    internal IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> GetAssociatedSubscriptions(
      IVssRequestContext requestContext,
      IList<Guid> subscriptionIds,
      string publisherId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051811, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetAssociatedSubscriptions));
      try
      {
        IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> associatedSubscriptions;
        if (subscriptionIds == null || subscriptionIds.Count == 0)
        {
          SubscriptionsQuery query = new SubscriptionsQuery();
          associatedSubscriptions = (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) this.QuerySubscriptions(requestContext, query).ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
        }
        else
        {
          requestContext.Trace(1051811, TraceLevel.Verbose, ServiceHooksService.s_area, ServiceHooksService.s_layer, string.Format("GetAssociatedSubscription calling GetSubscriptions with {0} subscriptionIds.", (object) subscriptionIds.Count));
          associatedSubscriptions = this.GetSubscriptions(requestContext, (IEnumerable<Guid>) subscriptionIds);
        }
        if (!string.IsNullOrEmpty(publisherId))
        {
          requestContext.Trace(1051811, TraceLevel.Verbose, ServiceHooksService.s_area, ServiceHooksService.s_layer, string.Format("Starting removal of subscriptions which do not match the publisherId: {0}", (object) publisherId));
          List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptionList = new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
          foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) associatedSubscriptions)
          {
            if (subscription.PublisherId != publisherId)
            {
              requestContext.Trace(1051811, TraceLevel.Verbose, ServiceHooksService.s_area, ServiceHooksService.s_layer, string.Format("Adding subscriptionId '{0}' to list of subscriptions to remove since it has a publisherId of '{1}'", (object) subscription.Id, (object) subscription.PublisherId));
              subscriptionList.Add(subscription);
            }
          }
          requestContext.Trace(1051811, TraceLevel.Verbose, ServiceHooksService.s_area, ServiceHooksService.s_layer, string.Format("Found {0} subscriptions to remove.", (object) subscriptionList.Count));
          foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in subscriptionList)
            associatedSubscriptions.Remove(subscription);
        }
        requestContext.Trace(1051811, TraceLevel.Verbose, ServiceHooksService.s_area, ServiceHooksService.s_layer, string.Format("Returning {0} subscriptions", (object) associatedSubscriptions.Count));
        return associatedSubscriptions;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051811, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051811, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetAssociatedSubscriptions));
      }
    }

    private IList<NotificationSummary> GetNotificationSummary(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions,
      NotificationStatus? status = null,
      NotificationResult? result = null,
      DateTime? minCreatedDate = null,
      DateTime? maxCreatedDate = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051820, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetNotificationSummary));
      try
      {
        List<NotificationSummary> notificationSummary = new List<NotificationSummary>();
        if (subscriptions == null || subscriptions.Count == 0)
          return (IList<NotificationSummary>) notificationSummary;
        using (IServiceHooksComponent serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
        {
          foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptions)
          {
            IList<NotificationResultsSummaryDetail> resultsSummaryDetailList = serviceHooksComponent.QueryNotificationSummary(subscription.Id, status, result, minCreatedDate, maxCreatedDate);
            if (resultsSummaryDetailList != null && resultsSummaryDetailList.Count > 0)
              notificationSummary.Add(new NotificationSummary()
              {
                SubscriptionId = subscription.Id,
                Results = resultsSummaryDetailList
              });
          }
        }
        return (IList<NotificationSummary>) notificationSummary;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051830, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051840, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetNotificationSummary));
      }
    }

    public void UpdateNotification(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      int notificationId,
      NotificationStatus? status = null,
      NotificationResult? result = null,
      string request = null,
      string response = null,
      string errorMessage = null,
      string errorDetail = null,
      DateTime? queuedDate = null,
      DateTime? dequeuedDate = null,
      DateTime? processedDate = null,
      DateTime? completedDate = null,
      double? requestDuration = null,
      bool incrementRequestAttempts = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051490, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (UpdateNotification));
      try
      {
        ArgumentUtility.CheckForOutOfRange(notificationId, nameof (notificationId), 1);
        IServiceHooksComponent serviceHooksComponent;
        using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          serviceHooksComponent.UpdateNotification(subscriptionId, notificationId, status, result, request, response, errorMessage, errorDetail, queuedDate, dequeuedDate, processedDate, completedDate, requestDuration, incrementRequestAttempts);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051500, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051510, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (UpdateNotification));
      }
    }

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification SendTestNotification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification)
    {
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription = (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription) null;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051670, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (SendTestNotification));
      try
      {
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>(notification, nameof (notification));
        ArgumentUtility.CheckForNull<NotificationDetails>(notification.Details, "notification.Details");
        ArgumentUtility.CheckForNull<Event>(notification.Details.Event, "notification.Details.Event");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(notification.Details.ConsumerActionId, "notification.Details.ConsumerActionId");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(notification.Details.ConsumerId, "notification.Details.ConsumerId");
        ArgumentUtility.CheckForNull<FormattedEventMessage>(notification.Details.Event.DetailedMessage, "notification.Details.Event.DetailedMessage");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(notification.Details.Event.DetailedMessage.Text, "notification.Details.Event.DetailedMessage.Text");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(notification.Details.Event.EventType, "notification.Details.Event.EventType");
        ArgumentUtility.CheckForNull<FormattedEventMessage>(notification.Details.Event.Message, "notification.Details.Event.Message");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(notification.Details.Event.Message.Text, "notification.Details.Event.Message.Text");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(notification.Details.Event.PublisherId, "notification.Details.Event.PublisherId");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(notification.Details.EventType, "notification.Details.EventType");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(notification.Details.PublisherId, "notification.Details.PublisherId");
        ArgumentUtility.CheckForNull<IDictionary<string, string>>(notification.Details.ConsumerInputs, "notification.Details.ConsumerInputs");
        if (notification.Details.EventType != notification.Details.Event.EventType)
          throw new ArgumentException("notification.Details.EventType != notification.Details.Event.EventType");
        if (notification.Details.PublisherId != notification.Details.Event.PublisherId)
          throw new ArgumentException("notification.Details.PublisherId != notification.Details.Event.PublisherId");
        if (notification.SubscriptionId != Guid.Empty)
        {
          using (IServiceHooksComponent serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
            subscription = serviceHooksComponent.ReadSubscription(notification.SubscriptionId);
          if (subscription == null)
            throw new ServiceHookException(string.Format(ServiceHooksResources.Error_MissingSubscriptionBySpecifiedIdTemplate, (object) notification.SubscriptionId));
          this.ValidateUpdatedConfidentialInputs(requestContext, notification.Details.ConsumerId, notification.Details.ConsumerActionId, notification.Details.ConsumerInputs, subscription.ConsumerInputs);
        }
        NotificationDetails details = notification.Details;
        List<Tuple<string, string, SubscriptionInputScope>> tupleList = (List<Tuple<string, string, SubscriptionInputScope>>) null;
        Event @event = notification.Details.Event;
        Dictionary<string, InputDescriptor> confidentialInputDescsById = SubscriptionInputValueStrongBoxHelper.GetConfidentialInputDescriptors(requestContext, details.ConsumerId, details.ConsumerActionId).ToDictionary<InputDescriptor, string>((Func<InputDescriptor, string>) (i => i.Id));
        if (@event.Id == Guid.Empty)
          @event.Id = Guid.NewGuid();
        @event.CreatedDate = DateTime.UtcNow;
        if (confidentialInputDescsById.Count != 0)
        {
          tupleList = new List<Tuple<string, string, SubscriptionInputScope>>();
          tupleList.AddRange(details.ConsumerInputs.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (i => confidentialInputDescsById.ContainsKey(i.Key) && !SecurityHelper.IsMasked(i.Value))).Select<KeyValuePair<string, string>, Tuple<string, string, SubscriptionInputScope>>((Func<KeyValuePair<string, string>, Tuple<string, string, SubscriptionInputScope>>) (pair => new Tuple<string, string, SubscriptionInputScope>(pair.Key, pair.Value, SubscriptionInputScope.Consumer))));
          if (details.PublisherInputs != null)
            tupleList.AddRange(details.PublisherInputs.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (i => confidentialInputDescsById.ContainsKey(i.Key) && !SecurityHelper.IsMasked(i.Value))).Select<KeyValuePair<string, string>, Tuple<string, string, SubscriptionInputScope>>((Func<KeyValuePair<string, string>, Tuple<string, string, SubscriptionInputScope>>) (pair => new Tuple<string, string, SubscriptionInputScope>(pair.Key, pair.Value, SubscriptionInputScope.Consumer))));
        }
        Consumer consumer = this.GetConsumer(requestContext, details.ConsumerId);
        foreach (KeyValuePair<string, string> consumerInput in (IEnumerable<KeyValuePair<string, string>>) details.ConsumerInputs)
        {
          if (!SecurityHelper.IsMasked(consumerInput.Value))
            this.ValidateConsumerInput(requestContext, consumerInput.Key, consumerInput.Value, consumer, details.ConsumerActionId);
        }
        TestNotification testNotification1 = new TestNotification();
        testNotification1.TesterUserId = notification.SubscriberId;
        testNotification1.SubscriptionId = notification.SubscriptionId;
        testNotification1.SubscriberId = subscription == null ? notification.SubscriberId : Guid.Parse((subscription.ModifiedBy ?? subscription.CreatedBy).Id);
        testNotification1.EventId = @event.Id;
        testNotification1.Status = NotificationStatus.Queued;
        testNotification1.Result = NotificationResult.Pending;
        NotificationDetailsInternal notificationDetailsInternal = new NotificationDetailsInternal();
        notificationDetailsInternal.EventType = @event.EventType;
        notificationDetailsInternal.Event = @event;
        notificationDetailsInternal.PublisherId = details.PublisherId;
        notificationDetailsInternal.ConsumerId = details.ConsumerId;
        notificationDetailsInternal.ConsumerActionId = details.ConsumerActionId;
        notificationDetailsInternal.ConsumerInputs = (IDictionary<string, string>) new List<KeyValuePair<string, string>>(details.ConsumerInputs.Cast<KeyValuePair<string, string>>().Select<KeyValuePair<string, string>, KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, KeyValuePair<string, string>>) (pair => !confidentialInputDescsById.ContainsKey(pair.Key) ? pair : new KeyValuePair<string, string>(pair.Key, SecurityHelper.GetMaskedValue(pair.Value))))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (i => i.Key), (Func<KeyValuePair<string, string>, string>) (i => i.Value));
        notificationDetailsInternal.PublisherInputs = details.PublisherInputs == null ? (IDictionary<string, string>) null : (IDictionary<string, string>) new List<KeyValuePair<string, string>>(details.PublisherInputs.Cast<KeyValuePair<string, string>>().Select<KeyValuePair<string, string>, KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, KeyValuePair<string, string>>) (pair => !confidentialInputDescsById.ContainsKey(pair.Key) ? pair : new KeyValuePair<string, string>(pair.Key, SecurityHelper.GetMaskedValue(pair.Value))))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (i => i.Key), (Func<KeyValuePair<string, string>, string>) (i => i.Value));
        notificationDetailsInternal.QueuedDate = new DateTime?(DateTime.UtcNow);
        notificationDetailsInternal.NotificationData = this.GetSampleNotificationData(requestContext, details.EventType);
        testNotification1.Details = (NotificationDetails) notificationDetailsInternal;
        testNotification1.OverriddenConfidentialInputs = tupleList;
        TestNotification testNotification2 = testNotification1;
        using (IServiceHooksComponent serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          serviceHooksComponent.CreateNotification((Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification) testNotification2, false, out int _);
        this.DeliverNotifications(requestContext, (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) new TestNotification[1]
        {
          testNotification2
        });
        return (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification) testNotification2;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051680, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051690, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (SendTestNotification));
      }
    }

    public void SynchronizeDashboard(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051550, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (SynchronizeDashboard));
      try
      {
        ServiceHooksStats stats;
        using (ServiceHooksComponent component = requestContext.CreateComponent<ServiceHooksComponent>())
          stats = component.GetStats();
        IEnumerable<ConsumerImplementation> consumerImplementations = this.GetConsumerImplementations(requestContext);
        stats.ConsumerCount = consumerImplementations.Count<ConsumerImplementation>();
        stats.ConsumerActionCount = consumerImplementations.SelectMany<ConsumerImplementation, ConsumerActionImplementation>((Func<ConsumerImplementation, IEnumerable<ConsumerActionImplementation>>) (ci => (IEnumerable<ConsumerActionImplementation>) ci.Actions)).Count<ConsumerActionImplementation>();
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerCount, (double) stats.ConsumerCount);
        properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerActionCount, (double) stats.ConsumerActionCount);
        properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEnabledSubscriptionCount, (double) stats.EnabledSubscriptionCount);
        properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyDisabledSubscriptionCount, (double) stats.DisabledSubscriptionCount);
        this.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ServiceHooksCustomerIntelligenceInfo.FeatureSynchronizeDashboard, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051560, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051570, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (SynchronizeDashboard));
      }
    }

    protected virtual IServiceHooksComponent CreateServiceHooksComponent(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051850, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CreateServiceHooksComponent));
      try
      {
        return (IServiceHooksComponent) requestContext.CreateComponent<ServiceHooksComponent>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051860, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051870, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CreateServiceHooksComponent));
      }
    }

    private bool ValidateEvent(IVssRequestContext requestContext, PublisherEvent @event)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051871, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (ValidateEvent));
      bool flag = true;
      try
      {
        ArgumentUtility.CheckForNull<PublisherEvent>(@event, nameof (@event));
        ArgumentUtility.CheckForNull<Event>(@event.Event, "event.Event");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(@event.Event.PublisherId, "event.Event.PublisherId");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(@event.Event.EventType, "event.Event.EventType");
        if (!(@event.Event.PublisherId == "tfs"))
        {
          if (!(@event.Event.PublisherId == "rm"))
            goto label_9;
        }
        if (@event.Event.ResourceContainers == null)
          requestContext.Trace(0, TraceLevel.Error, ServiceHooksService.s_area, ServiceHooksService.s_layer, "Service Hooks PublishEvent received event with no resource container for notification '" + @event.GetDiagnostic("NotificationId") + "', eventType '" + @event.Event.EventType + "'");
        else if (@event.Event.ResourceContainers.Count == 0)
          requestContext.Trace(0, TraceLevel.Error, ServiceHooksService.s_area, ServiceHooksService.s_layer, "Service Hooks PublishEvent received event with empty resource container for notification '" + @event.GetDiagnostic("NotificationId") + "', eventType '" + @event.Event.EventType + "'");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051872, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        flag = false;
      }
      finally
      {
        requestContext.TraceLeave(1051873, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (ValidateEvent));
      }
label_9:
      return flag;
    }

    internal void ValidateSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      bool isBeingCreated)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051874, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (ValidateSubscription));
      ServiceHooksTimer serviceHooksTimer = ServiceHooksTimer.StartNew();
      try
      {
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>(subscription, nameof (subscription));
        subscription.Validate(isBeingCreated);
        if (subscription.ConsumerInputs == null)
          subscription.ConsumerInputs = (IDictionary<string, string>) new Dictionary<string, string>(0);
        if (subscription.PublisherInputs == null)
          subscription.PublisherInputs = (IDictionary<string, string>) new Dictionary<string, string>(0);
        serviceHooksTimer.RecordTick();
        ConsumerAction consumerAction = (ConsumerAction) null;
        Consumer consumer = this.GetConsumer(requestContext, subscription.ConsumerId);
        serviceHooksTimer.RecordTick();
        if (consumer.Actions != null)
          consumerAction = consumer.Actions.FirstOrDefault<ConsumerAction>((Func<ConsumerAction, bool>) (a => string.Equals(a.Id, subscription.ConsumerActionId)));
        if (consumerAction == null)
          throw new ConsumerActionNotFoundException(consumer.Id, subscription.ConsumerActionId);
        string inputValue;
        if (subscription.PublisherInputs.TryGetValue("projectId", out inputValue))
          new InputDescriptor()
          {
            Id = "projectId",
            Validation = new InputValidation()
            {
              DataType = InputDataType.Guid,
              IsRequired = true
            }
          }.Validate(inputValue);
        serviceHooksTimer.RecordTick();
        foreach (KeyValuePair<string, string> publisherInput in (IEnumerable<KeyValuePair<string, string>>) subscription.PublisherInputs)
          this.ValidatePublisherInput(requestContext, publisherInput.Key, publisherInput.Value);
        serviceHooksTimer.RecordTick();
        foreach (KeyValuePair<string, string> consumerInput in (IEnumerable<KeyValuePair<string, string>>) subscription.ConsumerInputs)
        {
          if (isBeingCreated || !SecurityHelper.IsMasked(consumerInput.Value))
            this.ValidateConsumerInput(requestContext, consumerInput.Key, consumerInput.Value, consumer, subscription.ConsumerActionId);
        }
        serviceHooksTimer.RecordTick();
        if (consumer.InputDescriptors != null)
        {
          foreach (InputDescriptor inputDescriptor in consumer.InputDescriptors.Where<InputDescriptor>((Func<InputDescriptor, bool>) (des => des.Validation != null && des.Validation.IsRequired)))
          {
            if (!subscription.ConsumerInputs.ContainsKey(inputDescriptor.Id))
              throw new SubscriptionInputException(string.Format(ServiceHooksResources.Error_MissingRequiredConsumerInputFormat, (object) inputDescriptor.Id, (object) inputDescriptor.Name, (object) consumer.Name, (object) consumerAction.Name));
          }
        }
        serviceHooksTimer.RecordTick();
        if (consumerAction.InputDescriptors != null)
        {
          foreach (InputDescriptor inputDescriptor in consumerAction.InputDescriptors.Where<InputDescriptor>((Func<InputDescriptor, bool>) (des => des.Validation != null && des.Validation.IsRequired)))
          {
            if (!subscription.ConsumerInputs.ContainsKey(inputDescriptor.Id))
              throw new SubscriptionInputException(string.Format(ServiceHooksResources.Error_MissingRequiredConsumerInputFormat, (object) inputDescriptor.Id, (object) inputDescriptor.Name, (object) consumer.Name, (object) consumerAction.Name));
          }
        }
        serviceHooksTimer.RecordTick();
        this.GetConsumerService(requestContext).GetConsumerAction(requestContext, subscription.ConsumerId, subscription.ConsumerActionId).ValidateConsumerInputs(requestContext, subscription.ConsumerInputs);
        serviceHooksTimer.RecordTick();
        if (subscription.ResourceVersion == null)
          return;
        if (!consumerAction.AllowResourceVersionOverride)
          throw new SubscriptionInputException(string.Format(ServiceHooksResources.Error_UnexpectedResourceVersionFormat, (object) subscription.ResourceVersion));
        string[] strArray = (string[]) null;
        consumerAction.SupportedResourceVersions.TryGetValue(subscription.EventType, out strArray);
        bool flag = false;
        if (strArray == null)
        {
          flag = true;
        }
        else
        {
          foreach (string b in strArray)
          {
            if (string.Equals(subscription.ResourceVersion, b, StringComparison.OrdinalIgnoreCase))
            {
              flag = true;
              break;
            }
          }
        }
        if (!string.IsNullOrWhiteSpace(subscription.ResourceVersion) && !string.Equals("latest", subscription.ResourceVersion) && !flag)
          throw new SubscriptionInputException(string.Format(ServiceHooksResources.Error_UnsupportedResourceVersionFormat, (object) subscription.ResourceVersion));
        serviceHooksTimer.RecordTick();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051875, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        serviceHooksTimer.Stop();
        if (serviceHooksTimer.Millis > 10000L || requestContext.IsTracing(1051908, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer))
          requestContext.TraceAlways(1051908, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, "Subscription validation timing: " + serviceHooksTimer.Millis.ToString("D") + "ms " + serviceHooksTimer.Percents);
        requestContext.TraceLeave(1051876, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (ValidateSubscription));
      }
    }

    private void ValidatePublisherInput(
      IVssRequestContext requestContext,
      string inputId,
      string inputValue)
    {
      if (requestContext.IsFeatureEnabled("ServiceHooks.Subscription.ValidatePublisherInputs") && inputValue == null)
        throw new SubscriptionInputException(string.Format(ServiceHooksResources.Error_PublisherInputValueCannotBeNull, (object) inputId));
    }

    private void ValidateConsumerInput(
      IVssRequestContext requestContext,
      string inputId,
      string inputValue,
      Consumer consumer,
      string consumerActionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051877, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (ValidateConsumerInput));
      ServiceHooksTimer serviceHooksTimer = ServiceHooksTimer.StartNew();
      bool flag = true;
      try
      {
        if (string.IsNullOrWhiteSpace(inputId))
          throw new SubscriptionInputException(string.Format(ServiceHooksResources.Error_MissingSubscriptionInputElementTemplate, (object) ServiceHooksResources.InputIdentifierUnknown, (object) "ConsumerInput.Id"));
        if (string.IsNullOrWhiteSpace(inputValue))
          throw new SubscriptionInputException(string.Format(ServiceHooksResources.Error_MissingSubscriptionInputElementTemplate, (object) ServiceHooksResources.InputValueMissing, (object) "ConsumerInput.Value"));
        if (SecurityHelper.IsMasked(inputValue))
          throw new SubscriptionInputException(string.Format(ServiceHooksResources.Error_MaskedConsumerInputValueFormat, (object) inputId, (object) consumer.Id, (object) consumerActionId));
        serviceHooksTimer.RecordTick();
        InputDescriptor inputDescriptor = consumer.GetInputDescriptor(consumerActionId, inputId);
        if (inputDescriptor == null)
          throw new SubscriptionInputException(string.Format(ServiceHooksResources.Error_UnknownConsumerInputFormat, (object) inputId, (object) consumer.Id, (object) consumerActionId));
        serviceHooksTimer.RecordTick();
        flag = inputDescriptor.IsConfidential;
        inputDescriptor.ValidateInternal(requestContext, inputValue);
        serviceHooksTimer.RecordTick();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051878, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        serviceHooksTimer.Stop();
        if (serviceHooksTimer.Millis > 10000L || requestContext.IsTracing(1051908, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer))
        {
          string str = flag ? SecurityHelper.GetMaskedValue(inputValue) : inputValue;
          requestContext.TraceAlways(1051908, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, "Validate consumer input (" + inputId + "=" + str + "): " + serviceHooksTimer.Millis.ToString("D") + "ms " + serviceHooksTimer.Percents);
        }
        requestContext.TraceLeave(1051879, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (ValidateConsumerInput));
      }
    }

    internal void CreateNotificationForFilteredEvent(
      IVssRequestContext requestContext,
      PublisherEvent publisherEvent)
    {
      requestContext.TraceEnter(1051905, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CreateNotificationForFilteredEvent));
      try
      {
        if (publisherEvent.Subscription == null)
          return;
        IServiceHooksComponent serviceHooksComponent;
        using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
        {
          if (serviceHooksComponent.ReadSubscription(publisherEvent.Subscription.Id) == null)
            return;
          Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification = new Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification()
          {
            Id = 0,
            CreatedDate = DateTime.MinValue,
            ModifiedDate = DateTime.MinValue,
            SubscriptionId = publisherEvent.Subscription.Id,
            SubscriberId = Guid.Parse((publisherEvent.Subscription.ModifiedBy ?? publisherEvent.Subscription.CreatedBy).Id),
            Status = NotificationStatus.Completed,
            Result = NotificationResult.Filtered,
            EventId = Guid.Empty,
            Details = new NotificationDetails()
            {
              EventType = publisherEvent.Subscription.EventType,
              Event = new Event(),
              PublisherId = publisherEvent.Subscription.PublisherId,
              ConsumerId = publisherEvent.Subscription.ConsumerId,
              ConsumerActionId = publisherEvent.Subscription.ConsumerActionId,
              ConsumerInputs = (IDictionary<string, string>) new Dictionary<string, string>(),
              PublisherInputs = (IDictionary<string, string>) new Dictionary<string, string>(),
              RequestAttempts = 0
            }
          };
          serviceHooksComponent.CreateNotification(notification, false, out int _);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051906, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051907, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CreateNotificationForFilteredEvent));
      }
    }

    internal IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> CreateNotificationsForPublishedEvent(
      IVssRequestContext requestContext,
      PublisherEvent publisherEvent,
      ServiceHooksTimer eventTimer = null,
      Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>> subscriptionsByEventType = null)
    {
      IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions1 = Enumerable.Empty<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051877, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CreateNotificationsForPublishedEvent));
      try
      {
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.IsFeatureEnabled("ServiceHooks.Notification.ReadSingleSubscription") && publisherEvent.Subscription != null)
        {
          IServiceHooksComponent serviceHooksComponent;
          using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          {
            Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription element = serviceHooksComponent.ReadSubscription(publisherEvent.Subscription.Id);
            if (element != null)
              subscriptions1 = subscriptions1.Append<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>(element);
          }
        }
        if (!subscriptions1.Any<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>())
        {
          if (subscriptionsByEventType == null || !subscriptionsByEventType.ContainsKey(publisherEvent.Event.EventType))
          {
            IServiceHooksComponent serviceHooksComponent;
            using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
              subscriptions1 = serviceHooksComponent.QuerySubscriptions(new Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus?(), publisherEvent.Event.PublisherId, publisherEvent.Event.EventType, (string) null, (string) null);
            subscriptionsByEventType?.Add(publisherEvent.Event.EventType, (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptions1.ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>());
          }
          else
            subscriptions1 = (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptionsByEventType[publisherEvent.Event.EventType].ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
        }
        eventTimer?.RecordTick();
        int num1 = subscriptions1.Count<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
        if (subscriptions1.Count<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>() != 0 && publisherEvent.PublisherInputFilters != null)
          subscriptions1 = subscriptions1.Where<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, bool>) (s => publisherEvent.PublisherInputFilters.Evaluate(s.PublisherInputs)));
        eventTimer?.RecordTick();
        IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions2 = (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) this.RemoveSubscriptionsOfInactiveConsumers(requestContext, subscriptions1);
        int num2 = subscriptions2.Count<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
        eventTimer?.RecordTick();
        if (num2 == 0)
        {
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyPublisherId, publisherEvent.Event.PublisherId);
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventType, publisherEvent.Event.EventType);
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyNotificationId, publisherEvent.GetDiagnostic("NotificationId"));
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeySubscriptionsBeforeFilter, num1.ToString("D"));
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeySubscriptionsAfterFilter, num2.ToString("D"));
          this.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ServiceHooksCustomerIntelligenceInfo.FeatureNoSubscriptionsForPublishedEvent, properties);
          return (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>(0);
        }
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          this.VerifyPublisherEventSubscription(requestContext, publisherEvent, subscriptions2);
        eventTimer?.RecordTick();
        DateTime utcNow = DateTime.UtcNow;
        List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> forPublishedEvent = new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>(subscriptions2.Count<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>());
        foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in subscriptions2)
        {
          bool flag = false;
          if (subscription.Status != Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.Enabled && subscription.Status != Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.OnProbation)
            flag = true;
          if (subscription.Status == Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.OnProbation)
          {
            TimeSpan exponentialBackoff = BackoffTimerHelper.GetExponentialBackoff((int) subscription.ProbationRetries + 1, ServiceHooksService.s_subscriptionProbationMinBackoff, ServiceHooksService.s_subscriptionProbationMaxBackoff, ServiceHooksService.s_subscriptionProbationDeltaBackoff);
            DateTime dateTime = requestContext.IsFeatureEnabled("ServiceHooks.Notification.UseLastProbationRetryDate") ? subscription.LastProbationRetryDate : subscription.ModifiedDate;
            if ((utcNow - dateTime).Duration() < exponentialBackoff && this.m_registryService.GetValue(requestContext, (RegistryQuery) "/Service/ServiceHooks/Settings/DoNotWaitBetweenProbationRetries", true, "false").ToLower() == "false")
            {
              flag = true;
            }
            else
            {
              bool incrementProbationRetries = subscription.ProbationRetries < byte.MaxValue;
              this.UpdateSubscriptionStatus(requestContext, subscription.Id, subscription.Status, false, incrementProbationRetries, false);
              if (subscriptionsByEventType != null & incrementProbationRetries)
                ++subscription.ProbationRetries;
            }
          }
          string resourceVersion = this.NegotiateResourceVersion(requestContext, publisherEvent, subscription);
          if (resourceVersion == null)
            flag = true;
          if (flag)
          {
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyPublisherId, publisherEvent.Event.PublisherId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeySubscriptionId, (object) subscription.Id);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyNotificationId, publisherEvent.GetDiagnostic("NotificationId"));
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventType, publisherEvent.Event.EventType);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerId, subscription.ConsumerId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerActionId, subscription.ConsumerActionId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyMessage, (double) subscription.Status);
            this.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ServiceHooksCustomerIntelligenceInfo.FeatureSubscriptionSkippedForPublishedEvent, properties);
          }
          else
          {
            SubscriptionInputValueStrongBoxHelper.AddConfidentialInputValuesToSubscription(requestContext, subscription, true);
            Event eventForVersion = publisherEvent.GetEventForVersion(resourceVersion);
            Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification1 = new Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification();
            notification1.Id = 0;
            notification1.CreatedDate = DateTime.MinValue;
            notification1.ModifiedDate = DateTime.MinValue;
            notification1.SubscriptionId = subscription.Id;
            notification1.SubscriberId = Guid.Parse((subscription.ModifiedBy ?? subscription.CreatedBy).Id);
            notification1.EventId = eventForVersion.Id;
            notification1.Status = NotificationStatus.Queued;
            notification1.Result = NotificationResult.Pending;
            NotificationDetailsInternal notificationDetailsInternal = new NotificationDetailsInternal();
            notificationDetailsInternal.EventType = eventForVersion.EventType;
            notificationDetailsInternal.Event = eventForVersion;
            notificationDetailsInternal.NotificationData = publisherEvent.NotificationData;
            notificationDetailsInternal.PublisherId = subscription.PublisherId;
            notificationDetailsInternal.ConsumerId = subscription.ConsumerId;
            notificationDetailsInternal.ConsumerActionId = subscription.ConsumerActionId;
            notificationDetailsInternal.ConsumerInputs = subscription.ConsumerInputs;
            notificationDetailsInternal.PublisherInputs = subscription.PublisherInputs;
            notificationDetailsInternal.QueuedDate = new DateTime?(utcNow);
            notification1.Details = (NotificationDetails) notificationDetailsInternal;
            Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification2 = notification1;
            bool allowFullContent = requestContext.IsFeatureEnabled("ServiceHooks.Notification.AllowFullContentLength");
            IServiceHooksComponent serviceHooksComponent;
            int payloadLength;
            using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
              serviceHooksComponent.CreateNotification(notification2, allowFullContent, out payloadLength);
            this.TraceNotification(requestContext, notification2, "Created");
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyPublisherId, notification2.Details.PublisherId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventType, notification2.Details.EventType);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeySubscriptionId, (object) subscription.Id);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyNotificationId, (double) notification2.Id);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventId, (object) notification2.EventId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerId, notification2.Details.ConsumerId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerActionId, notification2.Details.ConsumerActionId);
            properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyPayloadSize, (double) payloadLength);
            this.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ServiceHooksCustomerIntelligenceInfo.FeatureNotificationQueued, properties);
            forPublishedEvent.Add(notification2);
          }
        }
        eventTimer?.RecordTick();
        return (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) forPublishedEvent;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051878, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051879, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (CreateNotificationsForPublishedEvent));
      }
    }

    private async Task<ActionTaskResult> NotifyConsumerAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification)
    {
      ServiceHooksService serviceHooksService = this;
      requestContext.TraceEnter(1051250, ServiceHooksService.s_area, ServiceHooksService.s_layer, "NotifyConsumer");
      try
      {
        ActionTask taskForNotification;
        try
        {
          taskForNotification = serviceHooksService.GetConsumerService(requestContext).GetConsumerActionTaskForNotification(requestContext, notification);
        }
        catch (Exception ex)
        {
          // ISSUE: explicit non-virtual call
          __nonvirtual (serviceHooksService.UpdateNotification(requestContext, notification.SubscriptionId, notification.Id, new NotificationStatus?(NotificationStatus.Completed), new NotificationResult?(NotificationResult.Failed), (string) null, (string) null, ServiceHooksResources.Error_CouldNotSendNotification, serviceHooksService.GetErrorDetailMessageWithActivityId(requestContext, ex), new DateTime?(), new DateTime?(), new DateTime?(), new DateTime?(), new double?(), false));
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyPublisherId, notification.Details.PublisherId);
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventType, notification.Details.EventType);
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyNotificationId, (double) notification.Id);
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventId, (object) notification.EventId);
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerId, notification.Details.ConsumerId);
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerActionId, notification.Details.ConsumerActionId);
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyExceptionType, ex.GetType().FullName);
          serviceHooksService.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ServiceHooksCustomerIntelligenceInfo.FeatureNotificationEventHandlerFailure, properties);
          requestContext.TraceAlways(1051260, TraceLevel.Error, ServiceHooksService.s_area, ServiceHooksService.s_layer, string.Format("Caught Exception while obtaining Consumer Action Task for Subscription {0} and Notification Id {1} {2}", (object) notification.SubscriptionId, (object) notification.Id, (object) ex));
          return (ActionTaskResult) null;
        }
        return await serviceHooksService.GetConsumerService(requestContext).RunActionTaskAsync(requestContext, taskForNotification, notification, (INotificationTracer) serviceHooksService);
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(1051260, TraceLevel.Error, ServiceHooksService.s_area, ServiceHooksService.s_layer, string.Format("Caught Exception after obtaining Consumer Action Task for Subscription {0} and Notification Id {1} {2}", (object) notification.SubscriptionId, (object) notification.Id, (object) ex));
        return (ActionTaskResult) null;
      }
      finally
      {
        requestContext.TraceLeave(1051270, ServiceHooksService.s_area, ServiceHooksService.s_layer, "NotifyConsumer");
      }
    }

    internal void HandleNotificationSuccess(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      DateTime utcActionTaskRunStart,
      IReadOnlyDictionary<Guid, string> notificationSubscriptionIdMap = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051883, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (HandleNotificationSuccess));
      try
      {
        DateTime utcNow = DateTime.UtcNow;
        IVssRequestContext requestContext1 = requestContext;
        Guid subscriptionId = notification.SubscriptionId;
        int id = notification.Id;
        NotificationStatus? status = new NotificationStatus?(NotificationStatus.Completed);
        NotificationResult? result = new NotificationResult?(NotificationResult.Succeeded);
        DateTime? nullable = new DateTime?(utcNow);
        DateTime? queuedDate = new DateTime?();
        DateTime? dequeuedDate = new DateTime?();
        DateTime? processedDate = new DateTime?();
        DateTime? completedDate = nullable;
        double? requestDuration = new double?();
        this.UpdateNotification(requestContext1, subscriptionId, id, status, result, (string) null, (string) null, (string) null, (string) null, queuedDate, dequeuedDate, processedDate, completedDate, requestDuration, false);
        if (notification is TestNotification || notification.SubscriptionId == Guid.Empty)
          return;
        IServiceHooksComponent serviceHooksComponent;
        Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription;
        using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          subscription = serviceHooksComponent.ReadSubscription(notification.SubscriptionId);
        if (subscription == null)
          throw new ServiceHookException(string.Format(ServiceHooksResources.Error_MissingSubscriptionBySpecifiedIdTemplate, (object) notification.SubscriptionId));
        CustomerIntelligenceData properties1 = new CustomerIntelligenceData();
        properties1.Add(ServiceHooksCustomerIntelligenceInfo.KeyPublisherId, notification.Details.PublisherId);
        properties1.Add(ServiceHooksCustomerIntelligenceInfo.KeySubscriptionId, subscription.Id.ToString());
        properties1.Add(ServiceHooksCustomerIntelligenceInfo.KeyNotificationId, (double) notification.Id);
        properties1.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventId, (object) notification.EventId);
        properties1.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventType, notification.Details.EventType);
        properties1.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerId, notification.Details.ConsumerId);
        properties1.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerActionId, notification.Details.ConsumerActionId);
        properties1.Add(ServiceHooksCustomerIntelligenceInfo.KeySendDuration, (utcNow - utcActionTaskRunStart).TotalSeconds);
        properties1.Add(ServiceHooksCustomerIntelligenceInfo.KeyProbationRetries, (double) subscription.ProbationRetries);
        this.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ServiceHooksCustomerIntelligenceInfo.FeatureNotificationSendSuccess, properties1);
        if (subscription.Status != Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.OnProbation)
          return;
        this.UpdateSubscriptionStatus(requestContext, subscription.Id, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.Enabled, true, false, false);
        CustomerIntelligenceData properties2 = new CustomerIntelligenceData();
        properties2.Add(ServiceHooksCustomerIntelligenceInfo.KeyPublisherId, notification.Details.PublisherId);
        properties2.Add(ServiceHooksCustomerIntelligenceInfo.KeySubscriptionId, subscription.Id.ToString());
        properties2.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventType, notification.Details.EventType);
        properties2.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerId, notification.Details.ConsumerId);
        properties2.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerActionId, notification.Details.ConsumerActionId);
        properties2.Add(ServiceHooksCustomerIntelligenceInfo.KeySendDuration, (utcNow - utcActionTaskRunStart).TotalSeconds);
        properties2.Add(ServiceHooksCustomerIntelligenceInfo.KeyProbationRetries, (double) subscription.ProbationRetries);
        this.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ServiceHooksCustomerIntelligenceInfo.FeatureSubscriptionReenableBySystem, properties2);
        this.TryUpdateRemoteSubscriptionStatus(requestContext, notification, subscription, notificationSubscriptionIdMap, true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051884, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051885, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (HandleNotificationSuccess));
      }
    }

    internal void HandleNotificationFailure(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      DateTime utcActionTaskRunStart,
      ActionTaskResult actionResult,
      IReadOnlyDictionary<Guid, string> notificationSubscriptionIdMap = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051886, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (HandleNotificationFailure));
      try
      {
        DateTime utcNow = DateTime.UtcNow;
        string str1 = WebUtility.HtmlEncode(SecretUtility.ScrubSecrets(actionResult.ErrorMessage, false));
        string str2 = actionResult.Exception == null ? (string) null : SecretUtility.ScrubSecrets(actionResult.Exception.Message, false);
        IVssRequestContext requestContext1 = requestContext;
        Guid subscriptionId = notification.SubscriptionId;
        int id = notification.Id;
        NotificationStatus? status = new NotificationStatus?(NotificationStatus.Completed);
        NotificationResult? result = new NotificationResult?(NotificationResult.Failed);
        string errorMessage = str1;
        string errorDetail = str2;
        DateTime? nullable = new DateTime?(DateTime.UtcNow);
        DateTime? queuedDate = new DateTime?();
        DateTime? dequeuedDate = new DateTime?();
        DateTime? processedDate = new DateTime?();
        DateTime? completedDate = nullable;
        double? requestDuration = new double?();
        this.UpdateNotification(requestContext1, subscriptionId, id, status, result, (string) null, (string) null, errorMessage, errorDetail, queuedDate, dequeuedDate, processedDate, completedDate, requestDuration, false);
        if (notification is TestNotification || notification.SubscriptionId == Guid.Empty)
          return;
        IServiceHooksComponent serviceHooksComponent;
        Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription;
        using (serviceHooksComponent = this.CreateServiceHooksComponent(requestContext))
          subscription = serviceHooksComponent.ReadSubscription(notification.SubscriptionId);
        if (subscription == null)
          throw new ServiceHookException(string.Format(ServiceHooksResources.Error_MissingSubscriptionBySpecifiedIdTemplate, (object) notification.SubscriptionId));
        double totalSeconds = (utcNow - utcActionTaskRunStart).TotalSeconds;
        this.PublishCiFailedNotificationEvent(requestContext, notification, subscription, actionResult, ServiceHooksCustomerIntelligenceInfo.FeatureNotificationSendFailure, totalSeconds);
        if (actionResult.ResultLevel == ActionTaskResultLevel.EventPayloadError)
          return;
        if (actionResult.ResultLevel == ActionTaskResultLevel.TerminalFailure)
        {
          this.DisableSubscriptionBySytem(requestContext, notification, subscription, actionResult, totalSeconds, notificationSubscriptionIdMap);
        }
        else
        {
          if (ServiceHooksConsumerService.IsFirstPartyHighlyAvailableConsumer(subscription.ConsumerId))
            return;
          if (subscription.Status == Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.OnProbation)
          {
            if (subscription.ProbationRetries < (byte) 7)
              return;
            this.DisableSubscriptionBySytem(requestContext, notification, subscription, actionResult, totalSeconds, notificationSubscriptionIdMap);
          }
          else if (requestContext.IsFeatureEnabled("ServiceHooks.Notification.PreventOnProbationOverwrite") && subscription.Status != Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.Enabled)
            requestContext.TraceAlways(1051928, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, string.Format("Preventing subscription {0} from being set on probation since its current status is {1}", (object) subscription.Id, (object) subscription.Status));
          else
            this.PlaceSubscriptionOnProbation(requestContext, notification, subscription, actionResult, totalSeconds);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051887, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051888, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (HandleNotificationFailure));
      }
    }

    private void PublishCiFailedNotificationEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      ActionTaskResult actionResult,
      string ciFeature,
      double sendDurationSeconds)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyPublisherId, notification.Details.PublisherId);
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeySubscriptionId, subscription.Id.ToString());
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyNotificationId, (double) notification.Id);
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventId, (object) notification.EventId);
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventType, notification.Details.EventType);
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerId, notification.Details.ConsumerId);
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerActionId, notification.Details.ConsumerActionId);
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeySendDuration, sendDurationSeconds);
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyProbationRetries, (double) subscription.ProbationRetries);
      Exception exception = actionResult.Exception;
      if (exception != null)
      {
        properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyExceptionType, exception.GetType().FullName);
        if (exception.InnerException != null)
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyInnerExceptionType, exception.InnerException.GetType().FullName);
        IEnumerable<KeyValuePair<string, string>> source = notification.Details.PublisherInputs.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (input => ServiceHooksService.s_publisherInputKeysToLog.Contains(input.Key)));
        if (source.Any<KeyValuePair<string, string>>())
          properties.Add(ServiceHooksCustomerIntelligenceInfo.KeySubscriptionMetadata, (object) source.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (input => input.Key), (Func<KeyValuePair<string, string>, string>) (input => input.Value)));
      }
      this.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ciFeature, properties);
      if (actionResult.ErrorMessage == null)
        return;
      requestContext.Trace(1051887, TraceLevel.Error, ServiceHooksService.s_area, ServiceHooksService.s_layer, "Service Hooks failed notification message: " + actionResult.ErrorMessage);
    }

    private void PublishCreateUpdateSubscriptionEvent(
      IVssRequestContext requestContext,
      string ciFeature,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeySubscriptionId, (object) subscription.Id);
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyPublisherId, subscription.PublisherId);
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyEventType, subscription.EventType);
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerId, subscription.ConsumerId);
      properties.Add(ServiceHooksCustomerIntelligenceInfo.KeyConsumerActionId, subscription.ConsumerActionId);
      this.m_ciService.Publish(requestContext, ServiceHooksCustomerIntelligenceInfo.Area, ciFeature, properties);
    }

    private string NegotiateResourceVersion(
      IVssRequestContext requestContext,
      PublisherEvent publisherEvent,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051885, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (NegotiateResourceVersion));
      try
      {
        if (subscription.ResourceVersion != null)
          return subscription.ResourceVersion;
        ConsumerActionImplementation consumerAction = this.GetConsumerService(requestContext).GetConsumerAction(requestContext, subscription.ConsumerId, subscription.ConsumerActionId);
        string[] versions2;
        return consumerAction.SupportedResourceVersions == null || !consumerAction.SupportedResourceVersions.TryGetValue(subscription.EventType, out versions2) ? "" : ResourceVersionComparer.Intersect((IEnumerable<string>) publisherEvent.GetOrderedResourceVersions(), (IEnumerable<string>) versions2).FirstOrDefault<string>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051896, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051897, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (NegotiateResourceVersion));
      }
    }

    public ServiceHooksConsumerService GetConsumerService(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).GetService<ServiceHooksConsumerService>();

    private IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> RemoveSubscriptionsOfInactiveConsumers(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions)
    {
      HashSet<string> activeConsumerIds = new HashSet<string>();
      foreach (ConsumerImplementation consumer in this.GetConsumerService(requestContext).GetConsumers(requestContext))
        activeConsumerIds.Add(consumer.Id);
      return (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptions.Where<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, bool>) (s => activeConsumerIds.Contains(s.ConsumerId))).ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
    }

    private Guid GetRequestorIdentityId(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1051901, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetRequestorIdentityId));
      try
      {
        return requestContext.GetUserId(true);
      }
      catch
      {
        return Guid.Empty;
      }
      finally
      {
        requestContext.TraceLeave(1051903, ServiceHooksService.s_area, ServiceHooksService.s_layer, nameof (GetRequestorIdentityId));
      }
    }

    public virtual bool IsOnPremisesDeployment(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment;

    private string GetErrorDetailMessageWithActivityId(
      IVssRequestContext requestContext,
      Exception ex = null)
    {
      if (ex == null)
        return string.Format(ServiceHooksResources.Error_DetailActivityIdFormat, (object) requestContext.ActivityId);
      while (ex is TargetInvocationException && ex.InnerException != null)
        ex = ex.InnerException;
      return string.Format(ServiceHooksResources.Error_DetailExPlusActivityIdFormat, (object) SecretUtility.ScrubSecrets(ex.Message, false), (object) requestContext.ActivityId);
    }

    private void PopulateSubscriptionIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions)
    {
      Dictionary<Guid, IdentityRef> dictionary = new Dictionary<Guid, IdentityRef>();
      foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptions)
      {
        if (subscription.CreatedBy != null)
          dictionary[new Guid(subscription.CreatedBy.Id)] = subscription.CreatedBy;
        if (subscription.ModifiedBy != null)
          dictionary[new Guid(subscription.ModifiedBy.Id)] = subscription.ModifiedBy;
      }
      List<Guid> list = dictionary.Keys.ToList<Guid>();
      if (list.Count <= 0)
        return;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.m_identityService.ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
      if (identityList == null)
        return;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
      {
        if (identity != null)
          dictionary[identity.Id] = identity.ToIdentityRef(requestContext, false, true);
      }
      foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptions)
      {
        subscription.CreatedBy = dictionary[new Guid(subscription.CreatedBy.Id)];
        subscription.ModifiedBy = dictionary[new Guid(subscription.ModifiedBy.Id)];
      }
    }

    private void VerifyPublisherEventSubscription(
      IVssRequestContext requestContext,
      PublisherEvent publisherEvent,
      IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions)
    {
      try
      {
        if (subscriptions != null)
        {
          int num = subscriptions.Count<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
          if (num > 1)
          {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Format("Found {0} subscriptions with ids;", (object) num));
            foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in subscriptions)
              stringBuilder.Append(string.Format(" {0}", (object) subscription.Id));
            requestContext.Trace(1053210, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, stringBuilder.ToString());
            return;
          }
        }
        if (publisherEvent.Subscription == null)
        {
          string message = "No subscription supplied with publisherEvent";
          requestContext.Trace(1053220, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, message);
        }
        else if (subscriptions == null || !subscriptions.Any<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>())
        {
          string message = string.Format("No subscriptions found in Service Hooks that matched {0}", (object) publisherEvent.Subscription.Id);
          requestContext.Trace(1053210, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, message);
        }
        else
        {
          Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription = publisherEvent.Subscription;
          Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription s2 = subscriptions.ElementAt<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>(0);
          string differMessage = (string) null;
          if (subscription.Id != s2.Id || subscription.PublisherId != s2.PublisherId || subscription.EventType != s2.EventType || subscription.ConsumerId != s2.ConsumerId || subscription.ConsumerActionId != s2.ConsumerActionId)
            ServiceHooksService.TraceDifferences(requestContext, 1053210, subscription, s2, ref differMessage);
          if (subscription.CreatedBy?.Id != s2.CreatedBy?.Id || subscription.ModifiedBy?.Id != s2.ModifiedBy?.Id)
            ServiceHooksService.TraceDifferences(requestContext, 1053230, subscription, s2, ref differMessage);
          if (subscription.Status != s2.Status || subscription.Subscriber?.Id != s2.Subscriber?.Id)
            ServiceHooksService.TraceDifferences(requestContext, 1053240, subscription, s2, ref differMessage);
          if (!ServiceHooksService.DictionariesMatch(subscription.PublisherInputs, s2.PublisherInputs))
            ServiceHooksService.TraceDifferences(requestContext, 1053250, subscription, s2, ref differMessage);
          if (ServiceHooksService.DictionariesMatch(subscription.ConsumerInputs, s2.ConsumerInputs))
            return;
          ServiceHooksService.TraceDifferences(requestContext, 1053260, subscription, s2, ref differMessage);
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1053210, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, ex.ToString());
      }
    }

    private void ValidateUpdatedConfidentialInputs(
      IVssRequestContext requestContext,
      string consumerId,
      string consumerActionId,
      IDictionary<string, string> newInputs,
      IDictionary<string, string> oldInputs)
    {
      foreach (InputDescriptor confidentialInputDescriptor in SubscriptionInputValueStrongBoxHelper.GetConfidentialInputDescriptors(requestContext, consumerId, consumerActionId))
      {
        InputDescriptor confidentialDescriptor = confidentialInputDescriptor;
        if (SecurityHelper.IsMasked(newInputs.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (i => i.Key == confidentialDescriptor.Id)).Value))
        {
          ConsumerImplementation consumer = this.GetConsumerService(requestContext).GetConsumer(requestContext, consumerId);
          ConsumerActionImplementation actionImplementation = consumer.Actions.FirstOrDefault<ConsumerActionImplementation>((Func<ConsumerActionImplementation, bool>) (a => a.Id == consumerActionId));
          if (actionImplementation != null && confidentialDescriptor.DependencyInputIds != null)
          {
            List<InputDescriptor> source = new List<InputDescriptor>();
            source.AddRange(consumer.InputDescriptors.Where<InputDescriptor>((Func<InputDescriptor, bool>) (i => confidentialDescriptor.DependencyInputIds.Contains(i.Id))));
            source.AddRange(actionImplementation.InputDescriptors.Where<InputDescriptor>((Func<InputDescriptor, bool>) (i => confidentialDescriptor.DependencyInputIds.Contains(i.Id))));
            if (source.Any<InputDescriptor>((Func<InputDescriptor, bool>) (i =>
            {
              KeyValuePair<string, string> keyValuePair = newInputs.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (j => j.Key == i.Id));
              string str1 = keyValuePair.Value;
              keyValuePair = oldInputs.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (j => j.Key == i.Id));
              string str2 = keyValuePair.Value;
              return str1 != str2;
            })))
              throw new ArgumentException(string.Format(ServiceHooksResources.Error_ConfidentialInputValuesRequired, (object) confidentialDescriptor.Name));
          }
        }
      }
    }

    private static void TraceDifferences(
      IVssRequestContext requestContext,
      int tracepoint,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription s1,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription s2,
      ref string differMessage)
    {
      if (differMessage == null)
      {
        string str1 = JsonConvert.SerializeObject((object) s1);
        string str2 = JsonConvert.SerializeObject((object) s2);
        differMessage = differMessage ?? "Subscription mismatch:\r\n" + str1 + "\r\n" + str2;
      }
      requestContext.Trace(tracepoint, TraceLevel.Info, ServiceHooksService.s_area, ServiceHooksService.s_layer, differMessage);
    }

    private static bool DictionariesMatch(
      IDictionary<string, string> first,
      IDictionary<string, string> second)
    {
      if (first == second)
        return true;
      if (first == null || second == null || first.Count != second.Count)
        return false;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) first)
      {
        string str;
        if (!second.TryGetValue(keyValuePair.Key, out str) || !keyValuePair.Value.Equals(str))
          return false;
      }
      return true;
    }

    private IDictionary<string, string> GetSampleNotificationData(
      IVssRequestContext requestContext,
      string eventType)
    {
      Dictionary<string, string> notificationData = new Dictionary<string, string>();
      if (eventType.Equals("workitem.created", StringComparison.OrdinalIgnoreCase) || eventType.Equals("workitem.updated", StringComparison.OrdinalIgnoreCase) || eventType.Equals("workitem.deleted", StringComparison.OrdinalIgnoreCase) || eventType.Equals("workitem.restored", StringComparison.OrdinalIgnoreCase) || eventType.Equals("workitem.commented", StringComparison.OrdinalIgnoreCase))
      {
        notificationData.Add("workItemColor", "CC293D");
        notificationData.Add("workItemIconUrl", requestContext.ExecutionEnvironment.IsHostedDeployment ? "https://tfsprodwcus0.visualstudio.com/_apis/wit/workItemIcons/icon_insect?color=CC293D" : (string) null);
        notificationData.Add("DisplayUrl", "https://dev.azure.com/fabrikam/_workitems/edit/5");
      }
      else if (eventType.Equals("tfvc.checkin", StringComparison.OrdinalIgnoreCase))
      {
        notificationData.Add("changesetWebAccessUrl", "https://dev.azure.com/DefaultCollection/_apis/tfvc/changesets/18");
        notificationData.Add("TeamProject", "FabrikamProject");
      }
      else if (eventType.Equals("git.pullrequest.created", StringComparison.OrdinalIgnoreCase) || eventType.Equals("git.pullrequest.updated", StringComparison.OrdinalIgnoreCase))
        notificationData.Add("status", "Approved");
      else if (eventType.Equals("ms.vss-release.deployment-completed-event", StringComparison.OrdinalIgnoreCase))
      {
        string str1 = "https://dev.azure.com/fabrikam/DefaultCollection/_apis/release/1";
        string str2 = "https://dev.azure.com/fabrikam/DefaultCollection/_apis/releasedefinition/1";
        string str3 = "https://dev.azure.com/fabrikam/DefaultCollection/_apis/releaseenvironment/1";
        notificationData.Add("title", string.Format("Deployment of release [FabrikamTestRelease]({0}) on [FabrikamTestReleaseDefinition]({1}) completed successfully", (object) str1, (object) str2));
        notificationData.Add("subtitle", string.Format("[FabrikamTestRelease]({0}) deployment on [FabrikamTestReleaseDefinition]({1}) : [FabrikamTestReleaseEnvironment]({2}) completed successfully", (object) str1, (object) str2, (object) str3));
      }
      return (IDictionary<string, string>) notificationData;
    }

    private class ConsumerPlugInAssemblyNameComparer : IEqualityComparer<ConsumerImplementation>
    {
      public bool Equals(ConsumerImplementation x, ConsumerImplementation y) => x.GetType().AssemblyQualifiedName == y.GetType().AssemblyQualifiedName;

      public int GetHashCode(ConsumerImplementation obj) => obj.GetType().AssemblyQualifiedName.GetHashCode();
    }

    private class ConsumerActionPlugInAssemblyNameComparer : 
      IEqualityComparer<ConsumerActionImplementation>
    {
      public bool Equals(ConsumerActionImplementation x, ConsumerActionImplementation y) => x.GetType().AssemblyQualifiedName == y.GetType().AssemblyQualifiedName;

      public int GetHashCode(ConsumerActionImplementation obj) => obj.GetType().AssemblyQualifiedName.GetHashCode();
    }

    private class NotificationItem
    {
      public readonly Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification Notification;
      public readonly DateTime QueueTime;
      public readonly bool ConfidentialValuesRestored;
      public readonly bool IsTest;

      public NotificationItem(Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification)
      {
        this.Notification = notification;
        this.QueueTime = DateTime.UtcNow;
        this.IsTest = notification is TestNotification;
      }

      private NotificationItem(ServiceHooksService.NotificationItem itemToClone)
      {
        this.Notification = itemToClone.Notification;
        this.QueueTime = DateTime.UtcNow;
        this.ConfidentialValuesRestored = true;
        this.IsTest = itemToClone.IsTest;
      }

      public ServiceHooksService.NotificationItem CreateNewAttempt() => new ServiceHooksService.NotificationItem(this);
    }
  }
}
