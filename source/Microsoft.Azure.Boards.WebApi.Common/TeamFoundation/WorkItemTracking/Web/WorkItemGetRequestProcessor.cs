// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.WorkItemGetRequestProcessor
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web
{
  public static class WorkItemGetRequestProcessor
  {
    private static readonly Version MinimumVersionForGetWorkItemsToAlwaysRespectGetWorkItemsErrorPolicy = new Version(5, 0);

    public static void ProcessWorkItemOptions(
      WorkItemExpand expand,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy errorPolicy,
      out bool includeLinks,
      out Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy serverErrorPolicy)
    {
      includeLinks = expand == WorkItemExpand.All || expand == WorkItemExpand.Links;
      if (errorPolicy != Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy.Fail)
      {
        if (errorPolicy != Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy.Omit)
          throw new ArgumentException(string.Format("Unexpected value: {0}", (object) errorPolicy), nameof (errorPolicy));
        serverErrorPolicy = Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy.Omit;
      }
      else
        serverErrorPolicy = Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy.Fail;
    }

    public static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetWorkItems(
      WorkItemTrackingRequestContext requestContext,
      ITeamFoundationWorkItemService workItemService,
      bool returnProjectScopedUrls,
      ICollection<int> workItemIds,
      ICollection<string> fields,
      DateTime? asOf,
      WorkItemExpand expand,
      bool includeLinks,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy errorPolicy,
      Guid? projectId,
      bool returnIdentityRef,
      bool shouldSkipMaxPageSize = false)
    {
      workItemIds = (ICollection<int>) workItemIds.Distinct<int>().ToList<int>();
      fields = (ICollection<string>) fields.Distinct<string>((IEqualityComparer<string>) VssStringComparer.FieldName).ToList<string>();
      int workItemPageSize = requestContext.ServerSettings.MaxWorkItemPageSize;
      if (!shouldSkipMaxPageSize && workItemIds.Count > workItemPageSize)
        throw new WorkItemPageSizeExceededException(workItemIds.Count, workItemPageSize);
      DateTime? utcAsOf = asOf.HasValue ? new DateTime?(asOf.Value.ToUniversalTime()) : new DateTime?();
      if (utcAsOf.HasValue && (utcAsOf.Value <= (DateTime) SqlDateTime.MinValue || utcAsOf.Value >= (DateTime) SqlDateTime.MaxValue))
        throw new ArgumentOutOfRangeException(ResourceStrings.InvalidAsOfParameter());
      if (fields.Any<string>())
      {
        if (expand != WorkItemExpand.None && expand != WorkItemExpand.Links)
          throw new ConflictingParametersException(ResourceStrings.ExpandParameterConflict());
        IEnumerable<WorkItemFieldData> workItemFieldValues = workItemService.GetWorkItemFieldValues(requestContext.RequestContext, (IEnumerable<int>) workItemIds, (IEnumerable<string>) fields, projectId, asOf: utcAsOf, useWorkItemIdentity: returnIdentityRef);
        if (WorkItemGetRequestProcessor.ShouldGetWorkItemsAlwaysRespectErrorPolicy(requestContext.RequestContext) && errorPolicy == Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy.Fail && workItemFieldValues.Count<WorkItemFieldData>() < workItemIds.Count)
          throw new WorkItemUnauthorizedAccessException(workItemIds.Except<int>(workItemFieldValues.Select<WorkItemFieldData, int>((Func<WorkItemFieldData, int>) (wi => wi.Id))).First<int>(), AccessType.Read);
        return workItemFieldValues.Select<WorkItemFieldData, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>((Func<WorkItemFieldData, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) (wi => WorkItemFactory.Create(requestContext, wi, fields: (IEnumerable<string>) fields, returnIdentityRef: returnIdentityRef, returnProjectScopedUrl: returnProjectScopedUrls, includeLinks: includeLinks)));
      }
      bool includeRelations = expand == WorkItemExpand.All || expand == WorkItemExpand.Relations;
      bool expandFields = expand == WorkItemExpand.All || expand == WorkItemExpand.Fields;
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem> workItems1 = workItemService.GetWorkItems(requestContext.RequestContext, (IEnumerable<int>) workItemIds, utcAsOf, projectId, includeRelations, includeRelations, false, errorPolicy: errorPolicy, includeInRecentActivity: true, useWorkItemIdentity: returnIdentityRef);
      return (!utcAsOf.HasValue ? (IEnumerable<WorkItemRevision>) workItems1 : (IEnumerable<WorkItemRevision>) workItems1.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem, WorkItemRevision>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem, WorkItemRevision>) (workItem =>
      {
        if (workItem == null)
        {
          if (errorPolicy == Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy.Omit)
            return (WorkItemRevision) null;
          throw new NullReferenceException(nameof (workItem));
        }
        if (WorkItemGetRequestProcessor.RevisionExistsAtDateTime((WorkItemRevision) workItem, utcAsOf.Value))
          return (WorkItemRevision) workItem;
        WorkItemRevision workItems2 = workItem.Revisions.FirstOrDefault<WorkItemRevision>((Func<WorkItemRevision, bool>) (revision => WorkItemGetRequestProcessor.RevisionExistsAtDateTime(revision, utcAsOf.Value)));
        if (workItems2 != null)
          return workItems2;
        if (errorPolicy == Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy.Omit)
          return (WorkItemRevision) null;
        throw new WitResourceNotFoundException(ResourceStrings.WorkItemNotFoundAtTime((object) workItem.Id, (object) utcAsOf.Value));
      })).ToArray<WorkItemRevision>()).Select<WorkItemRevision, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>((Func<WorkItemRevision, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) (wi =>
      {
        if (wi != null)
          return WorkItemFactory.Create(requestContext, wi, includeRelations, includeLinks, expandFields, asOf.HasValue, false, (IEnumerable<string>) null, false, false, (IDictionary<Guid, IdentityReference>) null, returnIdentityRef, returnProjectScopedUrls, true);
        if (errorPolicy == Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy.Omit)
          return (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem) null;
        throw new NullReferenceException(nameof (wi));
      }));
    }

    private static bool RevisionExistsAtDateTime(WorkItemRevision revision, DateTime dateTime) => revision.AuthorizedDate <= dateTime && dateTime < revision.RevisedDate;

    private static bool ShouldGetWorkItemsAlwaysRespectErrorPolicy(IVssRequestContext requestContext)
    {
      ApiResourceVersion apiResourceVersion = (ApiResourceVersion) null;
      return requestContext.TryGetItem<ApiResourceVersion>("WitApiResourceVersion", out apiResourceVersion) && apiResourceVersion.ApiVersion >= WorkItemGetRequestProcessor.MinimumVersionForGetWorkItemsToAlwaysRespectGetWorkItemsErrorPolicy;
    }
  }
}
