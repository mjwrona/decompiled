// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.EntitySetAggregateExpression
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class EntitySetAggregateExpression : AggregateExpressionBase
  {
    private readonly CollectionNavigationNode expression;
    private readonly IEnumerable<AggregateExpressionBase> children;

    public EntitySetAggregateExpression(
      CollectionNavigationNode expression,
      IEnumerable<AggregateExpressionBase> children)
      : base(AggregateExpressionKind.EntitySetAggregate, expression.NavigationProperty.Name)
    {
      this.expression = expression;
      this.children = children;
    }

    public CollectionNavigationNode Expression => this.expression;

    public IEnumerable<AggregateExpressionBase> Children => this.children;
  }
}
