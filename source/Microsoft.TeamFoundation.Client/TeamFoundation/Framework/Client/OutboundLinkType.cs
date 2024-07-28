// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.OutboundLinkType
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class OutboundLinkType
  {
    private string m_name;
    private string m_targetArtifactTypeName;
    private string m_targetArtifactTypeTool;

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public string TargetArtifactTypeName
    {
      get => this.m_targetArtifactTypeName;
      set => this.m_targetArtifactTypeName = value;
    }

    public string TargetArtifactTypeTool
    {
      get => this.m_targetArtifactTypeTool;
      set => this.m_targetArtifactTypeTool = value;
    }

    internal static OutboundLinkType FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      OutboundLinkType outboundLinkType = new OutboundLinkType();
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
              outboundLinkType.m_name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "TargetArtifactTypeName":
              outboundLinkType.m_targetArtifactTypeName = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "TargetArtifactTypeTool":
              outboundLinkType.m_targetArtifactTypeTool = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return outboundLinkType;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("OutboundLinkType instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  TargetArtifactTypeName: " + this.m_targetArtifactTypeName);
      stringBuilder.AppendLine("  TargetArtifactTypeTool: " + this.m_targetArtifactTypeTool);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.m_name);
      if (this.m_targetArtifactTypeName != null)
        XmlUtility.ToXmlElement(writer, "TargetArtifactTypeName", this.m_targetArtifactTypeName);
      if (this.m_targetArtifactTypeTool != null)
        XmlUtility.ToXmlElement(writer, "TargetArtifactTypeTool", this.m_targetArtifactTypeTool);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, OutboundLinkType obj) => obj.ToXml(writer, element);

    internal static Microsoft.TeamFoundation.Server.OutboundLinkType[] Convert(
      OutboundLinkType[] outboundLinkTypes)
    {
      if (outboundLinkTypes == null || outboundLinkTypes.Length == 0)
        return Array.Empty<Microsoft.TeamFoundation.Server.OutboundLinkType>();
      Microsoft.TeamFoundation.Server.OutboundLinkType[] outboundLinkTypeArray = new Microsoft.TeamFoundation.Server.OutboundLinkType[outboundLinkTypes.Length];
      for (int index = 0; index < outboundLinkTypes.Length; ++index)
      {
        if (outboundLinkTypes[index] != null)
          outboundLinkTypeArray[index] = outboundLinkTypes[index].ToOutboundLinkType();
      }
      return outboundLinkTypeArray;
    }

    internal Microsoft.TeamFoundation.Server.OutboundLinkType ToOutboundLinkType() => new Microsoft.TeamFoundation.Server.OutboundLinkType(this.Name, this.TargetArtifactTypeTool, this.TargetArtifactTypeName);
  }
}
