// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.TimeSeriesErrorCode
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

namespace Microsoft.Cloud.Metrics.Client
{
  public enum TimeSeriesErrorCode
  {
    InternalError = -64, // 0xFFFFFFC0
    BadRequest = -63, // 0xFFFFFFC1
    TimeOut = -62, // 0xFFFFFFC2
    Throttled = -4, // 0xFFFFFFFC
    RequestEntityTooLarge = -3, // 0xFFFFFFFD
    InvalidSamplingType = -2, // 0xFFFFFFFE
    InvalidSeries = -1, // 0xFFFFFFFF
    Success = 0,
  }
}
