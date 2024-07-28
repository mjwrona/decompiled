// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.XmlHelper
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("Use Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
  [CLSCompliant(false)]
  public static class XmlHelper
  {
    [Obsolete("Use property in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static XmlReaderSettings SecureReaderSettings => XmlUtility.SecureReaderSettings;

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static XmlDocument GetDocument(Stream input) => XmlUtility.GetDocument(input);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static XmlDocument GetDocument(string xml) => XmlUtility.GetDocument(xml);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static XmlDocument GetDocumentFromPath(string path) => XmlUtility.GetDocumentFromPath(path);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static DateTime ToDateTime(string s) => XmlUtility.ToDateTime(s);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static DateTime ToDateOnly(string s) => XmlUtility.ToDateOnly(s);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static string ToStringDateOnly(DateTime d) => XmlUtility.ToStringDateOnly(d);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static string ToString(DateTime d) => XmlUtility.ToString(d);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static string ToString(Uri uri) => XmlUtility.ToString(uri);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static Uri ToUri(string s) => XmlUtility.ToUri(s);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void EnumToXmlAttribute<T>(XmlWriter writer, string attr, T value) => XmlUtility.EnumToXmlAttribute<T>(writer, attr, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static T EnumFromXmlAttribute<T>(XmlReader reader) => XmlUtility.EnumFromXmlAttribute<T>(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void EnumToXmlElement<T>(XmlWriter writer, string element, T value) => XmlUtility.EnumToXmlElement<T>(writer, element, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static T EnumFromXmlElement<T>(XmlReader reader) => XmlUtility.EnumFromXmlElement<T>(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ObjectToXmlElement(XmlWriter writer, string element, object o) => XmlUtility.ObjectToXmlElement(writer, element, o);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static object ObjectFromXmlElement(XmlReader reader) => XmlUtility.ObjectFromXmlElement(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXml(XmlWriter writer, string element, object[] array) => XmlUtility.ToXml(writer, element, array);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static object[] ArrayOfObjectFromXml(XmlReader reader) => XmlUtility.ArrayOfObjectFromXml(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static T[] ArrayOfObjectFromXml<T>(
      XmlReader reader,
      string arrayElementName,
      bool inline,
      Func<XmlReader, T> objectFromXmlElement)
    {
      return XmlUtility.ArrayOfObjectFromXml<T>(reader, arrayElementName, inline, objectFromXmlElement);
    }

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static T[] ArrayOfObjectFromXml<T>(
      IServiceProvider serviceProvider,
      XmlReader reader,
      string arrayElementName,
      bool inline,
      Func<IServiceProvider, XmlReader, T> objectFromXmlElement)
    {
      return XmlUtility.ArrayOfObjectFromXml<T>(serviceProvider, reader, arrayElementName, inline, objectFromXmlElement);
    }

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ArrayOfObjectToXml<T>(
      XmlWriter writer,
      T[] array,
      string arrayName,
      string arrayElementName,
      bool inline,
      bool allowEmptyArrays,
      Action<XmlWriter, string, T> objectToXmlElement)
    {
      XmlUtility.ArrayOfObjectToXml<T>(writer, array, arrayName, arrayElementName, inline, allowEmptyArrays, objectToXmlElement);
    }

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void EnumerableOfObjectToXml<T>(
      XmlWriter writer,
      IEnumerable<T> enumerable,
      string arrayName,
      string arrayElementName,
      bool inline,
      bool allowEmptyArrays,
      Action<XmlWriter, string, T> objectToXmlElement)
    {
      XmlUtility.EnumerableOfObjectToXml<T>(writer, enumerable, arrayName, arrayElementName, inline, allowEmptyArrays, objectToXmlElement);
    }

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string elementName, XmlNode node) => XmlUtility.ToXmlElement(writer, elementName, node);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static XmlNode XmlNodeFromXmlElement(XmlReader reader) => XmlUtility.XmlNodeFromXmlElement(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static DateTime DateFromXmlAttribute(XmlReader reader) => XmlHelper.ToDateOnly(XmlHelper.StringFromXmlAttribute(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static bool BooleanFromXmlElement(XmlReader reader) => XmlConvert.ToBoolean(XmlHelper.StringFromXmlElement(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static byte ByteFromXmlAttribute(XmlReader reader) => XmlConvert.ToByte(XmlHelper.StringFromXmlAttribute(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static byte ByteFromXmlElement(XmlReader reader) => XmlConvert.ToByte(XmlHelper.StringFromXmlElement(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static char CharFromXmlAttribute(XmlReader reader) => (char) XmlConvert.ToInt32(XmlHelper.StringFromXmlAttribute(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static char CharFromXmlElement(XmlReader reader) => (char) XmlConvert.ToInt32(XmlHelper.StringFromXmlElement(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static DateTime DateFromXmlElement(XmlReader reader) => XmlHelper.ToDateOnly(XmlHelper.StringFromXmlElement(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void DateToXmlAttribute(XmlWriter writer, string name, DateTime value) => XmlHelper.StringToXmlAttribute(writer, name, XmlHelper.ToStringDateOnly(value));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void DateToXmlElement(XmlWriter writer, string name, DateTime value) => XmlHelper.StringToXmlElement(writer, name, XmlHelper.ToStringDateOnly(value));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static bool BooleanFromXmlAttribute(XmlReader reader) => XmlConvert.ToBoolean(XmlHelper.StringFromXmlAttribute(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static double DoubleFromXmlAttribute(XmlReader reader) => XmlConvert.ToDouble(XmlHelper.StringFromXmlAttribute(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static double DoubleFromXmlElement(XmlReader reader) => XmlConvert.ToDouble(XmlHelper.StringFromXmlElement(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static Guid GuidFromXmlAttribute(XmlReader reader) => XmlConvert.ToGuid(XmlHelper.StringFromXmlAttribute(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static Guid GuidFromXmlElement(XmlReader reader) => XmlConvert.ToGuid(XmlHelper.StringFromXmlElement(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static short Int16FromXmlAttribute(XmlReader reader) => XmlConvert.ToInt16(XmlHelper.StringFromXmlAttribute(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static short Int16FromXmlElement(XmlReader reader) => XmlConvert.ToInt16(XmlHelper.StringFromXmlElement(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static int Int32FromXmlAttribute(XmlReader reader) => XmlConvert.ToInt32(XmlHelper.StringFromXmlAttribute(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static int Int32FromXmlElement(XmlReader reader) => XmlConvert.ToInt32(XmlHelper.StringFromXmlElement(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static long Int64FromXmlAttribute(XmlReader reader) => XmlConvert.ToInt64(XmlHelper.StringFromXmlAttribute(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static long Int64FromXmlElement(XmlReader reader) => XmlConvert.ToInt64(XmlHelper.StringFromXmlElement(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static DateTime DateTimeFromXmlAttribute(XmlReader reader) => XmlHelper.ToDateTime(XmlHelper.StringFromXmlAttribute(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static DateTime DateTimeFromXmlElement(XmlReader reader) => XmlHelper.ToDateTime(XmlHelper.StringFromXmlElement(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static float SingleFromXmlAttribute(XmlReader reader) => XmlConvert.ToSingle(XmlHelper.StringFromXmlAttribute(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static float SingleFromXmlElement(XmlReader reader) => XmlConvert.ToSingle(XmlHelper.StringFromXmlElement(reader));

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static string StringFromXmlAttribute(XmlReader reader) => XmlUtility.StringFromXmlAttribute(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static string StringFromXmlElement(XmlReader reader) => XmlUtility.StringFromXmlElement(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static string StringFromXmlText(XmlReader reader) => XmlUtility.StringFromXmlText(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static TimeSpan TimeSpanFromXmlAttribute(XmlReader reader) => XmlUtility.TimeSpanFromXmlAttribute(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static TimeSpan TimeSpanFromXmlElement(XmlReader reader) => XmlUtility.TimeSpanFromXmlElement(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static ushort UInt16FromXmlAttribute(XmlReader reader) => XmlUtility.UInt16FromXmlAttribute(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static ushort UInt16FromXmlElement(XmlReader reader) => XmlUtility.UInt16FromXmlElement(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static uint UInt32FromXmlAttribute(XmlReader reader) => XmlUtility.UInt32FromXmlAttribute(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static uint UInt32FromXmlElement(XmlReader reader) => XmlUtility.UInt32FromXmlElement(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static ulong UInt64FromXmlAttribute(XmlReader reader) => XmlUtility.UInt64FromXmlAttribute(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static ulong UInt64FromXmlElement(XmlReader reader) => XmlUtility.UInt64FromXmlElement(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static Uri UriFromXmlAttribute(XmlReader reader) => XmlUtility.UriFromXmlAttribute(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static Uri UriFromXmlElement(XmlReader reader) => XmlUtility.UriFromXmlElement(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, bool value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, byte value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, char value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, DateTime value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, double value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, Guid value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, short value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, int value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, long value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, float value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, string value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, TimeSpan value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, ushort value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, uint value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, ulong value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string name, Uri value) => XmlUtility.ToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string name, bool value) => XmlUtility.ToXmlElement(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string name, byte value) => XmlUtility.ToXmlElement(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string name, char value) => XmlUtility.ToXmlElement(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string name, DateTime value) => XmlUtility.ToXmlElement(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string name, double value) => XmlUtility.ToXmlElement(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string name, Guid value) => XmlUtility.ToXmlElement(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string element, short value) => XmlUtility.ToXmlElement(writer, element, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string element, int value) => XmlUtility.ToXmlElement(writer, element, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string element, long value) => XmlUtility.ToXmlElement(writer, element, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string name, float value) => XmlUtility.ToXmlElement(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string name, string value) => XmlUtility.ToXmlElement(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string name, TimeSpan value) => XmlUtility.ToXmlElement(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string element, ushort value) => XmlUtility.ToXmlElement(writer, element, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string element, uint value) => XmlUtility.ToXmlElement(writer, element, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string element, ulong value) => XmlUtility.ToXmlElement(writer, element, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlElement(XmlWriter writer, string name, Uri value) => XmlUtility.ToXmlElement(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void StringToXmlAttribute(XmlWriter writer, string name, string value) => XmlUtility.StringToXmlAttribute(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void StringToXmlElement(XmlWriter writer, string name, string value) => XmlUtility.StringToXmlElement(writer, name, value);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void StringToXmlText(XmlWriter writer, string str) => XmlUtility.StringToXmlText(writer, str);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static byte[] ArrayOfByteFromXml(XmlReader reader) => XmlUtility.ArrayOfByteFromXml(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static byte[] ArrayOfByteFromXmlAttribute(XmlReader reader) => XmlUtility.ArrayOfByteFromXmlAttribute(reader);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXml(XmlWriter writer, string element, byte[] array) => XmlUtility.ToXml(writer, element, array);

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static void ToXmlAttribute(XmlWriter writer, string attr, byte[] array) => XmlUtility.ToXmlAttribute(writer, attr, array);

    [Obsolete("Use property in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static byte[] ZeroLengthArrayOfByte => XmlUtility.ZeroLengthArrayOfByte;

    [Obsolete("Use method in Microsoft.TeamFoundation.Services.Common.Internal.XmlUtility instead.")]
    public static bool CompareXmlDocuments(string xml1, string xml2) => XmlUtility.CompareXmlDocuments(xml1, xml2);
  }
}
