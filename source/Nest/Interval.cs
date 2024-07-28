// Decompiled with JetBrains decompiler
// Type: Nest.Interval
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
  [JsonFormatter(typeof (IntervalFormatter))]
  public class Interval : ScheduleBase, IComparable<Interval>, IEquatable<Interval>
  {
    private const long DaySeconds = 86400;
    private const long HourSeconds = 3600;
    private const long MinuteSeconds = 60;
    private const long Second = 1;
    private const long WeekSeconds = 604800;
    private static readonly Regex IntervalExpressionRegex = new Regex("^(?<factor>\\d+)(?<unit>(?:w|d|h|m|s))?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
    private long _seconds;

    public Interval(TimeSpan timeSpan)
    {
      long num = timeSpan.TotalSeconds >= 1.0 ? (long) timeSpan.TotalSeconds : throw new ArgumentException("must be greater than or equal to 1 second", nameof (timeSpan));
      if (num >= 604800L && num % 604800L == 0L)
      {
        this.Factor = num / 604800L;
        this.Unit = new IntervalUnit?(IntervalUnit.Week);
      }
      else if (num >= 86400L && num % 86400L == 0L)
      {
        this.Factor = num / 86400L;
        this.Unit = new IntervalUnit?(IntervalUnit.Day);
      }
      else if (num >= 3600L && num % 3600L == 0L)
      {
        this.Factor = num / 3600L;
        this.Unit = new IntervalUnit?(IntervalUnit.Hour);
      }
      else if (num >= 60L && num % 60L == 0L)
      {
        this.Factor = num / 60L;
        this.Unit = new IntervalUnit?(IntervalUnit.Minute);
      }
      else
      {
        this.Factor = num;
        this.Unit = new IntervalUnit?(IntervalUnit.Second);
      }
      this._seconds = num;
    }

    public Interval(long seconds)
    {
      this.Factor = seconds;
      this._seconds = seconds;
    }

    public Interval(long factor, IntervalUnit unit)
    {
      this.Factor = factor;
      this.Unit = new IntervalUnit?(unit);
      this.SetSeconds(this.Factor, unit);
    }

    public Interval(string intervalUnit)
    {
      Match match = !intervalUnit.IsNullOrEmpty() ? Interval.IntervalExpressionRegex.Match(intervalUnit) : throw new ArgumentException("Interval expression string cannot be null or empty", nameof (intervalUnit));
      if (!match.Success)
        throw new ArgumentException("Interval expression '" + intervalUnit + "' string is invalid", nameof (intervalUnit));
      this.Factor = long.Parse(match.Groups["factor"].Value, (IFormatProvider) CultureInfo.InvariantCulture);
      IntervalUnit interval = match.Groups["unit"].Success ? match.Groups["unit"].Value.ToEnum<IntervalUnit>().GetValueOrDefault(IntervalUnit.Second) : IntervalUnit.Second;
      this.Unit = new IntervalUnit?(interval);
      this.SetSeconds(this.Factor, interval);
    }

    public long Factor { get; }

    public IntervalUnit? Unit { get; }

    public int CompareTo(Interval other)
    {
      if (other == (Interval) null)
        return 1;
      if (this._seconds == other._seconds)
        return 0;
      return this._seconds < other._seconds ? -1 : 1;
    }

    public bool Equals(Interval other)
    {
      if ((object) other == null)
        return false;
      return (object) this == (object) other || this._seconds == other._seconds;
    }

    public static implicit operator Interval(TimeSpan timeSpan) => new Interval(timeSpan);

    public static implicit operator Interval(long seconds) => new Interval(seconds);

    public static implicit operator Interval(string expression) => new Interval(expression);

    public static bool operator <(Interval left, Interval right) => left.CompareTo(right) < 0;

    public static bool operator <=(Interval left, Interval right) => left.CompareTo(right) < 0 || left.Equals(right);

    public static bool operator >(Interval left, Interval right) => left.CompareTo(right) > 0;

    public static bool operator >=(Interval left, Interval right) => left.CompareTo(right) > 0 || left.Equals(right);

    public static bool operator ==(Interval left, Interval right) => (object) left != null ? left.Equals(right) : (object) right == null;

    public static bool operator !=(Interval left, Interval right) => !(left == right);

    public override string ToString()
    {
      string str = this.Factor.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      return !this.Unit.HasValue ? str : str + this.Unit.Value.GetStringValue();
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((Interval) obj);
    }

    public override int GetHashCode() => this._seconds.GetHashCode();

    internal override void WrapInContainer(IScheduleContainer container) => container.Interval = this;

    private void SetSeconds(long factor, IntervalUnit interval)
    {
      switch (interval)
      {
        case IntervalUnit.Second:
          this._seconds = factor;
          break;
        case IntervalUnit.Minute:
          this._seconds = factor * 60L;
          break;
        case IntervalUnit.Hour:
          this._seconds = factor * 3600L;
          break;
        case IntervalUnit.Day:
          this._seconds = factor * 86400L;
          break;
        case IntervalUnit.Week:
          this._seconds = factor * 604800L;
          break;
      }
    }
  }
}
