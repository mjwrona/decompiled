// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.LocationXmlOperator
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class LocationXmlOperator
  {
    private Dictionary<string, string> m_accessMappingLocationServiceUrls;
    private bool m_isClientCache;
    private static readonly string s_lastChangeId = "LastChangeId";
    private static readonly string s_cacheExpirationDate = "CacheExpirationDate";
    private static readonly string s_defaultAccessMappingMoniker = "DefaultAccessMappingMoniker";
    private static readonly string s_virtualDirectory = "VirtualDirectory";
    private static readonly string s_services = "Services";
    private static readonly string s_cachedMisses = "CachedMisses";
    private static readonly string s_serviceDefinition = "ServiceDefinition";
    private static readonly string s_cachedMiss = "CachedMiss";
    private static readonly string s_serviceType = "ServiceType";
    private static readonly string s_identifier = "Identifier";
    private static readonly string s_displayName = "DisplayName";
    private static readonly string s_locationServiceUrl = "LocationServiceUrl";
    private static readonly string s_description = "Description";
    private static readonly string s_toolId = "ToolId";
    private static readonly string s_relativePath = "RelativePath";
    private static readonly string s_relativeTo = "relativeTo";
    private static readonly string s_locationMappings = "LocationMappings";
    private static readonly string s_locationMapping = "LocationMapping";
    private static readonly string s_location = "Location";
    private static readonly string s_accessMappings = "AccessMappings";
    private static readonly string s_accessMapping = "AccessMapping";
    private static readonly string s_moniker = "Moniker";
    private static readonly string s_accessPoint = "AccessPoint";

    public LocationXmlOperator(bool isClientCache)
    {
      this.m_isClientCache = isClientCache;
      this.m_accessMappingLocationServiceUrls = new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.AccessMappingMoniker);
    }

    public List<ServiceDefinition> ReadServices(
      XmlDocument document,
      Dictionary<string, AccessMapping> accessMappings)
    {
      List<ServiceDefinition> serviceDefinitionList = new List<ServiceDefinition>();
      XmlNodeList xmlNodeList = document.SelectNodes("//" + LocationXmlOperator.s_services);
      if (xmlNodeList == null)
        return serviceDefinitionList;
      foreach (XmlNode xmlNode1 in xmlNodeList)
      {
        foreach (XmlNode selectNode1 in xmlNode1.SelectNodes("./" + LocationXmlOperator.s_serviceDefinition))
        {
          ServiceDefinition serviceDefinition = new ServiceDefinition();
          XmlNode node1 = selectNode1.SelectSingleNode("./" + LocationXmlOperator.s_serviceType);
          LocationXmlOperator.CheckXmlNodeNullOrEmpty(node1, LocationXmlOperator.s_serviceType, selectNode1);
          serviceDefinition.ServiceType = node1.InnerText;
          XmlNode node2 = selectNode1.SelectSingleNode("./" + LocationXmlOperator.s_identifier);
          if (this.m_isClientCache)
            LocationXmlOperator.CheckXmlNodeNullOrEmpty(node2, LocationXmlOperator.s_identifier, selectNode1);
          serviceDefinition.Identifier = node2 != null ? XmlConvert.ToGuid(node2.InnerText) : Guid.Empty;
          XmlNode node3 = selectNode1.SelectSingleNode("./" + LocationXmlOperator.s_displayName);
          LocationXmlOperator.CheckXmlNodeNullOrEmpty(node3, LocationXmlOperator.s_displayName, selectNode1);
          serviceDefinition.DisplayName = node3.InnerText;
          XmlNode xmlNode2 = selectNode1.SelectSingleNode("./" + LocationXmlOperator.s_description);
          serviceDefinition.Description = xmlNode2 != null ? xmlNode2.InnerText : string.Empty;
          XmlNode xmlNode3 = selectNode1.SelectSingleNode("./" + LocationXmlOperator.s_toolId);
          serviceDefinition.ToolType = xmlNode3?.InnerText;
          XmlNode xmlNode4 = selectNode1.SelectSingleNode("./" + LocationXmlOperator.s_relativePath);
          LocationXmlOperator.CheckXmlNodeNull(xmlNode4, LocationXmlOperator.s_relativePath, selectNode1);
          serviceDefinition.RelativePath = xmlNode4.InnerText;
          XmlAttribute attribute = xmlNode4.Attributes[LocationXmlOperator.s_relativeTo];
          LocationXmlOperator.CheckXmlAttributeNullOrEmpty(attribute, LocationXmlOperator.s_relativeTo, xmlNode4);
          RelativeToSetting relativeToSetting;
          if (!RelativeToEnumCache.GetRelativeToEnums().TryGetValue(attribute.InnerText, out relativeToSetting))
            throw new Microsoft.TeamFoundation.Framework.Common.ConfigFileException(attribute.InnerText);
          serviceDefinition.RelativeToSetting = relativeToSetting;
          if (serviceDefinition.RelativeToSetting == RelativeToSetting.FullyQualified && serviceDefinition.RelativePath == string.Empty)
            serviceDefinition.RelativePath = (string) null;
          serviceDefinition.LocationMappings = (IEnumerable<LocationMapping>) new List<LocationMapping>();
          if (serviceDefinition.RelativeToSetting == RelativeToSetting.FullyQualified)
          {
            foreach (XmlNode selectNode2 in selectNode1.SelectNodes(".//" + LocationXmlOperator.s_locationMapping))
            {
              LocationMapping locationMapping = new LocationMapping();
              XmlNode node4 = selectNode2.SelectSingleNode("./" + LocationXmlOperator.s_accessMapping);
              LocationXmlOperator.CheckXmlNodeNullOrEmpty(node4, LocationXmlOperator.s_accessMapping, selectNode2);
              AccessMapping accessMapping;
              if (!accessMappings.TryGetValue(node4.InnerText, out accessMapping))
                throw new Microsoft.TeamFoundation.Framework.Common.ConfigFileException(TFCommonResources.AccessMappingNotRegistered((object) node4.InnerText));
              if (accessMapping != null)
                locationMapping.AccessMappingMoniker = accessMapping.Moniker;
              XmlNode node5 = selectNode2.SelectSingleNode("./" + LocationXmlOperator.s_location);
              if (this.m_isClientCache)
                LocationXmlOperator.CheckXmlNodeNullOrEmpty(node5, LocationXmlOperator.s_location, selectNode2);
              locationMapping.Location = node5?.InnerText;
              serviceDefinition.Mappings.Add(locationMapping);
            }
          }
          serviceDefinitionList.Add(serviceDefinition);
        }
      }
      return serviceDefinitionList;
    }

    public List<string> ReadCachedMisses(XmlDocument document)
    {
      List<string> stringList = new List<string>();
      XmlNodeList xmlNodeList = document.SelectNodes("//" + LocationXmlOperator.s_cachedMisses);
      if (xmlNodeList == null)
        return stringList;
      foreach (XmlNode xmlNode in xmlNodeList)
      {
        foreach (XmlNode selectNode in xmlNode.SelectNodes("./" + LocationXmlOperator.s_cachedMiss))
          stringList.Add(selectNode.InnerText);
      }
      return stringList;
    }

    public List<AccessMapping> ReadAccessMappings(XmlDocument document)
    {
      List<AccessMapping> accessMappingList = new List<AccessMapping>();
      XmlNodeList xmlNodeList = document.SelectNodes("//" + LocationXmlOperator.s_accessMappings);
      if (xmlNodeList == null)
        return accessMappingList;
      foreach (XmlNode xmlNode1 in xmlNodeList)
      {
        foreach (XmlNode selectNode in xmlNode1.SelectNodes("./" + LocationXmlOperator.s_accessMapping))
        {
          AccessMapping accessMapping = new AccessMapping();
          XmlNode node1 = selectNode.SelectSingleNode("./" + LocationXmlOperator.s_moniker);
          LocationXmlOperator.CheckXmlNodeNullOrEmpty(node1, LocationXmlOperator.s_moniker, selectNode);
          accessMapping.Moniker = node1.InnerText;
          XmlNode node2 = selectNode.SelectSingleNode("./" + LocationXmlOperator.s_accessPoint);
          LocationXmlOperator.CheckXmlNodeNullOrEmpty(node2, LocationXmlOperator.s_accessPoint, selectNode);
          accessMapping.AccessPoint = node2.InnerText;
          XmlNode xmlNode2 = selectNode.SelectSingleNode("./" + LocationXmlOperator.s_displayName);
          accessMapping.DisplayName = xmlNode2?.InnerText;
          XmlNode xmlNode3 = selectNode.SelectSingleNode("./" + LocationXmlOperator.s_virtualDirectory);
          accessMapping.VirtualDirectory = xmlNode3?.InnerText;
          if (!this.m_isClientCache)
          {
            XmlNode xmlNode4 = selectNode.SelectSingleNode("./" + LocationXmlOperator.s_locationServiceUrl);
            string str = xmlNode4 != null ? xmlNode4.InnerText : string.Empty;
            this.m_accessMappingLocationServiceUrls[accessMapping.Moniker] = str;
          }
          accessMappingList.Add(accessMapping);
        }
      }
      return accessMappingList;
    }

    public int ReadLastChangeId(XmlDocument document)
    {
      XmlNode xmlNode = document.SelectSingleNode("//" + LocationXmlOperator.s_lastChangeId);
      return xmlNode == null ? -1 : XmlConvert.ToInt32(xmlNode.InnerText);
    }

    public DateTime ReadCacheExpirationDate(XmlDocument document)
    {
      XmlNode xmlNode = document.SelectSingleNode("//" + LocationXmlOperator.s_cacheExpirationDate);
      return xmlNode == null ? DateTime.MinValue : XmlConvert.ToDateTime(xmlNode.InnerText, XmlDateTimeSerializationMode.Utc);
    }

    public string ReadDefaultAccessMappingMoniker(XmlDocument document)
    {
      XmlNode node = document.SelectSingleNode("//" + LocationXmlOperator.s_defaultAccessMappingMoniker);
      LocationXmlOperator.CheckXmlNodeNullOrEmpty(node, LocationXmlOperator.s_defaultAccessMappingMoniker, (XmlNode) document);
      return node.InnerText;
    }

    public string ReadVirtualDirectory(XmlDocument document)
    {
      XmlNode node = document.SelectSingleNode("//" + LocationXmlOperator.s_virtualDirectory);
      LocationXmlOperator.CheckXmlNodeNull(node, LocationXmlOperator.s_virtualDirectory, (XmlNode) document);
      return node.InnerText;
    }

    public void WriteLastChangeId(XmlNode documentNode, int lastChangeId)
    {
      XmlNode node = documentNode.OwnerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_lastChangeId, (string) null);
      documentNode.AppendChild(node);
      node.InnerText = XmlConvert.ToString(lastChangeId);
    }

    public void WriteCacheExpirationDate(XmlNode documentNode, DateTime cacheExpirationDate)
    {
      XmlNode node = documentNode.OwnerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_cacheExpirationDate, (string) null);
      documentNode.AppendChild(node);
      node.InnerText = XmlConvert.ToString(cacheExpirationDate, XmlDateTimeSerializationMode.Utc);
    }

    public void WriteDefaultAccessMappingMoniker(
      XmlNode documentNode,
      string defaultAccessMappingMoniker)
    {
      XmlNode node = documentNode.OwnerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_defaultAccessMappingMoniker, (string) null);
      documentNode.AppendChild(node);
      node.InnerText = defaultAccessMappingMoniker;
    }

    public void WriteVirtualDirectory(XmlNode documentNode, string virtualDirectory)
    {
      XmlNode node = documentNode.OwnerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_virtualDirectory, (string) null);
      documentNode.AppendChild(node);
      node.InnerText = virtualDirectory;
    }

    public void WriteAccessMappings(XmlNode documentNode, IEnumerable<AccessMapping> accessMappings)
    {
      XmlDocument ownerDocument = documentNode.OwnerDocument;
      XmlNode node1 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_accessMappings, (string) null);
      documentNode.AppendChild(node1);
      foreach (AccessMapping accessMapping in accessMappings)
      {
        XmlNode node2 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_accessMapping, (string) null);
        node1.AppendChild(node2);
        XmlNode node3 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_moniker, (string) null);
        node2.AppendChild(node3);
        node3.InnerText = accessMapping.Moniker;
        XmlNode node4 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_accessPoint, (string) null);
        node2.AppendChild(node4);
        node4.InnerText = accessMapping.AccessPoint;
        XmlNode node5 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_displayName, (string) null);
        node2.AppendChild(node5);
        node5.InnerText = accessMapping.DisplayName;
        if (accessMapping.VirtualDirectory != null)
        {
          XmlNode node6 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_virtualDirectory, (string) null);
          node2.AppendChild(node6);
          node6.InnerText = accessMapping.VirtualDirectory;
        }
      }
    }

    public void WriteServices(
      XmlNode documentNode,
      IEnumerable<ServiceDefinition> serviceDefintions)
    {
      XmlDocument ownerDocument = documentNode.OwnerDocument;
      XmlNode node1 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_services, (string) null);
      documentNode.AppendChild(node1);
      foreach (ServiceDefinition serviceDefintion in serviceDefintions)
      {
        XmlNode node2 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_serviceDefinition, (string) null);
        node1.AppendChild(node2);
        XmlNode node3 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_serviceType, (string) null);
        node2.AppendChild(node3);
        node3.InnerText = serviceDefintion.ServiceType;
        XmlNode node4 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_identifier, (string) null);
        node2.AppendChild(node4);
        node4.InnerText = XmlConvert.ToString(serviceDefintion.Identifier);
        if (serviceDefintion.DisplayName != null)
        {
          XmlNode node5 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_displayName, (string) null);
          node2.AppendChild(node5);
          node5.InnerText = serviceDefintion.DisplayName;
        }
        if (serviceDefintion.Description != null)
        {
          XmlNode node6 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_description, (string) null);
          node2.AppendChild(node6);
          node6.InnerText = serviceDefintion.Description;
        }
        if (serviceDefintion.ToolType != null)
        {
          XmlNode node7 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_toolId, (string) null);
          node2.AppendChild(node7);
          node7.InnerText = serviceDefintion.ToolType;
        }
        XmlNode node8 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_relativePath, (string) null);
        node2.AppendChild(node8);
        node8.InnerText = serviceDefintion.RelativePath;
        TFCommonUtil.AddXmlAttribute(node8, LocationXmlOperator.s_relativeTo, serviceDefintion.RelativeToSetting.ToString());
        if (serviceDefintion.RelativeToSetting == RelativeToSetting.FullyQualified)
        {
          XmlNode node9 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_locationMappings, (string) null);
          node2.AppendChild(node9);
          foreach (LocationMapping locationMapping in serviceDefintion.LocationMappings)
          {
            XmlNode node10 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_locationMapping, (string) null);
            node9.AppendChild(node10);
            XmlNode node11 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_accessMapping, (string) null);
            node10.AppendChild(node11);
            node11.InnerText = locationMapping.AccessMappingMoniker;
            XmlNode node12 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_location, (string) null);
            node10.AppendChild(node12);
            node12.InnerText = locationMapping.Location;
          }
        }
      }
    }

    public void WriteCachedMisses(XmlNode documentNode, IEnumerable<string> cachedMisses)
    {
      XmlDocument ownerDocument = documentNode.OwnerDocument;
      XmlNode node1 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_cachedMisses, (string) null);
      documentNode.AppendChild(node1);
      foreach (string cachedMiss in cachedMisses)
      {
        XmlNode node2 = ownerDocument.CreateNode(XmlNodeType.Element, LocationXmlOperator.s_cachedMiss, (string) null);
        node2.InnerText = cachedMiss;
        node1.AppendChild(node2);
      }
    }

    public string GetLocationServiceUrl(string moniker) => this.m_accessMappingLocationServiceUrls[moniker];

    private static void CheckXmlNodeNull(XmlNode node, string nodeName, XmlNode parent)
    {
      if (node == null)
        throw new Microsoft.TeamFoundation.Framework.Common.ConfigFileException(TFCommonResources.XmlNodeMissing((object) nodeName, (object) parent));
    }

    private static void CheckXmlNodeNullOrEmpty(XmlNode node, string nodeName, XmlNode parent)
    {
      LocationXmlOperator.CheckXmlNodeNull(node, nodeName, parent);
      if (node.InnerText.Equals(string.Empty))
        throw new Microsoft.TeamFoundation.Framework.Common.ConfigFileException(TFCommonResources.XmlNodeEmpty((object) nodeName, (object) parent.Name));
    }

    private static void CheckXmlAttributeNullOrEmpty(
      XmlAttribute attribute,
      string attributeName,
      XmlNode element)
    {
      if (attribute == null)
        throw new Microsoft.TeamFoundation.Framework.Common.ConfigFileException(TFCommonResources.XmlAttributeNull((object) attributeName, (object) element.Name));
      if (attribute.InnerText.Equals(string.Empty))
        throw new Microsoft.TeamFoundation.Framework.Common.ConfigFileException(TFCommonResources.XmlAttributeEmpty((object) attributeName, (object) element.Name));
    }
  }
}
