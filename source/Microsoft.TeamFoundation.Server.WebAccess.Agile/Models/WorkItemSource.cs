// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemSource
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.GenericBoard.Filters;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  internal class WorkItemSource : IItemSource
  {
    internal const string ItemSourceWorkItemTracking = "wit";
    internal const int PerQueryExecutionLimit = 1000;
    private const string KanbanSettingsRootPath = "/Configuration/Application/Kanban";
    private const string InitialDiplayLimitRelativePath = "/InitialDisplayLimit";
    private const string OutgoingColumnChangedDateConstraintLimit = "/OutgoingColumnChangedDateConstraintLimit";
    private const int DefaultOuterColumnsDisplayLimit = 20;
    private const int DefaultOutgoingColumnChangedDateConstraintLimit = 183;
    private const string c_workItemColumnStateMismatchErrorMessage = "A WorkItem was encountered with a column that does not match the appropriate state. WorkItem Id: {0}, Column: {1}, State: {2}.\nCompleted columns: [{3}]\nProposedInProgress columns: [{4}]";
    private IVssRequestContext m_requestContext;
    private WebAccessWorkItemService m_workItemService;
    private IEnumerable<string> m_workItemTypes;
    private IAgileSettings m_settings;
    private ProductBacklogQueryBuilder m_queryBuilder;
    private IDictionary m_queryContext;
    private IDictionary<string, IDictionary<string, ISet<string>>> m_transitions;
    private System.Func<IDataRecord, bool> m_additionalFilter;
    private IEnumerable<string> m_additionalFields;
    private IList<string> m_pageColumns;
    private IList<int> m_idsToPage;
    private BacklogContext m_backlogcontext;
    private bool m_pageIdentityRefs;

    public WorkItemSource(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      ProductBacklogQueryBuilder queryBuilder,
      IDictionary queryContext,
      BacklogContext backlogContext,
      IDictionary<string, IDictionary<string, ISet<string>>> transitions,
      IEnumerable<string> additionalFields = null,
      System.Func<IDataRecord, bool> additionalFilter = null,
      bool pageIdentityRefs = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForNull<ProductBacklogQueryBuilder>(queryBuilder, nameof (queryBuilder));
      ArgumentUtility.CheckForNull<IDictionary>(queryContext, nameof (queryContext));
      ArgumentUtility.CheckForNull<IDictionary<string, IDictionary<string, ISet<string>>>>(transitions, nameof (transitions));
      this.m_requestContext = requestContext;
      this.m_workItemService = this.m_requestContext.GetService<WebAccessWorkItemService>();
      this.m_backlogcontext = backlogContext;
      this.m_idsToPage = (IList<int>) new List<int>();
      this.m_settings = settings;
      this.m_queryBuilder = queryBuilder;
      this.m_queryContext = queryContext;
      this.m_pageIdentityRefs = pageIdentityRefs;
      this.m_transitions = transitions;
      this.InProgressItemLimit = backlogContext.CurrentLevelConfiguration.WorkItemCountLimit;
      this.ItemDisplayLimit = this.InProgressItemLimit;
      this.m_workItemTypes = (IEnumerable<string>) backlogContext.CurrentLevelConfiguration.WorkItemTypes;
      this.HasReachedInProgressDisplayLimit = false;
      this.m_additionalFilter = additionalFilter;
      this.m_additionalFields = additionalFields;
    }

    public string Type
    {
      get => "wit";
      set
      {
      }
    }

    public IDictionary<string, IDictionary<string, ISet<string>>> Transitions
    {
      get => this.m_transitions;
      set
      {
      }
    }

    public IList<string> PageColumns
    {
      get
      {
        if (this.m_pageColumns == null)
        {
          List<string> stringList = new List<string>()
          {
            CoreFieldReferenceNames.Id,
            CoreFieldReferenceNames.State,
            CoreFieldReferenceNames.IterationPath,
            CoreFieldReferenceNames.AssignedTo,
            CoreFieldReferenceNames.Title,
            CoreFieldReferenceNames.WorkItemType,
            CoreFieldReferenceNames.ChangedDate,
            this.m_settings.GetClosedDateFieldReferenceName(this.m_requestContext),
            this.m_settings.Process.TeamField.Name,
            this.m_settings.Process.EffortField.Name,
            this.m_settings.Process.OrderByField.Name
          };
          if (this.m_additionalFields != null && this.m_additionalFields.Any<string>())
            stringList = stringList.Union<string>(this.m_additionalFields).ToList<string>();
          this.m_pageColumns = (IList<string>) stringList.Distinct<string>().ToList<string>();
        }
        return this.m_pageColumns;
      }
    }

    public bool HasReachedInProgressDisplayLimit { get; private set; }

    public int ItemDisplayLimit { get; private set; }

    public IEnumerable<IItem> GetItems() => Enumerable.Empty<IItem>();

    public IEnumerable<string> GetItemTypes() => this.m_workItemTypes;

    private int InProgressItemLimit { get; set; }

    public HierarchyDataReader GetPayload(BoardSettings boardSettings)
    {
      using (this.m_requestContext.TraceBlock(290079, 290080, "Agile", TfsTraceLayers.BusinessLogic, "WorkItemSource.GetPayload"))
        return this.m_requestContext.IsFeatureEnabled("WebAccess.Agile.Kanban.FlatQuery") ? this.GetPayloadFlat(boardSettings) : this.GetPayloadWithHierarchy(boardSettings);
    }

    private HierarchyDataReader GetPayloadFlat(BoardSettings boardSettings)
    {
      RegistryEntryCollection registryEntryCollection = this.m_requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(this.m_requestContext, (RegistryQuery) "/Configuration/Application/Kanban/*");
      int valueFromPath1 = registryEntryCollection.GetValueFromPath<int>("/Configuration/Application/Kanban/InitialDisplayLimit", 20);
      string fieldReferenceName = KanbanUtils.Instance.GetKanbanColumnFieldReferenceName(this.m_requestContext, boardSettings.ExtensionId.Value);
      BoardColumn boardColumn1 = boardSettings.Columns.First<BoardColumn>((System.Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Incoming));
      BoardColumn boardColumn2 = boardSettings.Columns.First<BoardColumn>((System.Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Outgoing));
      IEnumerable<BoardColumn> source1 = boardSettings.Columns.Where<BoardColumn>((System.Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.InProgress));
      DateTime now1 = DateTime.Now;
      DateTime now2 = DateTime.Now;
      IEnumerable<int> source2 = Enumerable.Empty<int>();
      IWorkItemQueryService service = this.m_requestContext.GetService<IWorkItemQueryService>();
      if (source1.Any<BoardColumn>())
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression idsByColumnNames = this.m_queryBuilder.GetBacklogWorkItemIdsByColumnNames(this.m_requestContext, this.m_queryContext, fieldReferenceName, (IReadOnlyCollection<string>) source1.Select<BoardColumn, string>((System.Func<BoardColumn, string>) (c => c.Name)).ToArray<string>(), (IReadOnlyCollection<string>) source1.SelectMany<BoardColumn, string>((System.Func<BoardColumn, IEnumerable<string>>) (c => (IEnumerable<string>) c.StateMappings.Values)).ToArray<string>());
        source2 = service.ExecuteQuery(this.m_requestContext, idsByColumnNames).WorkItemIds;
      }
      TimeSpan fullHierarchyElapsedTime = DateTime.Now - now2;
      DateTime now3 = DateTime.Now;
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression idsByColumnNames1 = this.m_queryBuilder.GetBacklogWorkItemIdsByColumnNames(this.m_requestContext, this.m_queryContext, fieldReferenceName, (IReadOnlyCollection<string>) new string[1]
      {
        boardColumn1.Name
      }, (IReadOnlyCollection<string>) boardColumn1.StateMappings.Values);
      IEnumerable<int> workItemIds1 = service.ExecuteQuery(this.m_requestContext, idsByColumnNames1).WorkItemIds;
      TimeSpan incomingIdsElapsedTime = DateTime.Now - now3;
      DateTime now4 = DateTime.Now;
      int valueFromPath2 = registryEntryCollection.GetValueFromPath<int>("/Configuration/Application/Kanban/OutgoingColumnChangedDateConstraintLimit", 183);
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression itemIdsByColumnName = this.m_queryBuilder.GetBacklogOutgoingWorkItemIdsByColumnName(this.m_requestContext, this.m_queryContext, fieldReferenceName, boardColumn2.Name, (IReadOnlyCollection<string>) boardColumn2.StateMappings.Values, valueFromPath2);
      IWorkItemQueryService itemQueryService = service;
      IVssRequestContext requestContext = this.m_requestContext;
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression query = itemIdsByColumnName;
      int num = this.m_requestContext.WitContext().ServerSettings.MaxQueryResultSize - 1;
      Guid? projectId = new Guid?();
      int topCount = num;
      IEnumerable<int> workItemIds2 = itemQueryService.ExecuteQuery(requestContext, query, projectId, topCount).WorkItemIds;
      TimeSpan outgoingIdsElapsedTime = DateTime.Now - now4;
      this.HasReachedInProgressDisplayLimit = source2.Count<int>() > this.InProgressItemLimit;
      this.m_idsToPage = (IList<int>) workItemIds1.Take<int>(valueFromPath1).Concat<int>(workItemIds2.Take<int>(valueFromPath1)).Concat<int>(source2.Take<int>(this.InProgressItemLimit)).Distinct<int>().ToList<int>();
      DateTime now5 = DateTime.Now;
      ICollection<IEnumerable<object>> data = this.PageWorkItemData((IEnumerable<int>) this.m_idsToPage);
      TimeSpan pageWorkItemsElapsedTime = DateTime.Now - now5;
      this.PublishCIEvents(DateTime.Now - now1, fullHierarchyElapsedTime, incomingIdsElapsedTime, outgoingIdsElapsedTime, pageWorkItemsElapsedTime);
      return new HierarchyDataReader((IEnumerable<string>) this.PageColumns, (IEnumerable<IEnumerable<object>>) data, orderedIncomingIds: workItemIds1, orderedOutgoingIds: workItemIds2);
    }

    private HierarchyDataReader GetPayloadWithHierarchy(BoardSettings boardSettings)
    {
      int valueFromPath = this.m_requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(this.m_requestContext, (RegistryQuery) "/Configuration/Application/Kanban/*").GetValueFromPath<int>("/Configuration/Application/Kanban/InitialDisplayLimit", 20);
      string fieldReferenceName = KanbanUtils.Instance.GetKanbanColumnFieldReferenceName(this.m_requestContext, boardSettings.ExtensionId.Value);
      string name1 = boardSettings.Columns.First<BoardColumn>((System.Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Incoming)).Name;
      boardSettings.Columns.Where<BoardColumn>((System.Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.InProgress)).Select<BoardColumn, string>((System.Func<BoardColumn, string>) (c => c.Name));
      string name2 = boardSettings.Columns.First<BoardColumn>((System.Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Outgoing)).Name;
      DateTime now1 = DateTime.Now;
      DateTime now2 = DateTime.Now;
      IWorkItemQueryService service = this.m_requestContext.GetService<IWorkItemQueryService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression backlogBoardQuery = this.m_queryBuilder.GetBacklogBoardQuery(this.m_requestContext, this.m_queryContext, WorkItemStateCategory.Proposed, WorkItemStateCategory.InProgress, WorkItemStateCategory.Completed, WorkItemStateCategory.Resolved);
      IEnumerable<LinkQueryResultEntry> workItemLinks = service.ExecuteQuery(this.m_requestContext, backlogBoardQuery).WorkItemLinks;
      TimeSpan fullHierarchyElapsedTime = DateTime.Now - now2;
      DateTime now3 = DateTime.Now;
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression idsByColumnNames = this.m_queryBuilder.GetBacklogWorkItemIdsByColumnNames(this.m_requestContext, this.m_queryContext, fieldReferenceName, (IReadOnlyCollection<string>) new string[1]
      {
        name1
      });
      IEnumerable<int> workItemIds1 = service.ExecuteQuery(this.m_requestContext, idsByColumnNames).WorkItemIds;
      TimeSpan incomingIdsElapsedTime = DateTime.Now - now3;
      DateTime now4 = DateTime.Now;
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression itemIdsByColumnName = this.m_queryBuilder.GetBacklogOutgoingWorkItemIdsByColumnName(this.m_requestContext, this.m_queryContext, fieldReferenceName, name2);
      IEnumerable<int> workItemIds2 = service.ExecuteQuery(this.m_requestContext, itemIdsByColumnName).WorkItemIds;
      TimeSpan outgoingIdsElapsedTime = DateTime.Now - now4;
      IEnumerable<int> orderedIncomingLeafIds;
      IEnumerable<int> orderedInProgressLeafIds;
      IEnumerable<int> orderedOutgoingLeafIds;
      BoardHierarchyDataHelper.GetOrderedLeafIds(workItemLinks, workItemIds1, workItemIds2, out orderedIncomingLeafIds, out orderedInProgressLeafIds, out orderedOutgoingLeafIds);
      this.HasReachedInProgressDisplayLimit = orderedInProgressLeafIds.Count<int>() > this.InProgressItemLimit;
      this.m_idsToPage = (IList<int>) BoardHierarchyDataHelper.GetSubTreeIdsByLeafNodes(workItemLinks, orderedIncomingLeafIds, orderedOutgoingLeafIds, valueFromPath, orderedInProgressLeafIds, this.InProgressItemLimit);
      DateTime now5 = DateTime.Now;
      ICollection<IEnumerable<object>> data = this.PageWorkItemData((IEnumerable<int>) this.m_idsToPage);
      TimeSpan pageWorkItemsElapsedTime = DateTime.Now - now5;
      this.PublishCIEvents(DateTime.Now - now1, fullHierarchyElapsedTime, incomingIdsElapsedTime, outgoingIdsElapsedTime, pageWorkItemsElapsedTime);
      ICollection<Tuple<int, int>> hierarchyByIds = BoardHierarchyDataHelper.GetHierarchyByIds(workItemLinks, (IEnumerable<int>) this.m_idsToPage);
      this.m_requestContext.GetService<CustomerIntelligenceService>().Publish(this.m_requestContext, CustomerIntelligenceArea.Agile, Microsoft.TeamFoundation.Framework.Server.CustomerIntelligenceFeature.KanbanBoardWorkItemSource, "HasSameTypeHierarchy", hierarchyByIds.Count > 0);
      return new HierarchyDataReader((IEnumerable<string>) this.PageColumns, (IEnumerable<IEnumerable<object>>) data, (IEnumerable<Tuple<int, int>>) hierarchyByIds, orderedIncomingLeafIds, orderedOutgoingLeafIds);
    }

    public ICollection<ParentChildWIMap> GetParentPayload(BoardSettings boardSettings)
    {
      using (this.m_requestContext.TraceBlock(290721, 290724, "Agile", TfsTraceLayers.BusinessLogic, "WorkItemSource.GetParentPayload"))
      {
        BoardParentWIFilterHelper parentWiFilterHelper = new BoardParentWIFilterHelper();
        ICollection<ParentChildWIMap> parentPayload = (ICollection<ParentChildWIMap>) null;
        try
        {
          BacklogLevelConfiguration levelConfiguration = this.m_settings.BacklogConfiguration.GetBacklogLevelConfiguration(boardSettings.BacklogLevelId);
          if (levelConfiguration != null)
          {
            BacklogLevelConfiguration parentBacklogLevel = (BacklogLevelConfiguration) null;
            if (this.m_settings.BacklogConfiguration.TryGetBacklogParent(levelConfiguration.Id, out parentBacklogLevel))
              parentPayload = parentWiFilterHelper.GetParentChildWIMap(this.m_requestContext, this.m_settings, parentBacklogLevel, this.m_idsToPage.ToArray<int>());
          }
        }
        catch (Exception ex)
        {
          this.m_requestContext.TraceException(290723, "Agile", TfsTraceLayers.BusinessLogic, ex);
          parentPayload = (ICollection<ParentChildWIMap>) null;
        }
        return parentPayload;
      }
    }

    public HierarchyDataReader GetBoardWorkItems(IEnumerable<int> workItemIds)
    {
      ICollection<Tuple<int, int>> hierarchy = (ICollection<Tuple<int, int>>) null;
      List<int> workItemIds1 = new List<int>(workItemIds);
      if (!this.m_requestContext.IsFeatureEnabled("WebAccess.Agile.Kanban.FlatQuery"))
      {
        IEnumerable<LinkQueryResultEntry> workItemHierarchy = this.GetWorkItemHierarchy(workItemIds);
        IEnumerable<int> ancestorIds = BoardHierarchyDataHelper.FindAncestorIds(workItemHierarchy, workItemIds);
        workItemIds1.AddRange(ancestorIds);
        hierarchy = BoardHierarchyDataHelper.GetHierarchyByIds(workItemHierarchy, (IEnumerable<int>) workItemIds1);
      }
      return new HierarchyDataReader((IEnumerable<string>) this.PageColumns, (IEnumerable<IEnumerable<object>>) this.PageWorkItemData((IEnumerable<int>) workItemIds1), (IEnumerable<Tuple<int, int>>) hierarchy);
    }

    private IEnumerable<LinkQueryResultEntry> GetWorkItemHierarchy(IEnumerable<int> workItemIds)
    {
      this.m_queryBuilder.Fields = (IEnumerable<string>) this.PageColumns;
      IWorkItemQueryService service = this.m_requestContext.GetService<IWorkItemQueryService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression boardBottomUpIdQuery = this.m_queryBuilder.GetBacklogBoardBottomUpIdQuery(this.m_requestContext, this.m_queryContext, workItemIds.ToArray<int>());
      IVssRequestContext requestContext = this.m_requestContext;
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression query = boardBottomUpIdQuery;
      Guid? projectId = new Guid?();
      return service.ExecuteQuery(requestContext, query, projectId).WorkItemLinks;
    }

    private ICollection<IEnumerable<object>> PageWorkItemData(IEnumerable<int> workItemIds)
    {
      ICollection<IEnumerable<object>> objects = (ICollection<IEnumerable<object>>) new LinkedList<IEnumerable<object>>();
      using (this.m_requestContext.TraceBlock(290077, 290078, "Agile", TfsTraceLayers.BusinessLogic, "WorkItemSource.PageWorkItemData"))
      {
        this.m_queryBuilder.Fields = (IEnumerable<string>) this.PageColumns;
        this.m_requestContext.GetService<IWorkItemQueryService>();
        foreach (IDataRecord workItem in this.m_workItemService.GetWorkItems(this.m_requestContext, workItemIds, (IEnumerable<string>) this.PageColumns, this.m_pageIdentityRefs))
        {
          object[] objArray = new object[this.PageColumns.Count];
          object[] values = objArray;
          workItem.GetValues(values);
          objects.Add((IEnumerable<object>) objArray);
        }
      }
      return objects;
    }

    private void PublishCIEvents(
      TimeSpan overallTime,
      TimeSpan fullHierarchyElapsedTime,
      TimeSpan incomingIdsElapsedTime,
      TimeSpan outgoingIdsElapsedTime,
      TimeSpan pageWorkItemsElapsedTime)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.GetPayloadElapsedTime, overallTime.TotalMilliseconds);
      properties.Add(CustomerIntelligenceProperty.FullHierarchyElapsedTime, fullHierarchyElapsedTime.TotalMilliseconds);
      properties.Add(CustomerIntelligenceProperty.IncomingIdsElapsedTime, incomingIdsElapsedTime.TotalMilliseconds);
      properties.Add(CustomerIntelligenceProperty.OutgoingIdsElapsedTime, outgoingIdsElapsedTime.TotalMilliseconds);
      properties.Add(CustomerIntelligenceProperty.PageWorkItemsElapsedTime, pageWorkItemsElapsedTime.TotalMilliseconds);
      this.m_requestContext.GetService<CustomerIntelligenceService>().Publish(this.m_requestContext, CustomerIntelligenceArea.Agile, Microsoft.TeamFoundation.Framework.Server.CustomerIntelligenceFeature.KanbanBoardPayload, properties);
    }

    public IEnumerable<ICardFieldDefinition> GetCardFieldDefinitions() => CardSettingsUtils.Instance.GetCardFieldDefinitions(this.m_requestContext, (IEnumerable<string>) this.PageColumns);

    public JsObject ToJson(BoardSettings boardSettings)
    {
      bool includeParentPayload = false;
      if (boardSettings.BoardFilterSettings != null)
      {
        Dictionary<string, WorkItemFilter> initialFilter = boardSettings.BoardFilterSettings.InitialFilter;
        List<string> values = (initialFilter == null ? 0 : (initialFilter.ContainsKey(BoardFilterSettingsManager.BoardFilterParentItemFieldRefName) ? 1 : 0)) != 0 ? initialFilter[BoardFilterSettingsManager.BoardFilterParentItemFieldRefName]?.Values : (List<string>) null;
        if (values != null && values.Count > 0)
          includeParentPayload = true;
      }
      return this.ToJson(includeParentPayload, boardSettings, this.m_pageIdentityRefs);
    }
  }
}
