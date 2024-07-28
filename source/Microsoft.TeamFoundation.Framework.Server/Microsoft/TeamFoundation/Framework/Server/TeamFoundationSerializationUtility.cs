// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationSerializationUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class TeamFoundationSerializationUtility
  {
    private static XmlReaderSettings s_readerSettings = new XmlReaderSettings()
    {
      ConformanceLevel = ConformanceLevel.Fragment,
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null
    };
    private static XmlWriterSettings s_writerSettings = new XmlWriterSettings()
    {
      Indent = false,
      OmitXmlDeclaration = true
    };
    private static ConcurrentDictionary<string, XmlSerializer> sm_serializerCache = new ConcurrentDictionary<string, XmlSerializer>((IEqualityComparer<string>) StringComparer.Ordinal);
    private static readonly string s_area = "Serialization";
    private static readonly string s_layer = nameof (TeamFoundationSerializationUtility);

    public static XmlSerializer CreateSerializer(Type type, XmlRootAttribute root)
    {
      XmlSerializer serializer;
      if (root != null)
      {
        string key = type.FullName + ":" + (root.Namespace ?? "") + ":" + (root.DataType ?? "") + ":" + root.ElementName;
        serializer = TeamFoundationSerializationUtility.sm_serializerCache.GetOrAdd(key, (Func<string, XmlSerializer>) (_ => new XmlSerializer(type, root)));
      }
      else
        serializer = new XmlSerializer(type);
      return serializer;
    }

    public static T Deserialize<T>(string serializedObject) => TeamFoundationSerializationUtility.Deserialize<T>(serializedObject, (XmlRootAttribute) null);

    public static T Deserialize<T>(string serializedObject, XmlRootAttribute root) => TeamFoundationSerializationUtility.Deserialize<T>(serializedObject, UnknownXmlNodeProcessing.TraceWarning, root);

    public static T Deserialize<T>(
      string serializedObject,
      UnknownXmlNodeProcessing unknownNodeProcessing)
    {
      return TeamFoundationSerializationUtility.Deserialize<T>(serializedObject, unknownNodeProcessing, (XmlRootAttribute) null);
    }

    public static T Deserialize<T>(
      string serializedObject,
      UnknownXmlNodeProcessing unknownNodeProcessing,
      XmlRootAttribute root)
    {
      if (serializedObject == null)
        throw new ArgumentNullException(nameof (serializedObject));
      DeserializationErrorHandler deserializationErrorHandler = (DeserializationErrorHandler) null;
      XmlSerializer xmlSerializer = (XmlSerializer) null;
      try
      {
        xmlSerializer = TeamFoundationSerializationUtility.CreateSerializer(typeof (T), root);
        if (root == null && unknownNodeProcessing != UnknownXmlNodeProcessing.Ignore)
        {
          deserializationErrorHandler = new DeserializationErrorHandler(serializedObject, typeof (T));
          xmlSerializer.UnknownElement += new XmlElementEventHandler(deserializationErrorHandler.OnUnknownXmlElement);
          xmlSerializer.UnknownAttribute += new XmlAttributeEventHandler(deserializationErrorHandler.OnUnknownXmlAttribute);
        }
        T obj;
        using (StringReader input = new StringReader(serializedObject))
        {
          using (XmlReader xmlReader = XmlReader.Create((TextReader) input, TeamFoundationSerializationUtility.s_readerSettings))
            obj = (T) xmlSerializer.Deserialize(xmlReader);
        }
        if (unknownNodeProcessing == UnknownXmlNodeProcessing.ThrowException && deserializationErrorHandler.Errors != null && deserializationErrorHandler.Errors.Length > 0)
          throw new TeamFoundationDeserializationException(deserializationErrorHandler.Errors.ToString());
        return obj;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(9600, TraceLevel.Warning, TeamFoundationSerializationUtility.s_area, TeamFoundationSerializationUtility.s_layer, ex);
        throw;
      }
      finally
      {
        if (xmlSerializer != null && deserializationErrorHandler != null)
        {
          xmlSerializer.UnknownElement -= new XmlElementEventHandler(deserializationErrorHandler.OnUnknownXmlElement);
          xmlSerializer.UnknownAttribute -= new XmlAttributeEventHandler(deserializationErrorHandler.OnUnknownXmlAttribute);
        }
      }
    }

    public static T Deserialize<T>(XmlNode xmlNode) => TeamFoundationSerializationUtility.Deserialize<T>(xmlNode.OuterXml);

    public static T Deserialize<T>(XmlNode xmlNode, UnknownXmlNodeProcessing unknownNodeProcessing) => TeamFoundationSerializationUtility.Deserialize<T>(xmlNode.OuterXml, unknownNodeProcessing);

    public static string SerializeToString<T>(T objectToSerialize) => TeamFoundationSerializationUtility.SerializeToString<T>(objectToSerialize, (XmlRootAttribute) null);

    public static string SerializeToString<T>(T objectToSerialize, XmlRootAttribute root) => TeamFoundationSerializationUtility.SerializeToString(typeof (T), (object) objectToSerialize, root);

    public static string SerializeToString(object objectToSerialize) => TeamFoundationSerializationUtility.SerializeToString(objectToSerialize, (XmlRootAttribute) null);

    public static string SerializeToString(object objectToSerialize, XmlRootAttribute root) => objectToSerialize != null ? TeamFoundationSerializationUtility.SerializeToString(objectToSerialize.GetType(), objectToSerialize, root) : throw new ArgumentNullException(nameof (objectToSerialize));

    public static string SerializeToString(Type type, object objectToSerialize) => TeamFoundationSerializationUtility.SerializeToString(type, objectToSerialize, (XmlRootAttribute) null);

    public static string SerializeToString(
      Type type,
      object objectToSerialize,
      XmlRootAttribute root,
      XmlSerializerNamespaces namespaces = null)
    {
      try
      {
        if (type == (Type) null)
          throw new ArgumentNullException(nameof (type));
        if (objectToSerialize == null)
          throw new ArgumentNullException(nameof (objectToSerialize));
        using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output, TeamFoundationSerializationUtility.s_writerSettings))
          {
            TeamFoundationSerializationUtility.CreateSerializer(type, root).Serialize(xmlWriter, objectToSerialize, namespaces);
            return output.ToString();
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(9601, TraceLevel.Warning, TeamFoundationSerializationUtility.s_area, TeamFoundationSerializationUtility.s_layer, ex);
        throw;
      }
    }

    public static XmlDocument SerializeToDocument(object objectToSerialize) => TeamFoundationSerializationUtility.SerializeToDocument(objectToSerialize, (XmlRootAttribute) null);

    public static XmlDocument SerializeToDocument(object objectToSerialize, XmlRootAttribute root) => TeamFoundationSerializationUtility.SerializeToDocument(TeamFoundationSerializationUtility.SerializeToString(objectToSerialize, root));

    public static XmlDocument SerializeToDocument(string xmlString)
    {
      XmlDocument document = new XmlDocument();
      document.XmlResolver = (XmlResolver) null;
      using (StringReader input = new StringReader(xmlString))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) input, TeamFoundationSerializationUtility.s_readerSettings))
        {
          document.Load(reader);
          return document;
        }
      }
    }

    public static XmlNode SerializeToXml(object objectToSerialize) => TeamFoundationSerializationUtility.SerializeToXml(objectToSerialize, (XmlRootAttribute) null);

    public static XmlNode SerializeToXml(object objectToSerialize, XmlRootAttribute root) => TeamFoundationSerializationUtility.SerializeToXml(TeamFoundationSerializationUtility.SerializeToString(objectToSerialize, root));

    public static XmlNode SerializeToXml(string xmlString) => (XmlNode) TeamFoundationSerializationUtility.SerializeToDocument(xmlString).DocumentElement;
  }
}
