// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.UriPrimitiveTypeParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Microsoft.Spatial;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace Microsoft.OData.UriParser
{
  [Guid("A77303D9-3A04-4829-BDDE-3B4D43E21CFC")]
  internal sealed class UriPrimitiveTypeParser : IUriLiteralParser
  {
    private static UriPrimitiveTypeParser singleInstance = new UriPrimitiveTypeParser();

    private UriPrimitiveTypeParser()
    {
    }

    public static UriPrimitiveTypeParser Instance => UriPrimitiveTypeParser.singleInstance;

    public object ParseUriStringToType(
      string text,
      IEdmTypeReference targetType,
      out UriLiteralParsingException parsingException)
    {
      object targetValue;
      return this.TryUriStringToPrimitive(text, targetType, out targetValue, out parsingException) ? targetValue : (object) null;
    }

    internal bool TryParseUriStringToType(
      string text,
      IEdmTypeReference targetType,
      out object targetValue,
      out UriLiteralParsingException parsingException)
    {
      return this.TryUriStringToPrimitive(text, targetType, out targetValue, out parsingException);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity is not too high; handling all the cases in one method is preferable to refactoring.")]
    private bool TryUriStringToPrimitive(
      string text,
      IEdmTypeReference targetType,
      out object targetValue,
      out UriLiteralParsingException exception)
    {
      exception = (UriLiteralParsingException) null;
      try
      {
        if (targetType.IsNullable && text == "null")
        {
          targetValue = (object) null;
          return true;
        }
        IEdmPrimitiveTypeReference type = targetType.AsPrimitiveOrNull();
        if (type == null)
        {
          targetValue = (object) null;
          return false;
        }
        EdmPrimitiveTypeKind primitiveTypeKind = Microsoft.OData.Edm.ExtensionMethods.PrimitiveKind(type);
        byte[] targetValue1;
        bool byteArray = UriPrimitiveTypeParser.TryUriStringToByteArray(text, out targetValue1);
        if (primitiveTypeKind == EdmPrimitiveTypeKind.Binary)
        {
          targetValue = (object) targetValue1;
          return byteArray;
        }
        if (byteArray)
          return this.TryUriStringToPrimitive(Encoding.UTF8.GetString(targetValue1, 0, targetValue1.Length), targetType, out targetValue);
        switch (primitiveTypeKind)
        {
          case EdmPrimitiveTypeKind.DateTimeOffset:
            DateTimeOffset targetValue2;
            bool dateTimeOffset = UriUtils.ConvertUriStringToDateTimeOffset(text, out targetValue2);
            targetValue = (object) targetValue2;
            return dateTimeOffset;
          case EdmPrimitiveTypeKind.Guid:
            Guid targetValue3;
            bool guid = UriUtils.TryUriStringToGuid(text, out targetValue3);
            targetValue = (object) targetValue3;
            return guid;
          case EdmPrimitiveTypeKind.Duration:
            TimeSpan targetValue4;
            bool duration = UriPrimitiveTypeParser.TryUriStringToDuration(text, out targetValue4);
            targetValue = (object) targetValue4;
            return duration;
          case EdmPrimitiveTypeKind.Geography:
            Geography targetValue5;
            bool geography = UriPrimitiveTypeParser.TryUriStringToGeography(text, out targetValue5, out exception);
            targetValue = (object) targetValue5;
            return geography;
          case EdmPrimitiveTypeKind.Geometry:
            Geometry targetValue6;
            bool geometry = UriPrimitiveTypeParser.TryUriStringToGeometry(text, out targetValue6, out exception);
            targetValue = (object) targetValue6;
            return geometry;
          case EdmPrimitiveTypeKind.Date:
            Date targetValue7;
            bool date = UriUtils.TryUriStringToDate(text, out targetValue7);
            targetValue = (object) targetValue7;
            return date;
          case EdmPrimitiveTypeKind.TimeOfDay:
            TimeOfDay targetValue8;
            bool timeOfDay = UriUtils.TryUriStringToTimeOfDay(text, out targetValue8);
            targetValue = (object) targetValue8;
            return timeOfDay;
          default:
            bool flag = primitiveTypeKind == EdmPrimitiveTypeKind.String;
            if (flag != UriParserHelper.IsUriValueQuoted(text))
            {
              targetValue = (object) null;
              return false;
            }
            if (flag)
              text = UriParserHelper.RemoveQuotes(text);
            try
            {
              switch (primitiveTypeKind)
              {
                case EdmPrimitiveTypeKind.Boolean:
                  targetValue = (object) XmlConvert.ToBoolean(text);
                  break;
                case EdmPrimitiveTypeKind.Byte:
                  targetValue = (object) XmlConvert.ToByte(text);
                  break;
                case EdmPrimitiveTypeKind.Decimal:
                  UriParserHelper.TryRemoveLiteralSuffix("M", ref text);
                  try
                  {
                    targetValue = (object) XmlConvert.ToDecimal(text);
                    break;
                  }
                  catch (FormatException ex)
                  {
                    Decimal result;
                    if (Decimal.TryParse(text, NumberStyles.Float, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
                    {
                      targetValue = (object) result;
                      break;
                    }
                    targetValue = (object) 0M;
                    return false;
                  }
                case EdmPrimitiveTypeKind.Double:
                  UriParserHelper.TryRemoveLiteralSuffix("D", ref text);
                  targetValue = (object) XmlConvert.ToDouble(text);
                  break;
                case EdmPrimitiveTypeKind.Int16:
                  targetValue = (object) XmlConvert.ToInt16(text);
                  break;
                case EdmPrimitiveTypeKind.Int32:
                  targetValue = (object) XmlConvert.ToInt32(text);
                  break;
                case EdmPrimitiveTypeKind.Int64:
                  UriParserHelper.TryRemoveLiteralSuffix("L", ref text);
                  targetValue = (object) XmlConvert.ToInt64(text);
                  break;
                case EdmPrimitiveTypeKind.SByte:
                  targetValue = (object) XmlConvert.ToSByte(text);
                  break;
                case EdmPrimitiveTypeKind.Single:
                  UriParserHelper.TryRemoveLiteralSuffix("f", ref text);
                  targetValue = (object) XmlConvert.ToSingle(text);
                  break;
                case EdmPrimitiveTypeKind.String:
                  targetValue = (object) text;
                  break;
                default:
                  throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.UriPrimitiveTypeParser_TryUriStringToPrimitive));
              }
              return true;
            }
            catch (FormatException ex)
            {
              targetValue = (object) null;
              return false;
            }
            catch (OverflowException ex)
            {
              targetValue = (object) null;
              return false;
            }
        }
      }
      catch (Exception ex)
      {
        exception = new UriLiteralParsingException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.OData.Strings.UriPrimitiveTypeParsers_FailedToParseTextToPrimitiveValue((object) text, (object) targetType), new object[1]
        {
          (object) ex
        }));
        targetValue = (object) null;
        return false;
      }
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity is not too high; handling all the cases in one method is preferable to refactoring.")]
    private bool TryUriStringToPrimitive(
      string text,
      IEdmTypeReference targetType,
      out object targetValue)
    {
      return this.TryUriStringToPrimitive(text, targetType, out targetValue, out UriLiteralParsingException _);
    }

    private static bool TryUriStringToByteArray(string text, out byte[] targetValue)
    {
      if (!UriParserHelper.TryRemoveLiteralPrefix("binary", ref text))
      {
        targetValue = (byte[]) null;
        return false;
      }
      if (!UriParserHelper.TryRemoveQuotes(ref text))
      {
        targetValue = (byte[]) null;
        return false;
      }
      try
      {
        targetValue = Convert.FromBase64String(text);
      }
      catch (FormatException ex)
      {
        targetValue = (byte[]) null;
        return false;
      }
      return true;
    }

    private static bool TryUriStringToDuration(string text, out TimeSpan targetValue)
    {
      if (!UriParserHelper.TryRemoveLiteralPrefix("duration", ref text))
      {
        targetValue = new TimeSpan();
        return false;
      }
      if (!UriParserHelper.TryRemoveQuotes(ref text))
      {
        targetValue = new TimeSpan();
        return false;
      }
      try
      {
        targetValue = EdmValueParser.ParseDuration(text);
        return true;
      }
      catch (FormatException ex)
      {
        targetValue = new TimeSpan();
        return false;
      }
    }

    private static bool TryUriStringToGeography(
      string text,
      out Geography targetValue,
      out UriLiteralParsingException parsingException)
    {
      parsingException = (UriLiteralParsingException) null;
      if (!UriParserHelper.TryRemoveLiteralPrefix("geography", ref text))
      {
        targetValue = (Geography) null;
        return false;
      }
      if (!UriParserHelper.TryRemoveQuotes(ref text))
      {
        targetValue = (Geography) null;
        return false;
      }
      try
      {
        targetValue = LiteralUtils.ParseGeography(text);
        return true;
      }
      catch (ParseErrorException ex)
      {
        targetValue = (Geography) null;
        parsingException = new UriLiteralParsingException(ex.Message);
        return false;
      }
    }

    private static bool TryUriStringToGeometry(
      string text,
      out Geometry targetValue,
      out UriLiteralParsingException parsingFailureReasonException)
    {
      parsingFailureReasonException = (UriLiteralParsingException) null;
      if (!UriParserHelper.TryRemoveLiteralPrefix("geometry", ref text))
      {
        targetValue = (Geometry) null;
        return false;
      }
      if (!UriParserHelper.TryRemoveQuotes(ref text))
      {
        targetValue = (Geometry) null;
        return false;
      }
      try
      {
        targetValue = LiteralUtils.ParseGeometry(text);
        return true;
      }
      catch (ParseErrorException ex)
      {
        targetValue = (Geometry) null;
        parsingFailureReasonException = new UriLiteralParsingException(ex.Message);
        return false;
      }
    }
  }
}
