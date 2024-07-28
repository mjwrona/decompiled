// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemChangedEventServiceBusPublisher
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Notification;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  public class WorkItemChangedEventServiceBusPublisher
  {
    private readonly ILockName m_serviceBusPublisherLock;
    private bool m_isServiceBusNotifierJobQueued;
    private IDictionary<Guid, IDictionary<string, HashSet<string>>> m_projectsModified = (IDictionary<Guid, IDictionary<string, HashSet<string>>>) new Dictionary<Guid, IDictionary<string, HashSet<string>>>();

    public WorkItemChangedEventServiceBusPublisher(IVssRequestContext systemRequestContext) => this.m_serviceBusPublisherLock = systemRequestContext.ServiceHost.CreateLockName(systemRequestContext.ServiceHost.InstanceId.ToString() + "sbPublisherLock");

    public virtual void TrySendNotificationsToServiceBus(
      IVssRequestContext requestContext,
      List<WorkItemChangedEventExtended> events,
      string workItemChangedEventType = "workitem.updated")
    {
      try
      {
        foreach (KeyValuePair<Guid, IEnumerable<string>> keyValuePair in events.GroupBy<WorkItemChangedEventExtended, string>((Func<WorkItemChangedEventExtended, string>) (x => x.LegacyChangedEvent.ProjectNodeId)).ToDictionary<IGrouping<string, WorkItemChangedEventExtended>, Guid, IEnumerable<string>>((Func<IGrouping<string, WorkItemChangedEventExtended>, Guid>) (x => new Guid(x.Key)), (Func<IGrouping<string, WorkItemChangedEventExtended>, IEnumerable<string>>) (x => x.Select<WorkItemChangedEventExtended, string>((Func<WorkItemChangedEventExtended, string>) (ev => ev.LegacyChangedEvent.WorkItemId)))))
          this.AddEventsToModifiedMap(requestContext, keyValuePair.Key, keyValuePair.Value, workItemChangedEventType);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, "Fire Work Item failed to queue event", ex, TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
      }
    }

    public void TrySendNotificationsToServiceBus(
      IVssRequestContext requestContext,
      ISet<Guid> projectIds)
    {
      try
      {
        foreach (Guid projectId in (IEnumerable<Guid>) projectIds)
          this.AddEventsToModifiedMap(requestContext, projectId, Enumerable.Empty<string>(), "workitem.updated");
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, "Fire Work Item failed to queue event", ex, TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
      }
    }

    private void AddEventsToModifiedMap(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> modifiedWorkItems,
      string workItemChangedEventType)
    {
      bool flag = false;
      using (requestContext.LockManager.Lock(this.m_serviceBusPublisherLock))
      {
        IDictionary<string, HashSet<string>> dictionary;
        if (this.m_projectsModified.TryGetValue(projectId, out dictionary))
        {
          HashSet<string> stringSet;
          if (dictionary.TryGetValue(workItemChangedEventType, out stringSet))
            stringSet.UnionWith(modifiedWorkItems);
          else
            dictionary.Add(workItemChangedEventType, new HashSet<string>(modifiedWorkItems, (IEqualityComparer<string>) StringComparer.Ordinal));
        }
        else
        {
          this.m_projectsModified.Add(projectId, (IDictionary<string, HashSet<string>>) new Dictionary<string, HashSet<string>>());
          this.m_projectsModified[projectId][workItemChangedEventType] = new HashSet<string>(modifiedWorkItems, (IEqualityComparer<string>) StringComparer.Ordinal);
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
      requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext.ServiceHost.InstanceId, task);
    }

    private void ExecuteServiceBusNotifier(IVssRequestContext requestContext, object payload)
    {
      IDictionary<Guid, IDictionary<string, HashSet<string>>> projectsModified;
      using (requestContext.LockManager.Lock(this.m_serviceBusPublisherLock))
      {
        projectsModified = this.m_projectsModified;
        this.m_projectsModified = (IDictionary<Guid, IDictionary<string, HashSet<string>>>) new Dictionary<Guid, IDictionary<string, HashSet<string>>>();
        this.m_isServiceBusNotifierJobQueued = false;
      }
      if (projectsModified.Count <= 0)
        return;
      List<WorkItemChangedProjectEvent> changedProjectEventList = new List<WorkItemChangedProjectEvent>();
      foreach (KeyValuePair<Guid, IDictionary<string, HashSet<string>>> keyValuePair in (IEnumerable<KeyValuePair<Guid, IDictionary<string, HashSet<string>>>>) projectsModified)
      {
        IDictionary<string, HashSet<string>> dictionary = keyValuePair.Value;
        WorkItemChangedProjectEvent changedProjectEvent = new WorkItemChangedProjectEvent()
        {
          ProjectId = keyValuePair.Key,
          EventTime = DateTime.UtcNow
        };
        HashSet<string> stringSet1;
        if (dictionary.TryGetValue("workitem.updated", out stringSet1))
          changedProjectEvent.WorkItemUpdatedList = (IEnumerable<string>) stringSet1;
        HashSet<string> stringSet2;
        if (dictionary.TryGetValue("workitem.destroyed", out stringSet2))
          changedProjectEvent.WorkItemDestroyedList = (IEnumerable<string>) stringSet2;
        changedProjectEventList.Add(changedProjectEvent);
      }
      ServiceBusPublisher.PublishToServiceBus(requestContext, "Microsoft.TeamFoundation.WorkItemTracking.Server", "workitem.changed", (object) new WorkItemChangedNotification()
      {
        Events = changedProjectEventList
      });
    }
  }
}
