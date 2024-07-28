// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Analytics.MetricsEntity
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Storage.Analytics
{
  public class MetricsEntity : TableEntity
  {
    public DateTimeOffset Time => DateTimeOffset.ParseExact(this.PartitionKey, "yyyyMMdd'T'HHmm", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

    public string AccessType
    {
      get
      {
        ArgumentUtility.CheckStringForNullOrEmpty(this.RowKey, "RowKey");
        string stringVar = ((IEnumerable<string>) this.RowKey.Split(';')).ElementAtOrDefault<string>(0);
        ArgumentUtility.CheckStringForNullOrEmpty(stringVar, nameof (AccessType));
        return stringVar;
      }
    }

    public string TransactionType
    {
      get
      {
        ArgumentUtility.CheckStringForNullOrEmpty(this.RowKey, "RowKey");
        string stringVar = ((IEnumerable<string>) this.RowKey.Split(';')).ElementAtOrDefault<string>(1);
        ArgumentUtility.CheckStringForNullOrEmpty(stringVar, nameof (TransactionType));
        return stringVar;
      }
    }

    public long TotalIngress { get; set; }

    public long TotalEgress { get; set; }

    public long TotalRequests { get; set; }

    public long TotalBillableRequests { get; set; }

    public double Availability { get; set; }

    public double AverageE2ELatency { get; set; }

    public double AverageServerLatency { get; set; }

    public double PercentSuccess { get; set; }

    public double PercentThrottlingError { get; set; }

    public double PercentTimeoutError { get; set; }

    public double PercentServerOtherError { get; set; }

    public double PercentClientOtherError { get; set; }

    public double PercentAuthorizationError { get; set; }

    public double PercentNetworkError { get; set; }

    public long Success { get; set; }

    public long AnonymousSuccess { get; set; }

    public long SASSuccess { get; set; }

    public long ThrottlingError { get; set; }

    public long AnonymousThrottlingError { get; set; }

    public long SASThrottlingError { get; set; }

    public long ClientTimeoutError { get; set; }

    public long AnonymousClientTimeoutError { get; set; }

    public long SASClientTimeoutError { get; set; }

    public long ServerTimeoutError { get; set; }

    public long AnonymousServerTimeoutError { get; set; }

    public long SASServerTimeoutError { get; set; }

    public long ClientOtherError { get; set; }

    public long SASClientOtherError { get; set; }

    public long AnonymousClientOtherError { get; set; }

    public long ServerOtherError { get; set; }

    public long AnonymousServerOtherError { get; set; }

    public long SASServerOtherError { get; set; }

    public long AuthorizationError { get; set; }

    public long AnonymousAuthorizationError { get; set; }

    public long SASAuthorizationError { get; set; }

    public long NetworkError { get; set; }

    public long AnonymousNetworkError { get; set; }

    public long SASNetworkError { get; set; }
  }
}
