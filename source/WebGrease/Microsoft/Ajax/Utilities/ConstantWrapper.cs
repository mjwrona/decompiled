// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ConstantWrapper
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Ajax.Utilities
{
  public class ConstantWrapper : Expression
  {
    private static Regex s_hexNumberFormat = new Regex("^\\s*(?<sign>[-+])?0X(?<hex>[0-9a-f]+)\\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static Regex s_aspNetSubstitution = new Regex("\\<%.*%\\>", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public bool MayHaveIssues { get; set; }

    public object Value { get; set; }

    public PrimitiveType PrimitiveType { get; set; }

    public override bool IsConstant => true;

    public bool IsNumericLiteral => this.PrimitiveType == PrimitiveType.Number;

    public bool IsFiniteNumericLiteral => this.IsNumericLiteral && !double.IsNaN((double) this.Value) && !double.IsInfinity((double) this.Value);

    public bool IsIntegerLiteral
    {
      get
      {
        try
        {
          return this.IsFiniteNumericLiteral && this.ToInteger() == (double) this.Value;
        }
        catch (InvalidCastException ex)
        {
          return false;
        }
      }
    }

    public bool IsExactInteger => this.IsIntegerLiteral && Math.Abs((double) this.Value) <= 9.00719925474099E+15;

    public bool IsNaN => this.IsNumericLiteral && double.IsNaN((double) this.Value);

    public bool IsInfinity => this.IsNumericLiteral && double.IsInfinity((double) this.Value);

    public bool IsZero => this.IsNumericLiteral && (double) this.Value == 0.0;

    public bool IsBooleanLiteral => this.PrimitiveType == PrimitiveType.Boolean;

    public bool IsStringLiteral => this.PrimitiveType == PrimitiveType.String;

    public bool IsParameterToRegExp { get; set; }

    public bool IsSpecialNumeric
    {
      get
      {
        bool isSpecialNumeric = false;
        if (this.IsNumericLiteral)
        {
          double d = (double) this.Value;
          isSpecialNumeric = double.IsNaN(d) || double.IsInfinity(d);
        }
        return isSpecialNumeric;
      }
    }

    public bool IsOtherDecimal => this.PrimitiveType == PrimitiveType.Other && this.Value != null && ConstantWrapper.IsOnlyDecimalDigits(this.Value.ToString());

    public bool StringContainsAspNetReplacement => this.IsStringLiteral && ConstantWrapper.s_aspNetSubstitution.IsMatch((string) this.Value);

    public ConstantWrapper(object value, PrimitiveType primitiveType, Context context)
      : base(context)
    {
      this.PrimitiveType = primitiveType;
      this.Value = primitiveType == PrimitiveType.Number ? (object) Convert.ToDouble(value, (IFormatProvider) CultureInfo.InvariantCulture) : value;
    }

    public override bool IsEquivalentTo(AstNode otherNode)
    {
      if (otherNode is ConstantWrapper constantWrapper && this.PrimitiveType == constantWrapper.PrimitiveType)
      {
        switch (this.PrimitiveType)
        {
          case PrimitiveType.Null:
            return true;
          case PrimitiveType.Boolean:
            return this.ToBoolean() == constantWrapper.ToBoolean();
          case PrimitiveType.Number:
            return this.ToNumber() == constantWrapper.ToNumber();
          case PrimitiveType.String:
            return string.CompareOrdinal(this.Value.ToString(), constantWrapper.ToString()) == 0;
          case PrimitiveType.Other:
            return false;
        }
      }
      return false;
    }

    public override PrimitiveType FindPrimitiveType() => this.PrimitiveType;

    public override void Accept(IVisitor visitor) => visitor?.Visit(this);

    private static void AddEscape(string unescapedRun, string escapedText, ref StringBuilder sb)
    {
      if (sb == null)
        sb = new StringBuilder();
      sb.Append(unescapedRun);
      sb.Append(escapedText);
    }

    public static string EscapeString(
      string text,
      bool isRegularExpression,
      bool useW3Strict,
      bool useStrict)
    {
      char ch1 = ConstantWrapper.OkayToDoubleQuote(text) ? '"' : '\'';
      StringBuilder sb = (StringBuilder) null;
      int startIndex = 0;
      if (!string.IsNullOrEmpty(text))
      {
        for (int index = 0; index < text.Length; ++index)
        {
          char ch2 = text[index];
          switch (ch2)
          {
            case '\b':
              ConstantWrapper.AddEscape(text.Substring(startIndex, index - startIndex), isRegularExpression ? "\\x08" : "\\b", ref sb);
              startIndex = index + 1;
              break;
            case '\t':
              ConstantWrapper.AddEscape(text.Substring(startIndex, index - startIndex), isRegularExpression ? "\\x09" : "\\t", ref sb);
              startIndex = index + 1;
              break;
            case '\n':
              ConstantWrapper.AddEscape(text.Substring(startIndex, index - startIndex), isRegularExpression ? "\\x0a" : "\\n", ref sb);
              startIndex = index + 1;
              break;
            case '\v':
              if (useW3Strict)
              {
                ConstantWrapper.AddEscape(text.Substring(startIndex, index - startIndex), isRegularExpression ? "\\x0b" : "\\v", ref sb);
                startIndex = index + 1;
                break;
              }
              goto default;
            case '\f':
              ConstantWrapper.AddEscape(text.Substring(startIndex, index - startIndex), isRegularExpression ? "\\x0c" : "\\f", ref sb);
              startIndex = index + 1;
              break;
            case '\r':
              ConstantWrapper.AddEscape(text.Substring(startIndex, index - startIndex), isRegularExpression ? "\\x0d" : "\\r", ref sb);
              startIndex = index + 1;
              break;
            case '"':
            case '\'':
              if ((int) ch1 == (int) ch2)
              {
                ConstantWrapper.AddEscape(text.Substring(startIndex, index - startIndex), "\\", ref sb);
                sb.Append(ch2);
                startIndex = index + 1;
                break;
              }
              break;
            case '\\':
              ConstantWrapper.AddEscape(text.Substring(startIndex, index - startIndex), "\\\\", ref sb);
              startIndex = index + 1;
              break;
            case '\u2028':
            case '\u2029':
              ConstantWrapper.AddEscape(text.Substring(startIndex, index - startIndex), "\\u", ref sb);
              sb.Append("{0:x}".FormatInvariant((object) (int) ch2));
              startIndex = index + 1;
              break;
            default:
              if ((' ' > ch2 || ch2 > '~') && ch2 < ' ')
              {
                if (isRegularExpression || useStrict)
                {
                  ConstantWrapper.AddEscape(text.Substring(startIndex, index - startIndex), "\\x{0:x2}".FormatInvariant((object) (int) ch2), ref sb);
                  startIndex = index + 1;
                  break;
                }
                ConstantWrapper.AddEscape(text.Substring(startIndex, index - startIndex), "\\", ref sb);
                int number = (int) ch2;
                if (number < 8)
                {
                  sb.Append(number.ToStringInvariant());
                }
                else
                {
                  sb.Append((number / 8).ToStringInvariant());
                  sb.Append((number % 8).ToStringInvariant());
                }
                startIndex = index + 1;
                break;
              }
              break;
          }
        }
      }
      string str;
      if (sb == null || string.IsNullOrEmpty(text))
      {
        str = text ?? string.Empty;
      }
      else
      {
        if (startIndex < text.Length)
          sb.Append(text.Substring(startIndex));
        str = sb.ToString();
      }
      return ch1.ToString() + str + (object) ch1;
    }

    private static bool OkayToDoubleQuote(string text)
    {
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < text.Length; ++index)
      {
        switch (text[index])
        {
          case '"':
            ++num1;
            break;
          case '\'':
            ++num2;
            break;
        }
      }
      return num1 <= num2;
    }

    public double ToNumber()
    {
      switch (this.PrimitiveType)
      {
        case PrimitiveType.Null:
          return 0.0;
        case PrimitiveType.Boolean:
          return (bool) this.Value ? 1.0 : 0.0;
        case PrimitiveType.Number:
          return (double) this.Value;
        case PrimitiveType.Other:
          throw new InvalidCastException("Cannot convert 'other' primitives to number");
        default:
          try
          {
            string str1 = this.Value.ToString();
            if (str1 == null || string.IsNullOrEmpty(str1.Trim()))
              return 0.0;
            if (this.MayHaveIssues)
              throw new InvalidCastException("cross-browser conversion issues");
            Match match;
            if (!(match = ConstantWrapper.s_hexNumberFormat.Match(str1)).Success)
              return double.Parse(str1, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(match.Result("${sign}")))
              throw new InvalidCastException("Cross-browser error converting signed hex string to number");
            double d = 0.0;
            string str2 = match.Result("${hex}");
            for (int index = 0; index < str2.Length && !double.IsInfinity(d); ++index)
            {
              char ch = str2[index];
              d = d * 16.0 + (ch <= '9' ? (double) ((int) ch & 15) : (double) (((int) ch & 15) + 9));
            }
            return d;
          }
          catch (FormatException ex)
          {
            return double.NaN;
          }
          catch (OverflowException ex)
          {
            return new Regex("^\\s*-").IsMatch(this.Value.ToString()) ? double.NegativeInfinity : double.PositiveInfinity;
          }
      }
    }

    public bool IsOkayToCombine
    {
      get
      {
        bool isOkayToCombine = !this.IsStringLiteral && !this.IsNumericLiteral || this.IsNumericLiteral && !this.MayHaveIssues && ConstantWrapper.NumberIsOkayToCombine((double) this.Value) || this.IsStringLiteral && !this.MayHaveIssues;
        if (isOkayToCombine && this.IsStringLiteral && ConstantWrapper.s_aspNetSubstitution.IsMatch((string) this.Value))
          isOkayToCombine = false;
        return isOkayToCombine;
      }
    }

    public static bool NumberIsOkayToCombine(double numericValue)
    {
      if (double.IsNaN(numericValue) || double.IsInfinity(numericValue))
        return true;
      return -9.00719925474099E+15 <= numericValue && numericValue <= 9.00719925474099E+15 && Math.Floor(numericValue) == numericValue;
    }

    public bool IsNotOneOrPositiveZero
    {
      get
      {
        if (this.IsNumericLiteral)
        {
          double num = (double) this.Value;
          if (num == 1.0 || num == 0.0 && !this.IsNegativeZero)
            return false;
        }
        return true;
      }
    }

    public bool IsNegativeZero => this.IsNumericLiteral && (double) this.Value == 0.0 && 1.0 / (double) this.Value < 0.0;

    internal double ToInteger()
    {
      double number = this.ToNumber();
      if (double.IsNaN(number))
        return 0.0;
      return number == 0.0 || double.IsInfinity(number) ? number : (double) Math.Sign(number) * Math.Floor(Math.Abs(number));
    }

    internal int ToInt32()
    {
      double number = this.ToNumber();
      if (Math.Floor(number) != number || number < (double) int.MinValue || (double) int.MaxValue < number)
        throw new InvalidCastException("Not an integer in the appropriate range; cross-browser issue");
      if (number == 0.0 || double.IsNaN(number) || double.IsInfinity(number))
        return 0;
      long num = Convert.ToInt64(number) % 4294967296L;
      return Convert.ToInt32(num >= 2147483648L ? num - 4294967296L : num);
    }

    internal uint ToUInt32()
    {
      double number = this.ToNumber();
      if (Math.Floor(number) != number || number < 0.0 || (double) uint.MaxValue < number)
        throw new InvalidCastException("Not an integer in the appropriate range; cross-browser issue");
      return number == 0.0 || double.IsNaN(number) || double.IsInfinity(number) ? 0U : (uint) ((ulong) Convert.ToInt64(number) & (ulong) uint.MaxValue);
    }

    public bool ToBoolean()
    {
      switch (this.PrimitiveType)
      {
        case PrimitiveType.Null:
          return false;
        case PrimitiveType.Boolean:
          return (bool) this.Value;
        case PrimitiveType.Number:
          double d = (double) this.Value;
          return d != 0.0 && !double.IsNaN(d);
        case PrimitiveType.Other:
          throw new InvalidCastException("Cannot convert 'other' primitive types to boolean");
        default:
          return !string.IsNullOrEmpty(this.Value.ToString());
      }
    }

    public override string ToString()
    {
      switch (this.PrimitiveType)
      {
        case PrimitiveType.Null:
          return "null";
        case PrimitiveType.Boolean:
          return !(bool) this.Value ? "false" : "true";
        case PrimitiveType.Number:
          double num = (double) this.Value;
          if (num == 0.0)
            return "0";
          if (double.IsNaN(num))
            return "NaN";
          if (double.IsPositiveInfinity(num))
            return "Infinity";
          return double.IsNegativeInfinity(num) ? "-Infinity" : num.ToStringInvariant("R");
        default:
          return this.Value.ToString();
      }
    }

    private static bool IsOnlyDecimalDigits(string text) => text.IfNotNull<string, bool>((Func<string, bool>) (s => !s.Any<char>((Func<char, bool>) (c => !JSScanner.IsDigit(c)))));
  }
}
