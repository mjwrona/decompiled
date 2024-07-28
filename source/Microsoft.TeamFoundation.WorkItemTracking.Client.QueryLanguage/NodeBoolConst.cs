// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeBoolConst
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeBoolConst : Node
  {
    private bool m_bool;

    public NodeBoolConst(bool value)
      : base(NodeType.BoolConst)
    {
      this.m_bool = value;
    }

    public bool Value => this.m_bool;

    public override DataType DataType => DataType.Bool;

    public override bool IsConst => true;

    public override bool IsScalar => false;

    public override Priority Priority => Priority.Operand;

    public override int Count => 0;

    public override Node this[int i]
    {
      get => (Node) null;
      set
      {
      }
    }

    public override void AppendTo(StringBuilder b) => b.Append(this.m_bool.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }
}
