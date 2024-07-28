// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.CollectionDeletedFeedsProcessingJobTelemetry
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class CollectionDeletedFeedsProcessingJobTelemetry : JobTelemetry
  {
    public CollectionDeletedFeedsProcessingJobTelemetry() => this.FeedsProcessed = (IList<Guid>) new List<Guid>();

    public int ProcessedFeedCount { get; set; }

    public long ProcessedChangeCount { get; set; }

    public bool MigrationStateDeletionEnabled { get; set; }

    public long? InitialContinuationToken { get; set; }

    public long? FinalContinuationToken { get; set; }

    public Guid CollectionId { get; set; }

    public int FeedCountBatchSize { get; set; }

    public TimeSpan FeedUpdateGracePeriod { get; set; }

    public DateTime JobStartTime { get; set; }

    public IList<Guid> FeedsProcessed { get; set; }

    internal int MaxFeedsToLog { get; set; } = 100;

    public void SetFeedsProcessed(Stack<Guid> feedIds) => this.FeedsProcessed = (IList<Guid>) feedIds.Take<Guid>(this.MaxFeedsToLog).ToList<Guid>();
  }
}
