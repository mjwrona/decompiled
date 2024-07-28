// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationIdentity
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class TeamFoundationIdentity
  {
    private string m_clientSideUniqueName;
    private Dictionary<string, object> m_properties = new Dictionary<string, object>();
    private Dictionary<string, object> m_localProperties = new Dictionary<string, object>();
    private HashSet<string> m_modifiedPropertiesLog;
    private HashSet<string> m_modifiedLocalPropertiesLog;
    internal KeyValueOfStringString[] m_attributesSet = Helper.ZeroLengthArrayOfKeyValueOfStringString;
    private IdentityDescriptor m_descriptor;
    private string m_displayName;
    private bool m_isActive;
    private bool m_isContainer;
    internal PropertyValue[] m_localPropertiesSet = Helper.ZeroLengthArrayOfPropertyValue;
    internal IdentityDescriptor[] m_memberOf = Helper.ZeroLengthArrayOfIdentityDescriptor;
    internal IdentityDescriptor[] m_members = Helper.ZeroLengthArrayOfIdentityDescriptor;
    internal PropertyValue[] m_propertiesSet = Helper.ZeroLengthArrayOfPropertyValue;
    private Guid m_teamFoundationId = Guid.Empty;
    private string m_uniqueName;
    private int m_uniqueUserId;

    internal TeamFoundationIdentity(
      IdentityDescriptor descriptor,
      string displayName,
      bool isActive,
      IdentityDescriptor[] members,
      IdentityDescriptor[] memberOf)
    {
      if (descriptor == null)
        throw new ArgumentNullException(nameof (descriptor));
      this.m_displayName = displayName != null ? displayName : throw new ArgumentNullException(nameof (displayName));
      this.m_descriptor = descriptor;
      this.m_isActive = isActive;
      this.m_teamFoundationId = Guid.Empty;
      this.m_uniqueUserId = 0;
      this.m_members = members == null ? Array.Empty<IdentityDescriptor>() : members;
      if (memberOf != null)
        this.m_memberOf = memberOf;
      else
        this.m_memberOf = Array.Empty<IdentityDescriptor>();
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

    public object GetProperty(string name) => this.GetProperty(IdentityPropertyScope.Both, name);

    public object GetProperty(IdentityPropertyScope propertyScope, string name)
    {
      if (propertyScope == IdentityPropertyScope.Global)
        return this.m_properties[name];
      if (propertyScope == IdentityPropertyScope.Local)
        return this.m_localProperties[name];
      object obj;
      return !this.TryGetProperty(IdentityPropertyScope.Local, name, out obj) || obj == null ? this.m_properties[name] : obj;
    }

    public IEnumerable<KeyValuePair<string, object>> GetProperties() => this.GetProperties(IdentityPropertyScope.Both);

    public IEnumerable<KeyValuePair<string, object>> GetProperties(
      IdentityPropertyScope propertyScope)
    {
      if (propertyScope == IdentityPropertyScope.Global)
        return (IEnumerable<KeyValuePair<string, object>>) this.m_properties;
      return propertyScope == IdentityPropertyScope.Local ? (IEnumerable<KeyValuePair<string, object>>) this.m_localProperties : this.m_localProperties.Concat<KeyValuePair<string, object>>(this.m_properties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => !this.m_localProperties.ContainsKey(kvp.Key))));
    }

    public void SetProperty(string name, object value) => this.SetProperty(IdentityPropertyScope.Global, name, value);

    public void SetProperty(IdentityPropertyScope propertyScope, string name, object value)
    {
      PropertyValidation.ValidatePropertyName(name);
      PropertyValidation.ValidatePropertyValue(name, value);
      if (IdentityAttributeTags.ReadOnlyProperties.Contains(name))
        throw new NotSupportedException(TFCommonResources.IdentityPropertyReadOnly((object) name));
      switch (propertyScope)
      {
        case IdentityPropertyScope.Global:
          this.SetGlobalProperty(name, value);
          this.RemoveProperty(IdentityPropertyScope.Local, name);
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

    public string GetAttribute(string name, string defaultValue)
    {
      object obj;
      return this.m_properties.TryGetValue(name, out obj) ? obj.ToString() : defaultValue;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void SetAttribute(string name, string value)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      this.m_properties[name] = (object) value;
      this.m_clientSideUniqueName = (string) null;
    }

    public bool IsContainer
    {
      get
      {
        bool isContainer = false;
        object obj;
        if (this.m_properties.TryGetValue("SchemaClassName", out obj) && obj != null && obj.ToString().Equals("Group", StringComparison.OrdinalIgnoreCase))
          isContainer = true;
        return isContainer;
      }
    }

    internal HashSet<string> GetModifiedPropertiesLog(IdentityPropertyScope propertyScope)
    {
      if (propertyScope == IdentityPropertyScope.Global)
        return this.m_modifiedPropertiesLog;
      if (propertyScope == IdentityPropertyScope.Local)
        return this.m_modifiedLocalPropertiesLog;
      throw new InvalidOperationException(TFCommonResources.InvalidPropertyScope());
    }

    internal void ResetModifiedProperties()
    {
      this.m_modifiedPropertiesLog = (HashSet<string>) null;
      this.m_modifiedLocalPropertiesLog = (HashSet<string>) null;
    }

    internal void InitializeFromWebService()
    {
      this.m_properties = new Dictionary<string, object>();
      this.m_localProperties = new Dictionary<string, object>();
      if (this.m_propertiesSet != null && this.m_propertiesSet.Length != 0)
      {
        foreach (PropertyValue properties in this.m_propertiesSet)
          this.m_properties[properties.PropertyName] = properties.Value;
      }
      if (this.m_localPropertiesSet != null && this.m_localPropertiesSet.Length != 0)
      {
        foreach (PropertyValue localProperties in this.m_localPropertiesSet)
          this.m_localProperties[localProperties.PropertyName] = localProperties.Value;
      }
      if (this.m_attributesSet != null)
      {
        foreach (KeyValueOfStringString attributes in this.m_attributesSet)
          this.m_properties[attributes.Key] = (object) attributes.Value;
      }
      this.m_attributesSet = (KeyValueOfStringString[]) null;
      this.m_propertiesSet = (PropertyValue[]) null;
    }

    public string UniqueName
    {
      get
      {
        if (!string.IsNullOrEmpty(this.m_uniqueName))
          return this.m_uniqueName;
        if (this.m_clientSideUniqueName == null)
        {
          string attribute1 = this.GetAttribute("Domain", string.Empty);
          string attribute2 = this.GetAttribute("Account", string.Empty);
          this.m_clientSideUniqueName = this.UniqueUserId != 0 ? (!string.IsNullOrEmpty(attribute1) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}:{2}", (object) attribute1, (object) attribute2, (object) this.UniqueUserId) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) attribute2, (object) this.UniqueUserId)) : (!string.IsNullOrEmpty(attribute1) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) attribute1, (object) attribute2) : attribute2);
        }
        return this.m_clientSideUniqueName;
      }
    }

    private TeamFoundationIdentity()
    {
    }

    public IdentityDescriptor Descriptor
    {
      get => this.m_descriptor;
      set => this.m_descriptor = value;
    }

    public string DisplayName
    {
      get => this.m_displayName;
      set => this.m_displayName = value;
    }

    public bool IsActive
    {
      get => this.m_isActive;
      set => this.m_isActive = value;
    }

    public IdentityDescriptor[] MemberOf
    {
      get => (IdentityDescriptor[]) this.m_memberOf.Clone();
      set => this.m_memberOf = value;
    }

    public IdentityDescriptor[] Members
    {
      get => (IdentityDescriptor[]) this.m_members.Clone();
      set => this.m_members = value;
    }

    public Guid TeamFoundationId
    {
      get => this.m_teamFoundationId;
      set => this.m_teamFoundationId = value;
    }

    public int UniqueUserId
    {
      get => this.m_uniqueUserId;
      set => this.m_uniqueUserId = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TeamFoundationIdentity FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      TeamFoundationIdentity foundationIdentity = new TeamFoundationIdentity();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "DisplayName":
              foundationIdentity.m_displayName = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "IsActive":
              foundationIdentity.m_isActive = XmlUtility.BooleanFromXmlAttribute(reader);
              continue;
            case "IsContainer":
              foundationIdentity.m_isContainer = XmlUtility.BooleanFromXmlAttribute(reader);
              continue;
            case "TeamFoundationId":
              foundationIdentity.m_teamFoundationId = XmlUtility.GuidFromXmlAttribute(reader);
              continue;
            case "UniqueName":
              foundationIdentity.m_uniqueName = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "UniqueUserId":
              foundationIdentity.m_uniqueUserId = XmlUtility.Int32FromXmlAttribute(reader);
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
            case "Attributes":
              foundationIdentity.m_attributesSet = Helper.ArrayOfKeyValueOfStringStringFromXml(serviceProvider, reader, false);
              continue;
            case "Descriptor":
              foundationIdentity.m_descriptor = IdentityDescriptor.FromXml(serviceProvider, reader);
              continue;
            case "LocalProperties":
              foundationIdentity.m_localPropertiesSet = Helper.ArrayOfPropertyValueFromXml(serviceProvider, reader, false);
              continue;
            case "MemberOf":
              foundationIdentity.m_memberOf = Helper.ArrayOfIdentityDescriptorFromXml(serviceProvider, reader, false);
              continue;
            case "Members":
              foundationIdentity.m_members = Helper.ArrayOfIdentityDescriptorFromXml(serviceProvider, reader, false);
              continue;
            case "Properties":
              foundationIdentity.m_propertiesSet = Helper.ArrayOfPropertyValueFromXml(serviceProvider, reader, false);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return foundationIdentity;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("TeamFoundationIdentity instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  AttributesSet: " + Helper.ArrayToString<KeyValueOfStringString>(this.m_attributesSet));
      stringBuilder.AppendLine("  Descriptor: " + this.m_descriptor?.ToString());
      stringBuilder.AppendLine("  DisplayName: " + this.m_displayName);
      stringBuilder.AppendLine("  IsActive: " + this.m_isActive.ToString());
      stringBuilder.AppendLine("  IsContainer: " + this.m_isContainer.ToString());
      stringBuilder.AppendLine("  LocalPropertiesSet: " + Helper.ArrayToString<PropertyValue>(this.m_localPropertiesSet));
      stringBuilder.AppendLine("  MemberOf: " + Helper.ArrayToString<IdentityDescriptor>(this.m_memberOf));
      stringBuilder.AppendLine("  Members: " + Helper.ArrayToString<IdentityDescriptor>(this.m_members));
      stringBuilder.AppendLine("  PropertiesSet: " + Helper.ArrayToString<PropertyValue>(this.m_propertiesSet));
      stringBuilder.AppendLine("  TeamFoundationId: " + this.m_teamFoundationId.ToString());
      stringBuilder.AppendLine("  UniqueName: " + this.m_uniqueName);
      stringBuilder.AppendLine("  UniqueUserId: " + this.m_uniqueUserId.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_displayName != null)
        XmlUtility.ToXmlAttribute(writer, "DisplayName", this.m_displayName);
      if (this.m_isActive)
        XmlUtility.ToXmlAttribute(writer, "IsActive", this.m_isActive);
      if (this.m_isContainer)
        XmlUtility.ToXmlAttribute(writer, "IsContainer", this.m_isContainer);
      if (this.m_teamFoundationId != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "TeamFoundationId", this.m_teamFoundationId);
      if (this.m_uniqueName != null)
        XmlUtility.ToXmlAttribute(writer, "UniqueName", this.m_uniqueName);
      if (this.m_uniqueUserId != 0)
        XmlUtility.ToXmlAttribute(writer, "UniqueUserId", this.m_uniqueUserId);
      Helper.ToXml(writer, "Attributes", this.m_attributesSet, false, false);
      if (this.m_descriptor != null)
        IdentityDescriptor.ToXml(writer, "Descriptor", this.m_descriptor);
      Helper.ToXml(writer, "LocalProperties", this.m_localPropertiesSet, false, false);
      Helper.ToXml(writer, "MemberOf", this.m_memberOf, false, false);
      Helper.ToXml(writer, "Members", this.m_members, false, false);
      Helper.ToXml(writer, "Properties", this.m_propertiesSet, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, TeamFoundationIdentity obj) => obj.ToXml(writer, element);
  }
}
