// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.ExpressionUtil
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class ExpressionUtil
  {
    internal static object ConvertToCanonicalValue(
      EvaluationOptions options,
      object val,
      out ValueKind kind,
      out object raw,
      out ResultMemory conversionResultMemory)
    {
      if (options != null)
      {
        int? count = options.Converters?.Count;
        int num = 0;
        Converter<object, ConversionResult> converter;
        if (count.GetValueOrDefault() > num & count.HasValue && val != null && options.Converters.TryGetValue(val.GetType(), out converter))
        {
          raw = val;
          ConversionResult conversionResult = converter(val);
          val = conversionResult.Result;
          conversionResultMemory = conversionResult.ResultMemory;
          goto label_4;
        }
      }
      raw = (object) null;
      conversionResultMemory = (ResultMemory) null;
label_4:
      switch (val)
      {
        case null:
          kind = ValueKind.Null;
          return (object) null;
        case IString @string:
          kind = ValueKind.String;
          return (object) @string.GetString();
        case IBoolean boolean:
          kind = ValueKind.Boolean;
          return (object) boolean.GetBoolean();
        case INumber number:
          kind = ValueKind.Number;
          return (object) number.GetNumber();
        case JToken _:
          JToken canonicalValue = val as JToken;
          switch (canonicalValue.Type)
          {
            case JTokenType.Object:
              kind = ValueKind.Object;
              return (object) canonicalValue;
            case JTokenType.Array:
              kind = ValueKind.Array;
              return (object) canonicalValue;
            case JTokenType.Integer:
            case JTokenType.Float:
              kind = ValueKind.Number;
              return (object) canonicalValue.ToObject<Decimal>();
            case JTokenType.String:
              kind = ValueKind.String;
              return (object) canonicalValue.ToObject<string>();
            case JTokenType.Boolean:
              kind = ValueKind.Boolean;
              return (object) canonicalValue.ToObject<bool>();
            case JTokenType.Null:
              kind = ValueKind.Null;
              return (object) null;
            case JTokenType.Date:
              kind = ValueKind.DateTime;
              return (object) (DateTimeOffset) canonicalValue;
          }
          break;
        case string _:
          kind = ValueKind.String;
          return val;
        default:
          if ((object) (val as Version) != null)
          {
            kind = ValueKind.Version;
            return val;
          }
          if (!val.GetType().GetTypeInfo().IsClass)
          {
            switch (val)
            {
              case bool _:
                kind = ValueKind.Boolean;
                return val;
              case DateTimeOffset _:
                kind = ValueKind.DateTime;
                return val;
              case DateTime dateTime:
                kind = ValueKind.DateTime;
                switch (dateTime.Kind)
                {
                  case DateTimeKind.Unspecified:
                    TimeSpan utcOffset = (options?.TimeZone ?? TimeZoneInfo.Local).GetUtcOffset(dateTime);
                    return (object) new DateTimeOffset(dateTime, utcOffset);
                  case DateTimeKind.Utc:
                    return (object) new DateTimeOffset(dateTime);
                  case DateTimeKind.Local:
                    TimeZoneInfo destinationTimeZone = options?.TimeZone ?? TimeZoneInfo.Local;
                    return (object) TimeZoneInfo.ConvertTime(new DateTimeOffset(dateTime), destinationTimeZone);
                  default:
                    throw new NotSupportedException(string.Format("Unexpected DateTimeKind '{0}'", (object) dateTime.Kind));
                }
              case Decimal _:
              case byte _:
              case sbyte _:
              case short _:
              case ushort _:
              case int _:
              case uint _:
              case long _:
              case ulong _:
              case float _:
              case double _:
                kind = ValueKind.Number;
                return (object) Convert.ToDecimal(val);
              case Enum _:
                string s = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:G}", val);
                Decimal result;
                if (Decimal.TryParse(s, NumberStyles.AllowLeadingSign, (IFormatProvider) CultureInfo.InvariantCulture, out result))
                {
                  kind = ValueKind.Number;
                  return (object) result;
                }
                kind = ValueKind.String;
                return (object) s;
            }
          }
          else
            break;
          break;
      }
      kind = ValueKind.Object;
      return val;
    }

    internal static string FormatValue(
      ISecretMasker secretMasker,
      EvaluationResult evaluationResult)
    {
      return ExpressionUtil.FormatValue(secretMasker, evaluationResult.Value, evaluationResult.Kind);
    }

    internal static string FormatValue(ISecretMasker secretMasker, object value, ValueKind kind)
    {
      switch (kind)
      {
        case ValueKind.Array:
        case ValueKind.Null:
        case ValueKind.Object:
          return kind.ToString();
        case ValueKind.Boolean:
          return ((bool) value).ToString();
        case ValueKind.DateTime:
          string input1 = "(DateTime)" + ((DateTimeOffset) value).ToString(ExpressionConstants.DateTimeFormat, (IFormatProvider) CultureInfo.InvariantCulture);
          return secretMasker == null ? input1 : secretMasker.MaskSecrets(input1);
        case ValueKind.Number:
          string input2 = ((Decimal) value).ToString(ExpressionConstants.NumberFormat, (IFormatProvider) CultureInfo.InvariantCulture);
          return secretMasker == null ? input2 : secretMasker.MaskSecrets(input2);
        case ValueKind.String:
          return "'" + ExpressionUtil.StringEscape(secretMasker != null ? secretMasker.MaskSecrets(value as string) : value as string) + "'";
        case ValueKind.Version:
          return "v" + (secretMasker != null ? secretMasker.MaskSecrets(value.ToString()) : value.ToString());
        default:
          throw new NotSupportedException(string.Format("Unable to convert to realized expression. Unexpected value kind: {0}", (object) kind));
      }
    }

    internal static char SafeCharAt(string str, int index) => str.Length > index ? str[index] : char.MinValue;

    internal static string StringEscape(string value) => !string.IsNullOrEmpty(value) ? value.Replace("'", "''") : string.Empty;
  }
}
