// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Utilities.UnixTimeExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace HdrHistogram.Utilities
{
  internal static class UnixTimeExtensions
  {
    private const long EpochInTicks = 621355968000000000;

    public static double SecondsSinceUnixEpoch(this DateTime source)
    {
      if (source.Kind == DateTimeKind.Unspecified)
        throw new ArgumentException("DateTime must have kind specified.");
      return (double) (source.ToUniversalTime().Ticks - 621355968000000000L) / 10000000.0;
    }

    public static long MillisecondsSinceUnixEpoch(this DateTime source)
    {
      if (source.Kind == DateTimeKind.Unspecified)
        throw new ArgumentException("DateTime must have kind specified.");
      return (source.ToUniversalTime().Ticks - 621355968000000000L) / 10000L;
    }

    public static DateTime ToDateFromSecondsSinceEpoch(this double secondsSinceUnixEpoch) => new DateTime(621355968000000000L + (long) (secondsSinceUnixEpoch * 10000000.0), DateTimeKind.Utc);

    public static DateTime ToDateFromMillisecondsSinceEpoch(this long millisecondsSinceUnixEpoch) => new DateTime(621355968000000000L + millisecondsSinceUnixEpoch * 10000L, DateTimeKind.Utc);
  }
}
