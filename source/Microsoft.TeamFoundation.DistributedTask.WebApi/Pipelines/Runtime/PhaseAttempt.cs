// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.PhaseAttempt
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PhaseAttempt
  {
    private List<JobAttempt> m_jobs;

    public PhaseInstance Phase { get; set; }

    public IList<JobAttempt> Jobs
    {
      get
      {
        if (this.m_jobs == null)
          this.m_jobs = new List<JobAttempt>();
        return (IList<JobAttempt>) this.m_jobs;
      }
    }
  }
}
