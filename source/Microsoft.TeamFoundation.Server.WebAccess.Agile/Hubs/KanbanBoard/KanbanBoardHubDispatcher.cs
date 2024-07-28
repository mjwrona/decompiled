// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.KanbanBoard.KanbanBoardHubDispatcher
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.AspNet.SignalR;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.KanbanBoard
{
  public sealed class KanbanBoardHubDispatcher : 
    BacklogsHubDispatcher<IKanbanBoardHubClient>,
    IKanbanBoardHubDispatcher,
    IBacklogsHubDispatcher,
    IVssFrameworkService
  {
    public Task WatchKanbanBoard(
      IVssRequestContext requestContext,
      Guid workItemTypeExtensionId,
      string connectionId)
    {
      ArgumentUtility.CheckForNull<string>(connectionId, nameof (connectionId));
      string iemTypeExtension = KanbanBoardHubDispatcher.GetGroupNameByWorkIemTypeExtension(requestContext, workItemTypeExtensionId);
      return this.m_hubContext.Groups.Add(connectionId, iemTypeExtension);
    }

    public Task UnwatchKanbanBoard(
      IVssRequestContext requestContext,
      Guid workItemTypeExtensionId,
      string connectionId)
    {
      ArgumentUtility.CheckForNull<string>(connectionId, nameof (connectionId));
      string iemTypeExtension = KanbanBoardHubDispatcher.GetGroupNameByWorkIemTypeExtension(requestContext, workItemTypeExtensionId);
      return this.m_hubContext.Groups.Remove(connectionId, iemTypeExtension);
    }

    public void NotifyCommonSettingsChanged(
      IVssRequestContext requestContext,
      IEnumerable<Guid> workItemTypeExtensionIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(workItemTypeExtensionIds, nameof (workItemTypeExtensionIds));
      try
      {
        foreach (Guid itemTypeExtensionId in workItemTypeExtensionIds)
        {
          string iemTypeExtension = KanbanBoardHubDispatcher.GetGroupNameByWorkIemTypeExtension(requestContext, itemTypeExtensionId);
          this.m_hubContext.Clients.Group(iemTypeExtension).OnCommonSettingsChanged(requestContext);
          requestContext.Trace(290544, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "OnCommonSettingsChanged notification sent for client with ID : {0}", (object) iemTypeExtension);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290535, "Agile", TfsTraceLayers.BusinessLogic, ex);
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<KanbanBoardHub, IKanbanBoardHubClient>();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => this.m_hubContext = (IHubContext<IKanbanBoardHubClient>) null;

    protected override HashSet<string> GetNewGroupNames(
      IVssRequestContext requestContext,
      WorkItemChangedEventExtended workItemChangedEvent)
    {
      return this.GetGroupNames(requestContext, workItemChangedEvent.CurrentExtensionIds);
    }

    protected override HashSet<string> GetOldGroupNames(
      IVssRequestContext requestContext,
      WorkItemChangedEventExtended workItemChangedEvent)
    {
      return this.GetGroupNames(requestContext, workItemChangedEvent.OldExtensionIds);
    }

    private HashSet<string> GetGroupNames(
      IVssRequestContext requestContext,
      List<Guid> extensionIds)
    {
      HashSet<string> groupNames = new HashSet<string>();
      if (extensionIds != null)
      {
        foreach (Guid extensionId in extensionIds)
        {
          string iemTypeExtension = KanbanBoardHubDispatcher.GetGroupNameByWorkIemTypeExtension(requestContext, extensionId);
          groupNames.Add(iemTypeExtension);
        }
      }
      return groupNames;
    }

    private static string GetGroupNameByWorkIemTypeExtension(
      IVssRequestContext requestContext,
      Guid workItemTypeExtensionId)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId, (object) workItemTypeExtensionId);
    }
  }
}
