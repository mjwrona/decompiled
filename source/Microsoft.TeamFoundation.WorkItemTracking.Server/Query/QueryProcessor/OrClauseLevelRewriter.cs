// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.OrClauseLevelRewriter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor
{
  public static class OrClauseLevelRewriter
  {
    public static bool CanRewrite(QueryLogicalExpressionNode logicalNode) => logicalNode != null && logicalNode.Operator == QueryLogicalExpressionOperator.And && logicalNode.Children.Any<QueryExpressionNode>((Func<QueryExpressionNode, bool>) (c => c is QueryLogicalExpressionNode logicalExpressionNode && logicalExpressionNode.Operator == QueryLogicalExpressionOperator.Or));

    public static void Rewrite(QueryLogicalExpressionNode logicalNode)
    {
      List<QueryExpressionNode> notOrClause = new List<QueryExpressionNode>();
      QueryLogicalExpressionNode logicalExpressionNode1 = (QueryLogicalExpressionNode) null;
      foreach (QueryExpressionNode child in logicalNode.Children)
      {
        if (logicalExpressionNode1 == null && child is QueryLogicalExpressionNode logicalExpressionNode2 && logicalExpressionNode2.Operator == QueryLogicalExpressionOperator.Or)
          logicalExpressionNode1 = logicalExpressionNode2;
        else
          notOrClause.Add(child);
      }
      IEnumerable<QueryLogicalExpressionNode> source = logicalExpressionNode1.Children.Select<QueryExpressionNode, QueryLogicalExpressionNode>((Func<QueryExpressionNode, QueryLogicalExpressionNode>) (c => new QueryLogicalExpressionNode()
      {
        Operator = QueryLogicalExpressionOperator.And,
        Children = OrClauseLevelRewriter.AppendChildren(c, (IEnumerable<QueryExpressionNode>) notOrClause)
      }));
      logicalNode.Operator = QueryLogicalExpressionOperator.Or;
      logicalNode.Children = (IEnumerable<QueryExpressionNode>) source.ToList<QueryLogicalExpressionNode>();
    }

    private static IEnumerable<QueryExpressionNode> AppendChildren(
      QueryExpressionNode first,
      IEnumerable<QueryExpressionNode> others)
    {
      List<QueryExpressionNode> queryExpressionNodeList = new List<QueryExpressionNode>();
      queryExpressionNodeList.Add(first);
      queryExpressionNodeList.AddRange(others);
      return (IEnumerable<QueryExpressionNode>) queryExpressionNodeList;
    }
  }
}
