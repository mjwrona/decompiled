// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.KanbanHelper
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.Azure.Boards.Agile.Common.Exceptions;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server
{
  public static class KanbanHelper
  {
    public static void ValidateDeletedKanbanColumns(
      IVssRequestContext requestContext,
      IAgileSettings agileSettings,
      BacklogContext backlogContext,
      Guid extensionId,
      IEnumerable<BoardColumn> deletedColumns)
    {
      if (!deletedColumns.Any<BoardColumn>())
        return;
      IWorkItemQueryService service1 = requestContext.GetService<IWorkItemQueryService>();
      TeamFoundationWorkItemService service2 = requestContext.GetService<TeamFoundationWorkItemService>();
      ProductBacklogQueryBuilder backlogQueryBuilder = new ProductBacklogQueryBuilder(requestContext, agileSettings, backlogContext, agileSettings.TeamSettings.GetBacklogIterationNode(requestContext).GetPath(requestContext));
      IEnumerable<string> source = deletedColumns.SelectMany<BoardColumn, string>((Func<BoardColumn, IEnumerable<string>>) (c => (IEnumerable<string>) c.StateMappings.Values));
      if (source.Count<string>() == 0)
        return;
      string parentsQueryString = backlogQueryBuilder.GetBacklogBoardParentsQueryString(requestContext, source.ToArray<string>());
      QueryResult queryResult = service1.ExecuteQuery(requestContext, parentsQueryString, skipWiqlTextLimitValidation: true);
      IEnumerable<int> first = queryResult.WorkItemLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (link => link.TargetId));
      IEnumerable<int> second = queryResult.WorkItemLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (link => link.SourceId));
      IEnumerable<WorkItem> workItems = service2.GetWorkItems(requestContext, first.Except<int>(second), true, true, true, true, WorkItemRetrievalMode.NonDeleted, WorkItemErrorPolicy.Fail, false, false, false, new DateTime?());
      string extensionFieldName = KanbanUtils.Instance.GetKanbanColumnFieldReferenceName(requestContext, extensionId);
      string markerFieldName = KanbanUtils.Instance.GetMarkerField(requestContext, extensionId).ReferenceName;
      foreach (BoardColumn deletedColumn in deletedColumns)
      {
        BoardColumn column = deletedColumn;
        if (!string.IsNullOrEmpty(column.Name))
        {
          IEnumerable<WorkItem> itemsInColumn = workItems.Where<WorkItem>((Func<WorkItem, bool>) (r => r.GetFieldValue(requestContext, markerFieldName) != null && r.GetFieldValue<bool>(requestContext, markerFieldName) && r.GetFieldValue(requestContext, extensionFieldName) != null && TFStringComparer.BoardColumnName.Equals(r.GetFieldValue<string>(requestContext, extensionFieldName), column.Name)));
          int num = itemsInColumn.Count<WorkItem>();
          if (num > 0)
          {
            requestContext.TraceConditionally(6000201, TraceLevel.Verbose, "AgileService", "AgileService", (Func<string>) (() => string.Format("Cannot delete column. Work Item Ids: " + string.Join<int>(",", itemsInColumn.Select<WorkItem, int>((Func<WorkItem, int>) (r => r.Id))))));
            throw new DeletedBoardColumnIsNotEmptyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AgileResources.SetBoardSettings_DeletedColumnHasItems, (object) column.Name, (object) num));
          }
        }
      }
    }

    public static void ValidateDeletedKanbanRows(
      IVssRequestContext requestContext,
      IAgileSettings agileSettings,
      BacklogContext backlogContext,
      BoardSettings boardSettings,
      IEnumerable<BoardRow> rowsToDelete)
    {
      IWorkItemQueryService service1 = requestContext.GetService<IWorkItemQueryService>();
      Guid extensionId = boardSettings.ExtensionId.Value;
      string fieldReferenceName1 = KanbanUtils.Instance.GetKanbanColumnFieldReferenceName(requestContext, extensionId);
      string fieldReferenceName2 = KanbanUtils.Instance.GetKanbanLaneFieldReferenceName(requestContext, extensionId);
      ProductBacklogQueryBuilder backlogQueryBuilder = new ProductBacklogQueryBuilder(requestContext, agileSettings, backlogContext, agileSettings.TeamSettings.GetBacklogIterationNode(requestContext).GetPath(requestContext));
      string[] array1 = boardSettings.Columns.Where<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.InProgress)).Select<BoardColumn, string>((Func<BoardColumn, string>) (c => c.Name)).ToArray<string>();
      if (array1.Length == 0)
        return;
      string[] array2 = rowsToDelete.Select<BoardRow, string>((Func<BoardRow, string>) (r => r.Name)).ToArray<string>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression columnAndRowNames = backlogQueryBuilder.GetBacklogWorkItemIdsByColumnAndRowNames(requestContext, fieldReferenceName1, fieldReferenceName2, array1, array2);
      int[] array3 = service1.ExecuteQuery(requestContext, columnAndRowNames).WorkItemIds.ToArray<int>();
      if (!((IEnumerable<int>) array3).Any<int>())
        return;
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression backlogBoardQuery = backlogQueryBuilder.GetBacklogBoardQuery(requestContext, (IDictionary) null, WorkItemStateCategory.Proposed, WorkItemStateCategory.InProgress, WorkItemStateCategory.Completed, WorkItemStateCategory.Resolved);
      IEnumerable<LinkQueryResultEntry> workItemLinks = service1.ExecuteQuery(requestContext, backlogBoardQuery).WorkItemLinks;
      IEnumerable<int> source = workItemLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (l => l.TargetId)).Except<int>(workItemLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (l => l.SourceId))).Intersect<int>((IEnumerable<int>) array3);
      if (source.Any<int>())
      {
        TeamFoundationWorkItemService service2 = requestContext.GetService<TeamFoundationWorkItemService>();
        int num = source.First<int>();
        IVssRequestContext requestContext1 = requestContext;
        int workItemId = num;
        Guid? projectId = new Guid?();
        DateTime? revisionsSince = new DateTime?();
        object obj = service2.GetWorkItemById(requestContext1, workItemId, true, true, true, WorkItemRetrievalMode.NonDeleted, false, false, projectId, false, revisionsSince).GetFieldValue(requestContext, fieldReferenceName2) ?? (object) string.Empty;
        requestContext.Trace(6000202, TraceLevel.Verbose, "AgileService", "AgileService", string.Format("Cannot delete lane : [{0}], work item id: {1}", obj, (object) num));
        throw new DeletedBoardRowIsNotEmptyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AgileResources.SetBoardSettings_DeletedRowHasItems, obj));
      }
    }
  }
}
