// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Property
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class Property
  {
    public string Name;
    public string Value;

    public Property(string name, string propValue)
    {
      this.Name = name;
      this.Value = propValue;
    }

    public Property()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Property FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      Property property = new Property();
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
            case "Name":
              property.Name = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "Value":
              property.Value = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return property;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Property instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Name: " + this.Name);
      stringBuilder.AppendLine("  Value: " + this.Value);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.Name != null)
        XmlUtility.ToXmlElement(writer, "Name", this.Name);
      if (this.Value != null)
        XmlUtility.ToXmlElement(writer, "Value", this.Value);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, Property obj) => obj.ToXml(writer, element);
  }
}
