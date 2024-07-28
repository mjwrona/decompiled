// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DateTimeHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class DateTimeHelper
  {
    public static string DisplayDateTimeFormat = "ddd, MMM dd yyyy HH':'mm':'ss 'GMT'zzz";

    public static string GetDurationText(DateTime from, DateTime to) => DateTimeHelper.GetDurationText(to - from);

    public static string GetDurationText(TimeSpan timeSpan)
    {
      if (timeSpan.TotalMinutes < 2.0)
        return BuildServerResources.DurationSeconds((object) Math.Round(timeSpan.TotalSeconds));
      if (timeSpan.TotalHours < 2.0)
        return BuildServerResources.DurationMinutes((object) Math.Round(timeSpan.TotalMinutes));
      return timeSpan.TotalDays < 2.0 ? BuildServerResources.DurationHours((object) Math.Round(timeSpan.TotalHours)) : BuildServerResources.DurationDays((object) Math.Round(timeSpan.TotalDays));
    }
  }
}
