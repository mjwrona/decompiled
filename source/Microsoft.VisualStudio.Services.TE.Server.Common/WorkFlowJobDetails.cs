// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.WorkFlowJobDetails
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  public class WorkFlowJobDetails
  {
    public int TestRunId { get; set; }

    public Phase PhaseStep { get; set; }

    public AutomationPhaseState PhaseState { get; set; }

    public Guid JobId { get; set; }

    public string PhaseData { get; set; }

    public DateTimeOffset Timeout { get; set; }

    public bool AbortRequested { get; set; }

    public Guid AbortedBy { get; set; }

    public WorkFlowContext Context { get; set; } = new WorkFlowContext();
  }
}
