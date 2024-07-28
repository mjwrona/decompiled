// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SingleResourceCastNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class SingleResourceCastNode : SingleResourceNode
  {
    private readonly SingleResourceNode source;
    private readonly IEdmStructuredTypeReference structuredTypeReference;
    private readonly IEdmNavigationSource navigationSource;

    public SingleResourceCastNode(SingleResourceNode source, IEdmStructuredType structuredType)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmStructuredType>(structuredType, nameof (structuredType));
      this.source = source;
      this.navigationSource = source?.NavigationSource;
      this.structuredTypeReference = structuredType.GetTypeReference();
    }

    public SingleResourceNode Source => this.source;

    public override IEdmTypeReference TypeReference => (IEdmTypeReference) this.structuredTypeReference;

    public override IEdmNavigationSource NavigationSource => this.navigationSource;

    public override IEdmStructuredTypeReference StructuredTypeReference => this.structuredTypeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.SingleResourceCast;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
