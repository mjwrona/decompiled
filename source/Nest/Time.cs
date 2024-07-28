// Decompiled with JetBrains decompiler
// Type: Nest.Time
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Nest
{
  [JsonFormatter(typeof (TimeFormatter))]
  public class Time : IComparable<Time>, IEquatable<Time>, IUrlParameter
  {
    private const double MicrosecondsInATick = 0.1;
    private const double MillisecondsInADay = 86400000.0;
    private const double MillisecondsInAMicrosecond = 0.001;
    private const double MillisecondsInAMillisecond = 1.0;
    private const double MillisecondsInAMinute = 60000.0;
    private const double MillisecondsInANanosecond = 1E-06;
    private const double MillisecondsInAnHour = 3600000.0;
    private const double MillisecondsInASecond = 1000.0;
    private const double NanosecondsInATick = 100.0;
    private static readonly Regex ExpressionRegex = new Regex("^\r\n\t\t\t\t(?<factor>[+\\-]? # open factor capture, allowing optional +- signs\r\n\t\t\t\t\t(?:(?#numeric)(?:\\d+(?:\\.\\d*)?)|(?:\\.\\d+)) #a numeric in the forms: (N, N., .N, N.N)\r\n\t\t\t\t\t(?:(?#exponent)e[+\\-]?\\d+)? #an optional exponential scientific component, E also matches here (IgnoreCase)\r\n\t\t\t\t) # numeric and exponent fall under the factor capture\r\n\t\t\t\t\\s{0,10} #optional spaces (sanity checked for max 10 repetitions)\r\n\t\t\t\t(?<interval>(?:d|h|m|s|ms|nanos|micros))? #optional interval indicator\r\n\t\t\t\t$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);
    private static readonly double FLOAT_TOLERANCE = 1E-07;

    private Time(int specialFactor, bool specialValue)
    {
      if (!specialValue)
        throw new ArgumentException("this constructor is only for static TimeValues");
      this.StaticTimeValue = new int?(specialFactor);
    }

    public Time(TimeSpan timeSpan)
      : this(timeSpan.TotalMilliseconds)
    {
    }

    public Time(double milliseconds)
    {
      if (Math.Abs(milliseconds - -1.0) < Time.FLOAT_TOLERANCE)
        this.StaticTimeValue = new int?(-1);
      else if (Math.Abs(milliseconds) < Time.FLOAT_TOLERANCE)
        this.StaticTimeValue = new int?(0);
      else
        this.Reduce(milliseconds);
    }

    public Time(double factor, TimeUnit interval)
    {
      this.Factor = new double?(factor);
      this.Interval = new TimeUnit?(interval);
      this.Milliseconds = new double?(Time.GetExactMilliseconds(this.Factor.Value, this.Interval.Value));
    }

    public Time(string timeUnit)
    {
      if (timeUnit.IsNullOrEmpty())
        throw new ArgumentException("Expression string is empty", nameof (timeUnit));
      if (timeUnit == "-1" || timeUnit == "0")
        this.StaticTimeValue = new int?(int.Parse(timeUnit));
      else
        this.ParseExpression(timeUnit);
    }

    public double? Factor { get; private set; }

    public TimeUnit? Interval { get; private set; }

    public double? Milliseconds { get; private set; }

    public static Time MinusOne { get; } = new Time(-1, true);

    public static Time Zero { get; } = new Time(0, true);

    private int? StaticTimeValue { get; }

    public static implicit operator Time(TimeSpan span) => new Time(span);

    public static implicit operator Time(double milliseconds) => new Time(milliseconds);

    public static implicit operator Time(string expression) => new Time(expression);

    private void ParseExpression(string timeUnit)
    {
      Match match = Time.ExpressionRegex.Match(timeUnit);
      if (!match.Success)
        throw new ArgumentException("Expression '" + timeUnit + "' is invalid", nameof (timeUnit));
      string s = match.Groups["factor"].Value;
      double result;
      if (!double.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        throw new ArgumentException("Expression '" + timeUnit + "' contains invalid factor: " + s, nameof (timeUnit));
      this.Factor = new double?(result);
      string str = match.Groups["interval"].Success ? match.Groups["interval"].Value : (string) null;
      this.Interval = str.ToEnum<TimeUnit>();
      if (!this.Interval.HasValue)
        throw new ArgumentException("Expression '" + str + "' cannot be parsed to an interval", nameof (timeUnit));
      this.Milliseconds = new double?(Time.GetExactMilliseconds(this.Factor.Value, this.Interval.Value));
    }

    public int CompareTo(Time other)
    {
      if (other == (Time) null)
        return 1;
      int? staticTimeValue = this.StaticTimeValue;
      if (staticTimeValue.HasValue)
      {
        staticTimeValue = other.StaticTimeValue;
        if (!staticTimeValue.HasValue)
          return -1;
      }
      staticTimeValue = this.StaticTimeValue;
      if (!staticTimeValue.HasValue)
      {
        staticTimeValue = other.StaticTimeValue;
        if (staticTimeValue.HasValue)
          return 1;
      }
      staticTimeValue = this.StaticTimeValue;
      if (staticTimeValue.HasValue)
      {
        staticTimeValue = other.StaticTimeValue;
        if (staticTimeValue.HasValue)
        {
          staticTimeValue = this.StaticTimeValue;
          int num1 = staticTimeValue.Value;
          staticTimeValue = other.StaticTimeValue;
          int num2 = staticTimeValue.Value;
          if (num1 == num2)
            return 0;
          staticTimeValue = this.StaticTimeValue;
          int num3 = staticTimeValue.Value;
          staticTimeValue = other.StaticTimeValue;
          int num4 = staticTimeValue.Value;
          return num3 < num4 ? -1 : 1;
        }
      }
      double? milliseconds = this.Milliseconds;
      if (!milliseconds.HasValue)
      {
        milliseconds = other.Milliseconds;
        if (!milliseconds.HasValue)
          return 0;
      }
      milliseconds = this.Milliseconds;
      if (milliseconds.HasValue)
      {
        milliseconds = other.Milliseconds;
        if (!milliseconds.HasValue)
          return 1;
      }
      milliseconds = this.Milliseconds;
      if (milliseconds.HasValue)
      {
        milliseconds = other.Milliseconds;
        if (milliseconds.HasValue)
        {
          milliseconds = this.Milliseconds;
          double num5 = milliseconds.Value;
          milliseconds = other.Milliseconds;
          double num6 = milliseconds.Value;
          if (Math.Abs(num5 - num6) < Time.FLOAT_TOLERANCE)
            return 0;
          milliseconds = other.Milliseconds;
          if (milliseconds.HasValue)
          {
            milliseconds = this.Milliseconds;
            double num7 = other.Milliseconds.Value;
            if (milliseconds.GetValueOrDefault() < num7 & milliseconds.HasValue)
              return -1;
          }
          return 1;
        }
      }
      return 1;
    }

    private static bool IsIntegerGreaterThanZero(double d) => Math.Abs(d % 1.0) < double.Epsilon;

    public static bool operator <(Time left, Time right) => left.CompareTo(right) < 0;

    public static bool operator <=(Time left, Time right) => left.CompareTo(right) < 0 || left.Equals(right);

    public static bool operator >(Time left, Time right) => left.CompareTo(right) > 0;

    public static bool operator >=(Time left, Time right) => left.CompareTo(right) > 0 || left.Equals(right);

    public static bool operator ==(Time left, Time right) => (object) left != null ? left.Equals(right) : (object) right == null;

    public static bool operator !=(Time left, Time right) => !(left == right);

    public TimeSpan ToTimeSpan()
    {
      if (this.StaticTimeValue.HasValue)
        throw new InvalidOperationException("Static time values like -1 or 0 have no logical TimeSpan representation");
      if (!this.Interval.HasValue)
        throw new InvalidOperationException("Time has no value for Interval so you can not call ToTimeSpan on it");
      switch (this.Interval.Value)
      {
        case TimeUnit.Nanoseconds:
          return this.Factor.HasValue ? TimeSpan.FromTicks((long) (this.Factor.Value / 100.0)) : throw new InvalidOperationException("Time is in nanoseconds but factor has no value, this is a bug please report!");
        case TimeUnit.Microseconds:
          return this.Factor.HasValue ? TimeSpan.FromTicks((long) (this.Factor.Value / 0.1)) : throw new InvalidOperationException("Time is in microseconds but factor has no value, this is a bug please report!");
        default:
          return this.Milliseconds.HasValue ? TimeSpan.FromMilliseconds(this.Milliseconds.Value) : throw new InvalidOperationException("Milliseconds is null so we have nothing to create a TimeSpan from, this is a bug please report!");
      }
    }

    public override string ToString()
    {
      if (this.StaticTimeValue.HasValue)
        return this.StaticTimeValue.Value.ToString();
      if (!this.Factor.HasValue)
        return "<bad Time object should not happen>";
      string str = this.Factor.Value.ToString("0." + Time.ExponentFormat(this.Factor.Value), (IFormatProvider) CultureInfo.InvariantCulture);
      return !this.Interval.HasValue ? str : str + this.Interval.Value.GetStringValue();
    }

    string IUrlParameter.GetString(IConnectionConfigurationValues settings)
    {
      if (this == Time.MinusOne)
        return "-1";
      if (this == Time.Zero)
        return "0";
      double? nullable = this.Factor;
      if (nullable.HasValue && this.Interval.HasValue)
        return this.ToString();
      nullable = this.Milliseconds;
      return nullable.ToString();
    }

    public bool Equals(Time other)
    {
      if ((object) other == null)
        return false;
      if ((object) this == (object) other)
        return true;
      int? staticTimeValue1 = this.StaticTimeValue;
      if (staticTimeValue1.HasValue)
      {
        staticTimeValue1 = other.StaticTimeValue;
        if (!staticTimeValue1.HasValue)
          return false;
      }
      staticTimeValue1 = this.StaticTimeValue;
      if (!staticTimeValue1.HasValue)
      {
        staticTimeValue1 = other.StaticTimeValue;
        if (staticTimeValue1.HasValue)
          return false;
      }
      staticTimeValue1 = this.StaticTimeValue;
      if (staticTimeValue1.HasValue)
      {
        staticTimeValue1 = other.StaticTimeValue;
        if (staticTimeValue1.HasValue)
        {
          staticTimeValue1 = this.StaticTimeValue;
          int? staticTimeValue2 = other.StaticTimeValue;
          return staticTimeValue1.GetValueOrDefault() == staticTimeValue2.GetValueOrDefault() & staticTimeValue1.HasValue == staticTimeValue2.HasValue;
        }
      }
      double? milliseconds = this.Milliseconds;
      if (!milliseconds.HasValue)
      {
        milliseconds = other.Milliseconds;
        if (!milliseconds.HasValue)
          return true;
      }
      milliseconds = this.Milliseconds;
      if (milliseconds.HasValue)
      {
        milliseconds = other.Milliseconds;
        if (milliseconds.HasValue)
        {
          milliseconds = this.Milliseconds;
          double num1 = milliseconds.Value;
          milliseconds = other.Milliseconds;
          double num2 = milliseconds.Value;
          return Math.Abs(num1 - num2) < Time.FLOAT_TOLERANCE;
        }
      }
      return false;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((Time) obj);
    }

    public override int GetHashCode() => !this.StaticTimeValue.HasValue ? this.Milliseconds.GetHashCode() : this.StaticTimeValue.Value.GetHashCode();

    private void Reduce(double ms)
    {
      this.Milliseconds = new double?(ms);
      if (ms >= 86400000.0)
      {
        double d = ms / 86400000.0;
        if (Time.IsIntegerGreaterThanZero(d))
        {
          this.Factor = new double?(d);
          this.Interval = new TimeUnit?(TimeUnit.Day);
          return;
        }
      }
      if (ms >= 3600000.0)
      {
        double d = ms / 3600000.0;
        if (Time.IsIntegerGreaterThanZero(d))
        {
          this.Factor = new double?(d);
          this.Interval = new TimeUnit?(TimeUnit.Hour);
          return;
        }
      }
      if (ms >= 60000.0)
      {
        double d = ms / 60000.0;
        if (Time.IsIntegerGreaterThanZero(d))
        {
          this.Factor = new double?(d);
          this.Interval = new TimeUnit?(TimeUnit.Minute);
          return;
        }
      }
      if (ms >= 1000.0)
      {
        double d = ms / 1000.0;
        if (Time.IsIntegerGreaterThanZero(d))
        {
          this.Factor = new double?(d);
          this.Interval = new TimeUnit?(TimeUnit.Second);
          return;
        }
      }
      if (Time.IsIntegerGreaterThanZero(ms))
      {
        this.Factor = new double?(ms);
        this.Interval = new TimeUnit?(TimeUnit.Millisecond);
      }
      else
      {
        double d = ms / 0.001;
        if (Time.IsIntegerGreaterThanZero(d))
        {
          this.Factor = new double?(d);
          this.Interval = new TimeUnit?(TimeUnit.Microseconds);
        }
        else
        {
          this.Factor = new double?(ms / 1E-06);
          this.Interval = new TimeUnit?(TimeUnit.Nanoseconds);
        }
      }
    }

    private static double GetExactMilliseconds(double factor, TimeUnit interval)
    {
      switch (interval)
      {
        case TimeUnit.Nanoseconds:
          return factor * 1E-06;
        case TimeUnit.Microseconds:
          return factor * 0.001;
        case TimeUnit.Millisecond:
          return factor;
        case TimeUnit.Second:
          return factor * 1000.0;
        case TimeUnit.Minute:
          return factor * 60000.0;
        case TimeUnit.Hour:
          return factor * 3600000.0;
        case TimeUnit.Day:
          return factor * 86400000.0;
        default:
          throw new ArgumentOutOfRangeException(nameof (interval), (object) interval, (string) null);
      }
    }

    private static string ExponentFormat(double d) => new string('#', Math.Max(2, (int) (BitConverter.DoubleToInt64Bits(d) >> 52 & 2047L)));
  }
}
