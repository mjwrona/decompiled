// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServiceDefinition
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ServiceDefinition
  {
    private List<LocationMapping> m_mappings;
    private static ServiceDefinition.DefinitionEqualityComparer s_equalityComparer = new ServiceDefinition.DefinitionEqualityComparer();
    private string m_description;
    private string m_displayName;
    private Guid m_identifier = Guid.Empty;
    private bool m_isSingleton;
    internal LocationMapping[] m_locationMappings = Helper.ZeroLengthArrayOfLocationMapping;
    private string m_relativePath;
    private int m_relativeToSettingValue;
    private string m_serviceType;
    private string m_toolId;

    public ServiceDefinition(
      string serviceType,
      Guid identifier,
      string displayName,
      string relativePath,
      RelativeToSetting relativeToSetting,
      string description,
      string toolType)
      : this(serviceType, identifier, displayName, relativePath, relativeToSetting, description, toolType, new List<LocationMapping>())
    {
    }

    internal ServiceDefinition(
      string serviceType,
      Guid identifier,
      string displayName,
      string relativePath,
      RelativeToSetting relativeToSetting,
      string description,
      string toolType,
      List<LocationMapping> locationMappings)
    {
      this.ServiceType = serviceType;
      this.Identifier = identifier;
      this.DisplayName = displayName;
      this.RelativePath = relativePath;
      this.RelativeToSetting = relativeToSetting;
      this.Description = description;
      this.ToolType = toolType;
      this.LocationMappings = locationMappings == null ? (IEnumerable<LocationMapping>) new List<LocationMapping>() : (IEnumerable<LocationMapping>) locationMappings;
    }

    public string ServiceType
    {
      get => this.m_serviceType;
      set => this.m_serviceType = value;
    }

    public Guid Identifier
    {
      get => this.m_identifier;
      set => this.m_identifier = value;
    }

    public string DisplayName
    {
      get => this.m_displayName;
      set => this.m_displayName = value;
    }

    public RelativeToSetting RelativeToSetting
    {
      get => (RelativeToSetting) this.m_relativeToSettingValue;
      set => this.m_relativeToSettingValue = (int) value;
    }

    public string RelativePath
    {
      get => this.m_relativePath;
      set => this.m_relativePath = value;
    }

    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    public string ToolType
    {
      get => this.m_toolId;
      set => this.m_toolId = value;
    }

    public IEnumerable<LocationMapping> LocationMappings
    {
      get => (IEnumerable<LocationMapping>) new List<LocationMapping>((IEnumerable<LocationMapping>) this.m_mappings);
      internal set => this.m_mappings = new List<LocationMapping>(value);
    }

    internal List<LocationMapping> Mappings
    {
      get => this.m_mappings;
      set => this.m_mappings = value;
    }

    public void AddLocationMapping(AccessMapping accessMapping, string location)
    {
      if (this.RelativeToSetting != RelativeToSetting.FullyQualified)
        throw new InvalidOperationException(TFCommonResources.RelativeLocationMappingErrorMessage());
      if (location == null)
        throw new ArgumentNullException(TFCommonResources.FullyQualifiedLocationParameter());
      foreach (LocationMapping mapping in this.m_mappings)
      {
        if (VssStringComparer.AccessMappingMoniker.Equals(mapping.AccessMappingMoniker, accessMapping.Moniker))
        {
          mapping.Location = location;
          return;
        }
      }
      this.m_mappings.Add(new LocationMapping(accessMapping.Moniker, location));
    }

    public bool RemoveLocationMapping(AccessMapping accessMapping)
    {
      for (int index = 0; index < this.m_mappings.Count; ++index)
      {
        if (VssStringComparer.AccessMappingMoniker.Equals(accessMapping.Moniker, this.m_mappings[index].AccessMappingMoniker))
        {
          this.m_mappings.RemoveAt(index);
          return true;
        }
      }
      return false;
    }

    public LocationMapping GetLocationMapping(AccessMapping accessMapping)
    {
      ArgumentUtility.CheckForNull<AccessMapping>(accessMapping, nameof (accessMapping));
      if (this.RelativeToSetting == RelativeToSetting.FullyQualified)
      {
        foreach (LocationMapping locationMapping in this.LocationMappings)
        {
          if (VssStringComparer.AccessMappingMoniker.Equals(locationMapping.AccessMappingMoniker, accessMapping.Moniker))
            return locationMapping;
        }
      }
      return (LocationMapping) null;
    }

    internal void PrepareForWebServiceSerialization()
    {
      if (this.m_mappings == null)
        return;
      this.m_locationMappings = this.m_mappings.ToArray();
    }

    internal void ReactToWebServiceDeserialization(Dictionary<string, AccessMapping> accessMappings)
    {
      this.m_mappings = new List<LocationMapping>();
      foreach (LocationMapping locationMapping in this.m_locationMappings)
        this.m_mappings.Add(locationMapping);
    }

    internal ServiceDefinition Clone()
    {
      List<LocationMapping> locationMappings = new List<LocationMapping>();
      foreach (LocationMapping mapping in this.m_mappings)
        locationMappings.Add(new LocationMapping(mapping.AccessMappingMoniker, mapping.Location));
      return new ServiceDefinition(this.ServiceType, this.Identifier, this.DisplayName, this.RelativePath, this.RelativeToSetting, this.Description, this.ToolType, locationMappings);
    }

    internal static IEqualityComparer<ServiceDefinition> EqualityComparer => (IEqualityComparer<ServiceDefinition>) ServiceDefinition.s_equalityComparer;

    internal ServiceDefinition()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ServiceDefinition FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ServiceDefinition serviceDefinition = new ServiceDefinition();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
          if (name != null)
          {
            switch (name.Length)
            {
              case 6:
                if (name == "toolId")
                {
                  serviceDefinition.m_toolId = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 10:
                if (name == "identifier")
                {
                  serviceDefinition.m_identifier = XmlUtility.GuidFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 11:
                switch (name[3])
                {
                  case 'c':
                    if (name == "description")
                    {
                      serviceDefinition.m_description = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'i':
                    if (name == "isSingleton")
                    {
                      serviceDefinition.m_isSingleton = XmlUtility.BooleanFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'p':
                    if (name == "displayName")
                    {
                      serviceDefinition.m_displayName = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'v':
                    if (name == "serviceType")
                    {
                      serviceDefinition.m_serviceType = XmlUtility.StringFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 12:
                if (name == "relativePath")
                {
                  serviceDefinition.m_relativePath = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 17:
                if (name == "relativeToSetting")
                {
                  serviceDefinition.m_relativeToSettingValue = XmlUtility.Int32FromXmlAttribute(reader);
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "LocationMappings")
            serviceDefinition.m_locationMappings = Helper.ArrayOfLocationMappingFromXml(serviceProvider, reader, false);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return serviceDefinition;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServiceDefinition instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Description: " + this.m_description);
      stringBuilder.AppendLine("  DisplayName: " + this.m_displayName);
      stringBuilder.AppendLine("  Identifier: " + this.m_identifier.ToString());
      stringBuilder.AppendLine("  IsSingleton: " + this.m_isSingleton.ToString());
      stringBuilder.AppendLine("  LocationMappings: " + Helper.ArrayToString<LocationMapping>(this.m_locationMappings));
      stringBuilder.AppendLine("  RelativePath: " + this.m_relativePath);
      stringBuilder.AppendLine("  RelativeToSettingValue: " + this.m_relativeToSettingValue.ToString());
      stringBuilder.AppendLine("  ServiceType: " + this.m_serviceType);
      stringBuilder.AppendLine("  ToolId: " + this.m_toolId);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      this.PrepareForWebServiceSerialization();
      writer.WriteStartElement(element);
      if (this.m_description != null)
        XmlUtility.ToXmlAttribute(writer, "description", this.m_description);
      if (this.m_displayName != null)
        XmlUtility.ToXmlAttribute(writer, "displayName", this.m_displayName);
      if (this.m_identifier != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "identifier", this.m_identifier);
      if (this.m_isSingleton)
        XmlUtility.ToXmlAttribute(writer, "isSingleton", this.m_isSingleton);
      if (this.m_relativePath != null)
        XmlUtility.ToXmlAttribute(writer, "relativePath", this.m_relativePath);
      if (this.m_relativeToSettingValue != 0)
        XmlUtility.ToXmlAttribute(writer, "relativeToSetting", this.m_relativeToSettingValue);
      if (this.m_serviceType != null)
        XmlUtility.ToXmlAttribute(writer, "serviceType", this.m_serviceType);
      if (this.m_toolId != null)
        XmlUtility.ToXmlAttribute(writer, "toolId", this.m_toolId);
      Helper.ToXml(writer, "LocationMappings", this.m_locationMappings, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ServiceDefinition obj) => obj.ToXml(writer, element);

    private class DefinitionEqualityComparer : IEqualityComparer<ServiceDefinition>
    {
      public bool Equals(ServiceDefinition x, ServiceDefinition y) => x.ServiceType.Equals(y.ServiceType) && x.Identifier.Equals(y.Identifier);

      public int GetHashCode(ServiceDefinition obj) => obj.Identifier.GetHashCode();
    }
  }
}
