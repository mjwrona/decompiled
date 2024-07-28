// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ServiceHooks.Sdk.Server.PipelineEventType
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

namespace Microsoft.TeamFoundation.ServiceHooks.Sdk.Server
{
  public enum PipelineEventType
  {
    XPipesReceived,
    XPipesRouted,
    ServiceHookEventReceived,
    NoActionableEvents,
    NoMatchingPipelines,
    JobCompleted,
    HandleEvent,
    BuildStatus,
    IgnoreEvent,
    DefinitionCountExceeded,
    ExceptionThrown,
    EventReceivedBySubscriber,
    EventReceivedByGitHubPullRequestPollingJob,
  }
}
