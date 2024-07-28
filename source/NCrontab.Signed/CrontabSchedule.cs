// Decompiled with JetBrains decompiler
// Type: NCrontab.CrontabSchedule
// Assembly: NCrontab.Signed, Version=3.3.2.0, Culture=neutral, PublicKeyToken=5247b4370afff365
// MVID: 57590294-74AB-400C-8AE3-EEC26A6094FB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\NCrontab.Signed.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NCrontab
{
  [Serializable]
  public sealed class CrontabSchedule
  {
    private readonly CrontabField _seconds;
    private readonly CrontabField _minutes;
    private readonly CrontabField _hours;
    private readonly CrontabField _days;
    private readonly CrontabField _months;
    private readonly CrontabField _daysOfWeek;
    private static readonly CrontabField SecondZero = CrontabField.Seconds("0");

    public static CrontabSchedule Parse(string expression) => CrontabSchedule.Parse(expression, (CrontabSchedule.ParseOptions) null);

    public static CrontabSchedule Parse(string expression, CrontabSchedule.ParseOptions options) => CrontabSchedule.TryParse<CrontabSchedule>(expression, options, (Func<CrontabSchedule, CrontabSchedule>) (v => v), (Func<ExceptionProvider, CrontabSchedule>) (e =>
    {
      throw e();
    }));

    public static CrontabSchedule TryParse(string expression) => CrontabSchedule.TryParse(expression, (CrontabSchedule.ParseOptions) null);

    public static CrontabSchedule TryParse(string expression, CrontabSchedule.ParseOptions options) => CrontabSchedule.TryParse<CrontabSchedule>(expression ?? string.Empty, options, (Func<CrontabSchedule, CrontabSchedule>) (v => v), (Func<ExceptionProvider, CrontabSchedule>) (_ => (CrontabSchedule) null));

    public static T TryParse<T>(
      string expression,
      Func<CrontabSchedule, T> valueSelector,
      Func<ExceptionProvider, T> errorSelector)
    {
      return CrontabSchedule.TryParse<T>(expression ?? string.Empty, (CrontabSchedule.ParseOptions) null, valueSelector, errorSelector);
    }

    public static T TryParse<T>(
      string expression,
      CrontabSchedule.ParseOptions options,
      Func<CrontabSchedule, T> valueSelector,
      Func<ExceptionProvider, T> errorSelector)
    {
      string[] strArray = expression != null ? expression.Split(StringSeparatorStock.Space, StringSplitOptions.RemoveEmptyEntries) : throw new ArgumentNullException(nameof (expression));
      bool includingSeconds = options != null && options.IncludingSeconds;
      int num1 = includingSeconds ? 6 : 5;
      if (strArray.Length < num1 || strArray.Length > num1)
        return errorSelector((ExceptionProvider) (() =>
        {
          string str = includingSeconds ? "6 components of a schedule in the sequence of seconds, minutes, hours, days, months, and days of week" : "5 components of a schedule in the sequence of minutes, hours, days, months, and days of week";
          return (Exception) new CrontabException("'" + expression + "' is an invalid crontab expression. It must contain " + str + ".");
        }));
      CrontabField[] crontabFieldArray = new CrontabField[6];
      int num2 = includingSeconds ? 0 : 1;
      for (int index = 0; index < strArray.Length; ++index)
      {
        var data = CrontabField.TryParse((CrontabFieldKind) (index + num2), strArray[index], v => new
        {
          ErrorProvider = (ExceptionProvider) null,
          Value = v
        }, e => new
        {
          ErrorProvider = e,
          Value = (CrontabField) null
        });
        if (data.ErrorProvider != null)
          return errorSelector(data.ErrorProvider);
        crontabFieldArray[index + num2] = data.Value;
      }
      return valueSelector(new CrontabSchedule(crontabFieldArray[0], crontabFieldArray[1], crontabFieldArray[2], crontabFieldArray[3], crontabFieldArray[4], crontabFieldArray[5]));
    }

    private CrontabSchedule(
      CrontabField seconds,
      CrontabField minutes,
      CrontabField hours,
      CrontabField days,
      CrontabField months,
      CrontabField daysOfWeek)
    {
      this._seconds = seconds;
      this._minutes = minutes;
      this._hours = hours;
      this._days = days;
      this._months = months;
      this._daysOfWeek = daysOfWeek;
    }

    public IEnumerable<DateTime> GetNextOccurrences(DateTime baseTime, DateTime endTime)
    {
      DateTime? occurrence;
      for (occurrence = this.TryGetNextOccurrence(baseTime, endTime); occurrence.HasValue; occurrence = this.TryGetNextOccurrence(occurrence.Value, endTime))
      {
        DateTime? nullable = occurrence;
        DateTime dateTime = endTime;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() < dateTime ? 1 : 0) : 0) != 0)
          yield return occurrence.Value;
        else
          break;
      }
      occurrence = new DateTime?();
    }

    public DateTime GetNextOccurrence(DateTime baseTime) => this.GetNextOccurrence(baseTime, DateTime.MaxValue);

    public DateTime GetNextOccurrence(DateTime baseTime, DateTime endTime) => this.TryGetNextOccurrence(baseTime, endTime) ?? endTime;

    private DateTime? TryGetNextOccurrence(DateTime baseTime, DateTime endTime)
    {
      int year1 = baseTime.Year;
      int month1 = baseTime.Month;
      int day1 = baseTime.Day;
      int hour1 = baseTime.Hour;
      int minute1 = baseTime.Minute;
      int second1 = baseTime.Second;
      int year2 = endTime.Year;
      int month2 = endTime.Month;
      int day2 = endTime.Day;
      int year3 = year1;
      int num = month1;
      int start1 = day1;
      int start2 = hour1;
      int start3 = minute1;
      int start4 = second1 + 1;
      CrontabField crontabField = this._seconds ?? CrontabSchedule.SecondZero;
      int second2 = crontabField.Next(start4);
      if (second2 == -1)
      {
        second2 = crontabField.GetFirst();
        ++start3;
      }
      int minute2 = this._minutes.Next(start3);
      if (minute2 == -1)
      {
        minute2 = this._minutes.GetFirst();
        ++start2;
      }
      int hour2 = this._hours.Next(start2);
      if (hour2 == -1)
      {
        minute2 = this._minutes.GetFirst();
        hour2 = this._hours.GetFirst();
        ++start1;
      }
      else if (hour2 > hour1)
        minute2 = this._minutes.GetFirst();
      int day3 = this._days.Next(start1);
      while (true)
      {
        if (day3 == -1)
        {
          second2 = crontabField.GetFirst();
          minute2 = this._minutes.GetFirst();
          hour2 = this._hours.GetFirst();
          day3 = this._days.GetFirst();
          ++num;
        }
        else if (day3 > day1)
        {
          second2 = crontabField.GetFirst();
          minute2 = this._minutes.GetFirst();
          hour2 = this._hours.GetFirst();
        }
        num = this._months.Next(num);
        if (num == -1)
        {
          second2 = crontabField.GetFirst();
          minute2 = this._minutes.GetFirst();
          hour2 = this._hours.GetFirst();
          day3 = this._days.GetFirst();
          num = this._months.GetFirst();
          ++year3;
        }
        else if (num > month1)
        {
          second2 = crontabField.GetFirst();
          minute2 = this._minutes.GetFirst();
          hour2 = this._hours.GetFirst();
          day3 = this._days.GetFirst();
        }
        if (year3 <= CrontabSchedule.Calendar.MaxSupportedDateTime.Year)
        {
          bool flag = day3 != day1 || num != month1 || year3 != year1;
          if (day3 > 28 & flag && day3 > CrontabSchedule.Calendar.GetDaysInMonth(year3, num))
          {
            if (year3 < year2 || num < month2 || day3 < day2)
              day3 = -1;
            else
              goto label_21;
          }
          else
            goto label_23;
        }
        else
          break;
      }
      return new DateTime?();
label_21:
      return new DateTime?(endTime);
label_23:
      DateTime dateTime = new DateTime(year3, num, day3, hour2, minute2, second2, 0, baseTime.Kind);
      if (dateTime >= endTime)
        return new DateTime?(endTime);
      return this._daysOfWeek.Contains((int) dateTime.DayOfWeek) ? new DateTime?(dateTime) : this.TryGetNextOccurrence(new DateTime(year3, num, day3, 23, 59, 59, 0, baseTime.Kind), endTime);
    }

    public override string ToString()
    {
      StringWriter writer = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      if (this._seconds != null)
      {
        this._seconds.Format((TextWriter) writer, true);
        writer.Write(' ');
      }
      this._minutes.Format((TextWriter) writer, true);
      writer.Write(' ');
      this._hours.Format((TextWriter) writer, true);
      writer.Write(' ');
      this._days.Format((TextWriter) writer, true);
      writer.Write(' ');
      this._months.Format((TextWriter) writer, true);
      writer.Write(' ');
      this._daysOfWeek.Format((TextWriter) writer, true);
      return writer.ToString();
    }

    private static Calendar Calendar => CultureInfo.InvariantCulture.Calendar;

    [Serializable]
    public sealed class ParseOptions
    {
      public bool IncludingSeconds { get; set; }
    }
  }
}
