// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.QueryPreprocessor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor
{
  internal static class QueryPreprocessor
  {
    public static void ValidateAndOptimize(
      QueryProcessorContext context,
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression)
    {
      context.m_requestContext.TraceBlock(906020, 906021, "Query", "QueryProcessor", "Preprocess", (Action) (() =>
      {
        context.Reset();
        if (queryExpression.SortFields == null)
          queryExpression.SortFields = Enumerable.Empty<QuerySortField>();
        else
          QueryPreprocessor.ValidateSortFields(context, queryExpression.QueryType, queryExpression.SortFields);
        if (queryExpression.AsOfDateTime.HasValue)
        {
          DateTime? asOfDateTime;
          if (queryExpression.AsOfDateTime.Value.Kind != DateTimeKind.Utc)
          {
            Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression1 = queryExpression;
            asOfDateTime = queryExpression.AsOfDateTime;
            DateTime? nullable = new DateTime?(DateTime.SpecifyKind(asOfDateTime.Value, DateTimeKind.Utc));
            queryExpression1.AsOfDateTime = nullable;
          }
          asOfDateTime = queryExpression.AsOfDateTime;
          if (!CommonWITUtils.IsValidSqlDateTime(asOfDateTime.Value))
            throw new WorkItemTrackingQueryInvalidAsOfDateException();
          QueryProcessorContext processorContext = context;
          QueryProcessorContext context1 = context;
          asOfDateTime = queryExpression.AsOfDateTime;
          // ISSUE: variable of a boxed type
          __Boxed<DateTime> local = (ValueType) asOfDateTime.Value;
          string str = QueryParameter.DefineParameter(context1, (object) local);
          processorContext.m_asOfParam = str;
          context.m_isAsOfQuery = true;
        }
        if (queryExpression.QueryType == QueryType.LinksRecursiveMustContain || queryExpression.QueryType == QueryType.LinksRecursiveDoesNotContain)
          throw new NotImplementedException();
        if (queryExpression.QueryType == QueryType.LinksRecursiveMayContain)
        {
          MDWorkItemLinkType linkTypeById = context.LinkService.GetLinkTypeById(context.m_requestContext, (int) queryExpression.RecursionLinkTypeId);
          if (linkTypeById.Topology != LinkTopology.Tree || (int) queryExpression.RecursionLinkTypeId != linkTypeById.ForwardId)
            throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InvalidLinkQueryRecursionID((object) queryExpression.RecursionLinkTypeId));
          if (context.m_asOfParam != null)
            throw new NotImplementedException();
        }
        IEnumerable<Enum> source = queryExpression.Optimizations.GetFlags();
        if (source.Contains<Enum>((Enum) QueryOptimization.ForceFullTextIndex) && source.Contains<Enum>((Enum) QueryOptimization.DoNotForceFullTextIndex))
          source = source.Where<Enum>((Func<Enum, bool>) (o => (QueryOptimization) o != QueryOptimization.ForceFullTextIndex));
        foreach (QueryOptimization queryOptimization in source)
        {
          switch (queryOptimization)
          {
            case QueryOptimization.ForceOrder:
              context.m_isForceOrderEnabled = true;
              continue;
            case QueryOptimization.ForceCustomTablePK:
              context.m_isLeftJoinIndexHintEnabled = true;
              continue;
            case QueryOptimization.FullTextSearchResultInTempTable:
              context.m_isFullTextResultInTempTableEnabled = true;
              continue;
            case QueryOptimization.FullTextJoinForceOrder:
              context.m_isFullTextJoinOptimizationEnabled = true;
              continue;
            case QueryOptimization.DisableNonClusteredColumnstoreIndex:
              context.m_isAllowNonClusteredColumnstoreIndexEnabled = false;
              continue;
            case QueryOptimization.ForceFullTextIndex:
              context.m_isFullTextIndexHintEnabled = true;
              continue;
            case QueryOptimization.DoNotForceFullTextIndex:
              context.m_isFullTextIndexHintEnabled = false;
              continue;
            case QueryOptimization.MoveOrClauseUp:
              context.m_isMoveOrClauseUpEnabled = true;
              continue;
            default:
              continue;
          }
        }
        queryExpression.LeftGroup = QueryPreprocessor.PreprocessExpression(context, queryExpression, queryExpression.LeftGroup, QueryTableAlias.Left);
        if (queryExpression.QueryType == QueryType.WorkItems)
          return;
        queryExpression.RightGroup = QueryPreprocessor.PreprocessExpression(context, queryExpression, queryExpression.RightGroup, QueryTableAlias.Right);
        if (queryExpression.QueryType == QueryType.LinksRecursiveMayContain)
          return;
        queryExpression.LinkGroup = QueryPreprocessor.PreprocessExpression(context, queryExpression, queryExpression.LinkGroup, QueryTableAlias.Link);
      }));
    }

    private static QueryExpressionNode PreprocessExpression(
      QueryProcessorContext context,
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression rootNode,
      QueryExpressionNode node,
      QueryTableAlias tableAlias,
      QueryLogicalExpressionOperator? parentLogicalExpression = null,
      int depth = 0)
    {
      if (node == null)
        return (QueryExpressionNode) null;
      if (depth >= 64)
        throw new WorkItemTrackingQueryDepthTooLargeException();
      if (node is QueryComparisonExpressionNode)
        return (QueryExpressionNode) QueryPreprocessor.PreprocessComparisonExpression(context, rootNode, node as QueryComparisonExpressionNode, tableAlias);
      QueryLogicalExpressionNode logicalNode = node as QueryLogicalExpressionNode;
      return QueryPreprocessor.PreprocessLogicalExpression(context, rootNode, logicalNode, tableAlias, parentLogicalExpression, depth);
    }

    private static QueryComparisonExpressionNode PreprocessComparisonExpression(
      QueryProcessorContext context,
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression rootNode,
      QueryComparisonExpressionNode node,
      QueryTableAlias tableAlias)
    {
      if (node.Field == null)
        throw new WorkItemTrackingQueryNullFieldException();
      if (node.Value == null)
        throw new WorkItemTrackingQueryNullPredicateException();
      if (!context.m_hasDeletedFilter && node.Field.FieldId == -404)
        context.m_hasDeletedFilter = true;
      if (node.Field.FieldId == 100)
      {
        if (tableAlias != QueryTableAlias.Link)
          throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidPredicateLinkType());
        if (node.Operator != QueryExpressionOperator.Equals)
        {
          context.m_hasForwardLinkType = true;
          context.m_hasReverseLinkType = true;
        }
        else
        {
          int numberValue = node.Value.NumberValue;
          MDWorkItemLinkType linkTypeById = context.LinkService.GetLinkTypeById(context.m_requestContext, numberValue);
          context.m_hasForwardLinkType = context.m_hasForwardLinkType || linkTypeById.ForwardId == numberValue;
          context.m_hasReverseLinkType = context.m_hasReverseLinkType || linkTypeById.ReverseId == numberValue || linkTypeById.ForwardId == linkTypeById.ReverseId;
        }
      }
      else if (tableAlias == QueryTableAlias.Link)
        throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidPredicateLinkType());
      if (node.ExpandConstant)
      {
        if (node.Operator != QueryExpressionOperator.Equals && node.Operator != QueryExpressionOperator.NotEquals && node.Operator != QueryExpressionOperator.Ever)
          throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidPredicateExpandFlag((object) node.Operator.ToString()));
        if (node.Field.IsIdentity && node.Operator != QueryExpressionOperator.Ever)
          rootNode.IsInGroupIdentityQuery = true;
      }
      if (node.Operator.IsEverable() && node.Operator.IsContains() && !node.Field.IsLongText)
        throw new WorkItemTrackingQueryInvalidEverClausePredicateException();
      if ((node.Operator.IsContainsWords() || node.Operator.IsFullTextContains()) && !node.Field.IsLongText && !node.Field.OftenQueriedAsText)
        throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidPredicateLongText());
      if (node.Field.IsLongText)
      {
        if (!node.Operator.UsesIsEmpty() && !node.Operator.IsContains())
          throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidPredicateLongText());
        if (node.Operator.UsesIsEmpty() && (node.Field.FieldId == 80 || node.Field.FieldId == 54))
          throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidPredicateIsEmpty((object) node.Field.ReferenceName));
      }
      if (node.Operator.IsContainsWords())
        context.m_isFullTextQuery = true;
      if (node.Operator.IsUnder() && (node.Value.ValueType != QueryExpressionValueType.Number || node.Value.IsNull))
        throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryInvalidValueType());
      if (node.Operator.IsContains())
      {
        if (node.Value.ValueType == QueryExpressionValueType.Array)
        {
          foreach (QueryExpressionValue queryExpressionValue in node.Value.GetArrayValue())
            QueryPreprocessor.ValidateValueForContains(queryExpressionValue, node.Field, QueryProcessorCommon.CanQueryAsText(context, node.Field, node.Operator));
        }
        else
          QueryPreprocessor.ValidateValueForContains(node.Value, node.Field, QueryProcessorCommon.CanQueryAsText(context, node.Field, node.Operator));
        if (node.Field.FieldId == 80)
          rootNode.IsQueryingOnTags = true;
        if (node.Fields.Count<FieldEntry>() > 1)
        {
          if (node.Operator.IsEverable() || node.Operator.IsNegated())
            throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidOptimizationLongText());
          foreach (FieldEntry field in node.Fields)
          {
            if (!QueryProcessorCommon.CanQueryAsText(context, field, node.Operator))
              throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidOptimizationLongText());
          }
        }
      }
      else if (node.Fields.Count<FieldEntry>() > 1)
        throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidOptimizationLongText());
      switch (node.Value.ValueType)
      {
        case QueryExpressionValueType.Array:
          if (node.Operator != QueryExpressionOperator.In && node.Operator != QueryExpressionOperator.NotIn && (!node.Operator.IsContains() || node.Field.FieldId != 80))
            throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidPredicateArrayOperator());
          QueryExpressionValue[] arrayValue = node.Value.GetArrayValue();
          if (node.Value.IsNull || arrayValue == null || arrayValue.Length < 1)
            throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidPredicateArrayEmpty());
          QueryExpressionValueType? nullable = new QueryExpressionValueType?();
          foreach (QueryExpressionValue queryExpressionValue in arrayValue)
          {
            if (queryExpressionValue.ValueType == QueryExpressionValueType.Array)
              throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidPredicateArrayNested());
            if (!nullable.HasValue)
              nullable = new QueryExpressionValueType?(queryExpressionValue.ValueType);
            else if (nullable.Value != queryExpressionValue.ValueType)
              throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidPredicateArrayTypeMismatch());
          }
          break;
        case QueryExpressionValueType.Column:
          if (node.Value.IsNull || node.Value.ColumnValue == null)
            throw new WorkItemTrackingQueryNullFieldException();
          if (node.Value.ColumnValue.IsLongText)
            throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidPredicateLongText());
          if (node.Value.ColumnValue.FieldType != node.Field.FieldType)
            throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidColumnTypeComparison());
          break;
        case QueryExpressionValueType.String:
          if (node.Operator == QueryExpressionOperator.Greater || node.Operator == QueryExpressionOperator.GreaterEquals || node.Operator == QueryExpressionOperator.Less || node.Operator == QueryExpressionOperator.LessEquals)
          {
            context.FieldsDoStringComparison.Add(node.Field.ReferenceName);
            break;
          }
          break;
        case QueryExpressionValueType.DateTime:
          if (!node.Value.IsNull)
          {
            if (node.Value.DateValue.Kind != DateTimeKind.Utc)
              node.Value.DateValue = DateTime.SpecifyKind(node.Value.DateValue, DateTimeKind.Utc);
            if (!CommonWITUtils.IsValidSqlDateTime(node.Value.DateValue))
              throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryInvalidDateValueException());
            break;
          }
          break;
      }
      return node;
    }

    private static void ValidateValueForContains(
      QueryExpressionValue value,
      FieldEntry field,
      bool isLongTextQuery)
    {
      if (value.ValueType != QueryExpressionValueType.String)
        throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryInvalidValueTypeForContainsOperator());
      if (value.IsNull || string.IsNullOrWhiteSpace(value.StringValue) & isLongTextQuery)
        throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryMissingValueForContainsOperator());
      if (field.FieldId == 80)
      {
        value.StringValue = value.StringValue.Trim();
        TagsUtil.ValidateTagName(value.StringValue);
      }
      QueryProcessorCommon.ValidateContainsValueLength(value.StringValue, field.ReferenceName);
    }

    private static QueryExpressionNode PreprocessLogicalExpression(
      QueryProcessorContext context,
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression rootNode,
      QueryLogicalExpressionNode logicalNode,
      QueryTableAlias tableAlias,
      QueryLogicalExpressionOperator? parentLogicalExpression,
      int depth)
    {
      if (logicalNode.Children == null || !logicalNode.Children.Any<QueryExpressionNode>() || logicalNode.Children.Any<QueryExpressionNode>((Func<QueryExpressionNode, bool>) (x => x == null)))
        throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidChildNode());
      int capacity = logicalNode.Children.Count<QueryExpressionNode>();
      if (capacity == 1)
        return QueryPreprocessor.PreprocessExpression(context, rootNode, logicalNode.Children.First<QueryExpressionNode>(), tableAlias, parentLogicalExpression, depth + 1);
      List<QueryExpressionNode> source1 = new List<QueryExpressionNode>(capacity);
      foreach (QueryExpressionNode child in logicalNode.Children)
      {
        QueryExpressionNode queryExpressionNode = QueryPreprocessor.PreprocessExpression(context, rootNode, child, tableAlias, new QueryLogicalExpressionOperator?(logicalNode.Operator), depth + 1);
        if (queryExpressionNode is QueryLogicalExpressionNode logicalExpressionNode && (logicalExpressionNode.Operator == logicalNode.Operator || logicalExpressionNode.Children.Count<QueryExpressionNode>() == 1))
          source1.AddRange(logicalExpressionNode.Children);
        else
          source1.Add(queryExpressionNode);
      }
      if (!parentLogicalExpression.HasValue || logicalNode.Operator != parentLogicalExpression.Value)
      {
        List<QueryExpressionNode> queryExpressionNodeList = new List<QueryExpressionNode>((IEnumerable<QueryExpressionNode>) source1.OfType<QueryLogicalExpressionNode>());
        List<QueryComparisonExpressionNode> comparisonExpressionNodeList1 = new List<QueryComparisonExpressionNode>();
        List<QueryComparisonExpressionNode> comparisonExpressionNodeList2 = new List<QueryComparisonExpressionNode>();
        foreach (IGrouping<QueryExpressionOperator, QueryComparisonExpressionNode> grouping in source1.OfType<QueryComparisonExpressionNode>().GroupBy<QueryComparisonExpressionNode, QueryExpressionOperator>((Func<QueryComparisonExpressionNode, QueryExpressionOperator>) (e => e.Operator)))
        {
          bool flag = !grouping.Key.IsNegated() && logicalNode.Operator == QueryLogicalExpressionOperator.Or || grouping.Key.IsNegated() && logicalNode.Operator == QueryLogicalExpressionOperator.And;
          switch (grouping.Key)
          {
            case QueryExpressionOperator.Equals:
            case QueryExpressionOperator.NotEquals:
              using (IEnumerator<IGrouping<FieldEntry, QueryComparisonExpressionNode>> enumerator = grouping.GroupBy<QueryComparisonExpressionNode, FieldEntry>((Func<QueryComparisonExpressionNode, FieldEntry>) (e => e.Field)).GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  IGrouping<FieldEntry, QueryComparisonExpressionNode> current = enumerator.Current;
                  if (!flag || current.Key.FieldId == -2 || current.Key.FieldId == -104 || current.Count<QueryComparisonExpressionNode>() == 1 || current.Any<QueryComparisonExpressionNode>((Func<QueryComparisonExpressionNode, bool>) (e => e.ExpandConstant || e.Value.ValueType == QueryExpressionValueType.Column)) || current.Select<QueryComparisonExpressionNode, QueryExpressionValueType>((Func<QueryComparisonExpressionNode, QueryExpressionValueType>) (e => e.Value.ValueType)).Distinct<QueryExpressionValueType>().Count<QueryExpressionValueType>() > 1)
                    queryExpressionNodeList.AddRange((IEnumerable<QueryExpressionNode>) current);
                  else
                    queryExpressionNodeList.Add((QueryExpressionNode) new QueryComparisonExpressionNode()
                    {
                      Field = current.Key,
                      Operator = (grouping.Key.IsNegated() ? QueryExpressionOperator.NotIn : QueryExpressionOperator.In),
                      Value = QueryExpressionValue.CreateArrayValue(current.Select<QueryComparisonExpressionNode, QueryExpressionValue>((Func<QueryComparisonExpressionNode, QueryExpressionValue>) (e => e.Value)).ToArray<QueryExpressionValue>())
                    });
                }
                continue;
              }
            case QueryExpressionOperator.Contains:
            case QueryExpressionOperator.NotContains:
            case QueryExpressionOperator.ContainsWords:
            case QueryExpressionOperator.NotContainsWords:
            case QueryExpressionOperator.FTContains:
            case QueryExpressionOperator.NotFTContains:
              using (IEnumerator<QueryComparisonExpressionNode> enumerator = grouping.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  QueryComparisonExpressionNode current = enumerator.Current;
                  if (!flag)
                    queryExpressionNodeList.Add((QueryExpressionNode) current);
                  else if (context.m_supportsFullTextSearch && QueryProcessorCommon.CanQueryAsText(context, current))
                    comparisonExpressionNodeList1.Add(current);
                  else if (current.Field.FieldId == 80)
                    comparisonExpressionNodeList2.Add(current);
                  else
                    queryExpressionNodeList.Add((QueryExpressionNode) current);
                }
                continue;
              }
            default:
              queryExpressionNodeList.AddRange((IEnumerable<QueryExpressionNode>) grouping);
              continue;
          }
        }
        if (comparisonExpressionNodeList1.Any<QueryComparisonExpressionNode>())
        {
          bool flag = false;
          if (comparisonExpressionNodeList1.Count > 1 && !context.IsFullTextJoinOptimizationEnabled)
          {
            flag = true;
            Dictionary<int, HashSet<string>> dictionary = new Dictionary<int, HashSet<string>>();
            foreach (QueryComparisonExpressionNode comparisonExpressionNode in comparisonExpressionNodeList1)
            {
              HashSet<string> stringSet = (HashSet<string>) null;
              if (!dictionary.TryGetValue(comparisonExpressionNode.Field.FieldId, out stringSet))
              {
                stringSet = new HashSet<string>();
                dictionary[comparisonExpressionNode.Field.FieldId] = stringSet;
              }
              stringSet.Add(comparisonExpressionNode.Value.StringValue);
            }
            HashSet<string> source2 = (HashSet<string>) null;
            foreach (HashSet<string> other in dictionary.Values)
            {
              if (source2 == null)
                source2 = other;
              else if (!source2.SetEquals((IEnumerable<string>) other))
              {
                flag = false;
                break;
              }
            }
            if (flag)
            {
              QueryComparisonExpressionNode comparisonExpressionNode = new QueryComparisonExpressionNode()
              {
                Operator = comparisonExpressionNodeList1.First<QueryComparisonExpressionNode>().Operator.IsNegated() ? QueryExpressionOperator.NotContainsWords : QueryExpressionOperator.ContainsWords,
                Value = QueryExpressionValue.CreateArrayValue(source2.Select<string, QueryExpressionValue>((Func<string, QueryExpressionValue>) (s => new QueryExpressionValue()
                {
                  ValueType = QueryExpressionValueType.String,
                  IsNull = false,
                  StringValue = s
                })).ToArray<QueryExpressionValue>())
              };
              comparisonExpressionNode.SetFields(comparisonExpressionNodeList1.Select<QueryComparisonExpressionNode, FieldEntry>((Func<QueryComparisonExpressionNode, FieldEntry>) (x => x.Field)).Distinct<FieldEntry>().ToArray<FieldEntry>());
              queryExpressionNodeList.Add((QueryExpressionNode) comparisonExpressionNode);
            }
          }
          if (!flag)
            queryExpressionNodeList.AddRange((IEnumerable<QueryExpressionNode>) comparisonExpressionNodeList1);
        }
        if (comparisonExpressionNodeList2.Any<QueryComparisonExpressionNode>())
        {
          if (comparisonExpressionNodeList2.Count > 1)
            queryExpressionNodeList.Add((QueryExpressionNode) new QueryComparisonExpressionNode()
            {
              Field = comparisonExpressionNodeList2.First<QueryComparisonExpressionNode>().Field,
              Operator = (comparisonExpressionNodeList2.First<QueryComparisonExpressionNode>().Operator.IsNegated() ? QueryExpressionOperator.NotContains : QueryExpressionOperator.Contains),
              Value = QueryExpressionValue.CreateArrayValue(comparisonExpressionNodeList2.Select<QueryComparisonExpressionNode, QueryExpressionValue>((Func<QueryComparisonExpressionNode, QueryExpressionValue>) (e => e.Value)).ToArray<QueryExpressionValue>())
            });
          else
            queryExpressionNodeList.AddRange((IEnumerable<QueryExpressionNode>) comparisonExpressionNodeList2);
        }
        if (queryExpressionNodeList.Count == 1)
          return queryExpressionNodeList[0];
        source1 = queryExpressionNodeList;
      }
      source1.Sort((IComparer<QueryExpressionNode>) new QueryExpressionNodeComparer());
      logicalNode.Children = (IEnumerable<QueryExpressionNode>) source1;
      return (QueryExpressionNode) logicalNode;
    }

    private static void ValidateSortFields(
      QueryProcessorContext context,
      QueryType queryType,
      IEnumerable<QuerySortField> sortFields)
    {
      HashSet<QuerySortField> querySortFieldSet = new HashSet<QuerySortField>();
      foreach (QuerySortField sortField in sortFields)
      {
        if (sortField.Field == null)
          throw new WorkItemTrackingQueryNullFieldException();
        if (sortField.TableAlias == QueryTableAlias.Right && queryType != QueryType.LinksOneHopMayContain && queryType != QueryType.LinksOneHopMustContain)
          throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidSortFieldTableAlias());
        if (sortField.TableAlias == QueryTableAlias.Link && (queryType == QueryType.WorkItems || queryType == QueryType.LinksOneHopDoesNotContain))
          throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidSortFieldTableAlias());
        if (sortField.TableAlias == QueryTableAlias.Right)
          context.m_sortRhs = true;
        if (sortField.Field.FieldId == 100)
        {
          if (sortField.TableAlias != QueryTableAlias.Link)
            throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidSortFieldLinkType());
        }
        else if (sortField.TableAlias == QueryTableAlias.Link)
          throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidSortFieldLinkType());
        if (!querySortFieldSet.Add(sortField))
          throw new WorkItemTrackingQueryException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.QueryExpressionInvalidSortFieldDupe((object) sortField.Field.ReferenceName));
      }
    }
  }
}
