// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RelatedWorkItemsHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class RelatedWorkItemsHelper
  {
    public static IEnumerable<SourceRelatedWorkItem> GetVstsWorkItemDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> workItemIds)
    {
      if (workItemIds != null && workItemIds.Any<int>())
      {
        IEnumerable<WorkItem> workItems = requestContext.GetService<IWorkItemRemotableService>().GetWorkItems(requestContext, workItemIds, projectId);
        if (workItems != null && workItems.Any<WorkItem>())
          return workItems.OrderBy<WorkItem, int?>((Func<WorkItem, int?>) (x => x.Id)).Select<WorkItem, SourceRelatedWorkItem>((Func<WorkItem, SourceRelatedWorkItem>) (x => RelatedWorkItemsHelper.ToRelatedWorkItem(requestContext, x)));
      }
      return Enumerable.Empty<SourceRelatedWorkItem>();
    }

    private static SourceRelatedWorkItem ToRelatedWorkItem(
      IVssRequestContext requestContext,
      WorkItem workItem)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      IdentityRef identityRef;
      workItem.Fields.TryGetValue<IdentityRef>(CoreFieldReferenceNames.AssignedTo, out identityRef);
      string str1;
      workItem.Fields.TryGetValue<string>(CoreFieldReferenceNames.State, out str1);
      string str2;
      workItem.Fields.TryGetValue<string>(CoreFieldReferenceNames.Description, out str2);
      string str3;
      workItem.Fields.TryGetValue<string>(CoreFieldReferenceNames.Title, out str3);
      string str4;
      workItem.Fields.TryGetValue<string>(CoreFieldReferenceNames.WorkItemType, out str4);
      string str5;
      workItem.Fields.TryGetValue<string>(CoreFieldReferenceNames.TeamProject, out str5);
      SourceRelatedWorkItem sourceRelatedWorkItem1 = new SourceRelatedWorkItem((ISecuredObject) workItem);
      sourceRelatedWorkItem1.AssignedTo = identityRef;
      sourceRelatedWorkItem1.CurrentState = str1;
      sourceRelatedWorkItem1.Description = str2;
      int? id1 = workItem.Id;
      ref int? local = ref id1;
      sourceRelatedWorkItem1.Id = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
      sourceRelatedWorkItem1.ProviderName = WorkItemProviderTypes.Vsts;
      sourceRelatedWorkItem1.Title = str3;
      sourceRelatedWorkItem1.Type = str4;
      SourceRelatedWorkItem relatedWorkItem = sourceRelatedWorkItem1;
      ReferenceLinks links1 = relatedWorkItem.Links;
      IVssRequestContext requestContext1 = requestContext;
      id1 = workItem.Id;
      int id2 = id1.Value;
      ILocationService locationService = service;
      string workItemUrlById = WITHelper.GetWorkItemUrlById(requestContext1, id2, locationService);
      SourceRelatedWorkItem sourceRelatedWorkItem2 = relatedWorkItem;
      links1.AddLink("self", workItemUrlById, (ISecuredObject) sourceRelatedWorkItem2);
      ReferenceLinks links2 = relatedWorkItem.Links;
      IVssRequestContext requestContext2 = requestContext;
      string projectName = str5;
      id1 = workItem.Id;
      int? id3 = new int?(id1.Value);
      string itemWebAccessUri = WebAccessUrlBuilder.GetWorkItemWebAccessUri(requestContext2, projectName, id3);
      SourceRelatedWorkItem sourceRelatedWorkItem3 = relatedWorkItem;
      links2.AddLink("web", itemWebAccessUri, (ISecuredObject) sourceRelatedWorkItem3);
      return relatedWorkItem;
    }
  }
}
