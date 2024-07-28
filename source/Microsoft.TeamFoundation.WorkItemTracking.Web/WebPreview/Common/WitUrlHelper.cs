// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Common.WitUrlHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Location.Server;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Common
{
  internal static class WitUrlHelper
  {
    public static string GetWorkItemUrl(
      IVssRequestContext tfsRequestContext,
      int workItemId,
      UrlHelper urlHelper)
    {
      return urlHelper != null ? urlHelper.RestLink(tfsRequestContext, WorkItemTrackingLocationIds.WorkItems, (object) new
      {
        id = workItemId
      }) : tfsRequestContext.GetService<ILocationService>().GetResourceUri(tfsRequestContext, "wit", WorkItemTrackingLocationIds.WorkItems, (object) new
      {
        id = workItemId
      }).AbsoluteUri;
    }

    public static string GetWorkItemUpdatesUrl(
      IVssRequestContext tfsRequestContext,
      int workItemId,
      UrlHelper urlHelper)
    {
      return urlHelper != null ? urlHelper.RestLink(tfsRequestContext, WorkItemTrackingLocationIds.Updates, (object) new
      {
        id = workItemId
      }) : tfsRequestContext.GetService<ILocationService>().GetResourceUri(tfsRequestContext, "wit", WorkItemTrackingLocationIds.Updates, (object) new
      {
        id = workItemId
      }).AbsoluteUri;
    }

    public static string GetWorkItemUpdatesUrl(
      IVssRequestContext tfsRequestContext,
      int revisionId,
      int updateNum,
      UrlHelper urlHelper)
    {
      return urlHelper != null ? urlHelper.RestLink(tfsRequestContext, WorkItemTrackingLocationIds.Updates, (object) new
      {
        id = revisionId,
        updateNumber = updateNum
      }) : tfsRequestContext.GetService<ILocationService>().GetResourceUri(tfsRequestContext, "wit", WorkItemTrackingLocationIds.Updates, (object) new
      {
        id = revisionId,
        updateNumber = updateNum
      }).AbsoluteUri;
    }

    public static string GetWorkItemRevisionUrl(
      IVssRequestContext tfsRequestContext,
      int revisionId,
      int RevisionNum,
      UrlHelper urlHelper)
    {
      return urlHelper != null ? urlHelper.RestLink(tfsRequestContext, WorkItemTrackingLocationIds.Revisions, (object) new
      {
        id = revisionId,
        revisionNumber = RevisionNum
      }) : tfsRequestContext.GetService<ILocationService>().GetResourceUri(tfsRequestContext, "wit", WorkItemTrackingLocationIds.Revisions, (object) new
      {
        id = revisionId,
        revisionNumber = RevisionNum
      }).AbsoluteUri;
    }

    public static string GetResourceLinkLocation(
      IVssRequestContext tfsRequestContext,
      WorkItemResourceLinkInfo linkInfo,
      UrlHelper urlHelper)
    {
      if (linkInfo.ResourceType != ResourceLinkType.Attachment)
        return linkInfo.Location;
      return urlHelper != null ? urlHelper.RestLink(tfsRequestContext, WorkItemTrackingLocationIds.Attachments, (object) new
      {
        id = linkInfo.Location,
        fileName = linkInfo.Name
      }) : tfsRequestContext.GetService<ILocationService>().GetResourceUri(tfsRequestContext, "wit", WorkItemTrackingLocationIds.Attachments, (object) new
      {
        id = linkInfo.Location,
        fileName = linkInfo.Name
      }).AbsoluteUri;
    }

    public static string GetFieldUrl(
      IVssRequestContext tfsRequestContext,
      int fieldId,
      UrlHelper urlHelper)
    {
      return urlHelper != null ? urlHelper.RestLink(tfsRequestContext, WorkItemTrackingLocationIds.Fields, (object) new
      {
        id = fieldId
      }) : tfsRequestContext.GetService<ILocationService>().GetResourceUri(tfsRequestContext, "wit", WorkItemTrackingLocationIds.Fields, (object) new
      {
        id = fieldId
      }).AbsoluteUri;
    }
  }
}
