// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobQueueComponent24
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobQueueComponent24 : JobQueueComponent23
  {
    public override int QueueJobs(
      Guid jobSource,
      IEnumerable<Tuple<Guid, int>> jobsToRun,
      Guid requesterActivityId,
      Guid requesterVsid,
      JobPriorityLevel priorityLevel,
      int delaySeconds,
      bool queueAsDormant)
    {
      this.PrepareStoredProcedure("prc_QueueJobs");
      this.BindGuid("@jobSource", jobSource);
      this.BindJobQueueUpdateTable("@jobList", jobsToRun);
      this.BindGuid("@requesterActivityId", requesterActivityId);
      this.BindGuid("@requesterVsid", requesterVsid);
      this.BindInt("@priorityLevel", (int) priorityLevel);
      this.BindInt("@delaySeconds", delaySeconds);
      this.BindBoolean("@queueAsDormant", queueAsDormant);
      return (int) this.ExecuteScalar();
    }
  }
}
