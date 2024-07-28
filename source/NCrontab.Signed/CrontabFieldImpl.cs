// Decompiled with JetBrains decompiler
// Type: NCrontab.CrontabFieldImpl
// Assembly: NCrontab.Signed, Version=3.3.2.0, Culture=neutral, PublicKeyToken=5247b4370afff365
// MVID: 57590294-74AB-400C-8AE3-EEC26A6094FB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\NCrontab.Signed.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;

namespace NCrontab
{
  [Serializable]
  internal sealed class CrontabFieldImpl : IObjectReference
  {
    public static readonly CrontabFieldImpl Second = new CrontabFieldImpl(CrontabFieldKind.Second, 0, 59, (string[]) null);
    public static readonly CrontabFieldImpl Minute = new CrontabFieldImpl(CrontabFieldKind.Minute, 0, 59, (string[]) null);
    public static readonly CrontabFieldImpl Hour = new CrontabFieldImpl(CrontabFieldKind.Hour, 0, 23, (string[]) null);
    public static readonly CrontabFieldImpl Day = new CrontabFieldImpl(CrontabFieldKind.Day, 1, 31, (string[]) null);
    public static readonly CrontabFieldImpl Month = new CrontabFieldImpl(CrontabFieldKind.Month, 1, 12, new string[12]
    {
      "January",
      "February",
      "March",
      "April",
      "May",
      "June",
      "July",
      "August",
      "September",
      "October",
      "November",
      "December"
    });
    public static readonly CrontabFieldImpl DayOfWeek = new CrontabFieldImpl(CrontabFieldKind.DayOfWeek, 0, 6, new string[7]
    {
      "Sunday",
      "Monday",
      "Tuesday",
      "Wednesday",
      "Thursday",
      "Friday",
      "Saturday"
    });
    private static readonly CrontabFieldImpl[] FieldByKind = new CrontabFieldImpl[6]
    {
      CrontabFieldImpl.Second,
      CrontabFieldImpl.Minute,
      CrontabFieldImpl.Hour,
      CrontabFieldImpl.Day,
      CrontabFieldImpl.Month,
      CrontabFieldImpl.DayOfWeek
    };
    private static readonly CompareInfo Comparer = CultureInfo.InvariantCulture.CompareInfo;
    private readonly string[] _names;

    public static CrontabFieldImpl FromKind(CrontabFieldKind kind) => Enum.IsDefined(typeof (CrontabFieldKind), (object) kind) ? CrontabFieldImpl.FieldByKind[(int) kind] : throw new ArgumentException("Invalid crontab field kind. Valid values are " + string.Join(", ", Enum.GetNames(typeof (CrontabFieldKind))) + ".", nameof (kind));

    private CrontabFieldImpl(CrontabFieldKind kind, int minValue, int maxValue, string[] names)
    {
      this.Kind = kind;
      this.MinValue = minValue;
      this.MaxValue = maxValue;
      this._names = names;
    }

    public CrontabFieldKind Kind { get; }

    public int MinValue { get; }

    public int MaxValue { get; }

    public int ValueCount => this.MaxValue - this.MinValue + 1;

    public void Format(ICrontabField field, TextWriter writer) => this.Format(field, writer, false);

    public void Format(ICrontabField field, TextWriter writer, bool noNames)
    {
      if (field == null)
        throw new ArgumentNullException(nameof (field));
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      int num1 = field.GetFirst();
      int num2 = 0;
      while (num1 != -1)
      {
        int num3 = num1;
        int num4;
        do
        {
          num4 = num1;
          num1 = field.Next(num4 + 1);
        }
        while (num1 - num4 == 1);
        if (num2 == 0 && num3 == this.MinValue && num4 == this.MaxValue)
        {
          writer.Write('*');
          break;
        }
        if (num2 > 0)
          writer.Write(',');
        if (num3 == num4)
        {
          this.FormatValue(num3, writer, noNames);
        }
        else
        {
          this.FormatValue(num3, writer, noNames);
          writer.Write('-');
          this.FormatValue(num4, writer, noNames);
        }
        ++num2;
      }
    }

