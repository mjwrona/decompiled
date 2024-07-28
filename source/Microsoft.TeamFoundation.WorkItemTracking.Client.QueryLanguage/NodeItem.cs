// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class NodeItem : Node
  {
    private string m_string;

    protected NodeItem(NodeType type, string s)
      : base(type)
    {
      this.m_string = s;
    }

    public string Value
    {
      get => this.m_string;
      set => this.m_string = value;
    }

    public override bool IsScalar => true;

    public override Priority Priority => Priority.Operand;

    public override int Count => 0;

    public override Node this[int i]
    {
      get => (Node) null;
      set
      {
      }
    }

    public override void AppendTo(StringBuilder builder) => builder.Append(this.m_string);
  }
}
