// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.TraceConstants
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  public static class TraceConstants
  {
    public const string Area = "Deployment";
    public const string ArtifactTriggerLayer = "ArtifactTrigger";
    public const string ServiceLayer = "Service";
    public const string TraceabilityComponentLayer = "TraceabilityComponentLayer";
    public const int CreateNoteTracePoint = 100161000;
    public const int CreateNoteFailedTracePoint = 100161001;
    public const int CreateOccurenceTracePoint = 100161002;
    public const int CreateOccurenceFailedTracePoint = 100161003;
    public const int AddImageDetailsTracePoint = 100161004;
    public const int AddImageDetailsFailedTracePoint = 100161005;
    public const int FailedToLoadYamlPipeline = 100161006;
    public const int CannotGetEndpoint = 100161007;
    public const int ArtifactTraceabilityError = 100161008;
    public const int WebHookSubscriptionDoesNotExists = 100161009;
    public const int PipelineWebHookNotificationJobQueued = 100161010;
    public const int ImageLayerTracePoint = 100161012;
    public const int PipelineRunTraceabilitySnapshotComponent = 100161013;
    public const int PipelineRunTraceabilitySnapshotJobJobStart = 100161014;
    public const int PipelineRunTraceabilitySnapshotJob = 100161015;
    public const int PipelineRunTraceabilitySnapshotJobJobCompleted = 100161016;
    public const int PipelineRunTraceabilityStartJob = 100161017;
    public const int CDPipelineRunReverseTraceabilityError = 100161018;
    public const int TriggeredPipelineForArtifactEventInfo = 100161024;
    public const int TriggeredPipelineForArtifactEventSkipTrigger = 100161025;
    public const int TriggeredPipelineForArtifactEventException = 100161026;
    public const int PipelineTriggerCreateStart = 100161020;
    public const int PipelineTriggerCreateComplete = 100161021;
    public const int PipelineTriggerDeleteStart = 100161022;
    public const int PipelineTriggerDeleteComplete = 100161023;
    public const int PipelineTriggerCreate = 100161030;
    public const int PipelineTriggerDelete = 100161031;
    public const int PipelineTriggerUpdate = 100161032;
    public const int PipelineTriggerUpdateWebHooks = 100161033;
  }
}
