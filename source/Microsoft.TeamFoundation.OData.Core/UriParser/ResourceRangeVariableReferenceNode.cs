// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ResourceRangeVariableReferenceNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class ResourceRangeVariableReferenceNode : SingleResourceNode
  {
    private readonly string name;
    private readonly IEdmStructuredTypeReference structuredTypeReference;
    private readonly ResourceRangeVariable rangeVariable;
    private readonly IEdmNavigationSource navigationSource;

    public ResourceRangeVariableReferenceNode(string name, ResourceRangeVariable rangeVariable)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(name, nameof (name));
      ExceptionUtils.CheckArgumentNotNull<ResourceRangeVariable>(rangeVariable, nameof (rangeVariable));
      this.name = name;
      this.navigationSource = rangeVariable.NavigationSource;
      this.structuredTypeReference = rangeVariable.StructuredTypeReference;
      this.rangeVariable = rangeVariable;
    }

    public string Name => this.name;

    public override IEdmTypeReference TypeReference => (IEdmTypeReference) this.structuredTypeReference;

    public ResourceRangeVariable RangeVariable => this.rangeVariable;

    public override IEdmNavigationSource NavigationSource => this.navigationSource;

    public override IEdmStructuredTypeReference StructuredTypeReference => this.structuredTypeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.ResourceRangeVariableReference;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
