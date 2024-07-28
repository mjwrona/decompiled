// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SingleComplexNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  public class SingleComplexNode : SingleResourceNode
  {
    private readonly SingleResourceNode source;
    private readonly IEdmProperty property;
    private readonly IEdmComplexTypeReference typeReference;
    private readonly IEdmNavigationSource navigationSource;

    public SingleComplexNode(SingleResourceNode source, IEdmProperty property)
      : this(ExceptionUtils.CheckArgumentNotNull<SingleResourceNode>(source, nameof (source)).NavigationSource, property)
    {
      this.source = source;
    }

    private SingleComplexNode(IEdmNavigationSource navigationSource, IEdmProperty property)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmProperty>(property, nameof (property));
      this.property = property.PropertyKind == EdmPropertyKind.Structural ? property : throw new ArgumentException(Microsoft.OData.Strings.Nodes_PropertyAccessShouldBeNonEntityProperty((object) property.Name));
      this.navigationSource = navigationSource;
      this.typeReference = property.Type.AsComplex();
    }

    public SingleResourceNode Source => this.source;

    public IEdmProperty Property => this.property;

    public override IEdmTypeReference TypeReference => (IEdmTypeReference) this.typeReference;

    public override IEdmNavigationSource NavigationSource => this.navigationSource;

    public override IEdmStructuredTypeReference StructuredTypeReference => (IEdmStructuredTypeReference) this.typeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.SingleComplexNode;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
