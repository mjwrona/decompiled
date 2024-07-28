// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.LocationMapping
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
  public sealed class LocationMapping
  {
    private string m_accessMappingMoniker;
    private string m_location;

    internal LocationMapping(string accessMappingMoniker, string location)
    {
      this.AccessMappingMoniker = accessMappingMoniker;
      this.m_location = location;
    }

    [Obsolete("This property is not used, use AccessMappingMoniker")]
    public AccessMapping AccessMapping => new AccessMapping(this.m_accessMappingMoniker, string.Empty, string.Empty);

    public string AccessMappingMoniker
    {
      get => this.m_accessMappingMoniker;
      internal set => this.m_accessMappingMoniker = value;
    }

    public string Location
    {
      get => this.m_location;
      internal set => this.m_location = value;
    }

    internal void PrepareForWebServiceSerialization()
    {
    }

    internal LocationMapping()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static LocationMapping FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      LocationMapping locationMapping = new LocationMapping();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "accessMappingMoniker":
              locationMapping.m_accessMappingMoniker = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "location":
              locationMapping.m_location = XmlUtility.StringFromXmlAttribute(reader);
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
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return locationMapping;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("LocationMapping instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  AccessMappingMoniker: " + this.m_accessMappingMoniker);
      stringBuilder.AppendLine("  Location: " + this.m_location);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      this.PrepareForWebServiceSerialization();
      writer.WriteStartElement(element);
      if (this.m_accessMappingMoniker != null)
        XmlUtility.ToXmlAttribute(writer, "accessMappingMoniker", this.m_accessMappingMoniker);
      if (this.m_location != null)
        XmlUtility.ToXmlAttribute(writer, "location", this.m_location);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, LocationMapping obj) => obj.ToXml(writer, element);
  }
}
