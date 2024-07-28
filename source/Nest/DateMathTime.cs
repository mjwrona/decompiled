// Decompiled with JetBrains decompiler
// Type: Nest.DateMathTime
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Nest
{
  [JsonFormatter(typeof (DateMathTimeFormatter))]
  public class DateMathTime : IComparable<DateMathTime>, IEquatable<DateMathTime>
  {
    private const double MillisecondsInADay = 86400000.0;
    private const double MillisecondsInAMinute = 60000.0;
    private const double MillisecondsInAMonthApproximate = 2628000000.0;
    private const double MillisecondsInAnHour = 3600000.0;
    private const double MillisecondsInASecond = 1000.0;
    private const double MillisecondsInAWeek = 604800000.0;
    private const double MillisecondsInAYearApproximate = 31536000000.0;
    private const int MonthsInAYear = 12;
    private static readonly Regex ExpressionRegex = new Regex("^\r\n\t\t\t\t(?<factor>[+\\-]? # open factor capture, allowing optional +- signs\r\n\t\t\t\t\t(?:(?#numeric)(?:\\d+(?:\\.\\d*)?)|(?:\\.\\d+)) #a numeric in the forms: (N, N., .N, N.N)\r\n\t\t\t\t\t(?:(?#exponent)e[+\\-]?\\d+)? #an optional exponential scientific component, E also matches here (IgnoreCase)\r\n\t\t\t\t) # numeric and exponent fall under the factor capture\r\n\t\t\t\t\\s{0,10} #optional spaces (sanity checked for max 10 repetitions)\r\n\t\t\t\t(?<interval>(?:y|w|d|h|m|s)) #interval indicator\r\n\t\t\t\t$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);
    private double _approximateSeconds;

    public DateMathTime(TimeSpan timeSpan, MidpointRounding rounding = MidpointRounding.AwayFromZero)
      : this(timeSpan.TotalMilliseconds, rounding)
    {
    }

    public DateMathTime(double milliseconds, MidpointRounding rounding = MidpointRounding.AwayFromZero) => this.SetWholeFactorIntervalAndSeconds(milliseconds, rounding);

    public DateMathTime(int factor, DateMathTimeUnit interval) => this.SetWholeFactorIntervalAndSeconds((double) factor, interval, MidpointRounding.AwayFromZero);

    public DateMathTime(string timeUnit, MidpointRounding rounding = MidpointRounding.AwayFromZero)
    {
      switch (timeUnit)
      {
        case null:
          throw new ArgumentNullException(nameof (timeUnit));
        case "":
          throw new ArgumentException("Expression string is empty", nameof (timeUnit));
        default:
          Match match = DateMathTime.ExpressionRegex.Match(timeUnit);
          if (!match.Success)
            throw new ArgumentException("Expression '" + timeUnit + "' string is invalid", nameof (timeUnit));
          string s = match.Groups["factor"].Value;
          double result;
          if (!double.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            throw new ArgumentException("Expression '" + timeUnit + "' contains invalid factor: " + s, nameof (timeUnit));
          string str = match.Groups["interval"].Value;
          DateMathTimeUnit interval;
          switch (str)
          {
            case "M":
              interval = DateMathTimeUnit.Month;
              break;
            case "m":
              interval = DateMathTimeUnit.Minute;
              break;
            default:
              interval = str.ToEnum<DateMathTimeUnit>().GetValueOrDefault();
              break;
          }
          this.SetWholeFactorIntervalAndSeconds(result, interval, rounding);
          break;
      }
    }

    public int Factor { get; private set; }

    public DateMathTimeUnit Interval { get; private set; }

    public int CompareTo(DateMathTime other)
    {
      if (other == (DateMathTime) null)
        return 1;
      if (Math.Abs(this._approximateSeconds - other._approximateSeconds) < double.Epsilon)
        return 0;
      return this._approximateSeconds < other._approximateSeconds ? -1 : 1;
    }

    public bool Equals(DateMathTime other)
    {
      if ((object) other == null)
        return false;
      return (object) this == (object) other || Math.Abs(this._approximateSeconds - other._approximateSeconds) < double.Epsilon;
    }

    public static implicit operator DateMathTime(TimeSpan span) => new DateMathTime(span);

    public static implicit operator DateMathTime(double milliseconds) => new DateMathTime(milliseconds);

    public static implicit operator DateMathTime(string expression) => new DateMathTime(expression);

    private void SetWholeFactorIntervalAndSeconds(
      double factor,
      DateMathTimeUnit interval,
      MidpointRounding rounding)
    {
      double d = factor;
      int num;
      if (DateMathTime.TryGetIntegerGreaterThanZero(d, out num))
      {
        this.Factor = num;
        this.Interval = interval;
        switch (interval)
        {
          case DateMathTimeUnit.Second:
            this._approximateSeconds = (double) num;
            break;
          case DateMathTimeUnit.Minute:
            this._approximateSeconds = (double) num * 60.0;
            break;
          case DateMathTimeUnit.Hour:
            this._approximateSeconds = (double) num * 3600.0;
            break;
          case DateMathTimeUnit.Day:
            this._approximateSeconds = (double) num * 86400.0;
            break;
          case DateMathTimeUnit.Week:
            this._approximateSeconds = (double) num * 604800.0;
            break;
          case DateMathTimeUnit.Month:
            this._approximateSeconds = (double) num * 2628000.0;
            break;
          case DateMathTimeUnit.Year:
            this._approximateSeconds = (double) num * 31536000.0;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (interval), (object) interval, (string) null);
        }
      }
      else
      {
        double milliseconds;
        switch (interval)
        {
          case DateMathTimeUnit.Second:
            milliseconds = factor * 1000.0;
            break;
          case DateMathTimeUnit.Minute:
            milliseconds = factor * 60000.0;
            break;
          case DateMathTimeUnit.Hour:
            milliseconds = factor * 3600000.0;
            break;
          case DateMathTimeUnit.Day:
            milliseconds = factor * 86400000.0;
            break;
          case DateMathTimeUnit.Week:
            milliseconds = factor * 604800000.0;
            break;
          case DateMathTimeUnit.Month:
            if (DateMathTime.TryGetIntegerGreaterThanZero(d, out num))
            {
              this.Factor = num;
              this.Interval = interval;
              this._approximateSeconds = (double) num * 2628000.0;
              return;
            }
            milliseconds = factor * 2628000000.0;
            break;
          case DateMathTimeUnit.Year:
            if (DateMathTime.TryGetIntegerGreaterThanZero(d, out num))
            {
              this.Factor = num;
              this.Interval = interval;
              this._approximateSeconds = (double) num * 31536000.0;
              return;
            }
            if (DateMathTime.TryGetIntegerGreaterThanZero(d * 12.0, out num))
            {
              this.Factor = num;
              this.Interval = DateMathTimeUnit.Month;
              this._approximateSeconds = (double) num * 2628000.0;
              return;
            }
            milliseconds = factor * 31536000000.0;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (interval), (object) interval, (string) null);
        }
        this.SetWholeFactorIntervalAndSeconds(milliseconds, rounding);
      }
    }

    private void SetWholeFactorIntervalAndSeconds(double milliseconds, MidpointRounding rounding)
    {
      int num;
      if (milliseconds >= 604800000.0 && DateMathTime.TryGetIntegerGreaterThanZero(milliseconds / 604800000.0, out num))
      {
        this.Factor = num;
        this.Interval = DateMathTimeUnit.Week;
        this._approximateSeconds = (double) this.Factor * 604800.0;
      }
      else if (milliseconds >= 86400000.0 && DateMathTime.TryGetIntegerGreaterThanZero(milliseconds / 86400000.0, out num))
      {
        this.Factor = num;
        this.Interval = DateMathTimeUnit.Day;
        this._approximateSeconds = (double) this.Factor * 86400.0;
      }
      else if (milliseconds >= 3600000.0 && DateMathTime.TryGetIntegerGreaterThanZero(milliseconds / 3600000.0, out num))
      {
        this.Factor = num;
        this.Interval = DateMathTimeUnit.Hour;
        this._approximateSeconds = (double) this.Factor * 3600.0;
      }
      else if (milliseconds >= 60000.0 && DateMathTime.TryGetIntegerGreaterThanZero(milliseconds / 60000.0, out num))
      {
        this.Factor = num;
        this.Interval = DateMathTimeUnit.Minute;
        this._approximateSeconds = (double) this.Factor * 60.0;
      }
      else if (milliseconds >= 1000.0 && DateMathTime.TryGetIntegerGreaterThanZero(milliseconds / 1000.0, out num))
      {
        this.Factor = num;
        this.Interval = DateMathTimeUnit.Second;
        this._approximateSeconds = (double) this.Factor;
      }
      else
      {
        this.Factor = Convert.ToInt32(Math.Round(milliseconds / 1000.0, rounding));
        this.Interval = DateMathTimeUnit.Second;
        this._approximateSeconds = (double) this.Factor;
      }
    }

    private static bool TryGetIntegerGreaterThanZero(double d, out int value)
    {
      if (Math.Abs(d % 1.0) < double.Epsilon)
      {
        value = Convert.ToInt32(d);
        return true;
      }
      value = 0;
      return false;
    }

    public static bool operator <(DateMathTime left, DateMathTime right) => left.CompareTo(right) < 0;

    public static bool operator <=(DateMathTime left, DateMathTime right) => left.CompareTo(right) < 0 || left.Equals(right);

    public static bool operator >(DateMathTime left, DateMathTime right) => left.CompareTo(right) > 0;

    public static bool operator >=(DateMathTime left, DateMathTime right) => left.CompareTo(right) > 0 || left.Equals(right);

    public static bool operator ==(DateMathTime left, DateMathTime right) => (object) left == null ? (object) right == null : __nonvirtual (left.Equals(right));

    public static bool operator !=(DateMathTime left, DateMathTime right) => !(left == right);

    public override string ToString() => this.Factor.ToString() + this.Interval.GetStringValue();

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((DateMathTime) obj);
    }

    public override int GetHashCode() => this._approximateSeconds.GetHashCode();
  }
}
