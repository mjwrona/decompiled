// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.Expressions.ReplaceWithAbstractTokensNodeRewriter
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.Expressions
{
  public class ReplaceWithAbstractTokensNodeRewriter : QueryNodeRewriter
  {
    public const string PropertyToken = "{property}";
    public const string ConstantToken = "{constant}";
    public const string AliasToken = "{alias}";
    public static readonly ConstantNode ConstantTokenNode = new ConstantNode((object) string.Empty, "{constant}", (IEdmTypeReference) null);
    private readonly string m_scopeProperty;

    public ReplaceWithAbstractTokensNodeRewriter(string scopeProperty) => this.m_scopeProperty = scopeProperty;

    public override QueryNode Visit(BinaryOperatorNode nodeIn)
    {
      SingleValueNode left = (SingleValueNode) nodeIn.Left.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      SingleValueNode right = (SingleValueNode) nodeIn.Right.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      return (QueryNode) new BinaryOperatorNode(this.BinaryOperatorToAbstract(nodeIn.OperatorKind), left, right);
    }

    private BinaryOperatorKind BinaryOperatorToAbstract(BinaryOperatorKind operatorKind)
    {
      switch (operatorKind)
      {
        case BinaryOperatorKind.Or:
        case BinaryOperatorKind.And:
        case BinaryOperatorKind.Add:
        case BinaryOperatorKind.Subtract:
        case BinaryOperatorKind.Multiply:
        case BinaryOperatorKind.Divide:
        case BinaryOperatorKind.Modulo:
        case BinaryOperatorKind.Has:
          return operatorKind;
        case BinaryOperatorKind.Equal:
        case BinaryOperatorKind.NotEqual:
        case BinaryOperatorKind.GreaterThan:
        case BinaryOperatorKind.GreaterThanOrEqual:
        case BinaryOperatorKind.LessThan:
        case BinaryOperatorKind.LessThanOrEqual:
          return BinaryOperatorKind.Equal;
        default:
          throw new ArgumentOutOfRangeException(AnalyticsResources.UNKNOWN_BINARY_OPERATOR(), nameof (operatorKind));
      }
    }

    public override QueryNode Visit(SingleValuePropertyAccessNode nodeIn) => nodeIn.Property.Name != this.m_scopeProperty ? (QueryNode) new SingleValueOpenPropertyAccessNode((SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this), "{property}") : base.Visit(nodeIn);

    public override QueryNode Visit(ConstantNode nodeIn) => (QueryNode) ReplaceWithAbstractTokensNodeRewriter.ConstantTokenNode;

    public override QueryNode Visit(ConvertNode nodeIn)
    {
      SingleValueNode source = (SingleValueNode) nodeIn.Source.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
      if (source.TypeReference?.Definition == null || nodeIn.TypeReference.Definition == source.TypeReference.Definition)
        return (QueryNode) source;
      if (source != nodeIn.Source)
        nodeIn = new ConvertNode(source, nodeIn.TypeReference);
      return (QueryNode) nodeIn;
    }

    public override QueryNode Visit(SingleValueFunctionCallNode nodeIn) => nodeIn.Name == "cast" ? nodeIn.Parameters.First<QueryNode>().Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this) : base.Visit(nodeIn);

    public override QueryNode Visit(UnaryOperatorNode nodeIn) => nodeIn.Operand.Accept<QueryNode>((QueryNodeVisitor<QueryNode>) this);
  }
}
