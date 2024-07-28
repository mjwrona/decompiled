// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Artifact
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Common
{
  public class Artifact
  {
    private string m_Uri;
    private string m_ArtifactTitle;
    private string m_ExternalId;
    private ExtendedAttribute[] m_ExtendedAttributes;
    private OutboundLink[] m_OutboundLinks;

    public string Uri
    {
      get => this.m_Uri;
      set => this.m_Uri = value;
    }

    public string ArtifactTitle
    {
      get => this.m_ArtifactTitle;
      set => this.m_ArtifactTitle = value;
    }

    public string ExternalId
    {
      get => this.m_ExternalId;
      set => this.m_ExternalId = value;
    }

    public ExtendedAttribute[] ExtendedAttributes
    {
      get => this.m_ExtendedAttributes;
      set => this.m_ExtendedAttributes = value;
    }

    public OutboundLink[] OutboundLinks
    {
      get => this.m_OutboundLinks;
      set => this.m_OutboundLinks = value;
    }

    internal static Artifact FromXml(XmlReader reader)
    {
      Artifact artifact = new Artifact();
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
            case "ArtifactTitle":
              artifact.m_ArtifactTitle = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "ExtendedAttributes":
              artifact.m_ExtendedAttributes = ExtendedAttribute.ExtendedAttributeArrayFromXml(reader);
              continue;
            case "ExternalId":
              artifact.m_ExternalId = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "OutboundLinks":
              artifact.m_OutboundLinks = OutboundLink.OutboundLinkArrayFromXml(reader);
              continue;
            case "Uri":
              artifact.m_Uri = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return artifact;
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_ArtifactTitle != null)
        XmlUtility.ToXmlElement(writer, "ArtifactTitle", this.m_ArtifactTitle);
      ExtendedAttribute.ExtendedAttributeArrayToXml(writer, "ExtendedAttributes", this.m_ExtendedAttributes);
      if (this.m_ExternalId != null)
        XmlUtility.ToXmlElement(writer, "ExternalId", this.m_ExternalId);
      OutboundLink.OutboundLinkArrayToXml(writer, "OutboundLinks", this.m_OutboundLinks);
      if (this.m_Uri != null)
        XmlUtility.ToXmlElement(writer, "Uri", this.m_Uri);
      writer.WriteEndElement();
    }

    public static Artifact[] ArtifactArrayFromXml(XmlReader reader)
    {
      List<Artifact> artifactList = new List<Artifact>();
      int num = reader.IsEmptyElement ? 1 : 0;
      reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.HasAttributes && reader.GetAttribute("xsi:nil") == "true")
          {
            artifactList.Add((Artifact) null);
            reader.Read();
          }
          else
            artifactList.Add(Artifact.FromXml(reader));
        }
        reader.ReadEndElement();
      }
      return artifactList.ToArray();
    }

    internal static void ArtifactArrayToXml(XmlWriter writer, string element, Artifact[] array)
    {
      if (array == null || array.Length == 0)
        return;
      writer.WriteStartElement(element);
      for (int index = 0; index < array.Length; ++index)
      {
        if (array[index] == null)
          throw new ArgumentNullException("array[" + index.ToString() + "]");
        array[index].ToXml(writer, nameof (Artifact));
      }
      writer.WriteEndElement();
    }
  }
}
