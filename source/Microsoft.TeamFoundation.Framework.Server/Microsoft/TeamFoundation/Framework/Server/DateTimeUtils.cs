// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DateTimeUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlTypes;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class DateTimeUtils
  {
    public static DateTime SqlDateMaxValue = (DateTime) SqlDateTime.MaxValue;
    public static DateTime SqlDateMinValue = (DateTime) SqlDateTime.MinValue;
    private const long TicksPerMillisecond = 10000;
    private const long TicksPerSecond = 10000000;
    private const long TicksPerMinute = 600000000;
    private const long TicksPerHour = 36000000000;
    private const long TicksPerDay = 864000000000;
    private const int MillisPerSecond = 1000;
    private const int MillisPerMinute = 60000;
    private const int MillisPerHour = 3600000;
    private const int MillisPerDay = 86400000;
    private const int DaysPerYear = 365;
    private const int DaysPer4Years = 1461;
    private const int DaysPer100Years = 36524;
    private const int DaysPer400Years = 146097;
    private const int DaysTo1601 = 584388;
    private const int DaysTo1899 = 693593;
    internal const int DaysTo1970 = 719162;
    private const int DaysTo10000 = 3652059;
    internal const long MinTicks = 0;
    internal const long MaxTicks = 3155378975999999999;
    private const long MaxMillis = 315537897600000;

    public static DateTime Max(DateTime dt1, DateTime dt2) => !(dt1 > dt2) ? dt2 : dt1;

    public static DateTime Min(DateTime dt1, DateTime dt2) => !(dt1 < dt2) ? dt2 : dt1;

    public static DateTime ConstrainToSql(this DateTime dt) => DateTimeUtils.Max(DateTimeUtils.Min(dt, DateTimeUtils.SqlDateMaxValue), DateTimeUtils.SqlDateMinValue);

    public static DateTime AddTimeSpan(this DateTime dt, TimeSpan ts, bool useSqlConstraints = false)
    {
      long ticks1 = ts.Ticks;
      long ticks2 = dt.Ticks;
      dt = ticks1 <= 3155378975999999999L - ticks2 ? (ticks1 >= -ticks2 ? dt.Add(ts) : DateTime.MinValue) : DateTime.MaxValue;
      return !useSqlConstraints ? dt : dt.ConstrainToSql();
    }
  }
}
