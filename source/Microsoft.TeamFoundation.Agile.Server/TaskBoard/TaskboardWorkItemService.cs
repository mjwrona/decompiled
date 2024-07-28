// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemService
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server.TaskBoard
{
  public class TaskboardWorkItemService : ITaskboardWorkItemService, IVssFrameworkService
  {
    private const string c_TaskboardWorkItemArea = "TaskboardWorkItem";
    private const string c_TaskboardTraceLayer = "Service";

    public IReadOnlyCollection<TaskboardWorkItemColumn> GetWorkItemColumns(
      IVssRequestContext context,
      ProjectInfo project,
      WebApiTeam team,
      Dictionary<int, (string wit, string state)> workItemIdToTypeStateMap,
      TaskboardColumns columns)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(project, nameof (project));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<Dictionary<int, (string, string)>>(workItemIdToTypeStateMap, nameof (workItemIdToTypeStateMap));
      using (context.TraceBlock(290951, 290952, "TaskboardWorkItem", "Service", nameof (GetWorkItemColumns)))
      {
        if (!columns.IsValidMapping)
          throw columns.ValidationException;
        if (!columns.IsCustomized)
          throw new TaskboardColumnNotCustomizedException();
        if (workItemIdToTypeStateMap.Keys.Count == 0)
          return (IReadOnlyCollection<TaskboardWorkItemColumn>) new List<TaskboardWorkItemColumn>(0);
        Dictionary<int, (Guid, string)> workItemColumns;
        using (TaskboardComponent component = context.CreateComponent<TaskboardComponent>())
          workItemColumns = component.GetWorkItemColumns(project.Id, team.Id, (IReadOnlyCollection<int>) workItemIdToTypeStateMap.Keys);
        return this.GetMergedData(workItemIdToTypeStateMap, workItemColumns, columns);
      }
    }

    public IReadOnlyCollection<TaskboardWorkItemColumn> GetWorkItemColumns(
      IVssRequestContext context,
      ProjectInfo project,
      WebApiTeam team,
      Guid iterationId)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(project, nameof (project));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForEmptyGuid(iterationId, nameof (iterationId));
      using (context.TraceBlock(290953, 290954, "TaskboardWorkItem", "Service", nameof (GetWorkItemColumns)))
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iterationNode;
        IAgileSettings agileSettings;
        this.CheckIterationNode(context, project, team, iterationId, out iterationNode, out agileSettings);
        List<string> fields = new List<string>()
        {
          "System.Id",
          "System.WorkItemType",
          "System.State"
        };
        string taskBoardQuery = context.GetService<IIterationBacklogService>().GetTaskBoardQuery(context, agileSettings, iterationNode.GetPath(context), (IEnumerable<string>) fields);
        IWorkItemQueryService service1 = context.GetService<IWorkItemQueryService>();
        Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression = service1.ConvertToQueryExpression(context, taskBoardQuery, skipWiqlTextLimitValidation: true);
        QueryResult queryResult = service1.ExecuteQuery(context, queryExpression);
        IEnumerable<int> source = queryResult.WorkItemLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (x => x.TargetId)).ToList<int>().Except<int>(queryResult.WorkItemLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (x => x.SourceId)));
        int count1 = 0;
        int count2 = 200;
        bool flag = true;
        ITeamFoundationWorkItemService service2 = context.GetService<ITeamFoundationWorkItemService>();
        Dictionary<int, (string, string)> workItemIdToTypeStateMap = new Dictionary<int, (string, string)>();
        while (flag)
        {
          List<int> list = source.Skip<int>(count1).Take<int>(count2).ToList<int>();
          IEnumerable<WorkItemFieldData> workItemFieldValues = service2.GetWorkItemFieldValues(context, (IEnumerable<int>) list, (IEnumerable<string>) fields);
          flag = list.Count >= count2;
          count1 = count2;
          foreach (WorkItemFieldData workItemFieldData in workItemFieldValues)
            workItemIdToTypeStateMap[workItemFieldData.Id] = (workItemFieldData.WorkItemType, workItemFieldData.State);
        }
        TaskboardColumns columns = context.GetService<ITaskboardColumnService>().GetColumns(context, project, team);
        return this.GetWorkItemColumns(context, project, team, workItemIdToTypeStateMap, columns);
      }
    }

    public void UpdateWorkItemColumn(
      IVssRequestContext context,
      ProjectInfo project,
      WebApiTeam team,
      Guid iterationId,
      int workItemId,
      string newColumn)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(project, nameof (project));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForEmptyGuid(iterationId, nameof (iterationId));
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(newColumn, nameof (newColumn));
      using (context.TraceBlock(290955, 290956, "TaskboardWorkItem", "Service", nameof (UpdateWorkItemColumn)))
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iterationNode;
        this.CheckIterationNode(context, project, team, iterationId, out iterationNode, out IAgileSettings _);
        WorkItem workItem = context.GetService<ITeamFoundationWorkItemService>().GetWorkItemById(context, workItemId, 32, false, false, false);
        if (workItem.IterationId != iterationNode.Id)
          throw new TaskboardWorkItemInvalidIterationException(workItemId, workItem.GetIterationPath(context), iterationNode.GetPath(context)).Expected(context.ServiceName);
        TaskboardColumns columns = context.GetService<ITaskboardColumnService>().GetColumns(context, project, team);
        if (!columns.IsCustomized)
          throw new TaskboardColumnNotCustomizedException();
        if (!columns.IsValidMapping)
          throw new TaskboardColumnMappingInvalidException();
        TaskboardColumn taskboardColumn = columns.Columns.FirstOrDefault<TaskboardColumn>((Func<TaskboardColumn, bool>) (c => TFStringComparer.WorkItemStateName.Equals(c.Name, newColumn)));
        if (taskboardColumn == null)
          throw new TaskboardColumnInvalidException(newColumn);
        if (!taskboardColumn.Mappings.Any<ITaskboardColumnMapping>((Func<ITaskboardColumnMapping, bool>) (m => TFStringComparer.WorkItemTypeName.Equals(m.WorkItemType, workItem.WorkItemType) && TFStringComparer.WorkItemStateName.Equals(m.State, workItem.State))))
          throw new TaskboardColumnInvalidForStateException(newColumn, workItem.State).Expected(context.ServiceName);
        using (TaskboardComponent component = context.CreateComponent<TaskboardComponent>())
          component.UpdateWorkItemColumn(project.Id, team.Id, workItemId, taskboardColumn.Id, context.GetUserId());
        TaskboardWorkItemColumnChangedEvent notificationEvent = new TaskboardWorkItemColumnChangedEvent()
        {
          ProjectId = project.Id,
          TeamId = team.Id,
          WorkItemId = workItemId,
          ColumnId = taskboardColumn.Id
        };
        context.GetService<ITeamFoundationEventService>().PublishNotification(context, (object) notificationEvent);
      }
    }

    private void CheckIterationNode(
      IVssRequestContext context,
      ProjectInfo project,
      WebApiTeam team,
      Guid iterationId,
      out Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iterationNode,
      out IAgileSettings agileSettings)
    {
      WorkItemTrackingTreeService service = context.GetService<WorkItemTrackingTreeService>();
      iterationNode = service.GetTreeNode(context, project.Id, iterationId);
      agileSettings = (IAgileSettings) new AgileSettings(context, CommonStructureProjectInfo.ConvertProjectInfo(project), team);
      if (!agileSettings.TeamSettings.Iterations.Any<ITeamIteration>((Func<ITeamIteration, bool>) (it => it.IterationId == iterationId)))
        throw new TaskboardInvalidTeamIterationException(iterationId);
    }

    private IReadOnlyCollection<TaskboardWorkItemColumn> GetMergedData(
      Dictionary<int, (string wit, string state)> workItemIdToTypeStateMap,
      Dictionary<int, (Guid colId, string colName)> taskBoardIdToColumnMap,
      TaskboardColumns taskBoardColumns)
    {
      List<TaskboardWorkItemColumn> mergedData = new List<TaskboardWorkItemColumn>();
      Dictionary<string, Dictionary<string, HashSet<Guid>>> dictionary1 = new Dictionary<string, Dictionary<string, HashSet<Guid>>>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      foreach (TaskboardColumn column in (IEnumerable<TaskboardColumn>) taskBoardColumns.Columns)
      {
        foreach (ITaskboardColumnMapping mapping in (IEnumerable<ITaskboardColumnMapping>) column.Mappings)
        {
          Dictionary<string, HashSet<Guid>> dictionary2;
          if (!dictionary1.TryGetValue(mapping.WorkItemType, out dictionary2))
          {
            dictionary2 = new Dictionary<string, HashSet<Guid>>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
            dictionary1[mapping.WorkItemType] = dictionary2;
          }
          HashSet<Guid> guidSet;
          if (!dictionary2.TryGetValue(mapping.State, out guidSet))
          {
            guidSet = new HashSet<Guid>();
            dictionary2[mapping.State] = guidSet;
          }
          guidSet.Add(column.Id);
        }
      }
      foreach (KeyValuePair<int, (string wit, string state)> itemIdToTypeState in workItemIdToTypeStateMap)
      {
        int key = itemIdToTypeState.Key;
        string wit = itemIdToTypeState.Value.wit;
        string state = itemIdToTypeState.Value.state;
        (Guid colId, string colName) tuple;
        bool flag = taskBoardIdToColumnMap.TryGetValue(key, out tuple);
        string column = state;
        Guid? computedColumnId = new Guid?();
        Dictionary<string, HashSet<Guid>> dictionary3;
        HashSet<Guid> source;
        if (dictionary1.TryGetValue(wit, out dictionary3) && dictionary3.TryGetValue(state, out source))
        {
          if (flag && source.Contains(tuple.colId))
          {
            column = tuple.colName;
            computedColumnId = new Guid?(tuple.colId);
          }
          else
          {
            computedColumnId = new Guid?(source.First<Guid>());
            column = taskBoardColumns.Columns.FirstOrDefault<TaskboardColumn>((Func<TaskboardColumn, bool>) (c =>
            {
              Guid id = c.Id;
              Guid? nullable = computedColumnId;
              return nullable.HasValue && id == nullable.GetValueOrDefault();
            }))?.Name;
          }
        }
        if (computedColumnId.HasValue)
        {
          TaskboardWorkItemColumn taskboardWorkItemColumn = new TaskboardWorkItemColumn(key, state, column, computedColumnId.Value);
          mergedData.Add(taskboardWorkItemColumn);
        }
      }
      return (IReadOnlyCollection<TaskboardWorkItemColumn>) mergedData;
    }

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
