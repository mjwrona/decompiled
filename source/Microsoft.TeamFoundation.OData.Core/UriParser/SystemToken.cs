// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SystemToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public sealed class SystemToken : PathSegmentToken
  {
    private readonly string identifier;

    public SystemToken(string identifier, PathSegmentToken nextToken)
      : base(nextToken)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(identifier, nameof (identifier));
      this.identifier = identifier;
    }

    public override string Identifier => this.identifier;

    public override bool IsNamespaceOrContainerQualified() => false;

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
