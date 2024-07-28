// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.DeploymentJobDetails
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal class DeploymentJobDetails
  {
    private List<int> m_candidateAgents;

    public DeploymentJobDetails() => this.m_candidateAgents = new List<int>();

    public List<int> CandidateAgents
    {
      get
      {
        if (this.m_candidateAgents == null)
          this.m_candidateAgents = new List<int>();
        return this.m_candidateAgents;
      }
    }

    public TaskCompletionSource<JobAssignedEventData> Request { get; set; }
  }
}
