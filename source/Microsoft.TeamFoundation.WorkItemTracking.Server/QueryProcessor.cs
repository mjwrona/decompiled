// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryProcessor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class QueryProcessor
  {
    private QueryProcessorContext context;

    protected QueryProcessor()
    {
    }

    public QueryProcessor(IVssRequestContext requestContext) => this.context = new QueryProcessorContext(requestContext);

    public virtual QueryExecutionDetail GenerateQueryExecutionDetail(
      QueryExpression queryExpression,
      IEnumerable<DateTime> asOfDateTimes,
      QuerySource querySource = QuerySource.Unknown)
    {
      ArgumentUtility.CheckForNull<QueryExpression>(queryExpression, nameof (queryExpression));
      if (queryExpression.AsOfDateTime.HasValue)
        throw new NotSupportedException();
      QueryPreprocessor.ValidateAndOptimize(this.context, queryExpression);
      this.context.m_asOfDatesParam = QueryParameter.DefineTableValuedParameter<DateTime>(this.context, (WorkItemTrackingTableValueParameter<DateTime>) new DateTimeTable(asOfDateTimes));
      this.context.m_isAsOfQuery = true;
      this.context.m_isParentQuery = queryExpression.IsParentQuery;
      this.context.m_isChartingQuery = querySource == QuerySource.Charting;
      if (this.context.m_isChartingQuery)
        queryExpression.SortFields = Enumerable.Empty<QuerySortField>();
      this.AddQueryHashTag(queryExpression);
      Dictionary<QuerySortField, string> sortFieldColumnMap = new Dictionary<QuerySortField, string>();
      SqlStatement sqlStatement = queryExpression.QueryType == QueryType.WorkItems ? this.CreateSqlStatement(queryExpression, QueryTableAlias.Left, sortFieldColumnMap) : throw new NotSupportedException(ServerResources.AsOfWorkItemQueriesNotSupported());
      foreach (QuerySortField sortField in queryExpression.SortFields)
      {
        string columnName;
        if (sortFieldColumnMap.TryGetValue(sortField, out columnName))
          sqlStatement.OrderBy.Add(QueryPredicate.BuildOrderBy(sortField.Field, columnName, sortField.Descending, sortField.NullsFirst));
      }
      if (this.context.IsQueryMAXDOPCapped)
        sqlStatement.Options.Add(string.Format("MAXDOP {0}", (object) this.context.m_queryMAXDOPValue));
      if (this.context.IsQueryMaxGrantCapped)
        sqlStatement.Options.Add(string.Format("MAX_GRANT_PERCENT = {0}", (object) this.context.m_queryMaxGrantPercent));
      if (this.context.IsForceOrderEnabled || this.context.IsFullTextJoinOptimizationEnabled && this.context.IsLongTextPredicateAppended)
        sqlStatement.Options.Add("FORCE ORDER");
      if (!this.context.IsAllowNonClusteredColumnstoreIndexEnabled)
        sqlStatement.Options.Add("IGNORE_NONCLUSTERED_COLUMNSTORE_INDEX");
      this.context.m_queryText.AppendLine(sqlStatement.GetSql());
      QueryExecutionDetail queryExecutionDetail = new QueryExecutionDetail();
      queryExecutionDetail.QueryText = this.context.m_queryText.ToString();
      queryExecutionDetail.Parameters = (IEnumerable<SqlParameter>) this.context.m_parameters;
      queryExecutionDetail.QueryCategory = this.SummarizeQueryCategory();
      queryExecutionDetail.FieldsDoStringComparison = this.context.FieldsDoStringComparison;
      this.context.m_requestContext.Trace(906026, TraceLevel.Verbose, "Query", nameof (QueryProcessor), "Number of parameters: {0}.", (object) this.context.m_parameters.Count);
      return queryExecutionDetail;
    }

    public virtual QueryExecutionDetail GenerateQueryExecutionDetail(
      QueryExpression queryExpression,
      int topCount = 2147483647,
      QuerySource querySource = QuerySource.Unknown)
    {
      ArgumentUtility.CheckForNull<QueryExpression>(queryExpression, nameof (queryExpression));
      ArgumentUtility.CheckForOutOfRange(topCount, nameof (topCount), 0);
      QueryPreprocessor.ValidateAndOptimize(this.context, queryExpression);
      this.context.m_isChartingQuery = querySource == QuerySource.Charting;
      this.context.m_isParentQuery = queryExpression.IsParentQuery;
      if (this.context.m_isChartingQuery)
        queryExpression.SortFields = Enumerable.Empty<QuerySortField>();
      this.AddQueryHashTag(queryExpression);
      Dictionary<QuerySortField, string> sortFieldColumnMap = new Dictionary<QuerySortField, string>();
      SqlStatement sqlStatement1 = (SqlStatement) null;
      switch (queryExpression.QueryType)
      {
        case QueryType.WorkItems:
          sqlStatement1 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Left, sortFieldColumnMap);
          break;
        case QueryType.LinksOneHopMustContain:
          sqlStatement1 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Left, sortFieldColumnMap);
          SqlStatement sqlStatement2 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Link, sortFieldColumnMap);
          sqlStatement1.DefineJoin("(" + sqlStatement2.GetSql() + ")", "links", "links.SourceId = lhs.Id");
          sqlStatement1.Select.AddRange((IEnumerable<string>) new string[3]
          {
            "links.TargetId",
            "links.LinkTypeId",
            "links.IsLocked"
          });
          SqlStatement sqlStatement3 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Right, sortFieldColumnMap);
          string joinKind = queryExpression.RightGroup != null ? string.Empty : "LEFT";
          sqlStatement1.DefineJoin(joinKind, "(" + sqlStatement3.GetSql() + ")", "rhs", "rhs.Id = links.TargetId");
          if (queryExpression.RightGroup != null)
          {
            sqlStatement1.Select.Add("rhs.LatestAreaId AS TargetLatestAreaId");
            break;
          }
          sqlStatement1.Select.Add("ISNULL(rhs.LatestAreaId, 0) AS TargetLatestAreaId");
          break;
        case QueryType.LinksOneHopMayContain:
          sqlStatement1 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Left, sortFieldColumnMap);
          SqlStatement sqlStatement4 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Link, sortFieldColumnMap);
          SqlStatement sqlStatement5 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Right, sortFieldColumnMap);
          sqlStatement4.DefineJoin("(" + sqlStatement5.GetSql() + ")", "rhs", "rhs.Id = links.TargetId");
          sqlStatement4.Select.Add("rhs.*");
          sqlStatement1.DefineJoin("LEFT", "(" + sqlStatement4.GetSql() + ")", "links", "links.SourceId = lhs.Id");
          sqlStatement1.Select.AddRange((IEnumerable<string>) new string[3]
          {
            "links.TargetId",
            "links.LinkTypeId",
            "links.IsLocked"
          });
          if (queryExpression.RightGroup != null)
          {
            sqlStatement1.Select.Add("links.LatestAreaId AS TargetLatestAreaId");
            break;
          }
          sqlStatement1.Select.Add("ISNULL(links.LatestAreaId, 0) AS TargetLatestAreaId");
          break;
        case QueryType.LinksOneHopDoesNotContain:
          sqlStatement1 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Left, sortFieldColumnMap);
          SqlStatement sqlStatement6 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Link, sortFieldColumnMap);
          if (queryExpression.RightGroup != null || this.context.m_sortRhs)
          {
            SqlStatement sqlStatement7 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Right, sortFieldColumnMap);
            sqlStatement6.DefineJoin("(" + sqlStatement7.GetSql() + ")", "rhs", "rhs.Id = links.TargetId");
          }
          sqlStatement1.Select.AddRange((IEnumerable<string>) new string[3]
          {
            "NULL AS TargetId",
            "NULL AS LinkTypeId",
            "NULL AS IsLocked"
          });
          if (!string.IsNullOrWhiteSpace(sqlStatement1.GetPredicate()))
            sqlStatement1.AppendPredicate(" AND ");
          sqlStatement1.AppendPredicate("lhs.Id NOT IN (" + sqlStatement6.GetSql() + ")");
          break;
        case QueryType.LinksRecursiveMayContain:
          if (queryExpression.RecursionOption == QueryRecursionOption.ParentFirst)
          {
            SqlStatement sqlStatement8 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Left, sortFieldColumnMap);
            sqlStatement8.Select = new List<string>(3);
            sqlStatement8.Select.AddRange((IEnumerable<string>) new string[3]
            {
              "lhs.Id",
              "ISNULL(links.Path, CAST(0x7FFFFFFF AS BINARY(4)) + CAST(lhs.Id AS BINARY(4))) AS Path",
              "DATALENGTH(links.Path) AS Level"
            });
            sqlStatement8.DefineJoin("LEFT", "dbo.LinkTreesLatest", "links", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "links.TargetID = lhs.Id AND links.PartitionId = lhs.PartitionId AND links.LinkType = {0}", (object) queryExpression.RecursionLinkTypeId));
            if (this.context.IsQueryMaxGrantCapped)
              sqlStatement8.Options.Add(string.Format("MAX_GRANT_PERCENT = {0}", (object) this.context.m_queryMaxGrantPercent));
            this.context.m_queryText.AppendLine("\r\nDECLARE @Count INT\r\n\r\nCREATE TABLE #roots\r\n(\r\n    Id INT NOT NULL,\r\n    Path VARBINARY(880) NOT NULL UNIQUE CLUSTERED,\r\n    Level INT NULL\r\n)\r\n\r\nCREATE TABLE #results\r\n(\r\n    SourceId    INT NOT NULL,\r\n    TargetId    INT NOT NULL UNIQUE CLUSTERED,\r\n    IsLocked    BIT NOT NULL,\r\n    MeetsParentCriteria BIT NOT NULL,\r\n    Level       INT NULL\r\n)\r\n\r\nCREATE TABLE #results_filtered\r\n(\r\n    SourceId            INT NOT NULL,\r\n    TargetId            INT NOT NULL,\r\n    IsLocked            BIT NOT NULL,\r\n    MeetsParentCriteria BIT NOT NULL,\r\n    MeetsChildCriteria  BIT NOT NULL,\r\n    TargetLatestAreaId  INT NOT NULL,\r\n    Level               INT NULL,\r\n    IsRemoveable        BIT NOT NULL,\r\n    RowNumber           INT NULL,\r\n    UNIQUE CLUSTERED (Level, TargetId, SourceId, MeetsParentCriteria, MeetsChildCriteria, IsRemoveable) WITH (IGNORE_DUP_KEY=ON)\r\n)\r\n\r\nCREATE TABLE #nodes_meet_parent\r\n(\r\n    Id INT NOT NULL UNIQUE CLUSTERED\r\n)\r\n\r\nINSERT #roots (Id, Path, Level)\r\n");
            this.context.m_queryText.AppendLine(sqlStatement8.GetSql());
            this.context.m_queryText.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\r\nSELECT @Count = @@rowcount\r\n\r\nDELETE Roots\r\nOUTPUT 0, deleted.Id, 0, 1, deleted.Level\r\nINTO #results\r\nFROM #roots Roots\r\nWHERE Roots.Path > 0x7FFFFFFF\r\n\r\nSELECT @Count = @Count - @@rowcount\r\n\r\nIF (@Count < 5000)\r\nBEGIN\r\n    DELETE Children\r\n    OUTPUT deleted.Id\r\n    INTO #nodes_meet_parent\r\n    FROM #roots as Parents\r\n    JOIN #roots as Children WITH(forceseek)\r\n        ON Children.Path > Parents.Path\r\n        AND Children.Path < Parents.Path + 0x7FFFFFFF\r\nEND\r\nELSE\r\nBEGIN\r\n    CREATE NONCLUSTERED INDEX RootLevels ON #roots\r\n    (\r\n        Level ASC,\r\n        Path ASC\r\n    )\r\n\r\n    DECLARE @minLevel SMALLINT\r\n    DECLARE @curLevel SMALLINT\r\n\r\n    SELECT TOP 1 @minLevel = Level\r\n    FROM #roots\r\n    ORDER BY Level ASC\r\n\r\n    WHILE @minLevel <> 0\r\n    BEGIN\r\n        DELETE Children\r\n        OUTPUT deleted.Id\r\n        INTO #nodes_meet_parent\r\n        FROM #roots as Parents\r\n        JOIN #roots as Children WITH(forceseek)\r\n            ON Children.Path > Parents.Path\r\n            AND Children.Path < Parents.Path + 0x7FFFFFFF\r\n            AND Parents.Level = @minLevel\r\n\r\n        SET @curLevel = @minLevel\r\n        SET @minLevel = 0 \r\n\r\n        SELECT TOP 1 @minLevel = Level\r\n        FROM #roots\r\n        WHERE Level > @curLevel\r\n        ORDER BY Level ASC\r\n    END\r\nEND\r\n\r\nINSERT #results (SourceId, TargetId, IsLocked, MeetsParentCriteria, Level)\r\nSELECT CASE\r\n        WHEN Children.TargetId IS NULL THEN 0\r\n        WHEN Children.TargetId = Parents.Id THEN 0\r\n        ELSE Children.SourceId\r\n    END AS SourceId,\r\n    ISNULL(Children.TargetId, Parents.Id) AS TargetId,\r\n    CONVERT(BIT,\r\n        CASE\r\n            WHEN Children.SourceId IS NULL THEN 0\r\n            ELSE ISNULL(Children.fLock, 0)\r\n        END) AS IsLocked,\r\n    CONVERT(BIT,\r\n        CASE\r\n            WHEN Children.TargetId IS NULL THEN 1\r\n            WHEN Children.TargetId = Parents.Id THEN 1\r\n            WHEN Children.TargetId = n.ID then 1\r\n            ELSE 0\r\n        END) AS MeetsParentCriteria, \r\n    DATALENGTH(Children.Path)\r\nFROM #roots AS Parents\r\nLEFT JOIN dbo.LinkTreesLatest AS Children\r\n    ON Children.Path >= Parents.Path\r\n    AND Children.Path < Parents.Path + 0x7FFFFFFF\r\n    AND Children.LinkType = {0}\r\n    AND Children.PartitionId = {1}\r\nLEFT JOIN #nodes_meet_parent n\r\n    ON Children.TargetId = n.Id\r\n", (object) queryExpression.RecursionLinkTypeId, (object) this.context.PartitionId);
            sqlStatement1 = new SqlStatement("#results", "Results", false);
            sqlStatement1.Select.AddRange((IEnumerable<string>) new string[8]
            {
              "Results.SourceId",
              "Results.TargetId",
              "Results.IsLocked",
              "Results.MeetsParentCriteria",
              "CASE WHEN rhs.Id IS NULL THEN CAST(0 AS BIT) ELSE rhs.MeetsChildCriteria END AS MeetsChildCriteria",
              "ISNULL(rhs.LatestAreaId, 0) AS TargetLatestAreaId",
              "Results.Level",
              "0"
            });
            SqlStatement sqlStatement9 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Right, sortFieldColumnMap);
            string str = sqlStatement9.GetPredicate();
            if (string.IsNullOrWhiteSpace(str))
              str = "1=1";
            sqlStatement9.Select.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CAST(CASE WHEN {0} THEN 1 ELSE 0 END AS BIT) AS MeetsChildCriteria", (object) str));
            sqlStatement9.ResetPredicate();
            sqlStatement9.AppendPredicate("{0}.PartitionId = {1}", sqlStatement9.TableAlias, this.context.PartitionId);
            sqlStatement1.DefineJoin("LEFT", "(" + sqlStatement9.GetSql() + ")", "rhs", "rhs.Id = Results.TargetId");
            break;
          }
          SqlStatement sqlStatement10 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Right, sortFieldColumnMap);
          sqlStatement10.Select = new List<string>(5);
          sqlStatement10.Select.AddRange((IEnumerable<string>) new string[5]
          {
            "ISNULL(links.SourceID, 0)",
            "rhs.Id",
            "ISNULL(links.fLock, CAST(0 AS BIT))",
            "ISNULL(DATALENGTH(links.Path), 4)",
            "1"
          });
          sqlStatement10.DefineJoin("LEFT", "dbo.LinkTreesLatest", "links", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "links.TargetID = rhs.Id AND links.LinkType = {0} AND links.PartitionId = rhs.PartitionId", (object) queryExpression.RecursionLinkTypeId));
          if (this.context.IsQueryMaxGrantCapped)
            sqlStatement10.Options.Add(string.Format("MAX_GRANT_PERCENT = {0}", (object) this.context.m_queryMaxGrantPercent));
          this.context.m_queryText.AppendLine("\r\nCREATE TABLE #results (\r\n    SourceId INT NOT NULL,\r\n    TargetId INT NOT NULL,\r\n    IsLocked BIT NOT NULL,\r\n    Level INT NOT NULL,\r\n    MeetsChildCriteria BIT NOT NULL,\r\n\r\n    UNIQUE CLUSTERED (Level, TargetId) WITH (IGNORE_DUP_KEY=ON)\r\n)\r\n\r\nINSERT #results(SourceId, TargetId, IsLocked, Level, MeetsChildCriteria)\r\n");
          this.context.m_queryText.AppendLine(sqlStatement10.GetSql());
          this.context.m_queryText.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\r\n\r\nDECLARE @maxLevel SMALLINT\r\n\r\nSELECT TOP 1 @maxLevel = Level\r\nFROM #results\r\nORDER BY Level DESC\r\n\r\nWHILE @maxLevel > 4\r\nBEGIN\r\n    INSERT #results(SourceId, TargetId, IsLocked, Level, MeetsChildCriteria)\r\n    SELECT S.SourceId, S.TargetId, S.IsLocked, S.Level, 0\r\n    FROM\r\n    (\r\n        SELECT DISTINCT ISNULL(p.SourceID, 0) as SourceId,\r\n            p.TargetID as TargetId,\r\n            ISNULL(p.fLock, CAST(0 AS BIT)) AS IsLocked,\r\n            DATALENGTH(p.Path) as Level\r\n        FROM #results c\r\n        JOIN dbo.LinkTreesLatest p\r\n            ON c.Level = @maxLevel\r\n            AND c.SourceId = p.TargetID\r\n            AND p.LinkType = {0}\r\n                            AND p.PartitionId = {1}\r\n    ) S\r\n    LEFT JOIN #results T\r\n        ON T.TargetId = S.TargetId\r\n        AND T.Level = S.Level\r\n    WHERE T.TargetId IS NULL\r\n                    OPTION (LOOP JOIN)\r\n    \r\n    SET @maxLevel = @maxLevel - 4\r\nEND\r\n\r\n", (object) queryExpression.RecursionLinkTypeId, (object) this.context.PartitionId);
          sqlStatement1 = new SqlStatement("#results", "Results", false);
          sqlStatement1.Select.AddRange((IEnumerable<string>) new string[6]
          {
            "Results.SourceId",
            "Results.TargetId",
            "Results.IsLocked",
            "Results.MeetsChildCriteria",
            "CASE WHEN lhs.Id IS NULL THEN CAST(0 AS BIT) ELSE lhs.MeetsParentCriteria END AS MeetsParentCriteria",
            "ISNULL(lhs.LatestAreaId, 0) AS TargetLatestAreaId"
          });
          SqlStatement sqlStatement11 = this.CreateSqlStatement(queryExpression, QueryTableAlias.Left, sortFieldColumnMap);
          string str1 = sqlStatement11.GetPredicate();
          if (string.IsNullOrWhiteSpace(str1))
            str1 = "1=1";
          sqlStatement11.Select.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CAST(CASE WHEN {0} THEN 1 ELSE 0 END AS BIT) AS MeetsParentCriteria", (object) str1));
          sqlStatement11.ResetPredicate();
          sqlStatement11.AppendPredicate("{0}.PartitionId = {1}", sqlStatement11.TableAlias, this.context.PartitionId);
          sqlStatement1.DefineJoin("LEFT", "(" + sqlStatement11.GetSql() + ")", "lhs", "lhs.Id = Results.TargetId");
          break;
      }
      foreach (QuerySortField sortField in queryExpression.SortFields)
      {
        string columnName;
        if (sortFieldColumnMap.TryGetValue(sortField, out columnName))
          sqlStatement1.OrderBy.Add(QueryPredicate.BuildOrderBy(sortField.Field, columnName, sortField.Descending, sortField.NullsFirst));
      }
      if (this.context.IsQueryMAXDOPCapped)
        sqlStatement1.Options.Add(string.Format("MAXDOP {0}", (object) this.context.m_queryMAXDOPValue));
      if (this.context.IsQueryMaxGrantCapped)
        sqlStatement1.Options.Add(string.Format("MAX_GRANT_PERCENT = {0}", (object) this.context.m_queryMaxGrantPercent));
      if (this.context.IsForceOrderEnabled || this.context.IsFullTextJoinOptimizationEnabled && this.context.IsLongTextPredicateAppended)
        sqlStatement1.Options.Add("FORCE ORDER");
      if (!this.context.IsAllowNonClusteredColumnstoreIndexEnabled)
        sqlStatement1.Options.Add("IGNORE_NONCLUSTERED_COLUMNSTORE_INDEX");
      int num1 = CommonWITUtils.HasCrossProjectQueryPermission(this.context.m_requestContext) ? this.context.m_requestContext.WitContext().ServerSettings.MaxQueryResultSize : this.context.m_requestContext.WitContext().ServerSettings.MaxQueryResultSizeForPublicUser;
      int num2 = topCount;
      if (num2 == 0 || num2 > num1)
        num2 = num1;
      sqlStatement1.Top = new int?(num2 == int.MaxValue ? num2 : num2 + 1);
      if (queryExpression.QueryType == QueryType.LinksRecursiveMayContain && queryExpression.RecursionOption == QueryRecursionOption.ParentFirst)
      {
        StringBuilder stringBuilder = new StringBuilder();
        int num3 = sqlStatement1.Top.Value;
        sqlStatement1.Top = new int?(int.MaxValue);
        string sql = sqlStatement1.GetSql();
        bool sortingFixEnabled = this.context.IsUsingRowNumberForSortingFixEnabled;
        bool flag = false;
        if (sortingFixEnabled && queryExpression.SortFields.Any<QuerySortField>())
        {
          string str2;
          List<string> list = queryExpression.SortFields.Where<QuerySortField>((Func<QuerySortField, bool>) (sortField => sortFieldColumnMap.TryGetValue(sortField, out str2) && !string.IsNullOrWhiteSpace(str2))).Select<QuerySortField, string>((Func<QuerySortField, string>) (sortField => QueryPredicate.BuildOrderBy(sortField.Field, sortFieldColumnMap[sortField], sortField.Descending, sortField.NullsFirst))).ToList<string>();
          if (list.Any<string>())
          {
            flag = true;
            sqlStatement1.OrderBy.Clear();
            sqlStatement1.Select.Add("ROW_NUMBER() OVER (ORDER BY " + string.Join(", ", (IEnumerable<string>) list) + ") as RowNumber");
          }
        }
        string format = sortingFixEnabled & flag ? "\r\nINSERT #results_filtered (SourceId, TargetId, IsLocked, MeetsParentCriteria, MeetsChildCriteria, TargetLatestAreaId, Level, IsRemoveable, RowNumber)\r\n{0}\r\n" : "\r\nINSERT #results_filtered (SourceId, TargetId, IsLocked, MeetsParentCriteria, MeetsChildCriteria, TargetLatestAreaId, Level, IsRemoveable)\r\n{0}\r\n";
        stringBuilder.Append(string.Format(format, (object) sqlStatement1.GetSql()));
        stringBuilder.Append(" \r\nDECLARE @maxLevel INT\r\n\r\nSELECT TOP 1 @maxLevel = Level -- Get the deepest children level \r\nFROM #results_filtered\r\nORDER BY Level DESC\r\n\r\nWHILE @maxLevel > 4 -- Do not process on root level (Level = 4), they are always preserved\r\nBEGIN\r\n    UPDATE     L\r\n    SET        IsRemoveable = 1\r\n    FROM       #results_filtered L\r\n    LEFT JOIN  #results_filtered R \r\n    ON         L.TargetId = R.SourceId\r\n               AND R.Level = @maxLevel + 4 \r\n               AND R.IsRemoveable = 0 -- Non-removeable children \r\n    WHERE      L.Level = @maxLevel  \r\n               AND L.MeetsChildCriteria = 0 -- this node can't be a child\r\n               AND L.MeetsParentCriteria = 0 -- this node can't be a parent\r\n               AND R.Level IS NULL -- this node does not have non-removeable children\r\n\r\n    SET @maxLevel = @maxLevel - 4\r\nEND\r\n");
        SqlStatement sqlStatement12 = new SqlStatement("#results_filtered", "FilteredResults", false);
        sqlStatement12.Select.AddRange((IEnumerable<string>) new string[6]
        {
          "SourceId",
          "TargetId",
          "IsLocked",
          "MeetsParentCriteria",
          "MeetsChildCriteria",
          "TargetLatestAreaId"
        });
        sqlStatement12.AppendPredicate("IsRemoveable = 0");
        if (sortingFixEnabled & flag)
          sqlStatement12.OrderBy.Add("RowNumber");
        stringBuilder.Append(sqlStatement12.GetSql());
        sqlStatement1.Top = new int?(num3);
        this.context.m_queryText.Append(string.Format("\r\nSELECT @Count = COUNT(*) FROM #results\r\nIF (@Count >= {0})\r\nBEGIN\r\n{1}\r\nEND\r\nELSE\r\nBEGIN\r\n{2}\r\nEND\r\n", (object) sqlStatement1.Top, (object) this.IndentSql(stringBuilder.ToString(), 1), (object) this.IndentSql(sql, 1)));
      }
      else
        this.context.m_queryText.AppendLine(sqlStatement1.GetSql());
      this.context.m_queryText.Append("SELECT CONVERT(BIT, ");
      StringBuilder stringBuilder1 = new StringBuilder();
      if (this.context.m_requestContext.GetService<WorkItemTrackingTreeService>().HasPendingReclassification && (this.context.m_unhandledTreeReferenceCount > 0 || this.context.m_treeReference.Count > 0))
      {
        if (this.context.m_unhandledTreeReferenceCount > 0 || this.context.m_treeReference.Any<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType>>((Func<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType>, bool>) (x => x.Value == TreeReferenceType.All)))
        {
          this.context.m_queryText.Append("1");
        }
        else
        {
          IEnumerable<WorkItemTrackingTreeService.ClassificationNodeId> classificationNodeIds1 = this.context.m_treeReference.Where<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType>>((Func<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType>, bool>) (x => x.Value == TreeReferenceType.In)).Select<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType>, WorkItemTrackingTreeService.ClassificationNodeId>((Func<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType>, WorkItemTrackingTreeService.ClassificationNodeId>) (x => x.Key));
          if (classificationNodeIds1.Any<WorkItemTrackingTreeService.ClassificationNodeId>())
          {
            this.context.m_tableValuedParameters.Add((ITableValuedParameter) new QuerySqlComponent.ClassificationNodeIdTable("@inNodes", classificationNodeIds1));
            stringBuilder1.AppendFormat("EXISTS (\r\n    SELECT *\r\n    FROM @inNodes t\r\n    JOIN dbo.tbl_ClassificationNodeChange c\r\n        ON c.PartitionId = {0}\r\n        AND c.DataspaceId = t.[Key]\r\n        AND c.Id = t.Value\r\n        AND c.IsWorkItemReconciled = 0\r\n)", (object) this.context.PartitionId);
          }
          IEnumerable<WorkItemTrackingTreeService.ClassificationNodeId> classificationNodeIds2 = this.context.m_treeReference.Where<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType>>((Func<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType>, bool>) (x => x.Value == TreeReferenceType.NotIn)).Select<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType>, WorkItemTrackingTreeService.ClassificationNodeId>((Func<KeyValuePair<WorkItemTrackingTreeService.ClassificationNodeId, TreeReferenceType>, WorkItemTrackingTreeService.ClassificationNodeId>) (x => x.Key));
          if (classificationNodeIds2.Any<WorkItemTrackingTreeService.ClassificationNodeId>())
          {
            if (stringBuilder1.Length > 0)
              stringBuilder1.Append(" OR ");
            this.context.m_tableValuedParameters.Add((ITableValuedParameter) new QuerySqlComponent.ClassificationNodeIdTable("@notInNodes", classificationNodeIds2));
            stringBuilder1.AppendFormat("EXISTS (\r\n    SELECT *\r\n    FROM dbo.tbl_ClassificationNodeChange c\r\n    WHERE c.PartitionId = {0}\r\n        AND c.IsWorkItemReconciled = 0\r\n        AND NOT EXISTS (\r\n            SELECT *\r\n            FROM @notInNodes\r\n            WHERE [Key] = c.DataspaceId\r\n                AND Value = c.Id\r\n        )\r\n)", (object) this.context.PartitionId);
          }
          this.context.m_queryText.Append("CASE WHEN ");
          this.context.m_queryText.Append(stringBuilder1.ToString());
          this.context.m_queryText.Append(" THEN 1 ELSE 0 END");
        }
      }
      else
        this.context.m_queryText.Append("0");
      this.context.m_queryText.AppendLine(")");
      this.context.m_queryText.Append("SELECT ");
      this.context.m_queryText.AppendLine(this.context.m_asOfParam == null ? "GETUTCDATE()" : this.context.m_asOfParam);
      QueryExecutionDetail queryExecutionDetail = new QueryExecutionDetail()
      {
        QueryText = this.context.m_queryText.ToString(),
        Parameters = (IEnumerable<SqlParameter>) this.context.m_parameters,
        TableValuedParameters = (IEnumerable<ITableValuedParameter>) this.context.m_tableValuedParameters,
        Top = topCount,
        SizeLimit = num1,
        QueryCategory = this.SummarizeQueryCategory(),
        FieldsDoStringComparison = this.context.FieldsDoStringComparison
      };
      this.context.m_requestContext.Trace(906026, TraceLevel.Verbose, "Query", nameof (QueryProcessor), "Number of parameters: {0}. Number of parameters: {1}. Top : {2}", (object) this.context.m_parameters.Count, (object) this.context.m_tableValuedParameters.Count, (object) queryExecutionDetail.Top);
      this.context.m_requestContext.Trace(906047, TraceLevel.Verbose, "Query", nameof (QueryProcessor), string.Format("Generated SQL: {0}", (object) this.context.m_queryText));
      return queryExecutionDetail;
    }

    private QueryCategory SummarizeQueryCategory()
    {
      QueryCategory queryCategory = QueryCategory.None;
      if (this.context.m_isLongTextPredicateAppended && this.context.m_supportsFullTextSearch)
        queryCategory |= QueryCategory.FullTextSearchQuery;
      if (this.context.m_isChartingQuery && this.context.m_isAsOfQuery)
        queryCategory |= QueryCategory.ChartingAsOfQuery;
      if (this.context.m_isIdentityInGroupQuery)
        queryCategory |= QueryCategory.IdentityInGroupQuery;
      if (this.context.m_isIdentityComparisonQuery)
        queryCategory |= QueryCategory.IdentityComparisonQuery;
      if (this.context.m_isDirectlyJoinOnLongTable)
        queryCategory |= QueryCategory.CustomLatestTableQuery;
      if (this.context.m_isEverPredicateAppended)
        queryCategory |= QueryCategory.WasEverQuery;
      if (this.context.m_isLowerLevelOrClauseQuery)
        queryCategory |= QueryCategory.LowerLevelOrClauseQuery;
      return queryCategory;
    }

    private SqlStatement CreateSqlStatement(
      QueryExpression queryExpression,
      QueryTableAlias tableAlias,
      Dictionary<QuerySortField, string> sortFieldColumnMap)
    {
      string mainTable = (string) null;
      string mainTableAlias = (string) null;
      QueryExpressionNode queryGroup = (QueryExpressionNode) null;
      bool isTreeQuerySelectStatement = false;
      switch (tableAlias)
      {
        case QueryTableAlias.Right:
          mainTable = QueryPredicate.GetWorkItemView(true, this.context.m_isAsOfQuery, true, queryExpression.IsParentQuery);
          mainTableAlias = "rhs";
          queryGroup = queryExpression.RightGroup;
          isTreeQuerySelectStatement = queryExpression.QueryType == QueryType.LinksRecursiveMayContain && queryExpression.RecursionOption == QueryRecursionOption.ParentFirst;
          break;
        case QueryTableAlias.Link:
          mainTable = QueryPredicate.GetLinkView(this.context);
          mainTableAlias = "links";
          queryGroup = queryExpression.LinkGroup;
          break;
        default:
          mainTable = QueryPredicate.GetWorkItemView(true, this.context.m_isAsOfQuery, true, queryExpression.IsParentQuery);
          mainTableAlias = "lhs";
          queryGroup = queryExpression.LeftGroup;
          isTreeQuerySelectStatement = queryExpression.QueryType == QueryType.LinksRecursiveMayContain && queryExpression.RecursionOption == QueryRecursionOption.ChildFirst;
          break;
      }
      bool canOptimizeForUnion = tableAlias != QueryTableAlias.Link && !isTreeQuerySelectStatement;
      SqlStatement sqlStatement = (SqlStatement) null;
      this.context.m_requestContext.TraceBlock(906022, 906023, "Query", nameof (QueryProcessor), nameof (CreateSqlStatement), (Action) (() => sqlStatement = this.CreateSqlStatement(mainTable, mainTableAlias, queryGroup, canOptimizeForUnion, (Action<SqlStatement, bool>) ((statement, isUnion) =>
      {
        bool flag = false;
        if (!this.context.m_hasDeletedFilter && tableAlias != QueryTableAlias.Link)
        {
          statement.AppendPredicate("{0}.IsDeleted = {1}", mainTableAlias, "0");
          flag = true;
        }
        if (!isTreeQuerySelectStatement)
        {
          string str = string.Empty;
          if (flag)
            str = " AND ";
          statement.AppendPredicate("{0}{1}.PartitionId = {2}", str, mainTableAlias, this.context.PartitionId);
          if (queryExpression.QueryType != QueryType.LinksRecursiveMayContain)
            QueryPredicate.AddDateFilter(this.context, statement, mainTableAlias, !isUnion && tableAlias != QueryTableAlias.Link);
        }
        if (!isUnion)
          return;
        string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.Id", (object) mainTableAlias);
        if (this.context.m_isAsOfQuery)
          str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, {1}.[System.Rev]", (object) str1, (object) mainTableAlias);
        statement.Select.Add(str1);
      }))));
      if (sqlStatement is SqlUnionStatement)
      {
        sqlStatement.TableAlias = "ids";
        string condition = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.Id = ids.Id AND {0}.PartitionId = {1}", (object) mainTableAlias, (object) this.context.PartitionId);
        if (this.context.m_isAsOfQuery)
          condition = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} AND {1}.[System.Rev] = ids.[System.Rev]", (object) condition, (object) mainTableAlias);
        sqlStatement.DefineJoin(QueryPredicate.GetWorkItemView(true, this.context.m_isAsOfQuery, true), mainTableAlias, condition);
        QueryPredicate.AddDateFilter(this.context, sqlStatement, mainTableAlias, true);
      }
      if (tableAlias == QueryTableAlias.Link)
      {
        sqlStatement.Select.Add("links.SourceID AS SourceId");
        if (queryExpression.QueryType != QueryType.LinksOneHopDoesNotContain)
          sqlStatement.Select.AddRange((IEnumerable<string>) new string[3]
          {
            "links.TargetID AS TargetId",
            "links.[System.Links.LinkType] as LinkTypeId",
            "links.Lock AS IsLocked"
          });
      }
      else
      {
        string str2 = tableAlias != QueryTableAlias.Left || !queryExpression.QueryType.IsOneHopQuery() ? string.Empty : "Source";
        sqlStatement.Select.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.Id AS {1}Id", (object) mainTableAlias, (object) str2));
        if (((tableAlias == QueryTableAlias.Left || tableAlias == QueryTableAlias.Right ? 1 : (queryGroup != null ? 1 : 0)) | (isTreeQuerySelectStatement ? 1 : 0)) != 0)
        {
          string fullColumnName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}.AreaPath", (object) mainTableAlias, this.context.m_isAsOfQuery ? (object) "L" : (object) string.Empty);
          sqlStatement.Select.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} AS {1}LatestAreaId", (object) QueryPredicate.TreePathToId(fullColumnName), (object) str2));
        }
        if (this.context.m_asOfDatesParam != null)
        {
          string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.[System.Rev]", (object) mainTableAlias);
          sqlStatement.Select.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} AS {1}Revision", (object) str3, (object) str2));
          string str4 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.Val", (object) "AsOfDates");
          sqlStatement.Select.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} AS {1}AsOf", (object) str4, (object) str2));
        }
      }
      this.context.m_requestContext.TraceBlock(906024, 906025, "Query", nameof (QueryProcessor), "ProcessSortFields", (Action) (() =>
      {
        foreach (QuerySortField key in queryExpression.SortFields.Where<QuerySortField>((Func<QuerySortField, bool>) (x => queryExpression.QueryType == QueryType.LinksRecursiveMayContain ? isTreeQuerySelectStatement : x.TableAlias == tableAlias)))
        {
          string str5;
          if (key.Field.FieldId == 100)
          {
            if (queryExpression.QueryType.IsOneHopQuery())
            {
              str5 = "links.LinkTypeName";
              sqlStatement.Select.Add("linkTypes.LinkType AS LinkTypeName");
              sqlStatement.DefineJoin("dbo.LinkTypeNames", "linkTypes", "linkTypes.PartitionId = links.PartitionId AND linkTypes.ID = links.[System.Links.LinkType]");
            }
            else if (queryExpression.QueryType == QueryType.LinksRecursiveMayContain && queryExpression.SortFields.Count<QuerySortField>() == 1)
              str5 = "Results.TargetId";
            else
              continue;
          }
          else
          {
            str5 = QueryPredicate.GetColumn(this.context, key.Field, mainTableAlias, sqlStatement, forOrderBy: true);
            if (tableAlias == QueryTableAlias.Right | isTreeQuerySelectStatement)
            {
              string str6 = "F" + key.Field.FieldId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo).Replace("-", "_");
              sqlStatement.Select.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} AS {1}", (object) str5, (object) str6));
              str5 = !queryExpression.QueryType.IsOneHopQuery() ? str6 : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", tableAlias != QueryTableAlias.Right || queryExpression.QueryType != QueryType.LinksOneHopMayContain ? (object) mainTableAlias : (object) "links", (object) str6);
            }
          }
          sortFieldColumnMap.Add(key, str5);
        }
      }));
      return sqlStatement;
    }

    private SqlStatement CreateSqlStatement(
      string workItemsTable,
      string workItemsTableAlias,
      QueryExpressionNode group,
      bool canOptimizeForUnion,
      Action<SqlStatement, bool> configure)
    {
      if (group == null)
      {
        SqlStatement sqlStatement = new SqlStatement(workItemsTable, workItemsTableAlias, this.context.IsLeftJoinIndexHintEnabled);
        configure(sqlStatement, false);
        return sqlStatement;
      }
      bool isUnion = false;
      QueryLogicalExpressionNode logicalNode = group as QueryLogicalExpressionNode;
      if (logicalNode != null & canOptimizeForUnion)
        this.PushDownProjectPredicate(logicalNode);
      if (OrClauseLevelRewriter.CanRewrite(logicalNode))
      {
        this.context.m_isLowerLevelOrClauseQuery = true;
        if (this.context.m_isMoveOrClauseUpEnabled)
          OrClauseLevelRewriter.Rewrite(logicalNode);
      }
      IEnumerable<QueryExpressionNode> source;
      if (((logicalNode == null || logicalNode.Operator != QueryLogicalExpressionOperator.Or ? 0 : (logicalNode.Children.Count<QueryExpressionNode>() <= this.context.m_topLevelOrOptimizationMaxClauseNumber ? 1 : 0)) & (canOptimizeForUnion ? 1 : 0)) != 0)
      {
        isUnion = true;
        source = logicalNode.Children;
      }
      else
        source = (IEnumerable<QueryExpressionNode>) new QueryExpressionNode[1]
        {
          group
        };
      SqlStatement[] array = source.Select<QueryExpressionNode, SqlStatement>((Func<QueryExpressionNode, SqlStatement>) (g =>
      {
        SqlStatement statement = new SqlStatement(workItemsTable, workItemsTableAlias, this.context.IsLeftJoinIndexHintEnabled);
        configure(statement, isUnion);
        if (!string.IsNullOrWhiteSpace(statement.GetPredicate()))
          statement.AppendPredicate(" AND ");
        QueryPredicate.AppendExpressionNodePredicate(this.context, workItemsTableAlias, g, statement);
        return statement;
      })).ToArray<SqlStatement>();
      return isUnion ? (SqlStatement) new SqlUnionStatement((IEnumerable<SqlStatement>) array, workItemsTableAlias) : ((IEnumerable<SqlStatement>) array).First<SqlStatement>();
    }

    private void PushDownProjectPredicate(QueryLogicalExpressionNode logicalNode)
    {
      if (logicalNode.Operator != QueryLogicalExpressionOperator.And || logicalNode.Children.Count<QueryExpressionNode>() != 2)
        return;
      List<QueryExpressionNode> list = logicalNode.Children.ToList<QueryExpressionNode>();
      if (!(list[0] is QueryComparisonExpressionNode) || !(list[1] is QueryLogicalExpressionNode))
        return;
      QueryComparisonExpressionNode comparisonExpressionNode = list[0] as QueryComparisonExpressionNode;
      QueryLogicalExpressionNode logicalExpressionNode = list[1] as QueryLogicalExpressionNode;
      if (comparisonExpressionNode.Operator != QueryExpressionOperator.Under || !(comparisonExpressionNode.Field.ReferenceName == "System.AreaId") || logicalExpressionNode.Operator != QueryLogicalExpressionOperator.Or || logicalExpressionNode.Children.Count<QueryExpressionNode>() > this.context.m_topLevelOrOptimizationMaxClauseNumber)
        return;
      logicalNode.Operator = QueryLogicalExpressionOperator.Or;
      List<QueryLogicalExpressionNode> logicalExpressionNodeList = new List<QueryLogicalExpressionNode>();
      foreach (QueryExpressionNode child in logicalExpressionNode.Children)
        logicalExpressionNodeList.Add(new QueryLogicalExpressionNode()
        {
          Operator = QueryLogicalExpressionOperator.And,
          Children = (IEnumerable<QueryExpressionNode>) new List<QueryExpressionNode>()
          {
            (QueryExpressionNode) comparisonExpressionNode,
            child
          }
        });
      logicalNode.Children = (IEnumerable<QueryExpressionNode>) logicalExpressionNodeList;
    }

    private void AddQueryHashTag(QueryExpression queryExpression)
    {
      if (this.context.m_isQueryHashTagDisabled)
        return;
      this.context.m_queryText.AppendLine("-- QueryHash: " + (queryExpression.QueryHash ?? "(null)"));
    }

    private string IndentSql(string sqlStatements, int indentLevel)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = sqlStatements;
      string[] separator = new string[1]
      {
        Environment.NewLine
      };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.None))
      {
        stringBuilder.Append(' ', indentLevel * 4);
        stringBuilder.AppendLine(str2);
      }
      return stringBuilder.ToString();
    }
  }
}
