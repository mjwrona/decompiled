// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.LinkFilter
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Common
{
  public class LinkFilter
  {
    private FilterType m_FilterType;
    private string[] m_FilterValues;

    public FilterType FilterType
    {
      get => this.m_FilterType;
      set => this.m_FilterType = value;
    }

    public string[] FilterValues
    {
      get => this.m_FilterValues;
      set => this.m_FilterValues = value;
    }

    internal static LinkFilter FromXml(XmlReader reader)
    {
      LinkFilter linkFilter = new LinkFilter();
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
            case "FilterType":
              linkFilter.m_FilterType = XmlUtility.EnumFromXmlElement<FilterType>(reader);
              continue;
            case "FilterValues":
              linkFilter.m_FilterValues = LinkFilter.StringArrayFromXml(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return linkFilter;
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_FilterType != FilterType.ToolType)
        XmlUtility.EnumToXmlElement<FilterType>(writer, "FilterType", this.m_FilterType);
      LinkFilter.StringArrayToXmlElement(writer, "FilterValues", this.m_FilterValues);
      writer.WriteEndElement();
    }

    private static string[] StringArrayFromXml(XmlReader reader)
    {
      List<string> stringList = new List<string>();
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.HasAttributes && reader.GetAttribute("xsi:nil") == "true")
          {
            stringList.Add((string) null);
            reader.Read();
          }
          else
            stringList.Add(XmlUtility.StringFromXmlElement(reader));
        }
        reader.ReadEndElement();
      }
      return stringList.ToArray();
    }

    private static void StringArrayToXmlElement(XmlWriter writer, string element, string[] array)
    {
      if (array == null || array.Length == 0)
        return;
      writer.WriteStartElement(element);
      for (int index = 0; index < array.Length; ++index)
        XmlUtility.ToXmlElement(writer, "string", array[index]);
      writer.WriteEndElement();
    }

    public static LinkFilter[] LinkFilterArrayFromXml(XmlReader reader)
    {
      List<LinkFilter> linkFilterList = new List<LinkFilter>();
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.HasAttributes && reader.GetAttribute("xsi:nil") == "true")
          {
            linkFilterList.Add((LinkFilter) null);
            reader.Read();
          }
          else
            linkFilterList.Add(LinkFilter.FromXml(reader));
        }
        reader.ReadEndElement();
      }
      return linkFilterList.ToArray();
    }

    public static void LinkFilterArrayToXml(XmlWriter writer, string element, LinkFilter[] array)
    {
      if (array == null || array.Length == 0)
        return;
      writer.WriteStartElement(element);
      for (int index = 0; index < array.Length; ++index)
      {
        if (array[index] == null)
          throw new ArgumentNullException("array[" + index.ToString() + "]");
        array[index].ToXml(writer, nameof (LinkFilter));
      }
      writer.WriteEndElement();
    }
  }
}
