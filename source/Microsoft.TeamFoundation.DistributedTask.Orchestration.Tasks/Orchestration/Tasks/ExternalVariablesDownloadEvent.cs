// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.ExternalVariablesDownloadEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public sealed class ExternalVariablesDownloadEvent : TaskEvent
  {
    public ExternalVariablesDownloadEvent()
      : base("FetchedExternalVariables")
    {
    }

    public ExternalVariablesDownloadEvent(
      Guid jobId,
      Guid taskId,
      Guid scopeId,
      Guid planId,
      TaskResult taskResult,
      IList<ServerTaskSectionExecutionOptions> executionOptions)
      : base("FetchedExternalVariables", jobId, taskId)
    {
      this.ScopeId = scopeId;
      this.PlanId = planId;
      this.Result = taskResult;
      this.ExecutionOptions = executionOptions;
    }

    [DataMember]
    public Guid ScopeId { get; set; }

    [DataMember]
    public Guid PlanId { get; set; }

    [DataMember]
    public string ArtifactUri { get; set; }

    [DataMember]
    public PlanEnvironment Environment { get; set; }

    [DataMember]
    public TaskResult Result { get; set; }

    [DataMember]
    public IList<ServerTaskSectionExecutionOptions> ExecutionOptions { get; set; }
  }
}
