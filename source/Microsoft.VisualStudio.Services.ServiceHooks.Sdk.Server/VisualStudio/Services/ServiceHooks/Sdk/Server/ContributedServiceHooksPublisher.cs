// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ContributedServiceHooksPublisher
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers;
using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public class ContributedServiceHooksPublisher : ServiceHooksPublisher
  {
    private static readonly string s_layer = typeof (ContributedServiceHooksPublisher).Name;
    private static readonly string s_area = typeof (ContributedServiceHooksPublisher).Namespace;
    private static readonly HashSet<string> s_consumersNotRequiringProjectPermissions = new HashSet<string>()
    {
      "office365",
      "teams",
      "workplaceMessagingApps"
    };
    private string m_id;
    private string m_name;
    private string m_description;
    private List<InputDescriptor> m_inputDescriptors;
    private List<EventTypeDescriptor> m_supportedEvents;
    private string m_serviceInstanceType;

    public ContributedServiceHooksPublisher(Publisher publisher)
    {
      this.m_id = publisher.Id;
      this.m_name = publisher.Name;
      this.m_description = publisher.Description;
      this.m_inputDescriptors = publisher.InputDescriptors;
      this.m_supportedEvents = publisher.SupportedEvents;
      this.m_serviceInstanceType = publisher.ServiceInstanceType;
    }

    public override string Id => this.m_id;

    public override string Name => this.m_name;

    public override string Description => this.m_description;

    public override IEnumerable<InputDescriptor> InputDescriptors => (IEnumerable<InputDescriptor>) this.m_inputDescriptors;

    public string ServiceInstanceType => this.m_serviceInstanceType;

    public override bool CheckPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      bool throwIfNoPermission = true,
      bool writePermission = false)
    {
      requestContext.TraceEnter(1061100, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (CheckPermission));
      try
      {
        Guid projectScope = this.GetProjectScope(subscription);
        Guid subscriberId = this.GetSubscriberId(subscription);
        if (subscriberId != Guid.Empty)
        {
          int permission = writePermission ? 2 : 1;
          return this.CheckSubscriberPermission(requestContext, permission, subscriberId, throwIfNoPermission);
        }
        int permission1 = writePermission ? 2 : 1;
        return this.CheckProjectPermission(requestContext, permission1, projectScope, throwIfNoPermission);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061110, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061120, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (CheckPermission));
      }
    }

    public override bool CheckPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      bool throwIfNoPermission = true,
      bool writePermission = false)
    {
      requestContext.TraceEnter(1061130, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (CheckPermission));
      try
      {
        Guid projectScope = this.GetProjectScope(notification);
        Guid subscriberId = this.GetSubscriberId(notification);
        if (subscriberId != Guid.Empty)
        {
          int permission = writePermission ? 2 : 1;
          return this.CheckSubscriberPermission(requestContext, permission, subscriberId, throwIfNoPermission);
        }
        int permission1 = writePermission ? 2 : 1;
        return this.CheckProjectPermission(requestContext, permission1, projectScope, throwIfNoPermission);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061140, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061150, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (CheckPermission));
      }
    }

    public override IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> QuerySubscriptions(
      IVssRequestContext requestContext,
      SubscriptionsQuery query,
      bool unmaskConfidentialInputs = false)
    {
      requestContext.TraceEnter(1061160, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (QuerySubscriptions));
      try
      {
        List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> list = base.QuerySubscriptions(requestContext, query, unmaskConfidentialInputs).Where<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, bool>) (subscription =>
        {
          subscription.SetSubscriptionSubscriber(requestContext);
          Guid projectScope = this.GetProjectScope(subscription);
          Guid subscriberId = this.GetSubscriberId(subscription);
          return subscriberId != Guid.Empty ? this.CheckSubscriberPermission(requestContext, 1, subscriberId, false) : this.CheckProjectPermission(requestContext, 1, projectScope, false);
        })).ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
        list.ForEach((Action<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) (subscription => this.FilterPublisherInputs(requestContext, subscription)));
        return (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) list;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061170, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061180, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (QuerySubscriptions));
      }
    }

    public override void UpdateSubscriptionStatuses(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> hooksSubscriptions)
    {
      INotificationSubscriptionService service = requestContext.GetService<INotificationSubscriptionService>();
      try
      {
        SubscriptionLookup anyFieldLookup = SubscriptionLookup.CreateAnyFieldLookup(channel: "ServiceHooks");
        List<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> notificationSubscriptions = service.QuerySubscriptions(requestContext, anyFieldLookup);
        hooksSubscriptions.UpdateStatuses(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>) notificationSubscriptions);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1063070, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, ex);
      }
    }

    public override void UpdateSubscriptionStatus(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription hooksSubscription)
    {
      INotificationSubscriptionService service = requestContext.GetService<INotificationSubscriptionService>();
      try
      {
        string classification;
        if (!hooksSubscription.PublisherInputs.TryGetValue("tfsSubscriptionId", out classification))
          return;
        SubscriptionLookup anyFieldLookup = SubscriptionLookup.CreateAnyFieldLookup(classification: classification);
        Microsoft.VisualStudio.Services.Notifications.Server.Subscription notificationSubscription = service.QuerySubscriptions(requestContext, anyFieldLookup).FirstOrDefault<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>();
        if (notificationSubscription == null)
          return;
        hooksSubscription.UpdateStatus(requestContext, notificationSubscription);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1063070, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, ex);
      }
    }

    public override Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription CreateSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscriptionToCreate)
    {
      requestContext.TraceEnter(1061190, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (CreateSubscription));
      try
      {
        Guid projectScope = this.GetProjectScope(subscriptionToCreate);
        Guid subscriberId = this.GetSubscriberId(subscriptionToCreate);
        if (projectScope == Guid.Empty)
          this.CheckCollectionPermission(requestContext);
        if (subscriberId != Guid.Empty)
          this.CheckSubscriberPermission(requestContext, 2, subscriberId);
        else
          this.CheckProjectPermission(requestContext, 2, projectScope);
        this.CheckConsumerPermission(requestContext, subscriptionToCreate.ConsumerId, projectScope);
        string str = Guid.NewGuid().ToString();
        if (subscriptionToCreate.PublisherInputs == null)
          subscriptionToCreate.PublisherInputs = (IDictionary<string, string>) new Dictionary<string, string>();
        IFrameworkServiceHooksEventRegistration serviceHookEventType = (IFrameworkServiceHooksEventRegistration) this.GetRegistrationForServiceHookEventType(requestContext, subscriptionToCreate.EventType);
        subscriptionToCreate.EventDescription = this.GetEventDescription(requestContext, serviceHookEventType, subscriptionToCreate.PublisherInputs);
        subscriptionToCreate.ActionDescription = this.GetConsumerActionDescription(requestContext, subscriptionToCreate);
        subscriptionToCreate.PublisherInputs["tfsSubscriptionId"] = str;
        if (subscriberId != Guid.Empty)
          subscriptionToCreate.PublisherInputs["subscriberId"] = subscriberId.ToString();
        if (subscriptionToCreate.Id == Guid.Empty)
          subscriptionToCreate.Id = Guid.NewGuid();
        subscriptionToCreate.CreatedDate = DateTime.UtcNow;
        subscriptionToCreate.ModifiedDate = DateTime.UtcNow;
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        subscriptionToCreate.CreatedBy = userIdentity.ToIdentityRef();
        subscriptionToCreate.ModifiedBy = subscriptionToCreate.CreatedBy;
        List<KeyValuePair<string, string>> confidentialPublisherInputs;
        List<KeyValuePair<string, string>> confidentialConsumerInputs;
        SubscriptionInputValueStrongBoxHelper.StripConfidentialInputs(requestContext, subscriptionToCreate, out confidentialPublisherInputs, out confidentialConsumerInputs);
        SubscriptionInputValueStrongBoxHelper.StoreConfidentialInputs(requestContext, subscriptionToCreate, confidentialPublisherInputs, confidentialConsumerInputs);
        string subscriptionXpathFilter = serviceHookEventType.GetSubscriptionXPathFilter(requestContext, subscriptionToCreate.EventType, subscriptionToCreate.PublisherInputs, new Guid?(projectScope));
        Guid guid = Guid.Empty;
        SubscriptionScope subscriptionScope1 = (SubscriptionScope) null;
        IVssRequestContext requestContext1 = requestContext.Elevate();
        INotificationSubscriptionServiceInternal service = requestContext.GetService<INotificationSubscriptionServiceInternal>();
        if (projectScope == Guid.Empty)
        {
          if (!this.EventSupportsScope(requestContext, serviceHookEventType.PublisherEventType, "collection"))
            throw new ArgumentException(string.Format(ServiceHooksPublisherResources.Error_EventNotAllowedAtCollectionLevel, (object) serviceHookEventType.PublisherEventType));
          SubscriptionScope subscriptionScope2 = new SubscriptionScope();
          subscriptionScope2.Id = NotificationClientConstants.CollectionScope;
          subscriptionScope1 = subscriptionScope2;
          Microsoft.VisualStudio.Services.Identity.Identity validUsersGroup = service.GetValidUsersGroup(requestContext);
          subscriberId = validUsersGroup != null ? validUsersGroup.Id : Guid.Empty;
        }
        else if (this.EventSupportsScope(requestContext, serviceHookEventType.PublisherEventType, "project"))
        {
          guid = projectScope;
          SubscriptionScope subscriptionScope3 = new SubscriptionScope();
          subscriptionScope3.Id = projectScope;
          subscriptionScope1 = subscriptionScope3;
        }
        string deliveryChannelName = this.GetDeliveryChannelName(requestContext, subscriptionToCreate);
        if (deliveryChannelName != "ServiceHooks")
          subscriptionToCreate.IsLocal = true;
        Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription1 = new Microsoft.VisualStudio.Services.Notifications.Server.Subscription()
        {
          SubscriptionFilter = (ISubscriptionFilter) new ExpressionFilter(serviceHookEventType.PublisherEventType),
          ConditionString = subscriptionXpathFilter,
          Channel = deliveryChannelName,
          LastModifiedBy = requestContext.GetUserId(),
          Matcher = "PathMatcher",
          Metadata = JsonConvert.SerializeObject((object) subscriptionToCreate, NotificationsSerialization.JsonSerializerSettings),
          Tag = str,
          ProjectId = guid,
          SubscriptionScope = subscriptionScope1,
          SubscriberId = subscriberId,
          UniqueId = subscriptionToCreate.Id,
          Diagnostics = subscriptionToCreate.IsLocal ? this.CreateSubscriptionDiagnostics() : (SubscriptionDiagnostics) null
        };
        int subscription2 = service.CreateSubscription(requestContext1, subscription1);
        SubscriptionInputValueStrongBoxHelper.ApplyConfidentialInputs(requestContext, subscriptionToCreate, confidentialPublisherInputs, confidentialConsumerInputs, subscriptionToCreate.IsLocal);
        try
        {
          return base.CreateSubscription(requestContext, subscriptionToCreate);
        }
        catch (Exception ex)
        {
          SubscriptionInputValueStrongBoxHelper.DeleteDrawer(requestContext, subscriptionToCreate.Id);
          service.DeleteSubscription(requestContext1, subscription2);
          throw;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061200, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061210, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (CreateSubscription));
      }
    }

    public override Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription UpdateSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription existingSubscription,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription newSubscription)
    {
      requestContext.TraceEnter(1061220, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (UpdateSubscription));
      try
      {
        Guid projectScope1 = this.GetProjectScope(existingSubscription);
        Guid subscriberId1 = this.GetSubscriberId(existingSubscription);
        if (projectScope1 == Guid.Empty)
          this.CheckCollectionPermission(requestContext);
        if (subscriberId1 != Guid.Empty)
          this.CheckSubscriberPermission(requestContext, 2, subscriberId1);
        else
          this.CheckProjectPermission(requestContext, 2, projectScope1);
        Guid projectScope2 = this.GetProjectScope(newSubscription);
        Guid subscriberId2 = this.GetSubscriberId(newSubscription);
        if (projectScope2 == Guid.Empty)
          this.CheckCollectionPermission(requestContext);
        if (subscriberId2 != Guid.Empty)
          this.CheckSubscriberPermission(requestContext, 2, subscriberId2);
        else
          this.CheckProjectPermission(requestContext, 2, projectScope2);
        this.CheckConsumerPermission(requestContext, newSubscription.ConsumerId, projectScope2);
        if (newSubscription.PublisherInputs == null)
          newSubscription.PublisherInputs = (IDictionary<string, string>) new Dictionary<string, string>();
        IFrameworkServiceHooksEventRegistration serviceHookEventType = (IFrameworkServiceHooksEventRegistration) this.GetRegistrationForServiceHookEventType(requestContext, newSubscription.EventType);
        string subscriptionXpathFilter = serviceHookEventType.GetSubscriptionXPathFilter(requestContext, newSubscription.EventType, newSubscription.PublisherInputs, new Guid?(projectScope2));
        string str1 = (string) null;
        if (existingSubscription.PublisherInputs == null || !existingSubscription.PublisherInputs.TryGetValue("tfsSubscriptionId", out str1))
          throw new ServiceHookException(string.Format(ServiceHooksPublisherResources.Error_NotificationSubscriptionIdNotSpecifiedOnUpdateFormat, (object) existingSubscription.Id.ToString()));
        string classification = str1;
        SubscriptionLookup anyFieldLookup = SubscriptionLookup.CreateAnyFieldLookup(dataspaceId: new Guid?(projectScope2), classification: classification);
        IVssRequestContext requestContext1 = requestContext.Elevate();
        INotificationSubscriptionService service = requestContext.GetService<INotificationSubscriptionService>();
        Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscriptionToPatch = service.QuerySubscriptions(requestContext1, anyFieldLookup).OrderByDescending<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, int>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, int>) (s => s.ID)).FirstOrDefault<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>();
        if (subscriptionToPatch == null)
          throw new ServiceHookException(string.Format(ServiceHooksPublisherResources.Error_NotificationSubscriptionNoLongerExistsFormat, (object) str1, (object) newSubscription.Id.ToString()));
        Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus? nullable1 = new Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus?();
        if (subscriptionToPatch.Status >= Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.Enabled)
        {
          if (newSubscription.Status == Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.DisabledByUser || newSubscription.Status == Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.DisabledBySystem)
            nullable1 = new Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus?(Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.Disabled);
        }
        else if (newSubscription.Status == Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionStatus.Enabled)
          nullable1 = new Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus?(Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus.Enabled);
        List<KeyValuePair<string, string>> confidentialPublisherInputs;
        List<KeyValuePair<string, string>> confidentialConsumerInputs;
        SubscriptionInputValueStrongBoxHelper.StripConfidentialInputs(requestContext, newSubscription, out confidentialPublisherInputs, out confidentialConsumerInputs);
        newSubscription.PublisherInputs["tfsSubscriptionId"] = str1;
        if (subscriberId2 != Guid.Empty)
          newSubscription.PublisherInputs["subscriberId"] = subscriberId2.ToString();
        newSubscription.EventDescription = this.GetEventDescription(requestContext, serviceHookEventType, newSubscription.PublisherInputs);
        newSubscription.ActionDescription = this.GetConsumerActionDescription(requestContext, newSubscription);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        newSubscription.ModifiedBy = userIdentity.ToIdentityRef();
        newSubscription.ModifiedDate = DateTime.UtcNow;
        if (newSubscription.CreatedDate == DateTime.MinValue)
          newSubscription.CreatedDate = existingSubscription.CreatedDate;
        if (newSubscription.CreatedBy == null)
          newSubscription.CreatedBy = existingSubscription.CreatedBy;
        Guid? nullable2 = new Guid?(Guid.Empty);
        if (projectScope2 == Guid.Empty)
        {
          if (!this.EventSupportsScope(requestContext, serviceHookEventType.PublisherEventType, "collection"))
            throw new ArgumentException(string.Format(ServiceHooksPublisherResources.Error_EventNotAllowedAtCollectionLevel, (object) serviceHookEventType.PublisherEventType));
          nullable2 = new Guid?(NotificationClientConstants.CollectionScope);
        }
        else if (this.EventSupportsScope(requestContext, serviceHookEventType.PublisherEventType, "project"))
          nullable2 = new Guid?(projectScope2);
        string str2 = JsonConvert.SerializeObject((object) newSubscription, NotificationsSerialization.JsonSerializerSettings);
        SubscriptionUpdate subscriptionUpdate = new SubscriptionUpdate(subscriptionToPatch.ID)
        {
          Expression = subscriptionXpathFilter,
          EventTypeName = serviceHookEventType.PublisherEventType,
          LastModifiedBy = new Guid?(requestContext.GetUserId()),
          MetaData = str2,
          ScopeId = nullable2,
          Status = nullable1,
          Diagnostics = existingSubscription.IsLocal ? this.CreateSubscriptionDiagnostics() : (SubscriptionDiagnostics) null
        };
        string input;
        if (newSubscription.PublisherInputs.TryGetValue("subscriberId", out input))
          subscriptionUpdate.SubscriberId = new Guid?(Guid.Parse(input));
        else if (!requestContext.IsServicingContext && !subscriptionToPatch.SubscriberIdentity.IsContainer)
          subscriptionUpdate.SubscriberId = new Guid?(requestContext.GetUserId());
        service.UpdateSubscription(requestContext1, subscriptionToPatch, subscriptionUpdate);
        SubscriptionInputValueStrongBoxHelper.UpdateUnmaskedInputsInStrongBox(requestContext, newSubscription, confidentialPublisherInputs, confidentialConsumerInputs, existingSubscription.IsLocal);
        SubscriptionInputValueStrongBoxHelper.RemoveOrphanedInputValuesFromStrongBox(requestContext, newSubscription, confidentialPublisherInputs, confidentialConsumerInputs);
        return base.UpdateSubscription(requestContext, existingSubscription, newSubscription);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061230, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061240, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (UpdateSubscription));
      }
    }

    public override void DeleteSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscriptionToDelete)
    {
      requestContext.TraceEnter(1061250, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (DeleteSubscription));
      try
      {
        Guid projectScope = this.GetProjectScope(subscriptionToDelete);
        Guid subscriberId = this.GetSubscriberId(subscriptionToDelete);
        if (projectScope == Guid.Empty)
          this.CheckCollectionPermission(requestContext);
        if (subscriberId != Guid.Empty)
          this.CheckSubscriberPermission(requestContext, 2, subscriberId);
        else if (!this.CheckProjectPermission(requestContext, 2, projectScope, false))
          this.CheckProjectPermission(requestContext, 4, projectScope);
        string classification;
        if (subscriptionToDelete.PublisherInputs.TryGetValue("tfsSubscriptionId", out classification))
        {
          IVssRequestContext requestContext1 = requestContext.Elevate();
          INotificationSubscriptionService service = requestContext.GetService<INotificationSubscriptionService>();
          SubscriptionLookup anyFieldLookup = SubscriptionLookup.CreateAnyFieldLookup(classification: classification);
          Microsoft.VisualStudio.Services.Notifications.Server.Subscription subscription = service.QuerySubscriptions(requestContext1, anyFieldLookup).OrderByDescending<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, int>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, int>) (s => s.ID)).FirstOrDefault<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>();
          if (subscription != null)
            service.DeleteSubscription(requestContext1, subscription.ID);
          SubscriptionInputValueStrongBoxHelper.DeleteDrawer(requestContext, subscriptionToDelete.Id);
          ServiceHooksTokenHelper.DeleteTokensForSubscription(requestContext, subscriptionToDelete.Id);
        }
        base.DeleteSubscription(requestContext, subscriptionToDelete);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061260, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061270, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (DeleteSubscription));
      }
    }

    public override Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification SendTestNotification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification testNotification)
    {
      requestContext.TraceEnter(1061280, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (SendTestNotification));
      try
      {
        this.CheckPermission(requestContext, testNotification, true, true);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        testNotification.SubscriberId = userIdentity.Id;
        return base.SendTestNotification(requestContext, testNotification);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061290, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061300, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (SendTestNotification));
      }
    }

    public override InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentValues)
    {
      return (InputValues) null;
    }

    protected override void ValidateSubscriptionInput(
      IVssRequestContext requestContext,
      EventTypeDescriptor eventType,
      string inputId,
      string inputValue,
      string scope)
    {
      requestContext.TraceEnter(1061370, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (ValidateSubscriptionInput));
      try
      {
        if (string.Equals(inputId, "tfsSubscriptionId"))
          return;
        base.ValidateSubscriptionInput(requestContext, eventType, inputId, inputValue, scope);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061380, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061390, ContributedServiceHooksPublisher.s_area, ContributedServiceHooksPublisher.s_layer, nameof (ValidateSubscriptionInput));
      }
    }

    public override InputFilterCondition GetPublisherSubscriptionFilterCondition(
      string tfsSubscriptionIdentifier)
    {
      return new InputFilterCondition()
      {
        InputId = "tfsSubscriptionId",
        Operator = InputFilterOperator.Equals,
        InputValue = tfsSubscriptionIdentifier
      };
    }

    public override Publisher ToPublisherModel(
      IVssRequestContext requestContext,
      IDictionary<string, string> publisherInputs = null)
    {
      Publisher publisherModel = base.ToPublisherModel(requestContext, publisherInputs);
      publisherModel.ServiceInstanceType = this.ServiceInstanceType;
      return publisherModel;
    }

    private Guid GetProjectScope(Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription) => this.GetPublisherInputGuid(subscription.PublisherInputs, "projectId");

    private Guid GetProjectScope(Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification) => this.GetPublisherInputGuid(notification.Details?.PublisherInputs, "projectId");

    private Guid GetSubscriberId(Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      Guid result = Guid.Empty;
      if (!Guid.TryParse(subscription?.Subscriber?.Id, out result))
        result = this.GetPublisherInputGuid(subscription?.PublisherInputs, "subscriberId");
      return result;
    }

    private Guid GetSubscriberId(Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification) => this.GetPublisherInputGuid(notification.Details?.PublisherInputs, "subscriberId");

    private Guid GetPublisherInputGuid(
      IDictionary<string, string> publisherInputs,
      string publisherInputId)
    {
      string input;
      Guid result;
      return publisherInputs != null && publisherInputs.TryGetValue(publisherInputId, out input) && Guid.TryParse(input, out result) ? result : Guid.Empty;
    }

    private string GetDeliveryChannelName(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      return !(subscription?.ConsumerId == AzureServiceBusConsumer.ConsumerId) || !requestContext.IsFeatureEnabled("Notifications.Channel.AzureServiceBus") ? "ServiceHooks" : "ServiceBus";
    }

    private bool EventSupportsScope(
      IVssRequestContext requestContext,
      string eventType,
      string scopeLevel)
    {
      NotificationEventType eventType1 = requestContext.GetService<NotificationEventService>().GetEventType(requestContext, eventType, EventTypeQueryFlags.None);
      return eventType1?.SupportedScopes == null || eventType1.SupportedScopes.Contains(scopeLevel);
    }

    private string GetEventDescription(
      IVssRequestContext requestContext,
      IFrameworkServiceHooksEventRegistration registration,
      IDictionary<string, string> publisherInputs)
    {
      return registration != null ? registration.GetEventDescription(requestContext, publisherInputs) : ServiceHooksPublisherResources.TfsEventDescriptionNone;
    }

    private bool CheckProjectPermission(
      IVssRequestContext requestContext,
      int permission,
      Guid teamProjectId,
      bool throwIfNoPermission = true)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ServiceHooksSecurityConstants.NamespaceId);
      string token = !(teamProjectId != Guid.Empty) ? "PublisherSecurity" : SecurityHelper.GetProjectSecurityToken(teamProjectId.ToString());
      if (!throwIfNoPermission)
        return securityNamespace.HasPermission(requestContext, token, permission, false);
      securityNamespace.CheckPermission(requestContext, token, permission, false);
      return true;
    }

    private bool CheckSubscriberPermission(
      IVssRequestContext requestContext,
      int permission,
      Guid subscriberId,
      bool throwIfNoPermission = true)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        subscriberId
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        throw new IdentityNotFoundException(subscriberId);
      if (!identity.IsContainer)
        throw new ArgumentException(string.Format(ServiceHooksPublisherResources.Error_SubscriberIdMustBeGroupFormat, (object) subscriberId));
      int num = requestContext.GetService<INotificationSubscriptionService>().HasPermissions(requestContext, identity, permission) ? 1 : 0;
      if (!(num == 0 & throwIfNoPermission))
        return num != 0;
      throw new ArgumentException(string.Format(ServiceHooksPublisherResources.Error_EditPermissionDenied, (object) requestContext.UserContext.Identifier));
    }

    private bool CheckCollectionPermission(
      IVssRequestContext requestContext,
      bool throwIfNoPermission = true)
    {
      int num = requestContext.GetService<INotificationSubscriptionService>().CallerHasAdminPermissions(requestContext) ? 1 : 0;
      if (!(num == 0 & throwIfNoPermission))
        return num != 0;
      throw new ArgumentException(string.Format(ServiceHooksPublisherResources.Error_EditPermissionDenied, (object) requestContext.UserContext.Identifier));
    }

    private SubscriptionDiagnostics CreateSubscriptionDiagnostics()
    {
      SubscriptionDiagnostics diagnostics = new SubscriptionDiagnostics();
      diagnostics.UpdateParameters(new UpdateSubscripitonDiagnosticsParameters()
      {
        DeliveryResults = new UpdateSubscripitonTracingParameters()
        {
          Enabled = true
        }
      });
      return diagnostics;
    }

    public bool CheckConsumerPermission(
      IVssRequestContext requestContext,
      string consumerId,
      Guid projectId,
      bool throwIfNoPermission = true)
    {
      if (this.CheckProjectPermission(requestContext, 1, projectId, false) || ContributedServiceHooksPublisher.s_consumersNotRequiringProjectPermissions.Contains(consumerId))
        return true;
      if (throwIfNoPermission)
        throw new ConsumerNotAvailableException(consumerId);
      return false;
    }

    private string GetConsumerActionDescription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>(subscription.ConsumerInputs);
      SubscriptionInputValueStrongBoxHelper.RestoreConfidentialInputValues(requestContext, subscription.ConsumerId, subscription.ConsumerActionId, subscription.Id, dictionary);
      return requestContext.To(TeamFoundationHostType.Deployment).GetService<ServiceHooksConsumerService>().GetConsumer(requestContext, subscription.ConsumerId).GetActionDescription(requestContext, subscription.ConsumerActionId, dictionary);
    }

    private void FilterPublisherInputs(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      string newEventDescription;
      if (!(this.GetRegistrationForServiceHookEventType(requestContext, subscription.EventType, false) is IFrameworkServiceHooksPublisherInputsResultFilter serviceHookEventType) || !serviceHookEventType.FilterInputs(requestContext, subscription.PublisherInputs, out newEventDescription))
        return;
      subscription.EventDescription = newEventDescription;
    }
  }
}
