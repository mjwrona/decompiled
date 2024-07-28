// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SyntacticTreeVisitor`1
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.UriParser.Aggregation;
using System;

namespace Microsoft.OData.UriParser
{
  internal abstract class SyntacticTreeVisitor<T> : ISyntacticTreeVisitor<T>
  {
    public virtual T Visit(AllToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(AnyToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(BinaryOperatorToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(InToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(DottedIdentifierToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(ExpandToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(ExpandTermToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(FunctionCallToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(LiteralToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(AggregateToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(GroupByToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(AggregateExpressionToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(EntitySetAggregateToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(LambdaToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(InnerPathToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(OrderByToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(EndPathToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(CustomQueryOptionToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(RangeVariableToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(SelectToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(SelectTermToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(StarToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(UnaryOperatorToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(FunctionParameterToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(ComputeToken tokenIn) => throw new NotImplementedException();

    public virtual T Visit(ComputeExpressionToken tokenIn) => throw new NotImplementedException();
  }
}
