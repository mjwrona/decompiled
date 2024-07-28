// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MetricsRowEntity
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class MetricsRowEntity
  {
    public string OperationType = "user;All";

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

    public long AnonymousSuccess { get; set; }

    public long AnonymousAuthorizationError { get; set; }

    public long AnonymousClientOtherError { get; set; }

    public long AnonymousClientTimeoutError { get; set; }

    public long AnonymousNetworkError { get; set; }

    public long AnonymousServerOtherError { get; set; }

    public long AnonymousServerTimeoutError { get; set; }

    public long AnonymousThrottlingError { get; set; }

    public long SasSuccess { get; set; }

    public long SasAuthorizationError { get; set; }

    public long SasClientOtherError { get; set; }

    public long SasClientTimeoutError { get; set; }

    public long SasNetworkError { get; set; }

    public long SasServerOtherError { get; set; }

    public long SasServerTimeoutError { get; set; }

    public long SasThrottlingError { get; set; }

    public double PercentSuccess => MetricsRowEntity.GetPercentage(this.Success, this.TotalRequests);

    public double PercentThrottlingError => MetricsRowEntity.GetPercentage(this.ThrottlingError, this.TotalRequests);

    public double PercentClientOtherError => MetricsRowEntity.GetPercentage(this.ClientOtherError, this.TotalRequests);

    public double PercentAuthorizationError => MetricsRowEntity.GetPercentage(this.AuthorizationError, this.TotalRequests);

    public double PercentTimeoutError => MetricsRowEntity.GetPercentage(this.ClientTimeoutError + this.ServerTimeoutError, this.TotalRequests);

    public double PercentNetworkError => MetricsRowEntity.GetPercentage(this.NetworkError, this.TotalRequests);

    public double PercentServerOtherError => MetricsRowEntity.GetPercentage(this.ServerOtherError, this.TotalRequests);

    public override string ToString() => string.Format("----- Timestamp: {0}\r\nIngress: {1}\r\nEgress: {2}\r\nTotalRequests: {3}\r\nSuccess: {4}\r\nThrottlingError: {5}\r\nClientOtherError: {6}\r\nAuthorizationError: {7}\r\nClientTimeoutError: {8}\r\nNetworkError: {9}\r\nServerOtherError: {10}\r\nServerTimeoutError: {11}\r\nPercentSuccess: {12}\r\nPercentThrottlingError: {13}\r\nPercentClientOtherError: {14}", (object) this.Timestamp, (object) this.Ingress, (object) this.Egress, (object) this.TotalRequests, (object) this.Success, (object) this.ThrottlingError, (object) this.ClientOtherError, (object) this.AuthorizationError, (object) this.ClientTimeoutError, (object) this.NetworkError, (object) this.ServerOtherError, (object) this.ServerTimeoutError, (object) this.PercentSuccess, (object) this.PercentThrottlingError, (object) this.PercentClientOtherError);

    private static double GetPercentage(long numerator, long denominator) => Math.Truncate((denominator == 0L ? 0.0 : (double) numerator / (double) denominator) * 100.0 * 1000000.0) / 1000000.0;
  }
}
