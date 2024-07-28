// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SerializationHelper
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class SerializationHelper
  {
    private static readonly XmlReaderSettings s_defaultXmlReaderSettings = new XmlReaderSettings()
    {
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null
    };

    public static T DeserializeDataContract<T>(string value) => SerializationHelper.DeserializeDataContract<T>(value, (XmlReaderSettings) null);

    public static T DeserializeDataContract<T>(string value, XmlReaderSettings settings)
    {
      T obj = default (T);
      XmlReader reader = (XmlReader) null;
      using (StringReader input = new StringReader(value))
      {
        try
        {
          reader = settings == null ? XmlReader.Create((TextReader) input, SerializationHelper.s_defaultXmlReaderSettings) : XmlReader.Create((TextReader) input, settings);
          return (T) new DataContractSerializer(typeof (T)).ReadObject(reader);
        }
        finally
        {
          reader?.Close();
        }
      }
    }

    public static T DeserializeDataContract<T>(
      string value,
      string rootName,
      string rootNamespace,
      XmlReaderSettings settings)
    {
      T obj = default (T);
      XmlReader reader = (XmlReader) null;
      using (StringReader input = new StringReader(value))
      {
        try
        {
          reader = settings == null ? XmlReader.Create((TextReader) input, SerializationHelper.s_defaultXmlReaderSettings) : XmlReader.Create((TextReader) input, settings);
          return (T) (!string.IsNullOrEmpty(rootName) ? (XmlObjectSerializer) new DataContractSerializer(typeof (T), rootName, rootNamespace) : (XmlObjectSerializer) new DataContractSerializer(typeof (T))).ReadObject(reader);
        }
        finally
        {
          reader?.Close();
        }
      }
    }

    public static T DeserializeXml<T>(string xmlText) => SerializationHelper.DeserializeXml<T>(xmlText, (XmlReaderSettings) null);

    public static T DeserializeXml<T>(string xmlText, XmlReaderSettings settings)
    {
      T obj = default (T);
      XmlReader xmlReader = (XmlReader) null;
      using (StringReader input = new StringReader(xmlText))
      {
        try
        {
          xmlReader = settings == null ? XmlReader.Create((TextReader) input, SerializationHelper.s_defaultXmlReaderSettings) : XmlReader.Create((TextReader) input, settings);
          return (T) new XmlSerializer(typeof (T)).Deserialize(xmlReader);
        }
        finally
        {
          xmlReader?.Close();
          input.Close();
        }
      }
    }

    public static string Serialize<T>(
      T item,
      XmlWriterSettings settings,
      XmlObjectSerializer serializer)
    {
      string str = (string) null;
      if (serializer != null)
      {
        XmlWriter writer = (XmlWriter) null;
        using (MemoryStream output = new MemoryStream())
        {
          try
          {
            writer = XmlWriter.Create((Stream) output, settings);
            serializer.WriteObject(writer, (object) item);
            writer.Flush();
            str = SerializationHelper.ConvertToString((Stream) output);
          }
          finally
          {
            writer?.Close();
          }
        }
      }
      return str?.ToString();
    }

    public static string SerializeDataContract<T>(T item) => SerializationHelper.SerializeDataContract<T>(item, (string) null, (string) null, (XmlWriterSettings) null);

    public static string SerializeDataContract<T>(T item, XmlWriterSettings settings) => SerializationHelper.SerializeDataContract<T>(item, (string) null, (string) null, settings);

    public static string SerializeDataContract<T>(
      T item,
      string rootName,
      string rootNamespace,
      XmlWriterSettings settings)
    {
      string str = (string) null;
      XmlWriter writer = (XmlWriter) null;
      using (MemoryStream output = new MemoryStream())
      {
        try
        {
          writer = XmlWriter.Create((Stream) output, settings);
          (!string.IsNullOrEmpty(rootName) ? (XmlObjectSerializer) new DataContractSerializer(typeof (T), rootName, rootNamespace) : (XmlObjectSerializer) new DataContractSerializer(typeof (T))).WriteObject(writer, (object) item);
          writer.Flush();
          str = SerializationHelper.ConvertToString((Stream) output);
        }
        finally
        {
          writer?.Close();
          output.Close();
        }
      }
      return str?.ToString();
    }

    public static string SerializeXml<T>(T item) => SerializationHelper.SerializeXml<T>(item, (XmlWriterSettings) null, (XmlSerializerNamespaces) null);

    public static string SerializeXml<T>(T item, XmlWriterSettings settings) => SerializationHelper.SerializeXml<T>(item, settings, (XmlSerializerNamespaces) null);

    public static string SerializeXml<T>(
      T item,
      XmlWriterSettings settings,
      XmlSerializerNamespaces namespaces)
    {
      string str = (string) null;
      XmlWriter xmlWriter = (XmlWriter) null;
      using (MemoryStream output = new MemoryStream())
      {
        try
        {
          xmlWriter = XmlWriter.Create((Stream) output, settings);
          XmlSerializer xmlSerializer = new XmlSerializer(typeof (T));
          if (namespaces == null)
            xmlSerializer.Serialize(xmlWriter, (object) item);
          else
            xmlSerializer.Serialize(xmlWriter, (object) item, namespaces);
          xmlWriter.Flush();
          str = SerializationHelper.ConvertToString((Stream) output);
        }
        finally
        {
          xmlWriter?.Close();
          output.Close();
        }
      }
      return str?.ToString();
    }

    private static string ConvertToString(Stream stream)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (stream != null)
      {
        stream.Seek(0L, SeekOrigin.Begin);
        StreamReader streamReader = new StreamReader(stream);
        stringBuilder.Append(streamReader.ReadToEnd());
      }
      return stringBuilder.ToString();
    }
  }
}
