// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions.WorkItemRevisionExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions
{
  public static class WorkItemRevisionExtensions
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItem ToWorkItem(
      this WorkItemRevision revision,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper)
    {
      WorkItemTrackingRequestContext witRequestContext = tfsRequestContext.WitContext();
      ITswaServerHyperlinkService service = tfsRequestContext.GetService<ITswaServerHyperlinkService>();
      Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItem workItem = new Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItem();
      workItem.Id = revision.Id;
      workItem.Rev = revision.Revision;
      workItem.Fields = revision.GetAllFieldValuesByFieldEntry(witRequestContext, true).Select<KeyValuePair<FieldEntry, object>, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue>((Func<KeyValuePair<FieldEntry, object>, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue>) (field => new Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue()
      {
        Field = field.Key.ToFieldReference(tfsRequestContext, urlHelper),
        Value = field.Value
      }));
      workItem.Links = revision.WorkItemLinks == null || !revision.WorkItemLinks.Any<WorkItemLinkInfo>() ? (IEnumerable<WorkItemLink>) null : revision.WorkItemLinks.Select<WorkItemLinkInfo, WorkItemLink>((Func<WorkItemLinkInfo, WorkItemLink>) (link => link.ToWorkItemLink(tfsRequestContext, urlHelper)));
      workItem.ResourceLinks = revision.ResourceLinks == null || !revision.ResourceLinks.Any<WorkItemResourceLinkInfo>() ? (IEnumerable<WorkItemResourceLink>) null : revision.ResourceLinks.Select<WorkItemResourceLinkInfo, WorkItemResourceLink>((Func<WorkItemResourceLinkInfo, WorkItemResourceLink>) (link => link.ToWorkItemResourceLink(tfsRequestContext, urlHelper)));
      workItem.Url = urlHelper.RestLink(tfsRequestContext, WorkItemTrackingLocationIds.WorkItems, (object) new
      {
        id = revision.Id
      });
      workItem.WebUrl = service.GetWorkItemEditorUrl(tfsRequestContext, revision.Id).ToString();
      return workItem;
    }

    public static WorkItemDelta ToWorkItemDelta(
      this WorkItemRevision revision,
      WorkItemRevision updatedRevision,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper)
    {
      return new WorkItemDelta()
      {
        Id = revision.Id,
        Rev = revision.Revision,
        RevisionUrl = WitUrlHelper.GetWorkItemRevisionUrl(tfsRequestContext, revision.Id, revision.Revision, urlHelper),
        Url = WitUrlHelper.GetWorkItemUpdatesUrl(tfsRequestContext, revision.Id, revision.Revision, urlHelper),
        Fields = WorkItemRevisionExtensions.CalculateFieldUpdates(revision, updatedRevision, tfsRequestContext, urlHelper),
        ResourceLinkUpdates = WorkItemRevisionExtensions.CalculateResourceLinkUpdates(revision, updatedRevision),
        LinkUpdates = WorkItemRevisionExtensions.CalculateLinkUpdates(revision, updatedRevision, tfsRequestContext)
      };
    }

    private static IEnumerable<FieldValueUpdate> CalculateFieldUpdates(
      WorkItemRevision original,
      WorkItemRevision updated,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper)
    {
      WorkItemTrackingRequestContext witContext = tfsRequestContext.WitContext();
      original.SetFieldUpdates(tfsRequestContext, (IEnumerable<KeyValuePair<int, object>>) updated.GetAllFieldValuesByFieldEntry(witContext, true).ToDictionary<KeyValuePair<FieldEntry, object>, int, object>((Func<KeyValuePair<FieldEntry, object>, int>) (field => field.Key.FieldId), (Func<KeyValuePair<FieldEntry, object>, object>) (field => field.Value)));
      List<FieldValueUpdate> list = original.Updates.Select<KeyValuePair<int, object>, FieldValueUpdate>((Func<KeyValuePair<int, object>, FieldValueUpdate>) (update =>
      {
        FieldEntry field = witContext.FieldDictionary.GetField(update.Key);
        return new FieldValueUpdate()
        {
          Field = field.ToFieldReference(tfsRequestContext, urlHelper),
          OriginalValue = original.GetFieldValue(tfsRequestContext, update.Key, true),
          UpdatedValue = original.GetFieldValue(tfsRequestContext, update.Key, false)
        };
      })).ToList<FieldValueUpdate>();
      original.Updates.Clear();
      return !list.Any<FieldValueUpdate>() ? (IEnumerable<FieldValueUpdate>) null : (IEnumerable<FieldValueUpdate>) list;
    }

    private static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate> CalculateResourceLinkUpdates(
      WorkItemRevision original,
      WorkItemRevision updated)
    {
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate> collection1 = original.ResourceLinks.Except<WorkItemResourceLinkInfo>((IEnumerable<WorkItemResourceLinkInfo>) updated.ResourceLinks).Select<WorkItemResourceLinkInfo, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkInfo, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate>) (link => link.ToWorkItemResourceLinkUpdate(LinkUpdateType.Delete)));
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate> collection2 = updated.ResourceLinks.Except<WorkItemResourceLinkInfo>((IEnumerable<WorkItemResourceLinkInfo>) original.ResourceLinks).Select<WorkItemResourceLinkInfo, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkInfo, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate>) (link => link.ToWorkItemResourceLinkUpdate(LinkUpdateType.Add)));
      List<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate> source = new List<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate>();
      source.AddRange(collection1);
      source.AddRange(collection2);
      return !source.Any<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate>() ? (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate>) null : (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate>) source;
    }

    private static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate> CalculateLinkUpdates(
      WorkItemRevision original,
      WorkItemRevision updated,
      IVssRequestContext tfsRequestContext)
    {
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate> collection1 = original.WorkItemLinks.Except<WorkItemLinkInfo>(updated.WorkItemLinks).Select<WorkItemLinkInfo, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate>((Func<WorkItemLinkInfo, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate>) (link => link.ToWorkItemLinkUpdate(LinkUpdateType.Delete, tfsRequestContext)));
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate> collection2 = updated.WorkItemLinks.Except<WorkItemLinkInfo>(original.WorkItemLinks).Select<WorkItemLinkInfo, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate>((Func<WorkItemLinkInfo, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate>) (link => link.ToWorkItemLinkUpdate(LinkUpdateType.Add, tfsRequestContext)));
      List<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate> source = new List<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate>();
      source.AddRange(collection1);
      source.AddRange(collection2);
      return !source.Any<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate>() ? (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate>) null : (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate>) source;
    }
  }
}
