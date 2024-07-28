// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.BlobRepairStats
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class BlobRepairStats
  {
    private long m_containers;
    private long m_blobs;
    private long m_blobsMissingOnSecondary;
    private long m_blobsMissingOnPrimary;
    private long m_inconsistentBlobs;
    private long m_successfulRepairs;
    private long m_failedRepairs;
    private readonly DateTime m_startTime;

    public BlobRepairStats() => this.m_startTime = DateTime.UtcNow;

    public long ProcessedContainers => Interlocked.Read(ref this.m_containers);

    public long ProcessedBlobs => Interlocked.Read(ref this.m_blobs);

    public long BlobsMissingOnSecondary => Interlocked.Read(ref this.m_blobsMissingOnSecondary);

    public long BlobsMissingOnPrimary => Interlocked.Read(ref this.m_blobsMissingOnPrimary);

    public long InconsistentBlobs => Interlocked.Read(ref this.m_inconsistentBlobs);

    public long SuccessfulRepairs => Interlocked.Read(ref this.m_successfulRepairs);

    public long FailedRepairs => Interlocked.Read(ref this.m_failedRepairs);

    public TimeSpan ElapsedTime => DateTime.UtcNow - this.m_startTime;

    public double AverageBlobsPerSecond => (double) this.ProcessedBlobs / this.ElapsedTime.TotalSeconds;

    public void IncrementProcessedContainers() => Interlocked.Increment(ref this.m_containers);

    public void IncrementProcessedBlobs() => Interlocked.Increment(ref this.m_blobs);

    public void IncrementBlobsMissingOnSecondary() => Interlocked.Increment(ref this.m_blobsMissingOnSecondary);

    public void IncrementBlobsMissingOnPrimary() => Interlocked.Increment(ref this.m_blobsMissingOnPrimary);

    public void IncrementInconsistentBlobs() => Interlocked.Increment(ref this.m_inconsistentBlobs);

    public void IncrementSuccessfulRepairs() => Interlocked.Increment(ref this.m_successfulRepairs);

    public void IncrementFailedRepairs() => Interlocked.Increment(ref this.m_failedRepairs);

    public override string ToString() => string.Format("{0}: Containers: {1}, Blobs: {2}, MissingFromSource: {3}, MissingFromTarget: {4}, Inconsistent: {5}, SuccessfulRepairs: {6}, FailedRepairs: {7}, AvgBlobsPerSec: {8:#.00}, ElapsedTime: {9}", (object) nameof (BlobRepairStats), (object) this.ProcessedContainers, (object) this.ProcessedBlobs, (object) this.BlobsMissingOnPrimary, (object) this.BlobsMissingOnSecondary, (object) this.InconsistentBlobs, (object) this.SuccessfulRepairs, (object) this.FailedRepairs, (object) this.AverageBlobsPerSecond, (object) this.ElapsedTime);
  }
}
