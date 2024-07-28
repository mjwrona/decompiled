// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ComputeBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class ComputeBinder
  {
    private MetadataBinder.QueryTokenVisitor bindMethod;

    public ComputeBinder(MetadataBinder.QueryTokenVisitor bindMethod) => this.bindMethod = bindMethod;

    public ComputeClause BindCompute(ComputeToken token)
    {
      ExceptionUtils.CheckArgumentNotNull<ComputeToken>(token, nameof (token));
      List<ComputeExpression> computedItems = new List<ComputeExpression>();
      foreach (ComputeExpressionToken expression in token.Expressions)
      {
        ComputeExpression expressionToken = this.BindComputeExpressionToken(expression);
        computedItems.Add(expressionToken);
      }
      return new ComputeClause((IEnumerable<ComputeExpression>) computedItems);
    }

    private ComputeExpression BindComputeExpressionToken(ComputeExpressionToken token)
    {
      QueryNode expression = this.bindMethod(token.Expression);
      return new ComputeExpression(expression, token.Alias, expression is SingleValueNode singleValueNode ? singleValueNode.TypeReference : (IEdmTypeReference) null);
    }
  }
}
