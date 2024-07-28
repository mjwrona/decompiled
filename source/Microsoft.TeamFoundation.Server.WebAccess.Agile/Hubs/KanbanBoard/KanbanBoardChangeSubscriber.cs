// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.KanbanBoard.KanbanBoardChangeSubscriber
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Events;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.KanbanBoard
{
  public class KanbanBoardChangeSubscriber : ISubscriber
  {
    private static readonly Type[] SubscribedTypes = new Type[3]
    {
      typeof (WorkItemsChangedWithExtensionsBatchEvent),
      typeof (TeamBoardSettingsChangedEvent),
      typeof (TeamSettingsChangedEvent)
    };

    public string Name => "Kanban Board Changed Notification Subscriber";

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
      if (notificationType == NotificationType.DecisionPoint)
        return EventNotificationStatus.ActionPermitted;
      using (new TraceWatch(requestContext, 290549, TraceLevel.Info, TimeSpan.FromMilliseconds(0.0), "Agile", TfsTraceLayers.BusinessLogic, "ProcessKanbanBoardChangeEvent", Array.Empty<object>()))
      {
        try
        {
          WorkItemsChangedWithExtensionsBatchEvent workItemChangedEvent = notificationEventArgs as WorkItemsChangedWithExtensionsBatchEvent;
          TeamBoardSettingsChangedEvent settingsChangedEvent1 = notificationEventArgs as TeamBoardSettingsChangedEvent;
          TeamSettingsChangedEvent settingsChangedEvent2 = notificationEventArgs as TeamSettingsChangedEvent;
          IKanbanBoardHubDispatcher service = requestContext.GetService<IKanbanBoardHubDispatcher>();
          if (workItemChangedEvent != null)
          {
            service.NotifyWorkItemsChanged(requestContext, workItemChangedEvent);
            if (workItemChangedEvent.OriginalNumberOfEvents.HasValue)
              requestContext.Trace(290563, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, "WorkItemsChangedWithExtensionBatchEvent size: {0}", (object) workItemChangedEvent.OriginalNumberOfEvents);
          }
          else if (settingsChangedEvent1 != null)
          {
            List<Guid> list = settingsChangedEvent1.TeamBoardSettingsArtifacts.Select<TeamBoardSettingsArtifact, Guid>((Func<TeamBoardSettingsArtifact, Guid>) (x => x.WorkItemTypeExtensionId)).Where<Guid>((Func<Guid, bool>) (y => y != Guid.Empty)).ToList<Guid>();
            if (list.Count > 0)
              service.NotifyCommonSettingsChanged(requestContext, (IEnumerable<Guid>) list);
          }
          else if (settingsChangedEvent2 != null)
          {
            if (settingsChangedEvent2.TeamIds.Any<Guid>())
            {
              if (settingsChangedEvent2.ChangeType != TeamSettingsChangeType.UpdateTeamFields && settingsChangedEvent2.ChangeType != TeamSettingsChangeType.UpdateBugsBehavior && settingsChangedEvent2.ChangeType != TeamSettingsChangeType.UpdateDefaultIteration)
              {
                if (settingsChangedEvent2.ChangeType != TeamSettingsChangeType.UpdateCumulativeFlowDiagram)
                  goto label_19;
              }
              IEnumerable<Guid> extensionIds = this.GetExtensionIds(requestContext, settingsChangedEvent2.ProjectId, settingsChangedEvent2.TeamIds.First<Guid>());
              if (extensionIds != null)
                service.NotifyCommonSettingsChanged(requestContext, extensionIds);
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(290538, "Agile", TfsTraceLayers.BusinessLogic, ex);
        }
      }
label_19:
      return EventNotificationStatus.ActionPermitted;
    }

    Type[] ISubscriber.SubscribedTypes() => KanbanBoardChangeSubscriber.SubscribedTypes;

    private IEnumerable<Guid> GetExtensionIds(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId)
    {
      IEnumerable<Guid> extensionIds = (IEnumerable<Guid>) null;
      IEnumerable<BoardRecord> allBoards;
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        allBoards = component.GetAllBoards(new Guid?(projectId), new Guid?(teamId));
      if (allBoards != null)
        extensionIds = allBoards.Select<BoardRecord, Guid>((Func<BoardRecord, Guid>) (x => x.WorkItemTypeExtensionId));
      return extensionIds;
    }
  }
}
