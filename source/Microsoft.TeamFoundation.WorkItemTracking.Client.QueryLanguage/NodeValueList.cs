// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeValueList
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeValueList : NodeVariableList
  {
    public NodeValueList()
      : base(NodeType.ValueList)
    {
    }

    public override Priority Priority => Priority.CommaOperator;

    public override bool CanCastTo(DataType type, CultureInfo culture) => this.GetChildrenCanCastTo(type, culture);

    public override void Bind(IExternal e, NodeTableName tableContext, NodeFieldName fieldContext)
    {
      this.BindChildren(e, tableContext, fieldContext);
      if (e != null)
        Tools.EnsureSyntax(this.GetChildrenDataType() != 0, SyntaxError.UnknownOrIncompatibleTypesInTheList, (Node) this);
      base.Bind(e, tableContext, fieldContext);
    }

    public override Node Optimize(
      IExternal e,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      this.OptimizeChildren(e, tableContext, fieldContext);
      return base.Optimize(e, tableContext, fieldContext);
    }
  }
}
