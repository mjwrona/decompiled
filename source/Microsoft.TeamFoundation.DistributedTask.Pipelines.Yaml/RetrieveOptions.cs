// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.RetrieveOptions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Flags]
  public enum RetrieveOptions
  {
    Default = 0,
    Process = 1,
    ContinuousIntegrationTrigger = 2,
    PullRequestTrigger = 4,
    PipelineSchedules = 8,
    PipelineParameters = 16, // 0x00000010
    All = PipelineParameters | PipelineSchedules | PullRequestTrigger | ContinuousIntegrationTrigger | Process, // 0x0000001F
  }
}
