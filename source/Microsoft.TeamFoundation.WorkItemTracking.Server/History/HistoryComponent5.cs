// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.HistoryComponent5
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  internal class HistoryComponent5 : HistoryComponent4
  {
    private IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinksInternal(
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      bool byRevision,
      bool includeExtentedProperties)
    {
      workItemIdRevPairs = (IEnumerable<WorkItemIdRevisionPair>) workItemIdRevPairs.Distinct<WorkItemIdRevisionPair>().ToArray<WorkItemIdRevisionPair>();
      this.PrepareStoredProcedure("prc_GetWorkItemResourceLinks");
      this.BindWorkItemIdRevPairs("@workItemIdRevPairs", workItemIdRevPairs);
      this.BindBoolean("@byRevision", byRevision);
      this.BindBoolean("@includeExtendedProperties", includeExtentedProperties);
      return this.ExecuteUnknown<IReadOnlyCollection<WorkItemResourceLinkInfo>>((System.Func<IDataReader, IReadOnlyCollection<WorkItemResourceLinkInfo>>) (reader => (IReadOnlyCollection<WorkItemResourceLinkInfo>) new HistoryComponent.WorkItemResourceLinkBinder(includeExtentedProperties).BindAll(reader).ToList<WorkItemResourceLinkInfo>()));
    }

    public override IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinks(
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      bool includeExtendedProperties)
    {
      return this.GetWorkItemResourceLinksInternal(workItemIdRevPairs, true, includeExtendedProperties);
    }

    public override IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinks(
      IEnumerable<int> workItemIds,
      bool includeExtendedProperties)
    {
      return this.GetWorkItemResourceLinksInternal(workItemIds.Select<int, WorkItemIdRevisionPair>((System.Func<int, WorkItemIdRevisionPair>) (id => new WorkItemIdRevisionPair()
      {
        Id = id,
        Revision = 0
      })), false, includeExtendedProperties);
    }
  }
}
