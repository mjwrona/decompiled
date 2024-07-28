// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemRecentActivityServiceBusPublisher
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Notification;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  public class WorkItemRecentActivityServiceBusPublisher
  {
    private readonly ILockName m_serviceBusPublisherLock;
    private bool m_isServiceBusNotifierJobQueued;
    private IDictionary<Guid, IDictionary<Guid, IDictionary<int, int>>> m_recencyData;

    public WorkItemRecentActivityServiceBusPublisher(IVssRequestContext systemRequestContext)
    {
      this.m_isServiceBusNotifierJobQueued = false;
      this.m_serviceBusPublisherLock = systemRequestContext.ServiceHost.CreateLockName(systemRequestContext.ServiceHost.InstanceId.ToString() + "RecentActivitySBPublisherLock");
    }

    public void TrySendNotificationsToServiceBus(
      IVssRequestContext requestContext,
      IReadOnlyCollection<(int workItemId, Guid projectId, int areaId)> recentActivityDetails,
      Guid userId)
    {
      try
      {
        foreach ((int workItemId, Guid projectId, int areaId) recentActivityDetail in (IEnumerable<(int workItemId, Guid projectId, int areaId)>) recentActivityDetails)
          this.AddEventsToRecencyDataMap(requestContext, recentActivityDetail, userId);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, "Fire Work Item failed to queue event", ex, TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
      }
    }

    private void AddEventsToRecencyDataMap(
      IVssRequestContext requestContext,
      (int workItemId, Guid projectId, int areaId) ev,
      Guid userId)
    {
      bool flag = false;
      using (requestContext.LockManager.Lock(this.m_serviceBusPublisherLock))
      {
        if (this.m_recencyData == null)
          this.m_recencyData = (IDictionary<Guid, IDictionary<Guid, IDictionary<int, int>>>) new Dictionary<Guid, IDictionary<Guid, IDictionary<int, int>>>();
        IDictionary<Guid, IDictionary<int, int>> dictionary1;
        if (!this.m_recencyData.TryGetValue(ev.projectId, out dictionary1))
        {
          this.m_recencyData.Add(ev.projectId, (IDictionary<Guid, IDictionary<int, int>>) new Dictionary<Guid, IDictionary<int, int>>()
          {
            {
              userId,
              (IDictionary<int, int>) new Dictionary<int, int>()
              {
                {
                  ev.workItemId,
                  ev.areaId
                }
              }
            }
          });
        }
        else
        {
          IDictionary<int, int> dictionary2;
          if (!dictionary1.TryGetValue(userId, out dictionary2))
            dictionary1.Add(userId, (IDictionary<int, int>) new Dictionary<int, int>()
            {
              {
                ev.workItemId,
                ev.areaId
              }
            });
          else if (!dictionary2.TryGetValue(ev.workItemId, out int _))
            dictionary2.Add(ev.workItemId, ev.areaId);
          else
            dictionary2[ev.workItemId] = ev.areaId;
        }
        if (!this.m_isServiceBusNotifierJobQueued)
        {
          flag = true;
          this.m_isServiceBusNotifierJobQueued = true;
        }
      }
      if (!flag)
        return;
      TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.ExecuteServiceBusNotifier), (object) null, DateTime.UtcNow.AddSeconds((double) requestContext.WitContext().ServerSettings.WorkItemUpdateEventsAggregationTime), 0);
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext.ServiceHost.InstanceId, task);
    }

    private void ExecuteServiceBusNotifier(IVssRequestContext requestContext, object payload)
    {
      IDictionary<Guid, IDictionary<Guid, IDictionary<int, int>>> recencyData;
      using (requestContext.LockManager.Lock(this.m_serviceBusPublisherLock))
      {
        recencyData = this.m_recencyData;
        this.m_recencyData = (IDictionary<Guid, IDictionary<Guid, IDictionary<int, int>>>) null;
        this.m_isServiceBusNotifierJobQueued = false;
      }
      if (recencyData.Count <= 0)
        return;
      List<Microsoft.TeamFoundation.WorkItemTracking.Notification.WorkItemRecentActivityEvent> recentActivityEvents = this.GetWorkItemRecentActivityEvents(recencyData);
      requestContext.Trace(904918, TraceLevel.Verbose, "Services", "WorkItemService", string.Format("Attempting to publish {0} events to the service bus", (object) recencyData.Count));
      ServiceBusPublisher.PublishToServiceBus(requestContext, "Microsoft.TeamFoundation.WorkItemTracking.Server", "workitem.recentactivity", (object) new WorkItemRecentActivityNotification()
      {
        Events = recentActivityEvents
      });
    }

    private List<Microsoft.TeamFoundation.WorkItemTracking.Notification.WorkItemRecentActivityEvent> GetWorkItemRecentActivityEvents(
      IDictionary<Guid, IDictionary<Guid, IDictionary<int, int>>> eventsToProcess)
    {
      List<Microsoft.TeamFoundation.WorkItemTracking.Notification.WorkItemRecentActivityEvent> recentActivityEvents = new List<Microsoft.TeamFoundation.WorkItemTracking.Notification.WorkItemRecentActivityEvent>();
      foreach (KeyValuePair<Guid, IDictionary<Guid, IDictionary<int, int>>> keyValuePair1 in (IEnumerable<KeyValuePair<Guid, IDictionary<Guid, IDictionary<int, int>>>>) eventsToProcess)
      {
        foreach (KeyValuePair<Guid, IDictionary<int, int>> keyValuePair2 in (IEnumerable<KeyValuePair<Guid, IDictionary<int, int>>>) keyValuePair1.Value)
        {
          Microsoft.TeamFoundation.WorkItemTracking.Notification.WorkItemRecentActivityEvent recentActivityEvent = new Microsoft.TeamFoundation.WorkItemTracking.Notification.WorkItemRecentActivityEvent()
          {
            ProjectId = keyValuePair1.Key,
            UserId = keyValuePair2.Key,
            WorkItemIdsAndAreaIds = (Dictionary<int, int>) keyValuePair2.Value
          };
          recentActivityEvents.Add(recentActivityEvent);
        }
      }
      return recentActivityEvents;
    }

    internal IDictionary<Guid, IDictionary<Guid, IDictionary<int, int>>> GetRecencyData() => this.m_recencyData;
  }
}
