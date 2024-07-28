// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.AggregateToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class AggregateToken : ApplyTransformationToken
  {
    private readonly IEnumerable<AggregateTokenBase> expressions;

    public AggregateToken(IEnumerable<AggregateTokenBase> expressions)
    {
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<AggregateTokenBase>>(expressions, nameof (expressions));
      this.expressions = expressions;
    }

    public override QueryTokenKind Kind => QueryTokenKind.Aggregate;

    [Obsolete("Use AggregateExpressions for all aggregation expressions or AggregateExpressions.OfType<AggregateExpressionToken>()  for aggregate(..) expressions only.")]
    public IEnumerable<AggregateExpressionToken> Expressions => this.expressions.OfType<AggregateExpressionToken>();

    public IEnumerable<AggregateTokenBase> AggregateExpressions => this.expressions;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
