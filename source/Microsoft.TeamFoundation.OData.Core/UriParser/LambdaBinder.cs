// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.LambdaBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  internal sealed class LambdaBinder
  {
    private readonly MetadataBinder.QueryTokenVisitor bindMethod;

    internal LambdaBinder(MetadataBinder.QueryTokenVisitor bindMethod)
    {
      ExceptionUtils.CheckArgumentNotNull<MetadataBinder.QueryTokenVisitor>(bindMethod, nameof (bindMethod));
      this.bindMethod = bindMethod;
    }

    internal LambdaNode BindLambdaToken(LambdaToken lambdaToken, BindingState state)
    {
      ExceptionUtils.CheckArgumentNotNull<LambdaToken>(lambdaToken, "LambdaToken");
      ExceptionUtils.CheckArgumentNotNull<BindingState>(state, nameof (state));
      CollectionNode collectionNode = this.BindParentToken(lambdaToken.Parent);
      RangeVariable newRangeVariable = (RangeVariable) null;
      if (lambdaToken.Parameter != null)
      {
        newRangeVariable = NodeFactory.CreateParameterNode(lambdaToken.Parameter, collectionNode);
        state.RangeVariables.Push(newRangeVariable);
      }
      SingleValueNode lambdaExpression = this.BindExpressionToken(lambdaToken.Expression);
      LambdaNode lambdaNode = NodeFactory.CreateLambdaNode(state, collectionNode, lambdaExpression, newRangeVariable, lambdaToken.Kind);
      if (newRangeVariable != null)
        state.RangeVariables.Pop();
      return lambdaNode;
    }

    private CollectionNode BindParentToken(QueryToken queryToken)
    {
      switch (this.bindMethod(queryToken))
      {
        case CollectionNode collectionNode:
          return collectionNode;
        case SingleValueOpenPropertyAccessNode propertyAccessNode:
          return (CollectionNode) new CollectionOpenPropertyAccessNode(propertyAccessNode.Source, propertyAccessNode.Name);
        default:
          throw new ODataException(Microsoft.OData.Strings.MetadataBinder_LambdaParentMustBeCollection);
      }
    }

    private SingleValueNode BindExpressionToken(QueryToken queryToken)
    {
      if (!(this.bindMethod(queryToken) is SingleValueNode segment))
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_AnyAllExpressionNotSingleValue);
      IEdmTypeReference edmTypeReference = segment.GetEdmTypeReference();
      if (edmTypeReference != null && !edmTypeReference.AsPrimitive().IsBoolean())
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_AnyAllExpressionNotSingleValue);
      return segment;
    }
  }
}
