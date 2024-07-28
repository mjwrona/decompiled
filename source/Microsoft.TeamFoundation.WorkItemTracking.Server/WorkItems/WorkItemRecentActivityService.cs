// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemRecentActivityService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  public class WorkItemRecentActivityService : IVssFrameworkService
  {
    private WorkItemRecentActivityServiceBusPublisher serviceBusPublisher;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.serviceBusPublisher = new WorkItemRecentActivityServiceBusPublisher(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual void TryFireWorkItemRecentActivityEvent(
      IVssRequestContext requestContext,
      WorkItemRecentActivityType recentActivityType,
      IReadOnlyCollection<(int workItemId, Guid projectId, int areaId)> workItemIds,
      Guid identityId,
      DateTime activityDate)
    {
      if (!workItemIds.Any<(int, Guid, int)>() || !WorkItemTrackingFeatureFlags.ShouldTrackRecentActivity(requestContext) || !CommonWITUtils.HasTrackRecentActivityPermission(requestContext))
        return;
      requestContext.TraceBlock(904893, 904895, 904894, "Services", nameof (WorkItemRecentActivityService), nameof (TryFireWorkItemRecentActivityEvent), (Action) (() =>
      {
        try
        {
          WorkItemRecentActivityEvent ev = new WorkItemRecentActivityEvent()
          {
            ActivityDate = activityDate,
            ActivityType = recentActivityType,
            IdentityId = identityId,
            WorkItemIds = (IReadOnlyCollection<(int, Guid)>) workItemIds.Select<(int, Guid, int), (int, Guid)>((Func<(int, Guid, int), (int, Guid)>) (a => (a.workItemId, a.projectId))).ToList<(int, Guid)>()
          };
          requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((innerRequestContext, _) => innerRequestContext.TraceBlock(909715, 909716, 909717, "Services", nameof (WorkItemRecentActivityService), "WorkItemRecentActivityEventsCallBack", (Action) (() => innerRequestContext.GetService<ITeamFoundationEventService>().PublishNotification(innerRequestContext, (object) ev)))));
          if (!WorkItemTrackingFeatureFlags.ShouldPublishRecentActivityEventToServiceBus(requestContext))
            return;
          this.serviceBusPublisher.TrySendNotificationsToServiceBus(requestContext, workItemIds, identityId);
        }
        catch (Exception ex)
        {
          TeamFoundationEventLog.Default.LogException(requestContext, "Fire Work Item failed to queue event", ex, TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
        }
      }));
    }
  }
}
