// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SingleValueOpenPropertyAccessNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class SingleValueOpenPropertyAccessNode : SingleValueNode
  {
    private readonly SingleValueNode source;
    private readonly string name;

    public SingleValueOpenPropertyAccessNode(SingleValueNode source, string openPropertyName)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(source, nameof (source));
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(openPropertyName, nameof (openPropertyName));
      this.name = openPropertyName;
      this.source = source;
    }

    public SingleValueNode Source => this.source;

    public string Name => this.name;

    public override IEdmTypeReference TypeReference => (IEdmTypeReference) null;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.SingleValueOpenPropertyAccess;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
