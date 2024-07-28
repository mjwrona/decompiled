// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.TaskBoardData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Extensions;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.Results;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  internal sealed class TaskBoardData
  {
    private static int UnparentedRowId = -1;
    private string m_taskboardQueryWiql;
    private IVssRequestContext m_requestContext;
    private IEnumerable<string> m_queryFields;
    private object m_payloadData;
    private string m_backlogIterationPath;
    private IEnumerable<string> m_taskWorkItemTypes;

    public TaskBoardData(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      WebAccessWorkItemService service,
      IEnumerable<string> requirementWitTypes,
      IEnumerable<string> states,
      IEnumerable<string> fields,
      string iterationPath,
      string backlogIterationPath,
      bool enforceDataLimit = true,
      IDictionary queryContext = null,
      bool returnIdentityRef = false)
    {
      TaskBoardData taskBoardData = this;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForNull<WebAccessWorkItemService>(service, nameof (service));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(requirementWitTypes, nameof (requirementWitTypes));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(states, nameof (states));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(fields, nameof (fields));
      ArgumentUtility.CheckStringForNullOrEmpty(iterationPath, nameof (iterationPath));
      ArgumentUtility.CheckStringForNullOrEmpty(backlogIterationPath, nameof (backlogIterationPath));
      requestContext.TraceBlock(290915, 290916, "Agile", nameof (TaskBoardData), nameof (TaskBoardData), (Action) (() =>
      {
        taskBoardData.States = states;
        taskBoardData.IterationPath = iterationPath;
        taskBoardData.m_requestContext = requestContext;
        taskBoardData.m_queryFields = fields;
        taskBoardData.m_backlogIterationPath = backlogIterationPath;
        taskBoardData.HasNestedTasks = false;
        taskBoardData.ReorderBlockingWorkItemIds = (ISet<int>) new HashSet<int>();
        taskBoardData.MissingWorkItemIds = (ISet<int>) new HashSet<int>();
        taskBoardData.m_taskboardQueryWiql = taskBoardData.m_requestContext.GetService<IIterationBacklogService>().GetTaskBoardQuery(taskBoardData.m_requestContext, settings, iterationPath, fields);
        IEnumerable<LinkQueryResultEntry> links = (IEnumerable<LinkQueryResultEntry>) null;
        taskBoardData.GetQueryResults(requestContext, service, settings, queryContext, out links);
        if (enforceDataLimit)
        {
          int workItemCountLimit = settings.Process.TaskBacklog.WorkItemCountLimit;
          if (workItemCountLimit != -1 && links.Skip<LinkQueryResultEntry>(workItemCountLimit).Any<LinkQueryResultEntry>())
            throw new BoardDataLimitReachedException(workItemCountLimit, links.Count<LinkQueryResultEntry>());
        }
        IEnumerable<IDataRecord> workItems = taskBoardData.GetWorkItems(requestContext, service, returnIdentityRef, links);
        taskBoardData.m_taskWorkItemTypes = (IEnumerable<string>) settings.BacklogConfiguration.TaskBacklog.WorkItemTypes;
        taskBoardData.SetupTaskboardItems(requestContext, links, workItems, requirementWitTypes);
      }));
    }

    public static TaskBoardData GetTaskBoardData(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      IEnumerable<string> fields,
      string iterationPath,
      bool enforceDataLimit,
      bool returnIdentityRef = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForNull<string>(iterationPath, nameof (iterationPath));
      return requestContext.TraceBlock<TaskBoardData>(290017, 290018, "Agile", nameof (TaskBoardData), nameof (GetTaskBoardData), (Func<TaskBoardData>) (() =>
      {
        WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
        IEnumerable<string> strings = settings.GetDefaultBacklogFields(true);
        if (fields != null)
          strings = fields.Union<string>(strings, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        BacklogLevelConfiguration taskBacklog = settings.BacklogConfiguration.TaskBacklog;
        BacklogLevelConfiguration requirementBacklog = settings.BacklogConfiguration.RequirementBacklog;
        IEnumerable<string> workItemStates = (IEnumerable<string>) settings.BacklogConfiguration.GetWorkItemStates(taskBacklog, new WorkItemStateCategory[4]
        {
          WorkItemStateCategory.Proposed,
          WorkItemStateCategory.InProgress,
          WorkItemStateCategory.Resolved,
          WorkItemStateCategory.Completed
        });
        IEnumerable<string> workItemTypes = (IEnumerable<string>) requirementBacklog.WorkItemTypes;
        string path = settings.TeamSettings.GetBacklogIterationNode(requestContext).GetPath(requestContext);
        IDictionary defaultQueryContext = requestContext.GetDefaultQueryContext(settings.ProjectName);
        TaskBoardData taskBoardData = new TaskBoardData(requestContext, settings, service, workItemTypes, workItemStates, strings, iterationPath, path, enforceDataLimit, defaultQueryContext, returnIdentityRef);
        TaskBoardData.PublishTaskBoardDataTelemetry(requestContext, settings, taskBoardData);
        return taskBoardData;
      }));
    }

    private static int AggregateCounts(
      IVssRequestContext requestContext,
      string projectName,
      CategoryConfiguration categoryConfiguration,
      IDictionary<string, int> counts)
    {
      int count;
      return categoryConfiguration == null || counts == null ? 0 : categoryConfiguration.GetWorkItemTypes(requestContext, projectName).Sum<string>((System.Func<string, int>) (typeName =>
      {
        counts.TryGetValue(typeName, out count);
        return count;
      }));
    }

    private static void PublishTaskBoardDataTelemetry(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      TaskBoardData taskBoardData)
    {
      try
      {
        int num1 = TaskBoardData.AggregateCounts(requestContext, settings.ProjectName, (CategoryConfiguration) settings.Process.TaskBacklog, taskBoardData.OrphanItemCounts);
        int num2 = TaskBoardData.AggregateCounts(requestContext, settings.ProjectName, settings.Process.BugWorkItems, taskBoardData.OrphanItemCounts);
        int num3 = TaskBoardData.AggregateCounts(requestContext, settings.ProjectName, (CategoryConfiguration) settings.Process.TaskBacklog, taskBoardData.ItemCounts);
        int num4 = TaskBoardData.AggregateCounts(requestContext, settings.ProjectName, settings.Process.BugWorkItems, taskBoardData.ItemCounts);
        int num5 = TaskBoardData.AggregateCounts(requestContext, settings.ProjectName, (CategoryConfiguration) settings.Process.RequirementBacklog, taskBoardData.ItemCounts);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("TeamId", settings.Team.Id.ToString());
        properties.Add("BugBehavior", settings.TeamSettings.BugsBehavior.ToString());
        properties.Add(AgileCustomerIntelligencePropertyName.TeamFieldValuesCount, (double) settings.TeamSettings.TeamFieldConfig.TeamFieldValues.Length);
        properties.Add("OrphanTasks", (double) num1);
        properties.Add("OrphanBugs", (double) num2);
        properties.Add("TotalTasks", (double) num3);
        properties.Add("TotalBugs", (double) num4);
        properties.Add("TotalStories", (double) num5);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Agile, AgileCustomerIntelligenceFeature.TaskBoardBugUsageFeature, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(599999, "Agile", TfsTraceLayers.BusinessLogic, ex);
      }
    }

    public IReadOnlyCollection<int> ParentIDs { get; private set; }

    public object PayloadData
    {
      get
      {
        if (this.m_payloadData == null)
        {
          List<string> fieldRefNames = this.m_queryFields.ToList<string>();
          fieldRefNames.Add("ParentId");
          Dictionary<string, List<object>> dictionary = this.WorkItemData.ToDictionary<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData, string, List<object>>((System.Func<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData, string>) (item => item.ID.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (System.Func<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData, List<object>>) (item => fieldRefNames.Select<string, object>((System.Func<string, object>) (name => item.GetFieldValue(name))).ToList<object>()));
          var data = new
          {
            fieldRefNames = fieldRefNames,
            data = dictionary
          };
          this.m_payloadData = (object) data;
        }
        return this.m_payloadData;
      }
    }

    public IList<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData> WorkItemData { get; internal set; }

    public bool IsEmpty => this.WorkItemData == null || this.WorkItemData.Count == 0;

    public IEnumerable<string> Fields => this.m_queryFields;

    public IEnumerable<string> States { get; private set; }

    public string IterationPath { get; private set; }

    public bool HasNestedTasks { get; private set; }

    public ISet<int> ReorderBlockingWorkItemIds { get; private set; }

    public ISet<int> MissingWorkItemIds { get; private set; }

    internal IDictionary<string, int> OrphanItemCounts { get; private set; }

    internal IDictionary<string, int> ItemCounts { get; private set; }

    public ProductBacklogQueryResults CreateQueryResultModel(
      IVssRequestContext requestContext,
      IDictionary<string, int> fieldWidthMap,
      ProductBacklogGridOptions options,
      IDictionary queryContext = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary<string, int>>(fieldWidthMap, nameof (fieldWidthMap));
      ArgumentUtility.CheckForNull<ProductBacklogGridOptions>(options, nameof (options));
      ProductBacklogQueryResults queryModel = new ProductBacklogQueryResults(requestContext, this.m_taskboardQueryWiql, fieldWidthMap, options, false, queryContext);
      queryModel.TargetIds = (IEnumerable<int>) this.WorkItemData.Select<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData, int>((System.Func<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData, int>) (workItem => workItem.ID)).ToArray<int>();
      queryModel.SourceIds = Enumerable.Empty<int>();
      queryModel.LinkIds = Enumerable.Empty<int>();
      queryModel.RealParentIds = Enumerable.Empty<int>();
      queryModel.UpdatePayload(requestContext);
      this.ConstructTreeData(queryModel);
      return queryModel;
    }

    internal IEnumerable<string> GetPeopleFilterValues() => this.WorkItemData == null ? Enumerable.Empty<string>() : (IEnumerable<string>) this.WorkItemData.Where<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>((System.Func<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData, bool>) (wid => this.m_taskWorkItemTypes.Contains<string>((string) wid.GetFieldValue("System.WorkItemType"), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName))).Select<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData, string>((System.Func<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData, string>) (wid =>
    {
      object fieldValue = wid.GetFieldValue("System.AssignedTo");
      if (fieldValue == null)
        return (string) null;
      return fieldValue is WitIdentityRef ? ((WitIdentityRef) fieldValue).DistinctDisplayName : fieldValue as string;
    })).Where<string>((System.Func<string, bool>) (assignedTo => !string.IsNullOrEmpty(assignedTo))).Distinct<string>().ToList<string>();

    private IEnumerable<IDataRecord> GetWorkItems(
      IVssRequestContext requestContext,
      WebAccessWorkItemService service,
      bool returnIdentityRef,
      IEnumerable<LinkQueryResultEntry> links)
    {
      return requestContext.TraceBlock<IEnumerable<IDataRecord>>(290919, 290920, "Agile", nameof (TaskBoardData), nameof (GetWorkItems), (Func<IEnumerable<IDataRecord>>) (() => service.GetWorkItems(requestContext, links.Select<LinkQueryResultEntry, int>((System.Func<LinkQueryResultEntry, int>) (wli => wli.TargetId)), this.m_queryFields, returnIdentityRef)));
    }

    private void GetQueryResults(
      IVssRequestContext requestContext,
      WebAccessWorkItemService service,
      IAgileSettings settings,
      IDictionary queryContext,
      out IEnumerable<LinkQueryResultEntry> links)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebAccessWorkItemService>(service, nameof (service));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      try
      {
        requestContext.TraceEnter(290921, "Agile", nameof (TaskBoardData), nameof (GetQueryResults));
        IIterationBacklogService service1 = requestContext.GetService<IIterationBacklogService>();
        string[] fields = new string[1]
        {
          CoreFieldReferenceNames.Id
        };
        string taskBoardQuery = service1.GetTaskBoardQuery(requestContext, settings, this.IterationPath, (IEnumerable<string>) fields);
        string key = "QueryResultsLinks_" + taskBoardQuery;
        object obj;
        if (!requestContext.Items.TryGetValue(key, out obj))
        {
          links = this.RunQuery(taskBoardQuery, queryContext);
          requestContext.Items[key] = (object) links;
        }
        else
          links = (IEnumerable<LinkQueryResultEntry>) obj;
      }
      finally
      {
        requestContext.TraceLeave(290922, "Agile", nameof (TaskBoardData), nameof (GetQueryResults));
      }
    }

    private void ConstructTreeData(ProductBacklogQueryResults queryModel)
    {
      List<int> intList1 = new List<int>(this.WorkItemData.Count);
      List<int> intList2 = new List<int>(this.WorkItemData.Count);
      List<int> intList3 = new List<int>(this.WorkItemData.Count);
      List<int> intList4 = new List<int>(this.WorkItemData.Count);
      foreach (Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData workItem in (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>) this.WorkItemData)
      {
        intList1.Add(this.IsUnparentedWorkItem(workItem) ? TaskBoardData.UnparentedRowId : workItem.ParentID);
        intList2.Add(workItem.ID);
        intList3.Add(workItem.ParentID == 0 ? 0 : 2);
        intList4.Add(workItem.realParentId);
      }
      if (intList1.Contains(TaskBoardData.UnparentedRowId))
      {
        intList2.Insert(0, TaskBoardData.UnparentedRowId);
        intList1.Insert(0, 0);
        intList3.Insert(0, 0);
        intList4.Insert(0, 0);
      }
      queryModel.SourceIds = (IEnumerable<int>) intList1;
      queryModel.TargetIds = (IEnumerable<int>) intList2;
      queryModel.LinkIds = (IEnumerable<int>) intList3;
      queryModel.RealParentIds = (IEnumerable<int>) intList4;
    }

    private bool IsUnparentedWorkItem(Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData workItem)
    {
      bool flag = this.m_taskWorkItemTypes.Contains<string>((string) workItem.GetFieldValue("System.WorkItemType"), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      return workItem.ParentID == 0 & flag;
    }

    private IEnumerable<LinkQueryResultEntry> RunQuery(string wiql, IDictionary queryContext = null)
    {
      try
      {
        return BacklogQueryExecutionHelper.OptimizeAndExecuteQuery(this.m_requestContext, wiql, true, queryContext: queryContext);
      }
      catch (SyntaxException ex)
      {
        TeamFoundationTrace.TraceException((Exception) ex);
        return BacklogQueryExecutionHelper.OptimizeAndExecuteQuery(this.m_requestContext, wiql, true, queryContext: queryContext);
      }
    }

    private void SetupTaskboardItems(
      IVssRequestContext requestContext,
      IEnumerable<LinkQueryResultEntry> allLinks,
      IEnumerable<IDataRecord> workitemRecords,
      IEnumerable<string> parentWorkItemTypes)
    {
      requestContext.TraceBlock(290923, 290924, "Agile", nameof (TaskBoardData), nameof (SetupTaskboardItems), (Action) (() =>
      {
        List<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData> workItemDataList1 = new List<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>();
        List<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData> workItemDataList2 = new List<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>();
        IEnumerable<int> parentIds;
        WorkItemNodes workItemNodes = TaskBoardData.SetupWorkItemNodes(allLinks, workitemRecords, parentWorkItemTypes, out parentIds);
        this.ClearItemCounts();
        foreach (KeyValuePair<int, Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData> keyValuePair in (Dictionary<int, Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>) workItemNodes)
        {
          bool flag1 = this.isWorkItemOrphanTask(keyValuePair.Value);
          bool flag2 = this.isWorkItemTaskWithChildren(keyValuePair.Value);
          if (flag1 & flag2)
          {
            workItemDataList2.AddRange((IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>) keyValuePair.Value.Children);
            this.HasNestedTasks = true;
            this.MissingWorkItemIds.Add(keyValuePair.Value.ID);
          }
          else if (flag1)
          {
            workItemDataList1.Add(keyValuePair.Value);
            this.IncrementItemCount(keyValuePair.Value, true);
          }
        }
        HashSet<int> intSet = new HashSet<int>(parentIds);
        foreach (Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData workItemData in workItemDataList2)
        {
          this.ReorderBlockingWorkItemIds.Add(workItemData.ID);
          if (!intSet.Contains(workItemData.ID))
          {
            workItemData.ParentID = 0;
            workItemDataList1.Add(workItemData);
            this.IncrementItemCount(workItemData, true);
          }
        }
        this.ParentIDs = (IReadOnlyCollection<int>) parentIds.Where<int>((System.Func<int, bool>) (id =>
        {
          Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData node = workItemNodes.GetOrCreateNode(id);
          return node.Children.Count == 0 || this.HasImmediateTasks(node);
        })).ToList<int>();
        foreach (int parentId in (IEnumerable<int>) this.ParentIDs)
        {
          Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData node = workItemNodes.GetOrCreateNode(parentId);
          workItemDataList1.Add(node);
          List<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData> list = this.GetChildItems(node).ToList<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>();
          workItemDataList1.AddRange((IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>) list);
          this.IncrementItemCount(node, false);
          foreach (Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData workItemData in list)
            this.IncrementItemCount(workItemData, false);
        }
        this.WorkItemData = (IList<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>) workItemDataList1;
      }));
    }

    private void ClearItemCounts()
    {
      if (this.OrphanItemCounts == null)
        this.OrphanItemCounts = (IDictionary<string, int>) new Dictionary<string, int>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      this.OrphanItemCounts.Clear();
      if (this.ItemCounts == null)
        this.ItemCounts = (IDictionary<string, int>) new Dictionary<string, int>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      this.ItemCounts.Clear();
    }

    private void IncrementItemCount(Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData workItemData, bool isOrphan)
    {
      if (!(workItemData.GetFieldValue("System.WorkItemType") is string fieldValue))
        return;
      int num;
      if (isOrphan)
      {
        this.OrphanItemCounts.TryGetValue(fieldValue, out num);
        this.OrphanItemCounts[fieldValue] = num + 1;
      }
      this.ItemCounts.TryGetValue(fieldValue, out num);
      this.ItemCounts[fieldValue] = num + 1;
    }

    private static WorkItemNodes SetupWorkItemNodes(
      IEnumerable<LinkQueryResultEntry> links,
      IEnumerable<IDataRecord> workitemRecords,
      IEnumerable<string> requirementWitTypes,
      out IEnumerable<int> parentIds)
    {
      WorkItemNodes workItemNodes = new WorkItemNodes(workitemRecords, requirementWitTypes);
      List<int> intList = new List<int>();
      parentIds = (IEnumerable<int>) intList;
      foreach (LinkQueryResultEntry link in links)
      {
        Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData node = workItemNodes.GetOrCreateNode(link.TargetId);
        if (node != null)
        {
          int sourceId = link.SourceId;
          node.realParentId = sourceId;
          if (node.IsParentType)
          {
            node.ParentID = 0;
            intList.Add(link.TargetId);
          }
          else
            node.ParentID = sourceId;
          if (sourceId != 0)
            workItemNodes.GetOrCreateNode(sourceId)?.AddChild(node);
        }
      }
      return workItemNodes;
    }

    private IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData> GetChildItems(
      Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData itemToWalk,
      int topLevelParentId = -1,
      IList<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData> result = null,
      int depth = 0)
    {
      if (result == null)
        result = (IList<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>) new List<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>();
      if (topLevelParentId < 0)
        topLevelParentId = itemToWalk.ID;
      if (depth > 1)
      {
        this.HasNestedTasks = true;
        this.ReorderBlockingWorkItemIds.Add(itemToWalk.ID);
        this.MissingWorkItemIds.Add(itemToWalk.ParentID);
      }
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData> source = itemToWalk.Children.Where<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>((System.Func<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData, bool>) (child => this.IsTask(child)));
      if (this.IsTask(itemToWalk) && source.Count<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>() == 0)
      {
        itemToWalk.ParentID = topLevelParentId;
        result.Add(itemToWalk);
      }
      else
      {
        foreach (Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData itemToWalk1 in source)
          this.GetChildItems(itemToWalk1, topLevelParentId, result, depth + 1);
      }
      return (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>) result;
    }

    private bool isWorkItemOrphanTask(Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData workItem) => workItem.ParentID == 0 && this.IsTask(workItem);

    private bool isWorkItemTaskWithChildren(Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData workItem) => this.IsTask(workItem) && workItem.Children.Count > 0;

    private bool IsTask(Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData workItem) => this.m_taskWorkItemTypes.Contains<string>((string) workItem.GetFieldValue("System.WorkItemType"), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);

    private bool HasImmediateTasks(Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData workItem)
    {
      foreach (Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData child in (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemData>) workItem.Children)
      {
        if (this.IsTask(child))
          return true;
      }
      return false;
    }
  }
}
