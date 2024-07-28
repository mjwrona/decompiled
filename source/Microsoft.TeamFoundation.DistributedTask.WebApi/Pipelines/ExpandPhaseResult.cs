// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ExpandPhaseResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ExpandPhaseResult
  {
    private List<JobInstance> m_jobs;

    public ExpandPhaseResult() => this.MaxConcurrency = 1;

    public bool ContinueOnError { get; set; }

    public bool FailFast { get; set; }

    public int MaxConcurrency { get; set; }

    public IList<JobInstance> Jobs
    {
      get
      {
        if (this.m_jobs == null)
          this.m_jobs = new List<JobInstance>();
        return (IList<JobInstance>) this.m_jobs;
      }
    }
  }
}
