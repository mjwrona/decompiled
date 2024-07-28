// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.CollectionComplexNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  public class CollectionComplexNode : CollectionResourceNode
  {
    private readonly SingleResourceNode source;
    private readonly IEdmProperty property;
    private readonly IEdmComplexTypeReference itemType;
    private readonly IEdmCollectionTypeReference collectionTypeReference;
    private readonly IEdmNavigationSource navigationSource;

    public CollectionComplexNode(SingleResourceNode source, IEdmProperty property)
      : this(ExceptionUtils.CheckArgumentNotNull<SingleResourceNode>(source, nameof (source)).NavigationSource, property)
    {
      this.source = source;
    }

    private CollectionComplexNode(IEdmNavigationSource navigationSource, IEdmProperty property)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmProperty>(property, nameof (property));
      this.property = property.PropertyKind == EdmPropertyKind.Structural ? property : throw new ArgumentException(Microsoft.OData.Strings.Nodes_PropertyAccessShouldBeNonEntityProperty((object) property.Name));
      this.collectionTypeReference = property.Type.AsCollection();
      this.itemType = this.collectionTypeReference.ElementType().AsComplex();
      this.navigationSource = navigationSource;
    }

    public SingleResourceNode Source => this.source;

    public IEdmProperty Property => this.property;

    public override IEdmTypeReference ItemType => (IEdmTypeReference) this.itemType;

    public override IEdmCollectionTypeReference CollectionType => this.collectionTypeReference;

    public override IEdmStructuredTypeReference ItemStructuredType => (IEdmStructuredTypeReference) this.itemType;

    public override IEdmNavigationSource NavigationSource => this.navigationSource;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.CollectionComplexNode;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
