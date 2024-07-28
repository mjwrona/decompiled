// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.NonSystemToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  public sealed class NonSystemToken : PathSegmentToken
  {
    private readonly IEnumerable<NamedValue> namedValues;
    private readonly string identifier;

    public NonSystemToken(
      string identifier,
      IEnumerable<NamedValue> namedValues,
      PathSegmentToken nextToken)
      : base(nextToken)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(identifier, nameof (identifier));
      this.identifier = identifier;
      this.namedValues = namedValues;
    }

    public IEnumerable<NamedValue> NamedValues => this.namedValues;

    public override string Identifier => this.identifier;

    public override bool IsNamespaceOrContainerQualified() => this.identifier.Contains(".");

    public override T Accept<T>(IPathSegmentTokenVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<IPathSegmentTokenVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }

    public override void Accept(IPathSegmentTokenVisitor visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<IPathSegmentTokenVisitor>(visitor, nameof (visitor));
      visitor.Visit(this);
    }
  }
}
