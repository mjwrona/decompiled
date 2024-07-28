// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.CollectionNavigationNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  public sealed class CollectionNavigationNode : CollectionResourceNode
  {
    private readonly IEdmNavigationProperty navigationProperty;
    private readonly IEdmEntityTypeReference edmEntityTypeReference;
    private readonly IEdmCollectionTypeReference collectionTypeReference;
    private readonly SingleResourceNode source;
    private readonly IEdmNavigationSource navigationSource;
    private readonly List<ODataPathSegment> parsedSegments;
    private readonly IEdmPathExpression bindingPath;

    public CollectionNavigationNode(
      SingleResourceNode source,
      IEdmNavigationProperty navigationProperty,
      IEdmPathExpression bindingPath)
      : this(ExceptionUtils.CheckArgumentNotNull<SingleResourceNode>(source, nameof (source)).NavigationSource, navigationProperty, bindingPath)
    {
      this.source = source;
    }

    internal CollectionNavigationNode(
      IEdmNavigationSource navigationSource,
      IEdmNavigationProperty navigationProperty,
      IEdmPathExpression bindingPath)
      : this(navigationProperty)
    {
      this.bindingPath = bindingPath;
      this.navigationSource = navigationSource?.FindNavigationTarget(navigationProperty, bindingPath);
    }

    internal CollectionNavigationNode(
      SingleResourceNode source,
      IEdmNavigationProperty navigationProperty,
      List<ODataPathSegment> parsedSegments)
      : this(ExceptionUtils.CheckArgumentNotNull<SingleResourceNode>(source, nameof (source)).NavigationSource, navigationProperty, parsedSegments)
    {
      this.source = source;
    }

    private CollectionNavigationNode(
      IEdmNavigationSource navigationSource,
      IEdmNavigationProperty navigationProperty,
      List<ODataPathSegment> parsedSegments)
      : this(navigationProperty)
    {
      this.parsedSegments = parsedSegments;
      this.navigationSource = navigationSource != null ? navigationSource.FindNavigationTarget(navigationProperty, new Func<IEdmPathExpression, List<ODataPathSegment>, bool>(BindingPathHelper.MatchBindingPath), this.parsedSegments, out this.bindingPath) : (IEdmNavigationSource) null;
    }

    private CollectionNavigationNode(IEdmNavigationProperty navigationProperty)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmNavigationProperty>(navigationProperty, nameof (navigationProperty));
      this.navigationProperty = navigationProperty.TargetMultiplicity() == EdmMultiplicity.Many ? navigationProperty : throw new ArgumentException(Microsoft.OData.Strings.Nodes_CollectionNavigationNode_MustHaveManyMultiplicity);
      this.collectionTypeReference = navigationProperty.Type.AsCollection();
      this.edmEntityTypeReference = this.collectionTypeReference.ElementType().AsEntityOrNull();
    }

    public SingleResourceNode Source => this.source;

    public EdmMultiplicity TargetMultiplicity => this.navigationProperty.TargetMultiplicity();

    public IEdmNavigationProperty NavigationProperty => this.navigationProperty;

    public override IEdmTypeReference ItemType => (IEdmTypeReference) this.edmEntityTypeReference;

    public override IEdmCollectionTypeReference CollectionType => this.collectionTypeReference;

    public IEdmEntityTypeReference EntityItemType => this.edmEntityTypeReference;

    public override IEdmStructuredTypeReference ItemStructuredType => (IEdmStructuredTypeReference) this.edmEntityTypeReference;

    public override IEdmNavigationSource NavigationSource => this.navigationSource;

    public IEdmPathExpression BindingPath => this.bindingPath;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.CollectionNavigationNode;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
