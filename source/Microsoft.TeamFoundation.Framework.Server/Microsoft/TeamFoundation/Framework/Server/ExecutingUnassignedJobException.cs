// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ExecutingUnassignedJobException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  internal class ExecutingUnassignedJobException : TeamFoundationJobServiceException
  {
    public ExecutingUnassignedJobException()
    {
    }

    public ExecutingUnassignedJobException(string message)
      : base(message)
    {
    }

    public ExecutingUnassignedJobException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ExecutingUnassignedJobException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public ExecutingUnassignedJobException(
      JobRunner existingJobRunner,
      TeamFoundationJobQueueEntry queueEntry,
      TeamFoundationJobHistoryEntry lastHistoryEntry,
      string startupTime,
      string deploymentServiceHostCreatedTime,
      int deploymentHostsCreated,
      Guid initialAgentId,
      Guid currentAgentId,
      string machineName,
      List<TeamFoundationServiceHostProcess> processes)
      : base(ExecutingUnassignedJobException.GetExceptionMessage(existingJobRunner, queueEntry, lastHistoryEntry, startupTime, deploymentServiceHostCreatedTime, deploymentHostsCreated, initialAgentId, currentAgentId, machineName, processes))
    {
      this.EventId = TeamFoundationEventId.JobAgentExecutingUnassignedJob;
      this.ReportException = true;
    }

    private static string GetExceptionMessage(
      JobRunner existingJobRunner,
      TeamFoundationJobQueueEntry queueEntry,
      TeamFoundationJobHistoryEntry lastHistoryEntry,
      string startupTime,
      string deploymentServiceHostCreatedTime,
      int deploymentHostsCreated,
      Guid initialAgentId,
      Guid currentAgentId,
      string machineName,
      List<TeamFoundationServiceHostProcess> processes)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (processes != null)
      {
        foreach (TeamFoundationServiceHostProcess process in processes)
          stringBuilder.AppendLine(process.ToString());
      }
      return FrameworkResources.ExecutingUnassignedJobError((object) existingJobRunner, (object) queueEntry, lastHistoryEntry == null ? (object) string.Empty : (object) lastHistoryEntry.ToString(), (object) DateTime.UtcNow, (object) startupTime, (object) deploymentHostsCreated, (object) deploymentServiceHostCreatedTime, (object) initialAgentId, (object) currentAgentId, (object) machineName, (object) stringBuilder.ToString());
    }
  }
}
