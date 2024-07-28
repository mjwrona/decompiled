// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.IPipelineIdGenerator
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IPipelineIdGenerator
  {
    Guid GetInstanceId(params string[] segments);

    string GetInstanceName(params string[] segments);

    string GetStageIdentifier(string stageName);

    Guid GetStageInstanceId(string stageName, int attempt);

    string GetStageInstanceName(string stageName, int attempt);

    string GetPhaseIdentifier(string stageName, string phaseName);

    Guid GetPhaseInstanceId(string stageName, string phaseName, int attempt);

    string GetPhaseInstanceName(string stageName, string phaseName, int attempt);

    string GetJobIdentifier(
      string stageName,
      string phaseName,
      string jobName,
      int checkRerunAttempt = 1);

    Guid GetJobInstanceId(
      string stageName,
      string phaseName,
      string jobName,
      int attempt,
      int checkRerunAttempt = 1);

    string GetJobInstanceName(
      string stageName,
      string phaseName,
      string jobName,
      int attempt,
      int checkRerunAttempt = 1);

    Guid GetTaskInstanceId(
      string stageName,
      string phaseName,
      string jobName,
      int jobAttempt,
      string taskName,
      int checkRerunAttempt = 1);

    string GetTaskInstanceName(
      string stageName,
      string phaseName,
      string jobName,
      int jobAttempt,
      string name,
      int checkRerunAttempt = 1);
  }
}
