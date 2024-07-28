// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions.WorkItemUpdateResultExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions
{
  public static class WorkItemUpdateResultExtensions
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemUpdateResult ToWorkItemUpdateResult(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdateResult workItemUpdateResult,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper)
    {
      IFieldTypeDictionary fieldDictionary = tfsRequestContext.WitContext().FieldDictionary;
      ITswaServerHyperlinkService service = tfsRequestContext.GetService<ITswaServerHyperlinkService>();
      Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemUpdateResult itemUpdateResult = new Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemUpdateResult();
      itemUpdateResult.Id = workItemUpdateResult.Id;
      itemUpdateResult.UpdateId = workItemUpdateResult.UpdateId;
      itemUpdateResult.Rev = workItemUpdateResult.Rev;
      itemUpdateResult.Url = workItemUpdateResult.Id > 0 ? urlHelper.RestLink(tfsRequestContext, WorkItemTrackingLocationIds.WorkItems, (object) new
      {
        id = workItemUpdateResult.Id
      }) : (string) null;
      itemUpdateResult.UpdatesUrl = workItemUpdateResult.Id > 0 ? urlHelper.RestLink(tfsRequestContext, WorkItemTrackingLocationIds.Updates, (object) new
      {
        id = workItemUpdateResult.Id
      }) : (string) null;
      itemUpdateResult.WebUrl = workItemUpdateResult.Id > 0 ? service.GetWorkItemEditorUrl(tfsRequestContext, workItemUpdateResult.Id).ToString() : (string) null;
      itemUpdateResult.Exception = workItemUpdateResult.Exception != null ? new TeamFoundationServiceException(WorkItemUpdateResultExtensions.GetExceptionMessage(workItemUpdateResult.Exception)) : (TeamFoundationServiceException) null;
      itemUpdateResult.Fields = workItemUpdateResult.Fields == null || !workItemUpdateResult.Fields.Any<KeyValuePair<string, object>>() ? (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue>) null : workItemUpdateResult.Fields.Select<KeyValuePair<string, object>, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue>((Func<KeyValuePair<string, object>, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue>) (field =>
      {
        FieldEntry fieldByNameOrId = fieldDictionary.GetFieldByNameOrId(field.Key);
        return new Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.FieldValue()
        {
          Field = fieldByNameOrId.ToFieldReference(tfsRequestContext, urlHelper),
          Value = field.Value
        };
      }));
      itemUpdateResult.Links = workItemUpdateResult.LinkUpdates == null || !workItemUpdateResult.LinkUpdates.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemLinkUpdateResult>() ? (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdateResult>) null : workItemUpdateResult.LinkUpdates.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemLinkUpdateResult, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdateResult>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemLinkUpdateResult, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdateResult>) (link => link.ToWorkItemLinkUpdateResult(tfsRequestContext, urlHelper)));
      itemUpdateResult.ResourceLinks = workItemUpdateResult.ResourceLinkUpdates == null || !workItemUpdateResult.ResourceLinkUpdates.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemResourceLinkUpdateResult>() ? (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdateResult>) null : workItemUpdateResult.ResourceLinkUpdates.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemResourceLinkUpdateResult, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdateResult>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemResourceLinkUpdateResult, Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdateResult>) (link => link.ToWorkItemResourceLinkUpdateResult(tfsRequestContext, urlHelper)));
      itemUpdateResult.AttachedExtensions = workItemUpdateResult.AttachedExtensions == null || !workItemUpdateResult.AttachedExtensions.Any<Guid>() ? (IEnumerable<Guid>) null : workItemUpdateResult.AttachedExtensions;
      itemUpdateResult.CurrentExtensions = workItemUpdateResult.CurrentExtensions == null || !workItemUpdateResult.CurrentExtensions.Any<Guid>() ? (IEnumerable<Guid>) null : workItemUpdateResult.CurrentExtensions;
      itemUpdateResult.DetachedExtensions = workItemUpdateResult.DetachedExtensions == null || !workItemUpdateResult.DetachedExtensions.Any<Guid>() ? (IEnumerable<Guid>) null : workItemUpdateResult.DetachedExtensions;
      return itemUpdateResult;
    }

    private static string GetExceptionMessage(TeamFoundationServiceException exception)
    {
      if (exception is WorkItemTrackingAggregateException)
      {
        WorkItemTrackingAggregateException aggregateException = (WorkItemTrackingAggregateException) exception;
        if (aggregateException.AllExceptions != null && aggregateException.AllExceptions.Any<TeamFoundationServiceException>())
          return string.Join(",", aggregateException.AllExceptions.Select<TeamFoundationServiceException, string>((Func<TeamFoundationServiceException, string>) (ex => ex.Message)));
      }
      return exception.Message;
    }
  }
}
