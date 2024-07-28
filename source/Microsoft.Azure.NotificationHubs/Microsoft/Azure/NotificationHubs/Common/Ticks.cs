// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Ticks
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common.Interop;
using System;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal static class Ticks
  {
    public static long Now
    {
      get
      {
        long time;
        UnsafeNativeMethods.GetSystemTimeAsFileTime(out time);
        return time;
      }
    }

    public static long FromMilliseconds(int milliseconds) => checked ((long) milliseconds * 10000L);

    public static int ToMilliseconds(long ticks) => checked ((int) unchecked (ticks / 10000L));

    public static long FromTimeSpan(TimeSpan duration) => duration.Ticks;

    public static TimeSpan ToTimeSpan(long ticks) => new TimeSpan(ticks);

    public static long Add(long firstTicks, long secondTicks)
    {
      if (firstTicks == long.MaxValue || firstTicks == long.MinValue)
        return firstTicks;
      if (secondTicks == long.MaxValue || secondTicks == long.MinValue)
        return secondTicks;
      if (firstTicks >= 0L && long.MaxValue - firstTicks <= secondTicks)
        return 9223372036854775806;
      return firstTicks <= 0L && long.MinValue - firstTicks >= secondTicks ? -9223372036854775807L : checked (firstTicks + secondTicks);
    }
  }
}
