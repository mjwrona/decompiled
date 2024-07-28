// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeArithmetic
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeArithmetic : NodeList
  {
    private Arithmetic m_operator;

    public NodeArithmetic()
      : base(NodeType.Arithmetic, 2)
    {
    }

    public Arithmetic Arithmetic
    {
      get => this.m_operator;
      set => this.m_operator = value;
    }

    public override DataType DataType => this.Left.DataType;

    public override Priority Priority => Priority.AddOperator;

    public override void AppendTo(StringBuilder builder) => this.AppendChildren(builder, " " + ArithmeticalOperators.GetString(this.m_operator) + " ");

    public Node Left
    {
      get => this[0];
      set => this[0] = value;
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
      this.Right.Bind(external, tableContext, fieldContext);
      Tools.EnsureSyntax(this.Left.IsConst, SyntaxError.ExpectingConst, this.Left);
      Tools.EnsureSyntax(this.Right.IsConst, SyntaxError.ExpectingConst, this.Right);
      base.Bind(external, tableContext, fieldContext);
    }

    public override Node Optimize(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      if (this.Left is NodeVariable && (this.Left as NodeVariable).DoesMacroExtensionHandleOffset)
        return base.Optimize(external, tableContext, fieldContext);
      this.OptimizeChildren(external, tableContext, fieldContext);
      return base.Optimize(external, tableContext, fieldContext);
    }
  }
}
