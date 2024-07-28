// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ArtifactPropertyValue
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ArtifactPropertyValue
  {
    private List<PropertyValue> m_values;
    internal PropertyValue[] m_internalPropertyValues = Helper.ZeroLengthArrayOfPropertyValue;
    private ArtifactSpec m_spec;

    public ArtifactPropertyValue(ArtifactSpec artifactSpec)
      : this(artifactSpec, new List<PropertyValue>())
    {
    }

    public ArtifactPropertyValue(ArtifactSpec artifactSpec, List<PropertyValue> values)
    {
      this.m_spec = artifactSpec;
      this.m_values = values;
    }

    public ArtifactPropertyValue(ArtifactSpec artifactSpec, string propertyName, int? value)
      : this(artifactSpec, new PropertyValue(propertyName, (object) value))
    {
    }

    public ArtifactPropertyValue(ArtifactSpec artifactSpec, string propertyName, double? value)
      : this(artifactSpec, new PropertyValue(propertyName, (object) value))
    {
    }

    public ArtifactPropertyValue(ArtifactSpec artifactSpec, string propertyName, string value)
      : this(artifactSpec, new PropertyValue(propertyName, value))
    {
    }

    public ArtifactPropertyValue(ArtifactSpec artifactSpec, string propertyName, DateTime? value)
      : this(artifactSpec, new PropertyValue(propertyName, (object) value))
    {
    }

    public ArtifactPropertyValue(ArtifactSpec artifactSpec, string propertyName, byte[] value)
      : this(artifactSpec, new PropertyValue(propertyName, value))
    {
    }

    internal ArtifactPropertyValue(ArtifactSpec artifactSpec, PropertyValue propertyValue)
      : this(artifactSpec, new List<PropertyValue>())
    {
      this.m_values.Add(propertyValue);
    }

    public List<PropertyValue> PropertyValues
    {
      get
      {
        if (this.m_values == null)
          this.m_values = new List<PropertyValue>((IEnumerable<PropertyValue>) this.m_internalPropertyValues);
        return this.m_values;
      }
    }

    internal void PrepareInternalValues()
    {
      List<PropertyValue> propertyValueList = new List<PropertyValue>();
      for (int index = 0; index < this.PropertyValues.Count; ++index)
      {
        if (this.PropertyValues[index].IsValueDirty)
          propertyValueList.Add(this.PropertyValues[index]);
      }
      this.m_internalPropertyValues = propertyValueList.ToArray();
    }

    internal ArtifactPropertyValue()
    {
    }

    internal PropertyValue[] InternalPropertyValues
    {
      get => (PropertyValue[]) this.m_internalPropertyValues.Clone();
      set => this.m_internalPropertyValues = value;
    }

    public ArtifactSpec Spec
    {
      get => this.m_spec;
      internal set => this.m_spec = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ArtifactPropertyValue FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ArtifactPropertyValue artifactPropertyValue = new ArtifactPropertyValue();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "PropertyValues":
              artifactPropertyValue.m_internalPropertyValues = Helper.ArrayOfPropertyValueFromXml(serviceProvider, reader, false);
              continue;
            case "Spec":
              artifactPropertyValue.m_spec = ArtifactSpec.FromXml(serviceProvider, reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return artifactPropertyValue;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ArtifactPropertyValue instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  InternalPropertyValues: " + Helper.ArrayToString<PropertyValue>(this.m_internalPropertyValues));
      stringBuilder.AppendLine("  Spec: " + this.m_spec?.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      Helper.ToXml(writer, "PropertyValues", this.m_internalPropertyValues, false, false);
      if (this.m_spec != null)
        ArtifactSpec.ToXml(writer, "Spec", this.m_spec);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ArtifactPropertyValue obj) => obj.ToXml(writer, element);
  }
}
