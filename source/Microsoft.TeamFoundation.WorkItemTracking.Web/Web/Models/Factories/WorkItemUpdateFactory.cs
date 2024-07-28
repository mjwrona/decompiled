// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.WorkItemUpdateFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.WorkItem;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories
{
  internal static class WorkItemUpdateFactory
  {
    public static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> Create(
      WorkItemUpdateCreateParams workItemUpdateCreateParams)
    {
      WorkItemTrackingRequestContext witRequestContext = workItemUpdateCreateParams.WitRequestContext;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem = workItemUpdateCreateParams.WorkItem;
      IEnumerable<WorkItemRevision> source = workItem.Revisions.Concat<WorkItemRevision>((IEnumerable<WorkItemRevision>) new WorkItemRevision[1]
      {
        (WorkItemRevision) workItem
      });
      Guid projectId = workItem.GetProjectGuid(witRequestContext.RequestContext);
      IEnumerable<string> first = source.Select<WorkItemRevision, string>((Func<WorkItemRevision, string>) (revision => revision.ModifiedBy));
      IEnumerable<string> second1 = source.SelectMany<WorkItemRevision, string>((Func<WorkItemRevision, IEnumerable<string>>) (x => x.AllLinks.Select<WorkItemLinkInfo, string>((Func<WorkItemLinkInfo, string>) (r => r.RevisedBy))));
      IEnumerable<string> second2 = source.SelectMany<WorkItemRevision, string>((Func<WorkItemRevision, IEnumerable<string>>) (x => x.AllLinks.Select<WorkItemLinkInfo, string>((Func<WorkItemLinkInfo, string>) (r => r.AuthorizedBy))));
      IDictionary<string, IdentityReference> allIdentityReferencesByDisplayName = IdentityReferenceBuilder.CreateFromWitIdentityNames(witRequestContext, first.Union<string>(second1).Union<string>(second2), true);
      Dictionary<DateTime, IDictionary<int, WorkItemResourceLinkInfo>> resourceLinksByRevisedDate = new Dictionary<DateTime, IDictionary<int, WorkItemResourceLinkInfo>>();
      WorkItemRevision previousRevision = (WorkItemRevision) null;
      int updateId = 1;
      IReadOnlyList<WorkItemRevision> revisions = workItem.Revisions;
      WorkItemRevision[] second3 = new WorkItemRevision[1]
      {
        (WorkItemRevision) workItem
      };
      foreach (WorkItemRevision revision in revisions.Concat<WorkItemRevision>((IEnumerable<WorkItemRevision>) second3))
      {
        WorkItemRelationUpdates resourceLinkUpdates = WorkItemUpdateFactory.GetResourceLinkUpdates(witRequestContext, revision, (IDictionary<DateTime, IDictionary<int, WorkItemResourceLinkInfo>>) resourceLinksByRevisedDate, workItemUpdateCreateParams.ReturnProjectScopedUrl);
        IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> workItemLinkUpdates = WorkItemUpdateFactory.GetWorkItemLinkUpdates(witRequestContext, revision, allIdentityReferencesByDisplayName, previousRevision, workItemUpdateCreateParams.ReturnProjectScopedUrl);
        WorkItemUpdateFactory.GetFieldsUpdateParams methodParams = new WorkItemUpdateFactory.GetFieldsUpdateParams()
        {
          witRequestContext = witRequestContext,
          revision = revision,
          previousRevision = previousRevision,
          returnIdentityRef = workItemUpdateCreateParams.ReturnIdentityRef,
          returnAuthorizedAs = workItemUpdateCreateParams.ReturnAuthorizedAs
        };
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate workItemUpdate1 = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate((ISecuredObject) workItem);
        workItemUpdate1.Id = updateId;
        workItemUpdate1.WorkItemId = workItem.Id;
        workItemUpdate1.Rev = revision.Revision;
        workItemUpdate1.Url = workItemUpdateCreateParams.IncludeUrls ? WitUrlHelper.GetWorkItemUpdatesUrl(witRequestContext, workItem.Id, new long?((long) updateId), new Guid?(projectId), workItemUpdateCreateParams.ReturnProjectScopedUrl) : (string) null;
        workItemUpdate1.RevisedBy = allIdentityReferencesByDisplayName[revision.ModifiedBy];
        workItemUpdate1.RevisedDate = revision.RevisedDate;
        workItemUpdate1.Fields = WorkItemUpdateFactory.GetFieldUpdates(methodParams);
        int skip;
        workItemUpdate1.Relations = WorkItemUpdateFactory.MergeRevisionRelationUpdates(revision, resourceLinkUpdates, workItemLinkUpdates, out skip);
        workItemUpdate1.Links = workItemUpdateCreateParams.IncludeLinks ? WorkItemUpdateFactory.GetReferenceLinks(witRequestContext, revision, (long) updateId, new Guid?(projectId), workItemUpdateCreateParams.ReturnProjectScopedUrl) : (ReferenceLinks) null;
        yield return workItemUpdate1;
        ++updateId;
        foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate workItemUpdate2 in workItemLinkUpdates.Skip<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>(skip))
        {
          workItemUpdate2.Id = updateId;
          workItemUpdate2.WorkItemId = workItem.Id;
          workItemUpdate2.Url = workItemUpdateCreateParams.IncludeUrls ? WitUrlHelper.GetWorkItemUpdatesUrl(witRequestContext, workItem.Id, new long?((long) updateId), new Guid?(projectId), workItemUpdateCreateParams.ReturnProjectScopedUrl) : (string) null;
          workItemUpdate2.Links = workItemUpdateCreateParams.IncludeLinks ? WorkItemUpdateFactory.GetReferenceLinks(witRequestContext, revision, (long) updateId, new Guid?(projectId), workItemUpdateCreateParams.ReturnProjectScopedUrl) : (ReferenceLinks) null;
          yield return workItemUpdate2;
          ++updateId;
        }
        previousRevision = revision;
        workItemLinkUpdates = (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>) null;
      }
    }

    public static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> Create(
      WorkItemTrackingRequestContext witRequestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem,
      DateTime? revisionsSince,
      bool includeLinks,
      int revisionNumber,
      bool returnIdentityRef = false,
      bool returnProjectScopedUrl = true)
    {
      WorkItemUpdatePayloadParams updatePayloadParams = new WorkItemUpdatePayloadParams();
      updatePayloadParams.WitRequestContext = witRequestContext;
      updatePayloadParams.WorkItem = workItem;
      updatePayloadParams.IncludeLinks = includeLinks;
      updatePayloadParams.ReturnIdentityRef = returnIdentityRef;
      updatePayloadParams.ReturnProjectScopedUrl = returnProjectScopedUrl;
      updatePayloadParams.RevisionNumber = revisionNumber;
      updatePayloadParams.ApiResourceVersion = new ApiResourceVersion(5.0);
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> updates = WorkItemUpdateFactory.Create((WorkItemUpdateCreateParams) updatePayloadParams).Where<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate, bool>) (u => u.Rev == revisionNumber));
      return revisionsSince.HasValue ? WorkItemUpdateFactory.FilterByUpdateTime(updates, (Func<DateTime, bool>) (updateDate =>
      {
        DateTime dateTime = updateDate;
        DateTime? nullable = revisionsSince;
        return nullable.HasValue && dateTime >= nullable.GetValueOrDefault();
      })) : updates;
    }

    public static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> FilterByUpdateTime(
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> updates,
      Func<DateTime, bool> updateTimePredicate)
    {
      Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate, DateTime> getUpdateTime = (Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate, DateTime>) (update =>
      {
        WorkItemFieldUpdate workItemFieldUpdate = (WorkItemFieldUpdate) null;
        update.Fields?.TryGetValue("System.AuthorizedDate", out workItemFieldUpdate);
        return workItemFieldUpdate == null || !(workItemFieldUpdate.NewValue is DateTime) ? update.RevisedDate : (DateTime) workItemFieldUpdate.NewValue;
      });
      return updates.Where<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate, bool>) (u => updateTimePredicate(getUpdateTime(u))));
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate CreateV2(
      WorkItemUpdatePayloadParams workItemUpdateCreateParams)
    {
      WorkItemTrackingRequestContext witRequestContext = workItemUpdateCreateParams.WitRequestContext;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem = workItemUpdateCreateParams.WorkItem;
      Guid projectGuid = workItem.GetProjectGuid(witRequestContext.RequestContext);
      IReadOnlyList<WorkItemRevision> revisions = workItem.Revisions;
      WorkItemRevision previousRevision = revisions != null ? revisions.LastOrDefault<WorkItemRevision>() : (WorkItemRevision) null;
      Dictionary<string, IdentityReference> identityNames = WorkItemUpdateFactory.GetIdentityNames(witRequestContext, (WorkItemRevision) workItem, previousRevision);
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> workItemLinkUpdates = WorkItemUpdateFactory.GetWorkItemLinkUpdates(witRequestContext, (WorkItemRevision) workItem, (IDictionary<string, IdentityReference>) identityNames, previousRevision, workItemUpdateCreateParams.ReturnProjectScopedUrl);
      int count = 0;
      int updateId = workItemUpdateCreateParams.UpdateId;
      if (workItemLinkUpdates != null && workItemLinkUpdates.Any<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>() && workItemLinkUpdates.First<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>().RevisedDate == workItem.AuthorizedDate)
        count = 1;
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate workItemLinkUpdate = workItemLinkUpdates.Skip<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>(count).LastOrDefault<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>();
      return workItemLinkUpdate != null ? WorkItemUpdateFactory.PrepareWorkItemLinksUpdateV2(witRequestContext, workItemLinkUpdate, workItem, updateId, projectGuid, (WorkItemUpdateCreateParams) workItemUpdateCreateParams) : WorkItemUpdateFactory.PrepareWorkItemUpdateV2(witRequestContext, (WorkItemRevision) workItem, previousRevision, updateId, projectGuid, (WorkItemUpdateCreateParams) workItemUpdateCreateParams, workItemLinkUpdates, identityNames);
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate PrepareWorkItemLinksUpdateV2(
      WorkItemTrackingRequestContext witRequestContext,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate workItemLinkUpdate,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem,
      int updateId,
      Guid projectId,
      WorkItemUpdateCreateParams workItemUpdateCreateParams)
    {
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate workItemUpdate = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate();
      workItemUpdate.Id = updateId;
      workItemUpdate.WorkItemId = workItem.Id;
      workItemUpdate.Rev = workItemLinkUpdate.Rev;
      workItemUpdate.RevisedBy = workItemLinkUpdate.RevisedBy;
      workItemUpdate.RevisedDate = workItemLinkUpdate.RevisedDate;
      workItemUpdate.Relations = workItemLinkUpdate.Relations;
      workItemUpdate.Url = workItemUpdateCreateParams.IncludeUrls ? WitUrlHelper.GetWorkItemUpdatesUrl(witRequestContext, workItem.Id, new long?((long) updateId), new Guid?(projectId), workItemUpdateCreateParams.ReturnProjectScopedUrl, workItemUpdateCreateParams.ApiResourceVersion, false, "sh") : (string) null;
      workItemUpdate.Links = workItemUpdateCreateParams.IncludeLinks ? WorkItemUpdateFactory.GetReferenceLinks(witRequestContext, (WorkItemRevision) workItem, (long) updateId, new Guid?(projectId), workItemUpdateCreateParams.ReturnProjectScopedUrl, workItemUpdateCreateParams.ApiResourceVersion) : (ReferenceLinks) null;
      return workItemUpdate;
    }

    private static ReferenceLinks GetReferenceLinks(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemRevision workItem,
      long updateNumber,
      Guid? projectId = null,
      bool returnProjectScopedUrl = true,
      ApiResourceVersion apiResourceVersion = null)
    {
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", WitUrlHelper.GetWorkItemUpdatesUrl(witRequestContext, workItem.Id, new long?(updateNumber), projectId, returnProjectScopedUrl, apiResourceVersion, false, "sh"), (ISecuredObject) workItem);
      WorkItemTrackingRequestContext witRequestContext1 = witRequestContext;
      int id = workItem.Id;
      Guid? nullable = projectId;
      bool flag = returnProjectScopedUrl;
      long? updateNumber1 = new long?();
      Guid? project = nullable;
      int num = flag ? 1 : 0;
      referenceLinks.AddLink("workItemUpdates", WitUrlHelper.GetWorkItemUpdatesUrl(witRequestContext1, id, updateNumber1, project, num != 0), (ISecuredObject) workItem);
      referenceLinks.AddLink("parent", WitUrlHelper.GetWorkItemUrl(witRequestContext.RequestContext, workItem.Id, project: projectId, generateProjectScopedUrl: returnProjectScopedUrl), (ISecuredObject) workItem);
      return referenceLinks;
    }

    private static IDictionary<string, WorkItemFieldUpdate> GetFieldUpdates(
      WorkItemUpdateFactory.GetFieldsUpdateParams methodParams)
    {
      WorkItemTrackingRequestContext witRequestContext = methodParams.witRequestContext;
      WorkItemRevision revision = methodParams.revision;
      WorkItemRevision previousRevision = methodParams.previousRevision;
      IEnumerable<KeyValuePair<FieldEntry, object>> source = revision.GetAllFieldValuesByFieldEntry(witRequestContext, true);
      if (previousRevision != null)
        source = source.Where<KeyValuePair<FieldEntry, object>>(new Func<KeyValuePair<FieldEntry, object>, bool>(FilterValues));
      Dictionary<string, WorkItemFieldUpdate> dictionary = source.ToDictionary<KeyValuePair<FieldEntry, object>, string, WorkItemFieldUpdate>((Func<KeyValuePair<FieldEntry, object>, string>) (kvp => kvp.Key.ReferenceName), (Func<KeyValuePair<FieldEntry, object>, WorkItemFieldUpdate>) (kvp =>
      {
        WorkItemFieldUpdate fieldUpdates = new WorkItemFieldUpdate((ISecuredObject) revision)
        {
          OldValue = previousRevision != null ? previousRevision.GetFieldValue(witRequestContext, kvp.Key.FieldId) : (object) null,
          NewValue = kvp.Value
        };
        if (methodParams.returnIdentityRef && kvp.Key.IsIdentity)
        {
          fieldUpdates.OldValue = (object) WorkItemIdentityHelper.GetIdentityRef(witRequestContext.RequestContext, fieldUpdates.OldValue, (ISecuredObject) previousRevision);
          fieldUpdates.NewValue = (object) WorkItemIdentityHelper.GetIdentityRef(witRequestContext.RequestContext, fieldUpdates.NewValue, (ISecuredObject) revision);
        }
        return fieldUpdates;
      }));
      return dictionary.Any<KeyValuePair<string, WorkItemFieldUpdate>>() ? (IDictionary<string, WorkItemFieldUpdate>) dictionary : (IDictionary<string, WorkItemFieldUpdate>) null;

      bool FilterValues(KeyValuePair<FieldEntry, object> kvp)
      {
        if (kvp.Key.ReferenceName.Equals("System.History", StringComparison.OrdinalIgnoreCase))
          return true;
        return methodParams.returnAuthorizedAs && kvp.Key.ReferenceName.Equals("System.AuthorizedAs", StringComparison.OrdinalIgnoreCase) ? !object.Equals((object) revision.ModifiedBy, kvp.Value) : !object.Equals(previousRevision.GetFieldValue(witRequestContext, kvp.Key.FieldId), kvp.Value);
      }
    }

    private static WorkItemRelationUpdates GetResourceLinkUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemRevision revision,
      IDictionary<DateTime, IDictionary<int, WorkItemResourceLinkInfo>> resourceLinksByRevisedDate,
      bool returnProjectScopedUrl = true)
    {
      List<WorkItemRelation> source1 = new List<WorkItemRelation>();
      List<WorkItemRelation> source2 = new List<WorkItemRelation>();
      List<WorkItemRelation> source3 = new List<WorkItemRelation>();
      Guid projectGuid = revision.GetProjectGuid(witRequestContext);
      IDictionary<int, WorkItemResourceLinkInfo> dictionary1;
      if (!resourceLinksByRevisedDate.TryGetValue(revision.AuthorizedDate, out dictionary1))
      {
        foreach (WorkItemResourceLinkInfo resourceLink in revision.ResourceLinks)
        {
          IDictionary<int, WorkItemResourceLinkInfo> dictionary2;
          if (!resourceLinksByRevisedDate.TryGetValue(resourceLink.RevisedDate, out dictionary2))
          {
            dictionary2 = (IDictionary<int, WorkItemResourceLinkInfo>) new Dictionary<int, WorkItemResourceLinkInfo>();
            resourceLinksByRevisedDate.Add(resourceLink.RevisedDate, dictionary2);
          }
          if (!dictionary2.ContainsKey(resourceLink.ResourceId))
          {
            dictionary2.Add(resourceLink.ResourceId, resourceLink);
            source1.Add(WorkItemRelationFactory.Create(witRequestContext.RequestContext, resourceLink, (ISecuredObject) revision, new Guid?(projectGuid), returnProjectScopedUrl));
          }
        }
      }
      else
      {
        foreach (WorkItemResourceLinkInfo resourceLink in revision.ResourceLinks)
        {
          if (dictionary1.ContainsKey(resourceLink.ResourceId))
          {
            source2.Add(WorkItemRelationFactory.Create(witRequestContext.RequestContext, resourceLink, (ISecuredObject) revision, new Guid?(projectGuid), returnProjectScopedUrl));
            dictionary1.Remove(resourceLink.ResourceId);
          }
          else
          {
            IDictionary<int, WorkItemResourceLinkInfo> dictionary3;
            if (!resourceLinksByRevisedDate.TryGetValue(resourceLink.RevisedDate, out dictionary3))
            {
              dictionary3 = (IDictionary<int, WorkItemResourceLinkInfo>) new Dictionary<int, WorkItemResourceLinkInfo>();
              resourceLinksByRevisedDate.Add(resourceLink.RevisedDate, dictionary3);
            }
            if (!dictionary3.ContainsKey(resourceLink.ResourceId))
            {
              dictionary3.Add(resourceLink.ResourceId, resourceLink);
              source1.Add(WorkItemRelationFactory.Create(witRequestContext.RequestContext, resourceLink, (ISecuredObject) revision, new Guid?(projectGuid), returnProjectScopedUrl));
            }
          }
        }
        foreach (WorkItemResourceLinkInfo linkInfo in (IEnumerable<WorkItemResourceLinkInfo>) dictionary1.Values)
          source3.Add(WorkItemRelationFactory.Create(witRequestContext.RequestContext, linkInfo, (ISecuredObject) revision, new Guid?(projectGuid), returnProjectScopedUrl));
      }
      bool flag1 = source1.Any<WorkItemRelation>();
      bool flag2 = source3.Any<WorkItemRelation>();
      bool flag3 = source2.Any<WorkItemRelation>();
      if (!(flag1 | flag2 | flag3))
        return (WorkItemRelationUpdates) null;
      return new WorkItemRelationUpdates()
      {
        Added = flag1 ? (IEnumerable<WorkItemRelation>) source1 : (IEnumerable<WorkItemRelation>) null,
        Removed = flag2 ? (IEnumerable<WorkItemRelation>) source3 : (IEnumerable<WorkItemRelation>) null,
        Updated = flag3 ? (IEnumerable<WorkItemRelation>) source2 : (IEnumerable<WorkItemRelation>) null
      };
    }

    private static WorkItemRelationUpdates GetResourceLinkUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemRevision currentRevision,
      WorkItemRevision previousRevision,
      bool returnProjectScopedUrl = true)
    {
      List<WorkItemRelation> addedLinks = new List<WorkItemRelation>();
      List<WorkItemRelation> removedLinks = new List<WorkItemRelation>();
      Guid projectGuid = currentRevision.GetProjectGuid(witRequestContext);
      List<WorkItemResourceLinkInfo> resourceLinkInfoList = previousRevision == null ? currentRevision.ResourceLinks : currentRevision.ResourceLinks.Except<WorkItemResourceLinkInfo>((IEnumerable<WorkItemResourceLinkInfo>) previousRevision.ResourceLinks).ToList<WorkItemResourceLinkInfo>();
      IEnumerable<WorkItemResourceLinkInfo> collection = previousRevision == null ? Enumerable.Empty<WorkItemResourceLinkInfo>() : (IEnumerable<WorkItemResourceLinkInfo>) previousRevision.ResourceLinks.Except<WorkItemResourceLinkInfo>((IEnumerable<WorkItemResourceLinkInfo>) currentRevision.ResourceLinks).ToList<WorkItemResourceLinkInfo>();
      resourceLinkInfoList.ForEach((Action<WorkItemResourceLinkInfo>) (x => addedLinks.Add(WorkItemRelationFactory.Create(witRequestContext.RequestContext, x, (ISecuredObject) currentRevision, new Guid?(projectGuid), returnProjectScopedUrl))));
      Action<WorkItemResourceLinkInfo> action = (Action<WorkItemResourceLinkInfo>) (x => removedLinks.Add(WorkItemRelationFactory.Create(witRequestContext.RequestContext, x, (ISecuredObject) currentRevision, new Guid?(projectGuid), returnProjectScopedUrl)));
      collection.ForEach<WorkItemResourceLinkInfo>(action);
      bool flag1 = addedLinks.Count > 0;
      bool flag2 = removedLinks.Count > 0;
      if (!(flag1 | flag2))
        return (WorkItemRelationUpdates) null;
      return new WorkItemRelationUpdates()
      {
        Added = flag1 ? (IEnumerable<WorkItemRelation>) addedLinks : (IEnumerable<WorkItemRelation>) null,
        Removed = flag2 ? (IEnumerable<WorkItemRelation>) removedLinks : (IEnumerable<WorkItemRelation>) null
      };
    }

    private static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> GetWorkItemLinkUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemRevision revision,
      IDictionary<string, IdentityReference> allIdentityReferencesByDisplayName,
      WorkItemRevision previousRevision = null,
      bool returnProjectScopedUrl = true)
    {
      IEnumerable<Tuple<DateTime, WorkItemLinkInfo>> first = revision.AllLinks.Where<WorkItemLinkInfo>((Func<WorkItemLinkInfo, bool>) (link => link.AuthorizedDate >= revision.AuthorizedDate)).Select<WorkItemLinkInfo, Tuple<DateTime, WorkItemLinkInfo>>((Func<WorkItemLinkInfo, Tuple<DateTime, WorkItemLinkInfo>>) (link => new Tuple<DateTime, WorkItemLinkInfo>(link.AuthorizedDate, link)));
      IEnumerable<WorkItemLinkInfo> workItemLinkInfos = revision.AllLinks == null ? Enumerable.Empty<WorkItemLinkInfo>() : (IEnumerable<WorkItemLinkInfo>) revision.AllLinks;
      if (previousRevision != null && previousRevision.AllLinks.Any<WorkItemLinkInfo>())
        workItemLinkInfos = workItemLinkInfos.Union<WorkItemLinkInfo>((IEnumerable<WorkItemLinkInfo>) previousRevision.AllLinks);
      IEnumerable<Tuple<DateTime, WorkItemLinkInfo>> second = workItemLinkInfos.Where<WorkItemLinkInfo>((Func<WorkItemLinkInfo, bool>) (link => link.RevisedDate >= revision.AuthorizedDate && link.RevisedDate < revision.RevisedDate)).SelectMany<WorkItemLinkInfo, Tuple<DateTime, WorkItemLinkInfo>>((Func<WorkItemLinkInfo, IEnumerable<Tuple<DateTime, WorkItemLinkInfo>>>) (link => !(link.AuthorizedDate > revision.AuthorizedDate) ? (IEnumerable<Tuple<DateTime, WorkItemLinkInfo>>) new Tuple<DateTime, WorkItemLinkInfo>[1]
      {
        new Tuple<DateTime, WorkItemLinkInfo>(link.RevisedDate, link)
      } : (IEnumerable<Tuple<DateTime, WorkItemLinkInfo>>) new Tuple<DateTime, WorkItemLinkInfo>[2]
      {
        new Tuple<DateTime, WorkItemLinkInfo>(link.AuthorizedDate, link),
        new Tuple<DateTime, WorkItemLinkInfo>(link.RevisedDate, link)
      }));
      foreach (IGrouping<DateTime, Tuple<DateTime, WorkItemLinkInfo>> source1 in (IEnumerable<IGrouping<DateTime, Tuple<DateTime, WorkItemLinkInfo>>>) first.Union<Tuple<DateTime, WorkItemLinkInfo>>(second).GroupBy<Tuple<DateTime, WorkItemLinkInfo>, DateTime>((Func<Tuple<DateTime, WorkItemLinkInfo>, DateTime>) (tuple => tuple.Item1)).OrderBy<IGrouping<DateTime, Tuple<DateTime, WorkItemLinkInfo>>, DateTime>((Func<IGrouping<DateTime, Tuple<DateTime, WorkItemLinkInfo>>, DateTime>) (group => group.Key)))
      {
        List<WorkItemRelation> source2 = new List<WorkItemRelation>();
        List<WorkItemRelation> source3 = new List<WorkItemRelation>();
        List<WorkItemRelation> source4 = new List<WorkItemRelation>();
        DateTime key = source1.Key;
        IdentityReference identityReference1 = (IdentityReference) null;
        IdentityReference identityReference2 = (IdentityReference) null;
        foreach (Tuple<DateTime, WorkItemLinkInfo> tuple in (IEnumerable<Tuple<DateTime, WorkItemLinkInfo>>) source1.OrderBy<Tuple<DateTime, WorkItemLinkInfo>, DateTime>((Func<Tuple<DateTime, WorkItemLinkInfo>, DateTime>) (tuple => tuple.Item2.AuthorizedDate)))
        {
          Guid? projectId = new Guid?(tuple.Item2.TargetProjectId != Guid.Empty ? tuple.Item2.TargetProjectId : revision.GetProjectGuid(witRequestContext));
          if (tuple.Item1 == tuple.Item2.AuthorizedDate)
          {
            source2.Add(WorkItemRelationFactory.Create(witRequestContext.RequestContext, tuple.Item2, (ISecuredObject) revision, projectId, returnProjectScopedUrl));
            allIdentityReferencesByDisplayName.TryGetValue(tuple.Item2.AuthorizedBy, out identityReference1);
          }
          else if (tuple.Item1 == tuple.Item2.RevisedDate)
          {
            source4.Add(WorkItemRelationFactory.Create(witRequestContext.RequestContext, tuple.Item2, (ISecuredObject) revision, projectId, returnProjectScopedUrl));
            allIdentityReferencesByDisplayName.TryGetValue(tuple.Item2.RevisedBy, out identityReference2);
          }
        }
        bool flag1 = source2.Any<WorkItemRelation>();
        bool flag2 = source4.Any<WorkItemRelation>();
        bool flag3 = source3.Any<WorkItemRelation>();
        if (flag1 | flag2 | flag3)
          yield return new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate()
          {
            Rev = revision.Revision,
            RevisedBy = flag1 | flag3 ? identityReference1 : identityReference2,
            RevisedDate = key,
            Relations = new WorkItemRelationUpdates()
            {
              Added = flag1 ? (IEnumerable<WorkItemRelation>) source2 : (IEnumerable<WorkItemRelation>) null,
              Removed = flag2 ? (IEnumerable<WorkItemRelation>) source4 : (IEnumerable<WorkItemRelation>) null,
              Updated = flag3 ? (IEnumerable<WorkItemRelation>) source3 : (IEnumerable<WorkItemRelation>) null
            }
          };
      }
    }

    private static WorkItemRelationUpdates MergeRevisionRelationUpdates(
      WorkItemRevision revision,
      WorkItemRelationUpdates resourceLinkUpdates,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> workItemLinkUpdates,
      out int skip)
    {
      skip = 0;
      if (workItemLinkUpdates != null && workItemLinkUpdates.Any<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>())
      {
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate workItemUpdate = workItemLinkUpdates.First<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>();
        if (workItemUpdate.RevisedDate == revision.AuthorizedDate)
        {
          skip = 1;
          if (resourceLinkUpdates == null)
          {
            resourceLinkUpdates = workItemUpdate.Relations;
          }
          else
          {
            resourceLinkUpdates.Added = WorkItemUpdateFactory.Merge<WorkItemRelation>(resourceLinkUpdates.Added, workItemUpdate.Relations.Added);
            resourceLinkUpdates.Removed = WorkItemUpdateFactory.Merge<WorkItemRelation>(resourceLinkUpdates.Removed, workItemUpdate.Relations.Removed);
          }
        }
      }
      return resourceLinkUpdates;
    }

    private static IEnumerable<T> Merge<T>(IEnumerable<T> x, IEnumerable<T> y)
    {
      if (x != null)
      {
        foreach (T obj in x)
          yield return obj;
      }
      if (y != null)
      {
        foreach (T obj in y)
          yield return obj;
      }
    }

    private static Dictionary<string, IdentityReference> GetIdentityNames(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemRevision revision1,
      WorkItemRevision previousRevision)
    {
      WorkItemRevision[] source;
      if (previousRevision != null)
        source = new WorkItemRevision[2]
        {
          revision1,
          previousRevision
        };
      else
        source = new WorkItemRevision[1]{ revision1 };
      IEnumerable<string> first = ((IEnumerable<WorkItemRevision>) source).Select<WorkItemRevision, string>((Func<WorkItemRevision, string>) (revision2 => revision2.ModifiedBy));
      IEnumerable<string> second1 = ((IEnumerable<WorkItemRevision>) source).SelectMany<WorkItemRevision, string>((Func<WorkItemRevision, IEnumerable<string>>) (x => x.AllLinks.Select<WorkItemLinkInfo, string>((Func<WorkItemLinkInfo, string>) (r => r.RevisedBy))));
      IEnumerable<string> second2 = ((IEnumerable<WorkItemRevision>) source).SelectMany<WorkItemRevision, string>((Func<WorkItemRevision, IEnumerable<string>>) (x => x.AllLinks.Select<WorkItemLinkInfo, string>((Func<WorkItemLinkInfo, string>) (r => r.AuthorizedBy))));
      IEnumerable<string> identityNames = first.Union<string>(second1).Union<string>(second2).AsEnumerable<string>();
      return (Dictionary<string, IdentityReference>) IdentityReferenceBuilder.CreateFromWitIdentityNames(witRequestContext, identityNames, true);
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate PrepareWorkItemUpdate(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemRevision revision,
      WorkItemRevision previousRevision,
      int updateId,
      Guid projectId,
      WorkItemUpdateCreateParams workItemUpdatePayloadParams,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> workItemLinkUpdates,
      Dictionary<string, IdentityReference> allIdentityReferencesByDisplayName,
      out int skip)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem = workItemUpdatePayloadParams.WorkItem;
      WorkItemRelationUpdates resourceLinkUpdates = WorkItemUpdateFactory.GetResourceLinkUpdates(witRequestContext, revision, previousRevision, workItemUpdatePayloadParams.ReturnProjectScopedUrl);
      WorkItemUpdateFactory.GetFieldsUpdateParams methodParams = new WorkItemUpdateFactory.GetFieldsUpdateParams()
      {
        witRequestContext = witRequestContext,
        revision = revision,
        previousRevision = previousRevision,
        returnIdentityRef = workItemUpdatePayloadParams.ReturnIdentityRef,
        returnAuthorizedAs = workItemUpdatePayloadParams.ReturnAuthorizedAs
      };
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate workItemUpdate = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate((ISecuredObject) workItem);
      workItemUpdate.Id = updateId;
      workItemUpdate.WorkItemId = workItem.Id;
      workItemUpdate.Rev = revision.Revision;
      workItemUpdate.Url = workItemUpdatePayloadParams.IncludeUrls ? WitUrlHelper.GetWorkItemUpdatesUrl(witRequestContext, workItem.Id, new long?((long) updateId), new Guid?(projectId), workItemUpdatePayloadParams.ReturnProjectScopedUrl) : (string) null;
      workItemUpdate.RevisedBy = allIdentityReferencesByDisplayName[revision.ModifiedBy];
      workItemUpdate.RevisedDate = revision.RevisedDate;
      workItemUpdate.Fields = WorkItemUpdateFactory.GetFieldUpdates(methodParams);
      workItemUpdate.Relations = WorkItemUpdateFactory.MergeRevisionRelationUpdates(revision, resourceLinkUpdates, workItemLinkUpdates, out skip);
      workItemUpdate.Links = workItemUpdatePayloadParams.IncludeLinks ? WorkItemUpdateFactory.GetReferenceLinks(witRequestContext, revision, (long) updateId, new Guid?(projectId), workItemUpdatePayloadParams.ReturnProjectScopedUrl) : (ReferenceLinks) null;
      return workItemUpdate;
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate PrepareWorkItemUpdateV2(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemRevision revision,
      WorkItemRevision previousRevision,
      int updateId,
      Guid projectId,
      WorkItemUpdateCreateParams workItemUpdateCreateParams,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> workItemLinkUpdates,
      Dictionary<string, IdentityReference> allIdentityReferencesByDisplayName)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem = workItemUpdateCreateParams.WorkItem;
      WorkItemRelationUpdates resourceLinkUpdates = WorkItemUpdateFactory.GetResourceLinkUpdates(witRequestContext, revision, previousRevision, workItemUpdateCreateParams.ReturnProjectScopedUrl);
      WorkItemUpdateFactory.GetFieldsUpdateParams methodParams = new WorkItemUpdateFactory.GetFieldsUpdateParams()
      {
        witRequestContext = witRequestContext,
        revision = revision,
        previousRevision = previousRevision,
        returnIdentityRef = workItemUpdateCreateParams.ReturnIdentityRef,
        returnAuthorizedAs = workItemUpdateCreateParams.ReturnAuthorizedAs
      };
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate workItemUpdate = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate((ISecuredObject) workItem);
      workItemUpdate.Id = updateId;
      workItemUpdate.WorkItemId = workItem.Id;
      workItemUpdate.Rev = revision.Revision;
      workItemUpdate.Url = workItemUpdateCreateParams.IncludeUrls ? WitUrlHelper.GetWorkItemUpdatesUrl(witRequestContext, workItem.Id, new long?((long) updateId), new Guid?(projectId), workItemUpdateCreateParams.ReturnProjectScopedUrl, workItemUpdateCreateParams.ApiResourceVersion, false, "sh") : (string) null;
      workItemUpdate.RevisedBy = allIdentityReferencesByDisplayName[revision.ModifiedBy];
      workItemUpdate.RevisedDate = revision.RevisedDate;
      workItemUpdate.Fields = WorkItemUpdateFactory.GetFieldUpdates(methodParams);
      workItemUpdate.Relations = WorkItemUpdateFactory.MergeRevisionRelationUpdates(revision, resourceLinkUpdates, workItemLinkUpdates, out int _);
      workItemUpdate.Links = workItemUpdateCreateParams.IncludeLinks ? WorkItemUpdateFactory.GetReferenceLinks(witRequestContext, revision, (long) updateId, new Guid?(projectId), workItemUpdateCreateParams.ReturnProjectScopedUrl, workItemUpdateCreateParams.ApiResourceVersion) : (ReferenceLinks) null;
      return workItemUpdate;
    }

    internal class GetFieldsUpdateParams
    {
      internal WorkItemTrackingRequestContext witRequestContext;
      internal WorkItemRevision revision;
      internal WorkItemRevision previousRevision;
      internal bool returnIdentityRef;
      internal bool returnAuthorizedAs;
    }
  }
}
