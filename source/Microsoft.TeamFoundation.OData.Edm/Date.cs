// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Date
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Globalization;

namespace Microsoft.OData.Edm
{
  public struct Date : IComparable, IComparable<Date>, IEquatable<Date>
  {
    public static readonly Date MinValue = new Date(1, 1, 1);
    public static readonly Date MaxValue = new Date(9999, 12, 31);
    private DateTime dateTime;

    public Date(int year, int month, int day)
      : this()
    {
      try
      {
        this.dateTime = new DateTime(year, month, day);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new FormatException(Strings.Date_InvalidDateParameters((object) year, (object) month, (object) day));
      }
    }

    public static Date Now => (Date) DateTime.Now;

    public int Year => this.dateTime.Year;

    public int Month => this.dateTime.Month;

    public int Day => this.dateTime.Day;

    public static bool operator ==(Date firstOperand, Date secondOperand) => firstOperand.dateTime == secondOperand.dateTime;

    public static bool operator !=(Date firstOperand, Date secondOperand) => firstOperand.dateTime != secondOperand.dateTime;

    public static bool operator <(Date firstOperand, Date secondOperand) => firstOperand.dateTime < secondOperand.dateTime;

    public static bool operator <=(Date firstOperand, Date secondOperand) => firstOperand.dateTime <= secondOperand.dateTime;

    public static bool operator >(Date firstOperand, Date secondOperand) => firstOperand.dateTime > secondOperand.dateTime;

    public static bool operator >=(Date firstOperand, Date secondOperand) => firstOperand.dateTime >= secondOperand.dateTime;

    public Date AddYears(int value)
    {
      try
      {
        return (Date) this.dateTime.AddYears(value);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentOutOfRangeException(nameof (value), Strings.Date_InvalidAddedOrSubtractedResults);
      }
    }

    public Date AddMonths(int value)
    {
      try
      {
        return (Date) this.dateTime.AddMonths(value);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentOutOfRangeException(nameof (value), Strings.Date_InvalidAddedOrSubtractedResults);
      }
    }

    public Date AddDays(int value)
    {
      try
      {
        return (Date) this.dateTime.AddDays((double) value);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentOutOfRangeException(nameof (value), Strings.Date_InvalidAddedOrSubtractedResults);
      }
    }

    public static implicit operator DateTime(Date operand) => operand.dateTime;

    public static implicit operator Date(DateTime operand) => new Date(operand.Year, operand.Month, operand.Day);

    public override string ToString() => this.dateTime.ToString("yyyy-MM-dd", (IFormatProvider) CultureInfo.InvariantCulture);

    public int CompareTo(object obj)
    {
      if (obj is Date other)
        return this.CompareTo(other);
      throw new ArgumentException(Strings.Date_InvalidCompareToTarget(obj));
    }

    public int CompareTo(Date other) => this.dateTime.CompareTo(other.dateTime);

    public bool Equals(Date other) => this.dateTime.Equals(other.dateTime);

    public override bool Equals(object obj) => obj != null && obj is Date date && this.dateTime.Equals(date.dateTime);

    public override int GetHashCode() => this.dateTime.GetHashCode();

    public static Date Parse(string text) => Date.Parse(text, (IFormatProvider) CultureInfo.CurrentCulture);

    public static Date Parse(string text, IFormatProvider provider)
    {
      Date result;
      if (Date.TryParse(text, provider, out result))
        return result;
      throw new FormatException(Strings.Date_InvalidParsingString((object) text));
    }

    public static bool TryParse(string text, out Date result) => Date.TryParse(text, (IFormatProvider) CultureInfo.CurrentCulture, out result);

    public static bool TryParse(string text, IFormatProvider provider, out Date result)
    {
      DateTime result1;
      bool flag = DateTime.TryParse(text, provider, DateTimeStyles.None, out result1);
      result = (Date) result1;
      return flag;
    }
  }
}
