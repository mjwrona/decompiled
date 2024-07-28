// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServiceTypeFilter
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class ServiceTypeFilter
  {
    private Guid m_identifier = Guid.Empty;
    private string m_serviceType;

    internal ServiceTypeFilter()
    {
    }

    public Guid Identifier
    {
      get => this.m_identifier;
      set => this.m_identifier = value;
    }

    public string ServiceType
    {
      get => this.m_serviceType;
      set => this.m_serviceType = value;
    }

    internal static ServiceTypeFilter FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ServiceTypeFilter serviceTypeFilter = new ServiceTypeFilter();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "Identifier":
              serviceTypeFilter.m_identifier = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            case "ServiceType":
              serviceTypeFilter.m_serviceType = XmlUtility.StringFromXmlAttribute(reader);
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
      return serviceTypeFilter;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServiceTypeFilter instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Identifier: " + this.m_identifier.ToString());
      stringBuilder.AppendLine("  ServiceType: " + this.m_serviceType);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_identifier != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "Identifier", this.m_identifier);
      if (this.m_serviceType != null)
        XmlUtility.ToXmlAttribute(writer, "ServiceType", this.m_serviceType);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, ServiceTypeFilter obj) => obj.ToXml(writer, element);
  }
}
