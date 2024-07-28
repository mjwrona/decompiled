// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExpandToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  public sealed class ExpandToken : QueryToken
  {
    private readonly IEnumerable<ExpandTermToken> expandTerms;

    public ExpandToken(params ExpandTermToken[] expandTerms)
      : this((IEnumerable<ExpandTermToken>) expandTerms)
    {
    }

    public ExpandToken(IEnumerable<ExpandTermToken> expandTerms) => this.expandTerms = (IEnumerable<ExpandTermToken>) new ReadOnlyEnumerableForUriParser<ExpandTermToken>((IEnumerable<ExpandTermToken>) ((object) expandTerms ?? (object) new ExpandTermToken[0]));

    public override QueryTokenKind Kind => QueryTokenKind.Expand;

    public IEnumerable<ExpandTermToken> ExpandTerms => this.expandTerms;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
