// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions.WorkItemResourceLinkInfoExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model;
using System;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions
{
  public static class WorkItemResourceLinkInfoExtensions
  {
    public static WorkItemResourceLink ToWorkItemResourceLink(
      this WorkItemResourceLinkInfo linkInfo,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper)
    {
      WorkItemResourceLink itemResourceLink = new WorkItemResourceLink();
      itemResourceLink.Comment = linkInfo.Comment;
      itemResourceLink.CreationDate = new DateTime?(linkInfo.ResourceCreatedDate);
      itemResourceLink.LastModifiedDate = new DateTime?(linkInfo.ResourceModifiedDate);
      itemResourceLink.Length = linkInfo.ResourceSize > 0 ? new int?(linkInfo.ResourceSize) : new int?();
      itemResourceLink.Location = linkInfo.Location;
      itemResourceLink.Name = linkInfo.Name;
      itemResourceLink.ResourceId = new int?(linkInfo.ResourceId);
      itemResourceLink.Source = new WorkItemReference(linkInfo.SourceId, tfsRequestContext, urlHelper);
      itemResourceLink.Type = new ResourceLinkType?(linkInfo.ResourceType.ToResourceLinkType());
      itemResourceLink.Url = WitUrlHelper.GetResourceLinkLocation(tfsRequestContext, linkInfo, urlHelper);
      return itemResourceLink;
    }

    public static WorkItemResourceLinkUpdate ToWorkItemResourceLinkUpdate(
      this WorkItemResourceLinkInfo linkInfo,
      LinkUpdateType updateType)
    {
      WorkItemResourceLinkUpdate resourceLinkUpdate = new WorkItemResourceLinkUpdate();
      resourceLinkUpdate.Comment = linkInfo.Comment;
      resourceLinkUpdate.CreationDate = new DateTime?(linkInfo.ResourceCreatedDate);
      resourceLinkUpdate.LastModifiedDate = new DateTime?(linkInfo.ResourceModifiedDate);
      resourceLinkUpdate.Length = linkInfo.ResourceSize > 0 ? new int?(linkInfo.ResourceSize) : new int?();
      resourceLinkUpdate.Location = linkInfo.Location;
      resourceLinkUpdate.Name = linkInfo.Name;
      resourceLinkUpdate.ResourceId = new int?(linkInfo.ResourceId);
      resourceLinkUpdate.SourceWorkItemId = linkInfo.SourceId;
      resourceLinkUpdate.Type = new ResourceLinkType?(linkInfo.ResourceType.ToResourceLinkType());
      resourceLinkUpdate.UpdateType = updateType;
      return resourceLinkUpdate;
    }
  }
}
