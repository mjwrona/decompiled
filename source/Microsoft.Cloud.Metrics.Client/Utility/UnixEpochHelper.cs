// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.UnixEpochHelper
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  internal static class UnixEpochHelper
  {
    internal const long TicksPerMillisecond = 10000;
    internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
    private static readonly long UnixEpochMsEpochDeltaMillis = UnixEpochHelper.UnixEpoch.Ticks / 10000L;

    internal static long GetMillis(DateTime utcTime) => utcTime.Ticks / 10000L - UnixEpochHelper.UnixEpochMsEpochDeltaMillis;

    internal static DateTime FromMillis(long millis) => new DateTime(10000L * (millis + UnixEpochHelper.UnixEpochMsEpochDeltaMillis), DateTimeKind.Utc);
  }
}
