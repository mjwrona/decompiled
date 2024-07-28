// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.StageAttempt
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StageAttempt
  {
    private List<PhaseAttempt> m_phases;

    internal StageAttempt()
    {
    }

    public StageAttempt(Timeline timeline) => this.Timeline = timeline;

    public StageInstance Stage { get; set; }

    public IList<PhaseAttempt> Phases
    {
      get
      {
        if (this.m_phases == null)
          this.m_phases = new List<PhaseAttempt>();
        return (IList<PhaseAttempt>) this.m_phases;
      }
    }

    public Timeline Timeline { get; internal set; }
  }
}
