// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ConstantNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.UriParser
{
  public sealed class ConstantNode : SingleValueNode
  {
    private readonly object constantValue;
    private readonly IEdmTypeReference typeReference;

    public ConstantNode(object constantValue, string literalText)
      : this(constantValue)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalText, nameof (literalText));
      this.LiteralText = literalText;
    }

    public ConstantNode(object constantValue, string literalText, IEdmTypeReference typeReference)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalText, nameof (literalText));
      this.constantValue = constantValue;
      this.LiteralText = literalText;
      this.typeReference = typeReference;
    }

    public ConstantNode(object constantValue)
    {
      this.constantValue = constantValue;
      this.typeReference = constantValue == null ? (IEdmTypeReference) null : (IEdmTypeReference) EdmLibraryExtensions.GetPrimitiveTypeReference(constantValue.GetType());
    }

    public object Value => this.constantValue;

    public string LiteralText { get; private set; }

    public override IEdmTypeReference TypeReference => this.typeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.Constant;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
