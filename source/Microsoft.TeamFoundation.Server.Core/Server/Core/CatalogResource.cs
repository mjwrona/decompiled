// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogResource
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class CatalogResource
  {
    private Guid m_tempCorrelationId;
    private List<string> m_nodeReferencePaths = new List<string>();
    private List<KeyValue<string, string>> m_propertyPairs = new List<KeyValue<string, string>>();
    private List<CatalogServiceReference> m_catalogServiceReferences = new List<CatalogServiceReference>();

    public CatalogResource()
      : this((CatalogResourceType) null, (string) null)
    {
    }

    public CatalogResource(CatalogResourceType resourceType, string displayName)
    {
      this.ResourceType = resourceType;
      this.DisplayName = displayName;
      this.TempCorrelationId = Guid.NewGuid();
      this.PropertyId = -1;
      this.NodeReferences = new List<CatalogNode>();
      this.Properties = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.PropertyName);
      this.ServiceReferences = (IDictionary<string, ServiceDefinition>) new Dictionary<string, ServiceDefinition>((IEqualityComparer<string>) VssStringComparer.CatalogServiceReference);
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public Guid Identifier { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string DisplayName { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public string Description { get; set; }

    [XmlIgnore]
    public CatalogResourceType ResourceType { get; set; }

    [XmlIgnore]
    public IDictionary<string, string> Properties { get; private set; }

    [XmlIgnore]
    public IDictionary<string, ServiceDefinition> ServiceReferences { get; private set; }

    [XmlIgnore]
    public List<CatalogNode> NodeReferences { get; internal set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public Guid ResourceTypeIdentifier { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public List<CatalogServiceReference> CatalogServiceReferences => this.m_catalogServiceReferences;

    [XmlElement("Properties", typeof (List<KeyValue<string, string>>))]
    [ClientProperty(ClientVisibility.Private)]
    public List<KeyValue<string, string>> PropertyPairs => this.m_propertyPairs;

    [ClientProperty(ClientVisibility.Private)]
    public List<string> NodeReferencePaths => this.m_nodeReferencePaths;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public Guid TempCorrelationId
    {
      get => this.Identifier == Guid.Empty ? this.m_tempCorrelationId : this.Identifier;
      set => this.m_tempCorrelationId = value;
    }

    [XmlIgnore]
    public CatalogChangeType ChangeType
    {
      get => (CatalogChangeType) this.ChangeTypeValue;
      set => this.ChangeTypeValue = (int) value;
    }

    [XmlAttribute("ctype")]
    [ClientProperty(ClientVisibility.Private)]
    public int ChangeTypeValue { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public bool MatchedQuery { get; set; }

    internal int PropertyId { get; set; }

    internal void UpdateSelf(CatalogResource updatedResource)
    {
      this.Description = updatedResource.Description;
      this.DisplayName = updatedResource.DisplayName;
      this.Identifier = updatedResource.Identifier;
      this.m_nodeReferencePaths = updatedResource.NodeReferencePaths;
      this.NodeReferences = updatedResource.NodeReferences;
      this.ResourceType = updatedResource.ResourceType;
      this.ResourceTypeIdentifier = updatedResource.ResourceTypeIdentifier;
      this.ServiceReferences = updatedResource.ServiceReferences;
      this.m_catalogServiceReferences = updatedResource.CatalogServiceReferences;
      this.Properties = updatedResource.Properties;
    }

    internal void PrepareForWebServiceSerialization(bool matchedQuery)
    {
      this.MatchedQuery = matchedQuery;
      foreach (KeyValuePair<string, ServiceDefinition> serviceReference in (IEnumerable<KeyValuePair<string, ServiceDefinition>>) this.ServiceReferences)
        this.CatalogServiceReferences.Add(new CatalogServiceReference()
        {
          ResourceIdentifier = this.Identifier,
          AssociationKey = serviceReference.Key,
          ServiceDefinition = serviceReference.Value
        });
      foreach (CatalogNode nodeReference in this.NodeReferences)
        this.NodeReferencePaths.Add(nodeReference.FullPath);
      this.m_propertyPairs = KeyValue<string, string>.Convert((IEnumerable<KeyValuePair<string, string>>) this.Properties);
    }

    internal void InitializeFromWebService(
      IVssRequestContext requestContext,
      ITeamFoundationCatalogService catalogService)
    {
      this.ResourceType = catalogService.QueryResourceType(requestContext, this.ResourceTypeIdentifier);
      foreach (CatalogServiceReference serviceReference in this.CatalogServiceReferences)
      {
        ServiceDefinition serviceDefinition = serviceReference.ServiceDefinition;
        this.ServiceReferences[serviceReference.AssociationKey] = serviceDefinition;
      }
      foreach (KeyValue<string, string> propertyPair in this.PropertyPairs)
        this.Properties[propertyPair.Key] = propertyPair.Value;
    }

    internal static void Validate(CatalogResource catalogResource)
    {
      ArgumentUtility.CheckForNull<CatalogResourceType>(catalogResource.ResourceType, "CatalogResource.ResourceType");
      ArgumentUtility.CheckForEmptyGuid(catalogResource.ResourceType.Identifier, "CatalogResource.ResourceType.Identifier");
      ArgumentUtility.CheckStringForNullOrEmpty(catalogResource.DisplayName, "CatalogResource.DisplayName");
      foreach (string key in (IEnumerable<string>) catalogResource.Properties.Keys)
        PropertyValidation.CheckPropertyLength(key, false, 0, 256, "ResourceProperty.Key", typeof (CatalogResource), "ResourceProperty.Key");
      foreach (string key in (IEnumerable<string>) catalogResource.ServiceReferences.Keys)
        PropertyValidation.CheckPropertyLength(key, false, 0, 256, "ServiceReference.Key", typeof (CatalogResource), "ServiceReference.Key");
    }
  }
}
