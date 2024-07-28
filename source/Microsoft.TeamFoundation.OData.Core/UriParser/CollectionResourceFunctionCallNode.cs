// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.CollectionResourceFunctionCallNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class CollectionResourceFunctionCallNode : CollectionResourceNode, IFunctionCallNode
  {
    private readonly string name;
    private readonly ReadOnlyCollection<IEdmFunction> functions;
    private readonly ReadOnlyCollection<QueryNode> parameters;
    private readonly IEdmStructuredTypeReference structuredTypeReference;
    private readonly IEdmCollectionTypeReference returnedCollectionTypeReference;
    private readonly IEdmEntitySetBase navigationSource;
    private readonly QueryNode source;

    public CollectionResourceFunctionCallNode(
      string name,
      IEnumerable<IEdmFunction> functions,
      IEnumerable<QueryNode> parameters,
      IEdmCollectionTypeReference returnedCollectionTypeReference,
      IEdmEntitySetBase navigationSource,
      QueryNode source)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(name, nameof (name));
      ExceptionUtils.CheckArgumentNotNull<IEdmCollectionTypeReference>(returnedCollectionTypeReference, nameof (returnedCollectionTypeReference));
      this.name = name;
      this.functions = new ReadOnlyCollection<IEdmFunction>(functions == null ? (IList<IEdmFunction>) new List<IEdmFunction>() : (IList<IEdmFunction>) functions.ToList<IEdmFunction>());
      this.parameters = new ReadOnlyCollection<QueryNode>(parameters == null ? (IList<QueryNode>) new List<QueryNode>() : (IList<QueryNode>) parameters.ToList<QueryNode>());
      this.returnedCollectionTypeReference = returnedCollectionTypeReference;
      this.navigationSource = navigationSource;
      this.structuredTypeReference = returnedCollectionTypeReference.ElementType().AsStructuredOrNull();
      if (this.structuredTypeReference == null)
        throw new ArgumentException(Microsoft.OData.Strings.Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity);
      this.source = source;
    }

    public string Name => this.name;

    public IEnumerable<IEdmFunction> Functions => (IEnumerable<IEdmFunction>) this.functions;

    public IEnumerable<QueryNode> Parameters => (IEnumerable<QueryNode>) this.parameters;

    public override IEdmTypeReference ItemType => (IEdmTypeReference) this.structuredTypeReference;

    public override IEdmCollectionTypeReference CollectionType => this.returnedCollectionTypeReference;

    public override IEdmStructuredTypeReference ItemStructuredType => this.structuredTypeReference;

    public override IEdmNavigationSource NavigationSource => (IEdmNavigationSource) this.navigationSource;

    public QueryNode Source => this.source;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.CollectionResourceFunctionCall;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
