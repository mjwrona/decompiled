// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.PublishEventStats
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class PublishEventStats
  {
    private int m_activeTask;
    private TeamFoundationTask m_task;
    private bool m_stopped;
    private object m_lock = new object();
    private Dictionary<(string EventType, string Publisher), int> m_publishedEvents = new Dictionary<(string, string), int>();
    private Dictionary<(string EventType, string Publisher), int> m_blockedEvents = new Dictionary<(string, string), int>();
    private const int c_defaultPublishEventsRateMs = 300000;

    public PublishEventStats() => this.m_task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.DoPublishStats), (object) null, 300000);

    public void PublishEvent(IVssRequestContext requestContext, SerializedNotificationEvent ev) => this.AddEvent(requestContext, ev, true);

    public void BlockEvent(IVssRequestContext requestContext, SerializedNotificationEvent ev) => this.AddEvent(requestContext, ev, false);

    private void AddEvent(
      IVssRequestContext requestContext,
      SerializedNotificationEvent ev,
      bool publish)
    {
      if (!this.EnsureTaskIsRunning(requestContext, ev, nameof (AddEvent)))
        return;
      string str1 = ev.EventType ?? string.Empty;
      string str2 = ev.ProcessQueue ?? string.Empty;
      int num = 0;
      lock (this.m_lock)
      {
        Dictionary<(string, string), int> dictionary = publish ? this.m_publishedEvents : this.m_blockedEvents;
        dictionary.TryGetValue((str1, str2), out num);
        dictionary[(str1, str2)] = num + 1;
      }
    }

    public int Interval
    {
      get => this.m_task.Interval;
      set
      {
        if (value == this.m_task.Interval)
          return;
        if (this.m_activeTask != 0)
          throw new InvalidOperationException("Can't change interval once task has started");
        this.m_task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.DoPublishStats), (object) null, value);
      }
    }

    public void Stop(IVssRequestContext requestContext)
    {
      this.m_stopped = true;
      if (Interlocked.CompareExchange(ref this.m_activeTask, 0, 1) != 1)
        return;
      try
      {
        requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().RemoveTask(requestContext.ServiceHost.InstanceId, this.m_task);
        this.DoPublishStats(requestContext, (object) null);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002214, "Notifications", "Events", ex);
      }
    }

    private bool EnsureTaskIsRunning(
      IVssRequestContext requestContext,
      SerializedNotificationEvent ev,
      [CallerMemberName] string caller = "")
    {
      bool flag = this.Enabled(requestContext);
      if (flag && Interlocked.CompareExchange(ref this.m_activeTask, 1, 0) == 0)
      {
        flag = !this.m_stopped;
        if (flag)
        {
          try
          {
            requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext.ServiceHost.InstanceId, this.m_task);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1002214, "Notifications", "Events", ex);
          }
        }
        else
          requestContext.Trace(1002215, TraceLevel.Error, "Notifications", "Events", caller + " after Stop for event id:" + ev.ItemId + " type:" + ev.EventType);
      }
      return flag;
    }

    private bool Enabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Notifications.EnableEventPublishStats");

    internal void DoPublishStats(IVssRequestContext requestContext, object taskArgs)
    {
      if (!this.Enabled(requestContext))
        return;
      Dictionary<(string, string), int> eventSummary1 = (Dictionary<(string, string), int>) null;
      Dictionary<(string, string), int> eventSummary2 = (Dictionary<(string, string), int>) null;
      lock (this.m_lock)
      {
        if (this.m_publishedEvents.Any<KeyValuePair<(string, string), int>>())
        {
          eventSummary1 = this.m_publishedEvents;
          this.m_publishedEvents = new Dictionary<(string, string), int>();
        }
        if (this.m_blockedEvents.Any<KeyValuePair<(string, string), int>>())
        {
          eventSummary2 = this.m_blockedEvents;
          this.m_blockedEvents = new Dictionary<(string, string), int>();
        }
      }
      this.AccumulateIntelligence(requestContext, eventSummary1, NotificationCustomerIntelligence.PublishedEventsAction);
      this.AccumulateIntelligence(requestContext, eventSummary2, NotificationCustomerIntelligence.BlockedEventsAction);
    }

    private void AccumulateIntelligence(
      IVssRequestContext requestContext,
      Dictionary<(string EventType, string Publisher), int> eventSummary,
      string action)
    {
      if (eventSummary == null)
        return;
      foreach (KeyValuePair<(string EventType, string Publisher), int> keyValuePair in eventSummary)
      {
        CustomerIntelligenceData ciData = new CustomerIntelligenceData();
        string eventType = keyValuePair.Key.EventType;
        string publisher = keyValuePair.Key.Publisher;
        int num = keyValuePair.Value;
        ciData.Add("EventType", eventType);
        ciData.Add("Publisher", publisher);
        ciData.Add("Count", (double) num);
        NotificationCustomerIntelligence.PublishEvent(requestContext, NotificationCustomerIntelligence.NotificationEventServiceFeature, action, ciData);
      }
    }
  }
}
