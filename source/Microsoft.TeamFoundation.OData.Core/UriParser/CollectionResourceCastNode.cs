// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.CollectionResourceCastNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class CollectionResourceCastNode : CollectionResourceNode
  {
    private readonly CollectionResourceNode source;
    private readonly IEdmStructuredTypeReference edmTypeReference;
    private readonly IEdmCollectionTypeReference collectionTypeReference;
    private readonly IEdmNavigationSource navigationSource;

    public CollectionResourceCastNode(
      CollectionResourceNode source,
      IEdmStructuredType structuredType)
    {
      ExceptionUtils.CheckArgumentNotNull<CollectionResourceNode>(source, nameof (source));
      ExceptionUtils.CheckArgumentNotNull<IEdmStructuredType>(structuredType, nameof (structuredType));
      this.source = source;
      this.edmTypeReference = structuredType.GetTypeReference();
      this.navigationSource = source.NavigationSource;
      this.collectionTypeReference = EdmCoreModel.GetCollection((IEdmTypeReference) this.edmTypeReference);
    }

    public CollectionResourceNode Source => this.source;

    public override IEdmTypeReference ItemType => (IEdmTypeReference) this.edmTypeReference;

    public override IEdmCollectionTypeReference CollectionType => this.collectionTypeReference;

    public override IEdmStructuredTypeReference ItemStructuredType => this.edmTypeReference;

    public override IEdmNavigationSource NavigationSource => this.navigationSource;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.CollectionResourceCast;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
