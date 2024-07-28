// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.FrameworkTemplateHeader
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class FrameworkTemplateHeader
  {
    private string m_description;
    private string m_metadata;
    private string m_name;
    private int m_rank;
    private string m_state;
    private int m_templateId;

    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    public string Metadata
    {
      get => this.m_metadata;
      set => this.m_metadata = value;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public int Rank
    {
      get => this.m_rank;
      set => this.m_rank = value;
    }

    public string State
    {
      get => this.m_state;
      set => this.m_state = value;
    }

    public int TemplateId
    {
      get => this.m_templateId;
      set => this.m_templateId = value;
    }

    internal static FrameworkTemplateHeader FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      FrameworkTemplateHeader frameworkTemplateHeader = new FrameworkTemplateHeader();
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
            case "Description":
              frameworkTemplateHeader.m_description = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Metadata":
              frameworkTemplateHeader.m_metadata = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Name":
              frameworkTemplateHeader.m_name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Rank":
              frameworkTemplateHeader.m_rank = XmlUtility.Int32FromXmlElement(reader);
              continue;
            case "State":
              frameworkTemplateHeader.m_state = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "TemplateId":
              frameworkTemplateHeader.m_templateId = XmlUtility.Int32FromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return frameworkTemplateHeader;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("FrameworkTemplateHeader instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Description: " + this.m_description);
      stringBuilder.AppendLine("  Metadata: " + this.m_metadata);
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  Rank: " + this.m_rank.ToString());
      stringBuilder.AppendLine("  State: " + this.m_state);
      stringBuilder.AppendLine("  TemplateId: " + this.m_templateId.ToString());
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_description != null)
        XmlUtility.ToXmlElement(writer, "Description", this.m_description);
      if (this.m_metadata != null)
        XmlUtility.ToXmlElement(writer, "Metadata", this.m_metadata);
      if (this.m_name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.m_name);
      if (this.m_rank != 0)
        XmlUtility.ToXmlElement(writer, "Rank", this.m_rank);
      if (this.m_state != null)
        XmlUtility.ToXmlElement(writer, "State", this.m_state);
      if (this.m_templateId != 0)
        XmlUtility.ToXmlElement(writer, "TemplateId", this.m_templateId);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, FrameworkTemplateHeader obj) => obj.ToXml(writer, element);
  }
}
