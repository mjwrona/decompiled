// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.XmlUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [CLSCompliant(false)]
  public static class XmlUtility
  {
    private static XmlReaderSettings s_safeSettings;
    [ThreadStatic]
    private static string[] ts_stringList;
    private const int c_stringCacheSize = 16;
    private static byte[] s_zeroLengthArrayOfByte;
    private static readonly XmlUtility.AttributeComparer s_xmlAttributeComparer = new XmlUtility.AttributeComparer();
    private static readonly XmlUtility.ElementComparer s_xmlElementComparer = new XmlUtility.ElementComparer();

    internal static FileStream OpenFile(string path, FileShare sharing, bool saveFile) => XmlUtility.OpenFileHelper(path, sharing, saveFile, false, out XmlDocument _);

    internal static XmlDocument OpenXmlFile(
      out FileStream file,
      string path,
      FileShare sharing,
      bool saveFile)
    {
      XmlDocument xmlDocument;
      file = XmlUtility.OpenFileHelper(path, sharing, saveFile, true, out xmlDocument);
      return xmlDocument;
    }

    private static FileStream OpenFileHelper(
      string path,
      FileShare sharing,
      bool saveFile,
      bool loadAsXmlDocument,
      out XmlDocument xmlDocument)
    {
      FileStream input = (FileStream) null;
      xmlDocument = (XmlDocument) null;
      if (string.IsNullOrEmpty(path))
        return (FileStream) null;
      if (!saveFile && !File.Exists(path))
        return (FileStream) null;
      int num = 0;
      Random random = (Random) null;
      for (; num <= 10; ++num)
      {
        try
        {
          FileAccess access = FileAccess.Read;
          FileMode mode = FileMode.Open;
          if (saveFile)
          {
            access = FileAccess.ReadWrite;
            mode = FileMode.OpenOrCreate;
          }
          input = new FileStream(path, mode, access, sharing);
          if (loadAsXmlDocument)
          {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
              DtdProcessing = DtdProcessing.Prohibit,
              XmlResolver = (XmlResolver) null
            };
            using (XmlReader reader = XmlReader.Create((Stream) input, settings))
            {
              xmlDocument = new XmlDocument();
              xmlDocument.Load(reader);
            }
          }
          return input;
        }
        catch (Exception ex1)
        {
          if (input != null)
          {
            input.Dispose();
            input = (FileStream) null;
          }
          switch (ex1)
          {
            case OperationCanceledException _:
              throw;
            case IOException _:
            case UnauthorizedAccessException _:
            case XmlException _:
              if (!saveFile)
                return (FileStream) null;
              try
              {
                if (ex1 is DirectoryNotFoundException)
                  Directory.CreateDirectory(Path.GetDirectoryName(path));
                if (ex1 is UnauthorizedAccessException)
                  File.SetAttributes(path, FileAttributes.Normal);
                xmlDocument = (XmlDocument) null;
                return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
              }
              catch (Exception ex2) when (ex2 is IOException || ex2 is UnauthorizedAccessException)
              {
                if (num >= 10)
                  throw new AggregateException(new Exception[2]
                  {
                    ex1,
                    ex2
                  });
                break;
              }
            default:
              if (num >= 10)
                throw new VssServiceException(CommonResources.ErrorReadingFile((object) Path.GetFileName(path), (object) ex1.Message), ex1);
              break;
          }
        }
        if (random == null)
          random = new Random();
        Task.Delay(random.Next(1, 150)).Wait();
      }
      return (FileStream) null;
    }

    internal static void AddXmlAttribute(XmlNode node, string attrName, string value)
    {
      if (value == null)
        return;
      XmlAttribute attribute = node.OwnerDocument.CreateAttribute((string) null, attrName, (string) null);
      node.Attributes.Append(attribute);
      attribute.InnerText = value;
    }

    public static XmlReaderSettings SecureReaderSettings
    {
      get
      {
        if (XmlUtility.s_safeSettings == null)
          XmlUtility.s_safeSettings = new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          };
        return XmlUtility.s_safeSettings;
      }
    }

    public static XmlDocument GetDocument(Stream input)
    {
      XmlDocument document = new XmlDocument();
      using (XmlReader reader = XmlReader.Create(input, XmlUtility.SecureReaderSettings))
        document.Load(reader);
      return document;
    }

    public static XmlDocument GetDocument(string xml)
    {
      XmlDocument document = new XmlDocument();
      using (StringReader input = new StringReader(xml))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) input, XmlUtility.SecureReaderSettings))
          document.Load(reader);
      }
      return document;
    }

    public static XmlDocument GetDocumentFromPath(string path)
    {
      XmlDocument documentFromPath = new XmlDocument();
      using (XmlReader reader = XmlReader.Create(path, XmlUtility.SecureReaderSettings))
        documentFromPath.Load(reader);
      return documentFromPath;
    }

    public static DateTime ToDateTime(string s)
    {
      DateTime dateTime = XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind);
      if (dateTime.Kind == DateTimeKind.Unspecified && dateTime != DateTime.MinValue && dateTime != DateTime.MaxValue)
        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
      dateTime = dateTime.Year != 1 ? dateTime.ToLocalTime() : DateTime.MinValue;
      return dateTime;
    }

    public static DateTime ToDateOnly(string s) => XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind);

    public static string ToStringDateOnly(DateTime d) => d.ToString("yyyy-MM-dd", (IFormatProvider) CultureInfo.InvariantCulture);

    public static string ToString(DateTime d) => XmlConvert.ToString(d, XmlDateTimeSerializationMode.RoundtripKind);

    public static void ObjectToXmlElement(XmlWriter writer, string element, object o)
    {
      if (o == null)
      {
        writer.WriteStartElement(element);
        writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
        writer.WriteEndElement();
      }
      else
      {
        string fullName = o.GetType().FullName;
        if (fullName != null)
        {
          string localName;
          string str;
          string ns;
          switch (fullName.Length)
          {
            case 11:
              switch (fullName[7])
              {
                case 'B':
                  if (fullName == "System.Byte")
                  {
                    localName = "unsignedByte";
                    str = XmlConvert.ToString((byte) o);
                    ns = "http://www.w3.org/2001/XMLSchema";
                    break;
                  }
                  goto label_34;
                case 'C':
                  if (fullName == "System.Char")
                  {
                    localName = "char";
                    str = XmlConvert.ToString((ushort) (char) o);
                    ns = "http://microsoft.com/wsdl/types/";
                    break;
                  }
                  goto label_34;
                case 'G':
                  if (fullName == "System.Guid")
                  {
                    localName = "guid";
                    str = XmlConvert.ToString((Guid) o);
                    ns = "http://microsoft.com/wsdl/types/";
                    break;
                  }
                  goto label_34;
                default:
                  goto label_34;
              }
              break;
            case 12:
              switch (fullName[10])
              {
                case '1':
                  if (fullName == "System.Int16")
                  {
                    localName = "short";
                    str = XmlConvert.ToString((short) o);
                    ns = "http://www.w3.org/2001/XMLSchema";
                    break;
                  }
                  goto label_34;
                case '3':
                  if (fullName == "System.Int32")
                  {
                    localName = "int";
                    str = XmlConvert.ToString((int) o);
                    ns = "http://www.w3.org/2001/XMLSchema";
                    break;
                  }
                  goto label_34;
                case '6':
                  if (fullName == "System.Int64")
                  {
                    localName = "long";
                    str = XmlConvert.ToString((long) o);
                    ns = "http://www.w3.org/2001/XMLSchema";
                    break;
                  }
                  goto label_34;
                default:
                  goto label_34;
              }
              break;
            case 13:
              switch (fullName[8])
              {
                case 'i':
                  if (fullName == "System.Single")
                  {
                    localName = "float";
                    str = XmlConvert.ToString((float) o);
                    ns = "http://www.w3.org/2001/XMLSchema";
                    break;
                  }
                  goto label_34;
                case 'o':
                  if (fullName == "System.Double")
                  {
                    localName = "double";
                    str = XmlConvert.ToString((double) o);
                    ns = "http://www.w3.org/2001/XMLSchema";
                    break;
                  }
                  goto label_34;
                case 't':
                  if (fullName == "System.String")
                  {
                    localName = "string";
                    str = (string) o;
                    ns = "http://www.w3.org/2001/XMLSchema";
                    break;
                  }
                  goto label_34;
                case 'y':
                  if (fullName == "System.Byte[]")
                  {
                    localName = "base64Binary";
                    byte[] inArray = (byte[]) o;
                    str = Convert.ToBase64String(inArray, 0, inArray.Length);
                    ns = "http://www.w3.org/2001/XMLSchema";
                    break;
                  }
                  goto label_34;
                default:
                  goto label_34;
              }
              break;
            case 14:
              switch (fullName[7])
              {
                case 'B':
                  if (fullName == "System.Boolean")
                  {
                    localName = "boolean";
                    str = XmlConvert.ToString((bool) o);
                    ns = "http://www.w3.org/2001/XMLSchema";
                    break;
                  }
                  goto label_34;
                case 'D':
                  if (fullName == "System.Decimal")
                  {
                    localName = "decimal";
                    str = XmlConvert.ToString((Decimal) o);
                    ns = "http://www.w3.org/2001/XMLSchema";
                    break;
                  }
                  goto label_34;
                default:
                  goto label_34;
              }
              break;
            case 15:
              if (fullName == "System.DateTime")
              {
                localName = "dateTime";
                str = XmlUtility.ToString((DateTime) o);
                ns = "http://www.w3.org/2001/XMLSchema";
                break;
              }
              goto label_34;
            default:
              goto label_34;
          }
          writer.WriteStartElement(element);
          writer.WriteStartAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
          writer.WriteQualifiedName(localName, ns);
          writer.WriteEndAttribute();
          writer.WriteValue(str);
          writer.WriteEndElement();
          return;
        }
label_34:
        if (!o.GetType().IsArray)
          throw new ArgumentException(CommonResources.UnknownTypeForSerialization((object) fullName));
        writer.WriteStartElement(element);
        writer.WriteAttributeString("type", "http://www.w3.org/2001/XMLSchema-instance", "ArrayOfAnyType");
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        XmlUtility.ArrayOfObjectToXml<object>(writer, (object[]) o, (string) null, "anyType", true, false, XmlUtility.\u003C\u003EO.\u003C0\u003E__ObjectToXmlElement ?? (XmlUtility.\u003C\u003EO.\u003C0\u003E__ObjectToXmlElement = new Action<XmlWriter, string, object>(XmlUtility.ObjectToXmlElement)));
        writer.WriteEndElement();
      }
    }

    public static object ObjectFromXmlElement(XmlReader reader)
    {
      string attribute = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
      if (!string.IsNullOrEmpty(attribute))
      {
        string[] strArray = attribute.Split(new char[1]
        {
          ':'
        }, StringSplitOptions.None);
        if (strArray.Length == 2)
          attribute = strArray[1];
        if (attribute != null)
        {
          switch (attribute.Length)
          {
            case 3:
              if (attribute == "int")
                return (object) XmlConvert.ToInt32(XmlUtility.StringFromXmlElement(reader));
              break;
            case 4:
              switch (attribute[0])
              {
                case 'c':
                  if (attribute == "char")
                    return (object) (char) XmlConvert.ToInt16(XmlUtility.StringFromXmlElement(reader));
                  break;
                case 'g':
                  if (attribute == "guid")
                    return (object) XmlConvert.ToGuid(XmlUtility.StringFromXmlElement(reader));
                  break;
                case 'l':
                  if (attribute == "long")
                    return (object) XmlConvert.ToInt64(XmlUtility.StringFromXmlElement(reader));
                  break;
              }
              break;
            case 5:
              switch (attribute[0])
              {
                case 'f':
                  if (attribute == "float")
                    return (object) XmlConvert.ToSingle(XmlUtility.StringFromXmlElement(reader));
                  break;
                case 's':
                  if (attribute == "short")
                    return (object) XmlConvert.ToInt16(XmlUtility.StringFromXmlElement(reader));
                  break;
              }
              break;
            case 6:
              switch (attribute[0])
              {
                case 'd':
                  if (attribute == "double")
                    return (object) XmlConvert.ToDouble(XmlUtility.StringFromXmlElement(reader));
                  break;
                case 's':
                  if (attribute == "string")
                    return (object) XmlUtility.StringFromXmlElement(reader);
                  break;
              }
              break;
            case 7:
              switch (attribute[0])
              {
                case 'b':
                  if (attribute == "boolean")
                    return (object) XmlConvert.ToBoolean(XmlUtility.StringFromXmlElement(reader));
                  break;
                case 'd':
                  if (attribute == "decimal")
                    return (object) XmlConvert.ToDecimal(XmlUtility.StringFromXmlElement(reader));
                  break;
              }
              break;
            case 8:
              if (attribute == "dateTime")
                return (object) XmlUtility.ToDateTime(XmlUtility.StringFromXmlElement(reader));
              break;
            case 12:
              switch (attribute[0])
              {
                case 'b':
                  if (attribute == "base64Binary")
                  {
                    string s = XmlUtility.StringFromXmlElement(reader);
                    return s != null ? (object) Convert.FromBase64String(s) : (object) XmlUtility.ZeroLengthArrayOfByte;
                  }
                  break;
                case 'u':
                  if (attribute == "unsignedByte")
                    return (object) XmlConvert.ToByte(XmlUtility.StringFromXmlElement(reader));
                  break;
              }
              break;
            case 14:
              if (attribute == "ArrayOfAnyType")
                return (object) XmlUtility.ArrayOfObjectFromXml(reader);
              break;
          }
        }
        throw new ArgumentException(CommonResources.UnknownTypeForSerialization((object) attribute));
      }
      if (!(reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true"))
        return (object) null;
      reader.ReadInnerXml();
      return (object) null;
    }

    public static void ToXml(XmlWriter writer, string element, object[] array)
    {
      if (array == null || array.Length == 0)
        return;
      if (!string.IsNullOrEmpty(element))
        writer.WriteStartElement(element);
      for (int index = 0; index < array.Length; ++index)
      {
        if (array[index] == null)
          throw new ArgumentNullException("array[" + index.ToString() + "]");
        XmlUtility.ObjectToXmlElement(writer, "anyType", array[index]);
      }
      if (string.IsNullOrEmpty(element))
        return;
      writer.WriteEndElement();
    }

    public static object[] ArrayOfObjectFromXml(XmlReader reader)
    {
      List<object> objectList = new List<object>();
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.HasAttributes && reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true")
          {
            objectList.Add((object) null);
            reader.Read();
          }
          else
            objectList.Add(XmlUtility.ObjectFromXmlElement(reader));
        }
        reader.ReadEndElement();
      }
      return objectList.ToArray();
    }

    public static void ToXmlElement(XmlWriter writer, string elementName, XmlNode node)
    {
      if (node == null)
        return;
      writer.WriteStartElement(elementName);
      node.WriteTo(writer);
      writer.WriteEndElement();
    }

    public static XmlNode XmlNodeFromXmlElement(XmlReader reader)
    {
      reader.Read();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.PreserveWhitespace = false;
      xmlDocument.Load(reader);
      xmlDocument.Normalize();
      reader.ReadEndElement();
      return (XmlNode) xmlDocument.DocumentElement;
    }

    public static DateTime DateFromXmlAttribute(XmlReader reader) => XmlUtility.ToDateOnly(XmlUtility.StringFromXmlAttribute(reader));

    public static DateTime DateFromXmlElement(XmlReader reader) => XmlUtility.ToDateOnly(XmlUtility.StringFromXmlElement(reader));

    public static void DateToXmlAttribute(XmlWriter writer, string name, DateTime value) => XmlUtility.StringToXmlAttribute(writer, name, XmlUtility.ToStringDateOnly(value));

    public static void DateToXmlElement(XmlWriter writer, string name, DateTime value) => XmlUtility.StringToXmlElement(writer, name, XmlUtility.ToStringDateOnly(value));

    public static bool BooleanFromXmlAttribute(XmlReader reader) => XmlConvert.ToBoolean(XmlUtility.StringFromXmlAttribute(reader));

    public static DateTime DateTimeFromXmlAttribute(XmlReader reader) => XmlUtility.ToDateTime(XmlUtility.StringFromXmlAttribute(reader));

    public static DateTime DateTimeFromXmlElement(XmlReader reader) => XmlUtility.ToDateTime(XmlUtility.StringFromXmlElement(reader));

    public static void ToXmlAttribute(XmlWriter writer, string name, DateTime value) => XmlUtility.StringToXmlAttribute(writer, name, XmlUtility.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string name, DateTime value) => XmlUtility.StringToXmlElement(writer, name, XmlUtility.ToString(value));

    public static void ToXml(XmlWriter writer, string element, byte[] array)
    {
      if (array == null || array.Length == 0)
        return;
      writer.WriteElementString(element, Convert.ToBase64String(array, 0, array.Length));
    }

    public static void ToXmlAttribute(XmlWriter writer, string attr, byte[] array)
    {
      if (array == null || array.Length == 0)
        return;
      writer.WriteAttributeString(attr, Convert.ToBase64String(array, 0, array.Length));
    }

    public static string ToString(Uri uri) => uri.AbsoluteUri;

    public static Uri ToUri(string s) => string.IsNullOrEmpty(s) ? (Uri) null : new Uri(s);

    public static void EnumToXmlAttribute<T>(XmlWriter writer, string attr, T value)
    {
      string str = Enum.Format(typeof (T), (object) value, "G").Replace(",", "");
      writer.WriteAttributeString(attr, str);
    }

    public static T EnumFromXmlAttribute<T>(XmlReader reader) => (T) Enum.Parse(typeof (T), XmlUtility.StringFromXmlAttribute(reader).Replace(' ', ','), true);

    public static void EnumToXmlElement<T>(XmlWriter writer, string element, T value)
    {
      string str = Enum.Format(typeof (T), (object) value, "G").Replace(",", "");
      writer.WriteElementString(element, str);
    }

    public static T EnumFromXmlElement<T>(XmlReader reader) => (T) Enum.Parse(typeof (T), XmlUtility.StringFromXmlElement(reader).Replace(' ', ','), true);

    public static T[] ArrayOfObjectFromXml<T>(
      XmlReader reader,
      string arrayElementName,
      bool inline,
      Func<XmlReader, T> objectFromXmlElement)
    {
      return XmlUtility.ArrayOfObjectFromXml<T>((IServiceProvider) null, reader, arrayElementName, inline, (Func<IServiceProvider, XmlReader, T>) ((x, y) => objectFromXmlElement(y)));
    }

    public static T[] ArrayOfObjectFromXml<T>(
      IServiceProvider serviceProvider,
      XmlReader reader,
      string arrayElementName,
      bool inline,
      Func<IServiceProvider, XmlReader, T> objectFromXmlElement)
    {
      List<T> objList = new List<T>();
      int num = reader.IsEmptyElement ? 1 : 0;
      if (!inline)
        reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element && (!inline || reader.Name == arrayElementName))
        {
          if (reader.HasAttributes && reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true")
          {
            objList.Add(default (T));
            reader.Read();
          }
          else
            objList.Add(objectFromXmlElement(serviceProvider, reader));
        }
        reader.ReadEndElement();
      }
      return objList.ToArray();
    }

    public static void ArrayOfObjectToXml<T>(
      XmlWriter writer,
      T[] array,
      string arrayName,
      string arrayElementName,
      bool inline,
      bool allowEmptyArrays,
      Action<XmlWriter, string, T> objectToXmlElement)
    {
      if (array == null)
        return;
      if (array.Length == 0)
      {
        if (!allowEmptyArrays || string.IsNullOrEmpty(arrayName))
          return;
        writer.WriteStartElement(arrayName);
        writer.WriteEndElement();
      }
      else if (!inline)
      {
        writer.WriteStartElement(arrayName);
        for (int index = 0; index < array.Length; ++index)
        {
          if ((object) array[index] == null)
          {
            writer.WriteStartElement(arrayElementName);
            writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
            writer.WriteEndElement();
          }
          else
            objectToXmlElement(writer, arrayElementName, array[index]);
        }
        writer.WriteEndElement();
      }
      else
      {
        for (int index = 0; index < array.Length; ++index)
        {
          if ((object) array[index] == null)
          {
            writer.WriteStartElement(arrayElementName);
            writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
            writer.WriteEndElement();
          }
          else
            objectToXmlElement(writer, arrayElementName, array[index]);
        }
      }
    }

    public static void EnumerableOfObjectToXml<T>(
      XmlWriter writer,
      IEnumerable<T> enumerable,
      string arrayName,
      string arrayElementName,
      bool inline,
      bool allowEmptyArrays,
      Action<XmlWriter, string, T> objectToXmlElement)
    {
      if (enumerable == null)
        return;
      if (!enumerable.Any<T>())
      {
        if (!allowEmptyArrays || string.IsNullOrEmpty(arrayName))
          return;
        writer.WriteStartElement(arrayName);
        writer.WriteEndElement();
      }
      else if (!inline)
      {
        writer.WriteStartElement(arrayName);
        foreach (T obj in enumerable)
        {
          if ((object) obj == null)
          {
            writer.WriteStartElement(arrayElementName);
            writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
            writer.WriteEndElement();
          }
          else
            objectToXmlElement(writer, arrayElementName, obj);
        }
        writer.WriteEndElement();
      }
      else
      {
        foreach (T obj in enumerable)
        {
          if ((object) obj == null)
          {
            writer.WriteStartElement(arrayElementName);
            writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
            writer.WriteEndElement();
          }
          else
            objectToXmlElement(writer, arrayElementName, obj);
        }
      }
    }

    public static bool BooleanFromXmlElement(XmlReader reader) => XmlConvert.ToBoolean(XmlUtility.StringFromXmlElement(reader));

    public static byte ByteFromXmlAttribute(XmlReader reader) => XmlConvert.ToByte(XmlUtility.StringFromXmlAttribute(reader));

    public static byte ByteFromXmlElement(XmlReader reader) => XmlConvert.ToByte(XmlUtility.StringFromXmlElement(reader));

    public static char CharFromXmlAttribute(XmlReader reader) => (char) XmlConvert.ToInt32(XmlUtility.StringFromXmlAttribute(reader));

    public static char CharFromXmlElement(XmlReader reader) => (char) XmlConvert.ToInt32(XmlUtility.StringFromXmlElement(reader));

    public static double DoubleFromXmlAttribute(XmlReader reader) => XmlConvert.ToDouble(XmlUtility.StringFromXmlAttribute(reader));

    public static double DoubleFromXmlElement(XmlReader reader) => XmlConvert.ToDouble(XmlUtility.StringFromXmlElement(reader));

    public static Guid GuidFromXmlAttribute(XmlReader reader) => XmlConvert.ToGuid(XmlUtility.StringFromXmlAttribute(reader));

    public static Guid GuidFromXmlElement(XmlReader reader) => XmlConvert.ToGuid(XmlUtility.StringFromXmlElement(reader));

    public static short Int16FromXmlAttribute(XmlReader reader) => XmlConvert.ToInt16(XmlUtility.StringFromXmlAttribute(reader));

    public static short Int16FromXmlElement(XmlReader reader) => XmlConvert.ToInt16(XmlUtility.StringFromXmlElement(reader));

    public static int Int32FromXmlAttribute(XmlReader reader) => XmlConvert.ToInt32(XmlUtility.StringFromXmlAttribute(reader));

    public static int Int32FromXmlElement(XmlReader reader) => XmlConvert.ToInt32(XmlUtility.StringFromXmlElement(reader));

    public static long Int64FromXmlAttribute(XmlReader reader) => XmlConvert.ToInt64(XmlUtility.StringFromXmlAttribute(reader));

    public static long Int64FromXmlElement(XmlReader reader) => XmlConvert.ToInt64(XmlUtility.StringFromXmlElement(reader));

    public static float SingleFromXmlAttribute(XmlReader reader) => XmlConvert.ToSingle(XmlUtility.StringFromXmlAttribute(reader));

    public static float SingleFromXmlElement(XmlReader reader) => XmlConvert.ToSingle(XmlUtility.StringFromXmlElement(reader));

    public static string StringFromXmlAttribute(XmlReader reader) => XmlUtility.GetCachedString(reader.Value);

    public static string StringFromXmlElement(XmlReader reader)
    {
      string str = string.Empty;
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num == 0)
      {
        if (reader.NodeType == XmlNodeType.CDATA || reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.Whitespace)
        {
          str = XmlUtility.GetCachedString(reader.ReadContentAsString().Replace("\n", "\r\n"));
          reader.ReadEndElement();
        }
        else if (reader.NodeType == XmlNodeType.EndElement)
          reader.ReadEndElement();
      }
      return str;
    }

    public static string StringFromXmlText(XmlReader reader)
    {
      string str = string.Empty;
      if (reader.NodeType == XmlNodeType.CDATA || reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.Whitespace)
        str = XmlUtility.GetCachedString(reader.ReadContentAsString().Replace("\n", "\r\n"));
      return str;
    }

    public static TimeSpan TimeSpanFromXmlAttribute(XmlReader reader) => XmlConvert.ToTimeSpan(XmlUtility.StringFromXmlAttribute(reader));

    public static TimeSpan TimeSpanFromXmlElement(XmlReader reader) => XmlConvert.ToTimeSpan(XmlUtility.StringFromXmlElement(reader));

    public static ushort UInt16FromXmlAttribute(XmlReader reader) => XmlConvert.ToUInt16(XmlUtility.StringFromXmlAttribute(reader));

    public static ushort UInt16FromXmlElement(XmlReader reader) => XmlConvert.ToUInt16(XmlUtility.StringFromXmlElement(reader));

    public static uint UInt32FromXmlAttribute(XmlReader reader) => XmlConvert.ToUInt32(XmlUtility.StringFromXmlAttribute(reader));

    public static uint UInt32FromXmlElement(XmlReader reader) => XmlConvert.ToUInt32(XmlUtility.StringFromXmlElement(reader));

    public static ulong UInt64FromXmlAttribute(XmlReader reader) => XmlConvert.ToUInt64(XmlUtility.StringFromXmlAttribute(reader));

    public static ulong UInt64FromXmlElement(XmlReader reader) => XmlConvert.ToUInt64(XmlUtility.StringFromXmlElement(reader));

    public static Uri UriFromXmlAttribute(XmlReader reader) => XmlUtility.ToUri(XmlUtility.StringFromXmlAttribute(reader));

    public static Uri UriFromXmlElement(XmlReader reader) => XmlUtility.ToUri(XmlUtility.StringFromXmlElement(reader));

    public static void ToXmlAttribute(XmlWriter writer, string name, bool value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, byte value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, char value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString((int) value));

    public static void ToXmlAttribute(XmlWriter writer, string name, double value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, Guid value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, short value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, int value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, long value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, float value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, string value) => XmlUtility.StringToXmlAttribute(writer, name, value);

    public static void ToXmlAttribute(XmlWriter writer, string name, TimeSpan value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, ushort value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, uint value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, ulong value) => XmlUtility.StringToXmlAttribute(writer, name, XmlConvert.ToString(value));

    public static void ToXmlAttribute(XmlWriter writer, string name, Uri value) => XmlUtility.StringToXmlAttribute(writer, name, XmlUtility.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string name, bool value) => XmlUtility.StringToXmlElement(writer, name, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string name, byte value) => XmlUtility.StringToXmlElement(writer, name, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string name, char value) => XmlUtility.StringToXmlElement(writer, name, XmlConvert.ToString((int) value));

    public static void ToXmlElement(XmlWriter writer, string name, double value) => XmlUtility.StringToXmlElement(writer, name, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string name, Guid value) => XmlUtility.StringToXmlElement(writer, name, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string element, short value) => XmlUtility.StringToXmlElement(writer, element, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string element, int value) => XmlUtility.StringToXmlElement(writer, element, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string element, long value) => XmlUtility.StringToXmlElement(writer, element, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string name, float value) => XmlUtility.StringToXmlElement(writer, name, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string name, string value) => XmlUtility.StringToXmlElement(writer, name, value);

    public static void ToXmlElement(XmlWriter writer, string name, TimeSpan value) => XmlUtility.StringToXmlElement(writer, name, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string element, ushort value) => XmlUtility.StringToXmlElement(writer, element, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string element, uint value) => XmlUtility.StringToXmlElement(writer, element, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string element, ulong value) => XmlUtility.StringToXmlElement(writer, element, XmlConvert.ToString(value));

    public static void ToXmlElement(XmlWriter writer, string name, Uri value) => XmlUtility.StringToXmlElement(writer, name, XmlUtility.ToString(value));

    public static void StringToXmlAttribute(XmlWriter writer, string name, string value) => writer.WriteAttributeString(name, value);

    public static void StringToXmlElement(XmlWriter writer, string name, string value)
    {
      try
      {
        writer.WriteElementString(name, value);
      }
      catch (ArgumentException ex)
      {
        throw new VssServiceException(CommonResources.StringContainsIllegalChars(), (Exception) ex);
      }
    }

    public static void StringToXmlText(XmlWriter writer, string str)
    {
      if (str == null)
        return;
      try
      {
        writer.WriteString(str);
      }
      catch (ArgumentException ex)
      {
        throw new VssServiceException(CommonResources.StringContainsIllegalChars(), (Exception) ex);
      }
    }

    public static byte[] ArrayOfByteFromXml(XmlReader reader)
    {
      string s = XmlUtility.StringFromXmlElement(reader);
      return s != null ? Convert.FromBase64String(s) : XmlUtility.ZeroLengthArrayOfByte;
    }

    public static byte[] ArrayOfByteFromXmlAttribute(XmlReader reader) => reader.Value.Length != 0 ? Convert.FromBase64String(reader.Value) : XmlUtility.ZeroLengthArrayOfByte;

    public static byte[] ZeroLengthArrayOfByte
    {
      get
      {
        if (XmlUtility.s_zeroLengthArrayOfByte == null)
          XmlUtility.s_zeroLengthArrayOfByte = Array.Empty<byte>();
        return XmlUtility.s_zeroLengthArrayOfByte;
      }
    }

    public static bool CompareXmlDocuments(string xml1, string xml2)
    {
      if (xml1 == xml2)
        return true;
      return !string.IsNullOrEmpty(xml1) && !string.IsNullOrEmpty(xml2) && XmlUtility.Compare((XContainer) XDocument.Parse(xml1)?.Root, (XContainer) XDocument.Parse(xml2)?.Root);
    }

    private static bool Compare(XContainer x1, XContainer x2)
    {
      if (x1 == x2)
        return true;
      XElement xelement1 = x1 as XElement;
      XElement xelement2 = x2 as XElement;
      return xelement1 != null && xelement2 != null && VssStringComparer.XmlNodeName.Equals(xelement1.Name.ToString(), xelement2.Name.ToString()) && xelement1.Attributes().OrderBy<XAttribute, string>((Func<XAttribute, string>) (a => a.Name.ToString())).SequenceEqual<XAttribute>((IEnumerable<XAttribute>) xelement2.Attributes().OrderBy<XAttribute, string>((Func<XAttribute, string>) (a => a.Name.ToString())), (IEqualityComparer<XAttribute>) XmlUtility.s_xmlAttributeComparer) && VssStringComparer.XmlElement.Equals(xelement1.Value, xelement2.Value) && x1.Elements().OrderBy<XElement, string>((Func<XElement, string>) (xe => xe.Name.ToString())).SequenceEqual<XElement>((IEnumerable<XElement>) x2.Elements().OrderBy<XElement, string>((Func<XElement, string>) (xe => xe.Name.ToString())), (IEqualityComparer<XElement>) XmlUtility.s_xmlElementComparer);
    }

    private static string GetCachedString(string fromXml)
    {
      if (fromXml == null)
        return (string) null;
      int length = fromXml.Length;
      if (length > 256)
        return fromXml;
      if (length == 0)
        return string.Empty;
      string[] strArray = XmlUtility.ts_stringList;
      if (strArray == null)
      {
        strArray = new string[16];
        XmlUtility.ts_stringList = strArray;
      }
      for (int index1 = 0; index1 < 16; ++index1)
      {
        string b = strArray[index1];
        if (b != null)
        {
          if (b.Length == length && (int) fromXml[0] == (int) b[0] && (length <= 5 || (int) fromXml[length - 5] == (int) b[length - 5]) && string.Equals(fromXml, b, StringComparison.Ordinal))
          {
            for (int index2 = index1 - 1; index2 >= 0; --index2)
              strArray[index2 + 1] = strArray[index2];
            strArray[0] = b;
            return b;
          }
        }
        else
          break;
      }
      for (int index = 14; index >= 0; --index)
        strArray[index + 1] = strArray[index];
      strArray[0] = fromXml;
      return fromXml;
    }

    private class AttributeComparer : IEqualityComparer<XAttribute>
    {
      public bool Equals(XAttribute x, XAttribute y)
      {
        if (x == y)
          return true;
        return x != null && y != null && VssStringComparer.XmlAttributeName.Equals(x.Name.ToString(), y.Name.ToString()) && VssStringComparer.XmlAttributeValue.Equals(x.Value, y.Value);
      }

      public int GetHashCode(XAttribute obj) => obj == null ? 0 : obj.GetHashCode();
    }

    private class ElementComparer : IEqualityComparer<XElement>
    {
      public bool Equals(XElement x, XElement y)
      {
        if (x == y)
          return true;
        return x != null && y != null && XmlUtility.Compare((XContainer) x, (XContainer) y);
      }

      public int GetHashCode(XElement obj) => obj == null ? 0 : obj.GetHashCode();
    }
  }
}
