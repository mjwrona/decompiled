// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.WorkItemNodes
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  internal sealed class WorkItemNodes : Dictionary<int, WorkItemData>
  {
    private IDictionary<int, IDataRecord> records;
    private HashSet<string> parentTypes;
    private string SystemIDField = "System.Id";

    public WorkItemNodes(
      IEnumerable<IDataRecord> workitemRecords,
      IEnumerable<string> parentWorkItemTypes)
    {
      this.parentTypes = new HashSet<string>(parentWorkItemTypes, (IEqualityComparer<string>) TFStringComparer.WorkItemType);
      this.records = (IDictionary<int, IDataRecord>) workitemRecords.ToDictionary<IDataRecord, int, IDataRecord>((System.Func<IDataRecord, int>) (row => (int) row[this.SystemIDField]), (System.Func<IDataRecord, IDataRecord>) (row => row));
    }

    public WorkItemData GetOrCreateNode(int itemID)
    {
      WorkItemData node = (WorkItemData) null;
      if (this.IsValidItemID(itemID) && !this.TryGetValue(itemID, out node))
      {
        node = new WorkItemData(itemID, this.records[itemID], this.parentTypes);
        this.Add(itemID, node);
      }
      return node;
    }

    public bool IsValidItemID(int itemID) => this.records.ContainsKey(itemID);
  }
}
