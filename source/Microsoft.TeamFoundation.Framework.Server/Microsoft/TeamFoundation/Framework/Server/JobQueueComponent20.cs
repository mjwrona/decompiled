// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent20
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent20 : JobQueueComponent19
  {
    internal override List<TeamFoundationJobQueueEntry> RescheduleJobs(
      IEnumerable<Guid> onlineProcesses)
    {
      this.PrepareStoredProcedure("prc_RescheduleJobs");
      this.BindSortedGuidTable("@onlineProcesses", onlineProcesses);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationJobQueueEntry>((ObjectBinder<TeamFoundationJobQueueEntry>) new TeamFoundationJobQueueEntryColumns3());
      return resultCollection.GetCurrent<TeamFoundationJobQueueEntry>().Items;
    }
  }
}
