// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RegistrationArtifactType
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
  internal sealed class RegistrationArtifactType
  {
    private string m_name;
    internal OutboundLinkType[] m_outboundLinkTypes = Helper.ZeroLengthArrayOfOutboundLinkType;

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public OutboundLinkType[] OutboundLinkTypes
    {
      get => (OutboundLinkType[]) this.m_outboundLinkTypes.Clone();
      set => this.m_outboundLinkTypes = value;
    }

    internal static RegistrationArtifactType FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      RegistrationArtifactType registrationArtifactType = new RegistrationArtifactType();
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
              registrationArtifactType.m_name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "OutboundLinkTypes":
              registrationArtifactType.m_outboundLinkTypes = Helper.ArrayOfOutboundLinkTypeFromXml(serviceProvider, reader, false);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return registrationArtifactType;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("RegistrationArtifactType instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  OutboundLinkTypes: " + Helper.ArrayToString<OutboundLinkType>(this.m_outboundLinkTypes));
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.m_name);
      Helper.ToXml(writer, "OutboundLinkTypes", this.m_outboundLinkTypes, false, false);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, RegistrationArtifactType obj) => obj.ToXml(writer, element);

    internal static ArtifactType[] Convert(RegistrationArtifactType[] artifactTypes)
    {
      if (artifactTypes == null || artifactTypes.Length == 0)
        return Array.Empty<ArtifactType>();
      ArtifactType[] artifactTypeArray = new ArtifactType[artifactTypes.Length];
      for (int index = 0; index < artifactTypes.Length; ++index)
      {
        if (artifactTypes[index] != null)
          artifactTypeArray[index] = artifactTypes[index].ToArtifactType();
      }
      return artifactTypeArray;
    }

    internal ArtifactType ToArtifactType() => new ArtifactType()
    {
      Name = this.Name,
      OutboundLinkTypes = OutboundLinkType.Convert(this.OutboundLinkTypes)
    };
  }
}
