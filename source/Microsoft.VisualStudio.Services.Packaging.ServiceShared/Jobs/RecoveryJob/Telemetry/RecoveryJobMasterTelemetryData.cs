// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RecoveryJob.Telemetry.RecoveryJobMasterTelemetryData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RecoveryJob.Telemetry
{
  public class RecoveryJobMasterTelemetryData : JobTelemetry
  {
    public DateTime RecoveryUtcTimePoint { get; set; }

    public bool PerformRepair { get; set; }

    public string FeedId { get; set; }

    public int NumberOfCollection { get; set; }

    public int NumberOfCollectionWithoutFeed { get; set; }

    public int RecoveryWorkerJobsQueued { get; set; }

    public int RecoveryWorkerJobsFailed { get; set; }

    public int RecoveryWorkerJobsSucceeded { get; set; }

    public int CollectionsQueuedForRecovery { get; set; }

    public int CollectionsCompletedRecovery { get; set; }

    public int CollectionChangeProcessingJobsQueued { get; set; }
  }
}
