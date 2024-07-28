// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ReferenceLinks
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [XmlRoot("ReferenceLinks")]
  [JsonConverter(typeof (ReferenceLinks.ReferenceLinksConverter))]
  public class ReferenceLinks : ICloneable, IXmlSerializable
  {
    private IDictionary<string, object> referenceLinks = (IDictionary<string, object>) new Dictionary<string, object>();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AddLink(string name, string href, ISecuredObject securedObject)
    {
      if (this.referenceLinks.ContainsKey(name))
      {
        IList<ReferenceLink> referenceLinkList;
        if (this.referenceLinks[name] is ReferenceLink)
        {
          referenceLinkList = (IList<ReferenceLink>) new List<ReferenceLink>();
          referenceLinkList.Add((ReferenceLink) this.referenceLinks[name]);
          this.referenceLinks[name] = (object) referenceLinkList;
        }
        else
          referenceLinkList = (IList<ReferenceLink>) this.referenceLinks[name];
        referenceLinkList.Add(new ReferenceLink(securedObject)
        {
          Href = href
        });
      }
      else
        this.referenceLinks[name] = (object) new ReferenceLink(securedObject)
        {
          Href = href
        };
    }

    public void AddLink(string name, string href) => this.AddLink(name, href, (ISecuredObject) null);

    public void AddLinkIfIsNotEmpty(string name, string href)
    {
      if (string.IsNullOrEmpty(href))
        return;
      this.AddLink(name, href, (ISecuredObject) null);
    }

    object ICloneable.Clone() => (object) this.Clone();

    public ReferenceLinks Clone()
    {
      ReferenceLinks target = new ReferenceLinks();
      this.CopyTo(target);
      return target;
    }

    public void CopyTo(ReferenceLinks target) => this.CopyTo(target, (ISecuredObject) null);

    public void CopyTo(ReferenceLinks target, ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ReferenceLinks>(target, nameof (target));
      foreach (KeyValuePair<string, object> link in (IEnumerable<KeyValuePair<string, object>>) this.Links)
      {
        if (link.Value is IList<ReferenceLink>)
        {
          if (link.Value is IList<ReferenceLink> referenceLinkList)
          {
            foreach (ReferenceLink referenceLink in (IEnumerable<ReferenceLink>) referenceLinkList)
              target.AddLink(link.Key, referenceLink.Href, securedObject);
          }
        }
        else if (link.Value is ReferenceLink && link.Value is ReferenceLink referenceLink1)
          target.AddLink(link.Key, referenceLink1.Href, securedObject);
      }
    }

    XmlSchema IXmlSerializable.GetSchema() => (XmlSchema) null;

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      XmlSerializer xmlSerializer1 = new XmlSerializer(typeof (string));
      XmlSerializer xmlSerializer2 = new XmlSerializer(typeof (List<ReferenceLink>));
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num != 0)
        return;
      while (reader.NodeType != XmlNodeType.EndElement)
      {
        reader.ReadStartElement("item");
        reader.ReadStartElement("key");
        string key = (string) xmlSerializer1.Deserialize(reader);
        reader.ReadEndElement();
        reader.ReadStartElement("value");
        List<ReferenceLink> referenceLinkList = (List<ReferenceLink>) xmlSerializer2.Deserialize(reader);
        reader.ReadEndElement();
        if (referenceLinkList.Count == 1)
          this.referenceLinks.Add(key, (object) referenceLinkList[0]);
        else if (referenceLinkList.Count > 1)
          this.referenceLinks.Add(key, (object) referenceLinkList);
        reader.ReadEndElement();
        int content = (int) reader.MoveToContent();
      }
      reader.ReadEndElement();
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
      XmlSerializer xmlSerializer1 = new XmlSerializer(typeof (string));
      XmlSerializer xmlSerializer2 = new XmlSerializer(typeof (List<ReferenceLink>));
      foreach (KeyValuePair<string, object> referenceLink in (IEnumerable<KeyValuePair<string, object>>) this.referenceLinks)
      {
        writer.WriteStartElement("item");
        writer.WriteStartElement("key");
        xmlSerializer1.Serialize(writer, (object) referenceLink.Key);
        writer.WriteEndElement();
        writer.WriteStartElement("value");
        if (!(referenceLink.Value is List<ReferenceLink> o))
          o = new List<ReferenceLink>()
          {
            (ReferenceLink) referenceLink.Value
          };
        xmlSerializer2.Serialize(writer, (object) o);
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }

    public IReadOnlyDictionary<string, object> Links => (IReadOnlyDictionary<string, object>) new ReadOnlyDictionary<string, object>(this.referenceLinks);

    private class ReferenceLinksConverter : VssSecureJsonConverter
    {
      public override bool CanConvert(Type objectType) => objectType == typeof (ReferenceLinks);

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        Dictionary<string, object> dictionary1 = serializer.Deserialize<Dictionary<string, object>>(reader);
        if (dictionary1 == null)
          return (object) null;
        Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> keyValuePair in dictionary1)
        {
          if (string.IsNullOrEmpty(keyValuePair.Key))
            throw new JsonSerializationException(WebApiResources.InvalidReferenceLinkFormat());
          if (keyValuePair.Value is JToken jtoken)
          {
            switch (jtoken.Type)
            {
              case JTokenType.Object:
                using (JsonReader reader1 = jtoken.CreateReader())
                {
                  dictionary2[keyValuePair.Key] = (object) serializer.Deserialize<ReferenceLink>(reader1);
                  continue;
                }
              case JTokenType.Array:
                using (JsonReader reader2 = jtoken.CreateReader())
                {
                  dictionary2[keyValuePair.Key] = (object) serializer.Deserialize<IList<ReferenceLink>>(reader2);
                  continue;
                }
              default:
                throw new JsonSerializationException(WebApiResources.InvalidReferenceLinkFormat());
            }
          }
          else
            dictionary2[keyValuePair.Key] = keyValuePair.Value is ReferenceLink || keyValuePair.Value is IList<ReferenceLink> ? keyValuePair.Value : throw new JsonSerializationException(WebApiResources.InvalidReferenceLinkFormat());
        }
        return (object) new ReferenceLinks()
        {
          referenceLinks = (IDictionary<string, object>) dictionary2
        };
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        base.WriteJson(writer, value, serializer);
        serializer.Serialize(writer, (object) ((ReferenceLinks) value).referenceLinks);
      }
    }
  }
}
