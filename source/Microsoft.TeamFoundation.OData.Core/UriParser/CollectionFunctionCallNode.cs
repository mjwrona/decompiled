// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.CollectionFunctionCallNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class CollectionFunctionCallNode : CollectionNode, IFunctionCallNode
  {
    private readonly string name;
    private readonly ReadOnlyCollection<IEdmFunction> functions;
    private readonly ReadOnlyCollection<QueryNode> parameters;
    private readonly IEdmTypeReference itemType;
    private readonly IEdmCollectionTypeReference returnedCollectionType;
    private readonly QueryNode source;

    public CollectionFunctionCallNode(
      string name,
      IEnumerable<IEdmFunction> functions,
      IEnumerable<QueryNode> parameters,
      IEdmCollectionTypeReference returnedCollectionType,
      QueryNode source)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(name, nameof (name));
      ExceptionUtils.CheckArgumentNotNull<IEdmCollectionTypeReference>(returnedCollectionType, nameof (returnedCollectionType));
      this.name = name;
      this.functions = new ReadOnlyCollection<IEdmFunction>(functions == null ? (IList<IEdmFunction>) new List<IEdmFunction>() : (IList<IEdmFunction>) functions.ToList<IEdmFunction>());
      this.parameters = new ReadOnlyCollection<QueryNode>(parameters == null ? (IList<QueryNode>) new List<QueryNode>() : (IList<QueryNode>) parameters.ToList<QueryNode>());
      this.returnedCollectionType = returnedCollectionType;
      this.itemType = returnedCollectionType.ElementType();
      if (!this.itemType.IsPrimitive() && !this.itemType.IsComplex() && !this.itemType.IsEnum())
        throw new ArgumentException(Microsoft.OData.Strings.Nodes_CollectionFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
      this.source = source;
    }

    public string Name => this.name;

    public IEnumerable<IEdmFunction> Functions => (IEnumerable<IEdmFunction>) this.functions;

    public IEnumerable<QueryNode> Parameters => (IEnumerable<QueryNode>) this.parameters;

    public override IEdmTypeReference ItemType => this.itemType;

    public override IEdmCollectionTypeReference CollectionType => this.returnedCollectionType;

    public QueryNode Source => this.source;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.CollectionFunctionCall;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
