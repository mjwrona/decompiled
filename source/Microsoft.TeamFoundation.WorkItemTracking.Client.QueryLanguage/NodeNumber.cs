// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeNumber
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeNumber : NodeItem
  {
    public NodeNumber(string s)
      : base(NodeType.Number, s)
    {
    }

    public override DataType DataType => DataType.Numeric;

    public override bool IsConst => true;

    public override string ConstStringValue => this.Value;

    public override bool CanCastTo(DataType type, CultureInfo culture) => type == DataType.Bool || base.CanCastTo(type, culture);

    public override void Bind(IExternal e, NodeTableName tableContext, NodeFieldName fieldContext)
    {
      Tools.EnsureSyntax(Tools.IsNumericString(this.Value), SyntaxError.StringIsNotANumber, (Node) this);
      base.Bind(e, tableContext, fieldContext);
    }
  }
}
