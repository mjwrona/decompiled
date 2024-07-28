// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.EntitySetAggregateToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class EntitySetAggregateToken : AggregateTokenBase
  {
    private readonly QueryToken entitySet;
    private readonly IEnumerable<AggregateTokenBase> expressions;

    public EntitySetAggregateToken(
      QueryToken entitySet,
      IEnumerable<AggregateTokenBase> expressions)
    {
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<AggregateTokenBase>>(expressions, nameof (expressions));
      this.expressions = expressions;
      this.entitySet = entitySet;
    }

    public override QueryTokenKind Kind => QueryTokenKind.EntitySetAggregateExpression;

    public IEnumerable<AggregateTokenBase> Expressions => this.expressions;

    public QueryToken EntitySet => this.entitySet;

    public static EntitySetAggregateToken Merge(
      EntitySetAggregateToken token1,
      EntitySetAggregateToken token2)
    {
      if (token1 == null)
        return token2;
      if (token2 == null)
        return token1;
      object.Equals((object) token1.entitySet, (object) token2.entitySet);
      return new EntitySetAggregateToken(token1.entitySet, token1.expressions.Concat<AggregateTokenBase>(token2.expressions));
    }

    public string Path()
    {
      List<string> stringList = new List<string>();
      for (PathToken pathToken = this.entitySet as PathToken; pathToken != null; pathToken = pathToken.NextToken as PathToken)
        stringList.Add(pathToken.Identifier);
      stringList.Reverse();
      return string.Join("/", stringList.ToArray());
    }

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
