// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.ServiceDefinition
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Location
{
  [DebuggerDisplay("{ServiceType}:{Identifier}")]
  [DataContract]
  public class ServiceDefinition : ISecuredObject
  {
    private bool m_isSingleton;
    private bool m_hasModifiedProperties = true;

    public ServiceDefinition()
    {
      this.LocationMappings = new List<LocationMapping>();
      this.Status = ServiceStatus.Active;
      this.Properties = new PropertiesCollection();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ServiceDefinition(
      string serviceType,
      Guid identifier,
      string displayName,
      string relativePath,
      RelativeToSetting relativeToSetting,
      string description,
      string toolId,
      List<LocationMapping> locationMappings = null,
      Guid serviceOwner = default (Guid))
    {
      this.ServiceType = serviceType;
      this.Identifier = identifier;
      this.DisplayName = displayName;
      this.RelativePath = relativePath;
      this.RelativeToSetting = relativeToSetting;
      this.Description = description;
      this.ToolId = toolId;
      if (locationMappings == null)
        locationMappings = new List<LocationMapping>();
      this.LocationMappings = locationMappings;
      this.ServiceOwner = serviceOwner;
      this.Properties = new PropertiesCollection();
      this.Status = ServiceStatus.Active;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [XmlAttribute("serviceType")]
    public string ServiceType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [XmlAttribute("identifier")]
    public Guid Identifier { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [XmlAttribute("displayName")]
    public string DisplayName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public RelativeToSetting RelativeToSetting { get; set; }

    [XmlAttribute("relativeToSetting")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int RelativeToSettingValue
    {
      get => (int) this.RelativeToSetting;
      set => this.RelativeToSetting = (RelativeToSetting) value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [XmlAttribute("relativePath")]
    public string RelativePath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [XmlAttribute("description")]
    public string Description { get; set; }

    [DataMember]
    public Guid ServiceOwner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<LocationMapping> LocationMappings { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [XmlAttribute("toolId")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ToolId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ParentServiceType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid ParentIdentifier { get; set; }

    [DefaultValue(ServiceStatus.Active)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ServiceStatus Status { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [XmlAttribute("inheritLevel")]
    public InheritLevel InheritLevel { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [XmlIgnore]
    public PropertiesCollection Properties { get; set; }

    public T GetProperty<T>(string name, T defaultValue)
    {
      T obj;
      return this.Properties != null && this.Properties.TryGetValue<T>(name, out obj) ? obj : defaultValue;
    }

    public bool TryGetProperty(string name, out object value)
    {
      value = (object) null;
      return this.Properties != null && this.Properties.TryGetValue(name, out value);
    }

    public void SetProperty(string name, object value)
    {
      this.m_hasModifiedProperties = true;
      this.Properties[name] = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool HasModifiedProperties => this.m_hasModifiedProperties;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ResetModifiedProperties() => this.m_hasModifiedProperties = false;

    [XmlAttribute("resourceVersion")]
    [DefaultValue(0)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ResourceVersion { get; set; }

    [XmlIgnore]
    public Version MinVersion { get; set; }

    [XmlAttribute("minVersion")]
    [DefaultValue(null)]
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "MinVersion")]
    public string MinVersionString
    {
      get => this.MinVersion == (Version) null ? (string) null : this.MinVersion.ToString(2);
      set
      {
        if (string.IsNullOrEmpty(value))
          this.MinVersion = (Version) null;
        else
          this.MinVersion = new Version(value);
      }
    }

    [XmlIgnore]
    public Version MaxVersion { get; set; }

    [XmlAttribute("maxVersion")]
    [DefaultValue(null)]
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "MaxVersion")]
    public string MaxVersionString
    {
      get => this.MaxVersion == (Version) null ? (string) null : this.MaxVersion.ToString(2);
      set
      {
        if (string.IsNullOrEmpty(value))
          this.MaxVersion = (Version) null;
        else
          this.MaxVersion = new Version(value);
      }
    }

    [XmlIgnore]
    public Version ReleasedVersion { get; set; }

    [XmlAttribute("releasedVersion")]
    [DefaultValue(null)]
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "ReleasedVersion")]
    public string ReleasedVersionString
    {
      get => this.ReleasedVersion == (Version) null ? (string) null : this.ReleasedVersion.ToString(2);
      set
      {
        if (string.IsNullOrEmpty(value))
          this.ReleasedVersion = (Version) null;
        else
          this.ReleasedVersion = new Version(value);
      }
    }

    public ServiceDefinition Clone() => this.Clone(true);

    public ServiceDefinition Clone(bool includeLocationMappings)
    {
      List<LocationMapping> locationMappingList;
      if (this.LocationMappings != null & includeLocationMappings)
      {
        locationMappingList = new List<LocationMapping>(this.LocationMappings.Count);
        foreach (LocationMapping locationMapping in this.LocationMappings)
          locationMappingList.Add(new LocationMapping()
          {
            AccessMappingMoniker = locationMapping.AccessMappingMoniker,
            Location = locationMapping.Location
          });
      }
      else
        locationMappingList = new List<LocationMapping>();
      PropertiesCollection propertiesCollection = this.Properties == null ? new PropertiesCollection() : new PropertiesCollection((IDictionary<string, object>) this.Properties, false);
      ServiceDefinition serviceDefinition = new ServiceDefinition();
      serviceDefinition.ServiceType = this.ServiceType;
      serviceDefinition.Identifier = this.Identifier;
      serviceDefinition.DisplayName = this.DisplayName;
      serviceDefinition.RelativePath = this.RelativePath;
      serviceDefinition.RelativeToSetting = this.RelativeToSetting;
      serviceDefinition.Description = this.Description;
      serviceDefinition.LocationMappings = locationMappingList;
      serviceDefinition.ServiceOwner = this.ServiceOwner;
      serviceDefinition.ToolId = this.ToolId;
      serviceDefinition.ParentServiceType = this.ParentServiceType;
      serviceDefinition.ParentIdentifier = this.ParentIdentifier;
      serviceDefinition.Status = this.Status;
      serviceDefinition.Properties = propertiesCollection;
      serviceDefinition.ResourceVersion = this.ResourceVersion;
      serviceDefinition.MinVersion = this.MinVersion;
      serviceDefinition.MaxVersion = this.MaxVersion;
      serviceDefinition.ReleasedVersion = this.ReleasedVersion;
      serviceDefinition.ResetModifiedProperties();
      return serviceDefinition;
    }

    internal static ServiceDefinition FromXml(IServiceProvider serviceProvider, XmlReader reader)
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
                  serviceDefinition.ToolId = reader.Value;
                  continue;
                }
                continue;
              case 10:
                switch (name[1])
                {
                  case 'a':
                    if (name == "maxVersion")
                    {
                      serviceDefinition.MaxVersionString = reader.Value;
                      continue;
                    }
                    continue;
                  case 'd':
                    if (name == "identifier")
                    {
                      serviceDefinition.Identifier = XmlConvert.ToGuid(reader.Value);
                      continue;
                    }
                    continue;
                  case 'i':
                    if (name == "minVersion")
                    {
                      serviceDefinition.MinVersionString = reader.Value;
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 11:
                switch (name[3])
                {
                  case 'c':
                    if (name == "description")
                    {
                      serviceDefinition.Description = reader.Value;
                      continue;
                    }
                    continue;
                  case 'i':
                    if (name == "isSingleton")
                    {
                      serviceDefinition.m_isSingleton = XmlConvert.ToBoolean(reader.Value);
                      continue;
                    }
                    continue;
                  case 'p':
                    if (name == "displayName")
                    {
                      serviceDefinition.DisplayName = reader.Value;
                      continue;
                    }
                    continue;
                  case 'v':
                    if (name == "serviceType")
                    {
                      serviceDefinition.ServiceType = reader.Value;
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 12:
                if (name == "relativePath")
                {
                  serviceDefinition.RelativePath = reader.Value;
                  continue;
                }
                continue;
              case 15:
                switch (name[2])
                {
                  case 'l':
                    if (name == "releasedVersion")
                    {
                      serviceDefinition.ReleasedVersionString = reader.Value;
                      continue;
                    }
                    continue;
                  case 's':
                    if (name == "resourceVersion")
                    {
                      serviceDefinition.ResourceVersion = XmlConvert.ToInt32(reader.Value);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 17:
                if (name == "relativeToSetting")
                {
                  serviceDefinition.RelativeToSetting = (RelativeToSetting) XmlConvert.ToInt32(reader.Value);
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
          switch (reader.Name)
          {
            case "LocationMappings":
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              serviceDefinition.LocationMappings = new List<LocationMapping>((IEnumerable<LocationMapping>) XmlUtility.ArrayOfObjectFromXml<LocationMapping>(serviceProvider, reader, "LocationMapping", false, ServiceDefinition.\u003C\u003EO.\u003C0\u003E__FromXml ?? (ServiceDefinition.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, LocationMapping>(LocationMapping.FromXml))));
              continue;
            case "Properties":
              reader.ReadOuterXml();
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return serviceDefinition;
    }

    public LocationMapping GetLocationMapping(AccessMapping accessMapping)
    {
      ArgumentUtility.CheckForNull<AccessMapping>(accessMapping, nameof (accessMapping));
      return this.GetLocationMapping(accessMapping.Moniker);
    }

    public LocationMapping GetLocationMapping(string accessMappingMoniker)
    {
      ArgumentUtility.CheckForNull<string>(accessMappingMoniker, nameof (accessMappingMoniker));
      if (this.RelativeToSetting == RelativeToSetting.FullyQualified)
      {
        foreach (LocationMapping locationMapping in this.LocationMappings)
        {
          if (VssStringComparer.AccessMappingMoniker.Equals(locationMapping.AccessMappingMoniker, accessMappingMoniker))
            return locationMapping;
        }
      }
      return (LocationMapping) null;
    }

    public void AddLocationMapping(AccessMapping accessMapping, string location)
    {
      if (this.RelativeToSetting != RelativeToSetting.FullyQualified)
        throw new InvalidOperationException(WebApiResources.RelativeLocationMappingErrorMessage());
      if (location == null)
        throw new ArgumentException(WebApiResources.FullyQualifiedLocationParameter());
      foreach (LocationMapping locationMapping in this.LocationMappings)
      {
        if (VssStringComparer.AccessMappingMoniker.Equals(locationMapping.AccessMappingMoniker, accessMapping.Moniker))
        {
          locationMapping.Location = location;
          return;
        }
      }
      this.LocationMappings.Add(new LocationMapping()
      {
        AccessMappingMoniker = accessMapping.Moniker,
        Location = location
      });
    }

    Guid ISecuredObject.NamespaceId => LocationSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => LocationSecurityConstants.ServiceDefinitionsToken;
  }
}
