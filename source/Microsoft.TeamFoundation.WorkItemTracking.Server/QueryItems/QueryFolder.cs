// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryFolder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  public class QueryFolder : QueryItem
  {
    internal QueryFolder(QueryItemEntry queryEntry)
      : base(queryEntry)
    {
      this.Children = (IList<QueryItem>) new List<QueryItem>();
      this.HasChildren = queryEntry.HasChildren;
    }

    internal QueryFolder() => this.Children = (IList<QueryItem>) new List<QueryItem>();

    public IList<QueryItem> Children { get; internal set; }

    public bool HasChildren { get; internal set; }

    public override object Clone()
    {
      QueryFolder queryFolder = new QueryFolder();
      queryFolder.DeepCopyFrom((QueryItem) this);
      return (object) queryFolder;
    }

    internal override void DeepCopyFrom(QueryItem queryItem)
    {
      base.DeepCopyFrom(queryItem);
      if (!(queryItem is QueryFolder queryFolder))
        return;
      if (queryFolder.HasChildren || queryFolder.Children != null)
      {
        List<QueryItem> queryItemList = new List<QueryItem>();
        foreach (QueryItem child in (IEnumerable<QueryItem>) queryFolder.Children)
          queryItemList.Add(child.Clone() as QueryItem);
        this.Children = (IList<QueryItem>) queryItemList;
      }
      this.HasChildren = queryFolder.HasChildren;
    }
  }
}
