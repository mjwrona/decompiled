// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.BoardParentWIFilterHelper
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public class BoardParentWIFilterHelper : IBoardParentWIFilterHelper
  {
    public const int ChildWorkItemLimit = 3000;

    public ICollection<ParentChildWIMap> GetParentChildWIMap(
      IVssRequestContext requestContext,
      IAgileSettings teamAgileSettings,
      BacklogLevelConfiguration parentBacklogLevel,
      int[] workitemIds)
    {
      ArgumentUtility.CheckForNull<int[]>(workitemIds, nameof (workitemIds));
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(parentBacklogLevel, nameof (parentBacklogLevel));
      if (workitemIds.Length > 3000)
        throw new BoardParentChildWIMapLimitExceededException(3000);
      if (workitemIds.Length == 0)
        return (ICollection<ParentChildWIMap>) new List<ParentChildWIMap>();
      ICollection<ParentChildWIMap> parentChildWiMap = (ICollection<ParentChildWIMap>) new List<ParentChildWIMap>();
      WebAccessWorkItemService witService = this.GetWitService(requestContext);
      List<string> stringList = new List<string>()
      {
        CoreFieldReferenceNames.Id,
        CoreFieldReferenceNames.Title,
        CoreFieldReferenceNames.WorkItemType
      };
      BacklogContext parentBacklogContext = this.GetParentBacklogContext(teamAgileSettings, parentBacklogLevel);
      IEnumerable<LinkQueryResultEntry> allLinks = this.GetAllLinks(requestContext, parentBacklogContext, teamAgileSettings, workitemIds, (IEnumerable<string>) stringList);
      if (allLinks.Count<LinkQueryResultEntry>() > 0)
      {
        IEnumerable<int> ints = allLinks.Where<LinkQueryResultEntry>((System.Func<LinkQueryResultEntry, bool>) (link => link.SourceId != 0)).Select<LinkQueryResultEntry, int>((System.Func<LinkQueryResultEntry, int>) (link => link.SourceId)).Distinct<int>();
        IDataRecord[] array = witService.GetWorkItems(requestContext, ints, (IEnumerable<string>) stringList).ToArray<IDataRecord>();
        List<int> workItemIds = new List<int>();
        workItemIds.AddRange(ints);
        workItemIds.AddRange((IEnumerable<int>) ((IEnumerable<int>) workitemIds).ToList<int>());
        IDictionary<int, List<int>> dictionary = this.ConstructParentchildMap(allLinks, (IEnumerable<int>) workItemIds);
        foreach (IDataRecord dataRecord in (IEnumerable<IDataRecord>) array)
        {
          int key = (int) dataRecord[CoreFieldReferenceNames.Id];
          List<int> intList;
          if (dictionary.TryGetValue(key, out intList))
          {
            ArgumentUtility.CheckForNull<object>(dataRecord[CoreFieldReferenceNames.Title], "Work Item Title of ID:" + key.ToString());
            parentChildWiMap.Add(new ParentChildWIMap()
            {
              Id = key,
              Title = dataRecord[CoreFieldReferenceNames.Title].ToString(),
              WorkItemTypeName = dataRecord[CoreFieldReferenceNames.WorkItemType].ToString(),
              ChildWorkItemIds = (IList<int>) intList
            });
          }
        }
      }
      return parentChildWiMap;
    }

    internal virtual IEnumerable<LinkQueryResultEntry> GetAllLinks(
      IVssRequestContext requestContext,
      BacklogContext backlogContext,
      IAgileSettings teamAgileSettings,
      int[] workitemIds,
      IEnumerable<string> fields)
    {
      ArgumentUtility.CheckForNull<BacklogContext>(backlogContext, nameof (backlogContext));
      ArgumentUtility.CheckForNull<int[]>(workitemIds, nameof (workitemIds));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(fields, nameof (fields));
      string itemsAllTypesQuery = new ProductBacklogQueryBuilder(requestContext, teamAgileSettings, backlogContext, teamAgileSettings.TeamSettings.GetBacklogIterationNode(requestContext).GetPath(requestContext))
      {
        Fields = ((IEnumerable<string>) fields.ToArray<string>())
      }.GetParentItemsAllTypesQuery(workitemIds);
      IWorkItemQueryService service = requestContext.GetService<IWorkItemQueryService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression = service.ConvertToQueryExpression(requestContext, itemsAllTypesQuery, skipWiqlTextLimitValidation: true);
      ProductBacklogQueryBuilder.SetLeftJoinHintOptimization(queryExpression);
      return service.ExecuteQuery(requestContext, queryExpression).WorkItemLinks;
    }

    internal virtual IDictionary<int, List<int>> ConstructParentchildMap(
      IEnumerable<LinkQueryResultEntry> allLinks,
      IEnumerable<int> workItemIds)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) allLinks, nameof (allLinks));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItemIds, nameof (workItemIds));
      ICollection<Tuple<int, int>> hierarchyByIds = BoardHierarchyDataHelper.GetHierarchyByIds(allLinks, workItemIds);
      IDictionary<int, List<int>> dictionary = (IDictionary<int, List<int>>) new Dictionary<int, List<int>>();
      foreach (Tuple<int, int> tuple in (IEnumerable<Tuple<int, int>>) hierarchyByIds)
      {
        List<int> intList;
        if (!dictionary.TryGetValue(tuple.Item2, out intList))
        {
          intList = new List<int>();
          dictionary[tuple.Item2] = intList;
        }
        intList.Add(tuple.Item1);
      }
      return dictionary;
    }

    internal virtual WebAccessWorkItemService GetWitService(IVssRequestContext requestContext) => requestContext.GetService<WebAccessWorkItemService>();

    internal virtual BacklogContext GetParentBacklogContext(
      IAgileSettings teamAgileSettings,
      BacklogLevelConfiguration parentBacklogLevel)
    {
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(parentBacklogLevel, nameof (parentBacklogLevel));
      return new BacklogContext(teamAgileSettings.Team, parentBacklogLevel);
    }
  }
}
