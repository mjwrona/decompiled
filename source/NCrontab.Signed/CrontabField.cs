// Decompiled with JetBrains decompiler
// Type: NCrontab.CrontabField
// Assembly: NCrontab.Signed, Version=3.3.2.0, Culture=neutral, PublicKeyToken=5247b4370afff365
// MVID: 57590294-74AB-400C-8AE3-EEC26A6094FB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\NCrontab.Signed.dll

using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace NCrontab
{
  [Serializable]
  public sealed class CrontabField : ICrontabField
  {
    private readonly BitArray _bits;
    private int _minValueSet;
    private int _maxValueSet;
    private readonly CrontabFieldImpl _impl;

    public static CrontabField Parse(CrontabFieldKind kind, string expression) => CrontabField.TryParse<CrontabField>(kind, expression, (Func<CrontabField, CrontabField>) (v => v), (Func<ExceptionProvider, CrontabField>) (e =>
    {
      throw e();
    }));

    public static CrontabField TryParse(CrontabFieldKind kind, string expression) => CrontabField.TryParse<CrontabField>(kind, expression, (Func<CrontabField, CrontabField>) (v => v), (Func<ExceptionProvider, CrontabField>) (_ => (CrontabField) null));

    public static T TryParse<T>(
      CrontabFieldKind kind,
      string expression,
      Func<CrontabField, T> valueSelector,
      Func<ExceptionProvider, T> errorSelector)
    {
      CrontabField crontabField = new CrontabField(CrontabFieldImpl.FromKind(kind));
      ExceptionProvider exceptionProvider = crontabField._impl.TryParse<ExceptionProvider>(expression, new CrontabFieldAccumulator<ExceptionProvider>(crontabField.Accumulate<ExceptionProvider>), (ExceptionProvider) null, (Func<ExceptionProvider, ExceptionProvider>) (e => e));
      return exceptionProvider != null ? errorSelector(exceptionProvider) : valueSelector(crontabField);
    }

    public static CrontabField Seconds(string expression) => CrontabField.Parse(CrontabFieldKind.Second, expression);

    public static CrontabField Minutes(string expression) => CrontabField.Parse(CrontabFieldKind.Minute, expression);

    public static CrontabField Hours(string expression) => CrontabField.Parse(CrontabFieldKind.Hour, expression);

    public static CrontabField Days(string expression) => CrontabField.Parse(CrontabFieldKind.Day, expression);

    public static CrontabField Months(string expression) => CrontabField.Parse(CrontabFieldKind.Month, expression);

    public static CrontabField DaysOfWeek(string expression) => CrontabField.Parse(CrontabFieldKind.DayOfWeek, expression);

    private CrontabField(CrontabFieldImpl impl)
    {
      this._impl = impl != null ? impl : throw new ArgumentNullException(nameof (impl));
      this._bits = new BitArray(impl.ValueCount);
      this._bits.SetAll(false);
      this._minValueSet = int.MaxValue;
      this._maxValueSet = -1;
    }

    public int GetFirst() => this._minValueSet >= int.MaxValue ? -1 : this._minValueSet;

    public int Next(int start)
    {
      if (start < this._minValueSet)
        return this._minValueSet;
      int index1 = this.ValueToIndex(start);
      int index2 = this.ValueToIndex(this._maxValueSet);
      for (int index3 = index1; index3 <= index2; ++index3)
      {
        if (this._bits[index3])
          return this.IndexToValue(index3);
      }
      return -1;
    }

    private int IndexToValue(int index) => index + this._impl.MinValue;

    private int ValueToIndex(int value) => value - this._impl.MinValue;

    public bool Contains(int value) => this._bits[this.ValueToIndex(value)];

    private T Accumulate<T>(
      int start,
      int end,
      int interval,
      T success,
      Func<ExceptionProvider, T> errorSelector)
    {
      int minValue = this._impl.MinValue;
      int maxValue = this._impl.MaxValue;
      if (start == end)
      {
        if (start < 0)
        {
          if (interval <= 1)
          {
            this._minValueSet = minValue;
            this._maxValueSet = maxValue;
            this._bits.SetAll(true);
            return success;
          }
          start = minValue;
          end = maxValue;
        }
        else
        {
          if (start < minValue)
            return this.OnValueBelowMinError<T>(start, errorSelector);
          if (start > maxValue)
            return this.OnValueAboveMaxError<T>(start, errorSelector);
        }
      }
      else
      {
        if (start > end)
        {
          end ^= start;
          start ^= end;
          end ^= start;
        }
        if (start < 0)
          start = minValue;
        else if (start < minValue)
          return this.OnValueBelowMinError<T>(start, errorSelector);
        if (end < 0)
          end = maxValue;
        else if (end > maxValue)
          return this.OnValueAboveMaxError<T>(end, errorSelector);
      }
      if (interval < 1)
        interval = 1;
      int index;
      for (index = start - minValue; index <= end - minValue; index += interval)
        this._bits[index] = true;
      if (this._minValueSet > start)
        this._minValueSet = start;
      int num = index + (minValue - interval);
      if (this._maxValueSet < num)
        this._maxValueSet = num;
      return success;
    }

    private T OnValueAboveMaxError<T>(int value, Func<ExceptionProvider, T> errorSelector) => errorSelector((ExceptionProvider) (() => (Exception) new CrontabException(string.Format("{0} is higher than the maximum allowable value for the [{1}] field. ", (object) value, (object) this._impl.Kind) + string.Format("Value must be between {0} and {1} (all inclusive).", (object) this._impl.MinValue, (object) this._impl.MaxValue))));

    private T OnValueBelowMinError<T>(int value, Func<ExceptionProvider, T> errorSelector) => errorSelector((ExceptionProvider) (() => (Exception) new CrontabException(string.Format("{0} is lower than the minimum allowable value for the [{1}] field. ", (object) value, (object) this._impl.Kind) + string.Format("Value must be between {0} and {1} (all inclusive).", (object) this._impl.MinValue, (object) this._impl.MaxValue))));

    public override string ToString() => this.ToString((string) null);

    public string ToString(string format)
    {
      StringWriter writer = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      switch (format)
      {
        case "G":
        case null:
          this.Format((TextWriter) writer, true);
          break;
        case "N":
          this.Format((TextWriter) writer);
          break;
        default:
          throw new FormatException();
      }
      return writer.ToString();
    }

    public void Format(TextWriter writer) => this.Format(writer, false);

    public void Format(TextWriter writer, bool noNames) => this._impl.Format((ICrontabField) this, writer, noNames);
  }
}
