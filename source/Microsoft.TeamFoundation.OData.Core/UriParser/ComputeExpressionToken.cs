// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ComputeExpressionToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData.UriParser
{
  public sealed class ComputeExpressionToken : QueryToken
  {
    private QueryToken expression;
    private string alias;

    public ComputeExpressionToken(QueryToken expression, string alias)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryToken>(expression, nameof (expression));
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(alias, nameof (alias));
      this.expression = expression;
      this.alias = alias;
    }

    public override QueryTokenKind Kind => QueryTokenKind.ComputeExpression;

    public QueryToken Expression => this.expression;

    public string Alias => this.alias;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor is SyntacticTreeVisitor<T> syntacticTreeVisitor ? syntacticTreeVisitor.Visit(this) : throw new NotImplementedException();
  }
}
