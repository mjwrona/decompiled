// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Location.ServiceDefinition
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.Location
{
  [CallOnSerialization("PrepareForWebServiceSerialization")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class ServiceDefinition
  {
    private List<LocationMapping> m_locationMappings;

    public ServiceDefinition() => this.m_locationMappings = new List<LocationMapping>();

    internal ServiceDefinition(
      string serviceType,
      Guid identifier,
      string displayName,
      string relativePath,
      RelativeToSetting relativeToSetting,
      string description,
      string toolId,
      List<LocationMapping> locationMappings)
    {
      this.ServiceType = serviceType;
      this.Identifier = identifier;
      this.DisplayName = displayName;
      this.RelativePath = relativePath;
      this.RelativeToSetting = relativeToSetting;
      this.Description = description;
      this.ToolId = toolId;
      this.m_locationMappings = locationMappings == null ? new List<LocationMapping>() : locationMappings;
    }

    [XmlAttribute("serviceType")]
    [ClientProperty(ClientVisibility.Private)]
    public string ServiceType { get; set; }

    [XmlAttribute("identifier")]
    [ClientProperty(ClientVisibility.Private)]
    public Guid Identifier { get; set; }

    [XmlAttribute("displayName")]
    [ClientProperty(ClientVisibility.Private)]
    public string DisplayName { get; set; }

    [XmlAttribute("isSingleton")]
    [ClientProperty(ClientVisibility.Private)]
    [Obsolete("The IsSingleton property is no longer used. It will always return true for backwards compatibility purposes")]
    public bool IsSingleton => true;

    [ClientProperty(ClientVisibility.Private)]
    public List<LocationMapping> LocationMappings => this.m_locationMappings;

    [XmlIgnore]
    public RelativeToSetting RelativeToSetting
    {
      get => (RelativeToSetting) this.RelativeToSettingValue;
      set => this.RelativeToSettingValue = (int) value;
    }

    [XmlAttribute("relativeToSetting")]
    [ClientProperty(ClientVisibility.Private)]
    public int RelativeToSettingValue { get; set; }

    [XmlAttribute("relativePath")]
    [ClientProperty(ClientVisibility.Private)]
    public string RelativePath { get; set; }

    [XmlAttribute("description")]
    [ClientProperty(ClientVisibility.Private)]
    public string Description { get; set; }

    [XmlAttribute("toolId")]
    [ClientProperty(ClientVisibility.Private)]
    public string ToolId { get; set; }

    public void AddLocationMapping(AccessMapping accessMapping, string location)
    {
      if (this.RelativeToSetting != RelativeToSetting.FullyQualified)
        throw new InvalidOperationException(TFCommonResources.RelativeLocationMappingErrorMessage());
      if (location == null)
        throw new ArgumentException(TFCommonResources.FullyQualifiedLocationParameter());
      foreach (LocationMapping locationMapping in this.LocationMappings)
      {
        if (VssStringComparer.AccessMappingMoniker.Equals(locationMapping.AccessMappingMoniker, accessMapping.Moniker))
        {
          locationMapping.Location = location;
          return;
        }
      }
      this.LocationMappings.Add(new LocationMapping(accessMapping.Moniker, location));
    }

    public bool RemoveLocationMapping(AccessMapping accessMapping)
    {
      for (int index = 0; index < this.LocationMappings.Count; ++index)
      {
        if (VssStringComparer.AccessMappingMoniker.Equals(accessMapping.Moniker, this.LocationMappings[index].AccessMappingMoniker))
        {
          this.LocationMappings.RemoveAt(index);
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

    internal ServiceDefinition Clone()
    {
      List<LocationMapping> locationMappings = new List<LocationMapping>();
      foreach (LocationMapping locationMapping in this.m_locationMappings)
        locationMappings.Add(new LocationMapping(locationMapping.AccessMappingMoniker, locationMapping.Location));
      return new ServiceDefinition(this.ServiceType, this.Identifier, this.DisplayName, this.RelativePath, this.RelativeToSetting, this.Description, this.ToolId, locationMappings);
    }

    internal void SetLocationMappings(IEnumerable<LocationMapping> locationMappings) => this.m_locationMappings = new List<LocationMapping>(locationMappings);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service Definition:\r\n    DisplayName: {0}\r\n    ServiceType: {1}\r\n    Identifier: {2}\r\n    Description: {3}\r\n    RelativePath: {4}\r\n    RelativeToSetting: {5}\r\n    ToolId: {6}", (object) this.DisplayName, (object) this.ServiceType, (object) this.Identifier, (object) this.Description, (object) this.RelativePath, (object) this.RelativeToSetting, (object) this.ToolId);
  }
}
