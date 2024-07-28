// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.EdmValueParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.OData
{
  internal static class EdmValueParser
  {
    internal static readonly Regex DayTimeDurationValidator = PlatformHelper.CreateCompiled("^[^YM]*[DT].*$", RegexOptions.Singleline);

    internal static TimeSpan ParseDuration(string value) => value != null && EdmValueParser.DayTimeDurationValidator.IsMatch(value) ? XmlConvert.ToTimeSpan(value) : throw new FormatException(Strings.ValueParser_InvalidDuration((object) value));

    internal static bool TryParseBinary(string value, out byte[] result)
    {
      if (value.Length % 2 != 0)
      {
        result = (byte[]) null;
        return false;
      }
      result = new byte[value.Length >> 1];
      int num;
      for (int index = 0; index < value.Length; index = num + 1)
      {
        byte b1;
        if (!EdmValueParser.TryParseCharAsBinary(value[index], out b1))
        {
          result = (byte[]) null;
          return false;
        }
        byte b2;
        if (!EdmValueParser.TryParseCharAsBinary(value[num = index + 1], out b2))
        {
          result = (byte[]) null;
          return false;
        }
        result[num >> 1] = (byte) ((uint) b1 << 4 | (uint) b2);
      }
      return true;
    }

    internal static bool TryParseBool(string value, out bool? result)
    {
      switch (value.Length)
      {
        case 1:
          switch (value[0])
          {
            case '0':
              result = new bool?(false);
              return true;
            case '1':
              result = new bool?(true);
              return true;
          }
          break;
        case 4:
          if ((value[0] == 't' || value[0] == 'T') && (value[1] == 'r' || value[1] == 'R') && (value[2] == 'u' || value[2] == 'U') && (value[3] == 'e' || value[3] == 'E'))
          {
            result = new bool?(true);
            return true;
          }
          break;
        case 5:
          if ((value[0] == 'f' || value[0] == 'F') && (value[1] == 'a' || value[1] == 'A') && (value[2] == 'l' || value[2] == 'L') && (value[3] == 's' || value[3] == 'S') && (value[4] == 'e' || value[4] == 'E'))
          {
            result = new bool?(false);
            return true;
          }
          break;
      }
      result = new bool?();
      return false;
    }

    internal static bool TryParseDuration(string value, out TimeSpan? result)
    {
      try
      {
        result = new TimeSpan?(EdmValueParser.ParseDuration(value));
        return true;
      }
      catch (FormatException ex)
      {
        result = new TimeSpan?();
        return false;
      }
      catch (OverflowException ex)
      {
        result = new TimeSpan?();
        return false;
      }
    }

    internal static bool TryParseDateTimeOffset(string value, out DateTimeOffset? result)
    {
      try
      {
        result = new DateTimeOffset?(PlatformHelper.ConvertStringToDateTimeOffset(value));
        return true;
      }
      catch (FormatException ex)
      {
        result = new DateTimeOffset?();
        return false;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        result = new DateTimeOffset?();
        return false;
      }
    }

    internal static bool TryParseInt(string value, out int? result)
    {
      try
      {
        result = new int?(XmlConvert.ToInt32(value));
        return true;
      }
      catch (FormatException ex)
      {
        result = new int?();
        return false;
      }
      catch (OverflowException ex)
      {
        result = new int?();
        return false;
      }
    }

    internal static bool TryParseLong(string value, out long? result)
    {
      try
      {
        result = new long?(XmlConvert.ToInt64(value));
        return true;
      }
      catch (FormatException ex)
      {
        result = new long?();
        return false;
      }
      catch (OverflowException ex)
      {
        result = new long?();
        return false;
      }
    }

    internal static bool TryParseDecimal(string value, out Decimal? result)
    {
      try
      {
        result = new Decimal?(XmlConvert.ToDecimal(value));
        return true;
      }
      catch (FormatException ex)
      {
        result = new Decimal?();
        return false;
      }
      catch (OverflowException ex)
      {
        result = new Decimal?();
        return false;
      }
    }

    internal static bool TryParseFloat(string value, out double? result)
    {
      try
      {
        result = new double?(XmlConvert.ToDouble(value));
        return true;
      }
      catch (FormatException ex)
      {
        result = new double?();
        return false;
      }
      catch (OverflowException ex)
      {
        result = new double?();
        return false;
      }
    }

    internal static bool TryParseGuid(string value, out Guid? result)
    {
      try
      {
        result = new Guid?(XmlConvert.ToGuid(value));
        return true;
      }
      catch (FormatException ex)
      {
        result = new Guid?();
        return false;
      }
    }

    internal static bool TryParseDate(string value, out Date? result)
    {
      result = new Date?();
      Date date;
      if (!PlatformHelper.TryConvertStringToDate(value, out date))
        return false;
      result = new Date?(date);
      return true;
    }

    internal static bool TryParseTimeOfDay(string value, out TimeOfDay? result)
    {
      try
      {
        result = new TimeOfDay?(PlatformHelper.ConvertStringToTimeOfDay(value));
        return true;
      }
      catch (FormatException ex)
      {
        result = new TimeOfDay?();
        return false;
      }
    }

    private static bool TryParseCharAsBinary(char c, out byte b)
    {
      uint num1 = (uint) c - 48U;
      switch (num1)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
          b = (byte) num1;
          return true;
        default:
          uint num2 = (uint) c - 65U;
          if (num2 < 0U || num2 > 5U)
            num2 = (uint) c - 97U;
          if (num2 >= 0U && num2 <= 5U)
          {
            b = (byte) (num2 + 10U);
            return true;
          }
          b = (byte) 0;
          return false;
      }
    }
  }
}
