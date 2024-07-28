// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.AggregateTransformationNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class AggregateTransformationNode : TransformationNode
  {
    private readonly IEnumerable<AggregateExpressionBase> expressions;

    public AggregateTransformationNode(IEnumerable<AggregateExpressionBase> expressions)
    {
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<AggregateExpressionBase>>(expressions, nameof (expressions));
      this.expressions = expressions;
    }

    [Obsolete("Use AggregateExpressions for all aggregation expressions or AggregateExpressions.OfType<AggregateExpressionToken>()  for aggregate(..) expressions only.")]
    public IEnumerable<AggregateExpression> Expressions => this.expressions.OfType<AggregateExpression>();

    public IEnumerable<AggregateExpressionBase> AggregateExpressions => this.expressions;

    public override TransformationNodeKind Kind => TransformationNodeKind.Aggregate;
  }
}
