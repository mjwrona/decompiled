// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.QueryPredicate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor
{
  internal static class QueryPredicate
  {
    private static readonly string initialLongTextTableAlias = "WLT";
    private static readonly Dictionary<int, string> sm_corePersonFields = new Dictionary<int, string>()
    {
      {
        -1,
        "AuthorizedAs"
      },
      {
        33,
        "CreatedBy"
      },
      {
        9,
        "ChangedBy"
      },
      {
        24,
        "AssignedTo"
      }
    };

    public static string GetColumn(
      QueryProcessorContext context,
      FieldEntry field,
      string workItemsTableAlias,
      SqlStatement statement,
      bool expandReference = true,
      bool forOrderBy = false,
      string fieldStringValue = null)
    {
      string tableAlias1;
      string columnName;
      if ((field.StorageTarget & FieldStorageTarget.WideTable) != FieldStorageTarget.Unknown || field.FieldId == 100)
      {
        tableAlias1 = workItemsTableAlias;
        columnName = "[" + field.ReferenceName + "]";
      }
      else
      {
        tableAlias1 = workItemsTableAlias + "F" + field.FieldId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo).Replace("-", "_");
        string condition1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.PartitionId = {1}.PartitionId AND {0}.Id = {1}.Id AND {1}.FieldId = {2}", (object) workItemsTableAlias, (object) tableAlias1, (object) field.FieldId);
        if (context.m_isAsOfQuery)
        {
          string condition2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} AND {1}.AuthorizedDate <= {2}.AuthorizedDate AND {1}.RevisedDate >= {2}.RevisedDate", (object) condition1, (object) tableAlias1, (object) workItemsTableAlias);
          statement.DefineJoin("LEFT", QueryPredicate.GetWorkItemView(false, true, hasParentasof: context.m_isParentQuery), tableAlias1, condition2);
        }
        else
        {
          context.m_isDirectlyJoinOnLongTable = true;
          statement.DefineJoin("LEFT", QueryPredicate.GetWorkItemView(false, false), tableAlias1, condition1);
        }
        columnName = QueryPredicate.GetLongTableColumn(field);
      }
      if (field.IsPerson)
      {
        if (expandReference && (field.StorageTarget & FieldStorageTarget.LongTable) != FieldStorageTarget.Unknown)
        {
          string tableAlias2 = workItemsTableAlias + "C" + field.FieldId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo).Replace("-", "_");
          string condition = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.PartitionId = {1}.PartitionId AND {0}.ConstID = {1}.{2}", (object) tableAlias2, (object) tableAlias1, (object) columnName);
          statement.DefineJoin("LEFT", "dbo.Constants", tableAlias2, condition);
          tableAlias1 = tableAlias2;
          columnName = "DisplayPart";
        }
        else if (!expandReference && (field.StorageTarget & FieldStorageTarget.WideTable) != FieldStorageTarget.Unknown)
          columnName = QueryPredicate.sm_corePersonFields[field.FieldId];
      }
      string fullColumnName = tableAlias1 + "." + columnName;
      if (forOrderBy && (field.IsAreaPath || field.IsIterationPath || field.IsPortfolioProject))
        fullColumnName = QueryPredicate.GetOrderByColumnForStructureField(context, field, workItemsTableAlias, statement, columnName, fullColumnName);
      return fullColumnName;
    }

    private static string GetOrderByColumnForStructureField(
      QueryProcessorContext context,
      FieldEntry field,
      string workItemsTableAlias,
      SqlStatement statement,
      string columnName,
      string fullColumnName)
    {
      if (string.IsNullOrEmpty(context.m_dataspaceIdToProjectNameMapTVPName))
      {
        IDictionary<int, string> idProjectNameMap = QueryPredicate.GetDataspaceIdProjectNameMap(context);
        context.m_dataspaceIdToProjectNameMapTVPName = QueryParameter.DefineTableValuedParameter<KeyValuePair<int, string>>(context, (WorkItemTrackingTableValueParameter<KeyValuePair<int, string>>) new Microsoft.TeamFoundation.WorkItemTracking.Server.Int32StringTable((IEnumerable<KeyValuePair<int, string>>) idProjectNameMap));
        context.m_queryText.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\r\n                        CREATE TABLE #{0}\r\n                        (\r\n                            DataspaceId INT NOT NULL,\r\n                            ProjectName NVARCHAR(256) COLLATE DATABASE_DEFAULT NOT NULL,\r\n                            UNIQUE CLUSTERED (DataspaceId, ProjectName) WITH (IGNORE_DUP_KEY=ON)\r\n                        )\r\n\r\n                        INSERT #{0}\r\n                        SELECT [Key], [Value]\r\n                        FROM {1}\r\n                        ", (object) "DataspaceIdProjectMap", (object) context.m_dataspaceIdToProjectNameMapTVPName);
      }
      string condition = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.DataspaceId = {1}.DataspaceId", (object) workItemsTableAlias, (object) "DataspaceIdProjectMap", (object) columnName);
      statement.DefineJoin("LEFT", "#DataspaceIdProjectMap", "DataspaceIdProjectMap", condition);
      fullColumnName = !field.IsPortfolioProject ? "ISNULL(DataspaceIdProjectMap.ProjectName,'') + ISNULL(" + fullColumnName + ",'')" : "ISNULL(DataspaceIdProjectMap.ProjectName,'')";
      return fullColumnName;
    }

    public static void AppendExpressionNodePredicate(
      QueryProcessorContext context,
      string workItemsTableAlias,
      QueryExpressionNode expressionNode,
      SqlStatement statement)
    {
      context.m_requestContext.TraceBlock(906039, 906040, "Query", "QueryProcessor", nameof (AppendExpressionNodePredicate), (Action) (() =>
      {
        if (expressionNode == null)
          return;
        statement.AppendPredicate("(");
        if (expressionNode is QueryComparisonExpressionNode)
        {
          QueryPredicate.AppendComparisonNodePredicate(context, workItemsTableAlias, (QueryComparisonExpressionNode) expressionNode, statement);
        }
        else
        {
          QueryLogicalExpressionNode logicalExpressionNode = (QueryLogicalExpressionNode) expressionNode;
          string format = logicalExpressionNode.Operator == QueryLogicalExpressionOperator.And ? " AND " : " OR ";
          bool flag = true;
          foreach (QueryExpressionNode child in logicalExpressionNode.Children)
          {
            if (flag)
              flag = false;
            else
              statement.AppendPredicate(format);
            QueryPredicate.AppendExpressionNodePredicate(context, workItemsTableAlias, child, statement);
          }
        }
        statement.AppendPredicate(")");
      }));
    }

    private static void AppendComparisonNodePredicate(
      QueryProcessorContext context,
      string workItemsTableAlias,
      QueryComparisonExpressionNode expressionNode,
      SqlStatement statement)
    {
      FieldEntry field = expressionNode.Field;
      QueryExpressionOperator op = expressionNode.Operator;
      QueryExpressionValue queryExpressionValue = expressionNode.Value;
      bool expandConstant = expressionNode.ExpandConstant;
      if (field.IsIdentity)
        context.m_isIdentityComparisonQuery = true;
      if (field.FieldId == -2 || field.FieldId == -104 || field.FieldId == -7 || field.FieldId == -105)
        ++context.m_unhandledTreeReferenceCount;
      if (op == QueryExpressionOperator.Ever)
        QueryPredicate.AppendEverPredicate(context, workItemsTableAlias, expressionNode, statement);
      else if (field.FieldId == 80)
        QueryPredicateTag.AppendTagPredicate(context, workItemsTableAlias, expressionNode, statement);
      else if (QueryProcessorCommon.CanQueryAsText(context, expressionNode))
        QueryPredicate.AppendLongTextPredicate(context, workItemsTableAlias, expressionNode, statement);
      else if (queryExpressionValue.ValueType == QueryExpressionValueType.Column)
      {
        FieldEntry columnValue = queryExpressionValue.ColumnValue;
        string column1 = QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, fieldStringValue: queryExpressionValue.StringValue);
        string column2 = QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, false, fieldStringValue: queryExpressionValue.StringValue);
        string column3 = QueryPredicate.GetColumn(context, columnValue, workItemsTableAlias, statement, fieldStringValue: queryExpressionValue.StringValue);
        string column4 = QueryPredicate.GetColumn(context, columnValue, workItemsTableAlias, statement, false, fieldStringValue: queryExpressionValue.StringValue);
        StringBuilder stringBuilder = new StringBuilder();
        if (columnValue.IsIdentity)
          context.m_isIdentityComparisonQuery = true;
        switch (op)
        {
          case QueryExpressionOperator.Equals:
            stringBuilder.Append("{0} = {1} OR ({0} IS NULL AND {1} IS NULL)");
            if (field.IsIdentity)
              stringBuilder.Append(" OR dbo.func_GetDisplayPart({2}, 1) = {3}");
            if (columnValue.IsIdentity)
              stringBuilder.Append(" OR dbo.func_GetDisplayPart({3}, 1) = {2}");
            statement.AppendPredicate(stringBuilder.ToString(), context.IsQueryIdentityConstIdOptimizationEnabled ? column2 : column1, context.IsQueryIdentityConstIdOptimizationEnabled ? column4 : column3, column1, column3);
            break;
          case QueryExpressionOperator.NotEquals:
            if (field.IsIdentity || columnValue.IsIdentity)
            {
              stringBuilder.Append("({0} IS NOT NULL AND {1} IS NULL) OR ({0} IS NULL AND {1} IS NOT NULL)");
              stringBuilder.Append(" OR(");
              if (field.IsIdentity)
                stringBuilder.Append("dbo.func_GetDisplayPart({2}, 1) <> {3} AND ");
              if (columnValue.IsIdentity)
                stringBuilder.Append("dbo.func_GetDisplayPart({3}, 1) <> {2} AND ");
              stringBuilder.Append("{0} <> {1})");
              statement.AppendPredicate(stringBuilder.ToString(), context.IsQueryIdentityConstIdOptimizationEnabled ? column2 : column1, context.IsQueryIdentityConstIdOptimizationEnabled ? column4 : column3, column1, column3);
              break;
            }
            statement.AppendPredicate("{0} <> {1} OR ({0} IS NOT NULL AND {1} IS NULL) OR ({0} IS NULL AND {1} IS NOT NULL)", column1, column3);
            break;
          case QueryExpressionOperator.Less:
            statement.AppendPredicate("{0} < {1} OR ({0} IS NULL AND {1} IS NOT NULL)", column1, column3);
            break;
          case QueryExpressionOperator.LessEquals:
            statement.AppendPredicate("{0} <= {1} OR ({0} IS NULL AND {1} IS NULL) OR ({0} IS NULL AND {1} IS NOT NULL)", column1, column3);
            break;
          case QueryExpressionOperator.Greater:
            statement.AppendPredicate("{0} > {1} OR ({0} IS NOT NULL AND {1} IS NULL)", column1, column3);
            break;
          case QueryExpressionOperator.GreaterEquals:
            statement.AppendPredicate("{0} >= {1} OR ({0} IS NULL AND {1} IS NULL) OR ({0} IS NOT NULL AND {1} IS NULL)", column1, column3);
            break;
          default:
            throw new NotSupportedException();
        }
      }
      else if ((field.FieldId == -2 || field.FieldId == -104) && (op.IsUnder() || op.IsEquals()))
      {
        QueryPredicate.AppendTreeFieldPredicate(context, workItemsTableAlias, expressionNode, statement);
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder();
        switch (op)
        {
          case QueryExpressionOperator.Equals:
            if (queryExpressionValue.IsNull)
            {
              statement.AppendPredicate("{0} IS NULL", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, false, fieldStringValue: queryExpressionValue.StringValue));
              break;
            }
            if (field.FieldType == InternalFieldType.Boolean && !queryExpressionValue.BoolValue)
            {
              statement.AppendPredicate("({0} = {1} OR {0} IS NULL)", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, fieldStringValue: queryExpressionValue.StringValue), QueryParameter.DefineParameter(context, queryExpressionValue, field.IsPerson));
              break;
            }
            if (expandConstant)
            {
              QueryPredicate.AppendInGroupPredicate(context, workItemsTableAlias, expressionNode, statement);
              break;
            }
            statement.AppendPredicate("{0} = {1}", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, fieldStringValue: queryExpressionValue.StringValue), QueryParameter.DefineParameter(context, queryExpressionValue, field.IsPerson));
            break;
          case QueryExpressionOperator.NotEquals:
            if (queryExpressionValue.IsNull)
            {
              statement.AppendPredicate("{0} IS NOT NULL", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, false, fieldStringValue: queryExpressionValue.StringValue));
              break;
            }
            if (field.FieldType == InternalFieldType.Boolean && !queryExpressionValue.BoolValue)
            {
              statement.AppendPredicate("({0} <> {1} AND {0} IS NOT NULL)", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, fieldStringValue: queryExpressionValue.StringValue), QueryParameter.DefineParameter(context, queryExpressionValue, field.IsPerson));
              break;
            }
            if (expandConstant)
            {
              QueryPredicate.AppendInGroupPredicate(context, workItemsTableAlias, expressionNode, statement);
              break;
            }
            statement.AppendPredicate("{0} IS NULL OR {1} <> {2}", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, false, fieldStringValue: queryExpressionValue.StringValue), QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, fieldStringValue: queryExpressionValue.StringValue), QueryParameter.DefineParameter(context, queryExpressionValue, field.IsPerson));
            break;
          case QueryExpressionOperator.Less:
            if (queryExpressionValue.IsNull)
            {
              statement.AppendPredicate("0=1");
              break;
            }
            statement.AppendPredicate("{0} IS NULL OR {1} < {2}", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, false), QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement), QueryParameter.DefineParameter(context, queryExpressionValue, field.IsPerson));
            break;
          case QueryExpressionOperator.LessEquals:
            if (queryExpressionValue.IsNull)
            {
              statement.AppendPredicate("{0} IS NULL", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, false));
              break;
            }
            statement.AppendPredicate("{0} IS NULL OR {1} <= {2}", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, false), QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement), QueryParameter.DefineParameter(context, queryExpressionValue, field.IsPerson));
            break;
          case QueryExpressionOperator.Greater:
            if (queryExpressionValue.IsNull)
            {
              statement.AppendPredicate("{0} IS NOT NULL", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, false));
              break;
            }
            statement.AppendPredicate("{0} > {1}", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement), QueryParameter.DefineParameter(context, queryExpressionValue, field.IsPerson));
            break;
          case QueryExpressionOperator.GreaterEquals:
            if (queryExpressionValue.IsNull)
            {
              statement.AppendPredicate("1=1");
              break;
            }
            statement.AppendPredicate("{0} >= {1}", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement), QueryParameter.DefineParameter(context, queryExpressionValue, field.IsPerson));
            break;
          case QueryExpressionOperator.Contains:
            if (queryExpressionValue.IsNull)
            {
              statement.AppendPredicate("0=1");
              break;
            }
            statement.AppendPredicate("{0} LIKE {1}", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, fieldStringValue: queryExpressionValue.StringValue), QueryProcessorCommon.EncodeSqlString(QueryProcessorCommon.FixContainsValue(queryExpressionValue.StringValue, expressionNode.Field.ReferenceName)));
            break;
          case QueryExpressionOperator.NotContains:
            if (queryExpressionValue.IsNull)
            {
              statement.AppendPredicate("0=1");
              break;
            }
            statement.AppendPredicate("{0} IS NULL OR {1} NOT LIKE {2}", QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, false, fieldStringValue: queryExpressionValue.StringValue), QueryPredicate.GetColumn(context, field, workItemsTableAlias, statement, fieldStringValue: queryExpressionValue.StringValue), QueryProcessorCommon.EncodeSqlString(QueryProcessorCommon.FixContainsValue(queryExpressionValue.StringValue, expressionNode.Field.ReferenceName)));
            break;
          case QueryExpressionOperator.In:
          case QueryExpressionOperator.NotIn:
            QueryPredicate.AppendInPredicate(context, workItemsTableAlias, expressionNode, statement);
            break;
          default:
            throw new NotSupportedException();
        }
      }
    }

    private static void AppendEverPredicate(
      QueryProcessorContext context,
      string workItemsTableAlias,
      QueryComparisonExpressionNode expressionNode,
      SqlStatement statement)
    {
      context.m_requestContext.TraceBlock(906027, 906028, "Query", "QueryProcessor", nameof (AppendEverPredicate), (Action) (() =>
      {
        string str1 = string.Empty;
        string empty = string.Empty;
        string str2 = string.Empty;
        string workItemView;
        string str3;
        string str4;
        if ((expressionNode.Field.StorageTarget & FieldStorageTarget.WideTable) != FieldStorageTarget.Unknown)
        {
          workItemView = QueryPredicate.GetWorkItemView(true, true);
          str3 = "WE";
          str4 = str3 + ".[" + expressionNode.Field.ReferenceName + "]";
        }
        else
        {
          workItemView = QueryPredicate.GetWorkItemView(false, true);
          int fieldId = expressionNode.Field.FieldId;
          str3 = "WE_F" + fieldId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo).Replace('-', '_');
          string longTableColumn = QueryPredicate.GetLongTableColumn(expressionNode.Field);
          str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AND {0}.FieldId = {1}", (object) str3, (object) expressionNode.Field.FieldId);
          if (expressionNode.Field.IsPerson)
          {
            fieldId = expressionNode.Field.FieldId;
            string str5 = "WE_C" + fieldId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo).Replace('-', '_');
            str2 = string.Format("\r\nLEFT JOIN dbo.Constants {0}\r\nON {0}.PartitionId = {1}.PartitionId\r\nAND {0}.ConstID = {1}.{2}", (object) str5, (object) str3, (object) longTableColumn);
            str4 = str5 + ".DisplayPart";
          }
          else
            str4 = str3 + "." + longTableColumn;
        }
        string str6 = !expressionNode.Value.IsNull ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} = {1}", (object) str4, (object) QueryParameter.DefineParameter(context, expressionNode.Value, expressionNode.Field.IsPerson)) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} IS NULL", (object) str4);
        statement.AppendPredicate("EXISTS (\r\nSELECT *\r\nFROM {0} {1} {2}\r\nWHERE {1}.PartitionId = {3}.PartitionId\r\nAND {1}.Id = {3}.Id\r\nAND {1}.AuthorizedDate <= {3}.AuthorizedDate\r\n{4}\r\nAND {5})", workItemView, str3, str2, workItemsTableAlias, str1, str6);
        context.m_isEverPredicateAppended = true;
      }));
    }

    private static void AppendLongTextPredicate(
      QueryProcessorContext context,
      string workItemsTableAlias,
      QueryComparisonExpressionNode expressionNode,
      SqlStatement statement)
    {
      context.m_requestContext.TraceBlock(906031, 906032, "Query", "QueryProcessor", nameof (AppendLongTextPredicate), (Action) (() =>
      {
        bool flag = expressionNode.Operator.IsNegated() ^ expressionNode.Operator.UsesIsEmpty();
        IEnumerable<QueryExpressionValue> source;
        if (expressionNode.Value.ValueType == QueryExpressionValueType.Array)
          source = (IEnumerable<QueryExpressionValue>) expressionNode.Value.GetArrayValue();
        else
          source = (IEnumerable<QueryExpressionValue>) new QueryExpressionValue[1]
          {
            expressionNode.Value
          };
        string searchValue = string.Join(" OR ", source.Select<QueryExpressionValue, string>((Func<QueryExpressionValue, string>) (expressionValue =>
        {
          string str = expressionValue.StringValue;
          if (!expressionNode.Operator.IsFullTextContains() && !expressionNode.Operator.UsesIsEmpty())
            str = context.m_supportsFullTextSearch ? QueryProcessorCommon.FixContainsValueFullText(str, expressionNode.Field.ReferenceName) : QueryProcessorCommon.FixContainsValue(str, expressionNode.Field.ReferenceName);
          return str;
        })));
        string longTextTableAlias = QueryPredicate.initialLongTextTableAlias;
        string table = "dbo.WorkItemLongTexts";
        bool forceFullTextIndexHint = context.IsFullTextIndexHintEnabled && expressionNode.Operator.IsContainsWords();
        IEnumerable<string> values = expressionNode.Fields.OrderBy<FieldEntry, int>((Func<FieldEntry, int>) (x => x.FieldId)).Select<FieldEntry, string>((Func<FieldEntry, string>) (x => x.FieldId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)));
        if (context.IsFullTextJoinOptimizationEnabled && !expressionNode.Operator.IsEverable())
        {
          string str1 = longTextTableAlias + string.Join(string.Empty, expressionNode.Fields.Select<FieldEntry, string>((Func<FieldEntry, string>) (x => x.FieldId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo).Replace("-", "_"))));
          string str2 = " AND " + str1 + ".FldID in (" + string.Join(",", values) + ")" + QueryPredicate.GetLongTextDatePredicateString(context, str1);
          statement.DefineJoin("LEFT", table, str1, workItemsTableAlias + ".Id=" + str1 + ".Id AND " + workItemsTableAlias + ".PartitionId = " + str1 + ".PartitionId" + str2);
          QueryPredicate.AppendCommonLongTextPredicate(context, str1, searchValue, expressionNode, statement);
        }
        else if (context.IsFullTextResultInTempTableEnabled && context.m_asOfDatesParam == null)
        {
          SqlStatement statement1 = new SqlStatement(table, longTextTableAlias, false, forceFullTextIndexHint);
          statement1.Select.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.ID", (object) longTextTableAlias));
          statement1.AppendPredicate("{0}.PartitionId = {1}\r\nAND {0}.FldID in ({2})\r\nAND ", longTextTableAlias, context.PartitionId, string.Join(",", values));
          bool alwaysCheckingNonEmpty = WorkItemTrackingFeatureFlags.IsAlwaysCheckNonEmptyInFullTextTempTableEnabled(context.m_requestContext);
          QueryPredicate.AppendCommonLongTextPredicate(context, longTextTableAlias, searchValue, expressionNode, statement1, alwaysCheckingNonEmpty);
          int tempTableId;
          if (QueryPredicateCommon.GetTempTableId(context, expressionNode, out tempTableId))
            context.m_queryText.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\r\nCREATE TABLE #temp{0}\r\n(\r\n    id INT NOT NULL,\r\n    UNIQUE CLUSTERED (id) WITH (IGNORE_DUP_KEY=ON)\r\n)\r\n\r\nINSERT #temp{0}\r\n{1}\r\n", (object) tempTableId, (object) statement1.GetSql());
          statement.AppendPredicate("{0}.Id {1}IN (SELECT * FROM #temp{2})", workItemsTableAlias, flag ? "NOT " : string.Empty, tempTableId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
        }
        else
        {
          statement.AppendPredicate("\r\n{0}.Id {1}IN (SELECT {2}.ID FROM {3} {2}{6}\r\nWHERE {2}.PartitionId = {4}\r\nAND {2}.FldID in ({5})\r\nAND ", workItemsTableAlias, flag ? "NOT " : string.Empty, longTextTableAlias, table, context.PartitionId, string.Join(",", values), forceFullTextIndexHint ? " WITH (FORCESEEK(IX_WorkItemLongTexts_PartitionTextID(PartitionTextID)))" : string.Empty);
          QueryPredicate.AppendCommonLongTextPredicate(context, longTextTableAlias, searchValue, expressionNode, statement, true);
          statement.AppendPredicate(")");
        }
        context.m_isLongTextPredicateAppended = true;
      }));
    }

    private static void AppendCommonLongTextPredicate(
      QueryProcessorContext context,
      string longTextTableAlias,
      string searchValue,
      QueryComparisonExpressionNode expressionNode,
      SqlStatement statement,
      bool alwaysCheckingNonEmpty = false)
    {
      if (expressionNode.Operator == QueryExpressionOperator.IsNotEmpty || expressionNode.Operator.UsesIsEmpty() & alwaysCheckingNonEmpty)
        statement.AppendPredicate("{0}.IsEmpty = 0", longTextTableAlias);
      else if (expressionNode.Operator == QueryExpressionOperator.IsEmpty)
      {
        statement.AppendPredicate("COALESCE({0}.IsEmpty, 1) = 1", longTextTableAlias);
      }
      else
      {
        string str = "{2}CONTAINS({0}.IndexedWords, {1})";
        string format = context.m_supportsFullTextSearch ? str : "{0}.Words {2}LIKE {1}";
        statement.AppendPredicate(format, longTextTableAlias, QueryProcessorCommon.EncodeSqlString(searchValue), !context.IsFullTextJoinOptimizationEnabled || !expressionNode.Operator.IsNegated() ? string.Empty : "NOT ");
      }
      if (context.IsFullTextJoinOptimizationEnabled && !expressionNode.Operator.IsEverable())
        return;
      QueryPredicate.AppendLongTextDatePredicate(context, longTextTableAlias, expressionNode, statement);
    }

    private static void AppendLongTextDatePredicate(
      QueryProcessorContext context,
      string longTextTableAlias,
      QueryComparisonExpressionNode expressionNode,
      SqlStatement statement)
    {
      bool flag = expressionNode.Operator.IsEverable();
      if (context.m_asOfParam != null)
      {
        statement.AppendPredicate(" AND {0}.AddedDate <= {1}", longTextTableAlias, context.m_asOfParam);
        if (flag)
          return;
        statement.AppendPredicate(" AND {0}.EndDate > {1}", longTextTableAlias, context.m_asOfParam);
      }
      else if (context.m_asOfDatesParam != null)
      {
        statement.AppendPredicate(" AND {0}.AddedDate <= {1}", longTextTableAlias, "AsOfDates" + ".Val");
        if (flag)
          return;
        statement.AppendPredicate(" AND {0}.EndDate > {1}", longTextTableAlias, "AsOfDates" + ".Val");
      }
      else
      {
        if (flag)
          return;
        statement.AppendPredicate(" AND {0}.EndDate = {1}", longTextTableAlias, "convert(datetime,'9999',126)");
      }
    }

    private static string GetLongTextDatePredicateString(
      QueryProcessorContext context,
      string longTextTableAlias)
    {
      string datePredicateString;
      if (context.m_asOfParam != null)
        datePredicateString = " AND " + longTextTableAlias + ".AddedDate <= " + context.m_asOfParam + " AND " + longTextTableAlias + ".EndDate > " + context.m_asOfParam;
      else if (context.m_asOfDatesParam != null)
        datePredicateString = " AND " + longTextTableAlias + ".AddedDate <= AsOfDates" + ".Val AND " + longTextTableAlias + ".EndDate > AsOfDates" + ".Val";
      else
        datePredicateString = " AND " + longTextTableAlias + ".EndDate = convert(datetime,'9999',126)";
      return datePredicateString;
    }

    private static void AppendTreeFieldPredicate(
      QueryProcessorContext context,
      string workItemsTableAlias,
      QueryComparisonExpressionNode expressionNode,
      SqlStatement statement)
    {
      context.m_requestContext.TraceBlock(906033, 906034, "Query", "QueryProcessor", nameof (AppendTreeFieldPredicate), (Action) (() =>
      {
        int numberValue = expressionNode.Value.NumberValue;
        QueryExpressionOperator op = expressionNode.Operator;
        if (numberValue == 0)
        {
          if (op <= QueryExpressionOperator.NotEquals)
          {
            if (op != QueryExpressionOperator.Equals)
            {
              if (op != QueryExpressionOperator.NotEquals)
                goto label_8;
            }
            else
              goto label_7;
          }
          else if (op != QueryExpressionOperator.Under)
          {
            if (op == QueryExpressionOperator.NotUnder)
              goto label_7;
            else
              goto label_8;
          }
          statement.AppendPredicate("1=1");
          return;
label_7:
          statement.AppendPredicate("1=0");
          return;
label_8:
          throw new NotSupportedException();
        }
        TreeNode node;
        if (!context.TreeSnapshot.LegacyTryGetTreeNode(numberValue, out node))
        {
          statement.AppendPredicate("1=0");
        }
        else
        {
          string binaryPathHex = "0x" + CommonWITUtils.GetNodeFullPathInHexadecimal(node);
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) workItemsTableAlias, expressionNode.Field.FieldId == -2 ? (object) "AreaPath" : (object) "IterationPath");
          if (op.IsUnder())
          {
            if (node.Children.Any<KeyValuePair<string, TreeNode>>())
            {
              string valueInHexadecimal = CommonWITUtils.GetNodeRangeEndPathValueInHexadecimal(binaryPathHex);
              statement.AppendPredicate("{0} {1} BETWEEN {2} AND {3}", str, op.IsNegated() ? "NOT" : string.Empty, binaryPathHex, valueInHexadecimal);
              if (op.IsNegated())
                return;
              QueryPredicate.AddTreeReference(context, node, TreeReferenceType.In);
              return;
            }
            op = !op.IsNegated() ? QueryExpressionOperator.Equals : QueryExpressionOperator.NotEquals;
          }
          if (op != QueryExpressionOperator.Equals)
          {
            if (op != QueryExpressionOperator.NotEquals)
              throw new NotSupportedException();
            statement.AppendPredicate("{0} <> {1}", str, binaryPathHex);
            QueryPredicate.AddTreeReference(context, node, TreeReferenceType.NotIn);
          }
          else
          {
            statement.AppendPredicate("{0} = {1}", str, binaryPathHex);
            QueryPredicate.AddTreeReference(context, node, TreeReferenceType.In);
          }
        }
      }));
    }

    private static void AddTreeReference(
      QueryProcessorContext context,
      TreeNode node,
      TreeReferenceType type)
    {
      WorkItemTrackingTreeService.ClassificationNodeId key = new WorkItemTrackingTreeService.ClassificationNodeId()
      {
        ProjectId = node.ProjectId,
        NodeId = node.Id
      };
      TreeReferenceType treeReferenceType;
      context.m_treeReference[key] = !context.m_treeReference.TryGetValue(key, out treeReferenceType) || (treeReferenceType & type) == type ? type : type | treeReferenceType;
      --context.m_unhandledTreeReferenceCount;
    }

    private static bool IsIdentityFieldWithDistinctDisplayName(
      QueryComparisonExpressionNode queryComparisonExpressionNode)
    {
      if (!queryComparisonExpressionNode.Field.IsIdentity || string.IsNullOrEmpty(queryComparisonExpressionNode.Value.StringValue) || queryComparisonExpressionNode.Value.StringValue.IndexOf(">") != queryComparisonExpressionNode.Value.StringValue.Length - 1)
        return false;
      return queryComparisonExpressionNode.Operator == QueryExpressionOperator.Equals || queryComparisonExpressionNode.Operator == QueryExpressionOperator.NotEquals;
    }

    private static bool IsBackCompatIdentityQueryExpressionField(
      IVssRequestContext requestContext,
      QueryComparisonExpressionNode queryComparisonExpressionNode)
    {
      return queryComparisonExpressionNode.Field.IsIdentity && !QueryPredicate.IsIdentityFieldWithDistinctDisplayName(queryComparisonExpressionNode);
    }

    private static void AppendInPredicate(
      QueryProcessorContext context,
      string workItemsTableAlias,
      QueryComparisonExpressionNode expressionNode,
      SqlStatement statement)
    {
      context.m_requestContext.TraceBlock(906035, 906036, "Query", "QueryProcessor", nameof (AppendInPredicate), (Action) (() =>
      {
        QueryExpressionValue[] queryExpressionValueArray = expressionNode.Value.GetArrayValue();
        int length = queryExpressionValueArray.Length;
        string column = QueryPredicate.GetColumn(context, expressionNode.Field, workItemsTableAlias, statement, length < QueryPredicate.GetInThreshold(context, expressionNode.Field));
        if (expressionNode.Field.FieldType == InternalFieldType.Boolean && ((IEnumerable<QueryExpressionValue>) queryExpressionValueArray).Any<QueryExpressionValue>((Func<QueryExpressionValue, bool>) (v => v.ValueType == QueryExpressionValueType.Boolean && !v.BoolValue)) && !((IEnumerable<QueryExpressionValue>) queryExpressionValueArray).Any<QueryExpressionValue>((Func<QueryExpressionValue, bool>) (v => v.IsNull)))
          queryExpressionValueArray = ((IEnumerable<QueryExpressionValue>) queryExpressionValueArray).Concat<QueryExpressionValue>((IEnumerable<QueryExpressionValue>) new QueryExpressionValue[1]
          {
            new QueryExpressionValue()
          }).ToArray<QueryExpressionValue>();
        bool flag1 = expressionNode.Operator.IsNegated();
        bool flag2 = ((IEnumerable<QueryExpressionValue>) queryExpressionValueArray).Any<QueryExpressionValue>((Func<QueryExpressionValue, bool>) (x => x.IsNull));
        bool flag3 = ((IEnumerable<QueryExpressionValue>) queryExpressionValueArray).Any<QueryExpressionValue>((Func<QueryExpressionValue, bool>) (x => !x.IsNull));
        if (flag3)
        {
          statement.AppendPredicate("{0}{1} IN (", column, flag1 ? " NOT" : string.Empty);
          if (length < QueryPredicate.GetInThreshold(context, expressionNode.Field))
          {
            statement.Append(string.Join(", ", ((IEnumerable<QueryExpressionValue>) queryExpressionValueArray).Where<QueryExpressionValue>((Func<QueryExpressionValue, bool>) (x => !x.IsNull)).Select<QueryExpressionValue, string>((Func<QueryExpressionValue, string>) (x => QueryParameter.DefineParameter(context, x, false)))));
          }
          else
          {
            int tempTableId;
            if (QueryPredicateCommon.GetTempTableId(context, expressionNode, out tempTableId))
            {
              string str = QueryParameter.DefineTableValuedParameter(context, expressionNode.Value);
              QueryExpressionValueType valueType = ((IEnumerable<QueryExpressionValue>) queryExpressionValueArray).First<QueryExpressionValue>().ValueType;
              context.m_queryText.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\r\nCREATE TABLE #temp{0}\r\n(\r\n    X {1} NOT NULL,\r\n    UNIQUE CLUSTERED (X) WITH (IGNORE_DUP_KEY=ON)\r\n)\r\n\r\nINSERT #temp{0}\r\nSELECT {2}\r\nFROM {3} X \r\n", (object) tempTableId, expressionNode.Field.IsPerson ? (object) "INT" : (object) QueryPredicate.GetSqlType(valueType), expressionNode.Field.IsPerson ? (object) "C.ConstId" : (object) QueryPredicate.GetTvpColumnName(valueType), (object) str);
              if (expressionNode.Field.IsPerson)
                context.m_queryText.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "JOIN dbo.Constants C\r\nON C.PartitionId = {0}\r\nAND X.Data = C.DisplayPart\r\n\r\n", (object) context.PartitionId);
            }
            statement.AppendPredicate("SELECT X FROM #temp{0}", tempTableId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
          }
          statement.AppendPredicate(")");
        }
        if (!(flag2 | flag1))
          return;
        if (flag3)
          statement.AppendPredicate(" {0}", flag2 & flag1 ? "AND" : "OR");
        statement.AppendPredicate(" {0} IS{1} NULL", column, flag2 & flag1 ? " NOT" : string.Empty);
      }));
    }

    private static int GetInThreshold(QueryProcessorContext context, FieldEntry field) => context.m_queryInThreshold;

    private static void AppendInGroupPredicate(
      QueryProcessorContext context,
      string workItemsTableAlias,
      QueryComparisonExpressionNode expressionNode,
      SqlStatement statement)
    {
      context.m_requestContext.TraceBlock(906037, 906038, "Query", "QueryProcessor", nameof (AppendInGroupPredicate), (Action) (() =>
      {
        context.m_isIdentityInGroupQuery = expressionNode.Field.IsIdentity;
        string str1 = expressionNode.Operator.IsNegated() ? "NOT" : string.Empty;
        string column1 = QueryPredicate.GetColumn(context, expressionNode.Field, workItemsTableAlias, statement);
        string column2 = QueryPredicate.GetColumn(context, expressionNode.Field, workItemsTableAlias, statement, false);
        bool flag1 = context.IsQueryIdentityConstIdOptimizationEnabled && expressionNode.Field.IsPerson;
        int tempTableId;
        bool tempTableId1 = QueryPredicateCommon.GetTempTableId(context, expressionNode, out tempTableId);
        if (expressionNode.Field.FieldId == 25)
        {
          if (tempTableId1)
            context.m_requestContext.TraceBlock(906045, 906046, "Query", "QueryProcessor", "AppendInGroupPredicate.DefineWorkItemTypeCategoriesTempTable", (Action) (() =>
            {
              IWorkItemTypeCategoryService service4 = context.m_requestContext.GetService<IWorkItemTypeCategoryService>();
              IProjectService service5 = context.m_requestContext.GetService<IProjectService>();
              IWorkItemTrackingProcessService service6 = context.m_requestContext.GetService<IWorkItemTrackingProcessService>();
              IVssRequestContext requestContext = context.m_requestContext;
              List<Guid> list = service5.GetProjects(requestContext, ProjectState.WellFormed).Select<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (p => p.Id)).ToList<Guid>();
              Dictionary<int, HashSet<string>> source3 = new Dictionary<int, HashSet<string>>();
              Dictionary<Guid, HashSet<string>> dictionary = new Dictionary<Guid, HashSet<string>>();
              bool flag2 = WorkItemTrackingFeatureFlags.IsSharedProcessEnabled(context.m_requestContext);
              foreach (Guid guid in list)
              {
                TreeNode node;
                if (context.m_requestContext.WitContext().TreeService.TryGetTreeNode(guid, guid, out node))
                {
                  ProcessDescriptor processDescriptor;
                  if (flag2 && service6.TryGetLatestProjectProcessDescriptor(context.m_requestContext, guid, out processDescriptor))
                  {
                    HashSet<string> source4;
                    WorkItemTypeCategory workItemTypeCategory;
                    if (!dictionary.TryGetValue(processDescriptor.TypeId, out source4) && service4.TryGetWorkItemTypeCategory(context.m_requestContext, guid, expressionNode.Value.StringValue, out workItemTypeCategory) && workItemTypeCategory.WorkItemTypeNames.Any<string>())
                    {
                      source4 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
                      dictionary[processDescriptor.TypeId] = source4;
                      foreach (string workItemTypeName in workItemTypeCategory.WorkItemTypeNames)
                        source4.Add(workItemTypeName);
                    }
                    if (source4 != null && source4.Any<string>())
                      source3[node.Id] = source4;
                  }
                  else
                  {
                    HashSet<string> stringSet;
                    WorkItemTypeCategory workItemTypeCategory;
                    if (!source3.TryGetValue(node.Id, out stringSet) && service4.TryGetWorkItemTypeCategory(context.m_requestContext, guid, expressionNode.Value.StringValue, out workItemTypeCategory) && workItemTypeCategory.WorkItemTypeNames.Any<string>())
                    {
                      stringSet = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
                      source3[node.Id] = stringSet;
                      foreach (string workItemTypeName in workItemTypeCategory.WorkItemTypeNames)
                        stringSet.Add(workItemTypeName);
                    }
                  }
                }
              }
              string str2 = string.Format("\r\nINSERT #temp{0}\r\nVALUES\r\n", (object) tempTableId);
              StringBuilder stringBuilder = new StringBuilder();
              if (source3.Any<KeyValuePair<int, HashSet<string>>>())
              {
                stringBuilder.Append(str2);
                int num3 = 0;
                int num4 = 0;
                foreach (KeyValuePair<int, HashSet<string>> keyValuePair in source3)
                {
                  for (int index = 1; index <= keyValuePair.Value.Count; ++index)
                  {
                    stringBuilder.AppendFormat("({0}, {1})", (object) keyValuePair.Key, (object) QueryProcessorCommon.EncodeSqlString(keyValuePair.Value.ElementAt<string>(index - 1)));
                    ++num4;
                    if (index < keyValuePair.Value.Count || ++num3 < source3.Count)
                    {
                      if (num4 % 1000 == 0)
                        stringBuilder.Append(';').Append(str2);
                      else
                        stringBuilder.Append(",\r\n");
                    }
                  }
                }
              }
              context.m_queryText.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\r\nCREATE TABLE #temp{0}\r\n(\r\n    ProjectId INT NOT NULL,\r\n    WorkItemType NVARCHAR(256) COLLATE DATABASE_DEFAULT NOT NULL,\r\n\r\n    UNIQUE CLUSTERED (ProjectId, WorkItemType)\r\n)\r\n{1};\r\n\r\n", (object) tempTableId, (object) stringBuilder);
            }));
          statement.AppendPredicate("{0} EXISTS(\r\n    SELECT * \r\n    FROM #temp{1} \r\n    WHERE ProjectId = CONVERT(INT, SUBSTRING({2}.AreaPath, 1, 4))\r\n        AND WorkItemType = {2}.WorkItemType\r\n)", str1, tempTableId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo), workItemsTableAlias);
        }
        else if (expressionNode.Field.IsIdentity && expressionNode.Value.ValueType == QueryExpressionValueType.IdentityGuid)
        {
          if (tempTableId1)
          {
            if (expressionNode.Value.IdentityGuidValues == null)
              throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.FieldUnknownIdentity((object) expressionNode.Value.StringValue, (object) expressionNode.Field.Name));
            string str3 = QueryParameter.DefineTableValuedParameter<Guid>(context, (WorkItemTrackingTableValueParameter<Guid>) new Microsoft.TeamFoundation.WorkItemTracking.Server.GuidTable(expressionNode.Value.IdentityGuidValues));
            context.m_queryText.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, flag1 ? "\r\nCREATE TABLE #temp{0}\r\n(\r\n    X INT NOT NULL,\r\n    UNIQUE CLUSTERED (X) WITH (IGNORE_DUP_KEY=ON)\r\n)\r\n\r\n;WITH AllNames\r\nas\r\n(\r\nSELECT C.ConstId, IdentityDisplayName FROM Constants C\r\nJOIN {1} T\r\nON T.Id = C.TeamFoundationId\r\nWHERE C.PartitionId = {2}\r\n)\r\n-- insert all constId and their ambiguous versions's constId.\r\nINSERT #temp{0}\r\nSELECT ConstId FROM AllNames\r\nUNION\r\nSELECT C.ConstId FROM Constants C\r\nJOIN AllNames A\r\nON A.IdentityDisplayName = C.DisplayPart\r\nWHERE C.PartitionId = {2}\r\nAND C.TeamFoundationId IS NULL\r\n;\r\n" : "\r\nCREATE TABLE #temp{0}\r\n(\r\n    X NVARCHAR(256) COLLATE DATABASE_DEFAULT NOT NULL,\r\n    UNIQUE CLUSTERED (X) WITH (IGNORE_DUP_KEY=ON)\r\n)\r\n\r\n;WITH AllNames\r\nas\r\n(\r\nSELECT IdentityDisplayName, DisplayPart FROM Constants C\r\nJOIN {1} T\r\nON T.Id = C.TeamFoundationId\r\nWHERE C.PartitionId = {2}\r\n)\r\n-- insert all display parts and their ambiguous versions.\r\nINSERT #temp{0}\r\nSELECT DisplayPart FROM AllNames\r\nUNION\r\nSELECT C.DisplayPart FROM Constants C\r\nJOIN AllNames A\r\nON A.IdentityDisplayName = C.DisplayPart\r\nWHERE C.PartitionId = {2}\r\nAND C.TeamFoundationId IS NULL\r\n;\r\n", (object) tempTableId, (object) str3, (object) context.PartitionId);
          }
          statement.AppendPredicate("{0} {1} IN (SELECT * FROM #temp{2})", flag1 ? column2 : column1, str1, tempTableId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
        }
        else
        {
          if (tempTableId1)
            context.m_queryText.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, flag1 ? "\r\nCREATE TABLE #temp{0}\r\n(\r\n    X INT NOT NULL,\r\n    UNIQUE CLUSTERED (X) WITH (IGNORE_DUP_KEY=ON)\r\n)\r\n\r\nINSERT #temp{0}\r\nSELECT ConstId\r\nFROM dbo.func_ExplodeSetAllConstId({1}, {2})\r\n\r\n" : "\r\nCREATE TABLE #temp{0}\r\n(\r\n    X NVARCHAR(256) COLLATE DATABASE_DEFAULT NOT NULL,\r\n    UNIQUE CLUSTERED (X) WITH (IGNORE_DUP_KEY=ON)\r\n)\r\n\r\nINSERT #temp{0}\r\nSELECT String\r\nFROM dbo.func_ExplodeSetAll({1}, {2})\r\n\r\n", (object) tempTableId, (object) context.PartitionId, (object) QueryParameter.DefineParameter(context, expressionNode.Value, true));
          statement.AppendPredicate("{0} {1} IN (SELECT * FROM #temp{2})", flag1 ? column2 : column1, str1, tempTableId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
        }
        if (string.IsNullOrWhiteSpace(str1))
          return;
        statement.AppendPredicate(" OR {0} IS NULL", flag1 ? column2 : column1);
      }));
    }

    private static IDictionary<int, string> GetDataspaceIdProjectNameMap(
      QueryProcessorContext context)
    {
      if (context.m_dataspaceIdToProjectNameMap == null)
      {
        context.m_dataspaceIdToProjectNameMap = new Dictionary<int, string>();
        IProjectService service1 = context.m_requestContext.GetService<IProjectService>();
        IDataspaceService service2 = context.m_requestContext.GetService<IDataspaceService>();
        IVssRequestContext requestContext = context.m_requestContext;
        foreach (ProjectInfo project in service1.GetProjects(requestContext, ProjectState.WellFormed))
        {
          int? dataspaceId = service2.QueryDataspace(context.m_requestContext, "WorkItem", project.Id, false)?.DataspaceId;
          if (dataspaceId.HasValue)
            context.m_dataspaceIdToProjectNameMap[dataspaceId.Value] = project.Name;
        }
      }
      return (IDictionary<int, string>) context.m_dataspaceIdToProjectNameMap;
    }

    private static string GetTvpColumnName(QueryExpressionValueType valueType)
    {
      string tvpColumnName;
      switch (valueType)
      {
        case QueryExpressionValueType.Number:
          tvpColumnName = "Val";
          break;
        case QueryExpressionValueType.DateTime:
          tvpColumnName = "Val";
          break;
        case QueryExpressionValueType.UniqueIdentifier:
          tvpColumnName = "Id";
          break;
        case QueryExpressionValueType.Boolean:
          tvpColumnName = "Flag";
          break;
        case QueryExpressionValueType.Double:
          tvpColumnName = "Val";
          break;
        default:
          tvpColumnName = "Data";
          break;
      }
      return tvpColumnName;
    }

    private static string GetSqlType(QueryExpressionValueType valueType)
    {
      string sqlType;
      switch (valueType)
      {
        case QueryExpressionValueType.Number:
          sqlType = "INT";
          break;
        case QueryExpressionValueType.DateTime:
          sqlType = "DATETIME";
          break;
        case QueryExpressionValueType.UniqueIdentifier:
          sqlType = "UNIQUEIDENTIFIER";
          break;
        case QueryExpressionValueType.Boolean:
          sqlType = "BIT";
          break;
        case QueryExpressionValueType.Double:
          sqlType = "FLOAT";
          break;
        default:
          sqlType = "NVARCHAR(4000) COLLATE DATABASE_DEFAULT";
          break;
      }
      return sqlType;
    }

    internal static string GetLongTableColumn(FieldEntry field)
    {
      if (field.IsPerson)
        return "IntValue";
      if (field.FieldId == 1)
        return "TextValue";
      switch (field.IsPicklist ? field.FieldType : FieldHelpers.ConvertToFieldType((FieldDBType) field.FieldDataType))
      {
        case InternalFieldType.Integer:
          return "IntValue";
        case InternalFieldType.DateTime:
          return "DateTimeValue";
        case InternalFieldType.Double:
          return "FloatValue";
        case InternalFieldType.Guid:
          return "GuidValue";
        case InternalFieldType.Boolean:
          return "BitValue";
        default:
          return "StringValue";
      }
    }

    private static int AddTempTable(QueryProcessorContext context, string key)
    {
      context.m_tempTableMap[key] = context.m_tempTableCounter;
      return context.m_tempTableCounter++;
    }

    private static bool IsNullable(FieldEntry field) => field.StorageTarget != FieldStorageTarget.WideTable && field.FieldId != 100 || field.FieldId == 24 || (field.FieldDataType & 240) == 160;

    public static string BuildOrderBy(
      FieldEntry field,
      string columnName,
      bool descending,
      bool? nullsFirst)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, !QueryPredicate.IsNullable(field) ? "{0}{1}" : (!nullsFirst.HasValue ? "CASE WHEN {0} IS NULL THEN 1 ELSE 0 END{1}, {0}{1}" : (nullsFirst.Value ? "CASE WHEN {0} IS NULL THEN 0 ELSE 1 END, {0}{1}" : "CASE WHEN {0} IS NULL THEN 1 ELSE 0 END, {0}{1}")), (object) columnName, descending ? (object) " DESC" : (object) string.Empty);
    }

    public static string GetLinkView(QueryProcessorContext context) => context.m_hasForwardLinkType == context.m_hasReverseLinkType ? (!context.m_isAsOfQuery ? "dbo.ForwardReverseLinks" : "dbo.ForwardReverseLinksAsOf") : (context.m_hasForwardLinkType ? (!context.m_isAsOfQuery ? "dbo.ForwardLinks" : "dbo.ForwardLinksAsOf") : (!context.m_isAsOfQuery ? "dbo.ReverseLinks" : "dbo.ReverseLinksAsOf"));

    public static string GetWorkItemView(bool core, bool all, bool newView = false, bool hasParentasof = false)
    {
      if (core)
      {
        if (!all)
          return "dbo.vw_denorm_WorkItemCoreLatest";
        if (!newView)
          return "dbo.vw_denorm_WorkItemCoreAll";
        return hasParentasof ? "dbo.vw_denorm_WorkItemCoreAll3WithParentHistory" : "dbo.vw_denorm_WorkItemCoreAll2";
      }
      return !all ? "dbo.tbl_WorkItemCustomLatest" : "dbo.vw_WorkItemCustomAll";
    }

    public static void AddDateFilter(
      QueryProcessorContext context,
      SqlStatement statement,
      string tableAlias,
      bool addLatestTable)
    {
      if (context.m_asOfParam != null)
      {
        if (!string.IsNullOrWhiteSpace(statement.GetPredicate()))
          statement.AppendPredicate(" AND ");
        statement.AppendPredicate("{0}.[System.AuthorizedDate] <= {1} AND {0}.[System.RevisedDate] > {1}", tableAlias, context.m_asOfParam);
        if (context.m_isParentQuery)
          statement.AppendPredicate(" AND {0}.[ParentAddedDate] <= {1} AND {0}.[ParentRemovedDate] > {1}", tableAlias, context.m_asOfParam);
        if (addLatestTable)
        {
          string tableAlias1 = tableAlias + "L";
          string condition = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.PartitionId = {1} AND {0}.Id = {2}.Id", (object) tableAlias1, (object) context.PartitionId, (object) tableAlias);
          statement.DefineJoin(QueryPredicate.GetWorkItemView(true, false), tableAlias1, condition);
        }
      }
      if (context.m_asOfDatesParam == null)
        return;
      string condition1 = string.Format("{0}.[System.AuthorizedDate] <= {1} AND {0}.[System.RevisedDate] > {1}", (object) tableAlias, (object) ("AsOfDates" + ".Val"));
      if (!WorkItemTrackingFeatureFlags.IsFixForAsOfFullTextQueriesEnabled(context.m_requestContext) && context.IsFullTextJoinOptimizationEnabled && context.m_isFullTextQuery)
        condition1 += QueryPredicate.GetLongTextDatePredicateString(context, tableAlias);
      statement.DefineJoin(context.m_asOfDatesParam, "AsOfDates", condition1);
      if (!addLatestTable)
        return;
      string tableAlias2 = tableAlias + "L";
      string condition2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.PartitionId = {1} AND {0}.Id = {2}.Id", (object) tableAlias2, (object) context.PartitionId, (object) tableAlias);
      statement.DefineJoin(QueryPredicate.GetWorkItemView(true, false), tableAlias2, condition2);
    }

    public static string TreePathToId(string fullColumnName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CONVERT(INT, SUBSTRING({0}, DATALENGTH({0}) - 3, 4))", (object) fullColumnName);
  }
}
