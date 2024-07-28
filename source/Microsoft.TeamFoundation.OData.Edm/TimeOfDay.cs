// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.TimeOfDay
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Globalization;

namespace Microsoft.OData.Edm
{
  public struct TimeOfDay : IComparable, IComparable<TimeOfDay>, IEquatable<TimeOfDay>
  {
    public const long MaxTickValue = 863999999999;
    public const long MinTickValue = 0;
    public const long TicksPerHour = 36000000000;
    public const long TicksPerMinute = 600000000;
    public const long TicksPerSecond = 10000000;
    public static readonly TimeOfDay MinValue = new TimeOfDay(0L);
    public static readonly TimeOfDay MaxValue = new TimeOfDay(863999999999L);
    private TimeSpan timeSpan;

    public TimeOfDay(int hour, int minute, int second, int millisecond)
      : this()
    {
      if (hour < 0 || hour > 23 || minute < 0 || minute > 59 || second < 0 || second > 59 || millisecond < 0 || millisecond > 999)
        throw new FormatException(Strings.TimeOfDay_InvalidTimeOfDayParameters((object) hour, (object) minute, (object) second, (object) millisecond));
      this.timeSpan = new TimeSpan(0, hour, minute, second, millisecond);
    }

    public TimeOfDay(long ticks)
      : this()
    {
      this.timeSpan = ticks >= 0L && ticks <= 863999999999L ? new TimeSpan(ticks) : throw new FormatException(Strings.TimeOfDay_TicksOutOfRange((object) ticks));
    }

    public static TimeOfDay Now => (TimeOfDay) DateTime.Now.TimeOfDay;

    public int Hours => this.timeSpan.Hours;

    public int Minutes => this.timeSpan.Minutes;

    public int Seconds => this.timeSpan.Seconds;

    public long Milliseconds => (long) this.timeSpan.Milliseconds;

    public long Ticks => this.timeSpan.Ticks;

    public static bool operator !=(TimeOfDay firstOperand, TimeOfDay secondOperand) => firstOperand.timeSpan != secondOperand.timeSpan;

    public static bool operator ==(TimeOfDay firstOperand, TimeOfDay secondOperand) => firstOperand.timeSpan == secondOperand.timeSpan;

    public static bool operator >=(TimeOfDay firstOperand, TimeOfDay secondOperand) => firstOperand.timeSpan >= secondOperand.timeSpan;

    public static bool operator >(TimeOfDay firstOperand, TimeOfDay secondOperand) => firstOperand.timeSpan > secondOperand.timeSpan;

    public static bool operator <=(TimeOfDay firstOperand, TimeOfDay secondOperand) => firstOperand.timeSpan <= secondOperand.timeSpan;

    public static bool operator <(TimeOfDay firstOperand, TimeOfDay secondOperand) => firstOperand.timeSpan < secondOperand.timeSpan;

    public static implicit operator TimeSpan(TimeOfDay time) => time.timeSpan;

    public static implicit operator TimeOfDay(TimeSpan timeSpan)
    {
      if (timeSpan.Days != 0 || timeSpan.Hours < 0 || timeSpan.Minutes < 0 || timeSpan.Milliseconds < 0 || timeSpan.Ticks < 0L || timeSpan.Ticks > 863999999999L)
        throw new FormatException(Strings.TimeOfDay_ConvertErrorFromTimeSpan((object) timeSpan));
      return new TimeOfDay(timeSpan.Ticks);
    }

    public bool Equals(TimeOfDay other) => this.timeSpan.Equals(other.timeSpan);

    public override bool Equals(object obj) => obj != null && obj is TimeOfDay timeOfDay && this.timeSpan.Equals(timeOfDay.timeSpan);

    public override int GetHashCode() => this.timeSpan.GetHashCode();

    public override string ToString() => this.timeSpan.ToString("hh\\:mm\\:ss\\.fffffff", (IFormatProvider) CultureInfo.InvariantCulture);

    public int CompareTo(object obj)
    {
      if (obj is TimeOfDay other)
        return this.CompareTo(other);
      throw new ArgumentException(Strings.TimeOfDay_InvalidCompareToTarget(obj));
    }

    public int CompareTo(TimeOfDay other) => this.timeSpan.CompareTo(other.timeSpan);

    public static TimeOfDay Parse(string text) => TimeOfDay.Parse(text, (IFormatProvider) CultureInfo.CurrentCulture);

    public static TimeOfDay Parse(string text, IFormatProvider provider)
    {
      TimeOfDay result;
      if (TimeOfDay.TryParse(text, provider, out result))
        return result;
      throw new FormatException(Strings.TimeOfDay_InvalidParsingString((object) text));
    }

    public static bool TryParse(string text, out TimeOfDay result) => TimeOfDay.TryParse(text, (IFormatProvider) CultureInfo.CurrentCulture, out result);

    public static bool TryParse(string text, IFormatProvider provider, out TimeOfDay result)
    {
      TimeSpan result1;
      if (TimeSpan.TryParse(text, provider, out result1) && result1.Ticks >= 0L && result1.Ticks <= 863999999999L)
      {
        result = new TimeOfDay(result1.Ticks);
        return true;
      }
      result = TimeOfDay.MinValue;
      return false;
    }
  }
}
