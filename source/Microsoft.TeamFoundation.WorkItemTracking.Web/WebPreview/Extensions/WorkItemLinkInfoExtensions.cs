// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions.WorkItemLinkInfoExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions
{
  public static class WorkItemLinkInfoExtensions
  {
    public static WorkItemLink ToWorkItemLink(
      this WorkItemLinkInfo linkInfo,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper)
    {
      WorkItemLink workItemLink = new WorkItemLink();
      workItemLink.Comment = linkInfo.Comment;
      workItemLink.LinkType = WorkItemLink.GetWorkItemLinkType(linkInfo.LinkType, tfsRequestContext);
      workItemLink.Locked = new bool?(linkInfo.IsLocked);
      workItemLink.Source = new WorkItemReference(linkInfo.SourceId, tfsRequestContext, urlHelper);
      workItemLink.Target = new WorkItemReference(linkInfo.TargetId, tfsRequestContext, urlHelper);
      return workItemLink;
    }

    public static WorkItemLinkUpdate ToWorkItemLinkUpdate(
      this WorkItemLinkInfo linkInfo,
      LinkUpdateType updateType,
      IVssRequestContext tfsRequestContext)
    {
      WorkItemLinkUpdate workItemLinkUpdate = new WorkItemLinkUpdate();
      workItemLinkUpdate.Comment = linkInfo.Comment;
      workItemLinkUpdate.LinkType = WorkItemLink.GetWorkItemLinkType(linkInfo.LinkType, tfsRequestContext);
      workItemLinkUpdate.Locked = new bool?(linkInfo.IsLocked);
      workItemLinkUpdate.SourceWorkItemId = linkInfo.SourceId;
      workItemLinkUpdate.TargetWorkItemId = linkInfo.TargetId;
      workItemLinkUpdate.UpdateType = updateType;
      return workItemLinkUpdate;
    }
  }
}
