// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.StarToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public sealed class StarToken : PathToken
  {
    private QueryToken nextToken;

    public StarToken(QueryToken nextToken) => this.nextToken = nextToken;

    public override QueryTokenKind Kind => QueryTokenKind.Star;

    public override QueryToken NextToken
    {
      get => this.nextToken;
      set => this.nextToken = value;
    }

    public override string Identifier => "*";

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
