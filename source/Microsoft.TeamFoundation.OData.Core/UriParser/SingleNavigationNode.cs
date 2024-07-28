// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SingleNavigationNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  public sealed class SingleNavigationNode : SingleEntityNode
  {
    private readonly IEdmNavigationSource navigationSource;
    private readonly SingleResourceNode source;
    private readonly IEdmNavigationProperty navigationProperty;
    private readonly IEdmEntityTypeReference entityTypeReference;
    private readonly List<ODataPathSegment> parsedSegments;
    private readonly IEdmPathExpression bindingPath;

    public SingleNavigationNode(
      SingleResourceNode source,
      IEdmNavigationProperty navigationProperty,
      IEdmPathExpression bindingPath)
      : this(ExceptionUtils.CheckArgumentNotNull<SingleResourceNode>(source, nameof (source)).NavigationSource, navigationProperty, bindingPath)
    {
      this.source = source;
    }

    internal SingleNavigationNode(
      IEdmNavigationSource navigationSource,
      IEdmNavigationProperty navigationProperty,
      IEdmPathExpression bindingPath)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmNavigationProperty>(navigationProperty, nameof (navigationProperty));
      switch (navigationProperty.TargetMultiplicity())
      {
        case EdmMultiplicity.ZeroOrOne:
        case EdmMultiplicity.One:
          this.navigationProperty = navigationProperty;
          this.entityTypeReference = (IEdmEntityTypeReference) this.NavigationProperty.Type;
          this.bindingPath = bindingPath;
          this.navigationSource = navigationSource?.FindNavigationTarget(navigationProperty, bindingPath);
          break;
        default:
          throw new ArgumentException(Microsoft.OData.Strings.Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity);
      }
    }

    internal SingleNavigationNode(
      SingleResourceNode source,
      IEdmNavigationProperty navigationProperty,
      List<ODataPathSegment> segments)
      : this(ExceptionUtils.CheckArgumentNotNull<SingleResourceNode>(source, nameof (source)).NavigationSource, navigationProperty, segments)
    {
      this.source = source;
    }

    private SingleNavigationNode(
      IEdmNavigationSource navigationSource,
      IEdmNavigationProperty navigationProperty,
      List<ODataPathSegment> segments)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmNavigationProperty>(navigationProperty, nameof (navigationProperty));
      switch (navigationProperty.TargetMultiplicity())
      {
        case EdmMultiplicity.ZeroOrOne:
        case EdmMultiplicity.One:
          this.navigationProperty = navigationProperty;
          this.entityTypeReference = (IEdmEntityTypeReference) this.NavigationProperty.Type;
          this.parsedSegments = segments;
          this.navigationSource = navigationSource != null ? navigationSource.FindNavigationTarget(navigationProperty, new Func<IEdmPathExpression, List<ODataPathSegment>, bool>(BindingPathHelper.MatchBindingPath), this.parsedSegments, out this.bindingPath) : (IEdmNavigationSource) null;
          break;
        default:
          throw new ArgumentException(Microsoft.OData.Strings.Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity);
      }
    }

    public SingleResourceNode Source => this.source;

    public IEdmNavigationProperty NavigationProperty => this.navigationProperty;

    public EdmMultiplicity TargetMultiplicity => this.NavigationProperty.TargetMultiplicity();

    public override IEdmTypeReference TypeReference => (IEdmTypeReference) this.entityTypeReference;

    public override IEdmEntityTypeReference EntityTypeReference => this.entityTypeReference;

    public override IEdmNavigationSource NavigationSource => this.navigationSource;

    public override IEdmStructuredTypeReference StructuredTypeReference => (IEdmStructuredTypeReference) this.entityTypeReference;

    public IEdmPathExpression BindingPath => this.bindingPath;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.SingleNavigationNode;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
