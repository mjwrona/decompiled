// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeCondition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeCondition : NodeList
  {
    private Condition m_condition;

    public NodeCondition()
      : base(NodeType.FieldCondition, 2)
    {
    }

    public NodeCondition(Condition condition, NodeFieldName left, Node right)
      : this()
    {
      this.m_condition = condition;
      this.Left = left;
      this.Right = right;
    }

    public Condition Condition
    {
      get => this.m_condition;
      set => this.m_condition = value;
    }

    public override DataType DataType => DataType.Bool;

    public override Priority Priority => Priority.ConditionalOperator;

    public override void AppendTo(StringBuilder builder) => this.AppendChildren(builder, " " + ConditionalOperators.GetString(this.m_condition) + (this.Right == null ? "" : " "));

    public NodeFieldName Left
    {
      get => (NodeFieldName) this[0];
      set => this[0] = (Node) value;
    }

    public Node Right
    {
      get => this[1];
      set => this[1] = value;
    }

    public override void Bind(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      this.Left.Bind(external, tableContext, fieldContext);
      if (this.Right != null)
      {
        this.Right.Bind(external, tableContext, this.Left);
        Tools.EnsureSyntax(this.Right.IsConst || this.Right is NodeFieldName, SyntaxError.InvalidRightExpressionInCondition, this.Right);
        if (external != null)
          Tools.EnsureSyntax(this.Right.CanCastTo(this.Left.DataType, external.CultureInfo), SyntaxError.IncompatibleConditionPartsType, (Node) this);
      }
      if (external != null && (this.Condition == Condition.Contains || this.Condition == Condition.ContainsWords))
        Tools.EnsureSyntax(this.Left.DataType == DataType.String, SyntaxError.ContainsWorksForStringsOnly, (Node) this);
      base.Bind(external, tableContext, fieldContext);
    }

    public override Node Optimize(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      this.Left = (NodeFieldName) this.Left.Optimize(external, tableContext, fieldContext);
      this.Right = this.Right?.Optimize(external, tableContext, this.Left);
      return base.Optimize(external, tableContext, fieldContext);
    }
  }
}
