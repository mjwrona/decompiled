// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeEnumerator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.Collections;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  internal class NodeEnumerator : IEnumerator
  {
    private int m_index = -1;
    private Node m_node;

    internal NodeEnumerator(Node node) => this.m_node = node;

    public void Reset() => this.m_index = -1;

    public object Current => (object) this.m_node[this.m_index];

    public bool MoveNext() => ++this.m_index < this.m_node.Count;
  }
}
