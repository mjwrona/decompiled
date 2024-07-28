// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [JsonConverter(typeof (EnumConverter))]
  [Flags]
  [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Zero value is defined already")]
  public enum DeploymentOperationStatus
  {
    Undefined = 0,
    Queued = 1,
    Scheduled = 2,
    Pending = 4,
    Approved = 8,
    Rejected = 16, // 0x00000010
    Deferred = 32, // 0x00000020
    QueuedForAgent = 64, // 0x00000040
    PhaseInProgress = 128, // 0x00000080
    PhaseSucceeded = 256, // 0x00000100
    PhasePartiallySucceeded = 512, // 0x00000200
    PhaseFailed = 1024, // 0x00000400
    Canceled = 2048, // 0x00000800
    PhaseCanceled = 4096, // 0x00001000
    ManualInterventionPending = 8192, // 0x00002000
    QueuedForPipeline = 16384, // 0x00004000
    Cancelling = 32768, // 0x00008000
    EvaluatingGates = 65536, // 0x00010000
    GateFailed = 131072, // 0x00020000
    All = GateFailed | EvaluatingGates | Cancelling | QueuedForPipeline | ManualInterventionPending | Canceled | PhaseFailed | PhasePartiallySucceeded | PhaseSucceeded | PhaseInProgress | QueuedForAgent | Deferred | Rejected | Approved | Pending | Scheduled | Queued, // 0x0003EFFF
  }
}
