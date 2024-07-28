// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeList
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class NodeList : Node
  {
    private Node[] m_array;

    protected NodeList(NodeType type, int count)
      : base(type)
    {
      this.m_array = new Node[count];
    }

    public override DataType DataType => this.GetChildrenDataType();

    public override bool IsConst => this.GetChildrenIsConst();

    public override bool IsScalar => false;

    public override int Count => this.m_array.Length;

    public override Node this[int i]
    {
      get => this.m_array[i];
      set => this.m_array[i] = value;
    }

    public override void AppendTo(StringBuilder builder) => this.AppendChildren(builder, ", ");
  }
}
