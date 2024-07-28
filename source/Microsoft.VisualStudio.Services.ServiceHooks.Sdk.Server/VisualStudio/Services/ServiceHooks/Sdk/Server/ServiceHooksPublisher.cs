// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ServiceHooksPublisher
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public abstract class ServiceHooksPublisher
  {
    private static readonly string s_layer = typeof (ServiceHooksPublisher).Name;
    private static readonly string s_area = typeof (ServiceHooksPublisher).Namespace;
    protected Dictionary<string, IServiceHooksPublisherEventRegistration> m_eventRegistrationsByPublisherEventType;
    protected Dictionary<string, IServiceHooksPublisherEventRegistration> m_eventRegistrationsByServiceHookEventType;
    protected IReadOnlyList<IServiceHooksPublisherEventRegistration> m_eventRegistrations;
    private ServiceHooksClientService m_clientHooksService;

    public abstract string Id { get; }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public abstract IEnumerable<InputDescriptor> InputDescriptors { get; }

    public virtual void Start(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1062010, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (Start));
      try
      {
        (this.m_eventRegistrations, this.m_eventRegistrationsByPublisherEventType, this.m_eventRegistrationsByServiceHookEventType) = ServiceHooksPublisher.GetExtensions(requestContext, this.Id);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061020, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061030, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (Start));
      }
    }

    public IEnumerable<string> GetSupportedPublisherEventTypes(IVssRequestContext requestContext) => (IEnumerable<string>) this.m_eventRegistrationsByPublisherEventType.Keys.ToArray<string>();

    public virtual IEnumerable<EventTypeDescriptor> GetSupportedEvents(
      IVssRequestContext requestContext,
      IDictionary<string, string> publisherInputs = null)
    {
      requestContext.TraceEnter(1061040, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetSupportedEvents));
      try
      {
        IReadOnlyList<IServiceHooksPublisherEventRegistration> eventRegistrations = this.m_eventRegistrations;
        bool removeReleaseRelatedFields = false;
        IDictionary<string, string> scopeValues = (IDictionary<string, string>) null;
        if (publisherInputs != null && publisherInputs.ContainsKey("projectId"))
        {
          scopeValues = (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "project",
              publisherInputs["projectId"]
            }
          };
          if (requestContext.IsFeatureEnabled("ServiceHooks.Publisher.Filter.ReleaseManagement"))
          {
            IServiceHooksExtensionManagement extension = requestContext.GetExtension<IServiceHooksExtensionManagement>();
            if (extension != null)
              removeReleaseRelatedFields = !extension.ShowReleaseManagementServiceHooksEvents(requestContext, publisherInputs["projectId"]);
          }
        }
        IContributedFeatureService featureService = requestContext.GetService<IContributedFeatureService>();
        Func<IServiceHooksPublisherEventRegistration, bool> predicate = (Func<IServiceHooksPublisherEventRegistration, bool>) (reg => reg.ProductClaim == null || featureService.IsFeatureEnabled(requestContext, reg.ProductClaim, scopeValues));
        return eventRegistrations.Where<IServiceHooksPublisherEventRegistration>(predicate).Where<IServiceHooksPublisherEventRegistration>((Func<IServiceHooksPublisherEventRegistration, bool>) (r => !removeReleaseRelatedFields || !string.Equals(r.PublisherId, "rm", StringComparison.OrdinalIgnoreCase))).SelectMany<IServiceHooksPublisherEventRegistration, EventTypeDescriptor>((Func<IServiceHooksPublisherEventRegistration, IEnumerable<EventTypeDescriptor>>) (r => r.GetDescriptors(requestContext, publisherInputs)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061050, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061060, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetSupportedEvents));
      }
    }

    public virtual Publisher ToPublisherModel(
      IVssRequestContext requestContext,
      IDictionary<string, string> publisherInputs = null)
    {
      Publisher publisherModel = new Publisher()
      {
        Description = this.Description,
        Id = this.Id,
        Name = this.Name,
        SupportedEvents = this.GetSupportedEvents(requestContext, publisherInputs).ToList<EventTypeDescriptor>(),
        InputDescriptors = this.InputDescriptors.ToList<InputDescriptor>()
      };
      if (publisherModel.SupportedEvents != null)
      {
        foreach (EventTypeDescriptor supportedEvent in publisherModel.SupportedEvents)
          supportedEvent.PublisherId = publisherModel.Id;
      }
      return publisherModel;
    }

    public virtual ServiceHooksClientService GetHooksService(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1062040, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetHooksService));
      try
      {
        if (this.m_clientHooksService == null)
          this.m_clientHooksService = new ServiceHooksClientService();
        return this.m_clientHooksService;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062050, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062060, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetHooksService));
      }
    }

    public virtual Event GetSampleEvent(
      IVssRequestContext requestContext,
      IDictionary<string, string> publisherInputs,
      string eventType,
      string resourceVersion)
    {
      requestContext.TraceEnter(1062070, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetSampleEvent));
      try
      {
        IServiceHooksPublisherEventRegistration serviceHookEventType = this.GetRegistrationForServiceHookEventType(requestContext, eventType);
        Event sampleEvent = serviceHookEventType.GetSampleEvent(requestContext, publisherInputs, eventType, resourceVersion);
        if (sampleEvent.Resource != null)
        {
          sampleEvent.ResourceVersion = sampleEvent.ResourceVersion ?? resourceVersion;
          if (string.IsNullOrEmpty(sampleEvent.ResourceVersion) || string.Equals("latest", sampleEvent.ResourceVersion, StringComparison.OrdinalIgnoreCase))
          {
            EventTypeDescriptor eventTypeDescriptor = serviceHookEventType.GetDescriptors(requestContext, publisherInputs).First<EventTypeDescriptor>((Func<EventTypeDescriptor, bool>) (e => string.Equals(e.Id, eventType, StringComparison.OrdinalIgnoreCase)));
            sampleEvent.ResourceVersion = ResourceVersionComparer.GetDefaultVersion((IEnumerable<string>) eventTypeDescriptor.SupportedResourceVersions, false);
          }
          sampleEvent.ResourceContainers = sampleEvent.ResourceContainers ?? this.GetSampleResourceContainers();
        }
        return sampleEvent;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062080, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062090, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetSampleEvent));
      }
    }

    private IDictionary<string, ResourceContainer> GetSampleResourceContainers() => (IDictionary<string, ResourceContainer>) new Dictionary<string, ResourceContainer>()
    {
      {
        "collection",
        new ResourceContainer()
        {
          Id = Guid.Parse("c12d0eb8-e382-443b-9f9c-c52cba5014c2")
        }
      },
      {
        "account",
        new ResourceContainer()
        {
          Id = Guid.Parse("f844ec47-a9db-4511-8281-8b63f4eaf94e")
        }
      },
      {
        "project",
        new ResourceContainer()
        {
          Id = Guid.Parse("be9b3917-87e6-42a4-a549-2bc06a7a878f")
        }
      }
    };

    public virtual Event GetRealSampleEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      string resourceVersion)
    {
      requestContext.TraceEnter(1062520, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetRealSampleEvent));
      try
      {
        IServiceHooksPublisherEventRegistration serviceHookEventType = this.GetRegistrationForServiceHookEventType(requestContext, notification.Details.EventType);
        if (!(serviceHookEventType is IRealEventDataProvider))
          throw new NotImplementedException(string.Format(ServiceHooksPublisherResources.Error_TestWithRealDataNotSupportedForEventType, (object) notification.Details.EventType));
        Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription = this.GetHooksService(requestContext).GetSubscription(requestContext, notification.SubscriptionId);
        IDictionary<string, string> consumerInputs = notification.Details.ConsumerInputs;
        if (consumerInputs != null && consumerInputs.Count<KeyValuePair<string, string>>() > 0)
          throw new InvalidOperationException(ServiceHooksPublisherResources.Error_CannotOverrideConsumerSettings);
        notification.Details.ConsumerInputs = subscription.ConsumerInputs;
        IDictionary<string, string> dictionary = subscription.PublisherInputs;
        if (notification.Details.PublisherInputs != null)
          dictionary = (IDictionary<string, string>) dictionary.Concat<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) notification.Details.PublisherInputs).GroupBy<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key)).ToDictionary<IGrouping<string, KeyValuePair<string, string>>, string, string>((Func<IGrouping<string, KeyValuePair<string, string>>, string>) (e => e.Key), (Func<IGrouping<string, KeyValuePair<string, string>>, string>) (e => e.Last<KeyValuePair<string, string>>().Value));
        notification.Details.PublisherInputs = dictionary;
        return this.ToPublisherEvent(((IRealEventDataProvider) serviceHookEventType).GetRealSampleEvent(requestContext, dictionary, notification.Details.EventType), requestContext, (string) null).Event;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062530, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062540, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetRealSampleEvent));
      }
    }

    public virtual Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification SendTestNotification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification testNotification)
    {
      requestContext.TraceEnter(1062100, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (SendTestNotification));
      try
      {
        return this.GetHooksService(requestContext).SendTestNotification(requestContext, testNotification);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062110, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062120, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (SendTestNotification));
      }
    }

    public virtual bool CheckPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      bool throwIfNoPermission = true,
      bool writePermission = false)
    {
      requestContext.TraceEnter(1062130, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (CheckPermission));
      try
      {
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062140, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062150, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (CheckPermission));
      }
    }

    public virtual bool CheckPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      bool throwIfNoPermission = true,
      bool writePermission = false)
    {
      requestContext.TraceEnter(1062160, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (CheckPermission));
      try
      {
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062170, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062180, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (CheckPermission));
      }
    }

    public IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> FilterSubscriptionsWithPermission(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions,
      bool writePermission = false)
    {
      requestContext.TraceEnter(1062190, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (FilterSubscriptionsWithPermission));
      try
      {
        return (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptions.Where<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, bool>) (s => this.CheckPermission(requestContext, s, false, writePermission))).ToList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062200, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062210, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (FilterSubscriptionsWithPermission));
      }
    }

    public virtual IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> QuerySubscriptions(
      IVssRequestContext requestContext,
      SubscriptionsQuery query,
      bool unmaskConfidentialInputs = false)
    {
      requestContext.TraceEnter(1062220, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (QuerySubscriptions));
      try
      {
        if (query.PublisherId != null && !string.Equals(query.PublisherId, this.Id, StringComparison.OrdinalIgnoreCase))
          return (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) Array.Empty<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
        string publisherId = query.PublisherId;
        List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> hooksSubscriptions = new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
        try
        {
          query.PublisherId = this.Id;
          hooksSubscriptions.AddRange(this.GetHooksService(requestContext).QuerySubscriptions(requestContext, query, unmaskConfidentialInputs));
          this.UpdateSubscriptionStatuses(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) hooksSubscriptions);
          hooksSubscriptions.AddRange(this.GetLocalSubscriptions(requestContext, query, unmaskConfidentialInputs));
        }
        finally
        {
          query.PublisherId = publisherId;
        }
        return (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) hooksSubscriptions;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062230, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062240, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (QuerySubscriptions));
      }
    }

    public virtual void UpdateSubscriptionStatuses(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> hooksSubscriptions)
    {
    }

    public virtual void UpdateSubscriptionStatus(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription hooksSubscription)
    {
    }

    public virtual Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription CreateSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscriptionToCreate)
    {
      requestContext.TraceEnter(1062250, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (CreateSubscription));
      try
      {
        if (subscriptionToCreate.IsLocal)
          return subscriptionToCreate;
        this.ValidateSubscriptionInputs(requestContext, subscriptionToCreate.EventType, subscriptionToCreate.PublisherInputs);
        return this.GetHooksService(requestContext).CreateSubscription(requestContext, subscriptionToCreate);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062260, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062270, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (CreateSubscription));
      }
    }

    public virtual void DeleteSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscriptionToDelete)
    {
      requestContext.TraceEnter(1062310, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (DeleteSubscription));
      try
      {
        if (subscriptionToDelete.IsLocal)
          return;
        this.GetHooksService(requestContext).DeleteSubscription(requestContext, subscriptionToDelete.Id);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062320, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062330, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (DeleteSubscription));
      }
    }

    public virtual Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription UpdateSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription existingSubscription,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription newSubscription)
    {
      requestContext.TraceEnter(1062280, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (UpdateSubscription));
      try
      {
        if (existingSubscription.IsLocal)
          return newSubscription;
        this.ValidateSubscriptionInputs(requestContext, newSubscription.EventType, newSubscription.PublisherInputs);
        return this.GetHooksService(requestContext).UpdateSubscription(requestContext, newSubscription);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062290, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062300, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (UpdateSubscription));
      }
    }

    protected virtual void ValidateSubscriptionInput(
      IVssRequestContext requestContext,
      EventTypeDescriptor eventType,
      string inputId,
      string inputValue,
      string scope)
    {
      requestContext.TraceEnter(1062340, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (ValidateSubscriptionInput));
      try
      {
        (this.GetInputDescriptor(requestContext, eventType, inputId) ?? throw new SubscriptionInputException(string.Format(ServiceHooksPublisherResources.Error_UnknownPublisherInputFormat, (object) inputId, (object) this.Name, (object) eventType.Name))).ValidateInternal(requestContext, inputValue, scope);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062350, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062360, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (ValidateSubscriptionInput));
      }
    }

    protected virtual void ValidateSubscriptionInputs(
      IVssRequestContext requestContext,
      string eventType,
      IDictionary<string, string> subscriptionPublisherInputs)
    {
      requestContext.TraceEnter(1062370, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (ValidateSubscriptionInputs));
      try
      {
        EventTypeDescriptor eventDescriptor = this.GetEventDescriptor(requestContext, eventType);
        if (eventDescriptor == null)
          throw new SubscriptionInputException(string.Format(ServiceHooksPublisherResources.Error_InvalidEventIdFormat, (object) eventType, (object) this.Name));
        string scope = subscriptionPublisherInputs.ContainsKey("projectId") ? "project" : "collection";
        foreach (KeyValuePair<string, string> subscriptionPublisherInput in (IEnumerable<KeyValuePair<string, string>>) subscriptionPublisherInputs)
          this.ValidateSubscriptionInput(requestContext, eventDescriptor, subscriptionPublisherInput.Key, subscriptionPublisherInput.Value, scope);
        if (this.InputDescriptors != null)
        {
          foreach (InputDescriptor inputDescriptor in this.InputDescriptors.Where<InputDescriptor>((Func<InputDescriptor, bool>) (des => des.Validation != null && des.Validation.IsRequired)))
          {
            if (!subscriptionPublisherInputs.ContainsKey(inputDescriptor.Id))
              throw new SubscriptionInputException(string.Format(ServiceHooksPublisherResources.Error_MissingRequiredPublisherInputFormat, (object) inputDescriptor.Id, (object) inputDescriptor.Name, (object) this.Name, (object) eventDescriptor.Name));
          }
        }
        if (eventDescriptor.InputDescriptors == null)
          return;
        foreach (InputDescriptor inputDescriptor in eventDescriptor.InputDescriptors.Where<InputDescriptor>((Func<InputDescriptor, bool>) (des => des.Validation != null && des.Validation.IsRequired)))
        {
          if (!subscriptionPublisherInputs.ContainsKey(inputDescriptor.Id))
            throw new SubscriptionInputException(string.Format(ServiceHooksPublisherResources.Error_MissingRequiredPublisherInputFormat, (object) inputDescriptor.Id, (object) inputDescriptor.Name, (object) this.Name, (object) eventDescriptor.Name));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062380, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062390, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (ValidateSubscriptionInputs));
      }
    }

    protected EventTypeDescriptor GetEventDescriptor(
      IVssRequestContext requestContext,
      string eventType)
    {
      requestContext.TraceEnter(1062400, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetEventDescriptor));
      try
      {
        EventTypeDescriptor eventDescriptor = (EventTypeDescriptor) null;
        IEnumerable<EventTypeDescriptor> supportedEvents = this.GetSupportedEvents(requestContext);
        if (supportedEvents != null)
          eventDescriptor = supportedEvents.FirstOrDefault<EventTypeDescriptor>((Func<EventTypeDescriptor, bool>) (e => string.Equals(eventType, e.Id)));
        return eventDescriptor;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062410, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062420, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetEventDescriptor));
      }
    }

    protected InputDescriptor GetInputDescriptor(
      IVssRequestContext requestContext,
      EventTypeDescriptor eventTypeDescriptor,
      string inputId)
    {
      requestContext.TraceEnter(1062430, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetInputDescriptor));
      try
      {
        IEnumerable<InputDescriptor> inputDescriptors = this.InputDescriptors;
        if (inputDescriptors != null)
        {
          InputDescriptor inputDescriptor = inputDescriptors.FirstOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (i => string.Equals(i.Id, inputId)));
          if (inputDescriptor != null)
            return inputDescriptor;
        }
        if (eventTypeDescriptor.InputDescriptors != null)
        {
          InputDescriptor inputDescriptor = eventTypeDescriptor.InputDescriptors.FirstOrDefault<InputDescriptor>((Func<InputDescriptor, bool>) (i => string.Equals(i.Id, inputId)));
          if (inputDescriptor != null)
            return inputDescriptor;
        }
        return (InputDescriptor) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062440, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062450, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetInputDescriptor));
      }
    }

    public virtual IList<InputValues> GetInputValues(
      IVssRequestContext requestContext,
      IEnumerable<string> inputIds,
      IDictionary<string, string> currentValues)
    {
      requestContext.TraceEnter(1062460, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, "GetSubscriptionInputValues");
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<IEnumerable<string>>(inputIds, nameof (inputIds));
        List<InputValues> inputValues1 = new List<InputValues>();
        foreach (string inputId in inputIds)
        {
          InputValues inputValues2 = this.GetInputValues(requestContext, inputId, currentValues) ?? new InputValues();
          inputValues2.InputId = inputId;
          inputValues1.Add(inputValues2);
        }
        return (IList<InputValues>) inputValues1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062470, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062480, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, "GetSubscriptionInputValues");
      }
    }

    public abstract InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentValues);

    public IList<InputValues> GetSubscriptionInputValues(
      IVssRequestContext requestContext,
      string eventType,
      IDictionary<string, string> currentPublisherInputValues,
      IEnumerable<string> inputIds)
    {
      requestContext.TraceEnter(1062460, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetSubscriptionInputValues));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckStringForNullOrEmpty(eventType, nameof (eventType));
        ArgumentUtility.CheckForNull<IEnumerable<string>>(inputIds, nameof (inputIds));
        List<InputValues> subscriptionInputValues = new List<InputValues>();
        foreach (string inputId in inputIds)
        {
          InputValues inputValues = this.GetSubscriptionInputValues(requestContext, eventType, currentPublisherInputValues, inputId) ?? new InputValues();
          inputValues.InputId = inputId;
          subscriptionInputValues.Add(inputValues);
        }
        return (IList<InputValues>) subscriptionInputValues;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062470, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062480, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetSubscriptionInputValues));
      }
    }

    protected virtual InputValues GetSubscriptionInputValues(
      IVssRequestContext requestContext,
      string eventType,
      IDictionary<string, string> currentPublisherInputValues,
      string inputId)
    {
      requestContext.TraceEnter(1062490, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetSubscriptionInputValues));
      try
      {
        return this.GetRegistrationForServiceHookEventType(requestContext, eventType).GetSubscriptionInputValues(requestContext, inputId, currentPublisherInputValues);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1062500, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1062510, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, nameof (GetSubscriptionInputValues));
      }
    }

    public virtual IServiceHooksPublisherEventRegistration GetRegistrationForServiceHookEventType(
      IVssRequestContext requestContext,
      string serviceHooksEventTypeId,
      bool throwIfNotFound = true)
    {
      IServiceHooksPublisherEventRegistration serviceHookEventType = (IServiceHooksPublisherEventRegistration) null;
      if (!this.m_eventRegistrationsByServiceHookEventType.TryGetValue(serviceHooksEventTypeId, out serviceHookEventType) & throwIfNotFound)
        throw new InvalidOperationException(string.Format(ServiceHooksPublisherResources.Error_NoRegisteredHandlerForSHEventType, (object) serviceHooksEventTypeId));
      return serviceHookEventType;
    }

    public virtual PublisherEvent HandleReceivedEvent(
      IVssRequestContext requestContext,
      string eventType,
      object payload,
      string channel = null,
      ServiceHooksChannelMetadata metadata = null,
      int notificationId = 0,
      string notificationSubscriptionId = null)
    {
      requestContext.TraceEnter(1061400, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, "PublishServiceHookEvents");
      try
      {
        PublisherEvent publisherEvent = this.ProcessReceivedEvent(requestContext, eventType, payload, DateTime.MinValue, DateTime.MinValue, channel, metadata, notificationId, notificationSubscriptionId);
        if (publisherEvent != null)
          this.GetHooksService(requestContext).PublishEvents(requestContext, (IEnumerable<PublisherEvent>) new PublisherEvent[1]
          {
            publisherEvent
          });
        return publisherEvent;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061410, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061440, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, "PublishServiceHookEvents");
      }
    }

    public virtual PublisherEvent ProcessReceivedEvent(
      IVssRequestContext requestContext,
      string eventType,
      object payload,
      DateTime eventCreatedTime,
      DateTime notificationCreatedTime,
      string channel = null,
      ServiceHooksChannelMetadata metadata = null,
      int notificationId = 0,
      string notificationSubscriptionId = null,
      int eventId = 0)
    {
      requestContext.TraceEnter(1061400, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, "PublishServiceHookEvents");
      try
      {
        IServiceHooksPublisherEventRegistration eventRegistration;
        if (this.m_eventRegistrationsByPublisherEventType.TryGetValue(eventType, out eventRegistration))
        {
          Dictionary<string, string> contextData = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCulture);
          contextData.Add(nameof (channel), channel);
          contextData.Add(nameof (eventId), eventId.ToString("D"));
          DateTime utcNow1 = DateTime.UtcNow;
          ServiceHooksPublisherEventData payloadFromEvent = eventRegistration.GetPayloadFromEvent(requestContext, payload, metadata, (IDictionary<string, string>) contextData);
          DateTime utcNow2 = DateTime.UtcNow;
          if (payloadFromEvent != null)
          {
            if (payloadFromEvent.Diagnostics == null)
              payloadFromEvent.Diagnostics = (IDictionary<string, string>) new Dictionary<string, string>();
            payloadFromEvent.Diagnostics.Add("EventType", eventType);
            payloadFromEvent.Diagnostics.Add("HooksEventType", payloadFromEvent.EventType);
            payloadFromEvent.Diagnostics.Add("Channel", channel);
            payloadFromEvent.Diagnostics.Add("EventId", eventId.ToString("D"));
            payloadFromEvent.Diagnostics.Add("NotificationId", notificationId.ToString("D"));
            payloadFromEvent.Diagnostics.Add("NotificationSubscriptionId", notificationSubscriptionId);
            payloadFromEvent.Diagnostics.Add("EventCreatedTime", eventCreatedTime.ToString("o"));
            payloadFromEvent.Diagnostics.Add("NotificationCreatedTime", notificationCreatedTime.ToString("o"));
            payloadFromEvent.Diagnostics.Add("ServiceHooksProcessingStartTime", utcNow1.ToString("o"));
            payloadFromEvent.Diagnostics.Add("PayloadCreatedTime", utcNow2.ToString("o"));
            return this.ToPublisherEvent(payloadFromEvent, requestContext, channel);
          }
          requestContext.Trace(1061410, TraceLevel.Warning, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, string.Format("No payload generated for event type '{0}'. Notification id is {1}.", (object) eventType, (object) notificationId));
        }
        else
          requestContext.Trace(1061410, TraceLevel.Error, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, string.Format("Event registration not found for event type '{0}'.  Have {1} registered event types for publisher '{2}'.  Notification id is {3}.", (object) eventType, (object) this.m_eventRegistrationsByPublisherEventType.Count, (object) this.Id, (object) notificationId));
        return (PublisherEvent) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1061410, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1061440, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, "PublishServiceHookEvents");
      }
    }

    public virtual PublisherEvent ToPublisherEvent(
      ServiceHooksPublisherEventData eventData,
      IVssRequestContext requestContext,
      string channel)
    {
      SortedList<string, object> payloads = eventData.Payloads;
      if (!payloads.Any<KeyValuePair<string, object>>())
        return (PublisherEvent) null;
      string key = payloads.Keys[0];
      object currentPayload = payloads[key];
      List<VersionedResource> otherResources = payloads.GetOtherResources(key, currentPayload);
      PublisherEvent publisherEvent = new PublisherEvent()
      {
        Event = new Event()
        {
          EventType = eventData.EventType,
          Resource = currentPayload,
          ResourceVersion = key,
          ResourceContainers = (IDictionary<string, ResourceContainer>) eventData.ResourceContainers,
          PublisherId = this.Id,
          Message = eventData.Message,
          DetailedMessage = eventData.DetailedMessage
        },
        OtherResourceVersions = (IEnumerable<VersionedResource>) otherResources,
        Diagnostics = eventData.Diagnostics
      };
      publisherEvent.NotificationData = eventData.NotificationData;
      InputFilterCondition subscriptionFilterCondition = this.GetPublisherSubscriptionFilterCondition(channel);
      if (subscriptionFilterCondition != null)
        publisherEvent.PublisherInputFilters = (IEnumerable<InputFilter>) subscriptionFilterCondition.ToFilters();
      return publisherEvent;
    }

    public virtual InputFilterCondition GetPublisherSubscriptionFilterCondition(string channel) => (InputFilterCondition) null;

    private IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> GetLocalSubscriptions(
      IVssRequestContext requestContext,
      SubscriptionsQuery query,
      bool unmaskConfidentialInputs)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      INotificationSubscriptionService service = requestContext.GetService<INotificationSubscriptionService>();
      List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> localSubscriptions = new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
      foreach (string hooksDeliveryChannel in NotificationFrameworkConstants.LocalServiceHooksDeliveryChannels)
      {
        SubscriptionLookup anyFieldLookup = SubscriptionLookup.CreateAnyFieldLookup(channel: hooksDeliveryChannel);
        IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> collection = service.QuerySubscriptions(requestContext1, anyFieldLookup).Select<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) (ls => ls.GetHooksSubscription())).Where<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, bool>) (hs => hs.Matches(query)));
        localSubscriptions.AddRange(collection);
      }
      foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in localSubscriptions)
        SubscriptionInputValueStrongBoxHelper.AddConfidentialInputValuesToSubscription(requestContext, subscription, !unmaskConfidentialInputs);
      return (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) localSubscriptions;
    }

    private static (IReadOnlyList<IServiceHooksPublisherEventRegistration> EventRegistrations, Dictionary<string, IServiceHooksPublisherEventRegistration> EventRegistrationsByPublisherEventType, Dictionary<string, IServiceHooksPublisherEventRegistration> EventRegistrationsByServiceHookEventType) GetExtensions(
      IVssRequestContext requestContext,
      string id)
    {
      IReadOnlyList<IServiceHooksPublisherEventRegistration> list = (IReadOnlyList<IServiceHooksPublisherEventRegistration>) requestContext.GetExtensions<IServiceHooksPublisherEventRegistration>(ExtensionLifetime.Service).Where<IServiceHooksPublisherEventRegistration>((Func<IServiceHooksPublisherEventRegistration, bool>) (r => string.Equals(r.PublisherId, id, StringComparison.InvariantCultureIgnoreCase))).ToList<IServiceHooksPublisherEventRegistration>();
      int count = list.Count;
      requestContext.Trace(1062010, TraceLevel.Info, ServiceHooksPublisher.s_area, ServiceHooksPublisher.s_layer, string.Format("Loaded {0} service hooks event registration plugins for publisher {1}", (object) count, (object) id));
      Dictionary<string, IServiceHooksPublisherEventRegistration> dictionary1 = new Dictionary<string, IServiceHooksPublisherEventRegistration>(count);
      foreach (IServiceHooksPublisherEventRegistration eventRegistration in (IEnumerable<IServiceHooksPublisherEventRegistration>) list)
      {
        if (dictionary1.ContainsKey(eventRegistration.PublisherEventType))
          throw new InvalidOperationException(string.Format(ServiceHooksPublisherResources.Error_DuplicateEventTypeRegistrationFormat, (object) eventRegistration.PublisherEventType));
        dictionary1.Add(eventRegistration.PublisherEventType, eventRegistration);
      }
      Dictionary<string, IServiceHooksPublisherEventRegistration> dictionary2 = new Dictionary<string, IServiceHooksPublisherEventRegistration>(count);
      foreach (IServiceHooksPublisherEventRegistration eventRegistration in (IEnumerable<IServiceHooksPublisherEventRegistration>) list)
      {
        foreach (EventTypeDescriptor descriptor in eventRegistration.GetDescriptors(requestContext))
          dictionary2[descriptor.Id] = eventRegistration;
      }
      return (list, dictionary1, dictionary2);
    }
  }
}
