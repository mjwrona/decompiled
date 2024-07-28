// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationsSerialization
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class NotificationsSerialization
  {
    private static readonly XmlWriterSettings CondensedXmlObjectWriterSettings = new XmlWriterSettings()
    {
      Indent = false,
      CheckCharacters = false
    };
    private static readonly XmlWriterSettings XmlObjectWriterSettings = new XmlWriterSettings()
    {
      Indent = true,
      CheckCharacters = false
    };
    private static readonly XmlReaderSettings XmlObjectReaderSettings = new XmlReaderSettings()
    {
      ConformanceLevel = ConformanceLevel.Fragment,
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null,
      CheckCharacters = false
    };
    internal static string s_notifTypeProperty = "$notifObjectType";

    public static string SerializeToXml(object eventObject, Type type, bool formatXml)
    {
      XmlSerializer xmlSerializer = new XmlSerializer(type);
      XmlWriterSettings settings = !formatXml ? NotificationsSerialization.CondensedXmlObjectWriterSettings : NotificationsSerialization.XmlObjectWriterSettings;
      using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output, settings))
        {
          xmlSerializer.Serialize(xmlWriter, eventObject);
          return output.ToString();
        }
      }
    }

    public static string SerializeToXml(object eventObject, bool formatXml) => NotificationsSerialization.SerializeToXml(eventObject, eventObject.GetType(), formatXml);

    public static XmlDocument SerializeToXmlDoc(object eventObject, Type type, bool formatXml) => NotificationsSerialization.LoadXml(NotificationsSerialization.SerializeToXml(eventObject, type, formatXml));

    public static XmlDocument SerializeToXmlDoc(object eventObject, bool formatXml) => NotificationsSerialization.SerializeToXmlDoc(eventObject, eventObject.GetType(), formatXml);

    public static XmlDocument LoadXml(string xmlString)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.XmlResolver = (XmlResolver) null;
      using (StringReader input = new StringReader(xmlString))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) input, NotificationsSerialization.XmlObjectReaderSettings))
        {
          xmlDocument.Load(reader);
          return xmlDocument;
        }
      }
    }

    public static object DeserializeFromXmlDoc(XmlDocument doc, Type type)
    {
      using (XmlReader xmlReader = (XmlReader) new XmlNodeReader((XmlNode) doc))
        return new XmlSerializer(type).Deserialize(xmlReader);
    }

    public static T DeserializeFromXmlDoc<T>(XmlDocument doc) => (T) NotificationsSerialization.DeserializeFromXmlDoc(doc, typeof (T));

    public static object DeserializeFromXml(string xml, Type type)
    {
      using (StringReader input = new StringReader(xml))
      {
        using (XmlReader xmlReader = XmlReader.Create((TextReader) input, NotificationsSerialization.XmlObjectReaderSettings))
          return new XmlSerializer(type).Deserialize(xmlReader);
      }
    }

    public static T DeserializeFromXml<T>(string xml) => (T) NotificationsSerialization.DeserializeFromXml(xml, typeof (T));

    public static JsonSerializerSettings SubscriptionFilterJsonSerializerSettings { get; } = new JsonSerializerSettings()
    {
      MissingMemberHandling = MissingMemberHandling.Ignore,
      TypeNameHandling = TypeNameHandling.None,
      ContractResolver = (IContractResolver) new NotificationsCamelCasePropertyNamesContractResolver(),
      Converters = (IList<JsonConverter>) new List<JsonConverter>()
      {
        (JsonConverter) new SubscriptionFilterConverter()
      }
    };

    public static JsonSerializerSettings NotificationDiagnosticLogJsonSerializerSettings { get; } = new JsonSerializerSettings()
    {
      MissingMemberHandling = MissingMemberHandling.Ignore,
      TypeNameHandling = TypeNameHandling.None,
      ContractResolver = (IContractResolver) new NotificationsCamelCasePropertyNamesPreserveEnumsContractResolver(),
      DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
      Converters = (IList<JsonConverter>) new List<JsonConverter>()
      {
        (JsonConverter) new NotificationDiagnosticLogConverter(),
        (JsonConverter) new StringEnumConverter()
        {
          NamingStrategy = (NamingStrategy) new CamelCaseNamingStrategy(),
          AllowIntegerValues = true
        }
      }
    };

    public static JsonSerializerSettings EmbedTypeJsonSerializerSettings { get; } = new JsonSerializerSettings()
    {
      MissingMemberHandling = MissingMemberHandling.Ignore,
      TypeNameHandling = TypeNameHandling.None,
      ContractResolver = (IContractResolver) new NotificationsEmbedTypeContractResolver()
    };

    public static JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings()
    {
      MissingMemberHandling = MissingMemberHandling.Ignore,
      TypeNameHandling = TypeNameHandling.None,
      ContractResolver = (IContractResolver) new NotificationsCamelCasePropertyNamesContractResolver()
    };

    public static JsonSerializerSettings JsonMinimalSerializerSettings { get; } = new JsonSerializerSettings()
    {
      MissingMemberHandling = MissingMemberHandling.Ignore,
      TypeNameHandling = TypeNameHandling.None,
      NullValueHandling = NullValueHandling.Ignore,
      ContractResolver = (IContractResolver) new NotificationsCamelCasePropertyNamesContractResolver()
    };

    public static string GetNotifObjectType(JObject jObject) => jObject.GetValue(NotificationsSerialization.s_notifTypeProperty)?.ToString();
  }
}
