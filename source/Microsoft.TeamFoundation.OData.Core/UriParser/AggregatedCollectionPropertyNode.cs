// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.AggregatedCollectionPropertyNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  public sealed class AggregatedCollectionPropertyNode : SingleResourceNode
  {
    private readonly CollectionNavigationNode source;
    private readonly IEdmProperty property;
    private readonly IEdmComplexTypeReference typeReference;
    private readonly IEdmNavigationSource navigationSource;

    public AggregatedCollectionPropertyNode(CollectionNavigationNode source, IEdmProperty property)
    {
      ExceptionUtils.CheckArgumentNotNull<CollectionNavigationNode>(source, nameof (source));
      ExceptionUtils.CheckArgumentNotNull<IEdmProperty>(property, nameof (property));
      if (property.PropertyKind != EdmPropertyKind.Structural)
        throw new ArgumentException(Microsoft.OData.Strings.Nodes_PropertyAccessShouldBeNonEntityProperty((object) property.Name));
      if (property.Type.IsCollection())
        throw new ArgumentException(Microsoft.OData.Strings.Nodes_PropertyAccessTypeShouldNotBeCollection((object) property.Name));
      this.source = source;
      this.property = property;
      this.typeReference = property.Type.AsComplex();
      this.navigationSource = source.NavigationSource;
    }

    public CollectionNavigationNode Source => this.source;

    public IEdmProperty Property => this.property;

    public override IEdmTypeReference TypeReference => this.Property.Type;

    public override IEdmNavigationSource NavigationSource => this.navigationSource;

    public override IEdmStructuredTypeReference StructuredTypeReference => (IEdmStructuredTypeReference) this.typeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.AggregatedCollectionPropertyNode;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
