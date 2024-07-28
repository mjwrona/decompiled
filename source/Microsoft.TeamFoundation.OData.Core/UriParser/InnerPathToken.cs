// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.InnerPathToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  public sealed class InnerPathToken : PathToken
  {
    private readonly string identifier;
    private readonly IEnumerable<NamedValue> namedValues;
    private QueryToken nextToken;

    public InnerPathToken(
      string identifier,
      QueryToken nextToken,
      IEnumerable<NamedValue> namedValues)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(identifier, nameof (identifier));
      this.identifier = identifier;
      this.nextToken = nextToken;
      this.namedValues = namedValues == null ? (IEnumerable<NamedValue>) null : (IEnumerable<NamedValue>) new ReadOnlyEnumerableForUriParser<NamedValue>(namedValues);
    }

    public override QueryTokenKind Kind => QueryTokenKind.InnerPath;

    public override string Identifier => this.identifier;

    public override QueryToken NextToken
    {
      get => this.nextToken;
      set => this.nextToken = value;
    }

    public IEnumerable<NamedValue> NamedValues => this.namedValues;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
