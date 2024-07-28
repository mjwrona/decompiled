// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.TimeSpanExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class TimeSpanExtensions
  {
    public static TimeSpan RoundUpTimeSpanToHighestUnit(this TimeSpan timeSpanInput)
    {
      if (timeSpanInput.Days > 0)
      {
        int num = 86400000;
        return TimeSpan.FromDays((double) TimeSpan.FromMilliseconds(timeSpanInput.TotalMilliseconds + (double) num - 1.0).Days);
      }
      if (timeSpanInput.Hours > 0)
      {
        int num1 = 24;
        int num2 = 3600000;
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeSpanInput.TotalMilliseconds + (double) num2 - 1.0);
        return TimeSpan.FromHours((double) (timeSpan.Hours + timeSpan.Days * num1));
      }
      if (timeSpanInput.Minutes > 0)
      {
        int num3 = 60;
        int num4 = 60000;
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeSpanInput.TotalMilliseconds + (double) num4 - 1.0);
        return TimeSpan.FromMinutes((double) (timeSpan.Minutes + timeSpan.Hours * num3));
      }
      if (timeSpanInput.Seconds <= 0)
        return TimeSpan.FromMilliseconds(timeSpanInput.TotalMilliseconds);
      int num5 = 60;
      int num6 = 1000;
      TimeSpan timeSpan1 = TimeSpan.FromMilliseconds(timeSpanInput.TotalMilliseconds + (double) num6 - 1.0);
      return TimeSpan.FromSeconds((double) (timeSpan1.Seconds + timeSpan1.Minutes * num5));
    }
  }
}
