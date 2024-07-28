// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemCountComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemCountComponent2 : WorkItemCountComponent
  {
    public WorkItemCountComponent2()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.None;
    }

    public override List<WorkItemCountContainer> GetTopWorkItemCounts(int topCount)
    {
      this.PrepareStoredProcedure("prc_QueryCollectionsWithHighestNumOfWorkItems");
      this.BindInt("@topCount", topCount);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<WorkItemCountContainer>((ObjectBinder<WorkItemCountContainer>) new WorkItemCountColumns());
        return resultCollection.GetCurrent<WorkItemCountContainer>().Items;
      }
    }
  }
}
