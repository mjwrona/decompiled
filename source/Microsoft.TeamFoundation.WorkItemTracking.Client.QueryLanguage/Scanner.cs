// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Scanner
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  internal class Scanner
  {
    private List<Node> m_tokens;
    private int m_pos;

    public Scanner(List<Node> tokens) => this.m_tokens = tokens;

    private void ThrowSyntaxError(SyntaxError err) => throw new SyntaxException(this.m_pos < this.m_tokens.Count ? this.m_tokens[this.m_pos] : (Node) null, err);

    private NodeItem NextToken() => this.m_pos < this.m_tokens.Count ? (NodeItem) this.m_tokens[this.m_pos++] : (NodeItem) null;

    private NodeItem ScanToken(NodeType type, string str)
    {
      int pos = this.m_pos;
      NodeItem nodeItem = this.NextToken();
      if (nodeItem != null && nodeItem.NodeType == type && string.Equals(nodeItem.Value, str, StringComparison.OrdinalIgnoreCase))
        return nodeItem;
      this.m_pos = pos;
      return (NodeItem) null;
    }

    private NodeItem EnsureToken(NodeType type, string str, SyntaxError err)
    {
      NodeItem nodeItem = this.ScanToken(type, str);
      if (nodeItem != null)
        return nodeItem;
      this.ThrowSyntaxError(err);
      return nodeItem;
    }

    private NodeFieldName ScanFieldName(NodeTableName table)
    {
      Node node;
      if (this.TryScanFieldName(out node))
        return (NodeFieldName) node;
      this.ThrowSyntaxError(SyntaxError.ExpectingFieldName);
      return (NodeFieldName) null;
    }

    private bool TryScanFieldName(out Node node)
    {
      node = (Node) null;
      int pos = this.m_pos;
      Node node1 = (Node) this.NextToken();
      if (node1 != null && node1.NodeType == NodeType.Name)
      {
        if (this.ScanToken(NodeType.Operation, ".") != null)
        {
          Node n = (Node) this.NextToken();
          if (n != null && n.NodeType == NodeType.Name)
            node = (Node) new NodeFieldName((NodeName) node1, (NodeName) n);
        }
        else
          node = (Node) new NodeFieldName((NodeName) node1);
      }
      if (node != null)
        return true;
      this.m_pos = pos;
      return false;
    }

    private NodeTableName ScanTableName()
    {
      int pos = this.m_pos;
      NodeItem n = this.NextToken();
      if (n != null && n.NodeType == NodeType.Name)
        return new NodeTableName((NodeName) n);
      this.m_pos = pos;
      this.ThrowSyntaxError(SyntaxError.ExpectingTableName);
      return (NodeTableName) null;
    }

    private NodeFieldList ScanFieldNameList(bool direction, NodeType nodeType)
    {
      NodeFieldList nodeFieldList = new NodeFieldList(nodeType);
      do
      {
        NodeFieldName nodeFieldName = this.ScanFieldName((NodeTableName) null);
        if (direction)
        {
          if (this.ScanToken(NodeType.Name, "asc") != null)
            nodeFieldName.Direction = Direction.Ascending;
          else if (this.ScanToken(NodeType.Name, "desc") != null)
            nodeFieldName.Direction = Direction.Descending;
          if (this.ScanToken(NodeType.Name, "nulls") != null)
          {
            if (this.ScanToken(NodeType.Name, "first") != null)
              nodeFieldName.NullsFirst = new bool?(true);
            else if (this.ScanToken(NodeType.Name, "last") != null)
              nodeFieldName.NullsFirst = new bool?(false);
            else
              this.ThrowSyntaxError(SyntaxError.ExpectingFirstOrLast);
          }
        }
        nodeFieldList.Add((Node) nodeFieldName);
      }
      while (this.ScanToken(NodeType.Operation, ",") != null);
      if (nodeFieldList.Count != 0)
      {
        nodeFieldList.StartOffset = nodeFieldList[0].StartOffset;
        nodeFieldList.EndOffset = nodeFieldList[nodeFieldList.Count - 1].EndOffset;
      }
      return nodeFieldList;
    }

    private Condition ScanConditionOperator(bool afterNotOrEver, out int endOffset)
    {
      int pos = this.m_pos;
      NodeItem nodeItem1 = this.NextToken();
      if (nodeItem1 != null && (nodeItem1.NodeType == NodeType.Name || nodeItem1.NodeType == NodeType.Operation && !afterNotOrEver))
      {
        Condition condition = ConditionalOperators.Find(nodeItem1.Value);
        NodeItem nodeItem2 = (NodeItem) null;
        if (condition == Condition.In && (nodeItem2 = this.ScanToken(NodeType.Name, "group")) != null)
          condition = Condition.Group;
        else if (condition == Condition.Contains && (nodeItem2 = this.ScanToken(NodeType.Name, "words")) != null)
          condition = Condition.ContainsWords;
        if (condition != Condition.None)
        {
          endOffset = (nodeItem2 ?? nodeItem1).EndOffset;
          return condition;
        }
        if (nodeItem1.NodeType == NodeType.Name && string.Equals(nodeItem1.Value, "is", StringComparison.OrdinalIgnoreCase))
        {
          NodeItem nodeItem3;
          if ((nodeItem3 = this.ScanToken(NodeType.Name, "empty")) != null)
          {
            endOffset = nodeItem3.EndOffset;
            return Condition.IsEmpty;
          }
          if (this.ScanToken(NodeType.Name, "not") == null)
            throw new SyntaxException(ResourceStrings.Get("ExpectingEmptyNotEmptyAfterIs"));
          NodeItem nodeItem4;
          if ((nodeItem4 = this.ScanToken(NodeType.Name, "empty")) == null)
            throw new SyntaxException(ResourceStrings.Get("ExpectingEmptyAfterIsNot"));
          endOffset = nodeItem4.EndOffset;
          return Condition.IsNotEmpty;
        }
      }
      this.m_pos = pos;
      endOffset = nodeItem1 != null ? nodeItem1.EndOffset : -1;
      return !afterNotOrEver ? Condition.None : Condition.Equals;
    }

    private Node ScanValueList(NodeTableName table)
    {
      NodeValueList nodeValueList = new NodeValueList();
      do
      {
        nodeValueList.Add(this.ScanExpression(table));
      }
      while (this.ScanToken(NodeType.Operation, ",") != null);
      nodeValueList.StartOffset = nodeValueList[0].StartOffset;
      nodeValueList.EndOffset = nodeValueList[nodeValueList.Count - 1].EndOffset;
      return (Node) nodeValueList;
    }

    private NodeMode ScanMode()
    {
      this.EnsureToken(NodeType.Operation, "(", SyntaxError.ExpectingLeftBracket);
      NodeMode nodeMode = new NodeMode();
      do
      {
        int pos = this.m_pos;
        NodeItem nodeItem = this.NextToken();
        if (nodeItem == null || nodeItem.NodeType != NodeType.Name)
        {
          this.m_pos = pos;
          this.ThrowSyntaxError(SyntaxError.ExpectingMode);
          return (NodeMode) null;
        }
        nodeMode.Add((Node) nodeItem);
      }
      while (this.ScanToken(NodeType.Operation, ",") != null);
      this.EnsureToken(NodeType.Operation, ")", SyntaxError.ExpectingRightBracket);
      nodeMode.StartOffset = nodeMode[0].StartOffset;
      nodeMode.EndOffset = nodeMode[nodeMode.Count - 1].EndOffset;
      return nodeMode;
    }

    private bool TryScanExpression(out Node node)
    {
      if (!this.TryScanSingleValue(out node))
        return false;
      int pos = this.m_pos;
      NodeItem nodeItem = this.NextToken();
      if (nodeItem != null)
      {
        Arithmetic arithmetic = Arithmetic.None;
        Node node1 = (Node) null;
        if (nodeItem.NodeType == NodeType.Number)
        {
          arithmetic = ArithmeticalOperators.Find(nodeItem.Value.Substring(0, 1));
          if (arithmetic != Arithmetic.None)
          {
            nodeItem.Value = nodeItem.Value.Substring(1);
            node1 = (Node) nodeItem;
          }
        }
        else if (nodeItem.NodeType == NodeType.Operation)
        {
          arithmetic = ArithmeticalOperators.Find(nodeItem.Value);
          if (arithmetic != Arithmetic.None && !this.TryScanSingleValue(out node1))
            throw new SyntaxException(string.Format(ResourceStrings.Get("ExpectingNumberAfterOp"), (object) string.Format("{0} {1}", (object) node, (object) nodeItem.Value)));
        }
        if (arithmetic != Arithmetic.None && node1 != null)
        {
          NodeArithmetic nodeArithmetic = new NodeArithmetic();
          nodeArithmetic.Arithmetic = arithmetic;
          nodeArithmetic.Left = node;
          nodeArithmetic.Right = node1;
          nodeArithmetic.StartOffset = nodeArithmetic.Left.StartOffset;
          nodeArithmetic.EndOffset = nodeArithmetic.Right.EndOffset;
          node = (Node) nodeArithmetic;
          return true;
        }
      }
      this.m_pos = pos;
      return true;
    }

    private Node ScanExpression(NodeTableName table)
    {
      Node node;
      if (!this.TryScanExpression(out node))
        this.ThrowSyntaxError(SyntaxError.ExpectingValue);
      return node;
    }

    private Node ScanVariableParameters(NodeVariable variable)
    {
      if (this.ScanToken(NodeType.Operation, "(") != null)
      {
        NodeItem nodeItem1;
        do
        {
          NodeItem nodeItem2 = this.NextToken();
          if (nodeItem2 == null || nodeItem2.NodeType != NodeType.String && nodeItem2.NodeType != NodeType.Number && nodeItem2.NodeType != NodeType.BoolValue)
            this.ThrowSyntaxError(SyntaxError.ExpectingValue);
          variable.Parameters.Arguments.Add(nodeItem2);
          nodeItem1 = this.NextToken();
          if (nodeItem1 != null && nodeItem1.NodeType == NodeType.Operation && string.Equals(nodeItem1?.Value, ")"))
            return (Node) variable;
        }
        while (nodeItem1 != null && nodeItem1.NodeType == NodeType.Operation && string.Equals(nodeItem1?.Value, ","));
        this.ThrowSyntaxError(SyntaxError.ExpectingRightBracket);
      }
      return (Node) variable;
    }

    private bool TryScanSingleValue(out Node node)
    {
      node = (Node) null;
      int pos = this.m_pos;
      Node variable = (Node) this.NextToken();
      if (variable != null)
      {
        if (variable.NodeType == NodeType.Variable)
        {
          node = this.ScanVariableParameters((NodeVariable) variable);
          return true;
        }
        if (variable.NodeType == NodeType.Number || variable.NodeType == NodeType.String || variable.NodeType == NodeType.BoolValue)
        {
          node = variable;
          return true;
        }
      }
      this.m_pos = pos;
      return false;
    }

    private Node ScanValue(NodeTableName table)
    {
      Node node;
      if (!this.TryScanSingleValue(out node))
        this.ThrowSyntaxError(SyntaxError.ExpectingValue);
      return node;
    }

    private Node ScanConditionExpression(NodeTableName table)
    {
      Node node1 = (Node) this.ScanToken(NodeType.Operation, "(");
      if (node1 != null)
      {
        Node node2 = this.ScanWhere(table);
        Node node3 = (Node) this.EnsureToken(NodeType.Operation, ")", SyntaxError.ExpectingLeftBracket);
        node2.StartOffset = node1.StartOffset;
        node2.EndOffset = node3.EndOffset;
        node2.HasParentheses = true;
        return node2;
      }
      NodeCondition nodeCondition = new NodeCondition();
      Node node4 = (Node) nodeCondition;
      nodeCondition.Left = this.ScanFieldName(table);
      Stack stack = new Stack();
      while (true)
      {
        while (this.ScanToken(NodeType.Name, "not") == null)
        {
          if (this.ScanToken(NodeType.Name, "ever") != null)
            stack.Push((object) NodeType.Ever);
          else if (this.ScanToken(NodeType.Name, "never") != null)
          {
            stack.Push((object) NodeType.Not);
            stack.Push((object) NodeType.Ever);
          }
          else
          {
            while (stack.Count > 0)
            {
              switch ((NodeType) stack.Pop())
              {
                case NodeType.Not:
                  node4 = (Node) new NodeNotOperator(node4);
                  continue;
                case NodeType.Ever:
                  node4 = (Node) new NodeEverOperator(node4);
                  continue;
                default:
                  continue;
              }
            }
            int endOffset;
            nodeCondition.Condition = this.ScanConditionOperator(node4 != nodeCondition, out endOffset);
            if (nodeCondition.Condition == Condition.None)
              this.ThrowSyntaxError(SyntaxError.ExpectingComparisonOperator);
            if (nodeCondition.Condition == Condition.Group)
              nodeCondition.Right = this.ScanValue(table);
            else if (nodeCondition.Condition == Condition.In)
            {
              Node node5 = (Node) this.EnsureToken(NodeType.Operation, "(", SyntaxError.ExpectingLeftBracket);
              Node node6 = this.ScanValueList(table);
              Node node7 = (Node) this.EnsureToken(NodeType.Operation, ")", SyntaxError.ExpectingRightBracket);
              node6.StartOffset = node5.StartOffset;
              node6.EndOffset = node7.EndOffset;
              nodeCondition.Right = node6;
            }
            else if (nodeCondition.Condition == Condition.Under)
            {
              Node node8;
              if (this.TryScanExpression(out node8))
                nodeCondition.Right = node8;
              else
                this.ThrowSyntaxError(SyntaxError.ExpectingExpression);
            }
            else if (nodeCondition.Condition != Condition.IsEmpty && nodeCondition.Condition != Condition.IsNotEmpty)
            {
              Node node9;
              if (this.TryScanFieldName(out node9) || this.TryScanExpression(out node9))
                nodeCondition.Right = node9;
              else
                this.ThrowSyntaxError(SyntaxError.ExpectingFieldOrExpression);
              node4.StartOffset = nodeCondition.Left.StartOffset;
              node4.EndOffset = nodeCondition.Right.EndOffset;
            }
            else
            {
              node4.StartOffset = nodeCondition.Left.StartOffset;
              node4.EndOffset = endOffset;
            }
            return node4;
          }
        }
        stack.Push((object) NodeType.Not);
      }
    }

    private Node ScanConditionNotEver(NodeTableName table)
    {
      Node node1 = (Node) this.ScanToken(NodeType.Name, "not") ?? (Node) this.ScanToken(NodeType.Operation, "!");
      if (node1 != null)
      {
        Node node2 = this.ScanConditionNotEver(table);
        NodeNotOperator nodeNotOperator = new NodeNotOperator(node2);
        nodeNotOperator.StartOffset = node1.StartOffset;
        nodeNotOperator.EndOffset = node2.EndOffset;
        return (Node) nodeNotOperator;
      }
      Node node3 = (Node) this.ScanToken(NodeType.Name, "ever");
      if (node3 != null)
      {
        Node node4 = this.ScanConditionNotEver(table);
        NodeEverOperator nodeEverOperator = new NodeEverOperator(node4);
        nodeEverOperator.StartOffset = node3.StartOffset;
        nodeEverOperator.EndOffset = node4.EndOffset;
        return (Node) nodeEverOperator;
      }
      Node node5 = (Node) this.ScanToken(NodeType.Name, "never");
      if (node5 == null)
        return this.ScanConditionExpression(table);
      Node node6 = this.ScanConditionNotEver(table);
      NodeNotOperator nodeNotOperator1 = new NodeNotOperator((Node) new NodeEverOperator(node6));
      nodeNotOperator1.StartOffset = node5.StartOffset;
      nodeNotOperator1.EndOffset = node6.EndOffset;
      return (Node) nodeNotOperator1;
    }

    private Node ScanConditionAnd(NodeTableName table)
    {
      Node node = this.ScanConditionNotEver(table);
      NodeAndOperator nodeAndOperator = (NodeAndOperator) null;
      while (((Node) this.ScanToken(NodeType.Name, "and") ?? (Node) this.ScanToken(NodeType.Operation, "&&")) != null)
      {
        if (nodeAndOperator == null)
        {
          nodeAndOperator = new NodeAndOperator();
          nodeAndOperator.Add(node);
        }
        nodeAndOperator.Add(this.ScanConditionNotEver(table));
      }
      if (nodeAndOperator != null)
      {
        nodeAndOperator.StartOffset = nodeAndOperator[0].StartOffset;
        nodeAndOperator.EndOffset = nodeAndOperator[nodeAndOperator.Count - 1].EndOffset;
        node = (Node) nodeAndOperator;
      }
      return node;
    }

    private Node ScanConditionOr(NodeTableName table)
    {
      Node node = this.ScanConditionAnd(table);
      NodeOrOperator nodeOrOperator = (NodeOrOperator) null;
      while (((Node) this.ScanToken(NodeType.Name, "or") ?? (Node) this.ScanToken(NodeType.Operation, "||")) != null)
      {
        if (nodeOrOperator == null)
        {
          nodeOrOperator = new NodeOrOperator();
          nodeOrOperator.Add(node);
        }
        nodeOrOperator.Add(this.ScanConditionAnd(table));
      }
      if (nodeOrOperator != null)
      {
        nodeOrOperator.StartOffset = nodeOrOperator[0].StartOffset;
        nodeOrOperator.EndOffset = nodeOrOperator[nodeOrOperator.Count - 1].EndOffset;
        node = (Node) nodeOrOperator;
      }
      return node;
    }

    private Node ScanWhere(NodeTableName table) => this.ScanConditionOr(table);

    private NodeSelect ScanSelect()
    {
      Node node = (Node) this.EnsureToken(NodeType.Name, "select", SyntaxError.ExpectingSelect);
      NodeSelect nodeSelect = new NodeSelect();
      nodeSelect.Fields = this.ScanToken(NodeType.Operation, "*") == null ? this.ScanFieldNameList(false, NodeType.FieldList) : (NodeFieldList) null;
      if (this.ScanToken(NodeType.Name, "from") != null)
        nodeSelect.From = this.ScanTableName();
      else if (!this.FieldsHasFrom(nodeSelect.Fields))
        this.ThrowSyntaxError(SyntaxError.FromIsNotSpecified);
      else
        this.ThrowSyntaxError(SyntaxError.FromIsNotSpecifiedFoundInFields);
      while (true)
      {
        if (this.ScanToken(NodeType.Name, "from") != null)
        {
          --this.m_pos;
          this.ThrowSyntaxError(SyntaxError.DuplicateFrom);
        }
        if (this.ScanToken(NodeType.Name, "where") != null)
        {
          if (nodeSelect.Where != null)
          {
            --this.m_pos;
            this.ThrowSyntaxError(SyntaxError.DuplicateWhere);
          }
          nodeSelect.Where = this.ScanWhere(nodeSelect.From);
        }
        else if (this.ScanToken(NodeType.Name, "group") != null)
        {
          this.EnsureToken(NodeType.Name, "by", SyntaxError.ExpectingBy);
          if (nodeSelect.GroupBy != null)
          {
            --this.m_pos;
            this.ThrowSyntaxError(SyntaxError.DuplicateGroupBy);
          }
          nodeSelect.GroupBy = this.ScanFieldNameList(false, NodeType.GroupFieldList);
        }
        else if (this.ScanToken(NodeType.Name, "order") != null)
        {
          this.EnsureToken(NodeType.Name, "by", SyntaxError.ExpectingBy);
          if (nodeSelect.OrderBy != null)
          {
            --this.m_pos;
            this.ThrowSyntaxError(SyntaxError.DuplicateOrderBy);
          }
          nodeSelect.OrderBy = this.ScanFieldNameList(true, NodeType.OrderFieldList);
        }
        else if (this.ScanToken(NodeType.Name, "asof") != null)
        {
          if (nodeSelect.AsOf != null)
          {
            --this.m_pos;
            this.ThrowSyntaxError(SyntaxError.DuplicateAsOf);
          }
          nodeSelect.AsOf = this.ScanValue(nodeSelect.From);
        }
        else if (this.ScanToken(NodeType.Name, "mode") != null)
        {
          if (nodeSelect.Mode != null)
          {
            --this.m_pos;
            this.ThrowSyntaxError(SyntaxError.DuplicateMode);
          }
          nodeSelect.Mode = this.ScanMode();
        }
        else
          break;
      }
      nodeSelect.StartOffset = node.StartOffset;
      nodeSelect.EndOffset = this.m_tokens[this.m_pos - 1].EndOffset;
      return nodeSelect;
    }

    private bool FieldsHasFrom(NodeFieldList fields)
    {
      if (fields != null)
      {
        for (int i = 0; i < fields.Count; ++i)
        {
          if (fields[i] != null && string.Equals(fields[i].Value, "from", StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    public NodeSelect Scan() => this.ScanSelect();

    public void CheckTail()
    {
      int pos = this.m_pos;
      NodeItem nodeItem = this.NextToken();
      if (nodeItem == null || nodeItem.NodeType == NodeType.Operation && nodeItem.Value.Length == 0)
        return;
      this.m_pos = pos;
      this.ThrowSyntaxError(SyntaxError.ExpectingEndOfString);
    }
  }
}
