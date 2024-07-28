// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.CollectionOpenPropertyAccessNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class CollectionOpenPropertyAccessNode : CollectionNode
  {
    private readonly SingleValueNode source;
    private readonly string name;

    public CollectionOpenPropertyAccessNode(SingleValueNode source, string openPropertyName)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(source, nameof (source));
      ExceptionUtils.CheckArgumentNotNull<string>(openPropertyName, nameof (openPropertyName));
      this.source = source;
      this.name = openPropertyName;
    }

    public SingleValueNode Source => this.source;

    public string Name => this.name;

    public override IEdmTypeReference ItemType => (IEdmTypeReference) null;

    public override IEdmCollectionTypeReference CollectionType => (IEdmCollectionTypeReference) null;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.CollectionOpenPropertyAccess;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
