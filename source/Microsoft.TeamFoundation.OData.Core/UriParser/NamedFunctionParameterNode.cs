// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.NamedFunctionParameterNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public class NamedFunctionParameterNode : QueryNode
  {
    private readonly string name;
    private readonly QueryNode value;

    public NamedFunctionParameterNode(string name, QueryNode value)
    {
      this.name = name;
      this.value = value;
    }

    public string Name => this.name;

    public QueryNode Value => this.value;

    public override QueryNodeKind Kind => (QueryNodeKind) this.InternalKind;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.NamedFunctionParameter;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
