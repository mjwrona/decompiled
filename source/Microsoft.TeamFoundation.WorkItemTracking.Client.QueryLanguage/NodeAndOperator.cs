// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeAndOperator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeAndOperator : NodeVariableList
  {
    public NodeAndOperator()
      : base(NodeType.And)
    {
    }

    public override DataType DataType => DataType.Bool;

    public override Priority Priority => Priority.AndOperator;

    public override void AppendTo(StringBuilder builder) => this.AppendChildren(builder, " and ");

    public override void Bind(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      this.BindChildren(external, tableContext, fieldContext);
      for (int i = 0; i < this.Count; ++i)
        Tools.EnsureSyntax(this[i].DataType == DataType.Bool, SyntaxError.ExpectingBoolean, this[i]);
      base.Bind(external, tableContext, fieldContext);
    }

    public override Node Optimize(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      this.OptimizeChildren(external, tableContext, fieldContext);
      int i = 0;
      while (i < this.Count)
      {
        if (this[i].NodeType == NodeType.BoolConst)
        {
          if (!((NodeBoolConst) this[i]).Value)
            return (Node) new NodeBoolConst(false);
          this.RemoveAt(i);
        }
        else
          ++i;
      }
      if (this.Count == 0)
        return (Node) new NodeBoolConst(true);
      return this.Count == 1 ? this[0] : base.Optimize(external, tableContext, fieldContext);
    }
  }
}
