// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmCollectionExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmCollectionExpression : 
    EdmElement,
    IEdmCollectionExpression,
    IEdmExpression,
    IEdmElement
  {
    private readonly IEdmTypeReference declaredType;
    private readonly IEnumerable<IEdmExpression> elements;

    public EdmCollectionExpression(params IEdmExpression[] elements)
      : this((IEnumerable<IEdmExpression>) elements)
    {
    }

    public EdmCollectionExpression(IEdmTypeReference declaredType, params IEdmExpression[] elements)
      : this(declaredType, (IEnumerable<IEdmExpression>) elements)
    {
    }

    public EdmCollectionExpression(IEnumerable<IEdmExpression> elements)
      : this((IEdmTypeReference) null, elements)
    {
    }

    public EdmCollectionExpression(
      IEdmTypeReference declaredType,
      IEnumerable<IEdmExpression> elements)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmExpression>>(elements, nameof (elements));
      this.declaredType = declaredType;
      this.elements = elements;
    }

    public IEdmTypeReference DeclaredType => this.declaredType;

    public IEnumerable<IEdmExpression> Elements => this.elements;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.Collection;
  }
}
