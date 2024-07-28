// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentRequestQueryResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskAgentRequestQueryResult
  {
    internal TaskAgentRequestQueryResult()
      : this((IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>(), (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>(), (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>())
    {
    }

    internal TaskAgentRequestQueryResult(
      IList<TaskAgentJobRequest> runningRequests,
      IList<TaskAgentJobRequest> queuedRequests,
      IList<TaskAgentJobRequest> finishedRequests)
    {
      this.RunningRequests = runningRequests;
      this.QueuedRequests = queuedRequests;
      this.FinishedRequests = finishedRequests;
    }

    public IList<TaskAgentJobRequest> RunningRequests { get; set; }

    public IList<TaskAgentJobRequest> QueuedRequests { get; set; }

    public IList<TaskAgentJobRequest> FinishedRequests { get; set; }
  }
}
