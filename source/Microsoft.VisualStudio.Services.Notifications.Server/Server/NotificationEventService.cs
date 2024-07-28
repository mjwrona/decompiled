// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationEventService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class NotificationEventService : 
    INotificationEventServiceInternal,
    INotificationEventService,
    IVssFrameworkService
  {
    private bool m_skipSubscriptionAdapters;
    private PublishEventStats m_publishEventStats = new PublishEventStats();
    private int m_notificationJobDelay;
    private Dictionary<Guid, IgnoreSubscription> m_ignoreSubscriptions;
    private DateTime m_lastStatusJobQueueTime = DateTime.MinValue;
    private int m_notificationBacklogStatusJobDelay;
    private int m_maxEventLength = 50000000;
    private int m_eventExpirationSeconds = 1209600;
    private ConcurrentDictionary<string, byte> m_supportedEventTypes = new ConcurrentDictionary<string, byte>();
    private int m_eventCreatedDeltaTriggerMilliseconds = 600000;
    private int m_eventSizeTraceLimit = 4000000;
    private static readonly TeamFoundationJobReference s_notificationBacklogStatusJobReference = new TeamFoundationJobReference(new Guid("EAB3CE0C-A4F4-4460-82A0-6CF6E8BF0A8B"), JobPriorityClass.AboveNormal);
    private static readonly TeamFoundationJobReference s_notificationJobReference = new TeamFoundationJobReference(new Guid("A4804DCF-4BB6-4109-B61C-E59C2E8A9FF7"), JobPriorityClass.High);
    internal static readonly string s_eventDefinitionsDetailsDataName = "Microsoft.VisualStudio.Services.Notifications.Server.EventDefinitionsDetails";
    private static readonly string s_eventDefinitionsDataName = "Microsoft.VisualStudio.Services.Notifications.Server.EventDefinitions";
    internal static readonly string s_eventTypeIdDataName = "Microsoft.VisualStudio.Services.Notifications.Server.EventTypeId";
    private static readonly string s_fieldTypesDataName = "Microsoft.VisualStudio.Services.Notifications.Server.FieldTypes";
    private static readonly string s_publishersDataName = "Microsoft.VisualStudio.Services.Notifications.Server.EventPublishers";
    private static readonly string s_gitCommentsProcessingJob = "AzureDevOps.Services.Notifications.GitCommentsProcessingJob";
    private static readonly string GitPullRequestCommentEvent = "ms.vss-code.git-pullrequest-comment-event";
    private const int c_defaultNotificationJobDelay = 1;
    private const int c_defaultRegexTimeoutSecs = 5;
    private const int c_defaultNotificationBacklogStatusJobDelay = 300;
    private const int c_defaultEventExpirationSeconds = 604800;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ClearIgnoreConditions), FrameworkServerConstants.IgnoreSubscriptionsRootPath + "/**");
      RegistryEntryCollection registryEntries = service.ReadEntriesFallThru(systemRequestContext, (RegistryQuery) (FrameworkServerConstants.NotificationRootPath + "/**"));
      this.m_notificationJobDelay = registryEntries.GetValueFromPath<int>(FrameworkServerConstants.NotificationJobDelay, 1);
      this.m_notificationBacklogStatusJobDelay = registryEntries.GetValueFromPath<int>(NotificationFrameworkConstants.NotificationBacklogStatusJobDelay, 300);
      this.m_skipSubscriptionAdapters = systemRequestContext.IsFeatureEnabled("Notifications.SkipSubscriptionAdapters");
      this.m_eventExpirationSeconds = registryEntries.GetValueFromPath<int>(NotificationFrameworkConstants.EventExpirationSeconds, 604800);
      this.m_eventCreatedDeltaTriggerMilliseconds = registryEntries.GetValueFromPath<int>(NotificationFrameworkConstants.EventCreatedDeltaTrigger, this.m_eventCreatedDeltaTriggerMilliseconds);
      this.m_publishEventStats.Interval = registryEntries.GetValueFromPath<int>(NotificationFrameworkConstants.PublishEventsRate, this.m_publishEventStats.Interval);
      this.m_eventSizeTraceLimit = registryEntries.GetValueFromPath<int>(NotificationFrameworkConstants.NotificationEventSizeTraceLimit, this.m_eventSizeTraceLimit);
      this.ReadIgnoreConditions(systemRequestContext, registryEntries);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ReadIgnoreConditions));
      this.m_publishEventStats.Stop(systemRequestContext);
    }

    private Dictionary<string, NotificationEventType> QueryEventTypesDetails(
      IVssRequestContext requestContext)
    {
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      IEnumerable<string> contributionIds = (IEnumerable<string>) new string[2]
      {
        NotificationClientConstants.EventPublisherContribution,
        NotificationClientConstants.CategoryContributionRoot
      };
      ContributionQueryOptions queryOptions = ContributionQueryOptions.IncludeAll;
      string cacheKey = ContributionUtils.GetCacheKey(NotificationEventService.s_eventDefinitionsDetailsDataName, contributionIds, (HashSet<string>) null, new ContributionQueryOptions?(queryOptions));
      IEnumerable<Contribution> contributions;
      Dictionary<string, NotificationEventType> associatedData;
      if (!service.QueryContributions<Dictionary<string, NotificationEventType>>(requestContext, contributionIds, (HashSet<string>) null, queryOptions, (ContributionQueryCallback) null, (ContributionDiagnostics) null, cacheKey, out contributions, out associatedData))
      {
        Dictionary<string, NotificationEventPublisher> dictionary1 = new Dictionary<string, NotificationEventPublisher>();
        Dictionary<string, NotificationEventType> eventTypes = new Dictionary<string, NotificationEventType>();
        Dictionary<string, NotificationEventType> dictionary2 = new Dictionary<string, NotificationEventType>();
        Dictionary<string, NotificationEventField> fields = new Dictionary<string, NotificationEventField>();
        Dictionary<string, Contribution> eventTypesContributions = new Dictionary<string, Contribution>();
        List<string> stringList = new List<string>();
        requestContext.WarnIfContributionsInFallbackMode(nameof (QueryEventTypesDetails));
        if (contributions != null)
        {
          this.ClassifyContributions(requestContext, service, contributions, dictionary1, eventTypes, eventTypesContributions, fields, stringList);
          Dictionary<string, NotificationEventFieldType> fieldTypes = this.GetFieldTypes(requestContext, service, stringList);
          foreach (NotificationEventType eventType in eventTypes.Values)
          {
            this.PopulateEventFields(eventType, fields, fieldTypes);
            dictionary2.Add(eventType.Id, eventType);
          }
          if (requestContext.ExecutionEnvironment.IsHostedDeployment)
            dictionary1 = this.FilterServiceIntancePublishers(requestContext, dictionary1);
          associatedData = this.PopulateEventPublishers(eventTypesContributions, dictionary2.Values.ToList<NotificationEventType>(), dictionary1);
          service.Set(requestContext, cacheKey, contributions, (object) associatedData);
        }
      }
      return associatedData ?? new Dictionary<string, NotificationEventType>();
    }

    public List<NotificationEventType> GetEventTypes(
      IVssRequestContext requestContext,
      EventTypeQueryFlags queryFlags = EventTypeQueryFlags.None)
    {
      List<NotificationEventType> eventTypes = new List<NotificationEventType>();
      Dictionary<string, NotificationEventType> dictionary = !queryFlags.HasFlag((Enum) EventTypeQueryFlags.IncludeFields) ? this.QueryEventTypes(requestContext) : this.QueryEventTypesDetails(requestContext);
      if (dictionary != null)
      {
        eventTypes = new List<NotificationEventType>((IEnumerable<NotificationEventType>) dictionary.Values.Select<NotificationEventType, NotificationEventType>((Func<NotificationEventType, NotificationEventType>) (x => x.ShallowClone())).ToList<NotificationEventType>());
        this.PopulateDynamicRoles(requestContext, eventTypes);
      }
      return eventTypes;
    }

    public Dictionary<string, NotificationEventType> GetKeyedEventTypes(
      IVssRequestContext requestContext)
    {
      Dictionary<string, NotificationEventType> keyedEventTypes = new Dictionary<string, NotificationEventType>();
      foreach (NotificationEventType notificationEventType in this.QueryEventTypes(requestContext).Values)
        keyedEventTypes.Add(notificationEventType.Id, notificationEventType.ShallowClone());
      return keyedEventTypes;
    }

    public IEnumerable<NotificationEventPublisher> GetPublishers(
      IVssRequestContext requestContext,
      EventPublisherQueryFlags queryFlags)
    {
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      string publishersDataName = NotificationEventService.s_publishersDataName;
      HashSet<string> contributionTypes = new HashSet<string>();
      contributionTypes.Add("ms.vss-notifications.event-publisher");
      ContributionQueryOptions? queryOptions = new ContributionQueryOptions?();
      string cacheKey = ContributionUtils.GetCacheKey(publishersDataName, (IEnumerable<string>) null, contributionTypes, queryOptions);
      IEnumerable<Contribution> contributions;
      List<NotificationEventPublisher> associatedData;
      if (!service.QueryContributionsForType<List<NotificationEventPublisher>>(requestContext, "ms.vss-notifications.event-publisher", cacheKey, out contributions, out associatedData))
      {
        requestContext.WarnIfContributionsInFallbackMode(nameof (GetPublishers));
        if (contributions != null)
        {
          associatedData = new List<NotificationEventPublisher>();
          foreach (Contribution contribution in contributions)
          {
            NotificationEventPublisher eventPublisher = this.CreateEventPublisher(contribution);
            associatedData.Add(eventPublisher);
          }
          service.Set(requestContext, cacheKey, contributions, (object) associatedData);
        }
      }
      bool flag = !queryFlags.HasFlag((Enum) EventPublisherQueryFlags.IncludeRemoteServices);
      if (associatedData != null & flag && requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        Guid currentServiceInstanceType = requestContext.ServiceInstanceType();
        associatedData = associatedData.Where<NotificationEventPublisher>((Func<NotificationEventPublisher, bool>) (p => this.ServiceInstanceTypeMatches(currentServiceInstanceType, p))).ToList<NotificationEventPublisher>();
      }
      return (IEnumerable<NotificationEventPublisher>) associatedData ?? (IEnumerable<NotificationEventPublisher>) new List<NotificationEventPublisher>();
    }

    private Dictionary<string, NotificationEventType> QueryEventTypes(
      IVssRequestContext requestContext)
    {
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      Dictionary<string, NotificationEventType> associatedData = new Dictionary<string, NotificationEventType>();
      IEnumerable<string> contributionIds = (IEnumerable<string>) new string[2]
      {
        NotificationClientConstants.EventPublisherContribution,
        NotificationClientConstants.CategoryContributionRoot
      };
      HashSet<string> contributionTypes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        "ms.vss-notifications.event-publisher",
        "ms.vss-notifications.event-type",
        "ms.vss-notifications.legacy-event-type",
        "ms.vss-notifications.event-category"
      };
      ContributionQueryOptions queryOptions = ContributionQueryOptions.IncludeAll;
      string cacheKey = ContributionUtils.GetCacheKey(NotificationEventService.s_eventDefinitionsDataName, contributionIds, contributionTypes, new ContributionQueryOptions?(queryOptions));
      IEnumerable<Contribution> contributions;
      if (!service.QueryContributions<List<NotificationEventType>>(requestContext, contributionIds, contributionTypes, queryOptions, (ContributionQueryCallback) null, (ContributionDiagnostics) null, cacheKey, out contributions, out List<NotificationEventType> _))
      {
        Dictionary<string, NotificationEventPublisher> eventsPublishers = new Dictionary<string, NotificationEventPublisher>();
        Dictionary<string, Contribution> eventTypesContributions = new Dictionary<string, Contribution>();
        requestContext.WarnIfContributionsInFallbackMode(nameof (QueryEventTypes));
        if (contributions != null)
        {
          List<NotificationEventType> eventTypes = new List<NotificationEventType>();
          Dictionary<string, NotificationEventTypeCategory> categories = new Dictionary<string, NotificationEventTypeCategory>();
          this.GetEventTypeCategories(contributions, categories);
          foreach (Contribution contribution in contributions)
          {
            if (!string.IsNullOrEmpty(contribution.Type))
            {
              if (this.IsEventTypeContribution(contribution.Type))
              {
                if (contribution.Targets != null)
                {
                  NotificationEventType eventDefinition = this.CreateEventDefinition(requestContext, contribution);
                  eventDefinition.Category = this.GetEventTypeCategory(contribution, categories);
                  eventDefinition.Url = this.GetEventTypeRestURL(requestContext, eventDefinition);
                  eventTypes.Add(eventDefinition);
                  eventTypesContributions.Add(eventDefinition.Id, contribution);
                }
              }
              else if (contribution.Type.Equals("ms.vss-notifications.event-publisher", StringComparison.OrdinalIgnoreCase))
              {
                NotificationEventPublisher eventPublisher = this.CreateEventPublisher(contribution);
                eventsPublishers.Add(contribution.Id, eventPublisher);
              }
            }
          }
          if (requestContext.ExecutionEnvironment.IsHostedDeployment)
            eventsPublishers = this.FilterServiceIntancePublishers(requestContext, eventsPublishers);
          associatedData = this.PopulateEventPublishers(eventTypesContributions, eventTypes, eventsPublishers);
          service.Set(requestContext, cacheKey, contributions, (object) associatedData);
        }
      }
      return associatedData;
    }

    private void PopulateDynamicRoles(
      IVssRequestContext requestContext,
      List<NotificationEventType> eventTypes)
    {
      foreach (NotificationEventType eventType in eventTypes)
      {
        if (eventType.HasDynamicRoles)
        {
          List<NotificationEventRole> dynamicEventTypeRoles = this.GetDynamicEventTypeRoles(requestContext, eventType.Id, "XPathMatcher");
          eventType.Roles.AddRange((IEnumerable<NotificationEventRole>) dynamicEventTypeRoles);
          if (!eventType.HasGroupRoles && dynamicEventTypeRoles.Any<NotificationEventRole>())
          {
            foreach (NotificationEventRole notificationEventRole in dynamicEventTypeRoles)
            {
              if (notificationEventRole.SupportsGroups)
              {
                eventType.HasGroupRoles = true;
                break;
              }
            }
          }
        }
      }
    }

    public NotificationEventType GetEventType(
      IVssRequestContext requestContext,
      string eventType,
      EventTypeQueryFlags queryFlags = EventTypeQueryFlags.None)
    {
      eventType = EventTypeMapper.ToContributed(requestContext, eventType);
      Dictionary<string, NotificationEventType> dictionary = this.QueryEventTypesDetails(requestContext);
      return dictionary.ContainsKey(eventType) ? dictionary[eventType] : (NotificationEventType) null;
    }

    public IEnumerable<NotificationEventType> GetPublisherEventTypes(
      IVssRequestContext requestContext,
      string publisherId)
    {
      return this.GetEventTypes(requestContext, EventTypeQueryFlags.None).Where<NotificationEventType>((Func<NotificationEventType, bool>) (s => s.EventPublisher.Id.Equals(publisherId, StringComparison.OrdinalIgnoreCase)));
    }

    public void PublishEvent(IVssRequestContext requestContext, VssNotificationEvent theEvent)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<VssNotificationEvent>(theEvent, nameof (theEvent));
      this.PublishEvents(requestContext, (IEnumerable<VssNotificationEvent>) new VssNotificationEvent[1]
      {
        theEvent
      }, false);
    }

    public void PublishEvents(
      IVssRequestContext requestContext,
      IEnumerable<VssNotificationEvent> theEvents,
      bool allowDuringServicing = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) theEvents, nameof (theEvents));
      if (requestContext.IsServicingContext && !allowDuringServicing)
        return;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId);
      if (securityNamespace != null)
        securityNamespace.CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 16, false);
      this.EnsureIgnoreConditionsLoaded(requestContext);
      HashSet<string> processQueues;
      List<SerializedNotificationEvent> notificationEventList = this.PrepareAndFilterEvents(requestContext, theEvents, out processQueues);
      if (notificationEventList.Count <= 0)
        return;
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
        component.FireEvents((IEnumerable<SerializedNotificationEvent>) notificationEventList);
      this.ScheduleProcessingJobs(requestContext, processQueues, notificationEventList);
      this.QueueNotificationBacklogStatusJob(requestContext);
    }

    private void ScheduleProcessingJobs(
      IVssRequestContext requestContext,
      HashSet<string> processQueues,
      List<SerializedNotificationEvent> convertedEvents)
    {
      if (!processQueues.Any<string>())
      {
        requestContext.Trace(1002017, TraceLevel.Error, NotificationEventService.Area, NotificationEventService.Layer, string.Format("No process queues were found but we've got {0} pending events", (object) convertedEvents.Count));
      }
      else
      {
        using (IDisposableReadOnlyList<INotificationProcessingJob> extensions = requestContext.GetExtensions<INotificationProcessingJob>())
        {
          INotificationProcessingJob notificationProcessingJob1 = (INotificationProcessingJob) null;
          Dictionary<string, INotificationProcessingJob> dictionary = new Dictionary<string, INotificationProcessingJob>();
          foreach (INotificationProcessingJob notificationProcessingJob2 in (IEnumerable<INotificationProcessingJob>) extensions)
          {
            if (notificationProcessingJob2.ProcessQueue == "*")
              notificationProcessingJob1 = notificationProcessingJob2;
            else if (processQueues.Contains(notificationProcessingJob2.ProcessQueue) && notificationProcessingJob2.CanBeQueued(requestContext))
              dictionary[notificationProcessingJob2.ProcessQueue] = notificationProcessingJob2;
          }
          INotificationJobService service = requestContext.GetService<INotificationJobService>();
          foreach (INotificationProcessingJob notificationProcessingJob3 in dictionary.Values)
            service.QueueDelayedJob(requestContext, notificationProcessingJob3.JobId, this.m_notificationJobDelay, notificationProcessingJob3.PriorityClass, notificationProcessingJob3.PriorityLevel);
          if (dictionary.Count >= processQueues.Count)
            return;
          if (notificationProcessingJob1 != null)
            service.QueueDelayedJob(requestContext, notificationProcessingJob1.JobId, this.m_notificationJobDelay, notificationProcessingJob1.PriorityClass, notificationProcessingJob1.PriorityLevel);
          else
            requestContext.Trace(1002303, TraceLevel.Error, NotificationEventService.Area, NotificationEventService.Layer, "Could not find Wildcard Notification Processing job!");
        }
      }
    }

    public EventSerializerType GetSerializationFormatForEvent(
      IVssRequestContext requestContext,
      string eventType)
    {
      eventType = EventTypeMapper.ToContributed(requestContext, eventType);
      EventTypeSerializationFormatCache service = requestContext.GetService<EventTypeSerializationFormatCache>();
      EventSerializerType serializationFormatForEvent1;
      if (service.TryGetValue(requestContext, eventType, out serializationFormatForEvent1))
        return serializationFormatForEvent1;
      NotificationEventType eventType1 = this.GetEventType(requestContext, eventType, EventTypeQueryFlags.None);
      EventSerializerType serializationFormatForEvent2 = eventType1 != null ? eventType1.SerializationFormat : EventSerializerType.Json;
      service.Set(requestContext, eventType, serializationFormatForEvent2);
      return serializationFormatForEvent2;
    }

    public IList<NotificationEventField> GetInputValues(
      IVssRequestContext requestContext,
      string eventType,
      FieldValuesQuery query)
    {
      return this.m_skipSubscriptionAdapters ? this.GetFieldsNoAdapters(requestContext, eventType, query, query.InputValues == null || query.InputValues.Count == 0) : this.GetFieldsWithAdapters(requestContext, eventType, query);
    }

    private static FieldInputValues GetFieldInputValues(ExpressionFilterField field)
    {
      FieldInputValues fieldInputValues = new FieldInputValues();
      fieldInputValues.InputId = field.InvariantFieldName;
      fieldInputValues.Operators = field.RawOperators.ToList<byte>();
      fieldInputValues.PossibleValues = (IList<InputValue>) new List<InputValue>();
      return fieldInputValues;
    }

    private IList<NotificationEventField> GetFieldsWithAdapters(
      IVssRequestContext requestContext,
      string eventType,
      FieldValuesQuery query)
    {
      SubscriptionScope scope = NotificationEventService.ExtractScope(query);
      string matcher = this.GetSerializationFormatForEvent(requestContext, eventType) == EventSerializerType.Json ? "JsonPathMatcher" : "XPathMatcher";
      if (!(SubscriptionAdapterFactory.CreateAdapter(requestContext, eventType, matcher, scope) is PathSubscriptionAdapter adapter))
        throw new NotSupportedException();
      return query.InputValues == null || query.InputValues.Count == 0 ? this.GetFields(requestContext, adapter, eventType, query) : this.GetFieldsWithValues(requestContext, adapter, eventType, query);
    }

    private IList<NotificationEventField> GetFields(
      IVssRequestContext requestContext,
      PathSubscriptionAdapter pathAdapter,
      string eventType,
      FieldValuesQuery query)
    {
      SubscriptionScope scope = NotificationEventService.ExtractScope(query);
      Dictionary<string, ExpressionFilterField> fields1 = pathAdapter.GetFields(requestContext, scope);
      List<NotificationEventField> fields2 = new List<NotificationEventField>();
      foreach (ExpressionFilterField field in fields1.Values)
      {
        FieldInputValues fieldInputValues = NotificationEventService.GetFieldInputValues(field);
        SubscriptionFieldType fieldType = field.FieldType;
        NotificationEventField notificationEventField = new NotificationEventField();
        notificationEventField.Name = field.LocalizedFieldName;
        notificationEventField.Id = fieldInputValues.InputId;
        notificationEventField.Path = fieldInputValues.InputId;
        notificationEventField.SupportedScopes = field.SupportedScopes;
        NotificationEventFieldType notificationEventFieldType = new NotificationEventFieldType();
        notificationEventFieldType.Id = fieldType.ToString();
        notificationEventFieldType.Operators = SubscriptionFilterOperators.GetLocalizedFieldOperators((IEnumerable<byte>) fieldInputValues.Operators);
        IEnumerable<string> values = field.GetValues(requestContext);
        if (values != null && values.Count<string>() > 0)
        {
          ValueDefinition valueDefinition = new ValueDefinition();
          valueDefinition.DataSource = new List<InputValue>();
          foreach (string str in values)
            valueDefinition.DataSource.Add(new InputValue()
            {
              Value = str,
              DisplayValue = str
            });
          notificationEventFieldType.Value = valueDefinition;
        }
        notificationEventField.FieldType = notificationEventFieldType;
        fields2.Add(notificationEventField);
      }
      return (IList<NotificationEventField>) fields2;
    }

    public static SubscriptionScope ExtractScope(FieldValuesQuery query)
    {
      SubscriptionScope scope = new SubscriptionScope();
      Guid result = Guid.Empty;
      if (Guid.TryParse(query.Scope, out result))
        scope.Id = result;
      return scope;
    }

    private IList<NotificationEventField> GetFieldsWithValues(
      IVssRequestContext requestContext,
      PathSubscriptionAdapter pathAdapter,
      string eventType,
      FieldValuesQuery query)
    {
      SubscriptionScope scope = NotificationEventService.ExtractScope(query);
      Dictionary<string, ExpressionFilterField> fields = pathAdapter.GetFields(requestContext, scope);
      List<NotificationEventField> fieldsWithValues = new List<NotificationEventField>();
      foreach (FieldInputValues inputValue in (IEnumerable<FieldInputValues>) query.InputValues)
      {
        ExpressionFilterField field;
        if (fields.TryGetValue(inputValue.InputId, out field))
        {
          NotificationEventService.GetFieldInputValues(field);
          fieldsWithValues.Add(pathAdapter.LoadPossibleFieldValues(requestContext, field, scope));
        }
      }
      return (IList<NotificationEventField>) fieldsWithValues;
    }

    private IList<NotificationEventField> GetFieldsNoAdapters(
      IVssRequestContext requestContext,
      string eventType,
      FieldValuesQuery query,
      bool getValues)
    {
      Dictionary<string, NotificationEventField> fields = this.GetEventType(requestContext, eventType, EventTypeQueryFlags.None).Fields;
      List<NotificationEventField> notificationEventFieldList;
      if (fields == null)
      {
        notificationEventFieldList = (List<NotificationEventField>) null;
      }
      else
      {
        Dictionary<string, NotificationEventField>.ValueCollection values = fields.Values;
        notificationEventFieldList = values != null ? values.ToList<NotificationEventField>() : (List<NotificationEventField>) null;
      }
      IList<NotificationEventField> fieldsNoAdapters = (IList<NotificationEventField>) notificationEventFieldList;
      if (getValues && fieldsNoAdapters != null)
      {
        foreach (NotificationEventField notificationEventField in (IEnumerable<NotificationEventField>) fieldsNoAdapters)
        {
          NotificationEventFieldType fieldType = notificationEventField.FieldType;
          if (notificationEventField.FieldType.Value == null)
            notificationEventField.FieldType.Value = new ValueDefinition()
            {
              DataSource = new List<InputValue>()
            };
          else if (notificationEventField.FieldType.Value.DataSource == null)
            notificationEventField.FieldType.Value.DataSource = new List<InputValue>();
          List<InputValue> values = this.GetValues(requestContext, fieldType);
          notificationEventField.FieldType.Value.DataSource.AddRange((IEnumerable<InputValue>) values);
        }
      }
      return fieldsNoAdapters;
    }

    private List<InputValue> GetValues(
      IVssRequestContext requestContext,
      NotificationEventFieldType fieldType)
    {
      return new List<InputValue>();
    }

    public void QueueNotificationBacklogStatusJob(IVssRequestContext requestContext)
    {
      if ((DateTime.UtcNow - this.m_lastStatusJobQueueTime).TotalSeconds <= (double) this.m_notificationBacklogStatusJobDelay)
        return;
      requestContext.GetService<INotificationJobService>().QueueDelayedJob(requestContext, NotificationEventService.s_notificationBacklogStatusJobReference.JobId, this.m_notificationBacklogStatusJobDelay, NotificationEventService.s_notificationBacklogStatusJobReference.PriorityClass, JobPriorityLevel.Normal);
      this.m_lastStatusJobQueueTime = DateTime.UtcNow;
    }

    private void ClearIgnoreConditions(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries)
    {
      this.m_ignoreSubscriptions = (Dictionary<Guid, IgnoreSubscription>) null;
    }

    private void ReadIgnoreConditions(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries)
    {
      Dictionary<Guid, IgnoreSubscription> dictionary = new Dictionary<Guid, IgnoreSubscription>();
      List<Guid> guidList = new List<Guid>();
      foreach (RegistryEntry registryEntry in registryEntries)
      {
        if (registryEntry.Path.StartsWith(FrameworkServerConstants.IgnoreSubscriptionsRootPath))
        {
          try
          {
            Guid subscriptionGuid = this.GetSubscriptionGuid(registryEntry.Path);
            if (subscriptionGuid != Guid.Empty)
            {
              if (!guidList.Contains(subscriptionGuid))
              {
                IgnoreSubscription ignoreSubscription = new IgnoreSubscription(subscriptionGuid);
                guidList.Add(subscriptionGuid);
                string str = FrameworkServerConstants.IgnoreSubscriptionsRootPath + "/" + subscriptionGuid.ToString();
                RegistryEntry entry1;
                if (registryEntries.TryGetValue(str + "/" + IgnoreSubscription.s_eventTypeFieldName, out entry1))
                {
                  ignoreSubscription.EventType = entry1.Value;
                  RegistryEntry entry2;
                  if (registryEntries.TryGetValue(str + "/" + IgnoreSubscription.s_scopeIdentifierFieldName, out entry2))
                    ignoreSubscription.ScopeIdentifier = entry2.Value;
                  RegistryEntry entry3;
                  if (registryEntries.TryGetValue(str + "/" + IgnoreSubscription.s_sourceIdentityFieldName, out entry3) && !string.IsNullOrEmpty(entry3.Value))
                  {
                    string[] source = entry3.Value.Split(new char[1]
                    {
                      ','
                    }, StringSplitOptions.RemoveEmptyEntries);
                    ignoreSubscription.Actors = ((IEnumerable<string>) source).Select<string, Guid>((Func<string, Guid>) (actorId => Guid.Parse(actorId))).ToArray<Guid>();
                  }
                  RegistryEntry entry4;
                  if (registryEntries.TryGetValue(str + "/" + IgnoreSubscription.s_conditionFieldName, out entry4))
                  {
                    TeamFoundationEventConditionParser parser = TeamFoundationEventConditionParser.GetParser(this.GetSerializationFormatForEvent(requestContext, ignoreSubscription.EventType), entry4.Value);
                    try
                    {
                      ignoreSubscription.Condition = parser.Parse();
                    }
                    catch (Exception ex)
                    {
                      requestContext.Trace(1002023, TraceLevel.Error, NotificationEventService.Area, NotificationEventService.Layer, "Failed to parse ignore subscription condition {0}:{1}", (object) entry4.Value, (object) ex.Message);
                      continue;
                    }
                  }
                  RegistryEntry entry5;
                  if (registryEntries.TryGetValue(str + "/" + IgnoreSubscription.s_allowedChannels, out entry5))
                  {
                    try
                    {
                      string[] items = entry5.Value.Split(new char[2]
                      {
                        ',',
                        ';'
                      }, StringSplitOptions.RemoveEmptyEntries);
                      ignoreSubscription.AllowedChannels = ((IEnumerable<string>) items).ToHashSet();
                    }
                    catch (Exception ex)
                    {
                      requestContext.Trace(1002023, TraceLevel.Error, NotificationEventService.Area, NotificationEventService.Layer, "Failed to process enabledChannels {0}:{1}", (object) entry5?.Value, (object) ex.Message);
                      continue;
                    }
                  }
                  if (ignoreSubscription.Condition == null)
                  {
                    if (!((IEnumerable<Guid>) ignoreSubscription.Actors).Any<Guid>())
                      continue;
                  }
                  dictionary.Add(subscriptionGuid, ignoreSubscription);
                }
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.Trace(1002023, TraceLevel.Error, NotificationEventService.Area, NotificationEventService.Layer, "Error when trying to parse ignore subscription with value {0}: {1}", (object) registryEntry.Value, (object) ex.Message);
          }
        }
      }
      this.m_ignoreSubscriptions = dictionary;
    }

    private Guid GetSubscriptionGuid(string path)
    {
      if (string.IsNullOrEmpty(path))
        return Guid.Empty;
      string[] strArray = path.Split('/');
      return strArray == null || strArray.Length < 3 ? Guid.Empty : new Guid(strArray[strArray.Length - 2]);
    }

    private void ClassifyContributions(
      IVssRequestContext requestContext,
      IContributionServiceWithData contributionsService,
      IEnumerable<Contribution> contributions,
      Dictionary<string, NotificationEventPublisher> eventPublishers,
      Dictionary<string, NotificationEventType> eventTypes,
      Dictionary<string, Contribution> eventTypesContributions,
      Dictionary<string, NotificationEventField> fields,
      List<string> fieldsTypes)
    {
      if (contributions == null)
        return;
      Dictionary<string, NotificationEventTypeCategory> categories = new Dictionary<string, NotificationEventTypeCategory>();
      this.GetEventTypeCategories(contributions, categories);
      foreach (Contribution contribution in contributions)
      {
        if (contribution.Type != null)
        {
          if (this.IsEventTypeContribution(contribution.Type))
          {
            NotificationEventType eventDefinition = this.CreateEventDefinition(requestContext, contribution, true);
            eventDefinition.Category = this.GetEventTypeCategory(contribution, categories);
            eventTypes.Add(eventDefinition.Id, eventDefinition);
            eventTypesContributions.Add(eventDefinition.Id, contribution);
            eventDefinition.Url = this.GetEventTypeRestURL(requestContext, eventDefinition);
          }
          else if (contribution.Type.Equals("ms.vss-notifications.event-field", StringComparison.OrdinalIgnoreCase))
          {
            NotificationEventField fieldDefinition = this.CreateFieldDefinition(contribution);
            fields.Add(fieldDefinition.Id, fieldDefinition);
            fieldsTypes.Add(fieldDefinition.FieldTypeId);
          }
          else if (contribution.Type.Equals("ms.vss-notifications.event-publisher", StringComparison.OrdinalIgnoreCase))
          {
            NotificationEventPublisher eventPublisher = this.CreateEventPublisher(contribution);
            eventPublishers.Add(contribution.Id, eventPublisher);
          }
        }
      }
    }

    private void GetEventTypeCategories(
      IEnumerable<Contribution> contributions,
      Dictionary<string, NotificationEventTypeCategory> categories)
    {
      foreach (Contribution contribution in contributions)
      {
        if (contribution.Type != null && contribution.Type.Equals("ms.vss-notifications.event-category", StringComparison.OrdinalIgnoreCase))
        {
          NotificationEventTypeCategory eventTypeCategory = this.CreateEventTypeCategory(contribution);
          categories.Add(eventTypeCategory.Id, eventTypeCategory);
        }
      }
    }

    private NotificationEventTypeCategory GetEventTypeCategory(
      Contribution eventTypeContribution,
      Dictionary<string, NotificationEventTypeCategory> categories)
    {
      IEnumerable<string> source = eventTypeContribution.Targets.Where<string>((Func<string, bool>) (t => categories.ContainsKey(t)));
      return source.Count<string>() == 1 ? categories[source.Single<string>()] : (NotificationEventTypeCategory) null;
    }

    private NotificationEventTypeCategory CreateEventTypeCategory(Contribution contribution) => new NotificationEventTypeCategory()
    {
      Id = contribution.Id,
      Name = contribution.GetProperty<string>(NotificationClientConstants.NameProperty, string.Empty)
    };

    private NotificationEventField CreateFieldDefinition(Contribution contribution) => new NotificationEventField()
    {
      Id = contribution.Id,
      Path = contribution.GetProperty<string>(NotificationClientConstants.PathProperty, string.Empty),
      Name = contribution.GetProperty<string>(NotificationClientConstants.NameProperty, string.Empty),
      FieldTypeId = contribution.GetProperty<string>(NotificationClientConstants.FieldTypeProperty, string.Empty),
      SupportedScopes = this.GetPropertyListValue(contribution, NotificationClientConstants.ScopesProperty)
    };

    private NotificationEventPublisher CreateEventPublisher(Contribution contribution) => new NotificationEventPublisher()
    {
      Id = contribution.Id,
      SubscriptionManagementInfo = {
        ServiceInstanceType = contribution.GetProperty<Guid>(NotificationClientConstants.ServiceInstanceTypeProperty, Guid.Empty)
      }
    };

    private List<string> GetPropertyListValue(
      Contribution contribution,
      string property,
      List<string> defaultValue = null)
    {
      if (contribution.Properties == null)
        return new List<string>();
      List<string> property1 = contribution.GetProperty<List<string>>(property);
      if (property1 != null)
        return property1;
      return defaultValue == null ? new List<string>() : defaultValue;
    }

    private Dictionary<string, NotificationEventPublisher> FilterServiceIntancePublishers(
      IVssRequestContext requestContext,
      Dictionary<string, NotificationEventPublisher> eventsPublishers)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return eventsPublishers;
      Guid currentServiceInstanceType = requestContext.ServiceInstanceType();
      Dictionary<string, NotificationEventPublisher> dictionary = new Dictionary<string, NotificationEventPublisher>();
      foreach (NotificationEventPublisher publisher in eventsPublishers.Values)
      {
        if (this.ServiceInstanceTypeMatches(currentServiceInstanceType, publisher))
          dictionary.Add(publisher.Id, publisher);
      }
      return dictionary;
    }

    private Dictionary<string, NotificationEventFieldType> GetFieldTypes(
      IVssRequestContext requestContext,
      IContributionServiceWithData contributionsService,
      List<string> fieldTypesTargets)
    {
      Dictionary<string, NotificationEventFieldType> associatedData = new Dictionary<string, NotificationEventFieldType>();
      if (fieldTypesTargets == null || fieldTypesTargets.Count == 0)
        return associatedData;
      ContributionQueryOptions queryOptions = ContributionQueryOptions.IncludeSelf;
      string cacheKey = ContributionUtils.GetCacheKey(NotificationEventService.s_fieldTypesDataName, (IEnumerable<string>) fieldTypesTargets, (HashSet<string>) null, new ContributionQueryOptions?(queryOptions));
      IEnumerable<Contribution> contributions;
      if (!contributionsService.QueryContributions<Dictionary<string, NotificationEventFieldType>>(requestContext, (IEnumerable<string>) fieldTypesTargets, (HashSet<string>) null, queryOptions, (ContributionQueryCallback) null, (ContributionDiagnostics) null, cacheKey, out contributions, out associatedData))
      {
        requestContext.WarnIfContributionsInFallbackMode(nameof (GetFieldTypes));
        if (contributions != null)
        {
          associatedData = new Dictionary<string, NotificationEventFieldType>();
          foreach (Contribution contribution in contributions)
          {
            NotificationEventFieldType notificationEventFieldType = new NotificationEventFieldType();
            notificationEventFieldType.Id = contribution.Id;
            notificationEventFieldType.Operators = contribution.GetProperty<List<NotificationEventFieldOperator>>(NotificationClientConstants.OperatorsProperty, new List<NotificationEventFieldOperator>());
            notificationEventFieldType.Operators.ForEach((Action<NotificationEventFieldOperator>) (op => op.DisplayName = SubscriptionFilterOperators.GetLocalizedOperator(op.Id)));
            notificationEventFieldType.Value = contribution.GetProperty<ValueDefinition>(NotificationClientConstants.ValuesProperty);
            string property = contribution.GetProperty<string>(NotificationClientConstants.ValuesProperty, string.Empty);
            bool flag = false;
            ref bool local = ref flag;
            bool.TryParse(property, out local);
            notificationEventFieldType.IsComplex = flag;
            notificationEventFieldType.OperatorConstraints = contribution.GetProperty<List<OperatorConstraint>>(NotificationClientConstants.OperatorsConstraintProperty, new List<OperatorConstraint>());
            associatedData.Add(notificationEventFieldType.Id, notificationEventFieldType);
          }
          contributionsService.Set(requestContext, cacheKey, contributions, (object) associatedData);
        }
      }
      return associatedData;
    }

    private Dictionary<string, NotificationEventType> PopulateEventPublishers(
      Dictionary<string, Contribution> eventTypesContributions,
      List<NotificationEventType> eventTypes,
      Dictionary<string, NotificationEventPublisher> eventsPublishers)
    {
      Dictionary<string, NotificationEventType> dictionary = new Dictionary<string, NotificationEventType>();
      foreach (NotificationEventType eventType in eventTypes)
      {
        foreach (string target in eventTypesContributions[eventType.Id].Targets)
        {
          NotificationEventPublisher notificationEventPublisher;
          if (eventsPublishers.TryGetValue(target, out notificationEventPublisher))
          {
            eventType.EventPublisher = notificationEventPublisher;
            dictionary.Add(eventType.Id, eventType);
            break;
          }
        }
      }
      return dictionary;
    }

    private void PopulateEventFields(
      NotificationEventType eventType,
      Dictionary<string, NotificationEventField> fields,
      Dictionary<string, NotificationEventFieldType> fieldTypes)
    {
      if (eventType.Fields != null)
      {
        foreach (string key in (IEnumerable<string>) eventType.Fields.Keys.ToList<string>())
        {
          NotificationEventField notificationEventField;
          if (fields.TryGetValue(key, out notificationEventField))
          {
            NotificationEventFieldType notificationEventFieldType;
            if (fieldTypes.TryGetValue(notificationEventField.FieldTypeId, out notificationEventFieldType))
              notificationEventField.FieldType = notificationEventFieldType;
            if (notificationEventField.SupportedScopes == null || notificationEventField.SupportedScopes.Count == 0)
              notificationEventField.SupportedScopes = eventType.SupportedScopes;
            if (notificationEventField.FieldType != null)
              eventType.Fields[key] = notificationEventField;
            else
              eventType.Fields.Remove(key);
          }
          else
            eventType.Fields.Remove(key);
        }
        List<KeyValuePair<string, NotificationEventField>> list = eventType.Fields.ToList<KeyValuePair<string, NotificationEventField>>();
        list.Sort((Comparison<KeyValuePair<string, NotificationEventField>>) ((field1, field2) => field1.Value.Name.CompareTo(field2.Value.Name)));
        eventType.Fields = list.ToDictionary<KeyValuePair<string, NotificationEventField>, string, NotificationEventField>((Func<KeyValuePair<string, NotificationEventField>, string>) (t => t.Key), (Func<KeyValuePair<string, NotificationEventField>, NotificationEventField>) (t => t.Value));
      }
      else
        eventType.Fields = new Dictionary<string, NotificationEventField>();
    }

    private NotificationEventType CreateEventDefinition(
      IVssRequestContext requestContext,
      Contribution contribution,
      bool addFields = false)
    {
      NotificationEventType eventDefinition = new NotificationEventType();
      eventDefinition.Id = contribution.Id;
      eventDefinition.Name = contribution.GetProperty<string>(NotificationClientConstants.NameProperty, string.Empty);
      eventDefinition.Alias = contribution.GetProperty<string>(NotificationClientConstants.AliasProperty, string.Empty);
      eventDefinition.HasDynamicRoles = contribution.GetProperty<bool>(NotificationClientConstants.HasDynamicRolesProperty);
      eventDefinition.CustomSubscriptionsAllowed = contribution.GetProperty<bool>(NotificationClientConstants.CustomSubscriptionsAllowed, true);
      eventDefinition.Roles = this.GetEventTypeRoles(requestContext, contribution);
      eventDefinition.SupportedScopes = this.GetPropertyListValue(contribution, NotificationClientConstants.ScopesProperty, new List<string>()
      {
        "collection",
        "project"
      });
      eventDefinition.HasInitiator = contribution.GetProperty<bool>(NotificationClientConstants.HasInitiatorProperty);
      eventDefinition.Color = contribution.GetProperty<string>(NotificationClientConstants.ColorProperty, string.Empty);
      eventDefinition.Icon = contribution.GetProperty<string>(NotificationClientConstants.IconProperty, string.Empty);
      if (eventDefinition.Roles != null && eventDefinition.Roles.Any<NotificationEventRole>())
      {
        foreach (NotificationEventRole role in eventDefinition.Roles)
        {
          if (role.SupportsGroups)
          {
            eventDefinition.HasGroupRoles = true;
            break;
          }
        }
      }
      eventDefinition.SerializationFormat = contribution.Type == "ms.vss-notifications.legacy-event-type" ? EventSerializerType.Xml : EventSerializerType.Json;
      if (addFields && contribution.Includes != null)
      {
        foreach (string include in contribution.Includes)
          eventDefinition.AddField(include);
      }
      return eventDefinition;
    }

    private List<NotificationEventRole> GetEventTypeRoles(
      IVssRequestContext requestContext,
      Contribution contribution)
    {
      return contribution.GetProperty<List<NotificationEventRole>>(NotificationClientConstants.RolesProperty, new List<NotificationEventRole>());
    }

    private List<NotificationEventRole> GetDynamicEventTypeRoles(
      IVssRequestContext requestContext,
      string eventType,
      string matcher)
    {
      List<NotificationEventRole> dynamicEventTypeRoles = new List<NotificationEventRole>();
      eventType = EventTypeMapper.ToLegacy(requestContext, eventType);
      ISubscriptionAdapter adapter = SubscriptionAdapterFactory.CreateAdapter(requestContext, eventType, matcher, (SubscriptionScope) null, throwIfNotFound: false);
      if (adapter != null)
        dynamicEventTypeRoles.AddRange((IEnumerable<NotificationEventRole>) adapter.GetRoles(requestContext));
      return dynamicEventTypeRoles;
    }

    private bool IsEventTypeContribution(string contributionType) => contributionType.Equals("ms.vss-notifications.event-type", StringComparison.OrdinalIgnoreCase) || contributionType.Equals("ms.vss-notifications.legacy-event-type", StringComparison.OrdinalIgnoreCase);

    private void MaybeFixupEventTypeForCodeReview(
      IVssRequestContext requestContext,
      VssNotificationEvent notifEvent)
    {
      if (notifEvent == null)
        return;
      using (IEnumerator<INotificationTransform> enumerator = requestContext.GetStaticNotificationExtentions<INotificationTransform>().GetEnumerator())
      {
        if (!enumerator.MoveNext() || !enumerator.Current.IsCodeReviewEventType(requestContext, notifEvent.Data))
          return;
        notifEvent.EventType = "WorkItem_CodeReview";
      }
    }

    private bool ServiceInstanceTypeMatches(
      Guid currentServiceInstanceType,
      NotificationEventPublisher publisher)
    {
      return publisher.SubscriptionManagementInfo.ServiceInstanceType.Equals(Guid.Empty) || publisher.SubscriptionManagementInfo.ServiceInstanceType.Equals(currentServiceInstanceType);
    }

    private void EnsureIgnoreConditionsLoaded(IVssRequestContext requestContext)
    {
      if (this.m_ignoreSubscriptions != null)
        return;
      RegistryEntryCollection registryEntries = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (FrameworkServerConstants.IgnoreSubscriptionsRootPath + "/**"));
      this.ReadIgnoreConditions(requestContext, registryEntries);
    }

    public void PublishSystemEvent(IVssRequestContext requestContext, VssNotificationEvent theEvent)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<VssNotificationEvent>(theEvent, nameof (theEvent));
      this.PublishEvent(requestContext.Elevate(), theEvent);
    }

    public void PublishSystemEvents(
      IVssRequestContext requestContext,
      IEnumerable<VssNotificationEvent> theEvents,
      bool allowDuringServicing = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<VssNotificationEvent>>(theEvents, "theEvent");
      this.PublishEvents(requestContext.Elevate(), theEvents, allowDuringServicing);
    }

    internal List<SerializedNotificationEvent> PrepareAndFilterEvents(
      IVssRequestContext requestContext,
      IEnumerable<VssNotificationEvent> unprocessedEvents,
      out HashSet<string> processQueues)
    {
      List<SerializedNotificationEvent> source1 = new List<SerializedNotificationEvent>();
      List<object> source2 = new List<object>();
      int minutes = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) (FrameworkServerConstants.NotificationRootPath + "/CleanupEventAgeMins"), 5760);
      processQueues = new HashSet<string>();
      Dictionary<string, NotificationEventType> eventTypes = this.QueryEventTypes(requestContext);
      foreach (VssNotificationEvent unprocessedEvent in unprocessedEvents)
      {
        SerializedNotificationEvent notificationEvent = new SerializedNotificationEvent(unprocessedEvent);
        string eventType = notificationEvent.EventType;
        try
        {
          if (eventType == "WorkItemChangedEvent")
            this.MaybeFixupEventTypeForCodeReview(requestContext, (VssNotificationEvent) notificationEvent);
          this.SerializeEvent(requestContext, (VssNotificationEvent) notificationEvent);
          if (notificationEvent.EventType == "WorkItemChangedEvent" && !string.IsNullOrEmpty((string) notificationEvent.Data))
          {
            if (!(notificationEvent.Data is string s))
              s = string.Empty;
            int byteCount = Encoding.Unicode.GetByteCount(s);
            Guid scope = notificationEvent.GetScope(VssNotificationEvent.ScopeNames.Project);
            CustomerIntelligenceData ciData = new CustomerIntelligenceData();
            ciData.Add("sizeWithTipValuesData", (double) byteCount);
            ciData.Add("project", scope.ToString());
            NotificationCustomerIntelligence.PublishEvent(requestContext, NotificationCustomerIntelligence.WorkItemSerializationFeature, NotificationCustomerIntelligence.SerializeWorkItemEventAction, ciData);
          }
          if (string.IsNullOrEmpty(notificationEvent.EventDataString()) || notificationEvent.EventDataString().Length > this.m_maxEventLength)
          {
            requestContext.Trace(1002020, TraceLevel.Error, NotificationEventService.Area, NotificationEventService.Layer, "Error when trying to serialize object. eventSize: {0}, maxEventSize: {1}, itemId: {2}, eventType: {3}", (object) notificationEvent.EventDataString().Length, (object) this.m_maxEventLength, (object) notificationEvent.ItemId, (object) notificationEvent.EventType);
          }
          else
          {
            if (notificationEvent.EventDataString().Length > this.m_eventSizeTraceLimit)
              requestContext.TraceAlways(1002236, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, "Event size is bigger than the trace limit. eventSize: {0}, currentEventSizeTraceLimit: {1}, itemId: {2}, eventType: {3}", (object) notificationEvent.EventDataString().Length, (object) this.m_eventSizeTraceLimit, (object) notificationEvent.ItemId, (object) notificationEvent.EventType);
            notificationEvent.NotifyIfEventCreatedDeltaExceeded(requestContext, this.m_eventCreatedDeltaTriggerMilliseconds);
            notificationEvent.ProcessQueue = this.GetProcessQueue(requestContext, notificationEvent, eventTypes);
            if (this.ShouldProcessEvent(requestContext, notificationEvent))
            {
              if (this.IsValidEventType(requestContext, eventType))
              {
                if (notificationEvent.ExpiresIn == VssNotificationEvent.DefaultExpiration)
                  notificationEvent.ExpiresIn = new TimeSpan(0, minutes, 0);
                processQueues.Add(notificationEvent.ProcessQueue);
                source1.Add(notificationEvent);
                this.m_publishEventStats.PublishEvent(requestContext, notificationEvent);
              }
              else
                requestContext.Trace(1002020, TraceLevel.Warning, NotificationEventService.Area, NotificationEventService.Layer, "Unsupported event type {0} for event {1}", (object) notificationEvent.EventType, (object) notificationEvent.EventDataString());
            }
            else
              this.m_publishEventStats.BlockEvent(requestContext, notificationEvent);
          }
        }
        catch (Exception ex)
        {
          source2.Add(notificationEvent.Data);
          requestContext.Trace(1002020, TraceLevel.Error, NotificationEventService.Area, NotificationEventService.Layer, "Error when deserializing object {0}. Exception: {1}", notificationEvent.Data, (object) ex);
        }
      }
      if (source2.Count<object>() > 0)
      {
        StringBuilder stringBuilder = new StringBuilder(string.Empty);
        foreach (object obj in source2)
        {
          stringBuilder.Append(obj.ToString());
          stringBuilder.Append("\r\n");
        }
        throw new ArgumentException(string.Format("{0} events were encountered that could not be serialized. The events are {1}", (object) source2.Count<object>(), (object) stringBuilder));
      }
      requestContext.Trace(1002210, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, "Read {0} unprocessed events, ignored {1} events", (object) unprocessedEvents.Count<VssNotificationEvent>(), (object) (unprocessedEvents.Count<VssNotificationEvent>() - source1.Count<SerializedNotificationEvent>()));
      return source1;
    }

    private string GetProcessQueue(
      IVssRequestContext requestContext,
      SerializedNotificationEvent ev,
      Dictionary<string, NotificationEventType> eventTypes)
    {
      string processQueue = (string) null;
      string key = !string.Equals(ev.EventType, "WorkItem_CodeReview") ? EventTypeMapper.ToContributed(requestContext, ev.EventType) : "ms.vss-codereview.codereview-changed-event";
      NotificationEventType notificationEventType;
      if (eventTypes.TryGetValue(key, out notificationEventType))
      {
        NotificationEventPublisher eventPublisher = notificationEventType.EventPublisher;
        if (eventPublisher != null)
          processQueue = !ev.EventType.Equals(NotificationEventService.GitPullRequestCommentEvent) || !requestContext.IsFeatureEnabled(NotificationEventService.s_gitCommentsProcessingJob) ? eventPublisher.Id : NotificationClientConstants.GitCommentsProcessQueueId;
        else
          requestContext.Trace(1002322, TraceLevel.Error, NotificationEventService.Area, NotificationEventService.Layer, "GPQ: No publisher for event type for {0}", (object) ev.EventType);
      }
      else
        requestContext.Trace(1002322, TraceLevel.Error, NotificationEventService.Area, NotificationEventService.Layer, "GPQ: Could not find a contributed event type for {0}", (object) ev.EventType);
      return processQueue;
    }

    public bool IsValidEventType(IVssRequestContext requestContext, string eventType)
    {
      if (EventTypeMapper.IsLegacy(requestContext, eventType))
        return true;
      if (!this.m_supportedEventTypes.ContainsKey(eventType))
        return this.IsContributedEventType(requestContext, eventType);
      requestContext.Trace(1002219, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, "Succeeded finding event type {0} in cache", (object) eventType);
      return true;
    }

    private bool IsContributedEventType(IVssRequestContext requestContext, string eventType)
    {
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      string cacheKey = ContributionUtils.GetCacheKey(NotificationEventService.s_eventTypeIdDataName, (IEnumerable<string>) new string[1]
      {
        eventType
      }, (HashSet<string>) null, new ContributionQueryOptions?());
      Contribution contribution;
      string associatedData1;
      if (!service.QueryContribution<string>(requestContext, eventType, cacheKey, out contribution, out associatedData1))
      {
        requestContext.WarnIfContributionsInFallbackMode(nameof (IsContributedEventType));
        if (contribution != null)
        {
          associatedData1 = this.GetContributionId(contribution.Id);
          IContributionServiceWithData contributionServiceWithData = service;
          IVssRequestContext requestContext1 = requestContext;
          string associatedDataName = cacheKey;
          List<Contribution> contributionList = new List<Contribution>();
          contributionList.Add(contribution);
          string associatedData2 = associatedData1;
          contributionServiceWithData.Set(requestContext1, associatedDataName, (IEnumerable<Contribution>) contributionList, (object) associatedData2);
        }
      }
      if (associatedData1 == null)
        return false;
      if (!this.m_supportedEventTypes.TryAdd(associatedData1, (byte) 0))
        requestContext.Trace(1002218, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, "Failed to add event type to cache with ud {0}", (object) associatedData1);
      return true;
    }

    private string GetContributionId(string contributionId) => string.IsNullOrEmpty(contributionId) ? string.Empty : new ContributionIdentifier(contributionId).RelativeId;

    private string GetEventTypeRestURL(
      IVssRequestContext requestContext,
      NotificationEventType eventType)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "notification", NotificationApiConstants.EventTypesResource.LocationId, (object) new
        {
          eventType = eventType.Id,
          resource = "EventTypes"
        }).AbsoluteUri.ToString();
      }
      catch (Exception ex)
      {
      }
      return string.Empty;
    }

    private void SerializeEvent(IVssRequestContext requestContext, VssNotificationEvent notifEvent)
    {
      object data = notifEvent.Data;
      if (data != null)
      {
        Type type = data.GetType();
        if (!(type != typeof (string)))
          return;
        object[] customAttributes = type.GetCustomAttributes(typeof (NotificationEventBindingsAttribute), true);
        EventSerializerType eventSerializerType;
        if (customAttributes.Length != 0)
        {
          eventSerializerType = ((NotificationEventBindingsAttribute) customAttributes[0]).SerializerType;
        }
        else
        {
          NotificationEventType eventType = requestContext.GetService<INotificationEventService>().GetEventType(requestContext, notifEvent.EventType);
          eventSerializerType = eventType != null ? eventType.SerializationFormat : EventSerializerType.Xml;
        }
        if (eventSerializerType == EventSerializerType.Json)
        {
          notifEvent.Data = (object) JsonConvert.SerializeObject(data, new VssJsonMediaTypeFormatter().SerializerSettings);
        }
        else
        {
          bool formatXml = requestContext.IsFeatureEnabled("Notifications.EnableFormattedXmlSerialization");
          notifEvent.Data = (object) NotificationsSerialization.SerializeToXml(data, formatXml);
        }
      }
      else
        notifEvent.Data = (object) string.Empty;
    }

    internal bool ShouldProcessEvent(
      IVssRequestContext requestContext,
      SerializedNotificationEvent notifEvent)
    {
      bool flag1 = true;
      string str1 = notifEvent.EventDataString();
      requestContext.Trace(1002211, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, "# of ignore subscriptions is {0}", (object) this.m_ignoreSubscriptions.Values.Count);
      if (this.m_ignoreSubscriptions.Count > 0)
      {
        TeamFoundationEvent teamFoundationEvent = (TeamFoundationEvent) null;
        bool flag2 = requestContext.IsTracing(1002217, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer);
        bool flag3 = requestContext.IsTracing(1002216, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer);
        foreach (KeyValuePair<Guid, IgnoreSubscription> ignoreSubscription1 in this.m_ignoreSubscriptions)
        {
          Guid key = ignoreSubscription1.Key;
          IgnoreSubscription ignoreSubscription2 = ignoreSubscription1.Value;
          if (!string.Equals(ignoreSubscription2.EventType, notifEvent.EventType, StringComparison.OrdinalIgnoreCase))
          {
            if (flag2)
              requestContext.Trace(1002217, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, string.Format("Event not blocked subsid:{0} itemid:{1} EventType {2} != {3}", (object) key, (object) notifEvent.ItemId, (object) ignoreSubscription2.EventType, (object) notifEvent.EventType));
          }
          else
          {
            if (((IEnumerable<Guid>) ignoreSubscription2.Actors).Any<Guid>())
            {
              bool flag4 = false;
              foreach (Guid actor in ignoreSubscription2.Actors)
              {
                if (actor != Guid.Empty && (actor == notifEvent.GetMainActorId() || actor == notifEvent.GetInitiator()))
                {
                  flag4 = true;
                  break;
                }
              }
              if (!flag4)
              {
                if (flag2)
                {
                  requestContext.Trace(1002217, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, string.Format("Event not blocked subsid:{0} itemid:{1} type:{2} No Actor match - subscription:{3} event main actor:{4} initiator:{5}", (object) key, (object) notifEvent.ItemId, (object) notifEvent.EventType, (object) ignoreSubscription2.ActorsValue, (object) notifEvent.GetMainActorId(), (object) notifEvent.GetActor(VssNotificationEvent.Roles.Initiator)));
                  continue;
                }
                continue;
              }
            }
            if (!string.IsNullOrEmpty(str1))
            {
              if (ignoreSubscription2.Condition == null)
              {
                flag1 = false;
                if (flag3)
                  requestContext.Trace(1002216, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, string.Format("Event blocked (empty condition) subsid:{0} itemId:{1} type:{2} {3}", (object) key, (object) notifEvent.ItemId, (object) notifEvent.EventType, (object) str1.Substring(0, (int) byte.MaxValue)));
              }
              else
              {
                if (teamFoundationEvent == null)
                  teamFoundationEvent = TeamFoundationEventFactory.GetEvent((VssNotificationEvent) notifEvent);
                Subscription subscription = new Subscription();
                EvaluationContext evaluationContext = new EvaluationContext()
                {
                  Verify = false,
                  RegexTimeout = TimeSpan.FromSeconds(5.0),
                  UseRegexMatch = requestContext.IsFeatureEnabled("NotificationJob.UseRegexForMatch"),
                  Tracer = (ISubscriptionObjectTrace) new SubscriptionObjectTracer(requestContext, NotificationEventService.Layer),
                  Subscription = subscription,
                  User = subscription.SubscriberIdentity,
                  Event = teamFoundationEvent,
                  MacrosLookup = new Dictionary<string, string>()
                };
                if (ignoreSubscription2.Condition.Evaluate(requestContext, evaluationContext))
                {
                  flag1 = false;
                  if (flag3)
                    requestContext.Trace(1002216, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, string.Format("Event blocked (matched condition) subsid:{0} itemId:{1} type:{2} {3}", (object) key, (object) notifEvent.ItemId, (object) notifEvent.EventType, (object) str1.Substring(0, (int) byte.MaxValue)));
                }
                else if (flag2)
                  requestContext.Trace(1002217, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, string.Format("Event not blocked (condition didn't match) subsid:{0} itemId:{1} type:{2} {3}", (object) key, (object) notifEvent.ItemId, (object) notifEvent.EventType, (object) str1.Substring(0, (int) byte.MaxValue)));
              }
            }
            if (!flag1)
            {
              if (ignoreSubscription2.AllowedChannels != null)
              {
                if (ignoreSubscription2.AllowedChannels.Count > 0)
                {
                  flag1 = true;
                  notifEvent.AllowedChannels = ignoreSubscription2.AllowedChannels.ToHashSet();
                  if (flag3)
                  {
                    string str2 = string.Join(",", (IEnumerable<string>) notifEvent.AllowedChannels);
                    requestContext.Trace(1002216, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, string.Format("Event partially blocked subsid:{0} itemId:{1} type:{2} allowedChannels are: {3}", (object) key, (object) notifEvent.ItemId, (object) notifEvent.EventType, (object) str2));
                    break;
                  }
                  break;
                }
                break;
              }
              break;
            }
          }
        }
      }
      return flag1;
    }

    internal Dictionary<Guid, IgnoreSubscription> IgnoreSubscriptions
    {
      [DebuggerStepThrough] get => this.m_ignoreSubscriptions;
    }

    internal static string Area
    {
      [DebuggerStepThrough] get => "Notifications";
    }

    internal static string Layer
    {
      [DebuggerStepThrough] get => nameof (NotificationEventService);
    }
  }
}
