// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.CatalogResourceType
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class CatalogResourceType
  {
    private string m_description;
    private string m_displayName;
    private Guid m_identifier = Guid.Empty;

    public Guid Identifier => this.m_identifier;

    public string DisplayName => this.m_displayName;

    public string Description => this.m_description;

    internal CatalogResourceType()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static CatalogResourceType FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      CatalogResourceType catalogResourceType = new CatalogResourceType();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "DisplayName":
              catalogResourceType.m_displayName = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "Identifier":
              catalogResourceType.m_identifier = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            default:
              continue;
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "Description")
            catalogResourceType.m_description = XmlUtility.StringFromXmlElement(reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return catalogResourceType;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("CatalogResourceType instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Description: " + this.m_description);
      stringBuilder.AppendLine("  DisplayName: " + this.m_displayName);
      stringBuilder.AppendLine("  Identifier: " + this.m_identifier.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_displayName != null)
        XmlUtility.ToXmlAttribute(writer, "DisplayName", this.m_displayName);
      if (this.m_identifier != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "Identifier", this.m_identifier);
      if (this.m_description != null)
        XmlUtility.ToXmlElement(writer, "Description", this.m_description);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, CatalogResourceType obj) => obj.ToXml(writer, element);
  }
}
