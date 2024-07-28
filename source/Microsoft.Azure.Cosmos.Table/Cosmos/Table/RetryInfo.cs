// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RetryInfo
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Utils;
using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class RetryInfo
  {
    private TimeSpan interval = TimeSpan.FromSeconds(3.0);

    public RetryInfo()
    {
      this.TargetLocation = StorageLocation.Primary;
      this.UpdatedLocationMode = LocationMode.PrimaryOnly;
    }

    public RetryInfo(RetryContext retryContext)
    {
      CommonUtility.AssertNotNull(nameof (retryContext), (object) retryContext);
      this.TargetLocation = retryContext.NextLocation;
      this.UpdatedLocationMode = retryContext.LocationMode;
    }

    public StorageLocation TargetLocation { get; set; }

    public LocationMode UpdatedLocationMode { get; set; }

    public TimeSpan RetryInterval
    {
      get => this.interval;
      set => this.interval = RestUtility.MaxTimeSpan(value, TimeSpan.Zero);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0},{1})", new object[2]
    {
      (object) this.TargetLocation,
      (object) this.RetryInterval
    });
  }
}
