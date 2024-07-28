// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RegistryEntry
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class RegistryEntry
  {
    private string m_name;
    private string m_path;
    private string m_value;

    public RegistryEntry(string registryPath, string registryValue)
    {
      this.m_path = registryPath;
      this.m_value = registryValue;
    }

    public string Name
    {
      get
      {
        if (this.m_name == null)
          this.m_name = RegistryUtility.GetKeyName(this.m_path);
        return this.m_name;
      }
    }

    public string GetValue(string defaultValue) => this.m_value == null ? defaultValue : this.m_value;

    public T GetValue<T>() => RegistryUtility.FromString<T>(this.m_value);

    public T GetValue<T>(T defaultValue) => RegistryUtility.FromString<T>(this.m_value, defaultValue);

    public string Value
    {
      get => this.m_value;
      set => this.m_value = value;
    }

    private RegistryEntry()
    {
    }

    public string Path => this.m_path;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static RegistryEntry FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      RegistryEntry registryEntry = new RegistryEntry();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          if (reader.Name == "Path")
            registryEntry.m_path = XmlUtility.StringFromXmlAttribute(reader);
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "Value")
            registryEntry.m_value = XmlUtility.StringFromXmlElement(reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return registryEntry;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("RegistryEntry instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Path: " + this.m_path);
      stringBuilder.AppendLine("  Value: " + this.m_value);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_path != null)
        XmlUtility.ToXmlAttribute(writer, "Path", this.m_path);
      if (this.m_value != null)
        XmlUtility.ToXmlElement(writer, "Value", this.m_value);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, RegistryEntry obj) => obj.ToXml(writer, element);
  }
}
