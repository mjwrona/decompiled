// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyValue
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class PropertyValue : ICacheable
  {
    private string m_propertyName;
    private object m_value;
    private Guid? m_changedBy;
    private DateTime? m_changedDate;

    internal PropertyValue()
    {
    }

    public PropertyValue(string propertyName, object value)
      : this(propertyName, value, PropertyTypeMatch.Unspecified, true)
    {
    }

    public PropertyValue(
      string propertyName,
      object value,
      DateTime? changedDate,
      Guid? changedBy)
      : this(propertyName, value, PropertyTypeMatch.Unspecified, changedDate, changedBy, true)
    {
    }

    internal PropertyValue(
      string propertyName,
      object value,
      PropertyTypeMatch typeMatch,
      bool performValidation)
      : this(propertyName, value, typeMatch, new DateTime?(), new Guid?(), true)
    {
    }

    internal PropertyValue(
      string propertyName,
      object value,
      PropertyTypeMatch typeMatch,
      DateTime? changedDate,
      Guid? changedBy,
      bool performValidation)
    {
      this.m_propertyName = propertyName;
      this.m_value = value;
      this.TypeMatch = typeMatch;
      this.m_changedBy = changedBy;
      this.m_changedDate = changedDate;
      if (!performValidation)
        return;
      this.Validate();
    }

    [XmlAttribute("pname")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string PropertyName
    {
      get => this.m_propertyName;
      set => this.m_propertyName = value;
    }

    [XmlElement(ElementName = "val", IsNullable = true, Type = typeof (object))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "InternalValue", Direction = ClientPropertySerialization.Bidirectional)]
    public object Value
    {
      get => this.m_value;
      set
      {
        if (value is DateTime dateTime)
          value = (object) dateTime.ToUniversalTime();
        this.m_value = value;
      }
    }

    public Guid? ChangedBy
    {
      get => this.m_changedBy;
      set => this.m_changedBy = value;
    }

    public DateTime? ChangedDate
    {
      get => this.m_changedDate;
      set => this.m_changedDate = value;
    }

    public int GetCachedSize() => (this.m_propertyName.Length << 1) + this.GetPropertyValueSize();

    [XmlIgnore]
    public PropertyTypeMatch TypeMatch { get; private set; }

    private int GetPropertyValueSize()
    {
      if (this.m_value == null)
        return 0;
      Type type = this.m_value.GetType();
      if (type == typeof (DateTime))
        return 68;
      if (type == typeof (byte[]))
        return ((byte[]) this.m_value).Length;
      return type == typeof (string) ? ((string) this.m_value).Length << 1 : 16;
    }

    internal void Validate()
    {
      PropertyValidation.ValidatePropertyName(this.m_propertyName);
      PropertyValidation.ValidatePropertyValue(this.m_propertyName, this.m_value);
    }
  }
}
