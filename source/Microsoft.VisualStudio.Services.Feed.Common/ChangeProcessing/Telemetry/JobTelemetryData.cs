// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ChangeProcessing.Telemetry.JobTelemetryData
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Feed.Common.ChangeProcessing.Telemetry
{
  public class JobTelemetryData
  {
    public int FeedsProcessedCount => this.FeedsProcessed.Length;

    public long ChangesProcessed { get; set; }

    public string InitialContinuationToken { get; set; }

    public string FinalContinuationToken { get; set; }

    public Guid CollectionId { get; set; }

    public int FeedCountBatchSize { get; set; }

    public TimeSpan FeedUpdateGracePeriod { get; set; }

    public DateTime JobStartTime { get; set; }

    public Guid[] FeedsProcessed { get; set; }
  }
}
