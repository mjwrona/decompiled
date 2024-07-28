// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationEventService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationEventService : 
    VssBaseService,
    ITeamFoundationEventService,
    IVssFrameworkService
  {
    private IDisposableReadOnlyList<ISubscriber> m_loadedSubscribers;
    private IVssServiceHost m_serviceHost;
    private Dictionary<Type, TeamFoundationEventService.SubscriptionList> m_subscriptions = new Dictionary<Type, TeamFoundationEventService.SubscriptionList>();
    private List<TeamFoundationEventService.PublicationArgs> m_publicationArgsList = new List<TeamFoundationEventService.PublicationArgs>();
    private ILockName m_publicationArgLock;
    private int m_subscriberCountThreshold;
    private int m_unprocessedEventsCountThreshold;
    private static readonly RegistryQuery s_subscriberThresholdQuery = new RegistryQuery("/Configuration/EventService/Settings/SubscriberThreshold");
    private static readonly RegistryQuery s_unprocessedEventsThresholdQuery = new RegistryQuery("/Configuration/EventService/Settings/UnprocessedEventsCountThreshold");
    private const int c_defaultSubscriberCountThreshold = 30;
    private const int c_unprocessedEventsCountThresholdQuery = 50;
    private const string s_Area = "EventService";
    private const string s_Layer = "TeamFoundationEventService";
    public const string SubscriberNameKey = "Microsoft.TeamFoundation.SubscriberName";
    public const string SubscriberTypeKey = "Microsoft.TeamFoundation.SubscriberType";
    public const string StatusCodeKey = "Microsoft.TeamFoundation.StatusCode";

    internal TeamFoundationEventService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      TeamFoundationTracingService.TraceEnterRaw(16000, "EventService", nameof (TeamFoundationEventService), "ServiceStart", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
        this.m_subscriberCountThreshold = service.GetValue<int>(systemRequestContext, in TeamFoundationEventService.s_subscriberThresholdQuery, 30);
        this.m_unprocessedEventsCountThreshold = service.GetValue<int>(systemRequestContext, in TeamFoundationEventService.s_unprocessedEventsThresholdQuery, 50);
        this.m_serviceHost = systemRequestContext.ServiceHost;
        this.m_loadedSubscribers = systemRequestContext.GetExtensions<ISubscriber>();
        foreach (ISubscriber loadedSubscriber in (IEnumerable<ISubscriber>) this.m_loadedSubscribers)
          this.Subscribe(systemRequestContext, loadedSubscriber);
        this.m_publicationArgLock = this.CreateLockName(systemRequestContext, "publicationArgLock");
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(16008, "EventService", nameof (TeamFoundationEventService), ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(16009, "EventService", nameof (TeamFoundationEventService), "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      TeamFoundationTracingService.TraceEnterRaw(16020, "EventService", nameof (TeamFoundationEventService), "Dispose", new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        if (this.m_loadedSubscribers != null)
        {
          this.m_loadedSubscribers.Dispose();
          this.m_loadedSubscribers = (IDisposableReadOnlyList<ISubscriber>) null;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(16028, "EventService", nameof (TeamFoundationEventService), ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(16029, "EventService", nameof (TeamFoundationEventService), "Dispose");
      }
      this.m_serviceHost = (IVssServiceHost) null;
    }

    private void Publish(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      bool runSynchronously,
      out Exception exception,
      bool allowDuringServicing = false)
    {
      exception = (Exception) null;
      if (notificationType == NotificationType.Notification && requestContext.IsServicingContext && !allowDuringServicing)
        return;
      requestContext.TraceEnter(16040, "EventService", nameof (TeamFoundationEventService), nameof (Publish));
      try
      {
        TeamFoundationEventService.SubscriptionList subscriptionList;
        if (!this.m_subscriptions.TryGetValue(notificationEventArgs.GetType(), out subscriptionList))
          return;
        if (notificationType == NotificationType.Notification)
        {
          if (runSynchronously)
          {
            int num = (int) subscriptionList.Notify(requestContext, notificationType, notificationEventArgs, out string _, out ExceptionPropertyCollection _, out exception);
          }
          else
          {
            ITeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
            TeamFoundationEventService.PublicationArgs publicationArgs = new TeamFoundationEventService.PublicationArgs(subscriptionList, notificationType, notificationEventArgs);
            int currentPublicationArgsListSize = 0;
            using (requestContext.Lock(this.m_publicationArgLock))
            {
              this.m_publicationArgsList.Add(publicationArgs);
              currentPublicationArgsListSize = this.m_publicationArgsList.Count;
            }
            if (currentPublicationArgsListSize > this.m_unprocessedEventsCountThreshold)
              requestContext.TraceConditionally(16042, TraceLevel.Warning, "EventService", nameof (TeamFoundationEventService), (Func<string>) (() => string.Format("The number of published events that are yet to be processed is at {0}, and has crossed the threshold value of {1}.", (object) currentPublicationArgsListSize, (object) this.m_unprocessedEventsCountThreshold)));
            bool flag = requestContext.IsFeatureEnabled("VisualStudio.Services.Framework.KeepTaskStartTimeOnRequeue");
            ITeamFoundationTaskService foundationTaskService = service;
            IVssRequestContext requestContext1 = requestContext;
            TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.AsyncPublication));
            task.KeepStartTimeOnRequeue = flag;
            foundationTaskService.AddTask(requestContext1, task);
          }
        }
        else
        {
          string statusMessage;
          ExceptionPropertyCollection properties;
          if (subscriptionList.Notify(requestContext, notificationType, notificationEventArgs, out statusMessage, out properties, out exception) == EventNotificationStatus.ActionDenied)
            throw new ActionDeniedBySubscriberException(statusMessage, properties);
        }
      }
      catch (ActionDeniedBySubscriberException ex)
      {
        requestContext.TraceException(16047, TraceLevel.Info, "EventService", nameof (TeamFoundationEventService), (Exception) ex);
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(16048, "EventService", nameof (TeamFoundationEventService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(16049, "EventService", nameof (TeamFoundationEventService), nameof (Publish));
      }
    }

    public void PublishDecisionPoint(IVssRequestContext requestContext, object notificationEvent) => this.Publish(requestContext, NotificationType.DecisionPoint, notificationEvent, false, out Exception _);

    public void PublishNotification(IVssRequestContext requestContext, object notificationEvent) => this.Publish(requestContext, NotificationType.Notification, notificationEvent, false, out Exception _);

    public void PublishNotification(
      IVssRequestContext requestContext,
      object notificationEvent,
      bool allowDuringServicing = false)
    {
      this.Publish(requestContext, NotificationType.Notification, notificationEvent, false, out Exception _, allowDuringServicing);
    }

    public void SyncPublishNotification(IVssRequestContext requestContext, object notificationEvent)
    {
      Exception exception;
      this.Publish(requestContext, NotificationType.Notification, notificationEvent, true, out exception);
      if (exception != null)
        throw exception;
    }

    public void AsyncPublishNotification(
      IVssRequestContext requestContext,
      Guid hostId,
      object notificationEvent)
    {
      requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(hostId, new TeamFoundationTask(new TeamFoundationTaskCallback(this.AsyncPublication), notificationEvent, 0));
    }

    public void Subscribe(IVssRequestContext requestContext, ISubscriber subscriber)
    {
      requestContext.TraceEnter(16060, "EventService", nameof (TeamFoundationEventService), nameof (Subscribe));
      try
      {
        TeamFoundationEventService.SubscriptionList subscriptionList = (TeamFoundationEventService.SubscriptionList) null;
        foreach (Type subscribedType in subscriber.SubscribedTypes())
        {
          if (!this.m_subscriptions.TryGetValue(subscribedType, out subscriptionList))
          {
            subscriptionList = new TeamFoundationEventService.SubscriptionList();
            this.m_subscriptions[subscribedType] = subscriptionList;
          }
          subscriptionList.AddSubscriber(subscriber);
          if (subscriptionList.Count > this.m_subscriberCountThreshold)
            requestContext.TraceAlways(16061, TraceLevel.Info, "EventService", nameof (TeamFoundationEventService), string.Format("The number of subscribers for type {0} is too large {1}, which is greater than {2}.", (object) subscribedType, (object) subscriptionList.Count, (object) this.m_subscriberCountThreshold));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(16068, "EventService", nameof (TeamFoundationEventService), ex);
        TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.SubscriberDisabledMessage((object) subscriber.GetType().FullName), ex);
      }
      finally
      {
        requestContext.TraceLeave(16069, "EventService", nameof (TeamFoundationEventService), nameof (Subscribe));
      }
    }

    public void Unsubscribe(ISubscriber subscriber)
    {
      TeamFoundationTracingService.TraceEnterRaw(16080, "EventService", nameof (TeamFoundationEventService), nameof (Unsubscribe), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        foreach (Type subscribedType in subscriber.SubscribedTypes())
        {
          TeamFoundationEventService.SubscriptionList subscriptionList;
          if (this.m_subscriptions.TryGetValue(subscribedType, out subscriptionList))
            subscriptionList.RemoveSubscriber(subscriber);
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(16088, "EventService", nameof (TeamFoundationEventService), ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(16089, "EventService", nameof (TeamFoundationEventService), nameof (Unsubscribe));
      }
    }

    private void AsyncPublication(IVssRequestContext requestContext, object obj)
    {
      List<TeamFoundationEventService.PublicationArgs> argsToProcess;
      using (requestContext.Lock(this.m_publicationArgLock))
      {
        argsToProcess = this.m_publicationArgsList;
        this.m_publicationArgsList = new List<TeamFoundationEventService.PublicationArgs>();
      }
      requestContext.TraceConditionally(16110, TraceLevel.Info, "EventService", nameof (TeamFoundationEventService), (Func<string>) (() => string.Format("The number of events being processed now : {0}", (object) argsToProcess?.Count)));
      try
      {
        requestContext.RootContext.Items["IsAsyncNotification"] = (object) true;
        foreach (TeamFoundationEventService.PublicationArgs publicationArgs in argsToProcess)
        {
          int num = (int) publicationArgs.m_subscriptionList.Notify(requestContext, publicationArgs.m_notificationType, publicationArgs.m_notificationEventArgs, out string _, out ExceptionPropertyCollection _, out Exception _);
        }
      }
      finally
      {
        requestContext.RootContext.Items.Remove("IsAsyncNotification");
      }
    }

    private class SubscriptionList
    {
      private List<TeamFoundationEventService.SubscriptionList.Subscription> m_subscriptions = new List<TeamFoundationEventService.SubscriptionList.Subscription>();

      public int Count => this.m_subscriptions.Count;

      internal void AddSubscriber(ISubscriber newSubscriber)
      {
        this.RemoveSubscriber(newSubscriber);
        int index = 0;
        foreach (TeamFoundationEventService.SubscriptionList.Subscription subscription in this.m_subscriptions)
        {
          if (newSubscriber.Priority <= subscription.Priority)
            ++index;
          else
            break;
        }
        this.m_subscriptions.Insert(index, new TeamFoundationEventService.SubscriptionList.Subscription(newSubscriber));
      }

      internal void RemoveSubscriber(ISubscriber subscriber)
      {
        foreach (TeamFoundationEventService.SubscriptionList.Subscription subscription in this.m_subscriptions)
        {
          if (subscription.Subscriber == subscriber)
          {
            this.m_subscriptions.Remove(subscription);
            break;
          }
        }
      }

      internal EventNotificationStatus Notify(
        IVssRequestContext requestContext,
        NotificationType notificationType,
        object notificationEventArgs,
        out string statusMessage,
        out ExceptionPropertyCollection properties,
        out Exception exception)
      {
        EventNotificationStatus notificationStatus1 = EventNotificationStatus.ActionPermitted;
        properties = (ExceptionPropertyCollection) null;
        statusMessage = string.Empty;
        exception = (Exception) null;
        List<Exception> innerExceptions = new List<Exception>();
        foreach (TeamFoundationEventService.SubscriptionList.Subscription subscription in this.m_subscriptions)
        {
          try
          {
            int statusCode;
            EventNotificationStatus notificationStatus2;
            using (new TraceWatch(requestContext, 16100, TraceLevel.Error, new TimeSpan(0, 0, 3), "EventService", nameof (TeamFoundationEventService), "Notify {0} with {1} is taking too long", new object[2]
            {
              (object) subscription.Name,
              (object) notificationType
            }))
              notificationStatus2 = subscription.Notify(requestContext, notificationType, notificationEventArgs, out statusCode, out statusMessage, out properties);
            if (notificationType == NotificationType.DecisionPoint)
            {
              if (notificationStatus2 != EventNotificationStatus.ActionDenied)
              {
                if (notificationStatus2 != EventNotificationStatus.ActionApproved)
                  continue;
              }
              notificationStatus1 = notificationStatus2;
              if (notificationStatus1 == EventNotificationStatus.ActionDenied)
              {
                if (properties == null)
                  properties = new ExceptionPropertyCollection();
                properties.Set("Microsoft.TeamFoundation.StatusCode", statusCode);
                properties.Set("Microsoft.TeamFoundation.SubscriberName", subscription.Subscriber.Name);
                properties.Set("Microsoft.TeamFoundation.SubscriberType", subscription.Subscriber.GetType().FullName);
                break;
              }
              break;
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(16108, "EventService", nameof (TeamFoundationEventService), ex);
            if (notificationType != NotificationType.Notification || !(ex is HostShutdownException))
              TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.SubscriberExceptionMessage((object) subscription.Name, (object) notificationEventArgs.GetType()), ex, TeamFoundationEventId.EventNotificationException, EventLogEntryType.Error);
            innerExceptions.Add(ex);
          }
        }
        if (notificationStatus1 == EventNotificationStatus.ActionPermitted)
          notificationStatus1 = EventNotificationStatus.ActionApproved;
        if (innerExceptions.Count > 0)
          exception = (Exception) new AggregateException((IEnumerable<Exception>) innerExceptions);
        return notificationStatus1;
      }

      private class Subscription
      {
        private bool m_isEnabled = true;
        private ISubscriber m_subscriber;

        internal Subscription(ISubscriber subscriber)
        {
          this.m_subscriber = subscriber;
          this.m_isEnabled = true;
        }

        internal bool IsEnabled => this.m_isEnabled;

        internal string Name => this.m_subscriber.Name;

        internal SubscriberPriority Priority => this.m_subscriber.Priority;

        internal ISubscriber Subscriber => this.m_subscriber;

        internal EventNotificationStatus Notify(
          IVssRequestContext requestContext,
          NotificationType notificationType,
          object notificationEventArgs,
          out int statusCode,
          out string statusMessage,
          out ExceptionPropertyCollection properties)
        {
          return this.m_subscriber.ProcessEvent(requestContext, notificationType, notificationEventArgs, out statusCode, out statusMessage, out properties);
        }
      }
    }

    private class PublicationArgs
    {
      internal TeamFoundationEventService.SubscriptionList m_subscriptionList;
      internal NotificationType m_notificationType;
      internal object m_notificationEventArgs;

      internal PublicationArgs(
        TeamFoundationEventService.SubscriptionList subscriptionList,
        NotificationType notificationType,
        object notificationEventArgs)
      {
        this.m_subscriptionList = subscriptionList;
        this.m_notificationType = notificationType;
        this.m_notificationEventArgs = notificationEventArgs;
      }
    }
  }
}
