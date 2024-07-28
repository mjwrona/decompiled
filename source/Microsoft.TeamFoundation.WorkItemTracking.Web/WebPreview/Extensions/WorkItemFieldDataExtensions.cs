// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions.WorkItemFieldDataExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions
{
  public static class WorkItemFieldDataExtensions
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItem ToWorkItem(
      this WorkItemFieldData workItemFieldData,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper,
      IEnumerable<string> fields = null)
    {
      WorkItemTrackingRequestContext witRequestContext = tfsRequestContext.WitContext();
      ITswaServerHyperlinkService service = tfsRequestContext.GetService<ITswaServerHyperlinkService>();
      Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItem workItem = new Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItem();
      workItem.Id = workItemFieldData.Id;
      workItem.Rev = workItemFieldData.Revision;
      workItem.Fields = workItemFieldData.GetAllFieldValuesByFieldEntry(witRequestContext, true).Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (field => fields == null || !fields.Any<string>() || fields.Contains<string>(field.Key.ReferenceName, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))).Select<KeyValuePair<FieldEntry, object>, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue>((Func<KeyValuePair<FieldEntry, object>, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue>) (field => new Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue()
      {
        Field = field.Key.ToFieldReference(tfsRequestContext, urlHelper),
        Value = field.Value
      }));
      workItem.Url = urlHelper.RestLink(tfsRequestContext, WorkItemTrackingLocationIds.WorkItems, (object) new
      {
        id = workItemFieldData.Id
      });
      workItem.UpdatesUrl = urlHelper.RestLink(tfsRequestContext, WorkItemTrackingLocationIds.Updates, (object) new
      {
        id = workItemFieldData.Id
      });
      workItem.WebUrl = service.GetWorkItemEditorUrl(tfsRequestContext, workItemFieldData.Id).ToString();
      return workItem;
    }
  }
}
