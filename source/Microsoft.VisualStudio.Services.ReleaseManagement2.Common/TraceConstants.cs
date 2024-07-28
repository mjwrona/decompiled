// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.TraceConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common
{
  public static class TraceConstants
  {
    public const string ReleaseManagementArea = "ReleaseManagementService";
    public const string ServiceLayer = "Service";
    public const string PipelineLayer = "Pipeline";
    public const string EventLayer = "Events";
    public const string JobLayer = "JobLayer";
    public const string AnalyticsLayer = "Analytics";
    public const string DataAccessLayer = "DataAccessLayer";
    public const string DistributedTaskLayer = "DistributedTask";
    public const string ArtifactExtensions = "ArtifactExtensions";
    public const string ArtifactTrigger = "ArtifactTrigger";
    public const string ReleaseRetentionJob = "ReleaseRetentionJob";
    public const string ApprovalsFilterLayer = "ApprovalsFilter";
    public const string SignalR = "signalR";
    public const string PipelineWorkflowArea = "PipelineWorkflow";
    public const string ReleaseViewArea = "ReleaseView";
    public const string ReleaseEventDispatcher = "ReleaseEventDispatcher";
    public const string ActionRequestsProcessorJob = "ActionRequestsProcessorJob";
    public const int ServiceTracePointStart = 1961000;
    public const int ServiceTracePointEnd = 1962000;
    public const int PipelineTracePointStart = 1960000;
    public const int PipelineTracePointEnd = 1961000;
    public const int ReleaseLogsTracePoint = 1900046;
    public const int ReleaseLogsExceptionTracePoint = 1900047;
    public const int ExternalVariablesDownloadJobException = 1900048;
    public const int ReleaseDefinitionRevisionNotFound = 1900050;
    public static readonly string ExternalVariablesDownloadJobName = nameof (ExternalVariablesDownloadJobName);
  }
}
