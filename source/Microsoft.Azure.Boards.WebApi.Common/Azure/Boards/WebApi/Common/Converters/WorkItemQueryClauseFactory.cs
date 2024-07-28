// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemQueryClauseFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  internal static class WorkItemQueryClauseFactory
  {
    public static WorkItemQueryClause BuildQueryClauses(
      WorkItemTrackingRequestContext witRequestContext,
      QueryExpression queryExpression,
      WorkItemQueryClauseFactory.ClauseType clauseType,
      bool includeUrls = true,
      bool useIsoDateFormat = false)
    {
      QueryExpressionNode queryExpressionNode;
      switch (clauseType)
      {
        case WorkItemQueryClauseFactory.ClauseType.SourceClause:
          queryExpressionNode = queryExpression.LeftGroup;
          break;
        case WorkItemQueryClauseFactory.ClauseType.TargetClause:
          queryExpressionNode = queryExpression.RightGroup;
          break;
        case WorkItemQueryClauseFactory.ClauseType.LinkClause:
          queryExpressionNode = queryExpression.LinkGroup;
          break;
        default:
          throw new NotSupportedException();
      }
      return WorkItemQueryClauseFactory.GetQueryClauses(witRequestContext, queryExpressionNode, includeUrls, useIsoDateFormat);
    }

    private static WorkItemQueryClause GetQueryClauses(
      WorkItemTrackingRequestContext witRequestContext,
      QueryExpressionNode queryExpressionNode,
      bool includeUrls = true,
      bool useIsoDateFormat = false)
    {
      switch (queryExpressionNode)
      {
        case QueryLogicalExpressionNode _:
          if (queryExpressionNode is QueryLogicalExpressionNode logicalExpressionNode)
          {
            QueryLogicalExpressionOperator expressionOperator = logicalExpressionNode.Operator;
            IEnumerable<QueryExpressionNode> source = logicalExpressionNode.Children ?? Enumerable.Empty<QueryExpressionNode>();
            return new WorkItemQueryClause()
            {
              LogicalOperator = expressionOperator == QueryLogicalExpressionOperator.And ? WorkItemQueryClause.LogicalOperation.AND : WorkItemQueryClause.LogicalOperation.OR,
              Clauses = (IEnumerable<WorkItemQueryClause>) source.Select<QueryExpressionNode, WorkItemQueryClause>((Func<QueryExpressionNode, WorkItemQueryClause>) (node => WorkItemQueryClauseFactory.GetQueryClauses(witRequestContext, node, includeUrls, useIsoDateFormat))).ToList<WorkItemQueryClause>()
            };
          }
          break;
        case QueryComparisonExpressionNode _:
          if (queryExpressionNode is QueryComparisonExpressionNode queryComparisonExpressionNode)
            return WorkItemQueryClauseFactory.GetComparisonNode(witRequestContext, queryComparisonExpressionNode, includeUrls, useIsoDateFormat);
          break;
      }
      return (WorkItemQueryClause) null;
    }

    private static WorkItemQueryClause GetComparisonNode(
      WorkItemTrackingRequestContext witRequestContext,
      QueryComparisonExpressionNode queryComparisonExpressionNode,
      bool includeUrls = true,
      bool isoDateFormat = false)
    {
      string str = (string) null;
      bool? nullable = new bool?();
      WorkItemFieldReference itemFieldReference1 = (WorkItemFieldReference) null;
      FieldEntry fieldEntry = queryComparisonExpressionNode.Fields.FirstOrDefault<FieldEntry>();
      QueryExpressionValue nodeValue = queryComparisonExpressionNode.Value;
      if (nodeValue.ValueType == QueryExpressionValueType.Column)
      {
        nullable = new bool?(true);
        WorkItemTrackingRequestContext witRequestContext1 = witRequestContext;
        FieldEntry columnValue = nodeValue.ColumnValue;
        bool flag = includeUrls;
        Guid? projectId = new Guid?();
        int num = flag ? 1 : 0;
        itemFieldReference1 = WorkItemFieldReferenceFactory.Create(witRequestContext1, columnValue, projectId: projectId, includeUrl: num != 0);
      }
      else if (fieldEntry.FieldId == 100)
      {
        int numberValue = nodeValue.NumberValue;
        str = WorkItemQueryClauseFactory.GetLinkTypeFromId(witRequestContext, numberValue);
      }
      else
        str = WorkItemQueryClauseFactory.GetFieldValue(nodeValue, isoDateFormat);
      WorkItemFieldOperation itemFieldOperation = new WorkItemFieldOperation()
      {
        ReferenceName = WiqlOperatorHelper.GetSupportedOperationReferenceName(queryComparisonExpressionNode.Operator, queryComparisonExpressionNode.ExpandConstant),
        Name = WiqlOperatorHelper.GetSupportedOperationName(queryComparisonExpressionNode.Operator, queryComparisonExpressionNode.ExpandConstant)
      };
      WorkItemTrackingRequestContext witRequestContext2 = witRequestContext;
      FieldEntry field = fieldEntry;
      bool flag1 = includeUrls;
      Guid? projectId1 = new Guid?();
      int num1 = flag1 ? 1 : 0;
      WorkItemFieldReference itemFieldReference2 = WorkItemFieldReferenceFactory.Create(witRequestContext2, field, projectId: projectId1, includeUrl: num1 != 0);
      return new WorkItemQueryClause()
      {
        Field = itemFieldReference2,
        Operator = itemFieldOperation,
        Value = !string.IsNullOrEmpty(str) || nullable.HasValue ? str : string.Empty,
        IsFieldValue = nullable,
        FieldValue = itemFieldReference1
      };
    }

    private static string GetLinkTypeFromId(
      WorkItemTrackingRequestContext witRequestContext,
      int linkTypeId)
    {
      try
      {
        MDWorkItemLinkType linkTypeById = witRequestContext.LinkService.GetLinkTypeById(witRequestContext.RequestContext, linkTypeId);
        return (linkTypeId == linkTypeById.ForwardEnd.Id ? linkTypeById.ForwardEnd : linkTypeById.ReverseEnd).ReferenceName;
      }
      catch (WorkItemTrackingLinkTypeNotFoundException ex)
      {
        return string.Empty;
      }
    }

    private static string GetFieldValue(QueryExpressionValue nodeValue, bool useIsoDateFormat = false)
    {
      if (nodeValue.IsNull)
        return string.Empty;
      if (nodeValue.IsVariable)
      {
        string macro = WiqlOperators.GetMacro(nodeValue.StringValue);
        return !string.IsNullOrEmpty(nodeValue.ArgumentsString) ? macro + nodeValue.ArgumentsString : macro;
      }
      switch (nodeValue.ValueType)
      {
        case QueryExpressionValueType.Array:
          return WorkItemQueryClauseFactory.GetNodeArrayValue(nodeValue.GetArrayValue(), nodeValue.IsArithmetic);
        case QueryExpressionValueType.String:
          return nodeValue.StringValue;
        case QueryExpressionValueType.Number:
          return nodeValue.NumberValue.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
        case QueryExpressionValueType.DateTime:
          return useIsoDateFormat ? nodeValue.DateValue.ToString("o") : nodeValue.DateValue.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
        case QueryExpressionValueType.UniqueIdentifier:
          return nodeValue.GuidValue.ToString();
        case QueryExpressionValueType.Boolean:
          return nodeValue.BoolValue.ToString();
        case QueryExpressionValueType.Double:
          return nodeValue.DoubleValue.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
        default:
          throw new NotSupportedException();
      }
    }

    private static string GetNodeArrayValue(
      QueryExpressionValue[] queryExpressionValues,
      bool isArithmetic)
    {
      if (queryExpressionValues == null || !((IEnumerable<QueryExpressionValue>) queryExpressionValues).Any<QueryExpressionValue>())
        return string.Empty;
      string[] strArray = new string[queryExpressionValues.Length];
      for (int index = 0; index < queryExpressionValues.Length; ++index)
        strArray[index] = WorkItemQueryClauseFactory.GetFieldValue(queryExpressionValues[index]);
      if (!isArithmetic)
        return string.Join(",", strArray);
      if (strArray.Length <= 3)
        return string.Join(" ", strArray);
      List<string> values = new List<string>();
      for (int startIndex = 0; startIndex < strArray.Length; startIndex += 3)
      {
        if (startIndex + 3 <= strArray.Length)
          values.Add(string.Join(" ", strArray, startIndex, 3));
      }
      return string.Join(",", (IEnumerable<string>) values);
    }

    public enum ClauseType
    {
      SourceClause,
      TargetClause,
      LinkClause,
    }
  }
}
