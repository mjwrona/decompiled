// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.QueryResultModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  [KnownType(typeof (Exception))]
  public class QueryResultModel
  {
    private HashSet<string> m_pagingFields = new HashSet<string>();
    private IEnumerable<string> m_requiredColumns;
    private readonly int m_topCount = int.MaxValue;

    protected QueryResultModel(
      IVssRequestContext requestContext,
      string wiql,
      IDictionary<string, int> fieldWidthMap,
      bool includePayload,
      bool includeEditInfo,
      string currentProjectName,
      bool runQuery,
      IDictionary queryContext = null,
      IEnumerable<string> requiredColumns = null,
      int topCount = 2147483647,
      bool skipWiqlTextLimitValidation = false,
      bool useIsoDateFormat = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(wiql, nameof (wiql));
      this.m_topCount = topCount;
      this.ExecutionQuery = requestContext.GetService<IWorkItemQueryService>().ConvertToQueryExpression(requestContext, wiql, queryContext, skipWiqlTextLimitValidation: skipWiqlTextLimitValidation);
      this.EditorQuery = this.ExecutionQuery;
      this.AddColumns(requiredColumns);
      this.Initialize(requestContext, fieldWidthMap, runQuery, includePayload, includeEditInfo, currentProjectName, useIsoDateFormat: useIsoDateFormat);
    }

    protected QueryResultModel()
    {
    }

    public QueryResultModel(
      IVssRequestContext requestContext,
      string wiql,
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryInfo,
      IDictionary<string, int> fieldWidthMap,
      int topCount = 2147483647)
      : this(requestContext, wiql, queryInfo, queryInfo, fieldWidthMap, true, false, (string) null, true, topCount: topCount)
    {
    }

    public QueryResultModel(
      IVssRequestContext requestContext,
      string wiql,
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression executionQueryInfo,
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression editorQueryInfo,
      IDictionary<string, int> fieldWidthMap,
      bool includePayload,
      bool includeEditInfo,
      string currentProjectName,
      bool runQuery,
      IEnumerable<int> workItemIdFilter = null,
      IEnumerable<string> requiredColumns = null,
      int topCount = 2147483647,
      bool useIsoDateFormat = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression>(executionQueryInfo, "queryInfo");
      this.m_topCount = topCount;
      this.ExecutionQuery = executionQueryInfo;
      this.EditorQuery = editorQueryInfo;
      this.Wiql = wiql;
      this.AddColumns(requiredColumns);
      this.Initialize(requestContext, fieldWidthMap, runQuery, includePayload, includeEditInfo, currentProjectName, workItemIdFilter, useIsoDateFormat);
    }

    public QueryResultModel(
      IVssRequestContext requestContext,
      QueryEditorModel editorModel,
      Exception error)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<QueryEditorModel>(editorModel, nameof (editorModel));
      this.EditInfo = editorModel;
      this.Columns = editorModel.Fields ?? (IEnumerable<QueryDisplayColumn>) new List<QueryDisplayColumn>();
      this.SortColumns = editorModel.SortFields;
      this.Wiql = editorModel.Wiql;
      this.IsLinkQuery = editorModel.QueryMode >= 2;
      this.IsTreeQuery = editorModel.QueryMode >= 5;
      this.SourceIds = (IEnumerable<int>) Array.Empty<int>();
      this.TargetIds = (IEnumerable<int>) Array.Empty<int>();
      this.QueryRan = false;
      this.Error = error?.Message;
    }

    [IgnoreDataMember]
    public IEnumerable<string> RequiredColumns => (IEnumerable<string>) QueryColumnHelper.GetRequiredColumns(this.m_requiredColumns);

    [IgnoreDataMember]
    public Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression ExecutionQuery { get; set; }

    [IgnoreDataMember]
    public Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression EditorQuery { get; set; }

    [IgnoreDataMember]
    public bool IncludeContextInfo { get; set; }

    [DataMember(Name = "queryRan")]
    public bool QueryRan { get; set; }

    [DataMember(Name = "error", EmitDefaultValue = false)]
    public string Error { get; set; }

    [DataMember(Name = "wiql", EmitDefaultValue = false)]
    public string Wiql { get; set; }

    [DataMember(Name = "isLinkQuery", EmitDefaultValue = false)]
    public bool IsLinkQuery { get; set; }

    [DataMember(Name = "isTreeQuery", EmitDefaultValue = false)]
    public bool IsTreeQuery { get; set; }

    [DataMember(Name = "columns", EmitDefaultValue = false)]
    public virtual IEnumerable<QueryDisplayColumn> Columns { get; set; }

    [DataMember(Name = "sortColumns", EmitDefaultValue = false)]
    public IEnumerable<QuerySortColumn> SortColumns { get; set; }

    [DataMember(Name = "sourceIds", EmitDefaultValue = false)]
    public IEnumerable<int> SourceIds { get; set; }

    [DataMember(Name = "linkIds", EmitDefaultValue = false)]
    public IEnumerable<int> LinkIds { get; set; }

    [DataMember(Name = "realParentIds", EmitDefaultValue = false)]
    public IEnumerable<int> RealParentIds { get; set; }

    [DataMember(Name = "targetIds", EmitDefaultValue = false)]
    public IEnumerable<int> TargetIds { get; set; }

    [DataMember(Name = "pageColumns", EmitDefaultValue = false)]
    public IEnumerable<string> PageColumns { get; set; }

    [DataMember(Name = "payload", EmitDefaultValue = false)]
    public QueryResultPayload Payload { get; set; }

    [DataMember(Name = "editInfo", EmitDefaultValue = false)]
    public QueryEditorModel EditInfo { get; set; }

    [DataMember(Name = "hasMoreResult", EmitDefaultValue = false)]
    public bool HasMoreResult { get; set; }

    private void AddColumns(IEnumerable<string> requiredColumns = null) => this.m_requiredColumns = requiredColumns == null ? (IEnumerable<string>) new string[0] : requiredColumns;

    private void Initialize(
      IVssRequestContext requestContext,
      IDictionary<string, int> fieldWidthMap,
      bool runQuery,
      bool includePayload,
      bool includeEditInfo,
      string currentProjectName,
      IEnumerable<int> workItemIdFilter = null,
      bool useIsoDateFormat = false)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression executionQuery = this.ExecutionQuery;
      if (this.Wiql == null)
        this.Wiql = this.EditorQuery.Wiql;
      this.IsLinkQuery = executionQuery.QueryType != 0;
      this.IsTreeQuery = executionQuery.QueryType == QueryType.LinksRecursiveDoesNotContain || executionQuery.QueryType == QueryType.LinksRecursiveMayContain || executionQuery.QueryType == QueryType.LinksRecursiveMustContain;
      WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
      List<QueryDisplayColumn> queryDisplayColumnList = new List<QueryDisplayColumn>();
      this.m_pagingFields = new HashSet<string>();
      foreach (string displayField in executionQuery.DisplayFields)
      {
        FieldEntry field = service.GetField(requestContext, displayField);
        if (field.FieldId != 100)
          this.m_pagingFields.Add(displayField);
        queryDisplayColumnList.Add(new QueryDisplayColumn()
        {
          Name = displayField,
          Text = field.Name,
          FieldId = field.FieldId,
          CanSortBy = field.CanSortBy,
          FieldType = field.FieldType,
          IsIdentity = field.IsIdentity,
          Width = fieldWidthMap == null || !fieldWidthMap.ContainsKey(displayField) ? QueryResultModel.GetDefaultColumnWidth(field.FieldId, field.FieldType, field.IsIdentity) : fieldWidthMap[displayField]
        });
      }
      this.Columns = (IEnumerable<QueryDisplayColumn>) queryDisplayColumnList;
      this.SortColumns = executionQuery.SortFields.Where<QuerySortField>((Func<QuerySortField, bool>) (sf => sf.TableAlias != QueryTableAlias.Right)).Select<QuerySortField, QuerySortColumn>((Func<QuerySortField, QuerySortColumn>) (sf => new QuerySortColumn()
      {
        FieldName = sf.Field.ReferenceName,
        Descending = sf.Descending
      }));
      if (runQuery)
        this.RunQuery(requestContext, includePayload, workItemIdFilter);
      if (!includeEditInfo)
        return;
      this.EditInfo = new QueryEditorModel(requestContext, this.Wiql, false, fieldWidthMap, currentProjectName, useIsoDateFormat);
    }

    protected void RunQuery(
      IVssRequestContext requestContext,
      bool includePayload,
      IEnumerable<int> workItemIdFilter)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression executionQuery = this.ExecutionQuery;
      IWorkItemQueryService service = requestContext.GetService<IWorkItemQueryService>();
      IVssRequestContext requestContext1 = requestContext;
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression query = executionQuery;
      int topCount1 = this.m_topCount;
      Guid? projectId = new Guid?();
      int topCount2 = topCount1;
      QueryResult queryResult = service.ExecuteQuery(requestContext1, query, projectId, topCount2);
      this.HasMoreResult = queryResult.HasMoreResult;
      IEnumerable<int> first;
      if (queryResult.ResultType == QueryResultType.WorkItem)
      {
        first = queryResult.WorkItemIds;
        if (queryResult.QueryType == QueryType.LinksOneHopDoesNotContain)
        {
          this.SourceIds = Enumerable.Repeat<int>(0, queryResult.Count);
          this.LinkIds = this.SourceIds;
        }
      }
      else
      {
        first = queryResult.WorkItemLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (x => x.TargetId));
        this.SourceIds = queryResult.WorkItemLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (x => x.SourceId));
        this.LinkIds = queryResult.WorkItemLinks.Select<LinkQueryResultEntry, int>((Func<LinkQueryResultEntry, int>) (x => (int) x.LinkTypeId));
      }
      if (workItemIdFilter != null)
        first = first.Intersect<int>(workItemIdFilter);
      this.TargetIds = first;
      this.QueryRan = true;
      if (!includePayload)
        return;
      this.GeneratePayload(requestContext);
    }

    protected void GeneratePayload(IVssRequestContext requestContext)
    {
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      this.UpdatePageColumns();
      IList<int> list = (IList<int>) this.GetPageWorkItemIds().ToList<int>();
      IVssRequestContext requestContext1 = requestContext;
      IList<int> ids = list;
      IEnumerable<string> pageColumns = this.PageColumns;
      DateTime? asOf = new DateTime?();
      GenericDataReader dataReader = service.PageWorkItems(requestContext1, (IEnumerable<int>) ids, pageColumns, asOf, WorkItemRetrievalMode.All);
      this.Payload = new QueryResultPayload(requestContext, dataReader, false, list);
    }

    protected virtual void UpdatePageColumns() => this.PageColumns = (IEnumerable<string>) this.RequiredColumns.Concat<string>((IEnumerable<string>) this.m_pagingFields).Concat<string>((IEnumerable<string>) new string[1]
    {
      CoreFieldReferenceNames.Parent
    }).Distinct<string>().ToArray<string>();

    protected virtual IEnumerable<int> GetPageWorkItemIds() => (IEnumerable<int>) this.TargetIds.Take<int>(200).ToList<int>();

    internal static int GetDefaultColumnWidth(
      int fieldId,
      InternalFieldType fieldType,
      bool isIdentityField = false)
    {
      int defaultColumnWidth = Resources.ResultListDefaultColumnWidth;
      switch (fieldId)
      {
        case -35:
          return Resources.ResultListDefaultParentColumnWidth;
        case -3:
          return Resources.ResultListDefaultIdColumnWidth;
        case 1:
          return Resources.ResultListDefaultTitleColumnWidth;
        case 80:
          return Resources.ResultListDefaultTagsColumnWidth;
        case 100:
          return Resources.ResultListDefaultLinkTypeColumnWidth;
        default:
          if (isIdentityField)
            return 125;
          switch (fieldType)
          {
            case InternalFieldType.String:
              return Resources.ResultListDefaultStringColumnWidth;
            case InternalFieldType.Integer:
              return Resources.ResultListDefaultIntegerColumnWidth;
            case InternalFieldType.DateTime:
              return Resources.ResultListDefaultDateTimeColumnWidth;
            case InternalFieldType.TreePath:
              return Resources.ResultListDefaultTreePathColumnWidth;
            case InternalFieldType.Double:
              return Resources.ResultListDefaultDoubleColumnWidth;
            default:
              return defaultColumnWidth;
          }
      }
    }
  }
}
