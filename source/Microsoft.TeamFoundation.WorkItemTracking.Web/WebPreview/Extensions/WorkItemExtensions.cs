// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions.WorkItemExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions
{
  public static class WorkItemExtensions
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItem ToWorkItem(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem,
      IVssRequestContext tfsRequestContext)
    {
      return WorkItemExtensions.ToWorkItem(workItem, tfsRequestContext, (UrlHelper) null);
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItem ToWorkItem(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper)
    {
      WorkItemTrackingRequestContext witRequestContext = tfsRequestContext.WitContext();
      ITswaServerHyperlinkService service = tfsRequestContext.GetService<ITswaServerHyperlinkService>();
      Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItem workItem1 = new Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItem();
      workItem1.Id = workItem.Id;
      workItem1.Rev = workItem.Revision;
      workItem1.Fields = (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue>) workItem.GetAllFieldValuesByFieldEntry(witRequestContext, true).Select<KeyValuePair<FieldEntry, object>, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue>((Func<KeyValuePair<FieldEntry, object>, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue>) (field => new Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue()
      {
        Field = field.Key.ToFieldReference(tfsRequestContext, urlHelper),
        Value = field.Value
      })).ToArray<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue>();
      workItem1.Links = workItem.WorkItemLinks == null || !workItem.WorkItemLinks.Any<WorkItemLinkInfo>() ? (IEnumerable<WorkItemLink>) (WorkItemLink[]) null : (IEnumerable<WorkItemLink>) workItem.WorkItemLinks.Select<WorkItemLinkInfo, WorkItemLink>((Func<WorkItemLinkInfo, WorkItemLink>) (link => link.ToWorkItemLink(tfsRequestContext, urlHelper))).ToArray<WorkItemLink>();
      workItem1.ResourceLinks = workItem.ResourceLinks == null || !workItem.ResourceLinks.Any<WorkItemResourceLinkInfo>() ? (IEnumerable<WorkItemResourceLink>) (WorkItemResourceLink[]) null : (IEnumerable<WorkItemResourceLink>) workItem.ResourceLinks.Select<WorkItemResourceLinkInfo, WorkItemResourceLink>((Func<WorkItemResourceLinkInfo, WorkItemResourceLink>) (link => link.ToWorkItemResourceLink(tfsRequestContext, urlHelper))).ToArray<WorkItemResourceLink>();
      workItem1.Url = WitUrlHelper.GetWorkItemUrl(tfsRequestContext, workItem.Id, urlHelper);
      workItem1.UpdatesUrl = WitUrlHelper.GetWorkItemUpdatesUrl(tfsRequestContext, workItem.Id, urlHelper);
      workItem1.WebUrl = service.GetWorkItemEditorUrl(tfsRequestContext, workItem.Id).ToString();
      return workItem1;
    }
  }
}
