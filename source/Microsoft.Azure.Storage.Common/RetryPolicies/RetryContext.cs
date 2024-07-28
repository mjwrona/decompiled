// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.RetryPolicies.RetryContext
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Storage.RetryPolicies
{
  public sealed class RetryContext
  {
    internal RetryContext(
      int currentRetryCount,
      RequestResult lastRequestResult,
      StorageLocation nextLocation,
      LocationMode locationMode)
    {
      this.CurrentRetryCount = currentRetryCount;
      this.LastRequestResult = lastRequestResult;
      this.NextLocation = nextLocation;
      this.LocationMode = locationMode;
    }

    public StorageLocation NextLocation { get; private set; }

    public LocationMode LocationMode { get; private set; }

    public int CurrentRetryCount { get; private set; }

    public RequestResult LastRequestResult { get; private set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0},{1})", (object) this.CurrentRetryCount, (object) this.LocationMode);
  }
}
