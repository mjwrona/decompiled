// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.NodeToStringBuilder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.OData
{
  internal sealed class NodeToStringBuilder : QueryNodeVisitor<string>
  {
    private bool searchFlag;

    public override string Visit(AllNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<AllNode>(node, nameof (node));
      return this.TranslateNode((QueryNode) node.Source) + "/" + "all" + "(" + node.CurrentRangeVariable.Name + ":" + this.TranslateNode((QueryNode) node.Body) + ")";
    }

    public override string Visit(AnyNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<AnyNode>(node, nameof (node));
      return node.CurrentRangeVariable == null && node.Body.Kind == QueryNodeKind.Constant ? this.TranslateNode((QueryNode) node.Source) + "/" + "any" + "(" + ")" : this.TranslateNode((QueryNode) node.Source) + "/" + "any" + "(" + node.CurrentRangeVariable.Name + ":" + this.TranslateNode((QueryNode) node.Body) + ")";
    }

    public override string Visit(BinaryOperatorNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<BinaryOperatorNode>(node, nameof (node));
      string str1 = this.TranslateNode((QueryNode) node.Left);
      if (node.Left.Kind == QueryNodeKind.BinaryOperator && NodeToStringBuilder.TranslateBinaryOperatorPriority(((BinaryOperatorNode) node.Left).OperatorKind) < NodeToStringBuilder.TranslateBinaryOperatorPriority(node.OperatorKind) || node.Left.Kind == QueryNodeKind.Convert && ((ConvertNode) node.Left).Source.Kind == QueryNodeKind.BinaryOperator && NodeToStringBuilder.TranslateBinaryOperatorPriority(((BinaryOperatorNode) ((ConvertNode) node.Left).Source).OperatorKind) < NodeToStringBuilder.TranslateBinaryOperatorPriority(node.OperatorKind))
        str1 = "(" + str1 + ")";
      string str2 = this.TranslateNode((QueryNode) node.Right);
      if (node.Right.Kind == QueryNodeKind.BinaryOperator && NodeToStringBuilder.TranslateBinaryOperatorPriority(((BinaryOperatorNode) node.Right).OperatorKind) < NodeToStringBuilder.TranslateBinaryOperatorPriority(node.OperatorKind) || node.Right.Kind == QueryNodeKind.Convert && ((ConvertNode) node.Right).Source.Kind == QueryNodeKind.BinaryOperator && NodeToStringBuilder.TranslateBinaryOperatorPriority(((BinaryOperatorNode) ((ConvertNode) node.Right).Source).OperatorKind) < NodeToStringBuilder.TranslateBinaryOperatorPriority(node.OperatorKind))
        str2 = "(" + str2 + ")";
      return str1 + (object) ' ' + this.BinaryOperatorNodeToString(node.OperatorKind) + (object) ' ' + str2;
    }

    public override string Visit(InNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<InNode>(node, nameof (node));
      return this.TranslateNode((QueryNode) node.Left) + (object) ' ' + "in" + (object) ' ' + this.TranslateNode((QueryNode) node.Right);
    }

    public override string Visit(CountNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<CountNode>(node, nameof (node));
      return this.TranslateNode((QueryNode) node.Source) + "/" + "$count";
    }

    public override string Visit(CollectionNavigationNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<CollectionNavigationNode>(node, nameof (node));
      return this.TranslatePropertyAccess((QueryNode) node.Source, node.NavigationProperty.Name, node.NavigationSource);
    }

    public override string Visit(CollectionPropertyAccessNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<CollectionPropertyAccessNode>(node, nameof (node));
      return this.TranslatePropertyAccess((QueryNode) node.Source, node.Property.Name);
    }

    public override string Visit(CollectionComplexNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<CollectionComplexNode>(node, nameof (node));
      return this.TranslatePropertyAccess((QueryNode) node.Source, node.Property.Name);
    }

    public override string Visit(ConstantNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<ConstantNode>(node, nameof (node));
      return node.Value == null ? "null" : node.LiteralText;
    }

    public override string Visit(CollectionConstantNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<CollectionConstantNode>(node, nameof (node));
      return string.IsNullOrEmpty(node.LiteralText) ? "null" : node.LiteralText;
    }

    public override string Visit(ConvertNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<ConvertNode>(node, nameof (node));
      return this.TranslateNode((QueryNode) node.Source);
    }

    public override string Visit(CollectionResourceCastNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<CollectionResourceCastNode>(node, nameof (node));
      return this.TranslatePropertyAccess((QueryNode) node.Source, node.ItemStructuredType.Definition.ToString());
    }

    public override string Visit(ResourceRangeVariableReferenceNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<ResourceRangeVariableReferenceNode>(node, nameof (node));
      return node.Name == "$it" ? string.Empty : node.Name;
    }

    public override string Visit(NonResourceRangeVariableReferenceNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<NonResourceRangeVariableReferenceNode>(node, nameof (node));
      return node.Name;
    }

    public override string Visit(SingleResourceCastNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleResourceCastNode>(node, nameof (node));
      return this.TranslatePropertyAccess((QueryNode) node.Source, node.StructuredTypeReference.Definition.ToString());
    }

    public override string Visit(SingleNavigationNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleNavigationNode>(node, nameof (node));
      return this.TranslatePropertyAccess((QueryNode) node.Source, node.NavigationProperty.Name, node.NavigationSource);
    }

    public override string Visit(SingleResourceFunctionCallNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleResourceFunctionCallNode>(node, nameof (node));
      string str = node.Name;
      if (node.Source != null)
        str = this.TranslatePropertyAccess(node.Source, str);
      return this.TranslateFunctionCall(str, node.Parameters);
    }

    public override string Visit(SingleValueFunctionCallNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueFunctionCallNode>(node, nameof (node));
      string str = node.Name;
      if (node.Source != null)
        str = this.TranslatePropertyAccess(node.Source, str);
      return this.TranslateFunctionCall(str, node.Parameters);
    }

    public override string Visit(CollectionFunctionCallNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<CollectionFunctionCallNode>(node, nameof (node));
      string str = node.Name;
      if (node.Source != null)
        str = this.TranslatePropertyAccess(node.Source, str);
      return this.TranslateFunctionCall(str, node.Parameters);
    }

    public override string Visit(CollectionResourceFunctionCallNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<CollectionResourceFunctionCallNode>(node, nameof (node));
      string str = node.Name;
      if (node.Source != null)
        str = this.TranslatePropertyAccess(node.Source, str);
      return this.TranslateFunctionCall(str, node.Parameters);
    }

    public override string Visit(SingleValueOpenPropertyAccessNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueOpenPropertyAccessNode>(node, nameof (node));
      return this.TranslatePropertyAccess((QueryNode) node.Source, node.Name);
    }

    public override string Visit(CollectionOpenPropertyAccessNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<CollectionOpenPropertyAccessNode>(node, nameof (node));
      return this.TranslatePropertyAccess((QueryNode) node.Source, node.Name);
    }

    public override string Visit(SingleValuePropertyAccessNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValuePropertyAccessNode>(node, nameof (node));
      return this.TranslatePropertyAccess((QueryNode) node.Source, node.Property.Name);
    }

    public override string Visit(SingleComplexNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleComplexNode>(node, nameof (node));
      return this.TranslatePropertyAccess((QueryNode) node.Source, node.Property.Name);
    }

    public override string Visit(ParameterAliasNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<ParameterAliasNode>(node, nameof (node));
      return node.Alias;
    }

    public override string Visit(NamedFunctionParameterNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<NamedFunctionParameterNode>(node, nameof (node));
      return node.Name + "=" + this.TranslateNode(node.Value);
    }

    public override string Visit(SearchTermNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<SearchTermNode>(node, nameof (node));
      return !NodeToStringBuilder.IsValidSearchWord(node.Text) ? "\"" + node.Text + "\"" : node.Text;
    }

    public override string Visit(UnaryOperatorNode node)
    {
      ExceptionUtils.CheckArgumentNotNull<UnaryOperatorNode>(node, nameof (node));
      string str = (string) null;
      if (node.OperatorKind == UnaryOperatorKind.Negate)
        str = "-";
      if (node.OperatorKind == UnaryOperatorKind.Not)
        str = !this.searchFlag ? "not" : "NOT";
      return node.Operand.Kind == QueryNodeKind.Constant || node.Operand.Kind == QueryNodeKind.SearchTerm ? str + (object) ' ' + this.TranslateNode((QueryNode) node.Operand) : str + "(" + this.TranslateNode((QueryNode) node.Operand) + ")";
    }

    internal static string TranslateLevelsClause(LevelsClause levelsClause) => levelsClause.IsMaxLevel ? "max" : levelsClause.Level.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    internal string TranslateNode(QueryNode node) => node.Accept<string>((QueryNodeVisitor<string>) this);

    internal string TranslateFilterClause(FilterClause filterClause) => this.TranslateNode((QueryNode) filterClause.Expression);

    internal string TranslateOrderByClause(OrderByClause orderByClause)
    {
      string str = this.TranslateNode((QueryNode) orderByClause.Expression);
      if (orderByClause.Direction == OrderByDirection.Descending)
        str = str + (object) ' ' + "desc";
      orderByClause = orderByClause.ThenBy;
      return orderByClause == null ? str : str + "," + this.TranslateOrderByClause(orderByClause);
    }

    internal string TranslateSearchClause(SearchClause searchClause)
    {
      this.searchFlag = true;
      string str = this.TranslateNode((QueryNode) searchClause.Expression);
      this.searchFlag = false;
      return str;
    }

    internal string TranslateComputeClause(ComputeClause computeClause)
    {
      bool flag = false;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ComputeExpression computedItem in computeClause.ComputedItems)
      {
        if (flag)
          stringBuilder.Append(",");
        else
          flag = true;
        stringBuilder.Append(this.TranslateNode(computedItem.Expression));
        stringBuilder.Append("%20");
        stringBuilder.Append("as");
        stringBuilder.Append("%20");
        stringBuilder.Append(computedItem.Alias);
      }
      return stringBuilder.ToString();
    }

    internal string TranslateParameterAliasNodes(IDictionary<string, SingleValueNode> dictionary)
    {
      string str1 = (string) null;
      if (dictionary != null)
      {
        foreach (KeyValuePair<string, SingleValueNode> keyValuePair in (IEnumerable<KeyValuePair<string, SingleValueNode>>) dictionary)
        {
          if (keyValuePair.Value != null)
          {
            string stringToEscape = this.TranslateNode((QueryNode) keyValuePair.Value);
            string str2;
            if (!string.IsNullOrEmpty(stringToEscape))
              str2 = str1 + (string.IsNullOrEmpty(str1) ? (string) null : "&") + keyValuePair.Key + "=" + Uri.EscapeDataString(stringToEscape);
            else
              str2 = str1;
            str1 = str2;
          }
        }
      }
      return str1;
    }

    private string TranslatePropertyAccess(
      QueryNode sourceNode,
      string edmPropertyName,
      IEdmNavigationSource navigationSource = null)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNode>(sourceNode, nameof (sourceNode));
      ExceptionUtils.CheckArgumentNotNull<string>(edmPropertyName, nameof (edmPropertyName));
      string str = this.TranslateNode(sourceNode);
      return !string.IsNullOrEmpty(str) ? str + "/" + edmPropertyName : edmPropertyName;
    }

    private string TranslateFunctionCall(string functionName, IEnumerable<QueryNode> argumentNodes)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(functionName, nameof (functionName));
      string str = string.Empty;
      foreach (QueryNode argumentNode in argumentNodes)
        str = str + (string.IsNullOrEmpty(str) ? (string) null : ",") + this.TranslateNode(argumentNode);
      return functionName + "(" + str + ")";
    }

    private string BinaryOperatorNodeToString(BinaryOperatorKind operatorKind)
    {
      switch (operatorKind)
      {
        case BinaryOperatorKind.Or:
          return this.searchFlag ? "OR" : "or";
        case BinaryOperatorKind.And:
          return this.searchFlag ? "AND" : "and";
        case BinaryOperatorKind.Equal:
          return "eq";
        case BinaryOperatorKind.NotEqual:
          return "ne";
        case BinaryOperatorKind.GreaterThan:
          return "gt";
        case BinaryOperatorKind.GreaterThanOrEqual:
          return "ge";
        case BinaryOperatorKind.LessThan:
          return "lt";
        case BinaryOperatorKind.LessThanOrEqual:
          return "le";
        case BinaryOperatorKind.Add:
          return "add";
        case BinaryOperatorKind.Subtract:
          return "sub";
        case BinaryOperatorKind.Multiply:
          return "mul";
        case BinaryOperatorKind.Divide:
          return "div";
        case BinaryOperatorKind.Modulo:
          return "mod";
        case BinaryOperatorKind.Has:
          return "has";
        default:
          return (string) null;
      }
    }

    private static int TranslateBinaryOperatorPriority(BinaryOperatorKind operatorKind)
    {
      switch (operatorKind)
      {
        case BinaryOperatorKind.Or:
          return 1;
        case BinaryOperatorKind.And:
          return 2;
        case BinaryOperatorKind.Equal:
        case BinaryOperatorKind.NotEqual:
        case BinaryOperatorKind.GreaterThan:
        case BinaryOperatorKind.GreaterThanOrEqual:
        case BinaryOperatorKind.LessThan:
        case BinaryOperatorKind.LessThanOrEqual:
          return 3;
        case BinaryOperatorKind.Add:
        case BinaryOperatorKind.Subtract:
          return 4;
        case BinaryOperatorKind.Multiply:
        case BinaryOperatorKind.Divide:
        case BinaryOperatorKind.Modulo:
          return 5;
        case BinaryOperatorKind.Has:
          return 6;
        default:
          return -1;
      }
    }

    private static bool IsValidSearchWord(string text) => !SearchLexer.InvalidWordPattern.Match(text).Success && !string.Equals(text, "AND", StringComparison.Ordinal) && !string.Equals(text, "OR", StringComparison.Ordinal) && !string.Equals(text, "NOT", StringComparison.Ordinal);
  }
}
