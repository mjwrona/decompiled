// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingEtwLogger
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics.Tracing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EventSource(Name = "Azure-Devops-Servicing-Trace")]
  public sealed class ServicingEtwLogger : EventSource
  {
    public static readonly ServicingEtwLogger Log = new ServicingEtwLogger();

    [Event(200, Level = EventLevel.Informational, Opcode = EventOpcode.Start, Version = 2)]
    public void OperationStarted(
      string operationName,
      string serviceName,
      string branchName,
      string buildNumber,
      string deploymentName,
      string releaseDefinitionId,
      string releaseId,
      string attemptNumber,
      string jobId,
      DateTime timestamp)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(200, (object) operationName, (object) serviceName, (object) branchName, (object) buildNumber, (object) deploymentName, (object) releaseDefinitionId, (object) releaseId, (object) attemptNumber, (object) jobId, (object) timestamp);
    }

    [Event(201, Level = EventLevel.Informational, Opcode = EventOpcode.Stop, Version = 2)]
    public void OperationEnded(
      string operationName,
      double duration,
      string serviceName,
      string branchName,
      string buildNumber,
      string deploymentName,
      string releaseDefinitionId,
      string releaseId,
      string attemptNumber,
      string jobId,
      DateTime timestamp)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(201, (object) operationName, (object) duration, (object) serviceName, (object) branchName, (object) buildNumber, (object) deploymentName, (object) releaseDefinitionId, (object) releaseId, (object) attemptNumber, (object) jobId, (object) timestamp);
    }

    [Event(300, Level = EventLevel.Informational, Opcode = EventOpcode.Start, Version = 2)]
    public void StepGroupStarted(
      string operationName,
      string stepGroupName,
      string serviceName,
      string branchName,
      string buildNumber,
      string deploymentName,
      string releaseDefinitionId,
      string releaseId,
      string attemptNumber,
      string jobId,
      DateTime timestamp)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(300, (object) operationName, (object) stepGroupName, (object) serviceName, (object) branchName, (object) buildNumber, (object) deploymentName, (object) releaseDefinitionId, (object) releaseId, (object) attemptNumber, (object) jobId, (object) timestamp);
    }

    [Event(301, Level = EventLevel.Informational, Opcode = EventOpcode.Stop, Version = 2)]
    public void StepGroupEnded(
      string operationName,
      string stepGroupName,
      double duration,
      string serviceName,
      string branchName,
      string buildNumber,
      string deploymentName,
      string releaseDefinitionId,
      string releaseId,
      string attemptNumber,
      string jobId,
      DateTime timestamp)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(301, (object) operationName, (object) stepGroupName, (object) duration, (object) serviceName, (object) branchName, (object) buildNumber, (object) deploymentName, (object) releaseDefinitionId, (object) releaseId, (object) attemptNumber, (object) jobId, (object) timestamp);
    }

    [Event(400, Level = EventLevel.Informational, Opcode = EventOpcode.Start, Version = 2)]
    public void StepStarted(
      string operationName,
      string stepGroupName,
      string stepName,
      string state,
      string serviceName,
      string branchName,
      string buildNumber,
      string deploymentName,
      string releaseDefinitionId,
      string releaseId,
      string attemptNumber,
      string jobId,
      DateTime timestamp)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(400, (object) operationName, (object) stepGroupName, (object) stepName, (object) state, (object) serviceName, (object) branchName, (object) buildNumber, (object) deploymentName, (object) releaseDefinitionId, (object) releaseId, (object) attemptNumber, (object) jobId, (object) timestamp);
    }

    [Event(401, Level = EventLevel.Informational, Opcode = EventOpcode.Stop, Version = 2)]
    public void StepEnded(
      string operationName,
      string stepGroupName,
      string stepName,
      string state,
      double duration,
      string serviceName,
      string branchName,
      string buildNumber,
      string deploymentName,
      string releaseDefinitionId,
      string releaseId,
      string attemptNumber,
      string jobId,
      DateTime timestamp)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEvent(401, (object) operationName, (object) stepGroupName, (object) stepName, (object) state, (object) duration, (object) serviceName, (object) branchName, (object) buildNumber, (object) deploymentName, (object) releaseDefinitionId, (object) releaseId, (object) attemptNumber, (object) jobId, (object) timestamp);
    }
  }
}
