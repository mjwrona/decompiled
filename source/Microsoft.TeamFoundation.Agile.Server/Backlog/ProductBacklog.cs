// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.Backlog.ProductBacklog
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server.Backlog
{
  public class ProductBacklog
  {
    private const int TreatItemsClosedBeforeInDayssAsUnownedDefault = 365;
    private const string c_ClosedItemsTimespanRegistryKey = "/Service/Agile/Settings/ClosedItemsAsUnownedDays";
    private const string BacklogSizeLimitCheckTimestampRegistryKeyTemplate = "/Service/Agile/Settings/BacklogSizeLimitCheckTimestamp/{0}/{1}";
    private const string BacklogWorkItemsPageLimitRegistryKey = "/Service/Agile/Settings/BacklogWorkItemsPageLimit";
    private const int BacklogSizeLimitCheckTimestampTtlInDays = 1;
    private int m_treatItemsClosedBeforeInDaysAsUnowned = 365;
    private IVssRequestContext m_requestContext;
    private IAgileSettings m_settings;
    private BacklogContext m_backlogContext;
    private IEnumerable<string> m_fieldReferenceNames;
    private WorkItemStateCategory[] m_stateCategories;
    private bool m_showCompletedChildItems;
    private bool? m_teamOwnsAllItems;
    private string m_drillDownQuery;
    private string m_backlogQuery;
    private string m_ownershipQuery;
    private string m_parentsQuery;
    private bool m_includePageData;
    private List<object[]> m_parentPageData;
    private OrderedWorkItemTreeResult m_drillDownQueryResult;

    protected int PageSize { get; set; }

    public ProductBacklog(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      BacklogContext backlogContext,
      IEnumerable<string> fieldReferenceNames,
      IEnumerable<WorkItemStateCategory> stateCategories,
      bool showCompletedChildItems = true)
      : this(requestContext, settings, backlogContext, fieldReferenceNames, stateCategories, false, showCompletedChildItems)
    {
    }

    public ProductBacklog(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      BacklogContext backlogContext,
      IEnumerable<string> fieldReferenceNames,
      IEnumerable<WorkItemStateCategory> stateCategories,
      bool includePageData,
      bool showCompletedChildItems = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForNull<BacklogContext>(backlogContext, nameof (backlogContext));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemStateCategory>>(stateCategories, nameof (stateCategories));
      this.m_requestContext = requestContext;
      this.m_settings = settings;
      this.m_backlogContext = backlogContext;
      this.m_fieldReferenceNames = fieldReferenceNames;
      this.m_stateCategories = stateCategories.ToArray<WorkItemStateCategory>();
      this.m_showCompletedChildItems = showCompletedChildItems;
      this.m_includePageData = includePageData;
      if (this.m_includePageData)
        this.m_parentPageData = new List<object[]>();
      this.ReadConfigurationFromRegistry();
      this.Initialize();
    }

    public int[] SourceIds { get; private set; }

    public int[] TargetIds { get; private set; }

    public int[] LinkIds { get; private set; }

    public int[] OwnedIds { get; private set; }

    public int[] ParentIds { get; private set; }

    public BacklogQueryResultType BacklogQueryResultType { get; private set; }

    public string BacklogWiql => this.m_backlogQuery;

    public IEnumerable<string> PageDataColumnReferenceNames => this.m_fieldReferenceNames.Concat<string>((IEnumerable<string>) this.GetRequiredColumns()).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);

    public IEnumerable<object[]> GeneratePayload() => this.m_backlogContext.IncludeParents ? (IEnumerable<object[]>) this.m_parentPageData : this.PageWorkItems(this.TargetIds.Length > this.PageSize ? this.m_drillDownQueryResult.GetItemsInBFSOrder(this.PageSize, (ISet<int>) null) : (IEnumerable<int>) this.TargetIds, this.PageDataColumnReferenceNames.ToArray<string>());

    internal virtual bool TeamOwnsAllItems
    {
      get
      {
        if (this.m_requestContext.IsFeatureEnabled("WebAccess.Agile.Backlog.Safeguard.SkipOwnershipQuery"))
          this.m_teamOwnsAllItems = new bool?(true);
        if (!this.m_teamOwnsAllItems.HasValue)
        {
          if (!this.m_settings.Process.IsTeamFieldAreaPath())
            return false;
          string projectName = this.m_settings.ProjectName;
          this.m_teamOwnsAllItems = new bool?(((IEnumerable<ITeamFieldValue>) this.m_settings.TeamSettings.TeamFieldConfig.TeamFieldValues).Any<ITeamFieldValue>((System.Func<ITeamFieldValue, bool>) (x => x.IncludeChildren && TFStringComparer.CssTreePathName.Equals(projectName, x.Value))));
        }
        return this.m_teamOwnsAllItems.Value;
      }
    }

    protected virtual void Initialize() => this.m_requestContext.TraceBlock(290201, 290202, 290203, "AgileService", "AgileService", "ProductBacklog.Initialize", (Action) (() =>
    {
      this.BuildQueries();
      this.m_drillDownQueryResult = this.ConstructDrillDownQueryResult();
      OrderedWorkItemResult orderedWorkItemResult;
      IEnumerable<int> ints;
      if (this.TeamOwnsAllItems)
      {
        orderedWorkItemResult = (OrderedWorkItemResult) this.m_drillDownQueryResult;
        ints = (IEnumerable<int>) this.m_drillDownQueryResult.Order.Keys;
      }
      else
      {
        orderedWorkItemResult = this.ConstructOwnershipQueryResult();
        ints = this.m_drillDownQueryResult.Order.Keys.Intersect<int>((IEnumerable<int>) orderedWorkItemResult.Order.Keys);
      }
      OrderedWorkItemTreeResult treeResult1 = this.m_drillDownQueryResult;
      WorkItemComparer workItemComparer;
      if (this.m_backlogContext.IncludeParents)
      {
        OrderedWorkItemTreeResult treeResult2 = this.ConstructShowParentsQueryResult(this.m_drillDownQueryResult);
        HashSet<int> nonLeafs = treeResult2.GetNonLeafs();
        Dictionary<int, PagedBacklogWorkItem> dictionary;
        if (this.m_includePageData)
        {
          HashSet<int> intSet = new HashSet<int>((IEnumerable<int>) treeResult2.Order.Keys);
          intSet.Remove(0);
          if (intSet.Count <= this.PageSize)
          {
            IEnumerable<int> itemsInBfsOrder = this.m_drillDownQueryResult.GetItemsInBFSOrder(this.PageSize - intSet.Count, (ISet<int>) intSet);
            dictionary = this.PageParentItems(intSet.Concat<int>(itemsInBfsOrder));
          }
          else
            dictionary = this.PageParentItems((IEnumerable<int>) intSet);
        }
        else
          dictionary = this.PageParentItems((IEnumerable<int>) nonLeafs);
        this.ParentIds = nonLeafs.ToArray<int>();
        IEnumerable<int> second = dictionary.Where<KeyValuePair<int, PagedBacklogWorkItem>>((System.Func<KeyValuePair<int, PagedBacklogWorkItem>, bool>) (x => x.Value.IsOwned)).Select<KeyValuePair<int, PagedBacklogWorkItem>, int>((System.Func<KeyValuePair<int, PagedBacklogWorkItem>, int>) (x => x.Key));
        ints = ints.Concat<int>(second).Distinct<int>();
        this.InjectUnparentedItems(treeResult2, dictionary);
        treeResult2.Merge(this.m_drillDownQueryResult);
        treeResult1 = treeResult2;
        workItemComparer = new WorkItemComparer((IDictionary<int, int>) orderedWorkItemResult.Order, (IDictionary<int, int>) this.m_drillDownQueryResult.Order, this.m_settings.TeamSettings.TeamFieldConfig, (IDictionary<int, PagedBacklogWorkItem>) dictionary, (ISet<int>) nonLeafs);
      }
      else
        workItemComparer = new WorkItemComparer((IDictionary<int, int>) orderedWorkItemResult.Order, (IDictionary<int, int>) this.m_drillDownQueryResult.Order);
      this.SortAndFlatten(treeResult1, (IComparer<int>) workItemComparer);
      this.OwnedIds = ints.ToArray<int>();
    }));

    internal virtual Dictionary<int, PagedBacklogWorkItem> PageParentItems(IEnumerable<int> ids) => this.m_requestContext.TraceBlock<Dictionary<int, PagedBacklogWorkItem>>(290204, 290205, 290206, "AgileService", "AgileService", "ProductBacklog.PageItems", (Func<Dictionary<int, PagedBacklogWorkItem>>) (() =>
    {
      Dictionary<int, PagedBacklogWorkItem> pagedItemCache = new Dictionary<int, PagedBacklogWorkItem>();
      if (!this.m_includePageData)
        this.FillPagedItemCache(ids, false, this.GetRequiredColumns(), ref pagedItemCache);
      else if (ids.Count<int>() > this.PageSize)
      {
        this.FillPagedItemCache(ids.Take<int>(this.PageSize), true, this.PageDataColumnReferenceNames.ToArray<string>(), ref pagedItemCache);
        this.FillPagedItemCache(ids.Skip<int>(this.PageSize), false, this.GetRequiredColumns(), ref pagedItemCache);
      }
      else
        this.FillPagedItemCache(ids, true, this.PageDataColumnReferenceNames.ToArray<string>(), ref pagedItemCache);
      return pagedItemCache;
    }));

    private string[] GetRequiredColumns() => ((IEnumerable<string>) QueryColumnHelper.GetRequiredColumns((IEnumerable<string>) new string[3]
    {
      this.m_settings.Process.TeamField.Name,
      this.m_settings.Process.OrderByField.Name,
      CoreFieldReferenceNames.IterationPath
    })).ToArray<string>();

    private void FillPagedItemCache(
      IEnumerable<int> ids,
      bool storePageData,
      string[] columns,
      ref Dictionary<int, PagedBacklogWorkItem> pagedItemCache)
    {
      Dictionary<string, int> columnIndices;
      IEnumerable<object[]> objArrays = this.PageWorkItems(ids, columns, out columnIndices);
      int index1 = columnIndices[CoreFieldReferenceNames.Id];
      int index2 = columnIndices[this.m_settings.Process.TeamField.Name];
      int index3 = columnIndices[this.m_settings.Process.OrderByField.Name];
      int index4 = columnIndices[CoreFieldReferenceNames.WorkItemType];
      foreach (object[] objArray in objArrays)
      {
        if (storePageData)
          this.m_parentPageData.Add(objArray);
        int key = (int) objArray[index1];
        object obj = objArray[index3];
        if (!(obj is double result) && (obj == null || !double.TryParse(obj.ToString(), out result)))
          result = double.MaxValue;
        string safeString = ProductBacklog.ToSafeString(objArray[index2]);
        bool ownership = OwnershipEvaluator.EvaluateOwnership(safeString, this.m_settings.TeamSettings.TeamFieldConfig);
        pagedItemCache[key] = new PagedBacklogWorkItem()
        {
          WorkItemType = ProductBacklog.ToSafeString(objArray[index4]),
          TeamFieldValue = safeString,
          OrderValue = result,
          IsOwned = ownership
        };
      }
    }

    private static string ToSafeString(object value) => value == null ? string.Empty : value.ToString();

    internal void InjectUnparentedItems(
      OrderedWorkItemTreeResult treeResult,
      Dictionary<int, PagedBacklogWorkItem> payloadCache)
    {
      this.m_requestContext.TraceBlock(290207, 290208, 290209, "AgileService", "AgileService", "ProductBacklog.InjectUnparentedItems", (Action) (() =>
      {
        int childLinkTypeId = this.GetChildLinkTypeId();
        if (!treeResult.Tree.ContainsKey(0))
          treeResult.Tree.Add(0, new List<int>());
        List<BacklogLevelConfiguration> source1 = new List<BacklogLevelConfiguration>();
        IReadOnlyCollection<BacklogLevelConfiguration> parentBacklogLevels = (IReadOnlyCollection<BacklogLevelConfiguration>) new List<BacklogLevelConfiguration>();
        if (this.m_settings.BacklogConfiguration.TryGetBacklogParents(this.m_backlogContext.CurrentLevelConfiguration, out parentBacklogLevels))
          source1.AddRange((IEnumerable<BacklogLevelConfiguration>) parentBacklogLevels);
        source1.Add(this.m_backlogContext.CurrentLevelConfiguration);
        int rank = this.m_settings.BacklogConfiguration.RequirementBacklog.Rank;
        BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
        if (this.m_settings.BacklogConfiguration.TryGetHighestVisiblePortfolioLevel(out backlogLevel))
          rank = backlogLevel.Rank;
        List<int> source2 = treeResult.Tree[0];
        int key1 = -1;
        foreach (BacklogLevelConfiguration levelConfiguration in (IEnumerable<BacklogLevelConfiguration>) source1.OrderBy<BacklogLevelConfiguration, int>((System.Func<BacklogLevelConfiguration, int>) (blc => blc.Rank)))
        {
          IReadOnlyCollection<string> levelTypes = levelConfiguration.WorkItemTypes;
          if (levelConfiguration.Rank >= rank && source2.All<int>((System.Func<int, bool>) (workItemId => workItemId < 0 || !payloadCache.ContainsKey(workItemId) || levelTypes.Contains<string>(payloadCache[workItemId].WorkItemType, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))))
            break;
          List<int> intList = new List<int>();
          for (int index = 0; index < source2.Count; ++index)
          {
            int key2 = source2[index];
            if (key2 < 0 || !payloadCache.ContainsKey(key2) || levelTypes.Contains<string>(payloadCache[key2].WorkItemType, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))
            {
              source2.RemoveAt(index);
              --index;
              intList.Add(key2);
              treeResult.LinkTypes[key2] = childLinkTypeId;
            }
          }
          if (intList.Count > 0)
          {
            source2.Add(key1);
            treeResult.Tree[key1] = intList;
            treeResult.LinkTypes.Add(key1, 0);
          }
          --key1;
        }
      }));
    }

    protected virtual int GetChildLinkTypeId() => this.m_requestContext.GetService<WebAccessWorkItemService>().GetLinkTypes(this.m_requestContext).FirstOrDefault<WorkItemLinkType>((System.Func<WorkItemLinkType, bool>) (x => x.ReferenceName == "System.LinkTypes.Hierarchy")).ForwardEnd.Id;

    internal void SortAndFlatten(OrderedWorkItemTreeResult treeResult, IComparer<int> comparer) => this.m_requestContext.TraceBlock(290210, 290211, 290212, "AgileService", "AgileService", "ProductBacklog.SortAndFlatten", (Action) (() =>
    {
      List<int> intList1 = new List<int>();
      List<int> intList2 = new List<int>();
      List<int> intList3 = new List<int>();
      Stack<Tuple<int, int>> s = new Stack<Tuple<int, int>>();
      Action<int, List<int>> action = (Action<int, List<int>>) ((parentWorkItemId, children) =>
      {
        children.Sort(comparer);
        for (int index = children.Count - 1; index >= 0; --index)
          s.Push(Tuple.Create<int, int>(parentWorkItemId, children[index]));
      });
      if (treeResult.Tree.ContainsKey(0))
        action(0, treeResult.Tree[0]);
      while (s.Count > 0)
      {
        Tuple<int, int> tuple = s.Pop();
        int num = tuple.Item1;
        int key = tuple.Item2;
        intList2.Add(num);
        intList1.Add(key);
        intList3.Add(treeResult.LinkTypes[key]);
        if (treeResult.Tree.ContainsKey(key))
        {
          List<int> intList4 = treeResult.Tree[key];
          action(key, intList4);
        }
      }
      this.SourceIds = intList2.ToArray();
      this.TargetIds = intList1.ToArray();
      this.LinkIds = intList3.ToArray();
    }));

    internal virtual void ReadConfigurationFromRegistry()
    {
      try
      {
        IVssRegistryService service = this.m_requestContext.GetService<IVssRegistryService>();
        this.m_treatItemsClosedBeforeInDaysAsUnowned = service.GetValue<int>(this.m_requestContext, (RegistryQuery) "/Service/Agile/Settings/ClosedItemsAsUnownedDays", true, 365);
        this.PageSize = service.GetValue<int>(this.m_requestContext, (RegistryQuery) "/Service/Agile/Settings/BacklogWorkItemsPageLimit", 200);
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(599999, "AgileService", "AgileService", ex);
      }
    }

    internal virtual ProductBacklogQueryBuilder CreateQueryBuilder() => new ProductBacklogQueryBuilder(this.m_requestContext, this.m_settings, this.m_backlogContext, this.m_settings.TeamSettings.GetBacklogIterationNode(this.m_requestContext).GetPath(this.m_requestContext))
    {
      Fields = this.m_fieldReferenceNames
    };

    internal virtual OrderedWorkItemTreeResult ConstructDrillDownQueryResult() => this.m_requestContext.TraceBlock<OrderedWorkItemTreeResult>(290213, 290214, 290215, "AgileService", "AgileService", "ProductBacklog.ConstructDrillDownQueryResult", (Func<OrderedWorkItemTreeResult>) (() =>
    {
      DateTime timestamp;
      QueryResult downQueryResults;
      if (!this.m_requestContext.IsFeatureEnabled("WebAccess.Agile.Backlog.OnWorkItemQuerySizeLimitException.DisableTrackingTimestamp") && this.TryGetBacklogSizeLimitCheckTimestampRegistryValue(out timestamp) && DateTime.UtcNow - timestamp < TimeSpan.FromDays(1.0))
      {
        this.m_requestContext.TraceAlways(290969, TraceLevel.Info, "AgileService", "AgileService", "Backlog size limit check timestamp is valid. Skipping default query.");
        downQueryResults = this.GetOneHopOrFlatDrillDownQueryResults();
      }
      else
      {
        try
        {
          downQueryResults = this.GetDefaultDrillDownQueryResults();
        }
        catch (WorkItemTrackingQueryResultSizeLimitExceededException ex)
        {
          if (!this.m_requestContext.IsFeatureEnabled("WebAccess.Agile.Backlog.OnWorkItemQuerySizeLimitException.DisableTrackingTimestamp"))
            this.m_requestContext.TraceException(290226, "AgileService", "AgileService", (Exception) ex);
          this.SetBacklogSizeLimitCheckTimestampRegistryValue();
          downQueryResults = this.GetOneHopOrFlatDrillDownQueryResults();
        }
      }
      return OrderedWorkItemTreeResult.CreateTopDown(downQueryResults);
    }));

    internal virtual QueryResult GetOneHopOrFlatDrillDownQueryResults()
    {
      try
      {
        return this.GetOneHopDrillDownQueryResults();
      }
      catch (WorkItemTrackingQueryResultSizeLimitExceededException ex)
      {
        this.m_requestContext.TraceException(290225, "AgileService", "AgileService", (Exception) ex);
        return this.GetFlatDrillDownQueryResults();
      }
    }

    internal virtual QueryResult GetDefaultDrillDownQueryResults()
    {
      QueryResult downQueryResults = this.RunQuery(this.m_drillDownQuery);
      this.BacklogQueryResultType = BacklogQueryResultType.Default;
      return downQueryResults;
    }

    internal virtual QueryResult GetFlatDrillDownQueryResults()
    {
      this.m_drillDownQuery = this.CreateQueryBuilder().GetQuery(this.m_stateCategories);
      QueryResult downQueryResults = this.RunQuery(this.m_drillDownQuery);
      this.BacklogQueryResultType = BacklogQueryResultType.Flat;
      return downQueryResults;
    }

    internal virtual QueryResult GetOneHopDrillDownQueryResults()
    {
      this.m_drillDownQuery = this.CreateQueryBuilder().GetTreeQuery(this.m_stateCategories, !this.TeamOwnsAllItems, true, this.m_showCompletedChildItems);
      QueryResult downQueryResults = this.RunQuery(this.m_drillDownQuery);
      this.BacklogQueryResultType = BacklogQueryResultType.OneHop;
      return downQueryResults;
    }

    internal virtual OrderedWorkItemResult ConstructOwnershipQueryResult() => this.m_requestContext.TraceBlock<OrderedWorkItemResult>(290216, 290217, 290218, "AgileService", "AgileService", "ProductBacklog.ConstructOwnershipQueryResult", (Func<OrderedWorkItemResult>) (() =>
    {
      try
      {
        return OrderedWorkItemResult.Create(this.RunQuery(this.m_ownershipQuery));
      }
      catch (WorkItemTrackingQueryResultSizeLimitExceededException ex)
      {
        this.m_requestContext.TraceException(599999, "AgileService", "AgileService", (Exception) ex);
        this.m_teamOwnsAllItems = new bool?(true);
        return !this.m_requestContext.IsFeatureEnabled("WebAccess.Agile.Backlog.OwnershipQueryOverLimit.DisableRerunDrilldownQuery") ? (OrderedWorkItemResult) this.m_requestContext.TraceBlock<OrderedWorkItemTreeResult>(290970, 290971, 290972, "AgileService", "AgileService", "ProductBacklog.RerunDrillDownQuery", (Func<OrderedWorkItemTreeResult>) (() =>
        {
          this.m_drillDownQuery = this.CreateQueryBuilder().GetTreeQuery(this.m_stateCategories, !this.TeamOwnsAllItems, showCompletedChildItems: this.m_showCompletedChildItems);
          return this.ConstructDrillDownQueryResult();
        })) : (OrderedWorkItemResult) this.m_drillDownQueryResult;
      }
    }));

    internal virtual OrderedWorkItemTreeResult ConstructShowParentsQueryResult(
      OrderedWorkItemTreeResult drillDownQueryResult)
    {
      return this.m_requestContext.TraceBlock<OrderedWorkItemTreeResult>(290219, 290220, 290221, "AgileService", "AgileService", "ProductBacklog.ConstructShowParentsQueryResult", (Func<OrderedWorkItemTreeResult>) (() => OrderedWorkItemTreeResult.CreateBottomUp(this.RunQuery(this.m_parentsQuery), drillDownQueryResult.GetLeafs())));
    }

    internal virtual QueryResult RunQuery(string wiql)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression optimizedLinkQuery = BacklogQueryExecutionHelper.GetOptimizedLinkQuery(this.m_requestContext, wiql, true);
      return this.m_requestContext.GetService<IWorkItemQueryService>().ExecuteQuery(this.m_requestContext, optimizedLinkQuery);
    }

    internal virtual IEnumerable<object[]> PageWorkItems(
      IEnumerable<int> workItemIds,
      string[] columnReferenceNames)
    {
      Dictionary<string, int> columnIndices = new Dictionary<string, int>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      return this.PageWorkItems(workItemIds, columnReferenceNames, out columnIndices);
    }

    internal virtual IEnumerable<object[]> PageWorkItems(
      IEnumerable<int> workItemIds,
      string[] columnReferenceNames,
      out Dictionary<string, int> columnIndices)
    {
      GenericDataReader source = this.m_requestContext.GetService<WebAccessWorkItemService>().PageWorkItems(this.m_requestContext, workItemIds, (IEnumerable<string>) columnReferenceNames);
      columnIndices = new Dictionary<string, int>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      for (int i = 0; i < columnReferenceNames.Length; ++i)
      {
        string name = source.GetName(i);
        columnIndices[name] = i;
      }
      return source.Select<IDataRecord, object[]>((System.Func<IDataRecord, object[]>) (record =>
      {
        object[] values = new object[columnReferenceNames.Length];
        record.GetValues(values);
        return values;
      }));
    }

    private void BuildQueries()
    {
      ProductBacklogQueryBuilder queryBuilder = this.CreateQueryBuilder();
      this.m_drillDownQuery = queryBuilder.GetTreeQuery(this.m_stateCategories, !this.TeamOwnsAllItems, showCompletedChildItems: this.m_showCompletedChildItems);
      this.m_backlogQuery = queryBuilder.GetTreeQuery(this.m_stateCategories, false, showCompletedChildItems: this.m_showCompletedChildItems);
      this.m_ownershipQuery = queryBuilder.GetOwnershipQuery(this.m_treatItemsClosedBeforeInDaysAsUnowned);
      if (!this.m_backlogContext.IncludeParents)
        return;
      this.m_parentsQuery = queryBuilder.GetParentsQuery(this.m_stateCategories);
    }

    private bool TryGetBacklogSizeLimitCheckTimestampRegistryKey(out string key)
    {
      key = string.Empty;
      try
      {
        key = string.Format("/Service/Agile/Settings/BacklogSizeLimitCheckTimestamp/{0}/{1}", (object) this.m_backlogContext.Team.Id, (object) this.m_backlogContext.CurrentLevelConfiguration.Id);
        return true;
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(290968, "AgileService", "AgileService", ex);
      }
      return false;
    }

    private bool TryGetBacklogSizeLimitCheckTimestampRegistryValue(out DateTime timestamp)
    {
      timestamp = new DateTime();
      try
      {
        IVssRegistryService service = this.m_requestContext.GetService<IVssRegistryService>();
        string key;
        if (this.TryGetBacklogSizeLimitCheckTimestampRegistryKey(out key))
        {
          timestamp = service.GetValue<DateTime>(this.m_requestContext, (RegistryQuery) key, new DateTime());
          return timestamp != new DateTime();
        }
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(290966, "AgileService", "AgileService", ex);
      }
      return false;
    }

    private void SetBacklogSizeLimitCheckTimestampRegistryValue()
    {
      try
      {
        IVssRegistryService service = this.m_requestContext.GetService<IVssRegistryService>();
        string key;
        if (!this.TryGetBacklogSizeLimitCheckTimestampRegistryKey(out key))
          return;
        service.SetValue<DateTime>(this.m_requestContext, key, DateTime.UtcNow);
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(290967, "AgileService", "AgileService", ex);
      }
    }
  }
}
