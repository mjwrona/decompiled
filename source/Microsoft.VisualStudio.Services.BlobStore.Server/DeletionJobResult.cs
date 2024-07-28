// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DeletionJobResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Common;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [JsonObject(MemberSerialization.OptIn)]
  [Serializable]
  public sealed class DeletionJobResult
  {
    private long blobsDeleted;
    private long blobDeletedSize;
    private long blobsKept;
    private long failures;
    private double? maxExpiredDays;
    private double? minExpiredDays;
    private string blobFailureMessage;
    public BlobIdentifier CurrentBlobId;
    public long NumConcurrentDeletes;
    public Exception Exception;
    private readonly object maxExpireLock = new object();
    private readonly object minExpireLock = new object();

    [JsonProperty]
    public long BlobsDeleted
    {
      get => this.blobsDeleted;
      private set => this.blobsDeleted = value;
    }

    [JsonProperty]
    public long BlobsDeletedSize
    {
      get => this.blobDeletedSize;
      private set => this.blobDeletedSize = value;
    }

    [JsonProperty]
    public long BlobsKept
    {
      get => this.blobsKept;
      private set => this.blobsKept = value;
    }

    [JsonProperty]
    public long Failures
    {
      get => this.failures;
      private set => this.failures = value;
    }

    [JsonProperty]
    public double? MaxExpiredDays
    {
      get => this.maxExpiredDays;
      private set => this.maxExpiredDays = value;
    }

    [JsonProperty]
    public double? MinExpiredDays
    {
      get => this.minExpiredDays;
      private set => this.minExpiredDays = value;
    }

    [JsonProperty]
    public LogHistogram ExpirationDaysLog2Histogram { get; } = new LogHistogram(2.0, 365000.0);

    [JsonProperty]
    public int JobCompletePerMille { get; set; }

    [JsonProperty]
    public bool IsResumedFromCheckpoint { get; set; }

    [JsonProperty]
    public string StartBlobId { get; set; }

    [JsonProperty]
    public string EndBlobId { get; set; }

    [JsonProperty]
    public long AvgNumConcurrentDeletes { get; set; }

    [JsonProperty]
    public int NumJobRetried { get; set; }

    [JsonProperty]
    public int NumJobThrottled { get; set; }

    [JsonProperty]
    public int CpuThreshold { get; set; }

    [JsonProperty]
    public int PartitionId { get; set; }

    [JsonProperty]
    public int TotalPartitions { get; set; }

    [JsonProperty(PropertyName = "Exception")]
    public string ExceptionMessage => JobHelper.GetNestedExceptionMessage(this.Exception);

    [JsonProperty]
    public string BlobFailureMessage
    {
      get => this.blobFailureMessage;
      private set => this.blobFailureMessage = value;
    }

    public override string ToString() => Microsoft.VisualStudio.Services.Content.Common.JsonSerializer.Serialize<DeletionJobResult>(this);

    public void LogDeletion(long blobLength)
    {
      Interlocked.Increment(ref this.blobsDeleted);
      Interlocked.Add(ref this.blobDeletedSize, blobLength);
    }

    public void LogKept() => Interlocked.Increment(ref this.blobsKept);

    public void LogFailure(string msg)
    {
      Interlocked.CompareExchange<string>(ref this.blobFailureMessage, msg, (string) null);
      Interlocked.Increment(ref this.failures);
    }

    public void LogExpiredDeletion(IClock clock, DateTime expiration)
    {
      double totalDays = clock.Now.UtcDateTime.Subtract(expiration).TotalDays;
      DeletionJobResult.AssignIf(this.maxExpireLock, (Func<double, double, bool>) ((x, y) => x < y), ref this.maxExpiredDays, new double?(totalDays));
      DeletionJobResult.AssignIf(this.minExpireLock, (Func<double, double, bool>) ((x, y) => x > y), ref this.minExpiredDays, new double?(totalDays));
      this.ExpirationDaysLog2Histogram.IncrementCount(totalDays);
    }

    private static void AssignIf(
      object lockObject,
      Func<double, double, bool> pred,
      ref double? a,
      double? b)
    {
      if (!a.HasValue)
      {
        lock (lockObject)
        {
          if (!a.HasValue)
          {
            a = b;
            return;
          }
        }
      }
      if (!b.HasValue || !pred(a.Value, b.Value))
        return;
      lock (lockObject)
      {
        if (!b.HasValue || !pred(a.Value, b.Value))
          return;
        a = new double?(b.Value);
      }
    }
  }
}
