// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.NodeDeserializers.ScalarNodeDeserializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
  public sealed class ScalarNodeDeserializer : INodeDeserializer
  {
    private const string BooleanTruePattern = "^(true|y|yes|on)$";
    private const string BooleanFalsePattern = "^(false|n|no|off)$";

    bool INodeDeserializer.Deserialize(
      IParser parser,
      Type expectedType,
      Func<IParser, Type, object> nestedObjectDeserializer,
      out object value)
    {
      Scalar scalar = parser.Allow<Scalar>();
      if (scalar == null)
      {
        value = (object) null;
        return false;
      }
      if (expectedType.IsEnum())
      {
        value = Enum.Parse(expectedType, scalar.Value, true);
      }
      else
      {
        TypeCode typeCode = expectedType.GetTypeCode();
        switch (typeCode)
        {
          case TypeCode.Boolean:
            value = this.DeserializeBooleanHelper(scalar.Value);
            break;
          case TypeCode.Char:
            value = (object) scalar.Value[0];
            break;
          case TypeCode.SByte:
          case TypeCode.Byte:
          case TypeCode.Int16:
          case TypeCode.UInt16:
          case TypeCode.Int32:
          case TypeCode.UInt32:
          case TypeCode.Int64:
          case TypeCode.UInt64:
            value = this.DeserializeIntegerHelper(typeCode, scalar.Value);
            break;
          case TypeCode.Single:
            value = (object) float.Parse(scalar.Value, (IFormatProvider) YamlFormatter.NumberFormat);
            break;
          case TypeCode.Double:
            value = (object) double.Parse(scalar.Value, (IFormatProvider) YamlFormatter.NumberFormat);
            break;
          case TypeCode.Decimal:
            value = (object) Decimal.Parse(scalar.Value, (IFormatProvider) YamlFormatter.NumberFormat);
            break;
          case TypeCode.DateTime:
            value = (object) DateTime.Parse(scalar.Value, (IFormatProvider) CultureInfo.InvariantCulture);
            break;
          case TypeCode.String:
            value = (object) scalar.Value;
            break;
          default:
            value = (object) expectedType != (object) typeof (object) ? TypeConverter.ChangeType((object) scalar.Value, expectedType) : (object) scalar.Value;
            break;
        }
      }
      return true;
    }

    private object DeserializeBooleanHelper(string value)
    {
      bool flag;
      if (Regex.IsMatch(value, "^(true|y|yes|on)$", RegexOptions.IgnoreCase))
      {
        flag = true;
      }
      else
      {
        if (!Regex.IsMatch(value, "^(false|n|no|off)$", RegexOptions.IgnoreCase))
          throw new FormatException(string.Format("The value \"{0}\" is not a valid YAML Boolean", (object) value));
        flag = false;
      }
      return (object) flag;
    }

    private object DeserializeIntegerHelper(TypeCode typeCode, string value)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      bool flag = false;
      ulong number = 0;
      if (value[0] == '-')
      {
        ++num;
        flag = true;
      }
      else if (value[0] == '+')
        ++num;
      if (value[num] == '0')
      {
        int fromBase;
        if (num == value.Length - 1)
        {
          fromBase = 10;
          number = 0UL;
        }
        else
        {
          ++num;
          if (value[num] == 'b')
          {
            fromBase = 2;
            ++num;
          }
          else if (value[num] == 'x')
          {
            fromBase = 16;
            ++num;
          }
          else
            fromBase = 8;
        }
        for (; num < value.Length; ++num)
        {
          if (value[num] != '_')
            stringBuilder.Append(value[num]);
        }
        switch (fromBase)
        {
          case 2:
          case 8:
            number = Convert.ToUInt64(stringBuilder.ToString(), fromBase);
            break;
          case 16:
            number = ulong.Parse(stringBuilder.ToString(), NumberStyles.HexNumber, (IFormatProvider) YamlFormatter.NumberFormat);
            break;
        }
      }
      else
      {
        string[] strArray = value.Substring(num).Split(':');
        number = 0UL;
        for (int index = 0; index < strArray.Length; ++index)
          number = number * 60UL + ulong.Parse(strArray[index].Replace("_", ""));
      }
      return flag ? ScalarNodeDeserializer.CastInteger(checked (-(long) number), typeCode) : ScalarNodeDeserializer.CastInteger(number, typeCode);
    }

    private static object CastInteger(long number, TypeCode typeCode)
    {
      switch (typeCode)
      {
        case TypeCode.SByte:
          return (object) checked ((sbyte) number);
        case TypeCode.Byte:
          return (object) checked ((byte) number);
        case TypeCode.Int16:
          return (object) checked ((short) number);
        case TypeCode.UInt16:
          return (object) checked ((ushort) number);
        case TypeCode.Int32:
          return (object) checked ((int) number);
        case TypeCode.UInt32:
          return (object) checked ((uint) number);
        case TypeCode.Int64:
          return (object) number;
        case TypeCode.UInt64:
          return (object) checked ((ulong) number);
        default:
          return (object) number;
      }
    }

    private static object CastInteger(ulong number, TypeCode typeCode)
    {
      switch (typeCode)
      {
        case TypeCode.SByte:
          return (object) checked ((sbyte) number);
        case TypeCode.Byte:
          return (object) checked ((byte) number);
        case TypeCode.Int16:
          return (object) checked ((short) number);
        case TypeCode.UInt16:
          return (object) checked ((ushort) number);
        case TypeCode.Int32:
          return (object) checked ((int) number);
        case TypeCode.UInt32:
          return (object) checked ((uint) number);
        case TypeCode.Int64:
          return (object) checked ((long) number);
        case TypeCode.UInt64:
          return (object) number;
        default:
          return (object) number;
      }
    }
  }
}
