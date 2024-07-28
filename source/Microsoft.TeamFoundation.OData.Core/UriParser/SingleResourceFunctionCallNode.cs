// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SingleResourceFunctionCallNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class SingleResourceFunctionCallNode : SingleResourceNode, IFunctionCallNode
  {
    private readonly string name;
    private readonly ReadOnlyCollection<IEdmFunction> functions;
    private readonly IEnumerable<QueryNode> parameters;
    private readonly IEdmStructuredTypeReference returnedStructuredTypeReference;
    private readonly IEdmNavigationSource navigationSource;
    private readonly QueryNode source;

    public SingleResourceFunctionCallNode(
      string name,
      IEnumerable<QueryNode> parameters,
      IEdmStructuredTypeReference returnedStructuredTypeReference,
      IEdmNavigationSource navigationSource)
      : this(name, (IEnumerable<IEdmFunction>) null, parameters, returnedStructuredTypeReference, navigationSource, (QueryNode) null)
    {
    }

    public SingleResourceFunctionCallNode(
      string name,
      IEnumerable<IEdmFunction> functions,
      IEnumerable<QueryNode> parameters,
      IEdmStructuredTypeReference returnedStructuredTypeReference,
      IEdmNavigationSource navigationSource,
      QueryNode source)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(name, nameof (name));
      ExceptionUtils.CheckArgumentNotNull<IEdmStructuredTypeReference>(returnedStructuredTypeReference, nameof (returnedStructuredTypeReference));
      this.name = name;
      this.functions = new ReadOnlyCollection<IEdmFunction>(functions != null ? (IList<IEdmFunction>) functions.ToList<IEdmFunction>() : (IList<IEdmFunction>) new List<IEdmFunction>());
      this.parameters = (IEnumerable<QueryNode>) new ReadOnlyCollection<QueryNode>(parameters == null ? (IList<QueryNode>) new List<QueryNode>() : (IList<QueryNode>) parameters.ToList<QueryNode>());
      this.returnedStructuredTypeReference = returnedStructuredTypeReference;
      this.navigationSource = navigationSource;
      this.source = source;
    }

    public string Name => this.name;

    public IEnumerable<IEdmFunction> Functions => (IEnumerable<IEdmFunction>) this.functions;

    public IEnumerable<QueryNode> Parameters => this.parameters;

    public override IEdmTypeReference TypeReference => (IEdmTypeReference) this.returnedStructuredTypeReference;

    public override IEdmNavigationSource NavigationSource => this.navigationSource;

    public override IEdmStructuredTypeReference StructuredTypeReference => this.returnedStructuredTypeReference;

    public QueryNode Source => this.source;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.SingleResourceFunctionCall;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
