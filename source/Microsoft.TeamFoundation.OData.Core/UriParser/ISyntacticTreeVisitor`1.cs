// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ISyntacticTreeVisitor`1
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.UriParser.Aggregation;

namespace Microsoft.OData.UriParser
{
  public interface ISyntacticTreeVisitor<T>
  {
    T Visit(AllToken tokenIn);

    T Visit(AnyToken tokenIn);

    T Visit(BinaryOperatorToken tokenIn);

    T Visit(InToken tokenIn);

    T Visit(DottedIdentifierToken tokenIn);

    T Visit(ExpandToken tokenIn);

    T Visit(ExpandTermToken tokenIn);

    T Visit(FunctionCallToken tokenIn);

    T Visit(LambdaToken tokenIn);

    T Visit(LiteralToken tokenIn);

    T Visit(InnerPathToken tokenIn);

    T Visit(OrderByToken tokenIn);

    T Visit(EndPathToken tokenIn);

    T Visit(CustomQueryOptionToken tokenIn);

    T Visit(RangeVariableToken tokenIn);

    T Visit(SelectToken tokenIn);

    T Visit(SelectTermToken tokenIn);

    T Visit(StarToken tokenIn);

    T Visit(UnaryOperatorToken tokenIn);

    T Visit(FunctionParameterToken tokenIn);

    T Visit(AggregateToken tokenIn);

    T Visit(AggregateExpressionToken tokenIn);

    T Visit(EntitySetAggregateToken tokenIn);

    T Visit(GroupByToken tokenIn);
  }
}
