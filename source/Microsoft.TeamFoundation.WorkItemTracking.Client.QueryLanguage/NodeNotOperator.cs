// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeNotOperator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeNotOperator : Node
  {
    private Node m_child;

    public NodeNotOperator()
      : base(NodeType.Not)
    {
    }

    public NodeNotOperator(Node node)
      : base(NodeType.Not)
    {
      this.m_child = node;
    }

    public override DataType DataType => DataType.Bool;

    public override bool IsConst => this.m_child.IsConst;

    public override bool IsScalar => false;

    public override Priority Priority => Priority.UnaryBoolOperator;

    public override int Count => 1;

    public override Node this[int i]
    {
      get => this.m_child;
      set => this.m_child = value;
    }

    public Node Value
    {
      get => this.m_child;
      set => this.m_child = value;
    }

    public override void AppendTo(StringBuilder builder)
    {
      builder.Append("not ");
      this.AppendChildren(builder, "");
    }

    public override void Bind(IExternal e, NodeTableName tableContext, NodeFieldName fieldContext)
    {
      this.Value.Bind(e, tableContext, fieldContext);
      Tools.EnsureSyntax(this.Value.DataType == DataType.Bool, SyntaxError.ExpectingBoolean, this.Value);
      base.Bind(e, tableContext, fieldContext);
    }

    public override Node Optimize(
      IExternal e,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      this.Value = this.Value.Optimize(e, tableContext, fieldContext);
      if (this.Value.NodeType == NodeType.BoolConst)
        return (Node) new NodeBoolConst(!((NodeBoolConst) this.Value).Value);
      return this.Value.NodeType == NodeType.Not ? ((NodeNotOperator) this.Value).Value : base.Optimize(e, tableContext, fieldContext);
    }
  }
}
