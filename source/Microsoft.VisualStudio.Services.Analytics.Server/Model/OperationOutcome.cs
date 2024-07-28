// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.OperationOutcome
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  public enum OperationOutcome
  {
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_UNDEFINED", false)] Undefined = 0,
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_QUEUED", false)] Queued = 1,
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_SCHEDULED", false)] Scheduled = 2,
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_PENDING", false)] Pending = 4,
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_APPROVED", false)] Approved = 8,
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_REJECTED", false)] Rejected = 16, // 0x00000010
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_DEFERRED", false)] Deferred = 32, // 0x00000020
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_QUEUED_FOR_AGENT", false)] QueuedForAgent = 64, // 0x00000040
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_PHASE_IN_PROGRESS", false)] PhaseInProgress = 128, // 0x00000080
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_PHASE_SUCCEEDED", false)] PhaseSucceeded = 256, // 0x00000100
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_PHASE_PARTIALLY_SUCCEEDED", false)] PhasePartiallySucceeded = 512, // 0x00000200
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_PHASE_FAILED", false)] PhaseFailed = 1024, // 0x00000400
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_CANCELED", false)] Canceled = 2048, // 0x00000800
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_PHASE_CANCELED", false)] PhaseCanceled = 4096, // 0x00001000
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_MANUAL_INTERVENTION_PENDING", false)] ManualInterventionPending = 8192, // 0x00002000
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_QUEUED_FOR_PIPELINE", false)] QueuedForPipeline = 16384, // 0x00004000
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_CANCELLING", false)] Cancelling = 32768, // 0x00008000
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_PHASE_CANCELED", false)] EvaluatingGates = 65536, // 0x00010000
    [LocalizedDisplayName("ENUM_TYPE_OPERATION_OUTCOME_GATE_FAILED", false)] GateFailed = 131072, // 0x00020000
  }
}
