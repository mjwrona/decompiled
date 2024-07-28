// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.InNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;

namespace Microsoft.OData.UriParser
{
  public sealed class InNode : SingleValueNode
  {
    private readonly IEdmTypeReference boolTypeReference = (IEdmTypeReference) EdmLibraryExtensions.GetPrimitiveTypeReference(typeof (bool));
    private readonly SingleValueNode left;
    private readonly CollectionNode right;

    public InNode(SingleValueNode left, CollectionNode right)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(left, nameof (left));
      ExceptionUtils.CheckArgumentNotNull<CollectionNode>(right, nameof (right));
      this.left = left;
      this.right = right;
      if (!this.left.GetEdmTypeReference().IsAssignableFrom(this.right.ItemType) && !this.right.ItemType.IsAssignableFrom(this.left.GetEdmTypeReference()))
        throw new ArgumentException(Microsoft.OData.Strings.Nodes_InNode_CollectionItemTypeMustBeSameAsSingleItemType((object) this.right.ItemType.FullName(), (object) this.left.GetEdmTypeReference().FullName()));
    }

    public SingleValueNode Left => this.left;

    public CollectionNode Right => this.right;

    public override IEdmTypeReference TypeReference => this.boolTypeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.In;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
