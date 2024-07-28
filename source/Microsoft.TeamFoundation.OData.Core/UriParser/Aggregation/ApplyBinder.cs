// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.ApplyBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser.Aggregation
{
  internal sealed class ApplyBinder
  {
    private MetadataBinder.QueryTokenVisitor bindMethod;
    private BindingState state;
    private FilterBinder filterBinder;
    private ODataUriParserConfiguration configuration;
    private ODataPathInfo odataPathInfo;
    private IEnumerable<AggregateExpressionBase> aggregateExpressionsCache;

    public ApplyBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
      : this(bindMethod, state, (ODataUriParserConfiguration) null, (ODataPathInfo) null)
    {
    }

    public ApplyBinder(
      MetadataBinder.QueryTokenVisitor bindMethod,
      BindingState state,
      ODataUriParserConfiguration configuration,
      ODataPathInfo odataPathInfo)
    {
      this.bindMethod = bindMethod;
      this.state = state;
      this.filterBinder = new FilterBinder(bindMethod, state);
      this.configuration = configuration;
      this.odataPathInfo = odataPathInfo;
    }

    public ApplyClause BindApply(IEnumerable<QueryToken> tokens)
    {
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<QueryToken>>(tokens, nameof (tokens));
      List<TransformationNode> transformations = new List<TransformationNode>();
      foreach (QueryToken token1 in tokens)
      {
        switch (token1.Kind)
        {
          case QueryTokenKind.Expand:
            ExpandTransformationNode transformationNode1 = new ExpandTransformationNode(SelectExpandSemanticBinder.Bind(this.odataPathInfo, (ExpandToken) token1, (SelectToken) null, this.configuration, (BindingState) null));
            transformations.Add((TransformationNode) transformationNode1);
            continue;
          case QueryTokenKind.Aggregate:
            AggregateTransformationNode transformationNode2 = this.BindAggregateToken((AggregateToken) token1);
            transformations.Add((TransformationNode) transformationNode2);
            this.aggregateExpressionsCache = transformationNode2.AggregateExpressions;
            this.state.AggregatedPropertyNames = new HashSet<EndPathToken>(transformationNode2.AggregateExpressions.Select<AggregateExpressionBase, EndPathToken>((Func<AggregateExpressionBase, EndPathToken>) (statement => new EndPathToken(statement.Alias, (QueryToken) null))));
            this.state.IsCollapsed = true;
            continue;
          case QueryTokenKind.AggregateGroupBy:
            GroupByTransformationNode transformationNode3 = this.BindGroupByToken((GroupByToken) token1);
            transformations.Add((TransformationNode) transformationNode3);
            this.state.IsCollapsed = true;
            continue;
          case QueryTokenKind.Compute:
            ComputeTransformationNode token2 = this.BindComputeToken((ComputeToken) token1);
            transformations.Add((TransformationNode) token2);
            this.state.AggregatedPropertyNames = new HashSet<EndPathToken>(token2.Expressions.Select<ComputeExpression, EndPathToken>((Func<ComputeExpression, EndPathToken>) (statement => new EndPathToken(statement.Alias, (QueryToken) null))));
            continue;
          default:
            FilterTransformationNode transformationNode4 = new FilterTransformationNode(this.filterBinder.BindFilter(token1));
            transformations.Add((TransformationNode) transformationNode4);
            continue;
        }
      }
      return new ApplyClause((IList<TransformationNode>) transformations);
    }

    private AggregateTransformationNode BindAggregateToken(AggregateToken token)
    {
      IEnumerable<AggregateTokenBase> aggregateTokenBases = ApplyBinder.MergeEntitySetAggregates(token.AggregateExpressions);
      List<AggregateExpressionBase> expressions = new List<AggregateExpressionBase>();
      foreach (AggregateTokenBase aggregateToken in aggregateTokenBases)
        expressions.Add(this.BindAggregateExpressionToken(aggregateToken));
      return new AggregateTransformationNode((IEnumerable<AggregateExpressionBase>) expressions);
    }

    private static IEnumerable<AggregateTokenBase> MergeEntitySetAggregates(
      IEnumerable<AggregateTokenBase> tokens)
    {
      List<AggregateTokenBase> first = new List<AggregateTokenBase>();
      Dictionary<string, AggregateTokenBase> dictionary = new Dictionary<string, AggregateTokenBase>();
      foreach (AggregateTokenBase token in tokens)
      {
        switch (token.Kind)
        {
          case QueryTokenKind.AggregateExpression:
            first.Add(token);
            continue;
          case QueryTokenKind.EntitySetAggregateExpression:
            EntitySetAggregateToken token1 = token as EntitySetAggregateToken;
            string key = token1.Path();
            AggregateTokenBase token2;
            if (dictionary.TryGetValue(key, out token2))
              dictionary.Remove(key);
            dictionary.Add(key, (AggregateTokenBase) EntitySetAggregateToken.Merge(token1, token2 as EntitySetAggregateToken));
            continue;
          default:
            continue;
        }
      }
      return (IEnumerable<AggregateTokenBase>) first.Concat<AggregateTokenBase>((IEnumerable<AggregateTokenBase>) dictionary.Values).ToList<AggregateTokenBase>();
    }

    private AggregateExpressionBase BindAggregateExpressionToken(AggregateTokenBase aggregateToken)
    {
      switch (aggregateToken.Kind)
      {
        case QueryTokenKind.AggregateExpression:
          AggregateExpressionToken aggregateExpressionToken = aggregateToken as AggregateExpressionToken;
          SingleValueNode expression1 = this.bindMethod(aggregateExpressionToken.Expression) as SingleValueNode;
          IEdmTypeReference expressionTypeReference = this.CreateAggregateExpressionTypeReference(expression1, aggregateExpressionToken.MethodDefinition);
          return (AggregateExpressionBase) new AggregateExpression(expression1, aggregateExpressionToken.MethodDefinition, aggregateExpressionToken.Alias, expressionTypeReference);
        case QueryTokenKind.EntitySetAggregateExpression:
          EntitySetAggregateToken setAggregateToken = aggregateToken as EntitySetAggregateToken;
          QueryNode expression2 = this.bindMethod(setAggregateToken.EntitySet);
          if (expression2.Kind != QueryNodeKind.CollectionNavigationNode)
            throw new ODataException(Microsoft.OData.Strings.ApplyBinder_UnsupportedForEntitySetAggregation((object) ((setAggregateToken.EntitySet is EndPathToken entitySet ? entitySet.Identifier : (string) null) ?? string.Empty), (object) expression2.Kind));
          this.state.InEntitySetAggregation = true;
          IEnumerable<AggregateExpressionBase> list = (IEnumerable<AggregateExpressionBase>) setAggregateToken.Expressions.Select<AggregateTokenBase, AggregateExpressionBase>((Func<AggregateTokenBase, AggregateExpressionBase>) (x => this.BindAggregateExpressionToken(x))).ToList<AggregateExpressionBase>();
          this.state.InEntitySetAggregation = false;
          return (AggregateExpressionBase) new EntitySetAggregateExpression((CollectionNavigationNode) expression2, list);
        default:
          throw new ODataException(Microsoft.OData.Strings.ApplyBinder_UnsupportedAggregateKind((object) aggregateToken.Kind));
      }
    }

    private IEdmTypeReference CreateAggregateExpressionTypeReference(
      SingleValueNode expression,
      AggregationMethodDefinition method)
    {
      IEdmTypeReference type = expression.TypeReference;
      if (type == null && this.aggregateExpressionsCache != null && expression is SingleValueOpenPropertyAccessNode propertyAccessNode)
        type = this.GetTypeReferenceByPropertyName(propertyAccessNode.Name);
      switch (method.MethodKind)
      {
        case AggregationMethod.Sum:
        case AggregationMethod.Min:
        case AggregationMethod.Max:
          return type;
        case AggregationMethod.Average:
          EdmPrimitiveTypeKind p1 = type.PrimitiveKind();
          switch (p1)
          {
            case EdmPrimitiveTypeKind.None:
              return type;
            case EdmPrimitiveTypeKind.Decimal:
              return (IEdmTypeReference) EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Decimal, type.IsNullable);
            case EdmPrimitiveTypeKind.Double:
            case EdmPrimitiveTypeKind.Int32:
            case EdmPrimitiveTypeKind.Int64:
              return (IEdmTypeReference) EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, type.IsNullable);
            default:
              throw new ODataException(Microsoft.OData.Strings.ApplyBinder_AggregateExpressionIncompatibleTypeForMethod((object) expression, (object) p1));
          }
        case AggregationMethod.CountDistinct:
        case AggregationMethod.VirtualPropertyCount:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int64, false);
        default:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, type.IsNullable);
      }
    }

    private IEdmTypeReference GetTypeReferenceByPropertyName(string name)
    {
      if (this.aggregateExpressionsCache != null)
      {
        AggregateExpression aggregateExpression = this.aggregateExpressionsCache.OfType<AggregateExpression>().FirstOrDefault<AggregateExpression>((Func<AggregateExpression, bool>) (statement => statement.AggregateKind == AggregateExpressionKind.PropertyAggregate && statement.Alias.Equals(name)));
        if (aggregateExpression != null)
          return aggregateExpression.TypeReference;
      }
      return (IEdmTypeReference) null;
    }

    private GroupByTransformationNode BindGroupByToken(GroupByToken token)
    {
      List<GroupByPropertyNode> groupByPropertyNodeList = new List<GroupByPropertyNode>();
      foreach (EndPathToken property in token.Properties)
      {
        QueryNode queryNode = this.bindMethod((QueryToken) property);
        SingleValuePropertyAccessNode node1 = queryNode as SingleValuePropertyAccessNode;
        SingleComplexNode node2 = queryNode as SingleComplexNode;
        if (node1 != null)
          ApplyBinder.RegisterProperty((IList<GroupByPropertyNode>) groupByPropertyNodeList, ApplyBinder.ReversePropertyPath((SingleValueNode) node1));
        else if (node2 != null)
        {
          ApplyBinder.RegisterProperty((IList<GroupByPropertyNode>) groupByPropertyNodeList, ApplyBinder.ReversePropertyPath((SingleValueNode) node2));
        }
        else
        {
          IEdmTypeReference type = queryNode is SingleValueOpenPropertyAccessNode expression ? this.GetTypeReferenceByPropertyName(expression.Name) : throw new ODataException(Microsoft.OData.Strings.ApplyBinder_GroupByPropertyNotPropertyAccessValue((object) property.Identifier));
          groupByPropertyNodeList.Add(new GroupByPropertyNode(expression.Name, (SingleValueNode) expression, type));
        }
      }
      HashSet<EndPathToken> endPathTokenSet = new HashSet<EndPathToken>(token.Properties);
      TransformationNode childTransformations = (TransformationNode) null;
      if (token.Child != null)
      {
        if (token.Child.Kind != QueryTokenKind.Aggregate)
          throw new ODataException(Microsoft.OData.Strings.ApplyBinder_UnsupportedGroupByChild((object) token.Child.Kind));
        childTransformations = (TransformationNode) this.BindAggregateToken((AggregateToken) token.Child);
        this.aggregateExpressionsCache = ((AggregateTransformationNode) childTransformations).AggregateExpressions;
        endPathTokenSet.UnionWith(this.aggregateExpressionsCache.Select<AggregateExpressionBase, EndPathToken>((Func<AggregateExpressionBase, EndPathToken>) (statement => new EndPathToken(statement.Alias, (QueryToken) null))));
      }
      this.state.AggregatedPropertyNames = endPathTokenSet;
      return new GroupByTransformationNode((IList<GroupByPropertyNode>) groupByPropertyNodeList, childTransformations, (CollectionNode) null);
    }

    private static bool IsPropertyNode(SingleValueNode node) => node.Kind == QueryNodeKind.SingleValuePropertyAccess || node.Kind == QueryNodeKind.SingleComplexNode || node.Kind == QueryNodeKind.SingleNavigationNode;

    private static Stack<SingleValueNode> ReversePropertyPath(SingleValueNode node)
    {
      Stack<SingleValueNode> singleValueNodeStack = new Stack<SingleValueNode>();
      do
      {
        singleValueNodeStack.Push(node);
        if (node.Kind == QueryNodeKind.SingleValuePropertyAccess)
          node = ((SingleValuePropertyAccessNode) node).Source;
        else if (node.Kind == QueryNodeKind.SingleComplexNode)
          node = (SingleValueNode) ((SingleComplexNode) node).Source;
        else if (node.Kind == QueryNodeKind.SingleNavigationNode)
          node = (SingleValueNode) ((SingleNavigationNode) node).Source;
      }
      while (node != null && ApplyBinder.IsPropertyNode(node));
      return singleValueNodeStack;
    }

    private static void RegisterProperty(
      IList<GroupByPropertyNode> properties,
      Stack<SingleValueNode> propertyStack)
    {
      SingleValueNode singleValueNode = propertyStack.Pop();
      string propertyName = ApplyBinder.GetNodePropertyName(singleValueNode);
      if (propertyStack.Count != 0)
      {
        GroupByPropertyNode groupByPropertyNode = properties.FirstOrDefault<GroupByPropertyNode>((Func<GroupByPropertyNode, bool>) (p => p.Name == propertyName));
        if (groupByPropertyNode == null)
        {
          groupByPropertyNode = new GroupByPropertyNode(propertyName, (SingleValueNode) null);
          properties.Add(groupByPropertyNode);
        }
        ApplyBinder.RegisterProperty(groupByPropertyNode.ChildTransformations, propertyStack);
      }
      else
        properties.Add(new GroupByPropertyNode(propertyName, singleValueNode, singleValueNode.TypeReference));
    }

    private static string GetNodePropertyName(SingleValueNode property)
    {
      if (property.Kind == QueryNodeKind.SingleValuePropertyAccess)
        return ((SingleValuePropertyAccessNode) property).Property.Name;
      if (property.Kind == QueryNodeKind.SingleComplexNode)
        return ((SingleComplexNode) property).Property.Name;
      if (property.Kind == QueryNodeKind.SingleNavigationNode)
        return ((SingleNavigationNode) property).NavigationProperty.Name;
      throw new NotSupportedException();
    }

    private ComputeTransformationNode BindComputeToken(ComputeToken token)
    {
      List<ComputeExpression> expressions = new List<ComputeExpression>();
      foreach (ComputeExpressionToken expression1 in token.Expressions)
      {
        SingleValueNode expression2 = (SingleValueNode) this.bindMethod(expression1.Expression);
        expressions.Add(new ComputeExpression((QueryNode) expression2, expression1.Alias, expression2.TypeReference));
      }
      return new ComputeTransformationNode((IEnumerable<ComputeExpression>) expressions);
    }
  }
}
