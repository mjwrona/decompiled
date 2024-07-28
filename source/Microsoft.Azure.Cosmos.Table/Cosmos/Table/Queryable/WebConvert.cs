// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.WebConvert
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal static class WebConvert
  {
    private const string HexValues = "0123456789ABCDEF";
    private const string XmlHexEncodePrefix = "0x";

    internal static string ConvertByteArrayToKeyString(byte[] byteArray)
    {
      StringBuilder stringBuilder = new StringBuilder(3 + byteArray.Length * 2);
      stringBuilder.Append("X");
      stringBuilder.Append("'");
      for (int index = 0; index < byteArray.Length; ++index)
      {
        stringBuilder.Append("0123456789ABCDEF"[(int) byteArray[index] >> 4]);
        stringBuilder.Append("0123456789ABCDEF"[(int) byteArray[index] & 15]);
      }
      stringBuilder.Append("'");
      return stringBuilder.ToString();
    }

    internal static bool IsKeyTypeQuoted(Type type) => type == typeof (XElement) || type == typeof (string);

    internal static bool TryKeyPrimitiveToString(object value, out string result)
    {
      if (value.GetType() == typeof (byte[]))
      {
        result = WebConvert.ConvertByteArrayToKeyString((byte[]) value);
      }
      else
      {
        if (!WebConvert.TryXmlPrimitiveToString(value, out result))
          return false;
        if (value.GetType() == typeof (DateTime))
          result = "datetime'" + result + "'";
        else if (value.GetType() == typeof (Decimal))
          result += "M";
        else if (value.GetType() == typeof (Guid))
          result = "guid'" + result + "'";
        else if (value.GetType() == typeof (long))
          result += "L";
        else if (value.GetType() == typeof (float))
          result += "f";
        else if (value.GetType() == typeof (double))
          result = WebConvert.AppendDecimalMarkerToDouble(result);
        else if (WebConvert.IsKeyTypeQuoted(value.GetType()))
          result = "'" + result.Replace("'", "''") + "'";
      }
      return true;
    }

    internal static bool TryXmlPrimitiveToString(object value, out string result)
    {
      result = (string) null;
      Type type1 = value.GetType();
      Type type2 = Nullable.GetUnderlyingType(type1);
      if ((object) type2 == null)
        type2 = type1;
      Type type3 = type2;
      if (typeof (string) == type3)
        result = (string) value;
      else if (typeof (bool) == type3)
        result = XmlConvert.ToString((bool) value);
      else if (typeof (byte) == type3)
        result = XmlConvert.ToString((byte) value);
      else if (typeof (DateTime) == type3)
        result = XmlConvert.ToString((DateTime) value, XmlDateTimeSerializationMode.RoundtripKind);
      else if (typeof (Decimal) == type3)
        result = XmlConvert.ToString((Decimal) value);
      else if (typeof (double) == type3)
        result = XmlConvert.ToString((double) value);
      else if (typeof (Guid) == type3)
        result = value.ToString();
      else if (typeof (short) == type3)
        result = XmlConvert.ToString((short) value);
      else if (typeof (int) == type3)
        result = XmlConvert.ToString((int) value);
      else if (typeof (long) == type3)
        result = XmlConvert.ToString((long) value);
      else if (typeof (sbyte) == type3)
        result = XmlConvert.ToString((sbyte) value);
      else if (typeof (float) == type3)
        result = XmlConvert.ToString((float) value);
      else if (typeof (byte[]) == type3)
      {
        byte[] inArray = (byte[]) value;
        result = Convert.ToBase64String(inArray);
      }
      else
      {
        if (ClientConvert.IsBinaryValue(value))
          return ClientConvert.TryKeyBinaryToString(value, out result);
        if (typeof (XElement) == type3)
        {
          result = ((XNode) value).ToString(SaveOptions.None);
        }
        else
        {
          result = (string) null;
          return false;
        }
      }
      return true;
    }

    private static string AppendDecimalMarkerToDouble(string input)
    {
      foreach (char c in input)
      {
        if (!char.IsDigit(c))
          return input;
      }
      return input + ".0";
    }
  }
}
