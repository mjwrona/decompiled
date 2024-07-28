// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.PagedItemsRowSets
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public class PagedItemsRowSets : IPagedItemsRowSets
  {
    private readonly RowSetCollection m_rowSets;

    public PagedItemsRowSets(RowSetCollection rowSets) => this.m_rowSets = rowSets;

    public IRowSet this[PagedItemsRowSetNames name] => (IRowSet) this.m_rowSets[name.ToString()];
  }
}
