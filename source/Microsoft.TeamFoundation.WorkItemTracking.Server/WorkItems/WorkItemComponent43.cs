// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent43
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent43 : WorkItemComponent42
  {
    public override List<int> GetWorkItemViolations(List<int> workItemIds)
    {
      this.PrepareStoredProcedure("prc_GetDependencyViolationsForWorkItems");
      this.BindInt32Table("@workItemIds", (IEnumerable<int>) workItemIds);
      return this.ExecuteUnknown<IEnumerable<int>>((System.Func<IDataReader, IEnumerable<int>>) (reader => new WorkItemComponent.WorkItemDependencyViolationsBinder().BindAll(reader))).ToList<int>();
    }
  }
}
