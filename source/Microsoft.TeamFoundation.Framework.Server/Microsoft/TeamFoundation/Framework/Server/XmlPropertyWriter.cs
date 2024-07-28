// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.XmlPropertyWriter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class XmlPropertyWriter
  {
    private static TimeSpan s_oneMillisecond = new TimeSpan(0, 0, 0, 0, 1);

    public static void WriteValue(XmlWriter xmlWriter, string propertyName, object value)
    {
      if (value == null)
        return;
      Type type = value.GetType();
      TypeCode typeCode = Type.GetTypeCode(type);
      if (typeCode == TypeCode.Object && value is byte[])
        XmlPropertyWriter.Write(xmlWriter, (byte[]) value);
      else if (typeCode == TypeCode.Object && value is Guid guid)
      {
        XmlPropertyWriter.Write(xmlWriter, guid.ToString("N"));
      }
      else
      {
        switch (typeCode)
        {
          case TypeCode.Empty:
            throw new PropertyTypeNotSupportedException(propertyName, type);
          case TypeCode.Object:
            throw new PropertyTypeNotSupportedException(propertyName, type);
          case TypeCode.DBNull:
            throw new PropertyTypeNotSupportedException(propertyName, type);
          case TypeCode.Int32:
            XmlPropertyWriter.Write(xmlWriter, (int) value);
            break;
          case TypeCode.Double:
            XmlPropertyWriter.Write(xmlWriter, (double) value);
            break;
          case TypeCode.DateTime:
            XmlPropertyWriter.Write(xmlWriter, (DateTime) value);
            break;
          case TypeCode.String:
            XmlPropertyWriter.Write(xmlWriter, (string) value);
            break;
          default:
            XmlPropertyWriter.Write(xmlWriter, value.ToString());
            break;
        }
      }
    }

    public static void Write(XmlWriter xmlWriter, int value) => XmlPropertyWriter.WriteXmlAttribute<int>(xmlWriter, "IntValue", value);

    public static void Write(XmlWriter xmlWriter, double value) => XmlPropertyWriter.WriteXmlAttribute<double>(xmlWriter, "DoubleValue", value);

    public static void Write(XmlWriter xmlWriter, DateTime value)
    {
      xmlWriter.WriteStartAttribute("DatetimeValue");
      if (value.Equals(DateTime.MaxValue))
        value = value.Subtract(XmlPropertyWriter.s_oneMillisecond);
      xmlWriter.WriteValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", (IFormatProvider) CultureInfo.InvariantCulture));
      xmlWriter.WriteEndAttribute();
    }

    public static void Write(XmlWriter xmlWriter, string value) => XmlPropertyWriter.WriteXmlAttribute<string>(xmlWriter, "StringValue", value);

    public static void Write(XmlWriter xmlWriter, byte[] value)
    {
      xmlWriter.WriteStartAttribute("BinaryValue");
      xmlWriter.WriteBase64(value, 0, value.Length);
      xmlWriter.WriteEndAttribute();
    }

    private static void WriteXmlAttribute<T>(
      XmlWriter xmlTextWriter,
      string attributeName,
      T value)
    {
      xmlTextWriter.WriteStartAttribute(attributeName);
      if ((object) value != null)
        xmlTextWriter.WriteValue((object) value);
      xmlTextWriter.WriteEndAttribute();
    }
  }
}
