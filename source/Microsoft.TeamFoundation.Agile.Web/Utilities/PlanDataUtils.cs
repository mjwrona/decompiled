// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.PlanDataUtils
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public class PlanDataUtils
  {
    public static IEnumerable<LinkQueryResultEntry> GetWorkItemLinks(
      IVssRequestContext requestContext,
      string wiql)
    {
      if (string.IsNullOrEmpty(wiql))
        return Enumerable.Empty<LinkQueryResultEntry>();
      IWorkItemQueryService service = requestContext.GetService<IWorkItemQueryService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression = service.ConvertToQueryExpression(requestContext, wiql, skipWiqlTextLimitValidation: true);
      ProductBacklogQueryBuilder.SetLeftJoinHintOptimization(queryExpression);
      return service.ExecuteQuery(requestContext, queryExpression).WorkItemLinks;
    }

    public static IEnumerable<LinkQueryResultEntry> GetNewWorkItemLinks(
      IVssRequestContext requestContext,
      string wiql,
      Guid? projectId)
    {
      if (string.IsNullOrEmpty(wiql))
        return Enumerable.Empty<LinkQueryResultEntry>();
      IWorkItemQueryService service = requestContext.GetService<IWorkItemQueryService>();
      IVssRequestContext requestContext1 = requestContext;
      string wiql1 = wiql;
      Guid? nullable = projectId;
      Guid? queryId = new Guid?();
      Guid? filterProjectId = nullable;
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression = service.ConvertToQueryExpression(requestContext1, wiql1, skipWiqlTextLimitValidation: true, queryId: queryId, filterProjectId: filterProjectId);
      ProductBacklogQueryBuilder.SetLeftJoinHintOptimization(queryExpression);
      return service.ExecuteQuery(requestContext, queryExpression).WorkItemLinks;
    }

    public static IDictionary<int, int> CreateChildIdToParentIdMap(
      IEnumerable<LinkQueryResultEntry> allLinks,
      IEnumerable<int> workItemIds)
    {
      ArgumentUtility.CheckForNull<IEnumerable<LinkQueryResultEntry>>(allLinks, nameof (allLinks));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      Dictionary<int, LinkQueryResultEntry> idToLinkQueryMap = PlanDataUtils.CreateTargetIdToLinkQueryMap(allLinks);
      Dictionary<int, int> childIdToParentIdMap = new Dictionary<int, int>();
      foreach (int workItemId in workItemIds)
      {
        LinkQueryResultEntry queryResultEntry = (LinkQueryResultEntry) null;
        if (idToLinkQueryMap.TryGetValue(workItemId, out queryResultEntry) && queryResultEntry.SourceId != 0 && !childIdToParentIdMap.ContainsKey(queryResultEntry.TargetId))
          childIdToParentIdMap.Add(queryResultEntry.TargetId, queryResultEntry.SourceId);
      }
      return (IDictionary<int, int>) childIdToParentIdMap;
    }

    public static Dictionary<int, LinkQueryResultEntry> CreateTargetIdToLinkQueryMap(
      IEnumerable<LinkQueryResultEntry> allLinks)
    {
      Dictionary<int, LinkQueryResultEntry> idToLinkQueryMap = new Dictionary<int, LinkQueryResultEntry>();
      foreach (LinkQueryResultEntry allLink in allLinks)
      {
        if (!idToLinkQueryMap.ContainsKey(allLink.TargetId))
          idToLinkQueryMap[allLink.TargetId] = allLink;
        else if (allLink.SourceId == 0)
          idToLinkQueryMap[allLink.TargetId] = allLink;
      }
      return idToLinkQueryMap;
    }

    public static bool EvaluateTeamOwnership(
      string teamFieldValue,
      ITeamFieldValue[] teamFieldValues)
    {
      foreach (ITeamFieldValue teamFieldValue1 in teamFieldValues)
      {
        if (TFStringComparer.CssTreePathName.Equals(teamFieldValue, teamFieldValue1.Value) || teamFieldValue1.IncludeChildren && TFStringComparer.CssTreePathName.StartsWith(teamFieldValue, teamFieldValue1.Value + "\\"))
          return true;
      }
      return false;
    }

    public static bool EvaluateIterationOwnership(string value, string iteration) => TFStringComparer.CssTreePathName.Equals(value, iteration) || TFStringComparer.CssTreePathName.StartsWith(value, iteration + "\\");

    public static PlanWorkItemPayload PageWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<string> queryFields,
      bool returnIdentityRef)
    {
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      queryFields = queryFields.Distinct<string>();
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<int> ids = workItemIds;
      IEnumerable<string> fields = queryFields;
      DateTime? asOf = new DateTime?();
      int num = returnIdentityRef ? 1 : 0;
      GenericDataReader dataReader = service.PageWorkItems(requestContext1, ids, fields, asOf, WorkItemRetrievalMode.NonDeleted, returnIdentityRef: num != 0);
      return PlanDataUtils.CreatePlanWorkItemPayload(requestContext, dataReader, workItemIds);
    }

    private static PlanWorkItemPayload CreatePlanWorkItemPayload(
      IVssRequestContext requestContext,
      GenericDataReader dataReader,
      IEnumerable<int> orderedWorkItemIds = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<GenericDataReader>(dataReader, nameof (dataReader));
      int fieldCount = dataReader.FieldCount;
      string[] array1 = Enumerable.Range(0, fieldCount).Select<int, string>((System.Func<int, string>) (i => dataReader.GetName(i))).ToArray<string>();
      object[][] array2 = dataReader.Select<IDataRecord, object[]>((System.Func<IDataRecord, object[]>) (record =>
      {
        object[] objArray = new object[fieldCount];
        record.GetValues(objArray);
        return ((IEnumerable<object>) objArray).Select<object, object>((System.Func<object, object>) (value => value)).ToArray<object>();
      })).ToArray<object[]>();
      if (orderedWorkItemIds != null && orderedWorkItemIds.Any<int>())
      {
        List<int> orderedId = orderedWorkItemIds.ToList<int>();
        array2 = ((IEnumerable<object[]>) array2).OrderBy<object[], int>((System.Func<object[], int>) (x => orderedId.IndexOf((int) x[0]))).ToArray<object[]>();
      }
      return new PlanWorkItemPayload()
      {
        FieldReferenceNames = (IEnumerable<string>) array1,
        WorkItems = (IReadOnlyList<object[]>) array2
      };
    }
  }
}
