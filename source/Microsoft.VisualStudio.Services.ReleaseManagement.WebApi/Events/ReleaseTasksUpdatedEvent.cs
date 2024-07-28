// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events.ReleaseTasksUpdatedEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events
{
  [DataContract]
  public class ReleaseTasksUpdatedEvent : RealtimeReleaseEvent
  {
    public ReleaseTasksUpdatedEvent(
      Guid projectId,
      int releaseId,
      int environmentId,
      int releaseStepId,
      int releaseDeployPhaseId,
      IEnumerable<ReleaseTask> tasks,
      ReleaseTask job)
      : base(projectId, releaseId, environmentId)
    {
      this.ReleaseStepId = releaseStepId;
      this.ReleaseDeployPhaseId = releaseDeployPhaseId;
      this.Tasks = tasks;
      this.Job = job;
    }

    [DataMember]
    public int ReleaseStepId { get; private set; }

    [Obsolete]
    [DataMember]
    public int ReleaseDeployPhaseId { get; private set; }

    [DataMember]
    public IEnumerable<ReleaseTask> Tasks { get; private set; }

    [DataMember]
    public ReleaseTask Job { get; private set; }

    [DataMember]
    public Guid PlanId { get; set; }
  }
}
