// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ExceptionPropertyCollection
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.Common
{
  public sealed class ExceptionPropertyCollection : 
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable
  {
    private Dictionary<string, object> m_properties = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public int Count => this.m_properties.Count;

    public void Set(string name, bool value) => this.m_properties[name] = (object) value;

    public void Set(string name, bool[] value) => this.m_properties[name] = (object) value;

    public void Set(string name, byte value) => this.m_properties[name] = (object) value;

    public void Set(string name, byte[] value) => this.m_properties[name] = (object) value;

    public void Set(string name, char value) => this.m_properties[name] = (object) value;

    public void Set(string name, DateTime value) => this.m_properties[name] = (object) value;

    public void Set(string name, DateTime[] value) => this.m_properties[name] = (object) value;

    public void Set(string name, Decimal value) => this.m_properties[name] = (object) value;

    public void Set(string name, Decimal[] value) => this.m_properties[name] = (object) value;

    public void Set(string name, double value) => this.m_properties[name] = (object) value;

    public void Set(string name, double[] value) => this.m_properties[name] = (object) value;

    public void Set(string name, Guid value) => this.m_properties[name] = (object) value;

    public void Set(string name, Guid[] value) => this.m_properties[name] = (object) value;

    public void Set(string name, short value) => this.m_properties[name] = (object) value;

    public void Set(string name, short[] value) => this.m_properties[name] = (object) value;

    public void Set(string name, int value) => this.m_properties[name] = (object) value;

    public void Set(string name, int[] value) => this.m_properties[name] = (object) value;

    public void Set(string name, long value) => this.m_properties[name] = (object) value;

    public void Set(string name, long[] value) => this.m_properties[name] = (object) value;

    public void Set(string name, float value) => this.m_properties[name] = (object) value;

    public void Set(string name, float[] value) => this.m_properties[name] = (object) value;

    public void Set(string name, string value) => this.m_properties[name] = (object) value;

    public void Set(string name, string[] value) => this.m_properties[name] = (object) value;

    public void Remove(string name) => this.m_properties.Remove(name);

    public void Copy(ExceptionPropertyCollection properties) => this.Copy(properties, false);

    public void Copy(ExceptionPropertyCollection properties, bool replace)
    {
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
      {
        if (!this.m_properties.ContainsKey(property.Key) | replace)
          this.m_properties[property.Key] = property.Value;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_properties.GetEnumerator();

    IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => (IEnumerator<KeyValuePair<string, object>>) this.m_properties.GetEnumerator();

    internal static IEnumerable<KeyValuePair<string, object>> FromXml(XmlReader reader)
    {
      List<KeyValuePair<string, object>> keyValuePairList = new List<KeyValuePair<string, object>>();
      while (reader.NodeType == XmlNodeType.Element)
      {
        string attribute = reader.GetAttribute("name");
        if (!string.IsNullOrEmpty(attribute))
        {
          reader.Read();
          keyValuePairList.Add(new KeyValuePair<string, object>(attribute, XmlUtility.ObjectFromXmlElement(reader)));
          reader.Read();
        }
        else
          reader.ReadOuterXml();
      }
      return (IEnumerable<KeyValuePair<string, object>>) keyValuePairList;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string elementName)
    {
      writer.WriteStartElement(elementName);
      writer.WriteAttributeString("xmlns", "xsd", (string) null, "http://www.w3.org/2001/XMLSchema");
      writer.WriteAttributeString("xmlns", "xsi", (string) null, "http://www.w3.org/2001/XMLSchema-instance");
      foreach (KeyValuePair<string, object> property in this.m_properties)
      {
        writer.WriteStartElement("property");
        writer.WriteAttributeString("name", property.Key);
        XmlUtility.ObjectToXmlElement(writer, "value", property.Value);
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }
  }
}
