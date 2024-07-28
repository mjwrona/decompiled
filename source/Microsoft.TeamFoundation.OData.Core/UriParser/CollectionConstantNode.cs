// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.CollectionConstantNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.OData.UriParser
{
  public sealed class CollectionConstantNode : CollectionNode
  {
    private readonly IList<ConstantNode> collection = (IList<ConstantNode>) new List<ConstantNode>();
    private readonly IEdmTypeReference itemType;
    private readonly IEdmCollectionTypeReference collectionTypeReference;

    public CollectionConstantNode(
      IEnumerable<object> objectCollection,
      string literalText,
      IEdmCollectionTypeReference collectionType)
    {
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<object>>(objectCollection, nameof (objectCollection));
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalText, nameof (literalText));
      ExceptionUtils.CheckArgumentNotNull<IEdmCollectionTypeReference>(collectionType, nameof (collectionType));
      this.LiteralText = literalText;
      this.itemType = (collectionType.Definition as EdmCollectionType).ElementType;
      this.collectionTypeReference = collectionType;
      foreach (object constantValue in objectCollection)
        this.collection.Add(new ConstantNode(constantValue, constantValue != null ? constantValue.ToString() : "null", this.itemType));
    }

    public IList<ConstantNode> Collection => (IList<ConstantNode>) new ReadOnlyCollection<ConstantNode>(this.collection);

    public string LiteralText { get; private set; }

    public override IEdmTypeReference ItemType => this.itemType;

    public override IEdmCollectionTypeReference CollectionType => this.collectionTypeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.CollectionConstant;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
