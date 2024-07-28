// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SingleValuePropertyAccessNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  public sealed class SingleValuePropertyAccessNode : SingleValueNode
  {
    private readonly SingleValueNode source;
    private readonly IEdmProperty property;

    public SingleValuePropertyAccessNode(SingleValueNode source, IEdmProperty property)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(source, nameof (source));
      ExceptionUtils.CheckArgumentNotNull<IEdmProperty>(property, nameof (property));
      if (property.PropertyKind != EdmPropertyKind.Structural)
        throw new ArgumentException(Microsoft.OData.Strings.Nodes_PropertyAccessShouldBeNonEntityProperty((object) property.Name));
      if (property.Type.IsCollection())
        throw new ArgumentException(Microsoft.OData.Strings.Nodes_PropertyAccessTypeShouldNotBeCollection((object) property.Name));
      this.source = source;
      this.property = property;
    }

    public SingleValueNode Source => this.source;

    public IEdmProperty Property => this.property;

    public override IEdmTypeReference TypeReference => this.Property.Type;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.SingleValuePropertyAccess;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
