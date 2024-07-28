// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Channels.TimeSpanHelper
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Diagnostics;
using System;
using System.Globalization;

namespace Microsoft.Azure.NotificationHubs.Channels
{
  internal static class TimeSpanHelper
  {
    public static TimeSpan FromMinutes(int minutes, string configValue)
    {
      TimeSpan timeSpan = TimeSpan.FromTicks(600000000L * (long) minutes);
      DiagnosticUtility.DebugAssert(timeSpan == TimeSpan.Parse(configValue, (IFormatProvider) CultureInfo.InvariantCulture), "");
      return timeSpan;
    }

    public static TimeSpan FromSeconds(int seconds, string configValue)
    {
      TimeSpan timeSpan = TimeSpan.FromTicks(10000000L * (long) seconds);
      DiagnosticUtility.DebugAssert(timeSpan == TimeSpan.Parse(configValue, (IFormatProvider) CultureInfo.InvariantCulture), "");
      return timeSpan;
    }

    public static TimeSpan FromMilliseconds(int ms, string configValue)
    {
      TimeSpan timeSpan = TimeSpan.FromTicks(10000L * (long) ms);
      DiagnosticUtility.DebugAssert(timeSpan == TimeSpan.Parse(configValue, (IFormatProvider) CultureInfo.InvariantCulture), "");
      return timeSpan;
    }

    public static TimeSpan FromDays(int days, string configValue)
    {
      TimeSpan timeSpan = TimeSpan.FromTicks(864000000000L * (long) days);
      DiagnosticUtility.DebugAssert(timeSpan == TimeSpan.Parse(configValue, (IFormatProvider) CultureInfo.InvariantCulture), "");
      return timeSpan;
    }
  }
}
