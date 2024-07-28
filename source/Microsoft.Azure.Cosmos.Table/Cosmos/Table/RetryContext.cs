// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RetryContext
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Table
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

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0},{1})", new object[2]
    {
      (object) this.CurrentRetryCount,
      (object) this.LocationMode
    });
  }
}
