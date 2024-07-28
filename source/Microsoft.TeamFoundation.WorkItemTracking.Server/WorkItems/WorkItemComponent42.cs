// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent42
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent42 : WorkItemComponent41
  {
    public override List<WorkItemDependencyGraph> LoadDependencyGraphForWorkItem(int workItemId)
    {
      this.PrepareStoredProcedure("prc_LoadDependencyGraph");
      this.BindInt("@workItemId", workItemId);
      return this.ExecuteUnknown<IEnumerable<WorkItemDependencyGraph>>((System.Func<IDataReader, IEnumerable<WorkItemDependencyGraph>>) (reader => new WorkItemComponent.WorkItemDependencyGraphBinder().BindAll(reader))).ToList<WorkItemDependencyGraph>();
    }
  }
}
