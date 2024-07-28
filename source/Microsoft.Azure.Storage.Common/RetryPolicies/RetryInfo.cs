// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.RetryPolicies.RetryInfo
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Globalization;

namespace Microsoft.Azure.Storage.RetryPolicies
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
      set => this.interval = CommonUtility.MaxTimeSpan(value, TimeSpan.Zero);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0},{1})", (object) this.TargetLocation, (object) this.RetryInterval);
  }
}
