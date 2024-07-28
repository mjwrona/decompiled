// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Agile.Server.Reorder.ProductBacklogReorderHelper
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.Exceptions;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Boards.Agile.Server.Reorder
{
  public class ProductBacklogReorderHelper
  {
    internal const double DefaultMinOrderByValue = 0.0;
    internal const double DefaultMaxOrderByValue = 2000000000.0;
    internal const double DefaultSufficientDelta = 128.0;
    internal static readonly WorkItemStateCategory[] ActiveStates = new WorkItemStateCategory[3]
    {
      WorkItemStateCategory.Proposed,
      WorkItemStateCategory.InProgress,
      WorkItemStateCategory.Resolved
    };
    internal static readonly WorkItemStateCategory[] ActiveAndCompleteStates = new WorkItemStateCategory[4]
    {
      WorkItemStateCategory.Proposed,
      WorkItemStateCategory.InProgress,
      WorkItemStateCategory.Resolved,
      WorkItemStateCategory.Completed
    };
    private IDictionary m_queryContext;
    private string m_productBacklogIterationPath;
    private WebAccessWorkItemService m_workItemService;
    private CustomerIntelligenceData m_BmlData;
    protected IAgileSettings m_settings;
    protected IVssRequestContext m_requestContext;
    protected string m_iterationPath;
    protected List<WorkItemUpdate> m_workItemUpdates = new List<WorkItemUpdate>();
    private string m_orderByFieldReferenceName;

    public ProductBacklogReorderHelper(
      IVssRequestContext requestContext,
      WebAccessWorkItemService workItemService,
      IAgileSettings settings,
      string productBacklogIterationPath,
      Guid projectId,
      Guid? iterationGuid,
      IDictionary queryContext = null)
    {
      ArgumentUtility.CheckForNull<WebAccessWorkItemService>(workItemService, nameof (workItemService));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      ArgumentUtility.CheckStringForNullOrEmpty(productBacklogIterationPath, nameof (productBacklogIterationPath));
      this.m_requestContext = requestContext;
      this.m_workItemService = workItemService;
      this.m_settings = settings;
      this.m_productBacklogIterationPath = productBacklogIterationPath;
      this.m_queryContext = queryContext;
      if (!iterationGuid.HasValue || !(iterationGuid.Value != Guid.Empty))
        return;
      this.m_iterationPath = (requestContext.GetService<WorkItemTrackingTreeService>().GetTreeNode(requestContext, projectId, iterationGuid.Value, false) ?? throw new InvalidIterationIdException(Microsoft.TeamFoundation.Agile.Server.AgileResources.ProductBacklogReorderHelper_InvalidIterationId)).GetPath(requestContext);
    }

    public IEnumerable<ReorderResult> Reorder(IEnumerable<ReorderOperation> operations)
    {
      if (operations.IsNullOrEmpty<ReorderOperation>())
        throw new ReorderWorkItemsInvalidOperationsException(Microsoft.TeamFoundation.Agile.Server.AgileResources.ReorderWorkItemsOperationsNullOrEmpty);
      List<ReorderResult> source = new List<ReorderResult>();
      foreach (ReorderOperation operation in operations)
        source.AddRange(this.Reorder(operation));
      return (IEnumerable<ReorderResult>) source.Distinct<ReorderResult>().ToList<ReorderResult>();
    }

    public IEnumerable<ReorderResult> Reorder(ReorderOperation operation)
    {
      ArgumentUtility.CheckForNull<ReorderOperation>(operation, nameof (operation));
      using (this.m_requestContext.TraceBlock(290302, 290303, "Agile", TfsTraceLayers.BusinessLogic, nameof (Reorder)))
      {
        try
        {
          this.m_BmlData = new CustomerIntelligenceData();
          Stopwatch stopwatch = Stopwatch.StartNew();
          ProductBacklogReorderHelper.ReorderContext reorderContext = new ProductBacklogReorderHelper.ReorderContext(operation);
          this.ProcessReorderOperation(reorderContext);
          ICollection<ReorderResult> reorderResults = this.SaveWorkItems(reorderContext);
          stopwatch.Stop();
          this.m_BmlData.Add(CustomerIntelligenceProperty.TeamName, string.IsNullOrEmpty(this.m_settings.Team.Name) ? "unknown" : this.m_settings.Team.Name);
          this.m_BmlData.Add(CustomerIntelligenceProperty.ProjectName, string.IsNullOrEmpty(this.m_settings.ProjectName) ? "unknown" : this.m_settings.ProjectName);
          CustomerIntelligenceData bmlData = this.m_BmlData;
          string onRootLevel = CustomerIntelligenceProperty.OnRootLevel;
          int? parentId = operation.ParentId;
          int num1 = 0;
          int num2 = parentId.GetValueOrDefault() <= num1 & parentId.HasValue ? 1 : 0;
          bmlData.Add(onRootLevel, num2 != 0);
          this.m_BmlData.Add(CustomerIntelligenceProperty.DurationMS, (double) stopwatch.ElapsedMilliseconds);
          this.m_BmlData.Add(CustomerIntelligenceProperty.ItemsReordered, (double) operation.Ids.Length);
          this.m_BmlData.Add(CustomerIntelligenceProperty.IsIterationPathSpecified, !string.IsNullOrWhiteSpace(this.m_iterationPath));
          this.m_BmlData.Add(CustomerIntelligenceProperty.BacklogLevel, string.IsNullOrWhiteSpace(this.m_iterationPath) ? "Product Backlog" : "Iteration Backlog: " + this.m_iterationPath);
          this.m_BmlData.Add(CustomerIntelligenceProperty.IterationBacklogSparsification, reorderContext.NeedsSparsification);
          CustomerIntelligenceService service = this.m_requestContext.GetService<CustomerIntelligenceService>();
          if (operation.Ids.Length <= 1)
            service.Publish(this.m_requestContext, CustomerIntelligenceArea.Agile, AgileCustomerIntelligenceFeature.BacklogReorder, this.m_BmlData);
          else
            service.Publish(this.m_requestContext, CustomerIntelligenceArea.Agile, AgileCustomerIntelligenceFeature.MultiSelectBacklogReorder, this.m_BmlData);
          return (IEnumerable<ReorderResult>) reorderResults;
        }
        catch
        {
          string message = string.Format("Reorder Operation failed: ReorderIds:[{0}]; NextId:{1}; PreviousId:{2}; ParentId:{3}; BacklogContext: {4}", (object) this.CreateCommaSeparatedString<int>((IEnumerable<int>) operation.Ids), (object) operation.NextId, (object) operation.PreviousId, (object) operation.ParentId, string.IsNullOrWhiteSpace(this.m_iterationPath) ? (object) "Product Backlog" : (object) ("Iteration Backlog: " + this.m_iterationPath));
          this.m_requestContext.Trace(290319, TraceLevel.Verbose, "Agile", TfsTraceLayers.BusinessLogic, message);
          throw;
        }
      }
    }

    private List<string> QueryFields => new List<string>(3)
    {
      "System.Id",
      this.m_settings.Process.OrderByField.Name,
      "System.Rev"
    };

    protected virtual string OrderByFieldReferenceName
    {
      get
      {
        if (this.m_orderByFieldReferenceName == null)
        {
          FieldDefinition field;
          this.m_workItemService.TryGetFieldDefinitionByName(this.m_requestContext, this.m_settings.Process.OrderByField.Name, out field);
          this.m_orderByFieldReferenceName = field.ReferenceName;
        }
        return this.m_orderByFieldReferenceName;
      }
    }

    private void ProcessReorderOperation(
      ProductBacklogReorderHelper.ReorderContext reorderContext)
    {
      ArgumentUtility.CheckForNull<ReorderOperation>(reorderContext.Operation, "Operation");
      using (this.m_requestContext.TraceBlock(290324, 290325, "Agile", TfsTraceLayers.BusinessLogic, nameof (ProcessReorderOperation)))
      {
        ReorderOperation operation = reorderContext.Operation;
        this.ValidateReasonableReorderOperation(operation);
        IDictionary<int, PagedWorkItemData> pagedData = this.PageBasicWorkItemData(reorderContext.GetReorderOperationIds());
        this.ValidateWorkItemExistence(operation, pagedData);
        BacklogContext backlogContext = this.GetBacklogContext(this.m_settings.Team, operation, pagedData);
        this.ValidateWorkItemTypes(operation, backlogContext, pagedData);
        this.GetReorderData(reorderContext, backlogContext);
        this.PerformReorder(reorderContext);
        this.m_BmlData.Add(CustomerIntelligenceProperty.BacklogHub, backlogContext.CurrentLevelConfiguration.Name);
        this.m_BmlData.Add(CustomerIntelligenceProperty.BacklogLevelId, backlogContext.CurrentLevelConfiguration.Id);
        int num = reorderContext.PreviousItem != null ? reorderContext.PreviousItem.Index + 1 : 0;
        this.m_BmlData.Add(CustomerIntelligenceProperty.InsertionIndex, (double) num);
        this.m_BmlData.Add(CustomerIntelligenceProperty.BacklogLevelSize, (double) reorderContext.WorkItems.Count);
      }
    }

    private string CreateCommaSeparatedString<T>(IEnumerable<T> values) => string.Join<T>(",", values);

    private void ValidateReasonableReorderOperation(ReorderOperation operation)
    {
      if (((IEnumerable<int>) operation.Ids).Any<int>((System.Func<int, bool>) (id => id < 1)) || ((IEnumerable<int>) operation.Ids).IsNullOrEmpty<int>())
        throw new ArgumentException(Microsoft.TeamFoundation.Agile.Server.AgileResources.ProductBacklogReorderHelper_InvalidReorderIds, "reorderOperation.Ids");
      if (!operation.PreviousId.HasValue && !operation.NextId.HasValue)
        operation.PreviousId = new int?(((IEnumerable<int>) operation.Ids).FirstOrDefault<int>());
      if (operation.PreviousId.GetValueOrDefault() < 0)
        operation.PreviousId = new int?();
      if (operation.NextId.GetValueOrDefault() < 0)
        operation.NextId = new int?();
      int? parentId1 = operation.ParentId;
      int? previousId = operation.PreviousId;
      int? nullable1;
      if (!(parentId1.GetValueOrDefault() == previousId.GetValueOrDefault() & parentId1.HasValue == previousId.HasValue))
      {
        int? parentId2 = operation.ParentId;
        int? nextId = operation.NextId;
        if (!(parentId2.GetValueOrDefault() == nextId.GetValueOrDefault() & parentId2.HasValue == nextId.HasValue))
        {
          int[] ids = operation.Ids;
          nullable1 = operation.ParentId;
          int valueOrDefault = nullable1.GetValueOrDefault();
          if (!((IEnumerable<int>) ids).Contains<int>(valueOrDefault))
            goto label_12;
        }
      }
      ReorderOperation reorderOperation = operation;
      nullable1 = new int?();
      int? nullable2 = nullable1;
      reorderOperation.ParentId = nullable2;
label_12:
      nullable1 = operation.PreviousId;
      if (nullable1.HasValue)
        return;
      nullable1 = operation.NextId;
      if (!nullable1.HasValue)
        throw new ArgumentException(Microsoft.TeamFoundation.Agile.Server.AgileResources.ProductBacklogReorderHelper_InvalidReorderIds, "reorderOperation.PreviousId");
    }

    private void FixPreviousAndNext(
      ProductBacklogReorderHelper.ReorderContext reorderContext,
      List<ProductBacklogReorderHelper.WorkItemData> notReorderedWorkItems)
    {
      ReorderOperation operation = reorderContext.Operation;
      int? previousId1 = operation.PreviousId;
      int num1 = 0;
      if (previousId1.GetValueOrDefault() == num1 & previousId1.HasValue)
      {
        reorderContext.PreviousItem = (ProductBacklogReorderHelper.WorkItemData) null;
        reorderContext.NextItem = notReorderedWorkItems.FirstOrDefault<ProductBacklogReorderHelper.WorkItemData>();
        if (reorderContext.NextItem == null)
          operation.NextId = new int?(0);
      }
      else if (reorderContext.PreviousItem != null)
      {
        if (((IEnumerable<int>) operation.Ids).Contains<int>(reorderContext.PreviousItem.Id))
        {
          int lastIndex = notReorderedWorkItems.FindLastIndex((Predicate<ProductBacklogReorderHelper.WorkItemData>) (x => x.Index <= reorderContext.PreviousItem.Index));
          if (lastIndex == -1)
          {
            reorderContext.PreviousItem = (ProductBacklogReorderHelper.WorkItemData) null;
            operation.PreviousId = new int?(0);
            reorderContext.NextItem = notReorderedWorkItems.FirstOrDefault<ProductBacklogReorderHelper.WorkItemData>();
          }
          else
          {
            reorderContext.PreviousItem = notReorderedWorkItems[lastIndex];
            operation.PreviousId = new int?(reorderContext.PreviousItem.Id);
            reorderContext.NextItem = lastIndex + 1 >= notReorderedWorkItems.Count ? (ProductBacklogReorderHelper.WorkItemData) null : notReorderedWorkItems[lastIndex + 1];
          }
          if (reorderContext.NextItem == null)
            operation.NextId = new int?(0);
        }
        else
        {
          reorderContext.NextItem = notReorderedWorkItems.Find((Predicate<ProductBacklogReorderHelper.WorkItemData>) (x => x.Index >= reorderContext.PreviousItem.Index && x.Id != reorderContext.PreviousItem.Id));
          if (reorderContext.NextItem == null)
            operation.NextId = new int?(0);
        }
      }
      else
      {
        int? nextId = operation.NextId;
        int num2 = 0;
        if (nextId.GetValueOrDefault() == num2 & nextId.HasValue)
        {
          reorderContext.PreviousItem = notReorderedWorkItems.LastOrDefault<ProductBacklogReorderHelper.WorkItemData>();
          reorderContext.NextItem = (ProductBacklogReorderHelper.WorkItemData) null;
          if (reorderContext.PreviousItem == null)
            operation.PreviousId = new int?(0);
        }
        else if (reorderContext.NextItem != null)
        {
          if (((IEnumerable<int>) operation.Ids).Contains<int>(reorderContext.NextItem.Id))
          {
            int index = notReorderedWorkItems.FindIndex((Predicate<ProductBacklogReorderHelper.WorkItemData>) (x => x.Index >= reorderContext.NextItem.Index));
            if (index == -1)
            {
              reorderContext.NextItem = (ProductBacklogReorderHelper.WorkItemData) null;
              operation.NextId = new int?(0);
              reorderContext.PreviousItem = notReorderedWorkItems.LastOrDefault<ProductBacklogReorderHelper.WorkItemData>();
            }
            else
            {
              reorderContext.NextItem = notReorderedWorkItems[index];
              operation.NextId = new int?(reorderContext.NextItem.Id);
              reorderContext.PreviousItem = index - 1 < 0 ? (ProductBacklogReorderHelper.WorkItemData) null : notReorderedWorkItems[index - 1];
            }
            if (reorderContext.PreviousItem == null)
              operation.PreviousId = new int?(0);
          }
          else
          {
            reorderContext.PreviousItem = notReorderedWorkItems.FindLast((Predicate<ProductBacklogReorderHelper.WorkItemData>) (x => x.Index <= reorderContext.NextItem.Index && x.Id != reorderContext.NextItem.Id));
            if (reorderContext.PreviousItem == null)
              operation.PreviousId = new int?(0);
          }
        }
        else
        {
          this.m_requestContext.Trace(290245, TraceLevel.Verbose, "Agile", TfsTraceLayers.BusinessLogic, "Failed to find pivot work items. PreviousId: {0}; NextId: {1}", (object) operation.PreviousId, (object) operation.NextId);
          throw new BacklogChangedException();
        }
      }
      int? previousId2 = operation.PreviousId;
      int num3 = 0;
      if (!(previousId2.GetValueOrDefault() == num3 & previousId2.HasValue) && reorderContext.PreviousItem != null)
        operation.PreviousId = new int?(reorderContext.PreviousItem.Id);
      int? nextId1 = operation.NextId;
      int num4 = 0;
      if (nextId1.GetValueOrDefault() == num4 & nextId1.HasValue || reorderContext.NextItem == null)
        return;
      operation.NextId = new int?(reorderContext.NextItem.Id);
    }

    private void GetReorderData(
      ProductBacklogReorderHelper.ReorderContext reorderContext,
      BacklogContext backlogContext)
    {
      using (this.m_requestContext.TraceBlock(290304, 290305, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetReorderData)))
      {
        ArgumentUtility.CheckForNull<ReorderOperation>(reorderContext.Operation, "Operation");
        ArgumentUtility.CheckForNull<BacklogContext>(backlogContext, nameof (backlogContext));
        ReorderOperation operation = reorderContext.Operation;
        int valueOrDefault1 = operation.PreviousId.GetValueOrDefault();
        int valueOrDefault2 = operation.NextId.GetValueOrDefault();
        IEnumerable<int> orderedWorkItemIds = this.GetOrderedWorkItemIds(reorderContext, backlogContext);
        IDictionary<int, IDataRecord> workItems = this.GetWorkItems(orderedWorkItemIds);
        if (reorderContext.IsNestedWorkItem && (valueOrDefault1 <= 0 || orderedWorkItemIds.Contains<int>(valueOrDefault1) ? (valueOrDefault2 <= 0 ? 0 : (!orderedWorkItemIds.Contains<int>(valueOrDefault2) ? 1 : 0)) : 1) != 0)
          throw new SameTypeHierarchyException(this.CreateCommaSeparatedString<int>((IEnumerable<int>) operation.Ids));
        int index = 0;
        reorderContext.WorkItems = new List<ProductBacklogReorderHelper.WorkItemData>();
        Dictionary<int, ProductBacklogReorderHelper.WorkItemData> reorderedWorkItems = new Dictionary<int, ProductBacklogReorderHelper.WorkItemData>();
        List<ProductBacklogReorderHelper.WorkItemData> notReorderedWorkItems = new List<ProductBacklogReorderHelper.WorkItemData>();
        foreach (int key in orderedWorkItemIds)
        {
          IDataRecord dataRecord;
          if (workItems.TryGetValue(key, out dataRecord))
          {
            ProductBacklogReorderHelper.WorkItemData workItemData = new ProductBacklogReorderHelper.WorkItemData(dataRecord, this.m_settings.Process.OrderByField.Name, index);
            reorderContext.WorkItems.Add(workItemData);
            ++index;
            if (((IEnumerable<int>) operation.Ids).Contains<int>(workItemData.Id))
              reorderedWorkItems.Add(workItemData.Id, workItemData);
            else
              notReorderedWorkItems.Add(workItemData);
            int? nullable = operation.PreviousId;
            int id1 = workItemData.Id;
            if (nullable.GetValueOrDefault() == id1 & nullable.HasValue)
              reorderContext.PreviousItem = workItemData;
            nullable = operation.NextId;
            int id2 = workItemData.Id;
            if (nullable.GetValueOrDefault() == id2 & nullable.HasValue)
              reorderContext.NextItem = workItemData;
          }
        }
        if (reorderContext.PreviousItem != null && reorderContext.NextItem != null && reorderContext.PreviousItem.OrderBy > reorderContext.NextItem.OrderBy)
        {
          if (reorderContext.HasNonActiveAncestor)
            throw new InvalidHierarchyException();
          this.m_requestContext.Trace(290244, TraceLevel.Verbose, "Agile", TfsTraceLayers.BusinessLogic, "Previous Item and Next Item have conflicting Order By");
          throw new BacklogChangedException();
        }
        if (operation.Ids.Length != reorderedWorkItems.Count || reorderedWorkItems.IsNullOrEmpty<KeyValuePair<int, ProductBacklogReorderHelper.WorkItemData>>())
        {
          this.m_requestContext.TraceAlways(290318, TraceLevel.Verbose, "Agile", TfsTraceLayers.BusinessLogic, "Failed to find reorder work items. Ids: [{0}]", (object) this.CreateCommaSeparatedString<int>(((IEnumerable<int>) operation.Ids).Where<int>((System.Func<int, bool>) (id => !reorderedWorkItems.Values.Any<ProductBacklogReorderHelper.WorkItemData>((System.Func<ProductBacklogReorderHelper.WorkItemData, bool>) (x => x.Id == id))))));
          throw new BacklogChangedException();
        }
        reorderContext.ReorderItems = ((IEnumerable<int>) operation.Ids).Select<int, ProductBacklogReorderHelper.WorkItemData>((System.Func<int, ProductBacklogReorderHelper.WorkItemData>) (id => reorderedWorkItems[id]));
        this.FixPreviousAndNext(reorderContext, notReorderedWorkItems);
      }
    }

    private IEnumerable<int> GetOrderedWorkItemIds(
      ProductBacklogReorderHelper.ReorderContext reorderContext,
      BacklogContext backlogContext)
    {
      using (this.m_requestContext.TraceBlock(290306, 290307, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetOrderedWorkItemIds)))
      {
        ReorderOperation operation = reorderContext.Operation;
        int[] reorderIds = operation.Ids;
        string query = this.GetQuery(reorderContext, backlogContext, (IEnumerable<string>) this.QueryFields);
        this.m_requestContext.Trace(290243, TraceLevel.Verbose, "Agile", TfsTraceLayers.BusinessLogic, "ReorderQuery: " + query);
        IEnumerable<LinkQueryResultEntry> queryResultEntries = this.RunQuery(query);
        if (!operation.ParentId.HasValue)
        {
          LinkQueryResultEntry queryResultEntry = queryResultEntries.FirstOrDefault<LinkQueryResultEntry>((System.Func<LinkQueryResultEntry, bool>) (link => link.TargetId == reorderIds[0]));
          operation.ParentId = queryResultEntry == null ? new int?(0) : new int?(queryResultEntry.SourceId);
        }
        ISet<int> intSet = this.ValidateHierarchy(reorderContext, backlogContext, queryResultEntries);
        if (intSet != null)
        {
          foreach (LinkQueryResultEntry queryResultEntry in queryResultEntries)
          {
            if (intSet.Contains(queryResultEntry.SourceId))
              queryResultEntry.SourceId = 0;
          }
          if (intSet.Contains(operation.ParentId.Value))
            operation.ParentId = new int?(0);
          if (this.ValidateHierarchy(reorderContext, backlogContext, queryResultEntries) != null && intSet.Count > 0)
            throw new InvalidHierarchyException();
        }
        int parentId = operation.ParentId.GetValueOrDefault();
        return queryResultEntries.Where<LinkQueryResultEntry>((System.Func<LinkQueryResultEntry, bool>) (link => link.SourceId == parentId)).Select<LinkQueryResultEntry, int>((System.Func<LinkQueryResultEntry, int>) (link => link.TargetId));
      }
    }

    private ISet<int> ValidateHierarchy(
      ProductBacklogReorderHelper.ReorderContext reorderContext,
      BacklogContext backlogContext,
      IEnumerable<LinkQueryResultEntry> queryResults)
    {
      ReorderOperation operation = reorderContext.Operation;
      Dictionary<int, int> parentMap = queryResults.ToDictionary<LinkQueryResultEntry, int, int>((System.Func<LinkQueryResultEntry, int>) (l => l.TargetId), (System.Func<LinkQueryResultEntry, int>) (l => l.SourceId));
      int? nullable = operation.PreviousId;
      if (nullable.HasValue)
      {
        nullable = operation.PreviousId;
        int valueOrDefault1 = nullable.GetValueOrDefault();
        nullable = operation.ParentId;
        int valueOrDefault2 = nullable.GetValueOrDefault();
        Dictionary<int, int> parentMap1 = parentMap;
        int immediateAncestor = this.GetImmediateAncestor(valueOrDefault1, valueOrDefault2, parentMap1);
        nullable = operation.PreviousId;
        int num = immediateAncestor;
        if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
        {
          string commaSeparatedString = this.CreateCommaSeparatedString<int>((IEnumerable<int>) operation.Ids);
          nullable = operation.PreviousId;
          int valueOrDefault3 = nullable.GetValueOrDefault();
          throw new SameTypeHierarchyException(commaSeparatedString, valueOrDefault3, true);
        }
      }
      nullable = operation.NextId;
      if (nullable.HasValue)
      {
        nullable = operation.NextId;
        int valueOrDefault4 = nullable.GetValueOrDefault();
        nullable = operation.ParentId;
        int valueOrDefault5 = nullable.GetValueOrDefault();
        Dictionary<int, int> parentMap2 = parentMap;
        int immediateAncestor = this.GetImmediateAncestor(valueOrDefault4, valueOrDefault5, parentMap2);
        nullable = operation.NextId;
        int num = immediateAncestor;
        if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
        {
          string commaSeparatedString = this.CreateCommaSeparatedString<int>((IEnumerable<int>) operation.Ids);
          nullable = operation.NextId;
          int valueOrDefault6 = nullable.GetValueOrDefault();
          throw new SameTypeHierarchyException(commaSeparatedString, valueOrDefault6, false);
        }
      }
      if (parentMap.ContainsKey(operation.Ids[0]) && parentMap[operation.Ids[0]] > 0)
      {
        int id = operation.Ids[0];
        int key = parentMap[operation.Ids[0]];
        IDictionary<int, PagedWorkItemData> dictionary = this.PageBasicWorkItemData((IEnumerable<int>) new int[2]
        {
          id,
          key
        });
        if (this.GetImmediateAncestor(id, 0, parentMap) != id || dictionary[id].Type == dictionary[key].Type)
          reorderContext.IsNestedWorkItem = true;
      }
      IEnumerable<int> source = ((IEnumerable<int>) operation.Ids).Where<int>((System.Func<int, bool>) (x => parentMap.ContainsKey(x))).Select<int, int>((System.Func<int, int>) (x => parentMap[x])).Distinct<int>();
      nullable = operation.PreviousId;
      int valueOrDefault7 = nullable.GetValueOrDefault();
      nullable = operation.NextId;
      int valueOrDefault8 = nullable.GetValueOrDefault();
      int num1 = source.Count<int>() <= 1 ? 0 : (source.Any<int>((System.Func<int, bool>) (x => x > 0)) ? 1 : 0);
      bool flag1 = valueOrDefault7 <= 0 && valueOrDefault8 <= 0;
      bool flag2 = parentMap.ContainsKey(valueOrDefault7) ? source.Contains<int>(parentMap[valueOrDefault7]) : flag1;
      bool flag3 = parentMap.ContainsKey(valueOrDefault8) ? source.Contains<int>(parentMap[valueOrDefault8]) : flag1;
      if (num1 == 0 && (flag2 || flag3))
        return (ISet<int>) null;
      IEnumerable<int> workItemIds = source.Where<int>((System.Func<int, bool>) (x => x > 0));
      IDictionary<int, string> workItemStates = this.GetWorkItemStates(workItemIds);
      ISet<string> levelActiveStates = this.getReorderLevelActiveStates(backlogContext);
      HashSet<int> intSet = new HashSet<int>();
      foreach (int key in workItemIds)
      {
        if (workItemStates.ContainsKey(key) && !levelActiveStates.Contains(workItemStates[key]))
          intSet.Add(key);
      }
      reorderContext.HasNonActiveAncestor = true;
      return (ISet<int>) intSet;
    }

    private void PerformReorder(
      ProductBacklogReorderHelper.ReorderContext reorderContext)
    {
      using (this.m_requestContext.TraceBlock(290310, 290311, "Agile", TfsTraceLayers.BusinessLogic, nameof (PerformReorder)))
      {
        ReorderOperation operation = reorderContext.Operation;
        int? nullable = operation.PreviousId;
        int num1 = 0;
        bool addedAtTop = nullable.GetValueOrDefault() <= num1 & nullable.HasValue;
        nullable = operation.NextId;
        int num2 = 0;
        bool addedAtBottom = nullable.GetValueOrDefault() <= num2 & nullable.HasValue;
        double num3 = !addedAtTop ? reorderContext.PreviousItem.OrderBy : 0.0;
        double nextOrderByValue = !addedAtBottom ? reorderContext.NextItem.OrderBy : Math.Max(2000000000.0, num3);
        IEnumerable<double> newOrderByValues = ProductBacklogReorderHelper.CalculateNewOrderByValues(num3, nextOrderByValue, reorderContext.ReorderItems.Count<ProductBacklogReorderHelper.WorkItemData>(), addedAtTop, addedAtBottom);
        if (newOrderByValues.FirstOrDefault<double>() <= num3 || newOrderByValues.LastOrDefault<double>() >= nextOrderByValue)
        {
          if (!string.IsNullOrWhiteSpace(this.m_iterationPath))
          {
            nullable = operation.ParentId;
            int num4 = 0;
            if (!(nullable.GetValueOrDefault() > num4 & nullable.HasValue) && !reorderContext.NeedsSparsification)
            {
              reorderContext.NeedsSparsification = true;
              this.ProcessReorderOperation(reorderContext);
              return;
            }
          }
          this.RenumberWorkItems(reorderContext.WorkItems, reorderContext.PreviousItem, reorderContext.ReorderItems);
        }
        else
        {
          for (int index = 0; index < reorderContext.ReorderItems.Count<ProductBacklogReorderHelper.WorkItemData>(); ++index)
            this.AddWorkItemUpdate(reorderContext.ReorderItems.ElementAt<ProductBacklogReorderHelper.WorkItemData>(index), newOrderByValues.ElementAt<double>(index));
          this.m_BmlData.Add(CustomerIntelligenceProperty.Action, "ReorderBacklog");
        }
      }
    }

    private static IEnumerable<double> CalculateNewOrderByValues(
      double previousOrderByValue,
      double nextOrderByValue,
      int numOfItems,
      bool addedAtTop,
      bool addedAtBottom)
    {
      List<double> newOrderByValues = new List<double>();
      if (addedAtTop && !addedAtBottom)
      {
        double num = nextOrderByValue;
        for (int index = 0; index < numOfItems; ++index)
        {
          num -= (double) (int) Math.Sqrt(num - previousOrderByValue);
          newOrderByValues.Add(num);
        }
        newOrderByValues.Reverse();
      }
      else if (addedAtBottom && !addedAtTop)
      {
        double num = previousOrderByValue;
        for (int index = 0; index < numOfItems; ++index)
        {
          num += (double) (int) Math.Sqrt(nextOrderByValue - num);
          newOrderByValues.Add(num);
        }
      }
      else
      {
        double num1 = (nextOrderByValue - previousOrderByValue) / (double) (numOfItems + 1);
        double num2 = previousOrderByValue;
        for (int index = 0; index < numOfItems; ++index)
        {
          num2 = Math.Round(num2 + num1);
          newOrderByValues.Add(num2);
        }
      }
      return (IEnumerable<double>) newOrderByValues;
    }

    private void RenumberWorkItems(
      List<ProductBacklogReorderHelper.WorkItemData> workItemsData,
      ProductBacklogReorderHelper.WorkItemData previousItem,
      IEnumerable<ProductBacklogReorderHelper.WorkItemData> reorderItems)
    {
      using (this.m_requestContext.TraceBlock(290312, 290313, "Agile", TfsTraceLayers.BusinessLogic, nameof (RenumberWorkItems)))
      {
        workItemsData = workItemsData.Except<ProductBacklogReorderHelper.WorkItemData>(reorderItems).ToList<ProductBacklogReorderHelper.WorkItemData>();
        int num1 = previousItem == null ? -1 : workItemsData.FindIndex((Predicate<ProductBacklogReorderHelper.WorkItemData>) (x => x.Id == previousItem.Id));
        int num2 = num1 == -1 ? 0 : num1 + 1;
        workItemsData.InsertRange(num2, reorderItems);
        int startIndex;
        int endIndex;
        double spaceBetweenItems;
        this.FindRenumberRange(num2, reorderItems.Count<ProductBacklogReorderHelper.WorkItemData>(), workItemsData, out startIndex, out endIndex, out spaceBetweenItems);
        if (startIndex == 0)
          this.AddWorkItemUpdate(workItemsData[startIndex], 0.0 + spaceBetweenItems);
        if (endIndex == workItemsData.Count - 1)
          this.AddWorkItemUpdate(workItemsData[endIndex], 2000000000.0 - spaceBetweenItems);
        double orderByValue = (startIndex == 0 ? 0.0 + spaceBetweenItems : Math.Ceiling(workItemsData[startIndex].OrderBy)) + spaceBetweenItems;
        int index = startIndex + 1;
        while (index < endIndex)
        {
          this.AddWorkItemUpdate(workItemsData[index], orderByValue);
          ++index;
          orderByValue += spaceBetweenItems;
        }
        this.m_BmlData.Add(CustomerIntelligenceProperty.Action, "SparsifyBacklog");
        this.m_BmlData.Add(CustomerIntelligenceProperty.ItemsModified, (double) this.m_workItemUpdates.Count);
        this.m_BmlData.Add(CustomerIntelligenceProperty.NewDelta, spaceBetweenItems);
      }
    }

    private void FindRenumberRange(
      int insertIndex,
      int itemsInserted,
      List<ProductBacklogReorderHelper.WorkItemData> workItemsData,
      out int startIndex,
      out int endIndex,
      out double spaceBetweenItems)
    {
      int num1 = workItemsData.Count - 1;
      startIndex = insertIndex;
      endIndex = insertIndex + itemsInserted - 1;
      spaceBetweenItems = 0.0;
      int num2;
      int gaps;
      do
      {
        startIndex = startIndex == 0 ? startIndex : startIndex - 1;
        endIndex = endIndex == num1 ? endIndex : endIndex + 1;
        if (workItemsData[startIndex].OrderBy < 0.0 || workItemsData[startIndex].OrderBy > 2000000000.0 || workItemsData[endIndex].OrderBy < 0.0 || workItemsData[endIndex].OrderBy > 2000000000.0)
        {
          startIndex = 0;
          endIndex = num1;
        }
        double num3 = startIndex == 0 ? 0.0 : workItemsData[startIndex].OrderBy;
        double num4 = endIndex == num1 ? 2000000000.0 : workItemsData[endIndex].OrderBy;
        int num5 = 0 + (startIndex == 0 ? 1 : 0) + (endIndex == num1 ? 1 : 0);
        num2 = endIndex - startIndex;
        gaps = num2 + num5;
        spaceBetweenItems = Math.Floor((num4 - num3) / (double) gaps);
      }
      while (num2 + 1 != workItemsData.Count && !this.SufficientSpaceBetweenItems(spaceBetweenItems, gaps, workItemsData.Count));
    }

    private int GetImmediateAncestor(int item, int targetParent, Dictionary<int, int> parentMap)
    {
      int num;
      for (int key = item; parentMap.TryGetValue(key, out num); key = num)
      {
        if (num == targetParent)
          return key;
      }
      return item;
    }

    private void ValidateWorkItemExistence(
      ReorderOperation operation,
      IDictionary<int, PagedWorkItemData> pagedData)
    {
      using (this.m_requestContext.TraceBlock(290320, 290321, "Agile", TfsTraceLayers.BusinessLogic, nameof (ValidateWorkItemExistence)))
      {
        ArgumentUtility.CheckForNull<ReorderOperation>(operation, nameof (operation));
        int? nullable = operation.PreviousId;
        int valueOrDefault1 = nullable.GetValueOrDefault();
        nullable = operation.NextId;
        int valueOrDefault2 = nullable.GetValueOrDefault();
        if (valueOrDefault1 > 0 && !pagedData.ContainsKey(valueOrDefault1))
          operation.PreviousId = new int?();
        if (valueOrDefault2 > 0 && !pagedData.ContainsKey(valueOrDefault2))
          operation.NextId = new int?();
        if (!operation.PreviousId.HasValue && !operation.NextId.HasValue || ((IEnumerable<int>) operation.Ids).Any<int>((System.Func<int, bool>) (x => !pagedData.ContainsKey(x))))
          throw new BacklogChangedException();
      }
    }

    private void ValidateWorkItemTypes(
      ReorderOperation operation,
      BacklogContext backlogContext,
      IDictionary<int, PagedWorkItemData> pagedData)
    {
      using (this.m_requestContext.TraceBlock(290322, 290323, "Agile", TfsTraceLayers.BusinessLogic, nameof (ValidateWorkItemTypes)))
      {
        ArgumentUtility.CheckForNull<ReorderOperation>(operation, nameof (operation));
        int? nullable1 = operation.PreviousId;
        int valueOrDefault1 = nullable1.GetValueOrDefault();
        nullable1 = operation.NextId;
        int valueOrDefault2 = nullable1.GetValueOrDefault();
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration levelConfiguration = backlogContext.CurrentLevelConfiguration;
        HashSet<string> reorderLevelTypes = new HashSet<string>((IEnumerable<string>) levelConfiguration.WorkItemTypes, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
        if ((!operation.PreviousId.HasValue || pagedData.ContainsKey(valueOrDefault1) && !reorderLevelTypes.Contains(pagedData[valueOrDefault1].Type)) && (!operation.NextId.HasValue || pagedData.ContainsKey(valueOrDefault2) && !reorderLevelTypes.Contains(pagedData[valueOrDefault2].Type)) || ((IEnumerable<int>) operation.Ids).Any<int>((System.Func<int, bool>) (x => !reorderLevelTypes.Contains(pagedData[x].Type))))
          throw new InvalidReorderOperationException(Microsoft.TeamFoundation.Agile.Server.AgileResources.ProductBacklogReorderHelper_InvalidTypes);
        int? nullable2;
        if (operation.ParentId.HasValue)
        {
          IDictionary<int, PagedWorkItemData> dictionary1 = pagedData;
          nullable2 = operation.ParentId;
          int valueOrDefault3 = nullable2.GetValueOrDefault();
          if (dictionary1.ContainsKey(valueOrDefault3))
          {
            if (levelConfiguration.IsRequirementsBacklog)
            {
              IReadOnlyCollection<string> workItemTypes = this.m_settings.BacklogConfiguration.RequirementBacklog.WorkItemTypes;
              IDictionary<int, PagedWorkItemData> dictionary2 = pagedData;
              nullable2 = operation.ParentId;
              int valueOrDefault4 = nullable2.GetValueOrDefault();
              string type = dictionary2[valueOrDefault4].Type;
              VssStringComparer workItemTypeName = TFStringComparer.WorkItemTypeName;
              if (workItemTypes.Contains<string>(type, (IEqualityComparer<string>) workItemTypeName))
                goto label_9;
            }
            if (levelConfiguration.IsTaskBacklog || !this.CanPerformReorderOperationUsingOnlyActiveWorkItems(operation, backlogContext, pagedData))
              goto label_9;
          }
          ReorderOperation reorderOperation = operation;
          nullable2 = new int?();
          int? nullable3 = nullable2;
          reorderOperation.ParentId = nullable3;
        }
label_9:
        if (valueOrDefault1 > 0 && !reorderLevelTypes.Contains(pagedData[valueOrDefault1].Type))
        {
          ReorderOperation reorderOperation = operation;
          nullable2 = new int?();
          int? nullable4 = nullable2;
          reorderOperation.PreviousId = nullable4;
        }
        if (valueOrDefault2 <= 0 || reorderLevelTypes.Contains(pagedData[valueOrDefault2].Type))
          return;
        ReorderOperation reorderOperation1 = operation;
        nullable2 = new int?();
        int? nullable5 = nullable2;
        reorderOperation1.NextId = nullable5;
      }
    }

    private bool CanPerformReorderOperationUsingOnlyActiveWorkItems(
      ReorderOperation operation,
      BacklogContext backlogContext,
      IDictionary<int, PagedWorkItemData> pagedData)
    {
      ISet<string> levelActiveStates = this.getReorderLevelActiveStates(backlogContext);
      foreach (int id in operation.Ids)
      {
        if (pagedData.ContainsKey(id) && !levelActiveStates.Contains(pagedData[id].State))
          return false;
      }
      int? nullable = operation.PreviousId;
      int valueOrDefault1 = nullable.GetValueOrDefault();
      nullable = operation.NextId;
      int valueOrDefault2 = nullable.GetValueOrDefault();
      int num = valueOrDefault1 <= 0 || !pagedData.ContainsKey(valueOrDefault1) ? 0 : (levelActiveStates.Contains(pagedData[valueOrDefault1].State) ? 1 : 0);
      bool flag = valueOrDefault2 > 0 && pagedData.ContainsKey(valueOrDefault2) && levelActiveStates.Contains(pagedData[valueOrDefault2].State);
      if (num == 0 && !flag)
        return false;
      nullable = operation.ParentId;
      int valueOrDefault3 = nullable.GetValueOrDefault();
      return valueOrDefault3 <= 0 || !pagedData.ContainsKey(valueOrDefault3) || new HashSet<string>((IEnumerable<string>) this.m_settings.BacklogConfiguration.GetWorkItemStates(this.m_settings.BacklogConfiguration.GetBacklogByWorkItemTypeName(pagedData[valueOrDefault3].Type), ProductBacklogReorderHelper.ActiveStates), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName).Contains(pagedData[valueOrDefault3].State);
    }

    private ISet<string> getReorderLevelActiveStates(BacklogContext backlogContext) => (ISet<string>) new HashSet<string>((IEnumerable<string>) this.m_settings.BacklogConfiguration.GetWorkItemStates(backlogContext.CurrentLevelConfiguration, ProductBacklogReorderHelper.ActiveStates), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);

    private BacklogContext GetBacklogContext(
      Team team,
      ReorderOperation operation,
      IDictionary<int, PagedWorkItemData> pagedData)
    {
      using (this.m_requestContext.TraceBlock(290316, 290317, "Agile", TfsTraceLayers.BusinessLogic, nameof (GetBacklogContext)))
      {
        ArgumentUtility.CheckForNull<ReorderOperation>(operation, nameof (operation));
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration backlogLevel = (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration) null;
        if (!this.m_settings.BacklogConfiguration.TryGetBacklogByWorkItemTypeName(pagedData[((IEnumerable<int>) operation.Ids).First<int>()].Type, out backlogLevel))
          backlogLevel = this.m_settings.BacklogConfiguration.RequirementBacklog;
        return new BacklogContext(team, backlogLevel);
      }
    }

    private void AddWorkItemUpdate(
      ProductBacklogReorderHelper.WorkItemData workItem,
      double orderByValue)
    {
      ArgumentUtility.CheckForNull<ProductBacklogReorderHelper.WorkItemData>(workItem, nameof (workItem));
      this.m_workItemUpdates.Add(new WorkItemUpdate()
      {
        Id = workItem.Id,
        Rev = workItem.Revision,
        Fields = (IEnumerable<KeyValuePair<string, object>>) new Dictionary<string, object>()
        {
          {
            this.OrderByFieldReferenceName,
            (object) orderByValue
          }
        }
      });
    }

    private bool SufficientSpaceBetweenItems(double delta, int gaps, int backlogSize)
    {
      double val2 = 2000000000.0 / (double) backlogSize;
      return delta >= Math.Min(128.0, val2);
    }

    protected virtual string GetQuery(
      ProductBacklogReorderHelper.ReorderContext reorderContext,
      BacklogContext backlogContext,
      IEnumerable<string> fields)
    {
      ReorderOperation operation = reorderContext.Operation;
      if (reorderContext.NeedsSparsification)
        return this.GetQueryForSparsification(backlogContext, fields, reorderContext.GetReorderOperationIds(), operation.ParentId.GetValueOrDefault(), this.m_iterationPath);
      return string.IsNullOrWhiteSpace(this.m_iterationPath) ? this.GetQueryForProductBacklog(backlogContext, fields, operation.ParentId.GetValueOrDefault()) : this.GetQueryForIterationBacklog(backlogContext, fields, operation.ParentId.GetValueOrDefault(), this.m_iterationPath);
    }

    private string GetQueryForSparsification(
      BacklogContext backlogContext,
      IEnumerable<string> fields,
      IEnumerable<int> ids,
      int parentId,
      string iterationPath)
    {
      ProductBacklogQueryBuilder backlogQueryBuilder = new ProductBacklogQueryBuilder(this.m_requestContext, this.m_settings, backlogContext, this.m_productBacklogIterationPath);
      backlogQueryBuilder.Fields = fields;
      backlogQueryBuilder.ParentId = parentId;
      WorkItemStateCategory[] stateCategories = ProductBacklogReorderHelper.ActiveStates;
      if (iterationPath != null || parentId > 0)
        stateCategories = ProductBacklogReorderHelper.ActiveAndCompleteStates;
      return backlogQueryBuilder.GetBacklogReorderSparsificationQuery(stateCategories, iterationPath, ids);
    }

    private string GetQueryForProductBacklog(
      BacklogContext backlogContext,
      IEnumerable<string> fields,
      int parentId)
    {
      ProductBacklogQueryBuilder backlogQueryBuilder = new ProductBacklogQueryBuilder(this.m_requestContext, this.m_settings, backlogContext, this.m_productBacklogIterationPath);
      backlogQueryBuilder.Fields = fields;
      backlogQueryBuilder.ParentId = parentId;
      WorkItemStateCategory[] itemStateCategoryArray = ProductBacklogReorderHelper.ActiveStates;
      if (parentId > 0)
        itemStateCategoryArray = ProductBacklogReorderHelper.ActiveAndCompleteStates;
      return backlogQueryBuilder.GetBacklogQueryFilteredParent(itemStateCategoryArray);
    }

    private string GetQueryForIterationBacklog(
      BacklogContext backlogContext,
      IEnumerable<string> fields,
      int parentId,
      string iterationPath)
    {
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration levelConfiguration = backlogContext.CurrentLevelConfiguration;
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration> sourceConfigurations = new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration>();
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration> targetConfigurations = new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration>();
      if (levelConfiguration.IsRequirementsBacklog || levelConfiguration.IsTaskBacklog)
      {
        sourceConfigurations.Add(this.m_settings.BacklogConfiguration.RequirementBacklog);
        targetConfigurations.Add(this.m_settings.BacklogConfiguration.RequirementBacklog);
        targetConfigurations.Add(this.m_settings.BacklogConfiguration.TaskBacklog);
      }
      else
      {
        sourceConfigurations.Add(levelConfiguration);
        targetConfigurations.Add(levelConfiguration);
      }
      WorkItemStateCategory[] andCompleteStates = ProductBacklogReorderHelper.ActiveAndCompleteStates;
      IterationQueryBuilder iterationQueryBuilder = new IterationQueryBuilder(this.m_requestContext, this.m_settings, (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration>) sourceConfigurations, (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration>) targetConfigurations, andCompleteStates, fields, iterationPath, this.m_productBacklogIterationPath);
      iterationQueryBuilder.ParentId = parentId;
      return iterationQueryBuilder.GetQuery();
    }

    protected virtual ICollection<ReorderResult> SaveWorkItems(
      ProductBacklogReorderHelper.ReorderContext reorderContext)
    {
      using (this.m_requestContext.TraceBlock(290314, 290315, "Agile", TfsTraceLayers.BusinessLogic, nameof (SaveWorkItems)))
      {
        for (IEnumerable<WorkItemUpdate> source = (IEnumerable<WorkItemUpdate>) this.m_workItemUpdates; source.Any<WorkItemUpdate>(); source = source.Skip<WorkItemUpdate>(200))
        {
          IEnumerable<WorkItemUpdateResult> itemUpdateResults = this.m_workItemService.SaveWorkItems(this.m_requestContext, source.Take<WorkItemUpdate>(200), true, true, reorderContext.Operation.Ids);
          TeamFoundationServiceException serviceException = (TeamFoundationServiceException) null;
          foreach (WorkItemUpdateResult itemUpdateResult in itemUpdateResults)
          {
            if (itemUpdateResult.Exception != null)
            {
              if (itemUpdateResult.Exception is WorkItemsBatchSaveFailedException)
              {
                serviceException = itemUpdateResult.Exception;
              }
              else
              {
                if (itemUpdateResult.Exception is WorkItemTrackingAggregateException exception)
                  throw exception.LeadingException;
                throw itemUpdateResult.Exception;
              }
            }
          }
          if (serviceException != null)
            throw serviceException;
        }
        List<ReorderResult> reorderResultList = new List<ReorderResult>();
        foreach (WorkItemUpdate workItemUpdate in this.m_workItemUpdates)
        {
          KeyValuePair<string, object> keyValuePair = workItemUpdate.Fields.FirstOrDefault<KeyValuePair<string, object>>((System.Func<KeyValuePair<string, object>, bool>) (kvp => StringComparer.OrdinalIgnoreCase.Equals(kvp.Key, this.OrderByFieldReferenceName)));
          double num = -1.0;
          if (keyValuePair.Value != null)
            num = Convert.ToDouble(keyValuePair.Value);
          reorderResultList.Add(new ReorderResult()
          {
            Id = workItemUpdate.Id,
            Order = num
          });
        }
        this.m_workItemUpdates.Clear();
        return (ICollection<ReorderResult>) reorderResultList;
      }
    }

    protected virtual IEnumerable<LinkQueryResultEntry> RunQuery(string wiql)
    {
      using (this.m_requestContext.TraceBlock(290308, 290309, "Agile", TfsTraceLayers.BusinessLogic, nameof (RunQuery)))
        return BacklogQueryExecutionHelper.OptimizeAndExecuteQuery(this.m_requestContext, wiql, true, queryContext: this.m_queryContext, applicationIntentOverride: WITQueryApplicationIntentOverride.ReadWrite);
    }

    protected virtual IDictionary<int, IDataRecord> GetWorkItems(IEnumerable<int> workItemIds) => (IDictionary<int, IDataRecord>) this.GetWorkItems(this.m_workItemService, this.m_requestContext, workItemIds, (IEnumerable<string>) this.QueryFields).ToDictionary<IDataRecord, int>((System.Func<IDataRecord, int>) (workItem => (int) workItem["System.Id"]));

    protected virtual IEnumerable<IDataRecord> GetWorkItems(
      WebAccessWorkItemService service,
      IVssRequestContext requestContext,
      IEnumerable<int> workitemIDs,
      IEnumerable<string> fieldIDs,
      bool returnIdentityRef = false)
    {
      ArgumentUtility.CheckForNull<WebAccessWorkItemService>(service, nameof (service));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workitemIDs, nameof (workitemIDs));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(fieldIDs, nameof (fieldIDs));
      int witMaxPageSize = 200;
      for (; workitemIDs.Any<int>(); workitemIDs = workitemIDs.Skip<int>(witMaxPageSize))
      {
        IEnumerable<int> ints = workitemIDs.Take<int>(witMaxPageSize);
        WebAccessWorkItemService accessWorkItemService = service;
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<int> ids = ints;
        IEnumerable<string> fields = fieldIDs;
        bool flag = returnIdentityRef;
        DateTime? asOf = new DateTime?();
        int num = flag ? 1 : 0;
        foreach (IDataRecord pageWorkItem in accessWorkItemService.PageWorkItems(requestContext1, ids, fields, asOf, returnIdentityRef: num != 0))
          yield return pageWorkItem;
      }
    }

    protected virtual IDictionary<int, PagedWorkItemData> PageBasicWorkItemData(
      IEnumerable<int> workItemIds)
    {
      return (IDictionary<int, PagedWorkItemData>) this.m_workItemService.PageWorkItems(this.m_requestContext, workItemIds.Where<int>((System.Func<int, bool>) (x => x > 0)), (IEnumerable<string>) new string[4]
      {
        "System.Id",
        "System.WorkItemType",
        "System.State",
        "System.IterationPath"
      }).ToDictionary<IDataRecord, int, PagedWorkItemData>((System.Func<IDataRecord, int>) (x => (int) x["System.Id"]), (System.Func<IDataRecord, PagedWorkItemData>) (x => new PagedWorkItemData((int) x["System.Id"], (string) x["System.WorkItemType"], (string) x["System.State"], (string) x["System.IterationPath"])));
    }

    protected virtual IDictionary<int, string> GetWorkItemStates(IEnumerable<int> workItemIds) => (IDictionary<int, string>) this.m_workItemService.PageWorkItems(this.m_requestContext, workItemIds.Where<int>((System.Func<int, bool>) (x => x > 0)), (IEnumerable<string>) new string[2]
    {
      "System.Id",
      "System.State"
    }).ToDictionary<IDataRecord, int, string>((System.Func<IDataRecord, int>) (x => (int) x["System.Id"]), (System.Func<IDataRecord, string>) (x => (string) x["System.State"]));

    protected class WorkItemData
    {
      private IDataRecord m_record;
      private string m_orderByFieldName;

      public WorkItemData(IDataRecord dataRecord, string orderByFieldName, int index)
      {
        ArgumentUtility.CheckForNull<IDataRecord>(dataRecord, nameof (dataRecord));
        ArgumentUtility.CheckStringForNullOrEmpty(orderByFieldName, nameof (orderByFieldName));
        this.m_record = dataRecord;
        this.m_orderByFieldName = orderByFieldName;
        this.Index = index;
      }

      public int Index { get; set; }

      public int Id => (int) this.m_record["System.Id"];

      public double OrderBy => this.m_record[this.m_orderByFieldName] == null ? 2000000000.0 : Convert.ToDouble(this.m_record[this.m_orderByFieldName], (IFormatProvider) CultureInfo.InvariantCulture);

      public int Revision => (int) this.m_record["System.Rev"];
    }

    protected class ReorderContext
    {
      public ReorderOperation Operation { get; }

      public ProductBacklogReorderHelper.WorkItemData PreviousItem { get; set; }

      public ProductBacklogReorderHelper.WorkItemData NextItem { get; set; }

      public IEnumerable<ProductBacklogReorderHelper.WorkItemData> ReorderItems { get; set; }

      public List<ProductBacklogReorderHelper.WorkItemData> WorkItems { get; set; }

      public bool HasNonActiveAncestor { get; set; }

      public bool NeedsSparsification { get; set; }

      public bool IsNestedWorkItem { get; set; }

      public ReorderContext(ReorderOperation operation)
      {
        ArgumentUtility.CheckForNull<ReorderOperation>(operation, nameof (operation));
        this.Operation = operation;
      }

      public IEnumerable<int> GetReorderOperationIds()
      {
        if (this.Operation == null)
          return Enumerable.Empty<int>();
        List<int> list = ((IEnumerable<int>) this.Operation.Ids).ToList<int>();
        list.Add(this.Operation.ParentId.GetValueOrDefault());
        list.Add(this.Operation.PreviousId.GetValueOrDefault());
        list.Add(this.Operation.NextId.GetValueOrDefault());
        return (IEnumerable<int>) list;
      }
    }
  }
}
