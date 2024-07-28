// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.AzureMonitorStorageMetrics
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class AzureMonitorStorageMetrics
  {
    public DateTimeOffset Timestamp { get; set; }

    public long Ingress { get; set; }

    public long Egress { get; set; }

    public long TotalRequests { get; set; }

    public long Availability { get; set; }

    public long AverageE2ELatency { get; set; }

    public long AverageServerLatency { get; set; }

    public long Success { get; set; }

    public long ThrottlingError { get; set; }

    public long ClientOtherError { get; set; }

    public long AuthorizationError { get; set; }

    public long ClientTimeoutError { get; set; }

    public long NetworkError { get; set; }

    public long ServerOtherError { get; set; }

    public long ServerTimeoutError { get; set; }

    public double PercentSuccess => AzureMonitorStorageMetrics.GetPercentage(this.Success, this.TotalRequests);

    public double PercentThrottlingError => AzureMonitorStorageMetrics.GetPercentage(this.ThrottlingError, this.TotalRequests);

    public double PercentClientOtherError => AzureMonitorStorageMetrics.GetPercentage(this.ClientOtherError, this.TotalRequests);

    public double PercentAuthorizationError => AzureMonitorStorageMetrics.GetPercentage(this.AuthorizationError, this.TotalRequests);

    public double PercentTimeoutError => AzureMonitorStorageMetrics.GetPercentage(this.ClientTimeoutError + this.ServerTimeoutError, this.TotalRequests);

    public double PercentNetworkError => AzureMonitorStorageMetrics.GetPercentage(this.NetworkError, this.TotalRequests);

    public double PercentServerOtherError => AzureMonitorStorageMetrics.GetPercentage(this.ServerOtherError, this.TotalRequests);

    public string OperationType { get; set; }

    public override string ToString() => string.Format("----- Timestamp: {0}\r\nIngress: {1}\r\nEgress: {2}\r\nTotalRequests: {3}\r\nSuccess: {4}\r\nThrottlingError: {5}\r\nClientOtherError: {6}\r\nAuthorizationError: {7}\r\nClientTimeoutError: {8}\r\nNetworkError: {9}\r\nServerOtherError: {10}\r\nServerTimeoutError: {11}\r\nPercentSuccess: {12}\r\nPercentThrottlingError: {13}\r\nPercentClientOtherError: {14}", (object) this.Timestamp, (object) this.Ingress, (object) this.Egress, (object) this.TotalRequests, (object) this.Success, (object) this.ThrottlingError, (object) this.ClientOtherError, (object) this.AuthorizationError, (object) this.ClientTimeoutError, (object) this.NetworkError, (object) this.ServerOtherError, (object) this.ServerTimeoutError, (object) this.PercentSuccess, (object) this.PercentThrottlingError, (object) this.PercentClientOtherError);

    private static double GetPercentage(long numerator, long denominator) => Math.Truncate((denominator == 0L ? 0.0 : (double) numerator / (double) denominator) * 100.0 * 1000000.0) / 1000000.0;

    public void LogToTracer(
      TeamFoundationTracingService tracingService,
      Guid executionId,
      string storageAccountName,
      string storageCluster)
    {
      tracingService.TraceStorageMetricsTransactions(executionId, storageAccountName, "", this.Timestamp.UtcDateTime, this.Ingress, this.Egress, this.TotalRequests, this.TotalRequests, (double) this.Availability, (double) this.AverageE2ELatency, (double) this.AverageServerLatency, this.PercentSuccess, this.PercentThrottlingError, this.PercentTimeoutError, this.PercentServerOtherError, this.PercentClientOtherError, this.PercentAuthorizationError, this.PercentNetworkError, this.Success, 0L, 0L, this.ThrottlingError, 0L, 0L, this.ClientTimeoutError, 0L, 0L, this.ServerTimeoutError, 0L, 0L, this.ClientOtherError, 0L, 0L, this.ServerOtherError, 0L, 0L, this.AuthorizationError, 0L, 0L, this.NetworkError, 0L, 0L, this.OperationType, storageCluster, "", "", DateTime.MaxValue, DateTime.MaxValue, DateTime.MaxValue);
    }
  }
}
