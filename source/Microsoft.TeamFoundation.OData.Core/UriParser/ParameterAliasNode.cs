// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ParameterAliasNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public class ParameterAliasNode : SingleValueNode
  {
    private readonly IEdmTypeReference typeReference;

    public ParameterAliasNode(string alias, IEdmTypeReference typeReference)
    {
      this.Alias = alias;
      this.typeReference = typeReference;
    }

    public string Alias { get; private set; }

    public override IEdmTypeReference TypeReference => this.typeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.ParameterAlias;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
