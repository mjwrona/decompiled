// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Period
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  [Flags]
  public enum Period
  {
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_NONE", false)] None = 0,
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_DAY", false)] Day = 1,
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_WEEK_ENDING_ON_SUNDAY", false)] WeekEndingOnSunday = 2,
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_WEEK_ENDING_ON_MONDAY", false)] WeekEndingOnMonday = 4,
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_WEEK_ENDING_ON_TUESDAY", false)] WeekEndingOnTuesday = 8,
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_WEEK_ENDING_ON_WEDNESDAY", false)] WeekEndingOnWednesday = 16, // 0x00000010
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_WEEK_ENDING_ON_THURSDAY", false)] WeekEndingOnThursday = 32, // 0x00000020
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_WEEK_ENDING_ON_FRIDAY", false)] WeekEndingOnFriday = 64, // 0x00000040
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_WEEK_ENDING_ON_SATURDAY", false)] WeekEndingOnSaturday = 128, // 0x00000080
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_MONTH", false)] Month = 256, // 0x00000100
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_QUARTER", false)] Quarter = 512, // 0x00000200
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_YEAR", false)] Year = 1024, // 0x00000400
    [LocalizedDisplayName("ENUM_TYPE_PERIOD_ALL", false)] All = Year | Quarter | Month | WeekEndingOnSaturday | WeekEndingOnFriday | WeekEndingOnThursday | WeekEndingOnWednesday | WeekEndingOnTuesday | WeekEndingOnMonday | WeekEndingOnSunday | Day, // 0x000007FF
  }
}
