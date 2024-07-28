// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SerializableDictionary`2
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Common
{
  [XmlRoot("dictionary")]
  public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
  {
    public XmlSchema GetSchema() => (XmlSchema) null;

    public void ReadXml(XmlReader reader)
    {
      XmlSerializer xmlSerializer1 = new XmlSerializer(typeof (TKey));
      XmlSerializer xmlSerializer2 = new XmlSerializer(typeof (TValue));
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num != 0)
        return;
      while (reader.NodeType != XmlNodeType.EndElement)
      {
        reader.ReadStartElement("item");
        reader.ReadStartElement("key");
        TKey key = (TKey) xmlSerializer1.Deserialize(reader);
        reader.ReadEndElement();
        reader.ReadStartElement("value");
        TValue obj = (TValue) xmlSerializer2.Deserialize(reader);
        reader.ReadEndElement();
        this.Add(key, obj);
        reader.ReadEndElement();
        int content = (int) reader.MoveToContent();
      }
      reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
      XmlSerializer xmlSerializer1 = new XmlSerializer(typeof (TKey));
      XmlSerializer xmlSerializer2 = new XmlSerializer(typeof (TValue));
      foreach (TKey key in this.Keys)
      {
        writer.WriteStartElement("item");
        writer.WriteStartElement("key");
        xmlSerializer1.Serialize(writer, (object) key);
        writer.WriteEndElement();
        writer.WriteStartElement("value");
        TValue o = this[key];
        xmlSerializer2.Serialize(writer, (object) o);
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }
  }
}
