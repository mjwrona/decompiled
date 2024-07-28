// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeVariableList
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class NodeVariableList : Node
  {
    private List<Node> m_list;

    protected NodeVariableList(NodeType type)
      : base(type)
    {
      this.m_list = new List<Node>();
    }

    public override DataType DataType => this.GetChildrenDataType();

    public override bool IsConst => this.GetChildrenIsConst();

    public override bool IsScalar => false;

    public override int Count => this.m_list.Count;

    public override Node this[int i]
    {
      get => this.m_list[i];
      set => this.m_list[i] = value;
    }

    public void RemoveAt(int i) => this.m_list.RemoveAt(i);

    public void Add(Node node) => this.m_list.Add(node);

    public void Clear() => this.m_list.Clear();

    public void Insert(int i, Node node) => this.m_list.Insert(i, node);

    public override void AppendTo(StringBuilder builder) => this.AppendChildren(builder, ", ");
  }
}
