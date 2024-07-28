// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeFieldList
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeFieldList : NodeVariableList
  {
    public NodeFieldList(NodeType nodeType)
      : base(nodeType)
    {
    }

    public override Priority Priority => Priority.CommaOperator;

    public NodeFieldName this[int i]
    {
      get => (NodeFieldName) base[i];
      set => this[i] = (Node) value;
    }

    public override void Bind(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      this.BindChildren(external, (NodeTableName) null, (NodeFieldName) null);
      for (int i = 0; i < this.Count; ++i)
        Tools.EnsureSyntax(this[i].NodeType == NodeType.FieldName, SyntaxError.ExpectingFieldName, (Node) this[i]);
      base.Bind(external, tableContext, fieldContext);
    }

    public override Node Optimize(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      this.OptimizeChildren(external, tableContext, fieldContext);
      return base.Optimize(external, tableContext, fieldContext);
    }
  }
}
