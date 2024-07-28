// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataRawValueUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Xml;

namespace Microsoft.OData
{
  internal static class ODataRawValueUtils
  {
    private static readonly char[] XmlWhitespaceChars = new char[4]
    {
      ' ',
      '\t',
      '\n',
      '\r'
    };

    internal static bool TryConvertPrimitiveToString(object value, out string result)
    {
      switch (value)
      {
        case bool b1:
          result = ODataRawValueConverter.ToString(b1);
          return true;
        case byte b2:
          result = ODataRawValueConverter.ToString(b2);
          return true;
        case Decimal d1:
          result = ODataRawValueConverter.ToString(d1);
          return true;
        case double d2:
          result = ODataRawValueConverter.ToString(d2);
          return true;
        case short i1:
          result = ODataRawValueConverter.ToString(i1);
          return true;
        case int i2:
          result = ODataRawValueConverter.ToString(i2);
          return true;
        case long i3:
          result = ODataRawValueConverter.ToString(i3);
          return true;
        case sbyte sb:
          result = ODataRawValueConverter.ToString(sb);
          return true;
        case string str:
          result = str;
          return true;
        case float s:
          result = ODataRawValueConverter.ToString(s);
          return true;
        case byte[] bytes:
          result = ODataRawValueConverter.ToString(bytes);
          return true;
        case DateTimeOffset dateTime:
          result = ODataRawValueConverter.ToString(dateTime);
          return true;
        case Guid guid:
          result = ODataRawValueConverter.ToString(guid);
          return true;
        case TimeSpan ts:
          result = ODataRawValueConverter.ToString(ts);
          return true;
        case Date date:
          result = ODataRawValueConverter.ToString(date);
          return true;
        case TimeOfDay time:
          result = ODataRawValueConverter.ToString(time);
          return true;
        default:
          result = (string) null;
          return false;
      }
    }

    internal static object ConvertStringToPrimitive(
      string text,
      IEdmPrimitiveTypeReference targetTypeReference)
    {
      try
      {
        switch (ExtensionMethods.PrimitiveKind(targetTypeReference))
        {
          case EdmPrimitiveTypeKind.Binary:
            return (object) Convert.FromBase64String(text);
          case EdmPrimitiveTypeKind.Boolean:
            return (object) ODataRawValueUtils.ConvertXmlBooleanValue(text);
          case EdmPrimitiveTypeKind.Byte:
            return (object) XmlConvert.ToByte(text);
          case EdmPrimitiveTypeKind.DateTimeOffset:
            return (object) PlatformHelper.ConvertStringToDateTimeOffset(text);
          case EdmPrimitiveTypeKind.Decimal:
            return (object) XmlConvert.ToDecimal(text);
          case EdmPrimitiveTypeKind.Double:
            return (object) XmlConvert.ToDouble(text);
          case EdmPrimitiveTypeKind.Guid:
            return (object) new Guid(text);
          case EdmPrimitiveTypeKind.Int16:
            return (object) XmlConvert.ToInt16(text);
          case EdmPrimitiveTypeKind.Int32:
            return (object) XmlConvert.ToInt32(text);
          case EdmPrimitiveTypeKind.Int64:
            return (object) XmlConvert.ToInt64(text);
          case EdmPrimitiveTypeKind.SByte:
            return (object) XmlConvert.ToSByte(text);
          case EdmPrimitiveTypeKind.Single:
            return (object) XmlConvert.ToSingle(text);
          case EdmPrimitiveTypeKind.String:
          case EdmPrimitiveTypeKind.PrimitiveType:
            return (object) text;
          case EdmPrimitiveTypeKind.Duration:
            return (object) EdmValueParser.ParseDuration(text);
          case EdmPrimitiveTypeKind.Date:
            return (object) PlatformHelper.ConvertStringToDate(text);
          case EdmPrimitiveTypeKind.TimeOfDay:
            return (object) PlatformHelper.ConvertStringToTimeOfDay(text);
          default:
            throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataRawValueUtils_ConvertStringToPrimitive));
        }
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          throw ReaderValidationUtils.GetPrimitiveTypeConversionException(targetTypeReference, ex, text);
        throw;
      }
    }

    private static bool ConvertXmlBooleanValue(string text)
    {
      text = text.Trim(ODataRawValueUtils.XmlWhitespaceChars);
      switch (text)
      {
        case "true":
        case "True":
        case "1":
          return true;
        case "false":
        case "False":
        case "0":
          return false;
        default:
          return XmlConvert.ToBoolean(text);
      }
    }
  }
}
