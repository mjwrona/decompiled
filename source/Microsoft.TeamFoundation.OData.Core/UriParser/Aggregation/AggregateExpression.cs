// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.AggregateExpression
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class AggregateExpression : AggregateExpressionBase
  {
    private readonly AggregationMethod method;
    private readonly AggregationMethodDefinition methodDefinition;
    private readonly SingleValueNode expression;
    private readonly IEdmTypeReference typeReference;

    public AggregateExpression(
      SingleValueNode expression,
      AggregationMethod method,
      string alias,
      IEdmTypeReference typeReference)
      : base(AggregateExpressionKind.PropertyAggregate, alias)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(expression, nameof (expression));
      ExceptionUtils.CheckArgumentNotNull<string>(alias, nameof (alias));
      this.expression = expression;
      this.method = method;
      this.typeReference = typeReference;
    }

    public AggregateExpression(
      SingleValueNode expression,
      AggregationMethodDefinition methodDefinition,
      string alias,
      IEdmTypeReference typeReference)
      : this(expression, methodDefinition.MethodKind, alias, typeReference)
    {
      this.methodDefinition = methodDefinition;
    }

    public SingleValueNode Expression => this.expression;

    public AggregationMethod Method => this.method;

    public AggregationMethodDefinition MethodDefinition => this.methodDefinition;

    public IEdmTypeReference TypeReference => this.typeReference;
  }
}
