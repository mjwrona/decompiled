// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class SelectToken : QueryToken
  {
    private readonly IEnumerable<SelectTermToken> selectTerms;

    public SelectToken(IEnumerable<PathSegmentToken> properties)
      : this(properties == null ? (IEnumerable<SelectTermToken>) null : properties.Select<PathSegmentToken, SelectTermToken>((Func<PathSegmentToken, SelectTermToken>) (e => new SelectTermToken(e))))
    {
    }

    public SelectToken(IEnumerable<SelectTermToken> selectTerms) => this.selectTerms = selectTerms != null ? (IEnumerable<SelectTermToken>) new ReadOnlyEnumerableForUriParser<SelectTermToken>(selectTerms) : (IEnumerable<SelectTermToken>) new ReadOnlyEnumerableForUriParser<SelectTermToken>((IEnumerable<SelectTermToken>) new SelectTermToken[0]);

    public override QueryTokenKind Kind => QueryTokenKind.Select;

    public IEnumerable<PathSegmentToken> Properties => this.selectTerms.Select<SelectTermToken, PathSegmentToken>((Func<SelectTermToken, PathSegmentToken>) (e => e.PathToProperty));

    public IEnumerable<SelectTermToken> SelectTerms => this.selectTerms;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