    private void FormatValue(int value, TextWriter writer, bool noNames)
    {
      if (noNames || this._names == null)
      {
        if (value >= 0 && value < 100)
          CrontabFieldImpl.FastFormatNumericValue(value, writer);
        else
          writer.Write(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      else
      {
        int index = value - this.MinValue;
        writer.Write(this._names[index]);
      }
    }

    private static void FastFormatNumericValue(int value, TextWriter writer)
    {
      if (value >= 10)
      {
        writer.Write((char) (48 + value / 10));
        writer.Write((char) (48 + value % 10));
      }
      else
        writer.Write((char) (48 + value));
    }

    public void Parse(string str, CrontabFieldAccumulator<ExceptionProvider> acc) => this.TryParse<ExceptionProvider>(str, acc, (ExceptionProvider) null, (Func<ExceptionProvider, ExceptionProvider>) (ep =>
    {
      throw ep();
    }));

    public T TryParse<T>(
      string str,
      CrontabFieldAccumulator<T> acc,
      T success,
      Func<ExceptionProvider, T> errorSelector)
    {
      if (acc == null)
        throw new ArgumentNullException(nameof (acc));
      if (string.IsNullOrEmpty(str))
        return success;
      try
      {
        return this.InternalParse<T>(str, acc, success, errorSelector);
      }
      catch (FormatException ex)
      {
        return this.OnParseException<T>((Exception) ex, str, errorSelector);
      }
      catch (CrontabException ex)
      {
        return this.OnParseException<T>((Exception) ex, str, errorSelector);
      }
    }

    private T OnParseException<T>(
      Exception innerException,
      string str,
      Func<ExceptionProvider, T> errorSelector)
    {
      return errorSelector((ExceptionProvider) (() => (Exception) new CrontabException(string.Format("'{0}' is not a valid [{1}] crontab field expression.", (object) str, (object) this.Kind), innerException)));
    }

    private T InternalParse<T>(
      string str,
      CrontabFieldAccumulator<T> acc,
      T success,
      Func<ExceptionProvider, T> errorSelector)
    {
      if (str.Length == 0)
        return errorSelector((ExceptionProvider) (() => (Exception) new CrontabException("A crontab field value cannot be empty.")));
      if (str.IndexOf(',') > 0)
      {
        T obj = success;
        foreach (string str1 in (IEnumerable<string>) str.Split(StringSeparatorStock.Comma))
        {
          if ((object) obj == null)
            obj = this.InternalParse<T>(str1, acc, success, errorSelector);
          else
            break;
        }
        return obj;
      }
      int? nullable = new int?();
      int length1 = str.IndexOf('/');
      if (length1 > 0)
      {
        nullable = new int?(int.Parse(str.Substring(length1 + 1), (IFormatProvider) CultureInfo.InvariantCulture));
        str = str.Substring(0, length1);
      }
      if (str.Length == 1 && str[0] == '*')
        return acc(-1, -1, nullable ?? 1, success, errorSelector);
      int length2 = str.IndexOf('-');
      if (length2 > 0)
      {
        int start = this.ParseValue(str.Substring(0, length2));
        int end = this.ParseValue(str.Substring(length2 + 1));
        return acc(start, end, nullable ?? 1, success, errorSelector);
      }
      int num = this.ParseValue(str);
      return !nullable.HasValue ? acc(num, num, 1, success, errorSelector) : acc(num, this.MaxValue, nullable.Value, success, errorSelector);
    }

    private int ParseValue(string str)
    {
      if (str.Length == 0)
        throw new CrontabException("A crontab field value cannot be empty.");
      switch (str[0])
      {
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          return int.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
        default:
          if (this._names == null)
          {
            object[] objArray = new object[4]
            {
              (object) str,
              null,
              null,
              null
            };
            int num = this.MinValue;
            objArray[1] = (object) num.ToString();
            num = this.MaxValue;
            objArray[2] = (object) num.ToString();
            objArray[3] = (object) this.Kind.ToString();
            throw new CrontabException(string.Format("'{0}' is not a valid [{3}] crontab field value. It must be a numeric value between {1} and {2} (all inclusive).", objArray));
          }
          for (int index = 0; index < this._names.Length; ++index)
          {
            if (CrontabFieldImpl.Comparer.IsPrefix(this._names[index], str, CompareOptions.IgnoreCase))
              return index + this.MinValue;
          }
          string str1 = string.Join(", ", this._names);
          throw new CrontabException("'" + str + "' is not a known value name. Use one of the following: " + str1 + ".");
      }
    }

    object IObjectReference.GetRealObject(StreamingContext context) => (object) CrontabFieldImpl.FromKind(this.Kind);
  }
}
