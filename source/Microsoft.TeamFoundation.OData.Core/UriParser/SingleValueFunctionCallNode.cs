// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SingleValueFunctionCallNode
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
  public sealed class SingleValueFunctionCallNode : SingleValueNode, IFunctionCallNode
  {
    private readonly string name;
    private readonly ReadOnlyCollection<IEdmFunction> functions;
    private readonly IEnumerable<QueryNode> parameters;
    private readonly IEdmTypeReference returnedTypeReference;
    private readonly QueryNode source;

    public SingleValueFunctionCallNode(
      string name,
      IEnumerable<QueryNode> parameters,
      IEdmTypeReference returnedTypeReference)
      : this(name, (IEnumerable<IEdmFunction>) null, parameters, returnedTypeReference, (QueryNode) null)
    {
    }

    public SingleValueFunctionCallNode(
      string name,
      IEnumerable<IEdmFunction> functions,
      IEnumerable<QueryNode> parameters,
      IEdmTypeReference returnedTypeReference,
      QueryNode source)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(name, nameof (name));
      this.name = name;
      this.functions = new ReadOnlyCollection<IEdmFunction>(functions != null ? (IList<IEdmFunction>) functions.ToList<IEdmFunction>() : (IList<IEdmFunction>) new List<IEdmFunction>());
      this.parameters = parameters ?? Enumerable.Empty<QueryNode>();
      this.returnedTypeReference = returnedTypeReference == null || !returnedTypeReference.IsCollection() && (returnedTypeReference.IsComplex() || returnedTypeReference.IsPrimitive() || returnedTypeReference.IsEnum()) ? returnedTypeReference : throw new ArgumentException(Microsoft.OData.Strings.Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
      this.source = source;
    }

    public string Name => this.name;

    public IEnumerable<IEdmFunction> Functions => (IEnumerable<IEdmFunction>) this.functions;

    public IEnumerable<QueryNode> Parameters => this.parameters;

    public override IEdmTypeReference TypeReference => this.returnedTypeReference;

    public QueryNode Source => this.source;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.SingleValueFunctionCall;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
