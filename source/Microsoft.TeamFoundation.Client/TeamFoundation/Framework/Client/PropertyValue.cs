// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.PropertyValue
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class PropertyValue
  {
    private bool m_isValueDirty;
    private object m_internalValue;
    private string m_propertyName;

    public PropertyValue(string propertyName, int value)
      : this(propertyName, (object) value)
    {
    }

    public PropertyValue(string propertyName, double value)
      : this(propertyName, (object) value)
    {
    }

    public PropertyValue(string propertyName, DateTime value)
      : this(propertyName, (object) value.ToUniversalTime())
    {
    }

    public PropertyValue(string propertyName, string value)
      : this(propertyName, (object) value)
    {
    }

    public PropertyValue(string propertyName, byte[] value)
      : this(propertyName, (object) value)
    {
    }

    public PropertyValue(string propertyName, object value)
    {
      this.m_propertyName = propertyName;
      this.m_internalValue = value;
      this.m_isValueDirty = true;
    }

    public object Value
    {
      get => this.m_internalValue is DateTime ? (object) ((DateTime) this.m_internalValue).ToLocalTime() : this.m_internalValue;
      set
      {
        if (value is DateTime dateTime)
          value = (object) dateTime.ToUniversalTime();
        this.m_internalValue = value;
        this.m_isValueDirty = true;
      }
    }

    public Type PropertyType => this.Value != null ? this.Value.GetType() : (Type) null;

    internal bool IsValueDirty => this.m_isValueDirty;

    internal PropertyValue()
    {
    }

    internal object InternalValue
    {
      get => this.m_internalValue;
      set => this.m_internalValue = value;
    }

    public string PropertyName
    {
      get => this.m_propertyName;
      internal set => this.m_propertyName = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PropertyValue FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      PropertyValue propertyValue = new PropertyValue();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          if (reader.Name == "pname")
            propertyValue.m_propertyName = XmlUtility.StringFromXmlAttribute(reader);
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "val")
            propertyValue.m_internalValue = XmlUtility.ObjectFromXmlElement(reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return propertyValue;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("PropertyValue instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  InternalValue: " + this.m_internalValue?.ToString());
      stringBuilder.AppendLine("  PropertyName: " + this.m_propertyName);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_propertyName != null)
        XmlUtility.ToXmlAttribute(writer, "pname", this.m_propertyName);
      if (this.m_internalValue != null)
        XmlUtility.ObjectToXmlElement(writer, "val", this.m_internalValue);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, PropertyValue obj) => obj.ToXml(writer, element);
  }
}
