// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.Backlog.WorkItemComparer
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Server.Backlog
{
  internal class WorkItemComparer : IComparer<int>
  {
    private IDictionary<int, int> m_ownershipQueryOrder;
    private IDictionary<int, int> m_drillDownQueryOrder;
    private IDictionary<int, PagedBacklogWorkItem> m_pageData;
    private ITeamFieldSettings m_teamFieldSettings;
    private ISet<int> m_parentItems;

    internal WorkItemComparer(
      IDictionary<int, int> ownershipQueryOrder,
      IDictionary<int, int> drillDownQueryOrder)
      : this(ownershipQueryOrder, drillDownQueryOrder, (ITeamFieldSettings) null, (IDictionary<int, PagedBacklogWorkItem>) null, (ISet<int>) null)
    {
    }

    internal WorkItemComparer(
      IDictionary<int, int> ownershipQueryOrder,
      IDictionary<int, int> drillDownQueryOrder,
      ITeamFieldSettings teamFieldSettings,
      IDictionary<int, PagedBacklogWorkItem> pageData,
      ISet<int> parentItems)
    {
      ArgumentUtility.CheckForNull<IDictionary<int, int>>(ownershipQueryOrder, nameof (ownershipQueryOrder));
      ArgumentUtility.CheckForNull<IDictionary<int, int>>(drillDownQueryOrder, nameof (drillDownQueryOrder));
      this.m_ownershipQueryOrder = ownershipQueryOrder;
      this.m_drillDownQueryOrder = drillDownQueryOrder;
      this.m_teamFieldSettings = teamFieldSettings;
      this.m_pageData = pageData;
      this.m_parentItems = parentItems;
    }

    public int Compare(int aId, int bId)
    {
      if (aId == bId)
        return 0;
      if (aId < 0)
        return 1;
      if (bId < 0)
        return -1;
      bool flag1 = this.IsParentItem(aId);
      bool flag2 = this.IsParentItem(bId);
      return flag1 != flag2 ? (flag1 ? (!this.IsOwnedParent(aId) ? 1 : -1) : (!this.IsOwnedParent(bId) ? -1 : 1)) : (flag1 ? this.CompareParents(aId, bId) : this.CompareDrillDown(aId, bId));
    }

    internal virtual int CompareParents(int aId, int bId)
    {
      bool flag1 = this.IsOwnedParent(aId);
      bool flag2 = this.IsOwnedParent(bId);
      if (flag1 == flag2)
      {
        if (flag1)
          return this.m_pageData[aId].OrderValue.CompareTo(this.m_pageData[bId].OrderValue);
        int num = 0;
        if (this.m_pageData.ContainsKey(aId) && this.m_pageData.ContainsKey(bId))
        {
          num = TFStringComparer.CssTreePathName.Compare(this.m_pageData[aId].TeamFieldValue, this.m_pageData[bId].TeamFieldValue);
          if (num == 0)
            num = this.m_pageData[aId].OrderValue.CompareTo(this.m_pageData[bId].OrderValue);
        }
        if (num == 0)
          num = aId.CompareTo(bId);
        return num;
      }
      return !flag1 ? 1 : -1;
    }

    internal virtual int CompareDrillDown(int aId, int bId)
    {
      bool flag1 = this.IsOwnedChild(aId);
      bool flag2 = this.IsOwnedChild(bId);
      return flag1 == flag2 ? (flag1 ? this.m_ownershipQueryOrder[aId] - this.m_ownershipQueryOrder[bId] : this.m_drillDownQueryOrder[aId] - this.m_drillDownQueryOrder[bId]) : (!flag1 ? 1 : -1);
    }

    private bool IsParentItem(int workItemId) => this.m_parentItems != null && this.m_parentItems.Contains(workItemId);

    private bool IsOwnedChild(int workItemId) => this.m_ownershipQueryOrder.ContainsKey(workItemId);

    private bool IsOwnedParent(int workItemId) => this.m_pageData.ContainsKey(workItemId) && this.m_pageData[workItemId].IsOwned;
  }
}
