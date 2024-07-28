// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.GenericBoard.Filters.BoardFilterSettingsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.GenericBoard.Filters
{
  public class BoardFilterSettingsManager
  {
    public static string BoardFilterParentItemFieldRefName = "System.ParentItem";
    private const string SelectStatement = "SELECT [System.Id] FROM WorkItems WHERE ";
    private readonly WiqlIdToNameTransformer wiqlIdToNameTransformer;

    public BoardFilterSettingsManager()
      : this(new WiqlIdToNameTransformer())
    {
    }

    public BoardFilterSettingsManager(WiqlIdToNameTransformer wiqlIdToNameTransformer) => this.wiqlIdToNameTransformer = wiqlIdToNameTransformer;

    public virtual BoardFilterSettingsModel GetBoardFilterSettingsModelFromWiqlandParentIds(
      IVssRequestContext requestContext,
      string wiql,
      IEnumerable<int> parentIds)
    {
      string names = this.TransformWiqlIdsToNames(requestContext, wiql);
      FilterModel filterModel = this.GetFilterModel(requestContext, names);
      if (parentIds == null)
        parentIds = (IEnumerable<int>) new List<int>();
      return new BoardFilterSettingsModel()
      {
        QueryText = names,
        QueryExpression = filterModel,
        ParentWorkItemIds = parentIds
      };
    }

    internal Dictionary<string, WorkItemFilter> GetFilterValue(
      IVssRequestContext requestContext,
      string projectId,
      string boardId)
    {
      return requestContext.GetService<ISettingsService>().GetValue<Dictionary<string, WorkItemFilter>>(requestContext, SettingsUserScope.User, "Project", projectId, BoardFilterSettingsManager.GetFilterRegistryPath(boardId), (Dictionary<string, WorkItemFilter>) null, false);
    }

    internal void SaveFilterValue(
      IVssRequestContext requestContext,
      string projectId,
      string boardId,
      Dictionary<string, WorkItemFilter> filter)
    {
      if (filter == null)
        return;
      requestContext.GetService<ISettingsService>().SetValue(requestContext, SettingsUserScope.User, "Project", projectId, BoardFilterSettingsManager.GetFilterRegistryPath(boardId), (object) filter, false);
    }

    internal Dictionary<string, WorkItemFilter> TryToConvertBoardFilterSettingsModelToFilterValue(
      IVssRequestContext requestContext,
      BoardFilterSettingsModel boardFilterSettingsModel)
    {
      Dictionary<string, WorkItemFilter> modelToFilterValue = new Dictionary<string, WorkItemFilter>();
      if (boardFilterSettingsModel != null)
      {
        try
        {
          if (boardFilterSettingsModel.QueryExpression != null && boardFilterSettingsModel.QueryExpression.Clauses != null)
          {
            foreach (IGrouping<string, FilterClause> source in boardFilterSettingsModel.QueryExpression.Clauses.GroupBy<FilterClause, string>((Func<FilterClause, string>) (g => g.FieldName)))
            {
              int num = source.Count<FilterClause>() <= 1 ? 0 : (string.Equals(source.Last<FilterClause>().LogicalOperator, "AND", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0);
              List<string> list = source.Select<FilterClause, string>((Func<FilterClause, string>) (s => s.Value)).ToList<string>();
              WorkItemFilter workItemFilter = new WorkItemFilter()
              {
                Values = list
              };
              if (num != 0)
              {
                workItemFilter.Options = new Dictionary<string, object>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
                workItemFilter.Options.Add("useAndOperator", (object) true);
              }
              modelToFilterValue.Add(source.Key, workItemFilter);
            }
          }
          if (boardFilterSettingsModel.ParentWorkItemIds != null)
          {
            if (boardFilterSettingsModel.ParentWorkItemIds.Count<int>() > 0)
              modelToFilterValue.Add(BoardFilterSettingsManager.BoardFilterParentItemFieldRefName, new WorkItemFilter()
              {
                Values = boardFilterSettingsModel.ParentWorkItemIds.Select<int, string>((Func<int, string>) (s => s.ToString())).ToList<string>()
              });
          }
        }
        catch (Exception ex)
        {
          string str = string.Join<int>(",", boardFilterSettingsModel.ParentWorkItemIds);
          string message = "Failed to migrate legacy board filter settings. QueryText: " + boardFilterSettingsModel.QueryText + ", ParentWorkItemIds: " + str + ", Stack trace: " + ex.StackTrace;
          requestContext.Trace(290775, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, message);
        }
      }
      return modelToFilterValue;
    }

    internal virtual FilterModel GetFilterModel(IVssRequestContext requestContext, string wiql)
    {
      if (string.IsNullOrEmpty(wiql))
        return new FilterModel();
      QueryEditorModel queryEditorModel = new QueryEditorModel(requestContext, "SELECT [System.Id] FROM WorkItems WHERE " + wiql, false, (IDictionary<string, int>) new Dictionary<string, int>(), (string) null);
      return this.ResolveFieldReferences(requestContext, (FilterModel) queryEditorModel.SourceFilter);
    }

    private FilterModel ResolveFieldReferences(
      IVssRequestContext requestContext,
      FilterModel filterModel)
    {
      WorkItemTrackingRequestContext witRequestContext = new WorkItemTrackingRequestContext(requestContext);
      QueryAdapter queryAdapter = new QueryAdapter(requestContext);
      filterModel.Clauses = (ICollection<FilterClause>) filterModel.Clauses.Select<FilterClause, FilterClause>((Func<FilterClause, FilterClause>) (c =>
      {
        c.FieldName = witRequestContext.FieldDictionary.GetFieldByNameOrId(c.FieldName).ReferenceName;
        c.Operator = queryAdapter.GetInvariantOperator(c.Operator);
        c.LogicalOperator = queryAdapter.GetInvariantOperator(c.LogicalOperator);
        if (c.Value.StartsWith("@", StringComparison.OrdinalIgnoreCase))
          c.Value = queryAdapter.GetInvariantFieldValue(c.FieldName, c.Operator, c.Value);
        return c;
      })).ToList<FilterClause>();
      return filterModel;
    }

    private string TransformWiqlIdsToNames(IVssRequestContext requestContext, string wiql)
    {
      if (!string.IsNullOrEmpty(wiql) && wiql.IndexOf('\a') >= 0)
      {
        wiql = "SELECT [System.Id] FROM WorkItems WHERE " + wiql;
        wiql = this.wiqlIdToNameTransformer.ReplaceIdWithText(requestContext, wiql);
        wiql = wiql.Substring("SELECT [System.Id] FROM WorkItems WHERE ".Length);
      }
      return wiql;
    }

    private static string GetFilterRegistryPath(string boardId) => "Filters/Kanban/" + boardId + "/Filter";
  }
}
