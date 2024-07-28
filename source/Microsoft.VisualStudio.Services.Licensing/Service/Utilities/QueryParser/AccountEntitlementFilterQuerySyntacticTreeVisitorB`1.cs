// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementFilterQuerySyntacticTreeVisitorBase`1
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementFilterQuerySyntacticTreeVisitorBase<T> : ISyntacticTreeVisitor<T>
  {
    public virtual T Visit(EndPathToken tokenIn) => default (T);

    public virtual T Visit(LiteralToken tokenIn) => default (T);

    public virtual T Visit(BinaryOperatorToken tokenIn) => default (T);

    public T Visit(AllToken tokenIn) => default (T);

    public T Visit(AnyToken tokenIn) => default (T);

    public T Visit(InToken tokenIn) => default (T);

    public T Visit(DottedIdentifierToken tokenIn) => default (T);

    public T Visit(ExpandToken tokenIn) => default (T);

    public T Visit(ExpandTermToken tokenIn) => default (T);

    public T Visit(FunctionCallToken tokenIn) => default (T);

    public T Visit(LambdaToken tokenIn) => default (T);

    public T Visit(InnerPathToken tokenIn) => default (T);

    public T Visit(OrderByToken tokenIn) => default (T);

    public T Visit(CustomQueryOptionToken tokenIn) => default (T);

    public T Visit(RangeVariableToken tokenIn) => default (T);

    public T Visit(SelectToken tokenIn) => default (T);

    public T Visit(StarToken tokenIn) => default (T);

    public T Visit(UnaryOperatorToken tokenIn) => default (T);

    public T Visit(FunctionParameterToken tokenIn) => default (T);

    public T Visit(AggregateToken tokenIn) => default (T);

    public T Visit(AggregateExpressionToken tokenIn) => default (T);

    public T Visit(EntitySetAggregateToken tokenIn) => default (T);

    public T Visit(GroupByToken tokenIn) => default (T);
  }
}
