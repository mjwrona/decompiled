// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.CatalogResource
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class CatalogResource
  {
    internal CatalogServiceReference[] m_catalogServiceReferences = Helper.ZeroLengthArrayOfCatalogServiceReference;
    private int m_changeTypeValue;
    private string m_description;
    private string m_displayName;
    private Guid m_identifier = Guid.Empty;
    private bool m_matchedQuery;
    internal string[] m_nodeReferencePaths = Helper.ZeroLengthArrayOfString;
    internal KeyValueOfStringString[] m_propertyPairs = Helper.ZeroLengthArrayOfKeyValueOfStringString;
    private Guid m_resourceTypeIdentifier = Guid.Empty;
    private Guid m_tempCorrelationId = Guid.Empty;

    internal CatalogResource(string displayName, CatalogResourceType resourceType)
    {
      this.DisplayName = displayName;
      this.ResourceType = resourceType;
      this.NodeReferences = new List<CatalogNode>().AsReadOnly();
      this.Properties = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.PropertyName);
      this.ServiceReferences = (IDictionary<string, ServiceDefinition>) new Dictionary<string, ServiceDefinition>((IEqualityComparer<string>) VssStringComparer.CatalogServiceReference);
      this.TempCorrelationId = Guid.NewGuid();
    }

    public Guid Identifier => this.m_identifier;

    public string DisplayName
    {
      get => this.m_displayName;
      set => this.m_displayName = value;
    }

    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    public CatalogResourceType ResourceType { get; internal set; }

    public IDictionary<string, string> Properties { get; internal set; }

    public IDictionary<string, ServiceDefinition> ServiceReferences { get; internal set; }

    public ReadOnlyCollection<CatalogNode> NodeReferences { get; internal set; }

    internal Guid TempCorrelationId
    {
      get => this.Identifier == Guid.Empty ? this.m_tempCorrelationId : this.Identifier;
      set => this.m_tempCorrelationId = value;
    }

    internal CatalogChangeType ChangeType
    {
      get => (CatalogChangeType) this.m_changeTypeValue;
      set => this.m_changeTypeValue = (int) value;
    }

    internal bool MatchedQuery => this.m_matchedQuery;

    internal void UpdateSelf(CatalogResource updatedResource)
    {
      this.Description = updatedResource.Description;
      this.DisplayName = updatedResource.DisplayName;
      this.m_identifier = updatedResource.Identifier;
      this.m_nodeReferencePaths = updatedResource.m_nodeReferencePaths;
      this.NodeReferences = updatedResource.NodeReferences;
      this.Properties = updatedResource.Properties;
      this.ResourceType = updatedResource.ResourceType;
      this.m_resourceTypeIdentifier = updatedResource.m_resourceTypeIdentifier;
      this.m_catalogServiceReferences = updatedResource.m_catalogServiceReferences;
      this.Properties = updatedResource.Properties;
      this.m_propertyPairs = updatedResource.m_propertyPairs;
      foreach (KeyValuePair<string, ServiceDefinition> serviceReference in (IEnumerable<KeyValuePair<string, ServiceDefinition>>) updatedResource.ServiceReferences)
      {
        ServiceDefinition serviceDefinition;
        if (this.ServiceReferences.TryGetValue(serviceReference.Key, out serviceDefinition))
          serviceDefinition.Identifier = serviceReference.Value.Identifier;
      }
      this.ServiceReferences = updatedResource.ServiceReferences;
    }

    internal void InitializeFromWebService(
      Dictionary<Guid, CatalogResourceType> resourceTypes,
      Dictionary<string, CatalogNode> nodes,
      ILocationService locationService)
    {
      this.ResourceType = resourceTypes[this.m_resourceTypeIdentifier];
      List<CatalogNode> catalogNodeList = new List<CatalogNode>();
      foreach (string nodeReferencePath in this.m_nodeReferencePaths)
      {
        CatalogNode node = nodes[nodeReferencePath];
        catalogNodeList.Add(node);
        node.Resource = this;
      }
      this.NodeReferences = catalogNodeList.AsReadOnly();
      this.ServiceReferences = (IDictionary<string, ServiceDefinition>) new Dictionary<string, ServiceDefinition>((IEqualityComparer<string>) VssStringComparer.CatalogServiceReference);
      foreach (CatalogServiceReference serviceReference in this.m_catalogServiceReferences)
      {
        ServiceDefinition serviceDefinition = (ServiceDefinition) null;
        if (serviceReference.ServiceDefinition != null)
          serviceDefinition = locationService.FindServiceDefinition(serviceReference.ServiceDefinition.ServiceType, serviceReference.ServiceDefinition.Identifier);
        this.ServiceReferences[serviceReference.AssociationKey] = serviceDefinition;
      }
      this.Properties = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.PropertyName);
      foreach (KeyValueOfStringString propertyPair in this.m_propertyPairs)
        this.Properties[propertyPair.Key] = propertyPair.Value;
    }

    internal static CatalogResource PrepareForWebServiceSerialization(CatalogResource resource)
    {
      resource.m_resourceTypeIdentifier = resource.ResourceType.Identifier;
      List<CatalogServiceReference> serviceReferenceList = new List<CatalogServiceReference>();
      if (resource.ServiceReferences != null)
      {
        foreach (KeyValuePair<string, ServiceDefinition> serviceReference in (IEnumerable<KeyValuePair<string, ServiceDefinition>>) resource.ServiceReferences)
          serviceReferenceList.Add(new CatalogServiceReference()
          {
            AssociationKey = serviceReference.Key,
            ResourceIdentifier = resource.TempCorrelationId,
            ServiceDefinition = serviceReference.Value
          });
      }
      resource.m_catalogServiceReferences = serviceReferenceList.ToArray();
      List<KeyValueOfStringString> valueOfStringStringList = new List<KeyValueOfStringString>();
      foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) resource.Properties)
        valueOfStringStringList.Add(new KeyValueOfStringString(property));
      resource.m_propertyPairs = valueOfStringStringList.ToArray();
      resource.m_nodeReferencePaths = (string[]) null;
      return resource;
    }

    internal CatalogResource()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static CatalogResource FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      CatalogResource catalogResource = new CatalogResource();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "ctype":
              catalogResource.m_changeTypeValue = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "DisplayName":
              catalogResource.m_displayName = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "Identifier":
              catalogResource.m_identifier = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            case "MatchedQuery":
              catalogResource.m_matchedQuery = XmlUtility.BooleanFromXmlAttribute(reader);
              continue;
            case "ResourceTypeIdentifier":
              catalogResource.m_resourceTypeIdentifier = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            case "TempCorrelationId":
              catalogResource.m_tempCorrelationId = XmlUtility.GuidFromXmlAttribute(reader);
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
          switch (reader.Name)
          {
            case "CatalogServiceReferences":
              catalogResource.m_catalogServiceReferences = Helper.ArrayOfCatalogServiceReferenceFromXml(serviceProvider, reader, false);
              continue;
            case "Description":
              catalogResource.m_description = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "NodeReferencePaths":
              catalogResource.m_nodeReferencePaths = Helper.ArrayOfStringFromXml(reader, false);
              continue;
            case "Properties":
              catalogResource.m_propertyPairs = Helper.ArrayOfKeyValueOfStringStringFromXml(serviceProvider, reader, false);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return catalogResource;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("CatalogResource instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  CatalogServiceReferences: " + Helper.ArrayToString<CatalogServiceReference>(this.m_catalogServiceReferences));
      stringBuilder.AppendLine("  ChangeTypeValue: " + this.m_changeTypeValue.ToString());
      stringBuilder.AppendLine("  Description: " + this.m_description);
      stringBuilder.AppendLine("  DisplayName: " + this.m_displayName);
      stringBuilder.AppendLine("  Identifier: " + this.m_identifier.ToString());
      stringBuilder.AppendLine("  MatchedQuery: " + this.m_matchedQuery.ToString());
      stringBuilder.AppendLine("  NodeReferencePaths: " + Helper.ArrayToString<string>(this.m_nodeReferencePaths));
      stringBuilder.AppendLine("  PropertyPairs: " + Helper.ArrayToString<KeyValueOfStringString>(this.m_propertyPairs));
      stringBuilder.AppendLine("  ResourceTypeIdentifier: " + this.m_resourceTypeIdentifier.ToString());
      stringBuilder.AppendLine("  TempCorrelationId: " + this.m_tempCorrelationId.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_changeTypeValue != 0)
        XmlUtility.ToXmlAttribute(writer, "ctype", this.m_changeTypeValue);
      if (this.m_displayName != null)
        XmlUtility.ToXmlAttribute(writer, "DisplayName", this.m_displayName);
      if (this.m_identifier != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "Identifier", this.m_identifier);
      if (this.m_matchedQuery)
        XmlUtility.ToXmlAttribute(writer, "MatchedQuery", this.m_matchedQuery);
      if (this.m_resourceTypeIdentifier != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "ResourceTypeIdentifier", this.m_resourceTypeIdentifier);
      if (this.m_tempCorrelationId != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "TempCorrelationId", this.m_tempCorrelationId);
      Helper.ToXml(writer, "CatalogServiceReferences", this.m_catalogServiceReferences, false, false);
      if (this.m_description != null)
        XmlUtility.ToXmlElement(writer, "Description", this.m_description);
      Helper.ToXml(writer, "NodeReferencePaths", this.m_nodeReferencePaths, false, false);
      Helper.ToXml(writer, "Properties", this.m_propertyPairs, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, CatalogResource obj) => obj.ToXml(writer, element);
  }
}
