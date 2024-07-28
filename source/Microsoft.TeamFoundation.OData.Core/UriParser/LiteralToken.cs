// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.LiteralToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class LiteralToken : QueryToken
  {
    private readonly string originalText;
    private readonly object value;
    private readonly IEdmTypeReference expectedEdmTypeReference;

    public LiteralToken(object value) => this.value = value;

    internal LiteralToken(object value, string originalText)
      : this(value)
    {
      this.originalText = originalText;
    }

    internal LiteralToken(
      object value,
      string originalText,
      IEdmTypeReference expectedEdmTypeReference)
      : this(value, originalText)
    {
      this.expectedEdmTypeReference = expectedEdmTypeReference;
    }

    public override QueryTokenKind Kind => QueryTokenKind.Literal;

    public object Value => this.value;

    internal string OriginalText => this.originalText;

    internal IEdmTypeReference ExpectedEdmTypeReference => this.expectedEdmTypeReference;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
