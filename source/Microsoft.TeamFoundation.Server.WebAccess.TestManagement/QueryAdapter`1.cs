// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryAdapter`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public abstract class QueryAdapter<TQueryItem> : QueryAdapter
  {
    private Dictionary<string, QueryField<TQueryItem>> m_fieldsByDisplayName = new Dictionary<string, QueryField<TQueryItem>>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
    private QueryOperatorCollection m_queryOperatorCollection;

    public QueryAdapter(TestManagerRequestContext testContext)
    {
      this.TestContext = testContext;
      this.InitialPayloadSize = 200;
    }

    public int InitialPayloadSize { get; protected set; }

    public QueryOperatorCollection OperatorCollection
    {
      get
      {
        if (this.m_queryOperatorCollection == null)
          this.m_queryOperatorCollection = new QueryOperatorCollection();
        return this.m_queryOperatorCollection;
      }
    }

    protected TestManagerRequestContext TestContext { get; private set; }

    protected abstract QueryResultDataRowModel CreateDataRowWithId(TQueryItem item);

    public override QueryField TryGetFieldByDisplayName(string displayName)
    {
      QueryField<TQueryItem> fieldByDisplayName;
      this.m_fieldsByDisplayName.TryGetValue(displayName, out fieldByDisplayName);
      return (QueryField) fieldByDisplayName;
    }

    public QueryField GetFieldByDisplayName(string displayName)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryAdapter<TQueryItem>.GetFieldByDisplayName");
        QueryField fieldByDisplayName = this.TryGetFieldByDisplayName(displayName);
        if (fieldByDisplayName == null)
        {
          this.TestContext.TraceError("BusinessLayer", string.Format(TestManagementServerResources.ErrorFieldNotFoundFormat, (object) displayName));
          throw new TeamFoundationServerException(string.Format(TestManagementServerResources.ErrorFieldNotFoundFormat, (object) displayName));
        }
        return fieldByDisplayName;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryAdapter<TQueryItem>.GetFieldByDisplayName");
      }
    }

    public QueryField<TQueryItem> GetQueryItemFieldByDisplayName(string displayName) => (QueryField<TQueryItem>) this.GetFieldByDisplayName(displayName);

    protected QueryField<TQueryItem> AddField(
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, string> getDataValueFunc)
    {
      return this.AddField((QueryField<TQueryItem>) new QueryStringField<TQueryItem>(this.TestContext, referenceName, displayName, isQueryable, getDataValueFunc));
    }

    protected QueryField<TQueryItem> AddField(
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, int> getDataValueFunc,
      Dictionary<int, string> valuesMap = null)
    {
      return this.AddField((QueryField<TQueryItem>) new QueryIntField<TQueryItem>(this.TestContext, referenceName, displayName, isQueryable, getDataValueFunc, valuesMap));
    }

    protected QueryField<TQueryItem> AddField(
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, DateTime> getDataValueFunc)
    {
      return this.AddField((QueryField<TQueryItem>) new QueryDateField<TQueryItem>(this.TestContext, referenceName, displayName, isQueryable, getDataValueFunc));
    }

    protected QueryField<TQueryItem> AddField(
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, bool> getDataValueFunc)
    {
      return this.AddField((QueryField<TQueryItem>) new QueryBoolField<TQueryItem>(this.TestContext, referenceName, displayName, isQueryable, getDataValueFunc));
    }

    protected QueryField<TQueryItem> AddTimeSpanField(
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, long> getDataValueFunc)
    {
      return this.AddField((QueryField<TQueryItem>) new QueryTimeSpanField<TQueryItem>(this.TestContext, referenceName, displayName, isQueryable, getDataValueFunc));
    }

    protected QueryField<TQueryItem> AddTFIdentityField(
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, Guid> getDataValueFunc)
    {
      return this.AddField((QueryField<TQueryItem>) new TestQueryTFIdentityField<TQueryItem>(this.TestContext, referenceName, displayName, isQueryable, getDataValueFunc));
    }

    protected QueryField<TQueryItem> AddField(QueryField<TQueryItem> field)
    {
      this.m_fieldsByDisplayName[field.DisplayName] = field;
      return field;
    }

    public override IEnumerable<QueryField> GetFields() => (IEnumerable<QueryField>) this.m_fieldsByDisplayName.Values;

    public override IEnumerable<QueryField> GetQueryableFields() => this.GetFields().Where<QueryField>((Func<QueryField, bool>) (f => f.IsQueryable));

    public override FilterModel GetDefaultFilter() => new FilterModel();

    public abstract List<TQueryItem> FetchItemsByIds(IEnumerable<string> Ids);

    public override QueryResultModel ExecuteQuery(QueryModel query)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryAdapter.ExecuteQuery");
        this.ValidateQueryFields(query);
        QueryResultModel queryResultModel = new QueryResultModel();
        try
        {
          queryResultModel.ItemIds = (IEnumerable<string>) this.GetQueryResultItemIds(query);
          IEnumerable<string> itemIds = queryResultModel.ItemIds;
          if (this.InitialPayloadSize > 0)
            itemIds = itemIds.Take<string>(this.InitialPayloadSize);
          queryResultModel.DataRows = this.FetchDataRowsFromIds(itemIds, query.Columns.DisplayColumns);
        }
        catch (Exception ex)
        {
          this.TestContext.TraceError("BusinessLayer", string.Format(TestManagementServerResources.RunQueryExceptionMessageFormat, (object) ex.Message));
          queryResultModel.Error = string.Format(TestManagementServerResources.RunQueryExceptionMessageFormat, (object) ex.Message);
        }
        return queryResultModel;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryAdapter.ExecuteQuery");
      }
    }

    public override IEnumerable<QueryResultDataRowModel> FetchDataRowsFromIds(
      IEnumerable<string> Ids,
      IEnumerable<QueryDisplayColumn> columns)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryAdapter.FetchDataRowsFromIds");
        List<QueryField<TQueryItem>> forDisplayColumns = this.GetFieldsForDisplayColumns(columns);
        IEnumerable<string> strings = Ids.Where<string>((Func<string, bool>) (resId => !string.IsNullOrEmpty(resId)));
        return strings.Any<string>() ? this.GetPagedResultsData((IEnumerable<TQueryItem>) this.FetchItemsByIds(strings), (IEnumerable<QueryField<TQueryItem>>) forDisplayColumns) : (IEnumerable<QueryResultDataRowModel>) Array.Empty<QueryResultDataRowModel>();
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryAdapter.FetchDataRowsFromIds");
      }
    }

    public string GetClauseConditionString(FilterClause clause)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryAdapter<TQueryItem>.GetClauseConditionString");
        QueryField fieldByDisplayName = this.GetFieldByDisplayName(clause.FieldName);
        QueryOperator op = this.OperatorCollection.GetOperatorByDisplayName(clause.Operator);
        if (op == null || !fieldByDisplayName.AllowedOperators.Any<QueryOperator>((Func<QueryOperator, bool>) (x => string.Equals(x.RawValue, op.RawValue))))
        {
          this.TestContext.TraceError("BusinessLayer", string.Format(TestManagementServerResources.ErrorInvalidFieldOperatorFormat, (object) clause.Operator, (object) fieldByDisplayName.DisplayName));
          throw new TeamFoundationServerException(string.Format(TestManagementServerResources.ErrorInvalidFieldOperatorFormat, (object) clause.Operator, (object) fieldByDisplayName.DisplayName));
        }
        return fieldByDisplayName.GetConditionString(op, clause.Value);
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryAdapter<TQueryItem>.GetClauseConditionString");
      }
    }

    public string GetFilterString(FilterModel filter)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryAdapter<TQueryItem>.GetFilterString");
        int num = 0;
        StringBuilder stringBuilder = new StringBuilder();
        if (filter != null && filter.Clauses != null)
        {
          foreach (FilterClause filterClause in filter.Clauses.Where<FilterClause>((Func<FilterClause, bool>) (c => c != null && c.IsValid())))
          {
            FilterClause clause = filterClause;
            clause.Index = ++num;
            if (clause.Index > 1)
            {
              bool flag = string.Equals(clause.LogicalOperator, TestManagementResources.QueryAnd, StringComparison.CurrentCultureIgnoreCase) || string.Equals(clause.LogicalOperator, "AND", StringComparison.CurrentCultureIgnoreCase);
              stringBuilder.Append(flag ? " AND " : " OR ");
            }
            if (filter.Groups != null)
              stringBuilder.Append('(', filter.Groups.Count<FilterGroup>((Func<FilterGroup, bool>) (g => g != null && g.Start == clause.Index)));
            stringBuilder.Append(this.GetClauseConditionString(clause));
            if (filter.Groups != null)
              stringBuilder.Append(')', filter.Groups.Count<FilterGroup>((Func<FilterGroup, bool>) (g => g != null && g.End == clause.Index)));
          }
        }
        return stringBuilder.ToString();
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryAdapter<TQueryItem>.GetFilterString");
      }
    }

    public string GetOrderByString(IEnumerable<QuerySortColumn> sortColumns)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryAdapter<TQueryItem>.GetOrderByString");
        StringBuilder stringBuilder = new StringBuilder();
        if (sortColumns != null)
        {
          List<QuerySortColumn> list = sortColumns.Where<QuerySortColumn>((Func<QuerySortColumn, bool>) (sc => sc != null)).ToList<QuerySortColumn>();
          if (list.Count > 0)
          {
            for (int index = 0; index < list.Count; ++index)
            {
              QueryField fieldByDisplayName = this.TryGetFieldByDisplayName(list[index].Name);
              if (fieldByDisplayName == null)
              {
                this.TestContext.TraceError("BusinessLayer", string.Format(TestManagementServerResources.ErrorQueryInvalidSortFieldFormat, (object) list[index].Name));
                throw new TeamFoundationServerException(string.Format(TestManagementServerResources.ErrorQueryInvalidSortFieldFormat, (object) list[index].Name));
              }
              if (index > 0)
                stringBuilder.Append(",");
              stringBuilder.AppendFormat("[{0}]", (object) fieldByDisplayName.ReferenceName);
              if (!string.Equals(list[index].Order, "asc", StringComparison.OrdinalIgnoreCase))
                stringBuilder.Append(" DESC");
            }
          }
        }
        return stringBuilder.ToString();
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryAdapter<TQueryItem>.GetOrderByString");
      }
    }

    protected List<QueryField<TQueryItem>> GetFieldsForDisplayColumns(
      IEnumerable<QueryDisplayColumn> displayColumns)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryAdapter<TQueryItem>.GetFieldsForDisplayColumns");
        List<QueryField<TQueryItem>> forDisplayColumns = new List<QueryField<TQueryItem>>();
        foreach (QueryDisplayColumn queryDisplayColumn in displayColumns.Where<QueryDisplayColumn>((Func<QueryDisplayColumn, bool>) (col => col != null)))
          forDisplayColumns.Add(this.GetQueryItemFieldByDisplayName(queryDisplayColumn.Name));
        return forDisplayColumns;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryAdapter<TQueryItem>.GetFieldsForDisplayColumns");
      }
    }

    public virtual IEnumerable<QueryResultDataRowModel> GetPagedResultsData(
      IEnumerable<TQueryItem> results,
      IEnumerable<QueryField<TQueryItem>> fields)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryAdapter<TQueryItem>.GetPagedResultsData");
        List<QueryResultDataRowModel> pagedResultsData = new List<QueryResultDataRowModel>();
        foreach (TQueryItem result in results)
        {
          QueryResultDataRowModel dataRowWithId = this.CreateDataRowWithId(result);
          dataRowWithId.Data = (IEnumerable<string>) this.GetDataRowColumns(result, fields);
          pagedResultsData.Add(dataRowWithId);
        }
        return (IEnumerable<QueryResultDataRowModel>) pagedResultsData;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryAdapter<TQueryItem>.GetPagedResultsData");
      }
    }

    protected virtual List<string> GetDataRowColumns(
      TQueryItem item,
      IEnumerable<QueryField<TQueryItem>> fields)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryAdapter<TQueryItem>.GetDataRowColumns");
        List<string> dataRowColumns = new List<string>();
        foreach (QueryField<TQueryItem> field in fields)
        {
          if (field != null)
            dataRowColumns.Add(field.GetDataDisplayValue(item));
          else
            dataRowColumns.Add("null field");
        }
        return dataRowColumns;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryAdapter<TQueryItem>.GetDataRowColumns");
      }
    }

    public void ValidateQueryFields(QueryModel query)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryAdapter<TQueryItem>.ValidateQueryFields");
        if (query == null || query.Filter == null || query.Filter.Clauses == null || query.Filter.Clauses.Count <= 0)
          return;
        foreach (FilterClause clause in (IEnumerable<FilterClause>) query.Filter.Clauses)
        {
          QueryField fieldByDisplayName = this.TryGetFieldByDisplayName(clause.FieldName);
          if (fieldByDisplayName != null && !fieldByDisplayName.IsQueryable)
          {
            this.TestContext.TraceError("BusinessLayer", string.Format(TestManagementServerResources.IncorrectQueryField, (object) clause.FieldName));
            throw new TeamFoundationServerException(string.Format(TestManagementServerResources.IncorrectQueryField, (object) clause.FieldName));
          }
        }
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryAdapter<TQueryItem>.ValidateQueryFields");
      }
    }
  }
}
