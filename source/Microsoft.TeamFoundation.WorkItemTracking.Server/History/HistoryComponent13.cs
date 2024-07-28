// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.HistoryComponent13
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  internal class HistoryComponent13 : HistoryComponent12
  {
    public override IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinksByResourceIds(
      IEnumerable<KeyValuePair<int, int>> workItemIdResourceIdPairs,
      bool includeExtendedProperties)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemResourceLinksByResourceIds");
      this.BindKeyValuePairInt32Int32Table("@workItemIdResourceIdPairs", workItemIdResourceIdPairs);
      this.BindBoolean("@includeExtendedProperties", includeExtendedProperties);
      return this.ExecuteUnknown<IReadOnlyCollection<WorkItemResourceLinkInfo>>((System.Func<IDataReader, IReadOnlyCollection<WorkItemResourceLinkInfo>>) (reader => (IReadOnlyCollection<WorkItemResourceLinkInfo>) new HistoryComponent.WorkItemResourceLinkBinder(includeExtendedProperties).BindAll(reader).ToList<WorkItemResourceLinkInfo>()));
    }
  }
}
