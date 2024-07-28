// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.NonResourceRangeVariableReferenceNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class NonResourceRangeVariableReferenceNode : SingleValueNode
  {
    private readonly string name;
    private readonly IEdmTypeReference typeReference;
    private readonly NonResourceRangeVariable rangeVariable;

    public NonResourceRangeVariableReferenceNode(
      string name,
      NonResourceRangeVariable rangeVariable)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(name, nameof (name));
      ExceptionUtils.CheckArgumentNotNull<NonResourceRangeVariable>(rangeVariable, nameof (rangeVariable));
      this.name = name;
      this.typeReference = rangeVariable.TypeReference;
      this.rangeVariable = rangeVariable;
    }

    public string Name => this.name;

    public override IEdmTypeReference TypeReference => this.typeReference;

    public NonResourceRangeVariable RangeVariable => this.rangeVariable;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.NonResourceRangeVariableReference;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
