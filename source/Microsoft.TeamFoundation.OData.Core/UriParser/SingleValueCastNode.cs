// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SingleValueCastNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class SingleValueCastNode : SingleValueNode
  {
    private readonly SingleValueNode source;
    private readonly IEdmPrimitiveTypeReference primitiveTypeReference;

    public SingleValueCastNode(SingleValueNode source, IEdmPrimitiveType primitiveType)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(source, nameof (source));
      ExceptionUtils.CheckArgumentNotNull<IEdmPrimitiveType>(primitiveType, nameof (primitiveType));
      this.source = source;
      this.primitiveTypeReference = (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference(primitiveType, true);
    }

    public SingleValueNode Source => this.source;

    public override IEdmTypeReference TypeReference => (IEdmTypeReference) this.primitiveTypeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.SingleValueCast;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
