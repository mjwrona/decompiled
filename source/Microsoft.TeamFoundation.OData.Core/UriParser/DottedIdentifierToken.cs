// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.DottedIdentifierToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public sealed class DottedIdentifierToken : PathToken
  {
    private readonly string identifier;
    private QueryToken nextToken;

    public DottedIdentifierToken(string identifier, QueryToken nextToken)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(identifier, nameof (Identifier));
      this.identifier = identifier;
      this.nextToken = nextToken;
    }

    public override QueryTokenKind Kind => QueryTokenKind.DottedIdentifier;

    public override string Identifier => this.identifier;

    public override QueryToken NextToken
    {
      get => this.nextToken;
      set => this.nextToken = value;
    }

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
