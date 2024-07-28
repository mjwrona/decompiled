// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.WorkItemRowSets
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal class WorkItemRowSets : IWorkItemRowSets
  {
    private readonly RowSetCollection m_rowSets;

    internal WorkItemRowSets(RowSetCollection rowSets) => this.m_rowSets = rowSets;

    public IRowSet this[WorkItemRowSetNames name] => (IRowSet) this.m_rowSets[name != WorkItemRowSetNames.Latest ? name.ToString() : "WorkItemInfo"];

    public IRowSet this[string name] => (IRowSet) this.m_rowSets[name];

    public bool TryGetRowSet(string name, out IRowSet rowset)
    {
      RowSet rowset1;
      if (this.m_rowSets.TryGetRowSet(name, out rowset1))
      {
        rowset = (IRowSet) rowset1;
        return true;
      }
      rowset = (IRowSet) null;
      return false;
    }
  }
}
