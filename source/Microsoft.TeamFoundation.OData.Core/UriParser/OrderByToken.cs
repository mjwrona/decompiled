// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.OrderByToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData.UriParser
{
  public sealed class OrderByToken : QueryToken
  {
    private readonly OrderByDirection direction;
    private readonly QueryToken expression;

    public OrderByToken(QueryToken expression, OrderByDirection direction)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryToken>(expression, nameof (expression));
      this.expression = expression;
      this.direction = direction;
    }

    public override QueryTokenKind Kind => QueryTokenKind.OrderBy;

    public OrderByDirection Direction => this.direction;

    public QueryToken Expression => this.expression;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => throw new NotImplementedException();
  }
}
