// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WorkItemHub.WorkItemHubDispatcher
// Assembly: Microsoft.Azure.Boards.WorkItemHub, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 749A696A-54F8-4B6F-8877-B350F1725C24
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.WorkItemHub.dll

using Microsoft.AspNet.SignalR;
using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Comments.Server.Events;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Boards.WorkItemHub
{
  public sealed class WorkItemHubDispatcher : IWorkItemHubDispatcher, IVssFrameworkService
  {
    private IHubContext<IWorkItemHubClient> m_hubContext;

    public async Task WatchWorkItem(
      IVssRequestContext requestContext,
      int workItemId,
      string connectionId)
    {
      ArgumentUtility.CheckForNull<string>(connectionId, nameof (connectionId));
      using (requestContext.TraceBlock(919752, 919753, "WebAccess.WorkItem.SignalR", nameof (WorkItemHubDispatcher), nameof (WatchWorkItem)))
      {
        string nameByWorkItemId = WorkItemHubDispatcher.GetGroupNameByWorkItemId(requestContext, workItemId);
        await this.m_hubContext.Groups.Add(connectionId, nameByWorkItemId).ConfigureAwait(false);
      }
    }

    public async Task UnwatchWorkItem(
      IVssRequestContext requestContext,
      int workItemId,
      string connectionId)
    {
      ArgumentUtility.CheckForNull<string>(connectionId, nameof (connectionId));
      using (requestContext.TraceBlock(919754, 919755, "WebAccess.WorkItem.SignalR", nameof (WorkItemHubDispatcher), nameof (UnwatchWorkItem)))
      {
        string nameByWorkItemId = WorkItemHubDispatcher.GetGroupNameByWorkItemId(requestContext, workItemId);
        await this.m_hubContext.Groups.Remove(connectionId, nameByWorkItemId).ConfigureAwait(false);
      }
    }

    public void NotifyWorkItemsChanged(
      IVssRequestContext requestContext,
      WorkItemChangedEvent workItemChangedEvent)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, nameof (workItemChangedEvent));
      try
      {
        if (this.m_hubContext != null)
        {
          ChangeType changeType = WorkItemChangedHelper.GetChangeType(workItemChangedEvent);
          if (changeType == ChangeType.New)
            return;
          int coreIntField1 = WorkItemChangedHelper.GetCoreIntField(workItemChangedEvent, "System.Id");
          int coreIntField2 = WorkItemChangedHelper.GetCoreIntField(workItemChangedEvent, "System.Rev");
          WorkItemUpdateEvent updateEvent = new WorkItemUpdateEvent(coreIntField1, coreIntField2, workItemChangedEvent.ProjectNodeId, changeType, workItemChangedEvent.HasOnlyLinkUpdates);
          this.SendEventsToClients(requestContext, coreIntField1, updateEvent);
        }
        else
          requestContext.Trace(290962, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, "Null reference to hub context in WorkItemHubDispatcher");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290534, "Agile", TfsTraceLayers.BusinessLogic, ex);
      }
    }

    public void NotifyWorkItemsChanged(
      IVssRequestContext requestContext,
      CommentReactionChangedEvent commentReactionChangedEvent)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<CommentReactionChangedEvent>(commentReactionChangedEvent, nameof (commentReactionChangedEvent));
      try
      {
        if (this.m_hubContext != null)
        {
          Guid projectId = commentReactionChangedEvent.ProjectId;
          Guid artifactKind = commentReactionChangedEvent.ArtifactKind;
          string artifactId = commentReactionChangedEvent.ArtifactId;
          int commentId = commentReactionChangedEvent.CommentId;
          IDisposableReadOnlyList<ICommentProvider> extensions = requestContext.GetExtensions<ICommentProvider>(ExtensionLifetime.Service);
          ArtifactInfo artifactInfo = ((extensions != null ? extensions.FirstOrDefault<ICommentProvider>((Func<ICommentProvider, bool>) (p => p.ArtifactKind == artifactKind)) : (ICommentProvider) null) ?? throw new CommentProviderNotRegisteredException(artifactKind)).GetArtifactInfo(requestContext, projectId, artifactId);
          int result;
          int revision;
          if (!int.TryParse(artifactId, out result) || !artifactInfo.ArtifactProperties.TryGetValue<int>("Revision", out revision))
            return;
          WorkItemUpdateEvent updateEvent = new WorkItemUpdateEvent(result, revision, projectId.ToString(), ChangeType.Change, false, true, commentId);
          this.SendEventsToClients(requestContext, result, updateEvent);
        }
        else
          requestContext.Trace(919756, TraceLevel.Error, "WebAccess.WorkItem.SignalR", TfsTraceLayers.BusinessLogic, "Null reference to hub context in WorkItemHubDispatcher");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(919757, "WebAccess.WorkItem.SignalR", TfsTraceLayers.BusinessLogic, ex);
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<Microsoft.Azure.Boards.WorkItemHub.WorkItemHub, IWorkItemHubClient>();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => this.m_hubContext = (IHubContext<IWorkItemHubClient>) null;

    private static string GetGroupNameByWorkItemId(
      IVssRequestContext requestContext,
      int workItemId)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId, (object) workItemId);
    }

    private void SendEventsToClients(
      IVssRequestContext requestContext,
      int workItemId,
      WorkItemUpdateEvent updateEvent)
    {
      Dictionary<string, List<WorkItemUpdateEvent>> groupToUpdateEventsMap = new Dictionary<string, List<WorkItemUpdateEvent>>();
      string nameByWorkItemId = WorkItemHubDispatcher.GetGroupNameByWorkItemId(requestContext, workItemId);
      groupToUpdateEventsMap[nameByWorkItemId] = new List<WorkItemUpdateEvent>()
      {
        updateEvent
      };
      this.DispatchEventsToClients(requestContext, groupToUpdateEventsMap);
    }

    private void DispatchEventsToClients(
      IVssRequestContext requestContext,
      Dictionary<string, List<WorkItemUpdateEvent>> groupToUpdateEventsMap)
    {
      foreach (KeyValuePair<string, List<WorkItemUpdateEvent>> groupToUpdateEvents in groupToUpdateEventsMap)
        this.m_hubContext.Clients.Group(groupToUpdateEvents.Key).OnWorkItemUpdated(requestContext, groupToUpdateEvents.Value);
    }
  }
}
