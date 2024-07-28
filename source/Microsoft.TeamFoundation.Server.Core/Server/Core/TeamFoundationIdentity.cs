// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationIdentity
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  public sealed class TeamFoundationIdentity : IVssIdentityServer, IVssIdentity, IReadOnlyVssIdentity
  {
    private static readonly HashSet<string> ExcludedProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)
    {
      "CUID",
      "CUIDState",
      "MetadataUpdateDate"
    };
    private HashSet<string> m_modifiedPropertiesLog;
    private HashSet<string> m_modifiedLocalPropertiesLog;
    private readonly Dictionary<string, object> m_properties;
    private readonly Dictionary<string, object> m_localProperties;
    private bool m_marshalPropertiesAsDev10Attributes;
    private int m_uniqueUserId;
    private string m_uniqueName;
    private string m_providerDisplayName;
    private string m_customDisplayName;
    private const string s_area = "IMS";

    public TeamFoundationIdentity(
      IdentityDescriptor descriptor,
      string displayName,
      bool isActive,
      ICollection<IdentityDescriptor> members,
      ICollection<IdentityDescriptor> memberOf)
      : this(descriptor, displayName, (string) null, isActive, Guid.Empty, Guid.Empty, 0, members, memberOf, (ICollection<Guid>) null, true)
    {
    }

    internal TeamFoundationIdentity(
      IdentityDescriptor descriptor,
      string providerDisplayName,
      string customDisplayName,
      bool isActive,
      Guid masterId,
      Guid teamFoundationId,
      int uniqueUserId,
      ICollection<IdentityDescriptor> members,
      ICollection<IdentityDescriptor> memberOf,
      ICollection<Guid> memberMasterIds,
      bool checkDisplayNames,
      SubjectDescriptor subjectDescriptor = default (SubjectDescriptor),
      IdentityMetaType metaType = IdentityMetaType.Unknown)
    {
      this.Initialize(descriptor, subjectDescriptor, metaType, providerDisplayName, customDisplayName, isActive, masterId, teamFoundationId, uniqueUserId, members, memberOf, memberMasterIds, checkDisplayNames);
      this.m_properties = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
      this.m_localProperties = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
    }

    public TeamFoundationIdentity(TeamFoundationIdentity clone)
    {
      if (clone == null)
        throw new ArgumentNullException(nameof (clone));
      this.Initialize(new IdentityDescriptor(clone.Descriptor), clone.SubjectDescriptor, clone.MetaType, clone.ProviderDisplayName, clone.CustomDisplayName, clone.IsActive, clone.MasterId, clone.TeamFoundationId, clone.m_uniqueUserId, clone.Members, clone.MemberOf, clone.MemberMasterIds, false);
      this.m_properties = new Dictionary<string, object>((IDictionary<string, object>) clone.m_properties, (IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
      this.m_localProperties = new Dictionary<string, object>((IDictionary<string, object>) clone.m_localProperties, (IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
      if (clone.m_modifiedLocalPropertiesLog != null)
        this.m_modifiedLocalPropertiesLog = new HashSet<string>((IEnumerable<string>) clone.m_modifiedLocalPropertiesLog);
      if (clone.m_modifiedPropertiesLog == null)
        return;
      this.m_modifiedPropertiesLog = new HashSet<string>((IEnumerable<string>) clone.m_modifiedPropertiesLog);
    }

    private void Initialize(
      IdentityDescriptor descriptor,
      SubjectDescriptor subjectDescriptor,
      IdentityMetaType metaType,
      string providerDisplayName,
      string customDisplayName,
      bool isActive,
      Guid masterId,
      Guid teamFoundationId,
      int uniqueUserId,
      ICollection<IdentityDescriptor> members,
      ICollection<IdentityDescriptor> memberOf,
      ICollection<Guid> memberMasterIds,
      bool checkDisplayNames)
    {
      if (descriptor == (IdentityDescriptor) null)
        throw new ArgumentNullException(nameof (descriptor));
      if (providerDisplayName == null)
        providerDisplayName = string.Empty;
      if (members == null)
        members = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      if (memberOf == null)
        memberOf = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      if (memberMasterIds == null)
        memberMasterIds = (ICollection<Guid>) Array.Empty<Guid>();
      this.Descriptor = descriptor;
      this.SubjectDescriptor = subjectDescriptor;
      this.MetaType = metaType;
      if (checkDisplayNames)
      {
        this.ProviderDisplayName = providerDisplayName;
        this.CustomDisplayName = customDisplayName;
      }
      else
      {
        this.m_providerDisplayName = providerDisplayName;
        this.m_customDisplayName = customDisplayName;
      }
      this.IsActive = isActive;
      this.TeamFoundationId = teamFoundationId;
      this.MasterId = masterId;
      this.m_uniqueUserId = uniqueUserId;
      this.Members = members;
      this.MemberOf = memberOf;
      this.MemberMasterIds = memberMasterIds;
    }

    public TeamFoundationIdentity()
    {
      this.m_properties = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
      this.m_localProperties = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
    }

    public void PrepareForWebServiceSerialization(TeamFoundationSupportedFeatures supportedFeatures)
    {
      this.AttributesSet = (KeyValue<string, string>[]) null;
      if ((supportedFeatures & TeamFoundationSupportedFeatures.IdentityProperties) == TeamFoundationSupportedFeatures.IdentityProperties)
      {
        this.m_marshalPropertiesAsDev10Attributes = false;
      }
      else
      {
        this.m_marshalPropertiesAsDev10Attributes = true;
        this.AttributesSet = new KeyValue<string, string>[this.m_properties.Count];
        int num = 0;
        foreach (KeyValuePair<string, object> property in this.m_properties)
          this.AttributesSet[num++] = new KeyValue<string, string>(property.Key, property.Value.ToString());
      }
      this.ScrubIdentityProperties((IDictionary<string, object>) this.m_properties);
      this.ScrubIdentityProperties((IDictionary<string, object>) this.m_localProperties);
      this.MembersSet = new IdentityDescriptor[this.Members.Count];
      this.Members.CopyTo(this.MembersSet, 0);
      this.MemberOfSet = new IdentityDescriptor[this.MemberOf.Count];
      this.MemberOf.CopyTo(this.MemberOfSet, 0);
    }

    private void ScrubIdentityProperties(IDictionary<string, object> properties)
    {
      if (properties == null || properties.Count == 0)
        return;
      foreach (string key in properties.Keys.ToArray<string>())
      {
        if (TeamFoundationIdentity.ExcludedProperties.Contains(key))
          properties.Remove(key);
      }
    }

    public IdentityDescriptor Descriptor { get; set; }

    [XmlIgnore]
    public SubjectDescriptor SubjectDescriptor { get; set; }

    [XmlIgnore]
    public IdentityMetaType MetaType { get; set; }

    [XmlAttribute]
    public string DisplayName
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(this.CustomDisplayName))
          return this.CustomDisplayName;
        return this.UniqueUserId != 0 && (StringComparer.OrdinalIgnoreCase.Equals(this.Descriptor.IdentityType, "Microsoft.TeamFoundation.BindPendingIdentity") || StringComparer.OrdinalIgnoreCase.Equals(this.Descriptor.IdentityType, "Microsoft.IdentityModel.Claims.ClaimsIdentity")) && this.ProviderDisplayName.IndexOf('@') > 0 ? IdentityHelper.GetUniqueName(string.Empty, this.ProviderDisplayName, this.UniqueUserId) : this.ProviderDisplayName;
      }
      set
      {
        this.ProviderDisplayName = value;
        this.CustomDisplayName = (string) null;
      }
    }

    public string GetAttribute(string name, string defaultValue)
    {
      object obj;
      return this.TryGetProperty(IdentityPropertyScope.Global, name, out obj) && obj != null ? obj.ToString() : defaultValue;
    }

    public bool TryGetProperty(string name, out object value) => this.TryGetProperty(IdentityPropertyScope.Both, name, out value);

    public bool TryGetProperty(IdentityPropertyScope propertyScope, string name, out object value)
    {
      if (propertyScope == IdentityPropertyScope.Global)
        return this.m_properties.TryGetValue(name, out value);
      if (propertyScope == IdentityPropertyScope.Local)
        return this.m_localProperties.TryGetValue(name, out value);
      return this.TryGetProperty(IdentityPropertyScope.Local, name, out value) && value != null || this.TryGetProperty(IdentityPropertyScope.Global, name, out value);
    }

    public T GetProperty<T>(string name, T defaultValue)
    {
      T obj = default (T);
      return this.m_localProperties.TryGetValidatedValue<T>(name, out obj, false) || this.m_properties.TryGetValidatedValue<T>(name, out obj, false) ? obj : defaultValue;
    }

    public object GetProperty(IdentityPropertyScope propertyScope, string name)
    {
      if (propertyScope == IdentityPropertyScope.Global)
        return this.m_properties[name];
      if (propertyScope == IdentityPropertyScope.Local)
        return this.m_localProperties[name];
      object obj;
      return !this.TryGetProperty(IdentityPropertyScope.Local, name, out obj) || obj == null ? this.m_properties[name] : obj;
    }

    public IEnumerable<KeyValuePair<string, object>> GetProperties(
      IdentityPropertyScope propertyScope)
    {
      if (propertyScope == IdentityPropertyScope.Global)
        return (IEnumerable<KeyValuePair<string, object>>) this.m_properties;
      return propertyScope == IdentityPropertyScope.Local ? (IEnumerable<KeyValuePair<string, object>>) this.m_localProperties : this.m_localProperties.Concat<KeyValuePair<string, object>>(this.m_properties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => !this.m_localProperties.ContainsKey(kvp.Key))));
    }

    [Obsolete]
    public void SetAttribute(string name, string value)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      this.m_properties[name] = (object) value;
      this.m_uniqueName = (string) null;
    }

    public void SetProperty(string name, object value) => this.SetProperty(IdentityPropertyScope.Global, name, value);

    void IVssIdentity.SetProperty(string name, object value) => this.SetProperty(IdentityPropertyScope.Global, name, value, false);

    public void SetProperty(IdentityPropertyScope propertyScope, string name, object value) => this.SetProperty(propertyScope, name, value, true);

    private void SetProperty(
      IdentityPropertyScope propertyScope,
      string name,
      object value,
      bool enforceReadOnly)
    {
      PropertyValidation.ValidatePropertyName(name);
      PropertyValidation.ValidatePropertyValue(name, value);
      if (enforceReadOnly && (this.IsContainer ? (IdentityAttributeTags.GroupReadOnlyProperties.Contains(name) ? 1 : 0) : (IdentityAttributeTags.ReadOnlyProperties.Contains(name) ? 1 : 0)) != 0)
        throw new NotSupportedException(TFCommonResources.IdentityPropertyReadOnly((object) name));
      switch (propertyScope)
      {
        case IdentityPropertyScope.Global:
          this.SetGlobalProperty(name, value);
          this.RemoveProperty(IdentityPropertyScope.Local, name, enforceReadOnly);
          break;
        case IdentityPropertyScope.Local:
          this.SetLocalProperty(name, value);
          break;
        case IdentityPropertyScope.Both:
          throw new InvalidOperationException(TFCommonResources.InvalidPropertyScope());
      }
      this.m_uniqueName = (string) null;
    }

    internal void SetPropertyInternal(
      IdentityPropertyScope propertyScope,
      string name,
      object value)
    {
      switch (propertyScope)
      {
        case IdentityPropertyScope.Global:
          this.SetGlobalProperty(name, value);
          break;
        case IdentityPropertyScope.Local:
          this.SetLocalProperty(name, value);
          break;
        case IdentityPropertyScope.Both:
          throw new InvalidOperationException(TFCommonResources.InvalidPropertyScope());
      }
    }

    private void SetGlobalProperty(string name, object value)
    {
      this.m_properties[name] = value;
      if (this.m_modifiedPropertiesLog == null)
        this.m_modifiedPropertiesLog = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
      this.m_modifiedPropertiesLog.Add(name);
    }

    private void SetLocalProperty(string name, object value)
    {
      this.m_localProperties[name] = value;
      if (this.m_modifiedLocalPropertiesLog == null)
        this.m_modifiedLocalPropertiesLog = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
      this.m_modifiedLocalPropertiesLog.Add(name);
    }

    public void RemoveProperty(string name) => this.SetProperty(IdentityPropertyScope.Global, name, (object) null);

    public void RemoveProperty(IdentityPropertyScope propertyScope, string name) => this.SetProperty(propertyScope, name, (object) null);

    private void RemoveProperty(
      IdentityPropertyScope propertyScope,
      string name,
      bool enforceReadOnly)
    {
      this.SetProperty(propertyScope, name, (object) null, enforceReadOnly);
    }

    internal Guid StorageKey(IVssRequestContext requestContext, TeamFoundationHostType hostLevel) => this.MasterId;

    [XmlElement("Attributes", typeof (KeyValue<string, string>[]))]
    [ClientProperty(ClientVisibility.Private)]
    public KeyValue<string, string>[] AttributesSet { get; set; }

    [XmlElement("Properties", typeof (StreamingCollection<PropertyValue>))]
    [ClientProperty(ClientVisibility.Private)]
    public StreamingCollection<PropertyValue> PropertiesSet
    {
      get
      {
        StreamingCollection<PropertyValue> propertiesSet = (StreamingCollection<PropertyValue>) null;
        if (!this.m_marshalPropertiesAsDev10Attributes)
        {
          propertiesSet = new StreamingCollection<PropertyValue>();
          foreach (KeyValuePair<string, object> property in this.m_properties)
          {
            PropertyValue propertyValue = new PropertyValue(property.Key, property.Value, PropertyTypeMatch.Unspecified, false);
            propertiesSet.Enqueue(propertyValue);
          }
        }
        return propertiesSet;
      }
      set => TeamFoundationTracingService.TraceRaw(0, TraceLevel.Warning, "IMS", "xmlSerialization", "Ignoring unexpected call to TeamFoundationIdentity.set_PropertiesSet.");
    }

    [XmlElement("LocalProperties", typeof (StreamingCollection<PropertyValue>))]
    [ClientProperty(ClientVisibility.Private)]
    public StreamingCollection<PropertyValue> LocalPropertiesSet
    {
      get
      {
        StreamingCollection<PropertyValue> localPropertiesSet = (StreamingCollection<PropertyValue>) null;
        if (!this.m_marshalPropertiesAsDev10Attributes)
        {
          localPropertiesSet = new StreamingCollection<PropertyValue>();
          foreach (KeyValuePair<string, object> localProperty in this.m_localProperties)
          {
            PropertyValue propertyValue = new PropertyValue(localProperty.Key, localProperty.Value);
            localPropertiesSet.Enqueue(propertyValue);
          }
        }
        return localPropertiesSet;
      }
      set => TeamFoundationTracingService.TraceRaw(0, TraceLevel.Warning, "IMS", "xmlSerialization", "Ignoring unexpected call to TeamFoundationIdentity.set_LocalPropertiesSet.");
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public bool IsContainer
    {
      get => string.Equals(this.m_properties.GetCastedValueOrDefault<string, string>("SchemaClassName", (string) null), "Group", StringComparison.OrdinalIgnoreCase);
      set => throw new NotSupportedException();
    }

    [XmlAttribute]
    public bool IsActive { get; set; }

    [XmlAttribute]
    public Guid TeamFoundationId { get; set; }

    Guid IReadOnlyVssIdentity.Id => this.TeamFoundationId;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string UniqueName
    {
      get
      {
        if (this.m_uniqueName == null)
          this.m_uniqueName = IdentityHelper.GetUniqueName(this.GetAttribute("Domain", string.Empty), this.GetAttribute("Account", string.Empty), this.UniqueUserId);
        return this.m_uniqueName;
      }
      set => this.m_uniqueName = value;
    }

    [XmlAttribute]
    public int UniqueUserId
    {
      get => this.m_uniqueUserId;
      set
      {
        this.m_uniqueUserId = value;
        this.m_uniqueName = (string) null;
      }
    }

    [XmlIgnore]
    public ICollection<IdentityDescriptor> Members { get; set; }

    [XmlIgnore]
    public ICollection<IdentityDescriptor> MemberOf { get; set; }

    [XmlIgnore]
    internal ICollection<Guid> MemberMasterIds { get; set; }

    [XmlElement("Members", typeof (IdentityDescriptor[]))]
    [ClientProperty(PropertyName = "Members")]
    public IdentityDescriptor[] MembersSet { get; set; }

    [XmlElement("MemberOf", typeof (IdentityDescriptor[]))]
    [ClientProperty(PropertyName = "MemberOf")]
    public IdentityDescriptor[] MemberOfSet { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string DistinctDisplayName => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} <{1}>", (object) this.DisplayName, (object) this.GetDisambiguationPart());

    private string GetDisambiguationPart()
    {
      string attribute1 = this.GetAttribute("Account", string.Empty);
      if ((this.IsExternalUser || this.IsMsaDomain) && !this.IsContainer)
        return attribute1;
      string attribute2 = this.GetAttribute("LocalScopeId", string.Empty);
      string attribute3 = this.GetAttribute("Domain", string.Empty);
      if (!string.IsNullOrEmpty(attribute2))
        return string.Format("{0}{1}", (object) "id:", (object) this.TeamFoundationId);
      return string.IsNullOrEmpty(attribute3) ? attribute1 : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) attribute3, (object) attribute1);
    }

    public bool IsExternalUser => Guid.TryParse(this.GetAttribute("Domain", string.Empty), out Guid _);

    internal bool IsMsaDomain => string.Compare(this.GetAttribute("Domain", string.Empty), "Windows Live ID", StringComparison.OrdinalIgnoreCase) == 0;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TeamFoundationIdentity (IdentityType: {0}; Identifier: {1}; TFID: {2}; UniqueName: {3}; Display Name: {4}; Active: {5})", this.Descriptor == (IdentityDescriptor) null ? (object) string.Empty : (object) this.Descriptor.IdentityType, this.Descriptor == (IdentityDescriptor) null ? (object) string.Empty : (object) this.Descriptor.Identifier, (object) this.TeamFoundationId, (object) this.UniqueName, (object) this.DisplayName, (object) this.IsActive);

    public string ProviderDisplayName
    {
      get => this.m_providerDisplayName;
      set => this.m_providerDisplayName = IdentityHelper.CleanProviderDisplayName(value, this.Descriptor);
    }

    internal void InitializeProperty(
      IdentityPropertyScope propertyScope,
      string name,
      object value)
    {
      switch (propertyScope)
      {
        case IdentityPropertyScope.Global:
          this.m_properties[name] = value;
          break;
        case IdentityPropertyScope.Local:
          this.m_localProperties[name] = value;
          break;
        case IdentityPropertyScope.Both:
          throw new InvalidOperationException(TFCommonResources.InvalidPropertyScope());
      }
      this.m_uniqueName = (string) null;
    }

    internal void ResetModifiedProperties()
    {
      this.m_modifiedPropertiesLog = (HashSet<string>) null;
      this.m_modifiedLocalPropertiesLog = (HashSet<string>) null;
    }

    public string CustomDisplayName
    {
      get => this.m_customDisplayName;
      set
      {
        if (value == null)
          this.m_customDisplayName = value;
        else
          this.m_customDisplayName = IdentityHelper.CleanCustomDisplayName(value, this.Descriptor);
      }
    }

    internal Guid MasterId { get; set; }

    internal HashSet<string> GetModifiedPropertiesLog(IdentityPropertyScope propertyScope)
    {
      if (propertyScope == IdentityPropertyScope.Global)
        return this.m_modifiedPropertiesLog;
      if (propertyScope == IdentityPropertyScope.Local)
        return this.m_modifiedLocalPropertiesLog;
      throw new InvalidOperationException(TFCommonResources.InvalidPropertyScope());
    }
  }
}
