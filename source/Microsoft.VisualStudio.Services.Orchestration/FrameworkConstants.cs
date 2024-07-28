// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.FrameworkConstants
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

namespace Microsoft.VisualStudio.Services.Orchestration
{
  internal class FrameworkConstants
  {
    public const string OrchestratorEndpointFormat = "{0}/orchestrator";
    public const string WorkerEndpointFormat = "{0}/worker";
    public const string TrackingEndpointFormat = "{0}/tracking";
    public const string TaskMessageContentType = "TaskMessage";
    public const string StateMessageContentType = "StateMessage";
    public const string HistoryEventIndexPropertyName = "HistoryEventIndex";
    public const int FakeTimerIdToSplitDecision = -100;
    public const int MaxDeliveryCount = 10;
    public const int OrchestrationTransientErrorBackOffSecs = 10;
    public const int OrchestrationNonTransientErrorBackOffSecs = 120;
    public const int OrchestrationDefaultMaxConcurrentItems = 100;
    public const int ActivityTransientErrorBackOffSecs = 10;
    public const int ActivityNonTransientErrorBackOffSecs = 120;
    public const int ActivityDefaultMaxConcurrentItems = 10;
    public const int TrackingTransientErrorBackOffSecs = 10;
    public const int TrackingNonTransientErrorBackOffSecs = 120;
    public const int TrackingDefaultMaxConcurrentItems = 20;
    public const string CompressionTypePropertyName = "CompressionType";
    public const string CompressionTypeGzipPropertyValue = "gzip";
    public const string CompressionTypeNonePropertyValue = "none";
  }
}
