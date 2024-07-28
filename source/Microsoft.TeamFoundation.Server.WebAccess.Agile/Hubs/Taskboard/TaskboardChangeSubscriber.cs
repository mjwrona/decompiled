// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Taskboard.TaskboardChangeSubscriber
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server.TaskBoard;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Events;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Taskboard
{
  public class TaskboardChangeSubscriber : ISubscriber
  {
    private static readonly Type[] SubscribedTypes = new Type[5]
    {
      typeof (WorkItemChangedEvent),
      typeof (TeamSettingsChangedEvent),
      typeof (TaskboardColumnOptionsChangedEvent),
      typeof (Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumnChangedEvent),
      typeof (TaskboardCardSettingsChangedEvent)
    };

    public string Name => "Taskboard Changed Notification Subscriber";

    public SubscriberPriority Priority => SubscriberPriority.Normal;

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      statusCode = 0;
      statusMessage = (string) null;
      properties = (ExceptionPropertyCollection) null;
      if (notificationType == NotificationType.DecisionPoint || requestContext.IsFeatureEnabled("WebAccess.Agile.Taskboard.DisableLiveUpdates"))
        return EventNotificationStatus.ActionPermitted;
      using (new TraceWatch(requestContext, 90002000, TraceLevel.Info, TimeSpan.FromMilliseconds(0.0), "Agile", TfsTraceLayers.BusinessLogic, "ProcessTaskboardChangeEvent", Array.Empty<object>()))
      {
        try
        {
          WorkItemChangedEvent workItemChangedEvent = notificationEventArgs as WorkItemChangedEvent;
          TeamSettingsChangedEvent settingsChangedEvent1 = notificationEventArgs as TeamSettingsChangedEvent;
          TaskboardColumnOptionsChangedEvent optionsChangedEvent = notificationEventArgs as TaskboardColumnOptionsChangedEvent;
          Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumnChangedEvent columnChangedEvent = notificationEventArgs as Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumnChangedEvent;
          TaskboardCardSettingsChangedEvent settingsChangedEvent2 = notificationEventArgs as TaskboardCardSettingsChangedEvent;
          ITaskboardHubDispatcher service = requestContext.GetService<ITaskboardHubDispatcher>();
          if (workItemChangedEvent != null)
            service.NotifyWorkItemChanged(requestContext, workItemChangedEvent);
          else if (settingsChangedEvent2 != null)
            service.NotifyTaskboardCardSettingsChanged(requestContext, settingsChangedEvent2.TeamId);
          else if (settingsChangedEvent1 != null && settingsChangedEvent1.TeamIds.Any<Guid>() && (settingsChangedEvent1.ChangeType == TeamSettingsChangeType.UpdateTeamFields || settingsChangedEvent1.ChangeType == TeamSettingsChangeType.UpdateBugsBehavior || settingsChangedEvent1.ChangeType == TeamSettingsChangeType.UpdateDefaultIteration))
            service.NotifyTeamSettingsChanged(requestContext, settingsChangedEvent1.TeamIds);
          else if (optionsChangedEvent != null)
            service.NotifyColumnOptionsChanged(requestContext, optionsChangedEvent.ProjectId, optionsChangedEvent.TeamId);
          else if (columnChangedEvent != null)
            service.NotifyWorkItemColumnChanged(requestContext, new Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumnChangedEvent()
            {
              ColumnId = columnChangedEvent.ColumnId,
              WorkItemId = columnChangedEvent.WorkItemId,
              TeamId = columnChangedEvent.TeamId,
              ProjectId = columnChangedEvent.ProjectId
            });
        }
        catch (Exception ex)
        {
          requestContext.TraceException(90002001, "Agile", TfsTraceLayers.BusinessLogic, ex);
        }
      }
      return EventNotificationStatus.ActionPermitted;
    }

    Type[] ISubscriber.SubscribedTypes() => TaskboardChangeSubscriber.SubscribedTypes;
  }
}
