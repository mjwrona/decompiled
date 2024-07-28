// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.CatalogServiceReference
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal sealed class CatalogServiceReference
  {
    private string m_associationKey;
    private Guid m_resourceIdentifier = Guid.Empty;
    private ServiceDefinition m_serviceDefinition;

    internal CatalogServiceReference()
    {
    }

    public string AssociationKey
    {
      get => this.m_associationKey;
      set => this.m_associationKey = value;
    }

    public Guid ResourceIdentifier
    {
      get => this.m_resourceIdentifier;
      set => this.m_resourceIdentifier = value;
    }

    public ServiceDefinition ServiceDefinition
    {
      get => this.m_serviceDefinition;
      set => this.m_serviceDefinition = value;
    }

    internal static CatalogServiceReference FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      CatalogServiceReference serviceReference = new CatalogServiceReference();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "AssociationKey":
              serviceReference.m_associationKey = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "ResourceIdentifier":
              serviceReference.m_resourceIdentifier = XmlUtility.GuidFromXmlAttribute(reader);
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
          if (reader.Name == "ServiceDefinition")
            serviceReference.m_serviceDefinition = ServiceDefinition.FromXml(serviceProvider, reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return serviceReference;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("CatalogServiceReference instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  AssociationKey: " + this.m_associationKey);
      stringBuilder.AppendLine("  ResourceIdentifier: " + this.m_resourceIdentifier.ToString());
      stringBuilder.AppendLine("  ServiceDefinition: " + this.m_serviceDefinition?.ToString());
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_associationKey != null)
        XmlUtility.ToXmlAttribute(writer, "AssociationKey", this.m_associationKey);
      if (this.m_resourceIdentifier != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "ResourceIdentifier", this.m_resourceIdentifier);
      if (this.m_serviceDefinition != null)
        ServiceDefinition.ToXml(writer, "ServiceDefinition", this.m_serviceDefinition);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, CatalogServiceReference obj) => obj.ToXml(writer, element);
  }
}
