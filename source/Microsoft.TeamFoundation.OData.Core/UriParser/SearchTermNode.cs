// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SearchTermNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.UriParser
{
  public sealed class SearchTermNode : SingleValueNode
  {
    private static readonly IEdmTypeReference BoolTypeReference = (IEdmTypeReference) EdmLibraryExtensions.GetPrimitiveTypeReference(typeof (bool));
    private readonly string text;

    public SearchTermNode(string text)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(text, "literalText");
      this.text = text;
    }

    public string Text => this.text;

    public override IEdmTypeReference TypeReference => SearchTermNode.BoolTypeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.SearchTerm;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
