// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.LiteralParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Microsoft.OData.UriParser
{
  internal abstract class LiteralParser
  {
    private static readonly LiteralParser DefaultInstance = (LiteralParser) new LiteralParser.DefaultLiteralParser();
    private static readonly LiteralParser KeysAsSegmentsInstance = (LiteralParser) new LiteralParser.KeysAsSegmentsLiteralParser();
    private static readonly IDictionary<Type, LiteralParser.PrimitiveParser> Parsers = (IDictionary<Type, LiteralParser.PrimitiveParser>) new Dictionary<Type, LiteralParser.PrimitiveParser>((IEqualityComparer<Type>) ReferenceEqualityComparer<Type>.Instance)
    {
      {
        typeof (byte[]),
        (LiteralParser.PrimitiveParser) new LiteralParser.BinaryPrimitiveParser()
      },
      {
        typeof (string),
        (LiteralParser.PrimitiveParser) new LiteralParser.StringPrimitiveParser()
      },
      {
        typeof (Decimal),
        (LiteralParser.PrimitiveParser) new LiteralParser.DecimalPrimitiveParser()
      },
      {
        typeof (Date),
        (LiteralParser.PrimitiveParser) new LiteralParser.DatePrimitiveParser()
      },
      {
        typeof (bool),
        (LiteralParser.PrimitiveParser) LiteralParser.DelegatingPrimitiveParser<bool>.WithoutMarkup(new Func<string, bool>(XmlConvert.ToBoolean))
      },
      {
        typeof (byte),
        (LiteralParser.PrimitiveParser) LiteralParser.DelegatingPrimitiveParser<byte>.WithoutMarkup(new Func<string, byte>(XmlConvert.ToByte))
      },
      {
        typeof (sbyte),
        (LiteralParser.PrimitiveParser) LiteralParser.DelegatingPrimitiveParser<sbyte>.WithoutMarkup(new Func<string, sbyte>(XmlConvert.ToSByte))
      },
      {
        typeof (short),
        (LiteralParser.PrimitiveParser) LiteralParser.DelegatingPrimitiveParser<short>.WithoutMarkup(new Func<string, short>(XmlConvert.ToInt16))
      },
      {
        typeof (int),
        (LiteralParser.PrimitiveParser) LiteralParser.DelegatingPrimitiveParser<int>.WithoutMarkup(new Func<string, int>(XmlConvert.ToInt32))
      },
      {
        typeof (DateTimeOffset),
        (LiteralParser.PrimitiveParser) LiteralParser.DelegatingPrimitiveParser<DateTimeOffset>.WithoutMarkup(new Func<string, DateTimeOffset>(XmlConvert.ToDateTimeOffset))
      },
      {
        typeof (Guid),
        (LiteralParser.PrimitiveParser) LiteralParser.DelegatingPrimitiveParser<Guid>.WithoutMarkup(new Func<string, Guid>(XmlConvert.ToGuid))
      },
      {
        typeof (TimeSpan),
        (LiteralParser.PrimitiveParser) LiteralParser.DelegatingPrimitiveParser<TimeSpan>.WithPrefix(new Func<string, TimeSpan>(EdmValueParser.ParseDuration), "duration")
      },
      {
        typeof (long),
        (LiteralParser.PrimitiveParser) LiteralParser.DelegatingPrimitiveParser<long>.WithSuffix(new Func<string, long>(XmlConvert.ToInt64), "L", false)
      },
      {
        typeof (float),
        (LiteralParser.PrimitiveParser) LiteralParser.DelegatingPrimitiveParser<float>.WithSuffix(new Func<string, float>(XmlConvert.ToSingle), "f", false)
      },
      {
        typeof (double),
        (LiteralParser.PrimitiveParser) LiteralParser.DelegatingPrimitiveParser<double>.WithSuffix(new Func<string, double>(XmlConvert.ToDouble), "D", false)
      }
    };

    internal static LiteralParser ForETags => LiteralParser.DefaultInstance;

    internal static LiteralParser ForKeys(bool keyAsSegment) => !keyAsSegment ? LiteralParser.DefaultInstance : LiteralParser.KeysAsSegmentsInstance;

    internal abstract bool TryParseLiteral(Type targetType, string text, out object result);

    private sealed class DefaultLiteralParser : LiteralParser
    {
      internal override bool TryParseLiteral(Type targetType, string text, out object result)
      {
        Type type = Nullable.GetUnderlyingType(targetType);
        if ((object) type == null)
          type = targetType;
        targetType = type;
        if (LiteralParser.DefaultLiteralParser.TryRemoveFormattingAndConvert(text, typeof (byte[]), out result))
        {
          byte[] bytes = (byte[]) result;
          if (!(targetType == typeof (byte[])))
            return LiteralParser.DefaultLiteralParser.TryRemoveFormattingAndConvert(Encoding.UTF8.GetString(bytes, 0, bytes.Length), targetType, out result);
          result = (object) bytes;
          return true;
        }
        if (!(targetType == typeof (byte[])))
          return LiteralParser.DefaultLiteralParser.TryRemoveFormattingAndConvert(text, targetType, out result);
        result = (object) null;
        return false;
      }

      private static bool TryRemoveFormattingAndConvert(
        string text,
        Type targetType,
        out object targetValue)
      {
        LiteralParser.PrimitiveParser parser = LiteralParser.Parsers[targetType];
        if (parser.TryRemoveFormatting(ref text))
          return parser.TryConvert(text, out targetValue);
        targetValue = (object) null;
        return false;
      }
    }

    private sealed class KeysAsSegmentsLiteralParser : LiteralParser
    {
      internal override bool TryParseLiteral(Type targetType, string text, out object result)
      {
        text = LiteralParser.KeysAsSegmentsLiteralParser.UnescapeLeadingDollarSign(text);
        Type type = Nullable.GetUnderlyingType(targetType);
        if ((object) type == null)
          type = targetType;
        targetType = type;
        return LiteralParser.Parsers[targetType].TryConvert(text, out result);
      }

      private static string UnescapeLeadingDollarSign(string text)
      {
        if (text.Length > 1 && text[0] == '$')
          text = text.Substring(1);
        return text;
      }
    }

    private abstract class PrimitiveParser
    {
      private static readonly char[] XmlWhitespaceChars = new char[4]
      {
        ' ',
        '\t',
        '\n',
        '\r'
      };
      private readonly string prefix;
      private readonly string suffix;
      private readonly bool suffixRequired;
      private readonly Type expectedType;

      protected PrimitiveParser(Type expectedType, string suffix, bool suffixRequired)
        : this(expectedType)
      {
        this.prefix = (string) null;
        this.suffix = suffix;
        this.suffixRequired = suffixRequired;
      }

      protected PrimitiveParser(Type expectedType, string prefix)
        : this(expectedType)
      {
        this.prefix = prefix;
        this.suffix = (string) null;
        this.suffixRequired = false;
      }

      protected PrimitiveParser(Type expectedType) => this.expectedType = expectedType;

      internal abstract bool TryConvert(string text, out object targetValue);

      internal virtual bool TryRemoveFormatting(ref string text) => (this.prefix == null || UriParserHelper.TryRemovePrefix(this.prefix, ref text)) && (this.prefix == null && !LiteralParser.PrimitiveParser.ValueOfTypeCanContainQuotes(this.expectedType) || UriParserHelper.TryRemoveQuotes(ref text)) && (this.suffix == null || LiteralParser.PrimitiveParser.TryRemoveLiteralSuffix(this.suffix, ref text) || !this.suffixRequired);

      internal static bool TryRemoveLiteralSuffix(string suffix, ref string text)
      {
        text = text.Trim(LiteralParser.PrimitiveParser.XmlWhitespaceChars);
        if (text.Length <= suffix.Length || LiteralParser.PrimitiveParser.IsValidNumericConstant(text) || !text.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
          return false;
        text = text.Substring(0, text.Length - suffix.Length);
        return true;
      }

      private static bool ValueOfTypeCanContainQuotes(Type type) => type == typeof (string);

      private static bool IsValidNumericConstant(string text) => string.Equals(text, "INF", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "-INF", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "NaN", StringComparison.OrdinalIgnoreCase);
    }

    private class DelegatingPrimitiveParser<T> : LiteralParser.PrimitiveParser
    {
      private readonly Func<string, T> convertMethod;

      protected DelegatingPrimitiveParser(
        Func<string, T> convertMethod,
        string suffix,
        bool suffixRequired)
        : base(typeof (T), suffix, suffixRequired)
      {
        this.convertMethod = convertMethod;
      }

      private DelegatingPrimitiveParser(Func<string, T> convertMethod)
        : base(typeof (T))
      {
        this.convertMethod = convertMethod;
      }

      private DelegatingPrimitiveParser(Func<string, T> convertMethod, string prefix)
        : base(typeof (T), prefix)
      {
        this.convertMethod = convertMethod;
      }

      internal static LiteralParser.DelegatingPrimitiveParser<T> WithoutMarkup(
        Func<string, T> convertMethod)
      {
        return new LiteralParser.DelegatingPrimitiveParser<T>(convertMethod);
      }

      internal static LiteralParser.DelegatingPrimitiveParser<T> WithPrefix(
        Func<string, T> convertMethod,
        string prefix)
      {
        return new LiteralParser.DelegatingPrimitiveParser<T>(convertMethod, prefix);
      }

      internal static LiteralParser.DelegatingPrimitiveParser<T> WithSuffix(
        Func<string, T> convertMethod,
        string suffix)
      {
        return LiteralParser.DelegatingPrimitiveParser<T>.WithSuffix(convertMethod, suffix, true);
      }

      internal static LiteralParser.DelegatingPrimitiveParser<T> WithSuffix(
        Func<string, T> convertMethod,
        string suffix,
        bool required)
      {
        return new LiteralParser.DelegatingPrimitiveParser<T>(convertMethod, suffix, required);
      }

      internal override bool TryConvert(string text, out object targetValue)
      {
        try
        {
          targetValue = (object) this.convertMethod(text);
          return true;
        }
        catch (FormatException ex)
        {
          targetValue = (object) default (T);
          return false;
        }
        catch (OverflowException ex)
        {
          targetValue = (object) default (T);
          return false;
        }
      }
    }

    private sealed class DecimalPrimitiveParser : LiteralParser.DelegatingPrimitiveParser<Decimal>
    {
      internal DecimalPrimitiveParser()
        : base(new Func<string, Decimal>(LiteralParser.DecimalPrimitiveParser.ConvertDecimal), "M", false)
      {
      }

      private static Decimal ConvertDecimal(string text)
      {
        try
        {
          return XmlConvert.ToDecimal(text);
        }
        catch (FormatException ex)
        {
          Decimal result;
          if (Decimal.TryParse(text, NumberStyles.Float, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
            return result;
          throw;
        }
      }
    }

    private sealed class BinaryPrimitiveParser : LiteralParser.PrimitiveParser
    {
      internal BinaryPrimitiveParser()
        : base(typeof (byte[]))
      {
      }

      internal override bool TryConvert(string text, out object targetValue)
      {
        try
        {
          targetValue = (object) Convert.FromBase64String(text);
        }
        catch (FormatException ex)
        {
          targetValue = (object) null;
          return false;
        }
        return true;
      }

      internal override bool TryRemoveFormatting(ref string text) => UriParserHelper.TryRemovePrefix("binary", ref text) && UriParserHelper.TryRemoveQuotes(ref text);
    }

    private sealed class StringPrimitiveParser : LiteralParser.PrimitiveParser
    {
      public StringPrimitiveParser()
        : base(typeof (string))
      {
      }

      internal override bool TryConvert(string text, out object targetValue)
      {
        targetValue = (object) text;
        return true;
      }

      internal override bool TryRemoveFormatting(ref string text) => UriParserHelper.TryRemoveQuotes(ref text);
    }

    private sealed class DatePrimitiveParser : LiteralParser.PrimitiveParser
    {
      public DatePrimitiveParser()
        : base(typeof (Date))
      {
      }

      internal override bool TryConvert(string text, out object targetValue)
      {
        Date? result;
        bool date = EdmValueParser.TryParseDate(text, out result);
        targetValue = (object) result;
        return date;
      }
    }
  }
}
