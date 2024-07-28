// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineTriggerType
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum PipelineTriggerType
  {
    ContinuousIntegration = 2,
    PullRequest = 64, // 0x00000040
    PipelineCompletion = 128, // 0x00000080
    ContainerImage = 256, // 0x00000100
    BuildResourceCompletion = 512, // 0x00000200
    PackageUpdate = 1024, // 0x00000400
    WebhookTriggeredEvent = 2048, // 0x00000800
  }
}
