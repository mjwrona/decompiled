// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.KeyValueOfStringString
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class KeyValueOfStringString
  {
    private string m_key;
    private string m_value;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public KeyValueOfStringString(KeyValuePair<string, string> keyValuePair)
    {
      this.Key = keyValuePair.Key;
      this.Value = keyValuePair.Value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static KeyValueOfStringString[] Convert(IEnumerable<KeyValuePair<string, string>> pairs)
    {
      if (pairs == null)
        return (KeyValueOfStringString[]) null;
      List<KeyValueOfStringString> valueOfStringStringList = new List<KeyValueOfStringString>();
      foreach (KeyValuePair<string, string> pair in pairs)
        valueOfStringStringList.Add(new KeyValueOfStringString(pair));
      return valueOfStringStringList.ToArray();
    }

    private KeyValueOfStringString()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Key
    {
      get => this.m_key;
      set => this.m_key = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Value
    {
      get => this.m_value;
      set => this.m_value = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static KeyValueOfStringString FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      KeyValueOfStringString valueOfStringString = new KeyValueOfStringString();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "Key":
              valueOfStringString.m_key = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Value":
              valueOfStringString.m_value = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return valueOfStringString;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("KeyValueOfStringString instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Key: " + this.m_key);
      stringBuilder.AppendLine("  Value: " + this.m_value);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_key != null)
        XmlUtility.ToXmlElement(writer, "Key", this.m_key);
      if (this.m_value != null)
        XmlUtility.ToXmlElement(writer, "Value", this.m_value);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, KeyValueOfStringString obj) => obj.ToXml(writer, element);
  }
}
