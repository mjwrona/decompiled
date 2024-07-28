// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.OutboundLink
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Common
{
  public class OutboundLink
  {
    private string m_LinkType;
    private string m_ReferencedUri;

    public string LinkType
    {
      get => this.m_LinkType;
      set => this.m_LinkType = value;
    }

    public string ReferencedUri
    {
      get => this.m_ReferencedUri;
      set => this.m_ReferencedUri = value;
    }

    internal static OutboundLink FromXml(XmlReader reader)
    {
      OutboundLink outboundLink = new OutboundLink();
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
            case "LinkType":
              outboundLink.m_LinkType = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "ReferencedUri":
              outboundLink.m_ReferencedUri = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return outboundLink;
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_LinkType != null)
        XmlUtility.ToXmlElement(writer, "LinkType", this.m_LinkType);
      if (this.m_ReferencedUri != null)
        XmlUtility.ToXmlElement(writer, "ReferencedUri", this.m_ReferencedUri);
      writer.WriteEndElement();
    }

    internal static OutboundLink[] OutboundLinkArrayFromXml(XmlReader reader)
    {
      List<OutboundLink> outboundLinkList = new List<OutboundLink>();
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.HasAttributes && reader.GetAttribute("xsi:nil") == "true")
          {
            outboundLinkList.Add((OutboundLink) null);
            reader.Read();
          }
          else
            outboundLinkList.Add(OutboundLink.FromXml(reader));
        }
        reader.ReadEndElement();
      }
      return outboundLinkList.ToArray();
    }

    internal static void OutboundLinkArrayToXml(
      XmlWriter writer,
      string element,
      OutboundLink[] array)
    {
      if (array == null || array.Length == 0)
        return;
      writer.WriteStartElement(element);
      for (int index = 0; index < array.Length; ++index)
      {
        if (array[index] == null)
          throw new ArgumentNullException("array[" + index.ToString() + "]");
        array[index].ToXml(writer, nameof (OutboundLink));
      }
      writer.WriteEndElement();
    }
  }
}
