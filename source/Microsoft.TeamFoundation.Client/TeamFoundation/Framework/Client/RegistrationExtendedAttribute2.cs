// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RegistrationExtendedAttribute2
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class RegistrationExtendedAttribute2
  {
    private string m_name;
    private string m_value;

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public string Value
    {
      get => this.m_value;
      set => this.m_value = value;
    }

    internal static RegistrationExtendedAttribute2 FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      RegistrationExtendedAttribute2 extendedAttribute2 = new RegistrationExtendedAttribute2();
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
            case "Name":
              extendedAttribute2.m_name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Value":
              extendedAttribute2.m_value = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return extendedAttribute2;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("RegistrationExtendedAttribute2 instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  Value: " + this.m_value);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.m_name);
      if (this.m_value != null)
        XmlUtility.ToXmlElement(writer, "Value", this.m_value);
      writer.WriteEndElement();
    }

    internal static void ToXml(
      XmlWriter writer,
      string element,
      RegistrationExtendedAttribute2 obj)
    {
      obj.ToXml(writer, element);
    }

    internal static RegistrationExtendedAttribute[] Convert(
      RegistrationExtendedAttribute2[] extendedAttributes)
    {
      if (extendedAttributes == null || extendedAttributes.Length == 0)
        return Array.Empty<RegistrationExtendedAttribute>();
      RegistrationExtendedAttribute[] extendedAttributeArray = new RegistrationExtendedAttribute[extendedAttributes.Length];
      for (int index = 0; index < extendedAttributes.Length; ++index)
      {
        if (extendedAttributes[index] != null)
          extendedAttributeArray[index] = extendedAttributes[index].ToExtendedAttribute();
      }
      return extendedAttributeArray;
    }

    internal RegistrationExtendedAttribute ToExtendedAttribute() => new RegistrationExtendedAttribute(this.Name, this.Value);
  }
}
