// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.LiteralFormatter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;

namespace Microsoft.OData.Evaluation
{
  internal abstract class LiteralFormatter
  {
    private static readonly LiteralFormatter DefaultInstance = (LiteralFormatter) new LiteralFormatter.DefaultLiteralFormatter();
    private static readonly LiteralFormatter DefaultInstanceWithoutEncoding = (LiteralFormatter) new LiteralFormatter.DefaultLiteralFormatter(true);
    private static readonly LiteralFormatter KeyAsSegmentInstance = (LiteralFormatter) new LiteralFormatter.KeysAsSegmentsLiteralFormatter();

    internal static LiteralFormatter ForConstants => LiteralFormatter.DefaultInstance;

    internal static LiteralFormatter ForConstantsWithoutEncoding => LiteralFormatter.DefaultInstanceWithoutEncoding;

    internal static LiteralFormatter ForKeys(bool keysAsSegment) => !keysAsSegment ? LiteralFormatter.DefaultInstance : LiteralFormatter.KeyAsSegmentInstance;

    internal abstract string Format(object value);

    protected virtual string EscapeResultForUri(string result) => Uri.EscapeDataString(result);

    private static string ConvertByteArrayToKeyString(byte[] byteArray) => Convert.ToBase64String(byteArray, 0, byteArray.Length);

    private static string FormatRawLiteral(object value)
    {
      switch (value)
      {
        case string str:
          return str;
        case bool flag:
          return XmlConvert.ToString(flag);
        case byte num1:
          return XmlConvert.ToString(num1);
        case Decimal num2:
          return XmlConvert.ToString(num2);
        case double num3:
          return LiteralFormatter.SharedUtils.AppendDecimalMarkerToDouble(XmlConvert.ToString(num3));
        case Guid _:
          return value.ToString();
        case short num4:
          return XmlConvert.ToString(num4);
        case int num5:
          return XmlConvert.ToString(num5);
        case long num6:
          return XmlConvert.ToString(num6);
        case sbyte num7:
          return XmlConvert.ToString(num7);
        case float num8:
          return XmlConvert.ToString(num8);
        case byte[] byteArray:
          return LiteralFormatter.ConvertByteArrayToKeyString(byteArray);
        case Date _:
          return value.ToString();
        case DateTimeOffset dateTimeOffset:
          return XmlConvert.ToString(dateTimeOffset);
        case TimeOfDay _:
          return value.ToString();
        case TimeSpan d:
          return EdmValueWriter.DurationAsXml(d);
        case Geography geography:
          return WellKnownTextSqlFormatter.Create(true).Write((ISpatial) geography);
        case Geometry geometry:
          return WellKnownTextSqlFormatter.Create(true).Write((ISpatial) geometry);
        case ODataEnumValue odataEnumValue:
          return odataEnumValue.Value;
        default:
          throw LiteralFormatter.SharedUtils.CreateExceptionForUnconvertableType(value);
      }
    }

    private string FormatAndEscapeLiteral(object value)
    {
      string result = LiteralFormatter.FormatRawLiteral(value);
      if (value is string)
        result = result.Replace("'", "''");
      return this.EscapeResultForUri(result);
    }

    private static class SharedUtils
    {
      internal static InvalidOperationException CreateExceptionForUnconvertableType(object value) => (InvalidOperationException) new ODataException(Microsoft.OData.Strings.ODataUriUtils_ConvertToUriLiteralUnsupportedType((object) value.GetType().ToString()));

      internal static bool TryConvertToStandardType(object value, out object converted)
      {
        byte[] array;
        if (LiteralFormatter.SharedUtils.TryGetByteArrayFromBinary(value, out array))
        {
          converted = (object) array;
          return true;
        }
        converted = (object) null;
        return false;
      }

      internal static string AppendDecimalMarkerToDouble(string input)
      {
        IEnumerable<char> source = (IEnumerable<char>) input.ToCharArray();
        if (input[0] == '-')
          source = source.Skip<char>(1);
        return source.All<char>(new Func<char, bool>(char.IsDigit)) ? input + ".0" : input;
      }

      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value", Justification = "Method is compiled into 3 assemblies, and the parameter is used in 2 of them.")]
      private static bool TryGetByteArrayFromBinary(object value, out byte[] array)
      {
        array = (byte[]) null;
        return false;
      }
    }

    private sealed class DefaultLiteralFormatter : LiteralFormatter
    {
      private readonly bool disableUrlEncoding;

      internal DefaultLiteralFormatter()
        : this(false)
      {
      }

      internal DefaultLiteralFormatter(bool disableUrlEncoding) => this.disableUrlEncoding = disableUrlEncoding;

      internal override string Format(object value)
      {
        object converted;
        if (LiteralFormatter.SharedUtils.TryConvertToStandardType(value, out converted))
          value = converted;
        return this.FormatLiteralWithTypePrefix(value);
      }

      protected override string EscapeResultForUri(string result)
      {
        if (!this.disableUrlEncoding)
          result = base.EscapeResultForUri(result);
        return result;
      }

      private string FormatLiteralWithTypePrefix(object value)
      {
        if (value is ODataEnumValue odataEnumValue)
        {
          if (string.IsNullOrEmpty(odataEnumValue.TypeName))
            throw new ODataException("Type name should not be null or empty when serializing an Enum value for URI key.");
          return odataEnumValue.TypeName + "'" + this.FormatAndEscapeLiteral((object) odataEnumValue.Value) + "'";
        }
        string str = this.FormatAndEscapeLiteral(value);
        if (value is byte[])
          return "binary'" + str + "'";
        if (value is Geography)
          return "geography'" + str + "'";
        if (value is Geometry)
          return "geometry'" + str + "'";
        if (value is TimeSpan)
          return "duration'" + str + "'";
        return value is string ? "'" + str + "'" : str;
      }
    }

    private sealed class KeysAsSegmentsLiteralFormatter : LiteralFormatter
    {
      internal KeysAsSegmentsLiteralFormatter()
      {
      }

      internal override string Format(object value)
      {
        if (value is ODataEnumValue odataEnumValue)
          value = (object) odataEnumValue.Value;
        object converted;
        if (LiteralFormatter.SharedUtils.TryConvertToStandardType(value, out converted))
          value = converted;
        if (value is string stringValue)
          value = (object) LiteralFormatter.KeysAsSegmentsLiteralFormatter.EscapeLeadingDollarSign(stringValue);
        return this.FormatAndEscapeLiteral(value);
      }

      private static string EscapeLeadingDollarSign(string stringValue)
      {
        if (stringValue.Length > 0 && stringValue[0] == '$')
          stringValue = "$" + stringValue;
        return stringValue;
      }
    }
  }
}
