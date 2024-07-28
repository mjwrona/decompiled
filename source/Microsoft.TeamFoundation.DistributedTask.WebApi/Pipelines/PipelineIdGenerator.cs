// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineIdGenerator
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PipelineIdGenerator : IPipelineIdGenerator
  {
    private bool m_preserveCase;

    public PipelineIdGenerator(bool preserveCase = false) => this.m_preserveCase = preserveCase;

    public Guid GetInstanceId(params string[] segments) => PipelineUtilities.GetInstanceId(this.GetInstanceName(segments), this.m_preserveCase);

    public string GetInstanceName(params string[] segments) => PipelineUtilities.GetInstanceName(segments);

    public string GetStageIdentifier(string stageName) => PipelineUtilities.GetStageIdentifier(stageName);

    public Guid GetStageInstanceId(string stageName, int attempt) => PipelineUtilities.GetStageInstanceId(stageName, attempt, this.m_preserveCase);

    public string GetStageInstanceName(string stageName, int attempt) => PipelineUtilities.GetStageInstanceName(stageName, attempt);

    public string GetPhaseIdentifier(string stageName, string phaseName) => PipelineUtilities.GetPhaseIdentifier(stageName, phaseName);

    public Guid GetPhaseInstanceId(string stageName, string phaseName, int attempt) => PipelineUtilities.GetPhaseInstanceId(stageName, phaseName, attempt, this.m_preserveCase);

    public string GetPhaseInstanceName(string stageName, string phaseName, int attempt) => PipelineUtilities.GetPhaseInstanceName(stageName, phaseName, attempt);

    public string GetJobIdentifier(
      string stageName,
      string phaseName,
      string jobName,
      int checkRerunAttempt = 1)
    {
      return PipelineUtilities.GetJobIdentifier(stageName, phaseName, jobName, checkRerunAttempt: checkRerunAttempt);
    }

    public Guid GetJobInstanceId(
      string stageName,
      string phaseName,
      string jobName,
      int attempt,
      int checkRerunAttempt = 1)
    {
      return PipelineUtilities.GetJobInstanceId(stageName, phaseName, jobName, attempt, this.m_preserveCase, checkRerunAttempt);
    }

    public string GetJobInstanceName(
      string stageName,
      string phaseName,
      string jobName,
      int attempt,
      int checkRerunAttempt = 1)
    {
      return PipelineUtilities.GetJobInstanceName(stageName, phaseName, jobName, attempt, checkRerunAttempt);
    }

    public string GetTaskInstanceName(
      string stageName,
      string phaseName,
      string jobName,
      int jobAttempt,
      string taskName,
      int checkRerunAttempt = 1)
    {
      return PipelineUtilities.GetTaskInstanceName(stageName, phaseName, jobName, jobAttempt, taskName, checkRerunAttempt);
    }

    public Guid GetTaskInstanceId(
      string stageName,
      string phaseName,
      string jobName,
      int jobAttempt,
      string taskName,
      int checkRerunAttempt = 1)
    {
      return PipelineUtilities.GetTaskInstanceId(stageName, phaseName, jobName, jobAttempt, taskName, this.m_preserveCase, checkRerunAttempt);
    }
  }
}
