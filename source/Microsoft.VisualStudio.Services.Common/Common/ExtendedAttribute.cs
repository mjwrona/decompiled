// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ExtendedAttribute
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Common
{
  public class ExtendedAttribute
  {
    private string m_Name;
    private string m_Value;
    private string m_FormatString;

    public string Name
    {
      get => this.m_Name;
      set => this.m_Name = value;
    }

    public string Value
    {
      get => this.m_Value;
      set => this.m_Value = value;
    }

    public string FormatString
    {
      get => this.m_FormatString;
      set => this.m_FormatString = value;
    }

    internal static ExtendedAttribute FromXml(XmlReader reader)
    {
      ExtendedAttribute extendedAttribute = new ExtendedAttribute();
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
            case "FormatString":
              extendedAttribute.m_FormatString = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Name":
              extendedAttribute.m_Name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Value":
              extendedAttribute.m_Value = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return extendedAttribute;
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_FormatString != null)
        XmlUtility.ToXmlElement(writer, "FormatString", this.m_FormatString);
      if (this.m_Name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.m_Name);
      if (this.m_Value != null)
        XmlUtility.ToXmlElement(writer, "Value", this.m_Value);
      writer.WriteEndElement();
    }

    internal static ExtendedAttribute[] ExtendedAttributeArrayFromXml(XmlReader reader)
    {
      List<ExtendedAttribute> extendedAttributeList = new List<ExtendedAttribute>();
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.HasAttributes && reader.GetAttribute("xsi:nil") == "true")
          {
            extendedAttributeList.Add((ExtendedAttribute) null);
            reader.Read();
          }
          else
            extendedAttributeList.Add(ExtendedAttribute.FromXml(reader));
        }
        reader.ReadEndElement();
      }
      return extendedAttributeList.ToArray();
    }

    internal static void ExtendedAttributeArrayToXml(
      XmlWriter writer,
      string element,
      ExtendedAttribute[] array)
    {
      if (array == null || array.Length == 0)
        return;
      writer.WriteStartElement(element);
      for (int index = 0; index < array.Length; ++index)
      {
        if (array[index] == null)
          throw new ArgumentNullException("array[" + index.ToString() + "]");
        array[index].ToXml(writer, nameof (ExtendedAttribute));
      }
      writer.WriteEndElement();
    }
  }
}
